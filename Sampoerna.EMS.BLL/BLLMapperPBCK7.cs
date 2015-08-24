using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void InitializePbck7And3()
        {
            #region PBCK7

            Mapper.CreateMap<PBCK3_PBCK7, Pbck7AndPbck3Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck3Pbck7Id, opt => opt.MapFrom(src => src.PBCK3_PBCK7_ID))
                .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.PBCK7_NUMBER))
                .ForMember(dest => dest.Pbck3Number, opt => opt.MapFrom(src => src.PBCK3_NUMBER))
                .ForMember(dest => dest.Pbck7Status, opt => opt.MapFrom(src => src.PBCK7_STATUS))
                .ForMember(dest => dest.Pbck3Status, opt => opt.MapFrom(src => src.PBCK3_STATUS))
                .ForMember(dest => dest.Pbck7Date, opt => opt.MapFrom(src => src.PBCK7_DATE))
                .ForMember(dest => dest.Pbck3Date, opt => opt.MapFrom(src => src.PBCK3_DATE))
                .ForMember(dest => dest.DocumnetType, opt => opt.MapFrom(src => src.DOC_TYPE))
                .ForMember(dest => dest.Pbck7Date, opt => opt.MapFrom(src => src.PBCK7_DATE))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBCK_ID))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.PlantCity, opt => opt.MapFrom(src => src.PLANT_CITY))
                .ForMember(dest => dest.ExecDateFrom, opt => opt.MapFrom(src => src.EXEC_DATE_FROM))
                .ForMember(dest => dest.ExecDateTo, opt => opt.MapFrom(src => src.EXEC_DATE_TO))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE));

            Mapper.CreateMap<BACK1, Pbck7AndPbck3Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck3Pbck7Id, opt => opt.MapFrom(src => src.PBCK3_PBCK7_ID));


            #endregion

        }
    }
}
