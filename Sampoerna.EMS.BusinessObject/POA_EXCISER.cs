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
    
    public partial class POA_EXCISER
    {
        public long EXCISER_ID { get; set; }
        public string POA_ID { get; set; }
        public bool IS_ACTIVE_EXCISER { get; set; }
    
        public virtual POA POA { get; set; }
    }
}
