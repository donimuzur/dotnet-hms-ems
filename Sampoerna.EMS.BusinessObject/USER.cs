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
    
    public partial class USER
    {
        public USER()
        {
            this.ZAIDM_EX_POA = new HashSet<ZAIDM_EX_POA>();
        }
    
        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public Nullable<int> MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
    
        public virtual ICollection<ZAIDM_EX_POA> ZAIDM_EX_POA { get; set; }
    }
}
