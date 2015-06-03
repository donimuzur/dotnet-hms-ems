using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website
{
    public class SampoernaEMSMapper
    {
        public static void Initialize()
        {
            //AutoMapper
            //company 
            Mapper.CreateMap<T1001, CompanyViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
                 .ForMember(dest => dest.DocumentBukrs, opt => opt.MapFrom(src => src.BUKRSTXT))
                 .ForMember(dest => dest.DocumentBukrstxt, opt => opt.MapFrom(src => src.BUKRSTXT))
                 .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));
            //company 
            Mapper.CreateMap<CompanyViewModel, T1001>().IgnoreAllNonExisting()
             .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                 .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.DocumentBukrs))
                 .ForMember(dest => dest.BUKRSTXT, opt => opt.MapFrom(src => src.DocumentBukrstxt))
                 .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate));

        }
    }
}