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
    
    public partial class ZAIDM_EX_NPPBKC
    {
        public ZAIDM_EX_NPPBKC()
        {
            this.CK4C = new HashSet<CK4C>();
            this.NPPBKC_PLANT = new HashSet<NPPBKC_PLANT>();
            this.PBCK1 = new HashSet<PBCK1>();
            this.PBCK3_7 = new HashSet<PBCK3_7>();
            this.PBCK3_CK5 = new HashSet<PBCK3_CK5>();
            this.PBCK4 = new HashSet<PBCK4>();
        }
    
        public long NPPBKC_ID { get; set; }
        public string NPPBKC_NO { get; set; }
        public string ADDR1 { get; set; }
        public string ADDR2 { get; set; }
        public string CITY { get; set; }
        public Nullable<long> KPPBC_ID { get; set; }
        public Nullable<int> REGION_OFFICE_ID { get; set; }
        public Nullable<long> COMPANY_ID { get; set; }
        public Nullable<long> VENDOR_ID { get; set; }
        public string TEXT_TO { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
    
        public virtual C1LFA1 C1LFA1 { get; set; }
        public virtual ICollection<CK4C> CK4C { get; set; }
        public virtual ICollection<NPPBKC_PLANT> NPPBKC_PLANT { get; set; }
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
        public virtual ICollection<PBCK3_7> PBCK3_7 { get; set; }
        public virtual ICollection<PBCK3_CK5> PBCK3_CK5 { get; set; }
        public virtual ICollection<PBCK4> PBCK4 { get; set; }
        public virtual T1001 T1001 { get; set; }
    }
}
