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
    
    public partial class PRINTOUT_LAYOUT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRINTOUT_LAYOUT()
        {
            this.PRINTOUT_VARIABLE = new HashSet<PRINTOUT_VARIABLE>();
            this.USER_PRINTOUT_LAYOUT = new HashSet<USER_PRINTOUT_LAYOUT>();
        }
    
        public int PRINTOUT_LAYOUT_ID { get; set; }
        public string NAME { get; set; }
        public string LAYOUT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRINTOUT_VARIABLE> PRINTOUT_VARIABLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USER_PRINTOUT_LAYOUT> USER_PRINTOUT_LAYOUT { get; set; }
    }
}
