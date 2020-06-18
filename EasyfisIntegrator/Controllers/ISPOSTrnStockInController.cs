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
    class ISPOSTrnStockInController
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
        public ISPOSTrnStockInController(Forms.TrnIntegrationForm form, String actDate)
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

        // =============
        // Sync Stock In
        // =============
        public async void SyncStockIn(String apiUrlHost, String branchCode)
        {
            await GetStockIn(apiUrlHost, branchCode);
        }

        // ============
        // Get Stock In
        // ============
        public Task GetStockIn(String apiUrlHost, String branchCode)
        {
            try
            {
                DateTime dateTimeToday = DateTime.Now;
                String stockInDate = Convert.ToDateTime(activityDate).ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);

                // ============
                // Http Request
                // ============
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + apiUrlHost + "/api/get/POSIntegration/stockIn/" + stockInDate + "/" + branchCode);
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
                    List<Entities.ISPOSTrnStockIn> stockInLists = (List<Entities.ISPOSTrnStockIn>)js.Deserialize(result, typeof(List<Entities.ISPOSTrnStockIn>));

                    if (stockInLists.Any())
                    {
                        foreach (var stockIn in stockInLists)
                        {
                            var currentStockIn = from d in posdb.TrnStockIns where d.Remarks.Equals("IN-" + stockIn.BranchCode + "-" + stockIn.INNumber) && d.TrnStockInLines.Count() > 0 && d.IsLocked == true select d;
                            if (!currentStockIn.Any())
                            {
                                trnIntegrationForm.salesIntegrationLogMessages("Saving Stock In: IN-" + stockIn.BranchCode + "-" + stockIn.INNumber + "\r\n\n");

                                var defaultPeriod = from d in posdb.MstPeriods select d;
                                var defaultSettings = from d in posdb.SysSettings select d;

                                var lastStockInNumber = from d in posdb.TrnStockIns.OrderByDescending(d => d.Id) select d;
                                var stockInNumberResult = defaultPeriod.FirstOrDefault().Period + "-000001";

                                if (lastStockInNumber.Any())
                                {
                                    var stockInNumberSplitStrings = lastStockInNumber.FirstOrDefault().StockInNumber;
                                    Int32 secondIndex = stockInNumberSplitStrings.IndexOf('-', stockInNumberSplitStrings.IndexOf('-'));
                                    var stockInNumberSplitStringValue = stockInNumberSplitStrings.Substring(secondIndex + 1);
                                    var stockInNumber = Convert.ToInt32(stockInNumberSplitStringValue) + 000001;
                                    stockInNumberResult = defaultPeriod.FirstOrDefault().Period + "-" + FillLeadingZeroes(stockInNumber, 6);
                                }

                                InnosoftPOSData.TrnStockIn newStockIn = new InnosoftPOSData.TrnStockIn
                                {
                                    PeriodId = defaultPeriod.FirstOrDefault().Id,
                                    StockInDate = Convert.ToDateTime(stockIn.INDate),
                                    StockInNumber = stockInNumberResult,
                                    SupplierId = defaultSettings.FirstOrDefault().PostSupplierId,
                                    Remarks = "IN-" + stockIn.BranchCode + "-" + stockIn.INNumber,
                                    IsReturn = false,
                                    PreparedBy = defaultSettings.FirstOrDefault().PostUserId,
                                    CheckedBy = defaultSettings.FirstOrDefault().PostUserId,
                                    ApprovedBy = defaultSettings.FirstOrDefault().PostUserId,
                                    IsLocked = true,
                                    EntryUserId = defaultSettings.FirstOrDefault().PostUserId,
                                    EntryDateTime = DateTime.Now,
                                    UpdateUserId = defaultSettings.FirstOrDefault().PostUserId,
                                    UpdateDateTime = DateTime.Now
                                };

                                posdb.TrnStockIns.InsertOnSubmit(newStockIn);
                                posdb.SubmitChanges();

                                if (stockIn.ListPOSIntegrationTrnStockInItem.Any())
                                {
                                    foreach (var item in stockIn.ListPOSIntegrationTrnStockInItem.ToList())
                                    {
                                        var currentItem = from d in posdb.MstItems where d.BarCode.Equals(item.ItemCode) && d.MstUnit.Unit.Equals(item.Unit) select d;
                                        if (currentItem.Any())
                                        {
                                            InnosoftPOSData.TrnStockInLine newStockInLine = new InnosoftPOSData.TrnStockInLine
                                            {
                                                StockInId = newStockIn.Id,
                                                ItemId = currentItem.FirstOrDefault().Id,
                                                UnitId = currentItem.FirstOrDefault().UnitId,
                                                Quantity = item.Quantity,
                                                Cost = item.Cost,
                                                Amount = item.Amount,
                                                ExpiryDate = currentItem.FirstOrDefault().ExpiryDate,
                                                LotNumber = currentItem.FirstOrDefault().LotNumber,
                                                AssetAccountId = currentItem.FirstOrDefault().AssetAccountId,
                                                Price = currentItem.FirstOrDefault().Price
                                            };

                                            posdb.TrnStockInLines.InsertOnSubmit(newStockInLine);

                                            var updateItem = currentItem.FirstOrDefault();
                                            updateItem.OnhandQuantity = currentItem.FirstOrDefault().OnhandQuantity + Convert.ToDecimal(item.Quantity);

                                            posdb.SubmitChanges();

                                            trnIntegrationForm.salesIntegrationLogMessages(" > " + currentItem.FirstOrDefault().ItemDescription + " * " + item.Quantity.ToString("#,##0.00") + "\r\n\n");
                                        }
                                    }
                                }

                                trnIntegrationForm.salesIntegrationLogMessages("Save Successful!" + "\r\n\n");
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
                trnIntegrationForm.salesIntegrationLogMessages("Stock-In Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");

                return Task.FromResult("");
            }
        }
    }
}