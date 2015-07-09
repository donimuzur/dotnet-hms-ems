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
    
        public virtual DbSet<CK5> CK5 { get; set; }
        public virtual DbSet<PBCK1> PBCK1 { get; set; }
        public virtual DbSet<FILE_UPLOAD> FILE_UPLOAD { get; set; }
        public virtual DbSet<C1LFA1> C1LFA1 { get; set; }
        public virtual DbSet<BACK1> BACK1 { get; set; }
        public virtual DbSet<BACK3_CK2> BACK3_CK2 { get; set; }
        public virtual DbSet<CARRIAGE_METHOD> CARRIAGE_METHOD { get; set; }
        public virtual DbSet<CK2> CK2 { get; set; }
        public virtual DbSet<CK3> CK3 { get; set; }
        public virtual DbSet<CK4C> CK4C { get; set; }
        public virtual DbSet<CK4C_ITEM> CK4C_ITEM { get; set; }
        public virtual DbSet<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public virtual DbSet<COUNTRY> COUNTRY { get; set; }
        public virtual DbSet<CURRENCY> CURRENCY { get; set; }
        public virtual DbSet<DOC_NUMBER_SEQ> DOC_NUMBER_SEQ { get; set; }
        public virtual DbSet<EX_SETTLEMENT> EX_SETTLEMENT { get; set; }
        public virtual DbSet<EX_STATUS> EX_STATUS { get; set; }
        public virtual DbSet<HEADER_FOOTER_FORM_MAP> HEADER_FOOTER_FORM_MAP { get; set; }
        public virtual DbSet<MENGETAHUI> MENGETAHUI { get; set; }
        public virtual DbSet<MONTH> MONTH { get; set; }
        public virtual DbSet<PAGE> PAGE { get; set; }
        public virtual DbSet<PAGE_MAP> PAGE_MAP { get; set; }
        public virtual DbSet<PARAMATER> PARAMATER { get; set; }
        public virtual DbSet<PBCK1_PROD_PLAN> PBCK1_PROD_PLAN { get; set; }
        public virtual DbSet<PBCK3_7> PBCK3_7 { get; set; }
        public virtual DbSet<PBCK3_7_ITEM> PBCK3_7_ITEM { get; set; }
        public virtual DbSet<PBCK3_CK5> PBCK3_CK5 { get; set; }
        public virtual DbSet<PBCK3_CK5_ITEM> PBCK3_CK5_ITEM { get; set; }
        public virtual DbSet<PBCK4> PBCK4 { get; set; }
        public virtual DbSet<PBCK4_ITEM> PBCK4_ITEM { get; set; }
        public virtual DbSet<REALISASI_PEMASUKAN> REALISASI_PEMASUKAN { get; set; }
        public virtual DbSet<RENCANA_PRODUKSI> RENCANA_PRODUKSI { get; set; }
        public virtual DbSet<REQUEST_TYPE> REQUEST_TYPE { get; set; }
        public virtual DbSet<SUPPLIER_PORT> SUPPLIER_PORT { get; set; }
        public virtual DbSet<T1001> T1001 { get; set; }
        public virtual DbSet<T1001K> T1001K { get; set; }
        public virtual DbSet<UOM> UOM { get; set; }
        public virtual DbSet<USER> USER { get; set; }
        public virtual DbSet<USER_GROUP> USER_GROUP { get; set; }
        public virtual DbSet<ZAIDM_EX_GOODTYP> ZAIDM_EX_GOODTYP { get; set; }
        public virtual DbSet<ZAIDM_EX_KPPBC> ZAIDM_EX_KPPBC { get; set; }
        public virtual DbSet<ZAIDM_EX_MARKET> ZAIDM_EX_MARKET { get; set; }
        public virtual DbSet<ZAIDM_EX_PCODE> ZAIDM_EX_PCODE { get; set; }
        public virtual DbSet<ZAIDM_EX_PRODTYP> ZAIDM_EX_PRODTYP { get; set; }
        public virtual DbSet<ZAIDM_EX_SERIES> ZAIDM_EX_SERIES { get; set; }
        public virtual DbSet<ZAIDM_POA_MAP> ZAIDM_POA_MAP { get; set; }
        public virtual DbSet<EX_GROUP_TYPE> EX_GROUP_TYPE { get; set; }
        public virtual DbSet<EMAIL_TEMPLATE> EMAIL_TEMPLATE { get; set; }
        public virtual DbSet<WORKFLOW_EMAIL> WORKFLOW_EMAIL { get; set; }
        public virtual DbSet<VIRTUAL_PLANT_MAP> VIRTUAL_PLANT_MAP { get; set; }
        public virtual DbSet<CHANGES_HISTORY> CHANGES_HISTORY { get; set; }
        public virtual DbSet<HEADER_FOOTER> HEADER_FOOTER { get; set; }
        public virtual DbSet<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual DbSet<ZAIDM_EX_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
        public virtual DbSet<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual DbSet<T1001W> T1001W { get; set; }
        public virtual DbSet<PLANT_RECEIVE_MATERIAL> PLANT_RECEIVE_MATERIAL { get; set; }
        public virtual DbSet<ZAIDM_EX_POA> ZAIDM_EX_POA { get; set; }
        public virtual DbSet<WORKFLOW_HISTORY> WORKFLOW_HISTORY { get; set; }
        public virtual DbSet<PBCK1_PROD_CONVERTER> PBCK1_PROD_CONVERTER { get; set; }
    }
}
