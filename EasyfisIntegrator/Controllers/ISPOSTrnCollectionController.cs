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
    class ISPOSTrnCollectionController
    {
        // ====
        // Data
        // ====
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(SysGlobalSettings.getConnectionString());
        public Forms.TrnIntegrationForm trnIntegrationForm;

        // ===========
        // Constructor
        // ===========
        public ISPOSTrnCollectionController(Forms.TrnIntegrationForm form)
        {
            trnIntegrationForm = form;
        }

        // ==========
        // Sync Sales
        // ==========
        public async void SyncCollection(String apiUrlHost, String branchCode, String userCode)
        {
            await GetCollection(apiUrlHost, branchCode, userCode);
        }

        // ==============
        // Get Collection
        // ==============
        public Task GetCollection(String apiUrlHost, String branchCode, String userCode)
        {
            try
            {
                var collections = from d in posdb.TrnCollections
                                  where (d.CollectionNumber != "NA" || d.CollectionNumber != "na")
                                  && d.SalesId != null
                                  && d.PostCode == null
                                  && d.IsLocked == true
                                  select d;

                if (collections.Any())
                {
                    var collection = collections.FirstOrDefault();
                    Int32 collectionId = collection.Id;

                    var listPayTypes = new List<String>();
                    if (collection.TrnCollectionLines.Any())
                    {
                        foreach (var collectionLine in collection.TrnCollectionLines)
                        {
                            if (collectionLine.Amount > 0)
                            {
                                listPayTypes.Add(collectionLine.MstPayType.PayType + ": " + collectionLine.Amount.ToString("#,##0.00"));
                            }
                        }
                    }

                    String[] payTypes = listPayTypes.ToArray();
                    List<Entities.ISPOSTrnCollectionLines> listCollectionLines = new List<Entities.ISPOSTrnCollectionLines>();

                    foreach (var salesLine in collection.TrnSale.TrnSalesLines)
                    {
                        listCollectionLines.Add(new Entities.ISPOSTrnCollectionLines()
                        {
                            ItemManualArticleCode = salesLine.MstItem.BarCode,
                            Particulars = salesLine.MstItem.ItemDescription,
                            Unit = salesLine.MstUnit.Unit,
                            Quantity = salesLine.Quantity,
                            Price = salesLine.Price,
                            Discount = salesLine.MstDiscount.Discount,
                            DiscountAmount = salesLine.DiscountAmount,
                            NetPrice = salesLine.NetPrice,
                            Amount = salesLine.Amount,
                            VAT = salesLine.MstTax.Tax,
                            SalesItemTimeStamp = salesLine.SalesLineTimeStamp.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)
                        });
                    }

                    var salesAgent = from d in posdb.MstUsers
                                     where d.Id == collection.TrnSale.SalesAgent
                                     select d;

                    var collectionData = new Entities.ISPOSTrnCollection()
                    {
                        SIDate = collection.CollectionDate.ToShortDateString(),
                        BranchCode = branchCode,
                        CustomerManualArticleCode = collection.TrnSale.MstCustomer.CustomerCode,
                        CreatedBy = userCode,
                        Term = collection.TrnSale.MstTerm.Term,
                        DocumentReference = collection.CollectionNumber,
                        ManualSINumber = collection.MstTerminal.Terminal + "-" + collection.TrnSale.SalesNumber,
                        Remarks = "User: " + collection.MstUser3.UserName + ", " + String.Join(", ", payTypes),
                        SalesAgentUserName = salesAgent.FirstOrDefault().UserName,
                        ListPOSIntegrationTrnSalesInvoiceItem = listCollectionLines.ToList()
                    };

                    String json = new JavaScriptSerializer().Serialize(collectionData);

                    trnIntegrationForm.salesIntegrationLogMessages("Sending Collection: " + collectionData.DocumentReference + "\r\n\n");
                    trnIntegrationForm.salesIntegrationLogMessages("Amount: " + collectionData.ListPOSIntegrationTrnSalesInvoiceItem.Sum(d => d.Amount).ToString("#,##0.00") + "\r\n\n");
                    SendCollection(apiUrlHost, json, collectionId);
                }

                return Task.FromResult("");
            }
            catch (Exception e)
            {
                trnIntegrationForm.salesIntegrationLogMessages("Collection Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");

                return Task.FromResult("");
            }
        }

        // ===============
        // Send Collection
        // ===============
        public void SendCollection(String apiUrlHost, String json, Int32 collectionId)
        {
            try
            {
                // ============
                // Http Request
                // ============
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrlHost + "/api/add/POSIntegration/salesInvoice");
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
                // Process Response
                // ================
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result != null)
                    {
                        var currentCollection = from d in posdb.TrnCollections
                                                where d.Id == collectionId
                                                select d;

                        if (currentCollection.Any())
                        {
                            var updateCollection = currentCollection.FirstOrDefault();
                            updateCollection.PostCode = result.Replace("\"", "");
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