using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PBCK4
{
    public class Pbck4SummaryReportsViewModel : BaseModel
    {
        public Pbck4SummaryReportsViewModel()
        {
            SearchView = new Pbck4SearchSummaryReportsViewModel();
            DetailsList = new List<Pbck4SummaryReportsItem>();
        }

        public Pbck4SearchSummaryReportsViewModel SearchView { get; set; }
        public List<Pbck4SummaryReportsItem> DetailsList { get; set; }
        public Pbck4ExportSummaryReportsViewModel ExportModel { get; set; }

    }

    public class Pbck4SearchSummaryReportsViewModel
    {
      
        public string Pbck4No { get; set; }
        public SelectList Pbck4NoList { get; set; }

        public int? YearFrom { get; set; }
        public SelectList YearFromList { get; set; }

        public int? YearTo { get; set; }
        public SelectList YearToList { get; set; }


        public string PlantId { get; set; }
        public SelectList PlantIdList { get; set; }

    }

    public class Pbck4SummaryReportsItem
    {
        public long Pbck4Id { get; set; }

        public string Pbck4Date { get; set; }
        public string Pbck4No { get; set; }
        public string CeOffice { get; set; }
        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string ProductType { get; set; }
        public string FiscalYear { get; set; }
        public string SeriesCode { get; set; }
        public string RequestedQty { get; set; }
        public string ExciseValue { get; set; }
        public string Remarks { get; set; }
        public string Back1Date { get; set; }
        public string Back1Number { get; set; }
        public string Ck3Date { get; set; }
        public string Ck3Number { get; set; }
        public string Ck3Value { get; set; }
        public string PrintingCost { get; set; }
        public string CompensatedCk1Date { get; set; }
        public string CompensatedCk1Number { get; set; }
        public string PaymentDate { get; set; }
        public string Status { get; set; }

    }

    public class Pbck4ExportSummaryReportsViewModel : Pbck4SearchSummaryReportsViewModel
    {
        public bool NoRow { get; set; }
        public bool Pbck4Date { get; set; }
        public bool Pbck4Number { get; set; }
        public bool CeOffice { get; set; }
        public bool Brand { get; set; }
        public bool Content { get; set; }
        public bool Hje { get; set; }
        public bool Tariff { get; set; }
        public bool ProductType { get; set; }
        public bool FiscalYear { get; set; }
        public bool SeriesCode { get; set; }
        public bool RequestedQty { get; set; }
        public bool ExciseValue { get; set; }
        public bool Remarks { get; set; }
        public bool Back1Date { get; set; }
        public bool Back1Number { get; set; }
        public bool Ck3Date { get; set; }
        public bool Ck3Number { get; set; }
        public bool Ck3Value { get; set; }
        public bool PrintingCost { get; set; }
        public bool CompensatedCk1Date { get; set; }
        public bool CompensatedCk1Number { get; set; }
        public bool PaymentDate { get; set; }
        public bool Status { get; set; }
        
    }

  
}