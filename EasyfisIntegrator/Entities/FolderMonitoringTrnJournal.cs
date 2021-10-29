using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyfisIntegrator.Entities
{
    public class FolderMonitoringTrnJournal
    {
        public String JournalDate { get; set; }
        public Decimal TotalDebitAmount { get; set; }
        public Decimal TotalCreditAmount { get; set; }
        public Decimal TotalBalance { get; set; }
    }
}