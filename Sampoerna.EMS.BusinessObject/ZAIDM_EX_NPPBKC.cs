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
    
    public partial class ZAIDM_EX_NPPBKC
    {
        public ZAIDM_EX_NPPBKC()
        {
            this.POA_MAP = new HashSet<POA_MAP>();
            this.T001W = new HashSet<T001W>();
            this.USER_PLANT_MAP = new HashSet<USER_PLANT_MAP>();
        }
    
        public string NPPBKC_ID { get; set; }
        public string ADDR1 { get; set; }
        public string ADDR2 { get; set; }
        public string CITY { get; set; }
        public string CITY_ALIAS { get; set; }
        public string KPPBC_ID { get; set; }
        public string REGION { get; set; }
        public string REGION_DGCE { get; set; }
        public string VENDOR_ID { get; set; }
        public string BUKRS { get; set; }
        public string TEXT_TO { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public Nullable<bool> FLAG_FOR_LACK1 { get; set; }
        public string DGCE_ADDRESS { get; set; }
        public string LOCATION { get; set; }
        public string KPPBC_ADDRESS { get; set; }
    
        public virtual LFA1 LFA1 { get; set; }
        public virtual ICollection<POA_MAP> POA_MAP { get; set; }
        public virtual T001 T001 { get; set; }
        public virtual T001 T0011 { get; set; }
        public virtual ICollection<T001W> T001W { get; set; }
        public virtual ZAIDM_EX_KPPBC ZAIDM_EX_KPPBC { get; set; }
        public virtual ICollection<USER_PLANT_MAP> USER_PLANT_MAP { get; set; }
    }
}
