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
    class ISPOSMstSupplierController
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
        public ISPOSMstSupplierController(Forms.TrnIntegrationForm form, String actDate)
        {
            trnIntegrationForm = form;
            activityDate = actDate;
        }

        // =============
        // Sync Supplier
        // =============
        public async void SyncSupplier(String apiUrlHost)
        {
            await GetSupplier(apiUrlHost);
        }

        // ============
        // Get Supplier
        // ============
        public Task GetSupplier(String apiUrlHost)
        {
            try
            {
                DateTime dateTimeToday = DateTime.Now;
                String currentDate = Convert.ToDateTime(activityDate).ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);

                // ============
                // Http Request
                // ============
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrlHost + "/api/get/POSIntegration/supplier/" + currentDate);
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
                    List<Entities.ISPOSMstSupplier> supplierLists = (List<Entities.ISPOSMstSupplier>)js.Deserialize(result, typeof(List<Entities.ISPOSMstSupplier>));

                    if (supplierLists.Any())
                    {
                        foreach (var supplier in supplierLists)
                        {
                            var terms = from d in posdb.MstTerms where d.Term.Equals(supplier.Term) select d;
                            if (terms.Any())
                            {
                                var defaultSettings = from d in posdb.SysSettings select d;

                                var currentSupplier = from d in posdb.MstSuppliers where d.Supplier.Equals(supplier.Article) && d.IsLocked == true select d;
                                if (currentSupplier.Any())
                                {
                                    Boolean foundChanges = false;

                                    if (!foundChanges)
                                    {
                                        if (!currentSupplier.FirstOrDefault().Supplier.Equals(supplier.Article))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentSupplier.FirstOrDefault().Address.Equals(supplier.Address))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentSupplier.FirstOrDefault().CellphoneNumber.Equals(supplier.ContactNumber))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentSupplier.FirstOrDefault().MstTerm.Term.Equals(supplier.Term))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentSupplier.FirstOrDefault().TIN.Equals(supplier.TaxNumber))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (foundChanges)
                                    {
                                        trnIntegrationForm.salesIntegrationLogMessages("Updating Supplier: " + currentSupplier.FirstOrDefault().Supplier + "\r\n\n");
                                        trnIntegrationForm.salesIntegrationLogMessages("Contact No.: " + currentSupplier.FirstOrDefault().CellphoneNumber + "\r\n\n");

                                        var updateSupplier = currentSupplier.FirstOrDefault();
                                        updateSupplier.Supplier = supplier.Article;
                                        updateSupplier.Address = supplier.Address;
                                        updateSupplier.CellphoneNumber = supplier.ContactNumber;
                                        updateSupplier.TermId = terms.FirstOrDefault().Id;
                                        updateSupplier.TIN = supplier.TaxNumber;
                                        updateSupplier.UpdateUserId = defaultSettings.FirstOrDefault().PostUserId;
                                        updateSupplier.UpdateDateTime = DateTime.Now;
                                        posdb.SubmitChanges();

                                        trnIntegrationForm.salesIntegrationLogMessages("Update Successful!" + "\r\n\n");
                                        trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");
                                    }
                                }
                                else
                                {
                                    trnIntegrationForm.salesIntegrationLogMessages("Saving Supplier: " + supplier.Article + "\r\n\n");
                                    trnIntegrationForm.salesIntegrationLogMessages("Contact No.: " + supplier.ContactNumber + "\r\n\n");

                                    InnosoftPOSData.MstSupplier newSupplier = new InnosoftPOSData.MstSupplier
                                    {
                                        Supplier = supplier.Article,
                                        Address = supplier.Address,
                                        TelephoneNumber = "NA",
                                        CellphoneNumber = supplier.ContactNumber,
                                        FaxNumber = "NA",
                                        TermId = terms.FirstOrDefault().Id,
                                        TIN = supplier.TaxNumber,
                                        AccountId = 254,
                                        EntryUserId = defaultSettings.FirstOrDefault().PostUserId,
                                        EntryDateTime = DateTime.Now,
                                        UpdateUserId = defaultSettings.FirstOrDefault().PostUserId,
                                        UpdateDateTime = DateTime.Now,
                                        IsLocked = true,
                                    };

                                    posdb.MstSuppliers.InsertOnSubmit(newSupplier);
                                    posdb.SubmitChanges();

                                    trnIntegrationForm.salesIntegrationLogMessages("Save Successful!" + "\r\n\n");
                                    trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");
                                }
                            }
                            else
                            {
                                trnIntegrationForm.salesIntegrationLogMessages("Cannot Save Supplier: " + supplier.Article + "\r\n\n");
                                trnIntegrationForm.salesIntegrationLogMessages("Term Mismatch!" + "\r\n\n");
                                trnIntegrationForm.salesIntegrationLogMessages("Save Failed!" + "\r\n\n");
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
                trnIntegrationForm.salesIntegrationLogMessages("Supplier Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.salesIntegrationLogMessages("\r\n\n");

                return Task.FromResult("");
            }
        }
    }
}