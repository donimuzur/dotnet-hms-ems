//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sampoerna.EMS.CustomService.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class EXCISE_CREDIT_ADJUST_CALDETAIL
    {
        public long EXCISE_CREDIT_ADJUST_ID { get; set; }
        public string BRAND_CE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal OLD_TARIFF { get; set; }
        public decimal NEW_TARIFF { get; set; }
        public decimal INCREASE_TARIFF { get; set; }
        public decimal CK1_AMOUNT { get; set; }
        public decimal WEIGHTED_INCREASE { get; set; }
        public long EXCISE_CREDIT_ID { get; set; }
    
        public virtual MASTER_PRODUCT_TYPE PRODUCT_TYPE { get; set; }
        public virtual EXCISE_CREDIT EXCISE_CREDIT { get; set; }
    }
}
