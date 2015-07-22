using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.BLL
{
    public class BLLMapper
    {
        public static void Initialize()
        {

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
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ORT01, opt => opt.MapFrom(src => src.ORT01))
               .ForMember(dest => dest.KPPBC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC == null ? string.Empty : src.ZAIDM_EX_NPPBKC.KPPBC_ID))
                ;

            #region CK5

            Mapper.CreateMap<CK5MaterialInput, CK5MaterialOutput>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5, CK5Dto>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.KppbcCityName,opt => opt.MapFrom(src => (src.ZAIDM_EX_KPPBC.ZAIDM_EX_NPPBKC).FirstOrDefault().CITY))
                //todo nppkbc atau kppbc
                .ForMember(dest => dest.GoodTypeDesc, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.ExSettlementName,
                    opt => opt.MapFrom(src => src.EX_SETTLEMENT.EX_SETTLEMENT_NAME))
                .ForMember(dest => dest.ExStatusName, opt => opt.MapFrom(src => src.EX_STATUS.EX_STATUS_NAME))
                .ForMember(dest => dest.RequestTypeName, opt => opt.MapFrom(src => src.REQUEST_TYPE.REQUEST_TYPE_NAME))
                .ForMember(dest => dest.SourcePlantName, opt => opt.MapFrom(src => src.T1001W.NAME1))
                .ForMember(dest => dest.SourcePlantWerks, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.DestPlantName, opt => opt.MapFrom(src => src.T1001W1.NAME1))
                .ForMember(dest => dest.DestPlantWerks, opt => opt.MapFrom(src => src.T1001W1.WERKS))
                .ForMember(dest => dest.PbckNumber, opt => opt.MapFrom(src => src.PBCK1.NUMBER))
                .ForMember(dest => dest.PbckDecreeDate, opt => opt.MapFrom(src => src.PBCK1.DECREE_DATE))
                .ForMember(dest => dest.CarriageMethodName,opt => opt.MapFrom(src => src.CARRIAGE_METHOD.CARRIAGE_METHOD_NAME))
                .ForMember(dest => dest.PackageUomName, opt => opt.MapFrom(src => src.UOM.UOM_NAME))
                .ForMember(dest => dest.KppbcCityName, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.CITY))
                .ForMember(dest => dest.CeOfficeCode,opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC.KPPBC_NUMBER))

                .ForMember(dest => dest.SourceNpwp, opt => opt.MapFrom(src => src.T1001W.ZAIDM_EX_NPPBKC.T1001.NPWP))
                .ForMember(dest => dest.SourceNppbkcId, opt => opt.MapFrom(src => src.T1001W.ZAIDM_EX_NPPBKC.NPPBKC_ID))
                    .ForMember(dest => dest.SourceCompanyName, opt => opt.MapFrom(src => src.T1001W.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT))
                    .ForMember(dest => dest.SourceAddress, opt => opt.MapFrom(src => src.T1001W.ADDRESS))
                    .ForMember(dest => dest.SourceKppbcName, opt => opt.MapFrom(src => src.T1001W.NAME1))

                .ForMember(dest => dest.DestNpwp, opt => opt.MapFrom(src => src.T1001W1.ZAIDM_EX_NPPBKC.T1001.NPWP))
                .ForMember(dest => dest.DestNppbkcId, opt => opt.MapFrom(src => src.T1001W1.ZAIDM_EX_NPPBKC.NPPBKC_ID))
                .ForMember(dest => dest.DestCompanyName, opt => opt.MapFrom(src => src.T1001W1.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT))
                .ForMember(dest => dest.DestAddress, opt => opt.MapFrom(src => src.T1001W1.ZAIDM_EX_NPPBKC.T1001.NPWP))
                    .ForMember(dest => dest.DestKppbcName, opt => opt.MapFrom(src => src.T1001W1.NAME1));

            Mapper.CreateMap<CK5Dto, CK5>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5MaterialDto, CK5_MATERIAL>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5_MATERIAL, CK5MaterialDto>().IgnoreAllNonExisting();

            #endregion

            #region Workflow History

            Mapper.CreateMap<WORKFLOW_HISTORY, WorkflowHistoryDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<WorkflowHistoryDto, WORKFLOW_HISTORY>().IgnoreAllNonExisting();

            #endregion
        }
    }
}
