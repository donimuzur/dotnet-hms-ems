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
    
    public partial class MONTH
    {
        public MONTH()
        {
            this.PBCK1_PROD_PLAN = new HashSet<PBCK1_PROD_PLAN>();
            this.DOC_NUMBER_SEQ = new HashSet<DOC_NUMBER_SEQ>();
            this.LACK1 = new HashSet<LACK1>();
            this.PBCK1 = new HashSet<PBCK1>();
            this.PBCK11 = new HashSet<PBCK1>();
            this.LACK2 = new HashSet<LACK2>();
        }
    
        public int MONTH_ID { get; set; }
        public string MONTH_NAME_IND { get; set; }
        public string MONTH_NAME_ENG { get; set; }
    
        public virtual ICollection<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual ICollection<DOC_NUMBER_SEQ> DOC_NUMBER_SEQ { get; set; }
        public virtual ICollection<LACK1> LACK1 { get; set; }
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
        public virtual ICollection<PBCK1> PBCK11 { get; set; }
        public virtual ICollection<LACK2> LACK2 { get; set; }
    }
}
