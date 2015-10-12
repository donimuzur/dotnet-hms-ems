using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Models.LACK2;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeLACK2()
        {
            Mapper.CreateMap<Lack2SummaryReportDto, Lack2SummaryReportsItem>().IgnoreAllNonExisting()
                ;

            Mapper.CreateMap<Lack2SearchSummaryReportsViewModel, Lack2GetSummaryReportByParamInput>().IgnoreAllNonExisting()
                ;

            Mapper.CreateMap<Lack2DetailReportDto, Lack2DetailReportsItem>().IgnoreAllNonExisting()
                ;

            Mapper.CreateMap<Lack2SearchDetailReportsViewModel, Lack2GetDetailReportByParamInput>().IgnoreAllNonExisting()
                ;
            Mapper.CreateMap<Lack2GenerateInputModel, Lack2GenerateDataParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PeriodYear))
                .ForMember(dest => dest.SourcePlantId, opt => opt.MapFrom(src => src.SourcePlantId))
                .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.ExcisableGoodsType))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                ;

            Mapper.CreateMap<LACK2CreateViewModel, Lack2CreateParamInput>().IgnoreAllNonExisting();
        }
    }
}