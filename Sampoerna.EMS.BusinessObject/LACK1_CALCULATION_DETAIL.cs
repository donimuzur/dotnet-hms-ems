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
    
    public partial class LACK1_CALCULATION_DETAIL
    {
        public int LACK1_CALCULATION_DETAIL_ID { get; set; }
        public Nullable<int> LACK1_ID { get; set; }
        public string MATERIAL_ID { get; set; }
        public string PLANT_ID { get; set; }
        public Nullable<decimal> AMOUNT_USAGE { get; set; }
        public string UOM_USAGE { get; set; }
        public string ORDR { get; set; }
        public string FA_CODE { get; set; }
        public string BRAND_CE { get; set; }
        public Nullable<decimal> AMOUNT_PRODUCTION { get; set; }
        public string UOM_PRODUCTION { get; set; }
        public Nullable<decimal> PROPORTIONAL { get; set; }
        public Nullable<int> TYPE { get; set; }
        public Nullable<decimal> CONVERTION { get; set; }
    
        public virtual LACK1 LACK1 { get; set; }
    }
}