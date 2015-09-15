﻿using System;
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
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DecreeDate))
                .ForMember(dest => dest.ReportedPeriod, opt => opt.MapFrom(src => src.ReportedPeriod))
                .ForMember(dest => dest.ReportedMonth, opt => opt.MapFrom(src => src.ReportedMonth))
                .ForMember(dest => dest.ReportedYears, opt => opt.MapFrom(src => src.ReportedYears))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusGoverment, opt => opt.MapFrom(src => src.StatusGoverment))
                .ForMember(dest => dest.ReportedMonthName, opt => opt.MapFrom(src => src.MonthNameEng))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.StatusGovName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.StatusGoverment)))
                .ForMember(dest => dest.BasedOn, opt => opt.MapFrom(src => src.PlantId != null ? "PLANT" : "NPPBKC"))
                .ForMember(dest => dest.Ck4cItemData, opt => opt.MapFrom(src => Mapper.Map<List<Ck4cItemData>>(src.Ck4cItem)));

            Mapper.CreateMap<Ck4CIndexDocumentListViewModel, Ck4CGetByParamInput>().IgnoreAllNonExisting()
               .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.Ck4cNumber))
               .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
               .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode));
            
            Mapper.CreateMap<Ck4cItem, Ck4cItemData>().IgnoreAllNonExisting()
                .ForMember(src => src.FaCode, opt => opt.MapFrom(dest => dest.FaCode))
                .ForMember(src => src.Werks, opt => opt.MapFrom(dest => dest.Werks))
                .ForMember(src => src.ProdQty, opt => opt.MapFrom(dest => dest.ProdQty))
                .ForMember(src => src.ProdQtyUom, opt => opt.MapFrom(dest => dest.ProdQtyUom))
                .ForMember(src => src.ProdDate, opt => opt.MapFrom(dest => dest.ProdDate))
                .ForMember(src => src.HjeIdr, opt => opt.MapFrom(dest => dest.HjeIdr))
                .ForMember(src => src.Tarif, opt => opt.MapFrom(dest => dest.Tarif))
                .ForMember(src => src.ProdCode, opt => opt.MapFrom(dest => dest.ProdCode))
                .ForMember(src => src.PackedQty, opt => opt.MapFrom(dest => dest.PackedQty))
                .ForMember(src => src.UnpackedQty, opt => opt.MapFrom(dest => dest.UnpackedQty));

            Mapper.CreateMap<Ck4CIndexDocumentListViewModel, Ck4cGetOpenDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.Ck4cNumber))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode));

            Mapper.CreateMap<Ck4CIndexDocumentListViewModel, Ck4cGetCompletedDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.Ck4cNumber))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyCode));

            #endregion

            #region Create Document List

            Mapper.CreateMap<DataDocumentList, Ck4CDto>().IgnoreAllNonExisting()
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
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DecreeDate))
                .ForMember(dest => dest.ReportedPeriod, opt => opt.MapFrom(src => src.ReportedPeriod))
                .ForMember(dest => dest.ReportedMonth, opt => opt.MapFrom(src => src.ReportedMonth))
                .ForMember(dest => dest.ReportedYears, opt => opt.MapFrom(src => src.ReportedYears))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusGoverment, opt => opt.MapFrom(src => src.StatusGoverment))
                .ForMember(dest => dest.Ck4cItem, opt => opt.MapFrom(src => Mapper.Map<List<Ck4cItemData>>(src.Ck4cItemData)));

            Mapper.CreateMap<Ck4cItemData, Ck4cItem>().IgnoreAllNonExisting()
                .ForMember(src => src.FaCode, opt => opt.MapFrom(dest => dest.FaCode))
                .ForMember(src => src.Werks, opt => opt.MapFrom(dest => dest.Werks))
                .ForMember(src => src.ProdQty, opt => opt.MapFrom(dest => dest.ProdQty))
                .ForMember(src => src.ProdQtyUom, opt => opt.MapFrom(dest => dest.ProdQtyUom))
                .ForMember(src => src.ProdDate, opt => opt.MapFrom(dest => dest.ProdDate))
                .ForMember(src => src.HjeIdr, opt => opt.MapFrom(dest => dest.HjeIdr))
                .ForMember(src => src.Tarif, opt => opt.MapFrom(dest => dest.Tarif))
                .ForMember(src => src.ProdCode, opt => opt.MapFrom(dest => dest.ProdCode))
                .ForMember(src => src.PackedQty, opt => opt.MapFrom(dest => dest.PackedQty))
                .ForMember(src => src.UnpackedQty, opt => opt.MapFrom(dest => dest.UnpackedQty));

            Mapper.CreateMap<Ck4cDecreeDocDto, Ck4cDecreeDocModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<Ck4cDecreeDocModel, Ck4cDecreeDocDto>().IgnoreAllNonExisting();

            #endregion
        }
    }
}