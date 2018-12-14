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
    class FolderMonitoringTrnDisbursementController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnDisbursementController(Forms.TrnIntegrationForm form, String textFile, String domain)
        {
            trnIntegrationForm = form;
            SendDisbursementData(textFile, domain);
        }

        public String GetDisbursementData(String textFile)
        {
            String json = "";
            String[] lines = File.ReadAllLines(textFile);

            if (lines.Length > 0)
            {
                List<Entities.FolderMonitoringTrnDisbursement> newDisbursements = new List<Entities.FolderMonitoringTrnDisbursement>();

                for (int r = 1; r < lines.Length; r++)
                {
                    String[] data = lines[r].Split(',');

                    newDisbursements.Add(new Entities.FolderMonitoringTrnDisbursement
                    {
                        BranchCode = data[0],
                        CVDate = data[1],
                        SupplierCode = data[2],
                        Payee = data[3],
                        PayType = data[4],
                        BankCode = data[5],
                        Remarks = data[6],
                        ManualCVNumber = data[7],
                        CheckNumber = data[8],
                        CheckDate = data[9],
                        IsCrossCheck = Convert.ToBoolean(data[10]),
                        IsClear = Convert.ToBoolean(data[11]),
                        UserCode = data[12],
                        CreatedDateTime = data[13],
                        AccountCode = data[14],
                        ArticleCode = data[15],
                        RRNumber = data[16],
                        Particulars = data[17],
                        Amount = Convert.ToDecimal(data[18]),
                    });
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                json = serializer.Serialize(newDisbursements);
            }

            return json;
        }

        public void SendDisbursementData(String textFile, String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/disbursement/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Entities.FolderMonitoringTrnDisbursement disbursement = serializer.Deserialize<Entities.FolderMonitoringTrnDisbursement>(GetDisbursementData(textFile));

                    streamWriter.Write(new JavaScriptSerializer().Serialize(disbursement));
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

                    trnIntegrationForm.logFolderMonitoringMessage("Disbursement Error: " + resp + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
        }
    }
}