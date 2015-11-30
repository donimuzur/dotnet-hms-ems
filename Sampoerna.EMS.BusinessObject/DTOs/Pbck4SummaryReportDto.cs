namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck4SummaryReportDto
    {
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

        public int? ReportedOn { get; set; }

        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
    }
}
