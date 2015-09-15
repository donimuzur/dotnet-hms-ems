using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.PRODUCTION
{
    public class ProductionUploadViewModel : BaseModel
    {
       
        public List<ProductionUploadItems> UploadItems { get; set; }
    }

    public class ProductionUploadDetail : ProductionUploadViewModel
    {
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public decimal? QtyUnpacked { get; set; }
        public decimal? QtyPacked { get; set; }
        public string Uom { get; set; }
        public string ProductionDate { get; set; }
    }

}