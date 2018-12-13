using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class ISPOSMstItemController
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
        public ISPOSMstItemController(Forms.TrnIntegrationForm form, String actDate)
        {
            trnIntegrationForm = form;
            activityDate = actDate;
        }

        // ===================
        // Fill Leading Zeroes
        // ===================
        public String FillLeadingZeroes(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = '0' + result;
                pad--;
            }

            return result;
        }

        // ========
        // Get Item
        // ========
        public void GetItem(String apiUrlHost)
        {
            try
            {
                DateTime dateTimeToday = DateTime.Now;
                String currentDate = Convert.ToDateTime(activityDate).ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);

                // ============
                // Http Request
                // ============
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + apiUrlHost + "/api/get/POSIntegration/item/" + currentDate);
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
                    List<Entities.ISPOSMstItem> itemLists = (List<Entities.ISPOSMstItem>)js.Deserialize(result, typeof(List<Entities.ISPOSMstItem>));

                    if (itemLists.Any())
                    {
                        foreach (var item in itemLists)
                        {
                            var units = from d in posdb.MstUnits where d.Unit.Equals(item.Unit) select d;
                            if (units.Any())
                            {
                                var taxes = from d in posdb.MstTaxes where d.Tax.Equals(item.OutputTax) select d;
                                if (taxes.Any())
                                {
                                    var supplier = from d in posdb.MstSuppliers select d;
                                    if (supplier.Any())
                                    {
                                        var defaultSettings = from d in posdb.SysSettings select d;

                                        var currentItem = from d in posdb.MstItems where d.BarCode.Equals(item.ManualArticleCode) && d.IsLocked == true select d;
                                        if (currentItem.Any())
                                        {
                                            Boolean foundChanges = false;

                                            if (!foundChanges)
                                            {
                                                if (!currentItem.FirstOrDefault().BarCode.Equals(item.ManualArticleCode))
                                                {
                                                    foundChanges = true;
                                                }
                                            }

                                            if (!foundChanges)
                                            {
                                                if (!currentItem.FirstOrDefault().ItemDescription.Equals(item.Article))
                                                {
                                                    foundChanges = true;
                                                }
                                            }

                                            if (!foundChanges)
                                            {
                                                if (!currentItem.FirstOrDefault().Category.Equals(item.Category))
                                                {
                                                    foundChanges = true;
                                                }
                                            }

                                            if (!foundChanges)
                                            {
                                                if (!currentItem.FirstOrDefault().MstUnit.Unit.Equals(item.Unit))
                                                {
                                                    foundChanges = true;
                                                }
                                            }

                                            if (!defaultSettings.FirstOrDefault().UseItemPrice)
                                            {
                                                if (!foundChanges)
                                                {
                                                    if (currentItem.FirstOrDefault().Price != item.Price)
                                                    {
                                                        foundChanges = true;
                                                    }
                                                }
                                            }

                                            if (!foundChanges)
                                            {
                                                if (currentItem.FirstOrDefault().Cost != item.Cost)
                                                {
                                                    foundChanges = true;
                                                }
                                            }

                                            if (!foundChanges)
                                            {
                                                if (currentItem.FirstOrDefault().IsInventory != item.IsInventory)
                                                {
                                                    foundChanges = true;
                                                }
                                            }

                                            if (!foundChanges)
                                            {
                                                if (currentItem.FirstOrDefault().Remarks != null)
                                                {
                                                    if (!currentItem.FirstOrDefault().Remarks.Equals(item.Particulars))
                                                    {
                                                        foundChanges = true;
                                                    }
                                                }
                                            }

                                            if (!foundChanges)
                                            {
                                                if (currentItem.FirstOrDefault().OutTaxId != taxes.FirstOrDefault().Id)
                                                {
                                                    foundChanges = true;
                                                }
                                            }

                                            if (!defaultSettings.FirstOrDefault().UseItemPrice)
                                            {
                                                if (!foundChanges)
                                                {
                                                    if (item.ListItemPrice.Any())
                                                    {
                                                        var posItemPrices = from d in posdb.MstItemPrices where d.MstItem.BarCode.Equals(item.ManualArticleCode) select d;
                                                        if (posItemPrices.Any())
                                                        {
                                                            int posItemPriceCount = posItemPrices.Count();
                                                            int itemPriceListCount = item.ListItemPrice.Count();

                                                            if (posItemPriceCount != itemPriceListCount)
                                                            {
                                                                foundChanges = true;
                                                            }
                                                            else
                                                            {
                                                                foreach (var itemPrice in item.ListItemPrice.ToList())
                                                                {
                                                                    var currentPOSItemPrices = from d in posItemPrices where d.PriceDescription.Equals(itemPrice.PriceDescription) && d.Price == itemPrice.Price select d;
                                                                    if (!currentPOSItemPrices.Any())
                                                                    {
                                                                        foundChanges = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }

                                            if (foundChanges)
                                            {
                                                trnIntegrationForm.logMessages("Updating Item: " + currentItem.FirstOrDefault().ItemDescription + "\r\n\n");
                                                trnIntegrationForm.logMessages("Barcode: " + currentItem.FirstOrDefault().BarCode + "\r\n\n");

                                                var updateItem = currentItem.FirstOrDefault();
                                                updateItem.BarCode = item.ManualArticleCode;
                                                updateItem.ItemDescription = item.Article;
                                                updateItem.Alias = item.Article;
                                                updateItem.GenericName = item.Article;
                                                updateItem.Category = item.Category;
                                                updateItem.UnitId = units.FirstOrDefault().Id;

                                                if (!defaultSettings.FirstOrDefault().UseItemPrice)
                                                {
                                                    updateItem.Price = item.Price;
                                                }

                                                updateItem.Cost = item.Cost;
                                                updateItem.IsInventory = item.IsInventory;
                                                updateItem.Remarks = item.Particulars;
                                                updateItem.OutTaxId = taxes.FirstOrDefault().Id;
                                                updateItem.UpdateUserId = defaultSettings.FirstOrDefault().PostUserId;
                                                updateItem.UpdateDateTime = DateTime.Now;
                                                posdb.SubmitChanges();

                                                if (!defaultSettings.FirstOrDefault().UseItemPrice)
                                                {
                                                    if (item.ListItemPrice.Any())
                                                    {
                                                        var posItemPrices = from d in posdb.MstItemPrices where d.ItemId == currentItem.FirstOrDefault().Id select d;

                                                        bool isEmpty = false;
                                                        if (posItemPrices.Any())
                                                        {
                                                            posdb.MstItemPrices.DeleteAllOnSubmit(posItemPrices);
                                                            posdb.SubmitChanges();

                                                            isEmpty = true;
                                                        }

                                                        if (isEmpty)
                                                        {
                                                            foreach (var itemPrice in item.ListItemPrice.ToList())
                                                            {
                                                                InnosoftPOSData.MstItemPrice newItemPrice = new InnosoftPOSData.MstItemPrice
                                                                {
                                                                    ItemId = currentItem.FirstOrDefault().Id,
                                                                    PriceDescription = itemPrice.PriceDescription,
                                                                    Price = itemPrice.Price,
                                                                    TriggerQuantity = 0
                                                                };

                                                                posdb.MstItemPrices.InsertOnSubmit(newItemPrice);
                                                            }

                                                            posdb.SubmitChanges();
                                                        }
                                                    }
                                                }

                                                trnIntegrationForm.logMessages("Update Successful!" + "\r\n\n");
                                                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                                trnIntegrationForm.logMessages("\r\n\n");
                                            }
                                        }
                                        else
                                        {
                                            trnIntegrationForm.logMessages("Saving Item: " + item.Article + "\r\n\n");
                                            trnIntegrationForm.logMessages("Barcode: " + item.ManualArticleCode + "\r\n\n");

                                            var defaultItemCode = "000001";
                                            var lastItem = from d in posdb.MstItems.OrderByDescending(d => d.Id) select d;
                                            if (lastItem.Any())
                                            {
                                                var OTNumber = Convert.ToInt32(lastItem.FirstOrDefault().ItemCode) + 000001;
                                                defaultItemCode = FillLeadingZeroes(OTNumber, 6);
                                            }

                                            InnosoftPOSData.MstItem newItem = new InnosoftPOSData.MstItem
                                            {
                                                ItemCode = defaultItemCode,
                                                BarCode = item.ManualArticleCode,
                                                ItemDescription = item.Article,
                                                Alias = item.Article,
                                                GenericName = item.Article,
                                                Category = item.Category,
                                                SalesAccountId = 159,
                                                AssetAccountId = 74,
                                                CostAccountId = 238,
                                                InTaxId = 4,
                                                OutTaxId = taxes.FirstOrDefault().Id,
                                                UnitId = units.FirstOrDefault().Id,
                                                DefaultSupplierId = supplier.FirstOrDefault().Id,
                                                Cost = item.Cost,
                                                MarkUp = 0,
                                                Price = item.Price,
                                                ImagePath = "NA",
                                                ReorderQuantity = 0,
                                                OnhandQuantity = 0,
                                                IsInventory = item.IsInventory,
                                                ExpiryDate = null,
                                                LotNumber = " ",
                                                Remarks = item.Particulars,
                                                EntryUserId = defaultSettings.FirstOrDefault().PostUserId,
                                                EntryDateTime = DateTime.Now,
                                                UpdateUserId = defaultSettings.FirstOrDefault().PostUserId,
                                                UpdateDateTime = DateTime.Now,
                                                IsLocked = true,
                                                DefaultKitchenReport = " ",
                                                IsPackage = false
                                            };

                                            posdb.MstItems.InsertOnSubmit(newItem);
                                            posdb.SubmitChanges();

                                            if (item.ListItemPrice.Any())
                                            {
                                                foreach (var itemPrice in item.ListItemPrice.ToList())
                                                {
                                                    InnosoftPOSData.MstItemPrice newItemPrice = new InnosoftPOSData.MstItemPrice
                                                    {
                                                        ItemId = newItem.Id,
                                                        PriceDescription = itemPrice.PriceDescription,
                                                        Price = itemPrice.Price,
                                                        TriggerQuantity = 0
                                                    };

                                                    posdb.MstItemPrices.InsertOnSubmit(newItemPrice);
                                                }

                                                posdb.SubmitChanges();
                                            }

                                            trnIntegrationForm.logMessages("Save Successful!" + "\r\n\n");
                                            trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                            trnIntegrationForm.logMessages("\r\n\n");
                                        }
                                    }
                                    else
                                    {
                                        trnIntegrationForm.logMessages("Cannot Save Item: " + item.Article + "\r\n\n");
                                        trnIntegrationForm.logMessages("Empty Supplier!" + "\r\n\n");
                                        trnIntegrationForm.logMessages("Save Failed!" + "\r\n\n");
                                        trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logMessages("\r\n\n");
                                    }
                                }
                                else
                                {
                                    trnIntegrationForm.logMessages("Cannot Save Item: " + item.Article + "\r\n\n");
                                    trnIntegrationForm.logMessages("Output Tax Mismatch!" + "\r\n\n");
                                    trnIntegrationForm.logMessages("Save Failed!" + "\r\n\n");
                                    trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logMessages("\r\n\n");
                                }
                            }
                            else
                            {
                                trnIntegrationForm.logMessages("Cannot Save Item: " + item.Article + "\r\n\n");
                                trnIntegrationForm.logMessages("Unit Mismatch!" + "\r\n\n");
                                trnIntegrationForm.logMessages("Save Failed!" + "\r\n\n");
                                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                trnIntegrationForm.logMessages("\r\n\n");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logMessages("Item Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logMessages("\r\n\n");
            }

            trnIntegrationForm.logMessages("Item Integration Done.");
        }
    }
}