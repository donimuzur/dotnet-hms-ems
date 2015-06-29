using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistration
{
    public class BrandRegistrationEditViewModel : BaseModel
    {
        public long BrandId { get; set; }

        [Required]
        [StringLength(18)]
        public string StickerCode { get; set; }

        [Required]
        public long PlantId { get; set; }
        public SelectList PlantList { get; set; }

        [Required]
        [StringLength(18)]
        public string FaCode { get; set; }

        [Required]
        public long PersonalizationCode { get; set; }
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
        public DateTime? SkepDate { get; set; }

        [Required]
        public int ProductCode { get; set; }
        public SelectList ProductCodeList { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }

        [Required]
        public long SeriesId { get; set; }
        public SelectList SeriesList { get; set; }
        public string SeriesCode { get; set; }
        public string SeriesValue { get; set; }

        public string Content { get; set; }

        [Required]
        public long MarketCode { get; set; }
        public SelectList MarketCodeList { get; set; }
        public string MarketDescription { get; set; }

        [Required]
        public int CountryCode { get; set; }
        public SelectList CountryCodeList { get; set; }

        [Required]
        [UIHint("FormatDecimal")]
        public decimal? HjeValue { get; set; }

        [Required]
        public int? HjeCurrency { get; set; }
        public SelectList HjeCurrencyList { get; set; }

        [Required]
        [UIHint("FormatDecimal")]
        public decimal? Tariff { get; set; }

        [Required]
        public int? TariffCurrency { get; set; }
        public SelectList TariffCurrencyList { get; set; }

        [Required]
        [StringLength(70)]
        public string ColourName { get; set; }

        [Required]
        public int GoodType { get; set; }
        public SelectList GoodTypeList { get; set; }
        public string GoodTypeDescription { get; set; }

        [Required]
        [UIHint("FormatDateTime")]
        public DateTime? StartDate { get; set; }

        [Required]
        [UIHint("FormatDateTime")]
        public DateTime? EndDate { get; set; }

        public string Convertion { get; set; }

        [UIHint("FormatDecimal")]
        public decimal? PrintingPrice { get; set; }

        [StringLength(25)]
        public string CutFilterCode { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }
        public bool IsFromSAP { get; set; }
    }
}