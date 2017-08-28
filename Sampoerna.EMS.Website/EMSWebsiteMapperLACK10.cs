using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Models.LACK10;
using Sampoerna.EMS.Utils;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeLACK10()
        {
            #region Index

            Mapper.CreateMap<Lack10Dto, DataDocumentList>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ReportedMonthName, opt => opt.MapFrom(src => src.MonthNameEng))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.StatusGoverment, opt => opt.MapFrom(src => src.GovStatus))
                .ForMember(dest => dest.StatusGovName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GovStatus)))
                .ForMember(dest => dest.BasedOn, opt => opt.MapFrom(src => src.PlantId != null ? "PLANT" : "NPPBKC"))
                .ForMember(dest => dest.Lack10ItemData, opt => opt.MapFrom(src => Mapper.Map<List<Lack10Item>>(src.Lack10Item)));

            Mapper.CreateMap<Lack10IndexViewModel, Lack10GetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.Month, opt => opt.ResolveUsing<StringToIntResolver>().FromMember(src => src.Month))
                .ForMember(dest => dest.Year, opt => opt.ResolveUsing<StringToIntResolver>().FromMember(src => src.Year));

            Mapper.CreateMap<Lack10Item, Lack10ItemData>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack10DecreeDocDto, Lack10DecreeDocModel>().IgnoreAllNonExisting();
            

            #endregion


            #region Create

            Mapper.CreateMap<DataDocumentList, Lack10Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack10Item, opt => opt.MapFrom(src => Mapper.Map<List<Lack10Item>>(src.Lack10ItemData)));

            Mapper.CreateMap<Lack10ItemData, Lack10Item>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack10DecreeDocModel, Lack10DecreeDocDto>().IgnoreAllNonExisting();

            #endregion

            #region ExportItem

            Mapper.CreateMap<Lack10ExportDto, Lack10Export>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemList, opt => opt.MapFrom(src => Mapper.Map<List<Lack10ExportItem>>(src.ItemList)));

            Mapper.CreateMap<Lack10ItemExportDto, Lack10ExportItem>().IgnoreAllNonExisting();

            #endregion

            #region SummaryReports

            Mapper.CreateMap<Lack10SummaryReportDto, SummaryReportsItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<SummaryReportsItem, Lack10SummaryReportDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<SearchSummaryReportsViewModel, Lack10GetSummaryReportByParamInput>().IgnoreAllNonExisting()
                .ForMember(src => src.Month, opt => opt.ResolveUsing<StringToIntResolver>().FromMember(dest => dest.Month))
                .ForMember(src => src.Year, opt => opt.ResolveUsing<StringToIntResolver>().FromMember(dest => dest.Year));

            #endregion
        }
    }
}