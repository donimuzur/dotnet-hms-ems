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
    
    public partial class USER_GROUP
    {
        public USER_GROUP()
        {
            this.PAGE_MAP = new HashSet<PAGE_MAP>();
            this.USER = new HashSet<USER>();
        }
    
        public int GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
    
        public virtual ICollection<PAGE_MAP> PAGE_MAP { get; set; }
        public virtual ICollection<USER> USER { get; set; }
    }
}
