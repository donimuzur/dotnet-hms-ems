﻿using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1GeneratedDto
    {
        public Lack1GeneratedDto()
        {
            PlantList = new List<Lack1GeneratedPlantDto>();
            Pbck1List = new List<Lack1GeneratedPbck1DataDto>();
            IncomeList = new List<Lack1GeneratedIncomeDataDto>();
            FusionSummaryProductionList = new List<Lack1GeneratedSummaryProductionDataDto>();
            //ProductionList = new List<Lack1GeneratedProductionDataDto>();
            //SummaryProductionList = new List<Lack1GeneratedSummaryProductionDataDto>();
        }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }

        public List<Lack1GeneratedPlantDto> PlantList { get; set; }

        public List<Lack1GeneratedPbck1DataDto> Pbck1List { get; set; }

        public string SupplierCompanyName { get; set; }
        public string SupplierCompanyCode { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantName { get; set; }
        public string SupplierPlantAddress { get; set; }
        public int PeriodMonthId { get; set; }
        public string PeriodMonthName { get; set; }
        public int PeriodYear { get; set; }
        public decimal BeginingBalance { get; set; }

        public List<Lack1GeneratedIncomeDataDto> IncomeList { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalUsage { get; set; }
        public decimal? TotalUsageTisToTis { get; set; }
        public decimal TotalProduction { get; set; }
        public decimal EndingBalance { get; set; }
        
        public string Noted { get; set; }
        public string DocumentNoted { get; set; }
        public string Lack1UomId { get; set; }
        public Lack1GeneratedInventoryAndProductionDto InventoryProductionTisToFa { get; set; }
        public Lack1GeneratedInventoryAndProductionDto InventoryProductionTisToTis { get; set; }
        public List<Lack1GeneratedSummaryProductionDataDto> FusionSummaryProductionList { get; set; }

        public List<Lack1GeneratedInvMovementProductionStepTracingItem> AlcoholTrackingList { get; set; }

    }

    public class Lack1GeneratedInventoryAndProductionDto
    {
        public Lack1GeneratedInventoryMovementDto InvetoryMovementData { get; set; }
        public Lack1GeneratedProductionDto ProductionData { get; set; }
    }

    public class Lack1GeneratedProductionDto
    {
        public List<Lack1GeneratedProductionDataDto> ProductionList { get; set; }
        public List<Lack1GeneratedProductionSummaryByProdTypeDataDto> ProductionSummaryByProdTypeList { get; set; }
        public List<Lack1GeneratedSummaryProductionDataDto> SummaryProductionList { get; set; }
    }

    public class Lack1GeneratedInventoryMovementDto
    {
        public List<Lack1GeneratedTrackingDto> InvMovementAllList { get; set; }
        public List<Lack1GeneratedTrackingDto> InvMovementReceivingCk5List { get; set; }
        public List<Lack1GeneratedTrackingDto> InvMovementReceivingList { get; set; }
    }

    public class Lack1GeneratedSummaryProductionDataDto
    {
        public decimal Amount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
    }

    public class Lack1GeneratedProductionSummaryByProdTypeDataDto
    {
        public string ProdCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public decimal TotalAmount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
    }

    public class Lack1GeneratedPlantDto
    {
        public string Werks { get; set; }
        public string Name1 { get; set; }
        public string Address { get; set; }
    }

    public class Lack1GeneratedPbck1DataDto
    {
        public int Pbck1Id { get; set; }
    }

    public class Lack1GeneratedIncomeDataDto
    {
        public long Ck5Id { get; set; }
        public decimal Amount { get; set; }
        public string StoReceiverNumber { get; set; }
        public string StoSenderNumber { get; set; }
        public string DnNumber { get; set; }
        public Enums.CK5Type Ck5Type { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string StringRegistrationDate { get; set; }
    }

    public class Lack1GeneratedProductionDataDto
    {
        public string FaCode { get; set; }
        public string Ordr { get; set; }
        public string ProdCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public decimal Amount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
        public bool IsTisToTisData { get; set; }
    }

    public class Lack1GeneratedTrackingDto
    {
        public long INVENTORY_MOVEMENT_ID { get; set; }
        public bool IsTisToTisData { get; set; }
    }

    public class Lack1GeneratedProductionDomesticAlcoholDto
    {
        public INVENTORY_MOVEMENT InvMovementUsage { get; set; }
        public List<Lack1GeneratedInvMovementProductionStepTracingItem> InvMovementProductionStepTracing { get; set; }
    }

    public class Lack1GeneratedInvMovementProductionStepTracingItem : INVENTORY_MOVEMENT
    {
        public string ParentProcessOrder { get; set; }
        public bool IsFinalGoodsType { get; set; }
        public bool IsFirstLevel { get; set; }
        public string ExGoodsTypeId { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
    }

}
