﻿using System;
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
    class FolderMonitoringTrnDisbursementController
    {
        // =================
        // Send Disbursement
        // =================
        public async void SendDisbursement(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnDisbursement> newDisbursements = new List<Entities.FolderMonitoringTrnDisbursement>();
            Boolean isExceptionErrorOccured = false;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Cleaning
            while (true)
            {
                try
                {
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Disbursement... (0%) \r\n\n");

                    Boolean isErrorLogged = false;
                    String previousErrorMessage = String.Empty;

                    String deleteTemporaryDisbursementTask = await DeleteTemporaryDisbursement(domain);
                    if (!deleteTemporaryDisbursementTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryDisbursementTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryDisbursementTask))
                            {
                                previousErrorMessage = deleteTemporaryDisbursementTask;
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
                        trnIntegrationForm.logFolderMonitoringMessage("CVIntegrationLogOnce");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Disbursement... (100%) \r\n\n");

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
                                    Amount = Convert.ToDecimal(data[19])
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
            if (newDisbursements.Any())
            {
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;
                        trnIntegrationForm.logFolderMonitoringMessage("Sending Disbursement... (0%) \r\n\n");

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newDisbursements.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newDisbursements.Skip(skip).Take(100));
                                send = true;

                                skip = i;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                            }
                            else
                            {
                                if (newDisbursements.Count() <= 100)
                                {
                                    if (i == newDisbursements.Count())
                                    {
                                        jsonData = serializer.Serialize(newDisbursements);
                                        send = true;

                                        percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                                    }
                                }
                                else
                                {
                                    if (i == newDisbursements.Count())
                                    {
                                        jsonData = serializer.Serialize(newDisbursements.Skip(skip).Take(i - skip));
                                        send = true;

                                        percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newDisbursements.Count())) * 100);
                                    }
                                }
                            }

                            if (send)
                            {
                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String insertTemporaryDisbursementTask = await InsertTemporaryDisbursement(domain, jsonData);
                                    if (!insertTemporaryDisbursementTask.Equals("Send Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = insertTemporaryDisbursementTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(insertTemporaryDisbursementTask))
                                            {
                                                previousErrorMessage = insertTemporaryDisbursementTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("CVIntegrationLogOnce");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending Disbursement... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (i == newDisbursements.Count())
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
                        var disbursements = from d in newDisbursements
                                            group d by new
                                            {
                                                d.BranchCode,
                                                d.ManualCVNumber
                                            } into g
                                            select g;

                        if (disbursements.Any())
                        {
                            Decimal percentage = 0;
                            trnIntegrationForm.logFolderMonitoringMessage("Posting Disbursement... (0%) \r\n\n");

                            Int32 disbursementCount = 0;

                            foreach (var disbursement in disbursements)
                            {
                                disbursementCount += 1;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(disbursementCount) / Convert.ToDecimal(disbursements.Count())) * 100);

                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String postDisbursementTask = await PostDisbursement(domain, disbursement.Key.BranchCode, disbursement.Key.ManualCVNumber);
                                    if (!postDisbursementTask.Equals("Post Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = postDisbursementTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(postDisbursementTask))
                                            {
                                                previousErrorMessage = postDisbursementTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("CVIntegrationLogOnce");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Disbursement... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (disbursementCount == disbursements.Count())
                                        {
                                            trnIntegrationForm.logFolderMonitoringMessage("Post Successful!" + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                        }

                                        break;
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
                    trnIntegrationForm.logFolderMonitoringMessage("Moving Disbursement File... (0%) \r\n\n");

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

                    trnIntegrationForm.logFolderMonitoringMessage("CVIntegrationLogOnce");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nMoving Disbursement File... (100%) \r\n\n");

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