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
    class FolderMonitoringTrnReceivingReceiptController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnReceivingReceiptController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> ext = new List<String> { ".csv" };
            List<String> files = new List<String>(Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).Where(e => ext.Contains(Path.GetExtension(e))));
            if (files.Any()) { foreach (var file in files) { SendReceivingReceiptData(userCode, file, domain); } }
        }

        public void SendReceivingReceiptData(String userCode, String file, String domain)
        {
            try
            {
                String json = "";
                List<Entities.FolderMonitoringTrnReceivingReceipt> newReceivingReceipts = new List<Entities.FolderMonitoringTrnReceivingReceipt>();

                if (SysFileControl.IsCurrentFileClosed(file))
                {
                    using (StreamReader dataStreamReader = new StreamReader(file))
                    {
                        dataStreamReader.ReadLine();
                        while (dataStreamReader.Peek() != -1)
                        {
                            String[] data = dataStreamReader.ReadLine().Split(',');
                            newReceivingReceipts.Add(new Entities.FolderMonitoringTrnReceivingReceipt
                            {
                                BranchCode = data[0],
                                RRDate = data[1],
                                SupplierCode = data[2],
                                Term = data[3],
                                DocumentReference = data[4],
                                ManualRRNumber = data[5],
                                Remarks = data[6],
                                UserCode = userCode,
                                CreatedDateTime = data[7],
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

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        json = serializer.Serialize(newReceivingReceipts);
                    }
                }

                String[] fileNamePrefix = file.Split('\\');
                trnIntegrationForm.logFolderMonitoringMessage("Sending Receiving Receipt: " + fileNamePrefix[fileNamePrefix.Length - 1] + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/receivingReceipt/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("Send Successful!" + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

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
                    }
                    else
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("Send Failed! " + resp + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                    }
                }
            }
            catch (WebException we)
            {
                using (StreamReader streamReader = new StreamReader(we.Response.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");

                    trnIntegrationForm.logFolderMonitoringMessage("Web Exception Error: " + resp + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
            catch (Exception ex)
            {
                trnIntegrationForm.logFolderMonitoringMessage("Exception Error: " + ex.Message + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }
        }
    }
}