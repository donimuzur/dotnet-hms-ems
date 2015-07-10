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
        public virtual DbSet<DOC_NUMBER_SEQ> DOC_NUMBER_SEQ { get; set; }
        public virtual DbSet<EMAIL_TEMPLATE> EMAIL_TEMPLATE { get; set; }
        public virtual DbSet<EX_GROUP_TYPE> EX_GROUP_TYPE { get; set; }
        public virtual DbSet<EX_GROUP_TYPE_DETAILS> EX_GROUP_TYPE_DETAILS { get; set; }
        public virtual DbSet<FILE_UPLOAD> FILE_UPLOAD { get; set; }
        public virtual DbSet<HEADER_FOOTER> HEADER_FOOTER { get; set; }
        public virtual DbSet<HEADER_FOOTER_FORM_MAP> HEADER_FOOTER_FORM_MAP { get; set; }
        public virtual DbSet<LFA1> LFA1 { get; set; }
        public virtual DbSet<MENGETAHUI> MENGETAHUI { get; set; }
        public virtual DbSet<MONTH> MONTH { get; set; }
        public virtual DbSet<PAGE> PAGE { get; set; }
        public virtual DbSet<PAGE_MAP> PAGE_MAP { get; set; }
        public virtual DbSet<PARAMATER> PARAMATER { get; set; }
        public virtual DbSet<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual DbSet<PLANT_RECEIVE_MATERIAL> PLANT_RECEIVE_MATERIAL { get; set; }
        public virtual DbSet<POA> POA { get; set; }
        public virtual DbSet<POA_MAP> POA_MAP { get; set; }
        public virtual DbSet<POA_SK> POA_SK { get; set; }
        public virtual DbSet<SUPPLIER_PORT> SUPPLIER_PORT { get; set; }
        public virtual DbSet<T001> T001 { get; set; }
        public virtual DbSet<T001K> T001K { get; set; }
        public virtual DbSet<T001W> T001W { get; set; }
        public virtual DbSet<USER> USER { get; set; }
        public virtual DbSet<USER_GROUP> USER_GROUP { get; set; }
        public virtual DbSet<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual DbSet<ZAIDM_EX_GOODTYP> ZAIDM_EX_GOODTYP { get; set; }
        public virtual DbSet<ZAIDM_EX_MARKET> ZAIDM_EX_MARKET { get; set; }
        public virtual DbSet<ZAIDM_EX_PCODE> ZAIDM_EX_PCODE { get; set; }
        public virtual DbSet<ZAIDM_EX_PRODTYP> ZAIDM_EX_PRODTYP { get; set; }
        public virtual DbSet<ZAIDM_EX_SERIES> ZAIDM_EX_SERIES { get; set; }
        public virtual DbSet<ZAIDM_EX_KPPBC> ZAIDM_EX_KPPBC { get; set; }
        public virtual DbSet<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
        public virtual DbSet<MATERIAL_UOM> MATERIAL_UOM { get; set; }
        public virtual DbSet<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual DbSet<PBCK1> PBCK1 { get; set; }
        public virtual DbSet<CK5> CK5 { get; set; }
        public virtual DbSet<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual DbSet<UOM> UOM { get; set; }
        public virtual DbSet<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER { get; set; }
        public virtual DbSet<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual DbSet<CARRIAGE_METHOD> CARRIAGE_METHOD { get; set; }
        public virtual DbSet<WORKFLOW_HISTORY> WORKFLOW_HISTORY { get; set; }
    }
}
