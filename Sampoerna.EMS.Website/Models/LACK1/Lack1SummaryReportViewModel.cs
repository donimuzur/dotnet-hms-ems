using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1SummaryReportViewModel : BaseModel
    {
        public Lack1SummaryReportViewModel()
        {
            SearchView = new Lack1SearchSummaryReportViewModel();
            DetailsList = new List<Lack1SummaryReportItemModel>();
        }
        public Lack1SearchSummaryReportViewModel SearchView { get; set; }
        public List<Lack1SummaryReportItemModel> DetailsList { get; set; }
        public Lack1ExportSummaryReportViewModel ExportModel { get; set; }
    }

    public class Lack1SearchSummaryReportViewModel
    {

        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivingPlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string SupplierPlantId { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }

        public Enums.DocumentStatus? DocumentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string Creator { get; set; }
        public string Approver { get; set; }

        public SelectList CompanyCodeList { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public SelectList ReceivingPlantIdList { get; set; }
        public SelectList ExcisableGoodsTypeList { get; set; }
        public SelectList SupplierPlantIdList { get; set; }
        public SelectList PeriodMonthList { get; set; }
        public SelectList PeriodYearList { get; set; }
        public Enums.DocumentStatus DocumentStatusList { get; set; }
        public SelectList CreatedByList { get; set; }
        public SelectList ApprovedByList { get; set; }
        public SelectList CreatorList { get; set; }
        public SelectList ApproverList { get; set; }
    }

    public class Lack1SummaryReportItemModel
    {
        public string Lack1Number { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivingPlantId { get; set; }
        public string ReceivingPlantName { get; set; }
        public string ExcisableGoodsTypeId { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantName { get; set; }
        public string Period { get; set; }
        public string DocumentStatus { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string Creator { get; set; }
        public string Approver { get; set; }
    }

    public class Lack1ExportSummaryReportViewModel : Lack1SearchSummaryReportViewModel
    {
        public bool BLack1Number { get; set; }
        public bool BCompanyCode { get; set; }
        public bool BCompanyName { get; set; }
        public bool BNppbkcId { get; set; }
        public bool BReceivingPlantId { get; set; }
        public bool BReceivingPlantName { get; set; }
        public bool BExcisableGoodsTypeId { get; set; }
        public bool BExcisableGoodsTypeDesc { get; set; }
        public bool BSupplierPlantId { get; set; }
        public bool BSupplierPlantName { get; set; }
        public bool BPeriod { get; set; }
        public bool BDocumentStatus { get; set; }
        public bool BCreatedDate { get; set; }
        public bool BCreatedBy { get; set; }
        public bool BApprovedDate { get; set; }
        public bool BApprovedBy { get; set; }
        public bool BCreator { get; set; }
        public bool BApprover { get; set; }
    }

    public class Lack1ExportSummaryDataModel
    {
        public string Lack1Number { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivingPlantId { get; set; }
        public string ReceivingPlantName { get; set; }
        public string ExcisableGoodsTypeId { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantName { get; set; }
        public string Period { get; set; }
        public string DocumentStatus { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string Creator { get; set; }
        public string Approver { get; set; }
    }

}