using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void InitializePbck4()
        {

            Mapper.CreateMap<PBCK4, Pbck4Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.REPORTED_ON))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                ;

            Mapper.CreateMap<Pbck4Dto, PBCK4>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
               .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantDescription))
               .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
               .ForMember(dest => dest.REPORTED_ON, opt => opt.MapFrom(src => src.ReportedOn))
               //.ForMember(dest => dest.APPROVED_BY_POA, opt => opt.MapFrom(src => src.poa))
               .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.Status))
               ;

            Mapper.CreateMap<Pbck4ItemsInput, Pbck4ItemsOutput>().IgnoreAllNonExisting()
                ;

            Mapper.CreateMap<PBCK4_ITEM, Pbck4ItemDto>().IgnoreAllNonExisting()
               ;
        }
    }
}
