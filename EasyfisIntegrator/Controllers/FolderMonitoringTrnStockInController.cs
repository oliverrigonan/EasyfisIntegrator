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
    class FolderMonitoringTrnStockInController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnStockInController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> ext = new List<String> { ".csv" };
            List<String> files = new List<String>(Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).Where(e => ext.Contains(Path.GetExtension(e))));
            if (files.Any()) { foreach (var file in files) { SendStockInData(userCode, file, domain); } }
        }

        public void SendStockInData(String userCode, String file, String domain)
        {
            try
            {
                String json = "";
                List<Entities.FolderMonitoringTrnStockIn> newStockIns = new List<Entities.FolderMonitoringTrnStockIn>();

                if (SysFileControl.IsCurrentFileClosed(file))
                {
                    using (StreamReader dataStreamReader = new StreamReader(file))
                    {
                        dataStreamReader.ReadLine();
                        while (dataStreamReader.Peek() >= 0)
                        {
                            List<String> data = dataStreamReader.ReadLine().Split(',').ToList();
                            newStockIns.Add(new Entities.FolderMonitoringTrnStockIn
                            {
                                BranchCode = data[0],
                                INDate = data[1],
                                AccountCode = data[2],
                                ArticleCode = data[3],
                                Remarks = data[4],
                                ManualINNumber = data[5],
                                IsProduce = Convert.ToBoolean(data[6]),
                                UserCode = userCode,
                                CreatedDateTime = data[7],
                                ItemCode = data[8],
                                Particulars = data[9],
                                Unit = data[10],
                                Quantity = Convert.ToDecimal(data[11]),
                                Cost = Convert.ToDecimal(data[12]),
                                Amount = Convert.ToDecimal(data[13])
                            });
                        }

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        json = serializer.Serialize(newStockIns);
                    }
                }

                String[] fileNamePrefix = file.Split('\\');
                trnIntegrationForm.logFolderMonitoringMessage("Sending Stock In: " + fileNamePrefix[fileNamePrefix.Length - 1] + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/stockIn/add";

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

                            if (!Directory.Exists(sysSettings.FolderForSentFiles + "\\IN_" + DateTime.Now.ToString("yyyyMMdd") + "\\"))
                            {
                                DirectoryInfo createDirectorySICSV = Directory.CreateDirectory(sysSettings.FolderForSentFiles + "\\IN_" + DateTime.Now.ToString("yyyyMMdd") + "\\", securityRules);
                            }

                            String folderForSentFiles = sysSettings.FolderForSentFiles + "\\IN_" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                            File.Move(file, folderForSentFiles + "IN_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
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
