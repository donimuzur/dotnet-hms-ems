using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.GeneralModel;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.CustomService.Services;

using Sampoerna.EMS.CustomService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using static Sampoerna.EMS.Core.Enums;
using Sampoerna.EMS.Core;


namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration
{
    public class BrandRegistrationReqViewModel : BaseModel
    {
        public BrandRegistrationReqViewModel()
        {
            SystemReferenceService refService = new SystemReferenceService();

            this.ViewModel = new BrandRegistrationReqModel();
            this.ListBrandRegistrationReq = new List<BrandRegistrationReqModel>();
            this.ViewModelProduct = new ProductDevDetailModel();
            this.ListProductDevDetail = new List<ProductDevDetailModel>();
            this.SearchInput = new BrandRegFilterViewModel();

            this.ItemDetail = new List<BrandRegistrationReqDetailModel>();
            this.Item = new List<BrandRegistrationReqDetailModel>();
            this.OtherFileList = new List<FileUpload.FileUploadModel>();
            this.RemovedFilesId = new List<long>();
            this.BrandRegSupportingDocument = new List<BrandRegSupportingDocumentModel>();
            this.File_Other_Name = new List<string>();
            this.File_Other_Path = new List<string>();
            this.ButtonCombination = "Create";
            this.UserAccess = new BrandAccess();
            this.Confirmation = new List<ConfirmDialogModel>();
            this.CurrentAction = "";
            this.WorkflowHistory = new List<WorkflowHistoryViewModel>();
            this.statusDraftNew = refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID;
            this.statusDraftEdit = refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID;
            this.statusWaitingforPOAApproval = refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID;
            this.statusWaitingforPOASKEPApproval = refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID;
            this.statusWaitingforGovApproval = refService.GetRefByKey("WAITING_GOVERNMENT_APPROVAL").REFF_ID;
            this.statusCompleted = refService.GetRefByKey("COMPLETED").REFF_ID;
            this.statusCanceled = refService.GetRefByKey("CANCELED").REFF_ID;
            File_SKEP_Path = new List<string>();
            File_SKEP_Name = new List<string>();
        }

        public Sampoerna.EMS.Core.Enums.BrandRegistrationAction brandAction { get; set; }

        public BrandAccess UserAccess { get; set; }
        public IEnumerable<SelectListItem> ListGovStatus { get; set; }

        public int TempActionBrand { get; set; }
        public BrandRegistrationReqModel ViewModel { get; set; }
        public List<BrandRegistrationReqModel> ListBrandRegistrationReq { get; set; }

        public BrandRegistrationReqDetailModel ViewDetailModel { get; set; }
        public List<BrandRegistrationReqDetailModel> ListBrandRegistrationDetailModel { get; set; }
        //public IEnumerable<BRAND_REGISTRATION_REQ_DETAIL> Item { get; set; }
        public List<BrandRegistrationReqDetailModel> Item { get; set; }

        public ProductDevDetailModel ViewModelProduct { get; set; }
        public List<ProductDevDetailModel> ListProductDevDetail { get; set; }

        public List<BrandRegistrationReqDetailModel> ItemDetail { get; set; }
        public List<FileUpload.FileUploadModel> OtherFileList { get; set; }
        public List<string> File_Other_Name { get; set; }
        public List<HttpPostedFileBase> File_Other { get; set; }
        public List<string> File_Other_Path { get; set; }
        public List<string> File_SKEP_Name { get; set; }
        public List<HttpPostedFileBase> File_SKEP { get; set; }
        public List<string> File_SKEP_Path { get; set; }
        public List<BrandRegSupportingDocumentModel> BrandRegSupportingDocument { get; set; }
        public List<long> RemovedFilesId { set; get; }


        public SelectList NppbkcList { set; get; }
        public SelectList BrandList { set; get; }
        public SelectList ProductTypeList { set; get; }

        public SelectList PoaList { set; get; }
        public POAGeneralModel POA { set; get; }
        public NppbkcGeneralModel NPPBKC { set; get; }
        public PlantGeneralModel Plant { get; set; }
        public SelectList CompanyTierList { set; get; }

        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
        public bool ShowActionOptions { get; set; }
        public bool EnableFormInput { get; set; }
        public bool IsActive { get; set; }
        public bool IsDetail { get; set; }
        public bool EditMode { get; set; }
        public bool IsAdminApprover { get; set; }
        public ConfirmDialogModel ApproveConfirm { get; set; }
        public BrandRegFilterViewModel SearchInput { get; set; }

        public bool IsFormReadOnly { get; set; }
        public Enums.UserRole CurrentRole { get; set; }
        public string ButtonCombination { set; get; }

        public long File_Size { get; set; }

        public int Count_Lamp { get; set; }

        public string ActionName { get; set; }
        public List<ConfirmDialogModel> Confirmation { set; get; }

        public string CurrentAction { get; set; }

        public long statusDraftNew { get; set; }
        public long statusDraftEdit { get; set; }
        public long statusWaitingforPOAApproval { get; set; }
        public long statusWaitingforGovApproval { get; set; }
        public long statusWaitingforPOASKEPApproval { get; set; }
        public long statusCompleted { get; set; }
        public long statusCanceled { get; set; }
        public string BaseUrl { set; get; }

    }

    public class BrandValidationResult
    {
        public BrandValidationResult()
        {
            this.IsValid = true;
            this.ResultMessage = "";
        }
        public Boolean IsValid { set; get; }
        public string  ResultMessage { set; get; }

    }

    public class BrandAccess
    {
        public BrandAccess()
        {
            this.CanCreate = false;
            this.CanEdit = false;
            this.CanSubmit = false;
            this.CanSubmitSKEP = false;
            this.CanApprove = false;
            this.CanView = false;
            this.CanViewSKEP = false;
            this.CanWithdraw = false;
            this.CanCancel = false;
        }
        public Boolean CanCreate { set; get; }
        public Boolean CanEdit { set; get; }
        public Boolean CanSubmit { set; get; }
        public Boolean CanSubmitSKEP { set; get; }
        public Boolean CanApprove { set; get; }
        public Boolean CanView { set; get; }
        public Boolean CanViewSKEP { set; get; }
        public Boolean CanWithdraw { set; get; }
        public Boolean CanCancel { set; get; }

    }
}