using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        // =================
        // Sync Sales Return
        // =================
        public async void SyncSalesReturn(String apiUrlHost, String branchCode, String userCode)
        {
            await GetSalesReturn(apiUrlHost, branchCode, userCode);
        }

        // ================
        // Get Sales Return
        // ================
        public Task GetSalesReturn(String apiUrlHost, String branchCode, String userCode)
        {
            try
            {
                var stockIns = from d in posdb.TrnStockIns
                               where d.IsReturn == true
                               && d.CollectionId != null
                               && d.PostCode == null
                               && d.IsLocked == true
                               && d.TrnStockInLines.Any() == true
                               select d;

                if (stockIns.Any())
                {
                    var stockIn = stockIns.FirstOrDefault();
                    Int32 stockInId = stockIn.Id;

                    List<Entities.ISPOSTrnCollectionLines> listCollectionLines = new List<Entities.ISPOSTrnCollectionLines>();

                    var stockInLines = from d in stockIn.TrnStockInLines select d;
                    foreach (var stockInLine in stockInLines)
                    {
                        listCollectionLines.Add(new Entities.ISPOSTrnCollectionLines()
                        {
                            ItemManualArticleCode = stockInLine.MstItem.BarCode,
                            Particulars = stockInLine.MstItem.ItemDescription,
                            Unit = stockInLine.MstUnit.Unit,
                            Quantity = stockInLine.Quantity * -1,
                            Price = stockInLine.Cost,
                            Discount = "Zero Discount",
                            DiscountAmount = 0,
                            NetPrice = stockInLine.Cost,
                            Amount = (stockInLine.Quantity * -1) * stockInLine.Cost,
                            VAT = stockInLine.MstItem.MstTax.Tax,
                            SalesItemTimeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)
                        });
                    }

                    var collectionData = new Entities.ISPOSTrnCollection()
                    {
                        SIDate = stockIn.StockInDate.ToShortDateString(),
                        BranchCode = branchCode,
                        CustomerManualArticleCode = stockIn.TrnCollection.TrnSale.MstCustomer.CustomerCode,
                        CreatedBy = userCode,
                        Term = stockIn.TrnCollection.TrnSale.MstTerm.Term,
                        DocumentReference = stockIn.StockInNumber,
                        ManualSINumber = "IN-" + stockIn.StockInNumber,
                        Remarks = "Return from Customer, OR-" + stockIn.TrnCollection.CollectionNumber + ", SI-" + stockIn.TrnCollection.TrnSale.SalesNumber,
                        ListPOSIntegrationTrnSalesInvoiceItem = listCollectionLines.ToList()
                    };

                    String json = new JavaScriptSerializer().Serialize(collectionData);

                    trnIntegrationForm.salesIntegrationLogMessages("Sending Sales Return: " + collectionData.ManualSINumber + "\r\n\n");
                    trnIntegrationForm.salesIntegrationLogMessages("Amount: " + collectionData.ListPOSIntegrationTrnSalesInvoiceItem.Sum(d => d.Amount).ToString("#,##0.00") + "\r\n\n");
                    SendSalesReturn(apiUrlHost, json, stockInId);
                }

                return Task.FromResult("");
            }
            catch (Exception e)
            {
                trnIntegrationForm.salesIntegrationLogMessages("Sales Return Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");

                return Task.FromResult("");
            }
        }

        // =================
        // Send Sales Return
        // =================
        public void SendSalesReturn(String apiUrlHost, String json, Int32 stockInId)
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
                        var stockIns = from d in posdb.TrnStockIns
                                       where d.Id == stockInId
                                       select d;

                        if (stockIns.Any())
                        {
                            var stockIn = stockIns.FirstOrDefault();
                            stockIn.PostCode = result.Replace("\"", "");
                            posdb.SubmitChanges();
                        }

                        trnIntegrationForm.salesIntegrationLogMessages("Send Succesful!" + "\r\n\n");
                        trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");
                    }
                }
            }
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();

                trnIntegrationForm.salesIntegrationLogMessages(resp.Replace("\"", "") + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");
            }
        }
    }
}