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
    
    public partial class POA_SK
    {
        public int POA_SK_ID { get; set; }
        public string POA_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
    
        public virtual POA POA { get; set; }
    }
}