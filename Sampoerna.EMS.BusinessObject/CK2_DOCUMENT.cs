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
    
    public partial class CK2_DOCUMENT
    {
        public long CK2_DOC_ID { get; set; }
        public string FILE_PATH { get; set; }
        public string FILE_NAME { get; set; }
        public int CK2_ID { get; set; }
    
        public virtual CK2 CK2 { get; set; }
    }
}