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
    class FolderMonitoringTrnSalesInvoiceController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnSalesInvoiceController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> ext = new List<String> { ".csv" };
            List<String> files = new List<String>(Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).Where(e => ext.Contains(Path.GetExtension(e))));
            if (files.Any()) { foreach (var file in files) { SendSalesInvoice(userCode, file, domain); } }
        }

        // ==================
        // Send Sales Invoice
        // ==================
        public async void SendSalesInvoice(String userCode, String file, String domain)
        {
            List<Entities.FolderMonitoringTrnSalesInvoice> newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";
            Boolean post = false;

            // Delete
            while (true)
            {
                String deleteTemporarySalesInvoiceTask = await DeleteTemporarySalesInvoice(domain);
                if (!deleteTemporarySalesInvoiceTask.Equals("Clean Successful..."))
                {
                    trnIntegrationForm.logFolderMonitoringMessage(deleteTemporarySalesInvoiceTask);
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                    Thread.Sleep(3000);
                }
                else
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Clean Successful!" + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                    break;
                }
            }

            // CSV
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
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logFolderMonitoringMessage("Exception Error: " + e + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }

            // Send
            if (newSalesInvoices.Any())
            {
                try
                {
                    String[] fileName = file.Split('\\');
                    trnIntegrationForm.logFolderMonitoringMessage("Sending Sales: " + fileName[fileName.Length - 1] + "... \r\n\n");

                    var data = newSalesInvoices.Take(100);
                    int skip = 100;

                    for (var i = 101; i <= newSalesInvoices.Count(); i++)
                    {
                        if (i % 100 == 0)
                        {
                            data = newSalesInvoices.Skip(skip).Take(100);
                            skip = i;
                        }
                        else
                        {
                            if (i == newSalesInvoices.Count())
                            {
                                data = newSalesInvoices.Skip(skip).Take(i - skip);
                            }
                        }

                        jsonData = serializer.Serialize(data);

                        while (true)
                        {
                            String insertTemporarySalesInvoiceTask = await InsertTemporarySalesInvoice(domain, jsonData);
                            if (!insertTemporarySalesInvoiceTask.Equals("Send Successful..."))
                            {
                                trnIntegrationForm.logFolderMonitoringMessage(insertTemporarySalesInvoiceTask);
                                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                Thread.Sleep(3000);
                            }
                            else
                            {
                                if (i == newSalesInvoices.Count())
                                {
                                    trnIntegrationForm.logFolderMonitoringMessage("Send Successful!" + "\r\n\n");
                                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                }

                                break;
                            }
                        }
                    }

                    post = true;
                }
                catch (Exception e)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Exception Error: " + e + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }

            // Post
            if (post)
            {
                var branchCodes = from d in newSalesInvoices
                                  group d by d.BranchCode into g
                                  select g.Key;

                var listBranchCodes = branchCodes.ToList();
                if (listBranchCodes.Any())
                {
                    Int32 branchCodeCount = 0;

                    foreach (var branchCode in listBranchCodes)
                    {
                        branchCodeCount += 1;

                        var manualSINumbers = from d in newSalesInvoices
                                              where d.BranchCode.Equals(branchCode)
                                              group d by d.ManualSINumber into g
                                              select g.Key;

                        var listManualSINumbers = manualSINumbers.ToList();
                        if (listManualSINumbers.Any())
                        {
                            foreach (var manualSINumber in listManualSINumbers)
                            {
                                while (true)
                                {
                                    String postSalesInvoiceTask = await PostSalesInvoice(domain, branchCode, manualSINumber);
                                    if (!postSalesInvoiceTask.Equals("Post Successful..."))
                                    {
                                        trnIntegrationForm.logFolderMonitoringMessage(postSalesInvoiceTask);
                                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("Retrying...\r\n\n");

                                        Thread.Sleep(3000);
                                    }
                                    else
                                    {
                                        if (branchCodeCount == listBranchCodes.Count())
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
                    }
                }

                // Move CSV File
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
                }
                catch (Exception e)
                {
                    trnIntegrationForm.logFolderMonitoringMessage("Exception Error: " + e + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
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
            catch (Exception ex)
            {
                return Task.FromResult("Exception Error: " + ex + "\r\n\n");
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
            catch (Exception ex)
            {
                return Task.FromResult("Exception Error: " + ex + "\r\n\n");
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
            catch (Exception ex)
            {
                return Task.FromResult("Exception Error: " + ex + "\r\n\n");
            }
        }
    }
}
