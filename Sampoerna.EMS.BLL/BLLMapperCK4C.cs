﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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

        public static void InitializeCk4C()
        {
            #region Ck4c Daily Produxction

            Mapper.CreateMap<CK4C, Ck4CDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck4CId, opt => opt.MapFrom(src => src.CK4C_ID))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.NUMBER))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.REPORTED_ON))
                .ForMember(dest => dest.ReportedPeriod, opt => opt.MapFrom(src => src.REPORTED_PERIOD))
                .ForMember(dest => dest.ReportedMonth, opt => opt.MapFrom(src => src.REPORTED_MONTH))
                .ForMember(dest => dest.ReportedYears, opt => opt.MapFrom(src => src.REPORTED_YEAR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.StatusGoverment, opt => opt.MapFrom(src => src.GOV_STATUS));

            Mapper.CreateMap<MONTH, Ck4CDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MonthId, opt => opt.MapFrom(src => src.MONTH_ID))
                .ForMember(dest => dest.MonthNameEng, opt => opt.MapFrom(src => src.MONTH_NAME_ENG))
                .ForMember(dest => dest.MonthNameIndo, opt => opt.MapFrom(src => src.MONTH_NAME_IND));

            Mapper.CreateMap<T001K, T001KDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.BWKEY, opt => opt.MapFrom(src => src.T001W.NAME1));

            Mapper.CreateMap<T001W, T001KDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BWKEY, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1));

            Mapper.CreateMap<CK4C_ITEM, Ck4CDto>().IgnoreAllNonExisting()
                .ForMember(src => src.Ck4CItemId, opt => opt.MapFrom(dest => dest.CK4C_ITEM_ID))
                .ForMember(src => src.FaCode, opt => opt.MapFrom(dest => dest.FA_CODE))
                .ForMember(src => src.Werks, opt => opt.MapFrom(dest => dest.WERKS))
                .ForMember(src => src.ProdQtyPacked, opt => opt.MapFrom(dest => dest.PROD_QTY))
                .ForMember(src => src.UomProudQty, opt => opt.MapFrom(dest => dest.UOM_PROD_QTY))
                .ForMember(src => src.ProdDate, opt => opt.MapFrom(dest => dest.PROD_DATE));

            #endregion
        }


    }
}
