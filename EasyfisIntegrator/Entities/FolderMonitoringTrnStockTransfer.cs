using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyfisIntegrator.Entities
{
    public class FolderMonitoringTrnStockTransfer
    {
        public String BranchCode { get; set; }
        public String STDate { get; set; }
        public String ManualSTNumber { get; set; }
        public String ToBranchCode { get; set; }
        public String ArticleCode { get; set; }
        public String Remarks { get; set; }
        public String UserCode { get; set; }
        public String CreatedDateTime { get; set; }
        public String ItemCode { get; set; }
        public String Particulars { get; set; }
        public String Unit { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal Cost { get; set; }
        public Decimal Amount { get; set; }
        public Int32 No { get; set; }
    }
}