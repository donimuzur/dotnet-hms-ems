using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class ProductionDto
    
    {
        public string CompanyCodeX { get; set; }
        public string PlantWerksX { get; set; }
        public string FaCodeX { get; set; }
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
        public decimal? QtyProduced { get; set; }
        public string TobaccoProductType { get; set; }
        public decimal? Hje { get; set; }
        public decimal? Tarif { get; set; }
        public string ProdCode { get; set; }
        public string Batch { get; set; }
        public decimal? ProdQtyStick { get; set; }
        public decimal? Qty { get; set; }
        public int? Bundle { get; set; }
        public string Market { get; set; }
        public string Docgmvter { get; set; }
        public string MatDoc { get; set; }
        public string Ordr { get; set; }
    }
}
