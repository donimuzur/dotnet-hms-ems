using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CIndexDocumentListViewModel : BaseModel
    {
        public Ck4CIndexDocumentListViewModel()
        {
            Detail = new List<DataDocumentList>();
        }
        public string Ck4cNumber { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }

        //selectlist
        public SelectList DocumentNumberList { get; set; }
        public SelectList CompanyNameList { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }
        public List<DataDocumentList> Detail { get; set; }
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
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ReportedOn { get; set; }
        public int? ReportedPeriod { get; set; }
        public int? ReportedMonth { get; set; }
        public int? ReportedYears { get; set; }
        public int Status { get; set; }
        public int StatusGoverment { get; set; }
        public string PoaList { get; set; }
    }
}