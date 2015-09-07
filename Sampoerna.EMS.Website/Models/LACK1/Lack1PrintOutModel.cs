using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1PrintOutModel
    {
        public long Lack1Id { get; set; }
        public string Lack1Number { get; set; }

        #region ------------ Lack1 Field List ------

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
        public string ExGoodsType { get; set; }
        public string ExGoodsTypeDesc { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public string WasteUomDesc { get; set; }
        public decimal ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public string ReturnUomDesc { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public string StatusDescription { get; set; }
        public Enums.DocumentStatus GovStatus { get; set; }
        public string GovStatusDescription { get; set; }
        public DateTime? DecreeDate { get; set; }
        public string NppbkcId { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalUsage { get; set; }
        public decimal EndingBalance { get; set; }
        public string Lack1UomId { get; set; }
        public string Lack1UomName { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedByPoa { get; set; }
        public DateTime? ApprovedPoaDate { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedManagerDate { get; set; }
        public List<Lack1DocumentItemModel> Lack1Document { get; set; }
        public List<Lack1IncomeDetailItemModel> IncomeList { get; set; }
        public List<Lack1Pbck1MappingItemModel> Lack1Pbck1Mapping { get; set; }
        public List<Lack1PlantItemModel> Lack1Plant { get; set; }
        public List<Lack1ProductionDetailItemModel> ProductionList { get; set; }
        public List<Lack1SummaryProductionItemModel> SummaryProductionList { get; set; }//todo: set manually from controller to create summary of ProductionList
        public string Noted { get; set; }

        public Lack1HeaderFooter HeaderFooter { get; set; }

        #endregion

    }

    public class Lack1HeaderFooter
    {
        public string ImageHeader { get; set; }
        public string FooterContent { get; set; }
    }
}