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
    
    public partial class LACK1_DOCUMENT
    {
        public int LACK1_DOCUMENT_ID { get; set; }
        public Nullable<int> LACK1_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
    
        public virtual LACK1 LACK1 { get; set; }
    }
}