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
    
    public partial class MASTER_KPPBC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_KPPBC()
        {
            this.NPPBKCS = new HashSet<MASTER_NPPBKC>();
        }
    
        public string KPPBC_ID { get; set; }
        public string KPPBC_TYPE { get; set; }
        public string MENGETAHUI { get; set; }
        public string CK1_KEP_HEADER { get; set; }
        public string CK1_KEP_FOOTER { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public string MENGETAHUI_DETAIL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_NPPBKC> NPPBKCS { get; set; }
    }
}
