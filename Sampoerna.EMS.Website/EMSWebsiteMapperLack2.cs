using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Utils;
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

            Mapper.CreateMap<Lack2DetailsDto, Lack2EditViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Burks))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.PeriodMonthNameEn, opt => opt.MapFrom(src => src.PerionNameEng))
                .ForMember(dest => dest.PeriodMonthNameId, opt => opt.MapFrom(src => src.PeriodNameInd))
                .ForMember(dest => dest.SourcePlantId, opt => opt.MapFrom(src => src.LevelPlantId))
                .ForMember(dest => dest.SourcePlantName, opt => opt.MapFrom(src => src.LevelPlantName))
                .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.ExGoodTyp))
                .ForMember(dest => dest.ExcisableGoodsTypeDesc, opt => opt.MapFrom(src => src.ExTypDesc))
                ;

            Mapper.CreateMap<Lack2EditViewModel, Lack2SaveEditInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack2DetailsDto, Lack2DetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Burks))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.PeriodMonthNameEn, opt => opt.MapFrom(src => src.PerionNameEng))
                .ForMember(dest => dest.PeriodMonthNameId, opt => opt.MapFrom(src => src.PeriodNameInd))
                .ForMember(dest => dest.SourcePlantId, opt => opt.MapFrom(src => src.LevelPlantId))
                .ForMember(dest => dest.SourcePlantName, opt => opt.MapFrom(src => src.LevelPlantName))
                .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.ExGoodTyp))
                .ForMember(dest => dest.ExcisableGoodsTypeDesc, opt => opt.MapFrom(src => src.ExTypDesc))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusName))
                .ForMember(dest => dest.GovStatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GovStatus)))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DecreeDate))
                ;

            #region ---------------- Dashboard ----------------

            Mapper.CreateMap<Lack2DashboardSearchViewModel, Lack2GetDashboardDataByParamInput>().IgnoreAllNonExisting();

            #endregion

        }
    }
}