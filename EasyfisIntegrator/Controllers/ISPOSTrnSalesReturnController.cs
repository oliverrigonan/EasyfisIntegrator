using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class ISPOSTrnSalesReturnController
    {
        // ====
        // Data
        // ====
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(SysGlobalSettings.getConnectionString());

        public Forms.TrnIntegrationForm trnIntegrationForm;

        // ===========
        // Constructor
        // ===========
        public ISPOSTrnSalesReturnController(Forms.TrnIntegrationForm form)
        {
            trnIntegrationForm = form;
        }

        // ================
        // Get Sales Return
        // ================
        public void GetSalesReturn(String apiUrlHost, String branchCode, String userCode)
        {
            try
            {
                var discounts = from d in posdb.MstDiscounts select d;
                if (discounts.Any())
                {
                    var taxes = from d in posdb.MstTaxes select d;
                    if (taxes.Any())
                    {
                        var terms = from d in posdb.MstTerms select d;
                        if (terms.Any())
                        {
                            var stockIns = from d in posdb.TrnStockIns where d.IsReturn == true && d.CollectionId != null && d.PostCode == null && d.IsLocked == true select d;
                            if (stockIns.Any())
                            {
                                var stockIn = stockIns.FirstOrDefault();

                                List<Entities.ISPOSTrnCollectionLines> listCollectionLines = new List<Entities.ISPOSTrnCollectionLines>();

                                var stockInLines = from d in posdb.TrnStockInLines where d.StockInId == stockIn.Id select d;
                                if (stockInLines.Any())
                                {
                                    foreach (var stockInLine in stockInLines)
                                    {
                                        listCollectionLines.Add(new Entities.ISPOSTrnCollectionLines()
                                        {
                                            ItemManualArticleCode = stockInLine.MstItem.BarCode,
                                            Particulars = stockInLine.MstItem.ItemDescription,
                                            Unit = stockInLine.MstUnit.Unit,
                                            Quantity = stockInLine.Quantity * -1,
                                            Price = stockInLine.Cost * -1,
                                            Discount = discounts.FirstOrDefault().Discount,
                                            DiscountAmount = 0,
                                            NetPrice = (stockInLine.Cost * -1),
                                            Amount = ((stockInLine.Quantity * -1) * (stockInLine.Cost * -1)) * -1,
                                            VAT = taxes.FirstOrDefault().Tax,
                                            SalesItemTimeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                        });
                                    }
                                }

                                var collectionData = new Entities.ISPOSTrnCollection()
                                {
                                    SIDate = stockIn.StockInDate.ToShortDateString(),
                                    BranchCode = branchCode,
                                    CustomerManualArticleCode = stockIn.TrnCollection.TrnSale.MstCustomer.CustomerCode,
                                    CreatedBy = userCode,
                                    Term = terms.FirstOrDefault().Term,
                                    DocumentReference = stockIn.StockInNumber,
                                    ManualSINumber = stockIn.TrnCollection.TrnSale.SalesNumber + "-RT",
                                    Remarks = "Return from Customer",
                                    ListPOSIntegrationTrnSalesInvoiceItem = listCollectionLines.ToList()
                                };

                                String json = new JavaScriptSerializer().Serialize(collectionData);

                                trnIntegrationForm.logMessages("Sending Returned Sales: " + collectionData.DocumentReference + "\r\n\n");
                                trnIntegrationForm.logMessages("Amount: " + collectionData.ListPOSIntegrationTrnSalesInvoiceItem.Sum(d => d.Amount).ToString("#,##0.00") + "\r\n\n");
                                SendSalesReturn(apiUrlHost, json);
                            }
                            else
                            {
                                trnIntegrationForm.logMessages("Sales Return Integration Done.");
                            }
                        }
                        else
                        {
                            trnIntegrationForm.logMessages("Sales Return Integration Done.");
                        }
                    }
                    else
                    {
                        trnIntegrationForm.logMessages("Sales Return Integration Done.");
                    }
                }
                else
                {
                    trnIntegrationForm.logMessages("Sales Return Integration Done.");
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logMessages("Sales Return Integration Done.");

                trnIntegrationForm.logMessages("Sales Return Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logMessages("\r\n\n");
            }
        }

        // =================
        // Send Sales Return
        // =================
        public void SendSalesReturn(String apiUrlHost, String json)
        {
            try
            {
                // ============
                // Http Request
                // ============
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + apiUrlHost + "/api/add/POSIntegration/salesInvoice");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                // ====
                // Data
                // ====
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    Entities.ISPOSTrnCollection collection = new JavaScriptSerializer().Deserialize<Entities.ISPOSTrnCollection>(json);
                    streamWriter.Write(new JavaScriptSerializer().Serialize(collection));
                }

                Boolean isRead = false;

                // ================
                // Process response
                // ================
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result != null)
                    {
                        Entities.ISPOSTrnCollection collection = new JavaScriptSerializer().Deserialize<Entities.ISPOSTrnCollection>(json);
                        var stockIns = from d in posdb.TrnStockIns where d.StockInNumber.Equals(collection.DocumentReference) select d;
                        if (stockIns.Any())
                        {
                            var stockIn = stockIns.FirstOrDefault();
                            stockIn.PostCode = result.Replace("\"", "");
                            posdb.SubmitChanges();
                        }

                        trnIntegrationForm.logMessages("Send Succesful!" + "\r\n\n");
                        trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logMessages("\r\n\n");
                    }

                    isRead = true;
                }

                if (isRead)
                {
                    trnIntegrationForm.logMessages("Sales Return Integration Done.");
                }
            }
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();

                trnIntegrationForm.logMessages("Sales Return Integration Done.");

                trnIntegrationForm.logMessages(resp.Replace("\"", "") + "\r\n\n");
                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logMessages("\r\n\n");
            }
        }
    }
}