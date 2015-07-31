using System;
using System.Collections.Generic;
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
        public string NppbkcId { get; set; }
        public string NppbkcKppbcId { get; set; }
        public string NppbkcCompanyCode { get; set; }
        public string NppbkcCompanyName { get; set; }
        public decimal? ExGoodsQuota { get; set; }
        /// <summary>
        /// calculate from pbck-1 child
        /// </summary>
        public decimal AdditionalExGoodsQuota { get; set; }
        /// <summary>
        /// Lates Saldo from LACK
        /// </summary>
        public decimal PreviousFinalBalance { get; set; }
        /// <summary>
        /// Get from Received Amount in CK5(data come from SAP)
        /// </summary>
        public decimal Received { get; set; }
        /// <summary>
        /// TotalPbck1Quota – Received 
        /// </summary>
        public decimal QuotaRemaining { get; set; }
        /// <summary>
        /// ExGoodsQuota + AdditionalExGoodsQuota - PreviousFinalBalance
        /// </summary>
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
        public bool Company { get; set; }
        public bool Nppbkc { get; set; }
        public bool Kppbc { get; set; }
        public bool Pbck1Number { get; set; }
        public bool Address { get; set; }
        public bool OriginalNppbkc { get; set; }
        public bool OriginalKppbc { get; set; }
        public bool OriginalAddress { get; set; }
        public bool ExcGoodsAmount { get; set; }
        public bool Status { get; set; }

        public string CompanyCode { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string NppbkcId { get; set; }
    }

}