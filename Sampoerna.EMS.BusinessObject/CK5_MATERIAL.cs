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
    
    public partial class CK5_MATERIAL
    {
        public long CK5_MATERIAL_ID { get; set; }
        public Nullable<long> CK5_ID { get; set; }
        public Nullable<long> MATERIAL_ID { get; set; }
        public Nullable<int> LINE_ITEM { get; set; }
        public Nullable<decimal> QTY { get; set; }
        public Nullable<decimal> CONVERTED_QTY { get; set; }
        public Nullable<int> CONVERTED_UOM_ID { get; set; }
        public string BRAND { get; set; }
        public string PLANT_ID { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> CONVERTION { get; set; }
        public Nullable<decimal> HJE { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public Nullable<decimal> EXCISE_VALUE { get; set; }
        public Nullable<decimal> USD_VALUE { get; set; }
        public string NOTE { get; set; }
        public string CONVERTED_UOM { get; set; }
        public string MATERIAL_DESC { get; set; }
    
        public virtual CK5 CK5 { get; set; }
    }
}