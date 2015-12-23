﻿using System;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1SummaryReportDto
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
        public Enums.DocumentStatusGovType2? GovStatus { get; set; }
        public DateTime? DecreeDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string NppbkcId { get; set; }
        public string ExTypDesc { get; set; }
        
        public decimal BeginingBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal Usage { get; set; }
        public decimal? UsageTisToTis { get; set; }
        public decimal TotalProduction { get; set; }

        public string Lack1UomId { get; set; }
        public string Lack1UomName { get; set; }
    }
}
