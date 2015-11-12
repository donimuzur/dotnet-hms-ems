using System;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.WasteStock
{
    public class WasteStockFormViewModel : BaseModel
    {
        public int WasteStockId { get; set; }

        public string PlantId { get; set; }
        public SelectList PlantList { get; set; }
        public string PlantDescription { get; set; }

        public string MaterialNumber { get; set; }
        
        public decimal Stock { get; set; }

        public string Uom { get; set; }
        public string UomDescription { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}