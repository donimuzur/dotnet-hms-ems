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
    
    public partial class DOCUMENT_TYPE
    {
        public DOCUMENT_TYPE()
        {
            this.PBCK3_CK5 = new HashSet<PBCK3_CK5>();
            this.PBCK3_7 = new HashSet<PBCK3_7>();
        }
    
        public int DOCUMENT_TYPE_ID { get; set; }
        public string DOCUMENT_TYPE_DESC { get; set; }
    
        public virtual ICollection<PBCK3_CK5> PBCK3_CK5 { get; set; }
        public virtual ICollection<PBCK3_7> PBCK3_7 { get; set; }
    }
}
