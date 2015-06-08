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
    
    public partial class ZAIDM_EX_POA
    {
        public ZAIDM_EX_POA()
        {
            this.PBCK1 = new HashSet<PBCK1>();
            this.PBCK3_7 = new HashSet<PBCK3_7>();
            this.PBCK4 = new HashSet<PBCK4>();
            this.ZAIDM_POA_MAP = new HashSet<ZAIDM_POA_MAP>();
        }
    
        public int POA_ID { get; set; }
        public string POA_CODE { get; set; }
        public string POA_ID_CARD { get; set; }
        public string POA_ADDRESS { get; set; }
        public string POA_PHONE { get; set; }
        public string POA_PRINTED_NAME { get; set; }
        public string TITLE { get; set; }
        public Nullable<int> USER_ID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
    
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
        public virtual ICollection<PBCK3_7> PBCK3_7 { get; set; }
        public virtual ICollection<PBCK4> PBCK4 { get; set; }
        public virtual USER USER { get; set; }
        public virtual ICollection<ZAIDM_POA_MAP> ZAIDM_POA_MAP { get; set; }
    }
}
