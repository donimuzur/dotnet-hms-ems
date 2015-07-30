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
       
        public string CompanyCodeSource { get; set; }
        public SelectList CompanyCodeSourceList { get; set; }

        public string CompanyCodeDest { get; set; }
        public SelectList CompanyCodeDestList { get; set; }

        public string NppbkcIdSource { get; set; }
        public SelectList NppbkcIdSourceList { get; set; }

        public string NppbkcIdDest { get; set; }
        public SelectList NppbkcIdDestList { get; set; }

        public string PlantSource { get; set; }
        public SelectList PlantSourceList { get; set; }

        public string PlantDest { get; set; }
        public SelectList PlantDestList { get; set; }

        public DateTime DateFrom { get; set; }
        public SelectList DateFromList { get; set; }

        public DateTime DateTo { get; set; }
        public SelectList DateToList { get; set; }

       
      

    }

    public class CK5SummaryReportsItem
    {
        public long Ck5Id { get; set; }

        public string ExciseStatus { get; set; }

        public string DocumentNumber { get; set; }

        public string SubmissionDate { get; set; }

        public string SealingNotifDate { get; set; }

        public string SealingNotifNumber { get; set; }

        public string UnSealingNotifDate { get; set; }

        public string unSealingNotifNumber { get; set; }

        public string Kppbc { get; set; }

      

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