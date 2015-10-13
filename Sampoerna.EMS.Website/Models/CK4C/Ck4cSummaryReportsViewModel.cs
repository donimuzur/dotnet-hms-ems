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

        public string PlantId { get; set; }
        public SelectList PlantIdList { get; set; }

    }

    public class Ck4CSummaryReportsItem
    {
        public long Ck4CId { get; set; }

        public string Ck4CNo { get; set; }
        public string CeOffice { get; set; }
        public string BasedOn { get; set; }
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string LicenseNumber { get; set; }
        public string ReportPeriod { get; set; }
        public string Period { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Status { get; set; }

        public List<string> ProductionDate { get; set; }
        public List<string> FaCode { get; set; }
        public List<string> TobaccoProductType { get; set; }
        public List<string> BrandDescription { get; set; }
        public List<decimal> Hje { get; set; }
        public List<decimal> Tariff { get; set; }
        public List<decimal> Content { get; set; }
        public List<decimal> PackedQty { get; set; }
        public List<decimal> PackedQtyInPack { get; set; }
        public List<decimal> UnPackQty { get; set; }
        public List<decimal> ProducedQty { get; set; }
        public List<string> UomProducedQty { get; set; }
        public List<string> Remarks { get; set; }
    }

    public class Ck4CExportSummaryReportsViewModel
    {
        public string Ck4CNumber { get; set; }
        public string Plant { get; set; }
        public bool NoRow { get; set; }
        public bool Ck4CNo { get; set; }
        public bool CeOffice { get; set; }
        public bool BasedOn { get; set; }
        public bool PlantId { get; set; }
        public bool PlantDescription { get; set; }
        public bool LicenseNumber { get; set; }
        public bool ReportPeriod { get; set; }
        public bool Period { get; set; }
        public bool Month { get; set; }
        public bool Year { get; set; }
        public bool Status { get; set; }

        public bool ProductionDate { get; set; }
        public bool FaCode { get; set; }
        public bool TobaccoProductType { get; set; }
        public bool BrandDescription { get; set; }
        public bool Hje { get; set; }
        public bool Tariff { get; set; }
        public bool Content { get; set; }
        public bool PackedQty { get; set; }
        public bool PackedQtyInPack { get; set; }
        public bool UnPackQty { get; set; }
        public bool ProducedQty { get; set; }
        public bool UomProducedQty { get; set; }
        public bool Remarks { get; set; }
    }
}