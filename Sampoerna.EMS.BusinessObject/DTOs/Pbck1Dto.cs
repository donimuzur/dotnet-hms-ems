using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1Dto
    {
        public int Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public long? Pbck1Reference { get; set; }
        public Enums.PBCK1Type Pbck1Type { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public DateTime? ReportedOn { get; set; }
        public string NppbkcId { get; set; }
        public string NppbkcCompanyCode { get; set; }
        public string NppbkcCompanyName { get; set; }
        public string GoodType { get; set; }
        public string GoodTypeDesc { get; set; }
        public string SupplierPlant { get; set; }
        public int? SupplierPortId { get; set; }
        public string SupplierPortName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierNppbkcId { get; set; }
        public string SupplierKppbcId { get; set; }
        public string SupplierKppbcName { get; set; }
        public string SupplierPlantWerks { get; set; }
        public DateTime? PlanProdFrom { get; set; }
        public DateTime? PlanProdTo { get; set; }
        public decimal? RequestQty { get; set; }
        public string RequestQtyUomId { get; set; }
        public string RequestQtyUomName { get; set; }
        public int? Lack1FromMonthId { get; set; }
        public string Lack1FromMonthName { get; set; }
        public int? Lack1FormYear { get; set; }
        public int? Lack1ToMonthId { get; set; }
        public string Lack1ToMonthName { get; set; }
        public int? Lack1ToYear { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatusGov? StatusGov { get; set; }
        public decimal? QtyApproved { get; set; }
        public DateTime? DecreeDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public string ApprovedByPoaId { get; set; }
        public string ApprovedByManagerId { get; set; }
        public DateTime? ApprovedByPoaDate { get; set; }
        public DateTime? ApprovedByManagerDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? LatestSaldo { get; set; }
        public string LatestSaldoUomId { get; set; }
        public string LatestSaldoUomName { get; set; }

        public List<Pbck1Dto> Pbck1Childs { get; set; }
        public Pbck1Dto Pbck1Parent { get; set; }

        public List<Pbck1ProdConverterDto> Pbck1ProdConverter { get; set; }
        public List<Pbck1ProdPlanDto> Pbck1ProdPlan { get; set; }
        public List<Pbck1DecreeDocDto> Pbck1DecreeDoc { get; set; }
        public string NppbkcKppbcId { get; set; }
        
    }
}
