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
    
    public partial class CK4C_ITEM
    {
        public long CK4C_ITEM_ID { get; set; }
        public int CK4C_ID { get; set; }
        public string FA_CODE { get; set; }
        public string WERKS { get; set; }
        public decimal PROD_QTY { get; set; }
        public string UOM_PROD_QTY { get; set; }
        public System.DateTime PROD_DATE { get; set; }
        public Nullable<decimal> HJE_IDR { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public string PROD_CODE { get; set; }
        public Nullable<decimal> PACKED_QTY { get; set; }
        public Nullable<decimal> UNPACKED_QTY { get; set; }
        public Nullable<int> CONTENT_PER_PACK { get; set; }
        public Nullable<int> PACKED_IN_PACK { get; set; }
        public string REMARKS { get; set; }
        public Nullable<decimal> ZB { get; set; }
        public Nullable<decimal> PACKED_ADJUSTED { get; set; }
        public Nullable<int> PACKED_IN_PACK_ZB { get; set; }
    
        public virtual CK4C CK4C { get; set; }
        public virtual UOM UOM { get; set; }
    }
}
