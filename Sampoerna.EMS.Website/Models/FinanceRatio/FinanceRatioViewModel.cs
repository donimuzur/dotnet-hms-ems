using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.FinanceRatio
{
    /// <summary>
    /// A class that serve as data model for views under module Financial Ratio (~/FinanceRatio)
    /// </summary>
    public class FinanceRatioViewModel : BaseModel
    {
        
        public FinanceRatioViewModel()
        {
            this.ListFinanceRatios = new List<FinanceRatioModel>();
            this.ViewModel = new FinanceRatioModel();
        }

        /// <summary>
        /// Property to support form data binding reperesenting Business Object of Finance Ratio
        ///
        /// </summary>
        public FinanceRatioModel ViewModel
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

        /// <summary>
        /// Property to bind list of financial ratio data displayed on data grid
        /// </summary>
        public List<FinanceRatioModel> ListFinanceRatios
        {
            set; get;
        }

        /// <summary>
        /// Property to bind list of companies data displayed as select list
        /// </summary>
        public SelectList CompanyList
        {
            set; get;
        }

        /// <summary>
        /// Property to bind list of year data displayed as select list
        /// </summary>
        public SelectList YearPeriods
        {
            set; get;
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




    }
}