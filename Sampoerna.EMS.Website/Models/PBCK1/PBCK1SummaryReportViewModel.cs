﻿using System;
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
        public string Pbck1ReferenceNumber { get; set; }
        public Enums.PBCK1Type Pbck1Type { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public DateTime? ReportedOn { get; set; }
        public string NppbkcId { get; set; }
        public string NppbkcKppbcId { get; set; }
        public string NppbkcKppbcName { get; set; }
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
        public List<string> PoaList { get; set; }
        public bool IsNppbkcImport { get; set; }
        public bool IsExternalSupplier { get { return this.SupplierPlantWerks == null; } }
        public string SupplierCompany { get; set; }
        public List<Sampoerna.EMS.Website.Models.CK5.CK5Item> CK5List { get; set; }
        public DateTime? CompletedDate { get; set; }
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
        public string PoaSearch { get; set; }
        public string CreatorSearch { get; set; }

        public bool Pbck1Type { get; set; }
        public bool PoaList { get; set; }
        public bool SupplierPortName { get; set; }
        public bool SupplierPlant { get; set; }
        public bool GoodTypeDesc { get; set; }
        public bool PlanProdFrom { get; set; }
        public bool PlanProdTo { get; set; }
        public bool SupplierPhone { get; set; }
        public bool Reference { get; set; }
        public bool LACKFrom { get; set; }
        public bool LACKTo { get; set; }
        public bool LatestSaldo { get; set; }
        public bool PeriodFrom { get; set; }
        public bool PeriodTo { get; set; }
        public bool ReportedOn { get; set; }
        public bool RequestQty { get; set; }
        public bool StatusGov { get; set; }
        public bool QtyApproved { get; set; }
        public bool DecreeDate { get; set; }
        public bool IsNppbkcImport { get; set; }
        public bool IsExternalSupplier { get; set; }
        public bool SupplierCompany { get; set; }
        public bool ApprovedByPoaId { get; set; }
        public bool ApprovedByManagerId { get; set; }
        public bool LatestSaldoUomName { get; set; }
        public bool RequestQtyUomName { get; set; }
        public bool DocNumberCk5 { get; set; }
        public bool StatusDoc { get; set; }
        public bool GrandTotalExciseable { get; set; }
        public bool CompletedDate { get; set; }
        public bool Creator { get; set; }

        public string pbck1NumberCode { get; set; }
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

        public string pbck1Number { get; set; }
        public SelectList pbck1NumberList { get; set; }

        public string Poa { get; set; }
        public SelectList PoaList { get; set; }

        public string Creator { get; set; }
        public SelectList CreatorList { get; set; }
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
        public string Pbck1Type { get; set; }
        public string SupplierPortName { get; set; }
        public string SupplierPlant { get; set; }
        public string GoodTypeDesc { get; set; }
        public string PlanProdFrom { get; set; }
        public string PlanProdTo { get; set; }
        public string SupplierPhone { get; set; }
        public string Reference { get; set; }
        public string LACKFrom { get; set; }
        public string LACKTo { get; set; }
        public string LatestSaldo { get; set; }
        public string PeriodFrom { get; set; }
        public string PeriodTo { get; set; }
        public string ReportedOn { get; set; }
        public string RequestQty { get; set; }
        public string StatusGov { get; set; }
        public string QtyApproved { get; set; }
        public string DecreeDate { get; set; }
        public string PoaList { get; set; }
        public string IsNppbkcImport { get; set; }
        public string SupplierCompany { get; set; }
        public string ApprovedByPoaId { get; set; }
        public string ApprovedByManagerId { get; set; }
        public string LatestSaldoUomName { get; set; }
        public string RequestQtyUomName { get; set; }
        public string DocNumberCk5 { get; set; }
        public string StatusDocCk5 { get; set; }
        public string GrandTotalExcisableCk5 { get; set; }
        public string CompletedDate { get; set; }
        public string Creator { get; set; }
    }

}