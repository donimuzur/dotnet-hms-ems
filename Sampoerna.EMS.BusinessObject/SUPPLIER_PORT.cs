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
    
    public partial class SUPPLIER_PORT
    {
        public SUPPLIER_PORT()
        {
            this.PBCK1 = new HashSet<PBCK1>();
        }
    
        public int SUPPLIER_PORT_ID { get; set; }
        public string PORT_NAME { get; set; }
    
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
    }
}
