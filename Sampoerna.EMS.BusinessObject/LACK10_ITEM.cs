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
    
    public partial class LACK10_ITEM
    {
        public long LACK10_ITEM_ID { get; set; }
        public int LACK10_ID { get; set; }
        public string FA_CODE { get; set; }
        public string WERKS { get; set; }
        public string TYPE { get; set; }
        public Nullable<decimal> WASTE_VALUE { get; set; }
        public string UOM { get; set; }
        public string BRAND_DESCRIPTION { get; set; }
        public string PLANT_NAME { get; set; }
    
        public virtual LACK10 LACK10 { get; set; }
        public virtual UOM UOM1 { get; set; }
    }
}