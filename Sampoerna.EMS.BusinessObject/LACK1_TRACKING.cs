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
    
    public partial class LACK1_TRACKING
    {
        public long LACK1_TRACKING_ID { get; set; }
        public Nullable<int> LACK1_ID { get; set; }
        public Nullable<long> INVENTORY_MOVEMENT_ID { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
    
        public virtual INVENTORY_MOVEMENT INVENTORY_MOVEMENT { get; set; }
        public virtual LACK1 LACK1 { get; set; }
    }
}
