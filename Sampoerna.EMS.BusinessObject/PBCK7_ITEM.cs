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
    
    public partial class PBCK7_ITEM
    {
        public long PBCK7_ITEM_ID { get; set; }
        public int PBCK7_ID { get; set; }
        public string FA_CODE { get; set; }
        public string PRODUCT_ALIAS { get; set; }
        public string BRAND_CE { get; set; }
        public Nullable<decimal> BRAND_CONTENT { get; set; }
        public Nullable<decimal> PBCK7_QTY { get; set; }
        public Nullable<decimal> BACK1_QTY { get; set; }
        public string SERIES_VALUE { get; set; }
        public Nullable<decimal> HJE { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public Nullable<int> FISCAL_YEAR { get; set; }
        public Nullable<decimal> EXCISE_VALUE { get; set; }
        public string STICKER_CODE { get; set; }
    
        public virtual PBCK7 PBCK7 { get; set; }
    }
}
