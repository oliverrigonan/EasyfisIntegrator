using System;
using System.Collections.Generic;
using System.Globalization;
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
        public DateTime journalDate = DateTime.Today;

        // ==================
        // Send Sales Invoice
        // ==================
        public async void SendSalesInvoice(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnSalesInvoice> newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Sales... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporarySalesInvoiceTask = await DeleteTemporarySalesInvoice(domain);
                    if (!deleteTemporarySalesInvoiceTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.folderMonitoringLogMessages(deleteTemporarySalesInvoiceTask);
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.folderMonitoringLogMessages("SIIntegrationLogOnce");

                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Sales... (100%) \r\n\n");
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
                                    PayType = data[15],
                                    DepositoryBankCode = data[16],
                                    No = count
                                });

                                journalDate = Convert.ToDateTime(data[1]);
                            }
                        }

                        trnIntegrationForm.folderMonitoringLogMessages("SIIntegrationLogOnce");

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

            if (newSalesInvoices.Any())
            {
                // =========================
                // Checking Muliple SI Dates
                // =========================
                trnIntegrationForm.folderMonitoringLogMessages("Checking Multiple SI Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedSIDates = from d in newSalesInvoices group d by d.SIDate into g select g.Key;

                        var SIDates = from d in groupedSIDates.ToList() select d;
                        if (SIDates.Count() > 1)
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("Checking Error: Cannot integrate multiple SI Dates. \r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("SIIntegrationLogOnce");

                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nChecking Multiple SI Dates... (100%) \r\n\n");
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
                trnIntegrationForm.folderMonitoringLogMessages("Sending Sales... (0%) \r\n\n");
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
                                            trnIntegrationForm.folderMonitoringLogMessages(insertTemporarySalesInvoiceTask);
                                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages("SIIntegrationLogOnce");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nSending Sales... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                            if (i == newSalesInvoices.Count())
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
                    trnIntegrationForm.folderMonitoringLogMessages("Posting Sales... (0%) \r\n\n");
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
                                                trnIntegrationForm.folderMonitoringLogMessages(postSalesInvoiceTask);
                                                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages("SIIntegrationLogOnce");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\nPosting Sales... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == salesInvoices.Count())
                                                {
                                                    trnIntegrationForm.folderMonitoringLogMessages("Post Successful!" + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                    String jDate = journalDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                                                    String apiURL = "http://" + domain + "/api/folderMonitoring/journal/" + jDate + "/SI";

                                                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                                                    httpWebRequest.Method = "GET";
                                                    httpWebRequest.Accept = "application/json";

                                                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                                    {
                                                        var result = streamReader.ReadToEnd();
                                                        JavaScriptSerializer js = new JavaScriptSerializer();
                                                        Entities.FolderMonitoringTrnJournal sumUpJournal = (Entities.FolderMonitoringTrnJournal)js.Deserialize(result, typeof(Entities.FolderMonitoringTrnJournal));

                                                        if (sumUpJournal != null)
                                                        {
                                                            trnIntegrationForm.folderMonitoringLogMessages("Date: " + sumUpJournal.JournalDate + "\r\n\n");
                                                            trnIntegrationForm.folderMonitoringLogMessages("Total Debit: " + sumUpJournal.TotalDebitAmount + "\r\n\n");
                                                            trnIntegrationForm.folderMonitoringLogMessages("Total Credit: " + sumUpJournal.TotalCreditAmount + "\r\n\n");
                                                            trnIntegrationForm.folderMonitoringLogMessages("Balance: " + sumUpJournal.TotalBalance + "\r\n\n");
                                                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
                                                        }
                                                    }
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
            trnIntegrationForm.folderMonitoringLogMessages("Moving Sales File... (0%) \r\n\n");
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

                    trnIntegrationForm.folderMonitoringLogMessages("SIIntegrationLogOnce");

                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\nMoving Sales File... (100%) \r\n\n");
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
