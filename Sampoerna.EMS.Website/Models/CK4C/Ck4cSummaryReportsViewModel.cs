using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CSummaryReportsViewModel : BaseModel
    {
        public Ck4CSummaryReportsViewModel()
        {
            SearchView = new Ck4CSearchSummaryReportsViewModel();
            DetailsList = new List<Ck4CSummaryReportsItem>();
        }

        public Ck4CSearchSummaryReportsViewModel SearchView { get; set; }
        public List<Ck4CSummaryReportsItem> DetailsList { get; set; }
        public Ck4CExportSummaryReportsViewModel ExportModel { get; set; }
    }

    public class Ck4CSearchSummaryReportsViewModel
    {

        public string Ck4CNo { get; set; }
        public SelectList Ck4CNoList { get; set; }

        //public int? YearFrom { get; set; }
        //public SelectList YearFromList { get; set; }

        //public int? YearTo { get; set; }
        //public SelectList YearToList { get; set; }


        public string PlantId { get; set; }
        public SelectList PlantIdList { get; set; }

    }

    public class Ck4CSummaryReportsItem
    {
        public long Ck4CId { get; set; }

        public string Ck4CNo { get; set; }
        public string CeOffice { get; set; }
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        
        public string LicenseNumber { get; set; }
        public string ReportPeriod { get; set; }
        
        public string Status { get; set; }

    }

    public class Ck4CExportSummaryReportsViewModel : Ck4CSearchSummaryReportsViewModel
    {
        public bool NoRow { get; set; }
        public bool Ck4CNumber { get; set; }
        public bool CeOffice { get; set; }
        public new bool PlantId { get; set; }
        public bool PlantDescription { get; set; }
        public bool LicenseNumber { get; set; }
        public bool ReportPeriod { get; set; }        
        public bool Status { get; set; }

    }
}