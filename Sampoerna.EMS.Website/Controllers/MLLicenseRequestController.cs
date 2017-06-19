using Microsoft.Ajax.Utilities;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.CustomService.Services.ManufactureLicense;
using Sampoerna.EMS.Website.Helpers;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ManufacturingLicense;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Utility;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Core;
using SpreadsheetLight;




namespace Sampoerna.EMS.Website.Controllers
{
    public class MLLicenseRequestController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        private LicenseRequestService service;
        private InterviewRequestService interService;
        private InterviewRequestDetailModel interDetailService;
        private ProductTypeService prodtypeService;
        private IChangesHistoryBLL chBLL;
        private IWorkflowHistoryBLL whBLL;
        public MLLicenseRequestController(IPageBLL pageBLL, IChangesHistoryBLL changeHistoryBLL, IWorkflowHistoryBLL workflowHistoryBLL) : base(pageBLL, Enums.MenuList.ManufactureLicense)
        { 
            this.mainMenu = Enums.MenuList.ManufactureLicense;
            this.service = new LicenseRequestService();
            this.refService = new SystemReferenceService();
            this.interService = new InterviewRequestService();
            this.interDetailService = new InterviewRequestDetailModel();
            this.prodtypeService = new ProductTypeService();
            this.chBLL = changeHistoryBLL;
            this.whBLL = workflowHistoryBLL;
            
        }
        


        public ActionResult Index()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                //AddMessageInfo(CurrentUser.UserRole.ToString(), Enums.MessageInfoType.Warning);

                var users = refService.GetAllUser();
                var poaList = refService.GetAllPOA();

                /* format date dd MMMM yyyy HH:mm:ss */

