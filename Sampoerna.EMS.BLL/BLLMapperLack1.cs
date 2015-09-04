using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {

        public static void InitializeLack1()
        {
            #region LACK1
            Mapper.CreateMap<LACK1, Lack1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Lack1Number, opt => opt.MapFrom(src => src.LACK1_NUMBER))
                .ForMember(dest => dest.Bukrs, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.BUTXT))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PeriodYears, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_ID : string.Empty))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_NAME : string.Empty))
                .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.WasteQty, opt => opt.MapFrom(src => src.WASTE_QTY))
                .ForMember(dest => dest.WasteUom, opt => opt.MapFrom(src => src.WASTE_UOM))
                .ForMember(dest => dest.ReturnQty, opt => opt.MapFrom(src => src.RETURN_QTY))
                .ForMember(dest => dest.ReturnUom, opt => opt.MapFrom(src => src.RETURN_UOM))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ExTypDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.Periode, opt => opt.MapFrom(src => new DateTime(src.PERIOD_YEAR.Value, src.PERIOD_MONTH.Value, 1)))
                .ForMember(dest => dest.BeginingBalance, opt => opt.MapFrom(src => src.BEGINING_BALANCE))
                .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src.TOTAL_INCOME))
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.USAGE))
                .ForMember(dest => dest.Lack1UomId, opt => opt.MapFrom(src => src.LACK1_UOM_ID))
                .ForMember(dest => dest.Lack1UomName, opt => opt.MapFrom(src => src.UOM11 != null ? src.UOM11.UOM_DESC : string.Empty))
                ;

            Mapper.CreateMap<MONTH, Lack1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.MONTH_ID))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH_NAME_IND))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH_NAME_ENG));

            Mapper.CreateMap<LACK1_PRODUCTION_DETAIL, Lack1ProductionDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UOM_DESC, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty));
                ;

            #endregion

            Mapper.CreateMap<Lack1GenerateDataParamInput, CK4CItemGetByParamInput>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1GenerateDataParamInput, Ck5GetForLack1ByParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1GenerateDataParamInput, Pbck1GetDataForLack1ParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ExcisableGoodsTypeId, opt => opt.MapFrom(src => src.ExcisableGoodsType))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SupplierPlantId))
                ;

            Mapper.CreateMap<Lack1GeneratedPlantDto, LACK1_PLANT>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.Werks))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.Name1))
                .ForMember(dest => dest.PLANT_ADDRESS, opt => opt.MapFrom(src => src.Address))
                ;

            Mapper.CreateMap<Lack1GeneratedProductionDataDto, LACK1_PRODUCTION_DETAIL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.AMOUNT, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProdCode))
                .ForMember(dest => dest.PRODUCT_TYPE, opt => opt.MapFrom(src => src.ProductType))
                .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProductAlias))
                .ForMember(dest => dest.UOM_ID, opt => opt.MapFrom(src => src.UomId))
                ;

            Mapper.CreateMap<Lack1GeneratedPbck1DataDto, LACK1_PBCK1_MAPPING>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK1_ID, opt => opt.MapFrom(src => src.Pbck1Id))
                ;

            Mapper.CreateMap<Lack1GeneratedIncomeDataDto, LACK1_INCOME_DETAIL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CK5_ID, opt => opt.MapFrom(src => src.Ck5Id))
                .ForMember(dest => dest.AMOUNT, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.REGISTRATION_NUMBER, opt => opt.MapFrom(src => src.RegistrationNumber))
                .ForMember(dest => dest.REGISTRATION_DATE, opt => opt.MapFrom(src => src.RegistrationDate))
                ;

            Mapper.CreateMap<CK5, Lack1GeneratedIncomeDataDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE))
                .ForMember(dest => dest.StringRegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy") : string.Empty))
                ;

            Mapper.CreateMap<Lack1GeneratedDto, LACK1>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.BUTXT, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PERIOD_MONTH, opt => opt.MapFrom(src => src.PeriodMonthId))
                .ForMember(dest => dest.PERIOD_YEAR, opt => opt.MapFrom(src => src.PeriodYear))
                .ForMember(dest => dest.SUPPLIER_PLANT, opt => opt.MapFrom(src => src.SupplierPlantName))
                .ForMember(dest => dest.SUPPLIER_PLANT_WERKS, opt => opt.MapFrom(src => src.SupplierPlantId))
                .ForMember(dest => dest.SUPPLIER_PLANT_ADDRESS, opt => opt.MapFrom(src => src.SupplierPlantAddress))
                .ForMember(dest => dest.EX_GOODTYP, opt => opt.MapFrom(src => src.ExcisableGoodsType))
                .ForMember(dest => dest.EX_TYP_DESC, opt => opt.MapFrom(src => src.ExcisableGoodsTypeDesc))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.BEGINING_BALANCE, opt => opt.MapFrom(src => src.BeginingBalance))
                .ForMember(dest => dest.TOTAL_INCOME, opt => opt.MapFrom(src => src.TotalIncome))
                .ForMember(dest => dest.USAGE, opt => opt.MapFrom(src => src.TotalUsage))
                .ForMember(dest => dest.LACK1_UOM_ID, opt => opt.MapFrom(src => src.Lack1UomId))
                .ForMember(dest => dest.LACK1_INCOME_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_INCOME_DETAIL>>(src.IncomeList)))
                //.ForMember(dest => dest.LACK1_PLANT, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PLANT>>(src.PlantList))) //todo: set from BLL
                .ForMember(dest => dest.LACK1_PRODUCTION_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PRODUCTION_DETAIL>>(src.ProductionList)))
                .ForMember(dest => dest.LACK1_PBCK1_MAPPING, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PBCK1_MAPPING>>(src.Pbck1List)))
                ;

            Mapper.CreateMap<T001W, LACK1_PLANT>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.PLANT_ADDRESS, opt => opt.MapFrom(src => src.ADDRESS))
                ;

            Mapper.CreateMap<PBCK1, Lack1GeneratedPbck1DataDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID));

            Mapper.CreateMap<LACK1, Lack1DetailsDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Lack1Number, opt => opt.MapFrom(src => src.LACK1_NUMBER))
                .ForMember(dest => dest.Bukrs, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.BUTXT))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PeriodYears, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_ID : string.Empty))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_NAME : string.Empty))
                .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.WasteQty, opt => opt.MapFrom(src => src.WASTE_QTY))
                .ForMember(dest => dest.WasteUom, opt => opt.MapFrom(src => src.WASTE_UOM))
                .ForMember(dest => dest.ReturnQty, opt => opt.MapFrom(src => src.RETURN_QTY))
                .ForMember(dest => dest.ReturnUom, opt => opt.MapFrom(src => src.RETURN_UOM))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedByPoa, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedPoaDate, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ExGoodsTypeDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.Periode, opt => opt.MapFrom(src => new DateTime(src.PERIOD_YEAR.Value, src.PERIOD_MONTH.Value, 1)))
                .ForMember(dest => dest.BeginingBalance, opt => opt.MapFrom(src => src.BEGINING_BALANCE))
                .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src.TOTAL_INCOME))
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.USAGE))
                .ForMember(dest => dest.Lack1UomId, opt => opt.MapFrom(src => src.LACK1_UOM_ID))
                .ForMember(dest => dest.Lack1UomName, opt => opt.MapFrom(src => src.UOM11 != null ? src.UOM11.UOM_DESC : string.Empty))
                ;

        }

    }
}
