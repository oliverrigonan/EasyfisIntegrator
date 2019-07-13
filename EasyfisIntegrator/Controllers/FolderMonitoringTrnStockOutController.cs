using System;
using System.Collections.Generic;
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
    class FolderMonitoringTrnStockOutController
    {
        // ==============
        // Send Stock Out
        // ==============
        public async void SendStockOut(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnStockOut> newStockOuts = new List<Entities.FolderMonitoringTrnStockOut>();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Delete
            try
            {
                trnIntegrationForm.logFolderMonitoringMessage("Cleaning Stock Out... (0%) \r\n\n");

                Boolean isErrorLogged = false;
                String previousErrorMessage = String.Empty;

                while (true)
                {
                    String deleteTemporaryStockOutTask = await DeleteTemporaryStockOut(domain);
                    if (!deleteTemporaryStockOutTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryStockOutTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryStockOutTask))
                            {
                                previousErrorMessage = deleteTemporaryStockOutTask;
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
                        trnIntegrationForm.logFolderMonitoringMessage("OTIntegrateSuccessful");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Stock Out... (100%) \r\n\n");

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
                            newStockOuts.Add(new Entities.FolderMonitoringTrnStockOut
                            {
                                BranchCode = data[0],
                                OTDate = data[1],
                                AccountCode = data[2],
                                ArticleCode = data[3],
                                ManualOTNumber = data[4],
                                Remarks = data[5],
                                UserCode = userCode,
                                CreatedDateTime = data[6],
                                ItemCode = data[7],
                                Particulars = data[8],
                                Unit = data[9],
                                Quantity = Convert.ToDecimal(data[10]),
                                Cost = Convert.ToDecimal(data[11]),
                                Amount = Convert.ToDecimal(data[12])
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
            if (newStockOuts.Any())
            {
                try
                {
                    Decimal percentage = 0;
                    trnIntegrationForm.logFolderMonitoringMessage("Sending Stock Out... (0%) \r\n\n");

                    Boolean send = false;
                    Int32 skip = 0;

                    for (Int32 i = 1; i <= newStockOuts.Count(); i++)
                    {
                        if (i % 100 == 0)
                        {
                            jsonData = serializer.Serialize(newStockOuts.Skip(skip).Take(100));
                            send = true;

                            skip = i;

                            percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newStockOuts.Count())) * 100);
                        }
                        else
                        {
                            if (newStockOuts.Count() <= 100)
                            {
                                jsonData = serializer.Serialize(newStockOuts);
                                send = true;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockOuts.Count())) * 100);
                            }
                            else
                            {
                                if (i == newStockOuts.Count())
                                {
                                    jsonData = serializer.Serialize(newStockOuts.Skip(skip).Take(i - skip));
                                    send = true;

                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockOuts.Count())) * 100);
                                }
                            }
                        }

                        if (send)
                        {
                            Boolean isErrorLogged = false;
                            String previousErrorMessage = String.Empty;

                            while (true)
                            {
                                String insertTemporaryStockOutTask = await InsertTemporaryStockOut(domain, jsonData);
                                if (!insertTemporaryStockOutTask.Equals("Send Successful..."))
                                {
                                    if (previousErrorMessage.Equals(String.Empty))
                                    {
                                        previousErrorMessage = insertTemporaryStockOutTask;
                                    }
                                    else
                                    {
                                        if (!previousErrorMessage.Equals(insertTemporaryStockOutTask))
                                        {
                                            previousErrorMessage = insertTemporaryStockOutTask;
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
                                    trnIntegrationForm.logFolderMonitoringMessage("OTIntegrateSuccessful");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Stock Out... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                    if (i == newStockOuts.Count())
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
                var branchCodes = from d in newStockOuts
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
                        trnIntegrationForm.logFolderMonitoringMessage("Posting Stock Out Branch: " + branchCode + " ... (0%) \r\n\n");

                        var manualOTNumbers = from d in newStockOuts
                                              where d.BranchCode.Equals(branchCode)
                                              group d by d.ManualOTNumber into g
                                              select g.Key;

                        var listManualOTNumbers = manualOTNumbers.ToList();
                        if (listManualOTNumbers.Any())
                        {
                            Int32 manualOTNumberCount = 0;

                            foreach (var manualOTNumber in listManualOTNumbers)
                            {
                                manualOTNumberCount += 1;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(manualOTNumberCount) / Convert.ToDecimal(listManualOTNumbers.Count())) * 100);

                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String postStockOutTask = await PostStockOut(domain, branchCode, manualOTNumber);
                                    if (!postStockOutTask.Equals("Post Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = postStockOutTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(postStockOutTask))
                                            {
                                                previousErrorMessage = postStockOutTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("OTIntegrateSuccessful");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Stock Out Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (manualOTNumberCount == listManualOTNumbers.Count())
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
                    trnIntegrationForm.logFolderMonitoringMessage("Moving Stock Out File... (0%) \r\n\n");

                    String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");
                    using (StreamReader trmRead = new StreamReader(settingsPath))
                    {
                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(trmRead.ReadToEnd());

                        String executingUser = WindowsIdentity.GetCurrent().Name;

                        DirectorySecurity securityRules = new DirectorySecurity();
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.Read, AccessControlType.Allow));
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.FullControl, AccessControlType.Allow));

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\OT_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryOTCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\OT_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\OT_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "OT_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.logFolderMonitoringMessage("OTIntegrateSuccessful");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Stock Out File... (100%) \r\n\n");

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

        // ==========================
        // Delete Temporary Stock Out
        // ==========================
        public Task<String> DeleteTemporaryStockOut(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockOut/temporary/delete";

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

        // ==========================
        // Insert Temporary Stock Out
        // ==========================
        public Task<String> InsertTemporaryStockOut(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockOut/temporary/insert";

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

        // ==============
        // Post Stock Out
        // ==============
        public Task<String> PostStockOut(String domain, String branchCode, String manualOTNumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockOut/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnStockOut jsonStockOut = new Entities.FolderMonitoringTrnStockOut()
                {
                    BranchCode = branchCode,
                    ManualOTNumber = manualOTNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonStockOut);

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
