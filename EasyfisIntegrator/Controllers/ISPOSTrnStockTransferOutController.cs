using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class ISPOSTrnStockTransferOutController
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
        public ISPOSTrnStockTransferOutController(Forms.TrnIntegrationForm form, String actDate)
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

        // =======================
        // Get Stock Transfer - OT
        // =======================
        public void GetStockTransferOT(String apiUrlHost, String fromBranchCode)
        {
            try
            {
                DateTime dateTimeToday = DateTime.Now;
                String stockTransferDate = Convert.ToDateTime(activityDate).ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);

                // ============
                // Http Request
                // ============
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + apiUrlHost + "/api/get/POSIntegration/stockTransferItems/OT/" + stockTransferDate + "/" + fromBranchCode);
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";

                Boolean isRead = false;

                // ================
                // Process Response
                // ================
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    List<Entities.ISPOSTrnStockTransfer> stockTransferLists = (List<Entities.ISPOSTrnStockTransfer>)js.Deserialize(result, typeof(List<Entities.ISPOSTrnStockTransfer>));

                    if (stockTransferLists.Any())
                    {
                        foreach (var stockTransfer in stockTransferLists)
                        {
                            var currentStockOut = from d in posdb.TrnStockOuts where d.Remarks.Equals("ST-" + stockTransfer.BranchCode + "-" + stockTransfer.STNumber) && d.TrnStockOutLines.Count() > 0 && d.IsLocked == true select d;
                            if (!currentStockOut.Any())
                            {
                                trnIntegrationForm.logMessages("Saving Stock Transfer (OT): ST-" + stockTransfer.BranchCode + "-" + stockTransfer.STNumber + "\r\n\n");

                                var defaultPeriod = from d in posdb.MstPeriods select d;
                                var defaultSettings = from d in posdb.SysSettings select d;

                                var lastStockOutNumber = from d in posdb.TrnStockOuts.OrderByDescending(d => d.Id) select d;
                                var stockOutNumberResult = defaultPeriod.FirstOrDefault().Period + "-000001";

                                if (lastStockOutNumber.Any())
                                {
                                    var stockOutNumberSplitStrings = lastStockOutNumber.FirstOrDefault().StockOutNumber;
                                    Int32 secondIndex = stockOutNumberSplitStrings.IndexOf('-', stockOutNumberSplitStrings.IndexOf('-'));
                                    var stockOutNumberSplitStringValue = stockOutNumberSplitStrings.Substring(secondIndex + 1);
                                    var stockOutNumber = Convert.ToInt32(stockOutNumberSplitStringValue) + 000001;
                                    stockOutNumberResult = defaultPeriod.FirstOrDefault().Period + "-" + FillLeadingZeroes(stockOutNumber, 6);
                                }

                                var accounts = from d in posdb.MstAccounts
                                               select d;

                                if (accounts.Any())
                                {
                                    InnosoftPOSData.TrnStockOut newStockOut = new InnosoftPOSData.TrnStockOut
                                    {
                                        PeriodId = defaultPeriod.FirstOrDefault().Id,
                                        StockOutDate = Convert.ToDateTime(stockTransfer.STDate),
                                        StockOutNumber = stockOutNumberResult,
                                        AccountId = accounts.FirstOrDefault().Id,
                                        Remarks = "ST-" + stockTransfer.BranchCode + "-" + stockTransfer.STNumber,
                                        PreparedBy = defaultSettings.FirstOrDefault().PostUserId,
                                        CheckedBy = defaultSettings.FirstOrDefault().PostUserId,
                                        ApprovedBy = defaultSettings.FirstOrDefault().PostUserId,
                                        IsLocked = true,
                                        EntryUserId = defaultSettings.FirstOrDefault().PostUserId,
                                        EntryDateTime = DateTime.Now,
                                        UpdateUserId = defaultSettings.FirstOrDefault().PostUserId,
                                        UpdateDateTime = DateTime.Now,
                                    };

                                    posdb.TrnStockOuts.InsertOnSubmit(newStockOut);
                                    posdb.SubmitChanges();

                                    if (stockTransfer.ListPOSIntegrationTrnStockTransferItem.Any())
                                    {
                                        foreach (var item in stockTransfer.ListPOSIntegrationTrnStockTransferItem.ToList())
                                        {
                                            var currentItem = from d in posdb.MstItems where d.BarCode.Equals(item.ItemCode) select d;
                                            if (currentItem.Any())
                                            {
                                                var currentItemUnit = from d in posdb.MstUnits where d.Unit.Equals(item.Unit) select d;
                                                if (currentItemUnit.Any())
                                                {
                                                    InnosoftPOSData.TrnStockOutLine newStockOutLine = new InnosoftPOSData.TrnStockOutLine
                                                    {
                                                        StockOutId = newStockOut.Id,
                                                        ItemId = currentItem.FirstOrDefault().Id,
                                                        UnitId = currentItemUnit.FirstOrDefault().Id,
                                                        Quantity = item.Quantity,
                                                        Cost = item.Cost,
                                                        Amount = item.Amount,
                                                        AssetAccountId = currentItem.FirstOrDefault().AssetAccountId,
                                                    };

                                                    posdb.TrnStockOutLines.InsertOnSubmit(newStockOutLine);

                                                    var updateItem = currentItem.FirstOrDefault();
                                                    updateItem.OnhandQuantity = currentItem.FirstOrDefault().OnhandQuantity - Convert.ToDecimal(item.Quantity);

                                                    posdb.SubmitChanges();

                                                    trnIntegrationForm.logMessages(" > " + currentItem.FirstOrDefault().ItemDescription + " * " + item.Quantity.ToString("#,##0.00") + "\r\n\n");
                                                }
                                            }
                                        }
                                    }

                                    trnIntegrationForm.logMessages("Save Successful!" + "\r\n\n");
                                    trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logMessages("\r\n\n");
                                }
                                else
                                {
                                    trnIntegrationForm.logMessages("Cannot Save Stock Transfer (OT): ST-" + stockTransfer.BranchCode + "-" + stockTransfer.STNumber + "\r\n\n");
                                    trnIntegrationForm.logMessages("Empty Accounts!" + "\r\n\n");
                                    trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logMessages("\r\n\n");
                                }
                            }
                        }
                    }

                    isRead = true;
                }

                if (isRead)
                {
                    trnIntegrationForm.logMessages("Stock Transfer Out Integration Done.");
                }
            }
            catch (Exception e)
            {
                trnIntegrationForm.logMessages("Stock Transfer Out Integration Done.");

                trnIntegrationForm.logMessages("Stock Transfer (Out) Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logMessages("\r\n\n");
            }
        }
    }
}