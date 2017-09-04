using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class MasterViewModel : BaseModel
    {
        public MasterViewModel() : base()
        {

        }

        /// <summary>
        /// Property to handle whether action list button displayed to user or not
        /// </summary>
        public bool ShowActionOptions
        {
            set; get;
        }

        /// <summary>
        /// Property to handle whether user able to edit form input or not
        /// </summary>
        public bool EnableFormInput
        {
            set; get;
        }

        /// <summary>
        /// Property that indicate which form to display to user, edit form or create form
        /// </summary>
        public bool EditMode
        {
            set; get;
        }

        /// <summary>
        /// Property that indicate whether current user is an approver
        /// </summary>
        public bool IsAdminApprover
        {
            set; get;
        }

        /// <summary>
        /// Property represents confirm dialog configuration
        /// </summary>
        public ConfirmDialogModel ApproveConfirm
        {
            set; get;
        }
        /// <summary>
        /// Property to bind workflow history data displayed on data grid
        /// </summary>
        public List<WorkflowHistoryViewModel> WorkflowHistory
        {
            set; get;
        }
    }
}