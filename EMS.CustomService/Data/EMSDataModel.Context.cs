﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sampoerna.EMS.CustomService.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EMSDataModel : DbContext
    {
        public EMSDataModel()
            : base("name=EMSDataModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<USER> USER { get; set; }
        public virtual DbSet<SYS_REFFERENCES> SYS_REFFERENCES { get; set; }
        public virtual DbSet<SYS_REFFERENCES_TYPE> SYS_REFFERENCES_TYPE { get; set; }
        public virtual DbSet<T001> T001 { get; set; }
        public virtual DbSet<MASTER_FINANCIAL_RATIO> MASTER_FINANCIAL_RATIO { get; set; }
        public virtual DbSet<WORKFLOW_HISTORY> WORKFLOW_HISTORY { get; set; }
        public virtual DbSet<CONTENTEMAIL> CONTENTEMAIL { get; set; }
        public virtual DbSet<EMAILVARIABEL> EMAILVARIABEL { get; set; }
        public virtual DbSet<MASTER_SUPPORTING_DOCUMENT> MASTER_SUPPORTING_DOCUMENT { get; set; }
        public virtual DbSet<TARIFF> TARIFF { get; set; }
        public virtual DbSet<MASTER_PRODUCT_TYPE> ZAIDM_EX_PRODTYP { get; set; }
        public virtual DbSet<MASTER_NPPBKC> ZAIDM_EX_NPPBKC { get; set; }
        public virtual DbSet<MASTER_PLANT> T001W { get; set; }
        public virtual DbSet<LFA1> LFA1 { get; set; }
        public virtual DbSet<MASTER_KPPBC> ZAIDM_EX_KPPBC { get; set; }
        public virtual DbSet<CK1> CK1 { get; set; }
        public virtual DbSet<EXCISE_CREDIT_ADJUST_CALDETAIL> EXCISE_CREDIT_ADJUST_CALDETAIL { get; set; }
        public virtual DbSet<EXCISE_CREDIT_DETAILCK1> EXCISE_CREDIT_DETAILCK1 { get; set; }
        public virtual DbSet<MASTER_CITY> MASTER_CITY { get; set; }
        public virtual DbSet<POA> POA { get; set; }
        public virtual DbSet<COMPANY_PLANT_MAPPING> T001K { get; set; }
        public virtual DbSet<EXCISE_ADJUSTMENT_CALCULATE> EXCISE_ADJUSTMENT_CALCULATE { get; set; }
        public virtual DbSet<CK1_ITEM> CK1_ITEM { get; set; }
        public virtual DbSet<POA_MAP> POA_MAP { get; set; }
        public virtual DbSet<POA_EXCISER> POA_EXCISER { get; set; }
        public virtual DbSet<ZAIDM_EX_BRAND> ZAIDM_EX_BRAND { get; set; }
        public virtual DbSet<ZAIDM_EX_MARKET> ZAIDM_EX_MARKET { get; set; }
        public virtual DbSet<ZAIDM_EX_MATERIAL> ZAIDM_EX_MATERIAL { get; set; }
        public virtual DbSet<FILE_UPLOAD> FILE_UPLOAD { get; set; }
        public virtual DbSet<CHANGES_HISTORY> CHANGES_HISTORY { get; set; }
        public virtual DbSet<BROLE_MAP> BROLE_MAP { get; set; }
        public virtual DbSet<ADMIN_APPROVAL_VIEW> ADMIN_APPROVAL_VIEW { get; set; }
        public virtual DbSet<INTERVIEW_REQUEST> INTERVIEW_REQUEST { get; set; }
        public virtual DbSet<INTERVIEW_REQUEST_DETAIL> INTERVIEW_REQUEST_DETAIL { get; set; }
        public virtual DbSet<MANUFACTURING_BOUND_CONDITION> MANUFACTURING_BOUND_CONDITION { get; set; }
        public virtual DbSet<MANUFACTURING_LISENCE_REQUEST> MANUFACTURING_LISENCE_REQUEST { get; set; }
        public virtual DbSet<MANUFACTURING_PRODUCT_TYPE> MANUFACTURING_PRODUCT_TYPE { get; set; }
        public virtual DbSet<REPLACEMENT_DOCUMENTS> REPLACEMENT_DOCUMENTS { get; set; }
        public virtual DbSet<REPLACEMENT_DOCUMENTS_DETAIL> REPLACEMENT_DOCUMENTS_DETAIL { get; set; }
        public virtual DbSet<MASTER_STATE> MASTER_STATE { get; set; }
        public virtual DbSet<POA_DELEGATION> POA_DELEGATION { get; set; }
        public virtual DbSet<EXCISE_CREDIT> EXCISE_CREDIT { get; set; }
        public virtual DbSet<CK1_EXCISE_CALCULATE> CK1_EXCISE_CALCULATE { get; set; }
        public virtual DbSet<ROLE_ADMIN_APPROVER_VIEW> ROLE_ADMIN_APPROVER_VIEW { get; set; }
        public virtual DbSet<PRODUCT_DEVELOPMENT_DETAIL> PRODUCT_DEVELOPMENT_DETAIL { get; set; }
        public virtual DbSet<PRODUCT_DEVELOPMENT> PRODUCT_DEVELOPMENT { get; set; }
        public virtual DbSet<vwMLLicenseRequest> vwMLLicenseRequest { get; set; }
        public virtual DbSet<vwMLInterviewRequest> vwMLInterviewRequest { get; set; }
        public virtual DbSet<PRINT_HISTORY> PRINT_HISTORY { get; set; }
        public virtual DbSet<PRINTOUT_LAYOUT> PRINTOUT_LAYOUT { get; set; }
        public virtual DbSet<PRINTOUT_VARIABLE> PRINTOUT_VARIABLE { get; set; }
        public virtual DbSet<BRAND_REGISTRATION_REQ> BRAND_REGISTRATION_REQ { get; set; }
        public virtual DbSet<BRAND_REGISTRATION_REQ_DETAIL> BRAND_REGISTRATION_REQ_DETAIL { get; set; }
        public virtual DbSet<RECEIVED_DECREE_DETAIL> RECEIVED_DECREE_DETAIL { get; set; }
        public virtual DbSet<RECEIVED_DECREE> RECEIVED_DECREE { get; set; }
        public virtual DbSet<vwPenetapanSKEP> vwPenetapanSKEP { get; set; }
        public virtual DbSet<USER_PRINTOUT_LAYOUT> USER_PRINTOUT_LAYOUT { get; set; }
        public virtual DbSet<vwProductDevDetail> vwProductDevDetail { get; set; }
        public virtual DbSet<PRODUCT_DETAIL_VIEW> PRODUCT_DETAIL_VIEWSet { get; set; }
        public virtual DbSet<PRODUCT_DEVELOPMENT_UPLOAD> PRODUCT_DEVELOPMENT_UPLOAD { get; set; }
        public virtual DbSet<vwBrandRegistration> vwBrandRegistration { get; set; }
        public virtual DbSet<EXCISE_CREDIT_APPROVED_DETAIL> EXCISE_CREDIT_APPROVED_DETAIL { get; set; }
        public virtual DbSet<CK1_EXCISE_CALCULATE_ADJUST> CK1_EXCISE_CALCULATE_ADJUST { get; set; }
        public virtual DbSet<DOC_NUMBER_SEQ> DOC_NUMBER_SEQ { get; set; }
    }
}
