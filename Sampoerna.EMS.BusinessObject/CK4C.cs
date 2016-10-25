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
    
    public partial class CK4C
    {
        public CK4C()
        {
            this.CK4C_ITEM = new HashSet<CK4C_ITEM>();
            this.CK4C_DECREE_DOC = new HashSet<CK4C_DECREE_DOC>();
        }
    
        public int CK4C_ID { get; set; }
        public string NUMBER { get; set; }
        public string COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string PLANT_ID { get; set; }
        public string PLANT_NAME { get; set; }
        public string NPPBKC_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<System.DateTime> REPORTED_ON { get; set; }
        public Nullable<int> REPORTED_PERIOD { get; set; }
        public Nullable<int> REPORTED_MONTH { get; set; }
        public Nullable<int> REPORTED_YEAR { get; set; }
        public Sampoerna.EMS.Core.Enums.DocumentStatus STATUS { get; set; }
        public Nullable<Sampoerna.EMS.Core.Enums.StatusGovCk4c> GOV_STATUS { get; set; }
        public string APPROVED_BY_POA { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE_POA { get; set; }
        public string APPROVED_BY_MANAGER { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE_MANAGER { get; set; }
        public Nullable<System.DateTime> DECREE_DATE { get; set; }
        public Nullable<int> CK4C_ID_REVISED { get; set; }
    
        public virtual USER USER { get; set; }
        public virtual MONTH MONTH { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual ICollection<CK4C_ITEM> CK4C_ITEM { get; set; }
        public virtual POA POA { get; set; }
        public virtual ICollection<CK4C_DECREE_DOC> CK4C_DECREE_DOC { get; set; }
    }
}
