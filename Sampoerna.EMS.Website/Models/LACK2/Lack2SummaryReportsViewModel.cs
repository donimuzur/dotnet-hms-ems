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
            public string Status { get; set; }
        }

        public class Lack2ExportSummaryReportsViewModel : Lack2SearchSummaryReportsViewModel
        {
            public bool Lack2Number { get; set; }
            public bool DocumentType { get; set; }
            public new bool CompanyCode { get; set; }
            public bool CompanyName { get; set; }
            public new bool NppbkcId { get; set; }
            public bool Ck5SendingPlant { get; set; }
            public bool SendingPlantAddress { get; set; }
            public bool Lack2Period { get; set; }
            public bool Lack2Date { get; set; }
            public bool TypeExcisableGoods { get; set; }
            public bool Status { get; set; }
        }
    
}