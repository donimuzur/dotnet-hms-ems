using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CIndexDocumentListViewModel : BaseModel
    {
        public Ck4CIndexDocumentListViewModel()
        {
            Detail = new List<DataDocumentList>();
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
            ActionType = "Edit";
        }
        public string Ck4cNumber { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowPrintDocument { get; set; }
        public bool AllowEditCompleted { get; set; }
        public string ActionType { get; set; }

        //selectlist
        public SelectList DocumentNumberList { get; set; }
        public SelectList CompanyNameList { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }
        public Enums.StatusGovCk4c StatusGovList { get; set; }
        public List<DataDocumentList> Detail { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public DataDocumentList Details { get; set; }
        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList PeriodList { get; set; }
        public SelectList PlanList { get; set; }
    }
    public class DataDocumentList
    {
        public int Ck4CId { get; set; }
        public string Number { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string NppbkcId { get; set; }
        public string ApprovedByPoa { get; set; }
        public DateTime? ApprovedDatePoa { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedDateManager { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ReportedOn { get; set; }
        public DateTime? DecreeDate { get; set; }
        public int? ReportedPeriod { get; set; }
        public int? ReportedMonth { get; set; }
        public int? ReportedYears { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.StatusGovCk4c? StatusGoverment { get; set; }
        public Enums.ActionType GovApprovalActionType { get; set; }
        public string PoaList { get; set; }
        public string ReportedMonthName { get; set; }
        public string StatusName { get; set; }
        public string StatusGovName { get; set; }
        public string BasedOn { get; set; }
        public List<Ck4cItemData> Ck4cItemData { get; set; }
        public List<HttpPostedFileBase> Ck4cDecreeFiles { get; set; }
        public List<Ck4cDecreeDocModel> Ck4cDecreeDoc { get; set; }
        public string IsSaveSubmit { get; set; }
        public string Comment { get; set; }
    }
    public class Ck4cItemData
    {
        public long Ck4CItemId { get; set; }
        public int Ck4CId { get; set; }
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public Decimal ProdQty { get; set; }
        public string ProdQtyUom { get; set; }
        public DateTime ProdDate { get; set; }
        public Decimal HjeIdr { get; set; }
        public Decimal Tarif { get; set; }
        public string ProdCode { get; set; }
        public Decimal PackedQty { get; set; }
        public Decimal UnpackedQty { get; set; }
        public string ProdDateName { get; set; }
        public string BrandDescription { get; set; }
        public string PlantName { get; set; }
        public string ProdType { get; set; }
        public int ContentPerPack { get; set; }
        public int PackedInPack { get; set; }
        public string Remarks { get; set; }
    }
}