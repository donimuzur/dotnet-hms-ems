using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1MonitoringUsageViewModel : BaseModel
    {
        public Pbck1MonitoringUsageViewModel()
        {
            SearchView = new Pbck1FilterMonitoringUsageViewModel();
            DetailsList = new List<Pbck1MonitoringUsageItem>();
            ExportModel = new Pbck1ExportMonitoringUsageViewModel();
        }
        public Pbck1FilterMonitoringUsageViewModel SearchView { get; set; }
        public List<Pbck1MonitoringUsageItem> DetailsList { get; set; }
        public Pbck1ExportMonitoringUsageViewModel ExportModel { get; set; }
    }

    public class Pbck1MonitoringUsageItem
    {
        public int Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string Pbck1PeriodDisplay { get; set; }
        public string NppbkcId { get; set; }
        public string NppbkcKppbcId { get; set; }
        public string NppbkcCompanyCode { get; set; }
        public string NppbkcCompanyName { get; set; }
        [UIHint("FormatQty")]
        public decimal? ExGoodsQuota { get; set; }
        /// <summary>
        /// calculate from pbck-1 child
        /// </summary>
        [UIHint("FormatQty")]
        public decimal AdditionalExGoodsQuota { get; set; }
        /// <summary>
        /// Lates Saldo from LACK
        /// </summary>
        public decimal PreviousFinalBalance { get; set; }
        /// <summary>
        /// Get from Received Amount in CK5(data come from SAP)
        /// </summary>
        [UIHint("FormatQty")]
        public decimal Received { get; set; }
        /// <summary>
        /// TotalPbck1Quota – Received 
        /// </summary>
        [UIHint("FormatQty")]
        public decimal QuotaRemaining { get; set; }
        /// <summary>
        /// ExGoodsQuota + AdditionalExGoodsQuota - PreviousFinalBalance
        /// </summary>
        [UIHint("FormatQty")]
        public decimal TotalPbck1Quota { get; set; }
    }

    public class Pbck1FilterMonitoringUsageViewModel
    {
        public string CompanyCode { get; set; }
        public SelectList CompanyCodeList { get; set; }

        public int? YearFrom { get; set; }
        public SelectList YearFromList { get; set; }

        public int? YearTo { get; set; }
        public SelectList YearToList { get; set; }

        public string NppbkcId { get; set; }
        public SelectList NppbkcIdList { get; set; }
    }

    public class Pbck1ExportMonitoringUsageViewModel
    {
        public bool Pbck1Decree { get; set; }
        public bool Company { get; set; }
        public bool Nppbkc { get; set; }
        public bool Kppbc { get; set; }
        public bool Pbck1Period { get; set; }
        public bool ExcGoodsQuota { get; set; }
        public bool AdditionalExcGoodsQuota { get; set; }
        public bool PrevYearsFinalBalance { get; set; }
        public bool TotalPbck1Quota { get; set; }
        public bool Received { get; set; }
        public bool QuotaRemaining { get; set; }

        public string CompanyCode { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string NppbkcId { get; set; }
    }

}