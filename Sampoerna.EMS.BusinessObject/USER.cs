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
    
    public partial class USER
    {
        public USER()
        {
            this.CHANGES_HISTORY = new HashSet<CHANGES_HISTORY>();
            this.CK5 = new HashSet<CK5>();
            this.PBCK1 = new HashSet<PBCK1>();
            this.USER1 = new HashSet<USER>();
            this.ZAIDM_EX_KPPBC = new HashSet<ZAIDM_EX_KPPBC>();
            this.ZAIDM_EX_POA = new HashSet<ZAIDM_EX_POA>();
        }
    
        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public Nullable<int> MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
        public Nullable<int> USER_GROUP_ID { get; set; }
    
        public virtual ICollection<CHANGES_HISTORY> CHANGES_HISTORY { get; set; }
        public virtual ICollection<CK5> CK5 { get; set; }
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
        public virtual ICollection<USER> USER1 { get; set; }
        public virtual USER USER2 { get; set; }
        public virtual USER_GROUP USER_GROUP { get; set; }
        public virtual ICollection<ZAIDM_EX_KPPBC> ZAIDM_EX_KPPBC { get; set; }
        public virtual ICollection<ZAIDM_EX_POA> ZAIDM_EX_POA { get; set; }
    }
}
