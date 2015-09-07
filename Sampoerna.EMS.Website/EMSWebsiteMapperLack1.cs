using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.LACK1;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeLACK1()
        {
            Mapper.CreateMap<Lack1GenerateInputModel, Lack1GenerateDataParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1Level, opt => opt.MapFrom(src => (Enums.Lack1Level)src.Lack1Level));
            Mapper.CreateMap<Lack1GeneratedDto, Lack1GeneratedItemModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1CreateViewModel, Lack1CreateParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Bukrs))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PeriodYears))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReceivedPlantId, opt => opt.MapFrom(src => src.LevelPlantId))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SupplierPlantId))
                .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.ExGoodsTypeId))
                .ForMember(dest => dest.ExcisableGoodsTypeDesc, opt => opt.MapFrom(src => src.ExGoodsTypeDesc))
                .ForMember(dest => dest.WasteAmount, opt => opt.MapFrom(src => src.WasteQty))
                .ForMember(dest => dest.WasteAmountUom, opt => opt.MapFrom(src => src.WasteUom))
                .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.ReturnQty))
                .ForMember(dest => dest.ReturnAmountUom, opt => opt.MapFrom(src => src.ReturnUom))
                .ForMember(dest => dest.Lack1Level, opt => opt.MapFrom(src => src.Lack1Level))
                .ForMember(dest => dest.Noted, opt => opt.MapFrom(src => src.Noted))
                ;

            Mapper.CreateMap<Lack1DocumentDto, Lack1DocumentItemModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1IncomeDetailDto, Lack1IncomeDetailItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1IncomeDetailId, opt => opt.MapFrom(src => src.LACK1_INCOME_ID))
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE))
                .ForMember(dest => dest.StringRegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy") : string.Empty))
                ;
            Mapper.CreateMap<Lack1Pbck1MappingDto, Lack1Pbck1MappingItemModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1PlantDto, Lack1PlantItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1PlantId, opt => opt.MapFrom(src => src.LACK1_PLANT_ID))
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.Name1, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.PLANT_ADDRESS))
                ;

            Mapper.CreateMap<Lack1ProductionDetailDto, Lack1ProductionDetailItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1ProductionDetailId, opt => opt.MapFrom(src => src.LACK1_PRODUCTION_ID))
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.ProdCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.UOM_ID))
                .ForMember(dest => dest.UomDesc, opt => opt.MapFrom(src => src.UOM_DESC))
                ;

            Mapper.CreateMap<Lack1DetailsDto, Lack1ItemViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.GovStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GovStatus)))
                .ForMember(dest => dest.EndingBalance, opt => opt.MapFrom(src => (src.BeginingBalance - src.Usage + src.TotalIncome)))
                .ForMember(dest => dest.TotalUsage, opt => opt.MapFrom(src => src.Usage))
                .ForMember(dest => dest.Lack1Document, opt => opt.MapFrom(src => Mapper.Map<List<Lack1DocumentItemModel>>(src.Lack1Document)))
                .ForMember(dest => dest.IncomeList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailItemModel>>(src.Lack1IncomeDetail)))
                .ForMember(dest => dest.Lack1Pbck1Mapping, opt => opt.MapFrom(src => Mapper.Map<List<Lack1Pbck1MappingItemModel>>(src.Lack1Pbck1Mapping)))
                .ForMember(dest => dest.Lack1Plant, opt => opt.MapFrom(src => Mapper.Map<List<Lack1PlantItemModel>>(src.Lack1Plant)))
                .ForMember(dest => dest.ProductionList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1ProductionDetailItemModel>>(src.Lack1ProductionDetail)))
                ;
        }
    }
}