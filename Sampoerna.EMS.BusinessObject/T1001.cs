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
    
    public partial class T1001
    {
        public T1001()
        {
            this.CK4C = new HashSet<CK4C>();
            this.HEADER_FOOTER = new HashSet<HEADER_FOOTER>();
            this.T1001K = new HashSet<T1001K>();
            this.VIRTUAL_PLANT_MAP = new HashSet<VIRTUAL_PLANT_MAP>();
            this.ZAIDM_EX_NPPBKC = new HashSet<ZAIDM_EX_NPPBKC>();
        }
    
        public long COMPANY_ID { get; set; }
        public string BUKRS { get; set; }
        public string BUKRSTXT { get; set; }
        public string NPWP { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
    
        public virtual ICollection<CK4C> CK4C { get; set; }
        public virtual ICollection<HEADER_FOOTER> HEADER_FOOTER { get; set; }
        public virtual ICollection<T1001K> T1001K { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual ICollection<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
    }
}
