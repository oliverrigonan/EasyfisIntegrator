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
    class ISPOSTrnItemPriceController
    {
        // ====
        // Data
        // ====
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(SysGlobalSettings.getConnectionString());

        public Forms.TrnIntegrationForm trnIntegrationForm;
        public String activityDate;

        // ===========
        // Constructor
        // ===========
        public ISPOSTrnItemPriceController(Forms.TrnIntegrationForm form, String actDate)
        {
            trnIntegrationForm = form;
            activityDate = actDate;
        }

        // ===============
        // Sync Item Price
        // ===============
        public async void SyncItemPrice(String apiUrlHost, String branchCode)
        {
            await GetItemPrice(apiUrlHost, branchCode);
        }

        // ==============
        // Get Item Price
        // ==============
        public Task GetItemPrice(String apiUrlHost, String branchCode)
        {
            try
            {
                DateTime dateTimeToday = DateTime.Now;
                String currentDate = Convert.ToDateTime(activityDate).ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);

                // ============
                // Http Request
                // ============
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + apiUrlHost + "/api/get/POSIntegration/itemPrice/" + branchCode + "/" + currentDate);
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";

                // ================
                // Process Response
                // ================
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    List<Entities.ISPOSTrnArticlePrice> itemPriceLists = (List<Entities.ISPOSTrnArticlePrice>)js.Deserialize(result, typeof(List<Entities.ISPOSTrnArticlePrice>));

                    if (itemPriceLists.Any())
                    {
                        foreach (var itemPrice in itemPriceLists)
                        {
                            var currentItem = from d in posdb.MstItems where d.BarCode.Equals(itemPrice.ItemCode) && d.IsLocked == true select d;
                            if (currentItem.Any())
                            {
                                var currentItemPrice = from d in posdb.MstItemPrices where d.ItemId == currentItem.FirstOrDefault().Id && d.PriceDescription.Equals("IP-" + itemPrice.BranchCode + "-" + itemPrice.IPNumber + " (" + itemPrice.IPDate + ")") select d;
                                if (!currentItemPrice.Any())
                                {
                                    trnIntegrationForm.salesIntegrationLogMessages("Saving Item Price: IP-" + itemPrice.BranchCode + "-" + itemPrice.IPNumber + " (" + itemPrice.IPDate + ")" + "\r\n\n");
                                    trnIntegrationForm.salesIntegrationLogMessages("Current Item: " + currentItem.FirstOrDefault().ItemDescription + "\r\n\n");

                                    InnosoftPOSData.MstItemPrice newPrice = new InnosoftPOSData.MstItemPrice()
                                    {
                                        ItemId = currentItem.FirstOrDefault().Id,
                                        PriceDescription = "IP-" + itemPrice.BranchCode + "-" + itemPrice.IPNumber + " (" + itemPrice.IPDate + ")",
                                        Price = itemPrice.Price,
                                        TriggerQuantity = itemPrice.TriggerQuantity
                                    };

                                    posdb.MstItemPrices.InsertOnSubmit(newPrice);

                                    var updateCurrentItem = currentItem.FirstOrDefault();
                                    updateCurrentItem.Price = itemPrice.Price;
                                    posdb.SubmitChanges();

                                    trnIntegrationForm.salesIntegrationLogMessages("Save Successful!" + "\r\n\n");
                                    trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");
                                }
                            }
                            else
                            {
                                trnIntegrationForm.salesIntegrationLogMessages("Cannot Save Item Price: IP-" + itemPrice.BranchCode + "-" + itemPrice.IPNumber + " (" + itemPrice.IPDate + ")" + "..." + "\r\n\n");
                                trnIntegrationForm.salesIntegrationLogMessages("Price: " + itemPrice.Price + "\r\n\n");
                                trnIntegrationForm.salesIntegrationLogMessages("Item Not Found!" + "\r\n\n");
                                trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");
                            }
                        }
                    }
                }

                return Task.FromResult("");
            }
            catch (Exception e)
            {
                trnIntegrationForm.salesIntegrationLogMessages("Item Price Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");

                return Task.FromResult("");
            }
        }
    }
}