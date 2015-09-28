using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2SummaryReportsViewModel : BaseModel
    {
        public Lack2SummaryReportsViewModel()
        {
            SearchView = new Lack2SearchSummaryReportsViewModel();
            DetailsList = new List<Lack2SummaryReportsItem>();
        }

        public Lack2SearchSummaryReportsViewModel SearchView { get; set; }
        public List<Lack2SummaryReportsItem> DetailsList { get; set; }

        public Lack2ExportSummaryReportsViewModel ExportModel { get; set; }
    }

    public class Lack2SearchSummaryReportsViewModel
        {

            public string CompanyCode { get; set; }
            public string NppbkcId { get; set; }
            public string SendingPlantId { get; set; }
            public string GoodType { get; set; }
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
            public SelectList SendingPlantIdList { get; set; }
            public SelectList GoodTypeList { get; set; }
            public Enums.DocumentStatus DocumentStatusList { get; set; }
            public SelectList PeriodMonthList { get; set; }
            public SelectList PeriodYearList { get; set; }

            public SelectList CreatedByList { get; set; }
            public SelectList ApprovedByList { get; set; }
            public SelectList CreatorList { get; set; }
            public SelectList ApproverList { get; set; }

        }

        public class Lack2SummaryReportsItem
        {
            public string Lack2Number { get; set; }
            public string DocumentType { get; set; }
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public string NppbkcId { get; set; }
            public string Ck5SendingPlant { get; set; }
            public string SendingPlantAddress { get; set; }
            public string Lack2Period { get; set; }
            public string Lack2Date { get; set; }
            public string TypeExcisableGoods { get; set; }

            public string TotalDeliveryExcisable { get; set; }
            public string Uom { get; set; }
            public string Poa { get; set; }
            public string PoaManager { get; set; }
            public string CreatedDate { get; set; }
            public string CreatedTime { get; set; }
            public string CreatedBy { get; set; }
            public string ApprovedDate { get; set; }
            public string ApprovedTime { get; set; }
            public string ApprovedBy { get; set; }
            public string LastChangedDate { get; set; }
            public string LastChangedTime { get; set; }
            public string Status { get; set; }
            public string LegalizeData { get; set; }
        }

        public class Lack2ExportSummaryReportsViewModel : Lack2SearchSummaryReportsViewModel
        {
            public bool BLack2Number { get; set; }
            public bool BDocumentType { get; set; }
            public bool BCompanyCode { get; set; }
            public bool BCompanyName { get; set; }
            public bool BNppbkcId { get; set; }
            public bool BCk5SendingPlant { get; set; }
            public bool BSendingPlantAddress { get; set; }
            public bool BLack2Period { get; set; }
            public bool BLack2Date { get; set; }
            public bool BTypeExcisableGoods { get; set; }
            public bool BTotalDeliveryExcisable { get; set; }
            public bool BUom { get; set; }
            public bool BPoa { get; set; }
            public bool BPoaManager { get; set; }
            public bool BCreatedDate { get; set; }
            public bool BCreatedTime { get; set; }
            public bool BCreatedBy { get; set; }
            public bool BApprovedDate { get; set; }
            public bool BApprovedTime { get; set; }
            public bool BApprovedBy { get; set; }
            public bool BLastChangedDate { get; set; }
            public bool BLastChangedTime { get; set; }
            public bool BStatus { get; set; }
            public bool BLegalizeData { get; set; }
        }
    
}