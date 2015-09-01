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
        }
    }
}