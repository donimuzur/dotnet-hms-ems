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
    
    public partial class CK2
    {
        public CK2()
        {
            this.CK2_DOCUMENT = new HashSet<CK2_DOCUMENT>();
        }
    
        public int CK2_ID { get; set; }
        public string CK2_NUMBER { get; set; }
        public Nullable<System.DateTime> CK2_DATE { get; set; }
        public Nullable<decimal> CK2_VALUE { get; set; }
        public int PBCK3_ID { get; set; }
    
        public virtual ICollection<CK2_DOCUMENT> CK2_DOCUMENT { get; set; }
        public virtual PBCK3 PBCK3 { get; set; }
    }
}