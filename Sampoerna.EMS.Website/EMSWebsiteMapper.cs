using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Website.Models;

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


            //Company
            Mapper.CreateMap<T1001, CompanyViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
            .ForMember(dest => dest.DocumentBukrs, opt => opt.MapFrom(src => src.BUKRS))
            .ForMember(dest => dest.DocumentBukrstxt, opt => opt.MapFrom(src => src.BUKRSTXT))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE)); 

            //Company
            Mapper.CreateMap<CompanyViewModel, T1001>().IgnoreAllNonExisting()
            .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
            .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.DocumentBukrs))
            .ForMember(dest => dest.BUKRSTXT, opt => opt.MapFrom(src => src.DocumentBukrstxt))
            .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate)); 

        }
    }
}