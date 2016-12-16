﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void InitializePBCK1()
        {
            #region PBCK1

            Mapper.CreateMap<PBCK1_PROD_CONVERTER, Pbck1ProdConverterDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdConvId, opt => opt.MapFrom(src => src.PBCK1_PROD_COV_ID))
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.ConverterOutput, opt => opt.MapFrom(src => src.CONVERTER_OUTPUT))
                .ForMember(dest => dest.BrandCE, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.ConverterOutputUomId, opt => opt.MapFrom(src => src.CONVERTER_UOM_ID))
                .ForMember(dest => dest.ConverterOutputUomName, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Range, opt => opt.MapFrom(src => src.RANGE_QTY))
                ;

            Mapper.CreateMap<Pbck1ProdConverterDto, PBCK1_PROD_CONVERTER>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK1_PROD_COV_ID, opt => opt.MapFrom(src => src.Pbck1ProdConvId))
                .ForMember(dest => dest.PBCK1_ID, opt => opt.MapFrom(src => src.Pbck1Id))
                .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.BrandCE))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.PRODUCT_TYPE, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProdAlias))
                .ForMember(dest => dest.CONVERTER_OUTPUT, opt => opt.MapFrom(src => src.ConverterOutput))
                .ForMember(dest => dest.CONVERTER_UOM_ID, opt => opt.MapFrom(src => src.ConverterOutputUomId))
                .ForMember(dest => dest.RANGE_QTY, opt => opt.MapFrom(src => src.Range));

            Mapper.CreateMap<PBCK1_PROD_PLAN, Pbck1ProdPlanDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdPlanId, opt => opt.MapFrom(src => src.PBCK1_PROD_PLAN_ID))
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.BkcRequired, opt => opt.MapFrom(src => src.BKC_REQUIRED))
                .ForMember(dest => dest.BkcRequiredUomId, opt => opt.MapFrom(src => src.BKC_REQUIRED_UOM_ID))
                .ForMember(dest => dest.BkcRequiredUomName, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.MonthId, opt => opt.MapFrom(src => src.MONTH))
                .ForMember(dest => dest.MonthName, opt => opt.MapFrom(src => src.MONTH1 != null ? src.MONTH1.MONTH_NAME_ENG : string.Empty))

                ;
            Mapper.CreateMap<Pbck1ProdPlanDto, PBCK1_PROD_PLAN>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK1_PROD_PLAN_ID, opt => opt.MapFrom(src => src.Pbck1ProdPlanId))
                .ForMember(dest => dest.PBCK1_ID, opt => opt.MapFrom(src => src.Pbck1Id))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.PRODUCT_TYPE, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProdAlias))
                .ForMember(dest => dest.AMOUNT, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.BKC_REQUIRED, opt => opt.MapFrom(src => src.BkcRequired))
                .ForMember(dest => dest.BKC_REQUIRED_UOM_ID, opt => opt.MapFrom(src => src.BkcRequiredUomId))
                .ForMember(dest => dest.MONTH, opt => opt.MapFrom(src => src.MonthId))
                ;

            Mapper.CreateMap<PBCK1_DECREE_DOC, Pbck1DecreeDocDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1DecreeDocDto, PBCK1_DECREE_DOC>().IgnoreAllNonExisting();

            Mapper.CreateMap<PBCK1, Pbck1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.NUMBER))
                .ForMember(dest => dest.Pbck1Reference, opt => opt.MapFrom(src => src.PBCK1_REF))
                .ForMember(dest => dest.Pbck1Type, opt => opt.MapFrom(src => src.PBCK1_TYPE))
                .ForMember(dest => dest.PeriodFrom, opt => opt.MapFrom(src => src.PERIOD_FROM.Value))
                .ForMember(dest => dest.PeriodTo, opt => opt.MapFrom(src => src.PERIOD_TO))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.REPORTED_ON))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.NppbkcKppbcId, opt => opt.MapFrom(src => src.NPPBKC_KPPBC_ID))
                .ForMember(dest => dest.NppbkcCompanyName, opt => opt.MapFrom(src => src.NPPBCK_BUTXT))
                .ForMember(dest => dest.NppbkcCompanyCode, opt => opt.MapFrom(src => src.NPPBKC_BUKRS))
                .ForMember(dest => dest.GoodType, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeDesc, opt => opt.MapFrom(src => src.EXC_TYP_DESC))
                .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.SupplierPlantWerks, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.SupplierPortId, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID))
                .ForMember(dest => dest.SupplierPortName, opt => opt.MapFrom(src => src.SUPPLIER_PORT_NAME))
                .ForMember(dest => dest.SupplierAddress, opt => opt.MapFrom(src => src.SUPPLIER_ADDRESS))
                .ForMember(dest => dest.SupplierPhone, opt => opt.MapFrom(src => src.SUPPLIER_PHONE))
                .ForMember(dest => dest.SupplierNppbkcId, opt => opt.MapFrom(src => src.SUPPLIER_NPPBKC_ID))
                .ForMember(dest => dest.SupplierKppbcId, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_ID))
                .ForMember(dest => dest.SupplierKppbcName, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_NAME))
                .ForMember(dest => dest.SupplierCompany, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY))
                .ForMember(dest => dest.IsNppbkcImport, opt => opt.MapFrom(src => src.IS_NPPBKC_IMPORT))
                .ForMember(dest => dest.IsDisplayRange, opt => opt.MapFrom(src => src.IS_DISPLAY_RANGE))
                .ForMember(dest => dest.PlanProdFrom, opt => opt.MapFrom(src => src.PLAN_PROD_FROM))
                .ForMember(dest => dest.PlanProdTo, opt => opt.MapFrom(src => src.PLAN_PROD_TO))
                .ForMember(dest => dest.RequestQty, opt => opt.MapFrom(src => src.REQUEST_QTY))
                .ForMember(dest => dest.RequestQtyUomId, opt => opt.MapFrom(src => src.REQUEST_QTY_UOM))
                .ForMember(dest => dest.RequestQtyUomName, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.Lack1FromMonthId, opt => opt.MapFrom(src => src.LACK1_FROM_MONTH))
                .ForMember(dest => dest.Lack1FromMonthName, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.Lack1FormYear, opt => opt.MapFrom(src => src.LACK1_FROM_YEAR))
                .ForMember(dest => dest.Lack1ToMonthId, opt => opt.MapFrom(src => src.LACK1_TO_MONTH))
                .ForMember(dest => dest.Lack1ToMonthName, opt => opt.MapFrom(src => src.MONTH1.MONTH_NAME_ENG))
                .ForMember(dest => dest.Lack1ToYear, opt => opt.MapFrom(src => src.LACK1_TO_YEAR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.StatusGov, opt => opt.MapFrom(src => src.STATUS_GOV))
                .ForMember(dest => dest.QtyApproved, opt => opt.MapFrom(src => src.QTY_APPROVED))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedByPoaId, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedByPoaDate, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.ApprovedByManagerId, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.ApprovedByManagerDate, opt => opt.MapFrom(src => src.APPROVED_DATE_MANAGER))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.LatestSaldo, opt => opt.MapFrom(src => src.LATEST_SALDO))
                .ForMember(dest => dest.LatestSaldoUomId, opt => opt.MapFrom(src => src.LATEST_SALDO_UOM))
                .ForMember(dest => dest.LatestSaldoUomName, opt => opt.MapFrom(src => src.UOM1 != null ? src.UOM1.UOM_DESC : string.Empty))
                .ForMember(dest => dest.Pbck1ProdConverter, opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConverterDto>>(src.PBCK1_PROD_CONVERTER)))
                .ForMember(dest => dest.Pbck1ProdPlan, opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlanDto>>(src.PBCK1_PROD_PLAN)))
                .ForMember(dest => dest.Pbck1DecreeDoc, opt => opt.MapFrom(src => Mapper.Map<List<Pbck1DecreeDocDto>>(src.PBCK1_DECREE_DOC)))
                ;

            Mapper.CreateMap<Pbck1Dto, PBCK1>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK1_ID, opt => opt.MapFrom(src => src.Pbck1Id))
                .ForMember(dest => dest.NUMBER, opt => opt.MapFrom(src => src.Pbck1Number))
                .ForMember(dest => dest.PBCK1_REF, opt => opt.MapFrom(src => src.Pbck1Reference))
                .ForMember(dest => dest.PBCK1_TYPE, opt => opt.MapFrom(src => src.Pbck1Type))
                .ForMember(dest => dest.PERIOD_FROM, opt => opt.MapFrom(src => src.PeriodFrom))
                .ForMember(dest => dest.PERIOD_TO, opt => opt.MapFrom(src => src.PeriodTo))
                .ForMember(dest => dest.REPORTED_ON, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.NPPBKC_KPPBC_ID, opt => opt.MapFrom(src => src.NppbkcKppbcId))
                .ForMember(dest => dest.NPPBKC_BUKRS, opt => opt.MapFrom(src => src.NppbkcCompanyCode))
                .ForMember(dest => dest.NPPBCK_BUTXT, opt => opt.MapFrom(src => src.NppbkcCompanyName))
                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodType))
                .ForMember(dest => dest.EXC_TYP_DESC, opt => opt.MapFrom(src => src.GoodTypeDesc))
                .ForMember(dest => dest.SUPPLIER_PLANT, opt => opt.MapFrom(src => src.SupplierPlant))
                .ForMember(dest => dest.SUPPLIER_PORT_ID, opt => opt.MapFrom(src => src.SupplierPortId))
                .ForMember(dest => dest.SUPPLIER_ADDRESS, opt => opt.MapFrom(src => src.SupplierAddress))
                .ForMember(dest => dest.SUPPLIER_PHONE, opt => opt.MapFrom(src => src.SupplierPhone))
                .ForMember(dest => dest.PLAN_PROD_FROM, opt => opt.MapFrom(src => src.PlanProdFrom))
                .ForMember(dest => dest.PLAN_PROD_TO, opt => opt.MapFrom(src => src.PlanProdTo))
                .ForMember(dest => dest.REQUEST_QTY, opt => opt.MapFrom(src => src.RequestQty))
                .ForMember(dest => dest.REQUEST_QTY_UOM, opt => opt.MapFrom(src => src.RequestQtyUomId))
                .ForMember(dest => dest.LACK1_FROM_MONTH, opt => opt.MapFrom(src => src.Lack1FromMonthId))
                .ForMember(dest => dest.LACK1_FROM_YEAR, opt => opt.MapFrom(src => src.Lack1FormYear))
                .ForMember(dest => dest.LACK1_TO_MONTH, opt => opt.MapFrom(src => src.Lack1ToMonthId))
                .ForMember(dest => dest.LACK1_TO_YEAR, opt => opt.MapFrom(src => src.Lack1ToYear))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.STATUS_GOV, opt => opt.MapFrom(src => src.StatusGov))
                .ForMember(dest => dest.QTY_APPROVED, opt => opt.MapFrom(src => src.QtyApproved))
                .ForMember(dest => dest.DECREE_DATE, opt => opt.MapFrom(src => src.DecreeDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.APPROVED_BY_POA, opt => opt.MapFrom(src => src.ApprovedByPoaId))
                .ForMember(dest => dest.APPROVED_DATE_POA, opt => opt.MapFrom(src => src.ApprovedByPoaDate))
                .ForMember(dest => dest.APPROVED_BY_MANAGER, opt => opt.MapFrom(src => src.ApprovedByManagerId))
                .ForMember(dest => dest.APPROVED_DATE_MANAGER, opt => opt.MapFrom(src => src.ApprovedByManagerDate))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.LATEST_SALDO, opt => opt.MapFrom(src => src.LatestSaldo))
                .ForMember(dest => dest.LATEST_SALDO_UOM, opt => opt.MapFrom(src => src.LatestSaldoUomId))
                .ForMember(dest => dest.SUPPLIER_PORT_NAME, opt => opt.MapFrom(src => src.SupplierPortName))
                .ForMember(dest => dest.SUPPLIER_NPPBKC_ID, opt => opt.MapFrom(src => src.SupplierNppbkcId))
                .ForMember(dest => dest.SUPPLIER_KPPBC_ID, opt => opt.MapFrom(src => src.SupplierKppbcId))
                .ForMember(dest => dest.SUPPLIER_KPPBC_NAME, opt => opt.MapFrom(src => src.SupplierKppbcName))
                .ForMember(dest => dest.SUPPLIER_COMPANY, opt => opt.MapFrom(src => src.SupplierCompany))
                .ForMember(dest => dest.IS_NPPBKC_IMPORT, opt => opt.MapFrom(src => src.IsNppbkcImport))
                .ForMember(dest => dest.IS_DISPLAY_RANGE, opt => opt.MapFrom(src => src.IsDisplayRange))
                .ForMember(dest => dest.SUPPLIER_PLANT_WERKS, opt => opt.MapFrom(src => src.SupplierPlantWerks))
                .ForMember(dest => dest.PBCK1_PROD_CONVERTER, opt => opt.MapFrom(src => Mapper.Map<List<PBCK1_PROD_CONVERTER>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.PBCK1_PROD_PLAN, opt => opt.MapFrom(src => Mapper.Map<List<PBCK1_PROD_PLAN>>(src.Pbck1ProdPlan)))
                .ForMember(dest => dest.PBCK1_DECREE_DOC, opt => opt.MapFrom(src => Mapper.Map<List<PBCK1_DECREE_DOC>>(src.Pbck1DecreeDoc)))
                ;

            Mapper.CreateMap<Pbck1WorkflowDocumentInput, WorkflowHistoryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
                .ForMember(dest => dest.FORM_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.COMMENT, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ROLE, opt => opt.MapFrom(src => src.UserRole))
                ;

            #endregion

            #region Pbck1ProdConv

            Mapper.CreateMap<Pbck1ProdConverterInput, Pbck1ProdConverterOutput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdPlanInput, Pbck1ProdPlanOutput>().IgnoreAllNonExisting();

            #endregion

            #region Summary Report

            Mapper.CreateMap<PBCK1, Pbck1SummaryReportDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.NUMBER))
                .ForMember(dest => dest.Pbck1Reference, opt => opt.MapFrom(src => src.PBCK1_REF))
                .ForMember(dest => dest.Pbck1Type, opt => opt.MapFrom(src => src.PBCK1_TYPE))
                .ForMember(dest => dest.PeriodFrom, opt => opt.MapFrom(src => src.PERIOD_FROM.Value))
                .ForMember(dest => dest.PeriodTo, opt => opt.MapFrom(src => src.PERIOD_TO))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.REPORTED_ON))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.NppbkcKppbcId, opt => opt.MapFrom(src => src.NPPBKC_KPPBC_ID))
                .ForMember(dest => dest.NppbkcCompanyName, opt => opt.MapFrom(src => src.NPPBCK_BUTXT))
                .ForMember(dest => dest.NppbkcCompanyCode, opt => opt.MapFrom(src => src.NPPBKC_BUKRS))
                .ForMember(dest => dest.GoodType, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeDesc, opt => opt.MapFrom(src => src.EXC_TYP_DESC))
                .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.SupplierPlantWerks, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.SupplierPortId, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID))
                .ForMember(dest => dest.SupplierPortName, opt => opt.MapFrom(src => src.SUPPLIER_PORT_NAME))
                .ForMember(dest => dest.SupplierAddress, opt => opt.MapFrom(src => src.SUPPLIER_ADDRESS))
                .ForMember(dest => dest.SupplierPhone, opt => opt.MapFrom(src => src.SUPPLIER_PHONE))
                .ForMember(dest => dest.SupplierNppbkcId, opt => opt.MapFrom(src => src.SUPPLIER_NPPBKC_ID))
                .ForMember(dest => dest.SupplierKppbcId, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_ID))
                .ForMember(dest => dest.SupplierKppbcName, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_NAME))
                .ForMember(dest => dest.SupplierCompany, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY))
                .ForMember(dest => dest.IsNppbkcImport, opt => opt.MapFrom(src => src.IS_NPPBKC_IMPORT))
                .ForMember(dest => dest.PlanProdFrom, opt => opt.MapFrom(src => src.PLAN_PROD_FROM))
                .ForMember(dest => dest.PlanProdTo, opt => opt.MapFrom(src => src.PLAN_PROD_TO))
                .ForMember(dest => dest.RequestQty, opt => opt.MapFrom(src => src.REQUEST_QTY))
                .ForMember(dest => dest.RequestQtyUomId, opt => opt.MapFrom(src => src.REQUEST_QTY_UOM))
                .ForMember(dest => dest.RequestQtyUomName,
                    opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.Lack1FromMonthId, opt => opt.MapFrom(src => src.LACK1_FROM_MONTH))
                .ForMember(dest => dest.Lack1FromMonthName, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.Lack1FormYear, opt => opt.MapFrom(src => src.LACK1_FROM_YEAR))
                .ForMember(dest => dest.Lack1ToMonthId, opt => opt.MapFrom(src => src.LACK1_TO_MONTH))
                .ForMember(dest => dest.Lack1ToMonthName, opt => opt.MapFrom(src => src.MONTH1.MONTH_NAME_ENG))
                .ForMember(dest => dest.Lack1ToYear, opt => opt.MapFrom(src => src.LACK1_TO_YEAR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.StatusGov, opt => opt.MapFrom(src => src.STATUS_GOV))
                .ForMember(dest => dest.QtyApproved, opt => opt.MapFrom(src => src.QTY_APPROVED))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CompletedDate, 
                    opt => opt.MapFrom(src => src.STATUS == Enums.DocumentStatus.Completed ? 
                        (src.MODIFIED_DATE == null ? src.DECREE_DATE : src.MODIFIED_DATE) : null))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedByPoaId, opt => opt.MapFrom(src => src.APPROVED_BY_POA))
                .ForMember(dest => dest.ApprovedByPoaDate, opt => opt.MapFrom(src => src.APPROVED_DATE_POA))
                .ForMember(dest => dest.ApprovedByManagerId, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.ApprovedByManagerDate, opt => opt.MapFrom(src => src.APPROVED_DATE_MANAGER))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.LatestSaldo, opt => opt.MapFrom(src => src.LATEST_SALDO))
                .ForMember(dest => dest.LatestSaldoUomId, opt => opt.MapFrom(src => src.LATEST_SALDO_UOM))
                .ForMember(dest => dest.LatestSaldoUomName,
                    opt => opt.MapFrom(src => src.UOM1 != null ? src.UOM1.UOM_DESC : string.Empty))
                    .ForMember(dest => dest.CK5List, opt => opt.MapFrom(src => src.CK5));

            #endregion

            #region Monitoring Usage

            Mapper.CreateMap<PBCK1, Pbck1MonitoringUsageDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.NUMBER))
                .ForMember(dest => dest.PeriodFrom, opt => opt.MapFrom(src => src.PERIOD_FROM))
                .ForMember(dest => dest.PeriodTo, opt => opt.MapFrom(src => src.PERIOD_TO))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.NppbkcKppbcId, opt => opt.MapFrom(src => src.NPPBKC_KPPBC_ID))
                .ForMember(dest => dest.NppbkcCompanyCode, opt => opt.MapFrom(src => src.NPPBKC_BUKRS))
                .ForMember(dest => dest.NppbkcCompanyName, opt => opt.MapFrom(src => src.NPPBCK_BUTXT))
                .ForMember(dest => dest.ExGoodsQuota, opt => opt.MapFrom(src => src.QTY_APPROVED))
                .ForMember(dest => dest.PreviousFinalBalance, opt => opt.MapFrom(src => src.LATEST_SALDO))
                .ForMember(dest => dest.AdditionalExGoodsQuota, opt => opt.MapFrom(src => src.PBCK11 != null ?
                    src.PBCK11.Where(c => c.STATUS == Enums.DocumentStatus.Completed
                    && c.QTY_APPROVED.HasValue).Sum(s => s.QTY_APPROVED != null ? s.QTY_APPROVED.Value : 0) : 0))
                //todo: ambil dari QTY_RECEIVED di CK5 yang sekarang belum ada
                .ForMember(dest => dest.Received, opt => opt.MapFrom(src => src.CK5 != null ?
                    src.CK5.Where(c => c.STATUS_ID != Enums.DocumentStatus.Cancelled).Sum(s => s.GRAND_TOTAL_EX) : 0))
                .ForMember(dest => dest.Pbck1Type, opt => opt.MapFrom(src => src.PBCK1_TYPE))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.APPROVED_BY_POA != null ? "-" : src.APPROVED_BY_POA))
                .ForMember(dest => dest.SupNppbkc, opt => opt.MapFrom(src => src.SUPPLIER_NPPBKC_ID))
                .ForMember(dest => dest.SupKppbc, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_NAME))
                .ForMember(dest => dest.SupPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS == null ? "" : src.SUPPLIER_PLANT_WERKS))
                .ForMember(dest => dest.SupPlantDesc, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.SupCompany, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsNppbkcImport, opt => opt.MapFrom(src => src.IS_NPPBKC_IMPORT))
                .ForMember(dest => dest.ExcGoodsType, opt => opt.MapFrom(src => src.EXC_TYP_DESC))
                .ForMember(dest => dest.QtyUom, opt => opt.MapFrom(src => src.REQUEST_QTY_UOM))
                ;

            Mapper.CreateMap<PBCK1_PROD_PLAN, Pbck1ReportProdPlanDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.BkcRequired, opt => opt.MapFrom(src => src.BKC_REQUIRED))
                .ForMember(dest => dest.BkcRequiredUomId, opt => opt.MapFrom(src => src.BKC_REQUIRED_UOM_ID))
                .ForMember(dest => dest.BkcRequiredUomName, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_DESC : string.Empty))
                .ForMember(dest => dest.MonthId, opt => opt.MapFrom(src => src.MONTH.Value))
                .ForMember(dest => dest.MonthName, opt => opt.MapFrom(src => src.MONTH1 != null ? src.MONTH1.MONTH_NAME_IND : string.Empty))
                ;

            #endregion

            #region -------- Print Out ----------

            Mapper.CreateMap<Lack1ProductionDetailDto, Pbck1RealisasiProductionDetailDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.UOM_ID))
                .ForMember(dest => dest.UomDesc, opt => opt.MapFrom(src => src.UOM_DESC))
                ;

            Mapper.CreateMap<Lack1DetailsDto, Pbck1RealisasiP3BkcDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.SaldoAwal, opt => opt.MapFrom(src => src.BeginingBalance))
                .ForMember(dest => dest.Pemasukan, opt => opt.MapFrom(src => src.TotalIncome))
                .ForMember(dest => dest.Penggunaan, opt => opt.MapFrom(src => src.Usage + src.UsageTisToTis))
                .ForMember(dest => dest.SaldoAkhir, opt => opt.MapFrom(src => (src.BeginingBalance + src.TotalIncome - src.Usage - src.UsageTisToTis - src.ReturnQty)))
                ;

            #endregion

            #region Monitoring Mutasi

            Mapper.CreateMap<PBCK1, Pbck1MonitoringMutasiDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.NUMBER))
                .ForMember(dest => dest.AdditionalExGoodsQuota, opt => opt.MapFrom(src => src.PBCK11 != null
                    ? src.PBCK11.Where(c => c.STATUS == Enums.DocumentStatus.Completed
                                            && c.QTY_APPROVED.HasValue)
                        .Sum(s => s.QTY_APPROVED != null ? s.QTY_APPROVED.Value : 0)
                    : 0))
                .ForMember(dest => dest.ExGoodsQuota, opt => opt.MapFrom(src => src.QTY_APPROVED))
                .ForMember(dest => dest.Received, opt => opt.MapFrom(src => src.CK5 != null
                    ? src.CK5.Where(c => c.STATUS_ID != Enums.DocumentStatus.Cancelled).Sum(s => s.GRAND_TOTAL_EX)
                    : 0))
                .ForMember(dest => dest.Pbck1Type, opt => opt.MapFrom(src => src.PBCK1_TYPE))
                 .ForMember(dest => dest.Ck5List, opt => opt.MapFrom(src => src.CK5.Where(c => c.STATUS_ID != Enums.DocumentStatus.Cancelled)))
                 .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.APPROVED_BY_POA == null ? "-" : src.APPROVED_BY_POA))
                 .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.CREATED_BY))
                 .ForMember(dest => dest.SupPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT_WERKS == null ? "" : src.SUPPLIER_PLANT_WERKS))
                 .ForMember(dest => dest.SupPlantDesc, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                 .ForMember(dest => dest.SupComp, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY))
                 .ForMember(dest => dest.OriNppbkc, opt => opt.MapFrom(src => src.SUPPLIER_NPPBKC_ID))
                 .ForMember(dest => dest.OriKppbc, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_NAME))
                 .ForMember(dest => dest.IsNppbkcImport, opt => opt.MapFrom(src => src.IS_NPPBKC_IMPORT))
                 .ForMember(dest => dest.ExcGoodsType, opt => opt.MapFrom(src => src.EXC_TYP_DESC))
                 .ForMember(dest => dest.RecComp, opt => opt.MapFrom(src => src.NPPBCK_BUTXT))
                 .ForMember(dest => dest.RecNppbkc, opt => opt.MapFrom(src => src.NPPBKC_ID))
                 .ForMember(dest => dest.RecKppbc, opt => opt.MapFrom(src => src.NPPBKC_KPPBC_ID))
                 ; 


            #endregion

        }
    }
}
