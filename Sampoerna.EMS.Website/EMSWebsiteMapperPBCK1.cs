﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.PLANT;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializePBCK1()
        {
            #region Pbck1

            Mapper.CreateMap<Pbck1Dto, Pbck1Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.StatusGovName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.StatusGov)))
                .ForMember(dest => dest.PbckTypeName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck1Type)))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.PeriodFrom.Year))
                .ForMember(dest => dest.Pbck1ReferenceNumber, opt => opt.MapFrom(src => src.Pbck1Reference != null && src.Pbck1Parent != null ? src.Pbck1Parent.Pbck1Number : string.Empty))
                .ForMember(dest => dest.Pbck1ProdConverter,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConvModel>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.Pbck1ProdPlan,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlanModel>>(src.Pbck1ProdPlan)))
             ;

            Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetOpenDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
                ;

            Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetCompletedDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
                ;

            Mapper.CreateMap<Pbck1ProdConvModel, Pbck1ProdConverterDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.ProdTypeAlias))
                .ForMember(dest => dest.ConverterOutput, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.ConverterOutput))
                .ForMember(dest => dest.ConverterOutputUomId, opt => opt.MapFrom(src => src.ConverterUomId))
                .ForMember(dest => dest.Range, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Range))
                ;
            Mapper.CreateMap<Pbck1ProdConverterDto, Pbck1ProdConvModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.ProdAlias))
                .ForMember(dest => dest.ConverterOutput, opt => opt.MapFrom(src => ConvertHelper.ConvertDecimalFiveToString(src.ConverterOutput).TrimEnd('0').TrimEnd('.')))
                .ForMember(dest => dest.ConverterUomId, opt => opt.MapFrom(src => src.ConverterOutputUomId))
                .ForMember(dest => dest.ConverterUom, opt => opt.MapFrom(src => src.ConverterOutputUomName))
                .ForMember(dest => dest.Range, opt => opt.MapFrom(src => ConvertHelper.ConvertDecimalFiveToString(src.Range).TrimEnd('0').TrimEnd('.')))
                ;

            Mapper.CreateMap<Pbck1ProdPlanModel, Pbck1ProdPlanDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MonthId, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Month))
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.ProdTypeAlias))
                .ForMember(dest => dest.Amount, opt => opt.ResolveUsing<StringToNullableDecimalResolver>().FromMember(src => src.Amount))
                .ForMember(dest => dest.BkcRequired, opt => opt.MapFrom(src => src.BkcRequired))
                ;

            Mapper.CreateMap<Pbck1ProdPlanDto, Pbck1ProdPlanModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.MonthId))
                .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.ProdAlias))
                ;

            Mapper.CreateMap<Pbck1Item, Pbck1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdConverter,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConverterDto>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.Pbck1ProdPlan,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlanModel>>(src.Pbck1ProdPlan)))
                    ;
            Mapper.CreateMap<Pbck1SummaryReportsItem, ExportSummaryDataModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.NppbkcCompanyName))
                .ForMember(dest => dest.Nppbkc, opt => opt.MapFrom(src => (src.NppbkcId)))
                .ForMember(dest => dest.Kppbc, opt => opt.MapFrom(src => src.NppbkcKppbcName))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => (src.Pbck1Number)))
                .ForMember(dest => dest.OriginalNppbkc, opt => opt.MapFrom(src => src.SupplierNppbkcId))
                .ForMember(dest => dest.OriginalKppbc, opt => opt.MapFrom(src => src.SupplierKppbcName))
                .ForMember(dest => dest.OriginalAddress, opt => opt.MapFrom(src => src.SupplierAddress))
                .ForMember(dest => dest.ExcGoodsAmount, opt => opt.MapFrom(src => src.QtyApproved == null ? "0.00" : String.Format("{0:n}", src.QtyApproved.Value)))
                .ForMember(dest => dest.Pbck1Type, opt => opt.MapFrom(src => src.Pbck1Type.ToString()))
                .ForMember(dest => dest.PlanProdFrom, opt => opt.MapFrom(src => src.PlanProdFrom == null ? "-" : src.PlanProdFrom.Value.ToString("dd MMMM yyyy")))
                .ForMember(dest => dest.PlanProdTo, opt => opt.MapFrom(src => src.PlanProdTo == null ? "-" : src.PlanProdTo.Value.ToString("dd MMMM yyyy")))
                .ForMember(dest => dest.PoaList, opt => opt.MapFrom(src => src.PoaList == null ? "" : src.PoaList.Count > 0 ? string.Join("<br />", src.PoaList.ToArray()) : ""))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.Pbck1ReferenceNumber))
                .ForMember(dest => dest.LACKFrom, opt => opt.MapFrom(src => (src.Lack1ToMonthName + src.Lack1ToYear.Value)))
                .ForMember(dest => dest.LatestSaldo, opt => opt.MapFrom(src => src.LatestSaldo == null ? "0.00" : String.Format("{0:n}", src.LatestSaldo.Value)))
                .ForMember(dest => dest.PeriodFrom, opt => opt.MapFrom(src => src.PeriodFrom.ToString("dd MMMM yyyy")))
                .ForMember(dest => dest.PeriodTo, opt => opt.MapFrom(src => src.PeriodTo == null ? "-" : src.PeriodTo.Value.ToString("dd MMMM yyyy")))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.ReportedOn == null ? "" : src.ReportedOn.Value.ToString("dd MMMM yyyy")))
                .ForMember(dest => dest.RequestQty, opt => opt.MapFrom(src => src.RequestQty == null ? "" : String.Format("{0:n}", src.RequestQty.Value)))
                .ForMember(dest => dest.StatusGov, opt => opt.MapFrom(src => src.StatusGovName))
                .ForMember(dest => dest.QtyApproved, opt => opt.MapFrom(src => src.QtyApproved == null ? "" : String.Format("{0:n}", src.QtyApproved.Value)))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DecreeDate == null ? "" : src.DecreeDate.Value.ToString("dd MMMM yyyy")))
                .ForMember(dest => dest.IsNppbkcImport, opt => opt.MapFrom(src => src.IsNppbkcImport ? "Yes" : "No"))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.ApprovedByPoaId, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.ApprovedByPoaId) ? "-" : src.ApprovedByPoaId))
                .ForMember(dest => dest.ApprovedByManagerId, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.ApprovedByManagerId) ? "-" : src.ApprovedByManagerId))
                .ForMember(dest => dest.LatestSaldoUomName, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.LatestSaldoUomName) ? "-" : src.LatestSaldoUomName))
                .ForMember(dest => dest.RequestQtyUomName, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.RequestQtyUomName) ? "-" : src.RequestQtyUomName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => String.Join("<br />", src.NppbkcPlants.Select(c => c.ADDRESS).ToArray())))
                .ForMember(dest => dest.DocNumberCk5, opt => opt.MapFrom(src => String.Join("<br />", src.CK5List.Select(c => String.Format("'{0}", c.DocumentNumber)).ToArray())))
                .ForMember(dest => dest.StatusDocCk5, opt => opt.MapFrom(src => String.Join("<br />", src.CK5List.Select(c => c.Status).ToArray())))
                .ForMember(dest => dest.GrandTotalExcisableCk5, opt => opt.MapFrom(src => String.Join("<br />", src.CK5List.Select(c => c.Qty).ToArray())))
                .ForMember(dest => dest.CompletedDate, opt => opt.MapFrom(src => src.CompletedDate == null ? "" : src.CompletedDate.Value.ToString("dd MMMM yyyy")))
                ;

            Mapper.CreateMap<Pbck1DashboardModel, Pbck1GetByParamInput>().IgnoreAllNonExisting();

            #endregion

            Mapper.CreateMap<Pbck1ProdPlanModel, Pbck1ProdPlanInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdPlanOutput, Pbck1ProdPlanModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1ProdConvModel, Pbck1ProdConverterInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdConverterOutput, Pbck1ProdConvModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1DecreeDocDto, Pbck1DecreeDocModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1DecreeDocModel, Pbck1DecreeDocDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1FilterSummaryReportViewModel, Pbck1GetSummaryReportByParamInput>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1SummaryReportDto, Pbck1SummaryReportsItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcPlants, opt => opt.MapFrom(src => Mapper.Map<List<T001WModel>>(src.NppbkcPlants)))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.StatusGovName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.StatusGov)))
                .ForMember(dest => dest.Pbck1ReferenceNumber, opt => opt.MapFrom(src => src.Pbck1Reference != null && src.Pbck1Parent != null ? src.Pbck1Parent.Pbck1Number : string.Empty))
                ;

            Mapper.CreateMap<Pbck1FilterMonitoringUsageViewModel, Pbck1GetMonitoringUsageByParamInput>()
                .IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1MonitoringUsageDto, Pbck1MonitoringUsageItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Received, opt => opt.MapFrom(src => (src.Received + src.ReceivedAdditional)))
                .ForMember(dest => dest.TotalPbck1Quota, opt => opt.MapFrom(src => (src.ExGoodsQuota + src.AdditionalExGoodsQuota)))
                .ForMember(dest => dest.QuotaRemaining, opt => opt.MapFrom(src => (src.ExGoodsQuota + src.AdditionalExGoodsQuota - src.Received - src.ReceivedAdditional)))
                .ForMember(dest => dest.Pbck1PeriodDisplay, opt => opt.MapFrom(src => (src.PeriodFrom.ToString("dd MMM yyyy") + " - " + src.PeriodTo.Value.ToString("dd MMM yyyy"))))
                .ForMember(dest => dest.IsNppbkcImport, opt => opt.MapFrom(src => src.IsNppbkcImport ? "Yes" : "No"))
                ;

            #region Monitoring Mutasi

            Mapper.CreateMap<Pbck1MonitoringMutasiViewModel, Pbck1GetMonitoringMutasiByParamInput>();

            Mapper.CreateMap<Pbck1MonitoringMutasiDto, Pbck1MonitoringMutasiItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Received, opt => opt.MapFrom(src => (src.Received + src.ReceivedAdditional)))
                .ForMember(dest => dest.TotalPbck1Quota, opt => opt.MapFrom(src => (src.ExGoodsQuota + src.AdditionalExGoodsQuota)))
                .ForMember(dest => dest.QuotaRemaining, opt => opt.MapFrom(src => (src.ExGoodsQuota + src.AdditionalExGoodsQuota - src.Received - src.ReceivedAdditional)))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.Pbck1Number))
                .ForMember(dest => dest.IsNppbkcImport, opt => opt.MapFrom(src => src.IsNppbkcImport ? "Yes" : "No"));

            Mapper.CreateMap<Pbck1MonitoringMutasiItem, ExportMonitoringMutasiDataModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Received, opt => opt.MapFrom(src => src.Received))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.Pbck1Number))
                .ForMember(dest => dest.TotalPbck1Quota, opt => opt.MapFrom(src => src.TotalPbck1Quota))
                .ForMember(dest => dest.QuotaRemaining, opt => opt.MapFrom(src => src.QuotaRemaining))
                .ForMember(dest => dest.DocNumberCk5,
                    opt =>
                        opt.MapFrom(src => String.Join("<br />", src.Ck5List.Select(p => String.Format("'{0}", p.DocumentNumber)).ToArray())))
                .ForMember(dest => dest.GrandTotalExciseable,
                    opt => opt.MapFrom(src => String.Join("<br />", src.Ck5List.Select(p => p.GRAND_TOTAL_EX).ToArray())))
                .ForMember(dest => dest.UoM,
                    opt => opt.MapFrom(src => String.Join("<br />", src.Ck5List.Select(p => p.PACKAGE_UOM_ID).ToArray())))
                .ForMember(dest => dest.DetailCk5Plant,
                    opt => opt.MapFrom(src => String.Join("<br />", src.Ck5List.Select(p => p.DestinationPlantId).ToArray())))
                .ForMember(dest => dest.DetailCk5PlantDesc,
                    opt => opt.MapFrom(src => String.Join("<br />", src.Ck5List.Select(p => p.DestinationPlantDesc).ToArray())))
                .ForMember(dest => dest.GrDateCk5,
                    opt => opt.MapFrom(src => String.Join("<br />", src.Ck5List.Select(p => p.GrDate).ToArray())))
                .ForMember(dest => dest.RegDateCk5,
                    opt => opt.MapFrom(src => String.Join("<br />", src.Ck5List.Select(p => p.RegDate).ToArray())))
                    ;

            #endregion

            Mapper.CreateMap<QUOTA_MONITORING, QuotaMonitoringModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.QUOTA_MONITORING_DETAIL));
            Mapper.CreateMap<QUOTA_MONITORING_DETAIL, QuotaMonitoringDetail>().IgnoreAllNonExisting();
        }
    }
}