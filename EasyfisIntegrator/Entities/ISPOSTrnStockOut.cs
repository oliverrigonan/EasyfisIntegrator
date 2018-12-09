using System;
using System.Collections.Generic;

namespace EasyfisIntegrator.Entities
{
    public class ISPOSTrnStockOut
    {
        public String BranchCode { get; set; }
        public String Branch { get; set; }
        public String OTNumber { get; set; }
        public String OTDate { get; set; }
        public List<ISPOSTrnStockOutItem> ListPOSIntegrationTrnStockOutItem { get; set; }
    }
}
