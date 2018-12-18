﻿using System;
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

        public FolderMonitoringTrnSalesInvoiceController(Forms.TrnIntegrationForm form, String userCode, String directory, String domain)
        {
            trnIntegrationForm = form;

            List<String> files = Directory.GetFiles(directory).ToList();
            if (files.Any()) { foreach (var file in files) { SendSalesInvoiceData(userCode, file, domain); } }
        }

        public void SendSalesInvoiceData(String userCode, String file, String domain)
        {
            try
            {
                String json = "";
                String[] lines = File.ReadAllLines(file);

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
                            UserCode = userCode,
                            CreatedDateTime = data[7],
                            ItemCode = data[8],
                            Particulars = data[9],
                            Unit = data[10],
                            Quantity = Convert.ToDecimal(data[11]),
                            Price = Convert.ToDecimal(data[12]),
                            Discount = data[13],
                            DiscountRate = Convert.ToDecimal(data[14]),
                            DiscountAmount = Convert.ToDecimal(data[15]),
                            NetPrice = Convert.ToDecimal(data[16]),
                            Amount = Convert.ToDecimal(data[17])
                        });
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    json = serializer.Serialize(newSalesInvoices);
                }

                trnIntegrationForm.logFolderMonitoringMessage("Sending Sales..." + "\r\n\n");

                String apiURL = "http://" + domain + "/api/folderMonitoring/salesInvoice/add";

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