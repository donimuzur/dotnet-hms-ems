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
    
    public partial class PBCK4_DOCUMENT
    {
        public long PBCK4_DOCUMENT_ID { get; set; }
        public int DOC_TYPE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
        public int PBCK4_ID { get; set; }
    
        public virtual PBCK4 PBCK4 { get; set; }
    }
}
