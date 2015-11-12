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
            this.MATERIAL_UOM = new HashSet<MATERIAL_UOM>();
            this.WASTE_STOCK = new HashSet<WASTE_STOCK>();
        }
    
        public string STICKER_CODE { get; set; }
        public string MATERIAL_DESC { get; set; }
        public string MATERIAL_GROUP { get; set; }
        public string PURCHASING_GROUP { get; set; }
        public string WERKS { get; set; }
        public string EXC_GOOD_TYP { get; set; }
        public string ISSUE_STORANGE_LOC { get; set; }
        public string BASE_UOM_ID { get; set; }
        public Nullable<decimal> HJE { get; set; }
        public string HJE_CURR { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public string TARIFF_CURR { get; set; }
        public bool IS_FROM_SAP { get; set; }
        public Nullable<bool> CLIENT_DELETION { get; set; }
        public Nullable<bool> PLANT_DELETION { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual ICollection<MATERIAL_UOM> MATERIAL_UOM { get; set; }
        public virtual T001W T001W { get; set; }
        public virtual ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }
        public virtual UOM UOM { get; set; }
        public virtual ICollection<WASTE_STOCK> WASTE_STOCK { get; set; }
    }
}
