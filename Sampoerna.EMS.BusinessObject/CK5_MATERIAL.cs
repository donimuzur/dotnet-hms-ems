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
    
        public virtual ZAIDM_EX_MATERIAL ZAIDM_EX_MATERIAL { get; set; }
        public virtual CK5 CK5 { get; set; }
    }
}
