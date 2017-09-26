﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistration
{
    public class BrandRegistrationEditViewModel : BaseModel
    {
       
        [Required]
        [StringLength(18)]
        public string StickerCode { get; set; }

        public SelectList StickerCodeList { get; set; }
        [Required]
        public string PlantId { get; set; }
        public SelectList PlantList { get; set; }

        [Required]
        [StringLength(18)]
        public string FaCode { get; set; }

        //[Required]
        public string PersonalizationCode { get; set; }
        public SelectList PersonalizationCodeList { get; set; }
        public string PersonalizationCodeDescription { get; set; }

        [Required]
        [StringLength(60)]
        public string BrandName { get; set; }

        [Required]
        [StringLength(30)]
        public string SkepNo { get; set; }

        [Required]
        [UIHint("FormatDateTime")]
        public DateTime SkepDate { get; set; }

        [Required]
        public string ProductCode { get; set; }
        public SelectList ProductCodeList { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }

        [Required]
        public string SeriesId { get; set; }
        public SelectList SeriesList { get; set; }
        public int SeriesCode { get; set; }
        public int SeriesValue { get; set; }

       
        public string Content { get; set; }

        [Required]
        public string MarketId { get; set; }
        public SelectList MarketCodeList { get; set; }
        public string MarketDescription { get; set; }

        [Required]
        public string CountryId { get; set; }
        public SelectList CountryCodeList { get; set; }

       
        [Required]
        public string HjeValueStr { get; set; }
        public decimal? HjeValue
        {
            get; set;
        }
        [Required]
        public string HjeCurrency { get; set; }
        public SelectList HjeCurrencyList { get; set; }

        public decimal? Tariff
        {
            get; set;
        }
        [Required]
        public string TariffValueStr { get; set; }
       
      

        [Required]
        public string TariffCurrency { get; set; }
        public SelectList TariffCurrencyList { get; set; }

        [Required]
        [StringLength(70)]
        public string ColourName { get; set; }

        [Required]
        public string GoodType { get; set; }
        public SelectList GoodTypeList { get; set; }
        public string GoodTypeDescription { get; set; }

        [Required]
      
        public DateTime? StartDate { get; set; }

        [Required]
       
        public DateTime? EndDate { get; set; }


        public decimal? Conversion
        {
            get;
            set;
        }
       
        public string ConversionValueStr
        {
            get;
            set;
        }

        public decimal? PrintingPrice
        {
            get; set;
        }
        
        public string PrintingPriceValueStr { get; set; }

        [StringLength(25)]
        public string CutFillerCode { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }
        public bool IsFromSAP { get; set; }

        public SelectList CutFillerCodeList { get; set; }

        public bool IsAllowDelete { get; set; }

        public string PlantName { get; set; }

        public bool? BoolIsDeleted { get; set; }

        public string IsDeleted { get; set; }

        public string BahanKemasan { get; set; }
        public SelectList BahanKemasanList { get; set; }
        public bool IsPackedAdjusted { get; set; }
        public string SAPBrandDescription { get; set; }
    }
}