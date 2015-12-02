using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class Pbck4ReportDto
    {
       public Pbck4ReportDto()
       {
           
           ReportDetails = new Pbck4ReportDetailsDto();
           ListPbck4Items = new List<Pbck4ItemReportDto>();
           ListPbck4MatrikCk1 = new List<Pbck4IMatrikCk1ReportDto>();
       }

       public Pbck4ReportDetailsDto ReportDetails { get; set; }
       public List<Pbck4ItemReportDto> ListPbck4Items { get; set; }
       public List<Pbck4IMatrikCk1ReportDto> ListPbck4MatrikCk1 { get; set; }
    }

   public class Pbck4ReportDetailsDto
   {
       public string Pbck4Number { get; set; }
       public string Pbck4Lampiran { get; set; }
       public string TextTo { get; set; }
       public string CityTo { get; set; }
       public string PoaName { get; set; }
       public string PoaTitle { get; set; }
       public string CompanyName { get; set; }
       public string CompanyAddress { get; set; }
       public string NppbkcId { get; set; }
       public string NppbkcDate { get; set; }
       public string PlantCity { get; set; }
       public string PrintDate { get; set; }
       public string RegionOffice { get; set; }
       public string HeaderImage { get; set; }
     
   }

   public class Pbck4ItemReportDto
   {
       public string Seri { get; set; }
       public decimal ReqQty { get; set; }
       public decimal Hje { get; set; }
       public decimal Content { get; set; }
       public decimal Tariff { get; set; }
       public decimal TotalHje { get; set; }
       public decimal TotalCukai { get; set; }
       public string NoPengawas { get; set; }

   }

   public class Pbck4IMatrikCk1ReportDto
   {

       public int Number { get; set; }
       public string SeriesCode { get; set; }
       public decimal Hje { get; set; }
       public string JenisHt { get; set; }
       public decimal Content { get; set; }
       public string BrandName { get; set; }
       public string Ck1No { get; set; }
       public string Ck1Date { get; set; }
       public decimal Ck1OrderQty { get; set; }
       public decimal Ck1RequestedQty { get; set; }
       public decimal Tariff { get; set; }
       public decimal TotalHje { get; set; }
       public decimal TotalCukai { get; set; }
       public string NoPengawas { get; set; }
     
   }
}
