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
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnCollectionController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> ext = new List<String> { ".csv" };
            List<String> files = new List<String>(Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).Where(e => ext.Contains(Path.GetExtension(e))));
            if (files.Any()) { foreach (var file in files) { SendCollectionData(userCode, file, domain); } }
        }

        public void SendCollectionData(String userCode, String file, String domain)
        {
            try
            {
                String json = "";
                List<Entities.FolderMonitoringTrnCollection> newCollections = new List<Entities.FolderMonitoringTrnCollection>();

                if (SysFileControl.IsCurrentFileClosed(file))
                {
                    using (StreamReader dataStreamReader = new StreamReader(file))
                    {
                        dataStreamReader.ReadLine();
                        while (dataStreamReader.Peek() >= 0)
                        {
                            List<String> data = dataStreamReader.ReadLine().Split(',').ToList();
                            newCollections.Add(new Entities.FolderMonitoringTrnCollection
                            {
                                BranchCode = data[0],
                                ORDate = data[1],
                                CustomerCode = data[2],
                                Remarks = data[3],
                                ManualORNumber = data[4],
                                UserCode = userCode,
                                CreatedDateTime = data[5],
                                LineBranchCode = data[6],
                                AccountCode = data[7],
                                ArticleCode = data[8],
                                SINumber = data[9],
                                Particulars = data[10],
                                Amount = Convert.ToDecimal(data[11]),
                                PayType = data[12],
                                CheckNumber = data[13],
                                CheckDate = data[14],
                                CheckBank = data[15],
                                DepositoryBankCode = data[16],
                                IsClear = Convert.ToBoolean(data[17])
                            });
                        }

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        json = serializer.Serialize(newCollections);
                    }
                }

                String[] fileNamePrefix = file.Split('\\');
                trnIntegrationForm.logFolderMonitoringMessage("Sending Collection: " + fileNamePrefix[fileNamePrefix.Length - 1] + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/add";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter jsonStreamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    jsonStreamWriter.Write(json);
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader responseStreamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = responseStreamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        trnIntegrationForm.logFolderMonitoringMessage("Send Successful!" + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                        String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

                        using (StreamReader settingsStreamReader = new StreamReader(settingsPath))
                        {
                            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                            Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(settingsStreamReader.ReadToEnd());

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
                using (StreamReader webResponseStreamReader = new StreamReader(we.Response.GetResponseStream()))
                {
                    String resp = webResponseStreamReader.ReadToEnd().Replace("\"", "");

                    trnIntegrationForm.logFolderMonitoringMessage("Web Exception Error: " + resp + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                trnIntegrationForm.logFolderMonitoringMessage("Exception Error: " + ex.Message + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
            }
        }
    }
}