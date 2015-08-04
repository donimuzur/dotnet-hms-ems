using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Validations;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1Item
    {
        public Pbck1Item()
        {
            Pbck1ProdConverter = new List<Pbck1ProdConvModel>();
            Pbck1ProdPlan = new List<Pbck1ProdPlanModel>();
            PeriodFrom = DateTime.Now;
            PeriodTo = DateTime.Now;
            ReportedOn = DateTime.Now;
            PlanProdFrom = DateTime.Now;
            PlanProdTo = DateTime.Now;
            DecreeDate = DateTime.Now;
        }
        public int Pbck1Id { get; set; }

        [Display(Name = "PBCK-1 No")]
        public string Pbck1Number { get; set; }

        [RequiredIf("Pbck1Type", Enums.PBCK1Type.Additional), Display(Name = "References")]
        public long? Pbck1Reference { get; set; }
        public string Pbck1ReferenceNumber { get; set; }

        [Required, Display(Name = "PBCK Type")]
        public Enums.PBCK1Type Pbck1Type { get; set; }

        public string PbckTypeName { get; set; }

        [Required, Display(Name = "Period From")]
        [UIHint("FormatDateTime")]
        public DateTime PeriodFrom { get; set; }

        [Required, Display(Name = "Period To")]
        [UIHint("FormatDateTime")]
        public DateTime? PeriodTo { get; set; }

        public string Year { get; set; }

        [Required, Display(Name = "Reported On")]
        [UIHint("FormatDateTime")]
        public DateTime ReportedOn { get; set; }

        [Required, Display(Name = "NPPBKC ID")]
        public string NppbkcId { get; set; }

        public string NppbkcKppbcId { get; set; }

        public string NppbkcCompanyCode { get; set; }
        public string NppbkcCompanyName { get; set; }
        
        public string PoaList { get; set; }

        [Required, Display(Name = "Exciseable Goods Description")]
        public string GoodType { get; set; }
        public string GoodTypeDesc { get; set; }

        [Required, Display(Name = "Supplier Plant")]
        public string SupplierPlant { get; set; }

        public string SupplierPlantWerks { get; set; }
        
        [Display(Name = "Supplier Port")]
        public int? SupplierPortId { get; set; }

        public string SupplierNppbkcId { get; set; }

        public string HiddenSupplierNppbkcId { get; set; }

        public string SupplierPortName { get; set; }

        [Display(Name = "Supplier Address")]
        public string SupplierAddress { get; set; }

        public string HiddendSupplierAddress { get; set; }

        [Display(Name = "Supplier Phone")]
        public string SupplierPhone { get; set; }

        public string SupplierKppbcId { get; set; }

        public string HiddenSupplierKppbcId { get; set; }

        [Required, Display(Name = "Plan Production From")]
        [UIHint("FormatDateTime")]
        public DateTime PlanProdFrom { get; set; }

        [Required, Display(Name = "Plan Production To")]
        [UIHint("FormatDateTime")]
        public DateTime PlanProdTo { get; set; }
        
        [UIHint("FormatQty")]
        [Required, Display(Name = "Request Qty")]
        public decimal RequestQty { get; set; }

        [Required]
        public string RequestQtyUomId { get; set; }

        public string RequestQtyUomName { get; set; }

        [Required, Display(Name = "LACK-1 From")]
        public int Lack1FromMonthId { get; set; }

        public string Lack1FromMonthName { get; set; }

        [Required]
        public int Lack1FormYear { get; set; }

        [Required, Display(Name = "LACK-1 To")]
        public int Lack1ToMonthId { get; set; }

        public string Lack1ToMonthName { get; set; }

        [Required]
        public int Lack1ToYear { get; set; }

        public Enums.DocumentStatus Status { get; set; }

        public string StatusName { get; set; }

        [RequiredIf("Status", Enums.DocumentStatus.WaitingGovApproval), Display(Name = "Status Gov")]
        public Enums.DocumentStatusGov StatusGov { get; set; }
        
        public string StatusGovName { get; set; }

        [RequiredIf("Status", Enums.DocumentStatus.WaitingGovApproval), Display(Name = "Qty Approved")]
        [UIHint("FormatQty")]
        public decimal QtyApproved { get; set; }

        [RequiredIf("Status", Enums.DocumentStatus.WaitingGovApproval), Display(Name = "Decree Date")]
        [UIHint("FormatDateTime")]
        public DateTime DecreeDate { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public string CreatedById { get; set; }

        public string ApprovedByPoaId { get; set; }
        public string ApprovedByManagerId { get; set; }
        public DateTime? ApprovedByPoaDate { get; set; }
        public DateTime? ApprovedByManagerDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public decimal LatestSaldo { get; set; }

        public string LatestSaldoUomId { get; set; }

        public string LatestSaldoUomName { get; set; }

        public List<Pbck1Item> Pbck1Childs { get; set; }

        public Pbck1Item Pbck1Parent { get; set; }

        public List<Pbck1ProdConvModel> Pbck1ProdConverter { get; set; }

        public List<Pbck1ProdPlanModel> Pbck1ProdPlan { get; set; }

        public string Comment { get; set; }
        
        public List<Pbck1DecreeDocModel> Pbck1DecreeDoc { get; set; }

        //[RequiredIf("Status", Enums.DocumentStatus.WaitingGovApproval), Display(Name = "Decree Doc")]
        public List<HttpPostedFileBase> Pbck1DecreeFiles { get; set; }

        public Enums.DocumentStatusGov DocStatusGov { get; set; }

        public Enums.ActionType GovApprovalActionType { get; set; }
        
    }
}