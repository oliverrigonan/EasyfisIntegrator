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

        public Forms.TrnInnosoftPOSIntegrationForm trnInnosoftPOSIntegrationForm;

        // ===========
        // Constructor
        // ===========
        public ISPOSTrnSalesReturnController(Forms.TrnInnosoftPOSIntegrationForm form)
        {
            trnInnosoftPOSIntegrationForm = form;
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

                                var stockInLines = from d in posdb.TrnStockInLines where d.StockInId == stockIn.Id select d;
                                if (stockInLines.Any())
                                {
                                    List<Entities.ISPOSTrnCollectionLines> listCollectionLines = new List<Entities.ISPOSTrnCollectionLines>();
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

                                    var collectionData = new Entities.ISPOSTrnCollection()
                                    {
                                        SIDate = stockIn.StockInDate.ToShortDateString(),
                                        BranchCode = branchCode,
                                        CustomerManualArticleCode = stockIn.TrnCollection.TrnSale.MstCustomer.CustomerCode,
                                        CreatedBy = userCode,
                                        Term = terms.FirstOrDefault().Term,
                                        DocumentReference = stockIn.StockInNumber,
                                        ManualSINumber = stockIn.TrnCollection.TrnSale.SalesNumber,
                                        Remarks = "Return from Customer",
                                        ListPOSIntegrationTrnSalesInvoiceItem = listCollectionLines.ToList()
                                    };

                                    String json = new JavaScriptSerializer().Serialize(collectionData);

                                    trnInnosoftPOSIntegrationForm.logMessages("Sending Returned Sales: " + collectionData.DocumentReference + "\r\n\n");
                                    trnInnosoftPOSIntegrationForm.logMessages("Amount: " + collectionData.ListPOSIntegrationTrnSalesInvoiceItem.Sum(d => d.Amount).ToString("#,##0.00") + "\r\n\n");
                                    SendSalesReturn(apiUrlHost, json);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                trnInnosoftPOSIntegrationForm.logMessages("Sales Return Error: " + e.Message + "\r\n\n");
                trnInnosoftPOSIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnInnosoftPOSIntegrationForm.logMessages("\r\n\n");
            }

            trnInnosoftPOSIntegrationForm.logMessages("Sales Return Integration Done.");
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

                        trnInnosoftPOSIntegrationForm.logMessages("Send Succesful!" + "\r\n\n");
                        trnInnosoftPOSIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnInnosoftPOSIntegrationForm.logMessages("\r\n\n");
                    }
                }
            }
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();

                trnInnosoftPOSIntegrationForm.logMessages(resp.Replace("\"", "") + "\r\n\n");
                trnInnosoftPOSIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnInnosoftPOSIntegrationForm.logMessages("\r\n\n");
            }
        }
    }
}