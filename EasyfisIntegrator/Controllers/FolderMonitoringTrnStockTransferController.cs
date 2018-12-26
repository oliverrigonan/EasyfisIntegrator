using System;
using System.Collections.Generic;
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
    class FolderMonitoringTrnStockTransferController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnStockTransferController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> ext = new List<String> { ".csv" };
            List<String> files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(s => ext.Contains(Path.GetExtension(s))).ToList();
            if (files.Any()) { foreach (var file in files) { SendStockTransferData(userCode, file, domain); } }
        }

        public void SendStockTransferData(String userCode, String file, String domain)
        {
            try
            {
                String json = "";
                String[] lines = File.ReadAllLines(file);

                if (lines.Length > 0)
                {
                    List<Entities.FolderMonitoringTrnStockTransfer> newStockTransfers = new List<Entities.FolderMonitoringTrnStockTransfer>();

                    for (int r = 1; r < lines.Length; r++)
                    {
                        String[] data = lines[r].Split(',');

                        newStockTransfers.Add(new Entities.FolderMonitoringTrnStockTransfer
                        {
                            BranchCode = data[0],
                            STDate = data[1],
                            ToBranchCode = data[2],
                            ArticleCode = data[3],
                            Remarks = data[4],
                            ManualSTNumber = data[5],
                            UserCode = userCode,
                            CreatedDateTime = data[6],
                            ItemCode = data[7],
                            Particulars = data[8],
                            Unit = data[9],
                            Quantity = Convert.ToDecimal(data[10]),
                            Cost = Convert.ToDecimal(data[11]),
                            Amount = Convert.ToDecimal(data[12])
                        });
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    json = serializer.Serialize(newStockTransfers);
                }

                trnIntegrationForm.logFolderMonitoringMessage("Sending Stock Transfer..." + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/stockTransfer/add";

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

                            if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\ST_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                            {
                                DirectoryInfo createDirectorySICSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\ST_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                            }

                            String folderForSentFiles = sysSettings.FolderForSentFiles + "\\ST_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                            File.Move(file, folderForSentFiles + "ST_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
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
