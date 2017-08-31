using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sampoerna.EMS.Website.Models.Tariff
{
    public class TariffModel : MasterModel
    {
        public TariffModel() : base()
        {

        }

        #region Entity properties mapping
        [Required, Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { set; get; }

        [Required(ErrorMessage = "Product Type Code can't be empty!", AllowEmptyStrings = false), StringLength(2, ErrorMessage = "Product Type Code must be 2 digits", MinimumLength = 2)]
        public String ProductTypeCode { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Minimum HJE is required!")]
        public decimal MinimumHJE { set; get; }
        public string MinimumHjeDisplay { set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Maximum HJE is required!")]
        public decimal MaximumHJE { set; get; }
        public string MaximumHjeDisplay { set; get; }

        [Required(ErrorMessage = "Valid Start Date is required!")]
        public DateTime ValidStartDate { set; get; }

        [Required(ErrorMessage = "Valid End Date is required!")]
        public DateTime ValidEndDate{ set; get; }

        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Entry can't be zero or less")]
        [Required(ErrorMessage = "Tariff value is required!")]
        public decimal Tariff { set; get; }
        public string TariffDisplay { set; get; }
        [Required(ErrorMessage = "Approval status is required!")]
        public long ApprovalStatus { set; get; }
        public String CreatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public String ModifiedBy { set; get; }
        public DateTime? ModifiedDate { set; get; }
        public String ApprovedBy { set; get; }
        public DateTime? ApprovedDate { set; get; }

        public ReferenceModel ApprovalStatusDescription { set; get; }
        #endregion

        #region Helper Properties
        public ProductType.ProductTypeFormViewModel ProductType { set; get; }
        public UserModel Creator { set; get; }
        public UserModel Approver { set; get; } 
        #endregion
    }
}