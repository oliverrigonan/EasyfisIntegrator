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
    class FolderMonitoringTrnStockTransferController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnStockTransferController(Forms.TrnIntegrationForm form, String textFile, String domain)
        {
            trnIntegrationForm = form;
            SendStockTransferData(textFile, domain);
        }

        public String GetStockTransferData(String textFile)
        {
            String json = "";
            String[] lines = File.ReadAllLines(textFile);

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
                        UserCode = data[6],
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
                json = serializer.Serialize(newStockTransfers);
            }

            return json;
        }

        public void SendStockTransferData(String textFile, String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/stockTransfer/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Entities.FolderMonitoringTrnStockTransfer stockTransfer = serializer.Deserialize<Entities.FolderMonitoringTrnStockTransfer>(GetStockTransferData(textFile));

                    streamWriter.Write(new JavaScriptSerializer().Serialize(stockTransfer));
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
