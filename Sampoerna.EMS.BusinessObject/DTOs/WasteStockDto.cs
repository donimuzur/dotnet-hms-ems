using System;


namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class WasteStockDto
    {
        public int WASTE_STOCK_ID { get; set; }
        public string WERKS { get; set; }
        public string MATERIAL_NUMBER { get; set; }
        public decimal STOCK { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }

        public string PlantDescription { get; set; }
    }
}
