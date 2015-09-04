using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;
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

        }
    }
}