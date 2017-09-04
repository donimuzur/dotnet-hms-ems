using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.ProductType;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.Tariff;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class TariffController : BaseController
    {
        private Enums.MenuList mainMenu;
        private TariffManagementService service;
        private SystemReferenceService refService;
        private IWorkflowHistoryBLL workflowBll;
        private IChangesHistoryBLL changeHistoryBll;
        public TariffController(IPageBLL pageBLL, IChangesHistoryBLL changeHistoryBll, IWorkflowHistoryBLL workflowBll)
            : base(pageBLL, Enums.MenuList.Tariff)
        {
            this.mainMenu = Enums.MenuList.MasterData;
            this.service = new TariffManagementService();
            this.refService = new SystemReferenceService();
            this.changeHistoryBll = changeHistoryBll;
            this.workflowBll = workflowBll;
        }

        #region Local Helpers
        /// <summary>
        /// Local helper method to help regenerate properties neccessary when navigating between actions
        /// </summary>
        /// <typeparam name="TariffViewModel">The type of view model object</typeparam>
        /// <typeparam name="bool">The type of update flag</typeparam>
        /// <param name="source">Old data of new generated properties, set it null will automatically set null all properties other than SelectList items</param>
        /// <param name="update">Boolean value that indicates the view model is going to update or reset. Set true</param>
        /// <returns>New view model data to bind to target view</returns>
        private TariffViewModel GenerateProperties(TariffViewModel source, bool update)
        {
            var productTypeList = service.GetProductTypes().Select(item => new ProductTypeFormViewModel()
            {
                ProdCode = item.PROD_CODE,
                ProductType = item.PRODUCT_TYPE
            }).AsQueryable();
            
            var data = source;

            if (!update || data == null)
            {
                data = new TariffViewModel();
            }
            data.MainMenu = mainMenu;
            data.CurrentMenu = PageInfo;
            data.IsNotViewer = CurrentUser.UserRole == Enums.UserRole.Administrator;
            data.ProductTypeList = GenericHelpers<ProductTypeFormViewModel>.GenerateList(productTypeList, item => item.ProdCode, item => item.ProductType);
            data.ShowActionOptions = data.IsNotViewer;
            data.EditMode = false; 
            data.EnableFormInput = true;
            data.ViewModel.IsCreator = false;
            data.ViewModel = this.FormatCurrency(data.ViewModel);
            return data;
        }
        /// <summary>
        /// Method to display execption message
        /// </summary>
        /// <param name="ex">The execption thrown from memory stack</param>
        private void AddExceptionMessage(Exception ex)
        {
            AddMessageInfo(String.Format("Operation Failed!\nMessage: {0}\nStackTrace: {1}\nInnerException: {2}", ex.Message, ex.StackTrace, ex.InnerException), 
                Enums.MessageInfoType.Error);
        }
        /// <summary>
        /// Method to format currency
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private TariffModel FormatCurrency(TariffModel src)
        {
            src.MinimumHjeDisplay = String.Format("{0:N}", src.MinimumHJE);
            src.MaximumHjeDisplay = String.Format("{0:N}", src.MaximumHJE);
            src.TariffDisplay = String.Format("{0:N}", src.Tariff);

            return src;
        }

        List<WorkflowHistoryViewModel> GetWorkflowHistory(long id)
        {

            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.Tariff;
            var workflow = this.workflowBll.GetByFormTypeAndFormId(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID);
            var workflowList = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
            var submittedStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval);

            var tariff = service.Find(id);
            if (tariff == null)
            {
                throw new Exception("Specified tariff not found!");
            }

            if (tariff.STATUS_APPROVAL == submittedStatus.REFF_ID)
            {
                var additional = refService.GetAdminApproverList();
                workflowList.Add(Mapper.Map<WorkflowHistoryViewModel>(additional));
            }



            return workflowList;

        }
        #endregion

        /// <summary>
        /// Action to display main page module of Master Tariff
        /// </summary>
        /// <returns>The action result that will handled by HTTP request</returns>
        public ActionResult Index()
        {
            var data = new TariffViewModel()
            {
                MainMenu = mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole == Enums.UserRole.Administrator ? true : false),
                IsAdminApprover = refService.IsAdminApprover(CurrentUser.USER_ID)
            };
            try
            {
                var tariff = service.GetTariff();
                data.TariffList = AutoMapper.Mapper.Map<List<TariffModel>>(tariff).ToList();
                var list = new List<TariffModel>(data.TariffList);
                data.TariffList = new List<TariffModel>();
                var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
                foreach (var item in list)
                {
                    item.IsCreator = CurrentUser.USER_ID == item.CreatedBy;
                    item.IsApproved = item.ApprovalStatus == approvalStatusApproved;
                    item.IsSubmitted = item.ApprovalStatus == approvalStatusSubmitted;
                    data.TariffList.Add(item);
                }

            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
            }
            return View("Index", data);
        }
        /// <summary>
        /// Action to display detail page module of Master Tariff
        /// </summary>
        /// <param name="id">Specific data to display</param>
        /// <returns>The action result that will handled by HTTP request</returns>

        public ActionResult Detail(string id)
        {
            try
            {
                var data = GenerateProperties(null, false);
                data.ViewModel = Mapper.Map<TariffModel>(service.Find(Convert.ToInt64(id)));
                var history = this.changeHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.Tariff, id);
                //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.Tariff, Int64.Parse(id)).ToList();
                data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                data.WorkflowHistory = GetWorkflowHistory(data.ViewModel.Id);
                data.EnableFormInput = false;
                data.EditMode = true;
                data.ViewModel = this.FormatCurrency(data.ViewModel);

                return View(data);
            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
                return View("Index");
            }
        }
        #region Create Action
        /// <summary>
        /// Action to display create form if current user has the authority, otherwise thrown back to main page
        /// </summary>
        /// <returns>The action result that will handled by HTTP request</returns>
        public ActionResult Create()
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }

                var data = GenerateProperties(null, false);
                data.ViewModel.ValidStartDate = DateTime.Now;
                data.ViewModel.ValidEndDate = DateTime.Now.AddDays(1);
                return View(data);

            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
                return RedirectToAction("Index");
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(TariffViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Model incomplete!", Enums.MessageInfoType.Error);
                    model = GenerateProperties(model, true);
                    return View("Create", model);
                }
                
                var data = model.ViewModel;
                if (data.ValidEndDate < data.ValidStartDate)
                {
                    AddMessageInfo("Valid end date must be later than start date!", Enums.MessageInfoType.Error);
                    model = GenerateProperties(model, true);
                    return View("Create", model);
                }

                if (data.MaximumHJE < data.MinimumHJE)
                {
                    AddMessageInfo("Maximum HJE must be greater than Minimum HJE!", Enums.MessageInfoType.Error);
                    model = GenerateProperties(model, true);
                    return View("Create", model);
                }
                var entity = Mapper.Map<TARIFF>(data);
                service.Save(entity, ReferenceKeys.ApprovalStatus.Draft, Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID, null, null);
                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
                model = GenerateProperties(model, true);
                return View("Create", model);
            }
        }
        #endregion

        #region Edit Action
        /// <summary>
        /// Action to display edit form if current user has the authority, otherwise thrown back to main page 
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            var data = GenerateProperties(null, false);
            var obj = service.Find(Convert.ToInt64(id));
            var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
            var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
            //if (obj.CREATED_BY != CurrentUser.USER_ID)
            //{
            //    AddMessageInfo("Operation not allowed!. You are not the creator of this entry", Enums.MessageInfoType.Error);
            //    //data = GenerateProperties(data, true);
            //    RedirectToAction("Index");
            //}
            data.ViewModel = Mapper.Map<TariffModel>(obj);
            data.ViewModel.IsCreator = true;
            data.ViewModel.IsSubmitted = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusSubmitted;
            data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;
            var history = this.changeHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.Tariff, id);
            //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.Tariff, Int64.Parse(id)).ToList();
            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            data.WorkflowHistory = GetWorkflowHistory(data.ViewModel.Id);
            //if (data.ViewModel.IsApproved)
            //{
            //    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);
            //    //data = GenerateProperties(data, true);
            //    RedirectToAction("Index");
            //}
            if (data.ViewModel.IsSubmitted)
            {
                AddMessageInfo("Operation not allowed!. This entry is awaiting for approval!", Enums.MessageInfoType.Error);
                //data = GenerateProperties(data, true);
                return RedirectToAction("Detail", new { id = data.ViewModel.Id });
            }
            data.EnableFormInput = true;
            data.EditMode = true;
            return View("Edit", data);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(TariffViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Form incomplete!", Enums.MessageInfoType.Error);
                    model = GenerateProperties(model, true);
                    var history = this.changeHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.Tariff, model.ViewModel.Id.ToString());
                    //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.Tariff, model.ViewModel.Id).ToList();
                    model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                    model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                    model.EnableFormInput = true;
                    model.EditMode = true;
                    return View("Edit", model);
                }

                var data = model.ViewModel;
                if (data.ValidEndDate < data.ValidStartDate || data.MaximumHJE < data.MinimumHJE)
                {
                    if (data.ValidEndDate < data.ValidStartDate)
                    {
                        AddMessageInfo("Valid end date must be later than start date!", Enums.MessageInfoType.Error);
                    }
                    if (data.MaximumHJE < data.MinimumHJE)
                    {
                        AddMessageInfo("Maximum HJE must be greater than Minimum HJE!", Enums.MessageInfoType.Error);
                    }
                    var history = refService.GetChangesHistory((int)Enums.MenuList.Tariff, model.ViewModel.Id.ToString()).ToList();
                    //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.Tariff, model.ViewModel.Id).ToList();
                    model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                    model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id) ;
                    model = GenerateProperties(model, true);
                    model.EnableFormInput = true;
                    model.EditMode = true;
                    return View("Edit", model);
                }
                var entity = Mapper.Map<TARIFF>(data);
                service.Save(entity, ReferenceKeys.ApprovalStatus.Edited, Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID, null, null);
                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
                model = GenerateProperties(model, true);
                model.EnableFormInput = true;
                model.EditMode = true;
                return View("Edit", model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(TariffViewModel model)
        {
            try
            {

                var entity = service.Find(model.ViewModel.Id);
                bool emailStatus = service.Save(entity, ReferenceKeys.ApprovalStatus.AwaitingAdminApproval, Enums.ActionType.Submit, (int)CurrentUser.UserRole, CurrentUser.USER_ID, refService.GetUserEmail(CurrentUser.USER_ID), String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.Admin), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME ));
                if(emailStatus)
                    AddMessageInfo(Constans.SubmitMessage.Updated + " and successfully sending email!", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo(Constans.SubmitMessage.Updated + " and failed sending email!", Enums.MessageInfoType.Success);

            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Approval
        public ActionResult Approve(string id)
        {
            try
            {
                var data = GenerateProperties(null, false);
                data.ViewModel = Mapper.Map<TariffModel>(service.Find(Convert.ToInt64(id)));
                data.EnableFormInput = false;
                data.EditMode = true;
                data.ViewModel = this.FormatCurrency(data.ViewModel);
                data.IsAdminApprover = refService.IsAdminApprover(CurrentUser.USER_ID);
                var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                data.ViewModel.IsCreator = true;
                data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;
                if (data.ViewModel.IsApproved)
                {
                    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);
                    return RedirectToAction("Detail", new { id = data.ViewModel.Id });
                }
                data.ApproveConfirm = new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "ApproveButtonConfirm",
                        CssClass = "btn btn-blue",
                        Label = "Approve"
                    },
                    CssClass = " approve-modal tariff",
                    Id = "TariffApproveModal",
                    Message = String.Format("You are going to approve master tariff for {0} ({1}). Are you sure?", data.ViewModel.ProductType.ProductType, data.ViewModel.ProductTypeCode),
                    Title = "Approve Confirmation",
                    ModalLabel = "ApproveModalLabel"

                };
                data.ViewModel.RevisionData = new WorkflowHistory()
                {
                    FormID = Convert.ToInt64(id),
                    FormTypeID = (int)Enums.MenuList.Tariff,
                    Action = (int)Enums.ActionType.Reject,
                    ActionBy = CurrentUser.USER_ID,
                    Role = (int)CurrentUser.UserRole
                };
                var history = refService.GetChangesHistory((int)Enums.MenuList.Tariff, data.ViewModel.Id.ToString()).ToList();
                //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.Tariff, model.ViewModel.Id).ToList();
                data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
                data.WorkflowHistory = GetWorkflowHistory(data.ViewModel.Id);
                return View(data);
            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
                return View("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken, PreventDuplicateRequest]
        public ActionResult Approve(TariffViewModel model)
        {
            try
            {

                var entity = service.Find(model.ViewModel.Id);
                bool emailStatus = service.Save(entity, ReferenceKeys.ApprovalStatus.Completed, Enums.ActionType.Approve, (int)Enums.UserRole.AdminApprover, CurrentUser.USER_ID, refService.GetUserEmail(CurrentUser.USER_ID), String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminApprover), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME));
                if (emailStatus)
                    AddMessageInfo(Constans.SubmitMessage.Updated + " and successfully sending email!", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo(Constans.SubmitMessage.Updated + " and failed sending email!", Enums.MessageInfoType.Success);

            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
            }
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Revise(TariffViewModel model)
        {
            try
            {

                var entity = service.Find(model.ViewModel.Id);
                bool emailStatus = service.Save(entity, ReferenceKeys.ApprovalStatus.Edited, Enums.ActionType.Reject, (int)Enums.UserRole.AdminApprover, CurrentUser.USER_ID, refService.GetUserEmail(CurrentUser.USER_ID), String.Format("{0} [{1} {2}]", ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.EmailSender.AdminApprover), CurrentUser.FIRST_NAME, CurrentUser.LAST_NAME), model.ViewModel.RevisionData.Comment);
                if (emailStatus)
                    AddMessageInfo(Constans.SubmitMessage.Updated + " and successfully send email!", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo(Constans.SubmitMessage.Updated + " but failed to sending email!", Enums.MessageInfoType.Success);

            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex);
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}