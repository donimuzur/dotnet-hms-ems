using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.LACK10
{
    public class Lack10IndexViewModel : BaseModel
    {
        public Lack10IndexViewModel()
        {
            Detail = new List<DataDocumentList>();
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
            ActionType = "Edit";
        }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string PlantId { get; set; }
        public bool IsOpenDocument { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowPrintDocument { get; set; }
        public bool AllowEditCompleted { get; set; }
        public string ActionType { get; set; }

        //selectlist
        public SelectList CompanyNameList { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public List<DataDocumentList> Detail { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public DataDocumentList Details { get; set; }
        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList PlanList { get; set; }
        public SelectList PoaList { get; set; }
        public Enums.Lack10ReportType ReportTypeList { get; set; }
        public Enums.DocumentStatusGovType2 StatusGovList { get; set; }
    }

    public class DataDocumentList
    {
        public int Lack10Id { get; set; }
        public string Lack10Number { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNpwp { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYears { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public Enums.Lack10ReportType ReportType { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatusGovType2? StatusGoverment { get; set; }
        public Enums.ActionType GovApprovalActionType { get; set; }
        public DateTime? DecreeDate { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ApprovedByPoa { get; set; }
        public DateTime? ApprovedDatePoa { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedDateManager { get; set; }
        public string PoaList { get; set; }
        public string ReportedMonthName { get; set; }
        public string StatusName { get; set; }
        public string StatusGovName { get; set; }
        public string BasedOn { get; set; }
        public List<Lack10ItemData> Lack10ItemData { get; set; }
        public List<HttpPostedFileBase> Lack10DecreeFiles { get; set; }
        public List<Lack10DecreeDocModel> Lack10DecreeDoc { get; set; }
        public List<string> Lack10UploadedDoc { get; set; }
        public string IsSaveSubmit { get; set; }
        public string Comment { get; set; }
    }

    public class Lack10ItemData
    {
        public long Lack10ItemId { get; set; }
        public int Lack10Id { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public string Werks { get; set; }
        public string PlantName { get; set; }
        public string Type { get; set; }
        public string Uom { get; set; }
        public Decimal WasteValue { get; set; }
    }
}