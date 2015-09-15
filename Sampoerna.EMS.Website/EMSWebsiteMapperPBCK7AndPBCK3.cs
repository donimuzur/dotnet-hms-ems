using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Models.PBCK7AndPBCK3;


namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializePbck7AndPbck3()
        {
            #region PBCK7

            Mapper.CreateMap<Pbck7AndPbck3Dto, DataListIndexPbck7>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.Pbck7Date))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Pbck7Status))
                .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.Pbck7Number));

            Mapper.CreateMap<Pbck7IndexViewModel, Pbck7AndPbck3Input>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.Pbck7Date, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.Pbck7AndPvck3Type, opt => opt.MapFrom(src => src.Pbck7Type))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            #endregion

            #region PBCK3

            Mapper.CreateMap<Pbck7AndPbck3Dto, DataListIndexPbck3>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.Pbck7Date))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Pbck7Status))
                .ForMember(dest => dest.Pbck3Number, opt => opt.MapFrom(src => src.Pbck3Number));

            Mapper.CreateMap<Pbck3IndexViewModel, Pbck7AndPbck3Input>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.Pbck3Date, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.Pbck7AndPvck3Type, opt => opt.MapFrom(src => src.Pbck3Type))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            #endregion

            #region PBCK7adPBCK3 Create

            Mapper.CreateMap<Pbck7AndPbck3Dto, Pbck7Pbck3CreateViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck7Pbck3CreateViewModel, Pbck7AndPbck3Dto>().IgnoreAllNonExisting();




            #endregion
        }
    }
}