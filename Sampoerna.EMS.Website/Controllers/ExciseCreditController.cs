using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Helpers;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.ExciseCredit;
using Sampoerna.EMS.Website.Models.FileUpload;
using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SpreadsheetLight;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ExciseCreditController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        private ExciseCreditService service;
        private IChangesHistoryBLL chBLL;
        private IWorkflowHistoryBLL whBLL;
        public ExciseCreditController(IPageBLL pageBLL, IChangesHistoryBLL changeHistoryBLL, IWorkflowHistoryBLL workflowHistoryBLL) : base(pageBLL, Enums.MenuList.ExciseCredit)
        {
            this.mainMenu = Enums.MenuList.ExciseCredit;
            this.service = new ExciseCreditService();
            this.refService = new SystemReferenceService();
            this.chBLL = changeHistoryBLL;
            this.whBLL = workflowHistoryBLL;
        }

        List<WorkflowHistoryViewModel> GetWorkflowHistory(long id)
        {
            var submittedStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval);
            var waitingGov = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval);
            var excise = service.Find(id);

            // Remarks for change back Approver to common POA
            //var workflowInput = new GetByFormNumberInput();
            //workflowInput.FormId = id;
            //workflowInput.FormType = Enums.FormType.ExciseCredit;
            //if (excise.LAST_STATUS == submittedStatus.REFF_ID)
            //{
            //	workflowInput.DocumentStatus = Enums.DocumentStatus.WaitingForApproval;
            //}
            //workflowInput.FormNumber = excise.EXCISE_CREDIT_NO;
            //workflowInput.DocumentCreator = excise.POA_ID;
            //workflowInput.NppbkcId = refService.GetPOANppbkc(excise.POA_ID);
            //var workflow = this.whBLL.GetByFormNumber(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID).ToList();
            //workflow.Add(this.whBLL.CreateWaitingApprovalRecord(workflowInput));
            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.ExciseCredit;
            var workflow = this.whBLL.GetByFormTypeAndFormId(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID).ToList();
            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

            WORKFLOW_HISTORY additional = new WORKFLOW_HISTORY();
            if (excise.LAST_STATUS == submittedStatus.REFF_ID || excise.LAST_STATUS == waitingGov.REFF_ID)
            {
                var poaApprovers = service.GetApprovers(id);
                var accounts = "";
                foreach (var approver in poaApprovers)
                {
                    if (accounts == "")
                    {
                        accounts += approver;
                    }
                    else
                    {
                        accounts += ", " + approver;
                    }
                }

                additional.ACTION_BY = accounts;
                additional.ACTION = (excise.LAST_STATUS == submittedStatus.REFF_ID) ? (int)Enums.ActionType.WaitingForApproval : (int)Enums.ActionType.WaitingForPOASKEPApproval;
                
                
                additional.ROLE = (int)Enums.UserRole.POA;
                //additional.ACTION_DATE = _CRModel.LastModifiedDate;
                workflowHistory.Add(Mapper.Map<WorkflowHistoryViewModel>(additional));
            }
            return workflowHistory;

        }

        public ActionResult Index(String partial = "Index", List<ExciseCreditModel> data = null)
        {
            try
            {
                var users = refService.GetAllUser();
                var poaList = refService.GetAllPOA();
                var documents = new List<ExciseCreditModel>();
                if (data == null)
                {
                    if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
                    {
                        documents = service.GetAll().Select(x => this.MapExciseCreditModel(x)).ToList();
                    }
                    else
                    {
                        documents = service.GetOpenDocuments(CurrentUser.USER_ID).Select(x => this.MapExciseCreditModel(x)).ToList();
                    }
                }
                else
                {
                    documents = data;
                }
                

                var model = new ExciseCreditViewModel()
                {
                    MainMenu = mainMenu,
                    CurrentMenu = PageInfo,
                    Filter = new ExciseFilterModel(),
                    CreatorList = GetUserList(users),
                    ExciseCreditDocuments = documents,
                    NppbkcList = GetNppbkcList(refService.GetAllNppbkc(CurrentUser.USER_ID)),
                    PoaList = GetPoaList(refService.GetAllPOA()),
                    TypeList = GetExciseCreditTypeList(service.GetExciseCreditTypes()),
                    YearList = GetYearList(documents),
                    IsNotViewer = (CurrentUser.UserRole == Enums.UserRole.Controller || CurrentUser.UserRole == Enums.UserRole.POA)
                };

                return View(partial, model);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Failed to access excise page. Reason: " + ex.Message, Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Unauthorized", "Error");
            }
            

        }

        public ActionResult Completed()
        {
            var documents = new List<ExciseCreditModel>();
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                documents = service.GetAll().Select(x => this.MapExciseCreditModel(x)).ToList();
            }
            else
            {
                documents = service.GetCompletedDocuments(CurrentUser.USER_ID).Select(x => this.MapExciseCreditModel(x)).ToList();
            }
            return Index("Completed", documents);
        }

        public ActionResult SummaryReport()
        {
            var documents = service.GetAll().Select(x => this.MapExciseCreditModel(x)).ToList();
            return Index("SummaryReport", documents);
        }

        public ActionResult Create()
        {
            try
            {
                var model = GenerateModelProperties(null);
                return View("Create", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Create(ExciseCreditFormModel model)
        {
            var formData = Request;
            var modelObj = JObject.Parse(formData["model"]);

            try
            {
                var viewModel = JsonConvert.DeserializeObject<ExciseCreditModel>(modelObj.GetValue("Master").ToString());
                var viewModelDetail = JsonConvert.DeserializeObject<List<ExciseAdjModel>>(modelObj.GetValue("Detail").ToString());
                var ratio = AutoMapper.Mapper.Map<List<FinanceRatioModel>>(service.GetFinancialStatements(viewModel.FinancialRatioIds.Split('_').Select(x => Convert.ToInt64(x)).ToList()));
                if (ratio.Count < 2)
                {
                    AddMessageInfo("Financial ratio data not valid!", Enums.MessageInfoType.Error);
                    return Json(false);
                }
                var draftStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft);
                EXCISE_CREDIT entity = new EXCISE_CREDIT()
                {
                    ADJUSTMENT_CALCULATED = viewModel.CalculatedAdjustment,
                    CREATED_BY = viewModel.POA,
                    CREATED_DATE = DateTime.Now,
                    EXCISE_CREDIT_AMOUNT = Math.Ceiling(viewModel.Amount),
                    GUARANTEE = viewModel.Guarantee,
                    NPPBKC_ID = viewModel.NppbkcId,
                    POA_ID = viewModel.POA,
                    REQUEST_TYPE = viewModel.RequestTypeID,
                    SUBMISSION_DATE = viewModel.SubmissionDate,
                    LIQUIDITY_RATIO_1 = (float)ratio[0].LiquidityRatio,
                    LIQUIDITY_RATIO_2 = (float)ratio[1].LiquidityRatio,
                    RENTABILITY_RATIO_1 = (float)ratio[0].RentabilityRatio,
                    RENTABILITY_RATIO_2 = (float)ratio[1].RentabilityRatio,
                    SOLVENCY_RATIO_1 = (float)ratio[0].SolvencyRatio,
                    SOLVENCY_RATIO_2 = (float)ratio[1].SolvencyRatio,
                    LAST_STATUS = draftStatus.REFF_ID
                };
                //                foreach var 
                List<EXCISE_CREDIT_ADJUST_CALDETAIL> adj = new List<EXCISE_CREDIT_ADJUST_CALDETAIL>();

                foreach (var item in viewModelDetail)
                {

                    EXCISE_CREDIT_ADJUST_CALDETAIL calculate = new EXCISE_CREDIT_ADJUST_CALDETAIL()
                    {
                        BRAND_CE = item.BRAND_CE,
                        CK1_AMOUNT = item.CK1_AMOUNT,
                        INCREASE_TARIFF = item.INCREASE_TARIFF,
                        NEW_TARIFF = item.NEW_TARIFF,
                        OLD_TARIFF = item.OLD_TARIFF,
                        PRODUCT_CODE = item.PRODUCT_CODE,
                        WEIGHTED_INCREASE = item.WEIGHTED_INCREASE
                    };
                    adj.Add(calculate);
                }



                AddMessageInfo("Successfully save Excise Request Document!", Enums.MessageInfoType.Success);
                long json = 0;
                if (viewModelDetail.Any())
                {
                    json = service.SaveAdjustment(entity, adj, CurrentUser.USER_ID, (int) CurrentUser.UserRole);
                }
                else
                {
                    json = service.SaveExcise(entity, CurrentUser.USER_ID, (int)CurrentUser.UserRole);
                }
                return Json(json);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        public ActionResult Detail(string id)
        {
            try
            {
                var model = GenerateModelProperties(null);
                var result = service.Find(Convert.ToInt64(id));
                model.POA = MapPoaModel(result.POA);
                var nppbkc = refService.GetNppbkc(result.NPPBKC_ID);
                var vm = MapExciseCreditModel(result);
                vm.IsCreator = service.IsAllowedToModify(CurrentUser.USER_ID, Convert.ToInt64(id));

                model.NPPBKC = MapNppbkcModel(nppbkc);
                model.SubmissionDate = vm.SubmissionDate;
                model.ViewModel = vm;
                model.FinancialStatements = vm.FinanceRatios.ToArray();
                model.SupportingDocuments = refService.GetSupportingDocuments((int)Enums.FormList.ExciseRequest, nppbkc.COMPANY.BUKRS, id).Select(x => MapSupportingDocumentModel(x)).ToList();
                model.Printouts = GetPrintoutsList(model, result);
                var changeHistoryList = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.ExciseCredit, id);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                return View("Detail", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string id)
        {
            try
            {
                if (CurrentUser.UserRole != Enums.UserRole.Administrator && !service.IsAllowedToModify(CurrentUser.USER_ID, Convert.ToInt64(id)))
                {
                    AddMessageInfo("You are not allowed to modify selected document!", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                var model = GenerateModelProperties(null);
                var result = service.Find(Convert.ToInt64(id));
                model.POA = MapPoaModel(result.POA);
                var nppbkc = refService.GetNppbkc(result.NPPBKC_ID);
                var vm = MapExciseCreditModel(result);
                vm.IsCreator = service.IsAllowedToModify(CurrentUser.USER_ID, Convert.ToInt64(id));
                vm.IsWaitingSkepApproval = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval).REFF_ID == vm.LastStatus;
                vm.Guarantee = service.GetExciseCreditGuaranteeId(result.GUARANTEE).ToString();
                model.NPPBKC = MapNppbkcModel(nppbkc);
                model.SubmissionDate = vm.SubmissionDate;
                model.ViewModel = vm;
                model.FinancialStatements = vm.FinanceRatios.ToArray();
                model.SupportingDocuments = refService.GetSupportingDocuments((int)Enums.FormList.ExciseRequest, nppbkc.COMPANY.BUKRS, id).Select(x => MapSupportingDocumentModel(x)).ToList();
                model.Printouts = GetPrintoutsList(model, result);
                var changeHistoryList = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.ExciseCredit, id);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                return View("Edit", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Edit(ExciseCreditFormModel model)
        {
            try
            {
                var formData = Request.Form;
                var uploadedFiles = Request.Files;
                var submitDate = DateTime.Parse(formData["submit_date"]);
                var id = Int64.Parse(formData["excise_id"]);
                var docNumber = formData["doc_number"];
                var guarantee = formData["guarantee"];
                var existingOtherDocs = JObject.Parse(formData["deleted_docs"]);
                UploadDocuments(uploadedFiles, id.ToString(), docNumber, false);
                foreach (var pair in existingOtherDocs)
                {
                    long fileId = Convert.ToInt64(pair.Key);
                    var detail = (JObject)pair.Value;
                    if (!detail["active"].ToObject<bool>())
                        RemoveUploadedFile(fileId);
                }
                refService.RemoveUploadedFiles(this.RemovedFiles);
                this.RemovedFiles = new List<long>();
                var editStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited);
                AddMessageInfo("Successfully save Excise Request Document!", Enums.MessageInfoType.Success);
                return Json(service.EditExcise(id, submitDate, editStatus, CurrentUser.USER_ID, (int)CurrentUser.UserRole, guarantee));
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(ExciseCreditFormModel model)
        {
            try
            {
                var submitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval);
                var status = service.SubmitExcise(model.ViewModel.Id, submitted, CurrentUser.USER_ID, (int)CurrentUser.UserRole);
                if (status)
                    AddMessageInfo("Submitted Excise Request Document and successfully  send email!", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Submitted Excise Request Document but failed to send email!", Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Failed to submit Excise Request Document", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Cancel(ExciseCreditFormModel model)
        {
            try
            {
                var cancelled = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Canceled);
                var status = service.CancelExcise(model.ViewModel.Id, cancelled, CurrentUser.USER_ID, (int)CurrentUser.UserRole);
                if (status)
                    AddMessageInfo("Canceled Excise Request Document", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Failed to Cancel Excise Request Document", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Failed to Cancel Excise Request Document", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Withdraw(ExciseCreditFormModel model)
        {
            try
            {
                var withdrawed = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited);

                var status = service.WithdrawExcise(model.ViewModel.Id, withdrawed, CurrentUser.USER_ID, (int)CurrentUser.UserRole, model.ViewModel.RevisionData.Comment);
                if (status)
                    AddMessageInfo("Withdrawed Excise Request Document", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Failed to Withdraw Excise Request Document", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Failed to submit Excise Request Document", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Approve(string id)
        {
            try
            {
                if (!service.IsAllowedToApprove(CurrentUser.USER_ID, Convert.ToInt64(id)))
                {
                    AddMessageInfo("You are not allowed to approve selected document!", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                var model = GenerateModelProperties(null);
                var result = service.Find(Convert.ToInt64(id));
                model.POA = MapPoaModel(result.POA);
                var nppbkc = refService.GetNppbkc(result.NPPBKC_ID);
                var vm = MapExciseCreditModel(result);
                vm.IsCreator = service.IsAllowedToModify(CurrentUser.USER_ID, Convert.ToInt64(id));
                vm.IsWaitingSkepApproval = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval).REFF_ID == vm.LastStatus;
                model.NPPBKC = MapNppbkcModel(nppbkc);
                model.SubmissionDate = vm.SubmissionDate;
                model.ViewModel = vm;
                model.FinancialStatements = vm.FinanceRatios.ToArray();
                model.SupportingDocuments = refService.GetSupportingDocuments((int)Enums.FormList.ExciseRequest, nppbkc.COMPANY.BUKRS, id).Select(x => MapSupportingDocumentModel(x)).ToList();
                model.Printouts = GetPrintoutsList(model, result);
                var changeHistoryList = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.ExciseCredit, id);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                model.ApproveConfirm = new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "ApproveButtonConfirm",
                        CssClass = "btn btn-blue",
                        Label = "Approve"
                    },
                    CssClass = " approve-modal excisecredit",
                    Id = "TariffApproveModal",
                    Message = String.Format("You are going to approve Excise Credit for {0}. Are you sure?", model.ViewModel.DocumentNumber),
                    Title = "Approve Confirmation",
                    ModalLabel = "ApproveModalLabel"

                };
                model.ViewModel.RevisionData = new WorkflowHistory()
                {
                    FormID = Convert.ToInt64(id),
                    FormTypeID = (int)Enums.MenuList.ExciseCredit,
                    Action = (int)Enums.ActionType.Reject,
                    ActionBy = CurrentUser.USER_ID,
                    Role = (int)CurrentUser.UserRole
                };
                return View("Approve", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Cannot Load Form Data!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Approve(ExciseCreditFormModel model)
        {
            try
            {
                var approved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval);
                var status = service.ApproveExcise(model.ViewModel.Id, approved, CurrentUser.USER_ID, (int)CurrentUser.UserRole);
                if (status)
                    AddMessageInfo("Approved Excise Request Document and successfully  send email!", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Approved Excise Request Document but failed to send email!", Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Failed to submit Excise Request Document", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Revise(ExciseCreditFormModel model)
        {
            try
            {
                var submitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited);
                var status = service.RejectExcise(model.ViewModel.Id, submitted, CurrentUser.USER_ID, (int)CurrentUser.UserRole, model.ViewModel.RevisionData.Comment);
                if (status)
                    AddMessageInfo("Rejected Excise Request Document and successfully  send email!", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Rejected Excise Request Document but failed to send email!", Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Failed to submit Excise Request Document", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }
        public ActionResult InputSkep(long id = 0)
        {
            try
            {
                if (!service.IsAllowedToModify(CurrentUser.USER_ID, id))
                {
                    AddMessageInfo("You are not allowed to modify selected document!", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                var model = GenerateModelProperties(null);
                var result = service.Find(Convert.ToInt64(id));
                model.POA = MapPoaModel(result.POA);
                var nppbkc = refService.GetNppbkc(result.NPPBKC_ID);
                var vm = MapExciseCreditModel(result);
                vm.IsCreator = service.IsAllowedToModify(CurrentUser.USER_ID, Convert.ToInt64(id));

                model.NPPBKC = MapNppbkcModel(nppbkc);
                model.SubmissionDate = vm.SubmissionDate;
                model.ViewModel = vm;
                model.FinancialStatements = vm.FinanceRatios.ToArray();
                model.SupportingDocuments = refService.GetSupportingDocuments((int)Enums.FormList.ExciseRequest, nppbkc.COMPANY.BUKRS, id.ToString()).Select(x => MapSupportingDocumentModel(x)).ToList();
                model.ViewModel.RevisionData = new WorkflowHistory()
                {
                    FormID = Convert.ToInt64(id),
                    FormTypeID = (int)Enums.MenuList.ExciseCredit,
                    Action = (int)Enums.ActionType.Withdraw,
                    ActionBy = CurrentUser.USER_ID,
                    Role = (int)CurrentUser.UserRole
                };
                model.Printouts = GetPrintoutsList(model, result);
                var changeHistoryList = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.ExciseCredit, id.ToString());
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                var productAliases = service.GetCK1ProductTypes(result.NPPBKC_ID, result.SUBMISSION_DATE).ToList();
                var productTypes = service.GetProductTypes(productAliases).ToList();
                Dictionary<string, string> list = new Dictionary<string, string>();
                foreach (var prod in productTypes)
                {
                    list.Add(prod.PROD_CODE, String.Format("{0}", prod.PRODUCT_ALIAS, prod.PRODUCT_TYPE));
                }
                var skepModel = new ExciseGovApprovalModel();

                skepModel.Id = id;
                skepModel.AvailableProductTypes = this.GetExciseCreditProductTypes(list);
                skepModel.ApprovedProducts = LoadApprovedProducts(result);
                skepModel.BpjDate = (result.BPJ_DATE.HasValue) ? result.BPJ_DATE.Value : DateTime.Today;
                skepModel.BpjDocument = result.BPJ_ATTACH;
                skepModel.BpjNumber = result.BPJ_NO;
                skepModel.CreditAmount = result.EXCISE_CREDIT_AMOUNT;
                skepModel.DecreeDate = (result.DECREE_DATE.HasValue) ? result.DECREE_DATE.Value : DateTime.Today;
                skepModel.DecreeNumber = result.DECREE_NO;
                skepModel.StartDate = (result.DECREE_STARTDATE.HasValue) ? result.DECREE_STARTDATE.Value : DateTime.Today;
                skepModel.SkepDocument = result.SKEP_ATTACHMENT;
                skepModel.SkepStatus = (result.SKEP_STATUS.HasValue) ? result.SKEP_STATUS.Value : false;
                skepModel.Notes = result.NOTES;
                skepModel.IsNewEntry = String.IsNullOrEmpty(result.SKEP_ATTACHMENT);
                if (!skepModel.IsNewEntry)
                {
                    var skepAttachment = service.GetGovernmentDoc(id, "skep");
                    if (skepAttachment != null)
                        skepModel.SkepFileUpload = Mapper.Map<FileUploadModel>(skepAttachment);

                    var bpjAttachment = service.GetGovernmentDoc(id, "bpj");
                    if (bpjAttachment != null)
                        skepModel.BpjFileUpload = Mapper.Map<FileUploadModel>(bpjAttachment);
                }
                skepModel.IsDetail = false;
                model.SkepInput = skepModel;


                return View("SkepInput", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                AddMessageInfo("Failed to load Excise Request Document", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public String SubmitSkep()
        {
            try
            {
                var forms = Request.Form;
                var files = Request.Files;
                var id = Convert.ToInt64(forms["id"]);
                var entity = service.Find(id);
                if (entity == null)
                {
                    throw new Exception("Excise Credit Data not found for id " + id);
                }
                var urls = UploadGovDocument(files, forms["id"], entity.EXCISE_CREDIT_NO);

                entity.SKEP_ATTACHMENT = urls["skep"];
                entity.SKEP_STATUS = Convert.ToInt32(forms["status"]) == 1;
                entity.DECREE_NO = forms["decree_number"];
                entity.DECREE_DATE = DateTime.Parse(forms["decree_date"]);
                entity.DECREE_STARTDATE = DateTime.Parse(forms["start_date"]);
                if (forms.AllKeys.Contains("bpj_number") && !String.IsNullOrEmpty(forms["bpj_number"]))
                {
                    entity.BPJ_NO = forms["bpj_number"];
                }
                if (forms.AllKeys.Contains("bpj_date") && !String.IsNullOrEmpty(forms["bpj_date"]))
                {
                    entity.BPJ_DATE = DateTime.Parse(forms["bpj_date"]);
                }
                if (urls.ContainsKey("bpj") && !String.IsNullOrEmpty(urls["bpj"]))
                {
                    entity.BPJ_ATTACH = urls["bpj"];
                }
                var details = JArray.Parse(forms["details"]);
                var previouslyApproved = entity.EXCISE_CREDIT_APPROVED_DETAIL.ToList();
                var approvedProducts = new List<EXCISE_CREDIT_APPROVED_DETAIL>();
                if (previouslyApproved != null && previouslyApproved.Count > 0)
                {
                    approvedProducts = previouslyApproved;
                }

                foreach (var token in details)
                {
                    var obj = (JObject)token;
                    var newDetail = new EXCISE_CREDIT_APPROVED_DETAIL()
                    {
                        EXSICE_CREDIT_ID = id,
                        PROD_CODE = obj["id"].ToString(),
                        AMOUNT_APPROVED = Convert.ToDecimal(obj["amount"])
                    };
                    var index = approvedProducts.FindIndex(x => x.EXSICE_CREDIT_ID == newDetail.EXSICE_CREDIT_ID && x.PROD_CODE == newDetail.PROD_CODE);
                    if (index >= 0 && index < approvedProducts.Count)
                        approvedProducts[index] = newDetail;
                    else
                        approvedProducts.Add(newDetail);
                }

                entity.EXCISE_CREDIT_APPROVED_DETAIL = approvedProducts;

                var emailResult = service.SubmitSkep(entity, CurrentUser.USER_ID, (int)CurrentUser.UserRole);
                if(emailResult)
                AddMessageInfo("Successfully submit SKEP data and send email", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Successfully submit SKEP data but failed to send email", Enums.MessageInfoType.Success);



                return Url.Action("Index", "ExciseCredit");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                AddMessageInfo("Failed to Submit SKEP Excise Request Document", Enums.MessageInfoType.Error);
                return "";
            }
        }

        public ActionResult DetailSkep(long id = 0)
        {
            try
            {
                var model = GenerateModelProperties(null);
                var result = service.Find(Convert.ToInt64(id));
                model.POA = MapPoaModel(result.POA);
                var nppbkc = refService.GetNppbkc(result.NPPBKC_ID);
                var vm = MapExciseCreditModel(result);
                vm.IsCreator = service.IsAllowedToModify(CurrentUser.USER_ID, Convert.ToInt64(id));

                model.NPPBKC = MapNppbkcModel(nppbkc);
                model.SubmissionDate = vm.SubmissionDate;
                model.ViewModel = vm;
                model.FinancialStatements = vm.FinanceRatios.ToArray();
                model.SupportingDocuments = refService.GetSupportingDocuments((int)Enums.FormList.ExciseRequest, nppbkc.COMPANY.BUKRS, id.ToString()).Select(x => MapSupportingDocumentModel(x)).ToList();
                model.Printouts = GetPrintoutsList(model, result);
                var changeHistoryList = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.ExciseCredit, id.ToString());
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                var productAliases = service.GetCK1ProductTypes(result.NPPBKC_ID, result.SUBMISSION_DATE).ToList();
                var productTypes = service.GetProductTypes(productAliases).ToList();
                Dictionary<string, string> list = new Dictionary<string, string>();
                foreach (var prod in productTypes)
                {
                    list.Add(prod.PROD_CODE, String.Format("{0}", prod.PRODUCT_ALIAS, prod.PRODUCT_TYPE));
                }
                var skepModel = new ExciseGovApprovalModel();

                skepModel.Id = id;
                skepModel.AvailableProductTypes = this.GetExciseCreditProductTypes(list);
                skepModel.ApprovedProducts = LoadApprovedProducts(result);
                skepModel.BpjDate = (result.BPJ_DATE.HasValue) ? result.BPJ_DATE.Value : DateTime.Today;
                skepModel.BpjDocument = result.BPJ_ATTACH;
                skepModel.BpjNumber = result.BPJ_NO;
                skepModel.CreditAmount = result.EXCISE_CREDIT_AMOUNT;
                skepModel.DecreeDate = (result.DECREE_DATE.HasValue) ? result.DECREE_DATE.Value : DateTime.Today;
                skepModel.DecreeNumber = result.DECREE_NO;
                skepModel.StartDate = (result.DECREE_STARTDATE.HasValue) ? result.DECREE_STARTDATE.Value : DateTime.Today;
                skepModel.SkepDocument = result.SKEP_ATTACHMENT;
                skepModel.SkepStatus = (result.SKEP_STATUS.HasValue) ? result.SKEP_STATUS.Value : false;
                skepModel.Notes = result.NOTES;
                skepModel.IsNewEntry = String.IsNullOrEmpty(result.SKEP_ATTACHMENT);
                if (!skepModel.IsNewEntry)
                {
                    var skepAttachment = service.GetGovernmentDoc(id, "skep");
                    if (skepAttachment != null)
                        skepModel.SkepFileUpload = Mapper.Map<FileUploadModel>(skepAttachment);

                    var bpjAttachment = service.GetGovernmentDoc(id, "bpj");
                    if (bpjAttachment != null)
                        skepModel.BpjFileUpload = Mapper.Map<FileUploadModel>(bpjAttachment);
                }
                skepModel.IsDetail = true;
                model.SkepInput = skepModel;


                return View("SkepDetail", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                AddMessageInfo("Failed to load Excise Request Document", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        public ActionResult ApproveSkep(long id = 0)
        {
            try
            {
                var model = GenerateModelProperties(null);
                var result = service.Find(Convert.ToInt64(id));
                model.POA = MapPoaModel(result.POA);
                var nppbkc = refService.GetNppbkc(result.NPPBKC_ID);
                var vm = MapExciseCreditModel(result);
                vm.IsCreator = service.IsAllowedToModify(CurrentUser.USER_ID, Convert.ToInt64(id));

                model.NPPBKC = MapNppbkcModel(nppbkc);
                model.SubmissionDate = vm.SubmissionDate;
                model.ViewModel = vm;
                model.FinancialStatements = vm.FinanceRatios.ToArray();
                model.SupportingDocuments = refService.GetSupportingDocuments((int)Enums.FormList.ExciseRequest, nppbkc.COMPANY.BUKRS, id.ToString()).Select(x => MapSupportingDocumentModel(x)).ToList();
                model.Printouts = GetPrintoutsList(model, result);
                var changeHistoryList = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.ExciseCredit, id.ToString());
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.WorkflowHistory = GetWorkflowHistory(model.ViewModel.Id);
                var productAliases = service.GetCK1ProductTypes(result.NPPBKC_ID, result.SUBMISSION_DATE).ToList();
                var productTypes = service.GetProductTypes(productAliases).ToList();
                Dictionary<string, string> list = new Dictionary<string, string>();
                foreach (var prod in productTypes)
                {
                    list.Add(prod.PROD_CODE, String.Format("{0}", prod.PRODUCT_ALIAS, prod.PRODUCT_TYPE));
                }
                var skepModel = new ExciseGovApprovalModel();

                skepModel.Id = id;
                skepModel.AvailableProductTypes = this.GetExciseCreditProductTypes(list);
                skepModel.ApprovedProducts = LoadApprovedProducts(result);
                skepModel.BpjDate = (result.BPJ_DATE.HasValue) ? result.BPJ_DATE.Value : DateTime.Today;
                skepModel.BpjDocument = result.BPJ_ATTACH;
                skepModel.BpjNumber = result.BPJ_NO;
                skepModel.CreditAmount = result.EXCISE_CREDIT_AMOUNT;
                skepModel.DecreeDate = (result.DECREE_DATE.HasValue) ? result.DECREE_DATE.Value : DateTime.Today;
                skepModel.DecreeNumber = result.DECREE_NO;
                skepModel.StartDate = (result.DECREE_STARTDATE.HasValue) ? result.DECREE_STARTDATE.Value : DateTime.Today;
                skepModel.SkepDocument = result.SKEP_ATTACHMENT;
                skepModel.SkepStatus = (result.SKEP_STATUS.HasValue) ? result.SKEP_STATUS.Value : false;
                skepModel.Notes = result.NOTES;
                skepModel.IsNewEntry = String.IsNullOrEmpty(result.SKEP_ATTACHMENT);
                if (!skepModel.IsNewEntry)
                {
                    var skepAttachment = service.GetGovernmentDoc(id, "skep");
                    if (skepAttachment != null)
                        skepModel.SkepFileUpload = Mapper.Map<FileUploadModel>(skepAttachment);

                    var bpjAttachment = service.GetGovernmentDoc(id, "bpj");
                    if (bpjAttachment != null)
                        skepModel.BpjFileUpload = Mapper.Map<FileUploadModel>(bpjAttachment);
                }
                skepModel.IsDetail = false;
                model.SkepInput = skepModel;
                model.ApproveConfirm = new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "ApproveButtonConfirm",
                        CssClass = "btn btn-blue",
                        Label = "Approve"
                    },
                    CssClass = " approve-modal excisecredit-skep",
                    Id = "SkepApproveModal",
                    Message = String.Format("You are going to approve Excise Credit {0} SKEP with decree number {1}. Are you sure?", model.ViewModel.DocumentNumber, skepModel.DecreeNumber),
                    Title = "Approve Confirmation",
                    ModalLabel = "ApproveModalLabel"

                };

                return View("SkepDetail", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                AddMessageInfo("Failed to load Excise Request Document", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public String ApproveSkep(ExciseCreditFormModel model)
        {
            try
            {
                var id = model.SkepInput.Id;
                var entity = service.Find(id);
                if (entity == null)
                {
                    throw new Exception("Excise Credit Data not found for id " + id);
                }
                var status = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed);
                var result = service.ApproveExciseSkep(entity.EXSICE_CREDIT_ID, status, CurrentUser.USER_ID, (int)CurrentUser.UserRole);
                if (result)
                    AddMessageInfo("Successfully approve SKEP data and send email notification", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Successfully approve SKEP data but failed send email notification", Enums.MessageInfoType.Info);



                return Url.Action("Index", "ExciseCredit");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                AddMessageInfo("Failed to load Excise Request Document", Enums.MessageInfoType.Error);
                return "";
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public String RejectSkep()
        {
            try
            {
                var forms = Request.Form;
                var id = Convert.ToInt64(forms["id"]);
                var notes = forms["notes"];
                var entity = service.Find(id);
                if (entity == null)
                {
                    throw new Exception("Excise Credit Data not found for id " + id);
                }
                var status = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval);
                var result = service.RejectExciseSkep(entity.EXSICE_CREDIT_ID, status, CurrentUser.USER_ID, (int)CurrentUser.UserRole, notes);
                if (result)
                    AddMessageInfo("Successfully reject SKEP data and send email notification", Enums.MessageInfoType.Success);
                else
                    AddMessageInfo("Successfully reject SKEP data but failed send email notification", Enums.MessageInfoType.Info);



                return Url.Action("Index", "ExciseCredit");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                AddMessageInfo("Failed to load Excise Request Document", Enums.MessageInfoType.Error);
                return "";
            }
        }



        #region Ajax Requests
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

        public JsonResult GetFinancialStatements(string company, int year)
        {
            try
            {
                var financials = service.GetFinancialStatements(year, company).ToList();
                var mapped = AutoMapper.Mapper.Map<List<FinanceRatioModel>>(financials);
                var serialized = JsonConvert.SerializeObject(mapped);
                var obj = new JObject
                {
                    new JProperty("Success", mapped.Count >= 2),
                    new JProperty("Data", JArray.Parse(serialized))
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

        public ActionResult GetSupportingDocuments(string company)
        {
            var formId = (long)Enums.FormList.ExciseRequest;
            var docs = refService.GetSupportingDocuments(formId, company).ToList();
            return PartialView("_SupportingDocuments", docs.Select(x => MapSupportingDocumentModel(x)));
        }

        public JsonResult GetCurrentDocumentId()
        {
            return Json(service.Count());
        }

        public ActionResult GetCalculationDetail(string nppbkc, string submit, string liquidity)
        {
            var submitDate = DateTime.Parse(submit);
            var model = new CalculationDetailModel()
            {
                LiquidityRatio = Convert.ToDouble(liquidity),
                NppbkcId = nppbkc,
                Year = submitDate.Year,
                ProductTypes = service.GetCK1ProductTypes(nppbkc, submitDate),
                Adjustment = service.GetExciseAdjustment(Convert.ToDouble(liquidity))
            };
            if (model.ProductTypes.Count <= 0)
            {
                throw new Exception("CK 1 Data not available!");
            }
            model.AdjustmentDisplay = String.Format("{0}%", model.Adjustment);
            Dictionary<string, double> values3 = new Dictionary<string, double>();
            Dictionary<string, double> values6 = new Dictionary<string, double>();
            foreach (var product in model.ProductTypes)
            {
                values3.Add(product, (double)service.GetCK1Average(submitDate, nppbkc, product, 3));
                values6.Add(product, (double)service.GetCK1Average(submitDate, nppbkc, product, 6));
            }
            model.CreditRanges.Add(3, values3);
            model.CreditRanges.Add(6, values6);
            model.CalculateMaxCreditRange();

            //var result = service.GetCK1List(nppbkc, submitDate);

            return PartialView("_CalculateNewExcise", model);
        }

        [HttpPost]
        public ActionResult GetCalculationAdjustment(string nppbkc, string submit)
        {
            var submitDate = DateTime.Parse(submit);
            var model = new CalculationAdjustmentModel()
            {

                NppbkcId = nppbkc,
                Year = submitDate.Year,
                ProductTypes = service.GetCK1ProductTypes(nppbkc, submitDate)
            };

            if (model.ProductTypes.Count <= 0)
            {
                throw new Exception("CK 1 Data not available!");
            }

            model.AdjustmentDisplay = String.Format("{0}%", model.Adjustment);
            Dictionary<string, double> values12 = new Dictionary<string, double>();
            var productFaCode = new ProductFaCode();
            foreach (var product in model.ProductTypes)
            {
                values12.Add(product, (double)service.GetCK1Average(submitDate, nppbkc, product, 12));

                productFaCode = new ProductFaCode()
                {
                    ProductAlias = product,
                    ProductCode = service.GetCK1ProductCode(product)
                };

                foreach (var itm in service.GetFaCode(productFaCode.ProductCode, nppbkc))
                {
                    var pitem = new ProductItem()
                    {
                        ItemString = itm,
                        ItemId = itm
                    };
                    productFaCode.FaCode.Add(pitem);
                }

                model.Product.Add(productFaCode);

            }

            model.CreditRanges.Add(1, values12);
            model.CalculateMaxCreditRange();



            //var result = service.GetCK1List(nppbkc, submitDate);

            return PartialView("_CalculateAdjustmentExcise", model);
        }

        [HttpPost]
        public ActionResult GetItemAdjustment(string facode, string nppbkc, string submit)
        {
            var item = service.GetadjItemFaCode(facode, nppbkc);
            var submitDate = DateTime.Parse(submit);
            var itemfirst = item.First();
            Debug.Write(item.Count());
            Debug.Write(item.OrderBy(m => m.SKEP_DATE).FirstOrDefault());
            Debug.Write(item.OrderByDescending(m => m.SKEP_DATE).FirstOrDefault());
            var itemoldTariff = item.OrderBy(m => m.SKEP_DATE).FirstOrDefault() == null
                ? 0
                : item.OrderBy(m => m.SKEP_DATE).FirstOrDefault().TARIFF;

            var itemIncreaseTariff = (itemfirst.TARIFF -
                                     itemoldTariff) / itemoldTariff;

            var ck12Month = service.GetCK1Average(submitDate, nppbkc, itemfirst.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS, 2);

            var itemWeightedIncrease = itemIncreaseTariff * ck12Month;

            var dataitem = new
            {
                BRAND = itemfirst.BRAND_CE,
                FACODE = itemfirst.FA_CODE,
                OLDTARIFF = itemoldTariff,
                NEWTARIFF = itemfirst.TARIFF,
                INCREASE = itemIncreaseTariff,
                CK12MONTH = ck12Month,
                WEIGHTEDINCREASE = itemWeightedIncrease,
                PRODUCTCODE = itemfirst.PROD_CODE
            };

            return Json(dataitem);
        }

        [HttpPost]
        public JsonResult UploadFiles()
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
                        var exciseId = Request.Form.Get("excise_id");
                        var docId = (!type.Contains("other")) ? Convert.ToInt64(type) : new long?();
                        var docFileName = "";
                        if (!type.Contains("other"))
                        {
                            var doc = refService.GetSupportingDocument(docId.Value);
                            docFileName = (doc != null) ? doc.SUPPORTING_DOCUMENT_NAME : "";
                        }
                        else
                            docFileName = type.Split('_').GetValue(1).ToString();

                        var isGovDoc = (Request.Form.AllKeys.Contains("gov_doc")) ? Convert.ToBoolean(Request.Form.Get("gov_doc")) : false;
                        var urlPath = "~/" + System.Configuration.ConfigurationManager.AppSettings["ExciseCreditDocPath"] + docNumber;
                        var path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["ExciseCreditDocPath"] + docNumber);
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        path = System.IO.Path.Combine(path, fileName);
                        urlPath += "/" + fileName;
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        this.AddUploadedDoc(exciseId, docFileName, urlPath, docId, isGovDoc);

                    }

                }
                refService.SaveUploadedFiles(this.UploadedFiles);
                AddMessageInfo("Successfully save Excise Request Document!", Enums.MessageInfoType.Success);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");

        }

        private List<FILE_UPLOAD> UploadedFiles;
        private List<long> RemovedFiles;
        private void AddUploadedDoc(string id, string fileName, string url, long? docId = null, bool isGovDoc = false)
        {
            try
            {
                if (UploadedFiles == null)
                {
                    UploadedFiles = new List<FILE_UPLOAD>();
                }
                var now = DateTime.Now;
                var doc = new FILE_UPLOAD()
                {
                    FORM_TYPE_ID = (int)Enums.FormList.ExciseRequest,
                    FORM_ID = id,
                    PATH_URL = url,
                    UPLOAD_DATE = now,
                    CREATED_BY = CurrentUser.USER_ID,
                    CREATED_DATE = now,
                    LASTMODIFIED_BY = CurrentUser.USER_ID,
                    LASTMODIFIED_DATE = now,
                    IS_ACTIVE = true,
                    IS_GOVERNMENT_DOC = isGovDoc,
                    DOCUMENT_ID = docId,
                    FILE_NAME = fileName
                };
                UploadedFiles.Add(doc);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveUploadedFile(long fileId)
        {
            if (this.RemovedFiles == null)
                this.RemovedFiles = new List<long>();
            if (this.RemovedFiles.IndexOf(fileId) < 0)
                this.RemovedFiles.Add(fileId);
        }

        public ActionResult GetCk1AdjustmentList(string submit, string nppbkc)
        {
            var model = new List<ExciseCreditCk1AdjustmentModel>();
            var submitDate = DateTime.Parse(submit);
            var types = service.GetCK1ProductTypes(nppbkc, submitDate);
            foreach (var product in types)
            {
                var entities = service.GetCK1AdjustmentList(nppbkc, DateTime.Parse(submit), product, 12);
                var details = entities.Select(x => MapToCk1AdjustmentListModel(x));
                var avgQty = (details == null || details.Count() <= 0) ? 0 : Math.Ceiling((double)details.Sum(x => x.OrderQuantity) / 3);
                var avgAmount = (details == null || details.Count() <= 0) ? 0 : Math.Ceiling((double)details.Sum(x => x.Amount) / 6);
                var item = new ExciseCreditCk1AdjustmentModel()
                {
                    Details = details,
                    AverageQuantity12 = avgQty,
                    AverageAmount12 = avgAmount,
                    StartMonth = submitDate.AddMonths(-12).ToString("MMMM yyyy"),
                    EndMonth = submitDate.AddMonths(-1).ToString("MMMM yyyy")
                };
                model.Add(item);
            }
            return PartialView("_Ck1AdjustmentList", model);
        }
        public ActionResult GetCk1List(string submit, string nppbkc)
        {
            var model = new List<ExciseCreditCk1Model>();
            var submitDate = DateTime.Parse(submit);
            var types = service.GetCK1ProductTypes(nppbkc, submitDate);
            foreach (var product in types)
            {
                var entities6 = service.GetCK1List(nppbkc, DateTime.Parse(submit), product, 6);
                var entities3 = service.GetCK1List(nppbkc, DateTime.Parse(submit), product, 3);
                var details3 = entities3.Select(x => MapToCk1ListModel(x));
                var details6 = entities6.Select(x => MapToCk1ListModel(x));
                var avgQty3 = (details3 == null || details3.Count() <= 0) ? 0 : Math.Ceiling((double)details3.Sum(x => x.OrderQuantity)/3);
                var avgQty6 = (details6 == null || details6.Count() <= 0) ? 0 : Math.Ceiling((double)details6.Sum(x => x.OrderQuantity)/3);
                var avgAmount3 = (details3 == null || details3.Count() <= 0) ? 0 : Math.Ceiling((double)details3.Sum(x => x.Amount)/3);
                var avgAmount6 = (details6 == null || details6.Count() <= 0) ? 0 : Math.Ceiling((double)details6.Sum(x => x.Amount)/6);
                var item = new ExciseCreditCk1Model()
                {
                    Details = details6,
                    AverageQuantity3 = avgQty3,
                    AverageQuantity6 = avgQty6,
                    AverageAmount3 = avgAmount3,
                    AverageAmount6 = avgAmount6,
                    StartMonth = submitDate.AddMonths(-6).ToString("MMMM yyyy"),
                    EndMonth = submitDate.AddMonths(-1).ToString("MMMM yyyy")

                };
                model.Add(item);
            }

            return PartialView("_Ck1List", model);
        }

        public JsonResult GetOtherDocuments(int type, string id)
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

        public JsonResult GetApprovedProduct(string id)
        {
            try
            {
                var _id = (String.IsNullOrEmpty(id)) ? 0L : Convert.ToInt64(id);
                var excise = this.service.Find(_id);
                if (excise == null)
                {
                    throw new Exception("Specified excise credit not available!");
                }

                var result = MapExciseCreditModel(excise);
                return Json(result.ApprovedProducts);

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(ex);
            }

        }

        public JsonResult GetPrintoutLayout(string key)
        {
            try
            {

                var result = service.GetPrintoutLayout(key, CurrentUser.USER_ID);
                var layout = "No Layout Found.";
                if (result != null)
                {
                    layout = result.LAYOUT;
                }
                return Json(layout);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }

        [HttpPost]
        public JsonResult GetDefaultPrintoutLayout(string key)
        {
            try
            {
                var result = service.GetPrintoutLayout(key, CurrentUser.USER_ID);
                var layout = "No Layout Found.";
                if (result != null)
                {
                    layout = result.LAYOUT;
                }
                return Json(layout);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return Json(false);
            }
        }
        #endregion

        #region Helpers

        private String GeneratePrintout(EXCISE_CREDIT excise, String template)
        {
            var layoutId = EnumHelper.GetValueFromDescription<ReferenceKeys.PrintoutLayout>(template);
            if (layoutId == ReferenceKeys.PrintoutLayout.None)
            {
                throw new Exception("The spesified printout template not available!");
            }
            var printout = this.GetPrintoutLayoutModel(excise, layoutId);
            var docNumber = excise.EXCISE_CREDIT_NO;
            var urlPath = String.Format("~/{0}{1}/Printouts/", System.Configuration.ConfigurationManager.AppSettings["ExciseCreditDocPath"], docNumber);
            var path = Server.MapPath(urlPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = String.Format("{2}_{0}_Printout_{1}.pdf", template, DateTime.Now.ToString("yyyyMMddHHmmss"), docNumber.Replace('/', '_'));
            using (FileStream stream = new FileStream(path + fileName, FileMode.Create))
            {

                var margin = Convert.ToSingle(System.Configuration.ConfigurationManager.AppSettings["DefaultMargin"]);
                var leftMargin = iTextSharp.text.Utilities.MillimetersToPoints(5);
                var rightMargin = iTextSharp.text.Utilities.MillimetersToPoints(5);
                var topMargin = iTextSharp.text.Utilities.MillimetersToPoints(5);
                var bottomtMargin = iTextSharp.text.Utilities.MillimetersToPoints(5);
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), leftMargin, rightMargin, topMargin, bottomtMargin);
                var writer = PdfWriter.GetInstance(document, stream);
                long LastApprovedStatusID = excise.LAST_STATUS;
                if ((LastApprovedStatusID == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID) || (LastApprovedStatusID == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID) || (LastApprovedStatusID == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval).REFF_ID))
                {
                    PdfWriterEvents writerEvent = new PdfWriterEvents("D R A F T E D");
                    writer.PageEvent = writerEvent;
                }
                writer.CloseStream = false;
                document.Open();
                var srHtml = new StringReader(printout.Layout.CompleteLayout);
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, srHtml);
                document.Close();
            }
            return MergePrintout(path + fileName, excise);
        }

        public String MergePrintout(string path, EXCISE_CREDIT excise)
        {
            try
            {
                var nppbkc = refService.GetNppbkc(excise.NPPBKC_ID);
                if (nppbkc == null)
                    throw new Exception("The specified excise document does not belong to valid NPPBKC!");

                var supportingDocs = refService.GetSupportingDocuments((int)Enums.FormList.ExciseRequest, nppbkc.COMPANY.BUKRS, excise.EXSICE_CREDIT_ID.ToString()).ToList();
                List<String> sourcePaths = new List<string>();
                sourcePaths.Add(path);
                foreach (var doc in supportingDocs)
                {
                    var files = doc.FILE_UPLOAD.ToList();
                    if (files.Count > 0)
                    {
                        sourcePaths.Add(Server.MapPath(files[0].PATH_URL));
                    }
                }

                var otherDocs = refService.GetUploadedFiles((int)Enums.FormList.ExciseRequest, excise.EXSICE_CREDIT_ID.ToString()).Where(x => x.DOCUMENT_ID == null).ToList();
                foreach (var doc in otherDocs)
                {

                    sourcePaths.Add(Server.MapPath(doc.PATH_URL));
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

        private List<ExciseApprovedProduct> LoadApprovedProducts(EXCISE_CREDIT data)
        {
            try
            {
                var approvedDetails = data.EXCISE_CREDIT_APPROVED_DETAIL;
                var approvedProducts = approvedDetails.Select(x => new ExciseApprovedProduct()
                {
                    ExciseId = x.EXSICE_CREDIT_ID,
                    Amount = x.AMOUNT_APPROVED,
                    ProductAlias = x.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS,
                    ProductCode = x.PROD_CODE

                }).ToList();
                return approvedProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void UploadDocuments(HttpFileCollectionBase files, string id, string documentNumber, bool? govDoc = null)
        {
            try
            {
                this.UploadedFiles = new List<FILE_UPLOAD>();
                foreach (string file in files)
                {
                    var fileContent = files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var fileName = fileContent.FileName;
                        var type = System.IO.Path.GetFileName(file);
                        var docNumber = documentNumber.Replace("/", "_");
                        var exciseId = id;
                        var docId = (!type.Contains("other")) ? Convert.ToInt64(type) : new long?();
                        var docFileName = "";
                        if (!type.Contains("other"))
                        {
                            var doc = refService.GetSupportingDocument(docId.Value);
                            docFileName = (doc != null) ? doc.SUPPORTING_DOCUMENT_NAME : "";
                        }
                        else
                            docFileName = type.Split('_').GetValue(1).ToString();

                        var isGovDoc = (govDoc.HasValue) ? govDoc.Value : false;
                        var urlPath = "~/" + System.Configuration.ConfigurationManager.AppSettings["ExciseCreditDocPath"] + docNumber;
                        var path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["ExciseCreditDocPath"] + docNumber);
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        path = System.IO.Path.Combine(path, fileName);
                        urlPath += "/" + fileName;
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        this.AddUploadedDoc(exciseId, docFileName, urlPath, docId, isGovDoc);

                    }

                }
                refService.SaveUploadedFiles(this.UploadedFiles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dictionary<String, String> UploadGovDocument(HttpFileCollectionBase files, string id, string documentNumber)
        {
            try
            {
                var govDocs = new List<FILE_UPLOAD>();
                var urlMap = new Dictionary<String, String>();
                foreach (string file in files)
                {
                    var fileContent = files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var fileName = fileContent.FileName;
                        var type = System.IO.Path.GetFileName(file);
                        var docNumber = documentNumber.Replace("/", "_");
                        var exciseId = id;
                        var docFileName = "";
                        if (!type.Contains("skep"))
                        {
                            docFileName = String.Format("SKEP {0}", documentNumber);
                        }
                        else
                            docFileName = String.Format("BPJ {0}", documentNumber);

                        var isGovDoc = true;
                        var urlPath = "~/" + System.Configuration.ConfigurationManager.AppSettings["ExciseCreditDocPath"] + docNumber;
                        var path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["ExciseCreditDocPath"] + docNumber);
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        path = System.IO.Path.Combine(path, fileName);
                        urlPath += "/" + fileName;
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        if (!urlMap.ContainsKey(type))
                        {
                            urlMap.Add(type, urlPath);
                        }
                        var now = DateTime.Now;
                        var fileUpload = new FILE_UPLOAD()
                        {
                            FORM_TYPE_ID = (int)Enums.FormList.ExciseRequest,
                            FORM_ID = id,
                            PATH_URL = urlPath,
                            UPLOAD_DATE = now,
                            CREATED_BY = CurrentUser.USER_ID,
                            CREATED_DATE = now,
                            LASTMODIFIED_BY = CurrentUser.USER_ID,
                            LASTMODIFIED_DATE = now,
                            IS_ACTIVE = true,
                            IS_GOVERNMENT_DOC = isGovDoc,
                            FILE_NAME = docFileName
                        };

                        govDocs.Add(fileUpload);
                    }

                }
                refService.SaveUploadedFiles(govDocs);
                return urlMap;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private SelectList GetYearList(IEnumerable<ExciseCreditModel> exciseCredits)
        {
            var query = from x in exciseCredits
                        select new SelectItemModel()
                        {
                            ValueField = x.SubmissionDate.Year,
                            TextField = x.SubmissionDate.Year.ToString()
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetExciseCreditTypeList(Dictionary<int, string> types)
        {
            var query = from x in types
                        select new SelectItemModel()
                        {
                            ValueField = x.Key,
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetExciseCreditGuaranteeList(Dictionary<int, string> guarantees)
        {
            var query = from x in guarantees
                        select new SelectItemModel()
                        {
                            ValueField = x.Key,
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetExciseCreditProductTypes(Dictionary<string, string> productTypes)
        {
            var query = from x in productTypes
                        select new SelectItemModel()
                        {
                            ValueField = x.Key,
                            TextField = x.Value
                        };
            var temp = query.ToList();
            return new SelectList(query, "ValueField", "TextField");
            //return new SelectList(query);
        }

        private SelectList GetPoaList(IEnumerable<CustomService.Data.POA> poaList)
        {
            var query = from x in poaList
                        select new SelectItemModel()
                        {
                            ValueField = x.POA_ID,
                            TextField = x.PRINTED_NAME
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

        private ExciseCreditFormModel GenerateModelProperties(ExciseCreditFormModel model)
        {
            if (model == null)
            {
                var limit = refService.GetUploadFileSizeLimit();
                model = new ExciseCreditFormModel()
                {
                    NppbkcList = GetNppbkcList(refService.GetAllNppbkc(CurrentUser.USER_ID)),
                    GuaranteeTypes = GetExciseCreditGuaranteeList(service.GetExciseCreditGuarantees()),
                    POA = MapPoaModel(refService.GetPOA(CurrentUser.USER_ID)),
                    RequestTypes = GetExciseCreditTypeList(service.GetExciseCreditTypes()),
                    MainMenu = Enums.MenuList.ExciseCredit,
                    CurrentMenu = PageInfo,
                    FileUploadLimit = (limit != null) ? Convert.ToDouble(limit.REFF_VALUE) : 0.0,
                    IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Controller && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator)
                };
            }
            return model;
        }

        #region Mappings

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
        private ExciseCreditPOA MapPoaModel(CustomService.Data.POA poa)
        {
            try
            {
                if (poa == null)
                    return null;

                return new ExciseCreditPOA()
                {
                    Id = poa.POA_ID,
                    Name = poa.PRINTED_NAME,
                    Address = poa.POA_ADDRESS,
                    Position = poa.TITLE
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private ExciseCreditNppbkc MapNppbkcModel(CustomService.Data.MASTER_NPPBKC nppbkc)
        {
            try
            {
                return new ExciseCreditNppbkc()
                {
                    Id = nppbkc.NPPBKC_ID,
                    Region = nppbkc.REGION_DGCE,
                    Address = nppbkc.DGCE_ADDRESS,
                    City = nppbkc.CITY,
                    CityAlias = nppbkc.CITY_ALIAS,
                    KppbcId = nppbkc.KPPBC_ID,
                    Company = (nppbkc.COMPANY != null) ? new CompanyModel()
                    {
                        Id = nppbkc.COMPANY.BUKRS,
                        Name = nppbkc.COMPANY.BUTXT,
                        Alias = nppbkc.COMPANY.BUTXT_ALIAS
                    } : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ExciseCreditModel MapExciseCreditModel(CustomService.Data.EXCISE_CREDIT entity)
        {
            try
            {

                List<FinanceRatioModel> financialRatio = new List<FinanceRatioModel>();
                financialRatio.Add(new FinanceRatioModel()
                {
                    LiquidityRatio = (entity.LIQUIDITY_RATIO_1.HasValue) ? (decimal)entity.LIQUIDITY_RATIO_1.Value : decimal.Zero,
                    SolvencyRatio = (entity.SOLVENCY_RATIO_1.HasValue) ? (decimal)entity.SOLVENCY_RATIO_1.Value : decimal.Zero,
                    RentabilityRatio = (entity.RENTABILITY_RATIO_1.HasValue) ? (decimal)entity.RENTABILITY_RATIO_1.Value : decimal.Zero,
                    YearPeriod = entity.SUBMISSION_DATE.Year - 1
                });
                financialRatio.Add(new FinanceRatioModel()
                {
                    LiquidityRatio = (entity.LIQUIDITY_RATIO_2.HasValue) ? (decimal)entity.LIQUIDITY_RATIO_2.Value : decimal.Zero,
                    SolvencyRatio = (entity.SOLVENCY_RATIO_2.HasValue) ? (decimal)entity.SOLVENCY_RATIO_2.Value : decimal.Zero,
                    RentabilityRatio = (entity.RENTABILITY_RATIO_2.HasValue) ? (decimal)entity.RENTABILITY_RATIO_2.Value : decimal.Zero,
                    YearPeriod = entity.SUBMISSION_DATE.Year - 2
                });
                return new ExciseCreditModel()
                {
                    Id = entity.EXSICE_CREDIT_ID,
                    DocumentNumber = entity.EXCISE_CREDIT_NO,
                    SubmissionDate = entity.SUBMISSION_DATE,
                    RequestTypeID = entity.REQUEST_TYPE,
                    RequestType = this.GetRequestTypeName(entity.REQUEST_TYPE),
                    NppbkcId = entity.NPPBKC_ID,
                    Guarantee = entity.GUARANTEE,
                    FinanceRatios = financialRatio,
                    SkepLastStatus = entity.SKEP_STATUS,
                    //SkepStatus = AutoMapper.Mapper.Map<ReferenceModel>(entity.)
                    DecreeNumber = entity.DECREE_NO,
                    DecreeDate = entity.DECREE_DATE,
                    DecreeStartDate = entity.DECREE_STARTDATE,
                    BpjNumber = entity.BPJ_NO,
                    BpjDate = entity.BPJ_DATE,
                    BpjAttachmentUrl = entity.BPJ_ATTACH,
                    CalculatedAdjustment = entity.ADJUSTMENT_CALCULATED.Value,
                    CalculatedAdjustmentDisplay = entity.ADJUSTMENT_CALCULATED.Value.ToString("N"),
                    Notes = entity.NOTES,
                    Amount = entity.EXCISE_CREDIT_AMOUNT,
                    AmountDisplay = entity.EXCISE_CREDIT_AMOUNT.ToString("N"),
                    CreatedBy = entity.CREATED_BY,
                    LastStatus = entity.LAST_STATUS,
                    ApprovalStatus = AutoMapper.Mapper.Map<ReferenceModel>(entity.APPROVAL_STATUS),
                    ApprovedBy = entity.APPROVED_BY,
                    ApprovedDate = entity.APPROVED_DATE,
                    CreatedDate = entity.CREATED_DATE,
                    ModifiedBy = entity.LAST_MODIFIED_BY,
                    ModifiedDate = entity.LAST_MODIFIED_DATE,
                    IsCreator = service.IsAllowedToModify(CurrentUser.USER_ID, entity.EXSICE_CREDIT_ID),
                    IsApprover = service.IsAllowedToApprove(CurrentUser.USER_ID, entity.EXSICE_CREDIT_ID),
                    POA = entity.POA_ID,
                    IsSubmitted = entity.LAST_STATUS == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval).REFF_ID,
                    IsApproved = entity.LAST_STATUS == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID,
                    IsCanceled = entity.LAST_STATUS == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Canceled).REFF_ID,
                    IsWaitingForGovernment = entity.LAST_STATUS == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval).REFF_ID,
                    IsWaitingSkepApproval = entity.LAST_STATUS == refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval).REFF_ID,
                    IsAdmin = CurrentUser.UserRole == Enums.UserRole.Administrator,
                    ApprovedProducts = entity.EXCISE_CREDIT_APPROVED_DETAIL.Select(x => MapApprovedProduct(x)).ToList()

                };
            }
            catch (Exception ex)
            {
                //var msg = String.Format("Message: {0}\nStack Trace: {1}\nInner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException);
                //AddMessageInfo(msg, Enums.MessageInfoType.Error);
                throw ex;
            }
        }

        public ExciseApprovedProduct MapApprovedProduct(EXCISE_CREDIT_APPROVED_DETAIL entity)
        {
            try
            {
                return new ExciseApprovedProduct()
                {
                    ExciseId = entity.EXSICE_CREDIT_ID,
                    Amount = entity.AMOUNT_APPROVED,
                    ProductAlias = entity.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS,
                    ProductCode = entity.PROD_CODE
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExciseCreditSupportingDocument MapSupportingDocumentModel(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new ExciseCreditSupportingDocument()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = (entity.COMPANY != null) ? new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    } : null,
                    FileList = (entity.FILE_UPLOAD != null) ? AutoMapper.Mapper.Map<List<FileUploadModel>>(entity.FILE_UPLOAD).ToList() : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExciseCreditCK1DetailModel MapToCk1ListModel(CustomService.Data.CK1_EXCISE_CALCULATE entity)
        {
            try
            {
                return new ExciseCreditCK1DetailModel()
                {
                    Id = entity.CK1_ID,
                    CK1Date = entity.CK1_DATE,
                    MonthPeriod = entity.BULAN.Value,
                    PeriodDisplay = entity.CK1_DATE.ToString("MMMM yyyy"),
                    YearPeriod = entity.TAHUN.Value,
                    ProductTypeCode = entity.PRODUCT_ALIAS,
                    CK1Number = entity.CK1_NUMBER,
                    OrderQuantity = entity.ORDERQTY.Value,
                    Amount = entity.NOMINAL.Value
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExciseCreditCK1DetailAdjustmentModel MapToCk1AdjustmentListModel(CustomService.Data.CK1_EXCISE_CALCULATE_ADJUST entity)
        {
            try
            {
                return new ExciseCreditCK1DetailAdjustmentModel()
                {
                    Id = entity.CK1_ID,
                    CK1Date = entity.CK1_DATE,
                    MonthPeriod = entity.BULAN.Value,
                    PeriodDisplay = entity.CK1_DATE.ToString("MMMM yyyy"),
                    YearPeriod = entity.TAHUN.Value,
                    ProductTypeCode = entity.PRODUCT_ALIAS,
                    CK1Number = entity.CK1_NUMBER,
                    OrderQuantity = entity.ORDERQTY.Value,
                    Amount = entity.NOMINAL.Value,
                    Brand_Content = entity.BRAND_CONTENT,
                    Brand_Ce = entity.BRAND_CE,
                    Series_Value = entity.SERIES_VALUE.Value,
                    Production = entity.PRODUCTION.Value
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ExciseCreditPrintoutModel MapPrintoutModel(EXCISE_CREDIT excise, ReferenceKeys.PrintoutLayout templateId)
        {
            try
            {
                var printoutModel = this.GetPrintoutLayoutModel(excise, templateId);
                printoutModel.Confirmation = new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = String.Format("RestoreButton{0}", printoutModel.Layout.LayoutId),
                        CssClass = "btn btn-blue",
                        Label = "Restore"
                    },
                    CssClass = String.Format("restore-modal excise-printout-{0}", printoutModel.Layout.LayoutId),
                    Message = String.Format("You are going restore template {0} to default. Are you sure?", printoutModel.PrintoutName),
                    Title = "Restore Template to Default Confirmation",
                    ModalLabel = String.Format("RestorePrintout{0}ModalLabel", printoutModel.Layout.LayoutId)
                };

                return printoutModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ExciseCreditPrintoutModel GetPrintoutLayoutModel(EXCISE_CREDIT excise, ReferenceKeys.PrintoutLayout templateId)
        {
            try
            {
                var entity = service.GetPrintoutLayout(ReferenceLookup.Instance.GetReferenceKey(templateId), CurrentUser.USER_ID);
                var draft = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft);
                var draftEdit = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited);
                var submitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval);
                if (entity == null)
                    return null;
                var userPrintoutList = entity.USER_PRINTOUT_LAYOUT;
                USER_PRINTOUT_LAYOUT userPrintout = null;
                if (userPrintoutList != null && userPrintoutList.Any())
                {
                    userPrintout = userPrintoutList.OrderByDescending(x => x.MODIFIED_DATE).FirstOrDefault();
                }
                var layout = new PrintoutLayout()
                {
                    Id = excise.EXSICE_CREDIT_ID,
                    LayoutId = entity.PRINTOUT_LAYOUT_ID,
                    DefaultLayout = entity.LAYOUT,
                    Name = entity.NAME,
                    Layout = entity.LAYOUT
                };
                if (userPrintout != null)
                {
                    layout.User = userPrintout.USER_ID;
                    layout.Layout = userPrintout.LAYOUT;
                }

                var printoutModel = new ExciseCreditPrintoutModel()
                {
                    ExciseId = excise.EXSICE_CREDIT_ID,
                    Layout = layout,
                    PrintoutType = templateId,
                    PrintoutName = EnumHelper.GetDescription(templateId),
                    IsAllowedToEdit = service.IsAllowedToModify(CurrentUser.USER_ID, excise.EXSICE_CREDIT_ID) || service.IsAllowedToApprove(CurrentUser.USER_ID, excise.EXSICE_CREDIT_ID),
                    IsDrafted = excise.LAST_STATUS == draft.REFF_ID || excise.LAST_STATUS == draftEdit.REFF_ID || excise.LAST_STATUS == submitted.REFF_ID
                };
                printoutModel.Layout.CompleteLayout = GetPrintoutContent(entity, excise, templateId);

                return printoutModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult SaveTemplate(string name, string content)
        {
            try
            {
                var result = service.SavePrintoutLayout(name, content, CurrentUser.USER_ID);

                if (result != null)
                {
                    AddMessageInfo("Successfully update printout template!", Enums.MessageInfoType.Success);
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(false);
            }
        }

        [HttpPost]
        public void DownloadPrintout(ExciseCreditFormModel model)
        {
            try
            {
                var id = model.ViewModel.Id;
                var templateDescription = Request.Form["PrintoutTypeDescription"];
                var excise = service.Find(id);
                if (excise == null)
                    throw new Exception("The specified excise credit document not available!");

                var templateId = EnumHelper.GetValueFromDescription<ReferenceKeys.PrintoutLayout>(templateDescription);

                if (templateId == ReferenceKeys.PrintoutLayout.None)
                    throw new Exception("The specified printout template not available!");

                var layout = service.GetPrintoutLayout(ReferenceLookup.Instance.GetReferenceKey(templateId), CurrentUser.USER_ID);

                if (layout == null)
                    throw new Exception("The specified printout template layout not available!");

                var content = this.GetPrintoutContent(layout, excise, templateId);
                var path = this.GeneratePrintout(excise, templateDescription);
                if (String.IsNullOrEmpty(path))
                    throw new Exception("The specified printout template content not available!");

                var newFile = new FileInfo(path);
                var fileName = Path.GetFileName(path);
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
                AddMessageInfo(String.Format("Cannot download printout!. Reason: {0}", ex.Message), Enums.MessageInfoType.Error);
            }
        }

        public List<ExciseCreditPrintoutModel> GetPrintoutsList(ExciseCreditFormModel model, EXCISE_CREDIT excise)
        {
            var printouts = new List<ExciseCreditPrintoutModel>();
            if (model.ViewModel.RequestTypeID == 1)
            {
                printouts.Add(MapPrintoutModel(excise, ReferenceKeys.PrintoutLayout.ExciseCreditNewRequest));
                printouts.Add(MapPrintoutModel(excise, ReferenceKeys.PrintoutLayout.ExciseCreditNewRequestMain));
            }
            else
            {
                printouts.Add(MapPrintoutModel(excise, ReferenceKeys.PrintoutLayout.ExciseCreditAdjustmentRequest));
                printouts.Add(MapPrintoutModel(excise, ReferenceKeys.PrintoutLayout.ExciseCreditAdjustmentRequestMain));
            }
            printouts.Add(MapPrintoutModel(excise, ReferenceKeys.PrintoutLayout.DetailExciseCalculation));
            printouts.Add(MapPrintoutModel(excise, ReferenceKeys.PrintoutLayout.FinanceRatio));
            printouts.Add(MapPrintoutModel(excise, ReferenceKeys.PrintoutLayout.ExciseRequestGuaranteeDecree));

            return printouts;
        }

        #endregion


        private string GetRequestTypeName(int id)
        {
            return service.GetExciseCreditTypeName(id);
        }
        #region Printout Utils
        public string GetPrintoutContent(PRINTOUT_LAYOUT entity, EXCISE_CREDIT excise, ReferenceKeys.PrintoutLayout templateId)
        {
            if (entity != null && excise != null)
            {
                // Load Printout
                var template = GetPrintoutTemplate(entity);


                // Load Printout parameters
                var parameters = GetPrintoutParameters(templateId, excise);

                var content = template;
                foreach (var param in parameters)
                {
                    content = content.Replace(String.Format("#{0}", param.Key), param.Value);
                }
                return content;


            }
            return null;
        }

        private String GetPrintoutTemplate(PRINTOUT_LAYOUT entity)
        {
            var userPrintoutList = entity.USER_PRINTOUT_LAYOUT;
            USER_PRINTOUT_LAYOUT userPrintout = null;
            if (userPrintoutList != null && userPrintoutList.Any())
            {
                userPrintout = userPrintoutList.OrderByDescending(x => x.MODIFIED_DATE).FirstOrDefault();
            }
            var template = (userPrintout == null || String.IsNullOrEmpty(userPrintout.LAYOUT)) ? entity.LAYOUT : userPrintout.LAYOUT;

            return template;
        }

        private Dictionary<string, string> GetPrintoutParameters(ReferenceKeys.PrintoutLayout templateId, EXCISE_CREDIT excise)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            // Load Printout Data
            var model = GetNewExciseRequestData(excise);
            var nppbkc = refService.GetNppbkc(excise.NPPBKC_ID);
            System.Globalization.CultureInfo cultureID = new System.Globalization.CultureInfo("id-ID");
            switch (templateId)
            {
                case ReferenceKeys.PrintoutLayout.ExciseCreditNewRequestMain:
                    var lampiran = service.GetFileCount(excise.EXSICE_CREDIT_ID);
                    var index11 = excise.EXCISE_CREDIT_AMOUNT >= 50000000000 ? "Kepala Kantor Wilayah" : "Kepada Yth.";
                    var amount = excise.EXCISE_CREDIT_AMOUNT;
                    var amountterbilang = service.Terbilang(amount);
                    var documentpendukung = service.GetFile(excise.EXSICE_CREDIT_ID);
                    var documentpendukungstring = "";
                    foreach (var fileUpload in documentpendukung.ToList())
                    {
                        documentpendukungstring += string.Format("<li>{0}</li>", fileUpload.FILE_NAME);
                    }
                    parameters.Add("KPPBCAddress", excise.ZAIDM_EX_NPPBKC.KPPBC_ADDRESS);
                    parameters.Add("SubmissionDate", excise.SUBMISSION_DATE.ToShortDateString());
                    parameters.Add("jumlahLampiran", string.Format("{0}, ({1}) Lampiran", lampiran, service.Terbilang(lampiran)));
                    parameters.Add("index11", index11);
                    parameters.Add("namaalamatkantor", excise.ZAIDM_EX_NPPBKC.KPPBC_ADDRESS);
                    parameters.Add("REGION_DGCE", excise.ZAIDM_EX_NPPBKC.REGION_DGCE);
                    parameters.Add("LOCATION", excise.ZAIDM_EX_NPPBKC.LOCATION);
                    parameters.Add("POAPRINTED_NAME", excise.POA.PRINTED_NAME);
                    parameters.Add("POATITLE", excise.POA.TITLE);
                    parameters.Add("POA_ADDRESS", excise.POA.POA_ADDRESS);
                    parameters.Add("COMPANY", nppbkc.COMPANY.BUTXT);
                    parameters.Add("NPPBKC", nppbkc.NPPBKC_ID);
                    parameters.Add("LOKASI_NPPBKC", nppbkc.KPPBC_ADDRESS);
                    parameters.Add("EXCISE_CREDIT_AMOUNT", string.Format("{0:N}", amount));
                    parameters.Add("TERBILANG", amountterbilang);
                    parameters.Add("documentpendukung", documentpendukungstring);
                    break;
                case ReferenceKeys.PrintoutLayout.ExciseCreditNewRequest:
                    // Load Sub Template CK-1 Excise Summary
                    Dictionary<string,decimal> grandTotal6 = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> grandTotal3 = new Dictionary<string, decimal>();
                    var ck1Tables = GetNewExciseRequestSubTemplate(model, excise,out grandTotal6,out grandTotal3);

                    // Load CK-1 Excise Summary Average
                    var ck1Avg6 = GetNewExciseCK1Avg(model, excise, 6,grandTotal6,grandTotal3);
                    var ck1Avg3 = GetNewExciseCK1Avg(model, excise, 3, grandTotal6, grandTotal3);

                    // Load CK-1 Excise Calculation Summary Maximum
                    var calculationMax = GetNewExciseCK1MaxAvg(model, excise, false,grandTotal6,grandTotal3);
                    var additionalMax = GetNewExciseCK1MaxAvg(model, excise, true, grandTotal6, grandTotal3);
                    parameters.Add("POA", excise.POA.PRINTED_NAME);
                    parameters.Add("COMPANY_NAME", nppbkc.COMPANY.BUTXT);
                    parameters.Add("COMPANY_ADDRESS", nppbkc.DGCE_ADDRESS);
                    parameters.Add("NPPBKC_ID", excise.NPPBKC_ID);
                    parameters.Add("TOTAL_AMOUNT", String.Format("Rp. {0:N}", excise.EXCISE_CREDIT_AMOUNT));
                    parameters.Add("CK1_DETAIL_TABLE", ck1Tables);
                    parameters.Add("CK1_AVG_6", ck1Avg6);
                    parameters.Add("CK1_AVG_3", ck1Avg3);
                    parameters.Add("PRODUCT_AMOUNT_CALCULATION", calculationMax);
                    parameters.Add("PRODUCT_ADDITIONAL_AMOUNT", additionalMax);
                    break;
                //case ReferenceKeys.PrintoutLayout.ExciseCreditAdjustmentRequest:
                //    var ck1Detail = GetCalculationExciseAdjustmentCK1Avg(model, excise);
                //    var ck1Calculation = GetNewExciseAdjustmentCK1Avg(model, excise, 12);
                //    var exciseAdjustment = GetAdjustmentExcise(model, excise);
                //    parameters.Clear();
                //    var delayAdjustment = GetDelayAdjustmentExcise(model, excise);
                //    parameters.Add("COMPANY_NAME", nppbkc.COMPANY.BUTXT);
                //    parameters.Add("SKEP_NO", String.IsNullOrEmpty(excise.DECREE_NO) ? "-" : excise.DECREE_NO);
                //    parameters.Add("SKEPDATE", (excise.DECREE_DATE != null) ? excise.DECREE_DATE.Value.ToString("dd MMMM yyyy", cultureID) : "-");
                //    parameters.Add("CITY", nppbkc.CITY);
                //    parameters.Add("CK1_DETAIL_TABLE", ck1Detail);
                //    parameters.Add("CK1_CALCULATION", ck1Calculation);
                //    parameters.Add("CK1_ADJUSTMENT", exciseAdjustment);
                //    parameters.Add("CK1_DELAY_ADJUSTMENT", delayAdjustment);
                //    parameters.Add("PEMOHON", excise.POA.PRINTED_NAME);
                //    break;
                case ReferenceKeys.PrintoutLayout.DetailExciseCalculation:
                    ck1Tables = this.GetCK1CalculationReport(excise);
                    parameters.Clear();
                    parameters.Add("POA", excise.POA.PRINTED_NAME);
                    parameters.Add("COMPANY_NAME", nppbkc.COMPANY.BUTXT);
                    parameters.Add("COMPANY_ADDRESS", nppbkc.DGCE_ADDRESS);
                    parameters.Add("NPPBKC", excise.NPPBKC_ID);
                    parameters.Add("CK1_PERIOD", String.Format(cultureID, "{0:MMMM yyyy} - {1: MMMM yyyy}", excise.SUBMISSION_DATE.AddMonths(-6), excise.SUBMISSION_DATE.AddMonths(-1)));
                    parameters.Add("CK1_DETAIL_TABLE", ck1Tables);
                    break;
                case ReferenceKeys.PrintoutLayout.ExciseRequestGuaranteeDecree:
                    parameters.Add("nomor", excise.EXCISE_CREDIT_NO);
                    parameters.Add("tanggal", excise.SUBMISSION_DATE.ToShortDateString());
                    parameters.Add("textto", excise.ZAIDM_EX_NPPBKC.TEXT_TO);
                    parameters.Add("alamat", excise.ZAIDM_EX_NPPBKC.KPPBC_ADDRESS);
                    parameters.Add("poanama", excise.POA.PRINTED_NAME);
                    parameters.Add("poajabatan", excise.POA.TITLE);
                    parameters.Add("poaalamat", excise.POA.POA_ADDRESS);
                    parameters.Add("company", nppbkc.COMPANY.BUTXT);
                    parameters.Add("nppbkc", nppbkc.NPPBKC_ID);
                    break;
                case ReferenceKeys.PrintoutLayout.ExciseCreditAdjustmentRequest:
                    var model2 = GetNewExciseRequestData(excise);
                    var ck1Tablesa = GetNewExciseAdjustmentCK1Avg(model2, excise, 12);
                    var ck1Calculation = GetCalculationExciseAdjustmentCK1Avg(model2, excise);
                    var adjustment = GetAdjustmentExcise(model2, excise);
                    var delayAdjustment = GetDelayAdjustmentExcise(model2, excise);
                    var nppbkc2 = refService.GetNppbkc(excise.NPPBKC_ID);
                    parameters.Add("POA", excise.POA_ID);
                    parameters.Add("COMPANY_NAME", nppbkc2.COMPANY.BUTXT);
                    parameters.Add("COMPANY_ADDRESS", nppbkc2.DGCE_ADDRESS);
                    parameters.Add("SKEPNO", excise.DECREE_NO);
                    parameters.Add("SKEPDATE", string.Format("{0}", excise.DECREE_DATE));
                    parameters.Add("HASIL", "Hasil Tembakau");
                    parameters.Add("CITY", nppbkc2.CITY);
                    parameters.Add("PEMOHON", excise.POA.POA_ID);
                    parameters.Add("NPPBKC_ID", excise.NPPBKC_ID);
                    parameters.Add("TOTAL_AMOUNT", String.Format("Rp. {0:N}", excise.EXCISE_CREDIT_AMOUNT));
                    parameters.Add("CK1_DETAIL_TABLE", ck1Tablesa);
                    parameters.Add("CK1_CALCULATION", ck1Calculation);
                    parameters.Add("CK1_ADJUSTMENT", adjustment);
                    parameters.Add("CK1_DELAY_ADJUSTMENT", delayAdjustment);
                    break;
                case ReferenceKeys.PrintoutLayout.ExciseCreditAdjustmentRequestMain:
                    var lampiranAdjustment = service.GetFileCount(excise.EXSICE_CREDIT_ID);
                    var index11Adjustment = excise.EXCISE_CREDIT_AMOUNT >= 50000000000 ? "Kepala Kantor Wilayah" : "Kepada Yth.";
                    var amountAdjustment = excise.EXCISE_CREDIT_AMOUNT;
                    var amountterbilangAdjustment = service.Terbilang(amountAdjustment);
                    var documentpendukungAdjustment = service.GetFile(excise.EXSICE_CREDIT_ID);
                    var documentpendukungstringAdjustment = "";
                    foreach (var fileUpload in documentpendukungAdjustment.ToList())
                    {
                        documentpendukungstringAdjustment += string.Format("<li>{0}</li>", fileUpload.FILE_NAME);
                    }
                    parameters.Add("kccpaddress", excise.ZAIDM_EX_NPPBKC.KPPBC_ADDRESS);
                    parameters.Add("SubmissionDate", excise.SUBMISSION_DATE.ToShortDateString());
                    parameters.Add("lampiran", string.Format("{0}, ({1}) Lampiran", lampiranAdjustment, service.Terbilang(lampiranAdjustment)));
                    parameters.Add("index11", index11Adjustment);
                    parameters.Add("namaalamatkantor", excise.ZAIDM_EX_NPPBKC.KPPBC_ADDRESS);
                    parameters.Add("regiondgce", excise.ZAIDM_EX_NPPBKC.REGION_DGCE);
                    parameters.Add("location", excise.ZAIDM_EX_NPPBKC.LOCATION);
                    parameters.Add("poanama", excise.POA.PRINTED_NAME);
                    parameters.Add("poajabatan", excise.POA.TITLE);
                    parameters.Add("poaalamat", excise.POA.POA_ADDRESS);
                    parameters.Add("company", nppbkc.COMPANY.BUTXT);
                    parameters.Add("nppbkc", nppbkc.NPPBKC_ID);
                    parameters.Add("kota", nppbkc.LOCATION);
                    parameters.Add("nominal", string.Format("{0:N}", amountAdjustment));
                    parameters.Add("terbilang", amountterbilangAdjustment);
                    parameters.Add("lastskep", "?");
                    break;
					case ReferenceKeys.PrintoutLayout.FinanceRatio:
                    parameters.Clear();
                    parameters.Add("COMPANY_NAME", nppbkc.COMPANY.BUTXT);
                    var financialRatios = service.GetFinancialStatements
                        (excise.SUBMISSION_DATE.Year, nppbkc.COMPANY.BUKRS).ToList();
                    if (financialRatios.Count != 2)
                    {
                        break;
                    }
                    parameters.Add("YEAR_1", financialRatios[1].YEAR_PERIOD.ToString());
                    parameters.Add("YEAR_2", financialRatios[0].YEAR_PERIOD.ToString());
                    parameters.Add("CURRENT_ASSET_1", financialRatios[1].CURRENT_ASSETS.ToString("N"));
                    parameters.Add("CURRENT_ASSET_2", financialRatios[0].CURRENT_ASSETS.ToString("N"));
                    parameters.Add("CURRENT_DEBT_1", financialRatios[1].CURRENT_DEBTS.ToString("N"));
                    parameters.Add("CURRENT_DEBT_2", financialRatios[0].CURRENT_DEBTS.ToString("N"));
                    parameters.Add("LIQUIDITY_1", financialRatios[1].LIQUIDITY_RATIO.ToString("N"));
                    parameters.Add("LIQUIDITY_2", financialRatios[0].LIQUIDITY_RATIO.ToString("N"));
                    parameters.Add("TOTAL_ASSET_1", financialRatios[1].TOTAL_ASSETS.ToString("N"));
                    parameters.Add("TOTAL_ASSET_2", financialRatios[0].TOTAL_ASSETS.ToString("N"));
                    parameters.Add("TOTAL_DEBT_1", financialRatios[1].TOTAL_DEBTS.ToString("N"));
                    parameters.Add("TOTAL_DEBT_2", financialRatios[0].TOTAL_DEBTS.ToString("N"));
                    parameters.Add("SOLVABILITY_1", financialRatios[1].SOLVENCY_RATIO.ToString("N"));
                    parameters.Add("SOLVABILITY_2", financialRatios[0].SOLVENCY_RATIO.ToString("N"));
                    parameters.Add("NET_PROFIT_1", financialRatios[1].NET_PROFIT.ToString("N"));
                    parameters.Add("NET_PROFIT_2", financialRatios[0].NET_PROFIT.ToString("N"));
                    parameters.Add("TOTAL_CAPITAL_1", financialRatios[1].TOTAL_CAPITAL.ToString("N"));
                    parameters.Add("TOTAL_CAPITAL_2", financialRatios[0].TOTAL_CAPITAL.ToString("N"));
                    parameters.Add("RENTABILITY_1", financialRatios[1].RENTABLE_RATIO.ToString("N"));
                    parameters.Add("RENTABILITY_2", financialRatios[0].RENTABLE_RATIO.ToString("N"));
                    break;
            }
            return parameters;
        }

        private CalculationDetailModel GetNewExciseRequestData(EXCISE_CREDIT excise)
        {
            try
            {
                var nppbkc = refService.GetNppbkc(excise.NPPBKC_ID);
                var productTypes = service.GetCK1ProductTypes(excise.NPPBKC_ID, excise.SUBMISSION_DATE);
                var liquidity = excise.LIQUIDITY_RATIO_1;
                var model = new CalculationDetailModel()
                {
                    LiquidityRatio = Convert.ToDouble(liquidity),
                    NppbkcId = excise.NPPBKC_ID,
                    Year = excise.SUBMISSION_DATE.Year,
                    ProductTypes = service.GetCK1ProductTypes(excise.NPPBKC_ID, excise.SUBMISSION_DATE),
                    Adjustment = service.GetExciseAdjustment(Convert.ToDouble(liquidity))
                };
                if (model.ProductTypes.Count <= 0)
                {
                    throw new Exception("CK 1 Data not available!");
                }
                model.AdjustmentDisplay = String.Format("{0}", model.AdjustmentDisplay);
                Dictionary<string, double> values3 = new Dictionary<string, double>();
                Dictionary<string, double> values6 = new Dictionary<string, double>();
                Dictionary<string, double> values12 = new Dictionary<string, double>();
                foreach (var product in model.ProductTypes)
                {
                    values3.Add(product, (double)service.GetCK1Average(excise.SUBMISSION_DATE, excise.NPPBKC_ID, product, 3));
                    values6.Add(product, (double)service.GetCK1Average(excise.SUBMISSION_DATE, excise.NPPBKC_ID, product, 6));
                    values12.Add(product, (double)service.GetCK1Average(excise.SUBMISSION_DATE, excise.NPPBKC_ID, product, 12));
                }
                model.CreditRanges.Add(3, values3);
                model.CreditRanges.Add(6, values6);
                model.CreditRanges.Add(12, values12);
                model.CalculateMaxCreditRange();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private String GetNewExciseRequestSubTemplate(CalculationDetailModel model, EXCISE_CREDIT excise,out Dictionary<String, Decimal> grandTotal6, out Dictionary<String, Decimal> grandTotal3)
        {
            try
            {
                #region Hack Variables
                var productTypeCount = model.ProductTypes.Count;
                var ck1Data = new Dictionary<string, List<EXCISE_CREDIT_DETAILCK1>>();
                var dataLength = 0;
                System.Globalization.CultureInfo cultureID = new System.Globalization.CultureInfo("id-ID");
                var ck1GroupedData = new Dictionary<String, Dictionary<String, Decimal>>();
                var durations = new List<String>();
                grandTotal3 = new Dictionary<String, Decimal>();
                grandTotal6 = new Dictionary<String, Decimal>();
                var productTypes = model.ProductTypes;
                var tableBuilder = new StringBuilder();
                #endregion

                #region Initiate Sub Template
                tableBuilder.Append("<table style='width: 100%; background-color: white!important; border-collapse: collapse;border: 1px solid black;' >")
                    .Append("<thead>")
                    .Append("<tr>")
                    .Append("<td rowspan='2' style='width: 20px; text-align: center;  border: 1px solid black;'>No</td>")
                    .Append("<td rowspan='2' style='width: 25%; text-align: center;  border: 1px solid black;'>Bulan</td>")
                    .AppendFormat("<td colspan='{0}' style='text-align: center; border: 1px solid black;'>Jumlah Cukai (Rp)</td>", productTypeCount + 1)
                    .Append("</tr>");
                #endregion

                #region Initiate Table Header
                tableBuilder.Append("<tr>");
                foreach (var ptype in productTypes)
                {
                    tableBuilder.AppendFormat("<td style='width: 25%;text-align: center; border: 1px solid black;'>{0}</td>", ptype.ToUpper());
                    var items = service.GetExciseCk1Detail(excise.EXSICE_CREDIT_ID, ptype).OrderBy(x => x.CK1_DATE).ToList();
                    var beginDate = items[0].CK1_DATE;
                    var endDate = items[items.Count - 1].CK1_DATE;
                    var current = DateTime.Parse(String.Format("{0}-{1}-{2}", beginDate.Year, beginDate.Month, DateTime.DaysInMonth(beginDate.Year, beginDate.Month)));
                    endDate = DateTime.Parse(String.Format("{0}-{1}-{2}", endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month)));
                    var ck1Info = new Dictionary<String, Decimal>();
                    var previous = current.AddMonths(-1);
                    while (current <= endDate)
                    {
                        var endOfMonth = DateTime.Parse(String.Format("{0}-{1}-{2}", current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month)));
                        var temp = items.Where(x => x.CK1_DATE <= current && x.CK1_DATE > previous).ToList();
                        var total = (temp != null && temp.Count > 0) ? temp.Sum(x => x.CUKAI_AMOUNT) : Decimal.Zero;
                        var key = endOfMonth.ToString("MMMM yyyy", cultureID);

                        ck1Info.Add(key, total);
                        if (durations.IndexOf(key) < 0)
                            durations.Add(key);

                        current = current.AddMonths(1);
                        previous = previous.AddMonths(1);
                    }
                    dataLength = Math.Max(dataLength, durations.Count);
                    ck1Data.Add(ptype, items);
                    ck1GroupedData.Add(ptype, ck1Info);
                    grandTotal3.Add(ptype, Decimal.Zero);
                    grandTotal6.Add(ptype, Decimal.Zero);

                }
                tableBuilder.Append("<td style='text-align: center; border: 1px solid black;'>Jumlah</td>")
                    .Append("</tr>")
                    .Append("</thead>");
                #endregion

                #region Build Table Body
                tableBuilder.Append("<tbody>");

                for (var i = 0; i < dataLength; i++)
                {
                    tableBuilder.Append("<tr>");
                    tableBuilder.AppendFormat("<td style='text-align: center;border: 1px solid black;'>{0}</td>", i + 1);
                    tableBuilder.AppendFormat("<td style='border: 1px solid black;'>{0}</td>", durations[i]);
                    var subTotal = Decimal.Zero;
                    foreach (var ptype in productTypes)
                    {
                        if (ck1GroupedData[ptype].ContainsKey(durations[i]))
                        {
                            tableBuilder.AppendFormat("<td style='text-align: right;border: 1px solid black;'>{0:N}</td>", ck1GroupedData[ptype][durations[i]]);
                            subTotal += ck1GroupedData[ptype][durations[i]];
                            var previousSum = Decimal.Zero;
                            if (i >= 3)
                            {
                                previousSum = grandTotal3[ptype];
                                grandTotal3[ptype] = previousSum + ck1GroupedData[ptype][durations[i]];
                            }
                            previousSum = grandTotal6[ptype];
                            grandTotal6[ptype] = previousSum + ck1GroupedData[ptype][durations[i]];
                        }
                        else
                            tableBuilder.AppendFormat("<td style='text-align: right;border: 1px solid black;'>{0:N}</td>", Decimal.Zero);
                    }


                    tableBuilder.AppendFormat("<td style='text-align: right;border: 1px solid black;'>{0:N}</td>", subTotal);
                    tableBuilder.Append("</tr>");

                }
                #region Grand Total 3 Months
                tableBuilder.Append("<tr>");
                tableBuilder.Append("<td colspan='2' style='border: 1px solid black;'>JUMLAH 3 BULAN</td>");
                var grandTotal = Decimal.Zero;
                foreach (var ptype in productTypes)
                {
                    tableBuilder.AppendFormat("<td style='text-align: right;border: 1px solid black;'>{0:N}</td>", grandTotal3[ptype]);
                    grandTotal += grandTotal3[ptype];
                }
                tableBuilder.AppendFormat("<td style='text-align: right;border: 1px solid black;'>{0:N}</td>", grandTotal);
                tableBuilder.Append("</tr>");
                #endregion

                #region Grand Total 6 Months
                tableBuilder.Append("<tr>");
                tableBuilder.Append("<td colspan='2' style='border: 1px solid black;'>JUMLAH 6 BULAN</td>");
                grandTotal = Decimal.Zero;
                foreach (var ptype in productTypes)
                {
                    tableBuilder.AppendFormat("<td style='text-align: right;border: 1px solid black;'>{0:N}</td>", grandTotal6[ptype]);
                    grandTotal += grandTotal6[ptype];
                }
                tableBuilder.AppendFormat("<td style='text-align: right;border: 1px solid black;'>{0:N}</td>", grandTotal);
                tableBuilder.Append("</tr>");
                #endregion

                tableBuilder.Append("</tbody>")
                    .Append("</table>");
                #endregion

                return tableBuilder.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private String GetNewExciseCK1Avg(CalculationDetailModel model, EXCISE_CREDIT excise, int duration,Dictionary<string,decimal> grandTotal6,Dictionary<string,decimal> grandTotal3)
        {
            try
            {
                #region Hack Variable
                StringBuilder builder = new StringBuilder();
                var productTypeCount = model.ProductTypes.Count;
                var calculationData = new Dictionary<String, Double>();

                foreach (var ptype in model.ProductTypes)
                {
                    double amount = 0;
                    if (duration == 6)
                    {
                        amount = (double) grandTotal6[ptype];
                    }
                    else
                    {
                        amount = (double)grandTotal3[ptype];
                    }
                    calculationData.Add(ptype, amount);
                }
                #endregion

                #region Build Table
                builder.Append("<table style='width: 100%; padding: 10px; background-color: white!important;' cellspacing='10'>");
                builder.Append("<tbody>");
                foreach (var ptype in model.ProductTypes)
                {
                    builder.Append("<tr>");
                    builder.Append("<td style='width: 25px'>&nbsp;</td>");
                    builder.AppendFormat("<td style='width: 30%'>{0} sebesar&nbsp;</td>", ptype);
                    builder.AppendFormat("<td style='width: 30%'>Rp. {0:N}/{1}&nbsp;</td>", calculationData[ptype], duration);
                    builder.AppendFormat("<td style='width: 30%'>= Rp. {0:N}</td>", Math.Ceiling(calculationData[ptype] / duration));
                    builder.Append("</tr>");
                }
                builder.Append("</tbody>");
                builder.Append("</table>");
                #endregion

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetCalculationExciseAdjustmentCK1Avg(CalculationDetailModel model, EXCISE_CREDIT excise)
        {
            try
            {
                #region Hack Variable
                StringBuilder builder = new StringBuilder();
                var productTypeCount = model.ProductTypes.Count;
                var calculationData = new Dictionary<String, CalculationDetailModel.CreditAdjustment>();
                service = new ExciseCreditService();
                foreach (var ptype in model.ProductTypes)
                {
                    var amount = model.MaxCreditRange[ptype];
                    calculationData.Add(ptype, amount);
                }
                #endregion

                #region Build Table
                var total = 0D;
                int number = 0;
                foreach (var ptype in model.ProductTypes)
                {
                    builder.AppendFormat("Jenis BKC:{0}", ptype);
                    builder.Append("<table style='width: 100%; padding: 10px' cellspacing='10'>");
                    builder.Append("<thead>");
                    builder.Append("<tr>");
                    builder.Append("<th>No</th>");
                    builder.Append("<th>MEREK</th>");
                    builder.Append("<th>ISI</th>");
                    builder.Append("<th>HJE</th>");
                    builder.Append("<th>Tarif Cukai(Lama)</th>");
                    builder.Append("<th>Tarif Cukai(Baru)</th>");
                    builder.Append("<th>Perubahan</th>");
                    builder.Append("<th>Produksi 2 Bulan Terakhir</th>");
                    builder.Append("<th>Kenaikan Tertimbang</th>");
                    builder.Append("</tr>");
                    builder.Append("</thead>");
                    builder.Append("<tbody>");
                    var exciseResult = excise.EXCISE_CREDIT_ADJUST_CALDETAIL.Where(m => m.PRODUCT_TYPE.PRODUCT_ALIAS == ptype);
                    foreach (var md in exciseResult)
                    {
                        number += 1;
                        var brand = service.GetBrand(md.BRAND_CE);
                        builder.Append("<tr>");
                        builder.AppendFormat("<td>{0}</td>", number);
                        builder.AppendFormat("<td>{0}</td>", md.BRAND_CE);
                        builder.AppendFormat("<td>{0}</td>", brand.BRAND_CONTENT);
                        builder.AppendFormat("<td>{0}</td>", string.Format("{0} {1:N}", brand.HJE_CURR, brand.HJE_IDR));
                        builder.AppendFormat("<td>{0}</td>", md.OLD_TARIFF);
                        builder.AppendFormat("<td>{0}</td>", md.NEW_TARIFF);
                        builder.AppendFormat("<td>{0}</td>", md.INCREASE_TARIFF);
                        builder.AppendFormat("<td>{0}</td>", md.CK1_AMOUNT);
                        builder.AppendFormat("<td>{0}</td>", (md.INCREASE_TARIFF * md.CK1_AMOUNT));
                        builder.Append("</tr>");
                    }
                    number = 0;

                    builder.AppendFormat("<td colspan='8'> Sub total kenaikan cukai yang di berikan penundaan: {0} / {1:N} * 100% : {2:N}</td>", exciseResult.Sum(m => m.INCREASE_TARIFF * m.CK1_AMOUNT), exciseResult.Sum(m => m.CK1_AMOUNT), (exciseResult.Sum(m => m.CK1_AMOUNT) != 0 ? exciseResult.Sum(m => m.INCREASE_TARIFF * m.CK1_AMOUNT) / exciseResult.Sum(m => m.CK1_AMOUNT) * 100 : 0));
                    //                    builder.Append("<td style='width: 25px'>&nbsp;</td>");
                    //                    builder.AppendFormat("<td style='width: 30%'>= Rp. {0:N}</td>", Math.Ceiling(calculationData[ptype].AdditionalValue));
                    Math.Ceiling(calculationData[ptype].AdditionalValue);
                    total += Math.Ceiling(calculationData[ptype].AdditionalValue);
                    builder.Append("</tbody>");
                    builder.Append("</table>");
                    builder.AppendFormat("Jenis BKC:{0}", ptype);
                }

                //                builder.Append("<td>&nbsp;</td>");
                //                builder.Append("<td>&nbsp;</td>");
                //                builder.Append("<td>&nbsp;</td>");
                //                builder.AppendFormat("<td>= Rp. {0:N}</td>", total);
                //                builder.Append("</tr>");
                #endregion

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetNewExciseAdjustmentCK1Avg(CalculationDetailModel model, EXCISE_CREDIT excise, int duration)
        {
            try
            {
                #region Hack Variable
                StringBuilder builder = new StringBuilder();
                var productTypeCount = model.ProductTypes.Count;
                var calculationData = new Dictionary<String, Double>();
                foreach (var ptype in model.ProductTypes)
                {
                    var amount = model.CreditRanges[duration][ptype];
                    calculationData.Add(ptype, amount);
                }
                #endregion

                int count = 0;
                #region Build Table
                builder.Append("<table style='width: 100%; padding: 10px' cellspacing='10'>");
                builder.Append("<thead>");
                builder.Append("<tr>");
                builder.Append("<th style='width:10px'>No</th>");
                builder.Append("<th style='width: 30%'>Jenis BKC</th>");
                builder.Append("<th style='width: 30%'>Nilai Penundaan</th>");
                builder.Append("</tr>");
                builder.Append("</thead>");
                builder.Append("<tbody>");
                foreach (var ptype in model.ProductTypes)
                {
                    count += 1;
                    builder.Append("<tr>");
                    builder.AppendFormat("<td style='width: 10px'>{0}</td>", count);
                    builder.AppendFormat("<td style='width: 30%'>{0}</td>", ptype);
                    builder.AppendFormat("<td style='width: 30%'>Rp. {0:N}&nbsp;</td>", calculationData[ptype]);
                    builder.Append("</tr>");
                }
                builder.Append("</tbody>");
                builder.Append("</table>");
                #endregion

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private String GetNewExciseCK1MaxAvg(CalculationDetailModel model, EXCISE_CREDIT excise, bool additional,Dictionary<string,decimal> grandTotal6, Dictionary<string,decimal> grandTotal3)
        {
            try
            {
                #region Hack Variable
                StringBuilder builder = new StringBuilder();
                var productTypeCount = model.ProductTypes.Count;
                var calculationData = new Dictionary<String, CalculationDetailModel.CreditAdjustment>();
                var calculationMax = new Dictionary<string,decimal>();
                foreach (var ptype in model.ProductTypes)
                {
                    var amount = model.MaxCreditRange[ptype];
                    calculationData.Add(ptype, amount);

                    var amountMax = grandTotal3[ptype] / 3 >= grandTotal6[ptype] /6  ? grandTotal3[ptype] /3 : grandTotal6[ptype] /6;
                    calculationMax.Add(ptype,amountMax);
                }
                #endregion

                #region Build Table
                builder.Append("<table style='width: 100%; padding: 10px; background-color: white!important;' cellspacing='10'>");
                builder.Append("<tbody>");
                var total = 0D;
                foreach (var ptype in model.ProductTypes)
                {
                    builder.Append("<tr>");
                    builder.AppendFormat("<td style='width: 30%'>{0} sebesar&nbsp;</td>", ptype);
                    if (!additional)
                    {
                        builder.AppendFormat("<td style='width: 30%'> : 2 x Rp. {0:N}&nbsp;</td>", calculationMax[ptype]);
                        builder.Append("<td style='width: 25px'>&nbsp;</td>");
                        builder.AppendFormat("<td style='width: 30%'>= Rp. {0:N}</td>", calculationMax[ptype] * 2);
                        total += Math.Ceiling((double)calculationMax[ptype] * 2);
                    }
                    else
                    {
                        builder.AppendFormat("<td style='width: 30%'> : {0}% x Rp. {1:N}&nbsp;</td>", calculationData[ptype].Adjustment * 100, calculationData[ptype].Value);
                        builder.Append("<td style='width: 25px'>&nbsp;</td>");
                        builder.AppendFormat("<td style='width: 30%'>= Rp. {0:N}</td>", Math.Ceiling(calculationData[ptype].AdditionalValue));
                        Math.Ceiling(calculationData[ptype].AdditionalValue);
                        total += Math.Ceiling(calculationData[ptype].AdditionalValue);
                    }

                    builder.Append("</tr>");
                }
                builder.Append("<tr>");
                builder.Append("<td>&nbsp;</td>");
                builder.Append("<td>&nbsp;</td>");
                builder.Append("<td>&nbsp;</td>");
                builder.AppendFormat("<td>= Rp. {0:N}</td>", total);
                builder.Append("</tr>");
                builder.Append("</tbody>");
                builder.Append("</table>");
                #endregion

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetAdjustmentExcise(CalculationDetailModel model, EXCISE_CREDIT excise)
        {
            try
            {
                #region Hack Variable
                StringBuilder builder = new StringBuilder();
                var productTypeCount = model.ProductTypes.Count;
                var calculationData = new Dictionary<String, CalculationDetailModel.CreditAdjustment>();

                foreach (var ptype in model.ProductTypes)
                {
                    var amount = model.MaxCreditRange[ptype];
                    calculationData.Add(ptype, amount);
                }
                #endregion

                #region Build Table
                builder.Append("<table style='width: 100%; padding: 10px' cellspacing='10'>");
                builder.Append("<tbody>");
                var total = 0D;
                foreach (var ptype in model.ProductTypes)
                {
                    var exciseResult = excise.EXCISE_CREDIT_ADJUST_CALDETAIL.Where(m => m.PRODUCT_TYPE.PRODUCT_ALIAS == ptype);
                    var sumck1 = exciseResult.Sum(m => m.CK1_AMOUNT);
                    var sumweightincreased = exciseResult.Sum(m => m.CK1_AMOUNT * m.INCREASE_TARIFF);
                    builder.Append("<tr>");
                    builder.AppendFormat("<td style='width: 30%'>{0}</td>", ptype);

                    builder.AppendFormat("<td style='width: 30%'> : {0:N} + {1:N}&nbsp;</td>", exciseResult.Sum(m => m.INCREASE_TARIFF * m.CK1_AMOUNT) / exciseResult.Sum(m => m.CK1_AMOUNT) * 100, sumck1);
                    builder.Append("<td style='width: 25px'>&nbsp;</td>");
                    builder.AppendFormat("<td style='width: 30%'>= Rp. {0:N}</td>", (exciseResult.Sum(m => m.INCREASE_TARIFF * m.CK1_AMOUNT) / exciseResult.Sum(m => m.CK1_AMOUNT) * 100 * sumck1) / 100);

                    builder.Append("</tr>");
                }
                builder.Append("<tr>");
                builder.Append("<td>&nbsp;</td>");
                builder.Append("<td>&nbsp;</td>");
                builder.Append("<td>&nbsp;</td>");
                builder.AppendFormat("<td>= Rp. {0:N}</td>", (excise.EXCISE_CREDIT_ADJUST_CALDETAIL.Sum(m => m.INCREASE_TARIFF * m.CK1_AMOUNT) / excise.EXCISE_CREDIT_ADJUST_CALDETAIL.Sum(m => m.CK1_AMOUNT) * 100 * excise.EXCISE_CREDIT_ADJUST_CALDETAIL.Sum(m => m.CK1_AMOUNT)) / 100);
                builder.Append("</tr>");
                builder.Append("</tbody>");
                builder.Append("</table>");
                #endregion

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetDelayAdjustmentExcise(CalculationDetailModel model, EXCISE_CREDIT excise)
        {
            try
            {
                #region Hack Variable
                StringBuilder builder = new StringBuilder();
                var productTypeCount = model.ProductTypes.Count;
                var calculationData = new Dictionary<String, CalculationDetailModel.CreditAdjustment>();

                foreach (var ptype in model.ProductTypes)
                {
                    var amount = model.MaxCreditRange[ptype];
                }
                #endregion

                #region Build Table
                builder.Append("<table style='width: 100%; padding: 10px' cellspacing='10'>");
                builder.Append("<tbody>");
                var total = 0D;
                foreach (var ptype in model.ProductTypes)
                {

                    var exciseResult = excise.EXCISE_CREDIT_ADJUST_CALDETAIL.Where(m => m.PRODUCT_TYPE.PRODUCT_ALIAS == ptype);
                    var sumck1 = exciseResult.Sum(m => m.CK1_AMOUNT);
                    var sumweightincreased = exciseResult.Sum(m => m.CK1_AMOUNT * m.INCREASE_TARIFF);
                    builder.Append("<tr>");
                    builder.AppendFormat("<td style='width: 30%'>{0}</td>", ptype);

                    builder.AppendFormat("<td style='width: 30%'> : {0:N} + {1:N}&nbsp;</td>", sumck1, sumweightincreased);
                    builder.Append("<td style='width: 25px'>&nbsp;</td>");
                    builder.AppendFormat("<td style='width: 30%'>= Rp. {0:N}</td>", sumck1 + sumweightincreased);

                    builder.Append("</tr>");
                }
                builder.Append("<tr>");
                builder.Append("<td>&nbsp;</td>");
                builder.Append("<td>&nbsp;</td>");
                builder.Append("<td>&nbsp;</td>");
                builder.AppendFormat("<td>= Rp. {0:N}</td>", excise.EXCISE_CREDIT_ADJUST_CALDETAIL.Sum(m => m.CK1_AMOUNT + (m.CK1_AMOUNT * m.INCREASE_TARIFF)));
                builder.Append("</tr>");
                builder.Append("</tbody>");
                builder.Append("</table>");
                #endregion

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private String GetCK1CalculationReport(EXCISE_CREDIT excise, int period = 6)
        {
            try
            {
                #region Hack Variables
                StringBuilder builder = new StringBuilder();
                System.Globalization.CultureInfo cultureID = new System.Globalization.CultureInfo("id-ID");
                var submitDate = excise.SUBMISSION_DATE;
                var end = DateTime.Parse(String.Format("{0}-{1}-{2}", submitDate.Year, submitDate.Month, 1)).AddDays(-1);
                var start = end.AddMonths(-1 *(period)).AddDays(1);
                var headers = new string[]
                    {
                        "No Urut", "Bulan", "Tanggal CK-1", "Nomor CK-1", "Jumlah Pesanan (lembar)", "Jumlah Cukai (Rp)", "Keterangan"
                    };
                var headerWidth = new string[]
                    {
                        "50px", "15%", "15%", "10%", "15%", "30%", "15%"
                    };
                #endregion

                #region Build Table
                builder.Append("<table style='width: 100%; background-color: white!important; border: black solid 2px'>");

                #region Headers
                builder.Append("<thead>");
                builder.Append("<tr>");
                for (var i = 0; i < headers.Length; i++)
                {
                    builder.AppendFormat("<th style='width: {0}; text-align: center; border: black solid 2px;' valign='middle'>{1}<br />({2})</th>", headerWidth[i], headers[i], i + 1);
                }
                builder.Append("</tr>");
                builder.Append("</thead>");
                #endregion

                #region Content
                builder.Append("<tbody>");
                var list = service.GetCk1ExciseCalculate(excise.NPPBKC_ID, start, end);
                var current = start;
                var number = 1;
                var totalQty = 0;
                var totalAmount = Decimal.Zero;
                
                while (current < end)
                {
                    var nextMonth = current.AddMonths(1);
                    var monthlyCk1 = list.Where(x => x.CK1_DATE < nextMonth && x.CK1_DATE >= current).ToList();
                    var qty = monthlyCk1.Sum(x => x.ORDERQTY);
                    var amount = monthlyCk1.Sum(x => x.NOMINAL);
                    bool first = true;
                   
                    foreach (var item in monthlyCk1)
                    {
                        builder.Append("<tr>");
                        builder.AppendFormat("<td style='text-align: left;  border: black solid 1px'>{0}</td>", number++);
                        if (first)
                        {
                            builder.AppendFormat("<td style='text-align: left; border: black solid 1px' rowspan='{0}'>{1}</td>", monthlyCk1.Count, current.ToString("MMMM yyyy", cultureID));
                            first = false;
                        }
                        builder.AppendFormat("<td style='text-align: left;border: black solid 1px'>{0}</td>", item.CK1_DATE.ToString("dd MMMM yyyy", cultureID));
                        builder.AppendFormat("<td style='text-align: right;border: black solid 1px'>{0}</td>", item.CK1_NUMBER);
                        builder.AppendFormat("<td style='text-align: right;border: black solid 1px'>{0}</td>", item.ORDERQTY);
                        builder.AppendFormat("<td style='text-align: right;border: black solid 1px'>{0:N}</td>", item.NOMINAL);
                        builder.Append("<td style='text-align: left;border: black solid 1px'>&nbsp;</td>");
                        builder.Append("</tr>");
                    }
                    
                    #region Subtotal
                    builder.Append("<tr>");
                    builder.AppendFormat("<td colspan='4'><b>Jumlah</b></td>");
                    builder.AppendFormat("<td style='text-align: right;border: black solid 1px'>{0}</td>", qty);
                    builder.AppendFormat("<td style='text-align: right;border: black solid 1px'>{0:N}</td>", amount);
                    builder.Append("<td style='text-align: left;border: black solid 1px'>&nbsp;</td>");
                    builder.Append("</tr>");
                    totalQty += qty.Value;
                    totalAmount += amount.Value;
                    #endregion
                    current = current.AddMonths(1);
                }
                #region Grand Total
                builder.Append("<tr>");
                builder.AppendFormat("<td colspan='4' style='text-align: center;  border: black solid 2px'><b>Total</b></td>");
                builder.AppendFormat("<td style='text-align: right;  border: black solid 2px'>{0}</td>", totalQty);
                builder.AppendFormat("<td style='text-align: right;  border: black solid 2px'>{0:N}</td>", totalAmount);
                builder.Append("<td style='text-align: left;  border: black solid 2px'>&nbsp;</td>");
                builder.Append("</tr>");
                #endregion
                builder.Append("</tbody>");
                #endregion
                builder.Append("</table>");
                #endregion

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private String GetFinancialRatioSummary(EXCISE_CREDIT excise)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

        public void ExportXlsSummaryReports(ExciseCreditViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryReports(model);


            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsSummaryReports(ExciseCreditViewModel modelExport)
        {
            var documents = service.GetAll().Select(x => this.MapExciseCreditModel(x));

            if (!string.IsNullOrEmpty(modelExport.ExportModel.POAExport))
            {
                documents = documents.Where(x => x.POA == modelExport.ExportModel.POAExport);
            }
            if (!string.IsNullOrEmpty(modelExport.ExportModel.NPPBKCExport))
            {
                documents = documents.Where(x => x.NppbkcId == modelExport.ExportModel.NPPBKCExport);
            }
            if (modelExport.ExportModel.ExciseCreditTypeExport > 0)
            {
                documents = documents.Where(x => x.RequestTypeID == modelExport.ExportModel.ExciseCreditTypeExport);
            }
            if (!string.IsNullOrEmpty(modelExport.ExportModel.CreatorExport))
            {
                documents = documents.Where(x => x.CreatedBy == modelExport.ExportModel.CreatorExport);
            }
            if (modelExport.ExportModel.YearExport > 0)
            {
                documents = documents.Where(x => x.SubmissionDate.Year == modelExport.ExportModel.YearExport);
            }

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcel(slDocument, modelExport.ExportModel);

            iRow++;
            int iColumn = 1;
            foreach (var data in documents)
            {

                iColumn = 1;


                if (modelExport.ExportModel.Type)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestType);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExportModel.SubmitDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate.ToString("dd MMMM yyyy"));
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExportModel.Poa)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedBy);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExportModel.NppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NppbkcId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExportModel.ExciseNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DocumentNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExportModel.Amount)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.AmountDisplay);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExportModel.LastUpdate)
                {
                    var lastUpdate = data.ModifiedDate == null ? data.CreatedDate.ToString("dd MMMM yyyy HH:mm:ss") : data.ModifiedDate.Value.ToString("dd MMMM yyyy HH:mm:ss");
                    slDocument.SetCellValue(iRow, iColumn, lastUpdate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExportModel.Status)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ApprovalStatus.Value);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFile(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, ExciseCreditSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;


            if (modelExport.Type)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.SubmitDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Submit Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.Poa)
            {
                slDocument.SetCellValue(iRow, iColumn, "Poa");
                iColumn = iColumn + 1;
            }

            if (modelExport.NppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nppbkc Id");
                iColumn = iColumn + 1;
            }

            if (modelExport.ExciseNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.Amount)
            {
                slDocument.SetCellValue(iRow, iColumn, "Amount");
                iColumn = iColumn + 1;
            }

            if (modelExport.LastUpdate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Update");
                iColumn = iColumn + 1;
            }

            if (modelExport.Status)
            {
                slDocument.SetCellValue(iRow, iColumn, "Status");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }

        private string CreateXlsFile(SLDocument slDocument, int iColumn, int iRow)
        {

            //create style
            SLStyle styleBorder = slDocument.CreateStyle();
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.SetWrapText(true);

            slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(1, 1, iRow - 1, iColumn - 1, styleBorder);

            var fileName = "ExciseCreditSummaryReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        [HttpPost]
        public PartialViewResult FilterSummaryReports(ExciseCreditViewModel model)
        {
            var documents = service.GetAll().Select(x => this.MapExciseCreditModel(x));

            if (!string.IsNullOrEmpty(model.Filter.POA))
            {
                documents = documents.Where(x => x.POA == model.Filter.POA);
            }
            if (!string.IsNullOrEmpty(model.Filter.NPPBKC))
            {
                documents = documents.Where(x => x.NppbkcId == model.Filter.NPPBKC);
            }
            if (model.Filter.ExciseCreditType > 0)
            {
                documents = documents.Where(x => x.RequestTypeID == model.Filter.ExciseCreditType);
            }
            if (!string.IsNullOrEmpty(model.Filter.Creator))
            {
                documents = documents.Where(x => x.CreatedBy == model.Filter.Creator);
            }
            if (model.Filter.Year > 0)
            {
                documents = documents.Where(x => x.SubmissionDate.Year == model.Filter.Year);
            }

            model.ExciseCreditDocuments = documents.ToList();

            return PartialView("_SummaryReportList", model);
        }
    }
}
