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
            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnStockTransfer> newStockTransfers = new List<Entities.FolderMonitoringTrnStockTransfer>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Stock Transfer... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporaryStockTransferTask = await DeleteTemporaryStockTransfer(domain);
                    if (!deleteTemporaryStockTransferTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.logFolderMonitoringMessage(deleteTemporaryStockTransferTask);
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

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
                    trnIntegrationForm.logFolderMonitoringMessage("Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                    trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                    Thread.Sleep(5000);
                }
            }

            // ================
            // Reading CSV Data
            // ================
            trnIntegrationForm.logFolderMonitoringMessage("Reading CSV Data... (0%) \r\n\n");
            while (true)
            {
                newStockTransfers = new List<Entities.FolderMonitoringTrnStockTransfer>();

                try
                {
                    if (SysFileControl.IsCurrentFileClosed(file))
                    {
                        Int32 count = 0;

                        using (StreamReader dataStreamReader = new StreamReader(file))
                        {
                            dataStreamReader.ReadLine();
                            while (dataStreamReader.Peek() >= 0)
                            {
                                count += 1;

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
                                    Amount = Convert.ToDecimal(data[12]),
                                    No = count
                                });
                            }
                        }

                        trnIntegrationForm.logFolderMonitoringMessage("STIntegrationLogOnce");

                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nReading CSV Data... (100%) \r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Read Successful!" + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        break;
                    }
                    else
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("Error: File: " + file + " is currently open. \r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                    trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                    Thread.Sleep(5000);
                }
            }

            if (newStockTransfers.Any())
            {
                // =========================
                // Checking Muliple ST Dates
                // =========================
                trnIntegrationForm.logFolderMonitoringMessage("Checking Multiple ST Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedSTDates = from d in newStockTransfers group d by d.STDate into g select g.Key;

                        var STDates = from d in groupedSTDates.ToList() select d;
                        if (STDates.Count() > 1)
                        {
                            trnIntegrationForm.logFolderMonitoringMessage("Checking Error: Cannot integrate multiple ST Dates. \r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.logFolderMonitoringMessage("STIntegrationLogOnce");

                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nChecking Multiple ST Dates... (100%) \r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Check Successful!" + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                }

                Boolean post = false;

                // =======
                // Sending
                // =======
                trnIntegrationForm.logFolderMonitoringMessage("Sending Stock Transfer... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newStockTransfers.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newStockTransfers.Skip(skip).Take(100));
                                skip = i;

                                send = true;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newStockTransfers.Count())) * 100);
                            }
                            else
                            {
                                if (i == newStockTransfers.Count())
                                {
                                    if (newStockTransfers.Count() <= 100)
                                    {
                                        jsonData = serializer.Serialize(newStockTransfers);
                                    }
                                    else
                                    {
                                        jsonData = serializer.Serialize(newStockTransfers.Skip(skip).Take(i - skip));
                                    }

                                    send = true;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockTransfers.Count())) * 100);
                                }
                            }

                            if (send)
                            {
                                while (true)
                                {
                                    try
                                    {
                                        String insertTemporaryStockTransferTask = await InsertTemporaryStockTransfer(domain, jsonData);
                                        if (!insertTemporaryStockTransferTask.Equals("Send Successful..."))
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage(insertTemporaryStockTransferTask);
                                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

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
                                    catch (Exception e)
                                    {
                                        trnIntegrationForm.logFolderMonitoringMessage("Sending Error: " + e.Message + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                        Thread.Sleep(5000);
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
                        trnIntegrationForm.logFolderMonitoringMessage("Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                }

                // =======
                // Posting
                // =======
                if (post)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Posting Stock Transfer... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            var groupedStockTransfers = from d in newStockTransfers
                                                        group d by new
                                                        {
                                                            d.BranchCode,
                                                            d.ManualSTNumber
                                                        } into g
                                                        select g.Key;

                            var stockTransfers = from d in groupedStockTransfers.ToList() select d;
                            if (stockTransfers.Any())
                            {
                                Decimal percentage = 0;
                                Int32 count = 0;

                                foreach (var stockTransfer in stockTransfers.ToList())
                                {
                                    count += 1;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(count) / Convert.ToDecimal(stockTransfers.Count())) * 100);

                                    while (true)
                                    {
                                        try
                                        {
                                            String postStockTransferTask = await PostStockTransfer(domain, stockTransfer.BranchCode, stockTransfer.ManualSTNumber);
                                            if (!postStockTransferTask.Equals("Post Successful..."))
                                            {
                                                trnIntegrationForm.logFolderMonitoringMessage(postStockTransferTask);
                                                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                                trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.logFolderMonitoringMessage("STIntegrationLogOnce");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Stock Transfer... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == stockTransfers.Count())
                                                {
                                                    trnIntegrationForm.logFolderMonitoringMessage("Post Successful!" + "\r\n\n");
                                                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                                }

                                                break;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage("Posting Error: " + e.Message + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                        catch (Exception e)
                        {
                            trnIntegrationForm.logFolderMonitoringMessage("Error: " + e.Message + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                    }
                }
            }
            else
            {
                trnIntegrationForm.logFolderMonitoringMessage("Erorr: Data Source Empty \r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }

            // =============
            // Move CSV File
            // =============
            trnIntegrationForm.logFolderMonitoringMessage("Moving Stock Transfer File... (0%) \r\n\n");
            while (true)
            {
                try
                {
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
                    trnIntegrationForm.logFolderMonitoringMessage("Moving File Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                    trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
            }
        }
    }
}
