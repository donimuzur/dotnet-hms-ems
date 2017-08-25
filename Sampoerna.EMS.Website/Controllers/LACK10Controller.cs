using System.Configuration;
using System.Data;
using System.IO;
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

                if (item.Lack10Item.Count == 0)
                {
                    AddMessageInfo("No item found", Enums.MessageInfoType.Warning);
                    model = InitialModel(model);
                    return View(model);
                }

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

        #endregion

        #region Completed Document

        public ActionResult CompletedDocument()
        {
            var data = InitIndexDocumentListViewModel(new Lack10IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator ? true : false)
            });
            return View("CompletedDocument", data);
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(Lack10IndexViewModel model)
        {
            model.Detail = GetListDocument(model);
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator ? true : false);
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
    }
}