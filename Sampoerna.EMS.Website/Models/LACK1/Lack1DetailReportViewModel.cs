using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1DetailReportViewModel : BaseModel
    {
        public Lack1DetailReportViewModel()
        {
            DetailList = new List<Lack1DetailReportItemModel>();
            SearchView = new Lack1SearchDetailReportViewModel();
        }
        public List<Lack1DetailReportItemModel> DetailList { get; set; }
        public Lack1SearchDetailReportViewModel SearchView { get; set; }

        public Lack1SearchDetailReportViewModel ExportSearchView { get; set; }
    }

    public class Lack1SearchDetailReportViewModel
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivingPlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string SupplierPlantId { get; set; }
        public string PeriodFrom { get; set; }
        public string PeriodTo { get; set; }
        public DateTime? GrDateFrom { get; set; }
        public DateTime? GrDateTo { get; set; }
        public DateTime? GiDateFrom { get; set; }
        public DateTime? GiDateTo { get; set; }

        public SelectList CompanyCodeList { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public SelectList ReceivingPlantIdList { get; set; }
        public SelectList ExcisableGoodsTypeList { get; set; }
        public SelectList SupplierPlantIdList { get; set; }
        public SelectList PeriodFromList { get; set; }
        public SelectList PeriodToList { get; set; }
    }

    public class Lack1DetailReportItemModel
    {
        public Lack1DetailReportItemModel()
        {
            TrackingConsolidations = new List<Lack1TrackingConsolidationDetailReportItemModel>();
        }
        public int Lack1Id { get; set; }
        public string Lack1Number { get; set; }
        public string Lack1LevelName { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public List<Lack1TrackingConsolidationDetailReportItemModel> TrackingConsolidations { get; set; }
    }

    public class Lack1TrackingConsolidationDetailReportItemModel
    {
        #region -------------- Receiving Table on FS Doc ------------
        public long Ck5Id { get; set; }
        public string Ck5Number { get; set; }
        public string Ck5RegistrationNumber { get; set; }
        public string Ck5RegistrationDate { get; set; }
        public string Ck5GrDate { get; set; }
        public decimal Qty { get; set; }
        #endregion

        #region ----------- Usage Table on FS Doc ---------
        public string GiDate { get; set; }
        public string PurchaseDoc { get; set; }
        public string MaterialCode { get; set; }
        public decimal? UsageQty { get; set; }
        public string OriginalUomId { get; set; }
        public string ConvertedUomId { get; set; }
        public string Batch { get; set; }
        #endregion 

        public int MaterialCodeUsageRecCount { get; set; }

    }

}