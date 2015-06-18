using System.Drawing;
using AutoMapper;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.GOODSTYPE;
using Sampoerna.EMS.Website.Models.NPPBKC;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.POA;
using Sampoerna.EMS.Website.Models.GOODSTYPE;

namespace Sampoerna.EMS.Website
{
    public class EMSWebsiteMapper
    {
        public static void Initialize()
        {
            //AutoMapper
            Mapper.CreateMap<USER, UserViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.USER2))
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.USER1));

            Mapper.CreateMap<UserTree, UserViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE.HasValue && src.IS_ACTIVE.Value))
                ;

            #region Company
            Mapper.CreateMap<T1001, CompanyDetail>().IgnoreAllNonExisting()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
            .ForMember(dest => dest.DocumentBukrs, opt => opt.MapFrom(src => src.BUKRS))
            .ForMember(dest => dest.DocumentBukrstxt, opt => opt.MapFrom(src => src.BUKRSTXT))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));
#endregion 

            Mapper.CreateMap<PBCK1, PBCK1Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.APPROVED_USER, opt => opt.MapFrom(src => src.USER))
                .ForMember(dest => dest.LACK1_FROM_MONTH_ID, opt => opt.MapFrom(src => src.LACK1_FROM_MONTH))
                .ForMember(dest => dest.LACK1_TO_MONTH_ID, opt => opt.MapFrom(src => src.LACK1_TO_MONTH))
                .ForMember(dest => dest.LACK1_FROM_MONTH, opt => opt.MapFrom(src => src.MONTH))
                .ForMember(dest => dest.LACK1_TO_MONTH, opt => opt.MapFrom(src => src.MONTH1))
                .ForMember(dest => dest.GOODTYPE, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP))
                .ForMember(dest => dest.STATUS_GOV_ID, opt => opt.MapFrom(src => src.STATUS_GOV))
                .ForMember(dest => dest.STATUS_GOV, opt => opt.MapFrom(src => src.STATUS_GOV1))
                .ForMember(dest => dest.NPPBKC, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC))
                .ForMember(dest => dest.STATUS_NAME, opt => opt.MapFrom(src => src.STATUS1 != null ? src.STATUS1.STATUS_NAME : string.Empty))
                ;

            Mapper.CreateMap<PBCK1FilterViewModel, PBCK1Input>().IgnoreAllNonExisting();

            //Virtual Mapping Plant
            Mapper.CreateMap<VIRTUAL_PLANT_MAP, VirtualMappingPlantDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.VirtualPlantMapId, opt => opt.MapFrom(src => src.VIRTUAL_PLANT_MAP_ID))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
                .ForMember(dest => dest.ImportPlantId, opt => opt.MapFrom(src => src.IMPORT_PLANT_ID))
                .ForMember(dest => dest.ExportPlantId, opt => opt.MapFrom(src => src.EXPORT_PLANT_ID));

            Mapper.CreateMap<SaveVirtualMappingPlantOutput, VirtualMappingPlantDetail>().IgnoreAllNonExisting()
               .ForMember(dest => dest.VirtualPlantMapId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company))
               .ForMember(dest => dest.ImportPlantId, opt => opt.MapFrom(src => src.ImportVitualPlant))
               .ForMember(dest => dest.ExportPlantId, opt => opt.MapFrom(src => src.ExportVirtualPlant));

            Mapper.CreateMap<ZAIDM_EX_POA, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.POA_CODE));

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.NPPBKC_NO))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.NPPBKC_NO));

            Mapper.CreateMap<USER, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => (src.USER_ID + "-" + src.USERNAME)));

            #region POA
            Mapper.CreateMap<ZAIDM_EX_POA, POAViewModelDetails>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PoaIdCard, opt => opt.MapFrom(src => src.POA_ID_CARD))
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.USER_ID))
               .ForMember(dest => dest.PoaPrintedName, opt => opt.MapFrom(src => src.POA_PRINTED_NAME))
               .ForMember(dest => dest.PoaAddress, opt => opt.MapFrom(src => src.POA_ADDRESS))
               .ForMember(dest => dest.PoaPhone, opt => opt.MapFrom(src => src.POA_PHONE))
               .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TITLE));
            #endregion

            #region NPPBKC

            Mapper.CreateMap<ZaidmExNPPBKCOutput, DetailNppbck>().IgnoreAllNonExisting();

            #endregion

            #region GoodsTypeGroup

            Mapper.CreateMap<ZAIDM_EX_GOODTYP, DetailsGoodsTypGroup>().IgnoreAllNonExisting()
                .ForMember(dest => dest.GoodsTypeId, opt => opt.MapFrom(src => src.GOODTYPE_ID))
                .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.ExtTypDescending, opt => opt.MapFrom(src => src.EXT_TYP_DESC))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE));

            #endregion

            #region Plant

            Mapper.CreateMap<T1001W, DetailPlantT1001W>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.IsMainPlant, opt => opt.MapFrom(src => src.IS_MAIN_PLANT))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY));

            #endregion

        }
    }
}