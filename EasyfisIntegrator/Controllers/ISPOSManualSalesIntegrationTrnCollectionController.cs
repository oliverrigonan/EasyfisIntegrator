using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class ISPOSManualSalesIntegrationTrnCollectionController
    {
        // ====
        // Data
        // ====
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(SysGlobalSettings.getConnectionString());

        // ==================
        // Send Sales Invoice
        // ==================
        public async void SendSalesInvoice(Forms.TrnIntegrationForm trnIntegrationForm, String domain, String salesDate)
        {
            List<Entities.FolderMonitoringTrnSalesInvoice> newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonData = "";

            List<Int32> collectionIds = new List<Int32>();
            // ========================
            // Get Integration Settings
            // ========================
            var settings = from d in posdb.SysSettings select d;
            if (settings.Any())
            {
                String branchCode = settings.FirstOrDefault().BranchCode;
                String userCode = settings.FirstOrDefault().UserCode;

                // =============
                // Getting Sales
                // =============
                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\nGetting POS Sales... (0%) \r\n\n");
                while (true)
                {
                    newSalesInvoices = new List<Entities.FolderMonitoringTrnSalesInvoice>();

                    try
                    {
                        var collections = from d in posdb.TrnCollections
                                          where (d.CollectionNumber.Equals("NA") == false || d.CollectionNumber.Equals("na") == false)
                                          && d.SalesId != null
                                          && d.PostCode == null
                                          && d.IsLocked == true
                                          select d;

                        if (collections.Any())
                        {
                            foreach (var collection in collections)
                            {
                                if (collectionIds.Contains(collection.Id) == false)
                                {
                                    collectionIds.Add(collection.Id);
                                }
                            }

                            var groupedCollections = from d in collections
                                                     group d by d.CustomerId
                                                     into g
                                                     select g;

                            foreach (var groupedCollection in groupedCollections)
                            {
                                var salesLines = from d in posdb.TrnSalesLines
                                                 where d.TrnSale.CustomerId == groupedCollection.Key
                                                 select d;

                                if (salesLines.Any())
                                {
                                    String defaultManualSINumber = Convert.ToDateTime(salesDate).ToString("yyyyMMdd") + "_" + salesLines.FirstOrDefault().TrnSale.MstCustomer.CustomerCode;

                                    var groupedSalesLines = from d in salesLines
                                                            group d by new
                                                            {
                                                                d.TrnSale.MstCustomer.CustomerCode,
                                                                d.MstItem.BarCode,
                                                                d.MstItem.MstUnit.Unit,
                                                                d.MstItem.Price,
                                                                d.NetPrice,
                                                                d.DiscountAmount,
                                                            } into g
                                                            select new
                                                            {
                                                                g.Key.CustomerCode,
                                                                g.Key.BarCode,
                                                                g.Key.Unit,
                                                                Quantity = g.Sum(s => s.Quantity),
                                                                g.Key.Price,
                                                                g.Key.DiscountAmount,
                                                                g.Key.NetPrice,
                                                                Amount = g.Sum(s => s.Amount)
                                                            };

                                    if (groupedSalesLines.ToList().Any())
                                    {
                                        Int32 count = 0;

                                        foreach (var groupedSalesLine in groupedSalesLines.ToList())
                                        {
                                            count += 1;

                                            newSalesInvoices.Add(new Entities.FolderMonitoringTrnSalesInvoice
                                            {
                                                BranchCode = branchCode,
                                                SIDate = salesDate,
                                                CustomerCode = groupedSalesLine.CustomerCode,
                                                ManualSINumber = defaultManualSINumber,
                                                DocumentReference = defaultManualSINumber,
                                                Remarks = defaultManualSINumber,
                                                UserCode = userCode,
                                                CreatedDateTime = salesDate,
                                                ItemCode = groupedSalesLine.BarCode,
                                                Particulars = defaultManualSINumber,
                                                Unit = groupedSalesLine.Unit,
                                                Quantity = groupedSalesLine.Quantity,
                                                Price = groupedSalesLine.Price,
                                                DiscountAmount = groupedSalesLine.DiscountAmount,
                                                NetPrice = groupedSalesLine.NetPrice,
                                                Amount = groupedSalesLine.Amount,
                                                No = count
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        trnIntegrationForm.logManualSalesIntegrationMessage("ManualSIIntegrationLogOnce");

                        trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\nGetting POS Sales... (100%) \r\n\n");
                        trnIntegrationForm.logManualSalesIntegrationMessage("Get POS Sales Successful!" + "\r\n\n");
                        trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                        break;
                    }
                    catch (Exception e)
                    {
                        trnIntegrationForm.logManualSalesIntegrationMessage("Error: " + e.Message + "\r\n\n");
                        trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                        trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                        Thread.Sleep(5000);
                    }
                }

                if (newSalesInvoices.Any())
                {
                    // ========
                    // Cleaning
                    // ========
                    trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\nCleaning Temporary Sales... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            String deleteTemporarySalesInvoiceTask = await DeleteTemporarySalesInvoice(domain);
                            if (!deleteTemporarySalesInvoiceTask.Equals("Clean Successful..."))
                            {
                                trnIntegrationForm.logManualSalesIntegrationMessage(deleteTemporarySalesInvoiceTask);
                                trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                                Thread.Sleep(5000);
                            }
                            else
                            {
                                trnIntegrationForm.logManualSalesIntegrationMessage("ManualSIIntegrationLogOnce");

                                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\nCleaning Sales... (100%) \r\n\n");
                                trnIntegrationForm.logManualSalesIntegrationMessage("Clean Successful!" + "\r\n\n");
                                trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            trnIntegrationForm.logManualSalesIntegrationMessage("Error: " + e.Message + "\r\n\n");
                            trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                            Thread.Sleep(5000);
                        }
                    }

                    Boolean post = false;

                    // =======
                    // Sending
                    // =======
                    trnIntegrationForm.logManualSalesIntegrationMessage("Sending Sales... (0%) \r\n\n");
                    while (true)
                    {
                        try
                        {
                            Decimal percentage = 0;

                            Boolean send = false;
                            Int32 skip = 0;

                            for (Int32 i = 1; i <= newSalesInvoices.Count(); i++)
                            {
                                if (i % 100 == 0)
                                {
                                    jsonData = serializer.Serialize(newSalesInvoices.Skip(skip).Take(100));
                                    skip = i;

                                    send = true;
                                    percentage = Convert.ToDecimal((Convert.ToDecimal(skip) / Convert.ToDecimal(newSalesInvoices.Count())) * 100);
                                }
                                else
                                {
                                    if (i == newSalesInvoices.Count())
                                    {
                                        if (newSalesInvoices.Count() <= 100)
                                        {
                                            jsonData = serializer.Serialize(newSalesInvoices);
                                        }
                                        else
                                        {
                                            jsonData = serializer.Serialize(newSalesInvoices.Skip(skip).Take(i - skip));
                                        }

                                        send = true;
                                        percentage = Convert.ToDecimal((Convert.ToDecimal(i) / Convert.ToDecimal(newSalesInvoices.Count())) * 100);
                                    }
                                }

                                if (send)
                                {
                                    while (true)
                                    {
                                        try
                                        {
                                            String insertTemporarySalesInvoiceTask = await InsertTemporarySalesInvoice(domain, jsonData);
                                            if (!insertTemporarySalesInvoiceTask.Equals("Send Successful..."))
                                            {
                                                trnIntegrationForm.logManualSalesIntegrationMessage(insertTemporarySalesInvoiceTask);
                                                trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                            else
                                            {
                                                trnIntegrationForm.logManualSalesIntegrationMessage("ManualSIIntegrationLogOnce");
                                                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\nSending Sales... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                if (i == newSalesInvoices.Count())
                                                {
                                                    trnIntegrationForm.logManualSalesIntegrationMessage("Send Successful!" + "\r\n\n");
                                                    trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");
                                                }

                                                break;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            trnIntegrationForm.logManualSalesIntegrationMessage("Sending Error: " + e.Message + "\r\n\n");
                                            trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                                            Thread.Sleep(5000);
                                        }
                                    }

                                    send = false;
                                }
                            }

                            post = true;
                            break;
                        }
                        catch (Exception e)
                        {
                            trnIntegrationForm.logManualSalesIntegrationMessage("Error: " + e.Message + "\r\n\n");
                            trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                            trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                            Thread.Sleep(5000);
                        }
                    }

                    // =======
                    // Posting
                    // =======
                    if (post)
                    {
                        trnIntegrationForm.logManualSalesIntegrationMessage("Posting Sales... (0%) \r\n\n");
                        while (true)
                        {
                            try
                            {
                                var groupedSalesInvoices = from d in newSalesInvoices
                                                           group d by new
                                                           {
                                                               d.BranchCode,
                                                               d.ManualSINumber
                                                           } into g
                                                           select g.Key;

                                var salesInvoices = from d in groupedSalesInvoices.ToList() select d;
                                if (salesInvoices.Any())
                                {
                                    Decimal percentage = 0;
                                    Int32 count = 0;

                                    foreach (var salesInvoice in salesInvoices.ToList())
                                    {
                                        count += 1;
                                        percentage = Convert.ToDecimal((Convert.ToDecimal(count) / Convert.ToDecimal(salesInvoices.Count())) * 100);

                                        while (true)
                                        {
                                            try
                                            {
                                                String postSalesInvoiceTask = await PostSalesInvoice(domain, salesInvoice.BranchCode, salesInvoice.ManualSINumber);
                                                if (!postSalesInvoiceTask.Equals("Post Successful..."))
                                                {
                                                    trnIntegrationForm.logManualSalesIntegrationMessage(postSalesInvoiceTask);
                                                    trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                    trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                                                    Thread.Sleep(5000);
                                                }
                                                else
                                                {
                                                    trnIntegrationForm.logManualSalesIntegrationMessage("ManualSIIntegrationLogOnce");
                                                    trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\nPosting Sales... (" + Math.Round(percentage, 2) + "%) \r\n\n");

                                                    if (count == salesInvoices.Count())
                                                    {
                                                        trnIntegrationForm.logManualSalesIntegrationMessage("Post Successful!" + "\r\n\n");
                                                        trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                        trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");
                                                    }

                                                    break;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                trnIntegrationForm.logManualSalesIntegrationMessage("Posting Error: " + e.Message + "\r\n\n");
                                                trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                                                Thread.Sleep(5000);
                                            }
                                        }
                                    }
                                }

                                if (collectionIds.Any())
                                {
                                    foreach (var collectionId in collectionIds)
                                    {
                                        var collection = from d in posdb.TrnCollections
                                                         where d.Id == collectionId
                                                         select d;

                                        if (collection.Any())
                                        {
                                            String defaultManualSINumber = Convert.ToDateTime(salesDate).ToString("yyyyMMdd") + "_" + collection.FirstOrDefault().MstCustomer.CustomerCode;

                                            var updateCollectionPostCode = collection.FirstOrDefault();
                                            updateCollectionPostCode.PostCode = defaultManualSINumber;

                                            posdb.SubmitChanges();
                                        }
                                    }
                                }

                                break;
                            }
                            catch (Exception e)
                            {
                                trnIntegrationForm.logManualSalesIntegrationMessage("Error: " + e.Message + "\r\n\n");
                                trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");

                                Thread.Sleep(5000);
                            }
                        }
                    }
                }
                else
                {
                    trnIntegrationForm.logManualSalesIntegrationMessage("Data Source Empty \r\n\n");
                    trnIntegrationForm.logManualSalesIntegrationMessage("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                    trnIntegrationForm.logManualSalesIntegrationMessage("\r\n\n");
                }
            }
        }

        // ==============================
        // Delete Temporary Sales Invoice
        // ==============================
        public Task<String> DeleteTemporarySalesInvoice(String domain)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/POSManualSalesIntegration/salesInvoice/temporary/delete";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "DELETE";
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { streamWriter.Write(""); }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        return Task.FromResult("Clean Successful...");
                    }
                    else
                    {
                        return Task.FromResult("Clean Failed! " + resp + "\r\n\n");
                    }
                }
            }
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
            }
        }

        // ==============================
        // Insert Temporary Sales Invoice
        // ==============================
        public Task<String> InsertTemporarySalesInvoice(String domain, String json)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/POSManualSalesIntegration/salesInvoice/temporary/insert";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { streamWriter.Write(json); }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        return Task.FromResult("Send Successful...");
                    }
                    else
                    {
                        return Task.FromResult("Send Failed! " + resp + "\r\n\n");
                    }
                }
            }
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
            }
        }

        // ==================
        // Post Sales Invoice
        // ==================
        public Task<String> PostSalesInvoice(String domain, String branchCode, String manualSINumber)
        {
            try
            {
                String apiURL = "http://" + domain + "/api/POSManualSalesIntegration/salesInvoice/post";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Entities.FolderMonitoringTrnSalesInvoice jsonSalesInvoice = new Entities.FolderMonitoringTrnSalesInvoice()
                {
                    BranchCode = branchCode,
                    ManualSINumber = manualSINumber
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String json = serializer.Serialize(jsonSalesInvoice);

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { streamWriter.Write(json); }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String resp = streamReader.ReadToEnd().Replace("\"", "");
                    if (resp.Equals(""))
                    {
                        return Task.FromResult("Post Successful...");
                    }
                    else
                    {
                        return Task.FromResult("Post Failed! " + resp + "\r\n\n");
                    }
                }
            }
            catch (WebException we)
            {
                var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return Task.FromResult("Web Exception Error: " + resp.Replace("\"", "") + "\r\n\n");
            }
        }
    }
}
