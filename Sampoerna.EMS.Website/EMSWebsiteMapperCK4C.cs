using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Models.CK4C;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapperCK4C
    {
        public static void InitializeCk4C()
        {
            Mapper.CreateMap<Ck4CDto, DataIndecCk4C>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CompnayId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompnayName))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PlantName));

            Mapper.CreateMap<Ck4CIndexViewModel, Ck4CGetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.DateProduction, opt => opt.MapFrom(src => src.ProductionDate));


        }
    }
}