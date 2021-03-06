﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistration
{
    public class BrandRegistrationIndexViewModel : BaseModel
    {
        public BrandRegistrationIndexViewModel()
        {
            Details = new List<BrandRegistrationDetail>();
        }

        public List<BrandRegistrationDetail> Details;
    }

    public class BrandRegistrationDetail
    {
        public string StickerCode { get; set; }
        public string PlantName { get; set; }
        public string FaCode { get; set; }
        public string BrandName { get; set; }
        public string SeriesValue { get; set; }
        public decimal? Conversion { get; set; }
        public decimal? PrintingPrice { get; set; }
        public string CutFilterCode { get; set; }
        public string IsDeleted { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}