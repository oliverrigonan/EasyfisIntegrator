using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easyfis.Integration.FolderMonitoring.Entities
{
    public class FolderMonitoringTrnStockIn
    {
        public Int32 Id { get; set; }
        public String BranchCode { get; set; }
        public String INDate { get; set; }
        public String AccountCode { get; set; }
        public String ArticleCode { get; set; }
        public String Remarks { get; set; }
        public String ManualINNumber { get; set; }
        public String IsProduce { get; set; }
        public String UserCode { get; set; }
        public String CreatedDateTime { get; set; }
        public String ItemCode { get; set; }
        public String Particulars { get; set; }
        public String Unit { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal Cost { get; set; }
        public Decimal Amount { get; set; }
    }
}