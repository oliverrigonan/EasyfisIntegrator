using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyfisIntegrator.Entities
{
    public class FolderMonitoringTrnJournalVoucher
    {
        public String BranchCode { get; set; }
        public String JVDate { get; set; }
        public String ManualJVNumber { get; set; }
        public String Remarks { get; set; }
        public String UserCode { get; set; }
        public String CreatedDateTime { get; set; }
        public String EntryBranchCode { get; set; }
        public String AccountCode { get; set; }
        public String ArticleCode { get; set; }
        public String Particulars { get; set; }
        public Decimal DebitAmount { get; set; }
        public Decimal CreditAmount { get; set; }
        public String APRRNumber { get; set; }
        public String APManualRRNumber { get; set; }
        public String ARSINumber { get; set; }
        public String ARManualSINumber { get; set; }
        public Boolean IsClear { get; set; }
    }
}