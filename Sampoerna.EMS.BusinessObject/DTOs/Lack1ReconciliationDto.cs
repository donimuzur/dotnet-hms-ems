using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1ReconciliationDto
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public int MonthNumber { get; set; }
        public string Date { get; set; }
        public string ItemCode { get; set; }
        public string FinishGoodCode { get; set; }
        public decimal Remaining { get; set; }
        public decimal BeginningStock { get; set; }
        public string ReceivedCk5No { get; set; }
        public decimal Received { get; set; }
        public decimal UsageOther { get; set; }
        public decimal UsageSelf { get; set; }
        public decimal ResultTis { get; set; }
        public decimal ResultStick { get; set; }
        public decimal EndingStock { get; set; }
        public string RemarkDesc { get; set; }
        public string RemarkCk5No { get; set; }
        public decimal RemarkQty { get; set; }
        public decimal StickProd { get; set; }
        public decimal PackProd { get; set; }
        public decimal Wip { get; set; }
        public decimal RejectMaker { get; set; }
        public decimal RejectPacker { get; set; }
        public decimal FloorSweep { get; set; }
        public decimal Stem { get; set; }
    }

    public class Lack1MonthReconciliation
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string NppbkcId { get; set; }
    }
}
