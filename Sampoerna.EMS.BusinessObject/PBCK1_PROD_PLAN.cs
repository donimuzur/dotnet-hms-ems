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
    
    public partial class PBCK1_PROD_PLAN
    {
        public long PBCK1_PROD_PLAN_ID { get; set; }
        public Nullable<int> PBCK1_ID { get; set; }
        public string PROD_CODE { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public string BKC_REQUIRED { get; set; }
        public Nullable<int> MONTH { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_ALIAS { get; set; }
    
        public virtual MONTH MONTH1 { get; set; }
        public virtual PBCK1 PBCK1 { get; set; }
    }
}
