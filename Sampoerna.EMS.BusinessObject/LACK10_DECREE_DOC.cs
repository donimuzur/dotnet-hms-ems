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
    
    public partial class LACK10_DECREE_DOC
    {
        public long LACK10_DECREE_DOC_ID { get; set; }
        public int LACK10_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    
        public virtual LACK10 LACK10 { get; set; }
        public virtual USER USER { get; set; }
    }
}
