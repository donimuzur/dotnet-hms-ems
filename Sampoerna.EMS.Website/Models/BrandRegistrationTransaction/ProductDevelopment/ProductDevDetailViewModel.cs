using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment
{
    public class ProductDevDetailViewModel : BaseModel
    {
        public ProductDevDetailViewModel()
        {
            this.ViewModel = new ProductDevDetailModel();
            this.ListProductDevDetail = new List<ProductDevDetailModel>();
        }

        public ProductDevDetailModel ViewModel { get; set; }
        public List<ProductDevDetailModel> ListProductDevDetail { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public bool ShowActionOptions { get; set; }
        public bool EnableFormInput { get; set; }
        public bool IsActive { get; set; }
        public bool EditMode { get; set; }
        public bool IsAdminApprover { get; set; }
        public ConfirmDialogModel ApproveConfirm { get; set; }
    }
}