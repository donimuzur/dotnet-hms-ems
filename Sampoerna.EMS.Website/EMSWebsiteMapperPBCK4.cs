using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.PBCK4;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializePBCK4()
        {
            Mapper.CreateMap<Pbck4Dto, Pbck4Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.PlantDescription))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn.HasValue ? src.ReportedOn.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                ;

            Mapper.CreateMap<Pbck4SearchViewModel, Pbck4GetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
                ;
        }
    }
}