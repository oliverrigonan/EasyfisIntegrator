﻿using System;
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
    class FolderMonitoringTrnReceivingReceiptController
    {
        // ======================
        // Send Receiving Receipt
        // ======================
        public async void SendReceivingReceipt(Forms.TrnIntegrationForm trnIntegrationForm, String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnReceivingReceipt> newReceivingReceipts = new List<Entities.FolderMonitoringTrnReceivingReceipt>();

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
                    String deleteTemporaryReceivingReceiptTask = await DeleteTemporaryReceivingReceipt(domain);
                    if (!deleteTemporaryReceivingReceiptTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporaryReceivingReceiptTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporaryReceivingReceiptTask))
                            {
                                previousErrorMessage = deleteTemporaryReceivingReceiptTask;
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
                        trnIntegrationForm.logFolderMonitoringMessage("RRIntegrateSuccessful");
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
                            newReceivingReceipts.Add(new Entities.FolderMonitoringTrnReceivingReceipt
                            {
                                BranchCode = data[0],
                                RRDate = data[1],
                                SupplierCode = data[2],
                                ManualRRNumber = data[3],
                                DocumentReference = data[4],
                                Remarks = data[5],
                                UserCode = userCode,
                                CreatedDateTime = data[6],
                                PONumber = data[7],
                                ManualPONumber = data[8],
                                PODate = data[9],
                                PODateNeeded = data[10],
                                ItemCode = data[11],
                                Particulars = data[12],
                                Unit = data[13],
                                Quantity = Convert.ToDecimal(data[14]),
                                Cost = Convert.ToDecimal(data[15]),
                                Amount = Convert.ToDecimal(data[16]),
                                ReceivedBranchCode = data[17]
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
            if (newReceivingReceipts.Any())
            {
                try
                {
                    Decimal percentage = 0;
                    trnIntegrationForm.logFolderMonitoringMessage("Sending... (0%) \r\n\n");

                    Boolean send = false;

                    var data = newReceivingReceipts.Take(100);
                    Int32 skip = 100;

                    for (Int32 i = 101; i <= newReceivingReceipts.Count(); i++)
                    {
                        if (i % 100 == 0)
                        {
                            data = newReceivingReceipts.Skip(skip).Take(100);
                            send = true;

                            skip = i;

                            percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newReceivingReceipts.Count())) * 100);
                        }
                        else
                        {
                            if (i == newReceivingReceipts.Count())
                            {
                                data = newReceivingReceipts.Skip(skip).Take(i - skip);
                                send = true;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newReceivingReceipts.Count())) * 100);
                            }
                        }

                        if (send)
                        {
                            jsonData = serializer.Serialize(data);

                            Boolean isErrorLogged = false;
                            String previousErrorMessage = String.Empty;

                            while (true)
                            {
                                String insertTemporaryReceivingReceiptTask = await InsertTemporaryReceivingReceipt(domain, jsonData);
                                if (!insertTemporaryReceivingReceiptTask.Equals("Send Successful..."))
                                {
                                    if (previousErrorMessage.Equals(String.Empty))
                                    {
                                        previousErrorMessage = insertTemporaryReceivingReceiptTask;
                                    }
                                    else
                                    {
                                        if (!previousErrorMessage.Equals(insertTemporaryReceivingReceiptTask))
                                        {
                                            previousErrorMessage = insertTemporaryReceivingReceiptTask;
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
                                    trnIntegrationForm.logFolderMonitoringMessage("RRIntegrateSuccessful");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nSending... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                    if (i == newReceivingReceipts.Count())
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
                var branchCodes = from d in newReceivingReceipts
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

                        var manualRRNumbers = from d in newReceivingReceipts
                                              where d.BranchCode.Equals(branchCode)
                                              group d by d.ManualRRNumber into g
                                              select g.Key;

                        var listManualRRNumbers = manualRRNumbers.ToList();
                        if (listManualRRNumbers.Any())
                        {
                            Int32 manualRRNumberCount = 0;

                            foreach (var manualRRNumber in listManualRRNumbers)
                            {
                                manualRRNumberCount += 1;
                                percentage = Convert.ToDecimal((Convert.ToDecimal(manualRRNumberCount) / Convert.ToDecimal(listManualRRNumbers.Count())) * 100);

                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String postReceivingReceiptTask = await PostReceivingReceipt(domain, branchCode, manualRRNumber);
                                    if (!postReceivingReceiptTask.Equals("Post Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = postReceivingReceiptTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(postReceivingReceiptTask))
                                            {
                                                previousErrorMessage = postReceivingReceiptTask;
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
                                        trnIntegrationForm.logFolderMonitoringMessage("RRIntegrateSuccessful");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                        if (manualRRNumberCount == listManualRRNumbers.Count())
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

                        if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\RR_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                        {
                            DirectoryInfo createDirectoryRRCSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\RR_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                        }

                        String folderForSentFiles = sysSettings.FolderForSentFiles + "\\RR_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        File.Move(file, folderForSentFiles + "RR_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
                    }

                    trnIntegrationForm.logFolderMonitoringMessage("RRIntegrateSuccessful");
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

        // ==================================
        // Delete Temporary Receiving Receipt
        // ==================================
        public Task<String> DeleteTemporaryReceivingReceipt(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/receivingReceipt/temporary/delete";

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

        // ==================================
        // Insert Temporary Receiving Receipt
        // ==================================
        public Task<String> InsertTemporaryReceivingReceipt(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/receivingReceipt/temporary/insert";

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

        // ======================
        // Post Receiving Receipt
        // ======================
        public Task<String> PostReceivingReceipt(String domain, String branchCode, String manualRRNumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/receivingReceipt/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnReceivingReceipt jsonReceivingReceipt = new Entities.FolderMonitoringTrnReceivingReceipt()
                {
                    BranchCode = branchCode,
                    ManualRRNumber = manualRRNumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonReceivingReceipt);

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