using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class Pbck1
    {

        //public Pbck1()
        //{
        //    Pbck1Parent = new Pbck1();
        //    Pbck1Childs = new List<Pbck1>();
        //    Pbck1ProdConverter = new List<Pbck1ProdConverter>();
        //    Pbck1ProdPlan = new List<Pbck1ProdPlan>();
        //}

        public long Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public long? Pbck1Reference { get; set; }
        public Enums.PBCK1Type Pbck1Type { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public DateTime? ReportedOn { get; set; }
        public long NppbkcId { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcNo { get; set; }
        public int? GoodTypeId { get; set; }
        public string GoodTypeDesc { get; set; }
        public string SupplierPlant { get; set; }
        public int? SupplierPortId { get; set; }
        public string SupplierPortName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierPhone { get; set; }
        public DateTime? PlanProdFrom { get; set; }
        public DateTime? PlanProdTo { get; set; }
        public decimal? RequestQty { get; set; }
        public int? RequestQtyUomId { get; set; }
        public string RequestQtyUomName { get; set; }
        public int? Lack1FromMonthId { get; set; }
        public string Lack1FromMonthName { get; set; }
        public int? Lack1FormYear { get; set; }
        public int? Lack1ToMonthId { get; set; }
        public string Lack1ToMonthName { get; set; }
        public int? Lack1ToYear { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatus StatusGov { get; set; }
        public decimal? QtyApproved { get; set; }
        public DateTime? DecreeDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedById { get; set; }
        public string CreatedUsername { get; set; }
        public int? ApprovedById { get; set; }
        public string ApprovedUsername { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? LatestSaldo { get; set; }
        public int? LatestSaldoUomId { get; set; }
        public string LatestSaldoUomName { get; set; }

        public List<Pbck1> Pbck1Childs { get; set; }
        public Pbck1 Pbck1Parent { get; set; }

        public List<Pbck1ProdConverter> Pbck1ProdConverter { get; set; }
        public List<Pbck1ProdPlan> Pbck1ProdPlan { get; set; }
        
    }
}
