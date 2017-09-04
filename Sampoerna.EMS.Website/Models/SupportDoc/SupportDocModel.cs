using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sampoerna.EMS.Website.Models.SupportDoc
{
    public class SupportDocModel : BaseModel
    {
        #region Entity properties mapping
        [Required, Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long DocumentID { set; get; }

        [Required(ErrorMessage = "Please select a company!"), StringLength(4, ErrorMessage = "Company ID must be 4 digits")]
        public string Bukrs { set; get; }

        [Required, Display(Name = "Form Name")]
        public long FormID { get; set; }

        [Required(ErrorMessage = "Please enter the supporting document name!"), StringLength(50, ErrorMessage = "Maximum character is 50 characters")]
        public string SupportDocName { get; set; }
        
        [Display(Name = "Status Active")]
        public bool IsActive { get; set; }
        
        public string CreatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public string LastModifiedBy { set; get; }
        public DateTime LastModifiedDate { set; get; }
        public string LastApprovedBy { set; get; }
        public DateTime? LastApprovedDate { set; get; }
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
        public bool IsCreator { set; get; }

        /// <summary>
        /// Property that indicate whether current entry is already submitted for approval
        /// </summary>
        public bool IsSubmitted { set; get; }

        /// <summary>
        /// Property that indicate whether current entry is already approved
        /// </summary>
        public bool IsApproved { set; get; }

        /// <summary>
        /// Property represents revision data message
        /// </summary>
        public Shared.WorkflowHistory RevisionData { set; get; }
        
        #endregion

    }
}