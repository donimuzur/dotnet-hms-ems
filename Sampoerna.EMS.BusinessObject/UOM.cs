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
    
    public partial class UOM
    {
        public UOM()
        {
            this.CK5 = new HashSet<CK5>();
            this.PBCK1 = new HashSet<PBCK1>();
            this.PBCK11 = new HashSet<PBCK1>();
            this.PBCK1_PROD_PLAN = new HashSet<PBCK1_PROD_PLAN>();
            this.ZAIDM_EX_MATERIAL = new HashSet<ZAIDM_EX_MATERIAL>();
            this.LACK1 = new HashSet<LACK1>();
            this.LACK11 = new HashSet<LACK1>();
            this.LACK111 = new HashSet<LACK1>();
            this.CK4C_ITEM = new HashSet<CK4C_ITEM>();
            this.LACK1_PRODUCTION_DETAIL = new HashSet<LACK1_PRODUCTION_DETAIL>();
            this.PBCK1_PROD_CONVERTER = new HashSet<PBCK1_PROD_CONVERTER>();
            this.LACK10_ITEM = new HashSet<LACK10_ITEM>();
        }
    
        public string UOM_ID { get; set; }
        public string UOM_DESC { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public Nullable<bool> IS_EMS { get; set; }
    
        public virtual ICollection<CK5> CK5 { get; set; }
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
        public virtual ICollection<PBCK1> PBCK11 { get; set; }
        public virtual ICollection<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual ICollection<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual ICollection<LACK1> LACK1 { get; set; }
        public virtual ICollection<LACK1> LACK11 { get; set; }
        public virtual ICollection<LACK1> LACK111 { get; set; }
        public virtual ICollection<CK4C_ITEM> CK4C_ITEM { get; set; }
        public virtual ICollection<LACK1_PRODUCTION_DETAIL> LACK1_PRODUCTION_DETAIL { get; set; }
        public virtual ICollection<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER { get; set; }
        public virtual ICollection<LACK10_ITEM> LACK10_ITEM { get; set; }
    }
}
