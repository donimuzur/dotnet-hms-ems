using AutoMapper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangeRequest;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Helpers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;


namespace Sampoerna.EMS.Website.Controllers
{
    public class MLChangeRequestController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        private ChangeRequestService service;
        private IChangesHistoryBLL chBLL;
        private IWorkflowHistoryBLL whBLL;
        private ChangeRequestModel CRmodel;
        private ChangeRequestViewModel CRViewmodel;
        private ChangeRequestFormModel CRFormmodel;

        public MLChangeRequestController(IPageBLL pageBLL, IChangesHistoryBLL changeHistoryBLL, IWorkflowHistoryBLL workflowHistoryBLL) : base(pageBLL, Enums.MenuList.ManufactureLicense)
        {
            this.mainMenu = Enums.MenuList.ManufactureLicense;
            this.service = new ChangeRequestService();
            this.refService = new SystemReferenceService();
            CRmodel = new ChangeRequestModel();

            //this.chBLL = changeHistoryBLL;
            //this.whBLL = workflowHistoryBLL;
        }

        public ActionResult Index()
        {
            try
            {

                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {

                    var users = refService.GetAllUser();
                    var poaList = refService.GetAllPOA();
                    var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);
                    //var documents = new REPLACEMENT_DOCUMENTS();
                    var documents = new List<ChangeRequestModel>();
                    var lists = service.GetAll().Where(w => ((w.LASTAPPROVED_STATUS != refService.GetRefByKey("COMPLETED").REFF_ID) && (w.LASTAPPROVED_STATUS != refService.GetRefByKey("CANCELED").REFF_ID)));
                    if (lists.Any())
                    {
                        switch (CurrentUser.UserRole)
                        {
                            case Enums.UserRole.Administrator:
                                break;

                            case Enums.UserRole.POA:
                                //lists = lists.Where(w => (nppbkclist.Contains(w.NPPBKC_ID)) ||
                                //                          (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) :
                                //                                                        w.CREATED_BY.Equals(CurrentUser.USER_ID))
                                //                   );

                                //lists = lists.Where(w => ((w.LASTAPPROVED_STATUS == refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID) || (w.LASTAPPROVED_STATUS == refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID)) ? w.CREATED_BY.Equals(CurrentUser.USER_ID) : 1 == 1);
                                //lists = lists.Where(w => ((w.LASTAPPROVED_STATUS == refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID) ? ((w.CREATED_BY.Equals(CurrentUser.USER_ID)) || (w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID))) : 1 == 1));
                                break;

                            case Enums.UserRole.Viewer:
                                break;
                        }
                    }
                    if (lists.Any())
                    {
                        documents = lists.OrderByDescending(o => o.REQUEST_DATE).Select(s => new ChangeRequestModel
                        {
                            Id = s.FORM_ID,
                            DocumentNumber = s.FORM_NO,
                            RequestDate = s.REQUEST_DATE,
                            strRequestDate = s.REQUEST_DATE.ToString("dd MMMM yyyy"),
                            DocumentType = s.DOCUMENT_TYPE,
                            NppbkcId = s.NPPBKC_ID,
                            CreatedBy = s.CREATED_BY,
                            KPPBCAddress = service.GetNppbkc(s.NPPBKC_ID).KPPBC_ADDRESS,
                            LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                            LastModifiedDate = Convert.ToDateTime(s.LASTMODIFIED_DATE),
                            IsApprover = IsPOACanApprove(s.NPPBKC_ID, s.CREATED_BY, s.LASTAPPROVED_BY == null ? "" : s.LASTAPPROVED_BY, s.FORM_ID),
                            IsCreator = (s.CREATED_BY == CurrentUser.USER_ID) ? true : false,
                            IsViewer = IsPOACanView(s.NPPBKC_ID, s.FORM_ID)
                        }).ToList();
                    }

                    CRViewmodel = new ChangeRequestViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        FilterInput = new ChangeRequestFilterModel(),
                        CreatorList = GetUserList(users),
                        ChangeRequestDocuments = documents,
                        NppbkcList = GetNppbkcListByUser(nppbkclist),
                        PoaList = GetPoaList(refService.GetAllPOA()),
                        DocumentTypeList = GetDocumentTypeList(service.GetDocumentTypes()),
                        YearList = GetYearList(documents),
                        IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.POA && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator),
                        CurrentRole = CurrentUser.UserRole

                    };

                    CRViewmodel.FilterInput.LastApprovedStatus = 0;

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

