using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1GeneratedDto
    {
        public Lack1GeneratedDto()
        {
            PlantList = new List<Lack1GeneratedPlantDto>();
            Pbck1List = new List<Lack1GeneratedPbck1DataDto>();
            IncomeList = new List<Lack1GeneratedIncomeDataDto>();
            ProductionList = new List<Lack1GeneratedProductionDataDto>();
            SummaryProductionList = new List<Lack1GeneratedSummaryProductionDataDto>();
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
        public decimal TotalProduction { get; set; }
        public decimal EndingBalance { get; set; }
        public List<Lack1GeneratedProductionDataDto> ProductionList { get; set; }
        public List<Lack1GeneratedSummaryProductionDataDto> SummaryProductionList { get; set; }
        public string Noted { get; set; }
        public string Lack1UomId { get; set; }

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
        public string RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string StringRegistrationDate { get; set; }
    }

    public class Lack1GeneratedProductionDataDto
    {
        public string ProdCode { get; set; }
        public string ProductType { get; set; }
        public string ProductAlias { get; set; }
        public decimal Amount { get; set; }
        public string UomId { get; set; }
        public string UomDesc { get; set; }
    }

    public class Lack1GeneratedTrackingDto
    {
        public long INVENTORY_MOVEMENT_ID { get; set; }
    }
    
}
