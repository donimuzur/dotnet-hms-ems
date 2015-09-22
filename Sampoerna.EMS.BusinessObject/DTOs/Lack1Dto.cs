using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1Dto
    {
        public int Lack1Id { get; set; }
        public string Lack1Number { get; set; }
        public string Bukrs { get; set; }
        public string Butxt { get; set; }
        public int? PeriodMonth { get; set; }
        public string PeriodNameInd { get; set; }
        public string PerionNameEng { get; set; }
        public int? PeriodYears { get; set; }
        public string LevelPlantId { get; set; }
        public string LevelPlantName { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string SupplierPlant { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantAddress { get; set; }
        public string SupplierCompanyName { get; set; }
        public string SupplierCompanyCode { get; set; }
        public string ExGoodsType { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public decimal ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatusGov? GovStatus { get; set; }
        public long DecreeDoc { get; set; }
        public DateTime? DecreeDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedBy  { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string NppbkcId { get; set;  }
        public string ExTypDesc { get; set; }
        /// <summary>
        /// Concate PeriodMonth and PeriodYear
        /// </summary>
        public DateTime Periode { get; set; }
        
        public decimal BeginingBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal Usage { get; set; }
        public decimal TotalProduction { get; set; }

        public string Lack1UomId { get; set; }
        public string Lack1UomName { get; set; }

    }

    public class Lack1DetailsDto
    {
        public Lack1DetailsDto()
        {
            Lack1Document = new List<Lack1DocumentDto>();
            Lack1IncomeDetail = new List<Lack1IncomeDetailDto>();
            Lack1Pbck1Mapping = new List<Lack1Pbck1MappingDto>();
            Lack1Plant = new List<Lack1PlantDto>();
            Lack1ProductionDetail = new List<Lack1ProductionDetailDto>();
        }
        public int Lack1Id { get; set; }
        public string Lack1Number { get; set; }
        public string Bukrs { get; set; }
        public string Butxt { get; set; }
        public int? PeriodMonth { get; set; }
        public string PeriodNameInd { get; set; }
        public string PerionNameEng { get; set; }
        public int? PeriodYears { get; set; }
        /// <summary>
        /// Concate PeriodMonth and PeriodYear
        /// </summary>
        public DateTime Periode { get; set; }
        public string LevelPlantId { get; set; }
        public string LevelPlantName { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string SupplierPlant { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantAddress { get; set; }
        public string SupplierCompanyName { get; set; }
        public string SupplierCompanyCode { get; set; }
        public string ExGoodsType { get; set; }
        public string ExGoodsTypeDesc { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public string WasteUomDesc { get; set; }
        public decimal ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public string ReturnUomDesc { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatusGov? GovStatus { get; set; }
        public DateTime? DecreeDate { get; set; }
        public string NppbkcId { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal Usage { get; set; }
        public string Lack1UomId { get; set; }
        public string Lack1UomName { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedByPoa { get; set; }
        public DateTime? ApprovedPoaDate { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedManagerDate { get; set; }
        public List<Lack1DocumentDto> Lack1Document { get; set; }
        public List<Lack1IncomeDetailDto> Lack1IncomeDetail { get; set; }
        public List<Lack1Pbck1MappingDto> Lack1Pbck1Mapping { get; set; }
        public List<Lack1PlantDto> Lack1Plant { get; set; }
        public List<Lack1ProductionDetailDto> Lack1ProductionDetail { get; set; }
        public string Noted { get; set; }
    }

}
