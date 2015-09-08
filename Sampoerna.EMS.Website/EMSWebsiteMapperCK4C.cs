using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Models.CK4C;
using Sampoerna.EMS.Utils;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeCk4C()
        {
            #region Index Daily Prduction

            Mapper.CreateMap<Ck4CDto, DataIndecCk4C>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PlantName))
                .ForMember(dest => dest.FinishGoods, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.ProdQtyPacked))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn));

            Mapper.CreateMap<Ck4CIndexViewModel, Ck4CGetByParamInput>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantName))
               .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode))
               .ForMember(dest => dest.DateProduction, opt => opt.MapFrom(src => src.ProductionDate));

            #endregion

            #region Index Waste Production
            Mapper.CreateMap<Ck4CDto, DataWasteProduction>().IgnoreAllNonExisting()
             .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CompanyId))
             .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
             .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
             .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PlantName))
             .ForMember(dest => dest.FinishGoods, opt => opt.MapFrom(src => src.FaCode))
             .ForMember(dest => dest.WasteQty, opt => opt.MapFrom(src => src.ProdQtyUnpacked))
             .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn));
            
            Mapper.CreateMap<Ck4CIndexWasteProductionViewModel, Ck4CGetByParamInput>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantName))
               .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode))
               .ForMember(dest => dest.DateProduction, opt => opt.MapFrom(src => src.ProductionDate));
            #endregion

            #region Index Document List
            Mapper.CreateMap<Ck4CDto, DataDocumentList>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck4CId, opt => opt.MapFrom(src => src.Ck4CId))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PlantName))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ApprovedByPoa, opt => opt.MapFrom(src => src.ApprovedByPoa))
                .ForMember(dest => dest.ApprovedDatePoa, opt => opt.MapFrom(src => src.ApprovedDatePoa))
                .ForMember(dest => dest.ApprovedByManager, opt => opt.MapFrom(src => src.ApprovedByManager))
                .ForMember(dest => dest.ApprovedDateManager, opt => opt.MapFrom(src => src.ApprovedDateManager))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.ReportedPeriod, opt => opt.MapFrom(src => src.ReportedPeriod))
                .ForMember(dest => dest.ReportedMonth, opt => opt.MapFrom(src => src.ReportedMonth))
                .ForMember(dest => dest.ReportedYears, opt => opt.MapFrom(src => src.ReportedYears))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusGoverment, opt => opt.MapFrom(src => src.StatusGoverment))
                .ForMember(dest => dest.ReportedMonthName, opt => opt.MapFrom(src => src.MonthId));
            Mapper.CreateMap<Ck4CIndexDocumentListViewModel, Ck4CGetByParamInput>().IgnoreAllNonExisting()
               .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.Ck4cNumber))
               .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode));
            #endregion

            #region Create Daily Production

            Mapper.CreateMap<Ck4CDto, Ck4cCreateViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.FinishGoods, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.QtyPacked, opt => opt.MapFrom(src => src.ProdQtyPacked))
                .ForMember(dest => dest.QtyUnpacked, opt => opt.MapFrom(src => src.ProdQtyUnpacked))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UomProudQty))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

            Mapper.CreateMap<Ck4cCreateViewModel, Ck4CDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FinishGoods))
                .ForMember(dest => dest.ProdQtyPacked, opt => opt.MapFrom(src => src.QtyPacked))
                .ForMember(dest => dest.ProdQtyUnpacked, opt => opt.MapFrom(src => src.QtyUnpacked))
                .ForMember(dest => dest.UomProudQty, opt => opt.MapFrom(src => src.Uom))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));

            #endregion

            #region Create Waste Production

            //Mapper.CreateMap<Ck4CDto, Ck4CCreateWasteProductionViewModel>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CompnayId))
            //    .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
            //    .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn))
            //    .ForMember(dest => dest.FinishGoods, opt => opt.MapFrom(src => src.FaCode))
            //    .ForMember(dest => dest.QtyPacked, opt => opt.MapFrom(src => src.ProdQtyPacked))
            //    .ForMember(dest => dest.QtyUnpacked, opt => opt.MapFrom(src => src.ProdQtyUnpacked))
            //    .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UomProudQty));


            #endregion
        }
    }
}