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
    
    public partial class ZAIDM_EX_PCODE
    {
        public ZAIDM_EX_PCODE()
        {
            this.ZAIDM_EX_BRAND = new HashSet<ZAIDM_EX_BRAND>();
        }
    
        public int PER_CODE { get; set; }
        public string PER_DESC { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
    }
}