            return View(CRViewmodel);
        }

        public ActionResult CompletedDocument()
        {
            try
            {

                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {

                    var users = refService.GetAllUser();
                    var poaList = refService.GetAllPOA();
                    var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

                    var documents = new List<ChangeRequestModel>();

                    //var lists = service.GetAll().Where(w => (w.LASTAPPROVED_STATUS == refService.GetRefByKey("COMPLETED").REFF_ID || w.LASTAPPROVED_STATUS == refService.GetRefByKey("CANCELED").REFF_ID) && (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) : w.CREATED_BY.Equals(CurrentUser.USER_ID)));
                    var lists = service.GetAll().Where(w => (w.LASTAPPROVED_STATUS == refService.GetRefByKey("COMPLETED").REFF_ID || w.LASTAPPROVED_STATUS == refService.GetRefByKey("CANCELED").REFF_ID));
                    if (lists.Any())
                    {
                        documents = lists.Select(s => new ChangeRequestModel
                        {
                            Id = s.FORM_ID,
                            DocumentNumber = s.FORM_NO,
                            RequestDate = s.REQUEST_DATE,
                            DocumentType = s.DOCUMENT_TYPE,
                            strRequestDate = s.REQUEST_DATE.ToString("dd MMMM yyyy"),
                            //RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                            NppbkcId = s.NPPBKC_ID,
                            CreatedBy = s.CREATED_BY,
                            KPPBCAddress = service.GetNppbkc(s.NPPBKC_ID).KPPBC_ADDRESS,
                            LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                            LastModifiedDate = Convert.ToDateTime(s.LASTMODIFIED_DATE),
                            IsApprover = IsPOACanApprove(s.NPPBKC_ID, s.CREATED_BY, s.LASTAPPROVED_BY == null ? "" : s.LASTAPPROVED_BY, s.FORM_ID),
                            IsCreator = (s.CREATED_BY == CurrentUser.USER_ID) ? true : false,

                        }).ToList();
                    }

                    CRViewmodel = new ChangeRequestViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        FilterInput = new ChangeRequestFilterModel(),
                        CreatorList = GetUserList(users),
                        ChangeRequestDocuments = documents,
                        NppbkcList = GetNppbkcListByUser(nppbkclist),
                        PoaList = GetPoaList(refService.GetAllPOA()),
                        DocumentTypeList = GetDocumentTypeList(service.GetDocumentTypes()),
                        YearList = GetYearList(documents),
                        IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Controller && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator)
                    };

                    CRViewmodel.FilterInput.LastApprovedStatus = (int)refService.GetRefByKey("COMPLETED").REFF_ID;

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

            return View("Index", CRViewmodel);

        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(ChangeRequestViewModel model)
        {
            var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

            var documents = service.GetAll().Where(w => (nppbkclist.Contains(w.NPPBKC_ID)) || (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) : w.CREATED_BY.Equals(CurrentUser.USER_ID)));

            if (model.FilterInput.LastApprovedStatus != 0)
            {
                documents = documents.Where(w => w.LASTAPPROVED_STATUS == model.FilterInput.LastApprovedStatus);
            }
            else
            {
                documents = documents.Where(w => w.LASTAPPROVED_STATUS != refService.GetRefByKey("COMPLETED").REFF_ID && w.LASTAPPROVED_STATUS != refService.GetRefByKey("CANCELED").REFF_ID);
            }



            if (model.FilterInput.NPPBKC != null)
            {
                documents = documents.Where(w => w.NPPBKC_ID == model.FilterInput.NPPBKC);
            }
            else
            {
                documents = documents.Where(w => nppbkclist.Contains(w.NPPBKC_ID));
            }

            if (model.FilterInput.DocumentType != null)
            {
                documents = documents.Where(w => w.DOCUMENT_TYPE == model.FilterInput.DocumentType);
            }

            if (model.FilterInput.Creator != null)
            {
                documents = documents.Where(w => w.CREATED_BY == model.FilterInput.Creator);
            }

            var listofDoc = new List<ChangeRequestModel>();

            if (documents.Any())
            {
                listofDoc = documents.Select(s => new ChangeRequestModel
                {
                    Id = s.FORM_ID,
                    DocumentNumber = s.FORM_NO,
                    RequestDate = s.REQUEST_DATE,
                    DocumentType = s.DOCUMENT_TYPE,
                    //RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                    NppbkcId = s.NPPBKC_ID,
                    CreatedBy = s.CREATED_BY,
                    KPPBCAddress = service.GetNppbkc(s.NPPBKC_ID).KPPBC_ADDRESS,
                    LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                    LastModifiedDate = Convert.ToDateTime(s.LASTMODIFIED_DATE)
                }).ToList();

            }


            model.ChangeRequestDocuments = listofDoc;

            return PartialView("_ChangeRequestTable", model);
        }

        public ActionResult ChangeLog(int CRID, string Token)
        {
            var changeRequest = new ChangeRequestModel();

            var history = refService.GetChangesHistory((int)Enums.MenuList.ChangeRequest, CRID.ToString()).ToList();
            changeRequest.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);

            return PartialView("_ChangesHistoryTable", changeRequest);
        }

        #region Create/Details/Edit/Approve
        public ActionResult Create()
        {
            try
            {
                if (CurrentUser.UserRole != Enums.UserRole.POA)
                {
                    AddMessageInfo("Can't create new Manufacturing License Interview Request Document for User with " + EnumHelper.GetDescription(CurrentUser.UserRole) + " Role", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                else
                {
                    var model = GenerateModelProperties(null);
                    model.Id = 0;
                    model.LastApprovedStatus = "DRAFT NEW";
                    model.ListOfUpdateNotes = null;
                    model.Confirmation = GenerateConfirmDialog(true, false, false);
                    return View("Create", model);
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return View("Index");
            }
        }

        public ActionResult Details(Int64 Id = 0)
        {
            //if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(Id, CurrentUser.USER_ID))
            //{

            CRmodel = new ChangeRequestModel();
            CRmodel = GetChangeRequestDetail(Id, "Detail");
            CRmodel.Confirmation = GenerateConfirmDialog(false, false, false);
            return View("Create", CRmodel);
            //}
            //else
            //{
            //    AddMessageInfo("You dont have access to view Manufacturing License Request detail.", Enums.MessageInfoType.Warning);
            //    return RedirectToAction("Index");
            //}

        }

        public ActionResult Edit(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(Id, CurrentUser.USER_ID))
            {

                CRmodel = new ChangeRequestModel();
                CRmodel = GetChangeRequestDetail(Id, "Edit");
                CRmodel.Confirmation = GenerateConfirmDialog(true, true, false);

                return View("Create", CRmodel);
            }
            else
            {
                AddMessageInfo("You dont have access to edit this Manufacturing License Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Approve(Int64 Id = 0)
        {
            CRmodel = new ChangeRequestModel();
            CRmodel = GetChangeRequestDetail(Id, "Approve");
            CRmodel.Confirmation = GenerateConfirmDialog(false, false, true);


            if ((CRmodel.CreatedBy != CurrentUser.USER_ID) && (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanApprove(CRmodel.NppbkcId, CRmodel.CreatedBy, CRmodel.LastApprovedBy == null ? "" : CRmodel.LastApprovedBy, Id)))
            {
                return View("Create", CRmodel);
            }
            else
            {
                AddMessageInfo("You dont have access to approve this Manufacturing License Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(ChangeRequestModel model)
        {
            try
            {
                var updated_list = "";

                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                //var obj = model;
                //obj.CreatedBy = CurrentUser.USER_ID;
                //obj.CreatedDate = DateTime.Now;
                //obj.LastModifiedBy = CurrentUser.USER_ID;
                //obj.LastModifiedDate = DateTime.Now;
                //model = obj;

                var maxFileSize = GetMaxFileSize();
                var isOkFileExt = true;
                var isOkFileSize = true;
                var supportingDocFile = new List<HttpPostedFileBase>();
                if (model.changeRequestSupportingDoc != null)
                {
                    supportingDocFile = model.changeRequestSupportingDoc.Select(s => s.File).ToList();
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
                        if (model.changeRequestSupportingDoc != null)
                        {
                            foreach (var SuppDoc in model.changeRequestSupportingDoc)
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

                        if (model.LastApprovedStatus.ToUpper() == "DRAFT NEW" && model.Id != 0 && model.Id != null)
                        {
                            model.LastApprovedStatus = "DRAFT EDIT";
                        }

                        var ActionType = 0;
                        if (model.LastApprovedStatus.ToUpper().Contains("DRAFT"))
                        {
                            ActionType = (int)Enums.ActionType.Modified;
                        }
                        else if (model.LastApprovedStatus.ToUpper().Equals("WAITING FOR POA APPROVAL"))
                        {
                            ActionType = (int)Enums.ActionType.Submit;
                        }


                        var data = MapToTable(model);

                        var ActionResult = new REPLACEMENT_DOCUMENTS();
                        if (data.FORM_ID == 0 || data.FORM_ID == null)
                        {
                            ActionResult = service.Save(data, (int)CurrentUser.UserRole);
                        }
                        else
                        {
                            ActionResult = service.Update(data, ActionType, (int)CurrentUser.UserRole);
                        }

                        if (ActionResult != null)
                        {
                            model.Id = ActionResult.FORM_ID;
                            model.DocumentNumber = ActionResult.FORM_NO;
                            if (model.Id != 0)
                            {
                                foreach (var removedfile in model.RemovedFilesId)
                                {
                                    if (removedfile != 0)
                                    {
                                        service.DeleteFileUpload(removedfile, CurrentUser.USER_ID);
                                    }
                                }
                                //// Supporting Doc
                                InsertUploadSuppDocFile(model.changeRequestSupportingDoc, model.Id);
                                //// Other Doc
                                InsertUploadCommonFile(model.File_Other_Path, model.Id, false, model.File_Other_Name);


                                if (model.ListOfUpdateNotes != null)
                                {
                                    //Process Delete ID
                                    var TheDetail = GetChangeRequestDetail(model.Id);
                                    var DeleteDetail = MapToDetail(TheDetail);
                                    var TheDeleteDetail = DeleteDetail.Where(w => model.RemovedDetailId.Contains(w.FORM_DET_ID)).ToList();
                                    service.DeleteChangeRequestDetail(model.Id, CurrentUser.USER_ID, TheDeleteDetail);

                                    updated_list = "<ul>";
                                    //service.DeleteChangeRequestDetail(model.Id);
                                    foreach (var Note in model.ListOfUpdateNotes)
                                    {
                                        if (Note.IsActive == 1)
                                        {
                                            var ReNewedDetail = DeleteDetail.Where(w => w.FORM_DET_ID == Note.DetailId).FirstOrDefault();
                                            var InsertCRDetail = service.InsertChangeRequestDetail(model.Id, Note.UpdateNotes, ReNewedDetail, CurrentUser.USER_ID);
                                            updated_list += "<li>" + Note + "</li>";
                                        }
                                    }
                                    updated_list += "</ul>";
                                }
                            }


                        }

                        var msgSuccess = "";
                        if (model.LastApprovedStatus.ToUpper() == "DRAFT NEW")
                        {
                            msgSuccess = "Success create Change Request";
                        }
                        else if (model.LastApprovedStatus.ToUpper() == "DRAFT EDIT")
                        {
                            msgSuccess = "Success update Change Request";
                        }

                        //else if (model.LastApprovedStatus.ToUpper() == "WAITING FOR POA APPROVAL")
                        //{
                        //    msgSuccess = "Success submit Change Request";
                        //    var poareceiverlistall = service.GetPOAApproverList(model.NppbkcId, model.CreatedBy);
                        //    if (poareceiverlistall.Count() > 0)
                        //    {
                        //        List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();

                        //        var nppbkc_data = service.GetNppbkc(model.NppbkcId);
                        //        var mapped_nppbkc = MapNppbkcModel(nppbkc_data);

                        //        var strreqdate = model.RequestDate.ToString("dd MMMM yyyy");
                        //        var CreatorName = refService.GetPOA(CurrentUser.USER_ID).PRINTED_NAME;
                        //        var sendmail = SendMail(model.DocumentNumber,  strreqdate, model.DocumentType, model.NppbkcId, model.NPPBKC.KppbcId, mapped_nppbkc.Address, mapped_nppbkc.Region, mapped_nppbkc.City, updated_list, CreatorName, model.LastApprovedStatus, "", strreqdate, "", model.Id, poareceiverList, "submit");
                        //        if (!sendmail)
                        //        {
                        //            msgSuccess += " , but failed send mail to POA Approver";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        msgSuccess += " , but failed send mail to POA Approver";
                        //    }
                        //}

                        AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success
                        );
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddMessageInfo("Maximum file size is " + maxFileSize.ToString() + " Mb", Enums.MessageInfoType.Warning);
                        return View();
                    }
                }
                else
                {
                    AddMessageInfo("Wrong File Extension", Enums.MessageInfoType.Warning);
                    return View();
                }
                
            }
            catch (Exception ex)
            {
                AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            //model = GenerateModelProperties(model);
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitSkep(ChangeRequestModel model)
        {
            try
            {
                model.LastApprovedStatus = "WAITING FOR POA SKEP APPROVAL";
                var maxFileSize = GetMaxFileSize();
                var isOkFileExt = true;
                var isOkFileSize = true;
                var msgSuccess = "Success submit Change Request";

                isOkFileExt = CheckFileExtension(model.File_BA);
                if (isOkFileExt)
                {
                    isOkFileSize = CheckFileSize(model.File_BA, maxFileSize);
                    if (isOkFileSize)
                    {
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
                        long ApproveStats = GetSysreffApprovalStatus(model.LastApprovedStatus);
                        if (model.Id != 0 && model.Id != null)
                        {
                            var ActionType = 0;
                            if (model.LastApprovedStatus.ToUpper().Contains("SKEP"))
                            {
                                ActionType = (int)Enums.ActionType.Submit;
                            }
                            else if (model.LastApprovedStatus.ToUpper().Contains("REJECTED"))
                            {
                                ActionType = (int)Enums.ActionType.Reject;
                            }
                            var updateSKEP = service.UpdateBASKEP(model.Id, Convert.ToBoolean(model.DecreeStatus), model.DecreeNumber, DateTime.Now, ApproveStats, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, "");

                            var dataChangeRequest = GetChangeRequestMasterForm(model.Id);
                            var nppbkc_data = service.GetNppbkc(dataChangeRequest.NppbkcId);
                            var mapped_nppbkc = MapNppbkcModel(nppbkc_data);

                            var document_details = service.GetDocumentDetails(dataChangeRequest.Id);

                            var updated_list = "";
                            if (document_details != null)
                            {
                                updated_list = "<ul>";
                                foreach (var Detail in document_details)
                                {
                                    updated_list += "<li>" + Detail.UPDATE_NOTES + "</li>";
                                }
                                updated_list += "</ul>";

                            }

                            if (updateSKEP != null)
                            {
                                InsertUploadCommonFile(model.File_BA_Path, model.Id, true, model.File_Other_Name);
                                //var poareceiverlistall = service.GetPOAApproverList(model.NppbkcId, model.CreatedBy);
                                var poareceiverlistall = service.GetPOAApproverList(model.Id);
                                if (poareceiverlistall.Count() > 0)
                                {
                                    List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();
                                    var strreqdate = updateSKEP.REQUEST_DATE.ToString("dd MMMM yyyy");
                                    var lastapproval_date = Convert.ToDateTime(updateSKEP.LASTAPPROVED_DATE).ToString("dd MMMM yyyy");
                                    var CreatorName = refService.GetPOA(CurrentUser.USER_ID).PRINTED_NAME;

                                    var sendmail = SendMail(updateSKEP.FORM_NO, strreqdate, updateSKEP.DOCUMENT_TYPE, updateSKEP.NPPBKC_ID, mapped_nppbkc.KppbcId, mapped_nppbkc.Address, mapped_nppbkc.Region, mapped_nppbkc.City, updated_list, CreatorName, dataChangeRequest.LastApprovedStatus, "", strreqdate, "", dataChangeRequest.Id, poareceiverList, "approve");
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


        #endregion

        #region Summary Reports

        public ActionResult SummaryReport()
        {

            ChangeRequestSummaryReportsViewModel model;
            try
            {

                model = new ChangeRequestSummaryReportsViewModel();

                //model.ChangeRequestType = Enums.ChangeRequestType.Domestic;

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new ChangeRequestSummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.ChangeRequest;
                model.CurrentMenu = PageInfo;
            }

            return View("SummaryReport", model);
        }

        private ChangeRequestSummaryReportsViewModel InitSummaryReports(ChangeRequestSummaryReportsViewModel model)
        {
            model.MainMenu = mainMenu;
            model.CurrentMenu = PageInfo;
            var users = refService.GetAllUser();
            var documents = service.GetAll().Select(s => new ChangeRequestModel
            {
                Id = s.FORM_ID,
                DocumentNumber = s.FORM_NO,
                RequestDate = s.REQUEST_DATE,
                DocumentType = s.DOCUMENT_TYPE,
                //RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                NppbkcId = s.NPPBKC_ID,
                CreatedBy = s.CREATED_BY,
                KPPBCAddress = service.GetNppbkc(s.NPPBKC_ID).KPPBC_ADDRESS,
                LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                LastModifiedDate = Convert.ToDateTime(s.LASTMODIFIED_DATE)

            }).ToList();

            //var input = new ChangeRequestGetSummaryReportByParamInput();
            //input.UserId = CurrentUser.USER_ID;
            //input.UserRole = CurrentUser.UserRole;
            //input.ListUserPlant = CurrentUser.ListUserPlants;

            var filter = new ChangeRequestSearchSummaryReportsViewModel();
            model.SearchView.NppbkcList = GetNppbkcList(refService.GetAllNppbkc());
            model.SearchView.PoaList = GetPoaList(refService.GetAllPOA());
            model.SearchView.DocumentTypeList = GetDocumentTypeList(service.GetDocumentTypes());
            model.SearchView.CreatorList = GetUserList(users);
            model.SearchView.YearList = GetYearList(documents);
            model.ChangeRequestDocuments = documents;



            model.DetailsList = new List<ChangeRequestSummaryReportsItem>(); //SearchDataSummaryReports(filter);

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterSummaryReports(ChangeRequestSummaryReportsViewModel model)
        {
            var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

            var documents = service.GetAll().Where(w => (nppbkclist.Contains(w.NPPBKC_ID)) || (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) : w.CREATED_BY.Equals(CurrentUser.USER_ID)));

            if (model.SearchView.DocumentTypeSource != null)
            {
                documents = documents.Where(w => w.DOCUMENT_TYPE == model.SearchView.DocumentTypeSource);
            }


            var listofDoc = new List<ChangeRequestModel>();

            if (documents.Any())
            {
                listofDoc = documents.Select(s => new ChangeRequestModel
                {
                    Id = s.FORM_ID,
                    DocumentNumber = s.FORM_NO,
                    RequestDate = s.REQUEST_DATE,
                    DocumentType = s.DOCUMENT_TYPE,
                    //RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                    NppbkcId = s.NPPBKC_ID,
                    CreatedBy = s.CREATED_BY,
                    KPPBCAddress = service.GetNppbkc(s.NPPBKC_ID).KPPBC_ADDRESS,
                    LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                    LastModifiedDate = Convert.ToDateTime(s.LASTMODIFIED_DATE)
                }).ToList();

            }


            model.ChangeRequestDocuments = listofDoc;

            return PartialView("_ChangeRequestTableSummaryReport", model);
        }


        public void ExportXlsSummaryReports(ChangeRequestSummaryReportsViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryReports(model.ExportModel);
            
            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);// "MLChangerRequest" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }


        private string CreateXlsSummaryReports(ChangeRequestExportSummaryReportsViewModel modelExport)
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

                if (modelExport.FormNo)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DocumentNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RequestDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestDate.ToString("dd MMMM yyyy"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.DocumentType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DocumentType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Nppbkc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NppbkcId);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Creator)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CreatorDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedDate.ToString("dd MMMM yyyy HH:mm:ss"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastModifiedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastModifiedBy);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LastModifiedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastModifiedDate.ToString("dd MMMM yyyy HH:mm:ss"));
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
                    slDocument.SetCellValue(iRow, iColumn, data.LastApprovedStatus);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DecreeStatus == true ? "Approved" : "Reject");
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DecreeNumber);
                    iColumn = iColumn + 1;
                }


                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, ChangeRequestExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.FormNo)
            {
                slDocument.SetCellValue(iRow, iColumn, "Form Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.RequestDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Request Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.DocumentType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Document Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.Nppbkc)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC");
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


            var fileName = "MLChangeRequest" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.MLFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }


        public List<ChangeRequestModel> GetExportData(ChangeRequestExportSummaryReportsViewModel modelExport)
        {
            var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

            var documents = service.GetAll().Where(w => (nppbkclist.Contains(w.NPPBKC_ID)) || (w.LASTAPPROVED_BY != null ? (w.CREATED_BY.Equals(CurrentUser.USER_ID) || w.LASTAPPROVED_BY.Equals(CurrentUser.USER_ID)) : w.CREATED_BY.Equals(CurrentUser.USER_ID)));

            if (modelExport.DocumentTypeSource != null)
            {
                documents = documents.Where(w => w.DOCUMENT_TYPE == modelExport.DocumentTypeSource);
            }


            //if (model.FilterInput.NPPBKC != null)
            //{
            //    documents = documents.Where(w => w.NPPBKC_ID == model.FilterInput.NPPBKC);
            //}

            //if (model.FilterInput.Creator != null)
            //{
            //    documents = documents.Where(w => w.CREATED_BY == model.FilterInput.Creator);
            //}

            var listofDoc = new List<ChangeRequestModel>();

            if (documents.Any())
            {
                listofDoc = documents.Select(s => new ChangeRequestModel
                {
                    Id = s.FORM_ID,
                    DocumentNumber = s.FORM_NO,
                    RequestDate = s.REQUEST_DATE,
                    DocumentType = s.DOCUMENT_TYPE,
                    //RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                    NppbkcId = s.NPPBKC_ID,
                    CreatedBy = s.CREATED_BY,
                    CreatedDate = s.CREATED_DATE,
                    KPPBCAddress = service.GetNppbkc(s.NPPBKC_ID).KPPBC_ADDRESS,
                    LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                    LastModifiedDate = Convert.ToDateTime(s.LASTMODIFIED_DATE),
                    LastModifiedBy = s.LASTMODIFIED_BY,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    LastApprovedDate = s.LASTAPPROVED_DATE,
                    DecreeStatus = s.DECREE_STATUS,
                    DecreeNumber = s.DECREE_NUMBER
                }).ToList();

            }


            return listofDoc;
        }


        #endregion


        #region Ajax Requests
        [HttpPost]
        public JsonResult GetNppbkc(string id)
        {
            try
            {
                var nppbkc = service.GetNppbkc(id);
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
        public ActionResult GetSupportingDocuments(string CompanyId, long CRId, bool IsReadonly)
        {
            var formId = (long)Enums.FormList.Change;
            var docs = refService.GetSupportingDocuments(formId, CompanyId);
            var model = docs.Select(x => MapSupportingDocumentModel(x)).ToList();
            if (CRId != 0 && CRId != null)
            {
                var Doclist = service.GetFileUploadByCRId(CRId);
                if (Doclist != null)
                {
                    Doclist = Doclist.Where(w => w.DOCUMENT_ID != null);
                    if (Doclist != null)
                    {
                        List<ChangeRequestSupportingDocModel> listDoc = Doclist.Select(s => new ChangeRequestSupportingDocModel
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

        public ActionResult GetSupportingDocuments(string CompanyId)
        {
            var formId = (long)Enums.FormList.Change;
            var docs = refService.GetSupportingDocuments(formId, CompanyId);
            var model = docs.Select(x => MapSupportingDocumentModel(x));
            return PartialView("_SupportingDocuments", model);
        }




        //[HttpPost]
        //public JsonResult UploadFiles()
        //{
        //    try
        //    {
        //        foreach (string file in Request.Files)
        //        {
        //            var fileContent = Request.Files[file];
        //            if (fileContent != null && fileContent.ContentLength > 0)
        //            {
        //                // get a stream
        //                var stream = fileContent.InputStream;
        //                // and optionally write the file to disk
        //                var fileName = fileContent.FileName;
        //                var type = System.IO.Path.GetFileName(file);
        //                var docNumber = Request.Form.Get("doc_number").Replace("/", "_");
        //                var path = Server.MapPath("~/files_upload/excise-credit/" + docNumber);
        //                if (!System.IO.Directory.Exists(path))
        //                {
        //                    System.IO.Directory.CreateDirectory(path);
        //                }
        //                path = System.IO.Path.Combine(path, fileName);
        //                using (var fileStream = System.IO.File.Create(path))
        //                {
        //                    stream.CopyTo(fileStream);
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        return Json("Upload failed");
        //    }

        //    return Json("File uploaded successfully");

        //}


        [HttpPost]
        public ActionResult ChangeStatus(long CRId, string Status, string Comment, string Action)
        {
            try
            {
                int ActionType = 0;
                Action = Action.ToLower();
                if (Action == "submit")
                {
                    ActionType = (int)Enums.ActionType.Submit;
                }
                else if (Action == "approve")
                {
                    ActionType = (int)Enums.ActionType.Approve;
                }
                else if (Action == "reject")
                {
                    ActionType = (int)Enums.ActionType.Reject;
                }
                else if ((Action == "revise") || (Action == "finalreject"))
                {
                    ActionType = (int)Enums.ActionType.Revise;
                }
                else if (Action == "cancel")
                {
                    ActionType = (int)Enums.ActionType.Cancel;
                }
                else if (Action == "withdraw")
                {
                    ActionType = (int)Enums.ActionType.Withdraw;
                }



                string ErrMsg = "";
                long ApproveStats = GetSysreffApprovalStatus(Status);
                //var updatedCRModel = GetChangeRequestMasterForm(CRId);
                var update = service.UpdateStatus(CRId, ApproveStats, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, Comment);
                var msgSuccess = "";

                if (update != null)
                {
                    var strreqdate = update.REQUEST_DATE.ToString("dd MMMM yyyy");
                    var strappdate = update.REQUEST_DATE.ToString("dd MMMM yyyy HH:mm");
                    var Creator = refService.GetPOA(update.CREATED_BY);
                    var CreatorName = Creator.PRINTED_NAME;
                    var CreatorMail = Creator.POA_EMAIL;

                    var nppbkc_data = service.GetNppbkc(update.NPPBKC_ID);
                    var mapped_nppbkc = MapNppbkcModel(nppbkc_data);

                    var document_details = service.GetDocumentDetails(update.FORM_ID);

                    var updated_list = "";
                    if (document_details != null)
                    {
                        updated_list = "<ul>";
                        foreach (var Detail in document_details)
                        {
                            updated_list += "<li>" + Detail.UPDATE_NOTES + "</li>";
                        }
                        updated_list += "</ul>";

                    }


                    if (Action == "submit")
                    {
                        msgSuccess = "Success Submit Change Request";

                        //var poareceiverlistall = service.GetPOAApproverList(update.NPPBKC_ID, update.CREATED_BY);
                        var poareceiverlistall = service.GetPOAApproverList(CRId);
                        if (poareceiverlistall.Count() > 0)
                        {
                            List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();

                            //var sendmail = SendMail(model.DocumentNumber, strreqdate, model.DocumentType, model.NppbkcId, model.NPPBKC.KppbcId, mapped_nppbkc.Address, mapped_nppbkc.Region, mapped_nppbkc.City, updated_list, CreatorName, model.LastApprovedStatus, "", strreqdate, "", model.Id, poareceiverList, "submit");
                            var sendmail = SendMail(update.FORM_NO, strreqdate, update.DOCUMENT_TYPE, update.NPPBKC_ID, mapped_nppbkc.KppbcId, mapped_nppbkc.Address, mapped_nppbkc.Region, mapped_nppbkc.City, updated_list, CreatorName, Status, "", strreqdate, "", CRId, poareceiverList, "submit");
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
                    else
                    {
                        var ApproverName = "";
                        var Sendto = new List<string>();
                        Sendto.Add(CreatorMail);
                        if (Action == "approve")
                        {
                            ApproverName = refService.GetPOA(update.LASTAPPROVED_BY).PRINTED_NAME;
                            msgSuccess = "Success approve Change Request";
                            var sendmail = SendMail(update.FORM_NO, strreqdate, update.DOCUMENT_TYPE, update.NPPBKC_ID, mapped_nppbkc.KppbcId, mapped_nppbkc.Address, mapped_nppbkc.Region, mapped_nppbkc.City, updated_list, CreatorName, Status, ApproverName, strappdate, Comment, CRId, Sendto, "approve");
                            if (!sendmail)
                            {
                                msgSuccess += " , but failed send mail to Creator";
                            }
                        }
                        else if (Action == "reject")
                        {
                            ApproverName = refService.GetPOA(update.LASTAPPROVED_BY).PRINTED_NAME;
                            msgSuccess = "Success reject Change Request";
                            var sendmail = SendMail(update.FORM_NO, strreqdate, update.DOCUMENT_TYPE, update.NPPBKC_ID, mapped_nppbkc.KppbcId, mapped_nppbkc.Address, mapped_nppbkc.Region, mapped_nppbkc.City, updated_list, CreatorName, Status, ApproverName, strappdate, Comment, CRId, Sendto, "reject");
                            if (!sendmail)
                            {
                                msgSuccess += " , but failed send mail to Creator";
                            }
                        }
                        else if (Action == "revise" || Action == "finalreject")
                        {
                            ApproverName = refService.GetPOA(CurrentUser.USER_ID).PRINTED_NAME;
                            msgSuccess = "Success revise Change Request";
                            var sendmail = SendMail(update.FORM_NO, strreqdate, update.DOCUMENT_TYPE, update.NPPBKC_ID, mapped_nppbkc.KppbcId, mapped_nppbkc.Address, mapped_nppbkc.Region, mapped_nppbkc.City, updated_list, CreatorName, Status, ApproverName, strappdate, Comment, CRId, Sendto, "revise");
                            if (!sendmail)
                            {
                                msgSuccess += " , but failed send mail to Creator";
                            }
                        }
                        else if (Action == "cancel")
                        {
                            msgSuccess = "Success cancel Change Request";
                        }
                        else if (Action == "withdraw")
                        {
                            msgSuccess = "Success Withdrawing Change Request";
                        }

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

        #endregion

        #region Helpers

        public string UploadFile(HttpPostedFileBase File)
        {
            try
            {
                var filePath = "";

                if (File != null && File.ContentLength > 0)
                {
                    var baseFolder = "/files_upload/Manufacture/ChangeRequest/Documents/";
                    var uploadToFolder = Server.MapPath("~" + baseFolder);
                    var date_now = DateTime.Now;
                    var date = String.Format("{0:ddMMyyyyHHmmss}", date_now);
                    var extension = Path.GetExtension(File.FileName);
                    var file_name = Path.GetFileNameWithoutExtension(File.FileName) + "=MLCR=" + CurrentUser.USER_ID + "-" + date + extension;
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



        public void InsertUploadSuppDocFile(IEnumerable<ChangeRequestSupportingDocModel> SuppDocList, long CRId)
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
                            service.InsertFileUpload(CRId, Doc.Path, CurrentUser.USER_ID, Doc.Id, false, filename);
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
                            var DocName = Doc.Replace("/files_upload/Manufacture/ChangeRequest/Documents/", "");
                            var arrfileext = DocName.Split('.');
                            var countext = arrfileext.Count();
                            var fileext = "";
                            if (countext > 0)
                            {
                                fileext = arrfileext[countext - 1];
                            }
                            DocName = DocName.Replace("=MLCR=", "/");
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
                            service.InsertFileUpload(CRId, Doc, CurrentUser.USER_ID, 0, IsGov, thefilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
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

         
        private ChangeRequestModel GetChangeRequestDetail(long ID = 0, string Mode = "")
        {
            try
            {
                var _CRModel = service.GetChangeRequestById(ID).Select(s => new ChangeRequestModel
                {
                    Id = s.FORM_ID,
                    DocumentNumber = s.FORM_NO,
                    RequestDate = s.REQUEST_DATE,
                    DocumentType = s.DOCUMENT_TYPE,
                    //RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                    NppbkcId = s.NPPBKC_ID,
                    CreatedBy = s.CREATED_BY,
                    LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                    LastApprovedStatusID = s.LASTAPPROVED_STATUS,
                    Status = s.SYS_REFFERENCES.REFF_KEYS,
                    LastApprovedBy = s.LASTAPPROVED_BY,
                    DecreeStatus = s.DECREE_STATUS,
                    DecreeNumber = s.DECREE_NUMBER,
                    CompanyAlias = s.ZAIDM_EX_NPPBKC.COMPANY.BUTXT == null ? "-" : s.ZAIDM_EX_NPPBKC.COMPANY.BUTXT,
                    TextTo = s.ZAIDM_EX_NPPBKC.TEXT_TO,
                    CityAlias = s.ZAIDM_EX_NPPBKC.CITY,
                    CompanyAddress = s.ZAIDM_EX_NPPBKC.COMPANY.SPRAS,
                    CurrentRole = CurrentUser.UserRole,
                    IsCreator = (s.CREATED_BY == CurrentUser.USER_ID) ? true : false,
                }).FirstOrDefault();

                _CRModel.IsApprover = IsPOACanApprove(_CRModel.NppbkcId, _CRModel.CreatedBy, _CRModel.LastApprovedBy == null ? "" : _CRModel.LastApprovedBy, _CRModel.Id);
                _CRModel.IsViewer = IsPOACanView(_CRModel.NppbkcId, _CRModel.Id);

                _CRModel.ListOfUpdateNotes = service.GetChangeRequestDetailByFormId(_CRModel.Id).Select(s => new ChangeRequestDetailModel
                {
                    DetailId = s.FORM_DET_ID,
                    FormId = s.FORM_ID,
                    UpdateNotes = s.UPDATE_NOTES,
                    IsActive = 1
                }).ToList();

                _CRModel.DetailCount = _CRModel.ListOfUpdateNotes.Count();

                List<string> nppbkclist = new List<string>();

                //_CRModel.NppbkcList = GetNppbkcListByUser(nppbkclist);

                //_CRModel.NppbkcList = GetNppbkcList(refService.GetAllNppbkc());
                //_CRModel.POA = MapPoaModel(refService.GetPOA(CurrentUser.USER_ID));

                _CRModel.POA = MapPoaModel(refService.GetPOA(_CRModel.CreatedBy));
                _CRModel.DocumentTypes = GetDocumentTypeList(service.GetDocumentTypes());
                _CRModel.MainMenu = Enums.MenuList.ManufactureLicense;
                _CRModel.CurrentMenu = PageInfo;
                _CRModel.File_Size = GetMaxFileSize();


                var filesupload = service.GetFileUploadByCRId(ID);
                if (filesupload != null)
                {
                    var Othersfileupload = filesupload.Where(w => w.DOCUMENT_ID == null && w.IS_GOVERNMENT_DOC == false);
                    _CRModel.ChangeRequestFileOtherList = Othersfileupload.Select(s => new ChangeRequestFileOtherModel
                    {
                        FileId = s.FILE_ID,
                        Path = s.PATH_URL,
                        FileName = s.FILE_NAME
                    }).ToList();
                    foreach (var file in _CRModel.ChangeRequestFileOtherList)
                    {
                        file.Name = GenerateFileName(file.Path);
                        file.Path = GenerateURL(file.Path);
                    }


                    var BAsfileupload = filesupload.Where(w => w.IS_GOVERNMENT_DOC == true);
                    _CRModel.File_BA_Path_Plus = BAsfileupload.Select(s => new ChangeRequestFileOtherModel
                    {
                        FileId = s.FILE_ID,
                        Path = s.PATH_URL,
                        FileName = s.FILE_NAME
                    }).ToList();
                    foreach (var file in _CRModel.File_BA_Path_Plus)
                    {
                        file.Name = GenerateFileName(file.Path);
                        file.Path = GenerateURL(file.Path);
                    }

                }

                var ext = "";
                var count_pdf = 0;
                foreach (var file_attach in filesupload)
                {
                    ext = file_attach.PATH_URL.Substring((file_attach.PATH_URL.Length - 3), 3);
                    if (ext == "pdf")
                    {
                        count_pdf++;
                    }
                }

                _CRModel.Count_Lamp = count_pdf;


                if (Mode == "Edit")
                {
                    nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

                    _CRModel.IsFormReadOnly = false;
                    _CRModel.IsDetail = false;
                }
                else if (Mode == "Detail")
                {
                    nppbkclist.Add(_CRModel.NppbkcId);

                    _CRModel.IsFormReadOnly = true;
                    _CRModel.IsDetail = true;
                }
                else if (Mode == "Approve")
                {
                    nppbkclist.Add(_CRModel.NppbkcId);

                    _CRModel.IsFormReadOnly = true;
                    _CRModel.IsDetail = false;
                }

                _CRModel.NppbkcList = GetNppbkcListByUser(nppbkclist);

                _CRModel.GovStatus_List = GetGovStatusList(service.GetGovStatusList());

                //var history = refService.GetChangesHistory((int)Enums.MenuList.ChangeRequest, ID.ToString()).ToList();
                //_CRModel.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);

                var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.ChangeRequest, ID).ToList();

                string account_name = "";
                string role = "";
                string action = "";

                _CRModel.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

                WorkflowHistoryViewModel additional = new WorkflowHistoryViewModel();
                var LastApprovedStatus_Key = refService.GetReferenceById(_CRModel.LastApprovedStatusID).REFF_KEYS;
                if ((LastApprovedStatus_Key == "WAITING_POA_APPROVAL") || (LastApprovedStatus_Key == "WAITING_POA_SKEP_APPROVAL"))
                {
                    //var poa_approvers = service.GetPOAApproverList(_CRModel.NppbkcId, _CRModel.CreatedBy).Distinct().ToList();
                    var poa_approvers = service.GetPOAApproverList(ID);
                    foreach(var poa_approver in poa_approvers)
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

                    additional.USERNAME = account_name;
                    if (LastApprovedStatus_Key == "WAITING_POA_APPROVAL")
                    {
                        additional.ACTION = "Waiting For POA Approval";
                    }
                    else
                    {
                        additional.ACTION = "Waiting For POA SKEP Approval";
                    }
                    additional.Role = "POA";
                    //additional.ROLE = 2;
                    //additional.ACTION_DATE = _CRModel.LastModifiedDate;
                    _CRModel.WorkflowHistory.Add(additional);
                }



                _CRModel.ButtonCombination = GetButtonCombination(_CRModel.LastApprovedStatus);

                if ((_CRModel.ButtonCombination != "Create") && (_CRModel.ButtonCombination != "Edit"))
                {
                    _CRModel.IsFormReadOnly = true;
                }

                //_CRModel.Confirmation = GenerateConfirmDialog();

                return _CRModel;
            }
            catch (Exception e)
            {
                return new ChangeRequestModel();
            }
        }

        private string GetButtonCombination(string LastApprovedStatus)
        {
            string result = "Create";

            switch(LastApprovedStatus.ToUpper())
            {
                case "DRAFT NEW":
                    result = "Create";
                    break;

                case "DRAFT EDIT":
                    result = "Edit";
                    break;

                case "WAITING FOR POA APPROVAL":
                    result = "ApproveReject";
                    break;

                case "WAITING FOR GOVERNMENT APPROVAL":
                    result = "SubmitSKEP";
                    break;

                case "WAITING FOR POA SKEP APPROVAL":
                    result = "ApproveRejectFinal";
                    break;

                case "COMPLETED":
                    result = "Withdraw";
                    break;
            }

            return result;
        }

        private SelectList GetYearList(IEnumerable<ChangeRequestModel> changeRequest)
        {
            var query = from x in changeRequest
                        select new SelectItemModel()
                        {
                            ValueField = x.RequestDate.Year,
                            TextField = x.RequestDate.Year.ToString()
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetDocumentTypeList(Dictionary<string, string> types)
        {
            var query = from x in types
                        select new SelectItemModel()
                        {
                            ValueField = x.Key,
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
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

        private SelectList GetUserList(IEnumerable<CustomService.Data.USER> userList)
        {
            var query = from x in userList
                        select new SelectItemModel()
                        {
                            ValueField = x.USER_ID,
                            TextField = String.Format("{0} {1}", x.FIRST_NAME, x.LAST_NAME)
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
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

        private ChangeRequestModel GenerateModelProperties(ChangeRequestModel model)
        {
            if (model == null)
            {
                var nppbkclist = service.GetNPPBKCByUser(CurrentUser.USER_ID);

                model = new ChangeRequestModel()
                {
                    NppbkcList = GetNppbkcListByUser(nppbkclist),
                    POA = MapPoaModel(refService.GetPOA(CurrentUser.USER_ID)),
                    DocumentTypes = GetDocumentTypeList(service.GetDocumentTypes()),
                    MainMenu = Enums.MenuList.ManufactureLicense,
                    CurrentMenu = PageInfo,
                    WorkflowHistory = new List<WorkflowHistoryViewModel>(),
                    File_Size = GetMaxFileSize()
            };
            }
            return model;
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
            new_number = service.SetNewFormNumber(partial_number);
            return new_number;
        }

        public long GetSysreffApprovalStatus(string currStatus)
        {
            long ApproveStats = 0;
            var Reff = refService.GetRefByType("APPROVAL_STATUS");
            if (Reff.Any())
            {
                ApproveStats = Reff.Where(w => w.REFF_VALUE.Equals(currStatus)).Select(s => s.REFF_ID).FirstOrDefault();
            }
            return ApproveStats;
        }

        public bool SendMail(string formnumber, string request_date, string document_type, string nppbkc, string kpbc, string address, string province, string city, string updated_list, string creator, string approval_status, string approver, string approvedDate, string remark, long crid, List<string> sendto, string MailFor)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("form_number", formnumber);
                parameters.Add("date", request_date);
                parameters.Add("document_type", document_type);
                parameters.Add("nppbkc", nppbkc);
                parameters.Add("kpbc", kpbc);
                parameters.Add("address", address);
                parameters.Add("province", province);
                parameters.Add("city", city);
                parameters.Add("updated_list", updated_list);
                parameters.Add("creator", creator);
                parameters.Add("approval_status", approval_status);
                parameters.Add("approver", approver);
                parameters.Add("approver_date", approvedDate);
                parameters.Add("remark", remark);
                parameters.Add("url_detail", Url.Action("Detail", "MLChangeRequest", new { Id = crid }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "MLChangeRequest", new { Id = crid }, this.Request.Url.Scheme));
                parameters.Add("url_edit", Url.Action("Edit", "MLChangeRequest", new { Id = crid }, this.Request.Url.Scheme));

                long mailcontentId = 0;
                if (MailFor == "submit")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseChangeApprovalRequest;
                }
                else if (MailFor == "approve")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseChangeApprovalNotification;
                }
                else if (MailFor == "revise")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseChangeRevisionRequest;
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

        //public bool IsPOACanApprove(long CRId, string UserId, string LastApprovedBy = "")
        public bool IsPOACanApprove(string NPPBKCId, string CreatedBy, string LastApprovedBy = "", long ID = 0)
        {
            var isOk = false;
            if (LastApprovedBy != "")
            {
                if (LastApprovedBy == CurrentUser.USER_ID)
                {
                    isOk = true;
                }
            }
            else
            {
                //var POAApprover = service.GetPOAApproverList(NPPBKCId, CreatedBy).ToList();
                var CRequest = service.GetChangeRequestById(ID).FirstOrDefault();

                if ((CRequest.LASTAPPROVED_STATUS != refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID) && (CRequest.LASTAPPROVED_STATUS != refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID))
                {
                    var POAApprover = service.GetPOAApproverList(ID).ToList();
                    if (POAApprover.Count() > 0)
                    {
                        List<string> ListPOA = POAApprover.Select(s => s.POA_ID.ToUpper()).ToList();
                        if (ListPOA.Contains(CurrentUser.USER_ID.ToUpper()))
                        {
                            isOk = true;
                        }
                    }
                }



                //var approverlist = service.GetPOAApproverList(CRId);
                //if (approverlist.Count() > 0)
                //{
                //    var isexist = approverlist.Where(w => w.POA_ID.Equals(UserId)).ToList();
                //    if (isexist.Count() > 0)
                //    {
                //        isOk = true;
                //    }
                //}
            }
            return isOk;
        }

        public bool IsPOACanView(string NPPBKCId, long ID = 0)
        {
            var isOk = false;
            //var POAApprover = service.GetPOAApproverList(NPPBKCId, CreatedBy).ToList();
            var CRequest = service.GetChangeRequestById(ID).FirstOrDefault();

            if ((CRequest.LASTAPPROVED_STATUS != refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID) && (CRequest.LASTAPPROVED_STATUS != refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID))
            {
                var POAList = service.GetPOAInNPPBKCList(NPPBKCId).ToList();
                if (POAList.Count() > 0)
                {
                    List<string> ListPOA = POAList.Select(s => s.POA_ID).ToList();
                    if (ListPOA.Contains(CurrentUser.USER_ID))
                    {
                        isOk = true;
                    }
                }
            }

            return isOk;
        }

        public bool IsPOACanAccess(long IRId, string UserId)
        {
            var isOk = true;
            var CreatorId = "";
            var IRequest = service.GetChangeRequestById(IRId).FirstOrDefault();
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

        private string GenerateURL(string path)
        {
            var url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + path;
            return url;
        }

        private string GenerateFileName(string path)
        {
            var filename = path.Replace("/files_upload/Manufacture/ChangeRequest/Documents/", "");
            var arrfileext = filename.Split('.');
            var countext = arrfileext.Count();
            var fileext = "";
            if (countext > 0)
            {
                fileext = arrfileext[countext - 1];
            }
            filename = filename.Replace("=MLCR=", "/");
            var arrfilename = filename.Split('/');
            if (arrfilename.Count() > 0)
            {
                filename = arrfilename[0] + "." + fileext;
            }
            return filename;
        }


        #endregion

        #region Mappings

        private REPLACEMENT_DOCUMENTS MapToTable(ChangeRequestModel model)
        {
            var now = DateTime.Now;

            var data = new REPLACEMENT_DOCUMENTS();
            if (model.Id != 0)
            {
                data.FORM_ID = model.Id;
                data.LASTAPPROVED_STATUS = GetSysreffApprovalStatus(model.LastApprovedStatus);
                data.LASTMODIFIED_BY = CurrentUser.USER_ID;
                data.LASTMODIFIED_DATE = now;
            }
            else
            {
                data.FORM_NO = GenerateFormNumber(model.CompanyAlias, model.CityAlias, now);
                data.CREATED_BY = CurrentUser.USER_ID;
                data.CREATED_DATE = now;
                data.LASTMODIFIED_BY = CurrentUser.USER_ID;
                data.LASTMODIFIED_DATE = now;
                data.LASTAPPROVED_STATUS = GetSysreffApprovalStatus("DRAFT NEW");
            }
            data.REQUEST_DATE = model.RequestDate;
            data.DOCUMENT_TYPE = model.DocumentType;
            data.NPPBKC_ID = model.NppbkcId;
            data.DECREE_STATUS = true;

            return data;
        }

        private List<REPLACEMENT_DOCUMENTS_DETAIL> MapToDetail(List<ChangeRequestDetailModel> detailModel)
        {
            var result = new List<REPLACEMENT_DOCUMENTS_DETAIL>();
            foreach (var detail in detailModel)
            {
                result.Add(new REPLACEMENT_DOCUMENTS_DETAIL
                {
                    FORM_DET_ID = detail.DetailId,
                    FORM_ID = detail.FormId,
                    UPDATE_NOTES = detail.UpdateNotes
                });
            }
            return result;
        }

        private UserModel MapToUserModel(CustomService.Data.USER user)
        {
            try
            {
                return new UserModel()
                {
                    UserId = user.USER_ID,
                    FirstName = user.FIRST_NAME,
                    LastName = user.LAST_NAME
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private ChangeRequestPOA MapPoaModel(CustomService.Data.POA poa)
        {
            try
            {
                return new ChangeRequestPOA()
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

        private ChangeRequestNppbkc MapNppbkcModel(CustomService.Data.MASTER_NPPBKC nppbkc)
        {
            try
            {
                var Plants = service.GetPlantByNPPBKC(nppbkc.NPPBKC_ID).OrderByDescending(o => o.IS_MAIN_PLANT).ThenBy(o => o.NAME1).Select(s => new ManufacturePlant
                {
                    WERKS = s.WERKS,
                    PlantName = s.NAME1,
                    PlantCity = s.ORT01,
                    PlantPhone = s.PHONE,
                    PlantAddress = s.ADDRESS,
                    NPPBKCId = s.NPPBKC_ID,
                    IsMainPlant = s.IS_MAIN_PLANT ?? false,
                    NPPBKCIdImport = s.NPPBKC_IMPORT_ID
                }).ToList();

                return new ChangeRequestNppbkc()
                {
                    Id = nppbkc.NPPBKC_ID,
                    Region = nppbkc.REGION,
                    Address = String.Format("{0}, {1}", nppbkc.ADDR1, nppbkc.ADDR2),
                    City = nppbkc.CITY,
                    KppbcId = nppbkc.KPPBC_ID,
                    CompanyAlias = nppbkc.COMPANY.BUTXT_ALIAS,
                    CityAlias = nppbkc.CITY_ALIAS,
                    Company = new CompanyModel()
                    {
                        Id = nppbkc.COMPANY.BUKRS,
                        Name = nppbkc.COMPANY.BUTXT
                    },
                    Plants = Plants
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //#endregion

        private ChangeRequestModel MapChangeRequestModel(CustomService.Data.REPLACEMENT_DOCUMENTS entity)
        {
            try
            {
                //List<FinanceRatioModel> financialRatio = new List<FinanceRatioModel>();
                //financialRatio.Add(new FinanceRatioModel()
                //{
                //    LiquidityRatio = entity.LIQUIDITY_RATIO_1,
                //    SolvencyRatio = entity.SOLVENCY_RATIO_1,
                //    RentabilityRatio = entity.RENTABILITY_RATIO_1
                //});
                //financialRatio.Add(new FinanceRatioModel()
                //{
                //    LiquidityRatio = entity.LIQUIDITY_RATIO_2,
                //    SolvencyRatio = entity.SOLVENCY_RATIO_2,
                //    RentabilityRatio = entity.RENTABILITY_RATIO_2
                //});
                return new ChangeRequestModel()
                {
                    Id = entity.FORM_ID,
                    DocumentNumber = entity.FORM_NO,
                    RequestDate = entity.REQUEST_DATE,
                    DocumentType = entity.DOCUMENT_TYPE,
                    //RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                    NppbkcId = entity.NPPBKC_ID,
                    CreatedBy = entity.CREATED_BY,
                    KPPBCAddress = service.GetNppbkc(entity.NPPBKC_ID).KPPBC_ADDRESS,
                    //LastApprovedStatus = entity.LASTAPPROVED_STATUS.
            };
            }
            catch (Exception ex)
            {
                //var msg = String.Format("Message: {0}\nStack Trace: {1}\nInner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException);
                //AddMessageInfo(msg, Enums.MessageInfoType.Error);
                throw ex;
            }
        }

        public ChangeRequestSupportingDocModel MapSupportingDocumentModel(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new ChangeRequestSupportingDocModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    IsBrowseFileEnable = true,
                    Path = "",
                    FileName = "",
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

        //private string GetRequestTypeName(int id)
        //{
        //    return service.GetExciseCreditTypeName(id);
        //}
        #endregion

        #region PrintOut

        private ChangeRequestModel GetChangeRequestMasterForm(long CRId)
        {

            var _CRModel = service.GetChangeRequestById(CRId).Select(s => new ChangeRequestModel
            {
                Id = s.FORM_ID,
                DocumentNumber = s.FORM_NO,
                LastApprovedStatus = s.SYS_REFFERENCES.REFF_VALUE,
                RequestDate = s.REQUEST_DATE,
                NppbkcId = s.NPPBKC_ID ?? "",
                CreatedBy = s.CREATED_BY,
                LastApprovedBy = s.LASTAPPROVED_BY,
                DecreeStatus = s.DECREE_STATUS,
                DecreeNumber = s.DECREE_NUMBER,
                CompanyAlias = s.ZAIDM_EX_NPPBKC.COMPANY.BUTXT == null ? "-" : s.ZAIDM_EX_NPPBKC.COMPANY.BUTXT,
                TextTo = s.ZAIDM_EX_NPPBKC.TEXT_TO,
                CityAlias = s.ZAIDM_EX_NPPBKC.CITY,
                CompanyAddress = s.ZAIDM_EX_NPPBKC.COMPANY.SPRAS
            }).FirstOrDefault();

            _CRModel.POA = MapPoaModel(refService.GetPOA(_CRModel.CreatedBy));

            _CRModel.Company = MapNppbkcModel(service.GetNppbkc(_CRModel.NppbkcId)).Company;


            var filesupload = service.GetFileUploadByCRId(_CRModel.Id);

            _CRModel.Count_Lamp = filesupload.Where(w => w.IS_GOVERNMENT_DOC == false).Count();

            return _CRModel;
        }

        public List<ChangeRequestDetailModel> GetChangeRequestDetail(long CRId = 0)
        {
            try
            {
                var CRDetModel = new List<ChangeRequestDetailModel>();

                var data = service.GetChangeRequestDetailByFormId(CRId);
                if (data.Any())
                {
                    CRDetModel = data.Select(s => new ChangeRequestDetailModel
                    {
                        DetailId = s.FORM_DET_ID,
                        FormId = s.FORM_ID,
                        UpdateNotes = s.UPDATE_NOTES
                    }).ToList();
                }
                return CRDetModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<ChangeRequestDetailModel>();
            }
        }


        [HttpPost]
        public ActionResult GeneratePrintout(long ChangeID)
        {
            var _CRModel = new ChangeRequestModel();
            _CRModel.Id = ChangeID;
            var layout = GetPrintout(_CRModel);
            return Json(layout);
        }

        private string GetPrintout(ChangeRequestModel _CRModel)
        {
            _CRModel = GetChangeRequestMasterForm(_CRModel.Id);
            _CRModel.ListOfUpdateNotes = GetChangeRequestDetail(_CRModel.Id);

            var parameters = new Dictionary<string, string>();
            parameters.Add("COMPANY_NAME", _CRModel.Company.Name);
            parameters.Add("COMPANY_ADDRESS", _CRModel.Company.Name);
            parameters.Add("COMPANY_CITY", _CRModel.CityAlias);
            parameters.Add("REQUEST_DATE", _CRModel.RequestDate.ToString("dd MMMM yyyy"));
            parameters.Add("FORM_NUMBER", _CRModel.DocumentNumber);
            parameters.Add("LAMPIRAN_COUNT", Convert.ToString(_CRModel.Count_Lamp));
            parameters.Add("DOCUMENT_TYPE", _CRModel.DocumentType);
            parameters.Add("KPPBC_TEXT_TO", _CRModel.TextTo);
            parameters.Add("POA_NAME", _CRModel.POA.Name);
            parameters.Add("POA_ROLE", _CRModel.POA.Position);
            parameters.Add("POA_ADDRESS", _CRModel.POA.Address);
            parameters.Add("NPPBKC", _CRModel.NppbkcId);

            var layout = "";
            var layout_item_updates = "<br /><table>";
            int no_item_update = 1;
            foreach (var itemUpdate in _CRModel.ListOfUpdateNotes)
            {
                layout_item_updates += "<tr><td>" + no_item_update.ToString() + ".</td><td>&nbsp;" + itemUpdate.UpdateNotes + "</td></tr>";
                no_item_update++;
            }
            layout_item_updates += "</table><br />";
            parameters.Add("ITEMS_UPDATE", layout_item_updates);

            layout += refService.GeneratePrintout("CHANGE_REQUEST_PRINTOUT", parameters, _CRModel.CreatedBy).LAYOUT + "<br /><br /><br />";

            return layout;
        }
        public ActionResult GetPrintOutLayout(string CreatedBy)
        {
            if (CreatedBy == "")
            {
                CreatedBy = CurrentUser.USER_ID;
            }
            var result = refService.GetPrintoutLayout("CHANGE_REQUEST_PRINTOUT", CreatedBy);
            var layout = "No Layout Found.";
            if (result.Any())
            {
                layout = result.FirstOrDefault().LAYOUT;
            }
            return Json(layout);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdatePrintOutLayout(string NewPrintout, string CreatedBy)
        {
            if (CreatedBy == "")
            {
                CreatedBy = CurrentUser.USER_ID;
            }
            var ErrMessage = refService.UpdatePrintoutLayout("CHANGE_REQUEST_PRINTOUT", NewPrintout, CreatedBy);
            return Json(ErrMessage);
        }

        [HttpPost]
        public void DownloadPrintOut(ChangeRequestModel _CRModel)
        {
            try
            {
                long InterviewID = _CRModel.Id;
                string FormNumber = _CRModel.DocumentNumber;
                FormNumber = FormNumber.Replace('/', '-');
                var now = DateTime.Now.ToString("ddMMyyyy");
                _CRModel.Id = InterviewID;
                var htmlText = GetPrintout(_CRModel);

                _CRModel = GetChangeRequestMasterForm(_CRModel.Id);

                //MemoryStream ms = new MemoryStream();
                var baseFolder = "/files_upload/Manufacture/ChangeRequest/PrintOut/";
                var uploadToFolder = Server.MapPath("~" + baseFolder);
                Directory.CreateDirectory(uploadToFolder);
                uploadToFolder += "PrintOut_ChangeRequest_" + FormNumber + "_" + now + ".pdf";
                FileStream stream = new FileStream(uploadToFolder, FileMode.Create);
                var leftMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var rightMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var topMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var bottomtMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, leftMargin, rightMargin, topMargin, bottomtMargin);
                var writer = PdfWriter.GetInstance(document, stream);
                long LastApprovedStatusID = GetSysreffApprovalStatus(_CRModel.LastApprovedStatus);
                //if ((LastApprovedStatusID != refService.GetRefByKey("COMPLETED").REFF_ID) && (LastApprovedStatusID != refService.GetRefByKey("CANCELED").REFF_ID))
                if ((LastApprovedStatusID == refService.GetRefByKey("DRAFT_NEW_STATUS").REFF_ID) || (LastApprovedStatusID == refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID) || (LastApprovedStatusID == refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID))
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

                var MergeDocs = MergePrintout(uploadToFolder, InterviewID);

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
                var filesupload = service.GetFileUploadByCRId(RegId).ToList();

                List<String> sourcePaths = new List<string>();
                sourcePaths.Add(path);
                var ext = "";
                foreach (var file_attach in filesupload)
                {
                    ext = file_attach.PATH_URL.Substring((file_attach.PATH_URL.Length - 3), 3);
                    if (ext.ToLower() == "pdf")
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

        //[HttpPost]
        //[ValidateInput(false)]
        //public void DownloadPrintOut(string htmlText)
        //{
        //    try
        //    {

        //        Byte[] bytes;
        //        MemoryStream ms = new MemoryStream();
        //        var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20f, 20f, 20f, 20f);
        //        var writer = PdfWriter.GetInstance(doc, ms);
        //        writer.CloseStream = false;
        //        doc.Open();
        //        var srHtml = new StringReader(htmlText);
        //        XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
        //        doc.Close();
        //        bytes = ms.ToArray();
        //        ms.Write(bytes, 0, bytes.Length);
        //        ms.Position = 0;
        //        Response.Clear();
        //        Response.ContentType = "application/pdf";
        //        Response.AddHeader("content-disposition", "attachment;filename=PrintOut_ChangeRequest.pdf");
        //        Response.Buffer = true;
        //        ms.WriteTo(Response.OutputStream);
        //        Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
                        CssClass = " submit-modal changerequest",
                        Message = "You are going to Submit Change Request. Are you sure?",
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
                        CssClass = " submitskep-modal changerequest",
                        Message = "You are going to Submit Change Request. Are you sure?",
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
                        CssClass = " cancel-modal changerequest",
                        Message = "You are going to Cancel Change Request. Are you sure?",
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
                        CssClass = " withdraw-modal changerequest",
                        Message = "You are going to Withdraw Change Request. Are you sure?",
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
                        CssClass = " approve-modal changerequest",
                        Message = "You are going to approve Change Request. Are you sure?",
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
                        CssClass = " approvefinal-modal changerequest",
                        Message = "You are going to approve Change Request. Are you sure?",
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
                    CssClass = " restoredefault-modal changerequest",
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


        //private List<ConfirmDialogModel> GenerateConfirmDialog()
        //{
        //    try
        //    {
        //        var listconfirmation = new List<ConfirmDialogModel>();

        //        //// FOR SET PRINTOUT TO DEFAULT CONFIRMATION ////                
        //        listconfirmation.Add(new ConfirmDialogModel()
        //        {
        //            Action = new ConfirmDialogModel.Button()
        //            {
        //                Id = "btnRestorePrintoutToDefault",
        //                CssClass = "btn btn-success btn_loader",
        //                Label = "Restore To Default"
        //            },
        //            CssClass = " restoredefault-modal changerequest",
        //            Message = "You are going to restore printout layout to default. Are you sure?",
        //            Title = "Restore Printout Confirmation",
        //            ModalLabel = "RestoreModalLabel"
        //        });
        //        //////////////////////////////////////////////////

        //        return listconfirmation;
        //    }
        //    catch (Exception e)
        //    {
        //        return new List<ConfirmDialogModel>();
        //    }
        //}

        [HttpPost]
        public ActionResult RestorePrintoutToDefault(string CreatedBy)
        {
            if (CreatedBy == "")
            {
                CreatedBy = CurrentUser.USER_ID;
            }

            var ErrMessage = refService.RestorePrintoutToDefault("CHANGE_REQUEST_PRINTOUT", CreatedBy);
            return Json(ErrMessage);
        }

        #endregion


    }
}