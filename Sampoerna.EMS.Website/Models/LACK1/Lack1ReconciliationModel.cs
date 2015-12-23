using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1ReconciliationModel : BaseModel
    {
        public Lack1ReconciliationModel()
        {
            SearchView = new Lack1SearchReconciliationModel();
            Detail = new List<DataReconciliation>();
        }

        public Lack1SearchReconciliationModel SearchView { get; set; }
        public Lack1SearchReconciliationModel ExportSearchView { get; set; }
        public List<DataReconciliation> Detail { get; set; }
    }

    public class Lack1SearchReconciliationModel
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string ExGoodType { get; set; }

        public SelectList NppbkcIdList { get; set; }
        public SelectList PlantIdList { get; set; }
        public SelectList ExGoodTypeList { get; set; }
    }

    public class DataReconciliation
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string Month { get; set; }
        public string Date { get; set; }
        public string ItemCode { get; set; }
        public string FinishGoodCode { get; set; }
        public decimal Remaining { get; set; }
        public decimal BeginningStock { get; set; }
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
}