using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class WasteDto
    {
        public string CompanyCodeX { get; set; }
        public string PlantWerksX { get; set; }
        public string FaCodeX { get; set; }
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public decimal? MarkerRejectStickQty { get; set; }
        public decimal? PackerRejectStickQty { get; set; }
        public DateTime WasteProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }
        public decimal? DustWasteGramQty { get; set; }
        public decimal? FloorWasteGramQty { get; set; }
        public decimal? DustWasteStickQty { get; set; }
        public decimal? FloorWasteStickQty { get; set; }
    }
}
