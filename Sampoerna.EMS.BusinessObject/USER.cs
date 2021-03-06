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
            this.BROLE_MAP = new HashSet<BROLE_MAP>();
            this.CHANGES_HISTORY = new HashSet<CHANGES_HISTORY>();
            this.CK5 = new HashSet<CK5>();
            this.CK51 = new HashSet<CK5>();
            this.CK52 = new HashSet<CK5>();
            this.CK5_FILE_UPLOAD = new HashSet<CK5_FILE_UPLOAD>();
            this.HEADER_FOOTER = new HashSet<HEADER_FOOTER>();
            this.HEADER_FOOTER1 = new HashSet<HEADER_FOOTER>();
            this.PBCK1 = new HashSet<PBCK1>();
            this.PBCK11 = new HashSet<PBCK1>();
            this.PBCK12 = new HashSet<PBCK1>();
            this.PBCK1_DECREE_DOC = new HashSet<PBCK1_DECREE_DOC>();
            this.PBCK1_QUOTA = new HashSet<PBCK1_QUOTA>();
            this.VIRTUAL_PLANT_MAP = new HashSet<VIRTUAL_PLANT_MAP>();
            this.VIRTUAL_PLANT_MAP1 = new HashSet<VIRTUAL_PLANT_MAP>();
            this.WORKFLOW_HISTORY = new HashSet<WORKFLOW_HISTORY>();
            this.WORKFLOW_STATE_USERS = new HashSet<WORKFLOW_STATE_USERS>();
            this.ZAIDM_EX_BRAND = new HashSet<ZAIDM_EX_BRAND>();
            this.ZAIDM_EX_BRAND1 = new HashSet<ZAIDM_EX_BRAND>();
            this.PRINT_HISTORY = new HashSet<PRINT_HISTORY>();
            this.POA = new HashSet<POA>();
            this.POA1 = new HashSet<POA>();
            this.POA2 = new HashSet<POA>();
            this.POA3 = new HashSet<POA>();
            this.POA_MAP = new HashSet<POA_MAP>();
            this.POA_MAP1 = new HashSet<POA_MAP>();
            this.PBCK3_PBCK7 = new HashSet<PBCK3_PBCK7>();
            this.PBCK3_PBCK71 = new HashSet<PBCK3_PBCK7>();
            this.PBCK3_PBCK72 = new HashSet<PBCK3_PBCK7>();
            this.LACK1 = new HashSet<LACK1>();
            this.LACK11 = new HashSet<LACK1>();
            this.CK4C = new HashSet<CK4C>();
            this.CK4C1 = new HashSet<CK4C>();
            this.LACK2 = new HashSet<LACK2>();
            this.LACK21 = new HashSet<LACK2>();
            this.LACK22 = new HashSet<LACK2>();
            this.CK4C_DECREE_DOC = new HashSet<CK4C_DECREE_DOC>();
            this.PBCK3_PBCK73 = new HashSet<PBCK3_PBCK7>();
            this.PBCK3_PBCK74 = new HashSet<PBCK3_PBCK7>();
            this.PBCK7 = new HashSet<PBCK7>();
            this.PBCK71 = new HashSet<PBCK7>();
            this.PBCK72 = new HashSet<PBCK7>();
            this.PBCK73 = new HashSet<PBCK7>();
            this.PBCK4 = new HashSet<PBCK4>();
            this.PBCK41 = new HashSet<PBCK4>();
            this.PBCK42 = new HashSet<PBCK4>();
            this.PBCK43 = new HashSet<PBCK4>();
            this.PBCK3 = new HashSet<PBCK3>();
            this.PBCK31 = new HashSet<PBCK3>();
            this.PBCK32 = new HashSet<PBCK3>();
            this.PBCK33 = new HashSet<PBCK3>();
            this.USER_PLANT_MAP = new HashSet<USER_PLANT_MAP>();
            this.WASTE_ROLE = new HashSet<WASTE_ROLE>();
            this.WASTE_ROLE1 = new HashSet<WASTE_ROLE>();
            this.WASTE_ROLE2 = new HashSet<WASTE_ROLE>();
            this.WASTE_STOCK = new HashSet<WASTE_STOCK>();
            this.WASTE_STOCK1 = new HashSet<WASTE_STOCK>();
            this.WASTE_STOCK2 = new HashSet<WASTE_STOCK>();
            this.POA_DELEGATION = new HashSet<POA_DELEGATION>();
            this.POA_DELEGATION1 = new HashSet<POA_DELEGATION>();
            this.POA_DELEGATION2 = new HashSet<POA_DELEGATION>();
            this.POA_DELEGATION3 = new HashSet<POA_DELEGATION>();
            this.MASTER_DATA_APPROVAL = new HashSet<MASTER_DATA_APPROVAL>();
            this.MASTER_DATA_APPROVAL1 = new HashSet<MASTER_DATA_APPROVAL>();
            this.QUOTA_MONITORING_DETAIL = new HashSet<QUOTA_MONITORING_DETAIL>();
            this.ZAIDM_EX_PRODTYP = new HashSet<ZAIDM_EX_PRODTYP>();
            this.FILE_UPLOAD = new HashSet<FILE_UPLOAD>();
            this.FILE_UPLOAD1 = new HashSet<FILE_UPLOAD>();
            this.LACK10 = new HashSet<LACK10>();
            this.LACK101 = new HashSet<LACK10>();
            this.LACK102 = new HashSet<LACK10>();
            this.LACK10_DECREE_DOC = new HashSet<LACK10_DECREE_DOC>();
        }
    
        public string USER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<int> IS_ACTIVE { get; set; }
        public string ACCT { get; set; }
        public Nullable<bool> IS_MASTER_DATA_APPROVER { get; set; }
    
        public virtual ICollection<BROLE_MAP> BROLE_MAP { get; set; }
        public virtual ICollection<CHANGES_HISTORY> CHANGES_HISTORY { get; set; }
        public virtual ICollection<CK5> CK5 { get; set; }
        public virtual ICollection<CK5> CK51 { get; set; }
        public virtual ICollection<CK5> CK52 { get; set; }
        public virtual ICollection<CK5_FILE_UPLOAD> CK5_FILE_UPLOAD { get; set; }
        public virtual ICollection<HEADER_FOOTER> HEADER_FOOTER { get; set; }
        public virtual ICollection<HEADER_FOOTER> HEADER_FOOTER1 { get; set; }
        public virtual ICollection<PBCK1> PBCK1 { get; set; }
        public virtual ICollection<PBCK1> PBCK11 { get; set; }
        public virtual ICollection<PBCK1> PBCK12 { get; set; }
        public virtual ICollection<PBCK1_DECREE_DOC> PBCK1_DECREE_DOC { get; set; }
        public virtual ICollection<PBCK1_QUOTA> PBCK1_QUOTA { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual ICollection<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP1 { get; set; }
        public virtual ICollection<WORKFLOW_HISTORY> WORKFLOW_HISTORY { get; set; }
        public virtual ICollection<WORKFLOW_STATE_USERS> WORKFLOW_STATE_USERS { get; set; }
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual ICollection<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND1 { get; set; }
        public virtual ICollection<PRINT_HISTORY> PRINT_HISTORY { get; set; }
        public virtual ICollection<POA> POA { get; set; }
        public virtual ICollection<POA> POA1 { get; set; }
        public virtual ICollection<POA> POA2 { get; set; }
        public virtual ICollection<POA> POA3 { get; set; }
        public virtual ICollection<POA_MAP> POA_MAP { get; set; }
        public virtual ICollection<POA_MAP> POA_MAP1 { get; set; }
        public virtual ICollection<PBCK3_PBCK7> PBCK3_PBCK7 { get; set; }
        public virtual ICollection<PBCK3_PBCK7> PBCK3_PBCK71 { get; set; }
        public virtual ICollection<PBCK3_PBCK7> PBCK3_PBCK72 { get; set; }
        public virtual ICollection<LACK1> LACK1 { get; set; }
        public virtual ICollection<LACK1> LACK11 { get; set; }
        public virtual ICollection<CK4C> CK4C { get; set; }
        public virtual ICollection<CK4C> CK4C1 { get; set; }
        public virtual ICollection<LACK2> LACK2 { get; set; }
        public virtual ICollection<LACK2> LACK21 { get; set; }
        public virtual ICollection<LACK2> LACK22 { get; set; }
        public virtual ICollection<CK4C_DECREE_DOC> CK4C_DECREE_DOC { get; set; }
        public virtual ICollection<PBCK3_PBCK7> PBCK3_PBCK73 { get; set; }
        public virtual ICollection<PBCK3_PBCK7> PBCK3_PBCK74 { get; set; }
        public virtual ICollection<PBCK7> PBCK7 { get; set; }
        public virtual ICollection<PBCK7> PBCK71 { get; set; }
        public virtual ICollection<PBCK7> PBCK72 { get; set; }
        public virtual ICollection<PBCK7> PBCK73 { get; set; }
        public virtual ICollection<PBCK4> PBCK4 { get; set; }
        public virtual ICollection<PBCK4> PBCK41 { get; set; }
        public virtual ICollection<PBCK4> PBCK42 { get; set; }
        public virtual ICollection<PBCK4> PBCK43 { get; set; }
        public virtual ICollection<PBCK3> PBCK3 { get; set; }
        public virtual ICollection<PBCK3> PBCK31 { get; set; }
        public virtual ICollection<PBCK3> PBCK32 { get; set; }
        public virtual ICollection<PBCK3> PBCK33 { get; set; }
        public virtual ICollection<USER_PLANT_MAP> USER_PLANT_MAP { get; set; }
        public virtual ICollection<WASTE_ROLE> WASTE_ROLE { get; set; }
        public virtual ICollection<WASTE_ROLE> WASTE_ROLE1 { get; set; }
        public virtual ICollection<WASTE_ROLE> WASTE_ROLE2 { get; set; }
        public virtual ICollection<WASTE_STOCK> WASTE_STOCK { get; set; }
        public virtual ICollection<WASTE_STOCK> WASTE_STOCK1 { get; set; }
        public virtual ICollection<WASTE_STOCK> WASTE_STOCK2 { get; set; }
        public virtual ICollection<POA_DELEGATION> POA_DELEGATION { get; set; }
        public virtual ICollection<POA_DELEGATION> POA_DELEGATION1 { get; set; }
        public virtual ICollection<POA_DELEGATION> POA_DELEGATION2 { get; set; }
        public virtual ICollection<POA_DELEGATION> POA_DELEGATION3 { get; set; }
        public virtual ICollection<MASTER_DATA_APPROVAL> MASTER_DATA_APPROVAL { get; set; }
        public virtual ICollection<MASTER_DATA_APPROVAL> MASTER_DATA_APPROVAL1 { get; set; }
        public virtual ICollection<QUOTA_MONITORING_DETAIL> QUOTA_MONITORING_DETAIL { get; set; }
        public virtual ICollection<ZAIDM_EX_PRODTYP> ZAIDM_EX_PRODTYP { get; set; }
        public virtual ICollection<FILE_UPLOAD> FILE_UPLOAD { get; set; }
        public virtual ICollection<FILE_UPLOAD> FILE_UPLOAD1 { get; set; }
        public virtual ICollection<LACK10> LACK10 { get; set; }
        public virtual ICollection<LACK10> LACK101 { get; set; }
        public virtual ICollection<LACK10> LACK102 { get; set; }
        public virtual ICollection<LACK10_DECREE_DOC> LACK10_DECREE_DOC { get; set; }
    }
}
