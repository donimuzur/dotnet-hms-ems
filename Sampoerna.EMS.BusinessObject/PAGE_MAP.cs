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
    
    public partial class PAGE_MAP
    {
        public int PAGE_MAP_ID { get; set; }
        public int PAGE_ID { get; set; }
        public string BROLE { get; set; }
    
        public virtual PAGE PAGE { get; set; }
        public virtual USER_BROLE USER_BROLE { get; set; }
    }
}