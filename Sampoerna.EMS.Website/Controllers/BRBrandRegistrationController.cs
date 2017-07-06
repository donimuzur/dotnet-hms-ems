using Microsoft.Ajax.Utilities;
using AutoMapper;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.Market;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.CustomService.Services.BrandRegistrationTransaction;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP;
using Sampoerna.EMS.Website.Helpers;


using Sampoerna.EMS.Website.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.GeneralModel;
using System.Net;
using System.Web;
using Sampoerna.EMS.Website.Utility;
using System.Globalization;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.FileUpload;
using Sampoerna.EMS.Website.Helpers;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using SpreadsheetLight;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;




namespace Sampoerna.EMS.Website.Controllers
{
    public class BRBrandRegistrationController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        private ProductDevelopmentService productDevelopmentService;
        private BrandRegistrationService brandRegistrationService;
        private PenetapanSKEPService penetapanSKEPService;
        private NppbkcManagementService nppbkcService;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBLL;
        private BrandRegistrationReqModel BRModel;
        private BrandRegistrationReqViewModel BRViewModel;
        private IDocumentSequenceNumberBLL _docbll;
        private IChangesHistoryBLL chBLL;
        private IWorkflowHistoryBLL whBLL;



        int ActionType;
        Enum ApprovalStatus;

        public BRBrandRegistrationController(IPageBLL pageBLL, IZaidmExNPPBKCBLL nppbkcbll, IChangesHistoryBLL changesHistoryBll, IWorkflowHistoryBLL workflowHistoryBLL, IDocumentSequenceNumberBLL docbll) : base(pageBLL, Enums.MenuList.BrandRegistrationTransaction)
        {
            this.mainMenu = Enums.MenuList.BrandRegistrationTransaction;
            this.refService = new SystemReferenceService();
            this.productDevelopmentService = new ProductDevelopmentService();
            this.brandRegistrationService = new BrandRegistrationService();
            this.penetapanSKEPService = new PenetapanSKEPService();
            this.nppbkcService = new NppbkcManagementService();
            this._nppbkcbll = nppbkcbll;
            this._changesHistoryBll = changesHistoryBll;
            this._workflowHistoryBLL = workflowHistoryBLL;
            this.ActionType = 0;
            BRModel = new BrandRegistrationReqModel();
            BRViewModel = new BrandRegistrationReqViewModel();
            this.chBLL = changesHistoryBll;
            this.whBLL = workflowHistoryBLL;
            _docbll = docbll;

        }

        #region Brand Registration

        #region User Access and Validation

        public BrandAccess GetUserAccess(long BrandRegId, long intLastApprovedStatus, string CreatedBy, string LastApprovedBy, string Action, string NPPBKCId = "")
        {
            BrandAccess result = new BrandAccess();

            if (intLastApprovedStatus == 0)
            {
                switch (CurrentUser.UserRole)
                {
                    case Enums.UserRole.Administrator:
                        result.CanCreate = true;
                        break;

                    case Enums.UserRole.POA:
                        result.CanCreate = true;
                        break;

                }
            }
            else
            {
                var LastApprovedStatus = brandRegistrationService.GetReffById(intLastApprovedStatus).REFF_KEYS;

                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    result.CanView = true;
                }
                else
                {
                    switch (Action)
                    {
                        case "Index":
                            switch (CurrentUser.UserRole)
                            {
                                case Enums.UserRole.Administrator:
                                    switch (LastApprovedStatus)
                                    {
                                        //DRAFT
                                        case "DRAFT_NEW_STATUS":
                                        case "DRAFT_EDIT_STATUS":
                                            result.CanEdit = true;
                                            result.CanView = true;
                                            break;

                                        //WAITING FOR APPROVAL
                                        case "WAITING_POA_APPROVAL":
                                        case "WAITING_POA_SKEP_APPROVAL":
                                        case "WAITING_GOVERNMENT_APPROVAL":
                                        case "COMPLETED":
                                            result.CanView = true;
                                            break;
                                    }
                                    break;

                                case Enums.UserRole.POA:
                                    switch (LastApprovedStatus)
                                    {
                                        //DRAFT
                                        case "DRAFT_NEW_STATUS":
                                        case "DRAFT_EDIT_STATUS":
                                            if (CreatedBy == CurrentUser.USER_ID)
                                            {
                                                result.CanCreate = true;
                                                result.CanEdit = true;
                                                result.CanSubmit = true;
                                                result.CanView = true;
                                            }

                                            break;

                                        //WAITING FOR APPROVAL
                                        case "WAITING_POA_APPROVAL":
                                            if (CreatedBy != CurrentUser.USER_ID)
                                            {
                                                result.CanCreate = true;
                                                //result.CanView = true;

                                                var POAApprover = brandRegistrationService.GetPOAApproverList(BrandRegId).ToList();
                                                if (POAApprover.Count() > 0)
                                                {
                                                    List<string> ListPOA = POAApprover.Select(s => s.POA_ID.ToUpper()).ToList();
                                                    if (ListPOA.Contains(CurrentUser.USER_ID.ToUpper()))
                                                    {
                                                        result.CanApprove = true;
                                                        result.CanView = true;
                                                    }
                                                    else
                                                    {
                                                        result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);
                                                    }
                                                }
                                                else
                                                {
                                                    result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);
                                                }
                                            }
                                            else
                                            {
                                                result.CanCreate = true;
                                                result.CanView = true;
                                            }
                                            break;

                                        //WAITING FOR SKEP APPROVAL
                                        case "WAITING_POA_SKEP_APPROVAL":
                                            if (LastApprovedBy == CurrentUser.USER_ID)
                                            {
                                                result.CanView = true;
                                                result.CanApprove = true;
                                            }
                                            else
                                            {
                                                result.CanCreate = true;
                                                //result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);

                                                var POAApprover = brandRegistrationService.GetPOAApproverList(BrandRegId).ToList();
                                                if (POAApprover.Count() > 0)
                                                {
                                                    List<string> ListPOA = POAApprover.Select(s => s.POA_ID.ToUpper()).ToList();
                                                    if (ListPOA.Contains(CurrentUser.USER_ID.ToUpper()))
                                                    {
                                                        result.CanView = true;
                                                    }
                                                    else
                                                    {
                                                        result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);
                                                    }
                                                }
                                                else
                                                {
                                                    result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);
                                                }


                                            }
                                            break;

                                        //Waiting for Government Approval
                                        case "WAITING_GOVERNMENT_APPROVAL":
                                            if (CreatedBy == CurrentUser.USER_ID)
                                            {
                                                result.CanCreate = true;
                                                result.CanEdit = true;
                                                result.CanSubmitSKEP = true;
                                                result.CanView = true;
                                            }
                                            else
                                            {
                                                //result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);

                                                var POAApprover = brandRegistrationService.GetPOAApproverList(BrandRegId).ToList();
                                                if (POAApprover.Count() > 0)
                                                {
                                                    List<string> ListPOA = POAApprover.Select(s => s.POA_ID.ToUpper()).ToList();
                                                    if (ListPOA.Contains(CurrentUser.USER_ID.ToUpper()))
                                                    {
                                                        result.CanView = true;
                                                    }
                                                    else
                                                    {
                                                        result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);
                                                    }
                                                }
                                                else
                                                {
                                                    result.CanView = brandRegistrationService.CheckPOANPPBKC(NPPBKCId, CurrentUser.USER_ID);
                                                }

                                            }
                                            break;

