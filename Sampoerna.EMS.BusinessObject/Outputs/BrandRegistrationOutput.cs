using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class BrandRegistrationOutput : BLLBaseOutput
    {
        //ZAIDM_EX_BRAND
        public long BrandIdZaidmExBrand { get; set; }
        public string StickerCode { get; set; }
        public long PlantId { get; set; }
        public string FaCode { get; set; }
        public long PerId { get; set; }
        public string BrandCe { get; set; }
        public string SkepNp { get; set; }
        public DateTime SkepDateTime { get; set; }
        public int? ProductId { get; set; }
        public long SeriesId { get; set; }
        public long MarketId { get; set; }
        public string Colour { get; set; }
        public int? CountryIdZaidmExBrand { get; set; }
        public string CutFilterCode { get; set; }
        public decimal? PrintingPrice { get; set; }
        public int? HjeCurr { get; set; }
        public decimal? HjdIdr { get; set; }
        public decimal? Tariff  { get; set; }
        public int? TariffCur { get; set; }
        public int GoodTypId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool? IsActive { get; set; }
    
        //Country
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        //Currency
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }

        //T1001W
        public long PlantIdT1001W { get; set; }
        public string Werks { get; set; }
        public string Name1 { get; set; }
        public string Ort01 { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Skeptis { get; set; }
        public long? NppbckId { get; set; }
        public bool? IsMainPlant { get; set; }
        public int? RecievedMaterialTypeId { get; set; }
        public DateTime? CreateDate { get; set; }

        //ZAIDM_EX_GOODTYP
        public int GoodTypeId { get; set; }
        public int? ExcGoodType { get; set; }
        public string ExtTypDesc { get; set; }
        public DateTime? CreateTime { get; set; }

        //ZAIDM_EX_MARKET
        public long MarketIdZaidmExMarket { get; set; }
        public int? MarketCode { get; set; }
        public string MarketDesc { get; set; }
        public DateTime? CreateDateTimeZaidmExMarket { get; set; }

        //ZAIDM_EX_PCODE
        public long PerIdZaimExPCode { get; set; }
        public int? PerCode { get; set; }
        public string PerDesc { get; set; }
        public DateTime? CreateDateTimeZaimExPCode { get; set; }

        //ZAIDM_EX_PRODTYP
        public int ProductIdZaimExProdTyp { get; set; }
        public int? ProductCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public DateTime? CreateDateZaimExProdTyp { get; set; }

        //ZAIDM_EX_SERIES
        public long SeriesIdZaimExSeries { get; set; }
        public int? SeriesCode { get; set; }
        public int? SeriesValue { get; set; }
        public DateTime CreateDateZaimExSeries { get; set; }

    }
}
