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
    
    public partial class EMAIL_TEMPLATE
    {
        public EMAIL_TEMPLATE()
        {
            this.WORKFLOW_STATE = new HashSet<WORKFLOW_STATE>();
        }
    
        public int EMAIL_TEMPLATE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string SUBJECT { get; set; }
        public string BODY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
    
        public virtual ICollection<WORKFLOW_STATE> WORKFLOW_STATE { get; set; }
    }
}
