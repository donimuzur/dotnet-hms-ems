using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ProductType;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.Shared;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ProductTypeController : BaseController
    {
         private Enums.MenuList _mainMenu;
         private IZaidmExProdTypeBLL _exProdTypeBll;
         private ProductTypeService  _productTypeService;
         private SystemReferenceService _refService;
         private IChangesHistoryBLL _changesHistoryBll;
         private IWorkflowHistoryBLL workflowHistoryBLL;

        public ProductTypeController(IZaidmExProdTypeBLL exProdTypeBll, IChangesHistoryBLL changesHistoryBll, IWorkflowHistoryBLL workflowHistoryBLL, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.ProductType)
        {
            _exProdTypeBll = exProdTypeBll;
            this._mainMenu = Enums.MenuList.MasterData;
            this._productTypeService = new ProductTypeService();
            this._refService = new SystemReferenceService();
            this._changesHistoryBll = changesHistoryBll;
            this.workflowHistoryBLL = workflowHistoryBLL;
        }

        #region Local Help
        private ProductTypeIndexViewModel GenerateProperties(ProductTypeIndexViewModel source, bool update)
        {            
            var data = source;

            if (!update || data == null)
            {
                data = new ProductTypeIndexViewModel();
            }         

            data.ViewModel.ProdCode = GenerateCodeSequence();

            data.MainMenu = _mainMenu;
            data.CurrentMenu = PageInfo;
            data.IsNotViewer = CurrentUser.UserRole == Enums.UserRole.Administrator;          
            data.ShowActionOptions = data.IsNotViewer;
            data.EditMode = false;
            data.EnableFormInput = true;
            data.ViewModel.IsCreator = false;

            return data;
        }

        private void ExecuteEdit(ProductTypeIndexViewModel model, ReferenceKeys.ApprovalStatus statusApproval, ReferenceKeys.EmailContent emailTemplate, bool sendEmail = false)
        {
            try
            {
                var obj = model.ViewModel;
                var data = _productTypeService.Find(model.ViewModel.ProdCode);
                var old = Mapper.Map<ProductTypeFormViewModel>(data);

                obj.CreatedBy = old.CreatedBy;
                obj.CreatedDate = old.CreatedDate;
                               
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

                if (obj.IsDeleted == true)
                {
                    obj.IsDeleted = false;
                }
                else
                {
                    obj.IsDeleted = true;
                }

                obj.ModifiedBy = CurrentUser.USER_ID;
                obj.ModifiedDate = DateTime.Now;
                obj.ApprovalStatus = _refService.GetReferenceByKey(statusApproval).REFF_ID;
                model.ViewModel = obj;

                var parameters = new Dictionary<string, string>();

                parameters.Add("product_type", data.PRODUCT_TYPE);
                parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy")); // without time
                //parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy hh:mm:ss")); // with time
                parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                parameters.Add("approval_status", data.APPROVALSTATUS.REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "ProductType", new { id = data.PROD_CODE }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "ProductType", new { id = data.PROD_CODE }, this.Request.Url.Scheme));

                bool success = _productTypeService.Edit(Mapper.Map<CustomService.Data.MASTER_PRODUCT_TYPE>(obj), (int)Enums.MenuList.ProductType, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                if (success)
                {
                    if (sendEmail)
                    {
                        var mailContent = _refService.GetMailContent((int)emailTemplate, parameters);
                        var sender = _refService.GetUserEmail(CurrentUser.USER_ID);
                        var display = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminCreator);
                        var sendToId = _refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover).REFF_VALUE;
                        var sendTo = _refService.GetUserEmail(sendToId);
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
        //
        // GET: /ProductType/
        public ActionResult Index()
        {           
            var data = new ProductTypeIndexViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Controller),
                ListProductTypes = Mapper.Map<List<ProductTypeFormViewModel>>(
               _productTypeService.GetAll().OrderByDescending(item => item.PROD_CODE)),
                IsAdminApprover = _refService.IsAdminApprover(CurrentUser.USER_ID)
            };

            var list = new List<ProductTypeFormViewModel>(data.ListProductTypes);

            data.ListProductTypes = new List<ProductTypeFormViewModel>();

            var approvalStatusApproved = _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
            var approvalStatusSubmitted = _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;

            foreach (var item in list)
            {
                item.IsCreator = CurrentUser.USER_ID == item.CreatedBy;
                item.IsApproved = item.ApprovalStatus == approvalStatusApproved;
                item.IsSubmitted = item.ApprovalStatus == approvalStatusSubmitted;
                data.ListProductTypes.Add(item);
            }
            return View("Index", data);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {           
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var data = GenerateProperties(null, false);

            return View(data);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(ProductTypeIndexViewModel model)
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }

                var obj = model.ViewModel;
                if (obj.IsDeleted == true)
                {
                    obj.IsDeleted = false;
                }
                else
                {
                    obj.IsDeleted = true;
                }
                obj.CreatedBy = CurrentUser.USER_ID;
                obj.CreatedDate = DateTime.Now;
                obj.ModifiedBy = CurrentUser.USER_ID;
                obj.ModifiedDate = DateTime.Now;
                model.ViewModel = obj;

                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                }
                else
                {
                    obj.ApprovalStatus = _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                    
                    var inserted = _productTypeService.Create(Mapper.Map<CustomService.Data.MASTER_PRODUCT_TYPE>(obj), (int)Enums.MenuList.ProductType, (int)Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID);

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

        #region Update

        public ActionResult Edit(string id)
        {            
            var data = GenerateProperties(null, false);
            var obj = _productTypeService.Find(id);
            var approvalStatusSubmitted = _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
            var approvalStatusApproved = _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
      
            var history = this._changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductType, id);            
         

            data.ViewModel = Mapper.Map<ProductTypeFormViewModel>(obj);
          
            data.ViewModel.IsCreator = true;
            data.ViewModel.IsSubmitted = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusSubmitted;
            data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;

            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
         

            if (data.ViewModel.IsDeleted == true)
            {
                data.ViewModel.IsDeleted = false;
            }
            else
            {
                data.ViewModel.IsDeleted = true;
            }

      
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ProductTypeIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                model = GenerateProperties(model, true);
                //var obj = service.Find(Convert.ToInt64(id));
                model.EnableFormInput = true;
                model.EditMode = true;
                model.ViewModel.IsCreator = CurrentUser.USER_ID == model.ViewModel.CreatedBy;
                             
                var history = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductType, model.ViewModel.ProdCode.ToString()).ToList();
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                model.WorkflowHistory = GetWorkflowHistory(Int64.Parse(model.ViewModel.ProdCode));

                return View("Edit", model);

            }
            //if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            //{
            //    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
            //    return RedirectToAction("Index");
            //}
            ExecuteEdit(model, ReferenceKeys.ApprovalStatus.Edited, ReferenceKeys.EmailContent.ProductTypeApprovalRequest);
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(ProductTypeIndexViewModel model)
        {
            try
            {
                var data = _productTypeService.Find(model.ViewModel.ProdCode);
                var sender = _refService.GetUserEmail(CurrentUser.USER_ID);
                //    var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminCreator), data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.Admin), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);                
                var parameters = new Dictionary<string, string>();

                parameters.Add("product_type", data.PRODUCT_TYPE);
                //parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy hh:mm:ss")); -->> with time
                parameters.Add("date", DateTime.Now.ToString("dddd, MMM dd yyyy")); // without time
                parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                parameters.Add("submitter", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "ProductType", new { id = data.PROD_CODE }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "ProductType", new { id = data.PROD_CODE }, this.Request.Url.Scheme));

                var mailContent = _refService.GetMailContent((int)ReferenceKeys.EmailContent.ProductTypeApprovalRequest, parameters);
                var reff = _refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
                var sendToId = reff.REFF_VALUE;
                var sendTo = _refService.GetUserEmail(sendToId);

                ExecuteApprovalAction(model, ReferenceKeys.ApprovalStatus.AwaitingAdminApproval, Enums.ActionType.WaitingForApproval, mailContent.EMAILCONTENT, mailContent.EMAILSUBJECT, sender, display, sendTo);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Submit Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
          
            return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public ActionResult Detail(string id)
        {         
            var data = GenerateProperties(null, false);

            data.ViewModel = Mapper.Map<ProductTypeFormViewModel>(_productTypeService.Find(id));

            if (data.ViewModel.IsDeleted == true)
            {
                data.ViewModel.IsDeleted = false;
            }
            else
            {
                data.ViewModel.IsDeleted = true;
            }

            //var history = _refService.GetChangesHistory((int)Enums.MenuList.ProductType, id).ToList();
            var history = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductType, id);
            //var workflow = _refService.GetWorkflowHistory((int)Enums.MenuList.ProductType, Int64.Parse(id)).ToList();

            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
            data.EnableFormInput = false;
            data.EditMode = true;            

            return View(data);
        }

        #endregion

        #region Approval

        private void ExecuteApprovalAction(ProductTypeIndexViewModel model, ReferenceKeys.ApprovalStatus statusApproval, Enums.ActionType actionType, string email, string subject, string sender, string display, string sendTo)
        {
            var comment = (model.ViewModel.RevisionData != null) ? model.ViewModel.RevisionData.Comment : null;
            var updated = _productTypeService.ChangeStatus(model.ViewModel.ProdCode, statusApproval, (int)Enums.MenuList.ProductType, (int)actionType, (int)CurrentUser.UserRole, CurrentUser.USER_ID, comment);

            if (updated != null)
            {
                AddMessageInfo(Constans.SubmitMessage.Updated + " and sending email", Enums.MessageInfoType.Success);
                List<string> mailAddresses = new List<string>();
                if (statusApproval == ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                {
                    var approvers = _refService.GetAdminApprovers().ToList();
                    foreach (var appr in approvers)
                    {
                        var _email = _refService.GetUserEmail(appr.REFF_VALUE.Trim());
                        if (!string.IsNullOrEmpty(_email))
                        {
                            mailAddresses.Add(_email);
                        }
                    }
                }
                else
                {
                    var admins = _refService.GetAdmins().ToList();
                    foreach (var adm in admins)
                    {
                        var _email = _refService.GetUserEmail(adm.USER_ID);
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
                data.ViewModel = Mapper.Map<ProductTypeFormViewModel>(_productTypeService.Find(id));

                var history = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.ProductType, id);            
                data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
                data.EnableFormInput = false;
                data.EditMode = true;                
                data.IsAdminApprover = _refService.IsAdminApprover(CurrentUser.USER_ID);
                if (data.ViewModel.IsDeleted == true)
                {
                    data.ViewModel.IsDeleted = false;
                }
                else
                {
                    data.ViewModel.IsDeleted = true;
                }
                var approvalStatusApproved = _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;

                data.ViewModel.IsCreator = true;
                data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;
                if (data.ViewModel.IsApproved)
                {
                    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);                    
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
                    CssClass = " approve-modal producttype",
                    Id = "ProductTypeApproveModal",
                    Message = String.Format("You are going to approve Product Type data. Are you sure?", data.ViewModel.ProductType),
                    Title = "Approve Confirmation",
                    ModalLabel = "ApproveModalLabel"

                };
                data.ViewModel.RevisionData = new WorkflowHistory()
                {
                    FormID = Convert.ToInt64(id),
                    FormTypeID = (int)Enums.MenuList.ProductType,
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
        public ActionResult Approve(ProductTypeIndexViewModel model)
        {
            try
            {
                var data = _productTypeService.Find(model.ViewModel.ProdCode);
                var parameters = new Dictionary<string, string>();

                parameters.Add("product_type", data.PRODUCT_TYPE);                
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy")); // without time
                //parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy hh:mm:ss")); // with time
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "ProductType", new { id = data.PROD_CODE }, this.Request.Url.Scheme));

                var mailContent = _refService.GetMailContent((int)ReferenceKeys.EmailContent.ProductTypeApproved, parameters);
                var sender = _refService.GetUserEmail(CurrentUser.USER_ID);
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
        
        public ActionResult Revise(ProductTypeIndexViewModel model)
        {         
            try
            {
                var data = _productTypeService.Find(model.ViewModel.ProdCode);
                var parameters = new Dictionary<string, string>();

                parameters.Add("product_type", data.PRODUCT_TYPE);                
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy")); // without time
                //parameters.Add("date", DateTime.Now.ToString("dddd, dd MMM yyyy hh:mm:ss")); // with time
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Rejected).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "ProductType", new { id = data.PROD_CODE }, this.Request.Url.Scheme));
                parameters.Add("url_edit", Url.Action("Edit", "ProductType", new { id = data.PROD_CODE }, this.Request.Url.Scheme));
                parameters.Add("remark", model.ViewModel.RevisionData.Comment);

                var mailContent = _refService.GetMailContent((int)ReferenceKeys.EmailContent.ProductTypeRejected, parameters);
                var sender = _refService.GetUserEmail(CurrentUser.USER_ID);
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

        #region Helper
        List<WorkflowHistoryViewModel> GetWorkflowHistory(long id)
        {
            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.ProductType;
            var workflow = this.workflowHistoryBLL.GetByFormTypeAndFormId(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID);

            var submittedStatus = _refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval);
            var workflowList = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
            var fratio = _productTypeService.Find(id.ToString());
            if (fratio == null)
            {
                throw new Exception("Specified product type data not found!");
            }

            if (fratio.APPROVED_STATUS == submittedStatus.REFF_ID)
            {
                var additional = _refService.GetAdminApproverList();
                workflowList.Add(Mapper.Map<WorkflowHistoryViewModel>(additional));
            }

            return workflowList;

        }

        [HttpPost]
        public JsonResult IsExist(string type, string alias)
        {
            var result = _productTypeService.IsExist(type, alias);
            JObject obj = new JObject()
            {
                new JProperty("exist", result != null),
                new JProperty("detail", (result != null) ? new JObject(
                    new JProperty("Code", result.PROD_CODE),
                    new JProperty("Type", result.PRODUCT_TYPE),
                    new JProperty("Alias", result.PRODUCT_ALIAS)
                    ) : null)
            };
            return Json(obj.ToString());
        }

        public string GenerateCodeSequence()
        {
            var lastData = _productTypeService.GetLastRecord();
            var code = Int16.Parse(lastData.PROD_CODE);
            var newCode = code + 1;

            string prefix = "0";
            string finalCode = "";

            if (newCode < 10)
            {
                finalCode = prefix + newCode.ToString();
            }
            else
            {
                finalCode = newCode.ToString();
            }

            return finalCode;
        }
        #endregion
    }
}