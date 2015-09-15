using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.PLANT;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1SummaryReportViewModel : BaseModel
    {
        public Pbck1SummaryReportViewModel()
        {
            SearchView = new Pbck1FilterSummaryReportViewModel();
            DetailsList = new List<Pbck1SummaryReportsItem>();
        }
        public Pbck1FilterSummaryReportViewModel SearchView { get; set; }
        public List<Pbck1SummaryReportsItem> DetailsList { get; set; }
        public Pbck1ExportSummaryReportsViewModel ExportModel { get; set; }
    }

    public class Pbck1SummaryReportsItem
    {
        public int Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public long? Pbck1Reference { get; set; }
        public Enums.PBCK1Type Pbck1Type { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public DateTime? ReportedOn { get; set; }
        public string NppbkcId { get; set; }
        public string NppbkcKppbcId { get; set; }
        public string NppbkcCompanyCode { get; set; }
        public string NppbkcCompanyName { get; set; }
        public string GoodType { get; set; }
        public string GoodTypeDesc { get; set; }
        public string SupplierPlant { get; set; }
        public int? SupplierPortId { get; set; }
        public string SupplierPortName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierNppbkcId { get; set; }
        public string SupplierKppbcId { get; set; }
        //=== Fixing Bug PBCK1 No. 168 ===
        public string SupplierKppbcName { get; set; }
        //================================
        public string SupplierPlantWerks { get; set; }
        public DateTime? PlanProdFrom { get; set; }
        public DateTime? PlanProdTo { get; set; }
        public decimal? RequestQty { get; set; }
        public string RequestQtyUomId { get; set; }
        public string RequestQtyUomName { get; set; }
        public int? Lack1FromMonthId { get; set; }
        public string Lack1FromMonthName { get; set; }
        public int? Lack1FormYear { get; set; }
        public int? Lack1ToMonthId { get; set; }
        public string Lack1ToMonthName { get; set; }
        public int? Lack1ToYear { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public string StatusName { get; set; }
        public Enums.DocumentStatusGov? StatusGov { get; set; }
        public string StatusGovName { get; set; }
        public decimal? QtyApproved { get; set; }
        public DateTime? DecreeDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public string ApprovedByPoaId { get; set; }
        public string ApprovedByManagerId { get; set; }
        public DateTime? ApprovedByPoaDate { get; set; }
        public DateTime? ApprovedByManagerDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? LatestSaldo { get; set; }
        public string LatestSaldoUomId { get; set; }
        public string LatestSaldoUomName { get; set; }
        public List<T001WModel> NppbkcPlants { get; set; }
    }

    public class Pbck1ExportSummaryReportsViewModel
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

    public class Pbck1FilterSummaryReportViewModel
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

    public class ExportSummaryDataModel
    {
        public string Company { get; set; }
        public string Nppbkc { get; set; }
        public string Kppbc { get; set; }
        public string Pbck1Number { get; set; }
        public string Address { get; set; }
        public string OriginalNppbkc { get; set; }
        public string OriginalKppbc { get; set; }
        public string OriginalAddress { get; set; }
        public string ExcGoodsAmount { get; set; }
        public string Status { get; set; }
    }

}