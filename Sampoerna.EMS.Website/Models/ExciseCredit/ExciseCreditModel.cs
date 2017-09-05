using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditModel : MasterModel
    {
        public ExciseCreditModel() : base()
        {
            Details = new List<ExciseCreditCK1DetailModel>();
            FinanceRatios = new List<FinanceRatioModel>();
            ApprovedProducts = new List<ExciseApprovedProduct>();

        }
        public long Id { set; get; }
        public string POA { set; get; }
        public string DocumentNumber { set; get; }
        public int RequestTypeID { set; get; }
        public string RequestType { set; get; }
        public DateTime SubmissionDate { set; get; }
        public string NppbkcId { set; get; }
        public decimal Amount { set; get; }
        public string AmountDisplay { set; get; }
        public decimal CalculatedAdjustment { set; get; }
        public string CalculatedAdjustmentDisplay { set; get; }
        public string Guarantee { set; get; }
        public long LastStatus { set; get; }
        public ReferenceModel ApprovalStatus { set; get; }
        public bool? SkepLastStatus { set; get; }
        public ReferenceModel SkepStatus { set; get; }
        public string CreatedBy { set; get; }
        public UserModel Creator { set; get; }
        public DateTime CreatedDate { set; get; }
        public string ModifiedBy { set; get; }
        public UserModel Editor { set; get; }
        public DateTime? ModifiedDate { set; get; }
        public string ApprovedBy { set; get; }
        public UserModel Approver { set; get; }
        public DateTime? ApprovedDate { set; get; }
        public string DecreeNumber { set; get; }
        public DateTime? DecreeDate { set; get; }
        public DateTime? DecreeStartDate { set; get; }
        public string BpjNumber { set; get; }
        public DateTime? BpjDate { set; get; }
        public string BpjAttachmentUrl { set; get; }
        public string Notes { set; get; }
        public string FinancialRatioIds { set; get; }
        public List<FinanceRatioModel> FinanceRatios { set; get; }
        public List<ExciseCreditCK1DetailModel> Details { set; get; }
        public List<ExciseApprovedProduct> ApprovedProducts { set; get; }
        public bool IsApprover { set; get; }
        public bool IsWaitingForGovernment { set; get; }
        public bool IsWaitingSkepApproval { set; get; }
        public bool IsCanceled { set; get; }

        public bool IsAdmin { set; get; }
    }
}