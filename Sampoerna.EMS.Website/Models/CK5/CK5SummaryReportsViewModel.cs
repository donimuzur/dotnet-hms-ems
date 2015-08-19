using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Sampoerna.EMS.Core;

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

        public Enums.CK5Type Ck5Type { get; set; }

        
    }

    public class CK5SearchSummaryReportsViewModel
    {
        public Enums.CK5Type Ck5Type { get; set; }
        public Enums.CK5Type Ck5TypeList { get; set; }

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

        public DateTime? DateFrom { get; set; }
        public SelectList DateFromList { get; set; }

        public DateTime? DateTo { get; set; }
        public SelectList DateToList { get; set; }

       
      

    }

    public class CK5SummaryReportsItem
    {
        public string Ck5TypeDescription { get; set; }
        public string KppbcCityName { get; set; }
        public string SubmissionNumber { get; set; }
        public string SubmissionDate { get; set; }
        public string ExGoodTypeDesc { get; set; }
        public string ExciseSettlement { get; set; }
        public string ExciseStatus { get; set; }
        public string RequestType { get; set; }
        public string SourcePlant { get; set; }
        public string DestinationPlant { get; set; }

        public long Ck5Id { get; set; }
        
      
    }

    public class CK5ExportSummaryReportsViewModel : CK5SearchSummaryReportsViewModel
    {
        public bool NoRow { get; set; }
        public bool Ck5TypeDescription { get; set; }
        public bool KppbcCityName { get; set; }
        public bool SubmissionNumber { get; set; }
        public bool SubmissionDate { get; set; }
        public bool ExGoodTypeDesc { get; set; }
        public bool ExciseSettlement { get; set; }
        public bool ExciseStatus { get; set; }
        public bool RequestType { get; set; }
        public bool SourcePlant { get; set; }
        public bool DestinationPlant { get; set; }

    }

  

}