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
    }
}
