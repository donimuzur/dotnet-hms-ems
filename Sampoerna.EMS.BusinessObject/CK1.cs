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
    
    public partial class CK1
    {
        public CK1()
        {
            this.CK1_ITEM = new HashSet<CK1_ITEM>();
            this.PBCK4_ITEM = new HashSet<PBCK4_ITEM>();
        }
    
        public long CK1_ID { get; set; }
        public string CK1_NUMBER { get; set; }
        public System.DateTime CK1_DATE { get; set; }
        public string PLANT_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string VENDOR_ID { get; set; }
        public string NPPBKC_ID { get; set; }
        public Nullable<System.DateTime> ORDER_DATE { get; set; }
        public Nullable<bool> IS_PAID_OFF { get; set; }
    
        public virtual ICollection<CK1_ITEM> CK1_ITEM { get; set; }
        public virtual ICollection<PBCK4_ITEM> PBCK4_ITEM { get; set; }
    }
}
