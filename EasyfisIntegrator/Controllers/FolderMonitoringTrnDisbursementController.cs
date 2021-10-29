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
    class FolderMonitoringTrnDisbursementController
    {
        public DateTime journalDate = DateTime.Today;

        // =================
        // Send Disbursement
        // =================
        public async void SendDisbursement(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnDisbursement> newDisbursements = new List<Entities.FolderMonitoringTrnDisbursement>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Disbursement... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporaryDisbursementTask = await DeleteTemporaryDisbursement(domain);
                    if (!deleteTemporaryDisbursementTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.folderMonitoringLogMessages(deleteTemporaryDisbursementTask);
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.folderMonitoringLogMessages("CVIntegrationLogOnce");

                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Disbursement... (100%) \r\n\n");
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
                newDisbursements = new List<Entities.FolderMonitoringTrnDisbursement>();

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
                                    Amount = Convert.ToDecimal(data[19]),
                                    No = count
                                });

                                journalDate = Convert.ToDateTime(data[1]);
                            }
                        }

                        trnIntegrationForm.folderMonitoringLogMessages("CVIntegrationLogOnce");

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

            if (newDisbursements.Any())
            {
                // =========================
                // Checking Muliple CV Dates
                // =========================
                trnIntegrationForm.folderMonitoringLogMessages("Checking Multiple CV Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedCVDates = from d in newDisbursements group d by d.CVDate into g select g.Key;

                        var CVDates = from d in groupedCVDates.ToList() select d;
                        if (CVDates.Count() > 1)
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("Checking Error: Cannot integrate multiple CV Dates. \r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("CVIntegrationLogOnce");

                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nChecking Multiple CV Dates... (100%) \r\n\n");
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
                trnIntegrationForm.folderMonitoringLogMessages("Sending Disbursement... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newDisbursements.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newDisbursements.Skip(skip).Take(100));
                                skip = i;

                                send = true;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                            }
                            else
                            {
                                if (i == newDisbursements.Count())
                                {
                                    if (newDisbursements.Count() <= 100)
                                    {
                                        jsonData = serializer.Serialize(newDisbursements);
                                    }
                                    else
                                    {
                                        jsonData = serializer.Serialize(newDisbursements.Skip(skip).Take(i - skip));
                                    }

                                    send = true;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                                }
                            }

                            if (send)
                            {
                                while (true)
                                {
                                    try
                                    {
                                        String insertTemporaryDisbursementTask = await InsertTemporaryDisbursement(domain, jsonData);
                                        if (!insertTemporaryDisbursementTask.Equals("Send Successful..."))
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages(insertTemporaryDisbursementTask);
                                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages("CVIntegrationLogOnce");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nSending Disbursement... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                            if (i == newDisbursements.Count())
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
                    trnIntegrationForm.folderMonitoringLogMessages("Posting Disbursement... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            var groupedDisbursements = from d in newDisbursements
                                                       group d by new
                                                       {
                                                           d.BranchCode,
                                                           d.ManualCVNumber
                                                       } into g
                                                       select g.Key;

                            var disbursements = from d in groupedDisbursements.ToList() select d;
                            if (disbursements.Any())
                            {
                                Decimal percentage = 0;
                                Int32 count = 0;

                                foreach (var disbursement in disbursements.ToList())
                                {
                                    count += 1;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(count) / Convert.ToDecimal(disbursements.Count())) * 100);

                                    while (true)
                                    {
                                        try
                                        {
                                            String postDisbursementTask = await PostDisbursement(domain, disbursement.BranchCode, disbursement.ManualCVNumber);
                                            if (!postDisbursementTask.Equals("Post Successful..."))
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages(postDisbursementTask);
                                                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages("CVIntegrationLogOnce");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\nPosting Disbursement... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == disbursements.Count())
                                                {
                                                    trnIntegrationForm.folderMonitoringLogMessages("Post Successful!" + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                    String jDate = journalDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                                                    String apiURL = "http://" + domain + "/api/folderMonitoring/journal/" + jDate + "/CV";

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
            trnIntegrationForm.folderMonitoringLogMessages("Moving Disbursement File... (0%) \r\n\n");
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

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\CV_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryCVCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\CV_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\CV_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "CV_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.folderMonitoringLogMessages("CVIntegrationLogOnce");

                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\nMoving Disbursement File... (100%) \r\n\n");
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
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
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
            }
        }
    }
}