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
    
    public partial class ZAIDM_EX_MATERIAL
    {
        public ZAIDM_EX_MATERIAL()
        {
            this.CK5_MATERIAL = new HashSet<CK5_MATERIAL>();
        }
    
        public long MATERIAL_ID { get; set; }
        public string MATERIAL_NUMBER { get; set; }
        public string MATERIAL_DESC { get; set; }
        public string MATERIAL_GROUP { get; set; }
        public string PURCHASING_GROUP { get; set; }
        public Nullable<long> PLANT_ID { get; set; }
        public Nullable<int> EX_GOODTYP { get; set; }
        public string ISSUE_STORANGE_LOC { get; set; }
        public Nullable<int> BASE_UOM { get; set; }
        public Nullable<int> CREATED_BY { get; set; }
        public Nullable<bool> IS_FROM_SAP { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public Nullable<long> BRAND_ID { get; set; }
        public Nullable<decimal> CONVERSION { get; set; }
    
        public virtual ICollection<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual UOM UOM { get; set; }
        public virtual USER USER { get; set; }
        public virtual T1001W T1001W { get; set; }
        public virtual ZAIDM_EX_BRAND ZAIDM_EX_BRAND { get; set; }
        public virtual ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }
    }
}
