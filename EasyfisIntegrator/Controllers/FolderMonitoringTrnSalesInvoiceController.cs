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
            List<Entities.FolderMonitoringTrnSalesInvoice> newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();
            Boolean isExceptionErrorOccured = false;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            Boolean post = false;

            // Cleaning
            while (true)
            {
                try
                {
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\nCleaning Sales... (0%) \r\n\n");

                    Boolean isErrorLogged = false;
                    String previousErrorMessage = String.Empty;

                    String deleteTemporarySalesInvoiceTask = await DeleteTemporarySalesInvoice(domain);
                    if (!deleteTemporarySalesInvoiceTask.Equals("Clean Successful..."))
                    {
                        if (previousErrorMessage.Equals(String.Empty))
                        {
                            previousErrorMessage = deleteTemporarySalesInvoiceTask;
                        }
                        else
                        {
                            if (!previousErrorMessage.Equals(deleteTemporarySalesInvoiceTask))
                            {
                                previousErrorMessage = deleteTemporarySalesInvoiceTask;
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
                                    Amount = Convert.ToDecimal(data[14])
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
            if (newSalesInvoices.Any())
            {
                while (true)
                {
                    try
                    {
                        Decimal percentage = 0;
                        trnIntegrationForm.logFolderMonitoringMessage("Sending Sales... (0%) \r\n\n");

                        Boolean send = false;
                        Int32 skip = 0;

                        for (Int32 i = 1; i <= newSalesInvoices.Count(); i++)
                        {
                            if (i % 100 == 0)
                            {
                                jsonData = serializer.Serialize(newSalesInvoices.Skip(skip).Take(100));
                                send = true;

                                skip = i;

                                percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newSalesInvoices.Count())) * 100);
                            }
                            else
                            {
                                if (newSalesInvoices.Count() <= 100)
                                {
                                    jsonData = serializer.Serialize(newSalesInvoices);
                                    send = true;

                                    percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newSalesInvoices.Count())) * 100);
                                }
                                else
                                {
                                    if (i == newSalesInvoices.Count())
                                    {
                                        jsonData = serializer.Serialize(newSalesInvoices.Skip(skip).Take(i - skip));
                                        send = true;

                                        percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newSalesInvoices.Count())) * 100);
                                    }
                                }
                            }

                            if (send)
                            {
                                Boolean isErrorLogged = false;
                                String previousErrorMessage = String.Empty;

                                while (true)
                                {
                                    String insertTemporarySalesInvoiceTask = await InsertTemporarySalesInvoice(domain, jsonData);
                                    if (!insertTemporarySalesInvoiceTask.Equals("Send Successful..."))
                                    {
                                        if (previousErrorMessage.Equals(String.Empty))
                                        {
                                            previousErrorMessage = insertTemporarySalesInvoiceTask;
                                        }
                                        else
                                        {
                                            if (!previousErrorMessage.Equals(insertTemporarySalesInvoiceTask))
                                            {
                                                previousErrorMessage = insertTemporarySalesInvoiceTask;
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
                        var branchCodes = from d in newSalesInvoices
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
                                trnIntegrationForm.logFolderMonitoringMessage("Posting Sales Branch: " + branchCode + " ... (0%) \r\n\n");

                                var manualSINumbers = from d in newSalesInvoices
                                                      where d.BranchCode.Equals(branchCode)
                                                      group d by d.ManualSINumber into g
                                                      select g.Key;

                                var listManualSINumbers = manualSINumbers.ToList();
                                if (listManualSINumbers.Any())
                                {
                                    Int32 manualSINumberCount = 0;

                                    foreach (var manualSINumber in listManualSINumbers)
                                    {
                                        manualSINumberCount += 1;
                                        percentage = Convert.ToDecimal((Convert.ToDecimal(manualSINumberCount) / Convert.ToDecimal(listManualSINumbers.Count())) * 100);

                                        Boolean isErrorLogged = false;
                                        String previousErrorMessage = String.Empty;

                                        while (true)
                                        {
                                            String postSalesInvoiceTask = await PostSalesInvoice(domain, branchCode, manualSINumber);
                                            if (!postSalesInvoiceTask.Equals("Post Successful..."))
                                            {
                                                if (previousErrorMessage.Equals(String.Empty))
                                                {
                                                    previousErrorMessage = postSalesInvoiceTask;
                                                }
                                                else
                                                {
                                                    if (!previousErrorMessage.Equals(postSalesInvoiceTask))
                                                    {
                                                        previousErrorMessage = postSalesInvoiceTask;
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
                                                trnIntegrationForm.logFolderMonitoringMessage("SIIntegrationLogOnce");
                                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\nPosting Sales Branch: " + branchCode + " ... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (manualSINumberCount == listManualSINumbers.Count())
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
                    trnIntegrationForm.logFolderMonitoringMessage("Moving Sales File... (0%) \r\n\n");

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
            catch (Exception e)
            {
                return Task.FromResult("Exception Error: " + e.Message + "\r\n\n");
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
            catch (Exception e)
            {
                return Task.FromResult("Exception Error: " + e.Message + "\r\n\n");
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
            catch (Exception e)
            {
                return Task.FromResult("Exception Error: " + e.Message + "\r\n\n");
            }
        }
    }
}
