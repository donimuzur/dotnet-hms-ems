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
    
    public partial class MASTER_DATA_APPROVAL
    {
        public MASTER_DATA_APPROVAL()
        {
            this.MASTER_DATA_APPROVAL_DETAIL = new HashSet<MASTER_DATA_APPROVAL_DETAIL>();
        }
    
        public int APPROVAL_ID { get; set; }
        public int PAGE_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public Sampoerna.EMS.Core.Enums.DocumentStatus STATUS_ID { get; set; }
    
        public virtual USER USER { get; set; }
        public virtual ICollection<MASTER_DATA_APPROVAL_DETAIL> MASTER_DATA_APPROVAL_DETAIL { get; set; }
        public virtual PAGE PAGE { get; set; }
        public virtual USER USER1 { get; set; }
    }
}
