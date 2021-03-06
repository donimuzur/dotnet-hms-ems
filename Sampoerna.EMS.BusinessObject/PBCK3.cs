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
    
    public partial class PBCK3
    {
        public PBCK3()
        {
            this.BACK3 = new HashSet<BACK3>();
            this.CK2 = new HashSet<CK2>();
        }
    
        public int PBCK3_ID { get; set; }
        public string PBCK3_NUMBER { get; set; }
        public System.DateTime PBCK3_DATE { get; set; }
        public Nullable<int> PBCK7_ID { get; set; }
        public Nullable<Sampoerna.EMS.Core.Enums.DocumentStatusGovType3> GOV_STATUS { get; set; }
        public Nullable<Sampoerna.EMS.Core.Enums.DocumentStatus> STATUS { get; set; }
        public string APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string APPROVED_BY_MANAGER { get; set; }
        public Nullable<System.DateTime> APPROVED_BY_MANAGER_DATE { get; set; }
        public string REJECTED_BY { get; set; }
        public Nullable<System.DateTime> REJECTED_DATE { get; set; }
        public Nullable<long> CK5_ID { get; set; }
        public System.DateTime EXEC_DATE_FROM { get; set; }
        public System.DateTime EXEC_DATE_TO { get; set; }
    
        public virtual ICollection<BACK3> BACK3 { get; set; }
        public virtual ICollection<CK2> CK2 { get; set; }
        public virtual CK5 CK5 { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual PBCK7 PBCK7 { get; set; }
        public virtual POA POA { get; set; }
        public virtual USER USER2 { get; set; }
        public virtual USER USER3 { get; set; }
    }
}
