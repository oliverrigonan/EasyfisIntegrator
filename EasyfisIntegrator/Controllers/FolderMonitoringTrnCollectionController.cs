using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class FolderMonitoringTrnCollectionController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnCollectionController(Forms.TrnIntegrationForm form, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> files = Directory.GetFiles(directory).ToList();
            if (files.Any()) { foreach (var file in files) { SendCollectionData(file, domain); } }
        }

        public void SendCollectionData(String file, String domain)
        {
            try
            {
                String json = "";
                String[] lines = File.ReadAllLines(file);

                if (lines.Length > 0)
                {
                    List<Entities.FolderMonitoringTrnCollection> newCollections = new List<Entities.FolderMonitoringTrnCollection>();

                    for (int r = 1; r < lines.Length; r++)
                    {
                        String[] data = lines[r].Split(',');

                        newCollections.Add(new Entities.FolderMonitoringTrnCollection
                        {
                            BranchCode = data[0],
                            ORDate = data[1],
                            CustomerCode = data[2],
                            Remarks = data[3],
                            ManualORNumber = data[4],
                            UserCode = "easyfis",
                            CreatedDateTime = data[5],
                            AccountCode = data[6],
                            ArticleCode = data[7],
                            SINumber = data[8],
                            Particulars = data[9],
                            Amount = Convert.ToDecimal(data[10]),
                            PayType = data[11],
                            CheckNumber = data[12],
                            CheckDate = data[13],
                            CheckBank = data[14],
                            DepositoryBankCode = data[15],
                            IsClear = Convert.ToBoolean(data[16])
                        });
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    json = serializer.Serialize(newCollections);
                }

                trnIntegrationForm.logFolderMonitoringMessage("Sending Collection..." + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/add";

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

                    trnIntegrationForm.logFolderMonitoringMessage("Send Successful!" + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");

                    File.Delete(file);
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