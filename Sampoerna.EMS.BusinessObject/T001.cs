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
    
    public partial class T001
    {
        public T001()
        {
            this.HEADER_FOOTER = new HashSet<HEADER_FOOTER>();
            this.T001K = new HashSet<T001K>();
            this.T001K1 = new HashSet<T001K>();
            this.VIRTUAL_PLANT_MAP = new HashSet<VIRTUAL_PLANT_MAP>();
            this.ZAIDM_EX_NPPBKC = new HashSet<ZAIDM_EX_NPPBKC>();
            this.ZAIDM_EX_NPPBKC1 = new HashSet<ZAIDM_EX_NPPBKC>();
        }
    
        public string BUKRS { get; set; }
        public string BUTXT { get; set; }
        public string NPWP { get; set; }
        public string ORT01 { get; set; }
        public string SPRAS { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
    
        public virtual ICollection<HEADER_FOOTER> HEADER_FOOTER { get; set; }
        public virtual ICollection<T001K> T001K { get; set; }
        public virtual ICollection<T001K> T001K1 { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual ICollection<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
        public virtual ICollection<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC1 { get; set; }
    }
}
