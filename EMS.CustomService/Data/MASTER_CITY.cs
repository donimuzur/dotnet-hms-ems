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
    
    public partial class MASTER_CITY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_CITY()
        {
            this.T001W = new HashSet<MASTER_PLANT>();
            this.INTERVIEW_REQUEST_DETAIL = new HashSet<INTERVIEW_REQUEST_DETAIL>();
            this.T001W1 = new HashSet<MASTER_PLANT>();
        }
    
        public long CITY_ID { get; set; }
        public long STATE_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public System.DateTime LASTMODIFIED_DATE { get; set; }
        public string CITY_NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_PLANT> T001W { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INTERVIEW_REQUEST_DETAIL> INTERVIEW_REQUEST_DETAIL { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_PLANT> T001W1 { get; set; }
    }
}