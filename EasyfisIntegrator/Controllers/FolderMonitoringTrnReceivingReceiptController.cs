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
    class FolderMonitoringTrnReceivingReceiptController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnReceivingReceiptController(Forms.TrnIntegrationForm form, String textFile, String domain)
        {
            trnIntegrationForm = form;
            SendReceivingReceiptData(textFile, domain);
        }

        public String GetReceivingReceiptData(String textFile)
        {
            String json = "";
            String[] lines = File.ReadAllLines(textFile);

            if (lines.Length > 0)
            {
                List<Entities.FolderMonitoringTrnReceivingReceipt> newReceivingReceipts = new List<Entities.FolderMonitoringTrnReceivingReceipt>();

                for (int r = 1; r < lines.Length; r++)
                {
                    String[] data = lines[r].Split(',');

                    newReceivingReceipts.Add(new Entities.FolderMonitoringTrnReceivingReceipt
                    {
                        BranchCode = data[0],
                        RRDate = data[1],
                        SupplierCode = data[2],
                        Term = data[3],
                        DocumentReference = data[4],
                        ManualRRNumber = data[5],
                        Remarks = data[6],
                        UserCode = data[7],
                        CreatedDateTime = data[8],
                        PONumber = data[9],
                        PODate = data[10],
                        PODateNeeded = data[11],
                        ItemCode = data[12],
                        Particulars = data[13],
                        Unit = data[14],
                        Quantity = Convert.ToDecimal(data[15]),
                        Cost = Convert.ToDecimal(data[16]),
                        Amount = Convert.ToDecimal(data[17]),
                        ReceivedBranchCode = data[18]
                    });
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                json = serializer.Serialize(newReceivingReceipts);
            }

            return json;
        }

        public void SendReceivingReceiptData(String textFile, String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/receivingReceipt/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Entities.FolderMonitoringTrnReceivingReceipt receivingReceipt = serializer.Deserialize<Entities.FolderMonitoringTrnReceivingReceipt>(GetReceivingReceiptData(textFile));

                    streamWriter.Write(new JavaScriptSerializer().Serialize(receivingReceipt));
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

                    trnIntegrationForm.logFolderMonitoringMessage("Receiving Receipt Error: " + resp + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
        }
    }
}
