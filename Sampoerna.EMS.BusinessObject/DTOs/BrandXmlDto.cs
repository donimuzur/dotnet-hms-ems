using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class BrandXmlDto
    {
        public string WERKS { get; set; }
        public string FA_CODE { get; set; }
        public string STICKER_CODE { get; set; }
        public string PER_CODE { get; set; }
        public string BRAND_CE { get; set; }
        public string SKEP_NO { get; set; }
        public DateTime? SKEP_DATE { get; set; }
        public string PROD_CODE { get; set; }
        public string SERIES_CODE { get; set; }
        public string BRAND_CONTENT { get; set; }
        public string MARKET_ID { get; set; }
        public string COUNTRY { get; set; }
        public decimal? HJE_IDR { get; set; }
        public string HJE_CURR { get; set; }
        public decimal? TARIFF { get; set; }
        public string TARIF_CURR { get; set; }
        public string COLOUR { get; set; }
        public string EXC_GOOD_TYP { get; set; }
        public string CUT_FILLER_CODE { get; set; }
        public decimal? PRINTING_PRICE { get; set; }
        public decimal? CONVERSION { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public bool? STATUS { get; set; }
        public bool? IS_FROM_SAP { get; set; }
        public bool? IS_DELETED { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string PER_CODE_DESC { get; set; }
        public string BAHAN_KEMASAN { get; set; }
        public bool? PACKED_ADJUSTED { get; set; }

        public string XmlPath { get; set; }
    }
}
