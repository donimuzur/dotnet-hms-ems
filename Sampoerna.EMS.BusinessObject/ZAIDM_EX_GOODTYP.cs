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
    
    public partial class ZAIDM_EX_GOODTYP
    {
        public ZAIDM_EX_GOODTYP()
        {
            this.ZAIDM_EX_BRAND = new HashSet<ZAIDM_EX_BRAND>();
            this.ZAIDM_EX_MATERIAL = new HashSet<ZAIDM_EX_MATERIAL>();
            this.PLANT_RECEIVE_MATERIAL = new HashSet<PLANT_RECEIVE_MATERIAL>();
            this.PLANT_RECEIVE_MATERIAL1 = new HashSet<PLANT_RECEIVE_MATERIAL>();
        }
    
        public int EXC_GOOD_TYP { get; set; }
        public string EXT_TYP_DESC { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual ICollection<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual ICollection<PLANT_RECEIVE_MATERIAL> PLANT_RECEIVE_MATERIAL { get; set; }
        public virtual ICollection<PLANT_RECEIVE_MATERIAL> PLANT_RECEIVE_MATERIAL1 { get; set; }
    }
}
