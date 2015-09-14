using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;
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
                .ForMember(dest => dest.PbckId, opt => opt.MapFrom(src => src.PBCK4_ID))
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

            Mapper.CreateMap<T001WDto, Pbck4PlantModel>().IgnoreAllNonExisting()
              .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode))
              .ForMember(dest => dest.PlantDesc, opt => opt.MapFrom(src => src.NAME1))
              .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
              .ForMember(dest => dest.NppbkcDescription, opt => opt.MapFrom(src => src.KppbcCity))
              
              ;

            Mapper.CreateMap<Pbck4FormViewModel, Pbck4Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK4_ID, opt => opt.MapFrom(src => src.Pbck4Id))
                .ForMember(dest => dest.PBCK4_NUMBER, opt => opt.MapFrom(src => src.Pbck4Number))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.PlantDesc))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.NPPBKC_DESCRIPTION, opt => opt.MapFrom(src => src.NppbkcDesc))
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DocumentStatus))
                ;

            Mapper.CreateMap<Pbck4Dto, Pbck4FormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck4Id, opt => opt.MapFrom(src => src.PBCK4_ID))
               .ForMember(dest => dest.Pbck4Number, opt => opt.MapFrom(src => src.PBCK4_NUMBER))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
               .ForMember(dest => dest.PlantDesc, opt => opt.MapFrom(src => src.PlantDescription))
               .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
               .ForMember(dest => dest.NppbkcDesc, opt => opt.MapFrom(src => src.NPPBKC_DESCRIPTION))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
               .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
               .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.DocumentStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
               ;

            Mapper.CreateMap<Pbck4UploadViewModel, Pbck4ItemsInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck4ItemsOutput, Pbck4UploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck4ItemDto, Pbck4UploadViewModel>().IgnoreAllNonExisting();
        }
    }
}