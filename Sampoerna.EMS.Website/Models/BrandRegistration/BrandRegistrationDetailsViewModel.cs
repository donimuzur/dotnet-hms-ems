using System;
using System.ComponentModel.DataAnnotations;

namespace Sampoerna.EMS.Website.Models.BrandRegistration
{
    public class BrandRegistrationDetailsViewModel : BaseModel
    {
       public string StickerCode { get; set; }
        public string PlantName { get; set; }
        public string FaCode { get; set; }
        public string PersonalizationCode { get; set; }
        public string PersonalizationCodeDescription { get; set; }
        public string BrandName { get; set; }
        public string SkepNo { get; set; }

        [UIHint("FormatDateTime")]
        public DateTime? SkepDate { get; set; }
        
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public string SeriesCode { get; set; }
        public string SeriesValue { get; set; }
        public string Content { get; set; }
        public string MarketCode { get; set; }
        public string MarketDescription { get; set; }
        public string CountryCode { get; set; }
        public decimal HjeValue { get; set; }
        [Required]
        public string HjeValueStr { get; set; }
        public string HjeCurrency { get; set; }
        public decimal? Tariff { get; set; }

        public string TariffValueStr { get; set; }
        public string TariffCurrency { get; set; }
        public string ColourName { get; set; }
        public string GoodType { get; set; }
        public string GoodTypeDescription { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; }
        [Required]
        public decimal? Conversion { get; set; }
        
        
        [Required]
        public decimal? PrintingPrice { get; set; }
        [Required]
        public string CutFilterCode { get; set; }
        public string IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllowDelete { get; set; }
        public bool? IsFromSap { get; set; }
        public bool? BoolIsDeleted { get; set; }
        
    }
}