//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sampoerna.EMS.CustomService.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class MANUFACTURING_BOUND_CONDITION
    {
        public string NORTH { get; set; }
        public string EAST { get; set; }
        public string SOUTH { get; set; }
        public string WEST { get; set; }
        public Nullable<decimal> LAND_AREA { get; set; }
        public Nullable<decimal> BUILDING_AREA { get; set; }
        public string OWNERSHIP_STATUS { get; set; }
        public long MNF_REQUEST_ID { get; set; }
        public long MNF_COND_ID { get; set; }
        public long VR_FORM_DETAIL_ID { get; set; }
    
        public virtual INTERVIEW_REQUEST_DETAIL INTERVIEW_REQUEST_DETAIL { get; set; }
        public virtual MANUFACTURING_LISENCE_REQUEST MANUFACTURING_LISENCE_REQUEST { get; set; }
    }
}