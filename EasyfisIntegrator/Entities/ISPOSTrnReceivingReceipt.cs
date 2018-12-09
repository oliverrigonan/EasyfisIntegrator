using System;
using System.Collections.Generic;

namespace EasyfisIntegrator.Entities
{
    public class ISPOSTrnReceivingReceipt
    {
        public String BranchCode { get; set; }
        public String Branch { get; set; }
        public String RRNumber { get; set; }
        public String RRDate { get; set; }
        public List<ISPOSTrnReceivingReceiptItem> ListPOSIntegrationTrnReceivingReceiptItem { get; set; }
    }
}
