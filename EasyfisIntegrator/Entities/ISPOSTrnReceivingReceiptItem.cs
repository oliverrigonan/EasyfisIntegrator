using System;

namespace EasyfisIntegrator.Entities
{
    public class ISPOSTrnReceivingReceiptItem
    {
        public Int32 RRId { get; set; }
        public String ItemCode { get; set; }
        public String Item { get; set; }
        public String BranchCode { get; set; }
        public String Unit { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal Cost { get; set; }
        public Decimal Amount { get; set; }
    }
}
