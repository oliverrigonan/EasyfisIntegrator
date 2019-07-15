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
    class FolderMonitoringTrnStockTransferController
    {
        // ===================
        // Send Stock Transfer
        // ===================
        public async void SendStockTransfer(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnStockTransfer> newStockTransfers = new List<Entities.FolderMonitoringTrnStockTransfer>();
            Boolean isExceptionErrorOccured = false;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Cleaning
            while (true)
            {
                try
                {
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Stock Transfer... (0%) \r\n\n");

                    Boolean isErrorLogged = false;
                    String previousErrorMessage = String.Empty;

                    String deleteTemporaryStockTransferTask = await DeleteTemporaryStockTransfer(domain);
                    if (!deleteTemporaryStockTransferTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryStockTransferTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryStockTransferTask))
                            {
                                previousErrorMessage = deleteTemporaryStockTransferTask;
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
                        trnIntegrationForm.logFolderMonitoringMessage("STIntegrationLogOnce");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Stock Transfer... (100%) \r\n\n");

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
                                newStockTransfers.Add(new Entities.FolderMonitoringTrnStockTransfer
                                {
                                    BranchCode = data[0],
                                    STDate = data[1],
                                    ToBranchCode = data[2],
                                    ArticleCode = data[3],
                                    ManualSTNumber = data[4],
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
            if (newStockTransfers.Any())
            {
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;
                        trnIntegrationForm.logFolderMonitoringMessage("Sending Stock Transfer... (0%) \r\n\n");

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newStockTransfers.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newStockTransfers.Skip(skip).Take(100));
                                send = true;

                                skip = i;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newStockTransfers.Count())) * 100);
                            }
                            else
                            {
                                if (newStockTransfers.Count() <= 100)
                                {
                                    jsonData = serializer.Serialize(newStockTransfers);
                                    send = true;

                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockTransfers.Count())) * 100);
                                }
                                else
                                {
                                    if (i == newStockTransfers.Count())
                                    {
                                        jsonData = serializer.Serialize(newStockTransfers.Skip(skip).Take(i - skip));
                                        send = true;

                                        percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockTransfers.Count())) * 100);
                                    }
                                }
                            }

                            if (send)
                            {
                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String insertTemporaryStockTransferTask = await InsertTemporaryStockTransfer(domain, jsonData);
                                    if (!insertTemporaryStockTransferTask.Equals("Send Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = insertTemporaryStockTransferTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(insertTemporaryStockTransferTask))
                                            {
                                                previousErrorMessage = insertTemporaryStockTransferTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("STIntegrationLogOnce");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Stock Transfer... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (i == newStockTransfers.Count())
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
                        var branchCodes = from d in newStockTransfers
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
                                trnIntegrationForm.logFolderMonitoringMessage("Posting Stock Transfer Branch: " + branchCode + " ... (0%) \r\n\n");

                                var manualSTNumbers = from d in newStockTransfers
                                                      where d.BranchCode.Equals(branchCode)
                                                      group d by d.ManualSTNumber into g
                                                      select g.Key;

                                var listManualSTNumbers = manualSTNumbers.ToList();
                                if (listManualSTNumbers.Any())
                                {
                                    Int32 manualSTNumberCount = 0;

                                    foreach (var manualSTNumber in listManualSTNumbers)
                                    {
                                        manualSTNumberCount += 1;
                                        percentage = Convert.ToDecimal((Convert.ToDecimal(manualSTNumberCount) / Convert.ToDecimal(listManualSTNumbers.Count())) * 100);

                                        Boolean isErrorLogged = false;
                                        String previousErrorMessage = String.Empty;

                                        while (true)
                                        {
                                            String postStockTransferTask = await PostStockTransfer(domain, branchCode, manualSTNumber);
                                            if (!postStockTransferTask.Equals("Post Successful..."))
                                            {
                                                if (previousErrorMessage.Equals(String.Empty))
                                                {
                                                    previousErrorMessage = postStockTransferTask;
                                                }
                                                else
                                                {
                                                    if (!previousErrorMessage.Equals(postStockTransferTask))
                                                    {
                                                        previousErrorMessage = postStockTransferTask;
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
                                                trnIntegrationForm.logFolderMonitoringMessage("STIntegrationLogOnce");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Stock Transfer Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (manualSTNumberCount == listManualSTNumbers.Count())
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
                    trnIntegrationForm.logFolderMonitoringMessage("Moving Stock Transfer File... (0%) \r\n\n");

                    String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");
                    using (StreamReader trmRead = new StreamReader(settingsPath))
                    {
                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(trmRead.ReadToEnd());

                        String executingUser = WindowsIdentity.GetCurrent().Name;

                        DirectorySecurity securityRules = new DirectorySecurity();
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.Read, AccessControlType.Allow));
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.FullControl, AccessControlType.Allow));

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\ST_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectorySTCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\ST_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\ST_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "ST_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.logFolderMonitoringMessage("STIntegrationLogOnce");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Stock Transfer File... (100%) \r\n\n");

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

        // ==========================
        // Delete Temporary Stock Out
        // ==========================
        public Task<String> DeleteTemporaryStockTransfer(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockTransfer/temporary/delete";

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
        public Task<String> InsertTemporaryStockTransfer(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockTransfer/temporary/insert";

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
        public Task<String> PostStockTransfer(String domain, String branchCode, String manualSTNumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockTransfer/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnStockTransfer jsonStockTransfer = new Entities.FolderMonitoringTrnStockTransfer()
                {
                    BranchCode = branchCode,
                    ManualSTNumber = manualSTNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonStockTransfer);

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
