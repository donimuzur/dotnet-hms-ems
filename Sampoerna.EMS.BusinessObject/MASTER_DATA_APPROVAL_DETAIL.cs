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
    
    public partial class MASTER_DATA_APPROVAL_DETAIL
    {
        public int APPROVAL_DETAIL_ID { get; set; }
        public int APPROVAL_ID { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_DESCRIPTION { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }
    
        public virtual MASTER_DATA_APPROVAL MASTER_DATA_APPROVAL { get; set; }
    }
}
