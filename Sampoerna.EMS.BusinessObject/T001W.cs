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
    
    public partial class T001W
    {
        public T001W()
        {
            this.VIRTUAL_PLANT_MAP = new HashSet<VIRTUAL_PLANT_MAP>();
            this.VIRTUAL_PLANT_MAP1 = new HashSet<VIRTUAL_PLANT_MAP>();
            this.ZAIDM_EX_BRAND = new HashSet<ZAIDM_EX_BRAND>();
            this.POA_MAP = new HashSet<POA_MAP>();
            this.ZAIDM_EX_MATERIAL = new HashSet<ZAIDM_EX_MATERIAL>();
        }
    
        public string WERKS { get; set; }
        public string NAME1 { get; set; }
        public string ORT01 { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public string SKEPTIS { get; set; }
        public string NPPBKC_ID { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<bool> IS_MAIN_PLANT { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
    
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP1 { get; set; }
        public virtual ZAIDM_EX_NPPBKC ZAIDM_EX_NPPBKC { get; set; }
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual USER USER { get; set; }
        public virtual ICollection<POA_MAP> POA_MAP { get; set; }
        public virtual ICollection<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
    }
}
