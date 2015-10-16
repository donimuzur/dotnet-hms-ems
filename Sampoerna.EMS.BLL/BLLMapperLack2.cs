using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Utils;
using System;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void InitializeLack2()
        {
            #region LACK2

            Mapper.CreateMap<LACK2, Lack2Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack2Id, opt => opt.MapFrom(src => src.LACK2_ID))
                .ForMember(dest => dest.Lack2Number, opt => opt.MapFrom(src => src.LACK2_NUMBER))
                .ForMember(dest => dest.Burks, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.BUTXT))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.LEVEL_PLANT_ID))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.LEVEL_PLANT_NAME))
                .ForMember(dest => dest.LevelPlantCity, opt => opt.MapFrom(src => src.LEVEL_PLANT_CITY))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.ExGoodTyp, opt => opt.MapFrom(src => src.EX_GOOD_TYP))
                .ForMember(dest => dest.ExTypDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS)))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.ApprovedByManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.ApprovedDateManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER_DATE))
                .ForMember(dest => dest.RejectedBy, opt => opt.MapFrom(src => src.REJECTED_BY))
                .ForMember(dest => dest.RejectedDate, opt => opt.MapFrom(src => src.REJECTED_DATE))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                ;
            
            Mapper.CreateMap<MONTH, Lack2Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.MONTH_ID))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH_NAME_IND))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH_NAME_ENG));

            Mapper.CreateMap<Lack2DocumentDto, LACK2_DOCUMENT>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack2WorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
                .ForMember(dest => dest.FORM_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.COMMENT, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ROLE, opt => opt.MapFrom(src => src.UserRole))
                ;

            Mapper.CreateMap<LACK2_ITEM, Lack2ItemDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LACK2_ITEM_ID))
                .ForMember(dest => dest.Lack2Id, opt => opt.MapFrom(src => src.LACK2_ID))
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.Ck5Number, opt => opt.MapFrom(src => src.CK5.REGISTRATION_NUMBER + "-" + src.CK5.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.Ck5GIDate, opt => opt.MapFrom(src => src.CK5.GI_DATE == null ? null : src.CK5.GI_DATE.Value.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CK5.DEST_PLANT_COMPANY_NAME))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.CK5.DEST_PLANT_ADDRESS))
               .ForMember(dest => dest.CompanyNppbkc, opt => opt.MapFrom(src => src.CK5.DEST_PLANT_NPPBKC_ID))
                .ForMember(dest => dest.Ck5ItemQty, opt => opt.MapFrom(src => src.CK5.GRAND_TOTAL_EX));

            Mapper.CreateMap<LACK2_DOCUMENT, Lack2DocumentDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK2_DOCUMENT_ID, opt => opt.MapFrom(src => src.LACK2_DOCUMENT_ID))
                .ForMember(dest => dest.FILE_NAME, opt => opt.MapFrom(src => src.FILE_NAME))
                .ForMember(dest => dest.FILE_PATH, opt => opt.MapFrom(src => src.FILE_PATH))
                ;

            Mapper.CreateMap<LACK2, Lack2DetailsDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack2Id, opt => opt.MapFrom(src => src.LACK2_ID))
                .ForMember(dest => dest.Lack2Number, opt => opt.MapFrom(src => src.LACK2_NUMBER))
                .ForMember(dest => dest.Burks, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.BUTXT))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.LEVEL_PLANT_ID))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.LEVEL_PLANT_NAME))
                .ForMember(dest => dest.LevelPlantCity, opt => opt.MapFrom(src => src.LEVEL_PLANT_CITY))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.ExGoodTyp, opt => opt.MapFrom(src => src.EX_GOOD_TYP))
                .ForMember(dest => dest.ExTypDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS)))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.ApprovedByManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.ApprovedDateManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER_DATE))
                .ForMember(dest => dest.RejectedBy, opt => opt.MapFrom(src => src.REJECTED_BY))
                .ForMember(dest => dest.RejectedDate, opt => opt.MapFrom(src => src.REJECTED_DATE))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => Mapper.Map<List<Lack2DocumentDto>>(src.LACK2_DOCUMENT)))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => Mapper.Map<List<Lack2ItemDto>>(src.LACK2_ITEM)))
                ;

            Mapper.CreateMap<CK5, Lack2GeneratedItemDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CK5_ID, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.GIDateStr, opt => opt.MapFrom(src => src.SUBMISSION_DATE == null ? string.Empty : Convert.ToDateTime(src.SUBMISSION_DATE).ToString("dd MMM yyyy")))
                .ForMember(dest => dest.NoDateWithFormat, opt => opt.MapFrom(src => src.REGISTRATION_DATE == null ? src.REGISTRATION_NUMBER : src.REGISTRATION_NUMBER + "-" + Convert.ToDateTime(src.REGISTRATION_DATE).ToString("dd.MM.yyyy")))
                ;

            #endregion

            Mapper.CreateMap<Lack2GeneratedItemDto, LACK2_ITEM>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CK5_ID, opt => opt.MapFrom(src => src.CK5_ID))
                ;

            Mapper.CreateMap<Lack2CreateParamInput, LACK2>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.SUBMISSION_DATE, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.PERIOD_MONTH, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PERIOD_YEAR, opt => opt.MapFrom(src => src.PeriodYear))
                .ForMember(dest => dest.LEVEL_PLANT_ID, opt => opt.MapFrom(src => src.SourcePlantId))
                .ForMember(dest => dest.EX_GOOD_TYP, opt => opt.MapFrom(src => src.ExcisableGoodsType))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                ;

            Mapper.CreateMap<Lack2SaveEditInput, Lack2Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Burks, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.SourcePlantId))
                .ForMember(dest => dest.LevelPlantCity, opt => opt.MapFrom(src => src.SourcePlantCity))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.SourcePlantName))
                .ForMember(dest => dest.ExGoodTyp, opt => opt.MapFrom(src => src.ExcisableGoodsType))
                .ForMember(dest => dest.ExTypDesc, opt => opt.MapFrom(src => src.ExcisableGoodsTypeDesc))
                .ForMember(dest => dest.Burks, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PeriodYear))
                ;
        }
    }
}
