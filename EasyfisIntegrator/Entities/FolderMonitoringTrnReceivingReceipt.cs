﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyfisIntegrator.Entities
{
    public class FolderMonitoringTrnReceivingReceipt
    {
        public String BranchCode { get; set; }
        public String RRDate { get; set; }
        public String ManualRRNumber { get; set; }
        public String DocumentReference { get; set; }
        public String SupplierCode { get; set; }
        public String Remarks { get; set; }
        public String UserCode { get; set; }
        public String CreatedDateTime { get; set; }
        public String PONumber { get; set; }
        public String ManualPONumber { get; set; }
        public String PODate { get; set; }
        public String PODateNeeded { get; set; }
        public Decimal POQuantity { get; set; }
        public Decimal POCost { get; set; }
        public Decimal POAmount { get; set; }
        public String ItemCode { get; set; }
        public String Particulars { get; set; }
        public String Unit { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal Cost { get; set; }
        public Decimal Amount { get; set; }
        public String ReceivedBranchCode { get; set; }
        public Int32 No { get; set; }
    }
}