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
    
    public partial class QUOTA_MONITORING_DETAIL
    {
        public int MONITORING_DETAIL_ID { get; set; }
        public Nullable<int> MONITORING_ID { get; set; }
        public string USER_ID { get; set; }
        public Nullable<int> ROLE_ID { get; set; }
        public Nullable<int> EMAIL_STATUS { get; set; }
    
        public virtual QUOTA_MONITORING QUOTA_MONITORING { get; set; }
    }
}
