using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ProductType
{
    public class ProductTypeFormViewModel : BaseModel
    {   
        public string ProdCode { get; set; }        
        [Required(ErrorMessage = "Product Type Required !"), Display(Name = "Product Type")]
        public string ProductType { get; set; }
        [Required(ErrorMessage = "Product Alias Required !"), Display(Name = "Product Alias")]
        public string ProductAlias { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        [Display(Name ="Status")]
        public bool IsDeleted { get; set; }
        [Display(Name ="CK4C Editable")]
        public bool IsCk4CEditable { get; set; }
        public string Ck4CEditable { get; set; }      
        public string LastApprovedBy { get; set; }
        public DateTime? LastApprovedDate { get; set; }
        public long ApprovalStatus { get; set; }
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

    }
}