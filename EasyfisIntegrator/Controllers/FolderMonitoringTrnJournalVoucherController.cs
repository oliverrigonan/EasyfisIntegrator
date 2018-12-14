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
    class FolderMonitoringTrnJournalVoucherController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnJournalVoucherController(Forms.TrnIntegrationForm form, String textFile, String domain)
        {
            trnIntegrationForm = form;
            SendJournalVoucherData(textFile, domain);
        }

        public String GetJournalVoucherData(String textFile)
        {
            String json = "";
            String[] lines = File.ReadAllLines(textFile);

            if (lines.Length > 0)
            {
                List<Entities.FolderMonitoringTrnJournalVoucher> newJournalVouchers = new List<Entities.FolderMonitoringTrnJournalVoucher>();

                for (int r = 1; r < lines.Length; r++)
                {
                    String[] data = lines[r].Split(',');

                    newJournalVouchers.Add(new Entities.FolderMonitoringTrnJournalVoucher
                    {
                        BranchCode = data[0],
                        JVDate = data[1],
                        Remarks = data[2],
                        ManualJVNumber = data[3],
                        UserCode = data[4],
                        CreatedDateTime = data[5],
                        EntryBranchCode = data[6],
                        AccountCode = data[7],
                        ArticleCode = data[8],
                        Particulars = data[9],
                        DebitAmount = Convert.ToDecimal(data[10]),
                        CreditAmount = Convert.ToDecimal(data[11]),
                        APRRNumber = data[12],
                        ARSINumber = data[13],
                        IsClear = Convert.ToBoolean(data[14])
                    });
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                json = serializer.Serialize(newJournalVouchers);
            }

            return json;
        }

        public void SendJournalVoucherData(String textFile, String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/folderMonitoring/journalVoucher/add";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Entities.FolderMonitoringTrnJournalVoucher journalVoucher = serializer.Deserialize<Entities.FolderMonitoringTrnJournalVoucher>(GetJournalVoucherData(textFile));

                    streamWriter.Write(new JavaScriptSerializer().Serialize(journalVoucher));
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

                    trnIntegrationForm.logFolderMonitoringMessage("Journal Voucher Error: " + resp + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logFolderMonitoringMessage("\r\n\n");
                }
            }
        }
    }
}