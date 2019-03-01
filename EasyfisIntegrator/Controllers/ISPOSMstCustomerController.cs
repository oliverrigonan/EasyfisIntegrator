using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace EasyfisIntegrator.Controllers
{
    class ISPOSMstCustomerController
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
        public ISPOSMstCustomerController(Forms.TrnIntegrationForm form, String actDate)
        {
            trnIntegrationForm = form;
            activityDate = actDate;
        }

        // ============
        // Get Customer
        // ============
        public void GetCustomer(String apiUrlHost)
        {
            try
            {
                DateTime dateTimeToday = DateTime.Now;
                String currentDate = Convert.ToDateTime(activityDate).ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);

                // ============
                // Http Request
                // ============
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + apiUrlHost + "/api/get/POSIntegration/customer/" + currentDate);
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
                    List<Entities.ISPOSMstCustomer> customerLists = (List<Entities.ISPOSMstCustomer>)js.Deserialize(result, typeof(List<Entities.ISPOSMstCustomer>));

                    if (customerLists.Any())
                    {
                        foreach (var customer in customerLists)
                        {
                            var terms = from d in posdb.MstTerms where d.Term.Equals(customer.Term) select d;
                            if (terms.Any())
                            {
                                var defaultSettings = from d in posdb.SysSettings select d;

                                var currentCustomer = from d in posdb.MstCustomers where d.CustomerCode.Equals(customer.ManualArticleCode) && d.CustomerCode != null && d.IsLocked == true select d;
                                if (currentCustomer.Any())
                                {
                                    Boolean foundChanges = false;

                                    if (!foundChanges)
                                    {
                                        if (!currentCustomer.FirstOrDefault().Customer.Equals(customer.Article))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentCustomer.FirstOrDefault().Address.Equals(customer.Address))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentCustomer.FirstOrDefault().ContactPerson.Equals(customer.ContactPerson))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentCustomer.FirstOrDefault().ContactNumber.Equals(customer.ContactNumber))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentCustomer.FirstOrDefault().MstTerm.Term.Equals(customer.Term))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (!currentCustomer.FirstOrDefault().TIN.Equals(customer.TaxNumber))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (!foundChanges)
                                    {
                                        if (Convert.ToDecimal(currentCustomer.FirstOrDefault().CreditLimit) != Convert.ToDecimal(customer.CreditLimit))
                                        {
                                            foundChanges = true;
                                        }
                                    }

                                    if (foundChanges)
                                    {
                                        trnIntegrationForm.logMessages("Updating Customer: " + currentCustomer.FirstOrDefault().Customer + "\r\n\n");
                                        trnIntegrationForm.logMessages("Customer Code: " + currentCustomer.FirstOrDefault().CustomerCode + "\r\n\n");

                                        var updateCustomer = currentCustomer.FirstOrDefault();
                                        updateCustomer.Customer = customer.Article;
                                        updateCustomer.Address = customer.Address;
                                        updateCustomer.ContactPerson = customer.ContactPerson;
                                        updateCustomer.ContactNumber = customer.ContactNumber;
                                        updateCustomer.CreditLimit = customer.CreditLimit;
                                        updateCustomer.TermId = terms.FirstOrDefault().Id;
                                        updateCustomer.TIN = customer.TaxNumber;
                                        updateCustomer.UpdateUserId = defaultSettings.FirstOrDefault().PostUserId;
                                        updateCustomer.UpdateDateTime = DateTime.Now;
                                        updateCustomer.CustomerCode = customer.ManualArticleCode;
                                        posdb.SubmitChanges();

                                        trnIntegrationForm.logMessages("Customer Integration Done.");

                                        trnIntegrationForm.logMessages("Update Successful!" + "\r\n\n");
                                        trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                        trnIntegrationForm.logMessages("\r\n\n");
                                    }
                                }
                                else
                                {
                                    trnIntegrationForm.logMessages("Saving Customer: " + customer.Article + "\r\n\n");
                                    trnIntegrationForm.logMessages("Customer Code: " + customer.ManualArticleCode + "\r\n\n");

                                    InnosoftPOSData.MstCustomer newCustomer = new InnosoftPOSData.MstCustomer
                                    {
                                        Customer = customer.Article,
                                        Address = customer.Address,
                                        ContactPerson = customer.ContactPerson,
                                        ContactNumber = customer.ContactNumber,
                                        CreditLimit = customer.CreditLimit,
                                        TermId = terms.FirstOrDefault().Id,
                                        TIN = customer.TaxNumber,
                                        WithReward = false,
                                        RewardNumber = null,
                                        RewardConversion = 4,
                                        AccountId = 64,
                                        EntryUserId = defaultSettings.FirstOrDefault().PostUserId,
                                        EntryDateTime = DateTime.Now,
                                        UpdateUserId = defaultSettings.FirstOrDefault().PostUserId,
                                        UpdateDateTime = DateTime.Now,
                                        IsLocked = true,
                                        DefaultPriceDescription = null,
                                        CustomerCode = customer.ManualArticleCode,
                                    };

                                    posdb.MstCustomers.InsertOnSubmit(newCustomer);
                                    posdb.SubmitChanges();

                                    trnIntegrationForm.logMessages("Customer Integration Done.");

                                    trnIntegrationForm.logMessages("Save Successful!" + "\r\n\n");
                                    trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                                    trnIntegrationForm.logMessages("\r\n\n");
                                }
                            }
                            else
                            {
                                trnIntegrationForm.logMessages("Customer Integration Done.");

                                trnIntegrationForm.logMessages("Cannot Save Customer: " + customer.Article + "\r\n\n");
                                trnIntegrationForm.logMessages("Term Mismatch!" + "\r\n\n");
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
                trnIntegrationForm.logMessages("Customer Integration Done.");

                trnIntegrationForm.logMessages("Customer Error: " + e.Message + "\r\n\n");
                trnIntegrationForm.logMessages("Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");
                trnIntegrationForm.logMessages("\r\n\n");
            }
        }
    }
}