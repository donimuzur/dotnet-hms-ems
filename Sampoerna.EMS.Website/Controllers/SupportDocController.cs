using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.SupportDoc;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Sampoerna.EMS.Core.Enums;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Website.Controllers
{
    public class SupportDocController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SupportDocManagementService service;
        private SystemReferenceService refService;
        private IChangesHistoryBLL _changesHistoryBll;        
        private IWorkflowHistoryBLL workflowHistoryBLL;

        public SupportDocController(IPageBLL pageBLL, IChangesHistoryBLL changesHistoryBll, IWorkflowHistoryBLL workflowHistoryBLL)
            : base(pageBLL, Enums.MenuList.SupportDoc)
        {

            this.mainMenu = Enums.MenuList.MasterData;
            this.service = new SupportDocManagementService();
            this.refService = new SystemReferenceService();
            this._changesHistoryBll = changesHistoryBll;
            this.workflowHistoryBLL = workflowHistoryBLL;
        }

        #region Local Helpers
        private SupportDocViewModel GenerateProperties(SupportDocViewModel source, bool update)
        {
            var companyList = service.GetCompanies().Select(item => new CompanyModel()
            {
                Id = item.BUKRS,
                Name = item.BUTXT
            });

            var data = source;

            if (!update || data == null)
            {
                data = new SupportDocViewModel();
            }

            IEnumerable<FormList> formTypes = Enum.GetValues(typeof(FormList)).Cast<FormList>();
            data.ListForm = from form in formTypes
                            select new SelectListItem
                            {
                                Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(FormList), form.ToString())),
                                Value = ((int)form).ToString()
                            };

            data.MainMenu = mainMenu;
            data.CurrentMenu = PageInfo;
            data.IsNotViewer = CurrentUser.UserRole == Enums.UserRole.Administrator;
            data.CompanyList = GenericHelpers<CompanyModel>.GenerateList(companyList, item => item.Id, item => item.Name);
            data.ShowActionOptions = data.IsNotViewer;
            data.EditMode = false;
            data.EnableFormInput = true;
            data.ViewModel.IsCreator = false;
            return data;
        }

        private void ExecuteEdit(SupportDocViewModel model, ReferenceKeys.ApprovalStatus statusApproval, ReferenceKeys.EmailContent emailTemplate, bool sendEmail = false)
        {
            try
            {
                var obj = model.ViewModel;
                var data = service.Find(model.ViewModel.DocumentID);
                var old = Mapper.Map<SupportDocModel>(data);
                obj.CreatedBy = old.CreatedBy;
                obj.CreatedDate = old.CreatedDate;
                obj.Company = Mapper.Map<CompanyModel>(service.GetCompany(obj.Bukrs));
                if (statusApproval == ReferenceKeys.ApprovalStatus.Edited)
                {
                    // do nothing

                }
                else if (statusApproval == ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                {
                    obj = old;
                }
                else if (statusApproval == ReferenceKeys.ApprovalStatus.Completed)
                {
                    obj = old;
                    obj.LastApprovedBy = CurrentUser.USER_ID;
                    obj.LastApprovedDate = DateTime.Now;
                }
                obj.LastModifiedBy = CurrentUser.USER_ID;
                obj.LastModifiedDate = DateTime.Now;
                obj.ApprovalStatus = refService.GetReferenceByKey(statusApproval).REFF_ID;
                model.ViewModel = obj;
                var parameters = new Dictionary<string, string>();
                parameters.Add("company", data.COMPANY.BUTXT);                
                parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy")); // without time
                //parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy hh:mm:ss")); // with time
                parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                parameters.Add("approval_status", data.APPROVALSTATUS.REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));


                bool success = service.Edit(Mapper.Map<CustomService.Data.MASTER_SUPPORTING_DOCUMENT>(obj), (int)Enums.MenuList.SupportDoc, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                if (success)
                {
                    if (sendEmail)
                    {
                        var mailContent = refService.GetMailContent((int)emailTemplate, parameters);
                        var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                        var display = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminCreator);
                        var sendToId = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover).REFF_VALUE;
                        var sendTo = refService.GetUserEmail(sendToId);
                        AddMessageInfo(Constans.SubmitMessage.Updated + "<br />Sending email", Enums.MessageInfoType.Success);
                        bool mailStatus = ItpiMailer.Instance.SendEmail(new string[] { sendTo }, null, null, null, mailContent.EMAILSUBJECT, mailContent.EMAILCONTENT, true, sender, display);
                        if (!mailStatus)
                        {
                            AddMessageInfo("Send email failed! Please try again", Enums.MessageInfoType.Warning);
                        }
                        else
                        {
                            AddMessageInfo("Email sent!", Enums.MessageInfoType.Success);
                        }
                    }
                    else
                    {
                        AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
                    }
                }
                else
                    AddMessageInfo("Submit failed! Please try again", Enums.MessageInfoType.Error);
            }
            catch (Exception ex)
            {
                var msg = String.Format("Message: {0}\nStack Trace: {1}\nInner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException);
                AddMessageInfo(msg, Enums.MessageInfoType.Error);
            }
        }

        #endregion

        #region Index
        public ActionResult Index()
        {
            var data = new SupportDocViewModel()
            {
                MainMenu = mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole == Enums.UserRole.Administrator ? true : false),
                ListSupportDocs = Mapper.Map<List<SupportDocModel>>(
                service.GetAll().OrderByDescending(item => item.DOCUMENT_ID)),
                IsAdminApprover = refService.IsAdminApprover(CurrentUser.USER_ID)
            };

            var list = new List<SupportDocModel>(data.ListSupportDocs);
            data.ListSupportDocs = new List<SupportDocModel>();
            var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
            var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;

            foreach (var item in list)
            {
                item.IsCreator = CurrentUser.USER_ID == item.CreatedBy;
                item.IsApproved = item.ApprovalStatus == approvalStatusApproved;
                item.IsSubmitted = item.ApprovalStatus == approvalStatusSubmitted;
                data.ListSupportDocs.Add(item);
            }
            return View("Index", data);
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var data = GenerateProperties(null, false);

            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(SupportDocViewModel model)
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }

                var obj = model.ViewModel;
                obj.CreatedBy = CurrentUser.USER_ID;
                obj.CreatedDate = DateTime.Now;
                obj.LastModifiedBy = CurrentUser.USER_ID;
                obj.LastModifiedDate = DateTime.Now;
                model.ViewModel = obj;

                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                }
                else
                {
                    obj.ApprovalStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                    var inserted = service.Create(Mapper.Map<CustomService.Data.MASTER_SUPPORTING_DOCUMENT>(obj), (int)Enums.MenuList.SupportDoc, (int)Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }

            model = GenerateProperties(model, true);
            return View(model);
        }

        #endregion

        #region Detail
        public ActionResult Detail(string id)
        {
            var data = GenerateProperties(null, false);
            data.ViewModel = Mapper.Map<SupportDocModel>(service.Find(Convert.ToInt64(id)));
            //var history = refService.GetChangesHistory((int)Enums.MenuList.SupportDoc, id).ToList();
            var history = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.SupportDoc, id);
         //   var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.SupportDoc, Int64.Parse(id)).ToList();
            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
            data.EnableFormInput = false;
            data.EditMode = true;
            //data.ViewModel = this.FormatCurrency(data.ViewModel);

            return View(data);
        }
        #endregion

        #region Edit
        public ActionResult Edit(string id)
        {
            var data = GenerateProperties(null, false);
            var obj = service.Find(Convert.ToInt64(id));
            var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
            var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
       
            var history = this._changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.SupportDoc, id);

            data.ViewModel = Mapper.Map<SupportDocModel>(obj);
            data.ViewModel.IsCreator = true;
            data.ViewModel.IsSubmitted = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusSubmitted;
            data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;

            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);    

            data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
         
            if (data.ViewModel.IsSubmitted)
            {
                AddMessageInfo("Operation not allowed!. This entry already submitted!", Enums.MessageInfoType.Error);
                //data = GenerateProperties(data, true);
                RedirectToAction("Index");
            }

            data.EnableFormInput = true;
            data.EditMode = true;
            return View("Edit", data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(SupportDocViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                model = GenerateProperties(model, true);
                //var obj = service.Find(Convert.ToInt64(id));
                model.EnableFormInput = true;
                model.EditMode = true;
                model.ViewModel.IsCreator = CurrentUser.USER_ID == model.ViewModel.CreatedBy;
                            
                var history = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.SupportDoc, model.ViewModel.DocumentID.ToString()).ToList();
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                model.WorkflowHistory =  GetWorkflowHistory(model.ViewModel.DocumentID);

                return View("Edit", model);

            }
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            ExecuteEdit(model, ReferenceKeys.ApprovalStatus.Edited, ReferenceKeys.EmailContent.SupportDocApprovalRequest);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(SupportDocViewModel model)
        {
            try
            {
                var data = service.Find(model.ViewModel.DocumentID);
                var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                //var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminCreator), data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.Admin), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);                
                var parameters = new Dictionary<string, string>();

                parameters.Add("company", data.COMPANY.BUTXT);
                //parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy hh:mm:ss")); // with time
                parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy"));// without time
                parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                parameters.Add("submitter", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));

                var mailContent = refService.GetMailContent((int)ReferenceKeys.EmailContent.SupportDocApprovalRequest, parameters);
                var reff = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
                var sendToId = reff.REFF_VALUE;
                var sendTo = refService.GetUserEmail(sendToId);

                ExecuteApprovalAction(model, ReferenceKeys.ApprovalStatus.AwaitingAdminApproval, Enums.ActionType.WaitingForApproval, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Submit Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Approval Section

        private void ExecuteApprovalAction(SupportDocViewModel model, ReferenceKeys.ApprovalStatus statusApproval, Enums.ActionType actionType, string email, string subject, string sender, string display, string sendTo)
        {
            var comment = (model.ViewModel.RevisionData != null) ? model.ViewModel.RevisionData.Comment : null;
            var updated = service.ChangeStatus(model.ViewModel.DocumentID, statusApproval, (int)Enums.MenuList.SupportDoc, (int)actionType, (int)CurrentUser.UserRole, CurrentUser.USER_ID, comment);

            if (updated != null)
            {
                AddMessageInfo(Constans.SubmitMessage.Updated + " and sending email", Enums.MessageInfoType.Success);
                List<string> mailAddresses = new List<string>();
                if (statusApproval == ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                {
                    var approvers = refService.GetAdminApprovers().ToList();
                    foreach (var appr in approvers)
                    {
                        var _email = refService.GetUserEmail(appr.REFF_VALUE.Trim());
                        if (!string.IsNullOrEmpty(_email))
                        {
                            mailAddresses.Add(_email);
                        }
                    }
                }
                else
                {
                    var admins = refService.GetAdmins().ToList();
                    foreach (var adm in admins)
                    {
                        var _email = refService.GetUserEmail(adm.USER_ID);
                        if (!string.IsNullOrEmpty(_email) && _email != sender)
                        {
                            mailAddresses.Add(_email);
                        }
                    }
                    //mailAddresses.Add(sendTo);
                }
                bool mailStatus = ItpiMailer.Instance.SendEmail(mailAddresses.ToArray(), null, null, null, subject, email, true, sender, display);
                if (!mailStatus)
                {
                    AddMessageInfo("Send email failed!", Enums.MessageInfoType.Warning);
                }
                else
                {
                    AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
                }
            }
        }

        public ActionResult Approve(string id)
        {
            try
            {
                var data = GenerateProperties(null, false);
                data.ViewModel = Mapper.Map<SupportDocModel>(service.Find(Convert.ToInt64(id)));

                var history = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.SupportDoc, id);              
                data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
                data.EnableFormInput = false;
                data.EditMode = true;                
                data.IsAdminApprover = refService.IsAdminApprover(CurrentUser.USER_ID);

                var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                data.ViewModel.IsCreator = true;
                data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;

                if (data.ViewModel.IsApproved)
                {
                    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);
                    //data = GenerateProperties(data, true);
                    RedirectToAction("Index");
                }
                data.ApproveConfirm = new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "ApproveButtonConfirm",
                        CssClass = "btn btn-success",
                        Label = "Approve"
                    },
                    CssClass = " approve-modal supportingdocument",
                    Id = "SupportingDocumentApproveModal",
                    Message = String.Format("You are going to approve Supporting Document data. Are you sure?", data.ViewModel.Company.Name, data.ViewModel.FormID),
                    Title = "Approve Confirmation",
                    ModalLabel = "ApproveModalLabel"

                };
                data.ViewModel.RevisionData = new WorkflowHistory()
                {
                    FormID = Convert.ToInt64(id),
                    FormTypeID = (int)Enums.MenuList.SupportDoc,
                    Action = (int)Enums.ActionType.Reject,
                    ActionBy = CurrentUser.USER_ID,
                    Role = (int)CurrentUser.UserRole
                };
                return View("Approve", data);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Submit Failed : " + ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Approve(SupportDocViewModel model)
        {
            try
            {
                var data = service.Find(model.ViewModel.DocumentID);

                var parameters = new Dictionary<string, string>();
                parameters.Add("company", data.COMPANY.BUTXT);                
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy")); // without time
                //parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy hh:mm:ss")); // with time
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));

                var mailContent = refService.GetMailContent((int)ReferenceKeys.EmailContent.SupportDocApproved, parameters);

                var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminApprover), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);
                var sendTo = data.CREATOR.EMAIL;
                ExecuteApprovalAction(model, ReferenceKeys.ApprovalStatus.Completed, Enums.ActionType.Approve, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Submit Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Revise(SupportDocViewModel model)
        {
            try
            {
                var data = service.Find(model.ViewModel.DocumentID);

                var parameters = new Dictionary<string, string>();
                parameters.Add("company", data.COMPANY.BUTXT);                
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy")); // without time
                //parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy hh:mm:ss")); // with time
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Rejected).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));
                parameters.Add("url_edit", Url.Action("Edit", "SupportDoc", new { id = data.DOCUMENT_ID }, this.Request.Url.Scheme));
                parameters.Add("remark", model.ViewModel.RevisionData.Comment);
                var mailContent = refService.GetMailContent((int)ReferenceKeys.EmailContent.SupportDocRejected, parameters);

                var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminApprover), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);
                var sendTo = data.CREATOR.EMAIL;
                ExecuteApprovalAction(model, ReferenceKeys.ApprovalStatus.Edited, Enums.ActionType.Reject, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Submit Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Additional Business Logic Handlers
        List<WorkflowHistoryViewModel> GetWorkflowHistory(long id)
        {

            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.SupportingDocument;
            var workflow = this.workflowHistoryBLL.GetByFormTypeAndFormId(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID);

            return Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

        }

        [HttpPost]
        public JsonResult IsExist(string docName, bool isActive)
        {
            var result = service.IsExist(docName, isActive);
            var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
            JObject obj = new JObject()
            {
                new JProperty("exist", result != null),
                new JProperty("detail", (result != null) ? new JObject(
                    new JProperty("IsActive", result.IS_ACTIVE),
                    new JProperty("Id", result.DOCUMENT_ID),
                    new JProperty("DocName", result.SUPPORTING_DOCUMENT_NAME),
                    new JProperty("Company", result.BUKRS),
                    new JProperty("Submitted", result.LASTAPPROVED_STATUS == approvalStatusSubmitted)
                    ) : null)
            };
            return Json(obj.ToString());
        }

        #endregion
    }
}