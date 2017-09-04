using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sampoerna.EMS.Website.Models.FinanceRatio
{
    public class FinanceRatioModel : BaseModel
    {
        #region Entity properties mapping
        [Required, Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { set; get; }

        [Required(ErrorMessage = "Please select a company!"), StringLength(4, ErrorMessage = "Company ID must be 4 digits")]
        public string Bukrs { set; get; }

        [Required(ErrorMessage = "Please select a period!")]
        public int YearPeriod { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Current assets value is required!")]
        public decimal CurrentAssets { set; get; }

        [Required(ErrorMessage = "Current assets value is required!")]
        public string CurrentAssetsDisplay { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Current debts value is required!")]
        public decimal CurrentDebts { set; get; }

        public String CurrentDebtsDisplay { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Total assets value is required!")]
        public decimal TotalAssets { set; get; }

        public String TotalAssetsDisplay { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Total debts value is required!")]
        public decimal TotalDebts { set; get; }

        public String TotalDebtsDisplay { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Net profit assets value is required!")]
        public decimal NetProfit { set; get; }

        public String NetProfitDisplay { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Total capital value is required!")]
        public decimal TotalCapital { set; get; }

        public String TotalCapitalString { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Calculation data not available!")]
        [Required(ErrorMessage = "Calculation data not available!")]
        public decimal LiquidityRatio { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Calculation data not available!")]
        [Required(ErrorMessage = "Calculation data not available!")]
        public decimal SolvencyRatio { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Calculation data not available!")]
        [Required(ErrorMessage = "Calculation data not available!")]
        public decimal RentabilityRatio { set; get; }

        public string CreatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public string LastModifiedBy { set; get; }
        public DateTime LastModifiedDate { set; get; }
        public string LastApprovedBy { set; get; }
        public DateTime ? LastApprovedDate { set; get; }
        public long ApprovalStatus { set; get; }
        #endregion

        #region Helper properties
        public CompanyModel Company { set; get; }
        public UserModel Creator { set; get; }
        public UserModel Approver { set; get; }
        public UserModel LastEditor { set; get; }
        public ReferenceModel ApprovalStatusDescription { set; get; }

        /// <summary>
        /// Property that indicate whether current user is the creator of a form data
        /// </summary>
        public bool IsCreator
        {
            set; get;
        }

        /// <summary>
        /// Property that indicate whether current entry is already submitted for approval
        /// </summary>
        public bool IsSubmitted
        {
            set; get;
        }

        /// <summary>
        /// Property that indicate whether current entry is already approved
        /// </summary>
        public bool IsApproved
        {
            set; get;
        }

        /// <summary>
        /// Property represents revision data message
        /// </summary>
        public Shared.WorkflowHistory RevisionData
        {
            set; get;
        }
        #endregion



    }
}