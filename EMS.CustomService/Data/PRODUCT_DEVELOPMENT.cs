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
    
    public partial class PRODUCT_DEVELOPMENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUCT_DEVELOPMENT()
        {
            this.PRODUCT_DEVELOPMENT_DETAIL = new HashSet<PRODUCT_DEVELOPMENT_DETAIL>();
        }
    
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public Nullable<System.DateTime> LASTMODIFIED_DATE { get; set; }
        public int NEXT_ACTION { get; set; }
        public long PD_ID { get; set; }
        public string PD_NO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCT_DEVELOPMENT_DETAIL> PRODUCT_DEVELOPMENT_DETAIL { get; set; }
        public virtual USER CREATOR { get; set; }
        public virtual USER LAST_EDITOR { get; set; }
    }
}