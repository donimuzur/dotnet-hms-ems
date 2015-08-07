﻿using System.Collections.Generic;
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
                ;

            Mapper.CreateMap<T001W, T001WDto>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Npwp, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.NPWP))
                  .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.BUTXT))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.SPRAS))
                 .ForMember(dest => dest.KppbcCity, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.CITY))
                 .ForMember(dest => dest.KppbcNo, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC == null ? string.Empty : src.ZAIDM_EX_NPPBKC.KPPBC_ID))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC == null || src.ZAIDM_EX_NPPBKC.T001 == null ? string.Empty : src.ZAIDM_EX_NPPBKC.T001.BUKRS));
            
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

            Mapper.CreateMap<POA_MAP, POA_MAPDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<POA, POADto>().IgnoreAllNonExisting();

            Mapper.CreateMap<T001W, T001WDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<PRINT_HISTORY, PrintHistoryDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<PrintHistoryDto, PRINT_HISTORY>().IgnoreAllNonExisting();

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, ZAIDM_EX_NPPBKCDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<ZAIDM_EX_KPPBC, ZAIDM_EX_KPPBCDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<LFA1, LFA1Dto>().IgnoreAllNonExisting();
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
        }
    }
}