                                        case "COMPLETED":
                                            if ((CreatedBy == CurrentUser.USER_ID) || (LastApprovedBy == CurrentUser.USER_ID))
                                            {
                                                result.CanView = true;
                                            }
                                            break;


                                    }
                                    break;


                            }
                            break;

                        case "Detail":
                            result.CanView = true;
                            switch (CurrentUser.UserRole)
                            {
                                case Enums.UserRole.Administrator:
                                    switch (LastApprovedStatus)
                                    {
                                        //WAITING FOR APPROVAL
                                        case "WAITING_POA_SKEP_APPROVAL":
                                        case "WAITING_GOVERNMENT_APPROVAL":
                                        case "COMPLETED":
                                            result.CanViewSKEP = true;
                                            break;
                                    }
                                    break;

                                case Enums.UserRole.POA:
                                    switch (LastApprovedStatus)
                                    {
                                        case "WAITING_POA_SKEP_APPROVAL":
                                        case "WAITING_GOVERNMENT_APPROVAL":
                                        case "COMPLETED":
                                            result.CanViewSKEP = true;
                                            break;
                                    }
                                    break;

                            }

                            break;


                        case "Edit":
                        case "EditSKEP":
                            switch (CurrentUser.UserRole)
                            {
                                case Enums.UserRole.Administrator:
                                    switch (LastApprovedStatus)
                                    {
                                        //DRAFT NEW
                                        case "DRAFT_NEW_STATUS":
                                        case "DRAFT_EDIT_STATUS":
                                            result.CanCreate = true;
                                            result.CanEdit = true;
                                            result.CanView = true;
                                            break;

                                        //WAITING FOR APPROVAL
                                        case "WAITING_POA_APPROVAL":
                                            result.CanView = true;
                                            break;

                                        case "WAITING_POA_SKEP_APPROVAL":
                                        case "WAITING_GOVERNMENT_APPROVAL":
                                            result.CanView = true;
                                            result.CanViewSKEP = true;
                                            break;
                                    }
                                    break;

                                case Enums.UserRole.POA:
                                    switch (LastApprovedStatus)
                                    {
                                        case "DRAFT_NEW_STATUS":
                                            if (CreatedBy == CurrentUser.USER_ID)
                                            {
                                                result.CanCreate = true;
                                                result.CanEdit = true;
                                                result.CanSubmit = true;
                                                result.CanCancel = true;
                                            }
                                            result.CanView = true;
                                            break;

                                        case "DRAFT_EDIT_STATUS":
                                            if (CreatedBy == CurrentUser.USER_ID)
                                            {
                                                result.CanCreate = true;
                                                result.CanEdit = true;
                                                result.CanSubmit = true;
                                                result.CanCancel = true;
                                            }
                                            result.CanView = true;
                                            break;


                                        //WAITING FOR APPROVAL
                                        case "WAITING_POA_APPROVAL":
                                            if (CreatedBy != CurrentUser.USER_ID)
                                            {
                                                result.CanView = true;
                                                result.CanApprove = true;
                                            }
                                            break;

                                        case "WAITING_POA_SKEP_APPROVAL":
                                            if (CreatedBy != CurrentUser.USER_ID)
                                            {
                                                result.CanView = true;
                                                result.CanViewSKEP = true;
                                                result.CanApprove = true;
                                            }
                                            break;

                                        //WAITING FOR GOV APPROVAL
                                        case "WAITING_GOVERNMENT_APPROVAL":
                                            if (CreatedBy == CurrentUser.USER_ID)
                                            {
                                                result.CanView = true;
                                                result.CanSubmitSKEP = true;
                                                result.CanViewSKEP = true;
                                                result.CanWithdraw = true;
                                                result.CanCancel = true;
                                            }
                                            break;
                                    }
                                    break;

                            }
                            break;

                        case "Approval":
                            switch (CurrentUser.UserRole)
                            {
                                case Enums.UserRole.Administrator:
                                    result.CanCreate = false;
                                    result.CanEdit = false;
                                    result.CanSubmit = false;
                                    result.CanView = true;
                                    result.CanApprove = false;
                                    break;

                                case Enums.UserRole.POA:
                                    result.CanCreate = false;
                                    result.CanEdit = false;
                                    result.CanSubmit = false;
                                    result.CanView = true;

                                    switch (LastApprovedStatus)
                                    {
                                        case "WAITING_POA_APPROVAL":
                                            var POAApprover = brandRegistrationService.GetPOAApproverList(BrandRegId).ToList();
                                            if (POAApprover.Count() > 0)
                                            {
                                                List<string> ListPOA = POAApprover.Select(s => s.POA_ID.ToUpper()).ToList();
                                                if (ListPOA.Contains(CurrentUser.USER_ID.ToUpper()))
                                                {
                                                    result.CanApprove = true;
                                                }
                                            }
                                            break;

                                        case "WAITING_POA_SKEP_APPROVAL":
                                            result.CanViewSKEP = true;

                                            if (LastApprovedBy == CurrentUser.USER_ID)
                                            {
                                                result.CanApprove = true;
                                            }
                                            break;
                                    }
                                    break;

                            }
                            break;


                    }

                }

            }



            return result;
        }

        #endregion

        #region Local Helper Brand Registration Req

        public string TerbilangLong2(double amount)
        {
            string word = "";
            double divisor = 1000000000000.00; double large_amount = 0;
            double tiny_amount = 0;
            double dividen = 0; double dummy = 0;
            string weight1 = ""; string unit = ""; string follower = "";
            string[] prefix = { "SE", "DUA ", "TIGA ", "EMPAT ", "LIMA ",
 "ENAM ", "TUJUH ", "DELAPAN ", "SEMBILAN " };
            string[] sufix = { "SATU ", "DUA ", "TIGA ", "EMPAT ", "LIMA ",
 "ENAM ", "TUJUH ", "DELAPAN ", "SEMBILAN " };
            large_amount = Math.Abs(Math.Truncate(amount));
            tiny_amount = Math.Round((Math.Abs(amount) - large_amount) * 100);
            if (large_amount > divisor)
                return "OUT OF RANGE";
            while (divisor >= 1)
            {
                dividen = Math.Truncate(large_amount / divisor);
                large_amount = large_amount % divisor;
                unit = "";
                if (dividen > 0)
                {
                    if (divisor == 1000000000000.00)
                        unit = "TRILYUN ";
                    else
                    if (divisor == 1000000000.00)
                        unit = "MILYAR ";
                    else
                    if (divisor == 1000000.00)
                        unit = "JUTA ";
                    else
                    if (divisor == 1000.00)
                        unit = "RIBU ";
                }
                weight1 = "";
                dummy = dividen;
                if (dummy >= 100)
                    weight1 = prefix[(int)Math.Truncate(dummy / 100) - 1] + "RATUS ";
                dummy = dividen % 100;
                if (dummy < 10)
                {
                    if (dummy == 1 && unit == "RIBU ")
                        weight1 += "SE";
                    else
                    if (dummy > 0)
                        weight1 += sufix[(int)dummy - 1];
                }
                else
                if (dummy >= 11 && dummy <= 19)
                {
                    weight1 += prefix[(int)(dummy % 10) - 1] + "BELAS ";
                }
                else
                {
                    weight1 += prefix[(int)Math.Truncate(dummy / 10) - 1] + "PULUH ";
                    if (dummy % 10 > 0)
                        weight1 += sufix[(int)(dummy % 10) - 1];
                }
                word += weight1 + unit;
                divisor /= 1000.00;
            }
            if (Math.Truncate(amount) == 0)
                word = "NOL ";
            follower = "";
            if (tiny_amount < 10)
            {
                if (tiny_amount > 0)
                    follower = "KOMA NOL " + sufix[(int)tiny_amount - 1];
            }
            else
            {
                follower = "KOMA " + sufix[(int)Math.Truncate(tiny_amount / 10) - 1];
                if (tiny_amount % 10 > 0)
                    follower += sufix[(int)(tiny_amount % 10) - 1];
            }
            word += follower;
            //if (amount < 0)
            //{
            //    word = "MINUS " + word + " RUPIAH";
            //}
            //else
            //{
            //    word = word + " RUPIAH";
            //}
            return word.Trim();
        }
        private BrandRegistrationReqViewModel GeneratePropertiesBrand(BrandRegistrationReqViewModel source, bool update)
        {

            var nppbkclist = brandRegistrationService.GetNPPBKCByUser(CurrentUser.USER_ID);

            var data = source;
            //    var history = refService.GetChangesHistory((int)Enums.MenuList.BrandRegistrationTransaction, id).ToList();
            //  var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.BrandRegistrationTransaction, Int64.Parse(id)).ToList();
            if (!update || data == null)
            {
                data = new BrandRegistrationReqViewModel();
            }

            //data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            //data.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
            data.MainMenu = mainMenu;
            data.CurrentMenu = PageInfo;
            data.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            data.POA = MapPoaModel(refService.GetPOA(CurrentUser.USER_ID));
            data.ShowActionOptions = data.IsNotViewer;
            data.EditMode = false;
            data.EnableFormInput = true;
            data.ViewModel.IsCreator = false;
            data.ViewModel.Registration_Type = 1;
            data.NppbkcList = GetNppbkcListByUser(nppbkclist);
            data.BrandList = GetBrandist(penetapanSKEPService.getMasterBrand());
            data.ProductTypeList = GetProdTypeList(penetapanSKEPService.getMasterProductType());
            data.CompanyTierList = GetCompanyTierList(refService.GetRefByType("COMPANY_TIER"));
            data.File_Size = GetMaxFileSize();

            data.ViewModel.Submission_Date = DateTime.Now;
            data.ViewModel.Effective_Date = DateTime.Now;
            data.ViewModel.Decree_Date = DateTime.Now;


            IEnumerable<Enums.DocumentStatusGovType2> statusTypes = Enum.GetValues(typeof(Enums.DocumentStatusGovType2)).Cast<Enums.DocumentStatusGovType2>();
            data.ListGovStatus = from form in statusTypes
                                 select new SelectListItem
                                 {
                                     Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(Enums.DocumentStatusGovType2), form.ToString())),
                                     Value = ((int)form).ToString()
                                 };

            //var infoSupport = brandRegTransService.FindSupportDetail(2);
            //data.ListSupportDoc = Mapper.Map<List<SupportDocModel>>(infoSupport);

            //IEnumerable<ProductDevNextAction> actionTypes = Enum.GetValues(typeof(ProductDevNextAction)).Cast<ProductDevNextAction>();
            //var tempAction  = from form in actionTypes
            //                select new SelectListItem
            //                {
            //                    Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(ProductDevNextAction), form.ToString())),
            //                    Value = ((int)form).ToString()
            //                };
            //data.ViewModel.ListAction = tempAction.ToList();

            return data;
        }

        public ActionResult GetSupportingDocumentsBrand(string company)
        {
            var formId = (long)Enums.FormList.BrandReq;
            var docs = refService.GetSupportingDocuments(formId, company);
            return PartialView("_SupportingDocumentBrand", docs.Select(x => MapSupportingDocumentModelBrand(x)));
        }


        [HttpPost]
        //public ActionResult GetSupportingDocuments(string CompanyId, long RegId, bool IsReadonly)
        public ActionResult GetSupportingDocuments(string nppbkc, long RegId, bool IsReadonly)
        {
            var company = "";
            if (nppbkc != "" && nppbkc != null)
            {
                company = refService.GetNppbkc(nppbkc).COMPANY.BUKRS;
            }
            var formId = (long)Enums.FormList.BrandReq;
            var docs = new List<MASTER_SUPPORTING_DOCUMENT>();
            //if (company != "" && company != "0" && company != null)
            //{
            docs = refService.GetSupportingDocuments(formId, company).ToList();
            var model = docs.Select(x => MapSupportingDocumentModel(x)).ToList();
            if (RegId != 0 && RegId != null)
            {
                var Doclist = brandRegistrationService.GetFileUploadByRegID(RegId);
                if (Doclist != null)
                {
                    Doclist = Doclist.Where(w => w.DOCUMENT_ID != null);
                    if (Doclist != null)
                    {
                        List<BrandRegSupportingDocumentModel> listDoc = Doclist.Select(s => new BrandRegSupportingDocumentModel
                        {
                            DocId = s.DOCUMENT_ID,
                            Path = s.PATH_URL,
                            FileUploadId = s.FILE_ID
                        }).ToList();
                        foreach (var doc in listDoc)
                        {
                            var whereModel = model.Where(w => w.Id.Equals(doc.DocId)).FirstOrDefault();
                            if (whereModel != null)
                            {
                                whereModel.FileName = GenerateFileName(doc.Path);
                                whereModel.Path = GenerateURL(doc.Path);                                
                                whereModel.IsBrowseFileEnable = false;
                                whereModel.FileUploadId = doc.FileUploadId;
                            }
                        }
                    }
                }
            }
            foreach (var mod in model)
            {
                mod.IsReadonly = IsReadonly;
            }

            //}
            return PartialView("_SupportingDocuments", model);



            //var formId = (long)Enums.FormList.BrandReq;
            //var docs = refService.GetSupportingDocuments(formId, CompanyId);
            //var model = docs.Select(x => MapSupportingDocumentModel(x)).ToList();
            //if (RegId != 0 && RegId != null)
            //{
            //    var Doclist = brandRegistrationService.GetFileUploadByRegID(RegId);
            //    if (Doclist != null)
            //    {
            //        Doclist = Doclist.Where(w => w.DOCUMENT_ID != null);
            //        if (Doclist != null)
            //        {
            //            List<BrandRegSupportingDocumentModel> listDoc = Doclist.Select(s => new BrandRegSupportingDocumentModel
            //            {
            //                DocId = s.DOCUMENT_ID,
            //                Path = s.PATH_URL,
            //                FileUploadId = s.FILE_ID
            //            }).ToList();
            //            foreach (var doc in listDoc)
            //            {
            //                var whereModel = model.Where(w => w.Id.Equals(doc.DocId)).FirstOrDefault();
            //                if (whereModel != null)
            //                {
            //                    whereModel.Path = doc.Path;
            //                    whereModel.IsBrowseFileEnable = false;
            //                    whereModel.FileUploadId = doc.FileUploadId;
            //                }
            //            }
            //        }
            //    }
            //}
            //foreach (var mod in model)
            //{
            //    mod.IsReadonly = IsReadonly;
            //}
            //return PartialView("_SupportingDocuments", model);
        }
        #endregion

        #region Index
        public ActionResult IndexBrandRegistration()
        {
            var model = new BrandRegistrationReqViewModel()
            {
                MainMenu = mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
            };

            try
            {

                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {

                    model.CurrentRole = CurrentUser.UserRole;
                    var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

                    if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                    {
                        var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                        nppbkc = new SelectList(filterNppbkc, "Value", "Text");
                    }

                    IEnumerable<Enums.BrandRegistrationAction> actionTypes = Enum.GetValues(typeof(Enums.BrandRegistrationAction)).Cast<Enums.BrandRegistrationAction>();
                    model.SearchInput.ListRegistrationType = from form in actionTypes
                                                             select new SelectListItem
                                                             {
                                                                 Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(Enums.BrandRegistrationAction), form.ToString())),
                                                                 Value = ((int)form).ToString()
                                                             };
                    model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();

                    model.SearchInput.NppbkcIdList = nppbkc;


                    var users = refService.GetAllUser();
                    var poaList = refService.GetAllPOA();
                    var nppbkclist = brandRegistrationService.GetNPPBKCByUser(CurrentUser.USER_ID);
                    //var documents = new REPLACEMENT_DOCUMENTS();
                    var documents = new List<BrandRegistrationReqModel>();
                    var lists = brandRegistrationService.GetAll().Where(w => ((w.LASTAPPROVED_STATUS != refService.GetRefByKey("COMPLETED").REFF_ID) && (w.LASTAPPROVED_STATUS != refService.GetRefByKey("CANCELED").REFF_ID)));
                    if (lists.Any())
                    {
                        switch (CurrentUser.UserRole)
                        {
                            case Enums.UserRole.Administrator:
                                break;

                            case Enums.UserRole.POA:
                                //lists = lists.Where(w => (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) :
                                //                                                        w.CREATED_BY.Equals(CurrentUser.USER_ID)));
                                break;

                            case Enums.UserRole.Viewer:
                                break;
                        }
                    }

                    if (lists.Any())
                    {
                        documents = lists.Select(s => new BrandRegistrationReqModel
                        {
                            Created_By = s.CREATOR.FIRST_NAME + " " + s.CREATOR.LAST_NAME,
                            Registration_ID = s.REGISTRATION_ID,
                            Registration_No = s.REGISTRATION_NO,
                            strSubmission_Date = Convert.ToDateTime(s.SUBMISSION_DATE).ToString("dd MMM yyyy"),
                            strEffective_Date = s.EFFECTIVE_DATE.ToString("dd MMM yyyy"),
                            Registration_Type = s.REGISTRATION_TYPE,
                            strRegistration_Type = EnumHelper.GetDescription((Enums.BrandRegistrationAction)s.REGISTRATION_TYPE),
                            Nppbkc_ID = s.NPPBKC_ID,
                            LastApproved_Status = s.LASTAPPROVED_STATUS,
                            Company = MapNppbkcModel(refService.GetNppbkc(s.NPPBKC_ID)).Company,
                            strLastApproved_Status = refService.GetReferenceById(s.LASTAPPROVED_STATUS).REFF_VALUE,
                            LastModified_Date = Convert.ToDateTime(s.LASTMODIFIED_DATE),
                            //IsApprover = IsPOACanApprove(s.REGISTRATION_ID, CurrentUser.USER_ID, s.LASTAPPROVED_BY == null ? "" : s.LASTAPPROVED_BY),
                            IsCreator = (s.CREATED_BY == CurrentUser.USER_ID) ? true : false,
                            UserAccess = GetUserAccess(s.REGISTRATION_ID, s.LASTAPPROVED_STATUS, s.CREATED_BY, s.LASTAPPROVED_BY, "Index", s.NPPBKC_ID)
                        }).ToList();
                    }

                    model.ListBrandRegistrationReq = documents;
                }
                else
                {
                    AddMessageInfo("You dont have access to Manufacturing License Request page.", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Unauthorized", "Error");

                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }


            return View("IndexBrandRegistration", model);


        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(BrandRegistrationReqViewModel model)
        {
            var lists = brandRegistrationService.GetAll().Where(w => ((w.LASTAPPROVED_STATUS != refService.GetRefByKey("COMPLETED").REFF_ID) && (w.LASTAPPROVED_STATUS != refService.GetRefByKey("CANCELED").REFF_ID)));

            if (model.SearchInput.RegistrationType != 0)
            {
                lists = lists.Where(w => w.REGISTRATION_TYPE == model.SearchInput.RegistrationType);
            }


            if (model.SearchInput.Creator != null)
            {
                lists = lists.Where(w => w.CREATED_BY == model.SearchInput.Creator);
            }

            var documents = new List<BrandRegistrationReqModel>();

            if (lists.Any())
            {
                documents = lists.Select(s => new BrandRegistrationReqModel
                {
                    Created_By = s.CREATOR.FIRST_NAME + " " + s.CREATOR.LAST_NAME,
                    Registration_ID = s.REGISTRATION_ID,
                    Registration_No = s.REGISTRATION_NO,
                    strSubmission_Date = Convert.ToDateTime(s.SUBMISSION_DATE).ToString("dd MMMM yyyy"),
                    strEffective_Date = s.EFFECTIVE_DATE.ToString("dd MMMM yyyy"),
                    Registration_Type = s.REGISTRATION_TYPE,
                    strRegistration_Type = EnumHelper.GetDescription((Enums.BrandRegistrationAction)s.REGISTRATION_TYPE),
                    Nppbkc_ID = s.NPPBKC_ID,
                    LastApproved_Status = s.LASTAPPROVED_STATUS,
                    Company = MapNppbkcModel(refService.GetNppbkc(s.NPPBKC_ID)).Company,
                    strLastApproved_Status = refService.GetReferenceById(s.LASTAPPROVED_STATUS).REFF_VALUE,
                    LastModified_Date = Convert.ToDateTime(s.LASTMODIFIED_DATE),
                    //IsApprover = IsPOACanApprove(s.REGISTRATION_ID, CurrentUser.USER_ID, s.LASTAPPROVED_BY == null ? "" : s.LASTAPPROVED_BY),
                    IsCreator = (s.CREATED_BY == CurrentUser.USER_ID) ? true : false,
                    UserAccess = GetUserAccess(s.REGISTRATION_ID, s.LASTAPPROVED_STATUS, s.CREATED_BY, s.LASTAPPROVED_BY, "Index", s.NPPBKC_ID)
                }).ToList();
            }

            model.ListBrandRegistrationReq = documents;

            return PartialView("_BrandRegistrationTable", model);
        }


        #endregion

        #region Create
        public ActionResult CreateBrandRegistration()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                return RedirectToAction("IndexBrandRegistration");
            }

            var data = GeneratePropertiesBrand(null, false);
            data.ActionName = "CreateNew";
            data.UserAccess = GetUserAccess(0, 0, CurrentUser.USER_ID, "", "");
            data.Confirmation = GenerateConfirmDialog(true, false, false);


            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateNew(BrandRegistrationReqViewModel model)
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("IndexBrandRegistration");
                }

                var maxFileSize = GetMaxFileSize();
                var isOkFileExt = true;
                var isOkFileSize = true;
                var supportingDocFile = new List<HttpPostedFileBase>();
                if (model.BrandRegSupportingDocument != null)
                {
                    supportingDocFile = model.BrandRegSupportingDocument.Select(s => s.File).ToList();
                }
                isOkFileExt = CheckFileExtension(supportingDocFile);
                if (isOkFileExt)
                {
                    isOkFileExt = CheckFileExtension(model.File_Other);
                    if (isOkFileExt)
                    {
                        //isOkFileExt = CheckFileExtension(model.File_BA);
                    }
                }

                if (isOkFileExt)
                {
                    if (isOkFileSize)
                    {
                        if (model.BrandRegSupportingDocument != null)
                        {
                            foreach (var SuppDoc in model.BrandRegSupportingDocument)
                            {
                                var PathFile = UploadFile(SuppDoc.File);
                                if (PathFile != "")
                                {
                                    SuppDoc.Path = PathFile;
                                }
                            }
                        }

                        if (model.File_Other != null)
                        {
                            foreach (var FileOther in model.File_Other)
                            {
                                var PathFile = UploadFile(FileOther);
                                if (PathFile != "")
                                {
                                    model.File_Other_Path.Add(PathFile);
                                }
                            }
                        }

                        model.ViewModel.LastApproved_Status = refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID;
                        model.ViewModel.NextStatus = refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID;
                        ActionType = (int)Enums.ActionType.Created;


                    }
                    else
                    {
                        AddMessageInfo("Maximum file size is " + maxFileSize.ToString() + " Mb", Enums.MessageInfoType.Warning);
                        return View();
                    }
                }


                var data = MapToTable(model.ViewModel);




                var ActionResult = new BRAND_REGISTRATION_REQ();
                ActionResult = brandRegistrationService.CreateBrand(data, (int)Enums.MenuList.BrandRegistrationReq, ActionType, (int)CurrentUser.UserRole, CurrentUser.USER_ID);

                model.ViewModel.Registration_ID = ActionResult.REGISTRATION_ID;

                if (ActionResult != null)
                {
                    brandRegistrationService.DeleteBrandRegistrationDetail(ActionResult.REGISTRATION_ID);
                    foreach (var item in model.Item)
                    {
                        if (item.PD_Detail_ID > -1)
                        {
                            BRAND_REGISTRATION_REQ_DETAIL detail = new BRAND_REGISTRATION_REQ_DETAIL()
                            {
                                BRAND_CE = item.Brand_Ce,
                                PROD_CODE = item.Prod_Code,
                                COMPANY_TIER = item.Company_Tier,
                                HJE = item.Hje,
                                UNIT = item.Unit,
                                BRAND_CONTENT = item.Brand_Content,
                                TARIFF = item.Tariff,
                                PD_DETAIL_ID = item.PD_Detail_ID,
                                REGISTRATION_ID = ActionResult.REGISTRATION_ID,
                                MATERIAL_PACKAGE = item.Packaging_Material,
                                MARKET_ID = "01",
                                FRONT_SIDE = (item.Front_Side == null) ? "-" : item.Front_Side,
                                BACK_SIDE = (item.Back_Side == null) ? "-" : item.Back_Side,
                                LEFT_SIDE = (item.Left_Side == null) ? "-" : item.Left_Side,
                                RIGHT_SIDE = (item.Right_Side == null) ? "-" : item.Right_Side,
                                TOP_SIDE = (item.Top_Side == null) ? "-" : item.Top_Side,
                                BOTTOM_SIDE = (item.Bottom_Side == null) ? "-" : item.Bottom_Side

                            };
                            brandRegistrationService.CreateBrandDetail(detail);
                        }
                    }


                }


                //// Supporting Doc
                InsertUploadSuppDocFile(model.BrandRegSupportingDocument, model.ViewModel.Registration_ID);
                //// Other Doc
                InsertUploadCommonFile(model.File_Other_Path, model.ViewModel.Registration_ID, false, model.File_Other_Name);

                AddMessageInfo("Successfully Save Brand Registration Document!", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            //model = GenerateModelProperties(model);
            return RedirectToAction("IndexBrandRegistration");
        }



        //[HttpPost]
        //public JsonResult CreateBrand(BrandRegistrationReqViewModel model)
        //{
        //    var formData = Request;
        //    var modelObj = JObject.Parse(formData["model"]);

        //    try
        //    {
        //        var viewModel = JsonConvert.DeserializeObject<BrandRegistrationReqModel>(modelObj.ToString());

        //        BRAND_REGISTRATION_REQ entity = new BRAND_REGISTRATION_REQ()
        //        {
        //            CREATED_DATE = DateTime.Now,
        //            CREATED_BY = CurrentUser.USER_ID,
        //            LASTMODIFIED_DATE = DateTime.Now,
        //            LASTMODIFIED_BY = CurrentUser.USER_ID,
        //            LASTAPPROVED_STATUS = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID,
        //            REGISTRATION_NO = viewModel.Registration_No,
        //            SUBMISSION_DATE = viewModel.Submission_Date,
        //            EFFECTIVE_DATE = viewModel.Effective_Date,
        //            REGISTRATION_TYPE = viewModel.Registration_Type,
        //            NPPBKC_ID = viewModel.Nppbkc_ID

        //       };
        //        //   AddMessageInfo("Successfully Save Product Development Document!", Enums.MessageInfoType.Success);
        //        return Json(brandRegistrationService.CreateBrand(entity, (int)Enums.MenuList.SupportDoc, (int)Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID));
        //    }
        //    catch (Exception ex)
        //    {
        //        AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
        //        Console.WriteLine(ex.StackTrace);
        //        return Json(false);
        //    }
        //}
        #endregion

        #region Edit

        public ActionResult Edit(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(Id, CurrentUser.USER_ID))
            {

                BRViewModel = GetBrandRegistrationData(Id, "Edit");
                BRViewModel.ActionName = "UpdateBrand";
                BRViewModel.Confirmation = GenerateConfirmDialog(true, true, false);


                return View("CreateBrandRegistration", BRViewModel);
            }
            else
            {
                AddMessageInfo("You dont have access to edit this Manufacturing License Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("IndexBrandRegistration");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateBrand(BrandRegistrationReqViewModel model)
        {
            try
            {
                var msgSuccess = "";

                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("IndexBrandRegistration");
                }

                BrandValidationResult ValidadtionResult = FileValidation(model);

                if (ValidadtionResult.IsValid)
                {
                    if (model.BrandRegSupportingDocument != null)
                    {
                        foreach (var SuppDoc in model.BrandRegSupportingDocument)
                        {
                            var PathFile = UploadFile(SuppDoc.File);
                            if (PathFile != "")
                            {
                                SuppDoc.Path = PathFile;
                            }
                        }
                    }

                    if (model.File_Other != null)
                    {
                        foreach (var FileOther in model.File_Other)
                        {
                            var PathFile = UploadFile(FileOther);
                            if (PathFile != "")
                            {
                                model.File_Other_Path.Add(PathFile);
                            }
                        }
                    }

                }

                var data = MapToTable(model.ViewModel);

                switch (brandRegistrationService.GetReffById(model.ViewModel.NextStatus).REFF_KEYS)
                {
                    case "DRAFT_EDIT_STATUS":
                        ActionType = (int)Enums.ActionType.Modified;
                        break;
                    //case refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID:
                    //    ActionType = (int)Enums.ActionType.Submit;
                    //    ApprovalStatus = ReferenceKeys.ApprovalStatus.AwaitingPoaApproval;

                    //    break;
                }

                var IsUpdateSuccess = brandRegistrationService.Edit(data, ActionType, (int)CurrentUser.UserRole);

                if (IsUpdateSuccess)
                {
                    brandRegistrationService.DeleteBrandRegistrationDetail(data.REGISTRATION_ID);
                    foreach (var item in model.Item)
                    {
                        if (item.PD_Detail_ID > -1)
                        {
                            BRAND_REGISTRATION_REQ_DETAIL detail = new BRAND_REGISTRATION_REQ_DETAIL()
                            {
                                BRAND_CE = item.Brand_Ce,
                                PROD_CODE = item.Prod_Code,
                                COMPANY_TIER = item.Company_Tier,
                                HJE = item.Hje,
                                UNIT = item.Unit,
                                BRAND_CONTENT = item.Brand_Content,
                                TARIFF = item.Tariff,
                                PD_DETAIL_ID = item.PD_Detail_ID,
                                REGISTRATION_ID = data.REGISTRATION_ID,
                                MATERIAL_PACKAGE = item.Packaging_Material,
                                MARKET_ID = "01",
                                FRONT_SIDE = (item.Front_Side == null) ? "-" : item.Front_Side,
                                BACK_SIDE = (item.Back_Side == null) ? "-" : item.Back_Side,
                                LEFT_SIDE = (item.Left_Side == null) ? "-" : item.Left_Side,
                                RIGHT_SIDE = (item.Right_Side == null) ? "-" : item.Right_Side,
                                TOP_SIDE = (item.Top_Side == null) ? "-" : item.Top_Side,
                                BOTTOM_SIDE = (item.Bottom_Side == null) ? "-" : item.Bottom_Side


                            };
                            brandRegistrationService.CreateBrandDetail(detail);
                        }
                    }

                    msgSuccess = "Success Save Brand Registration";


                    //if (model.ViewModel.NextStatus == refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID)
                    //{
                    //    msgSuccess = "Success Submit Brand Registration";

                    //    msgSuccess += ProcessMail(data.REGISTRATION_ID, "poa_approver", "approve");
                    //}
                }


                //// Supporting Doc
                InsertUploadSuppDocFile(model.BrandRegSupportingDocument, model.ViewModel.Registration_ID);
                //// Other Doc
                InsertUploadCommonFile(model.File_Other_Path, model.ViewModel.Registration_ID, false, model.File_Other_Name);

                AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);
                return RedirectToAction("IndexBrandRegistration");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            //model = GenerateModelProperties(model);
            return RedirectToAction("IndexBrandRegistration");
        }



        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult SubmitBrand(BrandRegistrationReqViewModel model)
        //{
        //    try
        //    {
        //        var data = brandRegistrationService.FindBrandRegistrationReq(model.ViewModel.Registration_ID);
        //        var dataDetail = brandRegistrationService.FindBrandRegistrationReqDetail(model.ViewModel.Registration_ID);
        //        var sender = refService.GetUserEmail(CurrentUser.USER_ID);
        //        var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminCreator), data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME);
        //        var parameters = new Dictionary<string, string>();


        //        // parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy hh:mm:ss"));
        //        parameters.Add("nppbkc", data.NPPBKC_ID);
        //        parameters.Add("tarifcukai", dataDetail.TARIFF.ToString());
        //        parameters.Add("hjeperpack", dataDetail.HJE.ToString());

        //        parameters.Add("market", dataDetail.ZAIDM_EX_MARKET.MARKET_DESC);

        //        parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));

        //        //parameters.Add("url_detail", Url.Action("Detail", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));
        //        //parameters.Add("url_approve", Url.Action("Approve", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));

        //        var mailContent = refService.GetMailContent((int)ReferenceKeys.EmailContent.BrandRegistrationApprovalRequest, parameters);
        //        var reff = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
        //        var sendToId = reff.REFF_VALUE;
        //        var sendTo = refService.GetUserEmail(sendToId);

        //   //     ExecuteApprovalAction(model, ReferenceKeys.ApprovalStatus.AwaitingAdminApproval, Enums.ActionType.WaitingForApproval, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);
        //    }
        //    catch (Exception ex)
        //    {
        //        AddMessageInfo("Submit Failed : " + ex.Message, Enums.MessageInfoType.Error);
        //    }

        //    return RedirectToAction("IndexBrandRegistration");
        //}


        #endregion

        #region Edit SKEP

        public ActionResult EditSKEP(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(Id, CurrentUser.USER_ID))
            {

                BRViewModel = GetBrandRegistrationData(Id, "EditSKEP");
                BRViewModel.ActionName = "UpdateSKEPBrand";
                BRViewModel.Confirmation = GenerateConfirmDialog(true, true, false);


                return View("CreateBrandRegistration", BRViewModel);
            }
            else
            {
                AddMessageInfo("You dont have access to edit this Manufacturing License Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("IndexBrandRegistration");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateSKEPBrand(BrandRegistrationReqViewModel model)
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("IndexBrandRegistration");
                }

                //BrandValidationResult ValidadtionResult = FileValidation(model);

                //if (ValidadtionResult.IsValid)
                //{
                //    if (model.BrandRegSupportingDocument != null)
                //    {
                //        foreach (var SuppDoc in model.BrandRegSupportingDocument)
                //        {
                //            var PathFile = UploadFile(SuppDoc.File);
                //            if (PathFile != "")
                //            {
                //                SuppDoc.Path = PathFile;
                //            }
                //        }
                //    }

                //    if (model.File_Other != null)
                //    {
                //        foreach (var FileOther in model.File_Other)
                //        {
                //            var PathFile = UploadFile(FileOther);
                //            if (PathFile != "")
                //            {
                //                model.File_Other_Path.Add(PathFile);
                //            }
                //        }
                //    }

                //}

                if (model.File_SKEP != null)
                {
                    var AddedfileBAList = new List<string>();
                    var removedIndex = new List<int>();
                    var index = 0;
                    foreach (var FileBA in model.File_SKEP)
                    {
                        if (AddedfileBAList.Contains(FileBA.FileName))
                        {
                            removedIndex.Add(index);
                        }
                        else
                        {
                            AddedfileBAList.Add(FileBA.FileName);
                            var PathFile = UploadFile(FileBA);
                            if (PathFile != "")
                            {
                                model.File_SKEP_Path.Add(PathFile);
                            }
                        }
                        index++;
                    }
                    removedIndex = removedIndex.OrderByDescending(o => o).ToList();
                    foreach (var i in removedIndex)
                    {
                        model.File_SKEP.RemoveAt(i);
                    }
                }

                var data = MapSKEPToTable(model.ViewModel);
                data.LASTAPPROVED_STATUS = model.ViewModel.NextStatus;

                ActionType = (int)Enums.ActionType.Submit;

                var IsUpdateSuccess = brandRegistrationService.EditSKEP(data, ActionType, (int)CurrentUser.UserRole);

                if (IsUpdateSuccess)
                {
                    InsertUploadCommonFile(model.File_SKEP_Path, data.REGISTRATION_ID, true, model.File_SKEP_Name);
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval;
                    ProcessMail(model.ViewModel.Registration_ID, "poa_approver", "approve");
                    AddMessageInfo("Successfully Submit SKEP Document!", Enums.MessageInfoType.Success);

                }
            }
            catch (Exception ex)
            {
                AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            //model = GenerateModelProperties(model);
            return RedirectToAction("IndexBrandRegistration");
        }


        #endregion


        #region Detail
        public ActionResult Details(Int64 Id = 0)
        {
            BRViewModel = GetBrandRegistrationData(Id, "Detail");
            BRViewModel.ActionName = "";
            
            return View("CreateBrandRegistration", BRViewModel);

        }

        //public ActionResult DetailBrand(long id)
        //{

        //    var data = GeneratePropertiesBrand(null, false);
        //    data.ViewModel = Mapper.Map<BrandRegistrationReqModel>(brandRegistrationService.FindBrandRegistrationReq(Convert.ToInt64(id)));
        //    var history = refService.GetChangesHistory((int)Enums.MenuList.BrandRegistrationTransaction, id.ToString()).ToList();
        //    var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.BrandRegistrationTransaction, id).ToList();
        //    data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
        //    data.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
        //    data.EnableFormInput = false;
        //    data.EditMode = true;

        //    return View(data);
        //}
        #endregion

        #region Approve & Revise

        public ActionResult Approval(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.POA)
            {
                BRViewModel = GetBrandRegistrationData(Id, "Approval");
                BRViewModel.ActionName = "ChangeStatus";
                BRViewModel.Confirmation = GenerateConfirmDialog(false, false, true);

                if (BRViewModel.UserAccess.CanApprove)
                {
                    return View("CreateBrandRegistration", BRViewModel);
                }
                else
                {
                    AddMessageInfo("You dont have access to approve this Brand Registration document.", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            else
            {
                AddMessageInfo("You dont have access to edit this Manufacturing License Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Unauthorized", "Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(BrandRegistrationReqViewModel model)
        {
            try
            {
                var ErrMessage = "";
                var SuccMessage = "";

                if (model.CurrentAction == "submit")
                {
                    SuccMessage = "Success Submit Brand Registration Document";
                    ActionType = (int)Enums.ActionType.Submit;
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.AwaitingPoaApproval;

                }
                else if (model.CurrentAction == "approve")
                {
                    SuccMessage = "Success Approve Brand Registration Document";
                    ActionType = (int)Enums.ActionType.Approve;
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval;

                }
                else if (model.CurrentAction == "reject")
                {
                    SuccMessage = "Success Reject Brand Registration Document";
                    ActionType = (int)Enums.ActionType.Reject;
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.Rejected;
                }
                else if (model.CurrentAction == "revise")
                {
                    SuccMessage = "Success Revise Brand Registration Document";
                    ActionType = (int)Enums.ActionType.Revise;
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.Rejected;
                }
                else if (model.CurrentAction == "cancel")
                {
                    SuccMessage = "Success Cancel Brand Registration Document";
                    ActionType = (int)Enums.ActionType.Cancel;
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.Canceled;
                }
                else if (model.CurrentAction == "withdraw")
                {
                    SuccMessage = "Success Withdraw Brand Registration Document";
                    ActionType = (int)Enums.ActionType.Withdraw;
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.Canceled;

                }

                model.ViewModel.LastApproved_Status = model.ViewModel.NextStatus;
                if (model.ViewModel.LastApproved_Status == refService.GetRefByKey("COMPLETED").REFF_ID)
                {
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.Completed;
                }

                var update = brandRegistrationService.UpdateStatus(model.ViewModel.Registration_ID, model.ViewModel.LastApproved_Status, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, "");

                if (model.ViewModel.LastApproved_Status == refService.GetRefByKey("COMPLETED").REFF_ID)
                {
                    var insert_brand = brandRegistrationService.InsertToBrand(model.ViewModel.Registration_ID, model.ViewModel.Nppbkc_ID);
                }

                if (update)
                {
                    if (model.CurrentAction != "cancel")
                    {
                        switch (model.CurrentAction)
                        {
                            case "submit":
                                SuccMessage += ProcessMail(model.ViewModel.Registration_ID, "poa_approver", "submit");
                                break;

                            //case "withdraw":
                            //    SuccMessage += ProcessMail(model.ViewModel.Registration_ID, "poa_approver", "notification");
                            //    break;

                            default:
                                SuccMessage += ProcessMail(model.ViewModel.Registration_ID, "creator", "notification");
                                break;
                        }
                    }
                }

                AddMessageInfo(SuccMessage, Enums.MessageInfoType.Success);

            }
            catch (Exception ex)
            {
                AddMessageInfo("Change Status Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("IndexBrandRegistration");
        }


        [HttpPost]
        public ActionResult ReviseBrand(long ID, int NextStatus, string Comment)
        {
            try
            {
                int ActionType = 0;
                if (NextStatus != refService.GetRefByKey("CANCELED").REFF_ID)
                {
                    ActionType = (int)Enums.ActionType.Revise;
                }
                else
                {
                    ActionType = (int)Enums.ActionType.Cancel;
                }

                var update = brandRegistrationService.UpdateStatus(ID, NextStatus, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, Comment);

                string ErrMsg = "";
                var msgSuccess = "Success Revise";

                if (update)
                {
                    if (NextStatus != refService.GetRefByKey("CANCELED").REFF_ID)
                    {
                        ApprovalStatus = ReferenceKeys.ApprovalStatus.Rejected;
                        msgSuccess += ProcessMail(ID, "creator", "notification", Comment);
                    }
                    else
                    {
                        ApprovalStatus = ReferenceKeys.ApprovalStatus.Canceled;
                        msgSuccess += ProcessMail(ID, "poa_approver", "notification_withdraw", Comment);
                    }
                }



                AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);

                //return RedirectToAction("IndexBrandRegistration");
                return Json(true);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult WithdrawBrand(long ID, int NextStatus, string Comment)
        {
            try
            {
                int ActionType = 0;
                ActionType = (int)Enums.ActionType.Revise;

                var update = brandRegistrationService.UpdateStatus(ID, NextStatus, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, Comment);

                string ErrMsg = "";
                var msgSuccess = "Success Withdraw";

                if (update)
                {
                    ActionType = (int)Enums.ActionType.Withdraw;
                    ApprovalStatus = ReferenceKeys.ApprovalStatus.Canceled;

                    msgSuccess = ProcessMail(ID, "poa_approver", "notification", Comment);
                }



                AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);

                return RedirectToAction("IndexBrandRegistration");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return Json(ex.Message);
            }
        }


        #endregion


        #region Completed and Summary Reports
        public ActionResult CompletedDocumentBrandRegistration()
        {
            var model = new BrandRegistrationReqViewModel()
            {
                MainMenu = mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
            };

            try
            {

                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {

                    model.CurrentRole = CurrentUser.UserRole;
                    var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

                    if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                    {
                        var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                        nppbkc = new SelectList(filterNppbkc, "Value", "Text");
                    }

                    IEnumerable<Enums.BrandRegistrationAction> actionTypes = Enum.GetValues(typeof(Enums.BrandRegistrationAction)).Cast<Enums.BrandRegistrationAction>();
                    model.SearchInput.ListRegistrationType = from form in actionTypes
                                                             select new SelectListItem
                                                             {
                                                                 Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(Enums.BrandRegistrationAction), form.ToString())),
                                                                 Value = ((int)form).ToString()
                                                             };
                    model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();

                    model.SearchInput.NppbkcIdList = nppbkc;


                    var users = refService.GetAllUser();
                    var poaList = refService.GetAllPOA();
                    var nppbkclist = brandRegistrationService.GetNPPBKCByUser(CurrentUser.USER_ID);
                    //var documents = new REPLACEMENT_DOCUMENTS();
                    var documents = new List<BrandRegistrationReqModel>();
                    var lists = brandRegistrationService.GetAll().Where(w => ((w.LASTAPPROVED_STATUS == refService.GetRefByKey("COMPLETED").REFF_ID) || (w.LASTAPPROVED_STATUS == refService.GetRefByKey("CANCELED").REFF_ID)));
                    if (lists.Any())
                    {
                        switch (CurrentUser.UserRole)
                        {
                            case Enums.UserRole.Administrator:
                                break;

                            case Enums.UserRole.POA:
                                //lists = lists.Where(w => (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) :
                                //                                                        w.CREATED_BY.Equals(CurrentUser.USER_ID)));
                                break;

                            case Enums.UserRole.Viewer:
                                break;
                        }
                    }
                    if (lists.Any())
                    {
                        documents = lists.Select(s => new BrandRegistrationReqModel
                        {
                            Created_By = s.CREATOR.FIRST_NAME + " " + s.CREATOR.LAST_NAME,
                            Registration_ID = s.REGISTRATION_ID,
                            Registration_No = s.REGISTRATION_NO,
                            strSubmission_Date = Convert.ToDateTime(s.SUBMISSION_DATE).ToString("dd MMMM yyyy"),
                            strEffective_Date = s.EFFECTIVE_DATE.ToString("dd MMMM yyyy"),
                            Registration_Type = s.REGISTRATION_TYPE,
                            strRegistration_Type = EnumHelper.GetDescription((Enums.BrandRegistrationAction)s.REGISTRATION_TYPE),
                            Nppbkc_ID = s.NPPBKC_ID,
                            LastApproved_Status = s.LASTAPPROVED_STATUS,
                            Company = MapNppbkcModel(refService.GetNppbkc(s.NPPBKC_ID)).Company,
                            strLastApproved_Status = refService.GetReferenceById(s.LASTAPPROVED_STATUS).REFF_VALUE,
                            LastModified_Date = Convert.ToDateTime(s.LASTMODIFIED_DATE),
                            //IsApprover = IsPOACanApprove(s.REGISTRATION_ID, CurrentUser.USER_ID, s.LASTAPPROVED_BY == null ? "" : s.LASTAPPROVED_BY),
                            IsCreator = (s.CREATED_BY == CurrentUser.USER_ID) ? true : false,
                            UserAccess = GetUserAccess(s.REGISTRATION_ID, s.LASTAPPROVED_STATUS, s.CREATED_BY, s.LASTAPPROVED_BY, "Index")
                        }).ToList();
                    }

                    model.ListBrandRegistrationReq = documents;
                }
                else
                {
                    AddMessageInfo("You dont have access to Manufacturing License Request page.", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Unauthorized", "Error");

                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }


            return View("IndexBrandRegistration", model);



        }

        //public ActionResult SummaryReportsBrandRegistration()
        //{
        //    var model = new BrandRegistrationReqViewModel()
        //    {
        //        MainMenu = mainMenu,
        //        CurrentMenu = PageInfo,
        //        IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
        //    };
        //    return View("SummaryReportsBrandRegistration", model);
        //}
        #endregion


        #region Email
        public bool SendMail(string registration_number, string last_approved_status, string nppbkc_id, string block_details, string comment, string creator, long id, string body_message, string skep_details, List<string> sendto, string MailFor)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("body_message", body_message);
                parameters.Add("skep_details", skep_details);
                parameters.Add("registration_number", registration_number);
                parameters.Add("last_approved_status", last_approved_status);
                parameters.Add("nppbkc_id", nppbkc_id);
                parameters.Add("block_details", block_details);
                parameters.Add("comment", comment);

                parameters.Add("url_detail", Url.Action("Detail", "BRBrandRegistration", new { Id = id }, this.Request.Url.Scheme));

                switch(MailFor)
                {
                    case "submit":
                        parameters.Add("url_approve", Url.Action("Approval", "BRBrandRegistration", new { Id = id }, this.Request.Url.Scheme));
                        parameters.Add("link_message", "APPROVE");
                        break;

                    case "notification":
                        parameters.Add("url_approve", Url.Action("Edit", "BRBrandRegistration", new { Id = id }, this.Request.Url.Scheme));
                        parameters.Add("link_message", "APPROVE");
                        break;

                    case "notification_withdraw":
                        break;

                    default:
                        parameters.Add("url_approve", Url.Action("Approval", "BRBrandRegistration", new { Id = id }, this.Request.Url.Scheme));
                        parameters.Add("link_message", "APPROVE");
                        break;

                }

                long mailcontentId = 0;
                mailcontentId = brandRegistrationService.GetMailId("Brand Registration Request Approval");
                if (MailFor == "submit")
                {
                    mailcontentId = brandRegistrationService.GetMailId("Brand Registration Request Approval");
                }
                else if (MailFor == "approve")
                {
                    mailcontentId = brandRegistrationService.GetMailId("Brand Registration Request Approval");
                }
                else if (MailFor == "revise" || MailFor == "notification_withdraw")
                {
                    mailcontentId = brandRegistrationService.GetMailId("Brand Registration Request Revised");
                }
                else if (MailFor == "reject")
                {
                }

                var mailContent = refService.GetMailContent(mailcontentId, parameters);
                var senderMail = refService.GetUserEmail(CurrentUser.USER_ID);
                string[] arrSendto = sendto.ToArray();
                bool mailStatus = ItpiMailer.Instance.SendEmail(arrSendto, null, null, null, mailContent.EMAILSUBJECT, mailContent.EMAILCONTENT, true, senderMail, creator);
                return mailStatus;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string ProcessMail(long RegistrationID, string SendTo, string MailFor, string Comment = "")
        {
            var BrandRegistrationData = GetBrandRegistrationData(RegistrationID);

            var status = refService.GetReferenceByKey(ApprovalStatus).REFF_VALUE;

            string body_message = "";
            string skep_details = "";

            var strLastApprovedStatus = brandRegistrationService.GetReffById(BrandRegistrationData.ViewModel.LastApproved_Status).REFF_KEYS;

            switch(strLastApprovedStatus)
            {
                case "WAITING_POA_APPROVAL":
                case "WAITING_GOVERNMENT_APPROVAL":
                default:
                    body_message = "New Brand Registration " + BrandRegistrationData.ViewModel.Registration_No + " is ";
                    break;

                case "WAITING_POA_SKEP_APPROVAL":
                    string decree_status = "";
                    if (BrandRegistrationData.ViewModel.Decree_Status)
                    {
                        decree_status = "Approved";
                    }
                    else
                    {
                        decree_status = "Rejected";
                    }
                    body_message = "SKEP Brand Registration " + BrandRegistrationData.ViewModel.Registration_No + " is " + decree_status + " and ";
                    skep_details = "<tr><td>SKEP</td><td>:</td><td>" + BrandRegistrationData.ViewModel.Decree_No + "</td></tr>";
                    break;

                case "CANCELED":
                    body_message = "Brand Registration " + BrandRegistrationData.ViewModel.Registration_No + " is ";
                    //skep_details = "<tr><td>SKEP</td><td>:</td><td>" + BrandRegistrationData.ViewModel.Decree_No + "</td></tr>";
                    break;
            }

            string block_details = "";
            string msgSuccess = "";
            decimal HJEPerStick = 0;
            foreach (var item in BrandRegistrationData.Item)
            {
                if (item.PD_Detail_ID > -1)
                {
                    HJEPerStick = item.Hje / Convert.ToInt32(item.Brand_Content);

                    block_details += "<tr><td>Tarif Cukai</td><td>:</td><td>" + item.Tariff.ToString("#,##") + "</td></tr>";
                    block_details += "<tr><td>HJE per pack</td><td>:</td><td>" + item.Hje.ToString("#,##") + "</td></tr>";
                    block_details += "<tr><td>HJE Per Stick / Gr</td><td>:</td><td>" + HJEPerStick.ToString("#,##0.00") + "</td></tr>";
                    block_details += "<tr><td>Content</td><td>:</td><td>" + Convert.ToInt32(item.Brand_Content).ToString("#,##") + " [" + item.Unit + "]</td></tr>";
                    block_details += "<tr><td>Excisable Good Type</td><td>:</td><td>" + item.ProductType + "</td></tr>";
                    block_details += "<tr><td>Market</td><td>:</td><td>" + item.MarketDesc + "</td></tr>";
                    block_details += "<tr><td colspan='2'>&nbsp;</td></tr>";
                }
            }

            block_details += "<tr><td>Creator</td><td>:</td><td>" + BrandRegistrationData.ViewModel.Created_By_Name + "</td></tr>";

            string rejection_notes = "";
            if (Comment != "")
            {
                if (strLastApprovedStatus == "CANCELED")
                {
                    rejection_notes += "<tr><td>Withdraw Notes</td><td>:</td><td>" + Comment + "</td></tr>";
                }
                else
                {
                    rejection_notes += "<tr><td>Rejection Notes</td><td>:</td><td>" + Comment + "</td></tr>";
                }

            }

            msgSuccess = "";

            List<string> ListPOA = new List<string>();
            switch (SendTo)
            {
                case "creator":
                    ListPOA.Add(BrandRegistrationData.ViewModel.Created_By_Email);
                    break;

                case "poa_approver":
                    var POAApprover = brandRegistrationService.GetPOAApproverList(RegistrationID).ToList();
                    if (POAApprover.Count() > 0)
                    {
                        ListPOA = POAApprover.Where(w => w.POA_EMAIL != "").Select(s => s.POA_EMAIL).ToList();
                    }
                    break;
            }



            var sendmail = SendMail(BrandRegistrationData.ViewModel.Registration_No, status, BrandRegistrationData.ViewModel.Nppbkc_ID, block_details, rejection_notes, BrandRegistrationData.ViewModel.Created_By, BrandRegistrationData.ViewModel.Registration_ID, body_message, skep_details, ListPOA, MailFor);
            if (!sendmail)
            {
                msgSuccess += " , but failed send mail to POA";
            }

            return msgSuccess;
        }

        #endregion

        #region Summary Reports

        public ActionResult SummaryReportsBrandRegistration()
        {

            BrandRegSummaryReportsViewModel model;
            try
            {

                model = new BrandRegSummaryReportsViewModel();

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new BrandRegSummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.BrandRegistrationReq;
                model.CurrentMenu = PageInfo;
            }

            return View("SummaryReportsBrandRegistration", model);
        }

        private BrandRegSummaryReportsViewModel InitSummaryReports(BrandRegSummaryReportsViewModel model)
        {
            model.MainMenu = mainMenu;
            model.CurrentMenu = PageInfo;
            var users = refService.GetAllUser();
            var documents = brandRegistrationService.GetvwBrandRegistrationAll().Select(s => new vwBrandRegistrationModel
            {
                RegistrationNo = s.REGISTRATION_NO,
                SubmissionDate = s.SUBMISSION_DATE,
                CreatedBy = s.CREATED_BY,
                CompanyName = s.BUTXT,
                NPPBKCId = s.NPPBKC_ID,
                RegistrationType = s.REGISTRATION_TYPE == 1 ? "Update HJE" : "Merek Baru",
                EffectiveDate = s.EFFECTIVE_DATE,
                LastApprovedStatusValue = s.REFF_VALUE,
            }).ToList();

            //var iridlist = service.GetIRIDAll();
            //var statusidlist = service.GetStatusIDAll();
            //var nppbkcList = service.GetAllNPPBKCId();
            //var comptplist = interService.GetInterviewReqCompanyTypeList();

            //var filter = new LicenseRequestSearchSummaryReportsViewModel();
            //model.SearchView.FormNumberList = GetFormNumList(interService.GetAll().Where(w => iridlist.Contains(w.VR_FORM_ID)));
            //model.SearchView.LastApprovedStatusList = GetLastApprovedStatus(service.GetReffValueAll().Where(w => statusidlist.Contains(w.REFF_ID) && w.REFF_VALUE != "COMPLETED" && w.REFF_VALUE != "CANCELED"));
            //model.SearchView.FormNumberList = GetFormNumList(interService.GetAll().Where(w => iridlist.Contains(w.VR_FORM_ID)));
            //model.SearchView.CompanyTypeList = GetCompTypeList(comptplist);
            //model.SearchView.KPPBCList = GetKPPBCList(nppbkcList);

            IEnumerable<Enums.BrandRegistrationAction> actionTypes = Enum.GetValues(typeof(Enums.BrandRegistrationAction)).Cast<Enums.BrandRegistrationAction>();
            model.SearchView.ListRegistrationType = from form in actionTypes
                                                    select new SelectListItem
                                                    {
                                                        Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(Enums.BrandRegistrationAction), form.ToString())),
                                                        Value = ((int)form).ToString()
                                                    };
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();


            model.DetailsList = documents;

            //model.DetailsList = new List<LicenseRequestSummaryReportsItem>(); //SearchDataSummaryReports(filter);

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterSummaryReports(BrandRegSummaryReportsViewModel model)
        {
            //var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

            var documents = brandRegistrationService.GetvwBrandRegistrationAll().Where(w => (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) : w.CREATED_BY.Equals(CurrentUser.USER_ID)));

            //if (model.SearchView.CompanyTypeSource != null)
            //{
            //    documents = documents.Where(w => w.COMPANY_TYPE.Equals(model.SearchView.CompanyTypeSource));
            //}
            //if (model.SearchView.KPPBCSource != null)
            //{
            //    documents = documents.Where(w => w.KPPBC.Equals(model.SearchView.KPPBCSource));
            //}



            var listofDoc = new List<vwBrandRegistrationModel>();

            listofDoc = documents.Select(s => new vwBrandRegistrationModel
            {
                RegistrationNo = s.REGISTRATION_NO,
                SubmissionDate = s.SUBMISSION_DATE,
                CreatedBy = s.CREATED_BY,
                CompanyName = s.BUTXT,
                NPPBKCId = s.NPPBKC_ID,
                RegistrationType = s.REGISTRATION_TYPE == 1 ? "Update HJE" : "Merek Baru",
                EffectiveDate = s.EFFECTIVE_DATE,
                LastApprovedStatusValue = s.REFF_VALUE,
            }).ToList();


            model.DetailsList = listofDoc;

            return PartialView("_LicenseRequestTableSummaryReport", model);
        }


        public void ExportXlsSummaryReports(BrandRegSummaryReportsViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryReports(model.ExportModel);

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);// "MLLicenserRequest" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }


        private string CreateXlsSummaryReports(BrandRegExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = GetExportData(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcel(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;

                if (modelExport.RegistrationNo)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationNo);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SubmissionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.NppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NPPBKCId);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.EffectiveDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.EffectiveDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Creator)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CreatorDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastModifiedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ModifyBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastModifiedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ModifyDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastApprovedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastApprovedBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastApprovedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastApprovedDate == null ? "" : data.LastApprovedDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastApprovedStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastApprovedStatusValue);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DecreeStatus != null ? (data.DecreeStatus == true ? "Approved" : "Reject") : "");
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DecreeNo);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DecreeDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeStartDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DecreeStartDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.BrandName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BrandName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.ProductType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ProductType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.CompanyTier)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyTier);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.HJE)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.HJE);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Unit)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Unit);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.BrandContent)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BrandContent);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Tarif)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Tarif);
                    iColumn = iColumn + 1;
                }


                if (modelExport.DetailExportModel.MaterialPackage)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.MaterialPackage);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.MarketDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.MarketDesc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.FrontSide)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.FrontSide);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.BackSide)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BackSide);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.LeftSide)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LeftSide);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.RightSide)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RightSide);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.TopSide)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TopSide);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.BottomSide)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BottomSide);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, BrandRegExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.RegistrationNo)
            {
                slDocument.SetCellValue(iRow, iColumn, "Registration Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.SubmissionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Submission Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Registration Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.NppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.CompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.EffectiveDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Effective Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.Creator)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created By");
                iColumn = iColumn + 1;
            }

            if (modelExport.CreatorDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created At");
                iColumn = iColumn + 1;
            }

            if (modelExport.LastModifiedBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Modified By");
                iColumn = iColumn + 1;
            }

            if (modelExport.LastModifiedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Modified Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.LastApprovedBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Approved By");
                iColumn = iColumn + 1;
            }

            if (modelExport.LastApprovedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Approved At");
                iColumn = iColumn + 1;
            }

            if (modelExport.LastApprovedStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.DecreeStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "SKEP Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.DecreeNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "SKEP No.");
                iColumn = iColumn + 1;
            }

            if (modelExport.DecreeDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "SKEP Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.DecreeStartDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "SKEP Start Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.BrandName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Brand Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.ProductType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Product Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.CompanyTier)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Tier");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.HJE)
            {
                slDocument.SetCellValue(iRow, iColumn, "HJE");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Unit)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unit");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.BrandContent)
            {
                slDocument.SetCellValue(iRow, iColumn, "Content");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Tarif)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tarif");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.MaterialPackage)
            {
                slDocument.SetCellValue(iRow, iColumn, "Material Package");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.MarketDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Market");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.FrontSide)
            {
                slDocument.SetCellValue(iRow, iColumn, "Front Side");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.BackSide)
            {
                slDocument.SetCellValue(iRow, iColumn, "Back Side");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.LeftSide)
            {
                slDocument.SetCellValue(iRow, iColumn, "Left Side");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.RightSide)
            {
                slDocument.SetCellValue(iRow, iColumn, "Right Side");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.TopSide)
            {
                slDocument.SetCellValue(iRow, iColumn, "Top Side");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.BottomSide)
            {
                slDocument.SetCellValue(iRow, iColumn, "Bottom Side");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }



        private string CreateXlsFileSummaryReports(SLDocument slDocument, int iColumn, int iRow)
        {

            //create style
            SLStyle styleBorder = slDocument.CreateStyle();
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(1, 1, iRow - 1, iColumn - 1, styleBorder);


            var fileName = "MLLicenseRequest" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.MLFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }


        public List<vwBrandRegistrationModel> GetExportData(BrandRegExportSummaryReportsViewModel model)
        {
            //var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

            var documents = brandRegistrationService.GetvwBrandRegistrationAll();

            //if (model.RegistrationTypeSource != null)
            //{
            //    documents = documents.Where(w => w.REGISTRATION_TYPE.Equals(model.RegistrationTypeSource));
            //}
            if (model.CreatorSource != null)
            {
                documents = documents.Where(w => w.CREATED_BY.Equals(model.CreatorSource));
            }


            var listofDoc = new List<vwBrandRegistrationModel>();

            if (model.DetailExportModel.BrandName ||
                model.DetailExportModel.ProductType ||
                model.DetailExportModel.CompanyTier ||
                model.DetailExportModel.HJE ||
                model.DetailExportModel.Unit ||
                model.DetailExportModel.BrandContent ||
                model.DetailExportModel.Tarif ||
                model.DetailExportModel.MaterialPackage ||
                model.DetailExportModel.MarketDesc ||
                model.DetailExportModel.FrontSide ||
                model.DetailExportModel.BackSide ||
                model.DetailExportModel.LeftSide ||
                model.DetailExportModel.RightSide ||
                model.DetailExportModel.TopSide ||
                model.DetailExportModel.BottomSide)
            {

                listofDoc = documents.Select(s => new vwBrandRegistrationModel
                {
                    RegistrationNo = s.REGISTRATION_NO,
                    SubmissionDate = s.SUBMISSION_DATE,
                    RegistrationType = s.REGISTRATION_TYPE == 1 ? "Update HJE" : "Merek Baru",
                    NPPBKCId = s.NPPBKC_ID,
                    CompanyName = s.BUTXT,
                    EffectiveDate = s.EFFECTIVE_DATE,
                    CreatedBy = s.CREATED_BY,
                    CreatedDate = s.CREATED_DATE,
                    ModifyBy = s.LASTMODIFIED_BY,
                    ModifyDate = s.LASTMODIFIED_DATE,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    LastApprovedDate = s.LASTAPPROVED_DATE,
                    LastApprovedStatus = s.LASTAPPROVED_STATUS,
                    LastApprovedStatusValue = s.REFF_VALUE,
                    DecreeNo = s.DECREE_NO == null ? "" : s.DECREE_NO,
                    DecreeDate = s.DECREE_DATE,
                    DecreeStatus = s.DECREE_STATUS,
                    DecreeStartDate = s.DECREE_STARTDATE,
                    BrandName = s.BRAND_CE,
                    ProductType = s.PRODUCT_TYPE,
                    CompanyTier = s.COMPANY_TIER.ToString(),
                    HJE = s.HJE,
                    Unit = s.UNIT,
                    BrandContent = s.BRAND_CONTENT,
                    Tarif = s.TARIFF ?? 0,
                    MaterialPackage = s.MATERIAL_PACKAGE,
                    MarketDesc = s.MARKET_DESC,
                    FrontSide = s.FRONT_SIDE,
                    BackSide = s.BACK_SIDE,
                    LeftSide = s.LEFT_SIDE,
                    RightSide = s.RIGHT_SIDE,
                    TopSide = s.TOP_SIDE,
                    BottomSide = s.BOTTOM_SIDE
                }).ToList();

            }
            else
            {
                listofDoc = documents.Select(s => new vwBrandRegistrationModel
                {
                    RegistrationNo = s.REGISTRATION_NO,
                    SubmissionDate = s.SUBMISSION_DATE,
                    RegistrationType = s.REGISTRATION_TYPE == 1 ? "Update HJE" : "Merek Baru",
                    NPPBKCId = s.NPPBKC_ID,
                    CompanyName = s.BUTXT,
                    EffectiveDate = s.EFFECTIVE_DATE,
                    CreatedBy = s.CREATED_BY,
                    CreatedDate = s.CREATED_DATE,
                    ModifyBy = s.LASTMODIFIED_BY,
                    ModifyDate = s.LASTMODIFIED_DATE,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    LastApprovedDate = s.LASTAPPROVED_DATE,
                    LastApprovedStatus = s.LASTAPPROVED_STATUS,
                    LastApprovedStatusValue = s.REFF_VALUE,
                    DecreeNo = s.DECREE_NO == null ? "" : s.DECREE_NO,
                    DecreeDate = s.DECREE_DATE,
                    DecreeStatus = s.DECREE_STATUS,
                    DecreeStartDate = s.DECREE_STARTDATE
                }).Distinct().ToList();

            }



            return listofDoc;
        }


        #endregion

        public ActionResult ChangeLog(int CRID)
        {
            var changeRequest = new BrandRegistrationReqModel();

            //var history = refService.GetChangesHistory((int)Enums.MenuList.BrandRegistrationReq, CRID.ToString()).ToList();
            var history = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.BrandRegistrationReq, CRID.ToString());
            changeRequest.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);

            return PartialView("_ChangesHistoryTable", changeRequest);
        }


        [HttpPost]
        public ActionResult ImportItems()
        {
            try
            {
                var FileImport = Request.Files[0];
                var ImportedItem = new List<BrandRegistrationReqDetailModel>();
                var err = "";
                if (FileImport != null)
                {
                    var allowedExtensions = new[] { ".xls", ".xlsx" };
                    var extension = Path.GetExtension(FileImport.FileName);
                    if (allowedExtensions.Contains(extension))
                    {
                        var data = (new ExcelReader()).ReadExcel(FileImport);
                        if (data != null)
                        {
                            string[] ArrItemNotin;
                            List<string> ItemNotinList = new List<string>();
                            var ItemNotIn = Request.Form.Get("item_notin");
                            if (ItemNotIn != "")
                            {
                                ArrItemNotin = ItemNotIn.Split(',');
                                if (ArrItemNotin.Count() > 0)
                                {
                                    ItemNotinList = ArrItemNotin.ToList();
                                }
                            }
                            var MasterProdtype = penetapanSKEPService.getMasterProductType();
                            //var ProductdetailAvailable = productDevelopmentService.GetProductDevDetail().Where(w => w.STATUS_APPROVAL == refService.GetRefByKey("COMPLETED").REFF_ID && w.PRODUCT_DEVELOPMENT.NEXT_ACTION == (int)Enums.ProductDevelopmentAction.PenetapanSKEP);
                            var RegistrationType = Convert.ToInt32(Request.Form.Get("registration_type"));
                            var nppbkc = Convert.ToString(Request.Form.Get("ViewModel.Nppbkc_ID"));
                            long RegID = 0;
                            var strRegID = Request.Form.Get("ViewModel.Registration_ID");
                            if (strRegID != "")
                            {
                                RegID = Convert.ToInt64(strRegID);
                            }

                            var ProductdetailAvailable = brandRegistrationService.GetProductDevDetail(RegistrationType, nppbkc, RegID);

                            foreach (var datarow in data.DataRows)
                            {
                                if (datarow != null)
                                {
                                    var v_requestNo = datarow[0];
                                    if (v_requestNo == "")
                                    {
                                        err += "* Request Number cannot be empty <br/>";
                                    }
                                    var v_facode_old = datarow[1];
                                    var v_facodedesc_old = datarow[2];
                                    var v_brandName = datarow[3];
                                    if (v_brandName == "")
                                    {
                                        if (RegistrationType == 1)
                                        {
                                            err += "* Brand Name cannot be empty <br/>";
                                        }
                                    }
                                    var v_companyTier = 0;
                                    if (datarow[4] != "")
                                    {
                                        var list_of_company_tier = refService.GetRefByType("COMPANY_TIER").Select(s => s.REFF_VALUE).ToList();
                                        var iscompanyTiernumeric = list_of_company_tier.Contains(datarow[4]) == true ? true : false;
                                        if (iscompanyTiernumeric)
                                        {
                                            var str_company_tier = "";
                                            foreach(var company_tier in list_of_company_tier)
                                            {
                                                str_company_tier += company_tier + ",";
                                            }

                                            if (v_companyTier < 0 || v_companyTier > 3)
                                            {
                                                err += "* Company Tier must be " + str_company_tier + " <br/>";
                                            }
                                        }
                                        else
                                        {
                                            v_companyTier = 0;
                                        }
                                    }
                                    var v_prodType = datarow[5];
                                    if (v_prodType != "")
                                    {
                                        var prodtype = MasterProdtype.Where(w => w.PRODUCT_ALIAS == v_prodType);
                                        if (!prodtype.Any())
                                        {
                                            err += "* Product Type " + v_prodType + " is not exist <br/>";
                                        }
                                        else
                                        {
                                            v_prodType = prodtype.Select(s => s.PROD_CODE).FirstOrDefault();
                                        }
                                    }
                                    Int64 v_hjePack = 0;
                                    if (datarow[6] != "")
                                    {
                                        var isHJEnumeric = Int64.TryParse(datarow[6], out v_hjePack);
                                        if (!isHJEnumeric)
                                        {
                                            err += "* HJE must be numeric <br/>";
                                        }
                                    }
                                    Int64 v_content = 0;
                                    if (datarow[7] != "")
                                    {
                                        var isContentnumeric = Int64.TryParse(datarow[7], out v_content);
                                        if (!isContentnumeric)
                                        {
                                            err += "* Content must be numeric <br/>";
                                        }
                                    }
                                    var v_unit = datarow[8];
                                    if (v_unit != "")
                                    {
                                        if (v_unit != "Batang" && v_unit != "Gram")
                                        {
                                            err += "* Unit must be Batang or Gram <br/>";
                                        }
                                    }

                                    var v_packaging_material = datarow[9];
                                    var v_front_side = "";
                                    var v_back_side = "";
                                    var v_left_side = "";
                                    var v_right_side = "";
                                    var v_top_side = "";
                                    var v_bottom_side = "";
                                    if (RegistrationType == 1)
                                    {
                                        if (datarow[10] == "" || datarow[11] == "" || datarow[12] == "" || datarow[13] == "" || datarow[14] == "" || datarow[15] == "")
                                        {
                                            err += "* Package Information cannot be empty <br/>";
                                        }
                                        else
                                        {
                                            v_front_side = datarow[10];
                                            v_back_side = datarow[11];
                                            v_left_side = datarow[12];
                                            v_right_side = datarow[13];
                                            v_top_side = datarow[14];
                                            v_bottom_side = datarow[15];
                                        }
                                    }


                                    var productdev = ProductdetailAvailable.Where(w => w.REQUEST_NO == v_requestNo && w.FA_CODE_OLD == v_facode_old).ToList();
                                    if (productdev.Count() > 0)
                                    {
                                        var item = MapProductDetailModelList(productdev , RegistrationType).FirstOrDefault();
                                        if (ItemNotinList.Contains(item.PD_DETAIL_ID.ToString()))
                                        {
                                            err += "* Product with request number " + item.Request_No + " already added before <br/>";
                                        }
                                        var str_startDate = Request.Form.Get("submission_date");
                                        var startDate = Convert.ToDateTime(str_startDate);
                                        decimal v_tariff = 0;
                                        long v_hjeBatang = 0;
                                        if (str_startDate != "" && v_hjePack != 0 && v_prodType != "")
                                        {
                                            if (v_content > 0)
                                            {
                                                v_hjeBatang = v_hjePack / v_content;
                                            }
                                            v_tariff = penetapanSKEPService.getTariffByCombine(v_prodType, v_hjePack, startDate).TARIFF_VALUE;
                                        }
                                        var itemDet = new BrandRegistrationReqDetailModel
                                        {
                                            Item = item,
                                            Brand_Ce = v_brandName,
                                            Company_Tier = v_companyTier,
                                            Prod_Code = v_prodType,
                                            HJEperPack = v_hjePack,
                                            Brand_Content = v_content.ToString(),
                                            HJEperBatang = v_hjeBatang,
                                            Unit = v_unit,
                                            Tariff = v_tariff,
                                            Packaging_Material = v_packaging_material,
                                            Front_Side = v_front_side,
                                            Back_Side = v_back_side,
                                            Left_Side = v_left_side,
                                            Right_Side = v_right_side,
                                            Top_Side = v_top_side,
                                            Bottom_Side = v_bottom_side
                                        };
                                        ImportedItem.Add(itemDet);
                                    }
                                    else
                                    {
                                        err += "* Product with request number " + datarow[0] + " could not be found <br/>";
                                    }
                                }
                            }
                        }
                        else
                        {
                            err = "* Data is empty";
                        }
                    }
                    else
                    {
                        err = "* File extension is not allowed.";
                    }
                }

                var dataAttr = new { data = ImportedItem, attribute = new { ErrorMessage = err } };
                return Json(dataAttr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion


        #region Helper Get Data

        private BrandRegistrationReqViewModel GetBrandRegistrationData(long ID = 0, string Mode = "")
        {
            var _BRViewModel = new BrandRegistrationReqViewModel();
            _BRViewModel.ViewModel = GetBrandRegistarationMaster(ID);

            var detail = brandRegistrationService.GetBrandDetailByRegistrationID(ID);
            _BRViewModel.Item = detail.Select(s => new BrandRegistrationReqDetailModel
            {
                Registration_Detail_ID = s.REGISTRATION_DETAIL_ID,
                Registration_ID = s.REGISTRATION_ID,
                Brand_Ce = s.BRAND_CE,
                Prod_Code = s.PROD_CODE,
                Company_Tier = s.COMPANY_TIER,
                Unit = s.UNIT,
                Brand_Content = s.BRAND_CONTENT,
                Tariff = s.TARIFF ?? 0,
                PD_Detail_ID = s.PD_DETAIL_ID,
                Hje = s.HJE,
                HJEperPack = s.HJE,
                HJEperBatang = s.HJE,
                Packaging_Material = s.MATERIAL_PACKAGE,
                Front_Side = s.FRONT_SIDE,
                Back_Side = s.BACK_SIDE,
                Left_Side = s.LEFT_SIDE,
                Right_Side = s.RIGHT_SIDE,
                Top_Side = s.TOP_SIDE,
                Bottom_Side = s.BOTTOM_SIDE,

                Request_No = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                Fa_Code_Old = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                Fa_Code_New_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                CompanyName = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                HlCode = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                MarketDesc = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                Market_ID = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_ID,
                ProductType = s.ZAIDM_EX_PRODTYP.PRODUCT_TYPE,
                Created_By_Name = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.CREATOR.FIRST_NAME + " " + s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.CREATOR.LAST_NAME,
                ProdDevNextAction = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.NEXT_ACTION,
                Is_Import = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT,
                ProductionCenter = s.PRODUCT_DEVELOPMENT_DETAIL.T001W.NAME1

            }).ToList();

            List<string> nppbkclist = new List<string>();

            foreach (var item in _BRViewModel.Item)
            {
                if (item.Market_ID == "02")
                {
                    _BRViewModel.ViewModel.DocExport = true;
                }
            }

            //data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            //data.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
            _BRViewModel.MainMenu = mainMenu;
            _BRViewModel.CurrentMenu = PageInfo;
            _BRViewModel.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            _BRViewModel.POA = MapPoaModel(refService.GetPOA(_BRViewModel.ViewModel.Created_By));
            _BRViewModel.ShowActionOptions = _BRViewModel.IsNotViewer;
            _BRViewModel.EditMode = false;
            _BRViewModel.EnableFormInput = true;
            _BRViewModel.ViewModel.IsCreator = false;
            _BRViewModel.BrandList = GetBrandist(penetapanSKEPService.getMasterBrand());
            _BRViewModel.ProductTypeList = GetProdTypeList(penetapanSKEPService.getMasterProductType());
            _BRViewModel.CompanyTierList = GetCompanyTierList(refService.GetRefByType("COMPANY_TIER"));
            _BRViewModel.File_Size = GetMaxFileSize();
            _BRViewModel.CurrentRole = CurrentUser.UserRole;
            _BRViewModel.UserAccess = GetUserAccess(_BRViewModel.ViewModel.Registration_ID, _BRViewModel.ViewModel.LastApproved_Status, _BRViewModel.ViewModel.Created_By, _BRViewModel.ViewModel.LastApproved_By, Mode);

            //IEnumerable<DocumentStatusGovType2> statusTypes = Enum.GetValues(typeof(DocumentStatusGovType2)).Cast<DocumentStatusGovType2>();
            //_BRViewModel.ListGovStatus = from form in statusTypes
            //                     select new SelectListItem
            //                     {
            //                         Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(DocumentStatusGovType2), form.ToString())),
            //                         Value = ((int)form).ToString()
            //                     };

            _BRViewModel.ListGovStatus = GetGovStatusList(brandRegistrationService.GetGovStatusList());


            var filesupload = brandRegistrationService.GetFileUploadByCRId(ID);
            if (filesupload != null)
            {
                var Othersfileupload = filesupload.Where(w => w.DOCUMENT_ID == null && w.IS_GOVERNMENT_DOC == false);
                _BRViewModel.ViewModel.BrandRegFileOtherList = Othersfileupload.Select(s => new BrandRegFileOtherModel
                {              
                    FileId = s.FILE_ID,
                    Path = s.PATH_URL,
                    FileName = s.FILE_NAME
                }).ToList();
                foreach (var file in _BRViewModel.ViewModel.BrandRegFileOtherList)
                {
                    file.Name = GenerateFileName(file.Path);
                    file.Path = GenerateURL(file.Path);
                }

                var SKEPfileupload = filesupload.Where(w => w.IS_GOVERNMENT_DOC == true);
                _BRViewModel.ViewModel.SKEPFileList = SKEPfileupload.Select(s => new BrandRegFileOtherModel
                {
                    FileId = s.FILE_ID,
                    Path = s.PATH_URL,
                    FileName = s.FILE_NAME
                }).ToList();
                foreach (var file in _BRViewModel.ViewModel.SKEPFileList)
                {
                    file.Name = GenerateFileName(file.Path);
                    file.Path = GenerateURL(file.Path);
                }
                //var BAsfileupload = filesupload.Where(w => w.IS_GOVERNMENT_DOC == true);
                //_CRModel.File_BA_Path_Plus = BAsfileupload.Select(s => new ChangeRequestFileOtherModel
                //{
                //    FileId = s.FILE_ID,
                //    Path = s.PATH_URL,
                //    FileName = s.FILE_NAME
                //}).ToList();

            }

            //_CRModel.Count_Lamp = filesupload.Count();


            if (Mode == "Edit")
            {
                nppbkclist = brandRegistrationService.GetNPPBKCByUser(CurrentUser.USER_ID);

                _BRViewModel.IsFormReadOnly = false;
                _BRViewModel.IsDetail = false;
                _BRViewModel.ViewModel.NextStatus = refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID;
            }
            else if (Mode == "Detail" || Mode == "EditSKEP")
            {
                nppbkclist.Add(_BRViewModel.ViewModel.Nppbkc_ID);
                nppbkclist = brandRegistrationService.GetNPPBKCByUser(CurrentUser.USER_ID);

                _BRViewModel.IsFormReadOnly = true;
                _BRViewModel.IsDetail = true;
            }
            else if (Mode == "Approval")
            {
                nppbkclist.Add(_BRViewModel.ViewModel.Nppbkc_ID);
                _BRViewModel.IsFormReadOnly = true;
                _BRViewModel.IsDetail = false;
                _BRViewModel.ViewModel.NextStatus = refService.GetRefByKey("WAITING_GOVERNMENT_APPROVAL").REFF_ID;
            }

            _BRViewModel.NppbkcList = GetNppbkcListByUser(nppbkclist);

            //var history = refService.GetChangesHistory((int)Enums.MenuList.ChangeRequest, ID.ToString()).ToList();
            //_CRModel.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);

            //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.BrandRegistrationReq, ID).ToList();
            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = ID;
            workflowInput.FormType = Enums.FormType.BrandRegistrationReq;
            var workflow = this.whBLL.GetByFormTypeAndFormId(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID).ToList();

            _BRViewModel.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

            string account_name = "";
            string role = "";
            string action = "";


            //WORKFLOW_HISTORY additional = new WORKFLOW_HISTORY();
            WorkflowHistoryViewModel additional = new WorkflowHistoryViewModel();

            if (_BRViewModel.ViewModel.LastApproved_Status.Equals(refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID) || _BRViewModel.ViewModel.LastApproved_Status.Equals(refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID))
            {
                var poa_approvers = brandRegistrationService.GetPOAApproverList(ID);
                foreach (var poa_approver in poa_approvers)
                {
                    if (_BRViewModel.ViewModel.LastApproved_Status.Equals(refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID))
                    {
                        if(_BRViewModel.ViewModel.LastApproved_By == poa_approver.POA_ID)
                        {
                            account_name = poa_approver.POA_ID;
                        }

                    }
                    else
                    {
                        if (account_name == "")
                        {
                            account_name += poa_approver.POA_ID;
                        }
                        else
                        {
                            account_name += ", " + poa_approver.POA_ID;
                        }
                    }

                }

                if (_BRViewModel.ViewModel.LastApproved_Status.Equals(refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID))
                {
                    additional.ACTION = "Waiting For POA Approval";
                }
                else
                {
                    additional.ACTION = "Waiting For POA SKEP Approval";
                }

                additional.USERNAME = account_name;
                //additional.ACTION = 11;
                additional.Role = "POA";
                //additional.ACTION_DATE = _CRModel.LastModifiedDate;
                _BRViewModel.WorkflowHistory.Add(additional);
            }



            //_BRViewModel.ButtonCombination = GetButtonCombination(_BRViewModel.ViewModel.LastApproved_Status);

            if ((_BRViewModel.ButtonCombination != "Create") && (_BRViewModel.ButtonCombination != "Edit"))
            {
                _BRViewModel.IsFormReadOnly = true;
            }

            _BRViewModel.BaseUrl = GenerateURL("/");

            return _BRViewModel;
        }

        private SelectList GetGovStatusList(Dictionary<int, string> status)
        {
            var query = from x in status
                        select new SelectItemModel()
                        {
                            ValueField = x.Key == 1 ? "True" : "False",
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        [HttpPost]
        public JsonResult ajaxGetBrandList()
        {
            var BrandList = GetBrandist(penetapanSKEPService.getMasterBrand());
            return Json(BrandList);
        }

        //[HttpPost]
        //public JsonResult GetBrandMaster()
        //{
        //    var data = brandRegistrationService.FindPlantNonImport(bukrs);
        //    return Json(new SelectList(data, "NPPBKC_ID", "NAME1"));

        //}


        //private string GetButtonCombination(long LastApprovedStatus)
        //{
        //    string result = "Create";

        //    switch (LastApprovedStatus)
        //    {
        //        case refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID:
        //            result = "Create";
        //            break;

        //        case refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID:
        //            result = "Edit";
        //            break;

        //        case refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID:
        //            result = "ApproveReject";
        //            break;

        //        case refService.GetRefByKey("WAITING_GOVERNMENT_APPROVAL").REFF_ID:
        //            result = "SubmitSKEP";
        //            break;

        //        case refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID:
        //            result = "ApproveRejectFinal";
        //            break;

        //        case refService.GetRefByKey("COMPLETED").REFF_ID:
        //            result = "Withdraw";
        //            break;
        //    }

        //    return result;
        //}


        private BrandRegistrationReqModel GetBrandRegistarationMaster(long ID = 0, string Mode = "")
        {
            try
            {
                var _CRModel = brandRegistrationService.GetBrandRegistrationById(ID).Select(s => new BrandRegistrationReqModel
                {
                    Registration_ID = s.REGISTRATION_ID,
                    Registration_No = s.REGISTRATION_NO,
                    Submission_Date = s.SUBMISSION_DATE,
                    Registration_Type = s.REGISTRATION_TYPE,
                    Nppbkc_ID = s.NPPBKC_ID,
                    Effective_Date = s.EFFECTIVE_DATE,
                    Created_By = s.CREATED_BY,
                    Created_Date = s.CREATED_DATE,
                    LastApproved_Status = s.LASTAPPROVED_STATUS,
                    LastApproved_By = s.LASTAPPROVED_BY == null ? "-" : s.LASTAPPROVED_BY,
                    Decree_Status = s.DECREE_STATUS ?? false,
                    Decree_No = s.DECREE_NO == null ? "N/A" : s.DECREE_NO,
                    Decree_Date = s.DECREE_DATE,
                    Decree_StartDate = s.DECREE_STARTDATE,
                    CurrentRole = CurrentUser.UserRole,
                    Text_To = s.ZAIDM_EX_NPPBKC.TEXT_TO,
                    Created_By_Name = s.CREATOR.FIRST_NAME + " " + s.CREATOR.LAST_NAME,
                    Created_By_Email = s.CREATOR.EMAIL
                    
                }).FirstOrDefault();

                _CRModel.Company = MapNppbkcModel(refService.GetNppbkc(_CRModel.Nppbkc_ID)).Company;
                _CRModel.Status = refService.GetReferenceById(_CRModel.LastApproved_Status).REFF_KEYS;
                _CRModel.strLastApproved_Status = refService.GetReferenceById(_CRModel.LastApproved_Status).REFF_VALUE;
                _CRModel.CompanyAddress = brandRegistrationService.FindMainPlantByNppbkcID(_CRModel.Nppbkc_ID).ADDRESS;

                return _CRModel;
            }
            catch (Exception e)
            {
                return new BrandRegistrationReqModel();
            }
        }


        [HttpPost]
        public ActionResult GetProductDevelopmentItemList(long[] ItemNotIn, int RegistrationType, string nppbkc, int RegId)
        {
            try
            {
                //var theapp = productDevelopmentService.GetProductDevDetail().Where(w => w.STATUS_APPROVAL == refService.GetRefByKey("COMPLETED").REFF_ID && w.PRODUCT_DEVELOPMENT.NEXT_ACTION == RegistrationType);
                var theapp = brandRegistrationService.GetProductDevDetail(RegistrationType, nppbkc, RegId);
                var list = MapProductDetailModelList(theapp, RegistrationType);
                return Json(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetTariff(long HJE, DateTime StartDate, string GoodType)
        {
            try
            {
                var data = penetapanSKEPService.getTariffByCombine(GoodType, HJE, StartDate);
                if (data != null)
                {
                    var dataAttr = new { attribute = new { TariffId = data.TARIFF_ID, Tariff = data.TARIFF_VALUE } };
                    return Json(dataAttr, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var dataAttr = new { attribute = new { TariffId = 0, Tariff = 0 } };
                    return Json(dataAttr, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
        private SelectList GetBrandist(IQueryable<CustomService.Data.ZAIDM_EX_BRAND> brandList)
        {
            var query = from x in brandList
                        select new SelectItemModel()
                        {
                            ValueField = x.BRAND_CE,
                            TextField = x.BRAND_CE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }



        private SelectList GetCompanyTierList(IQueryable<SYS_REFFERENCES> companyTierList)
        {
            var query = from x in companyTierList
                        select new SelectItemModel()
                        {
                            ValueField = x.REFF_ID,
                            TextField = x.REFF_VALUE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }



        private SelectList GetProdTypeList(IQueryable<CustomService.Data.MASTER_PRODUCT_TYPE> prodtypeList)
        {
            var query = from x in prodtypeList
                        select new SelectItemModel()
                        {
                            ValueField = x.PROD_CODE,
                            TextField = x.PRODUCT_TYPE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }


        public BrandRegSupportingDocumentModel MapSupportingDocumentModel(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new BrandRegSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    IsBrowseFileEnable = true,
                    Path = "",
                    FileUploadId = 0,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //----//

        List<WorkflowHistoryViewModel> GetWorkflowHistory(long id)
        {

            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.ProductDevelopment;
            var workflow = this._workflowHistoryBLL.GetByFormTypeAndFormId(workflowInput).OrderByDescending(item => item.ACTION_DATE);

            return Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

        }
        public List<ProductDevDetailModel> GetProductDevelopmentDetail(long PD_ID)
        {
            try
            {
                var PDDetModel = new List<ProductDevDetailModel>();
                var data = productDevelopmentService.GetProductDetailByProductID(PD_ID);
                if (data.Any())
                {
                    PDDetModel = data.Select(s => new ProductDevDetailModel
                    {
                        Fa_Code_Old = s.FA_CODE_OLD,
                        Fa_Code_New = s.FA_CODE_NEW,
                        Hl_Code = s.HL_CODE,
                        Market_Id = s.MARKET_ID,
                        Fa_Code_Old_Desc = s.FA_CODE_OLD_DESCR,
                        Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                        Werks = s.WERKS,
                        Is_Import = s.IS_IMPORT,
                        Request_No = s.REQUEST_NO,
                        Bukrs = s.BUKRS


                    }).ToList();
                }
                return PDDetModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<ProductDevDetailModel>();
            }
        }

        private SelectList GetPoaList(IEnumerable<CustomService.Data.POA> poaList)
        {
            var query = from x in poaList
                        select new SelectItemModel()
                        {
                            ValueField = x.POA_ID,
                            TextField = String.Format("{0} {1}", x.USER_LOGIN.FIRST_NAME, x.USER_LOGIN.LAST_NAME)
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        [HttpPost]
        public JsonResult GetNppbkc(string id)
        {
            try
            {
                var nppbkc = refService.GetNppbkc(id);
                var mapped = MapNppbkcModel(nppbkc);
                var serialized = JsonConvert.SerializeObject(mapped);
                var obj = new JObject
                {
                    new JProperty("Success", true),
                    new JProperty("Data", JObject.Parse(serialized))
                };
                var objStr = obj.ToString();
                return Json(objStr);

            }
            catch (Exception ex)
            {
                return Json(new JObject()
                {
                    new JProperty("Success", false),
                    new JProperty("Message", ex.Message)
                });
            }

        }

        [HttpPost]
        public JsonResult GetPlant(string nppbkcId)
        {
            try
            {
                var plant = brandRegistrationService.FindMainPlantByNppbkcID(nppbkcId);
                var mapped = MapPlantModel(plant);
                var serialized = JsonConvert.SerializeObject(mapped);
                var obj = new JObject
                {
                    new JProperty("Success", true),
                    new JProperty("Data", JObject.Parse(serialized))
                };
                var objStr = obj.ToString();
                return Json(objStr);

            }
            catch (Exception ex)
            {
                return Json(new JObject()
                {
                    new JProperty("Success", false),
                    new JProperty("Message", ex.Message)
                });
            }

        }

        private SelectList GetNppbkcList(IEnumerable<CustomService.Data.MASTER_NPPBKC> nppbkcList)
        {
            var query = from x in nppbkcList
                        select new SelectItemModel()
                        {
                            ValueField = x.NPPBKC_ID,
                            TextField = x.NPPBKC_ID
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetNppbkcListByUser(List<string> nppbkcList)
        {
            var selectListItems = nppbkcList.Select(x => new SelectItemModel() { ValueField = x, TextField = x }).ToList();
            return new SelectList(selectListItems.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }


        [HttpPost]
        public JsonResult GetLastRecordReceivedDecree()
        {
            var temp = penetapanSKEPService.GetLastRecordPenetapanSKEP();

            Int64 result;
            if (temp != null)
            {
                if (temp.RECEIVED_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.RECEIVED_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetLastRecordBR()
        {
            var temp = brandRegistrationService.GetLastRecordBrandReq();

            Int64 result;
            if (temp != null)
            {
                if (temp.REGISTRATION_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.REGISTRATION_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetLastRecordPD()
        {
            var temp = productDevelopmentService.GetLastRecordPD();

            Int64 result;
            if (temp != null)
            {
                if (temp.PD_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.PD_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetLastRecordItem()
        {
            var temp = productDevelopmentService.GetLastRecordPDDetail();

            Int64 result;
            if (temp != null)
            {
                if (temp.PD_DETAIL_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.PD_DETAIL_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetSupportDoc(long formId, string bukrs)
        {
            var tempList = productDevelopmentService.FindSupportDetail(formId, bukrs);
            var selectList = from s in tempList
                             select new SelectListItem
                             {
                                 Value = s.DOCUMENT_ID.ToString(),
                                 Text = s.SUPPORTING_DOCUMENT_NAME
                             };
            var nameList = new SelectList(selectList.GroupBy(p => p.Value).Select(g => g.First()), "Value", "Text");

            return Json(nameList);
        }

        [HttpPost]
        public JsonResult GetPlantByCompanyNonImport(string bukrs)
        {
            var data = productDevelopmentService.FindPlantNonImport(bukrs);
            return Json(new SelectList(data, "NPPBKC_ID", "NAME1"));
        }

        [HttpPost]
        public JsonResult GetPlantByCompanyImport(string bukrs)
        {
            var data = productDevelopmentService.FindPlantImport(bukrs);
            return Json(new SelectList(data, "NPPBKC_IMPORT_ID", "NAME1"));
        }

        [HttpPost]
        public JsonResult GetCodeDescription(string code)
        {
            var tempValue = productDevelopmentService.FindItemDescription(code);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.MATERIAL_DESC == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.MATERIAL_DESC.ToString();
                }
            }
            else
            {
                result = "";
            }
            return Json(result);
        }



        #endregion

        #region Upload Part Brand Registration

        //CSI
        public BrandValidationResult FileValidation(BrandRegistrationReqViewModel model)
        {
            var result = new BrandValidationResult();

            var maxFileSize = GetMaxFileSize();
            var isOkFileExt = true;
            var isOkFileSize = true;
            var supportingDocFile = new List<HttpPostedFileBase>();
            if (model.BrandRegSupportingDocument != null)
            {
                supportingDocFile = model.BrandRegSupportingDocument.Select(s => s.File).ToList();
            }

            isOkFileExt = CheckFileExtension(supportingDocFile);
            if (isOkFileExt)
            {
                isOkFileExt = CheckFileExtension(model.File_Other);
                if (isOkFileExt)
                {
                    //isOkFileExt = CheckFileExtension(model.File_BA);
                }
            }
            else
            {
                result.IsValid = false;
                result.ResultMessage = "The file extension is not permitted";
            }

            return result;
        }

        public string UploadFile(HttpPostedFileBase File)
        {
            try
            {
                var filePath = "";

                if (File != null && File.ContentLength > 0)
                {
                    var baseFolder = "/files_upload/";
                    var uploadToFolder = Server.MapPath("~" + baseFolder);
                    var date_now = DateTime.Now;
                    var date = String.Format("{0:ddMMyyyyHHmmss}", date_now);
                    var extension = Path.GetExtension(File.FileName);
                    var file_name = Path.GetFileNameWithoutExtension(File.FileName) + "=BrandReg=" + CurrentUser.USER_ID + "-" + date + extension;
                    var filePathServer = Path.Combine(uploadToFolder, file_name);
                    filePath = Path.Combine(baseFolder, file_name);
                    Directory.CreateDirectory(uploadToFolder);
                    File.SaveAs(filePathServer);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return "Error";
            }
        }


        public long GetMaxFileSize()
        {
            try
            {
                var size = refService.GetRefByType("UPLOAD_FILE_LIMIT").Select(s => s.REFF_VALUE == null ? 0 : Convert.ToInt64(s.REFF_VALUE)).FirstOrDefault();
                return size;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool CheckFileSize(List<HttpPostedFileBase> FileList, long MaxSize)
        {
            try
            {
                var isOk = true;
                if (FileList != null)
                {
                    foreach (var File in FileList)
                    {
                        if (File != null)
                        {
                            long b = File.ContentLength;
                            long kb = b / 1024;
                            long mb = kb / 1024;
                            if (mb > MaxSize)
                            {
                                isOk = false;
                                break;
                            }
                        }
                    }
                }
                return isOk;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return false;
            }
        }

        public bool CheckFileExtension(List<HttpPostedFileBase> FileList)
        {
            try
            {
                var isOk = true;
                var extList = brandRegistrationService.GetFileExtList();
                if (FileList != null)
                {
                    foreach (var File in FileList)
                    {
                        if (File != null)
                        {
                            var ext = Path.GetExtension(File.FileName);
                            if (!extList.Contains(ext.ToLower()))
                            {
                                isOk = false;
                                break;
                            }
                        }
                    }
                }
                return isOk;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return false;
            }
        }



        public void InsertUploadSuppDocFile(IEnumerable<BrandRegSupportingDocumentModel> SuppDocList, long BRId)
        {
            try
            {
                if (SuppDocList != null)
                {
                    foreach (var Doc in SuppDocList)
                    {
                        if (Doc.Path != "" && Doc.Path != null)
                        {
                            var filename = brandRegistrationService.GetSupportingDocName(Doc.Id);
                            brandRegistrationService.InsertFileUpload(BRId, Doc.Path, CurrentUser.USER_ID, Doc.Id, false, filename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void InsertUploadCommonFile(List<string> FilePath, long CRId, bool IsGov, List<string> FileName)
        {
            try
            {
                if (FilePath != null)
                {
                    var filenamelist = new List<SelectListItem>();
                    if (FileName != null)
                    {
                        foreach (var name in FileName)
                        {
                            var file = name.Split('^');
                            if (file.Count() > 1)
                            {
                                filenamelist.Add(new SelectListItem
                                {
                                    Text = file[0],
                                    Value = file[1]
                                });
                            }
                        }
                    }
                    foreach (var Doc in FilePath)
                    {
                        if (Doc != null && Doc != "")
                        {
                            var DocName = Doc.Replace("/files_upload/", "");
                            var arrfileext = DocName.Split('.');
                            var countext = arrfileext.Count();
                            var fileext = "";
                            if (countext > 0)
                            {
                                fileext = arrfileext[countext - 1];
                            }
                            DocName = DocName.Replace("=BrandReg=", "/");
                            var arrfilename = DocName.Split('/');
                            if (arrfilename.Count() > 0)
                            {
                                DocName = arrfilename[0] + "." + fileext;
                            }

                            var thefilename = filenamelist.Where(w => DocName.Contains(w.Text)).Select(s => s.Value).FirstOrDefault();
                            if (thefilename == null)
                            {
                                thefilename = "";
                            }
                            brandRegistrationService.InsertFileUpload(CRId, Doc, CurrentUser.USER_ID, 0, IsGov, thefilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }



        //ITPI

        [HttpPost]
        public JsonResult UploadFilesBrand()
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var fileName = fileContent.FileName;
                        var type = System.IO.Path.GetFileName(file);
                        var docNumber = Request.Form.Get("doc_number").Replace("/", "_");
                        var path = Server.MapPath("~/files_upload/brand-registration/" + docNumber);
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        path = System.IO.Path.Combine(path, fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload Failed.");
            }

            return Json("File Uploaded Successfully");

        }

        private List<FILE_UPLOAD> UploadedFilesBrand;
        private void AddUploadedDocBrand(string id, string url, long? docId = null, bool isGovDoc = false)
        {
            try
            {
                if (UploadedFilesBrand == null)
                {
                    UploadedFilesBrand = new List<FILE_UPLOAD>();
                }
                var now = DateTime.Now;
                var doc = new FILE_UPLOAD()
                {
                    FORM_TYPE_ID = (int)Enums.FormList.BrandReq,
                    FORM_ID = id,
                    PATH_URL = url,
                    UPLOAD_DATE = now,
                    CREATED_BY = CurrentUser.USER_ID,
                    CREATED_DATE = now,
                    LASTMODIFIED_BY = CurrentUser.USER_ID,
                    LASTMODIFIED_DATE = now,
                    IS_ACTIVE = true,
                    IS_GOVERNMENT_DOC = isGovDoc,
                    DOCUMENT_ID = docId
                };
                UploadedFilesBrand.Add(doc);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public JsonResult GetOtherDocumentsBrand(int type, string id)
        {
            try
            {
                var data = refService.GetUploadedFiles(type, id).Where(x => x.DOCUMENT_ID == null).ToList();
                var result = data.Select(x => new FileUploadModel()
                {
                    FileID = x.FILE_ID,
                    FormID = x.FORM_ID,
                    FormTypeID = x.FORM_TYPE_ID,
                    IsGovernmentDoc = x.IS_GOVERNMENT_DOC,
                    IsActive = x.IS_ACTIVE,
                    PathURL = x.PATH_URL,
                    UploadDate = x.UPLOAD_DATE,
                    FileName = x.FILE_NAME
                });
                return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(ex);
            }
        }

        [HttpPost]
        public PartialViewResult UploadFileExcelBrand(HttpPostedFileBase brandExcelfile)
        {
            var data = (new ExcelReader()).ReadExcel(brandExcelfile);
            var model = new BrandRegistrationReqDetailViewModel() { ViewModel = new BrandRegistrationReqDetailModel() };
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var uploadItem = new BrandRegistrationReqViewModel();

                    try
                    {
                        //uploadItem.ViewDetailModel.Fa_Code_Old = datarow[0];
                        //uploadItem.ViewDetailModel.Fa_Code_New = datarow[0];
                        //uploadItem.ViewDetailModel.Hl_Code = datarow[0];
                        //uploadItem.ViewDetailModel.Market_Id = datarow[0];
                        //uploadItem.ViewDetailModel.Werks = datarow[0];
                        //uploadItem.ViewDetailModel.Is_Import = datarow[0];
                        //uploadItem.ViewModel.Company.Name = datarow[0];

                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            //var input = Mapper.Map<List<Pbck1ProdConverterInput>>(model.Detail.Pbck1ProdConverter);
            //var outputResult = _pbck1Bll.ValidatePbck1ProdConverterUpload(input, nppbkc, Boolean.Parse(isNppbckImportChecked));

            //model.Detail.Pbck1ProdConverter = Mapper.Map<List<Pbck1ProdConvModel>>(outputResult);

            return PartialView("_BrandListItem", model);
        }

        #endregion

        private string GenerateURL(string path)
        {
            var url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + path;
            return url;
        }

        private string GenerateFileName(string path)
        {
            var filename = path.Replace("/files_upload/", "");
            var arrfileext = filename.Split('.');
            var countext = arrfileext.Count();
            var fileext = "";
            if (countext > 0)
            {
                fileext = arrfileext[countext - 1];
            }
            filename = filename.Replace("=BrandReg=", "/");
            var arrfilename = filename.Split('/');
            if (arrfilename.Count() > 0)
            {
                filename = arrfilename[0] + "." + fileext;
            }
            return filename;
        }

        #region Helper Model
        private string ChangeFormnumberCompanyCity(string formnumber, string companyalias, string cityalias)
        {
            try
            {
                var arr = formnumber.Split('/');
                if (arr.Count() == 5)
                {
                    formnumber = arr[0] + "/" + companyalias + "/" + cityalias + "/" + arr[3] + "/" + arr[4];
                }
                return formnumber;
            }
            catch (Exception)
            {
                return formnumber;
            }
        }


        private BRAND_REGISTRATION_REQ MapToTable(BrandRegistrationReqModel model)
        {
            var now = DateTime.Now;

            var docnumberinput = new GenerateDocNumberInput();
            docnumberinput.FormType = Enums.FormType.BrandRegistrationTrans;
            docnumberinput.Month = Convert.ToDateTime(model.Submission_Date).Month;
            docnumberinput.Year = Convert.ToDateTime(model.Submission_Date).Year;
            var formnumber = _docbll.GenerateNumber(docnumberinput);
            formnumber = ChangeFormnumberCompanyCity(formnumber, model.Company.Alias, model.Company.City);

            var data = new BRAND_REGISTRATION_REQ();
            if (model.Registration_ID != 0)
            {
                data.REGISTRATION_ID = model.Registration_ID;
                data.LASTAPPROVED_STATUS = model.LastApproved_Status;
                data.LASTMODIFIED_BY = CurrentUser.USER_ID;
                data.LASTMODIFIED_DATE = now;
            }
            else
            {
                //data.REGISTRATION_NO = GenerateFormNumber(model.Company.Alias, model.Company.City, now);
                data.REGISTRATION_NO = formnumber;
                data.CREATED_BY = CurrentUser.USER_ID;
                data.CREATED_DATE = now;
                data.LASTMODIFIED_BY = CurrentUser.USER_ID;
                data.LASTMODIFIED_DATE = now;
                //data.LASTAPPROVED_STATUS = refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID; //DRAFT NEW
            }
            data.SUBMISSION_DATE = model.Submission_Date;
            data.REGISTRATION_TYPE = model.Registration_Type;
            data.NPPBKC_ID = model.Nppbkc_ID;
            data.EFFECTIVE_DATE = model.Effective_Date;
            data.DECREE_STATUS = true;
            data.LASTAPPROVED_STATUS = model.NextStatus;
            data.REGISTRATION_TYPE = model.Registration_Type;

            return data;
        }

        private BRAND_REGISTRATION_REQ MapSKEPToTable(BrandRegistrationReqModel model)
        {
            var now = DateTime.Now;

            var data = new BRAND_REGISTRATION_REQ();
            data.REGISTRATION_ID = model.Registration_ID;
            data.DECREE_DATE = model.Decree_Date;
            data.DECREE_NO = model.Decree_No;
            data.DECREE_STARTDATE = model.Decree_StartDate;
            data.DECREE_STATUS = model.Decree_Status;
            data.LASTMODIFIED_BY = CurrentUser.USER_ID;
            data.LASTMODIFIED_DATE = now;

            return data;
        }

        //private List<BRProductDevDetailModel> MapProductDetailModelList(IQueryable<vwProductDevDetail> product, int RegistrationType)
        private List<BRProductDevDetailModel> MapProductDetailModelList(List<vwProductDevDetail> product, int RegistrationType)
        {
            var list = product.Select(s => new BRProductDevDetailModel
            {
                PD_DETAIL_ID = s.PD_DETAIL_ID,
                Request_No = s.REQUEST_NO,
                Fa_Code_Old = s.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.FA_CODE_NEW,
                Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                Company = new CompanyModel { Id = s.BUKRS, Name = s.COMPANY_NAME },
                Hl_Code = s.HL_CODE,
                Market = new MarketModel { Market_Id = s.MARKET_ID, Market_Desc = s.MARKET_DESC },
                ProdDevNextAction = s.NEXT_ACTION,
                ProductionCenter = s.PRODUCTION_CENTER
            }).ToList();

            var newList = new List<BRProductDevDetailModel>();
            foreach(var item in list)
            {
                //var brandMaster = brandRegistrationService.FindBrandMaster(item.Fa_Code_Old, item.Fa_Code_New, RegistrationType);
                var brandMaster = brandRegistrationService.FindBrandTransaction(item.Fa_Code_Old, item.Fa_Code_New, RegistrationType);
                if (brandMaster != null)
                {
                    item.HJE = brandMaster.HJE;
                    item.BrandContent = Convert.ToInt32(brandMaster.BRAND_CONTENT);
                    if (item.HJE > 0)
                    {
                        item.HJEperBatang = item.HJE / item.BrandContent;
                    }
                    else
                    {
                        item.HJEperBatang = 0;
                    }
                    item.Tariff = brandMaster.TARIFF ?? 0;
                    item.Unit = brandMaster.UNIT;
                    item.CompanyTier = brandMaster.COMPANY_TIER;
                    item.PackagingMaterial = brandMaster.MATERIAL_PACKAGE;
                    item.Market_Id = brandMaster.ZAIDM_EX_MARKET.MARKET_DESC;
                    item.BrandName = brandMaster.BRAND_CE;
                    item.ExciseGoodType = brandMaster.PROD_CODE;
                    item.FrontSide = brandMaster.FRONT_SIDE;
                    item.BackSide = brandMaster.BACK_SIDE;
                    item.LeftSide = brandMaster.LEFT_SIDE;
                    item.RightSide = brandMaster.RIGHT_SIDE;
                    item.TopSide = brandMaster.TOP_SIDE;
                    item.BottomSide = brandMaster.BOTTOM_SIDE;
                }
                else
                {
                    item.HJE = 0;
                    item.HJEperBatang = 0;
                    item.Unit = "";
                    item.BrandContent = 0;
                    item.Tariff = 0;
                    item.PackagingMaterial = "";
                    item.BrandName = "";
                    item.ExciseGoodType = "";
                    item.FrontSide = "";
                    item.BackSide = "";
                    item.LeftSide = "";
                    item.RightSide = "";
                    item.TopSide = "";
                    item.BottomSide = "";
                }
                newList.Add(item);
            }

            return newList;
        }


        public SKEPSupportingDocumentModel MapSupportingDocumentModelSKEP(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new SKEPSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BrandRegSupportingDocumentModel MapSupportingDocumentModelBrand(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new BrandRegSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProductDevSupportingDocumentModel MapSupportingDocumentModelProduct(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new ProductDevSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    },
                    FileList = AutoMapper.Mapper.Map<List<FileUploadModel>>(entity.FILE_UPLOAD).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PlantGeneralModel MapPlantModel(CustomService.Data.MASTER_PLANT plant)
        {
            try
            {
                return new PlantGeneralModel()
                {
                    IdPlant = plant.WERKS,
                    Name = plant.NAME1,
                    Address = plant.ADDRESS
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private NppbkcGeneralModel MapNppbkcModel(CustomService.Data.MASTER_NPPBKC nppbkc)
        {
            try
            {
                return new NppbkcGeneralModel()
                {
                    Id = nppbkc.NPPBKC_ID,
                    Region = nppbkc.REGION,
                    Address = String.Format("{0}, {1}", nppbkc.ADDR1, nppbkc.ADDR2),
                    City = nppbkc.CITY,
                    CityAlias = nppbkc.CITY_ALIAS,
                    KppbcId = nppbkc.KPPBC_ID,
                    Company = (nppbkc.COMPANY != null) ? new CompanyModel()
                    {
                        Id = nppbkc.COMPANY.BUKRS,
                        Name = nppbkc.COMPANY.BUTXT,
                        Alias = nppbkc.COMPANY.BUTXT_ALIAS,
                        Npwp = nppbkc.COMPANY.NPWP,
                        City = nppbkc.CITY,
                        PKP = nppbkc.COMPANY.PKP
                    } : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private POAGeneralModel MapPoaModel(CustomService.Data.POA poa)
        {
            try
            {
                return new POAGeneralModel()
                {
                    Id = poa.POA_ID,
                    Name = String.Format("{0} {1}", poa.USER_LOGIN.FIRST_NAME, poa.USER_LOGIN.LAST_NAME),
                    Address = poa.POA_ADDRESS,
                    Position = poa.TITLE
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private CompanyModel MapCompanyModel(CustomService.Data.T001 company)
        {
            try
            {
                return new CompanyModel()
                {
                    Id = company.BUKRS,
                    Name = company.BUTXT,
                    Address = company.SPRAS
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900); //EDIT: i've typed 400 instead 900
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }


        public string GenerateFormNumber(string CompanyAlias, string CityAlias, DateTime RequestDate)
        {
            string new_number = "";
            string partial_number = CompanyAlias + "/" + CityAlias + "/" + ToRoman(RequestDate.Month) + "/" + RequestDate.Year.ToString();
            new_number = brandRegistrationService.SetNewFormNumber(partial_number);
            return new_number;
        }

        //public bool IsPOACanApprove(long CRId, string UserId, string LastApprovedBy = "")
        //{
        //    var isOk = false;
        //    if (CRId != 0)
        //    {
        //        if (LastApprovedBy == UserId)
        //        {
        //            isOk = true;
        //        }
        //        else
        //        {
        //            var approverlist = brandRegistrationService.GetPOAApproverList(CRId);
        //            if (approverlist.Count() > 0)
        //            {
        //                var isexist = approverlist.Where(w => w.POA_ID.Equals(UserId)).ToList();
        //                if (isexist.Count() > 0)
        //                {
        //                    isOk = true;
        //                }
        //            }
        //        }
        //    }
        //    return isOk;
        //}
        public bool IsPOACanAccess(long IRId, string UserId)
        {
            var isOk = true;
            var CreatorId = "";
            var IRequest = brandRegistrationService.GetBrandRegistrationById(IRId).FirstOrDefault();
            if (IRequest != null)
            {
                CreatorId = IRequest.CREATED_BY;
            }
            if (CreatorId != "")
            {
                if (UserId != CreatorId)
                {
                    /////// Check delegation ///////
                    isOk = IsPOADelegate(UserId, CreatorId);
                }
            }
            return isOk;
        }

        private bool IsPOADelegate(string UserId, string CreatorId)
        {
            var isOk = false;
            var poadelegate = brandRegistrationService.GetPOADelegationOfUser(CreatorId);
            if (poadelegate != null)
            {
                if (UserId == poadelegate.POA_TO)
                {
                    isOk = true;
                }
            }
            return isOk;
        }



        #endregion

        #region Print Out

        [HttpPost]
        public ActionResult RestorePrintoutToDefault(int RegistrationType, string CreatedBy, bool DocExport, string LayoutName)
        {
            var LayoutName_field = "";
            switch (LayoutName)
            {
                case "main":
                    if (RegistrationType == 1)
                    {
                        LayoutName_field = "BRAND_REGISTRATION_NEWBRAND_MAIN";
                    }
                    else
                    {
                        LayoutName_field = "BRAND_REGISTRATION_UPDATEHJE_MAIN";
                    }

                    break;

                case "surat_pernyataan":
                    LayoutName_field = "BRAND_REGISTRATION_SURAT_PERNYATAAN";
                    break;

                case "item":
                    if (RegistrationType == 1)
                    {
                        LayoutName_field = "BRAND_REGISTRATION_LISTBRAND";
                    }
                    else
                    {
                        LayoutName_field = "BRAND_REGISTRATION_LISTITEM";
                    }
                    break;


                case "export":
                    LayoutName_field = "BRAND_REGISTRATION_EXPORT";
                    break;
            }

            var ErrMessage = refService.RestorePrintoutToDefault(LayoutName_field, CreatedBy);
            return Json(ErrMessage);

        }

        [HttpPost]
        public ActionResult GeneratePrintout(long ID)
        {
            var _BRViewModel = new BrandRegistrationReqViewModel();
            var layout = GetPrintout(_BRViewModel, ID);
            return Json(layout);
        }

        [HttpPost]
        public ActionResult GeneratePrintoutSuratPernyataan(long ID)
        {
            var _BRViewModel = new BrandRegistrationReqViewModel();
            var layout = GetPrintoutSuratPernyataan(_BRViewModel, ID);
            return Json(layout);
        }



        [HttpPost]
        public ActionResult GeneratePrintoutItem(long ID)
        {
            var _BRViewModel = new BrandRegistrationReqViewModel();
            var layout = GetPrintoutBrand(_BRViewModel, ID);
            return Json(layout);
        }

        [HttpPost]
        public ActionResult GeneratePrintoutExport(long ID)
        {
            var _BRViewModel = new BrandRegistrationReqViewModel();
            var layout = GetPrintoutExport(_BRViewModel, ID);
            return Json(layout);
        }



        private string GetPrintout(BrandRegistrationReqViewModel _BRViewModel, long ID)
        {
            _BRViewModel.ViewModel = GetBrandRegistarationMaster(ID);

            var detail = brandRegistrationService.GetBrandDetailByRegistrationID(ID);
            _BRViewModel.Item = detail.Select(s => new BrandRegistrationReqDetailModel
            {
                Registration_Detail_ID = s.REGISTRATION_DETAIL_ID,
                Registration_ID = s.REGISTRATION_ID,
                Brand_Ce = s.BRAND_CE,
                Prod_Code = s.PROD_CODE,
                Company_Tier = s.COMPANY_TIER,
                Unit = s.UNIT,
                Brand_Content = s.BRAND_CONTENT,
                Tariff = s.TARIFF ?? 0,
                PD_Detail_ID = s.PD_DETAIL_ID,
                Hje = s.HJE,
                HJEperPack = s.HJE,
                HJEperBatang = s.HJE,
                Packaging_Material = s.MATERIAL_PACKAGE,
                Front_Side = s.FRONT_SIDE,
                Back_Side = s.BACK_SIDE,
                Left_Side = s.LEFT_SIDE,
                Right_Side = s.RIGHT_SIDE,
                Top_Side = s.TOP_SIDE,
                Bottom_Side = s.BOTTOM_SIDE,
                ProductType = s.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS,


                Request_No = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                Fa_Code_Old = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                Fa_Code_New_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                CompanyName = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                HlCode = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                MarketDesc = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                Is_Import = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT
            }).ToList();

            var filesupload = brandRegistrationService.GetFileUploadByRegID(ID).ToList();

            var attachments = "";
            var ext = "";
            int attachment_number = 1;
            if(filesupload.Count() > 0)
            {
                attachments += "<table style='font-family:Arial;font-size:10pt'>";

                foreach(var file_attach in filesupload)
                {
                    if(file_attach.FILE_NAME != "")
                    {
                        ext = file_attach.PATH_URL.Substring((file_attach.PATH_URL.Length - 3), 3);
                        if (ext == "pdf")
                        {
                            attachments += "<tr><td style='font-family:Arial;font-size:10pt'>" + attachment_number.ToString() + "</td><td style='font-family:Arial;font-size:10pt'>" + file_attach.FILE_NAME + "</td></tr>";
                            attachment_number++;
                        }
                    }
                }

                attachments += "</table>";
            }

            _BRViewModel.Count_Lamp = filesupload.Count();

            var POAData = brandRegistrationService.GetPOAData(_BRViewModel.ViewModel.Created_By);

            var ProductAlias = brandRegistrationService.GetProductAlias(ID);

            var Plants = brandRegistrationService.GetPlantByNPPBKC(_BRViewModel.ViewModel.Nppbkc_ID).ToList();

            var company_address_text = "";
            var main_plant_address = "";
            int no_alamat = 1;
            foreach (var plant in Plants)
            {
                if (plant.IS_MAIN_PLANT == true)
                {
                    main_plant_address = plant.ADDRESS;
                }
                else
                {
                    company_address_text += no_alamat + ". " + plant.ADDRESS + "<BR/>";
                }
                no_alamat++;
            }

            System.Globalization.CultureInfo cultureID = new System.Globalization.CultureInfo("id-ID");

            var parameters = new Dictionary<string, string>();
            parameters.Add("REGISTRATION_NO", _BRViewModel.ViewModel.Registration_No);
            parameters.Add("COMPANY_NAME", _BRViewModel.ViewModel.Company.Name);
            parameters.Add("MAIN_PLANT_ADDRESS", main_plant_address);
            parameters.Add("COMPANY_ADDRESS", company_address_text);
            parameters.Add("COMPANY_CITY", _BRViewModel.ViewModel.Company.City);
            parameters.Add("SUBMISSION_DATE", Convert.ToDateTime(_BRViewModel.ViewModel.Submission_Date).ToString("dd MMMM yyyy", cultureID));
            parameters.Add("TEXT_TO", _BRViewModel.ViewModel.Text_To);
            parameters.Add("LAMPIRAN_COUNT", Convert.ToString(_BRViewModel.Count_Lamp) + " (" + TerbilangLong2(_BRViewModel.Count_Lamp) + ")");
            parameters.Add("POA_NAME", POAData.PRINTED_NAME);
            parameters.Add("POA_POSITION", POAData.TITLE);
            parameters.Add("POA_ADDRESS", POAData.POA_ADDRESS);
            parameters.Add("NPPBKC", _BRViewModel.ViewModel.Nppbkc_ID);
            parameters.Add("NPWP", _BRViewModel.ViewModel.Company.Npwp);
            parameters.Add("PKP", _BRViewModel.ViewModel.Company.PKP);
            parameters.Add("ATTACHMENTS", attachments);
            parameters.Add("PRODUCT_ALIAS", ProductAlias);

            var layout_item_updates = "";
            var layout_registration_items = "";
            int no_item_update = 1;

            var masterBrand = new ZAIDM_EX_BRAND();
            bool Doc_Export = false;
            string MarketDescription = "";
            decimal HJEIDR = 0;
            decimal HJEIDRPerBatang = 0;
            decimal Tarif = 0;
            foreach (var item in _BRViewModel.Item)
            {
                item.HJEperBatang = item.Hje / Convert.ToInt32(item.Brand_Content);
                if (item.Market_ID == "02")
                {
                    MarketDescription = "Luar Negeri";
                    Doc_Export = true;
                }
                else
                {
                    MarketDescription = "Dalam Negeri";
                }


                layout_item_updates += "<table width='100%' border=1 cellpadding=2 cellspacing=0 style='border-collapse:collapse; vertical-align:top; font-family:Arial;font-size:10pt'>";
                layout_item_updates += "<tr><td colspan='4' style='font-family:Arial;font-size:10pt'><b>" + no_item_update.ToString() + ". Tarif Cukai</b> Rp. " + item.Tariff.ToString("#,##") + " / " + item.Unit + "</td></tr>";
                layout_item_updates += "<tr><td width='5%'></td><td width='40%' style='vertical-align:top; font-family:Arial;font-size:10pt'>Merk</td><td width='5%' align='center' style='vertical-align:top; font-family:Arial;font-size:10pt'>:</td><td width='50%' style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Brand_Ce + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Jenis HT</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.ProductType + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Golongan Pengusaha Pabrik</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + refService.GetReferenceById(item.Company_Tier).REFF_VALUE + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>HJE (per kemasan)</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.HJEperPack.ToString("#,##") + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>HJE (per batang/gram)</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.HJEperBatang.ToString("#,##0.00") + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Isi Kemasan</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Brand_Content + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Bahan Kemasan</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Packaging_Material + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Tujuan Pemasaran</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " +  MarketDescription + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td colspan='3' style='vertical-align:top; font-family:Arial;font-size:10pt'><b>Tampilan kemasan :</b></td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi depan</td><td style='vertical-align:top; text-align:center; font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Front_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi belakang</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Back_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi kiri</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Left_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi kanan</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Right_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi atas</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Top_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi bawah</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Bottom_Side + "</td></tr>";
                layout_item_updates += "</table><br/>";


                masterBrand = brandRegistrationService.FindBrandByFACode(item.Fa_Code_Old);


                layout_registration_items += "<tr>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + no_item_update.ToString() + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Brand_Ce + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.ProductType + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + Convert.ToInt32(item.Brand_Content) + "</td>";
                if (masterBrand != null)
                {
                    HJEIDR = masterBrand.HJE_IDR ?? 0;
                    HJEIDRPerBatang = HJEIDR / Convert.ToInt32(masterBrand.BRAND_CONTENT);
                    Tarif = masterBrand.TARIFF ?? 0;


                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SKEP_NO + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SKEP_DATE.Value.ToString("dd MMMM yyyy", cultureID) + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SERIES_CODE + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + HJEIDR.ToString("#,##") + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + HJEIDRPerBatang.ToString("#,##0.00") + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + Tarif.ToString("#,##") + "</td>";
                }
                else
                {
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                }


                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Company_Tier + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Hje.ToString("#,##") + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.HJEperBatang.ToString("#,##0.00") + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Tariff.ToString("#,##") + "</td>";
                layout_registration_items += "</tr>";

                no_item_update++;
            }
            parameters.Add("ITEMS", layout_item_updates);

            var layout = "";
            if (_BRViewModel.ViewModel.Registration_Type == 1)
            {
                if (Doc_Export)
                {
                    parameters.Add("REGISTRATION_ITEMS", "");
                    layout += refService.GeneratePrintout("BRAND_REGISTRATION_NEWBRAND_MAIN", parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";
                    //layout += refService.GeneratePrintout("BRAND_REGISTRATION_EXPORT", parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";
                }
                else
                {
                    parameters.Add("REGISTRATION_ITEMS", "");
                    layout += refService.GeneratePrintout("BRAND_REGISTRATION_NEWBRAND_MAIN", parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";
                    //layout += refService.GeneratePrintout("BRAND_REGISTRATION_NEWBRAND", parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";
                }

            }
            else
            {
                parameters.Add("REGISTRATION_ITEMS", layout_registration_items);
                parameters.Add("EFFECTIVE_DATE", _BRViewModel.ViewModel.Effective_Date.ToString("dd MMMM yyyy", cultureID));
                layout += refService.GeneratePrintout("BRAND_REGISTRATION_UPDATEHJE_MAIN", parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";
            }

            return layout;
        }

        private string GetPrintoutSuratPernyataan(BrandRegistrationReqViewModel _BRViewModel, long ID)
        {
            _BRViewModel.ViewModel = GetBrandRegistarationMaster(ID);

            var detail = brandRegistrationService.GetBrandDetailByRegistrationID(ID);
            _BRViewModel.Item = detail.Select(s => new BrandRegistrationReqDetailModel
            {
                Registration_Detail_ID = s.REGISTRATION_DETAIL_ID,
                Registration_ID = s.REGISTRATION_ID,
                Brand_Ce = s.BRAND_CE,
                Prod_Code = s.PROD_CODE,
                Company_Tier = s.COMPANY_TIER,
                Unit = s.UNIT,
                Brand_Content = s.BRAND_CONTENT,
                Tariff = s.TARIFF ?? 0,
                PD_Detail_ID = s.PD_DETAIL_ID,
                Hje = s.HJE,
                HJEperPack = s.HJE,
                HJEperBatang = s.HJE,
                Packaging_Material = s.MATERIAL_PACKAGE,
                Front_Side = s.FRONT_SIDE,
                Back_Side = s.BACK_SIDE,
                Left_Side = s.LEFT_SIDE,
                Right_Side = s.RIGHT_SIDE,
                Top_Side = s.TOP_SIDE,
                Bottom_Side = s.BOTTOM_SIDE,
                ProductType = s.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS,


                Request_No = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                Fa_Code_Old = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                Fa_Code_New_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                CompanyName = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                HlCode = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                MarketDesc = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                Is_Import = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT
            }).ToList();

            var filesupload = brandRegistrationService.GetFileUploadByRegID(ID).ToList();

            var attachments = "";
            var ext = "";
            int attachment_number = 1;
            if (filesupload.Count() > 0)
            {
                attachments += "<table style='vertical-align:top; font-family:Arial;font-size:10pt'>";

                foreach (var file_attach in filesupload)
                {
                    ext = file_attach.PATH_URL.Substring((file_attach.PATH_URL.Length - 3), 3);
                    if (ext == "pdf")
                    {
                        attachments += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + attachment_number.ToString() + "</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + file_attach.FILE_NAME + "</td></tr>";
                        attachment_number++;
                    }
                }

                attachments += "</table>";
            }

            _BRViewModel.Count_Lamp = filesupload.Count();

            var POAData = brandRegistrationService.GetPOAData(_BRViewModel.ViewModel.Created_By);

            var ProductAlias = brandRegistrationService.GetProductAlias(ID);

            var Plants = brandRegistrationService.GetPlantByNPPBKC(_BRViewModel.ViewModel.Nppbkc_ID).ToList();

            var company_address_text = "";
            var main_plant_address = "";
            int no_alamat = 1;
            foreach (var plant in Plants)
            {
                if (plant.IS_MAIN_PLANT == true)
                {
                    main_plant_address = plant.ADDRESS;
                }
                else
                {
                    company_address_text += no_alamat + ". " + plant.ADDRESS + "<BR/>";
                }
                no_alamat++;
            }

            System.Globalization.CultureInfo cultureID = new System.Globalization.CultureInfo("id-ID");

            var parameters = new Dictionary<string, string>();
            parameters.Add("REGISTRATION_NO", _BRViewModel.ViewModel.Registration_No);
            parameters.Add("COMPANY_NAME", _BRViewModel.ViewModel.Company.Name);
            parameters.Add("MAIN_PLANT_ADDRESS", main_plant_address);
            parameters.Add("COMPANY_ADDRESS", company_address_text);
            parameters.Add("COMPANY_CITY", _BRViewModel.ViewModel.Company.City);
            parameters.Add("SUBMISSION_DATE", Convert.ToDateTime(_BRViewModel.ViewModel.Submission_Date).ToString("dd MMMM yyyy", cultureID));
            parameters.Add("TEXT_TO", _BRViewModel.ViewModel.Text_To);
            parameters.Add("LAMPIRAN_COUNT", Convert.ToString(_BRViewModel.Count_Lamp) + " (" + TerbilangLong2(_BRViewModel.Count_Lamp) + ")");
            parameters.Add("POA_NAME", POAData.PRINTED_NAME);
            parameters.Add("POA_POSITION", POAData.TITLE);
            parameters.Add("POA_ADDRESS", POAData.POA_ADDRESS);
            parameters.Add("NPPBKC", _BRViewModel.ViewModel.Nppbkc_ID);
            parameters.Add("NPWP", _BRViewModel.ViewModel.Company.Npwp);
            parameters.Add("PKP", _BRViewModel.ViewModel.Company.PKP);
            parameters.Add("ATTACHMENTS", attachments);
            parameters.Add("PRODUCT_ALIAS", ProductAlias);

            var layout_item_updates = "";
            var layout_registration_items = "";
            int no_item_update = 1;

            var masterBrand = new ZAIDM_EX_BRAND();
            bool Doc_Export = false;
            string MarketDescription = "";
            decimal HJEIDR = 0;
            decimal HJEIDRPerBatang = 0;
            decimal Tarif = 0;
            foreach (var item in _BRViewModel.Item)
            {
                item.HJEperBatang = item.Hje / Convert.ToInt32(item.Brand_Content);
                if (item.Market_ID == "02")
                {
                    MarketDescription = "Luar Negeri";
                    Doc_Export = true;
                }
                else
                {
                    MarketDescription = "Dalam Negeri";
                }


                layout_item_updates += "<table width='100%' border=1 cellpadding=2 cellspacing=0 style='border-collapse:collapse; vertical-align:top; font-family:Arial;font-size:10pt'>";
                layout_item_updates += "<tr><td colspan='4' style='font-family:Arial;font-size:10pt'><b>" + no_item_update.ToString() + ". Tarif Cukai</b> Rp. " + item.Tariff.ToString("#,##") + " / " + item.Unit + "</td></tr>";
                layout_item_updates += "<tr><td width='5%'></td><td width='40%' style='vertical-align:top; font-family:Arial;font-size:10pt'>Merk</td><td width='5%' align='center' style='vertical-align:top; font-family:Arial;font-size:10pt'>:</td><td width='50%' style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Brand_Ce + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Jenis HT</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.ProductType + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Golongan Pengusaha Pabrik</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + refService.GetReferenceById(item.Company_Tier).REFF_VALUE + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>HJE (per kemasan)</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.HJEperPack.ToString("#,##") + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>HJE (per batang/gram)</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.HJEperBatang.ToString("#,##0.00") + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Isi Kemasan</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Brand_Content + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Bahan Kemasan</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Packaging_Material + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Tujuan Pemasaran</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + MarketDescription + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td colspan='3' style='vertical-align:top; font-family:Arial;font-size:10pt'><b>Tampilan kemasan :</b></td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi depan</td><td style='vertical-align:top; text-align:center; font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Front_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi belakang</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Back_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi kiri</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Left_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi kanan</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Right_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi atas</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Top_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi bawah</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Bottom_Side + "</td></tr>";
                layout_item_updates += "</table><br/>";


                masterBrand = brandRegistrationService.FindBrandByFACode(item.Fa_Code_Old);


                layout_registration_items += "<tr>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + no_item_update.ToString() + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Brand_Ce + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.ProductType + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + Convert.ToInt32(item.Brand_Content) + "</td>";
                if (masterBrand != null)
                {
                    HJEIDR = masterBrand.HJE_IDR ?? 0;
                    HJEIDRPerBatang = HJEIDR / Convert.ToInt32(masterBrand.BRAND_CONTENT);
                    Tarif = masterBrand.TARIFF ?? 0;


                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SKEP_NO + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SKEP_DATE.Value.ToString("dd MMMM yyyy", cultureID) + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SERIES_CODE + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + HJEIDR.ToString("#,##") + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + HJEIDRPerBatang.ToString("#,##0.00") + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + Tarif.ToString("#,##") + "</td>";
                }
                else
                {
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                }


                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Company_Tier + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Hje.ToString("#,##") + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.HJEperBatang.ToString("#,##0.00") + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Tariff.ToString("#,##") + "</td>";
                layout_registration_items += "</tr>";

                no_item_update++;
            }
            parameters.Add("ITEMS", layout_item_updates);

            var layout = "";
            parameters.Add("REGISTRATION_ITEMS", "");
            layout += refService.GeneratePrintout("BRAND_REGISTRATION_SURAT_PERNYATAAN", parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";

            return layout;
        }

        private string GetPrintoutExport(BrandRegistrationReqViewModel _BRViewModel, long ID)
        {
            _BRViewModel.ViewModel = GetBrandRegistarationMaster(ID);

            var detail = brandRegistrationService.GetBrandDetailByRegistrationID(ID);
            _BRViewModel.Item = detail.Select(s => new BrandRegistrationReqDetailModel
            {
                Registration_Detail_ID = s.REGISTRATION_DETAIL_ID,
                Registration_ID = s.REGISTRATION_ID,
                Brand_Ce = s.BRAND_CE,
                Prod_Code = s.PROD_CODE,
                Company_Tier = s.COMPANY_TIER,
                Unit = s.UNIT,
                Brand_Content = s.BRAND_CONTENT,
                Tariff = s.TARIFF ?? 0,
                PD_Detail_ID = s.PD_DETAIL_ID,
                Hje = s.HJE,
                HJEperPack = s.HJE,
                HJEperBatang = s.HJE,
                Packaging_Material = s.MATERIAL_PACKAGE,
                Front_Side = s.FRONT_SIDE,
                Back_Side = s.BACK_SIDE,
                Left_Side = s.LEFT_SIDE,
                Right_Side = s.RIGHT_SIDE,
                Top_Side = s.TOP_SIDE,
                Bottom_Side = s.BOTTOM_SIDE,
                ProductType = s.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS,


                Request_No = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                Fa_Code_Old = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                Fa_Code_New_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                CompanyName = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                HlCode = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                MarketDesc = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                Is_Import = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT
            }).ToList();

            var filesupload = brandRegistrationService.GetFileUploadByRegID(ID).ToList();

            var attachments = "";
            var ext = "";
            int attachment_number = 1;
            if (filesupload.Count() > 0)
            {
                attachments += "<table style='font-family:Arial;font-size:10pt'>";

                foreach (var file_attach in filesupload)
                {
                    if (file_attach.FILE_NAME != "")
                    {
                        ext = file_attach.PATH_URL.Substring((file_attach.PATH_URL.Length - 3), 3);
                        if (ext == "pdf")
                        {
                            attachments += "<tr><td style='font-family:Arial;font-size:10pt'>" + attachment_number.ToString() + "</td><td style='font-family:Arial;font-size:10pt'>" + file_attach.FILE_NAME + "</td></tr>";
                            attachment_number++;
                        }
                    }
                }

                attachments += "</table>";
            }

            _BRViewModel.Count_Lamp = filesupload.Count();

            var POAData = brandRegistrationService.GetPOAData(_BRViewModel.ViewModel.Created_By);

            var ProductAlias = brandRegistrationService.GetProductAlias(ID);

            var Plants = brandRegistrationService.GetPlantByNPPBKC(_BRViewModel.ViewModel.Nppbkc_ID).ToList();

            var company_address_text = "";
            var main_plant_address = "";
            foreach (var plant in Plants)
            {
                if (plant.IS_MAIN_PLANT == true)
                {
                    main_plant_address = plant.ADDRESS;
                }
                company_address_text += plant.ADDRESS + "<BR/>";
            }

            System.Globalization.CultureInfo cultureID = new System.Globalization.CultureInfo("id-ID");

            var parameters = new Dictionary<string, string>();
            parameters.Add("REGISTRATION_NO", _BRViewModel.ViewModel.Registration_No);
            parameters.Add("COMPANY_NAME", _BRViewModel.ViewModel.Company.Name);
            parameters.Add("MAIN_PLANT_ADDRESS", main_plant_address);
            parameters.Add("COMPANY_ADDRESS", company_address_text);
            parameters.Add("COMPANY_CITY", _BRViewModel.ViewModel.Company.City);
            parameters.Add("SUBMISSION_DATE", Convert.ToDateTime(_BRViewModel.ViewModel.Submission_Date).ToString("dd MMMM yyyy", cultureID));
            parameters.Add("TEXT_TO", _BRViewModel.ViewModel.Text_To);
            parameters.Add("LAMPIRAN_COUNT", Convert.ToString(_BRViewModel.Count_Lamp) + " (" + TerbilangLong2(_BRViewModel.Count_Lamp) + ")");
            parameters.Add("POA_NAME", POAData.PRINTED_NAME);
            parameters.Add("POA_POSITION", POAData.TITLE);
            parameters.Add("POA_ADDRESS", POAData.POA_ADDRESS);
            parameters.Add("NPPBKC", _BRViewModel.ViewModel.Nppbkc_ID);
            parameters.Add("NPWP", _BRViewModel.ViewModel.Company.Npwp);
            parameters.Add("PKP", _BRViewModel.ViewModel.Company.PKP);
            parameters.Add("ATTACHMENTS", attachments);
            parameters.Add("PRODUCT_ALIAS", ProductAlias);

            var layout_item_updates = "";
            var layout_registration_items = "";
            int no_item_update = 1;

            var masterBrand = new ZAIDM_EX_BRAND();
            bool Doc_Export = false;
            string MarketDescription = "";
            decimal HJEIDR = 0;
            decimal HJEIDRPerBatang = 0;
            decimal Tarif = 0;
            foreach (var item in _BRViewModel.Item)
            {
                item.HJEperBatang = item.Hje / Convert.ToInt32(item.Brand_Content);
                if (item.Market_ID == "02")
                {
                    MarketDescription = "Luar Negeri";
                    Doc_Export = true;
                }
                else
                {
                    MarketDescription = "Dalam Negeri";
                }


                layout_item_updates += "<table width='100%' border=1 cellpadding=2 cellspacing=0 style='border-collapse:collapse; vertical-align:top; font-family:Arial;font-size:10pt'>";
                layout_item_updates += "<tr><td colspan='4' style='font-family:Arial;font-size:10pt'><b>" + no_item_update.ToString() + ". Tarif Cukai</b> Rp. " + item.Tariff.ToString("#,##") + " / " + item.Unit + "</td></tr>";
                layout_item_updates += "<tr><td width='5%'></td><td width='40%' style='vertical-align:top; font-family:Arial;font-size:10pt'>Merk</td><td width='5%' align='center' style='vertical-align:top; font-family:Arial;font-size:10pt'>:</td><td width='50%' style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Brand_Ce + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Jenis HT</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.ProductType + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Golongan Pengusaha Pabrik</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + refService.GetReferenceById(item.Company_Tier).REFF_VALUE + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>HJE (per kemasan)</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.HJEperPack.ToString("#,##") + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>HJE (per batang/gram)</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.HJEperBatang.ToString("#,##0.00") + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Isi Kemasan</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Brand_Content + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Bahan Kemasan</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Packaging_Material + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>Tujuan Pemasaran</td><td align='center'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + MarketDescription + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td colspan='3' style='vertical-align:top; font-family:Arial;font-size:10pt'><b>Tampilan kemasan :</b></td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi depan</td><td style='vertical-align:top; text-align:center; font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Front_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi belakang</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'> " + item.Back_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi kiri</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Left_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi kanan</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Right_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi atas</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Top_Side + "</td></tr>";
                layout_item_updates += "<tr><td style='vertical-align:top; font-family:Arial;font-size:10pt'></td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>&#9679;&nbsp;&nbsp;Sisi bawah</td><td style='vertical-align:top; text-align:center;font-family:Arial;font-size:10pt'>:</td><td style='vertical-align:top; font-family:Arial;font-size:10pt'>  " + item.Bottom_Side + "</td></tr>";
                layout_item_updates += "</table><br/>";


                masterBrand = brandRegistrationService.FindBrandByFACode(item.Fa_Code_Old);


                layout_registration_items += "<tr>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + no_item_update.ToString() + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Brand_Ce + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.ProductType + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + Convert.ToInt32(item.Brand_Content) + "</td>";
                if (masterBrand != null)
                {
                    HJEIDR = masterBrand.HJE_IDR ?? 0;
                    HJEIDRPerBatang = HJEIDR / Convert.ToInt32(masterBrand.BRAND_CONTENT);
                    Tarif = masterBrand.TARIFF ?? 0;


                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SKEP_NO + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SKEP_DATE.Value.ToString("dd MMMM yyyy", cultureID) + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + masterBrand.SERIES_CODE + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + HJEIDR.ToString("#,##") + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + HJEIDRPerBatang.ToString("#,##0.00") + "</td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + Tarif.ToString("#,##") + "</td>";
                }
                else
                {
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                }


                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Company_Tier + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Hje.ToString("#,##") + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.HJEperBatang.ToString("#,##0.00") + "</td>";
                layout_registration_items += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'>" + item.Tariff.ToString("#,##") + "</td>";
                layout_registration_items += "</tr>";

                no_item_update++;
            }
            parameters.Add("ITEMS", layout_item_updates);

            var layout = "";
            parameters.Add("REGISTRATION_ITEMS", "");
            layout += refService.GeneratePrintout("BRAND_REGISTRATION_EXPORT", parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";
            return layout;
        }



        private string GetPrintoutBrand(BrandRegistrationReqViewModel _BRViewModel, long ID)
        {
            System.Globalization.CultureInfo cultureID = new System.Globalization.CultureInfo("id-ID");

            _BRViewModel.ViewModel = GetBrandRegistarationMaster(ID);

            var detail = brandRegistrationService.GetBrandDetailByRegistrationID(ID);
            _BRViewModel.Item = detail.Select(s => new BrandRegistrationReqDetailModel
            {
                Registration_Detail_ID = s.REGISTRATION_DETAIL_ID,
                Registration_ID = s.REGISTRATION_ID,
                Brand_Ce = s.BRAND_CE,
                Prod_Code = s.PROD_CODE,
                Company_Tier = s.COMPANY_TIER,
                Unit = s.UNIT,
                Brand_Content = s.BRAND_CONTENT,
                Tariff = s.TARIFF ?? 0,
                PD_Detail_ID = s.PD_DETAIL_ID,
                Hje = s.HJE,
                HJEperPack = s.HJE,
                HJEperBatang = s.HJE,
                Packaging_Material = s.MATERIAL_PACKAGE,
                Front_Side = s.FRONT_SIDE,
                Back_Side = s.BACK_SIDE,
                Left_Side = s.LEFT_SIDE,
                Right_Side = s.RIGHT_SIDE,
                Top_Side = s.TOP_SIDE,
                Bottom_Side = s.BOTTOM_SIDE,
                ProductType = s.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS,


                Request_No = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                Fa_Code_Old = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                Fa_Code_New_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                CompanyName = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                HlCode = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                MarketDesc = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                Is_Import = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT
            }).ToList();

            var POAData = brandRegistrationService.GetPOAData(_BRViewModel.ViewModel.Created_By);
            var Plants = brandRegistrationService.GetPlantByNPPBKC(_BRViewModel.ViewModel.Nppbkc_ID).ToList();

            decimal HJEIDR = 0;
            decimal HJEIDRPerBatang = 0;
            decimal Tarif = 0;

            string layout_brands = "";
            int no_item_update = 1;
            string printout_layout_name = "";
            if (_BRViewModel.ViewModel.Registration_Type == 1)
            {
                printout_layout_name = "BRAND_REGISTRATION_LISTBRAND";
                var list_of_brands = brandRegistrationService.FindBrandByCompanies(Plants).Select(s => new { BrandName = s.BRAND_CE, ProductAlias = s.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS, HJE = s.HJE_IDR, BrandContent = s.BRAND_CONTENT, SKEPNo = s.SKEP_NO, SKEPDate = s.SKEP_DATE, Tarif = s.TARIFF }).Distinct().OrderByDescending(o => o.SKEPDate).ThenBy(o => o.ProductAlias).ThenBy(o => o.BrandName).ToList();

                foreach (var brand in list_of_brands)
                {
                    HJEIDR = brand.HJE ?? 0;
                    Tarif = brand.Tarif ?? 0;

                    layout_brands += "<tr>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: left'>" + no_item_update.ToString() + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: left'>" + brand.BrandName + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + brand.ProductAlias + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + HJEIDR.ToString("#,##") + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + Convert.ToInt32(brand.BrandContent).ToString("#,##") + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + brand.SKEPNo + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + brand.SKEPDate.Value.ToString("dd-MMM-yy", cultureID) + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + Tarif.ToString("#,##0.00") + "</td>";
                    layout_brands += "<td></td>";
                    layout_brands += "</tr>";
                    no_item_update++;
                }

            }
            else
            {
                printout_layout_name = "BRAND_REGISTRATION_LISTITEM";

                foreach (var item in _BRViewModel.Item)
                {
                    item.HJEperBatang = item.Hje / Convert.ToInt32(item.Brand_Content);

                    var masterBrand = brandRegistrationService.FindBrandByFACode(item.Fa_Code_Old);

                    layout_brands += "<tr>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: left'>" + no_item_update.ToString() + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: left'>" + item.Brand_Ce + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + item.ProductType + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + Convert.ToInt32(item.Brand_Content).ToString("#,##") + "</td>";
                    if (masterBrand != null)
                    {
                        HJEIDR = masterBrand.HJE_IDR ?? 0;
                        HJEIDRPerBatang = HJEIDR / Convert.ToInt32(masterBrand.BRAND_CONTENT);
                        Tarif = masterBrand.TARIFF ?? 0;


                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + masterBrand.SKEP_NO + "</td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + masterBrand.SKEP_DATE.Value.ToString("dd-MMM-yy") + "</td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + masterBrand.SERIES_CODE + "</td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + HJEIDR.ToString("#,##") + "</td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + HJEIDRPerBatang.ToString("#,##0.00") + "</td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + Tarif.ToString("#,##") + "</td>";
                    }
                    else
                    {
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                        layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt'></td>";
                    }


                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + refService.GetReferenceById(item.Company_Tier).REFF_VALUE + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + item.Hje.ToString("#,##") + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + item.HJEperBatang.ToString("#,##0.00") + "</td>";
                    layout_brands += "<td style='vertical-align:top; font-family:Arial;font-size:10pt; text-align: center'>" + item.Tariff.ToString("#,##") + "</td>";
                    layout_brands += "</tr>";

                    no_item_update++;
                }
            }


            var parameters = new Dictionary<string, string>();
            parameters.Add("COMPANY_NAME", _BRViewModel.ViewModel.Company.Name);
            parameters.Add("NPPBKC", _BRViewModel.ViewModel.Nppbkc_ID);
            parameters.Add("REGISTRATION_ITEMS", layout_brands);
            parameters.Add("POA_NAME", POAData.PRINTED_NAME);

            var layout = "";
            layout += refService.GeneratePrintout(printout_layout_name, parameters, _BRViewModel.ViewModel.Created_By).LAYOUT + "<br /><br /><br />";

            return layout;
        }

        public ActionResult GetPrintOutLayout(int RegistrationType, string CreatedBy, bool DocExport, string LayoutName)
        {
            var LayoutName_field = "";
            switch (LayoutName)
            {
                case "main":
                    if (RegistrationType == 1)
                    {
                        LayoutName_field = "BRAND_REGISTRATION_NEWBRAND_MAIN";
                    }
                    else
                    {
                        LayoutName_field = "BRAND_REGISTRATION_UPDATEHJE_MAIN";
                    }

                    break;

                case "surat_pernyataan":
                    LayoutName_field = "BRAND_REGISTRATION_SURAT_PERNYATAAN";
                    break;

                case "item":
                    if (RegistrationType == 1)
                    {
                        LayoutName_field = "BRAND_REGISTRATION_LISTBRAND";
                    }
                    else
                    {
                        LayoutName_field = "BRAND_REGISTRATION_LISTITEM";
                    }
                    break;


                case "export":
                    LayoutName_field = "BRAND_REGISTRATION_EXPORT";
                    break;
            }

            var result = refService.GetPrintoutLayout(LayoutName_field, CreatedBy);
            var layout = "No Layout Found.";
            if (result.Any())
            {
                layout = result.FirstOrDefault().LAYOUT;
            }
            return Json(layout);


            //if (RegistrationType == 1)
            //{
            //    if (DocExport)
            //    {
            //        var result = refService.GetPrintoutLayout("BRAND_REGISTRATION_EXPORT", CreatedBy);
            //        var layout = "No Layout Found.";
            //        if (result.Any())
            //        {
            //            layout = result.FirstOrDefault().LAYOUT;
            //        }
            //        return Json(layout);
            //    }
            //    else
            //    {
            //        var result = refService.GetPrintoutLayout("BRAND_REGISTRATION_NEWBRAND", CreatedBy);
            //        var layout = "No Layout Found.";
            //        if (result.Any())
            //        {
            //            layout = result.FirstOrDefault().LAYOUT;
            //        }
            //        return Json(layout);

            //    }
            //}
            //else
            //{
            //    var result = refService.GetPrintoutLayout("BRAND_REGISTRATION_UPDATEHJE", CreatedBy);
            //    var layout = "No Layout Found.";
            //    if (result.Any())
            //    {
            //        layout = result.FirstOrDefault().LAYOUT;
            //    }
            //    return Json(layout);

            //}

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdatePrintOutLayout(string NewPrintout, int RegistrationType, string CreatedBy, bool DocExport, string LayoutName)
        {
            var LayoutName_field = "";
            switch (LayoutName)
            {
                case "main":
                    if (RegistrationType == 1)
                    {
                        LayoutName_field = "BRAND_REGISTRATION_NEWBRAND_MAIN";
                    }
                    else
                    {
                        LayoutName_field = "BRAND_REGISTRATION_UPDATEHJE_MAIN";
                    }

                    break;

                case "surat_pernyataan":
                    LayoutName_field = "BRAND_REGISTRATION_SURAT_PERNYATAAN";
                    break;

                case "item":
                    if (RegistrationType == 1)
                    {
                        LayoutName_field = "BRAND_REGISTRATION_LISTBRAND";
                    }
                    else
                    {
                        LayoutName_field = "BRAND_REGISTRATION_LISTITEM";
                    }
                    break;


                case "export":
                    LayoutName_field = "BRAND_REGISTRATION_EXPORT";
                    break;
            }

            var ErrMessage = refService.UpdatePrintoutLayout(LayoutName_field, NewPrintout, CreatedBy);
            return Json(ErrMessage);

            //if (RegistrationType == 1)
            //{
            //    if (DocExport)
            //    {
            //        var ErrMessage = refService.UpdatePrintoutLayout("BRAND_REGISTRATION_EXPORT", NewPrintout, CreatedBy);
            //        return Json(ErrMessage);

            //    }
            //    else
            //    {
            //        var ErrMessage = refService.UpdatePrintoutLayout("BRAND_REGISTRATION_NEWBRAND", NewPrintout, CreatedBy);
            //        return Json(ErrMessage);
            //    }
            //}
            //else
            //{
            //    var ErrMessage = refService.UpdatePrintoutLayout("BRAND_REGISTRATION_UPDATEHJE", NewPrintout, CreatedBy);
            //    return Json(ErrMessage);
            //}

        }

        [HttpPost]
        public void DownloadPrintOut(BrandRegistrationReqViewModel _BRVModel)
        {
            try
            {
                brandRegistrationService.LogsPrintActivity(_BRVModel.ViewModel.Registration_ID, (int)Enums.MenuList.BrandRegistrationReq, CurrentUser.USER_ID);


                long BrandRegID = _BRVModel.ViewModel.Registration_ID;
                string FormNumber = _BRVModel.ViewModel.Registration_No;
                FormNumber = FormNumber.Replace('/', '-');
                var now = DateTime.Now.ToString("ddMMyyyy");
                _BRVModel.ViewModel.Registration_ID = BrandRegID;
                var htmlText = GetPrintout(_BRVModel, BrandRegID);
                
                //MemoryStream ms = new MemoryStream();
                var baseFolder = "/files_upload/BrandRegistration/PrintOut/";
                var uploadToFolder = Server.MapPath("~" + baseFolder);
                Directory.CreateDirectory(uploadToFolder);
                uploadToFolder += "PrintOut_BrandRegistration_" + FormNumber + "_" + now + ".pdf";

                FileStream stream = new FileStream(uploadToFolder, FileMode.Create);
                var leftMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var rightMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var topMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var bottomtMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);

                var leftMarginLandScape = iTextSharp.text.Utilities.MillimetersToPoints(10f);
                var rightMarginLandScape = iTextSharp.text.Utilities.MillimetersToPoints(10f);
                var topMarginLandScape = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var bottomtMarginLandScape = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);

                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, leftMargin, rightMargin, topMargin, bottomtMargin);


                var writer = PdfWriter.GetInstance(document, stream);
                if ((_BRVModel.ViewModel.LastApproved_Status == refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID) || (_BRVModel.ViewModel.LastApproved_Status == refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID))
                {
                    PdfWriterEvents writerEvent = new PdfWriterEvents("D R A F T E D");
                    writer.PageEvent = writerEvent;
                }

                writer.CloseStream = false;
                document.Open();
                var srHtml = new StringReader(htmlText);
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, srHtml);
                document.Close();
                stream.Close();

                //Print Brand - Surat Pernyataan
                var htmlTextSuratPernyataan = GetPrintoutSuratPernyataan(_BRVModel, BrandRegID);

                //MemoryStream ms = new MemoryStream();
                var uploadToFolderSuratPernyataan = Server.MapPath("~" + baseFolder);
                Directory.CreateDirectory(uploadToFolderSuratPernyataan);
                uploadToFolderSuratPernyataan += "PrintOut_BrandRegistration_SuratPernyataan" + FormNumber + "_" + now + ".pdf";

                FileStream streamSuratPernyataan = new FileStream(uploadToFolderSuratPernyataan, FileMode.Create);
                var documentSuratPernyataan = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, leftMargin, rightMargin, topMargin, bottomtMargin);

                var writerSuratPernyataan = PdfWriter.GetInstance(documentSuratPernyataan, streamSuratPernyataan);
                PdfWriterEvents writerEventSuratPernyataan = new PdfWriterEvents("D R A F T E D");
                writerSuratPernyataan.PageEvent = writerEventSuratPernyataan;
                writerSuratPernyataan.CloseStream = false;

                documentSuratPernyataan.Open();
                var srHtmlSuratPernyataan = new StringReader(htmlTextSuratPernyataan);
                XMLWorkerHelper.GetInstance().ParseXHtml(writerSuratPernyataan, documentSuratPernyataan, srHtmlSuratPernyataan);
                documentSuratPernyataan.Close();
                streamSuratPernyataan.Close();

                var uploadToFolderExport = Server.MapPath("~" + baseFolder);

                if (_BRVModel.ViewModel.DocExport)
                {
                    //Print Brand - Surat Pernyataan
                    var htmlTextExport = GetPrintoutExport(_BRVModel, BrandRegID);

                    //MemoryStream ms = new MemoryStream();
                    Directory.CreateDirectory(uploadToFolderExport);
                    uploadToFolderExport += "PrintOut_BrandRegistration_Export" + FormNumber + "_" + now + ".pdf";

                    FileStream streamExport = new FileStream(uploadToFolderExport, FileMode.Create);
                    var documentExport = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, leftMargin, rightMargin, topMargin, bottomtMargin);

                    var writerExport = PdfWriter.GetInstance(documentExport, streamExport);
                    if ((_BRVModel.ViewModel.LastApproved_Status == refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID) || (_BRVModel.ViewModel.LastApproved_Status == refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID))
                    {
                        PdfWriterEvents writerEventExport = new PdfWriterEvents("D R A F T E D");
                        writerExport.PageEvent = writerEventExport;
                    }
                    writerExport.CloseStream = false;

                    documentExport.Open();
                    var srHtmlExport = new StringReader(htmlTextExport);
                    XMLWorkerHelper.GetInstance().ParseXHtml(writerExport, documentExport, srHtmlExport);
                    documentExport.Close();
                    streamExport.Close();

                }


                //Print Brand - Landscape
                var htmlTextItem = GetPrintoutBrand(_BRVModel, BrandRegID);

                //MemoryStream ms = new MemoryStream();
                var uploadToFolderItem = Server.MapPath("~" + baseFolder);
                Directory.CreateDirectory(uploadToFolderItem);
                uploadToFolderItem += "PrintOut_BrandRegistration_Item" + FormNumber + "_" + now + ".pdf";

                FileStream streamItem = new FileStream(uploadToFolderItem, FileMode.Create);
                var documentItem = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), leftMarginLandScape, rightMarginLandScape, topMarginLandScape, bottomtMarginLandScape);

                var writerItem = PdfWriter.GetInstance(documentItem, streamItem);
                if ((_BRVModel.ViewModel.LastApproved_Status == refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID) || (_BRVModel.ViewModel.LastApproved_Status == refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID))
                {
                    PdfWriterEvents writerEventItem = new PdfWriterEvents("D R A F T E D");
                    writerItem.PageEvent = writerEventItem;
                }
                writerItem.CloseStream = false;

                documentItem.Open();
                var srHtmlItem = new StringReader(htmlTextItem);
                XMLWorkerHelper.GetInstance().ParseXHtml(writerItem, documentItem, srHtmlItem);
                documentItem.Close();
                streamItem.Close();

                //Merge Document & Item
                List<String> sourcePaths = new List<string>();
                sourcePaths.Add(uploadToFolder);
                sourcePaths.Add(uploadToFolderSuratPernyataan);
                sourcePaths.Add(uploadToFolderItem);
                if (_BRVModel.ViewModel.DocExport)
                {
                    sourcePaths.Add(uploadToFolderExport);
                }

                PdfMerge.Execute(sourcePaths.ToArray(), uploadToFolder);

                var MergeDocs = MergePrintout(uploadToFolder, BrandRegID);

                //var newFile = new FileInfo(uploadToFolder);
                //var fileName = Path.GetFileName(uploadToFolder);
                var newFile = new FileInfo(MergeDocs);
                var fileName = Path.GetFileName(MergeDocs);

                string attachment = string.Format("attachment; filename={0}", fileName);
                Response.Clear();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/pdf";
                Response.WriteFile(newFile.FullName);
                Response.Flush();
                newFile.Delete();
                Response.End();
                //byte[] bytesInStream = ms.ToArray();
                //ms.Close();
                //Response.Clear();
                //Response.ContentType = "application/force-download";
                //Response.AddHeader("content-disposition", "attachment;filename=PrintOut_InterviewRequest_" + FormNumber + "_" + now + ".pdf");
                //Response.BinaryWrite(bytesInStream);
                //Response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String MergePrintout(string path, long RegId)
        {
            try
            {
                var filesupload = brandRegistrationService.GetFileUploadByRegID(RegId).ToList();

                List<String> sourcePaths = new List<string>();
                sourcePaths.Add(path);
                var ext = "";
                foreach (var file_attach in filesupload)
                {
                    ext = file_attach.PATH_URL.Substring((file_attach.PATH_URL.Length - 3), 3);
                    if (ext == "pdf")
                    {
                        sourcePaths.Add(Server.MapPath("~" + file_attach.PATH_URL));
                    }
                }

                if (PdfMerge.Execute(sourcePaths.ToArray(), path))
                    return path;
                else
                    return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Confirmation

        private List<ConfirmDialogModel> GenerateConfirmDialog(bool Submit, bool Cancel, bool Approve)
        {
            try
            {
                var listconfirmation = new List<ConfirmDialogModel>();

                if (Submit)
                {
                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnSubmit",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Submit"
                        },
                        CssClass = " submit-modal brandregistrationrequest",
                        Message = "You are going to Submit Brand Registration. Are you sure?",
                        Title = "Submit Confirmation",
                        ModalLabel = "SubmitModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnSubmitSkep",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Submit"
                        },
                        CssClass = " submitskep-modal brandregistrationrequest",
                        Message = "You are going to Submit Brand Registration. Are you sure?",
                        Title = "Submit Confirmation",
                        ModalLabel = "SubmitModalLabel"
                    });
                }
                if (Cancel)
                {
                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnCancel",
                            CssClass = "btn btn-danger btn_loader",
                            Label = "Cancel Document"
                        },
                        CssClass = " cancel-modal brandregistrationrequest",
                        Message = "You are going to Cancel Brand Registration. Are you sure?",
                        Title = "Cancel Document Confirmation",
                        ModalLabel = "CancelModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnWithdraw",
                            CssClass = "btn btn-danger btn_loader",
                            Label = "Withdraw"
                        },
                        CssClass = " withdraw-modal brandregistrationrequest",
                        Message = "You are going to Withdraw Brand Registration. Are you sure?",
                        Title = "Withdraw Document Confirmation",
                        ModalLabel = "WithdrawModalLabel"
                    });
                }
                if (Approve)
                {
                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnApprove",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Approve"
                        },
                        CssClass = " approve-modal brandregistrationrequest",
                        Message = "You are going to approve Brand Registration. Are you sure?",
                        Title = "Approve Confirmation",
                        ModalLabel = "ApproveModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnApproveFinal",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Approve"
                        },
                        CssClass = " approvefinal-modal brandregistrationrequest",
                        Message = "You are going to approve Brand Registration. Are you sure?",
                        Title = "Approve Confirmation",
                        ModalLabel = "ApproveModalLabel"
                    });
                }

                //// FOR SET PRINTOUT TO DEFAULT CONFIRMATION ////                
                listconfirmation.Add(new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "btnRestorePrintoutToDefault",
                        CssClass = "btn btn-success btn_loader",
                        Label = "Restore To Default"
                    },
                    CssClass = " restoredefault-modal brandregistration",
                    Message = "You are going to restore printout layout to default. Are you sure?",
                    Title = "Restore Printout Confirmation",
                    ModalLabel = "RestoreModalLabel"
                });
                //////////////////////////////////////////////////

                return listconfirmation;
            }
            catch (Exception e)
            {
                return new List<ConfirmDialogModel>();
            }
        }

        #endregion
    }
}