using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ProductType
{
    public class ProductTypeIndexViewModel : BaseModel
    {
        public ProductTypeIndexViewModel()
        {
            this.ListProductTypes = new List<ProductTypeFormViewModel>();
            this.ViewModel = new ProductTypeFormViewModel();
        }
        public List<ProductTypeFormViewModel> ListProductTypes { get; set; }
        public ProductTypeFormViewModel ViewModel { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public bool ShowActionOptions { get; set; }
        public bool EnableFormInput { get; set; }
        public bool IsActive { get; set; }
        public bool EditMode { get; set; }
        public bool IsAdminApprover { get; set; }
        public ConfirmDialogModel ApproveConfirm { get; set; }
    }
}