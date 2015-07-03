﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1Item
    {
        public long Pbck1Id { get; set; }
        [Required, Display(Name = "PBCK-1 No")]

        public string Pbck1Number { get; set; }
        [Required, Display(Name = "References")]
        public long? Pbck1Reference { get; set; }

        [Required, Display(Name = "PBCK Type")]
        public Enums.PBCK1Type Pbck1Type { get; set; }

        public string PbckTypeName { get; set; }

        [Required, Display(Name = "Period From")]
        public DateTime PeriodFrom { get; set; }

        [Required, Display(Name = "Period To")]
        public DateTime? PeriodTo { get; set; }

        public string Year { get; set; }

        [Required, Display(Name = "Reported On")]
        public DateTime? ReportedOn { get; set; }

        [Required, Display(Name = "NPPBKC ID")]
        public long NppbkcId { get; set; }

        public string CompanyName { get; set; }

        public string NppbkcNo { get; set; }

        [Required, Display(Name = "Exciseable Goods Description")]
        public int? GoodTypeId { get; set; }

        public string GoodTypeDesc { get; set; }

        [Required, Display(Name = "Supplier Plant")]
        public string SupplierPlant { get; set; }

        [Display(Name = "Supplier Port")]
        public int? SupplierPortId { get; set; }

        public string SupplierPortName { get; set; }

        [Display(Name = "Supplier Address")]
        public string SupplierAddress { get; set; }

        [Display(Name = "Supplier Phone")]
        public string SupplierPhone { get; set; }

        [Required, Display(Name = "Plan Production From")]
        public DateTime? PlanProdFrom { get; set; }

        [Required, Display(Name = "Plan Production To")]
        public DateTime? PlanProdTo { get; set; }

        [UIHint("FormatQty")]
        [Required, Display(Name = "Request Qty")]
        public decimal? RequestQty { get; set; }

        [Required]
        public int? RequestQtyUomId { get; set; }

        public string RequestQtyUomName { get; set; }

        [Required, Display(Name = "LACK-1 From")]
        public int? Lack1FromMonthId { get; set; }

        public string Lack1FromMonthName { get; set; }

        [Required]
        public int? Lack1FormYear { get; set; }

        [Required, Display(Name = "LACK-1 To")]
        public int? Lack1ToMonthId { get; set; }

        public string Lack1ToMonthName { get; set; }

        public int? Lack1ToYear { get; set; }

        public Enums.DocumentStatus Status { get; set; }

        public string StatusName { get; set; }

        public Enums.DocumentStatus StatusGov { get; set; }

        public string StatusGovName { get; set; }

        [UIHint("FormatQty")]
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

        public List<Pbck1Item> Pbck1Childs { get; set; }

        public Pbck1Item Pbck1Parent { get; set; }

        public List<Pbck1ProdConverter> Pbck1ProdConverter { get; set; }

        public List<Pbck1ProdPlan> Pbck1ProdPlan { get; set; }
        
    }
}