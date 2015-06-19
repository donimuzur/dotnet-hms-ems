using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistration
{
    public class BrandRegistrationViewModel : BaseModel
    {
        public BrandRegistrationViewModel()
        {
            Details = new List<DetailBrandRegistration>();
        }

        public List<DetailBrandRegistration> Details;
    }

    public class DetailBrandRegistration
    {
        public string StickerCode { get; set; }
        public string Name1 { get; set; }
        public string FaCode { get; set; }
        public string BrandCe { get; set; }
        public string SeriesValue { get; set; }

        //public convertion
        public decimal? PrintingPrice { get; set; }
        public string CutFilterCode { get; set; }
    }
}