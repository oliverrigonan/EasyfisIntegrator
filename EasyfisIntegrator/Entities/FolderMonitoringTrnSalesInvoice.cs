using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyfisIntegrator.Entities
{
    public class FolderMonitoringTrnSalesInvoice
    {
        public String BranchCode { get; set; }
        public String SIDate { get; set; }
        public String CustomerCode { get; set; }
        public String Term { get; set; }
        public String DocumentReference { get; set; }
        public String ManualSINumber { get; set; }
        public String Remarks { get; set; }
        public String UserCode { get; set; }
        public String CreatedDateTime { get; set; }
        public String ItemCode { get; set; }
        public String Particulars { get; set; }
        public String Unit { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal Price { get; set; }
        public Decimal DiscountAmount { get; set; }
        public Decimal NetPrice { get; set; }
        public Decimal Amount { get; set; }
    }
}