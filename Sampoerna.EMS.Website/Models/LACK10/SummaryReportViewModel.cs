using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK10
{
    public class SummaryReportViewModel : BaseModel
    {
        public SummaryReportViewModel()
        {
            SearchView = new SearchSummaryReportsViewModel();
            DetailsList = new List<SummaryReportsItem>();
        }

        public SearchSummaryReportsViewModel SearchView { get; set; }
        public List<SummaryReportsItem> DetailsList { get; set; }
        public ExportSummaryReportsViewModel ExportModel { get; set; }

        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }
    }

    public class SearchSummaryReportsViewModel
    {

        public string Lack10No { get; set; }
        public SelectList Lack10NoList { get; set; }

        public string NppbkcId { get; set; }
        public SelectList NppbkcIdList { get; set; }
        public bool isForExport { get; set; }

        public string Poa { get; set; }
        public SelectList PoaList { get; set; }

        public string Creator { get; set; }
        public SelectList CreatorList { get; set; }

        public string Month { get; set; }
        public SelectList MonthList { get; set; }

        public string Year { get; set; }
        public SelectList YearList { get; set; }
    }

    public class SummaryReportsItem
    {
        public long Lack10Id { get; set; }

        public string Lack10No { get; set; }
        public string CeOffice { get; set; }
        public string BasedOn { get; set; }
        public string LicenseNumber { get; set; }
        public string Kppbc { get; set; }
        public string SubmissionDate { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string PoaApproved { get; set; }
        public string ManagerApproved { get; set; }
        public string Status { get; set; }
        public string CompletedDate { get; set; }
        public string Creator { get; set; }

        public List<string> FaCode { get; set; }
        public List<string> BrandDesc { get; set; }
        public List<string> Werks { get; set; }
        public List<string> PlantName { get; set; }
        public List<string> Type { get; set; }
        public List<string> WasteValue { get; set; }
        public List<string> Uom { get; set; }
    }

    public class ExportSummaryReportsViewModel
    {
        public string Lack10Number { get; set; }
        public string NppbkcId { get; set; }
        public string PoaSearch { get; set; }
        public string CreatorSearch { get; set; }
        public string MonthSearch { get; set; }
        public string YearSearch { get; set; }
        public bool NoRow { get; set; }
        public bool Lack10No { get; set; }
        public bool CeOffice { get; set; }
        public bool BasedOn { get; set; }
        public bool LicenseNumber { get; set; }
        public bool Kppbc { get; set; }
        public bool SubmissionDate { get; set; }
        public bool Month { get; set; }
        public bool Year { get; set; }
        public bool PoaApproved { get; set; }
        public bool ManagerApproved { get; set; }
        public bool Status { get; set; }
        public bool CompletedDate { get; set; }
        public bool Creator { get; set; }

        public bool FaCode { get; set; }
        public bool BrandDesc { get; set; }
        public bool Werks { get; set; }
        public bool PlantName { get; set; }
        public bool Type { get; set; }
        public bool WasteValue { get; set; }
        public bool Uom { get; set; }
    }
}