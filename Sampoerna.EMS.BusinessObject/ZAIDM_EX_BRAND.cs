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
    
    public partial class ZAIDM_EX_BRAND
    {
        public string WERKS { get; set; }
        public string FA_CODE { get; set; }
        public string STICKER_CODE { get; set; }
        public string PER_CODE { get; set; }
        public string BRAND_CE { get; set; }
        public string SKEP_NO { get; set; }
        public Nullable<System.DateTime> SKEP_DATE { get; set; }
        public string PROD_CODE { get; set; }
        public string SERIES_CODE { get; set; }
        public string BRAND_CONTENT { get; set; }
        public string MARKET_ID { get; set; }
        public string COUNTRY { get; set; }
        public Nullable<decimal> HJE_IDR { get; set; }
        public string HJE_CURR { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public string TARIF_CURR { get; set; }
        public string COLOUR { get; set; }
        public string EXC_GOOD_TYP { get; set; }
        public string CUT_FILLER_CODE { get; set; }
        public Nullable<decimal> PRINTING_PRICE { get; set; }
        public Nullable<decimal> CONVERSION { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public Nullable<bool> STATUS { get; set; }
        public Nullable<bool> IS_FROM_SAP { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string PER_CODE_DESC { get; set; }
        public string BAHAN_KEMASAN { get; set; }
        public Nullable<decimal> PACKED_ADJUSTED { get; set; }
    
        public virtual ZAIDM_EX_MARKET ZAIDM_EX_MARKET { get; set; }
        public virtual ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }
        public virtual T001W T001W { get; set; }
        public virtual ZAIDM_EX_PRODTYP ZAIDM_EX_PRODTYP { get; set; }
        public virtual ZAIDM_EX_SERIES ZAIDM_EX_SERIES { get; set; }
    }
}
