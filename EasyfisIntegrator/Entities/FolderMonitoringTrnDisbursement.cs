using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easyfis.Integration.FolderMonitoring.Entities
{
    public class FolderMonitoringTrnDisbursement
    {
        public Int32 Id { get; set; }
        public String BranchCode { get; set; }
        public String CVDate { get; set; }
        public String SupplierCode { get; set; }
        public String Payee { get; set; }
        public String PayType { get; set; }
        public String BankCode { get; set; }
        public String Remarks { get; set; }
        public String ManualCVNumber { get; set; }
        public String CheckNumber { get; set; }
        public String CheckDate { get; set; }
        public Boolean IsCrossCheck { get; set; }
        public Boolean IsClear { get; set; }
        public String UserCode { get; set; }
        public String CreatedDateTime { get; set; }
        public String AccountCode { get; set; }
        public String ArticleCode { get; set; }
        public String RRNumber { get; set; }
        public String Particulars { get; set; }
        public Decimal Amount { get; set; }
    }
}