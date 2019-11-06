using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class FolderMonitoringTrnSalesInvoiceController
    {
        // ==================
        // Send Sales Invoice
        // ==================
        public async void SendSalesInvoice(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnSalesInvoice> newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Sales... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporarySalesInvoiceTask = await DeleteTemporarySalesInvoice(domain);
                    if (!deleteTemporarySalesInvoiceTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.logFolderMonitoringMessage(deleteTemporarySalesInvoiceTask);
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("SIIntegrationLogOnce");

                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Sales... (100%) \r\n\n");
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
                newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();

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
                                newSalesInvoices.Add(new Entities.FolderMonitoringTrnSalesInvoice
                                {
                                    BranchCode = data[0],
                                    SIDate = data[1],
                                    CustomerCode = data[2],
                                    ManualSINumber = data[3],
                                    DocumentReference = data[4],
                                    Remarks = data[5],
                                    UserCode = userCode,
                                    CreatedDateTime = data[6],
                                    ItemCode = data[7],
                                    Particulars = data[8],
                                    Unit = data[9],
                                    Quantity = Convert.ToDecimal(data[10]),
                                    Price = Convert.ToDecimal(data[11]),
                                    DiscountAmount = Convert.ToDecimal(data[12]),
                                    NetPrice = Convert.ToDecimal(data[13]),
                                    Amount = Convert.ToDecimal(data[14]),
                                    No = count
                                });
                            }
                        }

                        trnIntegrationForm.logFolderMonitoringMessage("SIIntegrationLogOnce");

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

            if (newSalesInvoices.Any())
            {
                // =========================
                // Checking Muliple SI Dates
                // =========================
                trnIntegrationForm.logFolderMonitoringMessage("Checking Multiple SI Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedSIDates = from d in newSalesInvoices group d by d.SIDate into g select g.Key;

                        var SIDates = from d in groupedSIDates.ToList() select d;
                        if (SIDates.Count() > 1)
                        {
                            trnIntegrationForm.logFolderMonitoringMessage("Checking Error: Cannot integrate multiple SI Dates. \r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.logFolderMonitoringMessage("SIIntegrationLogOnce");

                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nChecking Multiple SI Dates... (100%) \r\n\n");
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
                trnIntegrationForm.logFolderMonitoringMessage("Sending Sales... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newSalesInvoices.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newSalesInvoices.Skip(skip).Take(100));
                                skip = i;

                                send = true;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newSalesInvoices.Count())) * 100);
                            }
                            else
                            {
                                if (i == newSalesInvoices.Count())
                                {
                                    if (newSalesInvoices.Count() <= 100)
                                    {
                                        jsonData = serializer.Serialize(newSalesInvoices);
                                    }
                                    else
                                    {
                                        jsonData = serializer.Serialize(newSalesInvoices.Skip(skip).Take(i - skip));
                                    }

                                    send = true;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newSalesInvoices.Count())) * 100);
                                }
                            }

                            if (send)
                            {
                                while (true)
                                {
                                    try
                                    {
                                        String insertTemporarySalesInvoiceTask = await InsertTemporarySalesInvoice(domain, jsonData);
                                        if (!insertTemporarySalesInvoiceTask.Equals("Send Successful..."))
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage(insertTemporarySalesInvoiceTask);
                                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage("SIIntegrationLogOnce");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Sales... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                            if (i == newSalesInvoices.Count())
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
                    trnIntegrationForm.logFolderMonitoringMessage("Posting Sales... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            var groupedSalesInvoices = from d in newSalesInvoices
                                                       group d by new
                                                       {
                                                           d.BranchCode,
                                                           d.ManualSINumber
                                                       } into g
                                                       select g.Key;

                            var salesInvoices = from d in groupedSalesInvoices.ToList() select d;
                            if (salesInvoices.Any())
                            {
                                Decimal percentage = 0;
                                Int32 count = 0;

                                foreach (var salesInvoice in salesInvoices.ToList())
                                {
                                    count += 1;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(count) / Convert.ToDecimal(salesInvoices.Count())) * 100);

                                    while (true)
                                    {
                                        try
                                        {
                                            String postSalesInvoiceTask = await PostSalesInvoice(domain, salesInvoice.BranchCode, salesInvoice.ManualSINumber);
                                            if (!postSalesInvoiceTask.Equals("Post Successful..."))
                                            {
                                                trnIntegrationForm.logFolderMonitoringMessage(postSalesInvoiceTask);
                                                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                                trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.logFolderMonitoringMessage("SIIntegrationLogOnce");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Sales... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == salesInvoices.Count())
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
            trnIntegrationForm.logFolderMonitoringMessage("Moving Sales File... (0%) \r\n\n");
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

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\SI_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectorySICSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\SI_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\SI_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "SI_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.logFolderMonitoringMessage("SIIntegrationLogOnce");

                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Sales File... (100%) \r\n\n");
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

        // ==============================
        // Delete Temporary Sales Invoice
        // ==============================
        public Task<String> DeleteTemporarySalesInvoice(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/salesInvoice/temporary/delete";

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

        // ==============================
        // Insert Temporary Sales Invoice
        // ==============================
        public Task<String> InsertTemporarySalesInvoice(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/salesInvoice/temporary/insert";

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

        // ==================
        // Post Sales Invoice
        // ==================
        public Task<String> PostSalesInvoice(String domain, String branchCode, String manualSINumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/salesInvoice/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnSalesInvoice jsonSalesInvoice = new Entities.FolderMonitoringTrnSalesInvoice()
                {
                    BranchCode = branchCode,
                    ManualSINumber = manualSINumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonSalesInvoice);

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
