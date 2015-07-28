﻿using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.PBCK1;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializePBCK1()
        {
            #region Pbck1

            Mapper.CreateMap<Pbck1Dto, Pbck1Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.StatusGovName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.StatusGov)))
                .ForMember(dest => dest.PbckTypeName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck1Type)))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.PeriodFrom.Year))
                .ForMember(dest => dest.Pbck1ReferenceNumber, opt => opt.MapFrom(src => src.Pbck1Reference != null && src.Pbck1Parent != null ? src.Pbck1Parent.Pbck1Number : string.Empty))
                .ForMember(dest => dest.Pbck1ProdConverter,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConvModel>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.Pbck1ProdPlan,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlanModel>>(src.Pbck1ProdPlan)))
             ;

            Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetOpenDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
                ;

            Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetCompletedDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
                ;
            
            Mapper.CreateMap<Pbck1ProdConvModel, Pbck1ProdConverterDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.ProdTypeAlias))
                .ForMember(dest => dest.ConverterOutput, opt => opt.MapFrom(src => src.ConverterOutput))
                .ForMember(dest => dest.ConverterOutputUomId, opt => opt.MapFrom(src => src.ConverterUomId))
                ;
            Mapper.CreateMap<Pbck1ProdConverterDto, Pbck1ProdConvModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.ProdAlias))
                .ForMember(dest => dest.ConverterOutput, opt => opt.MapFrom(src => src.ConverterOutput))
                .ForMember(dest => dest.ConverterUomId, opt => opt.MapFrom(src => src.ConverterOutputUomId))
                .ForMember(dest => dest.ConverterUom, opt => opt.MapFrom(src => src.ConverterOutputUomName))
                ;

            Mapper.CreateMap<Pbck1ProdPlanModel, Pbck1ProdPlanDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MonthId, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Month))
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.ProdTypeAlias))
                .ForMember(dest => dest.Amount, opt => opt.ResolveUsing<StringToNullableDecimalResolver>().FromMember(src => src.Amount))
                .ForMember(dest => dest.BkcRequired, opt => opt.MapFrom(src => src.BkcRequired))
                ;

            Mapper.CreateMap<Pbck1ProdPlanDto, Pbck1ProdPlanModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.MonthId))
                .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.ProdAlias))
                ;

            Mapper.CreateMap<Pbck1Item, Pbck1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdConverter,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConverterDto>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.Pbck1ProdPlan,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlanModel>>(src.Pbck1ProdPlan)))
                    ;

            #endregion

            Mapper.CreateMap<Pbck1ProdPlanModel, Pbck1ProdPlanInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdPlanOutput, Pbck1ProdPlanModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1ProdConvModel, Pbck1ProdConverterInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdConverterOutput, Pbck1ProdConvModel>().IgnoreAllNonExisting();

        }
    }
}