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
    
    public partial class QUOTA_MONITORING
    {
        public QUOTA_MONITORING()
        {
            this.QUOTA_MONITORING_DETAIL = new HashSet<QUOTA_MONITORING_DETAIL>();
        }
    
        public int MONITORING_ID { get; set; }
        public string NPPBKC_ID { get; set; }
        public string SUPPLIER_NPPBKC_ID { get; set; }
        public string SUPPLIER_WERKS { get; set; }
        public Nullable<System.DateTime> PERIOD_FROM { get; set; }
        public Nullable<System.DateTime> PERIOD_TO { get; set; }
        public Nullable<int> EX_GROUP_TYPE { get; set; }
        public int WARNING_LEVEL { get; set; }
    
        public virtual ICollection<QUOTA_MONITORING_DETAIL> QUOTA_MONITORING_DETAIL { get; set; }
        public virtual EX_GROUP_TYPE EX_GROUP_TYPE1 { get; set; }
    }
}
