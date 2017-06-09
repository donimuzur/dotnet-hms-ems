using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    /// <summary>
    /// This class serve as controller for module Finance Ratio (~/FinanceRatio)
    /// 
    /// </summary>
    public class FinanceRatioController : BaseController
    {
        private Enums.MenuList mainMenu;
        private FinanceRatioManagementService service;
        private SystemReferenceService refService;
        private IChangesHistoryBLL changeHistoryBLL;
        private IWorkflowHistoryBLL workflowHistoryBLL;
        public FinanceRatioController(IPageBLL pageBLL, IChangesHistoryBLL changeHistoryBLL, IWorkflowHistoryBLL workflowHistory)
            : base(pageBLL, Enums.MenuList.FinanceRatio)
        {
            this.mainMenu = Enums.MenuList.MasterData;
            this.service = new FinanceRatioManagementService();
            this.refService = new SystemReferenceService();
            this.changeHistoryBLL = changeHistoryBLL;
            this.workflowHistoryBLL = workflowHistory;
        }

        #region Local Helpers
        /// <summary>
        /// Local helper method to help regenerate properties neccessary when navigating between actions
        /// </summary>
        /// <typeparam name="FinanceRatioViewModel">The type of view model object</typeparam>
        /// <typeparam name="bool">The type of update flag</typeparam>
        /// <param name="source">Old data of new generated properties, set it null will automatically set null all properties other than SelectList items</param>
        /// <param name="update">Boolean value that indicates the view model is going to update or reset. Set true</param>
        /// <returns>New view model data to bind to target view</returns>
        private FinanceRatioViewModel GenerateProperties(FinanceRatioViewModel source, bool update)
        {
            var companyList = service.GetCompanies().Select(item => new CompanyModel()
            {
                Id = item.BUKRS,
                Name = item.BUTXT
            });
            var temp = 0; // default value
            var span = Int32.TryParse(ConfigurationManager.AppSettings["YearSpanLength"], out temp) ? temp : 50;
            var periods = new List<int>();
            for (int i = DateTime.Now.Year - span; i < DateTime.Now.Year + span; i++)
            {
                periods.Add(i);
            }
            var yearPeriods = from item in periods
                              select new SelectListItem()
                              {
                                  Value = item.ToString(),
                                  Text = item.ToString()
                              };
            var data = source;

            if (!update || data == null)
            {
                data = new FinanceRatioViewModel();
            }
            data.MainMenu = mainMenu;
            data.CurrentMenu = PageInfo;
            data.IsNotViewer = CurrentUser.UserRole == Enums.UserRole.Administrator;
            data.CompanyList = GenericHelpers<CompanyModel>.GenerateList(companyList, item => item.Id, item => item.Name);
            data.YearPeriods = new SelectList(yearPeriods
                .GroupBy(item => item.Value)
                .Select(group => group.First()), "Value", "Text");

            // set default data
            data.ViewModel.YearPeriod = DateTime.Now.Year;
            data.ShowActionOptions = data.IsNotViewer;
            data.EditMode = false;
            data.EnableFormInput = true;
            data.ViewModel.IsCreator = false;
            return data;
        }

        private void ExecuteEdit(FinanceRatioViewModel model, ReferenceKeys.ApprovalStatus statusApproval, ReferenceKeys.EmailContent emailTemplate, bool sendEmail = false)
        {
            try
            {
                var obj = model.ViewModel;
                var data = service.Find(model.ViewModel.Id);
                var old = Mapper.Map<FinanceRatioModel>(data);
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
                parameters.Add("period", data.YEAR_PERIOD.ToString());
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "FinanceRatio", new { id = data.FINANCERATIO_ID }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "FinanceRatio", new { id = data.FINANCERATIO_ID }, this.Request.Url.Scheme));


                bool success = service.Edit(Mapper.Map<CustomService.Data.MASTER_FINANCIAL_RATIO>(obj), (int)Enums.MenuList.FinanceRatio, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                if (success)
                {
                    if (sendEmail)
                    {
                        var mailContent = refService.GetMailContent((int)emailTemplate, parameters);
                        var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                        var display = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.Admin);
                        var sendToId = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover).REFF_VALUE;
                        var sendTo = refService.GetUserEmail(sendToId);
                        //AddMessageInfo(Constans.SubmitMessage.Updated + " and sending email", Enums.MessageInfoType.Success);
                        bool mailStatus = ItpiMailer.Instance.SendEmail(new string[] { sendTo }, null, null, null, mailContent.EMAILSUBJECT, mailContent.EMAILCONTENT, true, sender, display);
                        if (!mailStatus)
                        {
                            AddMessageInfo(Constans.SubmitMessage.Updated + " but failed to send email", Enums.MessageInfoType.Success);
                        }
                        else
                        {
                            AddMessageInfo(Constans.SubmitMessage.Updated + " and successfully send email", Enums.MessageInfoType.Success);
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

        List<WorkflowHistoryViewModel> GetWorkflowHistory(long id)
        { 
            
            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.FinanceRatio;
            var workflow = this.workflowHistoryBLL.GetByFormTypeAndFormId(workflowInput).OrderByDescending(x => x.WORKFLOW_HISTORY_ID);

            return Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

        }
        #endregion
        /// <summary>
        /// Action to display main page module of Finance Ratio
        /// </summary>
        /// <returns>The action result that will handled by HTTP request</returns>
        public ActionResult Index()
        {
            var data = new FinanceRatioViewModel()
            {
                MainMenu = mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole == Enums.UserRole.Administrator ? true : false),
                ListFinanceRatios = Mapper.Map<List<FinanceRatioModel>>(
                service.GetAll().OrderByDescending(item => item.BUKRS).ThenByDescending(item => item.YEAR_PERIOD)),
                IsAdminApprover = refService.IsAdminApprover(CurrentUser.USER_ID)
            };
            var list = new List<FinanceRatioModel>(data.ListFinanceRatios);
            data.ListFinanceRatios = new List<FinanceRatioModel>();
            var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
            var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
            foreach (var item in list)
            {
                item.IsCreator = CurrentUser.USER_ID == item.CreatedBy;
                item.IsApproved = /*false; */ item.ApprovalStatus == approvalStatusApproved;
                item.IsSubmitted = /*false; */item.ApprovalStatus == approvalStatusSubmitted;
                data.ListFinanceRatios.Add(item);
            }
            return View("Index", data);
        }




        #region Create Action
        /// <summary>
        /// Action to displays create form if current user has the authority, otherwise thrown back to main page
        /// </summary>
        /// <returns>The action result that will handled by HTTP request</returns>
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
        public ActionResult Create(FinanceRatioViewModel model)
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
                    var inserted = service.Create(Mapper.Map<CustomService.Data.MASTER_FINANCIAL_RATIO>(obj), (int)Enums.MenuList.FinanceRatio, (int)Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID);

                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                    );
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

        #region Detail Action
        public ActionResult Detail(string id)
        {
            var data = GenerateProperties(null, false);
            data.ViewModel = Mapper.Map<FinanceRatioModel>(service.Find(Convert.ToInt64(id)));
            var history = this.changeHistoryBLL.GetByFormTypeAndFormId(Enums.MenuList.FinanceRatio, id);
            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
            data.EnableFormInput = false;
            data.EditMode = true;
            data.ViewModel = this.FormatCurrency(data.ViewModel);

            return View(data);
        }
        #endregion

        #region Edit Action
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
            //if (obj.CREATED_BY != CurrentUser.USER_ID)
            //{
            //    AddMessageInfo("Operation not allowed!. You are not the creator of this entry", Enums.MessageInfoType.Error);
            //    //data = GenerateProperties(data, true);
            //    RedirectToAction("Index");
            //}
            data.ViewModel = Mapper.Map<FinanceRatioModel>(obj);
            data.ViewModel.IsCreator = CurrentUser.USER_ID == data.ViewModel.CreatedBy;
            data.ViewModel.IsSubmitted = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusSubmitted;
            data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;
            //if (data.ViewModel.IsApproved)
            //{
            //    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);
            //    //data = GenerateProperties(data, true);
            //    RedirectToAction("Index");
            //}

            if (data.ViewModel.IsSubmitted)
            {
                AddMessageInfo("Operation not allowed!. This entry already submitted!", Enums.MessageInfoType.Error);
                //data = GenerateProperties(data, true);
                RedirectToAction("Index");
            }

            data.EnableFormInput = true;
            data.EditMode = true;
            var history =  this.changeHistoryBLL.GetByFormTypeAndFormId(Enums.MenuList.FinanceRatio, id);
            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            data.WorkflowHistory = GetWorkflowHistory(Convert.ToInt64(id));
            return View("Edit", data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(FinanceRatioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                model = GenerateProperties(model, true);
                //var obj = service.Find(Convert.ToInt64(id));
                model.EnableFormInput = true;
                model.EditMode = true;
                model.ViewModel.IsCreator = CurrentUser.USER_ID == model.ViewModel.CreatedBy;
                var history = this.changeHistoryBLL.GetByFormTypeAndFormId(Enums.MenuList.FinanceRatio, model.ViewModel.Id.ToString());
                
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                return View("Edit", model);

            }
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            ExecuteEdit(model, ReferenceKeys.ApprovalStatus.Edited, ReferenceKeys.EmailContent.FinanceRatioApprovalRequest);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(FinanceRatioViewModel model)
        {
            try
            {
                
                var data = service.Find(model.ViewModel.Id);
                var sender = refService.GetUserEmail(CurrentUser.USER_ID);
                var display = String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.Admin), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME);
                var parameters = new Dictionary<string, string>();
                parameters.Add("company", data.COMPANY.BUTXT);
                parameters.Add("period", data.YEAR_PERIOD.ToString());
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                parameters.Add("submitter", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "FinanceRatio", new { id = data.FINANCERATIO_ID }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "FinanceRatio", new { id = data.FINANCERATIO_ID }, this.Request.Url.Scheme));
                var mailContent = refService.GetMailContent((int)ReferenceKeys.EmailContent.FinanceRatioApprovalRequest, parameters);
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

        #region Approve Action
        private void ExecuteApprovalAction(FinanceRatioViewModel model, ReferenceKeys.ApprovalStatus statusApproval, Enums.ActionType actionType, string email, string subject, string sender, string display, string sendTo)
        {
            var comment = (model.ViewModel.RevisionData != null) ? model.ViewModel.RevisionData.Comment : null;
            var updated = service.ChangeStatus(model.ViewModel.Id, statusApproval, (int)Enums.MenuList.FinanceRatio, (int)actionType, (int)CurrentUser.UserRole, CurrentUser.USER_ID, comment);

            if (updated != null)
            {
                
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
                    AddMessageInfo(Constans.SubmitMessage.Updated + " but failed to send email", Enums.MessageInfoType.Success);
                }
                else
                {
                    AddMessageInfo(Constans.SubmitMessage.Updated + " and successfully send email", Enums.MessageInfoType.Success);
                }
            }
        }
        public ActionResult Approve(string id)
        {
            try
            {
                var data = GenerateProperties(null, false);
                data.ViewModel = Mapper.Map<FinanceRatioModel>(service.Find(Convert.ToInt64(id)));
                data.EnableFormInput = false;
                data.EditMode = true;
                data.ViewModel = this.FormatCurrency(data.ViewModel);
                data.IsAdminApprover = refService.IsAdminApprover(CurrentUser.USER_ID);
                var approvalStatusApproved= refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
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
                        CssClass = "btn btn-blue",
                        Label = "Approve"
                    },
                    CssClass = " approve-modal financeratio",
                    Id = "FinancialRatioApproveModal",
                    Message = String.Format("You are going to approve {0} financial ratio data for {1} period. Are you sure?", data.ViewModel.Company.Name, data.ViewModel.YearPeriod),
                    Title = "Approve Confirmation",
                    ModalLabel = "ApproveModalLabel"

                };
                data.ViewModel.RevisionData = new WorkflowHistory()
                {
                    FormID = Convert.ToInt64(id),
                    FormTypeID = (int)Enums.MenuList.FinanceRatio,
                    Action = (int)Enums.ActionType.Reject,
                    ActionBy = CurrentUser.USER_ID,
                    Role = (int)CurrentUser.UserRole
                };
                var history = this.changeHistoryBLL.GetByFormTypeAndFormId(Enums.MenuList.FinanceRatio, data.ViewModel.Id.ToString());

                data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                data.WorkflowHistory = GetWorkflowHistory(data.ViewModel.Id);
                return View("Approve", data);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Submit Failed : " + ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Approve(FinanceRatioViewModel model)
        {
            try
            {
                var data = service.Find(model.ViewModel.Id);
                
                var parameters = new Dictionary<string, string>();
                parameters.Add("company", data.COMPANY.BUTXT);
                parameters.Add("period", data.YEAR_PERIOD.ToString());
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "FinanceRatio", new { id = data.FINANCERATIO_ID }, this.Request.Url.Scheme));
                
                var mailContent = refService.GetMailContent((int)ReferenceKeys.EmailContent.FinanceRatioApproved, parameters);

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

        public ActionResult Revise(FinanceRatioViewModel model)
        {
            try
            {
                var data = service.Find(model.ViewModel.Id);

                var parameters = new Dictionary<string, string>();
                parameters.Add("company", data.COMPANY.BUTXT);
                parameters.Add("period", data.YEAR_PERIOD.ToString());
                parameters.Add("date", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                parameters.Add("approver", String.Format("{0} {1}", CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Rejected).REFF_VALUE);
                parameters.Add("url_detail", Url.Action("Detail", "FinanceRatio", new { id = data.FINANCERATIO_ID }, this.Request.Url.Scheme));
                parameters.Add("url_edit", Url.Action("Edit", "FinanceRatio", new { id = data.FINANCERATIO_ID }, this.Request.Url.Scheme));
                parameters.Add("remark", model.ViewModel.RevisionData.Comment);
                var mailContent = refService.GetMailContent((int)ReferenceKeys.EmailContent.FinanceRatioRejected, parameters);

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

        [HttpPost]
        public JsonResult IsExist(string company, string period)
        {
            var result = service.IsExist(company, ConversionHelper.ToInt32OrDefault(period).Value);
            var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
            JObject obj = new JObject()
            {
                new JProperty("exist", result != null),
                new JProperty("detail", (result != null) ? new JObject(
                    new JProperty("Id", result.FINANCERATIO_ID),
                    new JProperty("YearPeriod", result.YEAR_PERIOD),
                    new JProperty("Company", result.BUKRS),
                    new JProperty("Submitted", result.STATUS_APPROVAL == approvalStatusSubmitted)
                    ) : null)
            };
            return Json(obj.ToString());
        }

        private FinanceRatioModel FormatCurrency(FinanceRatioModel src)
        {
            src.CurrentAssetsDisplay = String.Format("{0:N}", src.CurrentAssets);
            src.CurrentDebtsDisplay = String.Format("{0:N}", src.CurrentDebts);
            src.TotalAssetsDisplay = String.Format("{0:N}", src.TotalAssets);
            src.TotalDebtsDisplay = String.Format("{0:N}", src.TotalDebts);
            src.TotalCapitalString = String.Format("{0:N}", src.TotalCapital);
            src.NetProfitDisplay = String.Format("{0:N}", src.NetProfit);
            return src;
        }

        #endregion
    }
}