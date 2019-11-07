using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // ===============
        // Send Collection
        // ===============
        public async void SendCollection(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nOpening File: " + file + " \r\n\n");

            List<Entities.FolderMonitoringTrnCollection> newCollections = new List<Entities.FolderMonitoringTrnCollection>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            // ========
            // Cleaning
            // ========
            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Collection... (0%) \r\n\n");
            while (true)
            {
                try
                {
                    String deleteTemporaryCollectionTask = await DeleteTemporaryCollection(domain);
                    if (!deleteTemporaryCollectionTask.Equals("Clean Successful..."))
                    {
                        trnIntegrationForm.logFolderMonitoringMessage(deleteTemporaryCollectionTask);
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("ORIntegrationLogOnce");

                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Collection... (100%) \r\n\n");
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
                            }
                        }

                        trnIntegrationForm.logFolderMonitoringMessage("ORIntegrationLogOnce");

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

            if (newCollections.Any())
            {
                // =========================
                // Checking Muliple OR Dates
                // =========================
                trnIntegrationForm.logFolderMonitoringMessage("Checking Multiple OR Dates... (0%) \r\n\n");
                while (true)
                {
                    try
                    {
                        var groupedORDates = from d in newCollections group d by d.ORDate into g select g.Key;

                        var ORDates = from d in groupedORDates.ToList() select d;
                        if (ORDates.Count() > 1)
                        {
                            trnIntegrationForm.logFolderMonitoringMessage("Checking Error: Cannot integrate multiple OR Dates. \r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                            Thread.Sleep(5000);
                        }
                        else
                        {
                            trnIntegrationForm.logFolderMonitoringMessage("ORIntegrationLogOnce");

                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nChecking Multiple OR Dates... (100%) \r\n\n");
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
                trnIntegrationForm.logFolderMonitoringMessage("Sending Collection... (0%) \r\n\n");
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
                                            trnIntegrationForm.logFolderMonitoringMessage(insertTemporaryCollectionTask);
                                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                            trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage("ORIntegrationLogOnce");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Collection... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                            if (i == newCollections.Count())
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
                    trnIntegrationForm.logFolderMonitoringMessage("Posting Collection... (0%) \r\n\n");
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
                                                trnIntegrationForm.logFolderMonitoringMessage(postCollectionTask);
                                                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                                trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.logFolderMonitoringMessage("ORIntegrationLogOnce");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Collection... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (count == collections.Count())
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
            trnIntegrationForm.logFolderMonitoringMessage("Moving Collection File... (0%) \r\n\n");
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

                    trnIntegrationForm.logFolderMonitoringMessage("ORIntegrationLogOnce");

                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Collection File... (100%) \r\n\n");
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

        // ===========================
        // Delete Temporary Collection
        // ===========================
        public Task<String> DeleteTemporaryCollection(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/temporary/delete";

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
                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/temporary/insert";

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
                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/post";

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
    }
}