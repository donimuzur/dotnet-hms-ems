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
    
    public partial class T1001W
    {
        public T1001W()
        {
            this.CK4C = new HashSet<CK4C>();
            this.CK5 = new HashSet<CK5>();
            this.CK51 = new HashSet<CK5>();
            this.NPPBKC_PLANT = new HashSet<NPPBKC_PLANT>();
            this.PBCK3_7 = new HashSet<PBCK3_7>();
            this.PBCK3_CK5 = new HashSet<PBCK3_CK5>();
            this.PBCK4 = new HashSet<PBCK4>();
            this.VIRTUAL_PLANT_MAP = new HashSet<VIRTUAL_PLANT_MAP>();
            this.VIRTUAL_PLANT_MAP1 = new HashSet<VIRTUAL_PLANT_MAP>();
            this.ZAIDM_EX_BRAND = new HashSet<ZAIDM_EX_BRAND>();
            this.ZAIDM_EX_MATERIAL = new HashSet<ZAIDM_EX_MATERIAL>();
            this.ZAIDM_POA_MAP = new HashSet<ZAIDM_POA_MAP>();
        }
    
        public long PLANT_ID { get; set; }
        public string WERKS { get; set; }
        public string NAME1 { get; set; }
        public string ORT01 { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
    
        public virtual ICollection<CK4C> CK4C { get; set; }
        public virtual ICollection<CK5> CK5 { get; set; }
        public virtual ICollection<CK5> CK51 { get; set; }
        public virtual ICollection<NPPBKC_PLANT> NPPBKC_PLANT { get; set; }
        public virtual ICollection<PBCK3_7> PBCK3_7 { get; set; }
        public virtual ICollection<PBCK3_CK5> PBCK3_CK5 { get; set; }
        public virtual ICollection<PBCK4> PBCK4 { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP1 { get; set; }
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual ICollection<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual ICollection<ZAIDM_POA_MAP> ZAIDM_POA_MAP { get; set; }
    }
}
