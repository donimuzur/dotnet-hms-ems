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
    
    public partial class HEADER_FOOTER
    {
        public HEADER_FOOTER()
        {
            this.HEADER_FOOTER_FORM_MAP = new HashSet<HEADER_FOOTER_FORM_MAP>();
        }
    
        public int HEADER_FOOTER_ID { get; set; }
        public string FORM_NAME { get; set; }
        public string BUKRS { get; set; }
        public string HEADER_IMAGE_PATH { get; set; }
        public string FOOTER_CONTENT { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
    
        public virtual ICollection<HEADER_FOOTER_FORM_MAP> HEADER_FOOTER_FORM_MAP { get; set; }
        public virtual T001 T001 { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
    }
}
