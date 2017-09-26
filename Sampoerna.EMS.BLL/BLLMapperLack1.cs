using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.BusinessObject.Outputs;

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
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.SupplierCompanyName, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_NAME))
                .ForMember(dest => dest.SupplierCompanyCode, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_CODE))
                .ForMember(dest => dest.SupplierPlantAddress, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_ADDRESS))
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
                .ForMember(dest => dest.UsageTisToTis, opt => opt.MapFrom(src => src.USAGE_TISTOTIS))
                .ForMember(dest => dest.Lack1UomId, opt => opt.MapFrom(src => src.LACK1_UOM_ID))
                .ForMember(dest => dest.Lack1UomName, opt => opt.MapFrom(src => src.UOM11 != null ? src.UOM11.UOM_DESC : string.Empty))
                .ForMember(dest => dest.Noted, opt => opt.MapFrom(src => src.NOTED))
                .ForMember(dest => dest.DocumentNoted, opt => opt.MapFrom(src => src.DOCUMENT_NOTED))
                ;

            Mapper.CreateMap<MONTH, Lack1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.MONTH_ID))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH_NAME_IND))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH_NAME_ENG));

            Mapper.CreateMap<LACK1_PRODUCTION_DETAIL, Lack1ProductionDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UOM_DESC, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty));
            ;

            Mapper.CreateMap<LACK1_DOCUMENT, Lack1DocumentDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<LACK1_INCOME_DETAIL, Lack1IncomeDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PACKAGE_UOM_ID, opt => opt.MapFrom(src => src.CK5 != null ? src.CK5.PACKAGE_UOM_ID : string.Empty))
                .ForMember(dest => dest.CK5_TYPE, opt => opt.MapFrom(src => src.CK5.CK5_TYPE))
                .ForMember(dest => dest.FLAG_FOR_LACK1, opt => opt.MapFrom(src => src.CK5.FLAG_FOR_LACK1.HasValue && src.CK5.FLAG_FOR_LACK1.Value))
                .ForMember(dest => dest.PACKAGE_UOM_DESC, opt => opt.MapFrom(src => src.CK5 != null && src.CK5.UOM != null ? src.CK5.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.IS_REDUCE_TRIAL, opt => opt.MapFrom(src=> src.CK5 != null && (src.CK5.REDUCE_TRIAL.HasValue && src.CK5.REDUCE_TRIAL.Value)))
                ;

            Mapper.CreateMap<LACK1_PBCK1_MAPPING, Lack1Pbck1MappingDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK1_NUMBER, opt => opt.MapFrom(src => src.PBCK1.NUMBER))
                .ForMember(dest => dest.DECREE_DATE, opt => opt.MapFrom(src => src.PBCK1.DECREE_DATE))
                .ForMember(dest => dest.SUPPLIER_COMPANY, opt => opt.MapFrom(src => src.PBCK1.SUPPLIER_COMPANY))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.PBCK1.REQUEST_QTY_UOM))
                .ForMember(dest => dest.ApprovedQty, opt => opt.MapFrom(src => src.PBCK1.QTY_APPROVED.HasValue ? src.PBCK1.QTY_APPROVED.Value : 0))
                ;

            Mapper.CreateMap<LACK1_PLANT, Lack1PlantDto>().IgnoreAllNonExisting();

            #endregion

            Mapper.CreateMap<Lack1GenerateDataParamInput, CK4CItemGetByParamInput>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1GenerateDataParamInput, Ck5GetForLack1ByParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1GenerateDataParamInput, Pbck1GetDataForLack1ParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ExcisableGoodsTypeId, opt => opt.MapFrom(src => src.ExcisableGoodsType))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SupplierPlantId))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PeriodYear))
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
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.ORDR, opt => opt.MapFrom(src => src.Ordr))
                .ForMember(dest => dest.IS_TISTOTIS_DATA, opt => opt.MapFrom(src => src.IsTisToTisData))
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
                .ForMember(dest => dest.StoReceiverNumber, opt => opt.MapFrom(src => src.STO_RECEIVER_NUMBER))
                .ForMember(dest => dest.StoSenderNumber, opt => opt.MapFrom(src => src.STO_SENDER_NUMBER))
                .ForMember(dest => dest.DnNumber, opt => opt.MapFrom(src => src.DN_NUMBER))
                .ForMember(dest => dest.Ck5Type, opt => opt.MapFrom(src => src.CK5_TYPE))
                .ForMember(dest => dest.PackageUomId, opt => opt.MapFrom(src => src.PACKAGE_UOM_ID))
                .ForMember(dest => dest.PackageUomDesc, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE))
                .ForMember(dest => dest.SubmissionNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.GrDate, opt => opt.MapFrom(src => src.GR_DATE))
                .ForMember(dest => dest.GrandTotalEx, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX.HasValue? src.GRAND_TOTAL_EX.Value : 0))
                .ForMember(dest => dest.FlagForLack1, opt => opt.MapFrom(src => src.FLAG_FOR_LACK1.HasValue && src.FLAG_FOR_LACK1.Value))
                .ForMember(dest => dest.IsCk5ReduceTrial, opt => opt.MapFrom(src => src.REDUCE_TRIAL.HasValue && src.REDUCE_TRIAL.Value))
                .ForMember(dest => dest.StringRegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy") : string.Empty))
                .ForMember(dest => dest.IsForLab, opt => opt.MapFrom(src => src.FLAG_FOR_LACK1_LAB.HasValue ? src.FLAG_FOR_LACK1_LAB.Value : false))
                ;

            Mapper.CreateMap<Lack1GeneratedDto, LACK1>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.BUTXT, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PERIOD_MONTH, opt => opt.MapFrom(src => src.PeriodMonthId))
                .ForMember(dest => dest.PERIOD_YEAR, opt => opt.MapFrom(src => src.PeriodYear))
                .ForMember(dest => dest.SUPPLIER_PLANT, opt => opt.MapFrom(src => src.SupplierPlantName))
                .ForMember(dest => dest.SUPPLIER_COMPANY_CODE, opt => opt.MapFrom(src => src.SupplierCompanyCode))
                .ForMember(dest => dest.SUPPLIER_COMPANY_NAME, opt => opt.MapFrom(src => src.SupplierCompanyName))
                .ForMember(dest => dest.SUPPLIER_PLANT_WERKS, opt => opt.MapFrom(src => src.SupplierPlantId))
                .ForMember(dest => dest.SUPPLIER_PLANT_ADDRESS, opt => opt.MapFrom(src => src.SupplierPlantAddress))
                .ForMember(dest => dest.EX_GOODTYP, opt => opt.MapFrom(src => src.ExcisableGoodsType))
                .ForMember(dest => dest.EX_TYP_DESC, opt => opt.MapFrom(src => src.ExcisableGoodsTypeDesc))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.BEGINING_BALANCE, opt => opt.MapFrom(src => src.BeginingBalance))
                .ForMember(dest => dest.TOTAL_INCOME, opt => opt.MapFrom(src => src.TotalIncome))
                .ForMember(dest => dest.USAGE, opt => opt.MapFrom(src => src.TotalUsage))
                .ForMember(dest => dest.USAGE_TISTOTIS, opt => opt.MapFrom(src => src.TotalUsageTisToTis))
                .ForMember(dest => dest.LACK1_UOM_ID, opt => opt.MapFrom(src => src.Lack1UomId))
                .ForMember(dest => dest.DOCUMENT_NOTED, opt => opt.MapFrom(src => src.DocumentNoted))
                .ForMember(dest => dest.NOTED, opt => opt.MapFrom(src => src.Noted))
                //.ForMember(dest => dest.LACK1_INCOME_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_INCOME_DETAIL>>(src.IncomeList)))/*For display*/
                .ForMember(dest => dest.LACK1_INCOME_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_INCOME_DETAIL>>(src.AllIncomeList)))/*For saving to database*/
                //.ForMember(dest => dest.LACK1_PLANT, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PLANT>>(src.PlantList))) //todo: set from BLL
                //.ForMember(dest => dest.LACK1_PRODUCTION_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PRODUCTION_DETAIL>>(src.ProductionList))) //todo: set from BLL
                .ForMember(dest => dest.LACK1_PBCK1_MAPPING, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PBCK1_MAPPING>>(src.Pbck1List)))
                ;

            Mapper.CreateMap<Lack1IncomeDetailDto, LACK1_INCOME_DETAIL>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1Pbck1MappingDto, LACK1_PBCK1_MAPPING>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1PlantDto, LACK1_PLANT>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1ProductionDetailDto, LACK1_PRODUCTION_DETAIL>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1DetailsDto, LACK1>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK1_ID, opt => opt.MapFrom(src => src.Lack1Id))
                .ForMember(dest => dest.LACK1_NUMBER, opt => opt.MapFrom(src => src.Lack1Number))
                .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.Bukrs))
                .ForMember(dest => dest.BUTXT, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.PERIOD_MONTH, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PERIOD_YEAR, opt => opt.MapFrom(src => src.PeriodYears))
                .ForMember(dest => dest.SUBMISSION_DATE, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.SUPPLIER_PLANT, opt => opt.MapFrom(src => src.SupplierPlant))
                .ForMember(dest => dest.SUPPLIER_PLANT_WERKS, opt => opt.MapFrom(src => src.SupplierPlantId))
                .ForMember(dest => dest.SUPPLIER_PLANT_ADDRESS, opt => opt.MapFrom(src => src.SupplierPlantAddress))
                .ForMember(dest => dest.SUPPLIER_COMPANY_CODE, opt => opt.MapFrom(src => src.SupplierCompanyCode))
                .ForMember(dest => dest.SUPPLIER_COMPANY_NAME, opt => opt.MapFrom(src => src.SupplierCompanyName))
                .ForMember(dest => dest.EX_GOODTYP, opt => opt.MapFrom(src => src.ExGoodsType))
                .ForMember(dest => dest.EX_TYP_DESC, opt => opt.MapFrom(src => src.ExGoodsTypeDesc))
                .ForMember(dest => dest.WASTE_QTY, opt => opt.MapFrom(src => src.WasteQty))
                .ForMember(dest => dest.WASTE_UOM, opt => opt.MapFrom(src => src.WasteUom))
                .ForMember(dest => dest.RETURN_QTY, opt => opt.MapFrom(src => src.ReturnQty))
                .ForMember(dest => dest.RETURN_UOM, opt => opt.MapFrom(src => src.ReturnUom))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.GOV_STATUS, opt => opt.MapFrom(src => src.GovStatus))
                .ForMember(dest => dest.DECREE_DATE, opt => opt.MapFrom(src => src.DecreeDate))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.BEGINING_BALANCE, opt => opt.MapFrom(src => src.BeginingBalance))
                .ForMember(dest => dest.TOTAL_INCOME, opt => opt.MapFrom(src => src.TotalIncome))
                .ForMember(dest => dest.USAGE, opt => opt.MapFrom(src => src.Usage))
                .ForMember(dest => dest.USAGE_TISTOTIS, opt => opt.MapFrom(src => src.UsageTisToTis))
                .ForMember(dest => dest.LACK1_LEVEL, opt => opt.MapFrom(src => src.Lack1Level))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreateBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.LACK1_UOM_ID, opt => opt.MapFrom(src => src.Lack1UomId))
                .ForMember(dest => dest.NOTED, opt => opt.MapFrom(src => src.Noted))
                .ForMember(dest => dest.DOCUMENT_NOTED, opt => opt.MapFrom(src => src.DocumentNoted))
                .ForMember(dest => dest.LACK1_INCOME_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_INCOME_DETAIL>>(src.Lack1IncomeDetail)))
                .ForMember(dest => dest.LACK1_PLANT, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PLANT>>(src.Lack1Plant))) 
                .ForMember(dest => dest.LACK1_PRODUCTION_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PRODUCTION_DETAIL>>(src.Lack1ProductionDetail)))
                .ForMember(dest => dest.LACK1_PBCK1_MAPPING, opt => opt.MapFrom(src => Mapper.Map<List<LACK1_PBCK1_MAPPING>>(src.Lack1Pbck1Mapping)))
                .ForMember(dest => dest.IS_SUPPLIER_IMPORT, opt => opt.MapFrom(src => src.IsSupplierNppbkcImport))
                .ForMember(dest => dest.LACK1_PERIOD_SUMMARY, opt => opt.MapFrom(src => Mapper.Map<List<PeriodSummary>>(src.PeriodSummaries)))
                .ForMember(dest => dest.LACK1_CALCULATION_DETAIL, opt => opt.MapFrom(src => Mapper.Map<List<Lack1CalculationDetail>>(src.CalculationDetails)))
                ;

            Mapper.CreateMap<T001W, LACK1_PLANT>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.PLANT_ADDRESS, opt => opt.MapFrom(src => src.ADDRESS))
                ;

            Mapper.CreateMap<PBCK1, Lack1GeneratedPbck1DataDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.QtyApproved, opt => opt.MapFrom(src => src.QTY_APPROVED))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.REQUEST_QTY_UOM))
                .ForMember(dest => dest.Pbck1Convertion, opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConverterDto>>(src.PBCK1_PROD_CONVERTER)));

            Mapper.CreateMap<LACK1_PBCK1_MAPPING, Lack1GeneratedPbck1DataDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID));

            Mapper.CreateMap<LACK1, Lack1DetailsDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Lack1Number, opt => opt.MapFrom(src => src.LACK1_NUMBER))
                .ForMember(dest => dest.Bukrs, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.BUTXT))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.Periode, opt => opt.MapFrom(src => new DateTime(src.PERIOD_YEAR.Value, src.PERIOD_MONTH.Value, 1)))
                .ForMember(dest => dest.PeriodYears, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_ID : string.Empty))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_NAME : string.Empty))
                .ForMember(dest => dest.Lack1Level, opt => opt.MapFrom(src => src.LACK1_LEVEL))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.SupplierCompanyName, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_NAME))
                .ForMember(dest => dest.SupplierCompanyCode, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_CODE))
                .ForMember(dest => dest.SupplierPlantAddress, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_ADDRESS))
                .ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.ExGoodsTypeDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.WasteQty, opt => opt.MapFrom(src => src.WASTE_QTY))
                .ForMember(dest => dest.WasteUom, opt => opt.MapFrom(src => src.WASTE_UOM))
                .ForMember(dest => dest.WasteUomDesc, opt => opt.MapFrom(src => src.UOM1 != null ? src.UOM1.UOM_DESC : string.Empty))
                .ForMember(dest => dest.ReturnQty, opt => opt.MapFrom(src => src.RETURN_QTY))
                .ForMember(dest => dest.ReturnUom, opt => opt.MapFrom(src => src.RETURN_UOM))
                .ForMember(dest => dest.ReturnUomDesc, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.BeginingBalance, opt => opt.MapFrom(src => src.BEGINING_BALANCE))
                .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src.TOTAL_INCOME))
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.USAGE))
                .ForMember(dest => dest.UsageTisToTis, opt => opt.MapFrom(src => src.USAGE_TISTOTIS))
                .ForMember(dest => dest.EndingBalance, opt => opt.MapFrom(src => src.BEGINING_BALANCE + src.TOTAL_INCOME - (src.USAGE + (src.USAGE_TISTOTIS.HasValue ? src.USAGE_TISTOTIS.Value : 0)) - (src.RETURN_QTY.HasValue ? src.RETURN_QTY.Value : 0)))
                .ForMember(dest => dest.Lack1UomId, opt => opt.MapFrom(src => src.LACK1_UOM_ID))
                .ForMember(dest => dest.Lack1UomName, opt => opt.MapFrom(src => src.UOM11 != null ? src.UOM11.UOM_DESC : string.Empty))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedByPoa, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedPoaDate, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.Lack1Document, opt => opt.MapFrom(src => Mapper.Map<List<Lack1DocumentDto>>(src.LACK1_DOCUMENT)))
                //.ForMember(dest => dest.Lack1IncomeDetail, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.LACK1_INCOME_DETAIL)))//todo: set from BLL, need to exlude some CK5 Type for display only
                .ForMember(dest => dest.AllLack1IncomeDetail, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.LACK1_INCOME_DETAIL)))
                .ForMember(dest => dest.Lack1Pbck1Mapping, opt => opt.MapFrom(src => Mapper.Map<List<Lack1Pbck1MappingDto>>(src.LACK1_PBCK1_MAPPING)))
                .ForMember(dest => dest.Lack1Plant, opt => opt.MapFrom(src => Mapper.Map<List<Lack1PlantDto>>(src.LACK1_PLANT)))
                .ForMember(dest => dest.Lack1ProductionDetail, opt => opt.MapFrom(src => Mapper.Map<List<Lack1ProductionDetailDto>>(src.LACK1_PRODUCTION_DETAIL)))
                .ForMember(dest => dest.Noted, opt => opt.MapFrom(src => src.NOTED))
                .ForMember(dest => dest.DocumentNoted, opt => opt.MapFrom(src => src.DOCUMENT_NOTED))
                .ForMember(dest => dest.IsTisToTis, opt => opt.MapFrom(src => src.IS_TIS_TO_TIS.HasValue && src.IS_TIS_TO_TIS.Value))
                .ForMember(dest => dest.IsSupplierNppbkcImport, opt => opt.MapFrom(src => src.IS_SUPPLIER_IMPORT.HasValue && src.IS_SUPPLIER_IMPORT.Value))
                .ForMember(dest=> dest.PeriodSummaries, opt=> opt.MapFrom(src=> Mapper.Map<List<PeriodSummary>>(src.LACK1_PERIOD_SUMMARY)))
                .ForMember(dest => dest.CalculationDetails, opt => opt.MapFrom(src => Mapper.Map<List<Lack1CalculationDetail>>(src.LACK1_CALCULATION_DETAIL)))
                ;

            Mapper.CreateMap<LACK1, Lack1PrintOutDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Lack1Level, opt => opt.MapFrom(src => src.LACK1_LEVEL))
                .ForMember(dest => dest.Lack1Number, opt => opt.MapFrom(src => src.LACK1_NUMBER))
                .ForMember(dest => dest.Bukrs, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.BUTXT))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PERIOD_MONTH))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                .ForMember(dest => dest.Periode, opt => opt.MapFrom(src => new DateTime(src.PERIOD_YEAR.Value, src.PERIOD_MONTH.Value, 1)))
                .ForMember(dest => dest.PeriodYears, opt => opt.MapFrom(src => src.PERIOD_YEAR))
                .ForMember(dest => dest.LevelPlantId, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_ID : string.Empty))
                .ForMember(dest => dest.LevelPlantName, opt => opt.MapFrom(src => src.LACK1_PLANT != null && src.LACK1_LEVEL == Enums.Lack1Level.Plant && src.LACK1_PLANT.FirstOrDefault() != null ? src.LACK1_PLANT.FirstOrDefault().PLANT_NAME : string.Empty))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.SupplierPlantAddress, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_ADDRESS))
                .ForMember(dest => dest.SupplierCompanyCode, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_CODE))
                .ForMember(dest => dest.SupplierCompanyName, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_NAME))
                .ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.ExGoodsTypeDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.WasteQty, opt => opt.MapFrom(src => src.WASTE_QTY))
                .ForMember(dest => dest.WasteUom, opt => opt.MapFrom(src => src.WASTE_UOM))
                .ForMember(dest => dest.WasteUomDesc, opt => opt.MapFrom(src => src.UOM1 != null ? src.UOM1.UOM_DESC : string.Empty))
                .ForMember(dest => dest.ReturnQty, opt => opt.MapFrom(src => src.RETURN_QTY))
                .ForMember(dest => dest.ReturnUom, opt => opt.MapFrom(src => src.RETURN_UOM))
                .ForMember(dest => dest.ReturnUomDesc, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.BeginingBalance, opt => opt.MapFrom(src => src.BEGINING_BALANCE))
                .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src.TOTAL_INCOME))
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.USAGE))
                .ForMember(dest => dest.UsageTisToTis, opt => opt.MapFrom(src => src.USAGE_TISTOTIS))
                .ForMember(dest => dest.Lack1UomId, opt => opt.MapFrom(src => src.LACK1_UOM_ID))
                .ForMember(dest => dest.Lack1UomName, opt => opt.MapFrom(src => src.UOM11 != null ? src.UOM11.UOM_DESC : string.Empty))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedByPoa, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedPoaDate, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.Lack1Document, opt => opt.MapFrom(src => Mapper.Map<List<Lack1DocumentDto>>(src.LACK1_DOCUMENT)))
                //.ForMember(dest => dest.Lack1IncomeDetail, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.LACK1_INCOME_DETAIL)))//todo: set from BLL
                .ForMember(dest => dest.AllLack1IncomeDetail, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.LACK1_INCOME_DETAIL)))
                .ForMember(dest => dest.Lack1Pbck1Mapping, opt => opt.MapFrom(src => Mapper.Map<List<Lack1Pbck1MappingDto>>(src.LACK1_PBCK1_MAPPING)))
                .ForMember(dest => dest.Lack1Plant, opt => opt.MapFrom(src => Mapper.Map<List<Lack1PlantDto>>(src.LACK1_PLANT)))
                .ForMember(dest => dest.Lack1ProductionDetail, opt => opt.MapFrom(src => Mapper.Map<List<Lack1ProductionDetailDto>>(src.LACK1_PRODUCTION_DETAIL)))
                .ForMember(dest => dest.Noted, opt => opt.MapFrom(src => src.NOTED))
                .ForMember(dest => dest.DocumentNoted, opt => opt.MapFrom(src => src.DOCUMENT_NOTED))
                .ForMember(dest => dest.IsTisToTis, opt => opt.MapFrom(src => src.IS_TIS_TO_TIS))
                .ForMember(dest => dest.IsSupplierNppbkcImport, opt => opt.MapFrom(src => src.IS_SUPPLIER_IMPORT))
                .ForMember(dest => dest.PeriodSummaries, opt => opt.MapFrom(src => Mapper.Map<List<PeriodSummary>>(src.LACK1_PERIOD_SUMMARY)))
                .ForMember(dest => dest.CalculationDetails, opt => opt.MapFrom(src => Mapper.Map<List<Lack1CalculationDetail>>(src.LACK1_CALCULATION_DETAIL)))
                ;

            Mapper.CreateMap<InvMovementItemWithConvertion, Lack1GeneratedTrackingDto>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.INVENTORY_MOVEMENT_ID, opt => opt.MapFrom(src => src.INVENTORY_MOVEMENT_ID))
                ;

            Mapper.CreateMap<Lack1GeneratedTrackingDto, LACK1_TRACKING>().IgnoreAllNonExisting()
                .ForMember(dest => dest.INVENTORY_MOVEMENT_ID, opt => opt.MapFrom(src => src.INVENTORY_MOVEMENT_ID))
                .ForMember(dest => dest.IS_TISTOTIS_DATA, opt => opt.MapFrom(src => src.IsTisToTisData))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CONVERTED_QTY, opt => opt.MapFrom(src => src.ConvertedQty))
                .ForMember(dest => dest.CONVERTED_UOM_ID, opt => opt.MapFrom(src => src.ConvertedUomId))
                .ForMember(dest => dest.CONVERTED_UOM_DESC, opt => opt.MapFrom(src => src.ConvertedUomDesc))
                ;

            Mapper.CreateMap<Lack1WorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
                .ForMember(dest => dest.FORM_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.COMMENT, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ROLE, opt => opt.MapFrom(src => src.UserRole))
                ;


            Mapper.CreateMap<Lack1DocumentDto, LACK1_DOCUMENT>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1SaveEditInput, Lack1GenerateDataParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Detail.Bukrs))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Detail.Butxt))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.Detail.PeriodMonth))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.Detail.PeriodYears))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.Detail.NppbkcId))
                .ForMember(dest => dest.ReceivedPlantId, opt => opt.MapFrom(src => src.Detail.LevelPlantId))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.Detail.SubmissionDate))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.Detail.SupplierPlantId))
                .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.Detail.ExGoodsType))
                .ForMember(dest => dest.ExcisableGoodsTypeDesc, opt => opt.MapFrom(src => src.Detail.ExGoodsTypeDesc))
                .ForMember(dest => dest.WasteAmount, opt => opt.MapFrom(src => src.Detail.WasteQty))
                .ForMember(dest => dest.WasteAmountUom, opt => opt.MapFrom(src => src.Detail.WasteUom))
                .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.Detail.ReturnQty))
                .ForMember(dest => dest.ReturnAmountUom, opt => opt.MapFrom(src => src.Detail.ReturnUom))
                .ForMember(dest => dest.Lack1Level, opt => opt.MapFrom(src => src.Detail.Lack1Level))
                .ForMember(dest => dest.Noted, opt => opt.MapFrom(src => src.Detail.Noted))
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.Detail.Lack1Id))
                ;

            Mapper.CreateMap<Lack1GeneratedInventoryAndProductionDto, Lack1InventoryAndProductionDto>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1GeneratedProductionDto, Lack1ProductionDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1GeneratedProductionSummaryByProdTypeDataDto, Lack1ProductionSummaryByProdTypeDto>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1GeneratedProductionDataDto, Lack1ProductionDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.AMOUNT, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProdCode))
                .ForMember(dest => dest.PRODUCT_TYPE, opt => opt.MapFrom(src => src.ProductType))
                .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProductAlias))
                .ForMember(dest => dest.UOM_ID, opt => opt.MapFrom(src => src.UomId))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.ORDR, opt => opt.MapFrom(src => src.Ordr))
                .ForMember(dest => dest.IS_TISTOTIS_DATA, opt => opt.MapFrom(src => src.IsTisToTisData))
                .ForMember(dest => dest.UOM_DESC, opt => opt.MapFrom(src => src.UomDesc))
                ;

            Mapper.CreateMap<Lack1GeneratedDto, Lack1DetailsDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Bukrs, opt => opt.MapFrom(src => src.CompanyCode))
               .ForMember(dest => dest.Butxt, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PeriodMonthId))
               .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.PeriodMonthName))
               .ForMember(dest => dest.Periode, opt => opt.MapFrom(src => new DateTime(src.PeriodYear, src.PeriodMonthId, 1)))
               .ForMember(dest => dest.PeriodYears, opt => opt.MapFrom(src => src.PeriodYear))
               .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SupplierPlantName))
               .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SupplierPlantId))
               .ForMember(dest => dest.SupplierPlantAddress, opt => opt.MapFrom(src => src.SupplierPlantAddress))
               .ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.ExcisableGoodsType))
               .ForMember(dest => dest.ExGoodsTypeDesc, opt => opt.MapFrom(src => src.ExcisableGoodsTypeDesc))
               .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
               .ForMember(dest => dest.BeginingBalance, opt => opt.MapFrom(src => src.BeginingBalance))
               .ForMember(dest => dest.CloseBalance, opt => opt.MapFrom(src => src.CloseBalance))
               .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src.TotalIncome))
               .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.TotalUsage))
               .ForMember(dest => dest.UsageTisToTis, opt => opt.MapFrom(src => src.TotalUsageTisToTis))
               .ForMember(dest => dest.Lack1UomId, opt => opt.MapFrom(src => src.Lack1UomId))
               .ForMember(dest => dest.Noted, opt => opt.MapFrom(src => src.Noted))
               .ForMember(dest => dest.DocumentNoted, opt => opt.MapFrom(src => src.DocumentNoted))
               ;

            Mapper.CreateMap<LACK1, Lack1SummaryReportDto>().IgnoreAllNonExisting()
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
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.SupplierCompanyName, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_NAME))
                .ForMember(dest => dest.SupplierCompanyCode, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY_CODE))
                .ForMember(dest => dest.SupplierPlantAddress, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_ADDRESS))
                .ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.WasteQty, opt => opt.MapFrom(src => src.WASTE_QTY))
                .ForMember(dest => dest.WasteUom, opt => opt.MapFrom(src => src.WASTE_UOM))
                .ForMember(dest => dest.ReturnQty, opt => opt.MapFrom(src => src.RETURN_QTY))
                .ForMember(dest => dest.ReturnUom, opt => opt.MapFrom(src => src.RETURN_UOM))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.LACK1_PBCK1_MAPPING.FirstOrDefault().PBCK1.NUMBER))
                .ForMember(dest => dest.Pbck1Date, opt => opt.MapFrom(src => src.LACK1_PBCK1_MAPPING.FirstOrDefault().PBCK1.DECREE_DATE))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CompletedDate, opt => opt.MapFrom(src => src.STATUS == Enums.DocumentStatus.Completed ? 
                    (src.MODIFIED_DATE.HasValue ? src.MODIFIED_DATE : src.DECREE_DATE) : null))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.ExTypDesc, opt => opt.MapFrom(src => src.EX_TYP_DESC))
                .ForMember(dest => dest.PerionNameEng, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.PeriodNameInd, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_IND))
                //.ForMember(dest => dest.Periode, opt => opt.MapFrom(src => new DateTime(src.PERIOD_YEAR.Value, src.PERIOD_MONTH.Value, 1)))
                .ForMember(dest => dest.BeginingBalance, opt => opt.MapFrom(src => src.BEGINING_BALANCE))
                .ForMember(dest => dest.TotalIncome, opt => opt.MapFrom(src => src.TOTAL_INCOME))
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.USAGE))
                .ForMember(dest => dest.UsageTisToTis, opt => opt.MapFrom(src => src.USAGE_TISTOTIS))
                .ForMember(dest => dest.Lack1UomId, opt => opt.MapFrom(src => src.LACK1_UOM_ID))
                .ForMember(dest => dest.Lack1UomName, opt => opt.MapFrom(src => src.UOM11 != null ? src.UOM11.UOM_DESC : string.Empty))
                ;

            Mapper.CreateMap<LACK1, Lack1DetailReportTempDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PeriodDate,
                    opt => opt.MapFrom(src => new DateTime(src.PERIOD_YEAR.Value, src.PERIOD_MONTH.Value, 1)));

            Mapper.CreateMap<Lack1GeneratedInvMovementProductionStepTracingItem, LACK1_TRACKING_ALCOHOL>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.MVT, opt => opt.MapFrom(src => src.Mvt))
                .ForMember(dest => dest.MATERIAL_ID, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.QTY, opt => opt.MapFrom(src => src.Qty))
                .ForMember(dest => dest.BUN, opt => opt.MapFrom(src => src.Bun))
                .ForMember(dest => dest.PURCH_DOC, opt => opt.MapFrom(src => src.PurchDoc))
                .ForMember(dest => dest.MAT_DOC, opt => opt.MapFrom(src => src.MatDoc))
                .ForMember(dest => dest.BATCH, opt => opt.MapFrom(src => src.Batch))
                .ForMember(dest => dest.ORDR, opt => opt.MapFrom(src => src.Ordr))
                .ForMember(dest => dest.PROD_QTY, opt => opt.MapFrom(src => src.ProductionQty))
                .ForMember(dest => dest.IS_FINAL_GOODS, opt => opt.MapFrom(src => src.IsFinalGoodsType))
                .ForMember(dest => dest.TrackLevel, opt => opt.MapFrom(src => src.TrackLevel))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.PARENT_ORDR, opt => opt.MapFrom(src => src.ParentOrdr))
                .ForMember(dest => dest.CONVERTED_QTY, opt => opt.MapFrom(src => src.ConvertedQty))
                .ForMember(dest => dest.CONVERTED_UOM_ID, opt => opt.MapFrom(src => src.ConvertedUomId))
                .ForMember(dest => dest.CONVERTED_UOM_DESC, opt => opt.MapFrom(src => src.ConvertedUomDesc))
                ;

            Mapper.CreateMap<INVENTORY_MOVEMENT, Lack1GeneratedInvMovementProductionStepTracingItem>()
                .ForMember(dest => dest.Mvt, opt => opt.MapFrom(src => src.MVT))
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.QTY.HasValue ? src.QTY.Value : 0))
                .ForMember(dest => dest.Bun, opt => opt.MapFrom(src => src.BUN))
                .ForMember(dest => dest.PurchDoc, opt => opt.MapFrom(src => src.PURCH_DOC))
                .ForMember(dest => dest.PostingDate, opt => opt.MapFrom(src => src.POSTING_DATE))
                .ForMember(dest => dest.MatDoc, opt => opt.MapFrom(src => src.MAT_DOC))
                .ForMember(dest => dest.Batch, opt => opt.MapFrom(src => src.BATCH))
                .ForMember(dest => dest.Ordr, opt => opt.MapFrom(src => src.ORDR))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1GeneratedRemarkDto, Lack1RemarkDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5WasteData, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.Ck5WasteData)))
                .ForMember(dest => dest.Ck5ReturnData, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.Ck5ReturnData)))
                .ForMember(dest => dest.Ck5TrialData, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.Ck5TrialData)))
                ;

            Mapper.CreateMap<Lack1GeneratedIncomeDataDto, Lack1IncomeDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CK5_ID, opt => opt.MapFrom(src => src.Ck5Id))
                .ForMember(dest => dest.AMOUNT, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CK5_TYPE, opt => opt.MapFrom(src => src.Ck5Type))
                .ForMember(dest => dest.REGISTRATION_NUMBER, opt => opt.MapFrom(src => src.RegistrationNumber))
                .ForMember(dest => dest.REGISTRATION_DATE, opt => opt.MapFrom(src => src.RegistrationDate))
                .ForMember(dest => dest.PACKAGE_UOM_ID, opt => opt.MapFrom(src => src.PackageUomId))
                .ForMember(dest => dest.PACKAGE_UOM_DESC, opt => opt.MapFrom(src => src.PackageUomDesc))
                .ForMember(dest => dest.PACKAGE_UOM_DESC, opt => opt.MapFrom(src => src.PackageUomDesc))
                .ForMember(dest => dest.FLAG_FOR_LACK1, opt => opt.MapFrom(src => src.FlagForLack1))
                ;


            Mapper.CreateMap<Lack1ReceivingDetailReportDto, Lack1TrackingConsolidationDetailReportDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<LACK1_PRODUCTION_DETAIL, Lack1ProductionBreakdownDetail>().IgnoreAllNonExisting();
            Mapper.CreateMap<ZAIDM_EX_MATERIAL_BALANCE,Lack1BeginingSaldoDetail>().IgnoreAllNonExisting();
            #region LACK1 Detail TIS

            Mapper.CreateMap<Lack1GetDetailTisByParamInput, Ck5GetForLack1DetailTis>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5, Lack1DetailTisDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantIdReceiver, opt => opt.MapFrom(src => src.DEST_PLANT_ID))
                .ForMember(dest => dest.PlantDescReceiver, opt => opt.MapFrom(src => src.DEST_PLANT_NAME))
                .ForMember(dest => dest.PlantIdSupplier, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID))
                .ForMember(dest => dest.PlantDescSupplier, opt => opt.MapFrom(src => src.SOURCE_PLANT_NAME))
                .ForMember(dest => dest.Ck5EmsNo, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.Ck5RegNo, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.Ck5RegDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.Ck5GrDate, opt => opt.MapFrom(src => src.GR_DATE.HasValue ? src.GR_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.Ck5Qty, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX.Value))
                .ForMember(dest => dest.StoReceiverNumber, opt => opt.MapFrom(src => src.STO_RECEIVER_NUMBER))
                ;

            #endregion
            Mapper.CreateMap<ZAAP_SHIFT_RPT, Lack1CFUsagevsFaDetailDtoMvt>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Batch, opt => opt.MapFrom(src => src.BATCH))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.ORDR))
                .ForMember(dest => dest.Converted_Qty, opt => opt.MapFrom(src => src.QTY))
                .ForMember(dest => dest.Converted_Uom, opt => opt.MapFrom(src => src.UOM))
                .ForMember(dest => dest.Material_Id, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.Mvt, opt => opt.MapFrom(src => src.MVT))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PostingDate, opt => opt.MapFrom(src => src.POSTING_DATE))
                .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.PRODUCTION_DATE))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.ORIGINAL_QTY))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.ORIGINAL_UOM));

            Mapper.CreateMap<INVENTORY_MOVEMENT, Lack1CFUsagevsFaDetailDtoMvt>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Batch, opt => opt.MapFrom(src => src.BATCH))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.ORDR))
                
                
                .ForMember(dest => dest.Material_Id, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.Mvt, opt => opt.MapFrom(src => src.MVT))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PostingDate, opt => opt.MapFrom(src => src.POSTING_DATE))
                .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.POSTING_DATE))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.QTY))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.BUN));

            Mapper.CreateMap<InvMovementItemWithConvertion, Lack1CFUsagevsFaDetailDtoMvt>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Batch, opt => opt.MapFrom(src => src.BATCH))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.ORDR))


                .ForMember(dest => dest.Material_Id, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.Mvt, opt => opt.MapFrom(src => src.MVT))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PostingDate, opt => opt.MapFrom(src => src.POSTING_DATE.HasValue ? src.POSTING_DATE.Value : new DateTime()))
                .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.POSTING_DATE.HasValue ? src.POSTING_DATE.Value : new DateTime()))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.ConvertedQty))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.ConvertedUomId));

            Mapper.CreateMap<LACK1_PERIOD_SUMMARY, PeriodSummary>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Income, opt => opt.MapFrom(src => src.INCOME))
                .ForMember(dest => dest.Laboratorium, opt => opt.MapFrom(src => src.LABORATORIUM))
                .ForMember(dest => dest.Return, opt => opt.MapFrom(src => src.CK5_RETURN))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.SALDO))
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.USAGE))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TYPE));

            Mapper.CreateMap<PeriodSummary, LACK1_PERIOD_SUMMARY>().IgnoreAllNonExisting()
                .ForMember(dest => dest.INCOME, opt => opt.MapFrom(src => src.Income))
                .ForMember(dest => dest.LABORATORIUM, opt => opt.MapFrom(src => src.Laboratorium))
                .ForMember(dest => dest.CK5_RETURN, opt => opt.MapFrom(src => src.Return))
                .ForMember(dest => dest.SALDO, opt => opt.MapFrom(src => src.Saldo))
                .ForMember(dest => dest.USAGE, opt => opt.MapFrom(src => src.Usage))
                .ForMember(dest => dest.TYPE, opt => opt.MapFrom(src => src.Type));

            Mapper.CreateMap<LACK1_CALCULATION_DETAIL, Lack1CalculationDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.Ordr, opt => opt.MapFrom(src => src.ORDR))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.Proportional, opt => opt.MapFrom(src => src.PROPORTIONAL))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TYPE))
                .ForMember(dest => dest.UomProduction, opt => opt.MapFrom(src => src.UOM_PRODUCTION))
                .ForMember(dest => dest.UomUsage, opt => opt.MapFrom(src => src.UOM_USAGE))
                .ForMember(dest => dest.AmountProduction, opt => opt.MapFrom(src => src.AMOUNT_PRODUCTION))
                .ForMember(dest => dest.AmountUsage, opt => opt.MapFrom(src => src.AMOUNT_USAGE))
                .ForMember(dest => dest.BrandCe, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.Convertion, opt => opt.MapFrom(src => src.CONVERTION))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE));

            Mapper.CreateMap<Lack1CalculationDetail, LACK1_CALCULATION_DETAIL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MATERIAL_ID, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.ORDR, opt => opt.MapFrom(src => src.Ordr))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PROPORTIONAL, opt => opt.MapFrom(src => src.Proportional))
                .ForMember(dest => dest.TYPE, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.UOM_PRODUCTION, opt => opt.MapFrom(src => src.UomProduction))
                .ForMember(dest => dest.UOM_USAGE, opt => opt.MapFrom(src => src.UomUsage))
                .ForMember(dest => dest.AMOUNT_PRODUCTION, opt => opt.MapFrom(src => src.AmountProduction))
                .ForMember(dest => dest.AMOUNT_USAGE, opt => opt.MapFrom(src => src.AmountUsage))
                .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.BrandCe))
                .ForMember(dest => dest.CONVERTION, opt => opt.MapFrom(src => src.Convertion))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode));

            #region LACK1 Detail EA

            Mapper.CreateMap<Lack1GetDetailEaByParamInput, Ck5GetForLack1DetailEa>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5, Lack1DetailEaDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantIdReceiver, opt => opt.MapFrom(src => src.DEST_PLANT_ID))
                .ForMember(dest => dest.PlantDescReceiver, opt => opt.MapFrom(src => src.DEST_PLANT_NAME))
                .ForMember(dest => dest.PlantIdSupplier, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID))
                .ForMember(dest => dest.PlantDescSupplier, opt => opt.MapFrom(src => src.SOURCE_PLANT_NAME))
                .ForMember(dest => dest.Ck5EmsNo, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.Ck5RegNo, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.Ck5RegDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.Ck5Qty, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX.Value))
                .ForMember(dest => dest.DnNumber, opt => opt.MapFrom(src => src.DN_NUMBER))
                ;

            Mapper.CreateMap<InventoryMovementLevelDto, Lack1DetailLevelDto>().IgnoreAllNonExisting();

            #endregion
        }

    }
}
