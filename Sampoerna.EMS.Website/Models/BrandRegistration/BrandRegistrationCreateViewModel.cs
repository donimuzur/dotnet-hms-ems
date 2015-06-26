using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistration
{
    public class BrandRegistrationCreateViewModel : BaseModel
    {
        public long BrandId { get; set; }
        public string StickerCode { get; set; }
        public string PlantName { get; set; }
        public string FaCode { get; set; }
        public string PersonalizationCode { get; set; }
        public string PersonalizationCodeDescription { get; set; }
        public string BrandName { get; set; }
        public string SkepNo { get; set; }
        public DateTime SkepDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public string SeriesCode { get; set; }
        public string SeriesValue { get; set; }
        public string Content { get; set; }
        public string MarketCode { get; set; }
        public string MarketDescription { get; set; }
        public string CountryCode { get; set; }
        public string HjeValue { get; set; }
        public string HjeCurrency { get; set; }
        public string Tariff { get; set; }
        public string TariffCurrency { get; set; }
        public string ColourName { get; set; }
        public string GoodType { get; set; }
        public string GoodTypeDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public string Convertion { get; set; }
        public decimal? PrintingPrice { get; set; }
        public string CutFilterCode { get; set; }
    }
}