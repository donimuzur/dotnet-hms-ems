﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Utils;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void Initialize()
        {
            InitializeCK5();
            InitializePBCK1();
            InitializePbck7And3();
            InitializeCk4C();

            InitializeLack1();
            InitializeLack2();

            InitializePbck4();
            InitializeLack10();

            //Mapper.CreateMap<USER, UserTree>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.USER2))
            //    .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.USER1));
            //Mapper.CreateMap<USER, Login>().IgnoreAllNonExisting();

            Mapper.CreateMap<HEADER_FOOTER, HeaderFooter>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.T001.BUKRS))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.T001.BUTXT))
                .ForMember(dest => dest.COMPANY_NPWP, opt => opt.MapFrom(src => src.T001.NPWP));

            Mapper.CreateMap<HEADER_FOOTER_FORM_MAP, HeaderFooterMap>().IgnoreAllNonExisting();

            Mapper.CreateMap<HEADER_FOOTER, HeaderFooterDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.T001.BUKRS))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.T001.BUTXT))
                .ForMember(dest => dest.COMPANY_NPWP, opt => opt.MapFrom(src => src.T001.NPWP))
                .ForMember(dest => dest.HeaderFooterMapList, opt => opt.MapFrom(src => Mapper.Map<List<HeaderFooterMap>>(src.HEADER_FOOTER_FORM_MAP)));

            Mapper.CreateMap<HeaderFooterMap, HEADER_FOOTER_FORM_MAP>().IgnoreAllUnmapped()
                .ForMember(dest => dest.HEADER_FOOTER_FORM_MAP_ID, opt => opt.MapFrom(src => src.HEADER_FOOTER_FORM_MAP_ID))
                .ForMember(dest => dest.FORM_TYPE_ID, opt => opt.MapFrom(src => src.FORM_TYPE_ID))
                .ForMember(dest => dest.IS_HEADER_SET, opt => opt.MapFrom(src => src.IS_HEADER_SET))
                .ForMember(dest => dest.IS_FOOTER_SET, opt => opt.MapFrom(src => src.IS_FOOTER_SET))
                .ForMember(dest => dest.HEADER_FOOTER_ID, opt => opt.MapFrom(src => src.HEADER_FOOTER_ID));

            Mapper.CreateMap<HeaderFooterDetails, HEADER_FOOTER>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.COMPANY_ID))
                .ForMember(dest => dest.HEADER_FOOTER_FORM_MAP, opt => opt.MapFrom(src => Mapper.Map<List<HEADER_FOOTER_FORM_MAP>>(src.HeaderFooterMapList)));

            Mapper.CreateMap<T001W, AutoCompletePlant>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.WERKS));

            Mapper.CreateMap<Plant, T001W>().IgnoreAllNonExisting();
            Mapper.CreateMap<T001W, Plant>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPWP, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.NPWP))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.T001K.T001.BUTXT))
                .ForMember(dest => dest.COMPANY_ADDRESS, opt => opt.MapFrom(src => src.T001K.T001.SPRAS))
                .ForMember(dest => dest.KPPBC_CITY, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.CITY))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.NPPBKC_IMPORT_ID, opt => opt.MapFrom(src => src.NPPBKC_IMPORT_ID))
                .ForMember(dest => dest.ORT01, opt => opt.MapFrom(src => src.ORT01))
               .ForMember(dest => dest.KPPBC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC == null ? string.Empty : src.ZAIDM_EX_NPPBKC.KPPBC_ID))
               .ForMember(dest => dest.DROPDOWNTEXTFIELD, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1))
               .ForMember(dest => dest.SUPPLIER_COMPANY, opt => opt.MapFrom(src => src.T001K.T001.BUTXT))
               .ForMember(dest => dest.COMPANY_CODE, opt => opt.MapFrom(src => src.T001K.T001.BUKRS))

                ;

            Mapper.CreateMap<T001W, T001WDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Npwp, opt => opt.MapFrom(src => src.T001K.T001.NPWP))
                  .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.T001K.T001.BUTXT))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.T001K.T001.SPRAS))
                .ForMember(dest => dest.KppbcCity, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.CITY))
                 .ForMember(dest => dest.KppbcNo, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC == null ? string.Empty : src.ZAIDM_EX_NPPBKC.KPPBC_ID))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.T001K == null || src.T001K.T001 == null ? string.Empty : src.T001K.T001.BUKRS))
                .ForMember(dest => dest.DROPDOWNTEXTFIELD, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1));

            #region Workflow History

            Mapper.CreateMap<WORKFLOW_HISTORY, WorkflowHistoryDto>().IgnoreAllNonExisting()
                  .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.USER.USER_ID))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.USER.FIRST_NAME))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.USER.LAST_NAME));

            Mapper.CreateMap<WorkflowHistoryDto, WORKFLOW_HISTORY>().IgnoreAllNonExisting();

            #endregion


            #region Pbck1ProdConv

            Mapper.CreateMap<Pbck1ProdConverterInput, Pbck1ProdConverterOutput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdPlanInput, Pbck1ProdPlanOutput>().IgnoreAllNonExisting();

            #endregion

            Mapper.CreateMap<POA_MAP, POA_MAPDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.POA_MAP_ID, opt => opt.MapFrom(src => src.POA_MAP_ID))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.POA_ID, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.T001W.NAME1))
                .ForMember(dest => dest.POA_NAME, opt => opt.MapFrom(src => src.POA.PRINTED_NAME));
            Mapper.CreateMap<POA_MAPDto, POA_MAP>().IgnoreAllNonExisting()
                .ForMember(dest => dest.POA_MAP_ID, opt => opt.MapFrom(src => src.POA_MAP_ID))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.POA_ID, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.WERKS));
            
            Mapper.CreateMap<T001W, PlantDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.NPPBKC_IMPORT_ID, opt => opt.MapFrom(src => src.NPPBKC_IMPORT_ID))
                .ForMember(dest => dest.NPPBKC_IMPORT_ID, opt => opt.MapFrom(src => src.NPPBKC_IMPORT_ID))
                .ForMember(dest => dest.NAME1, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID));


            Mapper.CreateMap<PRINT_HISTORY, PrintHistoryDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<PrintHistoryDto, PRINT_HISTORY>().IgnoreAllNonExisting();

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, ZAIDM_EX_NPPBKCDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DROPDOWNTEXT, opt => opt.MapFrom(src => src.NPPBKC_ID))
                ;
            Mapper.CreateMap<ZAIDM_EX_NPPBKC, ZAIDM_EX_NPPBKCCompositeDto>().IgnoreAllNonExisting()
                ;
            Mapper.CreateMap<ZAIDM_EX_KPPBC, ZAIDM_EX_KPPBCDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.KPPBC_ID, opt => opt.MapFrom(src => src.KPPBC_ID))
                .ForMember(dest => dest.KPPBC_TYPE, opt => opt.MapFrom(src => src.KPPBC_TYPE))
                .ForMember(dest => dest.MENGETAHUI, opt => opt.MapFrom(src => src.MENGETAHUI))
                .ForMember(dest => dest.MENGETAHUI_DETAIL, opt => opt.MapFrom(src => src.MENGETAHUI_DETAIL))
                .ForMember(dest => dest.CK1_KEP_HEADER, opt => opt.MapFrom(src => src.CK1_KEP_HEADER))
                .ForMember(dest => dest.CK1_KEP_FOOTER, opt => opt.MapFrom(src => src.CK1_KEP_FOOTER))
                .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IS_DELETED));


            Mapper.CreateMap<LFA1, LFA1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_DELETED_STRING, opt => opt.MapFrom(src => src.IS_DELETED.HasValue ? src.IS_DELETED.Value ? "Yes" : "No" : "No"))
                .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IS_DELETED.HasValue ? src.IS_DELETED.Value : false));
            Mapper.CreateMap<T001, T001Dto>().IgnoreAllNonExisting();
            

            #region UserAuthorization

            Mapper.CreateMap<USER, UserDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.FIRST_NAME))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.LAST_NAME))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.EMAIL));
            Mapper.CreateMap<PAGE, PageDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PAGE_ID))
                .ForMember(dest => dest.PageName, opt => opt.MapFrom(src => src.PAGE_NAME));
            Mapper.CreateMap<PageDto, PAGE>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PAGE_ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PAGE_NAME, opt => opt.MapFrom(src => src.PageName));

            Mapper.CreateMap<PAGE_MAP, PageMapDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PAGE_MAP_ID))
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.PAGE));
            Mapper.CreateMap<PageMapDto, PAGE_MAP>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PAGE, opt => opt.MapFrom(src => src.Page))
               .ForMember(dest => dest.BROLE, opt => opt.MapFrom(src => src.Brole));

            Mapper.CreateMap<BROLE_MAP, BRoleMapDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BROLE_MAP_ID))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.USER))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE));

            Mapper.CreateMap<USER_BROLE, UserAuthorizationDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Brole, opt => opt.MapFrom(src => src.BROLE))
                .ForMember(dest => dest.BroleDescription, opt => opt.MapFrom(src => src.BROLE_DESC))
                .ForMember(dest => dest.BRoleMaps, opt => opt.MapFrom(src => src.BROLE_MAP))
                .ForMember(dest => dest.PageMaps, opt => opt.MapFrom(src => src.PAGE_MAP));

            Mapper.CreateMap<USER_BROLE, BRoleDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BROLE))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.BROLE_DESC));


            #endregion

            #region HEADER_FOOTER DTO

            Mapper.CreateMap<HEADER_FOOTER, HEADER_FOOTERDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<HEADER_FOOTER_FORM_MAP, HEADER_FOOTER_MAPDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.HEADER_IMAGE_PATH, opt => opt.MapFrom(src => src.HEADER_FOOTER.HEADER_IMAGE_PATH))
                .ForMember(dest => dest.FOOTER_CONTENT, opt => opt.MapFrom(src => src.HEADER_FOOTER.FOOTER_CONTENT))
                ;

            #endregion

            Mapper.CreateMap<POA, POADto>().IgnoreAllNonExisting();
            Mapper.CreateMap<POADto, POA>().IgnoreAllNonExisting();

            Mapper.CreateMap<T001K, T001KDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BUTXT, opt => opt.MapFrom(src => src.T001.BUTXT))
                .ForMember(dest => dest.NPWP, opt => opt.MapFrom(src => src.T001.NPWP))
                .ForMember(dest => dest.ORT01, opt => opt.MapFrom(src => src.T001.ORT01))
                .ForMember(dest => dest.SPRAS, opt => opt.MapFrom(src => src.T001.SPRAS))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.T001W.NPPBKC_ID))
                .ForMember(dest => dest.NPPBKC_KPPBC_ID, opt => opt.MapFrom(src => src.T001W.ZAIDM_EX_NPPBKC.KPPBC_ID))
                ;
            Mapper.CreateMap<USER_PLANT_MAP, UserPlantMapDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.USER_PLANT_MAP_ID))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.NAME1))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.USER.FIRST_NAME + " " + src.USER.LAST_NAME))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE == false || src.IS_ACTIVE == null ? "No" : "Yes"));

            Mapper.CreateMap<UserPlantMapDto, USER_PLANT_MAP>().IgnoreAllNonExisting()
                .ForMember(dest => dest.USER_PLANT_MAP_ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.USER_ID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.ResolveUsing<StringToBooleanResolver>().FromMember(src => src.IsActive));


            #region ExGoodTyp

            Mapper.CreateMap<EX_GROUP_TYPE, ExGoodTyp>().IgnoreAllNonExisting();


            #endregion

            #region Material Dto
            Mapper.CreateMap<MaterialDto, ZAIDM_EX_MATERIAL>().IgnoreAllNonExisting();
            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.GoodTypeDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.T001W, opt => opt.MapFrom(src => src.T001W))
                ;

            #endregion
            #region Email Template

            Mapper.CreateMap<EMAIL_TEMPLATE, EMAIL_TEMPLATEDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<EMAIL_TEMPLATEDto, EMAIL_TEMPLATE>().IgnoreAllNonExisting();

            #endregion

            #region Country

            Mapper.CreateMap<COUNTRY, CountryDto>().IgnoreAllNonExisting();

            #endregion

            Mapper.CreateMap<PBCK1, ZAIDM_EX_NPPBKCCompositeDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<T001W, T001WCompositeDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DROPDOWNTEXTFIELD, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1));
            
            Mapper.CreateMap<PBCK1, ZAIDM_EX_NPPBKCCompositeDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<PBCK1, T001WCompositeDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.SUPPLIER_PLANT_WERKS) ? src.SUPPLIER_PLANT_WERKS : src.SUPPLIER_PLANT))
                .ForMember(dest => dest.DROPDOWNTEXTFIELD,
                    opt => opt.MapFrom(src => (!string.IsNullOrEmpty(src.SUPPLIER_PLANT_WERKS) ? src.SUPPLIER_PLANT_WERKS : src.SUPPLIER_PLANT) + "-" + src.SUPPLIER_PLANT));
            Mapper.CreateMap<PBCK1, ZAIDM_EX_GOODTYPCompositeDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EXT_TYP_DESC, opt => opt.MapFrom(src => src.EXC_TYP_DESC))
                ;

            #region Production for CK4C

            Mapper.CreateMap<PRODUCTION, ProductionDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.PRODUCTION_DATE))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.COMPANY_CODE))
                .ForMember(dest => dest.PlantWerks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.BrandDescription, opt => opt.MapFrom(src => src.BRAND_DESC))
                .ForMember(dest => dest.QtyPacked, opt => opt.MapFrom(src => src.QTY_PACKED))
                .ForMember(dest => dest.QtyProduced, opt => opt.MapFrom(src => src.QTY))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UOM))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.ProdQtyStick, opt => opt.MapFrom(src => src.PROD_QTY_STICK))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.QTY))
                .ForMember(dest => dest.Bundle, opt => opt.MapFrom(src => src.BUNDLE))
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.MARKET))
                .ForMember(dest => dest.Docgmvter, opt => opt.MapFrom(src => src.DOCGMVTER))
                .ForMember(dest => dest.MatDoc, opt => opt.MapFrom(src => src.MATDOC))
                .ForMember(dest => dest.Ordr, opt => opt.MapFrom(src => src.ORDR))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(dest => dest.MODIFIED_BY))
                .ForMember(dest => dest.Batch, opt => opt.MapFrom(src => src.BATCH));



            Mapper.CreateMap<ProductionDto, PRODUCTION>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PRODUCTION_DATE, opt => opt.MapFrom(src => src.ProductionDate))
               .ForMember(dest => dest.COMPANY_CODE, opt => opt.MapFrom(src => src.CompanyCode))
               .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantWerks))
               .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
               .ForMember(dest => dest.BRAND_DESC, opt => opt.MapFrom(src => src.BrandDescription))
               .ForMember(dest => dest.QTY_PACKED, opt => opt.MapFrom(src => src.QtyPacked))
               .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.Uom))
               .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantName))
               .ForMember(dest => dest.PROD_QTY_STICK, opt => opt.MapFrom(src => src.ProdQtyStick))
               .ForMember(dest => dest.QTY, opt => opt.MapFrom(src => src.Qty))
               .ForMember(dest => dest.BUNDLE, opt => opt.MapFrom(src => src.Bundle))
               .ForMember(dest => dest.MARKET, opt => opt.MapFrom(src => src.Market))
               .ForMember(dest => dest.DOCGMVTER, opt => opt.MapFrom(src => src.Docgmvter))
               .ForMember(dest => dest.MATDOC, opt => opt.MapFrom(src => src.MatDoc))
               .ForMember(dest => dest.ORDR, opt => opt.MapFrom(src => src.Ordr))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(dest => dest.ModifiedBy))
               .ForMember(dest => dest.BATCH, opt => opt.MapFrom(src => src.Batch));

            Mapper.CreateMap<ProductionUploadItems, PRODUCTION>().IgnoreAllNonExisting()
               .ForMember(dest => dest.COMPANY_CODE, opt => opt.MapFrom(src => src.CompanyCode))
               .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantWerks))
               .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
               .ForMember(dest => dest.BRAND_DESC, opt => opt.MapFrom(src => src.BrandDescription))
               .ForMember(dest => dest.QTY_PACKED, opt => opt.MapFrom(src => src.QtyPacked))
               .ForMember(dest => dest.QTY, opt => opt.MapFrom(src => src.Qty))
               .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.Uom))
               .ForMember(dest => dest.PRODUCTION_DATE, opt => opt.MapFrom(src => src.ProductionDate))
               .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantName))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.ZB, opt => opt.MapFrom(src => src.Zb))
               .ForMember(dest => dest.PACKED_ADJUSTED, opt => opt.MapFrom(src => src.PackedAdjusted))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(dest => dest.ModifiedBy));
            

            Mapper.CreateMap<PRODUCTION, ProductionUploadItems>().IgnoreAllNonExisting()
            .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.COMPANY_CODE))
            .ForMember(dest => dest.PlantWerks, opt => opt.MapFrom(src => src.WERKS))
            .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
            .ForMember(dest => dest.BrandDescription, opt => opt.MapFrom(src => src.BRAND_DESC))
            .ForMember(dest => dest.QtyPacked, opt => opt.MapFrom(src => src.QTY_PACKED))
            .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.QTY))
            .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UOM))
            .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.PRODUCTION_DATE))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
            .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PLANT_NAME))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(dest => dest.MODIFIED_BY));

            #endregion

            #region Waste For CK4C

            Mapper.CreateMap<WASTE, WasteDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.COMPANY_CODE))
                .ForMember(dest => dest.PlantWerks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.BrandDescription, opt => opt.MapFrom(src => src.BRAND_DESC))
                .ForMember(dest => dest.MarkerRejectStickQty, opt => opt.MapFrom(src => src.MARKER_REJECT_STICK_QTY))
                .ForMember(dest => dest.PackerRejectStickQty, opt => opt.MapFrom(src => src.PACKER_REJECT_STICK_QTY))
                .ForMember(dest => dest.WasteProductionDate, opt => opt.MapFrom(src => src.WASTE_PROD_DATE))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.DustWasteGramQty, opt => opt.MapFrom(src => src.DUST_WASTE_GRAM_QTY))
                .ForMember(dest => dest.FloorWasteGramQty, opt => opt.MapFrom(src => src.FLOOR_WASTE_GRAM_QTY))
                .ForMember(dest => dest.DustWasteStickQty, opt => opt.MapFrom(src => src.DUST_WASTE_STICK_QTY))
                .ForMember(dest => dest.FloorWasteStickQty, opt => opt.MapFrom(src => src.FLOOR_WASTE_STICK_QTY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(dest => dest.MODIFIED_BY))
                .ForMember(dest => dest.StampWasteQty, opt => opt.MapFrom(dest=> dest.STAMP_WASTE_QTY))
                .ForMember(dest => dest.UseForLack10, opt => opt.MapFrom(src => src.USE_FOR_LACK10));


            Mapper.CreateMap<WasteDto, WASTE>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_CODE, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantWerks))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.BRAND_DESC, opt => opt.MapFrom(src => src.BrandDescription))
                .ForMember(dest => dest.MARKER_REJECT_STICK_QTY, opt => opt.MapFrom(src => src.MarkerRejectStickQty))
                .ForMember(dest => dest.PACKER_REJECT_STICK_QTY, opt => opt.MapFrom(src => src.PackerRejectStickQty))
                .ForMember(dest => dest.WASTE_PROD_DATE, opt => opt.MapFrom(src => src.WasteProductionDate))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantName))
                .ForMember(dest => dest.DUST_WASTE_GRAM_QTY, opt => opt.MapFrom(src => src.DustWasteGramQty))
                .ForMember(dest => dest.FLOOR_WASTE_GRAM_QTY, opt => opt.MapFrom(src => src.FloorWasteGramQty))
                .ForMember(dest => dest.DUST_WASTE_STICK_QTY, opt => opt.MapFrom(src => src.DustWasteStickQty))
                .ForMember(dest => dest.FLOOR_WASTE_STICK_QTY, opt => opt.MapFrom(src => src.FloorWasteStickQty))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(dest => dest.ModifiedBy))
                .ForMember(dest => dest.STAMP_WASTE_QTY, opt => opt.MapFrom(dest => dest.StampWasteQty))
                .ForMember(dest => dest.USE_FOR_LACK10, opt => opt.MapFrom(dest => dest.UseForLack10));


            Mapper.CreateMap<WasteUploadItems, WASTE>().IgnoreAllNonExisting()
               .ForMember(dest => dest.COMPANY_CODE, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantWerks))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.BRAND_DESC, opt => opt.MapFrom(src => src.BrandDescription))
                .ForMember(dest => dest.MARKER_REJECT_STICK_QTY, opt => opt.MapFrom(src => src.MarkerRejectStickQty))
                .ForMember(dest => dest.PACKER_REJECT_STICK_QTY, opt => opt.MapFrom(src => src.PackerRejectStickQty))
                .ForMember(dest => dest.WASTE_PROD_DATE, opt => opt.MapFrom(src => src.WasteProductionDate))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantName))
                .ForMember(dest => dest.DUST_WASTE_GRAM_QTY, opt => opt.MapFrom(src => src.DustWasteGramQty))
                .ForMember(dest => dest.FLOOR_WASTE_GRAM_QTY, opt => opt.MapFrom(src => src.FloorWasteGramQty))
                .ForMember(dest => dest.DUST_WASTE_STICK_QTY, opt => opt.MapFrom(src => src.DustWasteStickQty))
                .ForMember(dest => dest.FLOOR_WASTE_STICK_QTY, opt => opt.MapFrom(src => src.FloorWasteStickQty))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.USE_FOR_LACK10, opt => opt.MapFrom(src => src.UseForLack10 == "no" ? false : true))
                .ForMember(dest => dest.STAMP_WASTE_QTY, opt => opt.MapFrom(src =>src.StampWasteQty));


            #endregion

            Mapper.CreateMap<CK1, CK1Dto>().IgnoreAllNonExisting();

            Mapper.CreateMap<BLOCK_STOCK, BLOCK_STOCKDto>().IgnoreAllNonExisting();

            #region Waste Role

            Mapper.CreateMap<WASTE_ROLE, WasteRoleDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.FIRST_NAME ))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.LAST_NAME))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.EMAIL))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.PHONE))
                .ForMember(dest => dest.WasteGroupDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GROUP_ROLE)))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.T001W == null ? string.Empty : src.T001W.NAME1))
                
                ;

            Mapper.CreateMap<WasteRoleDto, WASTE_ROLE>().IgnoreAllNonExisting();

            Mapper.CreateMap<WasteDto, WasteGetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.PlantWerks));

            Mapper.CreateMap<WASTE_ROLE, WasteRoleDetailsDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WASTE_ROLE_ID, opt => opt.MapFrom(src => src.WASTE_ROLE_ID))
                .ForMember(dest => dest.WasteGroup, opt => opt.MapFrom(src => src.GROUP_ROLE))
                .ForMember(dest => dest.WasteGroupDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GROUP_ROLE)))
                ;

            #endregion

            #region Waste Stock

            Mapper.CreateMap<WASTE_STOCK, WasteStockDto>().IgnoreAllNonExisting()
              //.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.FIRST_NAME))
              //.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.LAST_NAME))
              //.ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.EMAIL))
              //.ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.USER == null ? string.Empty : src.USER.PHONE))
              //.ForMember(dest => dest.WasteGroupDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GROUP_ROLE)))
              .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.T001W == null ? string.Empty : src.T001W.NAME1))
              .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.ZAIDM_EX_MATERIAL == null ? string.Empty : src.ZAIDM_EX_MATERIAL.BASE_UOM_ID))
              ;

            Mapper.CreateMap<WasteStockDto, WASTE_STOCK>().IgnoreAllNonExisting();

            #endregion

            #region Nlog

            Mapper.CreateMap<NlogLogs, NlogDto>().IgnoreAllNonExisting();
            
            #endregion

            #region Xml Log

            Mapper.CreateMap<XML_LOGS, XML_LOGSDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DetailList, opt => opt.MapFrom(src => Mapper.Map<List<XML_LOGS_DETAILSDto>>(src.XML_LOGS_DETAILS)))
                ;

            Mapper.CreateMap<XML_LOGS_DETAILS, XML_LOGS_DETAILSDto>().IgnoreAllNonExisting();

            #endregion

            #region Poa Delegation

            Mapper.CreateMap<POA_DELEGATION, POA_DELEGATIONDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<POA_DELEGATIONDto, POA_DELEGATION>().IgnoreAllNonExisting();

            #endregion

            #region Reversal

            Mapper.CreateMap<REVERSAL, ReversalDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ReversalId, opt => opt.MapFrom(src => src.REVERSAL_ID))
                .ForMember(dest => dest.ZaapShiftId, opt => opt.MapFrom(src => src.ZAAP_SHIFT_RPT_ID))
                .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.PRODUCTION_DATE))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.ReversalQty, opt => opt.MapFrom(src => src.REVERSAL_QTY))
                .ForMember(dest => dest.InventoryMovementId, opt => opt.MapFrom(src => src.INVENTORY_MOVEMENT_ID))
                ;

            Mapper.CreateMap<ReversalDto, REVERSAL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.REVERSAL_ID, opt => opt.MapFrom(src => src.ReversalId))
                .ForMember(dest => dest.ZAAP_SHIFT_RPT_ID, opt => opt.MapFrom(src => src.ZaapShiftId))
                .ForMember(dest => dest.PRODUCTION_DATE, opt => opt.MapFrom(src => src.ProductionDate))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.Werks))
                .ForMember(dest => dest.REVERSAL_QTY, opt => opt.MapFrom(src => src.ReversalQty))
                .ForMember(dest => dest.INVENTORY_MOVEMENT_ID, opt => opt.MapFrom(src => src.InventoryMovementId))
                ;

            #endregion


            #region Month Closing

            Mapper.CreateMap<MONTH_CLOSING, MonthClosingDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MonthClosingId, opt => opt.MapFrom(src => src.MONTH_CLOSING_ID))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.NAME1))
                .ForMember(dest => dest.ClosingDay, opt => opt.MapFrom(src => src.CLOSING_DATE.Value.Day))
                .ForMember(dest => dest.ClosingMonth, opt => opt.MapFrom(src => src.CLOSING_DATE.Value.ToString("MMMM")))
                .ForMember(dest => dest.ClosingYear, opt => opt.MapFrom(src => src.CLOSING_DATE.Value.Year))
                .ForMember(dest => dest.ClosingDate, opt => opt.MapFrom(src => src.CLOSING_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE == false || src.IS_ACTIVE == null ? "No" : "Yes"));

            Mapper.CreateMap<MonthClosingDto, MONTH_CLOSING>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MONTH_CLOSING_ID, opt => opt.MapFrom(src => src.MonthClosingId))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.CLOSING_DATE, opt => opt.MapFrom(src => src.ClosingDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.ResolveUsing<StringToBooleanResolver>().FromMember(src => src.IsActive));

            Mapper.CreateMap<MONTH_CLOSING_DOCUMENT, MonthClosingDocDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<MonthClosingDocDto, MONTH_CLOSING_DOCUMENT>().IgnoreAllNonExisting();

            #endregion

            #region Master Data Approval

            Mapper.CreateMap<MASTER_DATA_APPROVE_SETTING, MasterDataApprovalSettingDetail>().IgnoreAllNonExisting();
            Mapper.CreateMap<MasterDataApprovalSettingDetail, MASTER_DATA_APPROVE_SETTING>().IgnoreAllNonExisting();
            Mapper.CreateMap<TableDetail, MasterDataApprovalSettingDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COLUMN_NAME, opt => opt.MapFrom(src => src.PropertyName))
                .ForMember(dest => dest.ColumnDescription,
                    opt =>
                        opt.MapFrom(
                            src => src.Documentation.LongDescription != null ? src.Documentation.LongDescription : ""));
            Mapper.CreateMap<PAGE, MasterDataApprovalSettingDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PageId, opt => opt.MapFrom(src => src.PAGE_ID))
                .ForMember(dest => dest.PageDescription, opt => opt.MapFrom(src => src.MENU_NAME));

            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandXmlDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<BrandXmlDto, ZAIDM_EX_BRAND>().IgnoreAllNonExisting();
            #endregion

            #region Quota Monitoring
            Mapper.CreateMap<Pbck1Dto, QUOTA_MONITORING>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.PERIOD_FROM, opt => opt.MapFrom(src => src.PeriodFrom))
                .ForMember(dest => dest.PERIOD_TO, opt => opt.MapFrom(src => src.PeriodTo))
                .ForMember(dest => dest.SUPPLIER_NPPBKC_ID, opt => opt.MapFrom(src => src.SupplierNppbkcId))
                .ForMember(dest => dest.SUPPLIER_WERKS, opt => opt.MapFrom(src => src.SupplierPlantWerks))
                .ForMember(dest => dest.EX_GROUP_TYPE, opt => opt.UseValue(3));
            #endregion
        }
    }
}
