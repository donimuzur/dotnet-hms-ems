using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.Website.Models.SupportDoc
{
    public class SupportDocViewModel : BaseModel
    {
        public SupportDocViewModel()
        {
            this.ListSupportDocs = new List<SupportDocModel>();
            this.ViewModel = new SupportDocModel();
        }

        public SupportDocModel ViewModel { set; get; }        
        public List<WorkflowHistoryViewModel> WorkflowHistory { set; get; }     
        public List<SupportDocModel> ListSupportDocs { set; get; }       
        public SelectList CompanyList { set; get; }        
        public IEnumerable<SelectListItem> ListForm { get; set; }       
        public bool ShowActionOptions { set; get; }       
        public bool EnableFormInput { get; set; }       
        public bool IsActive { set; get; }        
        public bool EditMode { set; get; }       
        /// <summary>
        /// Property that indicate whether current user is an approver
        /// </summary>
        public bool IsAdminApprover { set; get; }
       

        /// <summary>
        /// Property represents confirm dialog configuration
        /// </summary>
        public ConfirmDialogModel ApproveConfirm{ set; get;}       

    }

}