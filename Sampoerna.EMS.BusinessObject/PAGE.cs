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
    
    public partial class PAGE
    {
        public PAGE()
        {
            this.PAGE1 = new HashSet<PAGE>();
            this.PAGE_MAP = new HashSet<PAGE_MAP>();
            this.MASTER_DATA_APPROVE_SETTING = new HashSet<MASTER_DATA_APPROVE_SETTING>();
            this.MASTER_DATA_APPROVAL1 = new HashSet<MASTER_DATA_APPROVAL>();
        }
    
        public int PAGE_ID { get; set; }
        public string PAGE_NAME { get; set; }
        public string PAGE_URL { get; set; }
        public string MENU_NAME { get; set; }
        public Nullable<int> PARENT_PAGE_ID { get; set; }
        public string MAIN_TABLE { get; set; }
    
        public virtual ICollection<PAGE> PAGE1 { get; set; }
        public virtual PAGE PAGE2 { get; set; }
        public virtual ICollection<PAGE_MAP> PAGE_MAP { get; set; }
        public virtual ICollection<MASTER_DATA_APPROVE_SETTING> MASTER_DATA_APPROVE_SETTING { get; set; }
        public virtual MASTER_DATA_APPROVAL MASTER_DATA_APPROVAL { get; set; }
        public virtual ICollection<MASTER_DATA_APPROVAL> MASTER_DATA_APPROVAL1 { get; set; }
    }
}
