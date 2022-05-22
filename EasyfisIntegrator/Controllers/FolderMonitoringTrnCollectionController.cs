using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    class FolderMonitoringTrnCollectionController
    {
        public DateTime journalDate = DateTime.Today;

        // ===============
        // Send Collection
        // ===============
        public async void SendCollection(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnCollection> newCollections = new List<Entities.FolderMonitoringTrnCollection>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Collection... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporaryCollectionTask = await DeleteTemporaryCollection(domain);
                    if (!deleteTemporaryCollectionTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.folderMonitoringLogMessages(deleteTemporaryCollectionTask);
                        trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                        trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.folderMonitoringLogMessages("ORIntegrationLogOnce");

                        trnIntegrationForm.folderMonitoringLogMessages("\r\n\nCleaning Collection... (100%) \r\n\n");
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
                newCollections = new List<Entities.FolderMonitoringTrnCollection>();

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
                                newCollections.Add(new Entities.FolderMonitoringTrnCollection
                                {
                                    BranchCode = data[0],
                                    ORDate = data[1],
                                    CustomerCode = data[2],
                                    ManualORNumber = data[3],
                                    Remarks = data[4],
                                    UserCode = userCode,
                                    CreatedDateTime = data[5],
                                    LineBranchCode = data[6],
                                    AccountCode = data[7],
                                    ArticleCode = data[8],
                                    SINumber = data[9],
                                    ManualSINumber = data[10],
                                    Particulars = data[11],
                                    Amount = Convert.ToDecimal(data[12]),
                                    PayType = data[13],
                                    CheckNumber = data[14],
                                    CheckDate = data[15],
                                    CheckBank = data[16],
                                    DepositoryBankCode = data[17],
                                    IsClear = Convert.ToBoolean(data[18]),
                                    No = count
                                });

                                journalDate = Convert.ToDateTime(data[1]);
                            }
                        }

                        trnIntegrationForm.folderMonitoringLogMessages("ORIntegrationLogOnce");

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

            if (newCollections.Any())
            {
                // =========================
                // Checking Muliple OR Dates
                // =========================
                trnIntegrationForm.folderMonitoringLogMessages("Checking Multiple OR Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedORDates = from d in newCollections group d by d.ORDate into g select g.Key;

                        var ORDates = from d in groupedORDates.ToList() select d;
                        if (ORDates.Count() > 1)
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("Checking Error: Cannot integrate multiple OR Dates. \r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.folderMonitoringLogMessages("ORIntegrationLogOnce");

                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nChecking Multiple OR Dates... (100%) \r\n\n");
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
                trnIntegrationForm.folderMonitoringLogMessages("Sending Collection... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newCollections.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newCollections.Skip(skip).Take(100));
                                skip = i;

                                send = true;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newCollections.Count())) * 100);
                            }
                            else
                            {
                                if (i == newCollections.Count())
                                {
                                    if (newCollections.Count() <= 100)
                                    {
                                        jsonData = serializer.Serialize(newCollections);
                                    }
                                    else
                                    {
                                        jsonData = serializer.Serialize(newCollections.Skip(skip).Take(i - skip));
                                    }

                                    send = true;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newCollections.Count())) * 100);
                                }
                            }

                            if (send)
                            {
                                while (true)
                                {
                                    try
                                    {
                                        String insertTemporaryCollectionTask = await InsertTemporaryCollection(domain, jsonData);
                                        if (!insertTemporaryCollectionTask.Equals("Send Successful..."))
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages(insertTemporaryCollectionTask);
                                            trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                            trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            trnIntegrationForm.folderMonitoringLogMessages("ORIntegrationLogOnce");
                                            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nSending Collection... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                            if (i == newCollections.Count())
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
                    trnIntegrationForm.folderMonitoringLogMessages("Posting Collection... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            var groupedCollections = from d in newCollections
                                                     group d by new
                                                     {
                                                         d.BranchCode,
                                                         d.ManualORNumber
                                                     } into g
                                                     select g.Key;

                            var collections = from d in groupedCollections.ToList() select d;
                            if (collections.Any())
                            {
                                Decimal percentage = 0;
                                Int32 count = 0;

                                foreach (var collection in collections.ToList())
                                {
                                    count += 1;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(count) / Convert.ToDecimal(collections.Count())) * 100);

                                    while (true)
                                    {
                                        try
                                        {
                                            String postCollectionTask = await PostCollection(domain, collection.BranchCode, collection.ManualORNumber);
                                            if (!postCollectionTask.Equals("Post Successful..."))
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages(postCollectionTask);
                                                trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                trnIntegrationForm.folderMonitoringLogMessages("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.folderMonitoringLogMessages("ORIntegrationLogOnce");
                                                trnIntegrationForm.folderMonitoringLogMessages("\r\n\nPosting Collection... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == collections.Count())
                                                {
                                                    trnIntegrationForm.folderMonitoringLogMessages("Post Successful!" + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");

                                                    String jDate = journalDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                                                    String apiURL = "https://" + domain + "/api/folderMonitoring/journal/" + jDate + "/OR";

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
            trnIntegrationForm.folderMonitoringLogMessages("Moving Collection File... (0%) \r\n\n");
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

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\OR_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryORCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\OR_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\OR_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "OR_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.folderMonitoringLogMessages("ORIntegrationLogOnce");

                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\nMoving Collection File... (100%) \r\n\n");
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

        // ===========================
        // Delete Temporary Collection
        // ===========================
        public Task<String> DeleteTemporaryCollection(String domain)
        {
            try
            {
                String apiURL = "https://" + domain + "/api/folderMonitoring/collection/temporary/delete";

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

        // ===========================
        // Insert Temporary Collection
        // ===========================
        public Task<String> InsertTemporaryCollection(String domain, String json)
        {
            try
            {
                String apiURL = "https://" + domain + "/api/folderMonitoring/collection/temporary/insert";

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

        // ===============
        // Post Collection
        // ===============
        public Task<String> PostCollection(String domain, String branchCode, String manualORNumber)
        {
            try
            {
                String apiURL = "https://" + domain + "/api/folderMonitoring/collection/post";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnCollection jsonCollection = new Entities.FolderMonitoringTrnCollection()
                {
                    BranchCode = branchCode,
                    ManualORNumber = manualORNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonCollection);

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

        // =====================
        // Delete All Collection
        // =====================
        public async void DeleteAllCollection(Forms.TrnIntegrationForm trnIntegrationForm, String domain, String currentDate)
        {
            trnIntegrationForm.folderMonitoringLogMessages("\r\n\nDeleting Collection... (0%) \r\n\n");
            try
            {
                String deleteUploadedCollectionTask = await DeleteUploadedCollection(domain, currentDate);
                if (!deleteUploadedCollectionTask.Equals("Delete Successful..."))
                {
                    trnIntegrationForm.folderMonitoringLogMessages(deleteUploadedCollectionTask);
                    trnIntegrationForm.folderMonitoringLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\n");
                }
                else
                {
                    trnIntegrationForm.folderMonitoringLogMessages("ORIntegrationLogOnce");

                    trnIntegrationForm.folderMonitoringLogMessages("\r\n\nDeleting Collection... (100%) \r\n\n");
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

        // ===========================
        // Delete Uploaded  Collection
        // ===========================
        public Task<String> DeleteUploadedCollection(String domain, String currentDate)
        {
            try
            {
                String apiURL = "https://" + domain + "/api/folderMonitoring/collection/uploaded/delete/" + currentDate;

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