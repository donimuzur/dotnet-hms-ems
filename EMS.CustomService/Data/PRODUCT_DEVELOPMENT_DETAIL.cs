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
    
    public partial class PRODUCT_DEVELOPMENT_DETAIL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUCT_DEVELOPMENT_DETAIL()
        {
            this.BRAND_REGISTRATION_REQ_DETAIL = new HashSet<BRAND_REGISTRATION_REQ_DETAIL>();
            this.RECEIVED_DECREE_DETAIL = new HashSet<RECEIVED_DECREE_DETAIL>();
            this.PRODUCT_DEVELOPMENT_UPLOAD = new HashSet<PRODUCT_DEVELOPMENT_UPLOAD>();
        }
    
        public long PD_DETAIL_ID { get; set; }
        public string FA_CODE_OLD { get; set; }
        public string FA_CODE_NEW { get; set; }
        public string HL_CODE { get; set; }
        public string MARKET_ID { get; set; }
        public string FA_CODE_OLD_DESCR { get; set; }
        public string FA_CODE_NEW_DESCR { get; set; }
        public string WERKS { get; set; }
        public bool IS_IMPORT { get; set; }
        public long PD_ID { get; set; }
        public string REQUEST_NO { get; set; }
        public string BUKRS { get; set; }
        public string LASTAPPROVED_BY { get; set; }
        public Nullable<System.DateTime> LASTAPPROVED_DATE { get; set; }
        public long STATUS_APPROVAL { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public Nullable<System.DateTime> LASTMODIFIED_DATE { get; set; }
        public Nullable<int> COUNTRY_ID { get; set; }
        public string WEEK { get; set; }
    
        public virtual T001 T001 { get; set; }
        public virtual ZAIDM_EX_MARKET ZAIDM_EX_MARKET { get; set; }
        public virtual MASTER_PLANT T001W { get; set; }
        public virtual USER APPROVER { get; set; }
        public virtual SYS_REFFERENCES APPROVAL_STATUS { get; set; }
        public virtual PRODUCT_DEVELOPMENT PRODUCT_DEVELOPMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BRAND_REGISTRATION_REQ_DETAIL> BRAND_REGISTRATION_REQ_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECEIVED_DECREE_DETAIL> RECEIVED_DECREE_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCT_DEVELOPMENT_UPLOAD> PRODUCT_DEVELOPMENT_UPLOAD { get; set; }
        public virtual COUNTRY COUNTRY { get; set; }
    }
}
