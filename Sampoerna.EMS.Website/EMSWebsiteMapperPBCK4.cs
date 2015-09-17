using System;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.PBCK4;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializePBCK4()
        {
            Mapper.CreateMap<Pbck4Dto, Pbck4Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PbckId, opt => opt.MapFrom(src => src.PBCK4_ID))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.PlantDescription))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn.HasValue ? src.ReportedOn.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.APPROVED_BY_POA) ? string.Empty: src.APPROVED_BY_POA))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                ;

            Mapper.CreateMap<Pbck4SearchViewModel, Pbck4GetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
                ;

            Mapper.CreateMap<T001WDto, Pbck4PlantModel>().IgnoreAllNonExisting()
              .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode))
              .ForMember(dest => dest.PlantDesc, opt => opt.MapFrom(src => src.NAME1))
              .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
              .ForMember(dest => dest.NppbkcDescription, opt => opt.MapFrom(src => src.KppbcCity))
              
              ;

            Mapper.CreateMap<Pbck4FormViewModel, Pbck4Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK4_ID, opt => opt.MapFrom(src => src.Pbck4Id))
                .ForMember(dest => dest.PBCK4_NUMBER, opt => opt.MapFrom(src => src.Pbck4Number))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.PlantDesc))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.NPPBKC_DESCRIPTION, opt => opt.MapFrom(src => src.NppbkcDesc))
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DocumentStatus))
                .ForMember(dest => dest.POA_PRINTED_NAME, opt => opt.MapFrom(src => src.Poa))
                ;

            Mapper.CreateMap<Pbck4Dto, Pbck4FormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck4Id, opt => opt.MapFrom(src => src.PBCK4_ID))
               .ForMember(dest => dest.Pbck4Number, opt => opt.MapFrom(src => src.PBCK4_NUMBER))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
               .ForMember(dest => dest.PlantDesc, opt => opt.MapFrom(src => src.PlantDescription))
               .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
               .ForMember(dest => dest.NppbkcDesc, opt => opt.MapFrom(src => src.NPPBKC_DESCRIPTION))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
               .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
               .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.DocumentStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
               .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.POA_PRINTED_NAME))
               ;

            Mapper.CreateMap<Pbck4UploadViewModel, Pbck4ItemsInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck4ItemsOutput, Pbck4UploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck4UploadViewModel, Pbck4ItemDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CK1_ID, opt => opt.MapFrom(src => src.CK1_ID))
                .ForMember(dest => dest.TOTAL_HJE, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.TotalHje))
                .ForMember(dest => dest.TOTAL_STAMPS, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.TotalStamps))
                .ForMember(dest => dest.APPROVED_QTY, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.ApprovedQty))
                .ForMember(dest => dest.REMARKS, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.Plant))
                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.StickerCode))
                .ForMember(dest => dest.SERIES_CODE, opt => opt.MapFrom(src => src.SeriesCode))
                .ForMember(dest => dest.BRAND_NAME, opt => opt.MapFrom(src => src.BrandName))
                .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProductAlias))
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.HJE, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Hje))
                .ForMember(dest => dest.TARIFF, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Tariff))
                .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.Colour))
                .ForMember(dest => dest.REQUESTED_QTY, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.ReqQty))
                .ForMember(dest => dest.NO_PENGAWAS, opt => opt.MapFrom(src => src.NoPengawas))
                ;

            Mapper.CreateMap<Pbck4ItemDto, Pbck4UploadViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck1No, opt => opt.MapFrom(src => src.CK1_ID.HasValue ? src.CK1_ID.Value.ToString() : "0"))
                .ForMember(dest => dest.Ck1Date, opt => opt.MapFrom(src => src.CK1_DATE))
                .ForMember(dest => dest.TotalHje, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.TOTAL_HJE))
                .ForMember(dest => dest.TotalStamps, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.TOTAL_STAMPS))
                .ForMember(dest => dest.ApprovedQty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.APPROVED_QTY))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.SeriesCode, opt => opt.MapFrom(src => src.SERIES_CODE))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_NAME))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT))
                .ForMember(dest => dest.Hje, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.HJE))
                .ForMember(dest => dest.Tariff, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.TARIFF))
                .ForMember(dest => dest.Colour, opt => opt.MapFrom(src => src.COLOUR))
                .ForMember(dest => dest.ReqQty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.REQUESTED_QTY))
                .ForMember(dest => dest.NoPengawas, opt => opt.MapFrom(src => src.NO_PENGAWAS))
                
                ;

            Mapper.CreateMap<Pbck4FileUploadViewModel, PBCK4_DOCUMENTDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<PBCK4_DOCUMENTDto, Pbck4FileUploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck4Dto, Pbck4SummaryReportsItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck4Date, opt => opt.MapFrom(src => src.ReportedOn.HasValue ? src.ReportedOn.Value.ToString("dd MMM yyyy"): string.Empty))
                .ForMember(dest => dest.Pbck4No, opt => opt.MapFrom(src => src.PBCK4_NUMBER))
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                ;

            Mapper.CreateMap<Pbck4SearchSummaryReportsViewModel, Pbck4GetSummaryReportByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck4No, opt => opt.MapFrom(src => src.Pbck4No))
                .ForMember(dest => dest.YearFrom, opt => opt.MapFrom(src => src.YearFrom))
                .ForMember(dest => dest.YearTo, opt => opt.MapFrom(src => src.YearTo))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))

                ;

        }
    }
}