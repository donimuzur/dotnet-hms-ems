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
    
    public partial class REPLACEMENT_DOCUMENTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REPLACEMENT_DOCUMENTS()
        {
            this.REPLACEMENT_DOCUMENTS_DETAIL = new HashSet<REPLACEMENT_DOCUMENTS_DETAIL>();
        }
    
        public long FORM_ID { get; set; }
        public string FORM_NO { get; set; }
        public System.DateTime REQUEST_DATE { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string NPPBKC_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public Nullable<System.DateTime> LASTMODIFIED_DATE { get; set; }
        public string LASTAPPROVED_BY { get; set; }
        public Nullable<System.DateTime> LASTAPPROVED_DATE { get; set; }
        public long LASTAPPROVED_STATUS { get; set; }
        public Nullable<bool> DECREE_STATUS { get; set; }
        public string DECREE_NUMBER { get; set; }
    
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual SYS_REFFERENCES SYS_REFFERENCES { get; set; }
        public virtual USER USER2 { get; set; }
        public virtual MASTER_NPPBKC ZAIDM_EX_NPPBKC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REPLACEMENT_DOCUMENTS_DETAIL> REPLACEMENT_DOCUMENTS_DETAIL { get; set; }
    }
}
