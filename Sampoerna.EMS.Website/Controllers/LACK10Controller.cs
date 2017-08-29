using System.Configuration;
using System.Data;
using System.IO;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.Dashboard;
using Sampoerna.EMS.Website.Models.LACK10;
using Sampoerna.EMS.Website.Models.Shared;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using SpreadsheetLight;
using Sampoerna.EMS.Utils;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK10Controller : BaseController
    {
        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;

        private ILACK10BLL _lack10Bll;
        private ICompanyBLL _companyBll;
        private IPrintHistoryBLL _printHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IPOABLL _poabll;
        private IMonthBLL _monthBll;
        
        private IWorkflowBLL _workflowBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IHeaderFooterBLL _headerFooterBll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;
        private IPlantBLL _plantBll;

        public LACK10Controller(IPageBLL pageBll, ILACK10BLL lack10Bll, ICompanyBLL companyBll, IChangesHistoryBLL changesHistoryBll, IZaidmExNPPBKCBLL nppbkcbll,
            IPrintHistoryBLL printHistoryBll, IPOABLL poabll, IMonthBLL monthBll, IWorkflowBLL workflowBll, IWorkflowHistoryBLL workflowHistoryBll, IHeaderFooterBLL headerFooterBll,
            IUserPlantMapBLL userPlantBll, IPOAMapBLL poaMapBll, IPlantBLL plantbll)
            : base(pageBll, Enums.MenuList.LACK10)
        {
            _mainMenu = Enums.MenuList.LACK10;

            _lack10Bll = lack10Bll;
            _companyBll = companyBll;
            _printHistoryBll = printHistoryBll;
            _changesHistoryBll = changesHistoryBll;
            _poabll = poabll;
            _monthBll = monthBll;
            _nppbkcbll = nppbkcbll;
            _userPlantBll = userPlantBll;
            _poaMapBll = poaMapBll;
            _plantBll = plantbll;
            
            _workflowBll = workflowBll;
            _workflowHistoryBll = workflowHistoryBll;
            _headerFooterBll = headerFooterBll;
        }

        #endregion


        #region ---------------- Index --------------

        //
        // GET: /LACK10/
        public ActionResult Index()
        {
            var data = InitIndexDocumentListViewModel(new Lack10IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                IsOpenDocument = true,
                IsShowNewButton = (CurrentUser.UserRole != Enums.UserRole.Controller && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator ? true : false),
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator ? true : false)
            });

            return View("Index", data);
        }

        private Lack10IndexViewModel InitIndexDocumentListViewModel(
            Lack10IndexViewModel model)
        {
            var comp = GlobalFunctions.GetCompanyList(_companyBll);
            var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var userComp = _userPlantBll.GetCompanyByUserId(CurrentUser.USER_ID);
                var poaComp = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
                var distinctComp = comp.Where(x => userComp.Contains(x.Value));
                if (CurrentUser.UserRole == Enums.UserRole.POA) distinctComp = comp.Where(x => poaComp.Contains(x.Value));
                var getComp = new SelectList(distinctComp, "Value", "Text");

                comp = getComp;

                var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                var distinctNppbkc = new SelectList(filterNppbkc, "Value", "Text");

                nppbkc = distinctNppbkc;
            }

            model.CompanyNameList = comp;
            model.NppbkcIdList = nppbkc;
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = Lack10YearList();
            model.Month = DateTime.Now.Month.ToString();
            model.Year = DateTime.Now.Year.ToString();
            model.Detail = GetListDocument(model);

            return model;
        }

        private List<DataDocumentList> GetListDocument(Lack10IndexViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var lack10Data = _lack10Bll.GetByParam(new Lack10GetByParamInput()).OrderByDescending(d => d.Lack10Number);
                return Mapper.Map<List<DataDocumentList>>(lack10Data);
            }

            //getbyparams
            var input = Mapper.Map<Lack10GetByParamInput>(filter);
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;

            var dbData = _lack10Bll.GetByParam(input).OrderByDescending(c => c.Lack10Number);
            return Mapper.Map<List<DataDocumentList>>(dbData);
        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(Lack10IndexViewModel model)
        {
            model.IsOpenDocument = true;
            model.Detail = GetListDocument(model);
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator ? true : false);
            return PartialView("_LACK10List", model);
        }

        #endregion


        #region create

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Controller || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new Lack10IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = new DataDocumentList()
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(Lack10IndexViewModel model)
        {
            return View("Create", InitialModel(model));
        }

        private Lack10IndexViewModel InitialModel(Lack10IndexViewModel model)
        {
            var comp = GlobalFunctions.GetCompanyList(_companyBll);
            var userComp = _userPlantBll.GetCompanyByUserId(CurrentUser.USER_ID);
            var poaComp = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
            var distinctComp = comp.Where(x => userComp.Contains(x.Value));
            if (CurrentUser.UserRole == Enums.UserRole.POA) distinctComp = comp.Where(x => poaComp.Contains(x.Value));
            var getComp = new SelectList(distinctComp, "Value", "Text");

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyNameList = getComp;
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = Lack10YearList();
            model.PlanList = GlobalFunctions.GetPlantAll();
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.AllowPrintDocument = false;
            if (model.Details != null) model.Details.SubmissionDate = DateTime.Now;

            return (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lack10IndexViewModel model)
        {
            try
            {
                Lack10Dto item = new Lack10Dto();

                item = AutoMapper.Mapper.Map<Lack10Dto>(model.Details);

                var plant = _plantBll.GetT001WById(model.Details.PlantId);
                var company = _companyBll.GetById(model.Details.CompanyId);
                var nppbkcId = plant == null ? item.NppbkcId : plant.NPPBKC_ID;

                item.NppbkcId = nppbkcId;
                item.PlantName = plant == null ? "" : plant.NAME1;
                item.CompanyName = company.BUTXT;
                item.CompanyNpwp = company.NPWP;
                item.CreatedBy = CurrentUser.USER_ID;
                item.CreatedDate = DateTime.Now;
                item.Status = Enums.DocumentStatus.Draft;

                //same as lack10
                //if (item.Lack10Item.Count == 0)
                //{
                //    AddMessageInfo("No item found", Enums.MessageInfoType.Warning);
                //    model = InitialModel(model);
                //    return View(model);
                //}

                var existLack10 = _lack10Bll.GetByItem(item);
                if (existLack10 != null)
                {
                    AddMessageInfo("Data LACK-10 already exists", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Details", new { id = existLack10.Lack10Id });
                }

                var lack10Data = _lack10Bll.Save(item, CurrentUser.USER_ID);
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                Lack10Workflow(lack10Data.Lack10Id, Enums.ActionType.Created, string.Empty);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        #endregion

        #region Details

        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack10Data = _lack10Bll.GetById(id.Value);

            if (lack10Data == null)
            {
                return HttpNotFound();
            }

            try
            {
                var plant = _plantBll.GetT001WById(lack10Data.PlantId);
                var nppbkcId = lack10Data.NppbkcId;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = lack10Data.Lack10Number;
                workflowInput.DocumentStatus = lack10Data.Status;
                workflowInput.NppbkcId = nppbkcId;
                workflowInput.DocumentCreator = lack10Data.CreatedBy;
                if (plant != null)
                {
                    workflowInput.PlantId = lack10Data.PlantId;
                }

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                var changesHistory =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK10,
                        id.Value.ToString()));

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack10Data.Lack10Number));

                var model = new Lack10IndexViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    Details = Mapper.Map<DataDocumentList>(lack10Data),
                    WorkflowHistory = workflowHistory,
                    ChangesHistoryList = changesHistory,
                    PrintHistoryList = printHistory
                };

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Details.Status);

                model.AllowEditCompleted = _lack10Bll.AllowEditCompletedDocument(lack10Data, CurrentUser.USER_ID);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack10Data = _lack10Bll.GetById(id.Value);

            if (lack10Data == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                return RedirectToAction("Edits", new { id });
            }

            //first code when manager exists
            //if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }

            try
            {
                var plant = _plantBll.GetT001WById(lack10Data.PlantId);
                var nppbkcId = lack10Data.NppbkcId;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = lack10Data.Lack10Number;
                workflowInput.DocumentStatus = lack10Data.Status;
                workflowInput.NppbkcId = nppbkcId;
                workflowInput.DocumentCreator = lack10Data.CreatedBy;
                if (plant != null)
                {
                    workflowInput.PlantId = lack10Data.PlantId;
                }

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                var changesHistory =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK10,
                        id.Value.ToString()));

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack10Data.Lack10Number));

                var model = new Lack10IndexViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    Details = Mapper.Map<DataDocumentList>(lack10Data),
                    WorkflowHistory = workflowHistory,
                    ChangesHistoryList = changesHistory,
                    PrintHistoryList = printHistory
                };

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput
                {
                    DocumentStatus = model.Details.Status,
                    FormView = Enums.FormViewType.Detail,
                    UserRole = CurrentUser.UserRole,
                    CreatedUser = lack10Data.CreatedBy,
                    CurrentUser = CurrentUser.USER_ID,
                    CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                    DocumentNumber = model.Details.Lack10Number,
                    NppbkcId = nppbkcId,
                    ManagerApprove = model.Details.ApprovedByManager,
                    PlantId = lack10Data.PlantId
                };

                model.ActionType = "GovCompletedDocument";

                ////workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;

                if (allowApproveAndReject) model.ActionType = "ApproveDocument";

                //first code when manager exists
                //if (!allowApproveAndReject)
                //{
                //    model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
                //}

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Details.Status);

                model.AllowEditCompleted = _lack10Bll.AllowEditCompletedDocument(lack10Data, CurrentUser.USER_ID);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Edit

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack10Data = _lack10Bll.GetById(id.Value);

            if (lack10Data == null)
            {
                return HttpNotFound();
            }

            var model = new Lack10IndexViewModel();
            model = InitialModel(model);

            //first code when manager exists
            //if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }

            //first code when manager exists
            if (CurrentUser.UserRole == Enums.UserRole.Controller || (CurrentUser.UserRole == Enums.UserRole.POA && lack10Data.Status == Enums.DocumentStatus.WaitingForApproval))
            //if (CurrentUser.UserRole == Enums.UserRole.POA && ck4cData.Status == Enums.DocumentStatus.WaitingForApproval)
            {
                //redirect to details for approval/rejected
                return RedirectToAction("Details", new { id });
            }

            try
            {
                model.Details = Mapper.Map<DataDocumentList>(lack10Data);

                var plant = _plantBll.GetT001WById(lack10Data.PlantId);
                var nppbkcId = lack10Data.NppbkcId;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = lack10Data.Lack10Number;
                workflowInput.DocumentStatus = lack10Data.Status;
                workflowInput.NppbkcId = nppbkcId;
                workflowInput.DocumentCreator = lack10Data.CreatedBy;
                if (plant != null)
                {
                    workflowInput.PlantId = lack10Data.PlantId;
                }

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK10, id.Value.ToString()));

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack10Data.Lack10Number));

                model.WorkflowHistory = workflowHistory;
                model.ChangesHistoryList = changeHistory;
                model.PrintHistoryList = printHistory;

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput
                {
                    DocumentStatus = model.Details.Status,
                    FormView = Enums.FormViewType.Detail,
                    UserRole = CurrentUser.UserRole,
                    CreatedUser = lack10Data.CreatedBy,
                    CurrentUser = CurrentUser.USER_ID,
                    CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                    DocumentNumber = model.Details.Lack10Number,
                    NppbkcId = nppbkcId,
                    PlantId = lack10Data.PlantId
                };

                ////workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;

                if (!allowApproveAndReject)
                {
                    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                }

                if (model.Details.Status == Enums.DocumentStatus.WaitingGovApproval)
                {
                    model.ActionType = "GovApproveDocument";
                }

                if ((model.ActionType == "GovApproveDocument" && model.AllowGovApproveAndReject))
                {

                }
                else if (!ValidateEditDocument(model, false))
                {
                    return RedirectToAction("Details", new { id });
                }

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Details.Status);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lack10IndexViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();

                    if (errors.Count > 0)
                    {
                        //get error details
                    }

                    AddMessageInfo("Model error", Enums.MessageInfoType.Error);
                    model = InitialModel(model);
                    return View(model);
                }

                var dataToSave = Mapper.Map<Lack10Dto>(model.Details);

                if (dataToSave.Lack10Item.Count == 0)
                {
                    AddMessageInfo("No item found", Enums.MessageInfoType.Warning);
                    model.Details.StatusName = "Draft";
                    model = InitialModel(model);
                    return View(model);
                }

                var plant = _plantBll.GetT001WById(model.Details.PlantId);
                var company = _companyBll.GetById(model.Details.CompanyId);
                var nppbkcId = plant == null ? dataToSave.NppbkcId : plant.NPPBKC_ID;

                dataToSave.Status = Enums.DocumentStatus.Draft;
                dataToSave.NppbkcId = nppbkcId;
                dataToSave.PlantName = plant == null ? "" : plant.NAME1;
                dataToSave.CompanyName = company.BUTXT;
                dataToSave.CompanyNpwp = company.NPWP;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.MonthNameIndo = _monthBll.GetMonth(model.Details.PeriodMonth.Value).MONTH_NAME_IND;

                List<Lack10Item> list = dataToSave.Lack10Item;
                foreach (var item in list)
                {
                    item.Lack10Id = dataToSave.Lack10Id;
                }

                dataToSave.Lack10Item = list;

                bool isSubmit = model.Details.IsSaveSubmit == "submit";

                var saveResult = _lack10Bll.Save(dataToSave, CurrentUser.USER_ID);

                if (isSubmit)
                {
                    Lack10Workflow(model.Details.Lack10Id, Enums.ActionType.Submit, string.Empty);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Details", "LACK10", new { id = model.Details.Lack10Id });
                }

                //return RedirectToAction("Index");
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("Edit", new { id = model.Details.Lack10Id });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        public ActionResult Edits(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack10Data = _lack10Bll.GetById(id.Value);

            if (lack10Data == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                return RedirectToAction("Detail", new { id });
            }

            try
            {
                var plant = _plantBll.GetT001WById(lack10Data.PlantId);
                var nppbkcId = lack10Data.NppbkcId;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = lack10Data.Lack10Number;
                workflowInput.DocumentStatus = lack10Data.Status;
                workflowInput.NppbkcId = nppbkcId;
                workflowInput.DocumentCreator = lack10Data.CreatedBy;
                if (plant != null)
                {
                    workflowInput.PlantId = lack10Data.PlantId;
                }

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                var changesHistory =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK10,
                        id.Value.ToString()));

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack10Data.Lack10Number));

                var model = new Lack10IndexViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    Details = Mapper.Map<DataDocumentList>(lack10Data),
                    WorkflowHistory = workflowHistory,
                    ChangesHistoryList = changesHistory,
                    PrintHistoryList = printHistory
                };

                model.ActionType = "GovCompletedDocumentSuperAdmin";

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Details.Status);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Completed Document

        public ActionResult CompletedDocument()
        {
            var data = InitIndexDocumentListViewModel(new Lack10IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
            });
            return View("CompletedDocument", data);
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(Lack10IndexViewModel model)
        {
            model.Detail = GetListDocument(model);
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false);
            return PartialView("_Lack10Completed", model);
        }

        #endregion

        #region Workflow

        private void Lack10Workflow(int id, Enums.ActionType actionType, string comment)
        {
            var input = new Lack10WorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment
            };

            _lack10Bll.Lack10Workflow(input);
        }

        private bool ValidateEditDocument(Lack10IndexViewModel model, bool message = true)
        {

            //check is Allow Edit Document
            var isAllowEditDocument = _workflowBll.AllowEditDocumentPbck1(new WorkflowAllowEditAndSubmitInput()
            {
                DocumentStatus = model.Details.Status,
                CreatedUser = model.Details.CreatedBy,
                CurrentUser = CurrentUser.USER_ID
            });

            if (!isAllowEditDocument)
            {
                AddMessageInfo(
                    "Operation not allowed.",
                    Enums.MessageInfoType.Error);
                return false;
            }

            return true;

        }

        public ActionResult ApproveDocument(Lack10IndexViewModel model)
        {
            bool isSuccess = false;
            try
            {
                var input = new Lack10UpdateSubmissionDate()
                {
                    Id = model.Details.Lack10Id,
                    SubmissionDate = model.Details.SubmissionDate
                };

                _lack10Bll.UpdateSubmissionDate(input);

                Lack10Workflow(model.Details.Lack10Id, Enums.ActionType.Approve, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Details", "LACK10", new { id = model.Details.Lack10Id });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult RejectDocument(Lack10IndexViewModel model)
        {
            bool isSuccess = false;
            try
            {
                Lack10Workflow(model.Details.Lack10Id, Enums.ActionType.Reject, model.Details.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "LACK10", new { id = model.Details.Lack10Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GovApproveDocument(Lack10IndexViewModel model)
        {
            bool isSuccess = false;
            var currentUserId = CurrentUser;
            var message = string.Empty;

            try
            {
                if (model.Details.Status == Enums.DocumentStatus.WaitingGovApproval)
                {
                    model.Details.Lack10DecreeDoc = new List<Lack10DecreeDocModel>();
                    if (model.Details.Lack10DecreeFiles != null)
                    {
                        int counter = 0;
                        foreach (var item in model.Details.Lack10DecreeFiles)
                        {
                            if (item != null)
                            {
                                var filenamecheck = item.FileName;

                                if (filenamecheck.Contains("\\"))
                                {
                                    filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                                }

                                var decreeDoc = new Lack10DecreeDocModel()
                                {
                                    FILE_NAME = filenamecheck,
                                    FILE_PATH = SaveUploadedFile(item, model.Details.Lack10Id, counter),
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Lack10DecreeDoc.Add(decreeDoc);
                                counter += 1;
                            }
                        }
                    }

                    var input = new Lack10UpdateSubmissionDate()
                    {
                        Id = model.Details.Lack10Id,
                        SubmissionDate = model.Details.SubmissionDate
                    };

                    _lack10Bll.UpdateSubmissionDate(input);

                    message = "Document " + EnumHelper.GetDescription(model.Details.StatusGoverment);
                }

                Lack10WorkflowGovApprove(model.Details, model.Details.GovApprovalActionType, model.Details.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "LACK10", new { id = model.Details.Lack10Id });

            AddMessageInfo(message, Enums.MessageInfoType.Success);

            return RedirectToAction("CompletedDocument");
        }

        [HttpPost]
        public ActionResult GovCompletedDocument(Lack10IndexViewModel model)
        {
            bool isSuccess = false;
            var currentUserId = CurrentUser;
            var message = "Document is " + EnumHelper.GetDescription(Enums.DocumentStatus.WaitingGovApproval);
            var actionResult = "Index";

            try
            {
                if (model.Details.Status == Enums.DocumentStatus.Completed)
                {
                    model.Details.Lack10DecreeDoc = new List<Lack10DecreeDocModel>();

                    if (model.Details.Lack10DecreeFiles != null)
                    {
                        int counter = 0;
                        foreach (var item in model.Details.Lack10DecreeFiles)
                        {
                            if (item != null)
                            {
                                var filenamecheck = item.FileName;

                                if (filenamecheck.Contains("\\"))
                                {
                                    filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                                }

                                var decreeDoc = new Lack10DecreeDocModel()
                                {
                                    FILE_NAME = filenamecheck,
                                    FILE_PATH = SaveUploadedFile(item, model.Details.Lack10Id, counter),
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Lack10DecreeDoc.Add(decreeDoc);
                                counter += 1;
                            }
                        }

                        message = "Document " + EnumHelper.GetDescription(model.Details.StatusGoverment);
                        if (model.Details.StatusGoverment == Enums.DocumentStatusGovType2.Approved)
                            message = "Document has been saved";
                        actionResult = "CompletedDocument";
                    }

                    if (model.Details.Lack10UploadedDoc != null)
                    {
                        foreach (var item in model.Details.Lack10UploadedDoc)
                        {
                            if (item != null)
                            {
                                var valueDoc = item.Split('|').ToArray();

                                var decreeDoc = new Lack10DecreeDocModel()
                                {
                                    FILE_NAME = valueDoc[1],
                                    FILE_PATH = valueDoc[0],
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Lack10DecreeDoc.Add(decreeDoc);
                            }
                        }

                        message = "Document " + EnumHelper.GetDescription(model.Details.StatusGoverment);
                        if (model.Details.StatusGoverment == Enums.DocumentStatusGovType2.Approved)
                            message = "Document has been saved";
                        actionResult = "CompletedDocument";
                    }
                }

                Lack10WorkflowCompleted(model.Details, Enums.ActionType.Completed, model.Details.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "LACK10", new { id = model.Details.Lack10Id });

            AddMessageInfo(message, Enums.MessageInfoType.Success);

            return RedirectToAction(actionResult);
        }

        private void Lack10WorkflowCompleted(DataDocumentList lack10Data, Enums.ActionType actionType, string comment)
        {
            if (lack10Data.Status == Enums.DocumentStatus.Completed)
            {
                var input = new Lack10WorkflowDocumentInput();

                if (lack10Data.Lack10DecreeDoc.Count == 0)
                {
                    input = new Lack10WorkflowDocumentInput()
                    {
                        DocumentId = lack10Data.Lack10Id,
                        ActionType = actionType,
                        UserRole = CurrentUser.UserRole,
                        UserId = CurrentUser.USER_ID,
                        DocumentNumber = lack10Data.Lack10Number,
                        Comment = comment
                    };
                }
                else
                {
                    input = new Lack10WorkflowDocumentInput()
                    {
                        DocumentId = lack10Data.Lack10Id,
                        ActionType = actionType,
                        UserRole = CurrentUser.UserRole,
                        UserId = CurrentUser.USER_ID,
                        DocumentNumber = lack10Data.Lack10Number,
                        Comment = comment,
                        AdditionalDocumentData = new Lack10WorkflowDocumentData()
                        {
                            DecreeDate = lack10Data.DecreeDate.Value,
                            Lack10DecreeDoc = Mapper.Map<List<Lack10DecreeDocDto>>(lack10Data.Lack10DecreeDoc)
                        }
                    };
                }

                _lack10Bll.Lack10Workflow(input);
            }
        }

        private string SaveUploadedFile(HttpPostedFileBase file, int lack10Id, int counter)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            //initialize folders in case deleted by an test publish profile
            if (!Directory.Exists(Server.MapPath(Constans.Ck4cDecreeDocFolderPath)))
                Directory.CreateDirectory(Server.MapPath(Constans.Ck4cDecreeDocFolderPath));

            sFileName = Constans.Ck4cDecreeDocFolderPath + Path.GetFileName(lack10Id.ToString("'ID'-##") + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + counter + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        private void Lack10WorkflowGovApprove(DataDocumentList lack10Data, Enums.ActionType actionType, string comment)
        {
            if (lack10Data.Status == Enums.DocumentStatus.WaitingGovApproval)
            {
                var input = new Lack10WorkflowDocumentInput()
                {
                    DocumentId = lack10Data.Lack10Id,
                    ActionType = actionType,
                    UserRole = CurrentUser.UserRole,
                    UserId = CurrentUser.USER_ID,
                    DocumentNumber = lack10Data.Lack10Number,
                    Comment = comment,
                    AdditionalDocumentData = new Lack10WorkflowDocumentData()
                    {
                        DecreeDate = lack10Data.DecreeDate.Value,
                        Lack10DecreeDoc = Mapper.Map<List<Lack10DecreeDocDto>>(lack10Data.Lack10DecreeDoc)
                    }
                };

                _lack10Bll.Lack10Workflow(input);
            }
        }

        [HttpPost]
        public ActionResult GovCompletedDocumentSuperAdmin(Lack10IndexViewModel model)
        {
            bool isSuccess = false;
            var currentUserId = CurrentUser;
            var message = "Document has been saved";
            var actionResult = "CompletedDocument";

            try
            {
                if (model.Details.Status == Enums.DocumentStatus.Completed)
                {
                    model.Details.Lack10DecreeDoc = new List<Lack10DecreeDocModel>();

                    if (model.Details.Lack10DecreeFiles != null)
                    {
                        int counter = 0;
                        foreach (var item in model.Details.Lack10DecreeFiles)
                        {
                            if (item != null)
                            {
                                var filenamecheck = item.FileName;

                                if (filenamecheck.Contains("\\"))
                                {
                                    filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                                }

                                var decreeDoc = new Lack10DecreeDocModel()
                                {
                                    FILE_NAME = filenamecheck,
                                    FILE_PATH = SaveUploadedFile(item, model.Details.Lack10Id, counter),
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Lack10DecreeDoc.Add(decreeDoc);
                                counter += 1;
                            }
                        }
                    }

                    if (model.Details.Lack10UploadedDoc != null)
                    {
                        foreach (var item in model.Details.Lack10UploadedDoc)
                        {
                            if (item != null)
                            {
                                var valueDoc = item.Split('|').ToArray();

                                var decreeDoc = new Lack10DecreeDocModel()
                                {
                                    FILE_NAME = valueDoc[1],
                                    FILE_PATH = valueDoc[0],
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Lack10DecreeDoc.Add(decreeDoc);
                            }
                        }
                    }

                    var input = new Lack10UpdateSubmissionDate()
                    {
                        Id = model.Details.Lack10Id,
                        SubmissionDate = model.Details.SubmissionDate,
                        DecreeDate = model.Details.DecreeDate
                    };

                    _lack10Bll.UpdateSubmissionDate(input);
                }

                Lack10WorkflowCompleted(model.Details, model.Details.GovApprovalActionType, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Edits", "LACK10", new { id = model.Details.Lack10Id });

            AddMessageInfo(message, Enums.MessageInfoType.Success);

            return RedirectToAction(actionResult);
        }

        #endregion

        #region Get List Data

        private SelectList Lack10YearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }

        #endregion

        #region Export To Excel

        public void ExportClientsListToExcel(int id)
        {

            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK10, id.ToString());

            var model = Mapper.Map<List<ChangesHistoryItemModel>>(listHistory);

            var grid = new GridView
            {
                DataSource = from d in model
                             select new
                             {
                                 Date = d.MODIFIED_DATE.HasValue ? d.MODIFIED_DATE.Value.ToString("dd MMM yyyy HH:mm:ss") : string.Empty,
                                 FieldName = d.FIELD_NAME,
                                 OldValue = d.OLD_VALUE,
                                 NewValue = d.NEW_VALUE,
                                 User = d.USERNAME

                             }
            };

            grid.DataBind();

            var fileName = "LACK10" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //'Excel 2003 : "application/vnd.ms-excel"
            //'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());

            Response.Flush();

            Response.End();

        }

        public void ExportXlsLack10Item(int id)
        {
            string pathFile = "";

            pathFile = CreateXlsLack10Item(id);

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

        private string CreateXlsLack10Item(int lack10Id)
        {
            var dataExportItem = SearchDataItem(lack10Id);

            int iRow = 7;
            var slDocument = new SLDocument();

            slDocument.SetCellValue(1, 1, "Nama Pabrik");
            slDocument.SetCellValue(1, 2, ": " + dataExportItem.CompanyName);

            slDocument.SetCellValue(2, 1, "NPPBKC");
            slDocument.SetCellValue(2, 2, ": " + dataExportItem.Nppbkc);

            slDocument.SetCellValue(3, 1, "Alamat Pabrik");
            slDocument.SetCellValue(3, 2, ": " + dataExportItem.CompanyAddress);

            slDocument.SetCellValue(4, 1, "Periode");
            slDocument.SetCellValue(4, 2, ": " + dataExportItem.MonthName + " - " + dataExportItem.Year);

            //create header
            slDocument = CreateHeaderExcelForLack10Item(slDocument);

            int iColumn = 1;
            foreach (var data in dataExportItem.ItemList)
            {
                iColumn = 1;

                slDocument.SetCellValue(iRow, iColumn, data.FaCode);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Werks);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Type);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.WasteValue);
                iColumn = iColumn + 1;

                iRow++;
            }

            return CreateXlsFile(slDocument, iColumn, iRow);

        }

        private Lack10Export SearchDataItem(int lack10Id)
        {
            Lack10ExportDto dbData;

            dbData = _lack10Bll.GetLack10ExportById(lack10Id);

            return Mapper.Map<Lack10Export>(dbData);
        }

        private SLDocument CreateHeaderExcelForLack10Item(SLDocument slDocument)
        {
            int iColumn = 1;
            int iRow = 6;

            slDocument.SetCellValue(iRow, iColumn, "FA Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Type");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Waste Value");
            iColumn = iColumn + 1;

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
            slDocument.SetCellStyle(6, 1, iRow - 1, iColumn - 1, styleBorder);

            var fileName = "LACK10_Item_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        private string CreateXlsFileForSummary(SLDocument slDocument, int iColumn, int iRow)
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

            var fileName = "LACK10_Summary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        public void ExportXlsSummaryReports(SummaryReportViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryReports(model.ExportModel);


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

        private string CreateXlsSummaryReports(ExportSummaryReportsViewModel modelExport)
        {
            var filterModel = new SearchSummaryReportsViewModel();
            filterModel.Lack10No = modelExport.Lack10Number;
            filterModel.NppbkcId = modelExport.NppbkcId;
            filterModel.Poa = modelExport.PoaSearch;
            filterModel.Creator = modelExport.CreatorSearch;
            filterModel.Month = modelExport.MonthSearch;
            filterModel.Year = modelExport.YearSearch;
            filterModel.isForExport = true;

            var dataSummaryReport = SearchDataSummaryReports(filterModel);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcel(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;


                if (modelExport.Lack10No)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack10No);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Status)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Status);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BasedOn)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BasedOn);
                    iColumn = iColumn + 1;
                }
                if (modelExport.CeOffice)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CeOffice);
                    iColumn = iColumn + 1;
                }
                if (modelExport.LicenseNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LicenseNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Kppbc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Kppbc);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Month)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Month);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Year)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Year);
                    iColumn = iColumn + 1;
                }


                if (modelExport.FaCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.FaCode.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.BrandDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.BrandDesc.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Werks)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.Werks.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.PlantName)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.PlantName.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Type)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.Type.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.WasteValue)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.WasteValue.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Uom)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.Uom.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Creator)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Creator);
                    iColumn = iColumn + 1;
                }

                if (modelExport.PoaApproved)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PoaApproved);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SubmissionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CompletedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompletedDate);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileForSummary(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;


            if (modelExport.Lack10No)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-10 No");
                iColumn = iColumn + 1;
            }

            if (modelExport.Status)
            {
                slDocument.SetCellValue(iRow, iColumn, "Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.BasedOn)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-10 Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.CeOffice)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.LicenseNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.Kppbc)
            {
                slDocument.SetCellValue(iRow, iColumn, "KPPBC");
                iColumn = iColumn + 1;
            }

            if (modelExport.Month)
            {
                slDocument.SetCellValue(iRow, iColumn, "Month");
                iColumn = iColumn + 1;
            }

            if (modelExport.Year)
            {
                slDocument.SetCellValue(iRow, iColumn, "Year");
                iColumn = iColumn + 1;
            }

            if (modelExport.FaCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "FA Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.BrandDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Brand Description");
                iColumn = iColumn + 1;
            }

            if (modelExport.Werks)
            {
                slDocument.SetCellValue(iRow, iColumn, "Plant Id");
                iColumn = iColumn + 1;
            }

            if (modelExport.PlantName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Plant Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.Type)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.WasteValue)
            {
                slDocument.SetCellValue(iRow, iColumn, "Waste Value");
                iColumn = iColumn + 1;
            }

            if (modelExport.Uom)
            {
                slDocument.SetCellValue(iRow, iColumn, "UOM");
                iColumn = iColumn + 1;
            }

            if (modelExport.Creator)
            {
                slDocument.SetCellValue(iRow, iColumn, "Creator");
                iColumn = iColumn + 1;
            }

            if (modelExport.PoaApproved)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA Approved by");
                iColumn = iColumn + 1;
            }

            if (modelExport.SubmissionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Submission Date");
                iColumn = iColumn + 1;
            }


            if (modelExport.CompletedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Completed Date");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }

        #endregion

        #region Summary Reports

        public ActionResult SummaryReports()
        {

            SummaryReportViewModel model;
            try
            {

                model = new SummaryReportViewModel();


                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new SummaryReportViewModel();
                model.MainMenu = Enums.MenuList.LACK10;
                model.CurrentMenu = PageInfo;
            }

            return View("SummaryReport", model);
        }

        private SummaryReportViewModel InitSummaryReports(SummaryReportViewModel model)
        {
            model.MainMenu = Enums.MenuList.LACK10;
            model.CurrentMenu = PageInfo;
            model.SearchView.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.SearchView.YearList = Lack10YearList();
            model.SearchView.Month = DateTime.Now.Month.ToString();
            model.SearchView.Year = DateTime.Now.Year.ToString();

            var filter = new SearchSummaryReportsViewModel();
            filter.Month = model.SearchView.Month;
            filter.Year = model.SearchView.Year;

            model.DetailsList = SearchDataSummaryReports(filter);

            model.SearchView.Lack10NoList = GetLack10NumberList(model.DetailsList);
            model.SearchView.NppbkcIdList = GetNppbkcList(model.DetailsList);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchView.PoaList = GlobalFunctions.GetPoaAll(_poabll);

            return model;
        }

        private SelectList GetLack10NumberList(List<SummaryReportsItem> listLack10)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listLack10
                    select new SelectItemModel()
                    {
                        ValueField = x.Lack10No,
                        TextField = x.Lack10No
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetNppbkcList(List<SummaryReportsItem> listLack10)
        {

            IEnumerable<SelectItemModel> query;

            query = from x in listLack10
                    select new SelectItemModel()
                    {
                        ValueField = x.LicenseNumber,
                        TextField = x.LicenseNumber
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private List<SummaryReportsItem> SearchDataSummaryReports(SearchSummaryReportsViewModel filter = null)
        {
            Lack10GetSummaryReportByParamInput input;
            List<Lack10SummaryReportDto> dbData;
            List<SummaryReportsItem> retData = new List<SummaryReportsItem>();
            if (filter == null)
            {
                //Get All
                input = new Lack10GetSummaryReportByParamInput();

                dbData = _lack10Bll.GetSummaryReportsByParam(input);
                retData = Mapper.Map<List<SummaryReportsItem>>(dbData);

                return retData;
            }

            //getbyparams

            input = Mapper.Map<Lack10GetSummaryReportByParamInput>(filter);
            input.UserRole = CurrentUser.UserRole;
            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;

            dbData = _lack10Bll.GetSummaryReportsByParam(input);
            retData = Mapper.Map<List<SummaryReportsItem>>(dbData);

            return retData;
        }

        [HttpPost]
        public JsonResult SearchSummaryReportsAjax(DTParameters<SummaryReportViewModel> param)
        {
            var model = param.ExtraFilter;

            var data = model != null ? SearchDataSummaryReports(model.SearchView) : SearchDataSummaryReports();
            DTResult<SummaryReportsItem> result = new DTResult<SummaryReportsItem>();
            result.draw = param.Draw;
            result.recordsFiltered = data.Count;
            result.recordsTotal = data.Count;
            //param.TotalData = data.Count;
            //if (param != null && param.Start > 0)
            //{
            IEnumerable<SummaryReportsItem> dataordered;
            dataordered = data;
            if (param.Order.Length > 0)
            {
                foreach (var ordr in param.Order)
                {
                    if (ordr.Column == 0)
                    {
                        continue;
                    }
                    dataordered = SummaryReportsDataOrder(SummaryReportsOrderByIndex(ordr.Column), ordr.Dir, dataordered);
                }
            }
            data = dataordered.ToList();
            data = data.Skip(param.Start).Take(param.Length).ToList();

            //}
            result.data = data;

            return Json(result);

        }

        private IEnumerable<SummaryReportsItem> SummaryReportsDataOrder(string column, DTOrderDir dir, IEnumerable<SummaryReportsItem> data)
        {

            switch (column)
            {
                case "Lack10No": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.Lack10No).ToList() : data.OrderByDescending(x => x.Lack10No).ToList();
                case "CeOffice": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.CeOffice).ToList() : data.OrderByDescending(x => x.CeOffice).ToList();
                case "BasedOn": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.BasedOn).ToList() : data.OrderByDescending(x => x.BasedOn).ToList();
                case "LicenseNumber": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.LicenseNumber).ToList() : data.OrderByDescending(x => x.LicenseNumber).ToList();
                case "Kppbc": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.Kppbc).ToList() : data.OrderByDescending(x => x.Kppbc).ToList();
                case "SubmissionDate": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.SubmissionDate).ToList() : data.OrderByDescending(x => x.SubmissionDate).ToList();
                case "Month": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.Month).ToList() : data.OrderByDescending(x => x.Month).ToList();
                case "Year": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.Year).ToList() : data.OrderByDescending(x => x.Year).ToList();
                case "PoaApproved": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.PoaApproved).ToList() : data.OrderByDescending(x => x.PoaApproved).ToList();
                case "ManagerApproved": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.ManagerApproved).ToList() : data.OrderByDescending(x => x.ManagerApproved).ToList();
                case "Status": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.Status).ToList() : data.OrderByDescending(x => x.Status).ToList();
                case "CompletedDate": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.CompletedDate).ToList() : data.OrderByDescending(x => x.CompletedDate).ToList();
                case "Creator": return dir == DTOrderDir.ASC ? data.OrderBy(x => x.Creator).ToList() : data.OrderByDescending(x => x.Creator).ToList();

            }
            return null;
        }

        private string SummaryReportsOrderByIndex(int index)
        {
            Dictionary<int, string> columnDict = new Dictionary<int, string>();
            columnDict.Add(1, "Lack10No");
            columnDict.Add(2, "CeOffice");
            columnDict.Add(3, "BasedOn");
            columnDict.Add(4, "LicenseNumber");
            columnDict.Add(5, "Kppbc");
            columnDict.Add(6, "SubmissionDate");
            columnDict.Add(7, "Month");
            columnDict.Add(8, "Year");
            columnDict.Add(9, "PoaApproved");
            columnDict.Add(10, "ManagerApproved");
            columnDict.Add(11, "Status");
            columnDict.Add(12, "CompletedDate");
            columnDict.Add(13, "Creator");



            return columnDict[index];
        }

        #endregion

        #region Json

        [HttpPost]
        public JsonResult CompanyListPartialDocument(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var filterPlant = listPlant;

            var newListPlant = new SelectList(filterPlant, "Value", "Text");

            if (CurrentUser.UserRole == Enums.UserRole.User || CurrentUser.UserRole == Enums.UserRole.POA)
            {
                var newFilterPlant = listPlant.Where(x => CurrentUser.ListUserPlants.Contains(x.Value));

                newListPlant = new SelectList(newFilterPlant, "Value", "Text");
            }

            var model = new Lack10IndexViewModel() { PlanList = newListPlant };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetNppbkcByCompanyId(string companyId)
        {
            var data = _nppbkcbll.GetNppbkcsByCompany(companyId);

            data = data.Where(x => CurrentUser.ListUserNppbkc.Contains(x.NPPBKC_ID)).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetAllNppbkc()
        {
            var listNppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            var model = new Lack10IndexViewModel() { NppbkcIdList = listNppbkc };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetPoaByPlantId(string plantId, string documentCreator)
        {
            var creator = documentCreator == null ? CurrentUser.USER_ID : documentCreator;
            var plant = _plantBll.GetT001WById(plantId);
            var creatorPoa = _poabll.GetById(CurrentUser.USER_ID);
            var listPoa = creatorPoa != null ? _poabll.GetPoaByNppbkcIdAndMainPlant(plant.NPPBKC_ID).Where(x => x.POA_ID != creator).ToList() :
                            _poabll.GetPoaActiveByPlantId(plantId);
            var model = new Lack10IndexViewModel() { PoaList = new SelectList(listPoa.Distinct(), "POA_ID", "PRINTED_NAME") };

            return Json(model);
        }

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId, string documentCreator)
        {
            var creator = documentCreator == null ? CurrentUser.USER_ID : documentCreator;
            var listPoa = _poabll.GetPoaByNppbkcIdAndMainPlant(nppbkcId).Where(x => x.POA_ID != creator).ToList();
            var model = new Lack10IndexViewModel() { PoaList = new SelectList(listPoa.Distinct(), "POA_ID", "PRINTED_NAME") };
            return Json(model);
        }

        [HttpPost]
        public JsonResult GenerateWasteData(string comp, string plant, string nppbkc, int month, int year, bool isNppbkc)
        {
            var input = new Lack10GetWasteDataInput();
            input.CompanyId = comp;
            input.PlantId = plant;
            input.NppbkcId = nppbkc;
            input.Month = month;
            input.Year = year;
            input.IsNppbkc = isNppbkc;

            var data = _lack10Bll.GenerateWasteData(input);

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetNpwpByCompany(string company)
        {
            var data = _companyBll.GetById(company);

            return Json(data.NPWP);
        }

        #endregion

        #region Print and print preview

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            Stream stream = GetReportStream(id.Value, "LACK-10");
            return File(stream, "application/pdf");
        }

        [EncryptedParameter]
        public ActionResult PrintPreview(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            Stream stream = GetReportStream(id.Value, "PREVIEW LACK-10");
            return File(stream, "application/pdf");
        }

        private Stream GetReportStream(int id, string printTitle)
        {
            var lack10 = _lack10Bll.GetById(id);

            var dsLack10 = CreateLack10Ds();
            var dt = dsLack10.Tables[0];
            DataRow drow;
            drow = dt.NewRow();
            drow[0] = lack10.CompanyName;
            drow[1] = lack10.NppbkcId;

            var plant = _plantBll.GetT001WById(lack10.PlantId);
            var mainPlant = _plantBll.GetMainPlantByNppbkcId(lack10.NppbkcId);
            string plantAddress = mainPlant.ADDRESS;
            string plantCity = mainPlant.ORT01;
            if (plant != null)
            {
                plantAddress = plant.ADDRESS;
                plantCity = plant.ORT01;
            }

            drow[2] = plantAddress;

            var headerFooter = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput
            {
                CompanyCode = lack10.CompanyId,
                FormTypeId = Enums.FormType.LACK10
            });
            if (headerFooter != null)
            {
                drow[3] = GetHeader(headerFooter.HEADER_IMAGE_PATH);
                drow[4] = headerFooter.FOOTER_CONTENT.Replace("<br />", Environment.NewLine);
            }
            drow[5] = lack10.MonthNameIndo + " " + lack10.PeriodYears;
            drow[6] = plantCity;

            drow[7] = lack10.SubmissionDate == null ? null : string.Format("{0} {1} {2}", lack10.SubmissionDate.Value.Day, _monthBll.GetMonth(lack10.SubmissionDate.Value.Month).MONTH_NAME_IND, lack10.SubmissionDate.Value.Year);

            var creatorPoa = _poabll.GetById(lack10.CreatedBy);
            var poaUser = creatorPoa == null ? lack10.ApprovedBy : lack10.CreatedBy;

            var poa = _poabll.GetDetailsById(poaUser);
            if (poa != null)
            {
                drow[8] = poa.PRINTED_NAME;
            }

            drow[9] = printTitle;
            if (lack10.DecreeDate != null)
            {
                var lack2DecreeDate = lack10.DecreeDate.Value;
                var lack2Month = _monthBll.GetMonth(lack2DecreeDate.Month).MONTH_NAME_IND;

                drow[10] = string.Format("{0} {1} {2}", lack2DecreeDate.Day, lack2Month, lack2DecreeDate.Year);

            }

            var totalTembakau = lack10.Lack10Item.Where(x => x.Type == "Hasil Tembakau").Sum(x => x.WasteValue);
            var totalTis = lack10.Lack10Item.Where(x => x.Type == "TIS").Sum(x => x.WasteValue);
            var totalTisCase = lack10.Lack10Item.Where(x => x.Type != "TIS" && x.Type != "Hasil Tembakau").Sum(x => x.WasteValue);
            var total = totalTembakau.ToString("N3") + " Batang , " + totalTis.ToString("N3") + " Kg";
            if (totalTis == 0) total = totalTembakau.ToString("N3") + " Batang";
            if (totalTembakau == 0) total = totalTis.ToString("N3") + " Kg";

            drow[11] = lack10.CompanyNpwp;
            drow[12] = total;
            drow[13] = totalTembakau.ToString("N3");
            drow[14] = totalTis.ToString("N3");
            drow[15] = EnumHelper.GetDescription(lack10.ReportType).ToUpper();
            drow[16] = lack10.Reason;
            drow[17] = lack10.Remark;
            drow[18] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(EnumHelper.GetDescription(lack10.ReportType));

            var typeRpt = "Preview.rpt";
            if (totalTembakau == 0) typeRpt = "PreviewTis.rpt";
            if (totalTis == 0) typeRpt = "PreviewHt.rpt";
            if (lack10.Lack10Item.Count == 0) typeRpt = "PreviewNoData.rpt";
            if (totalTisCase > 0)
            {
                typeRpt = "PreviewTisCase.rpt";
                total = lack10.Lack10Item.Where(x => x.Type != "Hasil Tembakau").Sum(x => x.WasteValue).ToString("N3") + " Kg";

                drow[12] = total;
                drow[19] = lack10.Lack10Item.Where(x => x.Type != "TIS" && x.Type != "Hasil Tembakau").FirstOrDefault().Type;
                drow[20] = totalTisCase.ToString("N3");
            }

            dt.Rows.Add(drow);

            ReportClass rpt = new ReportClass();
            string report_path = ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = report_path + "LACK10\\" + typeRpt;
            rpt.Load();
            rpt.SetDataSource(dsLack10);

            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return stream;
        }

        private DataSet CreateLack10Ds()
        {
            DataSet ds = new DataSet("dsLack10");

            DataTable dt = new DataTable("Lack10");

            dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
            dt.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dt.Columns.Add("Alamat", System.Type.GetType("System.String"));
            dt.Columns.Add("Header", System.Type.GetType("System.Byte[]"));
            dt.Columns.Add("Footer", System.Type.GetType("System.String"));
            dt.Columns.Add("Period", System.Type.GetType("System.String"));
            dt.Columns.Add("City", System.Type.GetType("System.String"));
            dt.Columns.Add("CreatedDate", System.Type.GetType("System.String"));
            dt.Columns.Add("PoaPrintedName", System.Type.GetType("System.String"));
            dt.Columns.Add("Preview", System.Type.GetType("System.String"));
            dt.Columns.Add("DecreeDate", System.Type.GetType("System.String"));
            dt.Columns.Add("Npwp", System.Type.GetType("System.String"));
            dt.Columns.Add("Total", System.Type.GetType("System.String"));
            dt.Columns.Add("TotalTembakau", System.Type.GetType("System.String"));
            dt.Columns.Add("TotalTis", System.Type.GetType("System.String"));
            dt.Columns.Add("TypeTitle", System.Type.GetType("System.String"));
            dt.Columns.Add("Reason", System.Type.GetType("System.String"));
            dt.Columns.Add("Remark", System.Type.GetType("System.String"));
            dt.Columns.Add("TypeTable", System.Type.GetType("System.String"));
            dt.Columns.Add("TisCase", System.Type.GetType("System.String"));
            dt.Columns.Add("TisCaseTotal", System.Type.GetType("System.String"));

            ds.Tables.Add(dt);
            return ds;
        }

        private byte[] GetHeader(string imagePath)
        {
            byte[] imgbyte = null;
            try
            {

                FileStream fs;
                BinaryReader br;

                if (System.IO.File.Exists(Server.MapPath(imagePath)))
                {
                    fs = new FileStream(Server.MapPath(imagePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                else
                {
                    // if photo does not exist show the nophoto.jpg file 
                    fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                // initialise the binary reader from file streamobject 
                br = new BinaryReader(fs);
                // define the byte array of filelength 
                imgbyte = new byte[fs.Length + 1];
                // read the bytes from the binary reader 
                imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));


                br.Close();
                // close the binary reader 
                fs.Close();
                // close the file stream 

            }
            catch (Exception)
            {
            }
            return imgbyte;
            // Return Datatable After Image Row Insertion

        }

        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var lack10 = _lack10Bll.GetById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.LACK10,
                FORM_ID = lack10.Lack10Id,
                FORM_NUMBER = lack10.Lack10Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack10.Lack10Number));
            return PartialView("_PrintHistoryTable", model);

        }

        #endregion
    }
}