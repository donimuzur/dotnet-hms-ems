using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class Ck4CSummaryReportDto
    {
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
}
