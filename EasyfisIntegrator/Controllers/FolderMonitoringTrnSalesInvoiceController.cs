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
    class FolderMonitoringTrnSalesInvoiceController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnSalesInvoiceController(Forms.TrnIntegrationForm form, String textFile, String domain)
        {
            trnIntegrationForm = form;
            SendSalesInvoiceData(textFile, domain);
        }

        public String GetSalesInvoiceData(String textFile)
        {
            String json = "";
            String[] lines = File.ReadAllLines(textFile);

            if (lines.Length > 0)
            {
                List<Entities.FolderMonitoringTrnSalesInvoice> newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();

                for (int r = 1; r < lines.Length; r++)
                {
                    String[] data = lines[r].Split(',');

                    newSalesInvoices.Add(new Entities.FolderMonitoringTrnSalesInvoice
                    {
                        BranchCode = data[0],
                        SIDate = data[1],
                        CustomerCode = data[2],
                        Term = data[3],
                        DocumentReference = data[4],
                        ManualSINumber = data[5],
                        Remarks = data[6],
                        UserCode = data[7],
                        CreatedDateTime = data[8],
                        ItemCode = data[9],
                        Particulars = data[10],
                        Unit = data[11],
                        Quantity = Convert.ToDecimal(data[12]),
                        Price = Convert.ToDecimal(data[13]),
                        Discount = data[14],
                        DiscountRate = Convert.ToDecimal(data[15]),
                        DiscountAmount = Convert.ToDecimal(data[16]),
                        NetPrice = Convert.ToDecimal(data[17]),
                        Amount = Convert.ToDecimal(data[18])
                    });
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                json = serializer.Serialize(newSalesInvoices);
            }

            return json;
        }

        public void SendSalesInvoiceData(String textFile, String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/salesInvoice/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Entities.FolderMonitoringTrnSalesInvoice salesInvoice = serializer.Deserialize<Entities.FolderMonitoringTrnSalesInvoice>(GetSalesInvoiceData(textFile));

                    streamWriter.Write(new JavaScriptSerializer().Serialize(salesInvoice));
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
