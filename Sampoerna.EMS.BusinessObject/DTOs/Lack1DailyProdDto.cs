using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1DailyProdDto
    {
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string FaCode { get; set; }
        public string FaCodeDescription { get; set; }
        public DateTime ProductionDate { get; set; }
        public decimal? ProdQty { get; set; }
        public string ProdUom { get; set; }
        public decimal? RejectParkerQty { get; set; }
        public string RejectParkerUom { get; set; }
        
    }
}
