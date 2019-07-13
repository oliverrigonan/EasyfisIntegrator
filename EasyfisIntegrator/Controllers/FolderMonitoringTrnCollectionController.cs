using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class FolderMonitoringTrnCollectionController
    {
        // ===============
        // Send Collection
        // ===============
        public async void SendCollection(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnCollection> newCollections = new List<Entities.FolderMonitoringTrnCollection>();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Delete
            try
            {
                trnIntegrationForm.logFolderMonitoringMessage("Cleaning Collection... (0%) \r\n\n");

                Boolean isErrorLogged = false;
                String previousErrorMessage = String.Empty;

                while (true)
                {
                    String deleteTemporaryCollectionTask = await DeleteTemporaryCollection(domain);
                    if (!deleteTemporaryCollectionTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryCollectionTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryCollectionTask))
                            {
                                previousErrorMessage = deleteTemporaryCollectionTask;
                                isErrorLogged = false;
                            }
                        }

                        if (!isErrorLogged)
                        {
                            trnIntegrationForm.logFolderMonitoringMessage(previousErrorMessage);
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                            isErrorLogged = true;
                        }

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("ORIntegrateSuccessful");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Collection... (100%) \r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Clean Successful!" + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logFolderMonitoringMessage("Cleaning Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }

            // CSV Data
            try
            {
                if (SysFileControl.IsCurrentFileClosed(file))
                {
                    using (StreamReader dataStreamReader = new StreamReader(file))
                    {
                        dataStreamReader.ReadLine();
                        while (dataStreamReader.Peek() >= 0)
                        {
                            List<String> data = dataStreamReader.ReadLine().Split(',').ToList();
                            newCollections.Add(new Entities.FolderMonitoringTrnCollection
                            {
                                BranchCode = data[0],
                                ORDate = data[1],
                                CustomerCode = data[2],
                                ManualORNumber = data[3],
                                Remarks = data[4],
                                UserCode = userCode,
                                CreatedDateTime = data[5],
                                LineBranchCode = data[6],
                                AccountCode = data[7],
                                ArticleCode = data[8],
                                SINumber = data[9],
                                ManualSINumber = data[10],
                                Particulars = data[11],
                                Amount = Convert.ToDecimal(data[12]),
                                PayType = data[13],
                                CheckNumber = data[14],
                                CheckDate = data[15],
                                CheckBank = data[16],
                                DepositoryBankCode = data[17],
                                IsClear = Convert.ToBoolean(data[18])
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logFolderMonitoringMessage("CSV Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }

            // Send
            if (newCollections.Any())
            {
                try
                {
                    Decimal percentage = 0;
                    trnIntegrationForm.logFolderMonitoringMessage("Sending Collection... (0%) \r\n\n");

                    Boolean send = false;
                    Int32 skip = 0;

                    for (Int32 i = 1; i <= newCollections.Count(); i++)
                    {
                        if (i % 100 == 0)
                        {
                            jsonData = serializer.Serialize(newCollections.Skip(skip).Take(100));
                            send = true;

                            skip = i;

                            percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newCollections.Count())) * 100);
                        }
                        else
                        {
                            if (newCollections.Count() <= 100)
                            {
                                jsonData = serializer.Serialize(newCollections);
                                send = true;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newCollections.Count())) * 100);
                            }
                            else
                            {
                                if (i == newCollections.Count())
                                {
                                    jsonData = serializer.Serialize(newCollections.Skip(skip).Take(i - skip));
                                    send = true;

                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newCollections.Count())) * 100);
                                }
                            }
                        }

                        if (send)
                        {
                            Boolean isErrorLogged = false;
                            String previousErrorMessage = String.Empty;

                            while (true)
                            {
                                String insertTemporaryCollectionTask = await InsertTemporaryCollection(domain, jsonData);
                                if (!insertTemporaryCollectionTask.Equals("Send Successful..."))
                                {
                                    if (previousErrorMessage.Equals(String.Empty))
                                    {
                                        previousErrorMessage = insertTemporaryCollectionTask;
                                    }
                                    else
                                    {
                                        if (!previousErrorMessage.Equals(insertTemporaryCollectionTask))
                                        {
                                            previousErrorMessage = insertTemporaryCollectionTask;
                                            isErrorLogged = false;
                                        }
                                    }

                                    if (!isErrorLogged)
                                    {
                                        trnIntegrationForm.logFolderMonitoringMessage(previousErrorMessage);
                                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                        isErrorLogged = true;
                                    }

                                    Thread.Sleep(5000);
                                }
                                else
                                {
                                    trnIntegrationForm.logFolderMonitoringMessage("ORIntegrateSuccessful");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Collection... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                    if (i == newCollections.Count())
                                    {
                                        trnIntegrationForm.logFolderMonitoringMessage("Send Successful!" + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                    }

                                    break;
                                }
                            }

                            send = false;
                        }
                    }

                    post = true;
                }
                catch (Exception e)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Sending Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }

            // Post
            if (post)
            {
                var branchCodes = from d in newCollections
                                  group d by d.BranchCode into g
                                  select g.Key;

                var listBranchCodes = branchCodes.ToList();
                if (listBranchCodes.Any())
                {
                    Int32 branchCount = 0;

                    foreach (var branchCode in listBranchCodes)
                    {
                        branchCount += 1;

                        Decimal percentage = 0;
                        trnIntegrationForm.logFolderMonitoringMessage("Posting Collection Branch: " + branchCode + " ... (0%) \r\n\n");

                        var manualORNumbers = from d in newCollections
                                              where d.BranchCode.Equals(branchCode)
                                              group d by d.ManualORNumber into g
                                              select g.Key;

                        var listManualORNumbers = manualORNumbers.ToList();
                        if (listManualORNumbers.Any())
                        {
                            Int32 manualORNumberCount = 0;

                            foreach (var manualORNumber in listManualORNumbers)
                            {
                                manualORNumberCount += 1;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(manualORNumberCount) / Convert.ToDecimal(listManualORNumbers.Count())) * 100);

                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String postCollectionTask = await PostCollection(domain, branchCode, manualORNumber);
                                    if (!postCollectionTask.Equals("Post Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = postCollectionTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(postCollectionTask))
                                            {
                                                previousErrorMessage = postCollectionTask;
                                                isErrorLogged = false;
                                            }
                                        }

                                        if (!isErrorLogged)
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage(previousErrorMessage);
                                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                            isErrorLogged = true;
                                        }

                                        Thread.Sleep(5000);
                                    }
                                    else
                                    {
                                        trnIntegrationForm.logFolderMonitoringMessage("ORIntegrateSuccessful");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Collection Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (manualORNumberCount == listManualORNumbers.Count())
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage("Branch: " + branchCode + " Post Successful!" + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Move CSV File
                try
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Moving Collection File... (0%) \r\n\n");

                    String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");
                    using (StreamReader trmRead = new StreamReader(settingsPath))
                    {
                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(trmRead.ReadToEnd());

                        String executingUser = WindowsIdentity.GetCurrent().Name;

                        DirectorySecurity securityRules = new DirectorySecurity();
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.Read, AccessControlType.Allow));
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.FullControl, AccessControlType.Allow));

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\OR_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryORCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\OR_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\OR_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "OR_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.logFolderMonitoringMessage("ORIntegrateSuccessful");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Collection File... (100%) \r\n\n");

                    trnIntegrationForm.logFolderMonitoringMessage("Move Successful!" + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
                catch (Exception e)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Moving File Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
        }

        // ===========================
        // Delete Temporary Collection
        // ===========================
        public Task<String> DeleteTemporaryCollection(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/temporary/delete";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "DELETE";
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { streamWriter.Write(""); }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        return Task.FromResult("Clean Successful...");
                    }
                    else
                    {
                        return Task.FromResult("Clean Failed! " + resp + "\r\n\n");
                    }
                }
            }
            catch (Exception e)
            {
                return Task.FromResult("Exception Error: " + e.Message + "\r\n\n");
            }
        }

        // ===========================
        // Insert Temporary Collection
        // ===========================
        public Task<String> InsertTemporaryCollection(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/temporary/insert";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { streamWriter.Write(json); }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        return Task.FromResult("Send Successful...");
                    }
                    else
                    {
                        return Task.FromResult("Send Failed! " + resp + "\r\n\n");
                    }
                }
            }
            catch (Exception e)
            {
                return Task.FromResult("Exception Error: " + e.Message + "\r\n\n");
            }
        }

        // ===============
        // Post Collection
        // ===============
        public Task<String> PostCollection(String domain, String branchCode, String manualORNumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnCollection jsonCollection = new Entities.FolderMonitoringTrnCollection()
                {
                    BranchCode = branchCode,
                    ManualORNumber = manualORNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonCollection);

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { streamWriter.Write(json); }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        return Task.FromResult("Post Successful...");
                    }
                    else
                    {
                        return Task.FromResult("Post Failed! " + resp + "\r\n\n");
                    }
                }
            }
            catch (Exception e)
            {
                return Task.FromResult("Exception Error: " + e.Message + "\r\n\n");
            }
        }
    }
}