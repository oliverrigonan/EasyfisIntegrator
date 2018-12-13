using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

        // ==============
        // Get Collection
        // ==============
        public void GetCollection(String apiUrlHost, String branchCode, String userCode)
        {
            try
            {
                var collections = from d in posdb.TrnCollections where d.PostCode == null && d.CollectionNumber != "NA" && d.IsLocked == true select d;
                if (collections.Any())
                {
                    var collection = collections.FirstOrDefault();

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

                    if (collection.TrnSale != null)
                    {
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

                        var collectionData = new Entities.ISPOSTrnCollection()
                        {
                            SIDate = collection.CollectionDate.ToShortDateString(),
                            BranchCode = branchCode,
                            CustomerManualArticleCode = collection.TrnSale.MstCustomer.CustomerCode,
                            CreatedBy = userCode,
                            Term = collection.TrnSale.MstTerm.Term,
                            DocumentReference = collection.CollectionNumber,
                            ManualSINumber = collection.TrnSale.SalesNumber,
                            Remarks = "User: " + collection.MstUser4.UserName + ", " + String.Join(", ", payTypes),
                            ListPOSIntegrationTrnSalesInvoiceItem = listCollectionLines.ToList()
                        };

                        String json = new JavaScriptSerializer().Serialize(collectionData);

                        trnIntegrationForm.logMessages("Sending Collection: " + collectionData.DocumentReference + "\r\n\n");
                        trnIntegrationForm.logMessages("Amount: " + collectionData.ListPOSIntegrationTrnSalesInvoiceItem.Sum(d => d.Amount).ToString("#,##0.00") + "\r\n\n");
                        SendCollection(apiUrlHost, json);
                    }
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logMessages("Collection Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logMessages("\r\n\n");
            }

            trnIntegrationForm.logMessages("Collection Integration Done.");
        }

        // ===============
        // Send Collection
        // ===============
        public void SendCollection(String apiUrlHost, String json)
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
                // Process Response
                // ================
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result != null)
                    {
                        Entities.ISPOSTrnCollection collection = new JavaScriptSerializer().Deserialize<Entities.ISPOSTrnCollection>(json);
                        var currentCollection = from d in posdb.TrnCollections where d.CollectionNumber.Equals(collection.DocumentReference) select d;
                        if (currentCollection.Any())
                        {
                            var updateCollection = currentCollection.FirstOrDefault();
                            updateCollection.PostCode = result.Replace("\"", "");
                            posdb.SubmitChanges();
                        }

                        trnIntegrationForm.logMessages("Send Succesful!" + "\r\n\n");
                        trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logMessages("\r\n\n");
                    }
                }
            }
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                trnIntegrationForm.logMessages(resp.Replace("\"", "") + "\r\n\n");
                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logMessages("\r\n\n");
            }
        }
    }
}