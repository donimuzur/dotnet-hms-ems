using System;
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
        public decimal CloseBalance { get; set; }
        /// <summary>
        /// all income list that got use current logic for saving to database
        /// use this field for saving to database instead IncomeList 
        /// </summary>
        public List<Lack1GeneratedIncomeDataDto> AllIncomeList { get; set; }
        /// <summary>
        /// exclude ck5 type : waste, return and manual
        /// for display only
        /// </summary>
        public List<Lack1GeneratedIncomeDataDto> IncomeList { get; set; }

        public List<Lack1GeneratedIncomeDataDto> AllCk5List { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal TotalUsage { get; set; }
        public decimal? TotalUsageTisToTis { get; set; }
        public decimal TotalProduction { get; set; }
        public decimal EndingBalance { get; set; }
        public decimal TotalWaste { get; set; }

        public decimal TotalReturn { get; set; }
        public decimal TotalLaboratorium { get; set; }
        
        public string Noted { get; set; }
        public string DocumentNoted { get; set; }
        public string Lack1UomId { get; set; }
        public string WasteAmountUom { get; set; }
        public Lack1GeneratedInventoryAndProductionDto InventoryProductionTisToFa { get; set; }
        public Lack1GeneratedInventoryAndProductionDto InventoryProductionTisToTis { get; set; }
        public List<Lack1GeneratedSummaryProductionDataDto> FusionSummaryProductionList { get; set; }

        public List<Lack1GeneratedInvMovementProductionStepTracingItem> AlcoholTrackingList { get; set; }

        public List<Lack1CalculationDetail> CalculationDetails { get; set; }

        public List<PeriodSummary> PeriodSummaries { get; set; }

        public PeriodSummary StartPeriodData { get; set; }
        public PeriodSummary CurrentPeriodData { get; set; }
        public PeriodSummary EndPeriodData { get; set; }

        public Lack1GeneratedRemarkDto Ck5RemarkData { get; set; }

    }

    public class Lack1GeneratedRemarkDto
    {
        public List<Lack1GeneratedIncomeDataDto> Ck5WasteData { get; set; }
        public List<Lack1GeneratedIncomeDataDto> Ck5ReturnData { get; set; }
        public List<Lack1GeneratedIncomeDataDto> Ck5TrialData { get; set; }
    }

    public class Lack1GeneratedInventoryAndProductionDto
    {
        public Lack1GeneratedInventoryMovementDto InvetoryMovementData { get; set; }
        public Lack1GeneratedProductionDto ProductionData { get; set; }

        public List<Lack1CalculationDetail> CalculationDetails { get; set; } 
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

        public List<Pbck1ProdConverterDto> Pbck1Convertion { get; set; }
    }

    public class Lack1GeneratedIncomeDataDto
    {
        public long Ck5Id { get; set; }
        public decimal Amount { get; set; }
        public string StoReceiverNumber { get; set; }
        public string StoSenderNumber { get; set; }
        public string DnNumber { get; set; }
        public Enums.CK5Type Ck5Type { get; set; }
        public bool IsCk5ReduceTrial { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string StringRegistrationDate { get; set; }
        public string PackageUomId { get; set; }
        public string PackageUomDesc { get; set; }
        public bool FlagForLack1 { get; set; }

        public string SubmissionNumber { get; set; }
        public DateTime? GrDate { get; set; }

        public decimal GrandTotalEx { get; set; }
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

        public decimal ProportionalOrder { get; set; }
        public DateTime ProductionDate { get; set; } 
    }

    public class Lack1GeneratedTrackingDto : INVENTORY_MOVEMENT
    {
        //public long INVENTORY_MOVEMENT_ID { get; set; }
        public bool IsTisToTisData { get; set; }
        public string ConvertedUomId { get; set; }
        public string ConvertedUomDesc { get; set; }
        public decimal ConvertedQty { get; set; }
    }

    public class Lack1GeneratedProductionDomesticAlcoholDto
    {
        public Lack1GeneratedInvMovementProductionStepTracingItem InvMovementUsage { get; set; }
        public List<Lack1GeneratedInvMovementProductionStepTracingItem> InvMovementProductionStepTracing { get; set; }
    }

    public class Lack1GeneratedInvMovementProductionStepTracingItem
    {
        public string Mvt { get; set; }
        public string MaterialId { get; set; }
        public string PlantId { get; set; }
        public decimal Qty { get; set; }
        public decimal ProductionQty { get; set; }
        public string Bun { get; set; }
        public string PurchDoc { get; set; }
        public string MatDoc { get; set; }
        public string Batch { get; set; }
        public string Ordr { get; set; }
        public bool IsFinalGoodsType { get; set; }
        public int TrackLevel { get; set; }
        public string ParentOrdr { get; set; }
        public string ExGoodsTypeId { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
        public DateTime? PostingDate { get; set; }
        public decimal? ConvertedQty { get; set; }
        public string ConvertedUomId { get; set; }
        public string ConvertedUomDesc { get; set; }
    }


    public class Lack1FACodeProportional
    {
        
        public string Fa_Code { get; set; }

        public string Order { get; set; }

        public decimal QtyOrder { get; set; }

        public decimal QtyAllOrder { get; set; }
    }

    public class Lack1CFUsagevsFaDetailCombinedDto
    {
        public string PlantId { get; set; }
        public string PlantDesc { get; set; }
        public string Order { get; set; }
        public string Fa_Code { get; set; }
        public string Brand_Desc { get; set; }

        public string Mvt { get; set; }
        public string Batch { get; set; }
        
        public string Material_Id { get; set; }

        
        public string ProductionDateText
        {
            get; set;
        }

        public DateTime PostingDate { get; set; }

        public string PostingDateText
        {
            get;set;
        }

        public decimal Qty { get; set; }
        public decimal Converted_Qty { get; set; }
        public string Uom { get; set; }
        public string Converted_Uom { get; set; }
    }


    public class Lack1CFUsagevsFaDetailDto
    {
        public string PlantId { get; set; }
        public string PlantDesc { get; set; }
        public string Order { get; set; }
        public string Fa_Code { get; set; }
        public string Brand_Desc { get; set; }

        public List<Lack1CFUsagevsFaDetailDtoMvt> Lack1CFUsagevsFaDetailDtoMvt101 { get; set; }
        public List<Lack1CFUsagevsFaDetailDtoMvt> Lack1CFUsagevsFaDetailDtoMvt261 { get; set; }
        public List<WasteDto> Lack1CFUsagevsFaDetailDtoMvtWaste { get; set; }
    }

    public class Lack1CFUsagevsFaDetailDtoMvt
    {
        public string Mvt { get; set; }
        public string Batch { get; set; }
        public string Order { get; set; }
        public string PlantId { get; set; }
        public string Material_Id { get; set; }

        public DateTime ProductionDate { get; set; }
        public string ProductionDateText {
            get
            {
                return ProductionDate.ToString("dd-MMM-yyyy");
            }
        }

        public DateTime PostingDate { get; set; }

        public string PostingDateText
        {
            get
            {
                return ProductionDate.ToString("dd-MMM-yyyy");
            }
        }

        public decimal Qty { get; set; }
        public decimal Converted_Qty { get; set; }
        public string Uom { get; set; }
        public string Converted_Uom { get; set; }
    }


    public class Lack1CalculationDetail
    {

        public int LACK1_ID { get; set; }
        public string MaterialId { get; set; }
        public string PlantId { get; set; }
        public decimal AmountUsage { get; set; }

        public string UomUsage { get; set; }

        public string Ordr { get; set; }
        public string FaCode { get; set; }

        public string BrandCe { get; set; }
        public decimal AmountProduction { get; set; }

        public string UomProduction { get; set; }
        public decimal Proportional { get; set; }

        public decimal Convertion { get; set; }
        public Enums.Lack1Calculation Type { get; set; }
    }


    public class PeriodSummary
    {
        public int LACK1_ID { get; set; }
        public decimal Income { get; set; }
        public decimal Usage { get; set; }

        public decimal Laboratorium { get; set; }

        public decimal Return { get; set; }

        public decimal Saldo
        {
            get; 
            set;


        }


        public Enums.Lack1SummaryPeriod Type { get; set; }
    }

    //public class Lack1CFUsagevsFaDetailDtoWaste
    //{
        
    //    public string PlantId { get; set; }
    //    public string Fa_Code { get; set; }

    //    public DateTime ProductionDate { get; set; }
        
    //    public decimal Reject_Maker_Qty { get; set; }

    //    public string Reject_Maker_Uom { get; set; }

    //    public decimal Reject_Packer_Qty { get; set; }

    //    public string Reject_Packer_Uom { get; set; }

    //    public decimal Dust_Qty { get; set; }

    //    public string Dust_Uom { get; set; }

    //    public decimal Floor_Qty { get; set; }

    //    public string Floor_Uom { get; set; }

    //    public decimal Stem_Qty { get; set; }

    //    public string Stem_Uom { get; set; }
    //}
}
