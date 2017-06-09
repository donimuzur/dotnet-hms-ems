using Sampoerna.EMS.Website.Models.FinanceRatio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditFormModel : BaseModel
    {
        public ExciseCreditFormModel() : base()
        {
            this.POA = new ExciseCreditPOA();
            this.NPPBKC = new ExciseCreditNppbkc();
            this.FinancialStatements = new FinanceRatioModel[2];
            this.FinancialStatements[0] = new FinanceRatioModel();
            this.FinancialStatements[1] = new FinanceRatioModel();
            this.SubmissionDate = DateTime.Now;
            this.ViewModel = new ExciseCreditModel();
            this.CalculationDetail = new CalculationDetailModel();
            this.SkepInput = new ExciseGovApprovalModel();
            this.Printouts = new List<ExciseCreditPrintoutModel>();
        }

        #region Helper Models
        public ExciseCreditPOA POA { set; get; }
        public ExciseCreditNppbkc NPPBKC { set; get; }
        public FinanceRatioModel[] FinancialStatements { set; get; }
        #endregion

        #region Form Controls
        public SelectList RequestTypes { set; get; }
        public DateTime SubmissionDate { set; get; }
        public SelectList GuaranteeTypes { set; get; }
        public SelectList NppbkcList { set; get; }
        public List<ExciseCreditSupportingDocument> SupportingDocuments { set; get; }
        public List<HttpPostedFileBase> OtherSupportingDocuments { set; get; }
        public CalculationDetailModel CalculationDetail { set; get; }
        public Double FileUploadLimit { set; get; }
        #endregion

        public ExciseCreditModel ViewModel { set; get; }

        public List<WorkflowHistory.WorkflowHistoryViewModel> WorkflowHistory
        { set; get; }

        /// <summary>
        /// Property represents confirm dialog configuration
        /// </summary>
        public Shared.ConfirmDialogModel ApproveConfirm
        {
            set; get;
        }

        public ExciseGovApprovalModel SkepInput { set; get; }

        public List<ExciseCreditPrintoutModel> Printouts { set; get; }

    }
}