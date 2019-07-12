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
    class FolderMonitoringTrnJournalVoucherController
    {
        // ====================
        // Send Journal Voucher
        // ====================
        public async void SendJournalVoucher(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnJournalVoucher> newJournalVouchers = new List<Entities.FolderMonitoringTrnJournalVoucher>();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Delete
            try
            {
                trnIntegrationForm.logFolderMonitoringMessage("Cleaning... (0%) \r\n\n");

                Boolean isErrorLogged = false;
                String previousErrorMessage = String.Empty;

                while (true)
                {
                    String deleteTemporaryJournalVoucherTask = await DeleteTemporaryJournalVoucher(domain);
                    if (!deleteTemporaryJournalVoucherTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryJournalVoucherTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryJournalVoucherTask))
                            {
                                previousErrorMessage = deleteTemporaryJournalVoucherTask;
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
                        trnIntegrationForm.logFolderMonitoringMessage("JVIntegrateSuccessful");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning... (100%) \r\n\n");

                        trnIntegrationForm.logFolderMonitoringMessage("Clean Successful!" + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logFolderMonitoringMessage("Cleaning Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }

            // CSV Data
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
                                IsClear = Convert.ToBoolean(data[15])
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logFolderMonitoringMessage("CSV Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }

            // Send
            if (newJournalVouchers.Any())
            {
                try
                {
                    Decimal percentage = 0;
                    trnIntegrationForm.logFolderMonitoringMessage("Sending... (0%) \r\n\n");

                    Boolean send = false;

                    var data = newJournalVouchers.Take(100);
                    Int32 skip = 100;

                    for (Int32 i = 101; i <= newJournalVouchers.Count(); i++)
                    {
                        if (i % 100 == 0)
                        {
                            data = newJournalVouchers.Skip(skip).Take(100);
                            send = true;

                            skip = i;

                            percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newJournalVouchers.Count())) * 100);
                        }
                        else
                        {
                            if (i == newJournalVouchers.Count())
                            {
                                data = newJournalVouchers.Skip(skip).Take(i - skip);
                                send = true;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newJournalVouchers.Count())) * 100);
                            }
                        }

                        if (send)
                        {
                            jsonData = serializer.Serialize(data);

                            Boolean isErrorLogged = false;
                            String previousErrorMessage = String.Empty;

                            while (true)
                            {
                                String insertTemporaryJournalVoucherTask = await InsertTemporaryJournalVoucher(domain, jsonData);
                                if (!insertTemporaryJournalVoucherTask.Equals("Send Successful..."))
                                {
                                    if (previousErrorMessage.Equals(String.Empty))
                                    {
                                        previousErrorMessage = insertTemporaryJournalVoucherTask;
                                    }
                                    else
                                    {
                                        if (!previousErrorMessage.Equals(insertTemporaryJournalVoucherTask))
                                        {
                                            previousErrorMessage = insertTemporaryJournalVoucherTask;
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
                                    trnIntegrationForm.logFolderMonitoringMessage("JVIntegrateSuccessful");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                    if (i == newJournalVouchers.Count())
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
                }
                catch (Exception e)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Sending Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }

            // Post
            if (post)
            {
                var branchCodes = from d in newJournalVouchers
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
                        trnIntegrationForm.logFolderMonitoringMessage("Posting Branch: " + branchCode + " ... (0%) \r\n\n");

                        var manualJVNumbers = from d in newJournalVouchers
                                              where d.BranchCode.Equals(branchCode)
                                              group d by d.ManualJVNumber into g
                                              select g.Key;

                        var listManualJVNumbers = manualJVNumbers.ToList();
                        if (listManualJVNumbers.Any())
                        {
                            Int32 manualJVNumberCount = 0;

                            foreach (var manualJVNumber in listManualJVNumbers)
                            {
                                manualJVNumberCount += 1;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(manualJVNumberCount) / Convert.ToDecimal(listManualJVNumbers.Count())) * 100);

                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String postJournalVoucherTask = await PostJournalVoucher(domain, branchCode, manualJVNumber);
                                    if (!postJournalVoucherTask.Equals("Post Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = postJournalVoucherTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(postJournalVoucherTask))
                                            {
                                                previousErrorMessage = postJournalVoucherTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("JVIntegrateSuccessful");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (manualJVNumberCount == listManualJVNumbers.Count())
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

                // Move CSV File
                try
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Moving... (0%) \r\n\n");

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

                    trnIntegrationForm.logFolderMonitoringMessage("JVIntegrateSuccessful");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving... (100%) \r\n\n");

                    trnIntegrationForm.logFolderMonitoringMessage("Move Successful!" + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
                catch (Exception e)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Moving File Error: " + e.Message + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
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
                String apiURL = "http://" + domain + "/api/folderMonitoring/journalVoucher/temporary/delete";

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

        // ================================
        // Insert Temporary Journal Voucher
        // ================================
        public Task<String> InsertTemporaryJournalVoucher(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/journalVoucher/temporary/insert";

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

        // ====================
        // Post Journal Voucher
        // ====================
        public Task<String> PostJournalVoucher(String domain, String branchCode, String manualJVNumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/journalVoucher/post";

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
            catch (Exception e)
            {
                return Task.FromResult("Exception Error: " + e.Message + "\r\n\n");
            }
        }
    }
}