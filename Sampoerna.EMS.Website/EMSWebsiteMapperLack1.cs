using System;
using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.LACK1;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeLACK1()
        {

            #region Lack1

            Mapper.CreateMap<Lack1Dto, Lack1NppbkcData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.TobaccoGoodType, opt => opt.MapFrom(src => src.ExGoodsType + "-" + src.ExTypDesc))
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.SupplierPlant))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.Nppbkc, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.PerionNameEng + ' ' + src.PeriodYears));

            Mapper.CreateMap<Lack1IndexViewModel, Lack1GetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbKcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            Mapper.CreateMap<Lack1Input, Lack1GetByParamInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1Dto, Lack1PlantData>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Butxt))
               .ForMember(dest => dest.TobaccoGoodType, opt => opt.MapFrom(src => src.ExGoodsType + "-" + src.ExTypDesc))
               .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.SupplierPlant))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.LevelPlantId))
               .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.PerionNameEng + ' ' + src.PeriodYears));

            Mapper.CreateMap<Lack1IndexPlantViewModel, Lack1GetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbKcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            Mapper.CreateMap<Lack1Dto, Lack1CompletedDocumentData>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Butxt))
               .ForMember(dest => dest.TobaccoGoodType, opt => opt.MapFrom(src => src.ExGoodsType + "-" + src.ExTypDesc))
               .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.SupplierPlant))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
               .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.LevelPlantId))
               .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.PerionNameEng + ' ' + src.PeriodYears));

            #endregion

            Mapper.CreateMap<Lack1GenerateInputModel, Lack1GenerateDataParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1Level, opt => opt.MapFrom(src => (Enums.Lack1Level)src.Lack1Level));
            Mapper.CreateMap<Lack1GeneratedDto, Lack1GeneratedItemModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1CreateViewModel, Lack1CreateParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Bukrs))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.PeriodMonth, opt => opt.MapFrom(src => src.PeriodMonth))
                .ForMember(dest => dest.PeriodYear, opt => opt.MapFrom(src => src.PeriodYears))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReceivedPlantId, opt => opt.MapFrom(src => src.LevelPlantId))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SupplierPlantId))
                .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.ExGoodsTypeId))
                .ForMember(dest => dest.ExcisableGoodsTypeDesc, opt => opt.MapFrom(src => src.ExGoodsTypeDesc))
                .ForMember(dest => dest.WasteAmount, opt => opt.MapFrom(src => src.WasteQty))
                .ForMember(dest => dest.WasteAmountUom, opt => opt.MapFrom(src => src.WasteUom))
                .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.ReturnQty))
                .ForMember(dest => dest.ReturnAmountUom, opt => opt.MapFrom(src => src.ReturnUom))
                .ForMember(dest => dest.Lack1Level, opt => opt.MapFrom(src => src.Lack1Level))
                .ForMember(dest => dest.Noted, opt => opt.MapFrom(src => src.Noted))
                ;

            Mapper.CreateMap<Lack1DocumentDto, Lack1DocumentItemModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1IncomeDetailDto, Lack1IncomeDetailItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1IncomeDetailId, opt => opt.MapFrom(src => src.LACK1_INCOME_ID))
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE))
                .ForMember(dest => dest.StringRegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy") : string.Empty))
                ;
            Mapper.CreateMap<Lack1Pbck1MappingDto, Lack1Pbck1MappingItemModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<Lack1PlantDto, Lack1PlantItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1PlantId, opt => opt.MapFrom(src => src.LACK1_PLANT_ID))
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.Name1, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.PLANT_ADDRESS))
                ;

            Mapper.CreateMap<Lack1ProductionDetailDto, Lack1ProductionDetailItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1ProductionDetailId, opt => opt.MapFrom(src => src.LACK1_PRODUCTION_ID))
                .ForMember(dest => dest.Lack1Id, opt => opt.MapFrom(src => src.LACK1_ID))
                .ForMember(dest => dest.ProdCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.UOM_ID))
                .ForMember(dest => dest.UomDesc, opt => opt.MapFrom(src => src.UOM_DESC))
                ;

            Mapper.CreateMap<Lack1DetailsDto, Lack1ItemViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DisplayLevelPlantName, opt => opt.MapFrom(src => src.LevelPlantId + '-' + src.LevelPlantName))
                .ForMember(dest => dest.ExGoodsTypeId, opt => opt.MapFrom(src => src.ExGoodsType))
                .ForMember(dest => dest.DisplaySupplierPlant, opt => opt.MapFrom(src => src.SupplierPlantId + '-' + src.SupplierPlant))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.GovStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GovStatus)))
                .ForMember(dest => dest.EndingBalance, opt => opt.MapFrom(src => (src.BeginingBalance - src.Usage + src.TotalIncome)))
                .ForMember(dest => dest.TotalUsage, opt => opt.MapFrom(src => src.Usage))
                .ForMember(dest => dest.Lack1Document, opt => opt.MapFrom(src => Mapper.Map<List<Lack1DocumentItemModel>>(src.Lack1Document)))
                .ForMember(dest => dest.IncomeList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailItemModel>>(src.Lack1IncomeDetail)))
                .ForMember(dest => dest.Lack1Pbck1Mapping, opt => opt.MapFrom(src => Mapper.Map<List<Lack1Pbck1MappingItemModel>>(src.Lack1Pbck1Mapping)))
                .ForMember(dest => dest.Lack1Plant, opt => opt.MapFrom(src => Mapper.Map<List<Lack1PlantItemModel>>(src.Lack1Plant)))
                .ForMember(dest => dest.ProductionList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1ProductionDetailItemModel>>(src.Lack1ProductionDetail)))
                ;

            Mapper.CreateMap<Lack1DetailsDto, Lack1EditViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DisplayLevelPlantName, opt => opt.MapFrom(src => src.LevelPlantId + '-' + src.LevelPlantName))
                .ForMember(dest => dest.ExGoodsTypeId, opt => opt.MapFrom(src => src.ExGoodsType))
                .ForMember(dest => dest.DisplaySupplierPlant, opt => opt.MapFrom(src => src.SupplierPlantId + '-' + src.SupplierPlant))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.GovStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GovStatus)))
                .ForMember(dest => dest.EndingBalance, opt => opt.MapFrom(src => (src.BeginingBalance - src.Usage + src.TotalIncome)))
                .ForMember(dest => dest.TotalUsage, opt => opt.MapFrom(src => src.Usage))
                .ForMember(dest => dest.Lack1Document, opt => opt.MapFrom(src => Mapper.Map<List<Lack1DocumentItemModel>>(src.Lack1Document)))
                .ForMember(dest => dest.IncomeList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailItemModel>>(src.Lack1IncomeDetail)))
                .ForMember(dest => dest.ProductionList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1ProductionDetailItemModel>>(src.Lack1ProductionDetail)))
                ;

            Mapper.CreateMap<HEADER_FOOTER_MAPDto, Lack1HeaderFooter>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1PrintOutDto, Lack1PrintOutModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.GovStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GovStatus)))
                .ForMember(dest => dest.EndingBalance, opt => opt.MapFrom(src => (src.BeginingBalance - src.Usage + src.TotalIncome)))
                .ForMember(dest => dest.TotalUsage, opt => opt.MapFrom(src => src.Usage))
                .ForMember(dest => dest.Lack1Document, opt => opt.MapFrom(src => Mapper.Map<List<Lack1DocumentItemModel>>(src.Lack1Document)))
                .ForMember(dest => dest.IncomeList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailItemModel>>(src.Lack1IncomeDetail)))
                .ForMember(dest => dest.Lack1Pbck1Mapping, opt => opt.MapFrom(src => Mapper.Map<List<Lack1Pbck1MappingItemModel>>(src.Lack1Pbck1Mapping)))
                .ForMember(dest => dest.Lack1Plant, opt => opt.MapFrom(src => Mapper.Map<List<Lack1PlantItemModel>>(src.Lack1Plant)))
                .ForMember(dest => dest.ProductionList, opt => opt.MapFrom(src => Mapper.Map<List<Lack1ProductionDetailItemModel>>(src.Lack1ProductionDetail)))
                .ForMember(dest => dest.HeaderFooter, opt => opt.MapFrom(src => Mapper.Map<Lack1HeaderFooter>(src.HeaderFooter)))
                ;

            Mapper.CreateMap<Lack1DocumentItemModel, Lack1DocumentDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1Pbck1MappingItemModel, Lack1Pbck1MappingDto>().IgnoreAllNonExisting();


            Mapper.CreateMap<Lack1IncomeDetailItemModel, Lack1IncomeDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK1_INCOME_ID, opt => opt.MapFrom(src => src.Lack1IncomeDetailId))
                .ForMember(dest => dest.LACK1_ID, opt => opt.MapFrom(src => src.Lack1Id))
                .ForMember(dest => dest.CK5_ID, opt => opt.MapFrom(src => src.Ck5Id))
                .ForMember(dest => dest.AMOUNT, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.REGISTRATION_NUMBER, opt => opt.MapFrom(src => src.RegistrationNumber))
                .ForMember(dest => dest.REGISTRATION_DATE, opt => opt.MapFrom(src => src.RegistrationDate))
                ;

            Mapper.CreateMap<Lack1PlantItemModel, Lack1PlantDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK1_PLANT_ID, opt => opt.MapFrom(src => src.Lack1PlantId))
                .ForMember(dest => dest.LACK1_ID, opt => opt.MapFrom(src => src.Lack1Id))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.Werks))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.Name1))
                .ForMember(dest => dest.PLANT_ADDRESS, opt => opt.MapFrom(src => src.Address))
                ;

            Mapper.CreateMap<Lack1ProductionDetailItemModel, Lack1ProductionDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LACK1_PRODUCTION_ID, opt => opt.MapFrom(src => src.Lack1ProductionDetailId))
                .ForMember(dest => dest.LACK1_ID, opt => opt.MapFrom(src => src.Lack1Id))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProdCode))
                .ForMember(dest => dest.PRODUCT_TYPE, opt => opt.MapFrom(src => src.ProductType))
                .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProductAlias))
                .ForMember(dest => dest.AMOUNT, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.UOM_ID, opt => opt.MapFrom(src => src.UomId))
                .ForMember(dest => dest.UOM_DESC, opt => opt.MapFrom(src => src.UomDesc))
                ;

            Mapper.CreateMap<Lack1EditViewModel, Lack1DetailsDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => src.TotalUsage))
                .ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.ExGoodsTypeId))
                .ForMember(dest => dest.ExGoodsTypeDesc, opt => opt.MapFrom(src => src.ExGoodsTypeDesc))
                .ForMember(dest => dest.Lack1Document,
                    opt => opt.MapFrom(src => Mapper.Map<List<Lack1DocumentDto>>(src.Lack1Document)))
                .ForMember(dest => dest.Lack1IncomeDetail,
                    opt => opt.MapFrom(src => Mapper.Map<List<Lack1IncomeDetailDto>>(src.IncomeList)))
                .ForMember(dest => dest.Lack1ProductionDetail,
                    opt =>
                        opt.MapFrom(src => Mapper.Map<List<Lack1ProductionDetailDto>>(src.ProductionList)));


            #region ----------- Summary Report -----------

            Mapper.CreateMap<Lack1SearchSummaryReportViewModel, Lack1GetSummaryReportByParamInput>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1SummaryReportDto, Lack1SummaryReportItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1Number, opt => opt.MapFrom(src => src.Lack1Number))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Bukrs))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReceivingPlantId, opt => opt.MapFrom(src => src.LevelPlantId))
                .ForMember(dest => dest.ReceivingPlantName, opt => opt.MapFrom(src => src.LevelPlantName))
                .ForMember(dest => dest.ExcisableGoodsTypeId, opt => opt.MapFrom(src => src.ExGoodsType))
                .ForMember(dest => dest.ExcisableGoodsTypeDesc, opt => opt.MapFrom(src => src.ExTypDesc))
                .ForMember(dest => dest.SupplierPlantId, opt => opt.MapFrom(src => src.SupplierPlantId))
                .ForMember(dest => dest.SupplierPlantName, opt => opt.MapFrom(src => src.SupplierPlant))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.PerionNameEng + " " + src.PeriodYears))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreateDate.ToString("dd MMM yyyy HH:mm:ss")))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.ApprovedDate.HasValue ? src.ApprovedDate.Value.ToString("dd MMM yyyy HH:mm:ss") : string.Empty))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreateBy))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.CreateBy))
                .ForMember(dest => dest.Approver, opt => opt.MapFrom(src => src.ApprovedBy))
                ;

            Mapper.CreateMap<Lack1SummaryReportItemModel, Lack1ExportSummaryDataModel>().IgnoreAllNonExisting();

            #endregion


            #region ----------------- Detail Report ---------

            Mapper.CreateMap<Lack1TrackingConsolidationDetailReportDto, Lack1TrackingConsolidationDetailReportItemModel>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5RegistrationDate, opt => opt.MapFrom(src => src.Ck5RegistrationDate.HasValue ? src.Ck5RegistrationDate.Value.ToString("dd-MMM-yyyy") : string.Empty))
                .ForMember(dest => dest.Ck5GrDate, opt => opt.MapFrom(src => src.Ck5GrDate.HasValue ? src.Ck5GrDate.Value.ToString("dd-MMM-yyyy") : string.Empty))
                .ForMember(dest => dest.GiDate, opt => opt.MapFrom(src => src.GiDate.HasValue ? src.GiDate.Value.ToString("dd-MMM-yyyy") : string.Empty))
                ;

            Mapper.CreateMap<Lack1SearchDetailReportViewModel, Lack1GetDetailReportByParamInput>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Lack1DetailReportDto, Lack1DetailReportItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack1LevelName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Lack1Level)))
                .ForMember(dest => dest.TrackingConsolidations, opt => opt.MapFrom(src
                    => Mapper.Map<List<Lack1TrackingConsolidationDetailReportItemModel>>(src.TrackingConsolidations)))
                ;
            
            #endregion

        }
    }
}