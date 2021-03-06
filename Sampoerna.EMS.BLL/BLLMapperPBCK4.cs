﻿using System;
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
using Sampoerna.EMS.Utils;

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
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.IsWaitingGovApproval, opt => opt.MapFrom(src => src.STATUS == Enums.DocumentStatus.WaitingGovApproval))
                .ForMember(dest => dest.Pbck4DocumentDtos, opt => opt.MapFrom(src => Mapper.Map<List<PBCK4_DOCUMENTDto>>(src.PBCK4_DOCUMENT)))
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
                .ForMember(dest => dest.CK1_NUMBER, opt => opt.MapFrom(src => src.CK1.CK1_NUMBER))
                .ForMember(dest => dest.CK1_DATE, opt => opt.MapFrom(src => src.CK1.CK1_DATE))
               ;

            Mapper.CreateMap<Pbck4ItemDto, PBCK4_ITEM>().IgnoreAllNonExisting();

            Mapper.CreateMap<PBCK4_DOCUMENTDto, PBCK4_DOCUMENT>().IgnoreAllNonExisting();

            Mapper.CreateMap<PBCK4_DOCUMENT, PBCK4_DOCUMENTDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<ZAIDM_EX_BRAND, GetListBrandByPlantOutput>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                  .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                   .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                ;

            Mapper.CreateMap<CK1, GetListCk1ByNppbkcOutput>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Ck1Id, opt => opt.MapFrom(src => src.CK1_ID.ToString()))
                 .ForMember(dest => dest.Ck1No, opt => opt.MapFrom(src => src.CK1_NUMBER))
                 .ForMember(dest => dest.Ck1Date, opt => opt.MapFrom(src => src.CK1_DATE.ToString("dd MMM yyyy")))
                ;

            Mapper.CreateMap<ZAIDM_EX_BRAND, GetBrandItemsOutput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.SeriesCode, opt => opt.MapFrom(src => src.SERIES_CODE))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP != null ? src.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS : string.Empty ))
                .ForMember(dest => dest.BrandContent, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.BRAND_CONTENT).ToString()))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE_IDR.HasValue ? src.HJE_IDR.Value.ToString("f2") : "0"))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF.HasValue ? src.TARIFF.Value.ToString("f2") : "0"))
                .ForMember(dest => dest.Colour, opt => opt.MapFrom(src => src.COLOUR))
                ;
        }
    }
}
