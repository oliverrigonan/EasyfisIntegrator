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
    class FolderMonitoringTrnStockInController
    {
        // =============
        // Send Stock In
        // =============
        public async void SendStockIn(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnStockIn> newStockIns = new List<Entities.FolderMonitoringTrnStockIn>();
            Boolean isExceptionErrorOccured = false;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Cleaning
            while (true)
            {
                try
                {
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Stock In... (0%) \r\n\n");

                    Boolean isErrorLogged = false;
                    String previousErrorMessage = String.Empty;

                    String deleteTemporaryStockInTask = await DeleteTemporaryStockIn(domain);
                    if (!deleteTemporaryStockInTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryStockInTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryStockInTask))
                            {
                                previousErrorMessage = deleteTemporaryStockInTask;
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
                        trnIntegrationForm.logFolderMonitoringMessage("INIntegrationLogOnce");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Stock In... (100%) \r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Clean Successful!" + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        break;
                    }
                }
                catch (Exception e)
                {
                    if (!isExceptionErrorOccured)
                    {
                        isExceptionErrorOccured = true;

                        trnIntegrationForm.logFolderMonitoringMessage("Cleaning Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");
                    }

                    Thread.Sleep(5000);
                }
            }

            // Reading CSV Data
            isExceptionErrorOccured = false;
            while (true)
            {
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
                                newStockIns.Add(new Entities.FolderMonitoringTrnStockIn
                                {
                                    BranchCode = data[0],
                                    INDate = data[1],
                                    AccountCode = data[2],
                                    ArticleCode = data[3],
                                    ManualINNumber = data[4],
                                    IsProduce = Convert.ToBoolean(data[5]),
                                    Remarks = data[6],
                                    UserCode = userCode,
                                    CreatedDateTime = data[7],
                                    ItemCode = data[8],
                                    Particulars = data[9],
                                    Unit = data[10],
                                    Quantity = Convert.ToDecimal(data[11]),
                                    Cost = Convert.ToDecimal(data[12]),
                                    Amount = Convert.ToDecimal(data[13])
                                });
                            }
                        }

                        break;
                    }
                }
                catch (Exception e)
                {
                    if (!isExceptionErrorOccured)
                    {
                        isExceptionErrorOccured = true;

                        trnIntegrationForm.logFolderMonitoringMessage("CSV Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");
                    }

                    Thread.Sleep(5000);
                }
            }

            // Sending
            isExceptionErrorOccured = false;
            if (newStockIns.Any())
            {
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;
                        trnIntegrationForm.logFolderMonitoringMessage("Sending Stock In... (0%) \r\n\n");

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newStockIns.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newStockIns.Skip(skip).Take(100));
                                send = true;

                                skip = i;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newStockIns.Count())) * 100);
                            }
                            else
                            {
                                if (newStockIns.Count() <= 100)
                                {
                                    jsonData = serializer.Serialize(newStockIns);
                                    send = true;

                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockIns.Count())) * 100);
                                }
                                else
                                {
                                    if (i == newStockIns.Count())
                                    {
                                        jsonData = serializer.Serialize(newStockIns.Skip(skip).Take(i - skip));
                                        send = true;

                                        percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockIns.Count())) * 100);
                                    }
                                }
                            }

                            if (send)
                            {
                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String insertTemporaryStockInTask = await InsertTemporaryStockIn(domain, jsonData);
                                    if (!insertTemporaryStockInTask.Equals("Send Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = insertTemporaryStockInTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(insertTemporaryStockInTask))
                                            {
                                                previousErrorMessage = insertTemporaryStockInTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("INIntegrationLogOnce");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Stock In... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (i == newStockIns.Count())
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
                        break;
                    }
                    catch (Exception e)
                    {
                        if (!isExceptionErrorOccured)
                        {
                            isExceptionErrorOccured = true;

                            trnIntegrationForm.logFolderMonitoringMessage("Sending Error: " + e.Message + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");
                        }

                        Thread.Sleep(5000);
                    }
                }
            }

            // Posting
            isExceptionErrorOccured = false;
            if (post)
            {
                while (true)
                {
                    try
                    {
                        var branchCodes = from d in newStockIns
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
                                trnIntegrationForm.logFolderMonitoringMessage("Posting Stock In Branch: " + branchCode + " ... (0%) \r\n\n");

                                var manualINNumbers = from d in newStockIns
                                                      where d.BranchCode.Equals(branchCode)
                                                      group d by d.ManualINNumber into g
                                                      select g.Key;

                                var listManualINNumbers = manualINNumbers.ToList();
                                if (listManualINNumbers.Any())
                                {
                                    Int32 manualINNumberCount = 0;

                                    foreach (var manualINNumber in listManualINNumbers)
                                    {
                                        manualINNumberCount += 1;
                                        percentage = Convert.ToDecimal((Convert.ToDecimal(manualINNumberCount) / Convert.ToDecimal(listManualINNumbers.Count())) * 100);

                                        Boolean isErrorLogged = false;
                                        String previousErrorMessage = String.Empty;

                                        while (true)
                                        {
                                            String postStockInTask = await PostStockIn(domain, branchCode, manualINNumber);
                                            if (!postStockInTask.Equals("Post Successful..."))
                                            {
                                                if (previousErrorMessage.Equals(String.Empty))
                                                {
                                                    previousErrorMessage = postStockInTask;
                                                }
                                                else
                                                {
                                                    if (!previousErrorMessage.Equals(postStockInTask))
                                                    {
                                                        previousErrorMessage = postStockInTask;
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
                                                trnIntegrationForm.logFolderMonitoringMessage("INIntegrationLogOnce");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Stock In Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (manualINNumberCount == listManualINNumbers.Count())
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

                        break;
                    }
                    catch (Exception e)
                    {
                        if (!isExceptionErrorOccured)
                        {
                            isExceptionErrorOccured = true;

                            trnIntegrationForm.logFolderMonitoringMessage("Posting Error: " + e.Message + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");
                        }

                        Thread.Sleep(5000);
                    }
                }
            }

            // Move CSV File
            isExceptionErrorOccured = false;
            while (true)
            {
                try
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Moving Stock In File... (0%) \r\n\n");

                    String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");
                    using (StreamReader trmRead = new StreamReader(settingsPath))
                    {
                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(trmRead.ReadToEnd());

                        String executingUser = WindowsIdentity.GetCurrent().Name;

                        DirectorySecurity securityRules = new DirectorySecurity();
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.Read, AccessControlType.Allow));
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.FullControl, AccessControlType.Allow));

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\IN_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryINCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\IN_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\IN_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "IN_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.logFolderMonitoringMessage("INIntegrationLogOnce");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Stock In File... (100%) \r\n\n");

                    trnIntegrationForm.logFolderMonitoringMessage("Move Successful!" + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                    break;
                }
                catch (Exception e)
                {
                    if (!isExceptionErrorOccured)
                    {
                        isExceptionErrorOccured = true;

                        trnIntegrationForm.logFolderMonitoringMessage("Moving File Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");
                    }

                    Thread.Sleep(5000);
                }
            }
        }

        // =========================
        // Delete Temporary Stock In
        // =========================
        public Task<String> DeleteTemporaryStockIn(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockIn/temporary/delete";

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

        // =========================
        // Insert Temporary Stock In
        // =========================
        public Task<String> InsertTemporaryStockIn(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockIn/temporary/insert";

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

        // =============
        // Post Stock In
        // =============
        public Task<String> PostStockIn(String domain, String branchCode, String manualINNumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockIn/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnStockIn jsonStockIn = new Entities.FolderMonitoringTrnStockIn()
                {
                    BranchCode = branchCode,
                    ManualINNumber = manualINNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonStockIn);

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
