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
    
    public partial class PBCK1
    {
        public PBCK1()
        {
            this.PBCK11 = new HashSet<PBCK1>();
            this.PBCK1_PROD_PLAN = new HashSet<PBCK1_PROD_PLAN>();
            this.CK5 = new HashSet<CK5>();
            this.PBCK1_PROD_CONVERTER = new HashSet<PBCK1_PROD_CONVERTER>();
        }
    
        public int PBCK1_ID { get; set; }
        public string NUMBER { get; set; }
        public Nullable<int> PBCK1_REF { get; set; }
        public Sampoerna.EMS.Core.Enums.PBCK1Type PBCK1_TYPE { get; set; }
        public Nullable<System.DateTime> PERIOD_FROM { get; set; }
        public Nullable<System.DateTime> PERIOD_TO { get; set; }
        public Nullable<System.DateTime> REPORTED_ON { get; set; }
        public string NPPBKC_ID { get; set; }
        public string NPPBKC_BUKRS { get; set; }
        public string NPPBCK_BUTXT { get; set; }
        public string EXC_GOOD_TYP { get; set; }
        public string EXC_TYP_DESC { get; set; }
        public string SUPPLIER_PLANT { get; set; }
        public Nullable<int> SUPPLIER_PORT_ID { get; set; }
        public string SUPPLIER_ADDRESS { get; set; }
        public string SUPPLIER_PHONE { get; set; }
        public Nullable<System.DateTime> PLAN_PROD_FROM { get; set; }
        public Nullable<System.DateTime> PLAN_PROD_TO { get; set; }
        public Nullable<decimal> REQUEST_QTY { get; set; }
        public string REQUEST_QTY_UOM { get; set; }
        public Nullable<decimal> LATEST_SALDO { get; set; }
        public string LATEST_SALDO_UOM { get; set; }
        public Nullable<int> LACK1_FROM_MONTH { get; set; }
        public Nullable<int> LACK1_FROM_YEAR { get; set; }
        public Nullable<int> LACK1_TO_MONTH { get; set; }
        public Nullable<int> LACK1_TO_YEAR { get; set; }
        public int STATUS { get; set; }
        public int STATUS_GOV { get; set; }
        public Nullable<decimal> QTY_APPROVED { get; set; }
        public Nullable<System.DateTime> DECREE_DATE { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string SUPPLIER_PORT_NAME { get; set; }
        public string SUPPLIER_NPPBKC_ID { get; set; }
        public string SUPPLIER_KPPBC_ID { get; set; }
        public string SUPPLIER_PLANT_WERKS { get; set; }
    
        public virtual MONTH MONTH { get; set; }
        public virtual MONTH MONTH1 { get; set; }
        public virtual USER USER { get; set; }
        public virtual USER USER1 { get; set; }
        public virtual ICollection<PBCK1> PBCK11 { get; set; }
        public virtual PBCK1 PBCK12 { get; set; }
        public virtual ICollection<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual ICollection<CK5> CK5 { get; set; }
        public virtual UOM UOM { get; set; }
        public virtual UOM UOM1 { get; set; }
        public virtual ICollection<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER { get; set; }
    }
}
