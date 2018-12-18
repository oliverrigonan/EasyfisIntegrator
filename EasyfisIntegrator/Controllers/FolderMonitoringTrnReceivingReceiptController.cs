﻿using System;
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
    class FolderMonitoringTrnReceivingReceiptController
    {
        public Forms.TrnIntegrationForm trnIntegrationForm;

        public FolderMonitoringTrnReceivingReceiptController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> files = Directory.GetFiles(directory).ToList();
            if (files.Any()) { foreach (var file in files) { SendReceivingReceiptData(userCode, file, domain); } }
        }

        public void SendReceivingReceiptData(String userCode, String file, String domain)
        {
            try
            {
                String json = "";
                String[] lines = File.ReadAllLines(file);

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
                            UserCode = userCode,
                            CreatedDateTime = data[7],
                            PONumber = data[8],
                            PODate = data[9],
                            PODateNeeded = data[10],
                            ItemCode = data[11],
                            Particulars = data[12],
                            Unit = data[13],
                            Quantity = Convert.ToDecimal(data[14]),
                            Cost = Convert.ToDecimal(data[15]),
                            Amount = Convert.ToDecimal(data[16]),
                            ReceivedBranchCode = data[17]
                        });
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    json = serializer.Serialize(newReceivingReceipts);
                }

                Debug.WriteLine(json);

                trnIntegrationForm.logFolderMonitoringMessage("Sending Receiving Receipt..." + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/receivingReceipt/add";

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