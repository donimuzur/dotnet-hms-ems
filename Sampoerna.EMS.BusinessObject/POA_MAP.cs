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
    
    public partial class POA_MAP
    {
        public int POA_MAP_ID { get; set; }
        public string NPPBKC_ID { get; set; }
        public string WERKS { get; set; }
        public string POA_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual T001W T001W { get; set; }
        public virtual ZAIDM_EX_NPPBKC ZAIDM_EX_NPPBKC { get; set; }
        public virtual POA POA { get; set; }
    }
}
