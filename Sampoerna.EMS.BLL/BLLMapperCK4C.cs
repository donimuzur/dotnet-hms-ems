using System;
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
using Sampoerna.EMS.BusinessObject.Inputs;

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
                .ForMember(dest => dest.ApprovedByPoa, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedDatePoa, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedDateManager, opt => opt.MapFrom(src => src.APPROVED_DATE_MANAGER))
                .ForMember(dest => dest.ApprovedByManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.REPORTED_ON))
                .ForMember(dest => dest.ReportedPeriod, opt => opt.MapFrom(src => src.REPORTED_PERIOD))
                .ForMember(dest => dest.ReportedMonth, opt => opt.MapFrom(src => src.REPORTED_MONTH))
                .ForMember(dest => dest.ReportedYears, opt => opt.MapFrom(src => src.REPORTED_YEAR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.StatusGoverment, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.MonthId, opt => opt.MapFrom(src => src.MONTH.MONTH_ID))
                .ForMember(dest => dest.MonthNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.MonthNameIndo, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.Ck4cItem, opt => opt.MapFrom(src => Mapper.Map<List<Ck4cItem>>(src.CK4C_ITEM)));

            Mapper.CreateMap<T001K, T001KDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.BWKEY, opt => opt.MapFrom(src => src.T001W.NAME1));

            Mapper.CreateMap<T001W, T001KDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BWKEY, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1));

            #endregion

            #region Ck4c Document List

            Mapper.CreateMap<Ck4CDto, CK4C>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CK4C_ID, opt => opt.MapFrom(src => src.Ck4CId))
                .ForMember(dest => dest.NUMBER, opt => opt.MapFrom(src => src.Number))
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantName))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.APPROVED_BY_POA, opt => opt.MapFrom(src => src.ApprovedByPoa))
                .ForMember(dest => dest.APPROVED_DATE_POA, opt => opt.MapFrom(src => src.ApprovedDatePoa))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.APPROVED_DATE_MANAGER, opt => opt.MapFrom(src => src.ApprovedDateManager))
                .ForMember(dest => dest.APPROVED_BY_MANAGER, opt => opt.MapFrom(src => src.ApprovedByManager))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.REPORTED_ON, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.REPORTED_PERIOD, opt => opt.MapFrom(src => src.ReportedPeriod))
                .ForMember(dest => dest.REPORTED_MONTH, opt => opt.MapFrom(src => src.ReportedMonth))
                .ForMember(dest => dest.REPORTED_YEAR, opt => opt.MapFrom(src => src.ReportedYears))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.GOV_STATUS, opt => opt.MapFrom(src => src.StatusGoverment))
                .ForMember(dest => dest.CK4C_ITEM, opt => opt.MapFrom(src => Mapper.Map<List<Ck4cItem>>(src.Ck4cItem)));

            Mapper.CreateMap<CK4C_ITEM, Ck4cItem>().IgnoreAllNonExisting()
                .ForMember(src => src.Ck4CItemId, opt => opt.MapFrom(dest => dest.CK4C_ITEM_ID))
                .ForMember(src => src.Ck4CId, opt => opt.MapFrom(dest => dest.CK4C_ID))
                .ForMember(src => src.FaCode, opt => opt.MapFrom(dest => dest.FA_CODE))
                .ForMember(src => src.Werks, opt => opt.MapFrom(dest => dest.WERKS))
                .ForMember(src => src.ProdQty, opt => opt.MapFrom(dest => dest.PROD_QTY))
                .ForMember(src => src.ProdQtyUom, opt => opt.MapFrom(dest => dest.UOM_PROD_QTY))
                .ForMember(src => src.ProdDate, opt => opt.MapFrom(dest => dest.PROD_DATE))
                .ForMember(src => src.HjeIdr, opt => opt.MapFrom(dest => dest.HJE_IDR))
                .ForMember(src => src.Tarif, opt => opt.MapFrom(dest => dest.TARIFF))
                .ForMember(src => src.ProdCode, opt => opt.MapFrom(dest => dest.PROD_CODE))
                .ForMember(src => src.PackedQty, opt => opt.MapFrom(dest => dest.PACKED_QTY))
                .ForMember(src => src.UnpackedQty, opt => opt.MapFrom(dest => dest.UNPACKED_QTY));

            Mapper.CreateMap<Ck4cItem, CK4C_ITEM>().IgnoreAllNonExisting()
                .ForMember(src => src.CK4C_ITEM_ID, opt => opt.MapFrom(dest => dest.Ck4CItemId))
                .ForMember(src => src.CK4C_ID, opt => opt.MapFrom(dest => dest.Ck4CId))
                .ForMember(src => src.FA_CODE, opt => opt.MapFrom(dest => dest.FaCode))
                .ForMember(src => src.WERKS, opt => opt.MapFrom(dest => dest.Werks))
                .ForMember(src => src.PROD_QTY, opt => opt.MapFrom(dest => dest.ProdQty))
                .ForMember(src => src.UOM_PROD_QTY, opt => opt.MapFrom(dest => dest.ProdQtyUom))
                .ForMember(src => src.PROD_DATE, opt => opt.MapFrom(dest => dest.ProdDate))
                .ForMember(src => src.HJE_IDR, opt => opt.MapFrom(dest => dest.HjeIdr))
                .ForMember(src => src.TARIFF, opt => opt.MapFrom(dest => dest.Tarif))
                .ForMember(src => src.PROD_CODE, opt => opt.MapFrom(dest => dest.ProdCode))
                .ForMember(src => src.PACKED_QTY, opt => opt.MapFrom(dest => dest.PackedQty))
                .ForMember(src => src.UNPACKED_QTY, opt => opt.MapFrom(dest => dest.UnpackedQty));

            Mapper.CreateMap<Ck4cWorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
                .ForMember(dest => dest.FORM_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.COMMENT, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ROLE, opt => opt.MapFrom(src => src.UserRole))
                ;

            #endregion
        }


    }
}
