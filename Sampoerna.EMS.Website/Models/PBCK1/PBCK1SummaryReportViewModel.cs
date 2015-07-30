using System.Collections.Generic;
using System.Web.Mvc;

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
        public long Pbck1Id { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string Nppbkc { get; set; }

        public string Kppbc { get; set; }

        public string DocumentNumber { get; set; }

        public string Address { get; set; }
    }

    public class Pbck1ExportSummaryReportsViewModel
    {
        public bool Company { get; set; }
        public bool Nppbkc { get; set; }
        public bool Kppbc { get; set; }
        public bool DocumentNumber { get; set; }

        public string CompanyCode { get; set; }
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

}