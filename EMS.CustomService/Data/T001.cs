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
    
    public partial class T001
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T001()
        {
            this.MASTER_FINANCIAL_RATIO = new HashSet<MASTER_FINANCIAL_RATIO>();
            this.MASTER_SUPPORTING_DOCUMENT = new HashSet<MASTER_SUPPORTING_DOCUMENT>();
            this.NPPBKC = new HashSet<MASTER_NPPBKC>();
            this.INTERVIEW_REQUEST = new HashSet<INTERVIEW_REQUEST>();
            this.PRODUCT_DEVELOPMENT_DETAIL = new HashSet<PRODUCT_DEVELOPMENT_DETAIL>();
        }
    
        public string BUKRS { get; set; }
        public string BUTXT { get; set; }
        public string NPWP { get; set; }
        public string ORT01 { get; set; }
        public string SPRAS { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public string BUTXT_ALIAS { get; set; }
        public string PKP { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_FINANCIAL_RATIO> MASTER_FINANCIAL_RATIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_SUPPORTING_DOCUMENT> MASTER_SUPPORTING_DOCUMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_NPPBKC> NPPBKC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INTERVIEW_REQUEST> INTERVIEW_REQUEST { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCT_DEVELOPMENT_DETAIL> PRODUCT_DEVELOPMENT_DETAIL { get; set; }
    }
}