                var documents = service.GetAll().Where(w => w.SYS_REFFERENCES.REFF_KEYS != "COMPLETED" && w.SYS_REFFERENCES.REFF_KEYS != "CANCELED").Select(s => new LicenseRequestModel
                {
                    MnfFormNum = s.MNF_FORM_NUMBER,
                    RequestDate = s.REQUEST_DATE,
                    CompType = s.INTERVIEW_REQUEST.COMPANY_TYPE,
                    KPPBC = s.INTERVIEW_REQUEST.KPPBC == null ? "" : s.INTERVIEW_REQUEST.KPPBC,
                    Company = s.INTERVIEW_REQUEST.T001.BUTXT,
                    //List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_TYPE).ToList(),
                    List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS).ToList(),
                    Count_List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_TYPE).Count(),
                    Status = s.SYS_REFFERENCES.REFF_KEYS,
                    Status_Value = s.SYS_REFFERENCES.REFF_VALUE,
                    LastApprovedStatus = s.LASTAPPROVED_STATUS,
                    MnfRequestID = s.MNF_REQUEST_ID,
                    CreatedDate = s.CREATED_DATE,
                    CreatedBy = s.CREATED_BY,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    InterviewId = s.VR_FORM_ID,
                    IsCanApprove = CurrentUser.USER_ID
                }).OrderByDescending(o => o.CreatedDate).ToList();
                
                if (CurrentUser.UserRole == Enums.UserRole.POA)
                {
                    var delegation = service.GetPOADelegatedUser(CurrentUser.USER_ID);
                    List<string> delegatorname = new List<string>();
                    if (delegation.Any())
                    {
                        delegatorname = delegation.Select(s => s.POA_FROM).ToList();
                    }
                    //var IRWhithSameNPPBKC = service.GetLicenseNeedApproveWithSameNPPBKC(CurrentUser.USER_ID);
                    var IRWhithoutNPPBKC = service.GetLicenseNeedApproveWithoutNPPBKC(CurrentUser.USER_ID);
                    var IRWithNPPBKCButNoExcise = service.GetInterviewNeedApproveWithNPPBKCButNoExcise(CurrentUser.USER_ID);
                    // new
                    var LastApprover = service.GetLicenseLastApproveAfterSubmit(CurrentUser.USER_ID); 
                    documents = documents.Where(w => w.CreatedBy.Equals(CurrentUser.USER_ID) 
                        || delegatorname.Contains(w.CreatedBy)
                        || LastApprover.Contains(w.MnfRequestID)
                        //|| w.LastApprovedBy == CurrentUser.USER_ID 
                        //|| IRWhithSameNPPBKC.Contains(w.MnfRequestID) 
                        || IRWhithoutNPPBKC.Contains(w.MnfRequestID)
                        || IRWithNPPBKCButNoExcise.Contains(w.MnfRequestID)).ToList();
                }
                
                var iridlist = service.GetIRIDAll();
                var statusidlist = service.GetStatusIDAll();
                //var nppbkcList = service.GetAllNPPBKCId();
                var nppbkclist = service.GetAllNPPBKCForFilter();
                var comptplist = interService.GetInterviewReqCompanyTypeList();
                var model = new LicenseRequestViewModel()
                {
                    MainMenu = mainMenu,
                    CurrentMenu = PageInfo,
                    Filter = new LicenseRequestFilterModel(),
                    LastApprovedStatusList = GetLastApprovedStatus(service.GetReffValueAll().Where(w => statusidlist.Contains(w.REFF_ID) && w.REFF_KEYS != "COMPLETED" && w.REFF_KEYS != "CANCELED")),
                    //FormNumList = GetFormNumListFilter(service.GetAll().Where(w => iridlist.Contains(w.VR_FORM_ID))),
                    //FormNumList = GetFormNumListFilter(service.GetAll().Where(w => w.CREATED_BY.Equals(CurrentUser.USER_ID))),
                    //FormNumList = GetFormNumListFilter(service.GetAll().Where(w => w.SYS_REFFERENCES.REFF_KEYS != "COMPLETED" && w.SYS_REFFERENCES.REFF_KEYS != "CANCELED")),
                    FormNumList = GetListFormNumFilter(documents),
                    CompTypeList = GetCompTypeList(comptplist),
                    ProdTypeList = GetProdTypeList(service.GetAllProductTypeFromLR()),
                    //KPPBCList = GetKPPBCList(nppbkcList),
                    KPPBCList = GetKPPBCListForFilter(nppbkclist),
                    LicenseRequestDocuments = documents,
                    IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Controller && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator)
                    
                };

                foreach (var doc in documents)
                {
                    doc.StrRequestDate = doc.RequestDate.ToString("dd MMMM yyyy"); //dd MMMM yyyy HH:mm:ss
                    doc.IsApprover = IsPOACanApprove(doc.MnfRequestID, CurrentUser.USER_ID);
                    doc.IsAdministrator = (CurrentUser.UserRole == Enums.UserRole.Administrator);
                    doc.IsViewer = (CurrentUser.UserRole == Enums.UserRole.Viewer);
                }

                model.Filter.LastApprovedStatus = 0;
                
                return View("Index", model);
            }
            else
            {
                AddMessageInfo("You dont have access to Manufacturing License Request page.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Unauthorized","Error");
            }
        }


        public ActionResult CompletedDocument()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                var users = refService.GetAllUser();
                var poaList = refService.GetAllPOA();

                var documents = service.GetAll().Where(w => w.SYS_REFFERENCES.REFF_KEYS == "COMPLETED" || w.SYS_REFFERENCES.REFF_KEYS == "CANCELED").Select(s => new LicenseRequestModel
                {
                    MnfFormNum = s.MNF_FORM_NUMBER,
                    RequestDate = s.REQUEST_DATE,
                    CompType = s.INTERVIEW_REQUEST.COMPANY_TYPE,
                    KPPBC = s.INTERVIEW_REQUEST.KPPBC == null ? "" : s.INTERVIEW_REQUEST.KPPBC,
                    KPBCName = s.NPPBKC_ID == null ? "" : s.ZAIDM_EX_NPPBKC.TEXT_TO,
                    Company = s.INTERVIEW_REQUEST.T001.BUTXT,
                    //List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_TYPE).ToList(),
                    List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS).ToList(),
                    Count_List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_TYPE).Count(),
                    Status = s.SYS_REFFERENCES.REFF_KEYS,
                    Status_Value = s.SYS_REFFERENCES.REFF_VALUE,
                    LastApprovedStatus = s.LASTAPPROVED_STATUS,
                    MnfRequestID = s.MNF_REQUEST_ID,
                    CreatedBy = s.CREATED_BY,
                    CreatedDate = s.CREATED_DATE,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    InterviewId = s.VR_FORM_ID,
                    IsCanApprove = CurrentUser.USER_ID
                }).OrderByDescending(o => o.CreatedDate).ToList();
                
                if (CurrentUser.UserRole == Enums.UserRole.POA)
                {
                    var delegation = service.GetPOADelegatedUser(CurrentUser.USER_ID);
                    List<string> delegatorname = new List<string>();
                    if (delegation.Any())
                    {
                        delegatorname = delegation.Select(s => s.POA_FROM).ToList();
                    }
                    //var IRWhithSameNPPBKC = service.GetLicenseNeedApproveWithSameNPPBKC(CurrentUser.USER_ID);
                    var IRWhithoutNPPBKC = service.GetLicenseNeedApproveWithoutNPPBKC(CurrentUser.USER_ID);
                    var IRWithNPPBKCButNoExcise = service.GetInterviewNeedApproveWithNPPBKCButNoExcise(CurrentUser.USER_ID);
                    // new
                    var LastApprover = service.GetLicenseLastApproveAfterSubmit(CurrentUser.USER_ID);
                    documents = documents.Where(w => w.CreatedBy.Equals(CurrentUser.USER_ID) 
                        || delegatorname.Contains(w.CreatedBy)
                        || LastApprover.Contains(w.MnfRequestID)
                        //|| w.LastApprovedBy == CurrentUser.USER_ID
                        //|| IRWhithSameNPPBKC.Contains(w.MnfRequestID) 
                        || IRWhithoutNPPBKC.Contains(w.MnfRequestID)
                        || IRWithNPPBKCButNoExcise.Contains(w.MnfRequestID)).ToList();
                }

                var iridlist = service.GetIRIDAll();
                var statusidlist = service.GetStatusIDAll();
                //var nppbkcList = service.GetAllNPPBKCId();
                var nppbkclist = service.GetAllNPPBKCForFilter();
                var comptplist = interService.GetInterviewReqCompanyTypeList();
                var model = new LicenseRequestViewModel()
                {
                    MainMenu = mainMenu,
                    CurrentMenu = PageInfo,
                    Filter = new LicenseRequestFilterModel(),
                    LastApprovedStatusList = GetLastApprovedStatus(service.GetReffValueAll().Where(w => statusidlist.Contains(w.REFF_ID) && w.REFF_KEYS == "COMPLETED" || w.REFF_KEYS == "CANCELED")),
                    //FormNumList = GetFormNumList(interService.GetAll().Where(w => iridlist.Contains(w.VR_FORM_ID))),
                    //FormNumList = GetFormNumListFilter(service.GetAll().Where(w => w.SYS_REFFERENCES.REFF_KEYS == "COMPLETED" || w.SYS_REFFERENCES.REFF_KEYS == "CANCELED")),
                    //CompTypeList = GetCompTypeList(interService.GetInterviewReqCompanyTypeList()),
                    FormNumList = GetListFormNumFilter(documents),
                    CompTypeList = GetCompTypeList(comptplist),
                    ProdTypeList = GetProdTypeList(service.GetAllProductTypeFromLR()),
                    KPPBCList = GetKPPBCListForFilter(nppbkclist),
                    LicenseRequestDocuments = documents,
                    IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Controller && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator)
                    
                };

                foreach (var doc in documents)
                {
                    doc.StrRequestDate = doc.RequestDate.ToString("dd MMMM yyyy"); //dd MMMM yyyy HH:mm:ss
                    doc.IsApprover = IsPOACanApprove(doc.MnfRequestID, CurrentUser.USER_ID);
                    doc.IsAdministrator = (CurrentUser.UserRole == Enums.UserRole.Administrator);
                    doc.IsViewer = (CurrentUser.UserRole == Enums.UserRole.Viewer);
                }

                model.Filter.LastApprovedStatus = Convert.ToInt32(refService.GetRefByKey("COMPLETED").REFF_ID);

                return View("Index", model);
            }
            else
            {
                AddMessageInfo("You dont have access to Manufacturing License Request page.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Unauthorized","Error");
            }
        }


        [HttpPost]
        public PartialViewResult FilterOpenDocument(LicenseRequestViewModel model)
        {
            
            var documents = service.GetAll();

            if (model.Filter.LastApprovedStatus != 0)
            {
                //documents = documents.Where(w => w.LASTAPPROVED_STATUS == model.Filter.LastApprovedStatus);
                //documents = documents.Where(w => w.LASTAPPROVED_STATUS == refService.GetRefByKey("COMPLETED").REFF_ID || w.LASTAPPROVED_STATUS == refService.GetRefByKey("CANCELED").REFF_ID);
                documents = documents.Where(w => w.SYS_REFFERENCES.REFF_KEYS == "COMPLETED" || w.SYS_REFFERENCES.REFF_KEYS == "CANCELED");
            }
            else
            {
                //documents = documents.Where(w => w.LASTAPPROVED_STATUS != refService.GetRefByKey("COMPLETED").REFF_ID || w.LASTAPPROVED_STATUS != refService.GetRefByKey("CANCELED").REFF_ID);
                documents = documents.Where(w => w.SYS_REFFERENCES.REFF_KEYS != "COMPLETED" && w.SYS_REFFERENCES.REFF_KEYS != "CANCELED");
            }

            if (model.Filter.FormNum != null)
            {
                documents = documents.Where(w => w.VR_FORM_ID.ToString() == model.Filter.FormNum);
            }

            if (model.Filter.CompType != null)
            {
                documents = documents.Where(w => w.INTERVIEW_REQUEST.COMPANY_TYPE == model.Filter.CompType);
            }

            if (model.Filter.KPPBC != null)
            {
                documents = documents.Where(w => w.INTERVIEW_REQUEST.KPPBC == model.Filter.KPPBC);
            }

            
            if (model.Filter.ProdType != null)
            {
                var pt = service.GetLRIdProdType(model.Filter.ProdType);
                documents = documents.Where(w => pt.Contains(w.MNF_REQUEST_ID));
            }

            if (model.Filter.StatusFilter != 0)
            {
                documents = documents.Where(w => w.LASTAPPROVED_STATUS == model.Filter.StatusFilter);
            }

            var listofDoc = documents.Select(s => new LicenseRequestModel
            {
                MnfFormNum = s.MNF_FORM_NUMBER,
                RequestDate = s.REQUEST_DATE,
                CompType = s.INTERVIEW_REQUEST.COMPANY_TYPE,
                KPBCName = s.NPPBKC_ID == null ? "" : s.ZAIDM_EX_NPPBKC.TEXT_TO,
                KPPBC = s.INTERVIEW_REQUEST.KPPBC == null ? "" : s.INTERVIEW_REQUEST.KPPBC,
                Company = s.INTERVIEW_REQUEST.T001.BUTXT,
                //List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_TYPE).ToList(),
                List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS).ToList(),
                Count_List_ProdType = (from v in s.MANUFACTURING_PRODUCT_TYPE where (v.MNF_REQUEST_ID == s.MNF_REQUEST_ID && v.PROD_CODE != null) select v.ZAIDM_EX_PRODTYP.PRODUCT_TYPE).Count(),
                Status = s.SYS_REFFERENCES.REFF_KEYS == null ? "" : s.SYS_REFFERENCES.REFF_KEYS,
                Status_Value = s.SYS_REFFERENCES.REFF_VALUE == null ? "" : s.SYS_REFFERENCES.REFF_VALUE,
                LastApprovedStatus = s.LASTAPPROVED_STATUS,
                CreatedBy = s.CREATED_BY,
                CreatedDate = s.CREATED_DATE,
                LastApprovedBy = s.LASTAPPROVED_BY,
                InterviewId = s.VR_FORM_ID,
                MnfRequestID = s.MNF_REQUEST_ID,
                IsCanApprove = CurrentUser.USER_ID
            }).OrderByDescending(o => o.CreatedDate).ToList();

            if (CurrentUser.UserRole == Enums.UserRole.POA)
            {
                var delegation = service.GetPOADelegatedUser(CurrentUser.USER_ID);
                List<string> delegatorname = new List<string>();
                if (delegation.Any())
                {
                    delegatorname = delegation.Select(s => s.POA_FROM).ToList();
                }
                //var IRWhithSameNPPBKC = service.GetLicenseNeedApproveWithSameNPPBKC(CurrentUser.USER_ID);
                var IRWhithoutNPPBKC = service.GetLicenseNeedApproveWithoutNPPBKC(CurrentUser.USER_ID);
                var IRWithNPPBKCButNoExcise = service.GetInterviewNeedApproveWithNPPBKCButNoExcise(CurrentUser.USER_ID);
                // new
                var LastApprover = service.GetLicenseLastApproveAfterSubmit(CurrentUser.USER_ID);
                documents = documents.Where(w => w.CREATED_BY.Equals(CurrentUser.USER_ID) || delegatorname.Contains(w.CREATED_BY)
                    || LastApprover.Contains(w.MNF_REQUEST_ID)
                    //|| w.LASTAPPROVED_BY == CurrentUser.USER_ID
                    //|| IRWhithSameNPPBKC.Contains(w.MNF_REQUEST_ID) 
                    || IRWhithoutNPPBKC.Contains(w.MNF_REQUEST_ID)
                    || IRWithNPPBKCButNoExcise.Contains(w.MNF_REQUEST_ID));
            }

            foreach (var doc in listofDoc)
            {
                doc.StrRequestDate = doc.RequestDate.ToString("dd MMMM yyyy"); //dd MMMM yyyy HH:mm:ss
            }

            model.LicenseRequestDocuments = listofDoc;

            return PartialView("_LicenseRequestTable", model);
        }



        public ActionResult Create()
        {
            try
            {
                if (CurrentUser.UserRole != Enums.UserRole.POA)
                {
                    AddMessageInfo("Can't create Manufacturing License Request Document for User with " + EnumHelper.GetDescription(CurrentUser.UserRole) + " Role", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }

                var model = GenerateModelProperties(null);
                return View("Create", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return View("Index");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Edit(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(Id, CurrentUser.USER_ID))
            {
                if (IsCreatorWaitApproval(Id, CurrentUser.USER_ID))
                {
                    var model = new LicenseRequestModel();
                    model = GetLicenseRequestDetail(Id);
                    model = SetDetailProductType(model);
                    model.IsFormReadOnly = false;
                    model = SetIRDetail(model);
                    model = SetOtherProductType(model);
                    model.Confirmation = GenerateConfirmDialogForm(true,false,false);
                    return View("Edit", model);
                }
                else
                {
                    AddMessageInfo("You can access again after the document is approved.", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                AddMessageInfo("You dont have access to edit this License Request Form document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Detail(Int64 Id = 0, int st = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Controller /* || IsPOACanAccess(Id, CurrentUser.USER_ID)*/ || CurrentUser.UserRole == Enums.UserRole.Viewer /*|| IsPOAfromSameNPPBKC(Id)*/)
                //|| IsPOACanApprove(Id,CurrentUser.USER_ID))
            {
                var model = new LicenseRequestModel();
                model = GetLicenseRequestDetail(Id);
                model = SetDetailProductType(model);
                model.IsFormReadOnly = true;
                model = SetIRDetail(model);
                model = SetOtherProductType(model);

                model.IsApprover = IsPOACanApprove(Id, CurrentUser.USER_ID);
                model.Confirmation = GenerateConfirmDialogForm(false,false,true);
                switch (st)
                {
                    case 1: model.IsPage = "detail";
                        break;
                    case 2: model.IsPage = "approve";
                        break;
                }
                return View("Detail", model);
            }
            else
            {
                AddMessageInfo("You dont have access to view License Request detail Form.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }

        //public ActionResult ToApprove(Int64 id = 0, long st = 0)
        //{
        //    var status = "";
        //    if (IsPOACanApprove(id, CurrentUser.USER_ID))
        //    {
        //        switch(st)
        //        {
        //            case 26: status = "WAITING FOR GOVERNMENT APPROVAL";
        //                break;
        //            case 27: status = "COMPLETED";
        //                break; 
        //        }
        //        ChangeStatus(id, status, "", "approve");

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        AddMessageInfo("You dont have access to approve this Manufacturing License Request document.", Enums.MessageInfoType.Warning);
        //        return RedirectToAction("Index");
        //    }
        //}

        public ActionResult Approve(Int64 Id = 0, int st = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanApprove(Id, CurrentUser.USER_ID) || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                var model = new LicenseRequestModel();
                model = GetLicenseRequestDetail(Id);
                model = SetDetailProductType(model);
                model.IsFormReadOnly = true;
                model = SetIRDetail(model);
                model = SetOtherProductType(model);

                model.IsApprover = IsPOACanApprove(Id, CurrentUser.USER_ID);
                model.Confirmation = GenerateConfirmDialogForm(false,false,true);

                switch (st)
                {
                    case 1:
                        model.IsPage = "detail";
                        break;
                    case 2:
                        model.IsPage = "approve";
                        break;
                }

                return View("Detail", model);
            }
            else
            {
                AddMessageInfo("You dont have access to approve this Manufacturing License Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }

        public bool IsPOACanApprove(long IRId, string UserId)
        {
            var isOk = false;
            if (IRId != 0)
            {
                var approverlist = service.GetPOAApproverList(IRId);
                if (approverlist.Count() > 0)
                {
                    var isexist = approverlist.Where(w => w.POA_ID.Equals(UserId)).ToList();
                    if (isexist.Count() > 0)
                    {
                        isOk = true;
                    }
                }
            }
            return isOk;
        }

        public bool IsCreatorWaitApproval(long IRId, string UserId)
        {
            var isOk = true;
            long StatusId = 0;
            var CreatorId = "";
            var LRequest = service.GetLicenseRequestById(IRId).FirstOrDefault();
            if (LRequest != null)
            {
                StatusId    = LRequest.LASTAPPROVED_STATUS;
                CreatorId   = LRequest.CREATED_BY;
            }
            if (StatusId != 0)
            {
                if ((StatusId == refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID) || (StatusId == refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID))
                {
                    if (CreatorId == UserId)
                    {
                        isOk = false;
                    }
                }
            }
            return isOk;
        }

        public bool IsPOACanAccess(long IRId, string UserId)
        {
            var isOk = true;
            var CreatorId = "";
            var IRequest = service.GetLicenseRequestById(IRId).FirstOrDefault();
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
            var poadelegate = service.GetPOADelegationOfUser(CreatorId);
            if (poadelegate != null)
            {
                if (UserId == poadelegate.POA_TO)
                {
                    isOk = true;
                }
            }
            return isOk;
        }

        public string GetProductType(Int64 Id=0)
        {
            try
            {
                string strprodtype = "";

                foreach (var val in service.GetProductTypeById().Where(w=>w.MNF_REQUEST_ID.Equals(Id)&&w.PROD_CODE!=""))
                {
                    var det = prodtypeService.GetAll().Where(w => w.PROD_CODE.Equals(val.PROD_CODE)).FirstOrDefault();
                    strprodtype = strprodtype + " " + det.PRODUCT_ALIAS;
                }
                
                return strprodtype;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<ProductTypeDetails> GetProductTypeMaster(Int64 Id = 0)
        {
            try
            {
                var LPTModel = new List<ProductTypeDetails>();
                var data = service.GetProductTypeById().Where(w => w.MNF_REQUEST_ID.Equals(Id) && w.PROD_CODE != null);
                if (data.Any())
                {
                    LPTModel = data.Select(s => new ProductTypeDetails
                    {
                        ProdAlias = s.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS
                    }).ToList();
                }
                return LPTModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProductTypeDetails> GetOtherProductTypeMaster(Int64 Id = 0)
        {
            try
            {
                var LPTModel = new List<ProductTypeDetails>();
                var data = service.GetProductTypeById().Where(w => w.MNF_REQUEST_ID.Equals(Id) && w.PROD_CODE == null);
                if (data.Any())
                {
                    LPTModel = data.Select(s => new ProductTypeDetails
                    {
                        OtherProdCode = s.OTHERS_PROD_TYPE
                    }).ToList();
                }
                return LPTModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private LicenseRequestModel GetLicenseRequestMasterForm(LicenseRequestModel _LRModel)
        {
            _LRModel = service.GetLicenseRequestById(_LRModel.MnfRequestID).Select(s => new LicenseRequestModel
            {
                MnfRequestID = s.MNF_REQUEST_ID,
                InterviewId = s.VR_FORM_ID,
                MnfFormNum = s.MNF_FORM_NUMBER,
                Status = s.SYS_REFFERENCES.REFF_KEYS,
                StatusKey = s.SYS_REFFERENCES.REFF_KEYS ,
                RequestDate = s.REQUEST_DATE,
                NppbkcID = s.NPPBKC_ID ?? "",
                POA_ID = s.INTERVIEW_REQUEST.POA_ID,
                POAName = s.INTERVIEW_REQUEST.POA.PRINTED_NAME,
                POAAddress = s.INTERVIEW_REQUEST.POA.POA_ADDRESS,
                POAPosition = s.INTERVIEW_REQUEST.POA.TITLE,
                KPPBC_ID = s.INTERVIEW_REQUEST.KPPBC,
                KPBCName = s.INTERVIEW_REQUEST.ZAIDM_EX_NPPBKC.TEXT_TO ?? "",
                KPPBC_Address = s .INTERVIEW_REQUEST.KPPBC_ADDRESS ?? "",
                Npwp = s.INTERVIEW_REQUEST.T001.NPWP,
                Company_Address = s.INTERVIEW_REQUEST.T001.SPRAS,
                Company_City = s.INTERVIEW_REQUEST.T001.ORT01,
                Company = s.INTERVIEW_REQUEST.T001.BUTXT,
                BADate = s.INTERVIEW_REQUEST.BA_DATE,
                BANumber = s.INTERVIEW_REQUEST.BA_NUMBER,
                BAStatus = s.INTERVIEW_REQUEST.BA_STATUS,
                Perihal = s.INTERVIEW_REQUEST.PERIHAL,
                Company_Type = s.INTERVIEW_REQUEST.COMPANY_TYPE,
                City = s.INTERVIEW_REQUEST.CITY,
                CreatedBy = s.CREATED_BY
            }).FirstOrDefault();
            return _LRModel;
        }

        private LicenseRequestModel GetLicenseRequestDetail(long ID = 0, string Mode = "")
        {
            try
            {
                var model = service.GetLicenseRequestById(ID).Select(s => new LicenseRequestModel
                {
                    MnfRequestID = s.MNF_REQUEST_ID,
                    BANum = s.INTERVIEW_REQUEST.BA_NUMBER,
                    MnfFormNum = s.MNF_FORM_NUMBER,
                    InterviewFormNum = s.MNF_FORM_NUMBER,
                    InterviewId = s.INTERVIEW_REQUEST.VR_FORM_ID,
                    Status = s.SYS_REFFERENCES.REFF_KEYS,
                    StatusKey = s.SYS_REFFERENCES.REFF_KEYS,
                    RequestDate = s.REQUEST_DATE,
                    //KPPBC = s.NPPBKC_ID == null ? "" : s.ZAIDM_EX_NPPBKC.KPPBC_ID,
                    KPPBC = s.INTERVIEW_REQUEST.KPPBC,
                    KPBCName = s.ZAIDM_EX_NPPBKC.TEXT_TO == null ? "" : s.ZAIDM_EX_NPPBKC.TEXT_TO,
                    POAName = s.INTERVIEW_REQUEST.POA.PRINTED_NAME,
                    POAAddress = s.INTERVIEW_REQUEST.POA.POA_ADDRESS,
                    POAPosition = s.INTERVIEW_REQUEST.POA.TITLE,
                    //KppbcAddress = s.NPPBKC_ID == null ? "" : s.ZAIDM_EX_NPPBKC.ADDR1 + s.ZAIDM_EX_NPPBKC.ADDR2,
                    KppbcAddress = s.INTERVIEW_REQUEST.KPPBC_ADDRESS,
                    Npwp = s.INTERVIEW_REQUEST.T001.NPWP,
                    CompanyAddress = s.INTERVIEW_REQUEST.T001.SPRAS,
                    Company = s.INTERVIEW_REQUEST.T001.BUTXT,
                    CompanyId = s.INTERVIEW_REQUEST.BUKRS,
                    CompType = s.INTERVIEW_REQUEST.COMPANY_TYPE,
                    NppbkcID = s.NPPBKC_ID == null ? "" : s.NPPBKC_ID,
                    //NppbkcID = s.INTERVIEW_REQUEST.NPPBKC_ID == null ? "" : s.INTERVIEW_REQUEST.NPPBKC_ID,
                    DecreeDate = s.DECREE_DATE,
                    DecreeNo = s.DECREE_NO,
                    DecreeStatus = s.DECREE_STATUS,
                    CreatedBy = s.CREATED_BY
                }).FirstOrDefault();
                model.MainMenu = mainMenu;
                model.CurrentMenu = PageInfo;
                //model.FormNumList = GetFormNumList(interService.GetAll());
                model.FormNumList = GetFormNumListFilter(service.GetAll());
                model.GovStatus_List = GetGovStatusList(service.GetGovStatusList());
                var filesupload = service.GetFileUploadByLRId(ID);

                var LampiranCount = filesupload.Where(w=>w.IS_GOVERNMENT_DOC == false).Count();
                model.Lampiran_Count = LampiranCount.ToString() == null?0:LampiranCount;

                if (filesupload != null)
                {
                    var Othersfileupload = filesupload.Where(w => w.DOCUMENT_ID == null && w.IS_GOVERNMENT_DOC == false);
                    model.LicenseRequestFileOtherList = Othersfileupload.Select(s => new LicenseRequestFileOtherModel
                    {
                        FileId = s.FILE_ID,
                        Path = s.PATH_URL,
                        FileName = s.FILE_NAME,
                        Name = ""
                    }).ToList();
                    foreach(var fileother in model.LicenseRequestFileOtherList)
                    {
                        fileother.Name = GenerateFileName(fileother.Path);
                        fileother.Path = GenerateURL(fileother.Path);                        
                    }
                    var BAsfileupload = filesupload.Where(w => w.IS_GOVERNMENT_DOC == true);
                    model.LicenseRequestFileBAList = BAsfileupload.Select(s => new LicenseRequestFileOtherModel
                    {
                        FileId = s.FILE_ID,
                        Path = s.PATH_URL,
                        FileName = s.FILE_NAME,
                        Name = ""
                    }).ToList();
                    foreach (var fileother in model.LicenseRequestFileBAList)
                    {
                        fileother.Name = GenerateFileName(fileother.Path);
                        fileother.Path = GenerateURL(fileother.Path);
                    }
                }
                model.File_Size = GetMaxFileSize();
                string link_BA = "";
                if ( model.BANum != null)
                {
                    link_BA = GenerateURL("/MLInterviewRequest/Detail/"+model.InterviewId);
                }
                model.link_BA = link_BA;

                //var history = refService.GetChangesHistory((int)Enums.MenuList.LicenseRequest, ID.ToString()).ToList();
                //model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.LicenseRequest, ID).ToList();
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
                if (model.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval)) || model.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval)))
                {
                    var ApproverList = service.GetPOAApproverList(model.MnfRequestID).Select(s => s.POA_ID).ToList();
                    var approvername = "";
                    foreach (var approver in ApproverList)
                    {
                        if (approvername != "")
                        {
                            approvername += ", ";
                        }
                        approvername += approver;
                    }
                    var Action = "";
                    if (model.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval)))
                    {
                        Action = "Waiting For POA Approval";
                    }
                    else
                    {
                        Action = "Waiting For SKEP Approval";
                    }
                    model.WorkflowHistory.Add(new WorkflowHistoryViewModel
                    {
                        ACTION = Action,
                        USERNAME = approvername
                    });
                }
                return model;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new LicenseRequestModel();
            }
        }

        public List<InterviewRequestDetails> GetLicenseRequestBoundCondition(long LRId = 0)
        {
            try
            {
                var LRBCModel = new List<InterviewRequestDetails>();
                var data = service.GetBoundConditionById(LRId);
                if (data.Any())
                {
                    LRBCModel = data.Select(s => new InterviewRequestDetails
                    {
                        MnfDetailId = s.MNF_COND_ID,
                        MnfReqId = s.MNF_REQUEST_ID,
                        MnfAddr = s.INTERVIEW_REQUEST_DETAIL.MANUFACTURE_ADDRESS,
                        MnfCityId = s.INTERVIEW_REQUEST_DETAIL.CITY_ID,
                        MnfCity = s.INTERVIEW_REQUEST_DETAIL.MASTER_CITY.CITY_NAME,
                        MnfProvinceId = s.INTERVIEW_REQUEST_DETAIL.PROVINCE_ID,
                        MnfProvince = s.INTERVIEW_REQUEST_DETAIL.PROVINCE_ID.ToString() == null ? "" : s.INTERVIEW_REQUEST_DETAIL.MASTER_STATE.STATE_NAME,
                        MnfSubDist = s.INTERVIEW_REQUEST_DETAIL.SUB_DISTRICT,
                        MnfVillage = s.INTERVIEW_REQUEST_DETAIL.VILLAGE,
                        MnfPhone = s.INTERVIEW_REQUEST_DETAIL.PHONE,
                        MnfFax = s.INTERVIEW_REQUEST_DETAIL.FAX,
                        North = s.NORTH,
                        South = s.SOUTH,
                        East = s.EAST,
                        West = s.WEST,
                        LandArea = s.LAND_AREA ?? 0,
                        BuildingArea = s.BUILDING_AREA ?? 0,
                        OwnershipStatus = s.OWNERSHIP_STATUS,
                        InterviewDetailId = s.VR_FORM_DETAIL_ID
                    }).ToList();
                }
                return LRBCModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<InterviewRequestDetails>();
            }
        }

        public List<InterviewRequestDetails> GetListBoundCondition(long LRId = 0)
        {
            try
            {
                var LRBCModel = new List<InterviewRequestDetails>();
                var data = service.GetBoundConditionById(LRId);
                if (data.Any())
                {
                    LRBCModel = data.Select(s => new InterviewRequestDetails
                    {
                        MnfDetailId = s.MNF_COND_ID,
                        InterviewDetailId = s.VR_FORM_DETAIL_ID,
                        MnfReqId = s.MNF_REQUEST_ID,
                        North = s.NORTH,
                        South = s.SOUTH,
                        East = s.EAST,
                        West = s.WEST,
                        LandArea = s.LAND_AREA ?? 0,
                        BuildingArea = s.BUILDING_AREA ?? 0,
                        OwnershipStatus = s.OWNERSHIP_STATUS
                        
                    }).ToList();
                }
                return LRBCModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<InterviewRequestDetails>();
            }
        }

        [HttpPost]
        public ActionResult Save(LicenseRequestModel model)
        {
            try
            {
                var maxFileSize = GetMaxFileSize();
                var isOkFileExt = true;
                var isOkFileSize = true;
                var supportingDocFile = new List<HttpPostedFileBase>();
                if (model.LicenseRequestSupportingDoc != null)
                {
                    supportingDocFile = model.LicenseRequestSupportingDoc.Select(s => s.File).ToList();
                }
                isOkFileExt = CheckFileExtension(supportingDocFile);
                if (isOkFileExt)
                {
                    isOkFileExt = CheckFileExtension(model.File_Other);
                    if (isOkFileExt)
                    {
                        isOkFileExt = CheckFileExtension(model.File_BA);
                    }
                }

                if (isOkFileExt)
                {
                    //isOkFileSize = CheckFileSize(supportingDocFile, maxFileSize);
                    if (isOkFileSize)
                    {
                        isOkFileSize = CheckFileSize(model.File_Other, maxFileSize);
                        if (isOkFileSize)
                        {
                            isOkFileSize = CheckFileSize(model.File_BA, maxFileSize);
                        }
                    }

                    if (isOkFileSize)
                    {
                        if (model.LicenseRequestSupportingDoc != null)
                        {
                            foreach (var SuppDoc in model.LicenseRequestSupportingDoc)
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

                        if (model.File_BA != null)
                        {
                            foreach (var FileBA in model.File_BA)
                            {
                                var PathFile = UploadFile(FileBA);
                                if (PathFile != "")
                                {
                                    model.File_BA_Path.Add(PathFile);
                                }
                            }
                        }

                        if (model.Status.ToUpper() == "DRAFT_NEW_STATUS" && model.MnfRequestID != 0 && model.MnfRequestID.ToString() != null )
                        {
                            model.Status = "DRAFT_EDIT_STATUS";
                        }

                        var userid = CurrentUser.USER_ID;
                        var userrole = CurrentUser.UserRole;
                        var requestid = model.MnfRequestID;
                        long statusid = 0;

                        var ActionType = 0;
                        if (model.Status.ToUpper().Contains("DRAFT"))
                        {
                            ActionType = (int)Enums.ActionType.Modified;
                        }
                        else if (model.Status.ToUpper().Equals("WAITING_POA_APPROVAL"))
                        {
                            ActionType = (int)Enums.ActionType.Submit;
                        }

                        if ((requestid != 0) && (requestid.ToString() != null))
                        {
                            if (model.Status.ToUpper().Contains("DRAFT"))
                            {
                                statusid = refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID;
                            }
                            else if (model.Status.ToUpper().Equals("WAITING_POA_APPROVAL"))
                            {
                                statusid = refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID;
                            }                            
                            var updatelicenseReq = service.UpdateLicenseRequest(model.RequestDate, model.MnfRequestID, statusid, userid, (int)userrole, ActionType);

                            var reqId = updatelicenseReq.MNF_REQUEST_ID ;
                            if (updatelicenseReq != null)
                            {
                                if (model.IRDetails != null)
                                {

                                    //var TheBound = GetListBoundCondition(model.MnfRequestID);
                                    //var RemarkToErased = MapToBoundCondition(TheBound);
                                    //var TheRemarkToErased = RemarkToErased.Where(w => model.RemovedDetailId.Contains(w.MNF_COND_ID)).ToList();
                                    //service.RemoveBoundConditionSelected(model.MnfRequestID, CurrentUser.USER_ID, TheRemarkToErased);
                                    service.RemoveAllBoundCondition(reqId);
                                    //foreach (var uplreq in model.IRDetails)
                                    //{
                                    //    if (uplreq.North != null && uplreq.North != "")
                                    //    {
                                    //        var ReNewedBoundCondition = RemarkToErased.Where(w => w.MNF_COND_ID == uplreq.MnfDetailId).FirstOrDefault();
                                    //        service.InsertBoundCondition(model.MnfRequestID, uplreq.North, uplreq.East, uplreq.South, uplreq.West, uplreq.LandArea, uplreq.BuildingArea, uplreq.OwnershipStatus, uplreq.MnfDetailId, ReNewedBoundCondition, CurrentUser.USER_ID);
                                    //    }
                                    //}
                                    foreach (var uplreq in model.IRDetails)
                                    {
                                        service.InsertBoundCondition(reqId, uplreq.North, uplreq.East, uplreq.South, uplreq.West, uplreq.LandArea, uplreq.BuildingArea, uplreq.OwnershipStatus, uplreq.MnfDetailId, CurrentUser.USER_ID);
                                    }
                                }

                                if (model.ProdCode != null)
                                {
                                    service.InsertAllProductType(reqId, model.ProdCode,CurrentUser.USER_ID);
                                }

                                if (model.OtherProdCode != null)
                                {
                                    service.InsertAllOtherProductType(reqId, model.OtherProdCode, CurrentUser.USER_ID);
                                }

                                if (reqId != 0)
                                {
                                    //// InActiving/Removing Doc
                                    foreach (var removedfile in model.RemovedFilesId)
                                    {
                                        if (removedfile != 0)
                                        {
                                            service.DeleteFileUpload(removedfile, CurrentUser.USER_ID);
                                        }
                                    }
                                    //// Supporting Doc
                                    InsertUploadSuppDocFile(model.LicenseRequestSupportingDoc, reqId);
                                    //// Other Doc
                                    InsertUploadCommonFile(model.File_Other_Path, reqId, false, model.File_Other_Name);
                                    //// BA Doc
                                    //InsertUploadCommonFile(model.File_BA_Path, reqId, true, model.File_Other_Name);
                                }
                                

                            }
                        }
                        else
                        {

                            statusid = refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID;

                            var licenseReq = service.InsertLicenseRequest(model.RequestDate, model.InterviewId, model.InterviewFormNum, statusid, userid, (int)userrole);
                            if (licenseReq != null)
                            {
                                //var licenseReqID = service.GetAll().Where(w =>w.MNF_FORM_NUMBER.Equals(licenseReq.MNF_FORM_NUMBER)).Select(s => s.MNF_REQUEST_ID).FirstOrDefault();
                                var licenseReqID = licenseReq.MNF_REQUEST_ID;
                                if (model.IRDetails != null)
                                {
                                    //var thebound = GetLicenseRequestBoundCondition(licenseReqID);
                                    //var RemarkToErased = MapToBoundCondition(thebound);
                                    //var TheRemarkToErased = RemarkToErased.Where(w => model.RemovedDetailId.Contains(w.MNF_COND_ID)).ToList();
                                    //service.RemoveBoundConditionSelected(licenseReqID, CurrentUser.USER_ID, TheRemarkToErased);
                                    service.RemoveAllBoundCondition(licenseReqID);
                                    //foreach (var uplreq in model.IRDetails)
                                    //{
                                    //    if (uplreq.North != null && uplreq.North != "")
                                    //    {
                                    //        var ReNewedBoundCondition = RemarkToErased.Where(w => w.MNF_COND_ID == uplreq.MnfDetailId).FirstOrDefault();
                                    //        service.InsertBoundCondition(licenseReqID, uplreq.North, uplreq.East, uplreq.South, uplreq.West, uplreq.LandArea, uplreq.BuildingArea, uplreq.OwnershipStatus, uplreq.MnfDetailId, ReNewedBoundCondition, CurrentUser.USER_ID);
                                    //    }
                                    //}
                                    foreach (var lreq in model.IRDetails)
                                    {
                                        service.InsertBoundCondition(licenseReqID, lreq.North, lreq.East, lreq.South, lreq.West, lreq.LandArea, lreq.BuildingArea, lreq.OwnershipStatus, lreq.MnfDetailId, CurrentUser.USER_ID);
                                    }
                                }

                                if (model.ProdCode != null)
                                {
                                    foreach (var car in model.ProdCode)
                                    {
                                        service.InsertProductType(car, "", true, licenseReqID,CurrentUser.USER_ID);
                                    }
                                }

                                if (model.OtherProdCode != null)
                                {
                                    foreach (var otherpc in model.OtherProdCode)
                                    {
                                        service.InsertProductType("", otherpc, true, licenseReqID, CurrentUser.USER_ID);
                                    }
                                }

                                if (model.RemovedFilesId != null)
                                {
                                    //// InActiving/Removing Doc
                                    foreach (var removedfile in model.RemovedFilesId)
                                    {
                                        if (removedfile != 0)
                                        {
                                            service.DeleteFileUpload(removedfile, CurrentUser.USER_ID);
                                        }
                                    }
                                    //// Supporting Doc
                                    InsertUploadSuppDocFile(model.LicenseRequestSupportingDoc, licenseReqID);
                                    //// Other Doc
                                    InsertUploadCommonFile(model.File_Other_Path, licenseReqID, false, model.File_Other_Name);
                                    //// BA Doc
                                    InsertUploadCommonFile(model.File_BA_Path, licenseReqID, true, model.File_Other_Name);

                                }
                            }
                        }

                        var license_item = service.GetLicenseRequestById(model.MnfRequestID).FirstOrDefault();
                        
                        var msgSuccess = "";
                            if (model.Status.ToUpper() == "DRAFT_NEW_STATUS")
                            {
                                msgSuccess = "Success create License Request";
                            }
                            else if (model.Status.ToUpper() == "DRAFT_EDIT_STATUS")
                            {
                                msgSuccess = "Success update License Request";
                            }
                            else if (model.Status.ToUpper() == "WAITING_POA_APPROVAL")
                            {
                                msgSuccess = "Success submit License Request";
                                var EmailStatus = "has already submitted";
                                var poareceiverlistall = service.GetPOAApproverList(model.MnfRequestID);
                                if (poareceiverlistall.Count() > 0)
                                {
                                    List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();
                                    var strreqdate = model.RequestDate.ToString("dd MMMM yyyy");
                                    var interview_data = interService.GetInterviewRequestById(model.InterviewId).FirstOrDefault();
                                    var CreatorName = refService.GetPOA(CurrentUser.USER_ID).PRINTED_NAME;

                                    var model_mail = new LicenseRequestMailModel();
                                    model_mail.Mail_MnfFormNum = license_item.MNF_FORM_NUMBER;
                                    model_mail.Mail_Perihal = license_item.INTERVIEW_REQUEST.PERIHAL;
                                    model_mail.Mail_RequestDate = license_item.REQUEST_DATE.ToString("dd MMMM yyyy");
                                    model_mail.Mail_KPPBC = license_item.INTERVIEW_REQUEST.KPPBC;
                                    model_mail.Mail_Company = license_item.INTERVIEW_REQUEST.T001.BUTXT;
                                    model_mail.Mail_Creator = CreatorName;
                                    model_mail.Mail_Status = EmailStatus;
                                    model_mail.Mail_LastStatus = model.Status;
                                    model_mail.Mail_MnfRequestId = model.MnfRequestID;
                                    model_mail.Mail_POA_ReceiverList = poareceiverList;
                                    model_mail.Mail_MailFor = "submit";
                                    model_mail.Mail_Approver = "-";
                                    model_mail.Mail_ApprovedDate = "-";
                                    model_mail.Mail_Remark = "-";
                                    model_mail.Mail_Comment = "-";
                                    model_mail.Mail_DecreeDate = "-";
                                    model_mail.Mail_DecreeNo = "-";
                                    model_mail.Mail_BANo = license_item.INTERVIEW_REQUEST.BA_NUMBER;
                                    model_mail.Mail_Form = "";

                                    var License_detail_item = service.GetBoundCondAll().Where(w => w.MNF_REQUEST_ID.Equals(model.MnfRequestID)).ToList();

                                    var content_body = "";
                                    int no = 1;
                                    if(License_detail_item != null)
                                    {
                                        foreach (var items in License_detail_item)
                                        {   
                                            content_body += "<tr><td colspan='3'><b>Visit Location & Interview " + no.ToString() + "</b></td></tr>";
                                            content_body += "<tr><td style='padding-left:2em;'>Manufacture Address</td><td>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MANUFACTURE_ADDRESS+"</td></tr>";
                                            content_body += "<tr><td style='padding-left:2em;'>City</td><td>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MASTER_CITY.CITY_NAME  + "</td></tr>";
                                            content_body += "<tr><td style='padding-left:2em;'>Province</td><td>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MASTER_STATE.STATE_NAME + "</td></tr>";
                                            content_body += "<tr><td colspan='3'></td></tr>";
                                            no++;
                                        }
                                    }
                                    model_mail.Mail_Company_Detail = content_body;

                                    var content_footer_body = "";
                                    content_footer_body += "<tr><td>&nbsp;Creator</td><td>:</td><td>&nbsp;" + CreatorName + "</td></tr>";
                                    content_footer_body += "<tr><td>&nbsp;BA Number Reffence</td><td>:</td><td>" + license_item.INTERVIEW_REQUEST.BA_NUMBER + "</td></tr>";

                                    model_mail.Mail_Sub = content_footer_body;

                                    var sendmail = SendMail(model_mail);
                                    if (!sendmail)
                                        {
                                            msgSuccess += " , but failed send mail to POA Approver";
                                        }
                                    }
                                    else
                                    {
                                        msgSuccess += " , but failed send mail to POA Approver";
                                    }
                        }
                            AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);
                            return RedirectToAction("index");
                        }
                        else
                        {
                            AddMessageInfo("Maximum file size is " + maxFileSize.ToString() + " Mb", Enums.MessageInfoType.Warning);
                            return RedirectToAction("index");
                        }
                    }
                    else
                    {
                        AddMessageInfo("Wrong File Extension", Enums.MessageInfoType.Warning);
                        return RedirectToAction("index");
                    }

                }
            
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Warning);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public ActionResult ChangeStatus(long LRId, string Status, string Comment, string Action)
        {
            try
            {
                int ActionType = 0;
                Action = Action.ToLower();
                var EmailStatus = "";
                if (Action == "approve")
                {
                    ActionType = (int)Enums.ActionType.Approve;
                    EmailStatus = Status;
                }
                else if (Action == "reject")
                {
                    ActionType = (int)Enums.ActionType.Reject;
                    EmailStatus = "rejected";
                }
                else if ((Action == "revise") || (Action == "reviseskep"))
                {
                    ActionType = (int)Enums.ActionType.Revise;
                    EmailStatus = "revised";
                }
                else if (Action == "cancel")
                {
                    ActionType = (int)Enums.ActionType.Cancel;
                    EmailStatus = "canceled";
                }
                else if (Action == "withdraw")
                {
                    ActionType = (int)Enums.ActionType.Withdraw;
                    EmailStatus = "Withdraw";
                }
                else if (Action == "submit")
                {
                    ActionType = (int)Enums.ActionType.Submit;
                    EmailStatus = "has already submitted";
                }
                string ErrMsg = "";
                long ApproveStats = GetSysreffApprovalStatus(Status);
                var update = service.UpdateStatus(LRId, ApproveStats, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, Comment);
                if (update != null)
                {
                    var msgSuccess = "";
                    var strreqdate = update.REQUEST_DATE.ToString("dd MMMM yyyy");
                    var strappdate = update.REQUEST_DATE.ToString("dd MMMM yyyy HH:mm");
                    
                    var Creator = refService.GetPOA(update.CREATED_BY);
                    var CreatorName = Creator.PRINTED_NAME;
                    var CreatorMail = Creator.POA_EMAIL;
                    //var ApproverName = refService.GetPOA(update.LASTAPPROVED_BY).PRINTED_NAME;
                    var perihal = update.INTERVIEW_REQUEST.PERIHAL;
                    var company_type = update.INTERVIEW_REQUEST.COMPANY_TYPE;
                    var Sendto = new List<string>();
                    Sendto.Add(CreatorMail);

                    /* update 04282017 - insert to model */
                    var model_mail = new LicenseRequestMailModel();
                    model_mail.Mail_Form = "";
                    model_mail.Mail_MnfFormNum = update.MNF_FORM_NUMBER;
                    model_mail.Mail_Perihal = update.INTERVIEW_REQUEST.PERIHAL;
                    model_mail.Mail_RequestDate = strreqdate;
                    model_mail.Mail_KPPBC = update.INTERVIEW_REQUEST.KPPBC;
                    model_mail.Mail_Company = update.INTERVIEW_REQUEST.T001.BUTXT;
                    model_mail.Mail_Creator = CreatorName;
                    model_mail.Mail_MnfRequestId = LRId;
                    model_mail.Mail_POA_ReceiverList = Sendto;
                    model_mail.Mail_Approver = update.LASTAPPROVED_BY;
                    model_mail.Mail_ApprovedDate = update.LASTAPPROVED_DATE.ToString();
                    model_mail.Mail_Remark = "-";
                    model_mail.Mail_Comment = Comment;
                    model_mail.Mail_DecreeDate = update.DECREE_DATE.ToString() == null ? "-": Convert.ToDateTime(update.DECREE_DATE).ToString("dd MMMM yyyy");
                    model_mail.Mail_DecreeNo = update.DECREE_NO;
                    model_mail.Mail_BANo = update.INTERVIEW_REQUEST.BA_NUMBER;
                    model_mail.Mail_Status = update.SYS_REFFERENCES.REFF_VALUE;
                    model_mail.Mail_LastStatus = update.SYS_REFFERENCES.REFF_VALUE;
                    /* end update */

                    var License_detail_item = service.GetBoundCondAll().Where(w => w.MNF_REQUEST_ID.Equals(LRId)).ToList();

                    var content_body = "";
                    int no = 1;
                    if (License_detail_item != null)
                    {

                        foreach (var items in License_detail_item)
                        {
                            content_body += "<tr><td colspan='3'><b>Visit Location & Interview " + no.ToString() + "</b></td></tr>";
                            content_body += "<tr><td style='padding-left:2em;'>Manufacture Address</td><td>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MANUFACTURE_ADDRESS + "</td></tr>";
                            content_body += "<tr><td style='padding-left:2em;'>City</td><td>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MASTER_CITY.CITY_NAME + "</td></tr>";
                            content_body += "<tr><td style='padding-left:2em;'>Province</td><td>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MASTER_STATE.STATE_NAME + "</td></tr>";
                            content_body += "<tr><td colspan='3'></td></tr>";
                            no++;
                        }
                    }
                    model_mail.Mail_Company_Detail = content_body;
                    var content_footer_body = "";
                    

                    if (Action == "approve")
                    {
                        bool sendmail;

                        if (Status == "WAITING_GOVERNMENT_APPROVAL")
                        {
                            /* update 04282017 - insert to model */
                            model_mail.Mail_MailFor = "approve";
                            msgSuccess = "Success Approve License Request";
                            //var sendmail = SendMail(update.MNF_FORM_NUMBER, update.INTERVIEW_REQUEST.PERIHAL,update.INTERVIEW_REQUEST.COMPANY_TYPE , strreqdate, CreatorName, Status, ApproverName, strappdate, Comment, LRId, Sendto, "approve");
                            content_footer_body += "<tr><td>&nbsp;BA Number Reffence</td><td>:</td><td>" + update.INTERVIEW_REQUEST.BA_NUMBER + "</td></tr>";
                            model_mail.Mail_Sub = content_footer_body;
                            model_mail.Mail_Status = EmailStatus;
                            sendmail = SendMail(model_mail);
                            /* end update */
                        }
                        else
                        {
                            model_mail.Mail_MailFor = "approve";
                            msgSuccess = "Success Approve License Request";
                            content_footer_body += "<tr><td>&nbsp;DECREE No</td><td>:</td><td>" + update.DECREE_NO + "</td></tr>";
                            content_footer_body += "<tr><td>&nbsp;DECREE Date</td><td>:</td><td>" + Convert.ToDateTime(update.DECREE_DATE).ToString("dd MMMM yyyy") + "</td></tr>";
                            model_mail.Mail_Sub = content_footer_body;
                            model_mail.Mail_Status = "approved by Government and " + EmailStatus;
                            sendmail = SendMail(model_mail);
                        }

                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Creator";
                        }
                    }
                    else if (Action == "reject")
                    {
                        /* update 04282017 - insert to model */
                        model_mail.Mail_MailFor = "reject";
                        msgSuccess = "Success reject License Request";
                        //var sendmail = SendMail(update.MNF_FORM_NUMBER, update.INTERVIEW_REQUEST.PERIHAL, update.INTERVIEW_REQUEST.COMPANY_TYPE, strreqdate, CreatorName, Status, ApproverName, strappdate, Comment, LRId, Sendto, "reject");
                        var sendmail = SendMail(model_mail);
                        /* end update */
                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Creator";
                        }
                    }
                    else if (Action == "revise")
                    {
                        /* update 04282017 - insert to model */
                        model_mail.Mail_MailFor = "revise";
                        msgSuccess = "Success Revise License Request";

                        content_footer_body += "<tr><td>&nbsp;BA Number Reffence</td><td>:</td><td>" + update.INTERVIEW_REQUEST.BA_NUMBER + "</td></tr>";
                        content_footer_body += "<tr><td>&nbsp;Comment</td><td>:</td><td>" + Comment + "</td></tr>";
                        model_mail.Mail_Sub = content_footer_body;
                        model_mail.Mail_Status = EmailStatus;
                        //var sendmail = SendMail(update.MNF_FORM_NUMBER, update.INTERVIEW_REQUEST.PERIHAL, update.INTERVIEW_REQUEST.COMPANY_TYPE, strreqdate, CreatorName, Status, ApproverName, strappdate, Comment, LRId, Sendto, "revise");
                        var sendmail = SendMail(model_mail);
                        /* end update */
                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Creator";
                        }
                    }
                    else if(Action == "reviseskep")
                    {
                        /* update 04282017 - insert to model */
                        model_mail.Mail_MailFor = "revise";
                        msgSuccess = "Success Revise SKEP License Request";

                        content_footer_body += "<tr><td>&nbsp;DECREE Number</td><td>:</td><td>" + update.DECREE_NO + "</td></tr>";
                        content_footer_body += "<tr><td>&nbsp;DECREE Date</td><td>:</td><td>" + Convert.ToDateTime(update.DECREE_DATE).ToString("dd MMMM yyyy") + "</td></tr>";
                        content_footer_body += "<tr><td>&nbsp;BA Number Reffence</td><td>:</td><td>" + update.INTERVIEW_REQUEST.BA_NUMBER + "</td></tr>";
                        content_footer_body += "<tr><td>&nbsp;Comment</td><td>:</td><td>" + Comment + "</td></tr>";
                        model_mail.Mail_Sub = content_footer_body;
                        model_mail.Mail_Status = EmailStatus;
                        //var sendmail = SendMail(update.MNF_FORM_NUMBER, update.INTERVIEW_REQUEST.PERIHAL, update.INTERVIEW_REQUEST.COMPANY_TYPE, strreqdate, CreatorName, Status, ApproverName, strappdate, Comment, LRId, Sendto, "revise");
                        var sendmail = SendMail(model_mail);
                        /* end update */
                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Creator";
                        }
                    }
                    else if (Action == "submit")
                    {
                        msgSuccess = "Success submit License Request";
                        var poareceiverlistall = service.GetPOAApproverList(LRId);
                        if (poareceiverlistall.Count() > 0)
                        {
                            List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();
                            var interview_data = interService.GetInterviewRequestById(update.VR_FORM_ID).FirstOrDefault();

                            model_mail.Mail_MailFor = "submit";

                            content_footer_body += "<tr><td>&nbsp;Creator</td><td>:</td><td>&nbsp;" + CreatorName + "</td></tr>";
                            content_footer_body += "<tr><td>&nbsp;BA Number Reffence</td><td>:</td><td>" + update.INTERVIEW_REQUEST.BA_NUMBER + "</td></tr>";

                            model_mail.Mail_Sub = content_footer_body;
                            model_mail.Mail_Status = EmailStatus;
                            var sendmail = SendMail(model_mail);
                            if (!sendmail)
                            {
                                msgSuccess += " , but failed send mail to POA Approver";
                            }
                        }
                        else
                        {
                            msgSuccess += " , but failed send mail to POA Approver";
                        }
                    } else if(Action == "withdraw")
                    {
                        model_mail.Mail_MailFor = "withdraw";
                        msgSuccess = "Success Withdraw License Request";

                        content_footer_body += "<tr><td>&nbsp;BA Number Reffence</td><td>:</td><td>" + update.INTERVIEW_REQUEST.BA_NUMBER + "</td></tr>";
                        content_footer_body += "<tr><td>&nbsp;Comment</td><td>:</td><td>" + Comment + "</td></tr>";
                        model_mail.Mail_Sub = content_footer_body;
                        model_mail.Mail_Status = EmailStatus;
                        
                        var sendmail = SendMail(model_mail);
                        
                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Creator";
                        }
                    }
                    else if(Action ==  "cancel")
                    {
                        model_mail.Mail_MailFor = "cancel";
                        msgSuccess = "Success Cancel License Request";
                    }
                    AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);
                }
                return Json(ErrMsg);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return Json(ex.Message);
            }
        }



        [HttpPost]
        public ActionResult GetInterviewRequestDetail(Int64 IRId)
        {
            try
            {
                var Interview = interService.GetAll().Where(w => w.VR_FORM_ID.Equals(IRId)).Select(s => new InterviewRequestModel
                {
                    FormNumber = s.FORM_NUMBER,
                    BANumber = s.BA_NUMBER,
                    Company_Type = s.COMPANY_TYPE,
                    POAName = s.POA.PRINTED_NAME,
                    POAPosition = s.POA.TITLE,
                    POAAddress = s.POA.POA_ADDRESS,
                    KPPBC_ID = s.KPPBC,  
                    //KPPBC_Address = s.NPPBKC_ID == null ? "" : s.ZAIDM_EX_NPPBKC.ADDR1 + " " + s.ZAIDM_EX_NPPBKC.ADDR2,
                    KPPBC_Address = s.KPPBC_ADDRESS == null ? "" : s.KPPBC_ADDRESS,
                    Company_Name = s.T001.BUTXT,
                    Company_ID = s.T001.BUKRS,
                    Npwp = s.T001.NPWP,
                    Company_Address = s.T001.SPRAS
                }).FirstOrDefault();

                var irmodel = new LicenseRequestModel();
                irmodel.InterviewId = IRId;
                
                var LRModel = SetIRDetail(irmodel);
                var InterviewDet = new List<InterviewRequestDetails>();
                if (LRModel != null)
                {
                    InterviewDet = LRModel.IRDetails;
                }

                var dataAttr = new { Interview = Interview, InterviewDet = InterviewDet };
                return Json(dataAttr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return Json(null);
            }
        }

        [HttpPost]
        public PartialViewResult GetInterviewRequestDetailFormView(string add, long mfi, string village, string phone, string fax, string province, string city, string subdist, int index, int numlist)
        {
            try
            {                
                var Interview = new InterviewRequestDetails();
                Interview.MnfAddr = add;
                Interview.MnfDetailId = mfi;
                Interview.MnfVillage = village;
                Interview.MnfPhone = phone;
                Interview.MnfFax = fax;
                Interview.MnfProvince = province;
                Interview.MnfCity = city;
                Interview.MnfSubDist = subdist;
                Interview.Index = index;
                Interview.NumList = numlist;                
                Interview.North = "";
                Interview.East = "";
                Interview.South = "";
                Interview.West = "";
                Interview.LandArea = 0;
                Interview.BuildingArea = 0;
                Interview.OwnershipStatus = "";
                return PartialView("_LicenseRequestInterview", Interview);
            }
            catch (Exception)
            {
                return PartialView();
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
        
        private SelectList GetFormNumList(IQueryable<CustomService.Data.INTERVIEW_REQUEST> FormNumList)
        {
            var query = from x in FormNumList
                        select new SelectItemModel()
                        {
                            ValueField = x.VR_FORM_ID.ToString(),
                            TextField = x.FORM_NUMBER
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetFormNumListFilter(IQueryable<CustomService.Data.MANUFACTURING_LISENCE_REQUEST> FormNumList)
        {
            var query = from x in FormNumList
                        select new SelectItemModel()
                        {
                            ValueField = x.VR_FORM_ID.ToString(),
                            TextField = x.MNF_FORM_NUMBER
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetListFormNumFilter(List<LicenseRequestModel> FormNumList)
        {
            var query = from x in FormNumList
                        select new SelectItemModel()
                        {
                            ValueField = x.InterviewId,
                            TextField = x.MnfFormNum
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

private SelectList GetCompTypeList(Dictionary<int, string> types)
        {
            var query = from x in types
                        select new SelectItemModel()
                        {
                            ValueField = x.Value,
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetProdTypeList(List<MANUFACTURING_PRODUCT_TYPE> ProdTypeList)
        {
            var query = from x in ProdTypeList
                        select new SelectItemModel()
                        {
                            ValueField = x.PROD_CODE ,
                            TextField = x.ZAIDM_EX_PRODTYP.PRODUCT_TYPE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetKPPBCList(List<string> nppbkcList)
        {
            var query = from x in nppbkcList
                        select new SelectItemModel()
                        {
                            ValueField = x,
                            TextField = x
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetKPPBCListForFilter(IQueryable<CustomService.Data.MANUFACTURING_LISENCE_REQUEST> kppbcList)
        {
            var query = from x in kppbcList
                        select new SelectItemModel()
                        {
                            ValueField = x.INTERVIEW_REQUEST.KPPBC ,
                            TextField = x.INTERVIEW_REQUEST.KPPBC
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetLastApprovedStatus(IQueryable<SYS_REFFERENCES> status)
        {
            var query = from x in status
                        select new SelectItemModel()
                        {
                            ValueField = x.REFF_ID.ToString() ,
                            TextField = x.REFF_VALUE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
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

        private SelectList GetOwnerStatusList(Dictionary<string,string> status)
        {
            var query = from x in status
                        select new SelectItemModel()
                        {
                            ValueField = x.Key,
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private LicenseRequestModel SetDetailProductType(LicenseRequestModel pcmodel)
        {

            var listDetailsProdType = new List<ProductTypeDetails>();
            var pcindex = 0;
            foreach (var val in prodtypeService.GetAll().Where(w=>w.IS_DELETED == true || w.IS_DELETED == null))
            {
                var detailsProdType = new ProductTypeDetails();
                
                detailsProdType.ProdCode = val.PROD_CODE;
                detailsProdType.ProductType = val.PRODUCT_TYPE;
                detailsProdType.ProdAlias = val.PRODUCT_ALIAS;

                var val_detailsProdType = service.GetProductTypeById().Where(w => w.PROD_CODE.Equals(val.PROD_CODE) && w.MNF_REQUEST_ID.Equals(pcmodel.MnfRequestID)).FirstOrDefault();

                if( val_detailsProdType != null)
                {
                    if (val_detailsProdType.PROD_CODE == val.PROD_CODE)
                    {
                        detailsProdType.IsChecked = "checked";
                    }
                    else
                    {
                        detailsProdType.IsChecked = "";
                    }
                }
                else
                {
                    detailsProdType.IsChecked = "";
                }

                detailsProdType.ptIndex = pcindex++;
                listDetailsProdType.Add(detailsProdType);
            }

            pcmodel.PCDetails = listDetailsProdType;
            return pcmodel;
        }

        private LicenseRequestModel SetOtherProductType(LicenseRequestModel otherpt)
        {
            var listOtherProdType = new List<ProductTypeDetails>();
            var otheridx = 0;

            foreach(var val in service.GetProductTypeById().Where(w => w.MNF_REQUEST_ID.Equals(otherpt.MnfRequestID) && w.PROD_CODE==null ))
            {
                var otherProdType = new ProductTypeDetails();

                otherProdType.ProdTypeId = val.MNF_PROD_TYPE_ID;
                otherProdType.OtherProdCode = val.OTHERS_PROD_TYPE;

                otherProdType.ptIndex = otheridx++;
                listOtherProdType.Add(otherProdType);
                
            }

            otherpt.OtherPCDetails = listOtherProdType;
            return otherpt;
        }

        [HttpPost]
        public JsonResult RemoveOtherProdType(Int64 OtherProdTypeId)
        {
            return Json(service.RemoveProductType(OtherProdTypeId));
        }

        private LicenseRequestModel GenerateModelProperties(LicenseRequestModel model)
        {
            if (model == null)
            {
                model = new LicenseRequestModel()
                {
                    RequestDate = DateTime.Now,
                    Status_Value = "DRAFT NEW",
                    Status       ="DRAFT_NEW_STATUS",
                    //ProdTypeList    = GetProdTypeList(prodtypeService.GetAll().Where(w=>w.IS_DELETED != true && w.APPROVED_STATUS == 29)),
                    ProdTypeList = GetProdTypeList(service.GetAllProductTypeFromLR()),
                    //FormNumList   = GetFormNumList(interService.GetAll()),
                    FormNumList     = GetFormNumList(service.GetIRCompleteAll()),
                    MainMenu        = mainMenu,
                    CurrentMenu     = PageInfo,
                    File_Size       = GetMaxFileSize()
                };
            }

            model = SetDetailProductType(model);
            model.link_BA = GenerateURL("MLInterviewRequest");
            //model.Confirmation = GenerateConfirmDialog();
            model.Confirmation = GenerateConfirmDialogForm(true, false, false);
            //model.ChangesHistoryList = new List<ChangesHistoryItemModel>() ;
            return model;
        }

        private LicenseRequestModel SetIRDetail(LicenseRequestModel irmodel)
        {
            try
            {
                var listIRDetails = new List<InterviewRequestDetails>();
                var index = 0;
                foreach (var val in service.GetDetailAll().Where(s => s.VR_FORM_ID.Equals(irmodel.InterviewId)))
                {
                    var IRdetails = new InterviewRequestDetails();
                    IRdetails.MnfDetailId = val.VR_FORM_DETAIL_ID;
                    IRdetails.MnfAddr = val.MANUFACTURE_ADDRESS;
                    IRdetails.MnfVillage = val.VILLAGE;
                    IRdetails.MnfPhone = val.PHONE;
                    IRdetails.MnfCity = val.MASTER_CITY.CITY_NAME;
                    IRdetails.MnfProvince = val.MASTER_STATE.STATE_NAME;
                    IRdetails.MnfFax = val.FAX;
                    IRdetails.MnfSubDist = val.SUB_DISTRICT;

                    var BCdetails = service.GetBoundCondAll().Where(x => x.VR_FORM_DETAIL_ID.Equals(val.VR_FORM_DETAIL_ID) && x.MNF_REQUEST_ID.Equals(irmodel.MnfRequestID)).FirstOrDefault();
                    if (BCdetails != null)
                    {
                        IRdetails.North = BCdetails.NORTH;
                        IRdetails.East = BCdetails.EAST;
                        IRdetails.West = BCdetails.WEST;
                        IRdetails.South = BCdetails.SOUTH;
                        IRdetails.LandArea = Convert.ToDecimal(BCdetails.LAND_AREA);
                        IRdetails.BuildingArea = Convert.ToDecimal(BCdetails.BUILDING_AREA);
                        IRdetails.OwnershipStatus = BCdetails.OWNERSHIP_STATUS;
                    }
                    else
                    {
                        IRdetails.North = "";
                        IRdetails.East = "";
                        IRdetails.West = "";
                        IRdetails.South = "";
                        IRdetails.LandArea = 0;
                        IRdetails.BuildingArea = 0;
                        IRdetails.OwnershipStatus = "";
                    }
                    IRdetails.Index = index++;
                    listIRDetails.Add(IRdetails);
                }

                irmodel.IRDetails = listIRDetails;
                return irmodel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return null;
            }
        }
        
        public List<LicenseRequestSupportingDocModel> GetSupportingDocumentMaster(long interviewId)
        {
            try
            {
                var SDModel = new List<LicenseRequestSupportingDocModel>();

                var bukrs = interService.GetInterviewRequestById(interviewId).Select(s=>s.BUKRS).FirstOrDefault();
                var formId = (long)Enums.FormList.License;
                var docs = refService.GetSupportingDocuments(formId, bukrs);
                var model = docs.Select(x => MapSupportingDocumentModel(x)).ToList();

                if (model.Any())
                {
                    SDModel = model.Select(s => new LicenseRequestSupportingDocModel
                    {
                        Name = s.Name
                    }).ToList();
                }
                return SDModel;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //public ActionResult GetSupportingDocuments(string CompanyId, long IRId, bool IsReadonly)
        //{
        //    var formId = (long)Enums.FormList.License;
        //    var docs = refService.GetSupportingDocuments(formId, CompanyId);
        //    var model = docs.Select(x => MapSupportingDocumentModel(x)).ToList();
        //    if (IRId != 0 && IRId != null)
        //    {
        //        var Doclist = service.GetFileUploadByIRId(IRId);
        //        if (Doclist != null)
        //        {
        //            Doclist = Doclist.Where(w => w.DOCUMENT_ID != null);
        //            if (Doclist != null)
        //            {
        //                List<InterviewRequestSupportingDocModel> listDoc = Doclist.Select(s => new InterviewRequestSupportingDocModel
        //                {
        //                    DocId = s.DOCUMENT_ID,
        //                    Path = s.PATH_URL,
        //                    FileUploadId = s.FILE_ID
        //                }).ToList();
        //                foreach (var doc in listDoc)
        //                {
        //                    var whereModel = model.Where(w => w.Id.Equals(doc.DocId)).FirstOrDefault();
        //                    if (whereModel != null)
        //                    {
        //                        whereModel.Path = doc.Path;
        //                        whereModel.IsBrowseFileEnable = false;
        //                        whereModel.FileUploadId = doc.FileUploadId;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    foreach (var mod in model)
        //    {
        //        mod.IsReadonly = IsReadonly;
        //    }
        //    return PartialView("_SupportingDocuments", model);
        //}

        [HttpPost]
        public ActionResult GetSupportingDocuments(string CompanyId, long LRId, bool IsReadonly, string StatusKey)
        {
            var formId = (long)Enums.FormList.License ;
            var docs = refService.GetSupportingDocuments(formId, CompanyId);
            var model = docs.Select(x => MapSupportingDocumentModel(x)).ToList();
            if (LRId != 0 && LRId.ToString() != null)
            {
                var Doclist = service.GetFileUploadByLRId(LRId);
                if (Doclist != null)
                {
                    Doclist = Doclist.Where(w => w.DOCUMENT_ID != null);
                    if (Doclist != null)
                    {
                        var listAvaDocId = Doclist.Select(s => s.DOCUMENT_ID).ToList();
                        if (StatusKey != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Draft) && StatusKey != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Edited))
                        {
                            model = model.Where(w => listAvaDocId.Contains(w.Id)).ToList();
                        }
                        List<InterviewRequestSupportingDocModel> listDoc = Doclist.Select(s => new InterviewRequestSupportingDocModel
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
                                whereModel.Path = doc.Path;
                                whereModel.IsBrowseFileEnable = false;
                                whereModel.FileUploadId = doc.FileUploadId;
                                whereModel.FileName = GenerateFileName(whereModel.Path);
                                whereModel.Path = GenerateURL(whereModel.Path);
                            }
                        }
                    }
                }
            }
            foreach (var mod in model)
            {
                mod.IsReadonly = IsReadonly;
            }
            return PartialView("_SupportingDocuments", model);
        }

        private List<MANUFACTURING_BOUND_CONDITION> MapToBoundCondition(List<InterviewRequestDetails> LReqDet)
        {
            var result = new List<MANUFACTURING_BOUND_CONDITION>();
            foreach (var detail in LReqDet)
            {
                result.Add(new MANUFACTURING_BOUND_CONDITION
                {
                    MNF_COND_ID = detail.MnfDetailId,
                    VR_FORM_DETAIL_ID = detail.InterviewDetailId,
                    MNF_REQUEST_ID = detail.MnfReqId,
                    NORTH = detail.North,
                    SOUTH = detail.South,
                    EAST = detail.East,
                    WEST = detail.West,
                    LAND_AREA = detail.LandArea,
                    BUILDING_AREA = detail.BuildingArea,
                    OWNERSHIP_STATUS = detail.OwnershipStatus
                });
            }
            return result;
        }

        public LicenseRequestSupportingDocModel MapSupportingDocumentModel(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new LicenseRequestSupportingDocModel()
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

        private LicenseRequestModel MapLicenseRequestModel(CustomService.Data.MANUFACTURING_LISENCE_REQUEST entity)
        {
            try
            {
                
                return new LicenseRequestModel()
                {
                    Id = entity.VR_FORM_ID,
                    MnfFormNum = entity.MNF_FORM_NUMBER,
                    RequestDate = entity.REQUEST_DATE,
                    
                    LastApprovedBy = entity.INTERVIEW_REQUEST.COMPANY_TYPE,
                    KPBCName = entity.ZAIDM_EX_NPPBKC.TEXT_TO,
                    Company = entity.INTERVIEW_REQUEST.T001.BUTXT,
                    LastApprovedStatus = entity.SYS_REFFERENCES.REFF_ID
                    
                };
            }
            catch (Exception ex)
            {
                throw ex;
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
                var extList = service.GetFileExtList();
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
                    var file_name = Path.GetFileNameWithoutExtension(File.FileName) + "=MLR=" + CurrentUser.USER_ID + "-" + date + extension;
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

        public void InsertUploadSuppDocFile(IEnumerable<LicenseRequestSupportingDocModel> SuppDocList, long IRId)
        {
            try
            {
                if (SuppDocList != null)
                {
                    foreach (var Doc in SuppDocList)
                    {
                        if (Doc.Path != "" && Doc.Path != null)
                        {
                            var filename = service.GetSupportingDocName(Doc.Id);
                            service.InsertFileUpload(IRId, Doc.Path, CurrentUser.USER_ID, Doc.Id, false, filename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                throw;
            }
        }

        //public void InsertUploadCommonFile(List<string> FilePath, long IRId, bool IsGov)
        //{
        //    try
        //    {
        //        if (FilePath != null)
        //        {
        //            foreach (var Doc in FilePath)
        //            {
        //                if (Doc != null && Doc != "")
        //                {
        //                    service.InsertFileUpload(IRId, Doc, CurrentUser.USER_ID, 0, IsGov);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }
        //}

        public void InsertUploadCommonFile(List<string> FilePath, long IRId, bool IsGov, List<string> FileName)
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
                            DocName = DocName.Replace("=MLR=", "/");
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
                            service.InsertFileUpload(IRId, Doc, CurrentUser.USER_ID, 0, IsGov, thefilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                throw;
            }
        }

        //public bool SendMail(string formnumber, string perihal, string company_type, string request_date, string creator, string approval_status, string approver, string approvedDate, string remark, long irid, List<string> sendto, string MailFor)
        public bool SendMail(LicenseRequestMailModel model)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("form", model.Mail_Form == null ? "#" : model.Mail_Form);
                parameters.Add("number", model.Mail_MnfFormNum == null ? "#": model.Mail_MnfFormNum);
                parameters.Add("perihal", model.Mail_Perihal == null ? "#": model.Mail_Perihal);
                parameters.Add("company", model.Mail_Company == null ? "#": model.Mail_Company);
                parameters.Add("date", model.Mail_RequestDate == null ? "#": model.Mail_RequestDate);
                parameters.Add("kppbc", model.Mail_KPPBC == null ?"#": model.Mail_KPPBC);
                //parameters.Add("creator", model.Mail_Creator == null ?"#": model.Mail_Creator);
                parameters.Add("status", model.Mail_Status == null ?"#": model.Mail_Status);
                parameters.Add("last", model.Mail_LastStatus == null ? "#" : model.Mail_LastStatus);
                //parameters.Add("approver", model.Mail_Approver == null ?"#": model.Mail_Approver);
                parameters.Add("block_details", model.Mail_Company_Detail == null ?"#": model.Mail_Company_Detail);
                //parameters.Add("remark", model.Mail_Comment == null ?"#": model.Mail_Comment);
                //parameters.Add("bano", model.Mail_BANo == null ?"#": model.Mail_BANo);
                //parameters.Add("decreeno", model.Mail_DecreeNo == null ?"#": model.Mail_DecreeNo);
                //parameters.Add("decreedate", model.Mail_DecreeDate ==null ?"#": model.Mail_DecreeDate);
                parameters.Add("url_detail", Url.Action("Detail", "MLLicenseRequest", new { Id = model.Mail_MnfRequestId, st=1 }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "MLLicenseRequest", new { Id = model.Mail_MnfRequestId, st=2 }, this.Request.Url.Scheme));
                parameters.Add("url_edit", Url.Action("Edit", "MLLicenseRequest", new { Id = model.Mail_MnfRequestId }, this.Request.Url.Scheme));
                parameters.Add("footer", model.Mail_Sub == null ? "#" : model.Mail_Sub);

                long mailcontentId = 0;
                if (model.Mail_MailFor == "submit")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseApprovalRequest;
                }
                else if (model.Mail_MailFor == "approve")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseApprovalNotification;
                }
                else if (model.Mail_MailFor == "revise")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseRevisionRequest;
                }
                else if(model.Mail_MailFor == "withdraw")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseApprovalNotification;
                }
                else if (model.Mail_MailFor == "reject")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseApprovalNotification;
                }

                var mailContent = refService.GetMailContent(mailcontentId, parameters);
                var senderMail = refService.GetUserEmail(CurrentUser.USER_ID);
                string[] arrSendto = model.Mail_POA_ReceiverList.ToArray();
                //bool mailStatus = ItpiMailer.Instance.SendEmail(arrSendto, null, null, null, mailContent.EMAILSUBJECT, mailContent.EMAILCONTENT, true, senderMail, creator);
                bool mailStatus = ItpiMailer.Instance.SendEmail(arrSendto, null, null, null, mailContent.EMAILSUBJECT, mailContent.EMAILCONTENT, true, senderMail, model.Mail_Creator);
                return mailStatus;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return false;
            }
        }

        public long GetSysreffApprovalStatus(string currStatus)
        {
            long ApproveStats = 0;
            var Reff = refService.GetRefByType("APPROVAL_STATUS");
            if (Reff.Any())
            {
                currStatus = currStatus.ToUpper();
                if (currStatus == "DRAFT_NEW_STATUS")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("DRAFT_NEW_STATUS")).Select(s => s.REFF_ID).FirstOrDefault();
                }
                else if (currStatus == "DRAFT_EDIT_STATUS")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("DRAFT_EDIT_STATUS")).Select(s => s.REFF_ID).FirstOrDefault();
                }
                else if (currStatus == "WAITING_POA_APPROVAL")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("WAITING_POA_APPROVAL")).Select(s => s.REFF_ID).FirstOrDefault();
                }
                else if (currStatus == "WAITING_GOVERNMENT_APPROVAL")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("WAITING_GOVERNMENT_APPROVAL")).Select(s => s.REFF_ID).FirstOrDefault();
                }
                else if (currStatus == "WAITING_POA_SKEP_APPROVAL")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("WAITING_POA_SKEP_APPROVAL")).Select(s => s.REFF_ID).FirstOrDefault();
                }
                else if (currStatus == "REJECTED")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("REJECTED")).Select(s => s.REFF_ID).FirstOrDefault();
                }
                else if (currStatus == "COMPLETED")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("COMPLETED")).Select(s => s.REFF_ID).FirstOrDefault();
                }
                else if (currStatus == "CANCELED")
                {
                    ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("CANCELED")).Select(s => s.REFF_ID).FirstOrDefault();
                }
            }
            return ApproveStats;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSkep(LicenseRequestModel model)
        {
            try
            {
                var maxFileSize = GetMaxFileSize();
                var isOkFileExt = true;
                var isOkFileSize = true;
                isOkFileExt = CheckFileExtension(model.File_BA);
                var msgSuccess = "Success submit License Request";
                if (isOkFileExt)
                {
                    isOkFileSize = CheckFileSize(model.File_BA, maxFileSize);
                    if (isOkFileSize)
                    {
                        //if (model.File_BA != null)
                        //{
                        //    foreach (var FileBA in model.File_BA)
                        //    {
                        //        var PathFile = UploadFile(FileBA);
                        //        if (PathFile != "")
                        //        {
                        //            model.File_BA_Path.Add(PathFile);
                        //        }
                        //    }
                        //}
                        if (model.File_BA != null)
                        {
                            var AddedfileBAList = new List<string>();
                            var removedIndex = new List<int>();
                            var index = 0;
                            foreach (var FileBA in model.File_BA)
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
                                        model.File_BA_Path.Add(PathFile);
                                    }
                                }
                                index++;
                            }
                            removedIndex = removedIndex.OrderByDescending(o => o).ToList();
                            foreach (var i in removedIndex)
                            {
                                model.File_BA.RemoveAt(i);
                            }
                        }
                        long ApproveStats = GetSysreffApprovalStatus("WAITING_POA_SKEP_APPROVAL");
                        if (model.Status == "WAITING_GOVERNMENT_APPROVAL")
                        {
                            
                            if (model.MnfRequestID != 0 && model.MnfRequestID.ToString() != null)
                            {
                                var IRmodel = interService.GetInterviewRequestById(model.InterviewId).FirstOrDefault();

                                if (model.NppbkcID != "" )
                                {
                                    service.InsertNPPBKC(model.NppbkcID, IRmodel.KPPBC_ADDRESS == null ? "" : IRmodel.KPPBC_ADDRESS, IRmodel.CITY, IRmodel.CITY_ALIAS, IRmodel.TEXT_TO, IRmodel.KPPBC, CurrentUser.USER_ID);
                                }

                                //// InActiving/Removing Doc
                                foreach (var removedfile in model.RemovedFilesId)
                                {
                                    if (removedfile != 0)
                                    {
                                        service.DeleteFileUpload(removedfile, CurrentUser.USER_ID);
                                    }
                                }

                                var ActionType = 0;
                                ActionType = (int)Enums.ActionType.Submit;
                                var updateSKEP = service.UpdateBASKEP(model.MnfRequestID, Convert.ToBoolean(model.DecreeStatus), model.DecreeNo , Convert.ToDateTime(model.DecreeDate), model.NppbkcID, ApproveStats, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, model.Comment, model.InterviewId);
                                if (updateSKEP != null)
                                {
                                    InsertUploadCommonFile(model.File_BA_Path, model.MnfRequestID, true, model.File_Other_Name);
                                    var poareceiverlistall = service.GetPOAApproverList(model.MnfRequestID);
                                    if (poareceiverlistall.Count() > 0)
                                    {
                                        List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();
                                        var strreqdate = updateSKEP.REQUEST_DATE.ToString("dd MMMM yyyy");
                                        var CreatorName = refService.GetPOA(CurrentUser.USER_ID).PRINTED_NAME;
                                        var PERIHAL = updateSKEP.INTERVIEW_REQUEST.PERIHAL;
                                        var COMPANY_TYPE = updateSKEP.INTERVIEW_REQUEST.COMPANY_TYPE;

                                        var model_mail = new LicenseRequestMailModel();
                                        model_mail.Mail_Form = "SKEP";
                                        model_mail.Mail_MnfFormNum = updateSKEP.MNF_FORM_NUMBER;
                                        model_mail.Mail_Perihal = updateSKEP.INTERVIEW_REQUEST.PERIHAL;
                                        model_mail.Mail_RequestDate = updateSKEP.REQUEST_DATE.ToString("dd MMMM yyyy");
                                        model_mail.Mail_KPPBC = updateSKEP.INTERVIEW_REQUEST.KPPBC;
                                        model_mail.Mail_Company = updateSKEP.INTERVIEW_REQUEST.T001.BUTXT;
                                        model_mail.Mail_Creator = CreatorName;
                                        model_mail.Mail_Status = model.Status;
                                        model_mail.Mail_LastStatus = model.Status;
                                        model_mail.Mail_MnfRequestId = model.MnfRequestID;
                                        model_mail.Mail_POA_ReceiverList = poareceiverList;
                                        model_mail.Mail_MailFor = "submit";
                                        model_mail.Mail_Approver = "";
                                        model_mail.Mail_ApprovedDate = "";
                                        model_mail.Mail_Remark = "";
                                        model_mail.Mail_BANo = updateSKEP.INTERVIEW_REQUEST.BA_NUMBER;
                                        model_mail.Mail_DecreeDate = Convert.ToDateTime(updateSKEP.DECREE_DATE).ToString("dd MMMM yyyy");
                                        model_mail.Mail_DecreeNo = updateSKEP.DECREE_NO;

                                        var License_detail_item = service.GetBoundCondAll().Where(w => w.MNF_REQUEST_ID.Equals(model.MnfRequestID)).ToList();

                                        var content_body = "";
                                        int no = 1;
                                        if (License_detail_item != null)
                                        {

                                            foreach (var items in License_detail_item)
                                            {
                                                content_body += "<tr><td colspan='2'>Location " + no.ToString() + "</td></tr>";
                                                content_body += "<tr><td colspan='2'><table><tr><td colspan='2'></td></tr>";
                                                content_body += "<tr><td width='30%'>Manufature Address</td><td width='5px'>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MANUFACTURE_ADDRESS + "</td>";
                                                content_body += "<tr><td width='30%'>Manufature Address</td><td width='5px'>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MASTER_CITY.CITY_NAME + "</td>";
                                                content_body += "<tr><td width='30%'>Manufature Address</td><td width='5px'>:</td><td>" + items.INTERVIEW_REQUEST_DETAIL.MASTER_STATE.STATE_NAME + "</td>";
                                                content_body += "<tr><td colspan='2'></td></tr></table></td></tr>";
                                                no++;
                                            }
                                        }
                                        model_mail.Mail_Company_Detail = content_body;

                                        var content_footer_body = "";

                                        content_footer_body += "<tr><td>&nbsp;DECREE Number</td><td>:</td><td>" + updateSKEP.DECREE_NO + "</td></tr>";
                                        content_footer_body += "<tr><td>&nbsp;DECREE Date</td><td>:</td><td>" + Convert.ToDateTime(updateSKEP.DECREE_DATE).ToString("dd MMMM yyyy") + "</td></tr>";

                                        model_mail.Mail_Sub = content_footer_body;
                                        //var mailsent = SendMail(updateSKEP.MNF_FORM_NUMBER, PERIHAL, COMPANY_TYPE, strreqdate, CreatorName, model.Status, "", "", "", model.MnfRequestID, poareceiverList, "submit");
                                        var mailsent = SendMail(model_mail);
                                        if (!mailsent)
                                        {
                                            msgSuccess += " , but failed send mail to POA Approver";
                                        }
                                    }
                                    else
                                    {
                                        msgSuccess += " , but failed send mail to POA Approver";
                                    }
                                }
                            }
                        }
                    }
                }
                AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        public ActionResult ChangeLog(int CRID, string Token)
        {
            var licenseRequest = new LicenseRequestModel();

            var history = refService.GetChangesHistory((int)Enums.MenuList.LicenseRequest , CRID.ToString()).ToList();
            licenseRequest.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);

            return PartialView("_ChangesHistoryTable", licenseRequest);
        }

        private List<ConfirmDialogModel> GenerateConfirmDialogForm(bool Submit, bool Cancel, bool Approve)
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
                            CssClass = "btn btn-blue btn_loader",
                            Label = "Submit"
                        },
                        CssClass = " submit-modal licenserequest",
                        Message = "You are going to Submit License Request. Are you sure?",
                        Title = "Submit Confirmation",
                        ModalLabel = "SubmitModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnSubmitSkep",
                            CssClass = "btn btn-blue btn_loader",
                            Label = "Submit"
                        },
                        CssClass = " submitskep-modal licenserequest",
                        Message = "You are going to Submit Skep License Request. Are you sure?",
                        Title = "Submit Confirmation",
                        ModalLabel = "SubmitModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnWithdraw",
                            CssClass = "btn btn-blue btn_loader",
                            Label = "Withdraw"
                        },
                        CssClass = " withdraw-modal licenserequest",
                        Message = "You are going to Withdraw License Request. Are you sure?",
                        Title = "Withdraw Document Confirmation",
                        ModalLabel = "WithdrawModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnCancel",
                            CssClass = "btn btn-blue btn_loader",
                            Label = "Cancel Document"
                        },
                        CssClass = " cancel-modal licenserequest",
                        Message = "You are going to Cancel License Request. Are you sure?",
                        Title = "Cancel Document Confirmation",
                        ModalLabel = "CancelModalLabel"
                    });
                }
                if (Cancel)
                {
                    
                }
                if (Approve)
                {
                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnApprove",
                            CssClass = "btn btn-blue btn_loader",
                            Label = "Approve"
                        },
                        CssClass = " approve-modal licenserequest",
                        Message = "You are going to approve License Request. Are you sure?",
                        Title = "Approve Confirmation",
                        ModalLabel = "ApproveModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnApproveFinal",
                            CssClass = "btn btn-blue btn_loader",
                            Label = "Approve"
                        },
                        CssClass = " approvefinal-modal licenserequest",
                        Message = "You are going to approve License Request. Are you sure?",
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
                        CssClass = "btn btn-blue btn_loader",
                        Label = "Restore To Default"
                    },
                    CssClass = " restoredefault-modal licenserequest",
                    Message = "You are going to restore printout layout to default. Are you sure?",
                    Title = "Restore Printout Confirmation",
                    ModalLabel = "RestoreModalLabel"
                });
                //////////////////////////////////////////////////

                return listconfirmation;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<ConfirmDialogModel>();
            }
        }

        public ActionResult GetPrintOutLayout(string Creator)
        {
            /*var result = new FilePathResult("~/Views/MLLicenseRequest/PrintLayout.html", "text/html");
            return result;*/
            var result = refService.GetPrintoutLayout("MANUFACTURING_LICENSE_REQUEST_PRINTOUT", Creator);
            var layout = "No Layout Found.";
            if (result.Any())
            {
                layout = result.FirstOrDefault().LAYOUT;
            }
            return Json(layout);
        }

        private string GetPrintout(LicenseRequestModel _LRModel)
        {
            _LRModel = GetLicenseRequestMasterForm(_LRModel);
            _LRModel.LicenseRequestBoundCondition       = GetLicenseRequestBoundCondition(_LRModel.MnfRequestID);
            _LRModel.LicenseRequestProductType          = GetProductTypeMaster(_LRModel.MnfRequestID);
            _LRModel.LicenseRequestOtherProductType     = GetOtherProductTypeMaster(_LRModel.MnfRequestID);
            _LRModel.LicenseSupportingDocumentMaster    = GetSupportingDocumentMaster(_LRModel.InterviewId);

            var layout = "";

            System.Globalization.CultureInfo CIndo = new System.Globalization.CultureInfo("id-ID");

            var parameters = new Dictionary<string, string>();
            parameters.Add("COMPANY_NAME", _LRModel.Company);
            parameters.Add("COMPANY_ADDRESS", _LRModel.Company_Address);
            parameters.Add("COMPANY_NPWP", _LRModel.Npwp);
            parameters.Add("CITY", _LRModel.Company_City);
            parameters.Add("REQUEST_DATE", _LRModel.RequestDate.ToString("dd MMMM yyyy",CIndo));
            parameters.Add("FORM_NUMBER", _LRModel.MnfFormNum);
            parameters.Add("KPPBC", _LRModel.KPBCName);
            parameters.Add("ADDRESS_KPPBC", _LRModel.KPPBC_Address);
            parameters.Add("POA_NAME", _LRModel.POAName);
            parameters.Add("POA_ROLE", _LRModel.POAPosition);
            parameters.Add("POA_ADDRESS", _LRModel.POAAddress);
            //parameters.Add("PERIHAL", _LRModel.Perihal);
            parameters.Add("COMPANY_TYPE", _LRModel.Company_Type);

            var mnfcity_list = "";
            var mnfaddr_list = "";
            var location_list = "";
            int idx_bc = 1;
            foreach (var detlocation in _LRModel.LicenseRequestBoundCondition)
            {
                location_list += "<table bgcolor='#ffffff' style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td width='10pt'></td><td><b>Detil Pabrik " + idx_bc.ToString()  + "</b></td></tr></table><table bgcolor='#ffffff' style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr>";
                location_list += "<td width='40pt'></td><td>1. Lokasi Pabrik</td></tr></table>";
                location_list += "<table bgcolor='#ffffff'><tr><td width='50pt'></td><td><table bgcolor='#ffffff' style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td>a.</td><td style='width:100pt'>Alamat Jalan</td><td>:</td><td>" + detlocation.MnfAddr + "</td></tr>";
                location_list += "<tr><td>b.</td><td>Kelurahan/Desa</td><td>:</td><td>" + detlocation.MnfVillage + "</td></tr><tr><td>c.</td><td>Kecamatan</td><td>:</td><td>" + detlocation.MnfSubDist + "</td></tr><tr><td>d.</td><td>Kabupaten/Kota</td><td>:</td><td>" + detlocation.MnfCity + "</td></tr><tr><td>e.</td><td>Provinsi</td><td>:</td><td>" + detlocation.MnfProvince + "</td></tr><tr><td>f.</td><td>Telepon/Faksimili</td><td>:</td><td>" + detlocation.MnfPhone + "/" + detlocation.MnfFax + "</td></tr></table>";
                location_list += "</td></tr></table><br/><table bgcolor='#ffffff'style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td width='40pt'></td><td>2. Batas-batas</td></tr></table><table bgcolor='#ffffff'style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td width='50pt'></td>";
                location_list += "<td><table bgcolor='#ffffff'style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td>a.</td><td style='width:100pt'>Utara</td><td>:</td><td>" + detlocation.North+"</td></tr><tr><td>b.</td><td>Timur</td><td>:</td><td>"+detlocation.East+"</td></tr><tr><td>c.</td><td>Selatan</td><td>:</td><td>"+detlocation.South+"</td></tr><tr><td>d.</td><td>Barat</td><td>:</td><td>"+detlocation.West+"</td></tr></table></td></tr></table>";
                location_list += "<br/><table bgcolor='#ffffff'style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td width='40pt'></td><td>3. Kondisi&nbsp;" + _LRModel.Company_Type + " :</td></tr></table><table bgcolor='#ffffff'><tr><td width='50pt'></td>";
                location_list += "<td><table bgcolor='#ffffff'style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td>a.</td><td style='width:100pt'>Luas Tanah</td><td>:</td><td>" + detlocation.LandArea+"&nbsp;m<sup>2</sup></td></tr><tr><td>b.</td><td>Luas Bangunan</td><td>:</td><td>"+detlocation.BuildingArea+"&nbsp;m<sup>2</sup></td></tr><tr><td>c.</td><td>Status kepemilikan</td><td>:</td><td>"+detlocation.OwnershipStatus+"</td></tr></table></td></tr></table><br/>";
                

                mnfcity_list += detlocation.MnfCity;
                mnfaddr_list += detlocation.MnfAddr + " " + detlocation.MnfCity;
                if (idx_bc < _LRModel.LicenseRequestBoundCondition.Count)
                {
                    mnfcity_list += ", ";
                    mnfaddr_list += ", ";
                }

                idx_bc++;
            }
            
            parameters.Add("MANUFACTURE_INFO", location_list);

            string prodtype_info = "";

            prodtype_info += "<table bgcolor='#ffffff'><tr><td width='10pt'></td><td><table bgcolor='#ffffff' style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td></td><td></td></tr>";
            int ptidx = 0;
            string[] ptnumerator = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            
            foreach(var prodtype in _LRModel.LicenseRequestProductType)
            {
                    prodtype_info += "<tr><td>" + ptnumerator[ptidx] + ".</td><td>" + prodtype.ProdAlias + "</td></tr>";
                    ptidx++;
            }
            prodtype_info += "</table></td></tr></table>";

            parameters.Add("PROD_TYPE_LIST", prodtype_info);

            string otherprodtype_info = "";
            otherprodtype_info += "<table bgcolor='#ffffff'><tr><td width='10pt'></td><td><table bgcolor='#ffffff' style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td></td><td></td></tr>";
            int otheridx = 0;

            foreach (var otherprodtype in _LRModel.LicenseRequestOtherProductType)
            {

                otherprodtype_info += "<tr><td>" + ptnumerator[otheridx] + ".</td><td>" + otherprodtype.OtherProdCode + "</td></tr>";
                otheridx++;
            }

            otherprodtype_info += "</table></td></tr></table>";


            parameters.Add("OTHERPRODTYPE_LIST", otherprodtype_info);

            string supportdoc_info = "";

            supportdoc_info += "<table bgcolor='#ffffff'><tr><td width='10pt'></td><td><table bgcolor='#ffffff' style='font-family:Arial,Calibri,sans-serif;font-size:10pt;'><tr><td></td><td></td></tr>";
            int sdidx = 0;

            foreach(var supportdoc in _LRModel.LicenseSupportingDocumentMaster )
            {
                supportdoc_info += "<tr><td>" + ptnumerator[sdidx] + ".</td><td>" + supportdoc.Name + "</td></tr>";
                sdidx++;
            }

            supportdoc_info += "</table></td></tr></table>";
            
            parameters.Add("SUPPORTDOC_LIST", supportdoc_info);

            parameters.Add("MNFCITY_LIST", mnfcity_list);
            parameters.Add("MNFADDR_LIST", mnfaddr_list);


            var filesupload = service.GetFileUploadByLRId(_LRModel.MnfRequestID);

            //var LampiranCount = filesupload.Count();
            //var lampirancount_info = LampiranCount.ToString() ?? "0";

            var LampiranCount = filesupload.Where(w => w.IS_GOVERNMENT_DOC == false).Count();
            var lampirancount_info = LampiranCount.ToString() == null ? "0" : LampiranCount.ToString();

            parameters.Add("LAMPIRAN_COUNT", lampirancount_info);


            layout = refService.GeneratePrintout("MANUFACTURING_LICENSE_REQUEST_PRINTOUT", parameters, _LRModel.CreatedBy).LAYOUT;

            return layout;
        }

        [HttpPost]
        public ActionResult GeneratePrintout(long MnfRequestID)
        {
            var _LRModel = new LicenseRequestModel();
            _LRModel.MnfRequestID = MnfRequestID;
            var layout = GetPrintout(_LRModel);
            return Json(layout);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdatePrintOutLayout(string NewPrintout, string CreatedBy)
        {
            var ErrMessage = refService.UpdatePrintoutLayout("MANUFACTURING_LICENSE_REQUEST_PRINTOUT", NewPrintout, CreatedBy);
            return Json(ErrMessage);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void DownloadPrintOut(LicenseRequestModel _LRModel)
        {
            try
            {
                long InterviewID = _LRModel.MnfRequestID ;
                string FormNumber = _LRModel.MnfFormNum;
                FormNumber = FormNumber.Replace('/', '-');
                var now = DateTime.Now.ToString("ddMMyyyy");
                _LRModel.MnfRequestID = InterviewID;
                var htmlText = GetPrintout(_LRModel);
                //MemoryStream ms = new MemoryStream();
                var baseFolder = "/files_upload/Manufacture/LicenseRequest/PrintOut/";
                var uploadToFolder = Server.MapPath("~" + baseFolder);
                Directory.CreateDirectory(uploadToFolder);
                uploadToFolder += "PrintOut_LicenseRequest_" + FormNumber + "_" + now + ".pdf";
                var leftMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var rightMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var topMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var bottomtMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, leftMargin, rightMargin, topMargin, bottomtMargin);
                FileStream stream = new FileStream(uploadToFolder, FileMode.Create);
                var writer = PdfWriter.GetInstance(document, stream);
                if (_LRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Draft)) || _LRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Edited)) || _LRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Canceled)) || _LRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval)))
                //if ((_LRModel.Status != "COMPLETED") && (_LRModel.Status != "CANCELED") && (_LRModel.Status != "WAITING FOR GOVERNMENT APPROVAL" ) && (_LRModel.Status != "WAITING FOR POA SKEP APPROVAL"))
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
                //var newFile = new FileInfo(uploadToFolder);
                //var fileName = Path.GetFileName(uploadToFolder);
                var mergeFile = MergePrintout(uploadToFolder, InterviewID);
                var newFile = new FileInfo(mergeFile);
                var fileName = Path.GetFileName(mergeFile);
                string attachment = string.Format("attachment; filename={0}", fileName);
                Response.Clear();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/pdf";
                Response.WriteFile(newFile.FullName);
                Response.Flush();
                newFile.Delete();
                Response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region Summary Reports

        public ActionResult SummaryReport()
        {

            LicenseRequestSummaryReportsViewModel model;
            try
            {

                model = new LicenseRequestSummaryReportsViewModel();

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new LicenseRequestSummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.LicenseRequest;
                model.CurrentMenu = PageInfo;
            }

            return View("SummaryReport", model);
        }

        private LicenseRequestSummaryReportsViewModel InitSummaryReports(LicenseRequestSummaryReportsViewModel model)
        {
            model.MainMenu = mainMenu;
            model.CurrentMenu = PageInfo;
            var users = refService.GetAllUser();
            var documents = service.GetvwLicenseRequestAll().Select(s => new vwMLLicenseRequestModel
            {
                FormNumber = s.MNF_FORM_NUMBER,
                RequestDate = s.REQUEST_DATE,
                CreatedBy = s.CREATED_BY,
                CompanyName = s.COMPANY_NAME,
                CompanyType = s.COMPANY_TYPE,
                LastApprovedStatusValue = s.LASTAPPROVED_STATUS_VALUE,
                KPPBC = s.KPPBC
            }).ToList();

            var iridlist = service.GetIRIDAll();
            var statusidlist = service.GetStatusIDAll();
            var nppbkcList = service.GetAllNPPBKCId();
            var comptplist = interService.GetInterviewReqCompanyTypeList();

            var filter = new LicenseRequestSearchSummaryReportsViewModel();
            model.SearchView.FormNumberList = GetFormNumList(interService.GetAll().Where(w => iridlist.Contains(w.VR_FORM_ID)));
            model.SearchView.LastApprovedStatusList = GetLastApprovedStatus(service.GetReffValueAll().Where(w => statusidlist.Contains(w.REFF_ID) && w.REFF_KEYS != "COMPLETED" && w.REFF_KEYS != "CANCELED"));
            model.SearchView.FormNumberList = GetFormNumList(interService.GetAll().Where(w => iridlist.Contains(w.VR_FORM_ID)));
            model.SearchView.CompanyTypeList = GetCompTypeList(comptplist);
            model.SearchView.KPPBCList = GetKPPBCList(nppbkcList);
            model.LicenseRequestDocuments = documents;

            //model.DetailsList = new List<LicenseRequestSummaryReportsItem>(); //SearchDataSummaryReports(filter);

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterSummaryReports(LicenseRequestSummaryReportsViewModel model)
        {
            //var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

            var documents = service.GetvwLicenseRequestAll().Where(w => (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) : w.CREATED_BY.Equals(CurrentUser.USER_ID)));

            //if (model.SearchView.FormNumberSource != 0)
            //{
            //    documents = documents.Where(w => w.MNF_FORM_NUMBER.Equals(model.SearchView.FormNumberSource));
            //}
            if (model.SearchView.CompanyTypeSource != null)
            {
                documents = documents.Where(w => w.COMPANY_TYPE.Equals(model.SearchView.CompanyTypeSource));
            }
            if (model.SearchView.KPPBCSource != null)
            {
                documents = documents.Where(w => w.KPPBC.Equals(model.SearchView.KPPBCSource));
            }
            //if (model.SearchView.LastApprovedStatusSource != null)
            //{
            //    documents = documents.Where(w => w.LASTAPPROVED_STATUS.Equals(model.SearchView.LastApprovedStatusSource));
            //}


            var listofDoc = new List<vwMLLicenseRequestModel>();

            listofDoc = documents.Select(s => new vwMLLicenseRequestModel
            {
                FormNumber = s.MNF_FORM_NUMBER,
                RequestDate = s.REQUEST_DATE,
                CreatedBy = s.CREATED_BY,
                CompanyName = s.COMPANY_NAME,
                CompanyType = s.COMPANY_TYPE,
                LastApprovedStatusValue = s.LASTAPPROVED_STATUS_VALUE,
                KPPBC = s.KPPBC
            }).ToList();

            
            model.LicenseRequestDocuments = listofDoc;

            return PartialView("_LicenseRequestTableSummaryReport", model);
        }


        public void ExportXlsSummaryReports(LicenseRequestSummaryReportsViewModel model)
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


        private string CreateXlsSummaryReports(LicenseRequestExportSummaryReportsViewModel modelExport)
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

                if (modelExport.FormNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.FormNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RequestDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestDate.ToString("dd MMMM yyyy"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.CompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.KPPBC)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.KPPBC);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CreatedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CreatedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ModifyBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ModifyBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ModifyDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ModifyDate.Value.ToString("dd MMMM yyyy HH:mm:ss"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastApprovedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastApprovedBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastApprovedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastApprovedDate == null ? "" : data.LastApprovedDate.Value.ToString("dd MMMM yyyy HH:mm:ss"));
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

                if (modelExport.DecreeNo)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DecreeNo);
                    iColumn = iColumn + 1;
                }

                if (modelExport.NppbkcID)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NppbkcID);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.ManufactureAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ManufactureAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.CityName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CityName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.StateName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.StateName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.SubDistrict)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubDistrict);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Village)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Village);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Phone)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Phone);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Fax)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Fax);
                    iColumn = iColumn + 1;
                }


                if (modelExport.DetailExportModel.North)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.North);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.East)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.East);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.South)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.South);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.West)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.West);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.LandArea)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LandArea);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.BuildingArea)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BuildingArea);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.OwnershipStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.OwnershipStatus);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, LicenseRequestExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.FormNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Form Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.RequestDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Request Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.CompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.KPPBC)
            {
                slDocument.SetCellValue(iRow, iColumn, "KPPBC");
                iColumn = iColumn + 1;
            }

            if (modelExport.CreatedBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created By");
                iColumn = iColumn + 1;
            }

            if (modelExport.CreatedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created At");
                iColumn = iColumn + 1;
            }

            if (modelExport.ModifyBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Modified By");
                iColumn = iColumn + 1;
            }

            if (modelExport.ModifyDate)
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

            if (modelExport.DecreeNo)
            {
                slDocument.SetCellValue(iRow, iColumn, "SKEP No.");
                iColumn = iColumn + 1;
            }

            if (modelExport.NppbkcID)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.ManufactureAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.CityName)
            {
                slDocument.SetCellValue(iRow, iColumn, "City");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.StateName)
            {
                slDocument.SetCellValue(iRow, iColumn, "State");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.SubDistrict)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sub District");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Village)
            {
                slDocument.SetCellValue(iRow, iColumn, "Village");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Phone)
            {
                slDocument.SetCellValue(iRow, iColumn, "Phone");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Fax)
            {
                slDocument.SetCellValue(iRow, iColumn, "Fax");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.North)
            {
                slDocument.SetCellValue(iRow, iColumn, "North");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.East)
            {
                slDocument.SetCellValue(iRow, iColumn, "East");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.South)
            {
                slDocument.SetCellValue(iRow, iColumn, "South");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.West)
            {
                slDocument.SetCellValue(iRow, iColumn, "West");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.LandArea)
            {
                slDocument.SetCellValue(iRow, iColumn, "Land Area");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.BuildingArea)
            {
                slDocument.SetCellValue(iRow, iColumn, "Building Area");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.OwnershipStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Ownership Status");
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


        public List<vwMLLicenseRequestModel> GetExportData(LicenseRequestExportSummaryReportsViewModel model)
        {
            //var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

            var documents = service.GetvwLicenseRequestAll().Where(w => (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) : w.CREATED_BY.Equals(CurrentUser.USER_ID)));

            //if (model.SearchView.FormNumberSource != 0)
            //{
            //    documents = documents.Where(w => w.MNF_FORM_NUMBER.Equals(model.SearchView.FormNumberSource));
            //}
            if (model.CompanyTypeSource != null)
            {
                documents = documents.Where(w => w.COMPANY_TYPE.Equals(model.CompanyTypeSource));
            }
            if (model.KPPBCSource != null)
            {
                documents = documents.Where(w => w.KPPBC.Equals(model.KPPBCSource));
            }
            //if (model.SearchView.LastApprovedStatusSource != null)
            //{
            //    documents = documents.Where(w => w.LASTAPPROVED_STATUS.Equals(model.SearchView.LastApprovedStatusSource));
            //}


            var listofDoc = new List<vwMLLicenseRequestModel>();

            if (model.DetailExportModel.ManufactureAddress ||
                model.DetailExportModel.CityName ||
                model.DetailExportModel.StateName ||
                model.DetailExportModel.SubDistrict ||
                model.DetailExportModel.Village ||
                model.DetailExportModel.Phone ||
                model.DetailExportModel.Fax ||
                model.DetailExportModel.North ||
                model.DetailExportModel.East ||
                model.DetailExportModel.South ||
                model.DetailExportModel.West ||
                model.DetailExportModel.LandArea ||
                model.DetailExportModel.BuildingArea ||
                model.DetailExportModel.OwnershipStatus)
            {

                listofDoc = documents.Select(s => new vwMLLicenseRequestModel
                {
                    FormNumber = s.MNF_FORM_NUMBER,
                    RequestDate = s.REQUEST_DATE,
                    CompanyType = s.COMPANY_TYPE,
                    CreatedBy = s.CREATED_BY,
                    CreatedDate = s.CREATED_DATE,
                    ModifyBy = s.LASTMODIFIED_BY,
                    ModifyDate = s.LASTMODIFIED_DATE,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    LastApprovedDate = s.LASTAPPROVED_DATE,
                    LastApprovedStatus = s.LASTAPPROVED_STATUS,
                    LastApprovedStatusValue = s.LASTAPPROVED_STATUS_VALUE,
                    DecreeNo = s.DECREE_NO == null ? "" : s.DECREE_NO,
                    DecreeDate = s.DECREE_DATE,
                    DecreeStatus = s.DECREE_STATUS,
                    NppbkcID = s.NPPBKC_ID,
                    KPPBC = s.KPPBC,
                    CompanyName = s.COMPANY_NAME,
                    ManufactureAddress = s.MANUFACTURE_ADDRESS,
                    CityName = s.CITY_NAME,
                    StateName = s.STATE_NAME,
                    SubDistrict = s.SUB_DISTRICT,
                    Village = s.VILLAGE,
                    Phone = s.PHONE,
                    Fax = s.FAX,
                    North = s.NORTH,
                    East = s.EAST,
                    South = s.SOUTH,
                    West = s.WEST,
                    LandArea = s.LAND_AREA == null ? "0" : s.LAND_AREA.Value.ToString(),
                    BuildingArea = s.BUILDING_AREA == null ? "0" : s.BUILDING_AREA.Value.ToString(),
                    OwnershipStatus = s.OWNERSHIP_STATUS
                }).ToList();

            }
            else
            {
                listofDoc = documents.Select(s => new vwMLLicenseRequestModel
                {
                    FormNumber = s.MNF_FORM_NUMBER,
                    RequestDate = s.REQUEST_DATE,
                    CompanyType = s.COMPANY_TYPE,
                    CreatedBy = s.CREATED_BY,
                    CreatedDate = s.CREATED_DATE,
                    ModifyBy = s.LASTMODIFIED_BY,
                    ModifyDate = s.LASTMODIFIED_DATE,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    LastApprovedDate = s.LASTAPPROVED_DATE,
                    LastApprovedStatus = s.LASTAPPROVED_STATUS,
                    LastApprovedStatusValue = s.LASTAPPROVED_STATUS_VALUE,
                    DecreeNo = s.DECREE_NO == null ? "" : s.DECREE_NO,
                    DecreeDate = s.DECREE_DATE,
                    DecreeStatus = s.DECREE_STATUS,
                    NppbkcID = s.NPPBKC_ID,
                    KPPBC = s.KPPBC,
                    CompanyName = s.COMPANY_NAME
                }).Distinct().ToList();

            }



            return listofDoc;
        }


        #endregion

        [HttpPost]
        public ActionResult RestorePrintoutToDefault()
        {
            var ErrMessage = refService.RestorePrintoutToDefault("MANUFACTURING_LICENSE_REQUEST_PRINTOUT", CurrentUser.USER_ID);
            return Json(ErrMessage);
        }

        private List<ConfirmDialogModel> GenerateConfirmDialog()
        {
            try
            {
                var listconfirmation = new List<ConfirmDialogModel>();
                
                //// FOR SET PRINTOUT TO DEFAULT CONFIRMATION ////                
                listconfirmation.Add(new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "btnRestorePrintoutToDefault",
                        CssClass = "btn btn-success btn_loader",
                        Label = "Restore To Default"
                    },
                    CssClass = " restoredefault-modal licenserequest",
                    Message = "You are going to restore printout layout to default. Are you sure?",
                    Title = "Restore Printout Confirmation",
                    ModalLabel = "RestoreModalLabel"
                });
                //////////////////////////////////////////////////

                return listconfirmation;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<ConfirmDialogModel>();
            }
        }

        private bool IsPOAfromSameNPPBKC(long LRID)
        {
            var yes = true;
            var list = service.GetLicenseNeedApproveWithSameNPPBKC(CurrentUser.USER_ID).Where(w => w == LRID);
            if (list.Count() == 0 || list == null)
            {
                yes = false;
            }
            return yes;
        }

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
            filename = filename.Replace("=MLR=", "/");
            var arrfilename = filename.Split('/');
            if (arrfilename.Count() > 0)
            {
                filename = arrfilename[0] + "." + fileext;
            }
            return filename;
        }

        public String MergePrintout(string path, long MnfRegID)
        {
            try
            {
                var supportingDocs = refService.GetUploadedFiles((int)Enums.FormList.License, MnfRegID.ToString()).Where(x => x.DOCUMENT_ID != null).ToList();
                var ext = "";
                List<String> sourcePaths = new List<string>();
                sourcePaths.Add(path);
                foreach (var doc in supportingDocs)
                {
                    ext = doc.PATH_URL.Substring((doc.PATH_URL.Length - 3), 3);
                    if (ext.ToLower() == "pdf")
                    {
                        sourcePaths.Add(Server.MapPath("~" + doc.PATH_URL));
                    }
                }

                var otherDocs = refService.GetUploadedFiles((int)Enums.FormList.License, MnfRegID.ToString()).Where(x => x.DOCUMENT_ID == null && x.IS_GOVERNMENT_DOC == false).ToList();
                foreach (var doc in otherDocs)
                {                    
                    ext = doc.PATH_URL.Substring((doc.PATH_URL.Length - 3), 3);
                    if (ext.ToLower() == "pdf")
                    {
                        sourcePaths.Add(Server.MapPath("~" + doc.PATH_URL));
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
    }
}
