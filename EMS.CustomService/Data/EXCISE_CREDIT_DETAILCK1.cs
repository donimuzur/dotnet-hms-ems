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
    
    public partial class EXCISE_CREDIT_DETAILCK1
    {
        public long EXCISE_CREDIT_CK1_ID { get; set; }
        public long EXCISE_CREDIT_ID { get; set; }
        public int PERIOD_MONTH { get; set; }
        public int PERIOD_YEAR { get; set; }
        public string PRODUCT_CODE { get; set; }
        public System.DateTime CK1_DATE { get; set; }
        public string CK1_NO { get; set; }
        public int ORDER_QTY { get; set; }
        public decimal CUKAI_AMOUNT { get; set; }
        public long CK1_ID { get; set; }
    
        public virtual MASTER_PRODUCT_TYPE PRODUCT_TYPE { get; set; }
        public virtual CK1 CK1 { get; set; }
        public virtual EXCISE_CREDIT EXCISE_CREDIT { get; set; }
    }
}
