using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class ProductionDto
    {
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public decimal? QtyUnpacked { get; set; }
        public decimal? QtyPacked { get; set; }
        public string Uom { get; set; }
        public DateTime ProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }

    }
}
