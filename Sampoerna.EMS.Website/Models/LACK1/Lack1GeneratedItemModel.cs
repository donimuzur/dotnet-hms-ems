using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1GeneratedItemModel
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }

        public List<Lack1GeneratedPlantDto> PlantList { get; set; }

        public List<Lack1GeneratedPbck1DataDto> Pbck1List { get; set; }

        public string SupplierCompanyName { get; set; }
        public string SupplierCompanyCode { get; set; }
        public string SupplierPlantAddress { get; set; }
        public int PeriodMonthId { get; set; }
        public string PeriodMonthName { get; set; }
        public int PeriodYear { get; set; }
        [UIHint("FormatQty")]
        public decimal BeginingBalance { get; set; }

        public List<Lack1GeneratedIncomeDataDto> IncomeList { get; set; }
        [UIHint("FormatQty")]
        public decimal TotalIncome { get; set; }
        [UIHint("FormatQty")]
        public decimal TotalUsage { get; set; }
        [UIHint("FormatQty")]
        public decimal TotalProduction { get; set; }
        [UIHint("FormatQty")]
        public decimal EndingBalance { get; set; }
        public List<Lack1GeneratedProductionDataDto> ProductionList { get; set; }
        public string Noted { get; set; }
    }
}