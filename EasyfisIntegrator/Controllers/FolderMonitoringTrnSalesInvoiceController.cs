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
            try
            {
                trnIntegrationForm.logFolderMonitoringMessage("Cleaning Temporary Data..." + "\r\n\n");

                String deleteTemporarySalesInvoiceTask = await DeleteTemporarySalesInvoice(domain);
                trnIntegrationForm.logFolderMonitoringMessage(deleteTemporarySalesInvoiceTask);
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                String json = "";
                List<Entities.FolderMonitoringTrnSalesInvoice> newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();

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

                if (newSalesInvoices.Any())
                {
                    String[] fileNamePrefix = file.Split('\\');
                    trnIntegrationForm.logFolderMonitoringMessage("Sending Sales: " + fileNamePrefix[fileNamePrefix.Length - 1] + "... \r\n\n");

                    Int32 skip = 0;

                    for (var i = 1; i <= newSalesInvoices.Count(); i++)
                    {
                        if (i % 100 == 0)
                        {
                            Int32 take = 100;

                            var jsonSalesInvoices = newSalesInvoices.Skip(skip).Take(take);
                            skip += 100;

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            json = serializer.Serialize(jsonSalesInvoices);

                            String insertTemporarySalesInvoiceTask = await InsertTemporarySalesInvoice(domain, json, file);
                            if (!insertTemporarySalesInvoiceTask.Equals("Send Successful..."))
                            {
                                trnIntegrationForm.logFolderMonitoringMessage(insertTemporarySalesInvoiceTask);
                                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                break;
                            }
                            else
                            {
                                if (i == newSalesInvoices.Count())
                                {
                                    trnIntegrationForm.logFolderMonitoringMessage("Send Successful..." + "\r\n\n");
                                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                    trnIntegrationForm.logFolderMonitoringMessage("Posting Sales Invoice..." + "\r\n\n");

                                    String postSalesInvoiceTask = await PostSalesInvoice(domain, file);
                                    trnIntegrationForm.logFolderMonitoringMessage(postSalesInvoiceTask);
                                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                }
                            }
                        }
                        else
                        {
                            if (i == newSalesInvoices.Count())
                            {
                                Int32 take = newSalesInvoices.Count() - skip;

                                var jsonSalesInvoices = newSalesInvoices.Skip(skip).Take(take);
                                skip += take;

                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                json = serializer.Serialize(jsonSalesInvoices);

                                String insertTemporarySalesInvoiceTask = await InsertTemporarySalesInvoice(domain, json, file);
                                if (!insertTemporarySalesInvoiceTask.Equals("Send Successful..."))
                                {
                                    trnIntegrationForm.logFolderMonitoringMessage(insertTemporarySalesInvoiceTask);
                                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                    break;
                                }
                                else
                                {
                                    if (i == newSalesInvoices.Count())
                                    {
                                        trnIntegrationForm.logFolderMonitoringMessage("Send Successful..." + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                                        trnIntegrationForm.logFolderMonitoringMessage("Posting Sales Invoice..." + "\r\n\n");

                                        String postSalesInvoiceTask = await PostSalesInvoice(domain, file);
                                        trnIntegrationForm.logFolderMonitoringMessage(postSalesInvoiceTask);
                                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                trnIntegrationForm.logFolderMonitoringMessage("Exception Error: " + ex + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
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
                        return Task.FromResult("Clean Successful..." + "\r\n\n");
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
        public Task<String> InsertTemporarySalesInvoice(String domain, String json, String file)
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
        public Task<String> PostSalesInvoice(String domain, String file)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/salesInvoice/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { streamWriter.Write(""); }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
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

                        return Task.FromResult("Post Successful..." + "\r\n\n");
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
