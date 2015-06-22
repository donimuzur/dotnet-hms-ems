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
            this.CK5 = new HashSet<CK5>();
            this.PBCK11 = new HashSet<PBCK1>();
            this.PBCK1_PROD_CONVERTER = new HashSet<PBCK1_PROD_CONVERTER>();
            this.PBCK1_PROD_PLAN = new HashSet<PBCK1_PROD_PLAN>();
            this.REALISASI_PEMASUKAN = new HashSet<REALISASI_PEMASUKAN>();
            this.RENCANA_PRODUKSI = new HashSet<RENCANA_PRODUKSI>();
        }
    
        public long PBCK1_ID { get; set; }
        public string NUMBER { get; set; }
        public Nullable<long> PBCK1_REF { get; set; }
        public string PBCK1_TYPE { get; set; }
        public Nullable<System.DateTime> PERIOD_FROM { get; set; }
        public Nullable<System.DateTime> PERIOD_TO { get; set; }
        public Nullable<System.DateTime> REPORTED_ON { get; set; }
        public Nullable<long> NPPBKC_ID { get; set; }
        public Nullable<int> GOODTYPE_ID { get; set; }
        public string SUPPLIER_PLANT { get; set; }
        public Nullable<int> SUPPLIER_PORT_ID { get; set; }
        public string SUPPLIER_ADDRESS { get; set; }
        public string SUPPLIER_PHONE { get; set; }
        public Nullable<System.DateTime> PLAN_PROD_FROM { get; set; }
        public Nullable<System.DateTime> PLAN_PROD_TO { get; set; }
        public Nullable<decimal> REQUEST_QTY { get; set; }
        public Nullable<int> REQUEST_QTY_UOM { get; set; }
        public Nullable<int> LACK1_FROM_MONTH { get; set; }
        public Nullable<int> LACK1_FROM_YEAR { get; set; }
        public Nullable<int> LACK1_TO_MONTH { get; set; }
        public Nullable<int> LACK1_TO_YEAR { get; set; }
        public Nullable<int> STATUS { get; set; }
        public Nullable<int> STATUS_GOV { get; set; }
        public Nullable<decimal> QTY_APPROVED { get; set; }
        public Nullable<System.DateTime> DECREE_DATE { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<int> CREATED_BY { get; set; }
        public Nullable<int> APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual ICollection<CK5> CK5 { get; set; }
        public virtual MONTH MONTH { get; set; }
        public virtual MONTH MONTH1 { get; set; }
        public virtual ICollection<PBCK1> PBCK11 { get; set; }
        public virtual PBCK1 PBCK12 { get; set; }
        public virtual ICollection<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER { get; set; }
        public virtual ICollection<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual SUPPLIER_PORT SUPPLIER_PORT { get; set; }
        public virtual UOM UOM { get; set; }
        public virtual USER USER { get; set; }
        public virtual ZAIDM_EX_GOODTYP ZAIDM_EX_GOODTYP { get; set; }
        public virtual ZAIDM_EX_NPPBKC ZAIDM_EX_NPPBKC { get; set; }
        public virtual ZAIDM_EX_POA ZAIDM_EX_POA { get; set; }
        public virtual ICollection<REALISASI_PEMASUKAN> REALISASI_PEMASUKAN { get; set; }
        public virtual ICollection<RENCANA_PRODUKSI> RENCANA_PRODUKSI { get; set; }
    }
}
