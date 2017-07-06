using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.Website.Models.FileUpload;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.SupportDoc;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment
{
    public class ProductDevelopmentViewModel : BaseModel
    {
        public ProductDevelopmentViewModel()
        {
            this.ViewModel = new ProductDevelopmentModel();
            this.DetailModel = new ProductDevDetailModel();
            this.ListProductDevelopment = new List<ProductDevelopmentModel>();
            this.ListProductDevDetail = new List<ProductDevDetailModel>();
            this.SearchInput = new ProductDevFilterViewModel();        
        }
        public int TempAction { get; set; }
        public Enums.ProductDevelopmentAction productAction { get; set; }
        public List<ProductDevelopmentModel> ListProductDevelopment { get; set; }
        public ProductDevelopmentModel ViewModel { get; set; }

        public List<ProductDevDetailModel> ListProductDevDetail { get; set; }
        public ProductDevDetailModel DetailModel { get; set; }
        public IEnumerable<PRODUCT_DEVELOPMENT_DETAIL> Item { get; set; }
        public ProductDevelopmentFilterModel Filter { get; set; }
        public FileUploadModel FileUpload { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public SelectList CompanyList { set; get; }
        public SelectList PlantList { set; get; }
        public SelectList MarketList { set; get; }
        public SelectList BrandList { set; get; }
        public SelectList MaterialListOld { set; get; }
        public SelectList MaterialListNew { get; set; }
        public SelectList PoaList { set; get; }
        public SelectList CreatorList { set; get; }
        public SelectList CountryList { get; set; }
        public SelectList WeekList { set; get; }
        public string Plant { set; get; }
    
        public bool ShowActionOptions { get; set; }
        public bool EnableFormInput { get; set; }
        public bool IsActive { get; set; }
        public bool EditMode { get; set; }
        public bool IsAdminApprover { get; set; }
        public bool IsExciser { get; set; }
        public bool IsCreated { get; set; }
        public ConfirmDialogModel ApproveConfirm { get; set; }

        public List<ProductDevSupportingDocumentModel> SupportingDocuments { set; get; }
        public List<HttpPostedFileBase> OtherSupportingDocuments { set; get; }
        public Decimal FileUploadLimit { set; get; }
        public ProductDevFilterViewModel SearchInput { get; set; }
    }
    public class ProductDevelopmentFilterModel
    {
        public int Year { set; get; }
        public string POA { set; get; }
        public string Creator { set; get; }
        public string KPPBC { set; get; }
        public string CompanyType { set; get; }
    }

    public class vwProductDevelopmentModel
    {
        public string PD_NO { get; set; }
        public long PD_ID { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_DateString
        {
            get
            {
                return Created_Date.ToString("dd MMM yyyy");
            }
        }
        public string Modified_By { get; set; }
        public DateTime? Modified_Date { get; set; }
        public string Creator { set; get; }
        public string LastEditor { set; get; }
        public int Next_Action { get; set; }

        public long PD_DETAIL_ID { get; set; }
        public string Fa_Code_Old { get; set; }
        public string Fa_Code_New { get; set; }
        public string Hl_Code { get; set; }
        public string Market_Id { get; set; }
        public string Fa_Code_Old_Desc { get; set; }
        public string Fa_Code_New_Desc { get; set; }
        public string Werks { get; set; }
        public bool Is_Import { get; set; }        
        public string Request_No { get; set; }
        public string Bukrs { get; set; }
        public string Approved_By { get; set; }
        public DateTime? Approved_Date { get; set; }
        public long Approval_Status { get; set; }        
        public string CreatedById { get; set; }
        public string Approver { get; set; }
        public ReferenceModel ApprovalStatusDescription { set; get; }
        public string LastStatus { get; set; }
        public string MarketDesc { get; set; }
        public string PlantName { get; set; }
        public string CompanyName { get; set; }
    }

}