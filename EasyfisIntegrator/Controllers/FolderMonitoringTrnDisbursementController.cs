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

        public FolderMonitoringTrnDisbursementController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> files = Directory.GetFiles(directory).ToList();
            if (files.Any()) { foreach (var file in files) { SendDisbursementData(userCode, file, domain); } }
        }

        public void SendDisbursementData(String userCode, String file, String domain)
        {
            try
            {
                String json = "";
                String[] lines = File.ReadAllLines(file);

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
                            UserCode = userCode,
                            CreatedDateTime = data[12],
                            AccountCode = data[13],
                            ArticleCode = data[14],
                            RRNumber = data[15],
                            Particulars = data[16],
                            Amount = Convert.ToDecimal(data[17]),
                        });
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    json = serializer.Serialize(newDisbursements);
                }

                trnIntegrationForm.logFolderMonitoringMessage("Sending Disbursement..." + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/disbursement/add";

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