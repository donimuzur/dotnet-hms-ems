using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5SummaryReportsViewModel : BaseModel
    {
        public CK5SummaryReportsViewModel()
        {
            SearchView = new CK5SearchSummaryReportsViewModel();
            DetailsList = new List<CK5SummaryReportsItem>();
        }

        public CK5SearchSummaryReportsViewModel SearchView { get; set; }
        public List<CK5SummaryReportsItem> DetailsList { get; set; }

        public CK5ExportSummaryReportsViewModel ExportModel { get; set; }
    }

    public class CK5SearchSummaryReportsViewModel
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

    public class CK5SummaryReportsItem
    {
        public long Ck5Id { get; set; }

        public string Company { get; set; }

        public string Nppbkc { get; set; }

        public string Kppbc { get; set; }

        public string DocumentNumber { get; set; }

        public string Address { get; set; }

    }

    public class CK5ExportSummaryReportsViewModel
    {
        public bool Company { get; set; }
        public bool Nppbkc { get; set; }
        public bool Kppbc { get; set; }
        public bool DocumentNumber { get; set; }

        public string CompanyCode { get; set; }
    }
}