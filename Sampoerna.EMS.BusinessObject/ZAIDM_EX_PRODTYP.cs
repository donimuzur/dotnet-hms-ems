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
    
    public partial class ZAIDM_EX_PRODTYP
    {
        public ZAIDM_EX_PRODTYP()
        {
            this.PBCK1_PROD_PLAN = new HashSet<PBCK1_PROD_PLAN>();
            this.REALISASI_PEMASUKAN = new HashSet<REALISASI_PEMASUKAN>();
            this.RENCANA_PRODUKSI = new HashSet<RENCANA_PRODUKSI>();
            this.ZAIDM_EX_BRAND = new HashSet<ZAIDM_EX_BRAND>();
            this.ZAIDM_EX_MATERIAL = new HashSet<ZAIDM_EX_MATERIAL>();
        }
    
        public int PRODUCT_ID { get; set; }
        public Nullable<int> PRODUCT_CODE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT_ALIAS { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
    
        public virtual ICollection<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual ICollection<REALISASI_PEMASUKAN> REALISASI_PEMASUKAN { get; set; }
        public virtual ICollection<RENCANA_PRODUKSI> RENCANA_PRODUKSI { get; set; }
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual ICollection<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
    }
}
