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
    
    public partial class LFA1
    {
        public LFA1()
        {
            this.ZAIDM_EX_NPPBKC = new HashSet<ZAIDM_EX_NPPBKC>();
        }
    
        public string LIFNR { get; set; }
        public string NAME1 { get; set; }
        public string NAME2 { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public string ORT01 { get; set; }
        public string STRAS { get; set; }
    
        public virtual ICollection<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
    }
}
