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
    
    public partial class LACK1_INCOME_DETAIL
    {
        public long LACK1_INCOME_ID { get; set; }
        public int LACK1_ID { get; set; }
        public long CK5_ID { get; set; }
        public decimal AMOUNT { get; set; }
    
        public virtual CK5 CK5 { get; set; }
        public virtual LACK1 LACK1 { get; set; }
    }
}
