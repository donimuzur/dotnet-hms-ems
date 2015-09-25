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
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string LicenseNumber { get; set; }
        public string ReportPeriod { get; set; }
        public string Status { get; set; }

        public string ProductionDate { get; set; }
        public string TobaccoProductType { get; set; }
        public string BrandDescription { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string ProducedQty { get; set; }
        public string PackedQty { get; set; }
        public string Content { get; set; }
        public string UnPackQty { get; set; }
    }
}
