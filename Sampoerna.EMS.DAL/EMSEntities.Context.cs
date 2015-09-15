﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EMSEntities : DbContext
    {
        public EMSEntities()
            : base("name=EMSEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CHANGES_HISTORY> CHANGES_HISTORY { get; set; }
        public virtual DbSet<EMAIL_TEMPLATE> EMAIL_TEMPLATE { get; set; }
        public virtual DbSet<EX_GROUP_TYPE> EX_GROUP_TYPE { get; set; }
        public virtual DbSet<EX_GROUP_TYPE_DETAILS> EX_GROUP_TYPE_DETAILS { get; set; }
        public virtual DbSet<FILE_UPLOAD> FILE_UPLOAD { get; set; }
        public virtual DbSet<HEADER_FOOTER> HEADER_FOOTER { get; set; }
        public virtual DbSet<HEADER_FOOTER_FORM_MAP> HEADER_FOOTER_FORM_MAP { get; set; }
        public virtual DbSet<MENGETAHUI> MENGETAHUI { get; set; }
        public virtual DbSet<MONTH> MONTH { get; set; }
        public virtual DbSet<PAGE> PAGE { get; set; }
        public virtual DbSet<PARAMATER> PARAMATER { get; set; }
        public virtual DbSet<PLANT_RECEIVE_MATERIAL> PLANT_RECEIVE_MATERIAL { get; set; }
        public virtual DbSet<SUPPLIER_PORT> SUPPLIER_PORT { get; set; }
        public virtual DbSet<T001K> T001K { get; set; }
        public virtual DbSet<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual DbSet<ZAIDM_EX_MARKET> ZAIDM_EX_MARKET { get; set; }
        public virtual DbSet<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER { get; set; }
        public virtual DbSet<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual DbSet<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual DbSet<WORKFLOW_HISTORY> WORKFLOW_HISTORY { get; set; }
        public virtual DbSet<MATERIAL_UOM> MATERIAL_UOM { get; set; }
        public virtual DbSet<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual DbSet<DOC_NUMBER_SEQ> DOC_NUMBER_SEQ { get; set; }
        public virtual DbSet<PBCK1_DECREE_DOC> PBCK1_DECREE_DOC { get; set; }
        public virtual DbSet<POA_SK> POA_SK { get; set; }
        public virtual DbSet<CK5> CK5 { get; set; }
        public virtual DbSet<PBCK1> PBCK1 { get; set; }
        public virtual DbSet<CK5_FILE_UPLOAD> CK5_FILE_UPLOAD { get; set; }
        public virtual DbSet<WORKFLOW_STATE> WORKFLOW_STATE { get; set; }
        public virtual DbSet<WORKFLOW_STATE_USERS> WORKFLOW_STATE_USERS { get; set; }
        public virtual DbSet<PBCK1_QUOTA> PBCK1_QUOTA { get; set; }
        public virtual DbSet<BROLE_MAP> BROLE_MAP { get; set; }
        public virtual DbSet<PAGE_MAP> PAGE_MAP { get; set; }
        public virtual DbSet<USER> USER { get; set; }
        public virtual DbSet<USER_BROLE> USER_BROLE { get; set; }
        public virtual DbSet<LACK2_DOCUMENT> LACK2_DOCUMENT { get; set; }
        public virtual DbSet<LACK2_ITEM> LACK2_ITEM { get; set; }
        public virtual DbSet<PRINT_HISTORY> PRINT_HISTORY { get; set; }
        public virtual DbSet<COUNTRY> COUNTRY { get; set; }
        public virtual DbSet<POA> POA { get; set; }
        public virtual DbSet<POA_MAP> POA_MAP { get; set; }
        public virtual DbSet<CURRENCY> CURRENCY { get; set; }
        public virtual DbSet<ZAIDM_EX_PCODE> ZAIDM_EX_PCODE { get; set; }
        public virtual DbSet<LFA1> LFA1 { get; set; }
        public virtual DbSet<ZAIDM_EX_GOODTYP> ZAIDM_EX_GOODTYP { get; set; }
        public virtual DbSet<T001> T001 { get; set; }
        public virtual DbSet<T001W> T001W { get; set; }
        public virtual DbSet<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
        public virtual DbSet<ZAIDM_EX_PRODTYP> ZAIDM_EX_PRODTYP { get; set; }
        public virtual DbSet<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual DbSet<ZAIDM_EX_KPPBC> ZAIDM_EX_KPPBC { get; set; }
        public virtual DbSet<USER_PLANT_MAP> USER_PLANT_MAP { get; set; }
        public virtual DbSet<BACK1> BACK1 { get; set; }
        public virtual DbSet<BACK1_DOCUMENT> BACK1_DOCUMENT { get; set; }
        public virtual DbSet<BACK3_CK2_PBCK3_PBCK7> BACK3_CK2_PBCK3_PBCK7 { get; set; }
        public virtual DbSet<PBCK3_PBCK7> PBCK3_PBCK7 { get; set; }
        public virtual DbSet<ZAIDM_EX_SERIES> ZAIDM_EX_SERIES { get; set; }
        public virtual DbSet<UOM> UOM { get; set; }
        public virtual DbSet<LACK1> LACK1 { get; set; }
        public virtual DbSet<CK4C> CK4C { get; set; }
        public virtual DbSet<LACK2> LACK2 { get; set; }
        public virtual DbSet<BOM> BOM { get; set; }
        public virtual DbSet<INVENTORY_MOVEMENT> INVENTORY_MOVEMENT { get; set; }
        public virtual DbSet<CK4C_ITEM> CK4C_ITEM { get; set; }
        public virtual DbSet<BLOCK_STOCK> BLOCK_STOCK { get; set; }
        public virtual DbSet<PRODUCTION> PRODUCTION { get; set; }
        public virtual DbSet<WASTE> WASTE { get; set; }
        public virtual DbSet<PBCK4_DOCUMENT> PBCK4_DOCUMENT { get; set; }
        public virtual DbSet<CK1> CK1 { get; set; }
        public virtual DbSet<PBCK4_ITEM> PBCK4_ITEM { get; set; }
        public virtual DbSet<PBCK4> PBCK4 { get; set; }
        public virtual DbSet<LACK1_DOCUMENT> LACK1_DOCUMENT { get; set; }
        public virtual DbSet<LACK1_INCOME_DETAIL> LACK1_INCOME_DETAIL { get; set; }
        public virtual DbSet<LACK1_PBCK1_MAPPING> LACK1_PBCK1_MAPPING { get; set; }
        public virtual DbSet<LACK1_TRACKING> LACK1_TRACKING { get; set; }
        public virtual DbSet<LACK1_PLANT> LACK1_PLANT { get; set; }
        public virtual DbSet<LACK1_PRODUCTION_DETAIL> LACK1_PRODUCTION_DETAIL { get; set; }
        public virtual DbSet<CK4C_DECREE_DOC> CK4C_DECREE_DOC { get; set; }
        public virtual DbSet<BACK3> BACK3 { get; set; }
        public virtual DbSet<BACK3_DOCUMENT> BACK3_DOCUMENT { get; set; }
        public virtual DbSet<CK2> CK2 { get; set; }
        public virtual DbSet<CK2_DOCUMENT> CK2_DOCUMENT { get; set; }
        public virtual DbSet<PBCK3_PBCK7_ITEM> PBCK3_PBCK7_ITEM { get; set; }
    }
}
