using Sampoerna.EMS.Website.Models.FinanceRatio;
using System;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ChangeRequest
{
    public class ChangeRequestFormModel : BaseModel
    {
        public ChangeRequestFormModel() : base()
        {
            this.POA = new ChangeRequestPOA();
            this.NPPBKC = new ChangeRequestNppbkc();
            //this.FinancialStatements = new FinanceRatioModel[2];
            //this.FinancialStatements[0] = new FinanceRatioModel();
            //this.FinancialStatements[1] = new FinanceRatioModel();
            this.RequestDate = DateTime.Now;
            this.ViewModel = new ChangeRequestModel();
        }

        #region Helper Models
        public ChangeRequestPOA POA { set; get; }
        public ChangeRequestNppbkc NPPBKC { set; get; }
        public FinanceRatioModel[] FinancialStatements { set; get; }
        #endregion

        #region Form Controls
        public SelectList DocumentTypes { set; get; }
        public DateTime RequestDate { set; get; }
        public SelectList GuaranteeTypes { set; get; }
        public SelectList NppbkcList { set; get; }
        //public List<ChangeRequestSupportingDocument> SupportingDocuments { set; get; }
        //public List<HttpPostedFileBase> OtherSupportingDocuments { set; get; }
        #endregion

        public ChangeRequestModel ViewModel { set; get; }


    }
}