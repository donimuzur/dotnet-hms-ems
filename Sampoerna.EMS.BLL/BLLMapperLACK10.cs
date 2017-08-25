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
        public static void InitializeLack10()
        {
            #region Index

            Mapper.CreateMap<LACK10, Lack10Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack10Id, opt => opt.MapFrom(src => src.LACK10_ID))
                .ForMember(dest => dest.Lack10Number, opt => opt.MapFrom(src => src.LACK10_NUMBER))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
                .ForMember(dest => dest.CompanyNpwp, opt => opt.MapFrom(src => src.COMPANY_NPWP))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.ApprovedDateManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER_DATE))
                .ForMember(dest => dest.ApprovedByManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.ReportType, opt => opt.MapFrom(src => src.REPORT_TYPE))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PeriodYears, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.MonthId, opt => opt.MapFrom(src => src.MONTH.MONTH_ID))
                .ForMember(dest => dest.MonthNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.MonthNameIndo, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.Lack10Item, opt => opt.MapFrom(src => Mapper.Map<List<Lack10Item>>(src.LACK10_ITEM)))
                .ForMember(dest => dest.Lack10DecreeDoc, opt => opt.MapFrom(src => Mapper.Map<List<Lack10DecreeDocDto>>(src.LACK10_DECREE_DOC)));

            Mapper.CreateMap<LACK10_ITEM, Lack10Item>().IgnoreAllNonExisting()
                .ForMember(src => src.Lack10ItemId, opt => opt.MapFrom(dest => dest.LACK10_ITEM_ID))
                .ForMember(src => src.Lack10Id, opt => opt.MapFrom(dest => dest.LACK10_ID))
                .ForMember(src => src.FaCode, opt => opt.MapFrom(dest => dest.FA_CODE))
                .ForMember(src => src.BrandDescription, opt => opt.MapFrom(dest => dest.BRAND_DESCRIPTION))
                .ForMember(src => src.Werks, opt => opt.MapFrom(dest => dest.WERKS))
                .ForMember(src => src.PlantName, opt => opt.MapFrom(dest => dest.PLANT_NAME))
                .ForMember(src => src.Type, opt => opt.MapFrom(dest => dest.TYPE))
                .ForMember(src => src.WasteValue, opt => opt.MapFrom(dest => dest.WASTE_VALUE))
                .ForMember(src => src.Uom, opt => opt.MapFrom(dest => dest.UOM));

            Mapper.CreateMap<LACK10_DECREE_DOC, Lack10DecreeDocDto>().IgnoreAllNonExisting();

            #endregion

            #region Create

            Mapper.CreateMap<Lack10Dto, LACK10>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK10_ID, opt => opt.MapFrom(src => src.Lack10Id))
                .ForMember(dest => dest.LACK10_NUMBER, opt => opt.MapFrom(src => src.Lack10Number))
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.COMPANY_NPWP, opt => opt.MapFrom(src => src.CompanyNpwp))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantName))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.APPROVED_BY, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.APPROVED_DATE, opt => opt.MapFrom(src => src.ApprovedDate))
                .ForMember(dest => dest.APPROVED_BY_MANAGER_DATE, opt => opt.MapFrom(src => src.ApprovedDateManager))
                .ForMember(dest => dest.APPROVED_BY_MANAGER, opt => opt.MapFrom(src => src.ApprovedByManager))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.SUBMISSION_DATE, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.REPORT_TYPE, opt => opt.MapFrom(src => src.ReportType))
                .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.DECREE_DATE, opt => opt.MapFrom(src => src.DecreeDate))
                .ForMember(dest => dest.PERIOD_MONTH, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PERIOD_YEAR, opt => opt.MapFrom(src => src.PeriodYears))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.GOV_STATUS, opt => opt.MapFrom(src => src.GovStatus))
                .ForMember(dest => dest.LACK10_ITEM, opt => opt.MapFrom(src => Mapper.Map<List<Lack10Item>>(src.Lack10Item)))
                .ForMember(dest => dest.LACK10_DECREE_DOC, opt => opt.MapFrom(src => Mapper.Map<List<LACK10_DECREE_DOC>>(src.Lack10DecreeDoc)));

            Mapper.CreateMap<Lack10Item, LACK10_ITEM>().IgnoreAllNonExisting()
                .ForMember(src => src.LACK10_ITEM_ID, opt => opt.MapFrom(dest => dest.Lack10ItemId))
                .ForMember(src => src.LACK10_ID, opt => opt.MapFrom(dest => dest.Lack10Id))
                .ForMember(src => src.FA_CODE, opt => opt.MapFrom(dest => dest.FaCode))
                .ForMember(src => src.BRAND_DESCRIPTION, opt => opt.MapFrom(dest => dest.BrandDescription))
                .ForMember(src => src.WERKS, opt => opt.MapFrom(dest => dest.Werks))
                .ForMember(src => src.PLANT_NAME, opt => opt.MapFrom(dest => dest.PlantName))
                .ForMember(src => src.TYPE, opt => opt.MapFrom(dest => dest.Type))
                .ForMember(src => src.WASTE_VALUE, opt => opt.MapFrom(dest => dest.WasteValue))
                .ForMember(src => src.UOM, opt => opt.MapFrom(dest => dest.Uom));

            Mapper.CreateMap<Lack10DecreeDocDto, LACK10_DECREE_DOC>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack10WorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
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
