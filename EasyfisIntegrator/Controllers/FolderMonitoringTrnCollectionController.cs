using System;
using System.Collections.Generic;
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

        public FolderMonitoringTrnCollectionController(Forms.TrnIntegrationForm form, String textFile, String domain)
        {
            trnIntegrationForm = form;
            SendCollectionData(textFile, domain);
        }

        public String GetCollectionData(String textFile)
        {
            String json = "";
            String[] lines = File.ReadAllLines(textFile);

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
                        UserCode = data[5],
                        CreatedDateTime = data[6],
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

            return json;
        }

        public void SendCollectionData(String textFile, String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/collection/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Entities.FolderMonitoringTrnCollection collection = serializer.Deserialize<Entities.FolderMonitoringTrnCollection>(GetCollectionData(textFile));

                    streamWriter.Write(new JavaScriptSerializer().Serialize(collection));
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");

                    trnIntegrationForm.logFolderMonitoringMessage("Send Successful!" + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
            catch (WebException we)
            {
                using (StreamReader streamReader = new StreamReader(we.Response.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");

                    trnIntegrationForm.logFolderMonitoringMessage("Collection Error: " + resp + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
        }
    }
}