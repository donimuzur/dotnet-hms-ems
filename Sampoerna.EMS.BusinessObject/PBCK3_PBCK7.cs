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
    
    public partial class PBCK3_PBCK7
    {
        public PBCK3_PBCK7()
        {
            this.PBCK3_PBCK7_ITEM = new HashSet<PBCK3_PBCK7_ITEM>();
        }
    
        public int PBCK3_PBCK7_ID { get; set; }
        public string PBCK7_NUMBER { get; set; }
        public string PBCK3_NUMBER { get; set; }
        public Sampoerna.EMS.Core.Enums.DocumentStatus PBCK7_STATUS { get; set; }
        public Nullable<Sampoerna.EMS.Core.Enums.DocumentStatus> PBCK3_STATUS { get; set; }
        public System.DateTime PBCK7_DATE { get; set; }
        public Nullable<System.DateTime> PBCK3_DATE { get; set; }
        public Sampoerna.EMS.Core.Enums.DocumentTypePbck7AndPbck3 DOC_TYPE { get; set; }
        public string NPPBCK_ID { get; set; }
        public string PLANT_ID { get; set; }
        public string PLANT_NAME { get; set; }
        public string PLANT_CITY { get; set; }
        public Nullable<System.DateTime> EXEC_DATE_FROM { get; set; }
        public Nullable<System.DateTime> EXEC_DATE_TO { get; set; }
        public Sampoerna.EMS.Core.Enums.DocumentStatusGov GOV_STATUS { get; set; }
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
    
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual USER USER2 { get; set; }
        public virtual ICollection<PBCK3_PBCK7_ITEM> PBCK3_PBCK7_ITEM { get; set; }
        public virtual USER USER3 { get; set; }
        public virtual USER USER4 { get; set; }
    }
}
