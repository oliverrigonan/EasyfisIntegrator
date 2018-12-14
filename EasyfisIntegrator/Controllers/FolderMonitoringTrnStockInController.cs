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
    class FolderMonitoringTrnStockInController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnStockInController(Forms.TrnIntegrationForm form, String textFile, String domain)
        {
            trnIntegrationForm = form;
            SendStockInData(textFile, domain);
        }

        public String GetStockInData(String textFile)
        {
            String json = "";
            String[] lines = File.ReadAllLines(textFile);

            if (lines.Length > 0)
            {
                List<Entities.FolderMonitoringTrnStockIn> newStockIns = new List<Entities.FolderMonitoringTrnStockIn>();

                for (int r = 1; r < lines.Length; r++)
                {
                    String[] data = lines[r].Split(',');

                    newStockIns.Add(new Entities.FolderMonitoringTrnStockIn
                    {
                        BranchCode = data[0],
                        INDate = data[1],
                        AccountCode = data[2],
                        ArticleCode = data[3],
                        Remarks = data[5],
                        ManualINNumber = data[6],
                        IsProduce = Convert.ToBoolean(data[7]),
                        UserCode = data[8],
                        CreatedDateTime = data[9],
                        ItemCode = data[10],
                        Particulars = data[11],
                        Unit = data[12],
                        Quantity = Convert.ToDecimal(data[13]),
                        Cost = Convert.ToDecimal(data[14]),
                        Amount = Convert.ToDecimal(data[15])
                    });
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                json = serializer.Serialize(newStockIns);
            }

            return json;
        }

        public void SendStockInData(String textFile, String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockIn/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Entities.FolderMonitoringTrnStockIn stockIn = serializer.Deserialize<Entities.FolderMonitoringTrnStockIn>(GetStockInData(textFile));

                    streamWriter.Write(new JavaScriptSerializer().Serialize(stockIn));
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

                    trnIntegrationForm.logFolderMonitoringMessage("Sales Invoice Error: " + resp + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
        }
    }
}
