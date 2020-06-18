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
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnStockOut> newStockOuts = new List<Entities.FolderMonitoringTrnStockOut>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Stock Out... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporaryStockOutTask = await DeleteTemporaryStockOut(domain);
                    if (!deleteTemporaryStockOutTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.folderMonitoringLogMessages(deleteTemporaryStockOutTask);
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.folderMonitoringLogMessages("OTIntegrationLogOnce");

                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Stock Out... (100%) \r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("Clean Successful!" + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        break;
                    }
                }
                catch (Exception e)
                {
                    trnIntegrationForm.folderMonitoringLogMessages("Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                    trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                    Thread.Sleep(5000);
                }
            }

            // ================
            // Reading CSV Data
            // ================
            trnIntegrationForm.folderMonitoringLogMessages("Reading CSV Data... (0%) \r\n\n");
            while (true)
            {
                newStockOuts = new List<Entities.FolderMonitoringTrnStockOut>();

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
                                    Amount = Convert.ToDecimal(data[12]),
                                    No = count
                                });
                            }
                        }

                        trnIntegrationForm.folderMonitoringLogMessages("OTIntegrationLogOnce");

                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\nReading CSV Data... (100%) \r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("Read Successful!" + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        break;
                    }
                    else
                    {
                        trnIntegrationForm.folderMonitoringLogMessages("Error: File: " + file + " is currently open. \r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    trnIntegrationForm.folderMonitoringLogMessages("Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                    trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                    Thread.Sleep(5000);
                }
            }

            if (newStockOuts.Any())
            {
                // =========================
                // Checking Muliple OT Dates
                // =========================
                trnIntegrationForm.folderMonitoringLogMessages("Checking Multiple OT Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedOTDates = from d in newStockOuts group d by d.OTDate into g select g.Key;

                        var OTDates = from d in groupedOTDates.ToList() select d;
                        if (OTDates.Count() > 1)
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("Checking Error: Cannot integrate multiple OT Dates. \r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("OTIntegrationLogOnce");

                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nChecking Multiple OT Dates... (100%) \r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Check Successful!" + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        trnIntegrationForm.folderMonitoringLogMessages("Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                }

                Boolean post = false;

                // =======
                // Sending
                // =======
                trnIntegrationForm.folderMonitoringLogMessages("Sending Stock Out... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newStockOuts.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newStockOuts.Skip(skip).Take(100));
                                skip = i;

                                send = true;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newStockOuts.Count())) * 100);
                            }
                            else
                            {
                                if (i == newStockOuts.Count())
                                {
                                    if (newStockOuts.Count() <= 100)
                                    {
                                        jsonData = serializer.Serialize(newStockOuts);
                                    }
                                    else
                                    {
                                        jsonData = serializer.Serialize(newStockOuts.Skip(skip).Take(i - skip));
                                    }

                                    send = true;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newStockOuts.Count())) * 100);
                                }
                            }

                            if (send)
                            {
                                while (true)
                                {
                                    try
                                    {
                                        String insertTemporaryStockOutTask = await InsertTemporaryStockOut(domain, jsonData);
                                        if (!insertTemporaryStockOutTask.Equals("Send Successful..."))
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages(insertTemporaryStockOutTask);
                                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages("OTIntegrationLogOnce");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nSending Stock Out... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                            if (i == newStockOuts.Count())
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages("Send Successful!" + "\r\n\n");
                                                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
                                            }

                                            break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        trnIntegrationForm.folderMonitoringLogMessages("Sending Error: " + e.Message + "\r\n\n");
                                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

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
                        trnIntegrationForm.folderMonitoringLogMessages("Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                }

                // =======
                // Posting
                // =======
                if (post)
                {
                    trnIntegrationForm.folderMonitoringLogMessages("Posting Stock Out... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            var groupedStockOuts = from d in newStockOuts
                                                   group d by new
                                                   {
                                                       d.BranchCode,
                                                       d.ManualOTNumber
                                                   } into g
                                                   select g.Key;

                            var stockOuts = from d in groupedStockOuts.ToList() select d;
                            if (stockOuts.Any())
                            {
                                Decimal percentage = 0;
                                Int32 count = 0;

                                foreach (var stockOut in stockOuts.ToList())
                                {
                                    count += 1;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(count) / Convert.ToDecimal(stockOuts.Count())) * 100);

                                    while (true)
                                    {
                                        try
                                        {
                                            String postStockOutTask = await PostStockOut(domain, stockOut.BranchCode, stockOut.ManualOTNumber);
                                            if (!postStockOutTask.Equals("Post Successful..."))
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages(postStockOutTask);
                                                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages("OTIntegrationLogOnce");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\nPosting Stock Out... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == stockOuts.Count())
                                                {
                                                    trnIntegrationForm.folderMonitoringLogMessages("Post Successful!" + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
                                                }

                                                break;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages("Posting Error: " + e.Message + "\r\n\n");
                                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                        catch (Exception e)
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("Error: " + e.Message + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                    }
                }
            }
            else
            {
                trnIntegrationForm.folderMonitoringLogMessages("Erorr: Data Source Empty \r\n\n");
                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
            }

            // =============
            // Move CSV File
            // =============
            trnIntegrationForm.folderMonitoringLogMessages("Moving Stock Out File... (0%) \r\n\n");
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

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\OT_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryOTCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\OT_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\OT_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "OT_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.folderMonitoringLogMessages("OTIntegrationLogOnce");

                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\nMoving Stock Out File... (100%) \r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("Move Successful!" + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                    break;
                }
                catch (Exception e)
                {
                    trnIntegrationForm.folderMonitoringLogMessages("Moving File Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                    trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                    Thread.Sleep(5000);
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
            }
        }
    }
}
