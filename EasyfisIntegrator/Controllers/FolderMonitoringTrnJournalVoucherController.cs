using System;
using System.Collections.Generic;
using System.Globalization;
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
    class FolderMonitoringTrnJournalVoucherController
    {
        public DateTime journalDate = DateTime.Today;

        // ====================
        // Send Journal Voucher
        // ====================
        public async void SendJournalVoucher(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnJournalVoucher> newJournalVouchers = new List<Entities.FolderMonitoringTrnJournalVoucher>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Journal Voucher... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporaryJournalVoucherTask = await DeleteTemporaryJournalVoucher(domain);
                    if (!deleteTemporaryJournalVoucherTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.folderMonitoringLogMessages(deleteTemporaryJournalVoucherTask);
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.folderMonitoringLogMessages("JVIntegrationLogOnce");

                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Journal Voucher... (100%) \r\n\n");
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
                newJournalVouchers = new List<Entities.FolderMonitoringTrnJournalVoucher>();

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
                                newJournalVouchers.Add(new Entities.FolderMonitoringTrnJournalVoucher
                                {
                                    BranchCode = data[0],
                                    JVDate = data[1],
                                    Remarks = data[2],
                                    ManualJVNumber = data[3],
                                    UserCode = userCode,
                                    CreatedDateTime = data[4],
                                    EntryBranchCode = data[5],
                                    AccountCode = data[6],
                                    ArticleCode = data[7],
                                    Particulars = data[8],
                                    DebitAmount = Convert.ToDecimal(data[9]),
                                    CreditAmount = Convert.ToDecimal(data[10]),
                                    APRRNumber = data[11],
                                    APManualRRNumber = data[12],
                                    ARSINumber = data[13],
                                    ARManualSINumber = data[14],
                                    IsClear = Convert.ToBoolean(data[15]),
                                    No = count
                                });

                                journalDate = Convert.ToDateTime(data[1]);
                            }
                        }

                        trnIntegrationForm.folderMonitoringLogMessages("JVIntegrationLogOnce");

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

            if (newJournalVouchers.Any())
            {
                // =========================
                // Checking Muliple JV Dates
                // =========================
                trnIntegrationForm.folderMonitoringLogMessages("Checking Multiple JV Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedJVDates = from d in newJournalVouchers group d by d.JVDate into g select g.Key;

                        var JVDates = from d in groupedJVDates.ToList() select d;
                        if (JVDates.Count() > 1)
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("Checking Error: Cannot integrate multiple JV Dates. \r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("JVIntegrationLogOnce");

                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nChecking Multiple JV Dates... (100%) \r\n\n");
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
                trnIntegrationForm.folderMonitoringLogMessages("Sending Journal Voucher... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newJournalVouchers.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newJournalVouchers.Skip(skip).Take(100));
                                skip = i;

                                send = true;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newJournalVouchers.Count())) * 100);
                            }
                            else
                            {
                                if (i == newJournalVouchers.Count())
                                {
                                    if (newJournalVouchers.Count() <= 100)
                                    {
                                        jsonData = serializer.Serialize(newJournalVouchers);
                                    }
                                    else
                                    {
                                        jsonData = serializer.Serialize(newJournalVouchers.Skip(skip).Take(i - skip));
                                    }

                                    send = true;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newJournalVouchers.Count())) * 100);
                                }
                            }

                            if (send)
                            {
                                while (true)
                                {
                                    try
                                    {
                                        String insertTemporaryJournalVoucherTask = await InsertTemporaryJournalVoucher(domain, jsonData);
                                        if (!insertTemporaryJournalVoucherTask.Equals("Send Successful..."))
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages(insertTemporaryJournalVoucherTask);
                                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages("JVIntegrationLogOnce");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nSending Journal Voucher... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                            if (i == newJournalVouchers.Count())
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
                    trnIntegrationForm.folderMonitoringLogMessages("Posting Journal Voucher... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            var groupedJournalVouchers = from d in newJournalVouchers
                                                         group d by new
                                                         {
                                                             d.BranchCode,
                                                             d.ManualJVNumber
                                                         } into g
                                                         select g.Key;

                            var journalVouchers = from d in groupedJournalVouchers.ToList() select d;
                            if (journalVouchers.Any())
                            {
                                Decimal percentage = 0;
                                Int32 count = 0;

                                foreach (var journalVoucher in journalVouchers.ToList())
                                {
                                    count += 1;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(count) / Convert.ToDecimal(journalVouchers.Count())) * 100);

                                    while (true)
                                    {
                                        try
                                        {
                                            String postJournalVoucherTask = await PostJournalVoucher(domain, journalVoucher.BranchCode, journalVoucher.ManualJVNumber);
                                            if (!postJournalVoucherTask.Equals("Post Successful..."))
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages(postJournalVoucherTask);
                                                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages("JVIntegrationLogOnce");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\nPosting Journal Voucher... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == journalVouchers.Count())
                                                {
                                                    trnIntegrationForm.folderMonitoringLogMessages("Post Successful!" + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                    String jDate = journalDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                                                    String apiURL = domain + "/api/folderMonitoring/journal/" + jDate + "/JV";

                                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
            trnIntegrationForm.folderMonitoringLogMessages("Moving Journal Voucher File... (0%) \r\n\n");
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

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\JV_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryJVCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\JV_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\JV_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "JV_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.folderMonitoringLogMessages("JVIntegrationLogOnce");

                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\nMoving JournalVoucher File... (100%) \r\n\n");
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

        // ================================
        // Delete Temporary Journal Voucher
        // ================================
        public Task<String> DeleteTemporaryJournalVoucher(String domain)
        {
            try
            {
                String apiURL = domain + "/api/folderMonitoring/journalVoucher/temporary/delete";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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

        // ================================
        // Insert Temporary Journal Voucher
        // ================================
        public Task<String> InsertTemporaryJournalVoucher(String domain, String json)
        {
            try
            {
                String apiURL = domain + "/api/folderMonitoring/journalVoucher/temporary/insert";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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

        // ====================
        // Post Journal Voucher
        // ====================
        public Task<String> PostJournalVoucher(String domain, String branchCode, String manualJVNumber)
        {
            try
            {
                String apiURL = domain + "/api/folderMonitoring/journalVoucher/post";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnJournalVoucher jsonJournalVoucher = new Entities.FolderMonitoringTrnJournalVoucher()
                {
                    BranchCode = branchCode,
                    ManualJVNumber = manualJVNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonJournalVoucher);

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

        // ==========================
        // Delete All Journal Voucher
        // ==========================
        public async void DeleteAllJournalVoucher(Forms.TrnIntegrationForm trnIntegrationForm, String domain, String currentDate)
        {
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nDeleting Journal Voucher... (0%) \r\n\n");
            try
            {
                String deleteUploadedJournalVoucherTask = await DeleteUploadedJournalVoucher(domain, currentDate);
                if (!deleteUploadedJournalVoucherTask.Equals("Delete Successful..."))
                {
                    trnIntegrationForm.folderMonitoringLogMessages(deleteUploadedJournalVoucherTask);
                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
                }
                else
                {
                    trnIntegrationForm.folderMonitoringLogMessages("CVIntegrationLogOnce");

                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\nDeleting Journal Voucher... (100%) \r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("Clean Successful!" + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.folderMonitoringLogMessages("Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
            }
        }

        // ===============================
        // Delete Uploaded Journal Voucher
        // ===============================
        public Task<String> DeleteUploadedJournalVoucher(String domain, String currentDate)
        {
            try
            {
                String apiURL = domain + "/api/folderMonitoring/journalVoucher/uploaded/delete/" + currentDate;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
                        return Task.FromResult("Delete Successful...");
                    }
                    else
                    {
                        return Task.FromResult("Delete Failed! " + resp + "\r\n\n");
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