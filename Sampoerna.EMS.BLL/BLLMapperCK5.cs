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
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void InitializeCK5()
        {
            #region CK5

            Mapper.CreateMap<CK5MaterialInput, CK5MaterialOutput>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5, CK5Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.USER1.USERNAME))
                .ForMember(dest => dest.PackageUomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PbckNumber, opt => opt.MapFrom(src => src.PBCK1.NUMBER))
                .ForMember(dest => dest.PbckDecreeDate, opt => opt.MapFrom(src => src.PBCK1.DECREE_DATE))
                .ForMember(dest => dest.IsCk5Export,opt => opt.MapFrom(src => src.CK5_TYPE == Enums.CK5Type.Export))
                .ForMember(dest => dest.IsCk5Manual, opt => opt.MapFrom(src => src.CK5_TYPE == Enums.CK5Type.Manual))
                .ForMember(dest => dest.IsWaitingGovApproval, opt => opt.MapFrom(src => src.STATUS_ID == Enums.DocumentStatus.WaitingGovApproval));

            Mapper.CreateMap<CK5Dto, CK5>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5MaterialDto, CK5_MATERIAL>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5_MATERIAL, CK5MaterialDto>().IgnoreAllNonExisting();

            #endregion
        }
    }
}
