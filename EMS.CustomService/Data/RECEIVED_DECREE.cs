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
    
    public partial class RECEIVED_DECREE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RECEIVED_DECREE()
        {
            this.RECEIVED_DECREE_DETAIL = new HashSet<RECEIVED_DECREE_DETAIL>();
        }
    
        public long RECEIVED_ID { get; set; }
        public string RECEIVED_NO { get; set; }
        public string NPPBKC_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public Nullable<System.DateTime> LASTMODIFIED_DATE { get; set; }
        public string LASTAPPROVED_BY { get; set; }
        public Nullable<System.DateTime> LASTAPPROVED_DATE { get; set; }
        public long LASTAPPROVED_STATUS { get; set; }
        public string DECREE_NO { get; set; }
        public System.DateTime DECREE_DATE { get; set; }
        public System.DateTime DECREE_STARTDATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECEIVED_DECREE_DETAIL> RECEIVED_DECREE_DETAIL { get; set; }
        public virtual SYS_REFFERENCES APPROVAL_STATUS { get; set; }
        public virtual USER APPROVER { get; set; }
        public virtual USER CREATOR { get; set; }
        public virtual USER LASTEDITOR { get; set; }
        public virtual MASTER_NPPBKC ZAIDM_EX_NPPBKC { get; set; }
    }
}
