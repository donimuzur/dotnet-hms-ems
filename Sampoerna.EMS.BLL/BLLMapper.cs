using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

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
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.BUTXT))
                .ForMember(dest => dest.COMPANY_ADDRESS, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.SPRAS))
                .ForMember(dest => dest.KPPBC_CITY, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.CITY))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ORT01, opt => opt.MapFrom(src => src.ORT01))
               .ForMember(dest => dest.KPPBC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC == null ? string.Empty : src.ZAIDM_EX_NPPBKC.KPPBC_ID))
               .ForMember(dest => dest.DROPDOWNTEXTFIELD, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1))
               .ForMember(dest => dest.SUPPLIER_COMPANY, opt => opt.MapFrom(src => src.T001K.T001.BUTXT))
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
                .ForMember(dest => dest.NAME1, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID));
           

            Mapper.CreateMap<PRINT_HISTORY, PrintHistoryDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<PrintHistoryDto, PRINT_HISTORY>().IgnoreAllNonExisting();

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, ZAIDM_EX_NPPBKCDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DROPDOWNTEXT, opt => opt.MapFrom(src => src.NPPBKC_ID))
                ;
            Mapper.CreateMap<ZAIDM_EX_KPPBC, ZAIDM_EX_KPPBCDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.KPPBC_ID, opt => opt.MapFrom(src => src.KPPBC_ID))
                .ForMember(dest => dest.KPPBC_TYPE, opt => opt.MapFrom(src => src.KPPBC_TYPE))
                .ForMember(dest => dest.MENGETAHUI, opt => opt.MapFrom(src => src.MENGETAHUI))
                .ForMember(dest => dest.MENGETAHUI_DETAIL, opt => opt.MapFrom(src => src.MENGETAHUI_DETAIL))
                .ForMember(dest => dest.CK1_KEP_HEADER, opt => opt.MapFrom(src => src.CK1_KEP_HEADER))
                .ForMember(dest => dest.CK1_KEP_FOOTER, opt => opt.MapFrom(src => src.CK1_KEP_FOOTER));
  

            Mapper.CreateMap<LFA1, LFA1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_DELETED_STRING,opt => opt.MapFrom(src => src.IS_DELETED.HasValue ? src.IS_DELETED.Value ? "Yes" : "No" : "No"))
                .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IS_DELETED.HasValue ? src.IS_DELETED.Value : false));
            Mapper.CreateMap<T001, T001Dto>().IgnoreAllNonExisting();
            
            #region LACK2

            Mapper.CreateMap<LACK2, Lack2Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack2Id, opt => opt.MapFrom(src => src.LACK2_ID))
                .ForMember(dest => dest.Lack2Number, opt => opt.MapFrom(src => src.LACK2_NUMBER))
                .ForMember(dest => dest.Burks, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.BUTXT))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.LEVEL_PLANT_ID))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.LEVEL_PLANT_NAME))
                .ForMember(dest => dest.LevelPlantCity, opt => opt.MapFrom(src => src.LEVEL_PLANT_CITY))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.ExGoodTyp, opt => opt.MapFrom(src => src.EX_GOOD_TYP))
                .ForMember(dest => dest.ExTypDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.ApprovedByManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.ApprovedDateManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER_DATE))
                .ForMember(dest => dest.RejectedBy, opt => opt.MapFrom(src => src.REJECTED_BY))
                .ForMember(dest => dest.RejectedDate, opt => opt.MapFrom(src => src.REJECTED_DATE))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.LACK2_DOCUMENT));
                



            Mapper.CreateMap<Lack2Dto, LACK2>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK2_ID, opt => opt.MapFrom(src => src.Lack2Id))
                .ForMember(dest => dest.LACK2_NUMBER, opt => opt.MapFrom(src => src.Lack2Number))
                .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.Burks))
                .ForMember(dest => dest.BUTXT, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.PERIOD_MONTH, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PERIOD_YEAR, opt => opt.MapFrom(src => src.PeriodYear))
                .ForMember(dest => dest.LEVEL_PLANT_ID, opt => opt.MapFrom(src => src.LevelPlantId))
                .ForMember(dest => dest.LEVEL_PLANT_NAME, opt => opt.MapFrom(src => src.LevelPlantName))
                .ForMember(dest => dest.LEVEL_PLANT_CITY, opt => opt.MapFrom(src => src.LevelPlantCity))
                .ForMember(dest => dest.SUBMISSION_DATE, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.EX_GOOD_TYP, opt => opt.MapFrom(src => src.ExGoodTyp))
                .ForMember(dest => dest.EX_TYP_DESC, opt => opt.MapFrom(src => src.ExTypDesc))
                .ForMember(dest => dest.GOV_STATUS, opt => opt.MapFrom(src => src.GovStatus))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.DECREE_DATE, opt => opt.MapFrom(src => src.DecreeDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.APPROVED_BY, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.APPROVED_DATE, opt => opt.MapFrom(src => src.ApprovedDate))
                .ForMember(dest => dest.REJECTED_BY, opt => opt.MapFrom(src => src.RejectedBy))
                .ForMember(dest => dest.REJECTED_DATE, opt => opt.MapFrom(src => src.RejectedDate))
                .ForMember(dest => dest.APPROVED_BY_MANAGER, opt => opt.MapFrom(src => src.ApprovedByManager))
                .ForMember(dest => dest.APPROVED_BY_MANAGER_DATE, opt => opt.MapFrom(src => src.ApprovedDateManager))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.LACK2_ITEM, opt => opt.MapFrom(src => src.Items));



            Mapper.CreateMap<MONTH, Lack2Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.MONTH_ID))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH_NAME_IND))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH_NAME_ENG));

            #endregion

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
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.USER.FIRST_NAME + " " + src.USER.LAST_NAME));
            Mapper.CreateMap<UserPlantMapDto, USER_PLANT_MAP>().IgnoreAllNonExisting()
                .ForMember(dest => dest.USER_PLANT_MAP_ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.USER_ID, opt => opt.MapFrom(src => src.UserId));
                
	    
	        #region ExGoodTyp

            Mapper.CreateMap<EX_GROUP_TYPE, ExGoodTyp>().IgnoreAllNonExisting();
            

            #endregion

            #region Material Dto
            Mapper.CreateMap<MaterialDto, ZAIDM_EX_MATERIAL>().IgnoreAllNonExisting();
            Mapper.CreateMap<ZAIDM_EX_MATERIAL,MaterialDto>().IgnoreAllNonExisting();
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

            Mapper.CreateMap<Lack2ItemDto, LACK2_ITEM>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK2_ITEM_ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LACK2_ITEM_ID, opt => opt.MapFrom(src => src.Lack2Id))
                .ForMember(dest => dest.CK5_ID, opt => opt.MapFrom(src => src.Ck5Id));

            Mapper.CreateMap<LACK2_ITEM, Lack2ItemDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LACK2_ITEM_ID))
                .ForMember(dest => dest.Lack2Id, opt => opt.MapFrom(src => src.LACK2_ID))
               
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
               
                .ForMember(dest => dest.Ck5Number, opt => opt.MapFrom(src => src.CK5.SUBMISSION_NUMBER))
                .ForMember(dest => dest.Ck5GIDate, opt => opt.MapFrom(src => src.CK5.GI_DATE == null ? null : src.CK5.GI_DATE.Value.ToString("dd MMM yyyy")))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CK5.DEST_PLANT_COMPANY_NAME))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.CK5.DEST_PLANT_ADDRESS))
               .ForMember(dest => dest.CompanyNppbkc, opt => opt.MapFrom(src => src.CK5.DEST_PLANT_NPPBKC_ID))
                .ForMember(dest => dest.Ck5ItemQty, opt => opt.MapFrom(src => src.CK5.GRAND_TOTAL_EX));
            
            Mapper.CreateMap<PBCK1, ZAIDM_EX_NPPBKCCompositeDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<PBCK1, T001WCompositeDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.DROPDOWNTEXTFIELD,
                    opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS + "-" + src.SUPPLIER_PLANT));
            Mapper.CreateMap<PBCK1, ZAIDM_EX_GOODTYPCompositeDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EXT_TYP_DESC, opt => opt.MapFrom(src => src.EXC_TYP_DESC))
                ;
        }
    }
}
