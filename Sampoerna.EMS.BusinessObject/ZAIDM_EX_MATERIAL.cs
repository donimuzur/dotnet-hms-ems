//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sampoerna.EMS.BusinessObject
{
    using System;
    using System.Collections.Generic;
    
    public partial class ZAIDM_EX_MATERIAL
    {
        public ZAIDM_EX_MATERIAL()
        {
            this.CK5_MATERIAL = new HashSet<CK5_MATERIAL>();
        }
    
        public long MATERIAL_ID { get; set; }
        public string STICKER_ID { get; set; }
        public string STICKER_CODE { get; set; }
        public long PLANT_ID { get; set; }
        public string FA_CODE { get; set; }
        public long PER_ID { get; set; }
        public string BRAND_CE { get; set; }
        public string SKEP_NP { get; set; }
        public System.DateTime SKEP_DATE { get; set; }
        public int PRODUCT_ID { get; set; }
        public long SERIES_ID { get; set; }
        public long MARKET_ID { get; set; }
        public string COLOUR { get; set; }
        public Nullable<int> COUNTRY_ID { get; set; }
        public string CUT_FILLER_CODE { get; set; }
        public Nullable<decimal> PRINTING_PRICE { get; set; }
        public Nullable<decimal> HJE_IDR { get; set; }
        public Nullable<int> HJE_CURR { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public Nullable<int> TARIFF_CURR { get; set; }
        public Nullable<int> GOODTYP_ID { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
    
        public virtual ICollection<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual COUNTRY COUNTRY { get; set; }
        public virtual CURRENCY CURRENCY { get; set; }
        public virtual CURRENCY CURRENCY1 { get; set; }
        public virtual T1001W T1001W { get; set; }
        public virtual ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }
        public virtual ZAIDM_EX_MARKET ZAIDM_EX_MARKET { get; set; }
        public virtual ZAIDM_EX_MATERIAL ZAIDM_EX_MATERIAL1 { get; set; }
        public virtual ZAIDM_EX_MATERIAL ZAIDM_EX_MATERIAL2 { get; set; }
        public virtual ZAIDM_EX_PCODE ZAIDM_EX_PCODE { get; set; }
        public virtual ZAIDM_EX_PRODTYP ZAIDM_EX_PRODTYP { get; set; }
        public virtual ZAIDM_EX_SERIES ZAIDM_EX_SERIES { get; set; }
    }
}
