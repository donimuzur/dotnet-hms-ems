using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class ProductionUploadItemsInput
    {
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string StickerCode { get; set; }
        public string BrandDescription { get; set; }
        public decimal? Qty { get; set; }
        public decimal? QtyPacked { get; set; }
        public string Uom { get; set; }
        public string ProductionDate { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
}
