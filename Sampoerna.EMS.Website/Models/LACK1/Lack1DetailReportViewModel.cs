﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;

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
        public int Lack1Id { get; set; }
        public string Lack1Number { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public List<Lack1ReceivingDetailReportItemModel> ReceivingList { get; set; }
        public List<Lack1UsageDetailReportItemModel> UsageList { get; set; }
    }

    public class Lack1ReceivingDetailReportItemModel
    {
        public int Ck5Id { get; set; }
        public string Ck5Number { get; set; }
        public string Ck5RegistrationNumber { get; set; }
        public string Ck5RegistrationDate { get; set; }
        public string Ck5GrDate { get; set; }
        public decimal Qty { get; set; }
    }

    public class Lack1UsageDetailReportItemModel
    {
        public string GiDate { get; set; }
        public string MaterialCode { get; set; }
        public decimal UsageQty { get; set; }
        public string OriginalUomId { get; set; }
        public string ConvertedUomId { get; set; }
    }
}