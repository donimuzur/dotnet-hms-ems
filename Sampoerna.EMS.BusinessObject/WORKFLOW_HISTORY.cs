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
    
    public partial class WORKFLOW_HISTORY
    {
        public long WORKFLOW_HISTORY_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public long FORM_ID { get; set; }
        public string FORM_NUMBER { get; set; }
        public Sampoerna.EMS.Core.Enums.ActionType ACTION { get; set; }
        public int ACTION_BY { get; set; }
        public Nullable<System.DateTime> ACTION_DATE { get; set; }
        public string COMMENT { get; set; }
    
        public virtual USER USER { get; set; }
    }
}
