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
    class FolderMonitoringTrnDisbursementController
    {
        // =================
        // Send Disbursement
        // =================
        public async void SendDisbursement(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnDisbursement> newDisbursements = new List<Entities.FolderMonitoringTrnDisbursement>();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Delete
            try
            {
                trnIntegrationForm.logFolderMonitoringMessage("Cleaning... (0%) \r\n\n");

                Boolean isErrorLogged = false;
                String previousErrorMessage = String.Empty;

                while (true)
                {
                    String deleteTemporaryDisbursementTask = await DeleteTemporaryDisbursement(domain);
                    if (!deleteTemporaryDisbursementTask.Equals("Clean Disbursement Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryDisbursementTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryDisbursementTask))
                            {
                                previousErrorMessage = deleteTemporaryDisbursementTask;
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
                        trnIntegrationForm.logFolderMonitoringMessage("CVIntegrateSuccessful");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Disbursement... (100%) \r\n\n");

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
                            newDisbursements.Add(new Entities.FolderMonitoringTrnDisbursement
                            {
                                BranchCode = data[0],
                                CVDate = data[1],
                                SupplierCode = data[2],
                                Payee = data[3],
                                PayType = data[4],
                                BankCode = data[5],
                                Remarks = data[6],
                                ManualCVNumber = data[7],
                                CheckNumber = data[8],
                                CheckDate = data[9],
                                IsCrossCheck = Convert.ToBoolean(data[10]),
                                IsClear = Convert.ToBoolean(data[11]),
                                UserCode = userCode,
                                CreatedDateTime = data[12],
                                LineBranchCode = data[13],
                                AccountCode = data[14],
                                ArticleCode = data[15],
                                RRNumber = data[16],
                                ManualRRNumber = data[17],
                                Particulars = data[18],
                                Amount = Convert.ToDecimal(data[19])
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
            if (newDisbursements.Any())
            {
                try
                {
                    Decimal percentage = 0;
                    trnIntegrationForm.logFolderMonitoringMessage("Sending Disbursement... (0%) \r\n\n");

                    Boolean send = false;
                    Int32 skip = 0;

                    for (Int32 i = 1; i <= newDisbursements.Count(); i++)
                    {
                        if (i % 100 == 0)
                        {
                            jsonData = serializer.Serialize(newDisbursements.Skip(skip).Take(100));
                            send = true;

                            skip = i;

                            percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                        }
                        else
                        {
                            if (newDisbursements.Count() <= 100)
                            {
                                jsonData = serializer.Serialize(newDisbursements);
                                send = true;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                            }
                            else
                            {
                                if (i == newDisbursements.Count())
                                {
                                    jsonData = serializer.Serialize(newDisbursements.Skip(skip).Take(i - skip));
                                    send = true;

                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                                }
                            }
                        }

                        if (send)
                        {
                            Boolean isErrorLogged = false;
                            String previousErrorMessage = String.Empty;

                            while (true)
                            {
                                String insertTemporaryDisbursementTask = await InsertTemporaryDisbursement(domain, jsonData);
                                if (!insertTemporaryDisbursementTask.Equals("Send Successful..."))
                                {
                                    if (previousErrorMessage.Equals(String.Empty))
                                    {
                                        previousErrorMessage = insertTemporaryDisbursementTask;
                                    }
                                    else
                                    {
                                        if (!previousErrorMessage.Equals(insertTemporaryDisbursementTask))
                                        {
                                            previousErrorMessage = insertTemporaryDisbursementTask;
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
                                    trnIntegrationForm.logFolderMonitoringMessage("CVIntegrateSuccessful");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Disbursement... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                    if (i == newDisbursements.Count())
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
                var branchCodes = from d in newDisbursements
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
                        trnIntegrationForm.logFolderMonitoringMessage("Posting Disbursement Branch: " + branchCode + " ... (0%) \r\n\n");

                        var manualCVNumbers = from d in newDisbursements
                                              where d.BranchCode.Equals(branchCode)
                                              group d by d.ManualCVNumber into g
                                              select g.Key;

                        var listManualCVNumbers = manualCVNumbers.ToList();
                        if (listManualCVNumbers.Any())
                        {
                            Int32 manualCVNumberCount = 0;

                            foreach (var manualCVNumber in listManualCVNumbers)
                            {
                                manualCVNumberCount += 1;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(manualCVNumberCount) / Convert.ToDecimal(listManualCVNumbers.Count())) * 100);

                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String postDisbursementTask = await PostDisbursement(domain, branchCode, manualCVNumber);
                                    if (!postDisbursementTask.Equals("Post Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = postDisbursementTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(postDisbursementTask))
                                            {
                                                previousErrorMessage = postDisbursementTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("CVIntegrateSuccessful");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Disbursement Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (manualCVNumberCount == listManualCVNumbers.Count())
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
                    trnIntegrationForm.logFolderMonitoringMessage("Moving Disbursement File... (0%) \r\n\n");

                    String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");
                    using (StreamReader trmRead = new StreamReader(settingsPath))
                    {
                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(trmRead.ReadToEnd());

                        String executingUser = WindowsIdentity.GetCurrent().Name;

                        DirectorySecurity securityRules = new DirectorySecurity();
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.Read, AccessControlType.Allow));
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.FullControl, AccessControlType.Allow));

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\CV_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryCVCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\CV_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\CV_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "CV_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.logFolderMonitoringMessage("CVIntegrateSuccessful");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Disbursement File... (100%) \r\n\n");

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

        // =============================
        // Delete Temporary Disbursement
        // =============================
        public Task<String> DeleteTemporaryDisbursement(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/disbursement/temporary/delete";

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

        // =============================
        // Insert Temporary Disbursement
        // =============================
        public Task<String> InsertTemporaryDisbursement(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/disbursement/temporary/insert";

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

        // =================
        // Post Disbursement
        // =================
        public Task<String> PostDisbursement(String domain, String branchCode, String manualCVNumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/disbursement/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnDisbursement jsonDisbursement = new Entities.FolderMonitoringTrnDisbursement()
                {
                    BranchCode = branchCode,
                    ManualCVNumber = manualCVNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonDisbursement);

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