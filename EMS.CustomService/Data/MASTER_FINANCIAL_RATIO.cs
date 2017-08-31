//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sampoerna.EMS.CustomService.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class MASTER_FINANCIAL_RATIO
    {
        public long FINANCERATIO_ID { get; set; }
        public string BUKRS { get; set; }
        public int YEAR_PERIOD { get; set; }
        public decimal CURRENT_ASSETS { get; set; }
        public decimal CURRENT_DEBTS { get; set; }
        public double LIQUIDITY_RATIO { get; set; }
        public decimal TOTAL_ASSETS { get; set; }
        public decimal TOTAL_DEBTS { get; set; }
        public double RENTABLE_RATIO { get; set; }
        public double SOLVENCY_RATIO { get; set; }
        public decimal NET_PROFIT { get; set; }
        public decimal TOTAL_CAPITAL { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public Nullable<System.DateTime> LASTMODIFIED_DATE { get; set; }
        public string LASTAPPROVED_BY { get; set; }
        public Nullable<System.DateTime> LASTAPPROVED_DATE { get; set; }
        public long STATUS_APPROVAL { get; set; }
    
        public virtual T001 COMPANY { get; set; }
        public virtual USER CREATOR { get; set; }
        public virtual USER APPROVER { get; set; }
        public virtual USER LASTEDITOR { get; set; }
        public virtual SYS_REFFERENCES APPROVALSTATUS { get; set; }
    }
}
