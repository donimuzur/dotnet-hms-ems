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
            this.CHANGES_HISTORY = new HashSet<CHANGES_HISTORY>();
            this.CK5 = new HashSet<CK5>();
            this.CK51 = new HashSet<CK5>();
            this.HEADER_FOOTER = new HashSet<HEADER_FOOTER>();
            this.HEADER_FOOTER1 = new HashSet<HEADER_FOOTER>();
            this.PBCK1 = new HashSet<PBCK1>();
            this.PBCK11 = new HashSet<PBCK1>();
            this.POA = new HashSet<POA>();
            this.POA1 = new HashSet<POA>();
            this.POA2 = new HashSet<POA>();
            this.POA3 = new HashSet<POA>();
            this.POA_MAP = new HashSet<POA_MAP>();
            this.POA_MAP1 = new HashSet<POA_MAP>();
            this.T001 = new HashSet<T001>();
            this.T001W = new HashSet<T001W>();
            this.UOM = new HashSet<UOM>();
            this.VIRTUAL_PLANT_MAP = new HashSet<VIRTUAL_PLANT_MAP>();
            this.VIRTUAL_PLANT_MAP1 = new HashSet<VIRTUAL_PLANT_MAP>();
            this.ZAIDM_EX_BRAND = new HashSet<ZAIDM_EX_BRAND>();
            this.ZAIDM_EX_BRAND1 = new HashSet<ZAIDM_EX_BRAND>();
            this.ZAIDM_EX_NPPBKC = new HashSet<ZAIDM_EX_NPPBKC>();
            this.ZAIDM_EX_MATERIAL = new HashSet<ZAIDM_EX_MATERIAL>();
            this.ZAIDM_EX_MATERIAL1 = new HashSet<ZAIDM_EX_MATERIAL>();
        }
    
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string USER_GROUP_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
    
        public virtual ICollection<CHANGES_HISTORY> CHANGES_HISTORY { get; set; }
        public virtual ICollection<CK5> CK5 { get; set; }
        public virtual ICollection<CK5> CK51 { get; set; }
        public virtual ICollection<HEADER_FOOTER> HEADER_FOOTER { get; set; }
        public virtual ICollection<HEADER_FOOTER> HEADER_FOOTER1 { get; set; }
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
        public virtual ICollection<PBCK1> PBCK11 { get; set; }
        public virtual ICollection<POA> POA { get; set; }
        public virtual ICollection<POA> POA1 { get; set; }
        public virtual ICollection<POA> POA2 { get; set; }
        public virtual ICollection<POA> POA3 { get; set; }
        public virtual ICollection<POA_MAP> POA_MAP { get; set; }
        public virtual ICollection<POA_MAP> POA_MAP1 { get; set; }
        public virtual ICollection<T001> T001 { get; set; }
        public virtual ICollection<T001W> T001W { get; set; }
        public virtual ICollection<UOM> UOM { get; set; }
        public virtual USER_GROUP USER_GROUP { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP1 { get; set; }
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND1 { get; set; }
        public virtual ICollection<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
        public virtual ICollection<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual ICollection<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL1 { get; set; }
    }
}
