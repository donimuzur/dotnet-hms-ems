using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistration
{
    public class BrandRegistrationCreateViewModel : BaseModel
    {
        public long BrandId { get; set; }

        [Required]
        public string StickerCode { get; set; }
        public SelectList StickerCodeList { get; set; }

        [Required]
        public string PlantId { get; set; }
        public SelectList PlantList { get; set; }

        [Required]
        [StringLength(18)]
        public string FaCode { get; set; }

        [Required]
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
        public DateTime? SkepDate { get; set; }

        [Required]
        public string ProductCode { get; set; }
        public SelectList ProductCodeList { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }

        [Required]
        public string SeriesId { get; set; }
        public SelectList SeriesList { get; set; }
        public string SeriesCode { get; set; }
        public string SeriesValue { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string MarketId { get; set; }
        public SelectList MarketCodeList { get; set; }
        public string MarketDescription { get; set; }

        [Required]
        public string CountryId { get; set; }
        public SelectList CountryCodeList { get; set; }


        public decimal? HjeValue
        {
            get { return Convert.ToDecimal(HjeValueStr); }
            set { value = HjeValue; }
        }

        [Required]
        public string HjeValueStr { get; set; }

        [Required]
        public string HjeCurrency { get; set; }
        public SelectList HjeCurrencyList { get; set; }

        public decimal? Tariff
        {
            get { return Convert.ToDecimal(TariffValueStr); }
            set { value = Tariff; }
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
            get { return Convert.ToDecimal(ConversionValueStr); }
            set { value = Conversion; }
        }
        
        public string ConversionValueStr
        {
            get;
            set;
        }
        
        public decimal? PrintingPrice
        {
            get { return Convert.ToDecimal(PrintingPriceValueStr); } 
            set { value = PrintingPrice; }
        }
       
        public string PrintingPriceValueStr { get; set; }

        [StringLength(25)]
        public string CutFilterCode { get; set; }

        public bool IsActive { get; set; }
        
    }
}