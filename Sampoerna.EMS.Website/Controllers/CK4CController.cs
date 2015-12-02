using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.ReportingData;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.CK4C;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System.Configuration;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class CK4CController : BaseController
    {
        private ICK4CBLL _ck4CBll;
        private IMonthBLL _monthBll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poabll;
        private ICompanyBLL _companyBll;
        private IPlantBLL _plantBll;
        private IT001KBLL _t001KBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IProductionBLL _productionBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IWorkflowBLL _workflowBll;
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IHeaderFooterBLL _headerFooterBll;
        private IPrintHistoryBLL _printHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;

        public CK4CController(IPageBLL pageBll, IPOABLL poabll, ICK4CBLL ck4Cbll, IPlantBLL plantbll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll,
            IBrandRegistrationBLL brandRegistrationBll, ICompanyBLL companyBll, IT001KBLL t001Kbll, IZaidmExNPPBKCBLL nppbkcbll, IProductionBLL productionBll,
            IWorkflowHistoryBLL workflowHistoryBll, IWorkflowBLL workflowBll, IZaidmExProdTypeBLL prodTypeBll,
            IHeaderFooterBLL headerFooterBll, IPrintHistoryBLL printHistoryBll, IChangesHistoryBLL changesHistoryBll, IUserPlantMapBLL userPlantBll, IPOAMapBLL poaMapBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _ck4CBll = ck4Cbll;
            _plantBll = plantbll;
            _monthBll = monthBll;
            _poabll = poabll;
            _companyBll = companyBll;
            _mainMenu = Enums.MenuList.CK4C;
            _t001KBll = t001Kbll;
            _uomBll = uomBll;
            _brandRegistrationBll = brandRegistrationBll;
            _nppbkcbll = nppbkcbll;
            _productionBll = productionBll;
            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
            _prodTypeBll = prodTypeBll;
            _headerFooterBll = headerFooterBll;
            _printHistoryBll = printHistoryBll;
            _changesHistoryBll = changesHistoryBll;
            _userPlantBll = userPlantBll;
            _poaMapBll = poaMapBll;
        }

        #region Index Document List

        public ActionResult DocumentList()
        {
            var data = InitIndexDocumentListViewModel(new Ck4CIndexDocumentListViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.Ck4CDocument,
                IsShowNewButton = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer
            });

            return View("DocumentList", data);
        }

        private Ck4CIndexDocumentListViewModel InitIndexDocumentListViewModel(
            Ck4CIndexDocumentListViewModel model)
        {
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll); ;
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            switch (model.Ck4CType)
            {
                case Enums.CK4CType.CompletedDocument:
                    model.Detail = GetCompletedDocument(model);
                    var listCk4cCompleted = _ck4CBll.GetCompletedDocument();
                    model.DocumentNumberList = new SelectList(listCk4cCompleted, "NUMBER", "NUMBER");
                    break;
                case Enums.CK4CType.Ck4CDocument:
                    model.Detail = GetOpenDocument(model);
                    var listCk4cData = _ck4CBll.GetOpenDocument();
                    model.DocumentNumberList = new SelectList(listCk4cData, "NUMBER", "NUMBER");
                    break;
            }

            return model;
        }

        private List<DataDocumentList> GetOpenDocument(Ck4CIndexDocumentListViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var ck4cData = _ck4CBll.GetOpenDocumentByParam(new Ck4cGetOpenDocumentByParamInput()).OrderByDescending(d => d.Number);
                return Mapper.Map<List<DataDocumentList>>(ck4cData);
            }

            //getbyparams
            var input = Mapper.Map<Ck4cGetOpenDocumentByParamInput>(filter);
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;

            var dbData = _ck4CBll.GetOpenDocumentByParam(input).OrderByDescending(c => c.Number);
            return Mapper.Map<List<DataDocumentList>>(dbData);
        }

        private List<DataDocumentList> GetCompletedDocument(Ck4CIndexDocumentListViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var ck4cData = _ck4CBll.GetCompletedDocumentByParam(new Ck4cGetCompletedDocumentByParamInput()).OrderByDescending(d => d.Number);
                return Mapper.Map<List<DataDocumentList>>(ck4cData);
            }

            //getbyparams
            var input = Mapper.Map<Ck4cGetCompletedDocumentByParamInput>(filter);
            var dbData = _ck4CBll.GetCompletedDocumentByParam(input).OrderByDescending(d => d.Number);
            return Mapper.Map<List<DataDocumentList>>(dbData);
        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(Ck4CIndexDocumentListViewModel model)
        {
            model.Detail = GetOpenDocument(model);
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            return PartialView("_CK4CTableDocumentList", model);
        }

        #endregion

        #region Completed Document

        public ActionResult CompletedDocument()
        {
            var data = InitIndexDocumentListViewModel(new Ck4CIndexDocumentListViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.CompletedDocument,
                IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer
            });
            return View("CompletedDocument", data);
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(Ck4CIndexDocumentListViewModel model)
        {
            model.Detail = GetCompletedDocument(model);
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            return PartialView("_CK4CTableCompletedDocument", model);
        }

        #endregion

        #region Json

        [HttpPost]
        public JsonResult CompanyListPartialCk4CDocument(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var userPlant = _userPlantBll.GetPlantByUserId(CurrentUser.USER_ID);

            var poaMap = _poaMapBll.GetPlantByPoaId(CurrentUser.USER_ID);

            var filterPlant = listPlant.Where(x => userPlant.Contains(x.Value) || poaMap.Contains(x.Value));

            var newListPlant = new SelectList(filterPlant, "Value", "Text");

            var model = new Ck4CIndexDocumentListViewModel() { PlanList = newListPlant };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetNppbkcByCompanyId(string companyId)
        {
            var data = _nppbkcbll.GetNppbkcsByCompany(companyId);

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetAllNppbkc()
        {
            var listNppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            var model = new Ck4CIndexDocumentListViewModel() { NppbkcIdList = listNppbkc };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetFaCodeDescription(string plantWerk, string faCode)
        {
            var fa = _brandRegistrationBll.GetByFaCode(plantWerk, faCode);
            return Json(fa.BRAND_CE);
        }

        [HttpPost]
        public JsonResult GetProductionData(string comp, string plant, string nppbkc, int period, int month, int year, bool isNppbkc)
        {
            var data = _productionBll.GetByCompPlant(comp, plant, nppbkc, period, month, year, isNppbkc).ToList();

            var result = _productionBll.GetExactResult(data);

            return Json(data);
        }

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId)
        {
            var listPoa = _poabll.GetPoaByNppbkcIdAndMainPlant(nppbkcId);
            var model = new Ck4CIndexDocumentListViewModel() { PoaList = new SelectList(listPoa, "POA_ID", "PRINTED_NAME") };
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetPoaByPlantId(string plantId)
        {
            var listPoa = _poabll.GetPoaActiveByPlantId(plantId);
            var model = new Ck4CIndexDocumentListViewModel() { PoaList = new SelectList(listPoa, "POA_ID", "PRINTED_NAME") };

            return Json(model);
        }

        #endregion

        #region create Document List
        public ActionResult Ck4CCreateDocumentList()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager || CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("DocumentList");
            }

            var model = new Ck4CIndexDocumentListViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = new DataDocumentList()
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(Ck4CIndexDocumentListViewModel model)
        {
            return View("Ck4CCreateDocumentList", InitialModel(model));
        }

        private Ck4CIndexDocumentListViewModel InitialModel(Ck4CIndexDocumentListViewModel model)
        {
            var comp = GlobalFunctions.GetCompanyList(_companyBll);
            var userComp = _userPlantBll.GetCompanyByUserId(CurrentUser.USER_ID);
            var poaComp = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
            var distinctComp = comp.Where(x => userComp.Contains(x.Value) || poaComp.Contains(x.Value));
            var getComp = new SelectList(distinctComp, "Value", "Text");

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyNameList = getComp;
            model.PeriodList = Ck4cPeriodList();
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = Ck4cYearList();
            model.PlanList = GlobalFunctions.GetPlantAll();
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.AllowPrintDocument = false;
            if (model.Details != null) model.Details.ReportedOn = DateTime.Now;

            return (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ck4CCreateDocumentList(Ck4CIndexDocumentListViewModel model)
        {
            try
            {
                Ck4CDto item = new Ck4CDto();

                item = AutoMapper.Mapper.Map<Ck4CDto>(model.Details);

                var plant = _plantBll.GetT001WById(model.Details.PlantId);
                var company = _companyBll.GetById(model.Details.CompanyId);
                var nppbkcId = plant == null ? item.NppbkcId : plant.NPPBKC_ID;

                item.NppbkcId = nppbkcId;
                item.PlantName = plant == null ? "" : plant.NAME1;
                item.CompanyName = company.BUTXT;
                item.CreatedBy = CurrentUser.USER_ID;
                item.CreatedDate = DateTime.Now;
                item.Status = Enums.DocumentStatus.Draft;

                if (item.Ck4cItem.Count == 0)
                {
                    AddMessageInfo("No item found", Enums.MessageInfoType.Warning);
                    model = InitialModel(model);
                    return View(model);
                }

                var existCk4c = _ck4CBll.GetByItem(item);
                if (existCk4c != null)
                {
                    AddMessageInfo("Data CK-4C already exists", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Details", new {id = existCk4c.Ck4CId});
                }

                var ck4cData = _ck4CBll.Save(item, CurrentUser.USER_ID);
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                Ck4cWorkflow(ck4cData.Ck4CId, Enums.ActionType.Created, string.Empty);
                return RedirectToAction("DocumentList");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        #endregion

        #region Get List Data

        private SelectList Ck4cPeriodList()
        {
            var period = new List<SelectItemModel>();
            var currentPeriod = 1;
            period.Add(new SelectItemModel() { ValueField = currentPeriod, TextField = currentPeriod.ToString() });
            period.Add(new SelectItemModel() { ValueField = currentPeriod + 1, TextField = (currentPeriod + 1).ToString() });
            return new SelectList(period, "ValueField", "TextField");
        }

        private SelectList Ck4cYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }

        private SelectList Ck4cDashboardYear()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }

        private List<Ck4cItemData> SetOtherCk4cItemData(List<Ck4cItemData> ck4cItemData)
        {
            List<Ck4cItemData> listData;

            listData = ck4cItemData.OrderBy(x => x.ProdDate).ToList();

            foreach (var item in listData)
            {
                var brand = _brandRegistrationBll.GetByFaCode(item.Werks, item.FaCode);
                var plant = _plantBll.GetT001WById(item.Werks);
                var prodType = _prodTypeBll.GetByCode(item.ProdCode);

                if (item.ContentPerPack == 0)
                    item.ContentPerPack = Convert.ToInt32(brand.BRAND_CONTENT);
                if (item.PackedInPack == 0)
                    item.PackedInPack = Convert.ToInt32(item.PackedQty) / Convert.ToInt32(brand.BRAND_CONTENT);
                if (item.ProdQty == 0)
                    item.ProdQty = item.PackedQty + item.UnpackedQty;
                item.ProdDateName = item.ProdDate.ToString("dd MMM yyyy");
                item.BrandDescription = brand.BRAND_CE;
                item.PlantName = item.Werks + "-" + plant.NAME1;
                item.ProdType = prodType.PRODUCT_TYPE;
            }

            return listData;
        }

        #endregion

        #region Details

        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var ck4cData = _ck4CBll.GetById(id.Value);

            if (ck4cData == null)
            {
                return HttpNotFound();
            }

            try
            {
                var plant = _plantBll.GetT001WById(ck4cData.PlantId);
                var nppbkcId = ck4cData.NppbkcId;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = ck4cData.Number;
                workflowInput.DocumentStatus = ck4cData.Status;
                workflowInput.NppbkcId = nppbkcId;
                if (plant != null)
                {
                    workflowInput.PlantId = ck4cData.PlantId;
                }

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                var changesHistory =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK4C,
                        id.Value.ToString()));

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(ck4cData.Number));

                var model = new Ck4CIndexDocumentListViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    Details = Mapper.Map<DataDocumentList>(ck4cData),
                    WorkflowHistory = workflowHistory,
                    ChangesHistoryList = changesHistory,
                    PrintHistoryList = printHistory
                };

                model.Details.Ck4cItemData = SetOtherCk4cItemData(model.Details.Ck4cItemData);

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Details.Status);

                model.AllowEditCompleted = _ck4CBll.AllowEditCompletedDocument(ck4cData, CurrentUser.USER_ID);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("DocumentList");
            }
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var ck4cData = _ck4CBll.GetById(id.Value);

            if (ck4cData == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }

            try
            {
                var plant = _plantBll.GetT001WById(ck4cData.PlantId);
                var nppbkcId = ck4cData.NppbkcId;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = ck4cData.Number;
                workflowInput.DocumentStatus = ck4cData.Status;
                workflowInput.NppbkcId = nppbkcId;
                if (plant != null)
                {
                    workflowInput.PlantId = ck4cData.PlantId;
                }

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                var changesHistory =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK4C,
                        id.Value.ToString()));

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(ck4cData.Number));

                var model = new Ck4CIndexDocumentListViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    Details = Mapper.Map<DataDocumentList>(ck4cData),
                    WorkflowHistory = workflowHistory,
                    ChangesHistoryList = changesHistory,
                    PrintHistoryList = printHistory
                };

                model.Details.Ck4cItemData = SetOtherCk4cItemData(model.Details.Ck4cItemData);

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput
                {
                    DocumentStatus = model.Details.Status,
                    FormView = Enums.FormViewType.Detail,
                    UserRole = CurrentUser.UserRole,
                    CreatedUser = ck4cData.CreatedBy,
                    CurrentUser = CurrentUser.USER_ID,
                    CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                    DocumentNumber = model.Details.Number,
                    NppbkcId = nppbkcId,
                    ManagerApprove = model.Details.ApprovedByManager,
                    PlantId = ck4cData.PlantId
                };

                ////workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;

                if (!allowApproveAndReject)
                {
                    model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
                }

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Details.Status);

                model.AllowEditCompleted = _ck4CBll.AllowEditCompletedDocument(ck4cData, CurrentUser.USER_ID);

                return View(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("DocumentList");
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

            var ck4cData = _ck4CBll.GetById(id.Value);

            if (ck4cData == null)
            {
                return HttpNotFound();
            }

            var model = new Ck4CIndexDocumentListViewModel();
            model = InitialModel(model);

            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Detail", new { id });
            }

            if (CurrentUser.UserRole == Enums.UserRole.Manager || (CurrentUser.UserRole == Enums.UserRole.POA && ck4cData.Status == Enums.DocumentStatus.WaitingForApproval))
            {
                //redirect to details for approval/rejected
                return RedirectToAction("Details", new { id });
            }

            try
            {
                model.Details = Mapper.Map<DataDocumentList>(ck4cData);

                model.Details.Ck4cItemData = SetOtherCk4cItemData(model.Details.Ck4cItemData);

                var plant = _plantBll.GetT001WById(ck4cData.PlantId);
                var nppbkcId = ck4cData.NppbkcId;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = ck4cData.Number;
                workflowInput.DocumentStatus = ck4cData.Status;
                workflowInput.NppbkcId = nppbkcId;
                if (plant != null)
                {
                    workflowInput.PlantId = ck4cData.PlantId;
                }

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK4C, id.Value.ToString()));

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(ck4cData.Number));

                model.WorkflowHistory = workflowHistory;
                model.ChangesHistoryList = changeHistory;
                model.PrintHistoryList = printHistory;

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput
                {
                    DocumentStatus = model.Details.Status,
                    FormView = Enums.FormViewType.Detail,
                    UserRole = CurrentUser.UserRole,
                    CreatedUser = ck4cData.CreatedBy,
                    CurrentUser = CurrentUser.USER_ID,
                    CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                    DocumentNumber = model.Details.Number,
                    NppbkcId = nppbkcId,
                    PlantId = ck4cData.PlantId
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
                return RedirectToAction("DocumentList");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ck4CIndexDocumentListViewModel model)
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

                var dataToSave = Mapper.Map<Ck4CDto>(model.Details);

                if (dataToSave.Ck4cItem.Count == 0)
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
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;
                dataToSave.MonthNameIndo = _monthBll.GetMonth(model.Details.ReportedMonth.Value).MONTH_NAME_IND;

                List<Ck4cItem> list = dataToSave.Ck4cItem;
                foreach (var item in list)
                {
                    item.Ck4CId = dataToSave.Ck4CId;
                }

                dataToSave.Ck4cItem = list;

                bool isSubmit = model.Details.IsSaveSubmit == "submit";

                var saveResult = _ck4CBll.Save(dataToSave, CurrentUser.USER_ID);

                if (isSubmit)
                {
                    Ck4cWorkflow(model.Details.Ck4CId, Enums.ActionType.Submit, string.Empty);
                    AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                    return RedirectToAction("Details", "CK4C", new { id = model.Details.Ck4CId });
                }

                //return RedirectToAction("Index");
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                return RedirectToAction("Edit", new { id = model.Details.Ck4CId });

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        #endregion

        #region Workflow

        public ActionResult ApproveDocument(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            bool isSuccess = false;
            try
            {
                Ck4cWorkflow(id.Value, Enums.ActionType.Approve, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Details", "CK4C", new { id });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction("DocumentList");
        }

        public ActionResult RejectDocument(Ck4CIndexDocumentListViewModel model)
        {
            bool isSuccess = false;
            try
            {
                Ck4cWorkflow(model.Details.Ck4CId, Enums.ActionType.Reject, model.Details.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "CK4C", new { id = model.Details.Ck4CId });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("DocumentList");
        }

        private void Ck4cWorkflow(int id, Enums.ActionType actionType, string comment)
        {
            var input = new Ck4cWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment
            };

            _ck4CBll.Ck4cWorkflow(input);
        }

        private bool ValidateEditDocument(Ck4CIndexDocumentListViewModel model, bool message = true)
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

        [HttpPost]
        public ActionResult GovApproveDocument(Ck4CIndexDocumentListViewModel model)
        {
            bool isSuccess = false;
            var currentUserId = CurrentUser;
            var message = string.Empty;

            try
            {
                if (model.Details.Status == Enums.DocumentStatus.WaitingGovApproval)
                {
                    model.Details.Ck4cDecreeDoc = new List<Ck4cDecreeDocModel>();
                    if (model.Details.Ck4cDecreeFiles != null)
                    {
                        foreach (var item in model.Details.Ck4cDecreeFiles)
                        {
                            if (item != null)
                            {
                                var filenamecheck = item.FileName;

                                if (filenamecheck.Contains("\\"))
                                {
                                    filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                                }

                                var decreeDoc = new Ck4cDecreeDocModel()
                                {
                                    FILE_NAME = filenamecheck,
                                    FILE_PATH = SaveUploadedFile(item, model.Details.Ck4CId),
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Ck4cDecreeDoc.Add(decreeDoc);
                            }
                        }
                    }

                    var input = new Ck4cUpdateReportedOn()
                    {
                        Id = model.Details.Ck4CId,
                        ReportedOn = model.Details.ReportedOn
                    };

                    _ck4CBll.UpdateReportedOn(input);

                    message = "Document " + EnumHelper.GetDescription(model.Details.StatusGoverment);
                }

                Ck4cWorkflowGovApprove(model.Details, model.Details.GovApprovalActionType, model.Details.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "CK4C", new { id = model.Details.Ck4CId });

            AddMessageInfo(message, Enums.MessageInfoType.Success);

            return RedirectToAction("CompletedDocument");
        }

        [HttpPost]
        public ActionResult GovCompletedDocument(Ck4CIndexDocumentListViewModel model)
        {
            bool isSuccess = false;
            var currentUserId = CurrentUser;
            var message = "Document is " + EnumHelper.GetDescription(Enums.DocumentStatus.WaitingGovApproval);
            var actionResult = "DocumentList";

            try
            {
                if (model.Details.Status == Enums.DocumentStatus.Completed)
                {
                    model.Details.Ck4cDecreeDoc = new List<Ck4cDecreeDocModel>();

                    if (model.Details.Ck4cDecreeFiles != null)
                    {
                        foreach (var item in model.Details.Ck4cDecreeFiles)
                        {
                            if (item != null)
                            {
                                var filenamecheck = item.FileName;

                                if (filenamecheck.Contains("\\"))
                                {
                                    filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                                }

                                var decreeDoc = new Ck4cDecreeDocModel()
                                {
                                    FILE_NAME = filenamecheck,
                                    FILE_PATH = SaveUploadedFile(item, model.Details.Ck4CId),
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Ck4cDecreeDoc.Add(decreeDoc);
                            }
                        }

                        message = "Document " + EnumHelper.GetDescription(model.Details.StatusGoverment);
                        if (model.Details.StatusGoverment == Enums.StatusGovCk4c.Approved)
                            message = "Document has been saved";
                        actionResult = "CompletedDocument";
                    }

                    if (model.Details.Ck4cUploadedDoc != null)
                    {
                        foreach (var item in model.Details.Ck4cUploadedDoc)
                        {
                            if (item != null)
                            {
                                var valueDoc = item.Split('|').ToArray();

                                var decreeDoc = new Ck4cDecreeDocModel()
                                {
                                    FILE_NAME = valueDoc[1],
                                    FILE_PATH = valueDoc[0],
                                    CREATED_BY = currentUserId.USER_ID,
                                    CREATED_DATE = DateTime.Now
                                };
                                model.Details.Ck4cDecreeDoc.Add(decreeDoc);
                            }
                        }

                        message = "Document " + EnumHelper.GetDescription(model.Details.StatusGoverment);
                        if (model.Details.StatusGoverment == Enums.StatusGovCk4c.Approved)
                            message = "Document has been saved";
                        actionResult = "CompletedDocument";
                    }
                }

                Ck4cWorkflowCompleted(model.Details, model.Details.GovApprovalActionType, model.Details.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "CK4C", new { id = model.Details.Ck4CId });

            AddMessageInfo(message, Enums.MessageInfoType.Success);

            return RedirectToAction(actionResult);
        }

        private string SaveUploadedFile(HttpPostedFileBase file, int ck4cId)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            //initialize folders in case deleted by an test publish profile
            if (!Directory.Exists(Server.MapPath(Constans.Ck4cDecreeDocFolderPath)))
                Directory.CreateDirectory(Server.MapPath(Constans.Ck4cDecreeDocFolderPath));

            sFileName = Constans.Ck4cDecreeDocFolderPath + Path.GetFileName(ck4cId.ToString("'ID'-##") + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        private void Ck4cWorkflowGovApprove(DataDocumentList ck4cData, Enums.ActionType actionType, string comment)
        {
            if (ck4cData.Status == Enums.DocumentStatus.WaitingGovApproval)
            {
                var input = new Ck4cWorkflowDocumentInput()
                {
                    DocumentId = ck4cData.Ck4CId,
                    ActionType = actionType,
                    UserRole = CurrentUser.UserRole,
                    UserId = CurrentUser.USER_ID,
                    DocumentNumber = ck4cData.Number,
                    Comment = comment,
                    AdditionalDocumentData = new Ck4cWorkflowDocumentData()
                    {
                        DecreeDate = ck4cData.DecreeDate.Value,
                        Ck4cDecreeDoc = Mapper.Map<List<Ck4cDecreeDocDto>>(ck4cData.Ck4cDecreeDoc)
                    }
                };

                _ck4CBll.Ck4cWorkflow(input);
            }
        }

        private void Ck4cWorkflowCompleted(DataDocumentList ck4cData, Enums.ActionType actionType, string comment)
        {
            if (ck4cData.Status == Enums.DocumentStatus.Completed)
            {
                var input = new Ck4cWorkflowDocumentInput();

                if (ck4cData.Ck4cDecreeDoc.Count == 0)
                {
                    input = new Ck4cWorkflowDocumentInput()
                    {
                        DocumentId = ck4cData.Ck4CId,
                        ActionType = actionType,
                        UserRole = CurrentUser.UserRole,
                        UserId = CurrentUser.USER_ID,
                        DocumentNumber = ck4cData.Number,
                        Comment = comment
                    };
                }
                else
                {
                    input = new Ck4cWorkflowDocumentInput()
                    {
                        DocumentId = ck4cData.Ck4CId,
                        ActionType = actionType,
                        UserRole = CurrentUser.UserRole,
                        UserId = CurrentUser.USER_ID,
                        DocumentNumber = ck4cData.Number,
                        Comment = comment,
                        AdditionalDocumentData = new Ck4cWorkflowDocumentData()
                        {
                            DecreeDate = ck4cData.DecreeDate.Value,
                            Ck4cDecreeDoc = Mapper.Map<List<Ck4cDecreeDocDto>>(ck4cData.Ck4cDecreeDoc)
                        }
                    };
                }

                _ck4CBll.Ck4cWorkflow(input);
            }
        }

        #endregion

        #region Print Preview

        [EncryptedParameter]
        public ActionResult PrintPreview(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            var ck4cData = _ck4CBll.GetCk4cReportDataById(id.Value);
            if (ck4cData == null)
                HttpNotFound();

            Stream stream = GetReport(ck4cData, "PREVIEW CK-4C");

            return File(stream, "application/pdf");
        }

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            //Get Report Source
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var ck4cData = _ck4CBll.GetCk4cReportDataById(id.Value);
            if (ck4cData == null)
                HttpNotFound();

            Stream stream = GetReport(ck4cData, "CK-4C");

            return File(stream, "application/pdf");
        }

        private Stream GetReport(Ck4cReportDto ck4cReport, string printTitle)
        {
            var dataSet = SetDataSetReport(ck4cReport, printTitle);

            ReportClass rpt = new ReportClass
            {
                FileName = ConfigurationManager.AppSettings["Report_Path"] + "CK4C\\Preview.rpt"

            };
            rpt.Load();
            rpt.SetDataSource(dataSet);
            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rpt.Close();
            return stream;
        }

        private DataSet SetDataSetReport(Ck4cReportDto ck4cReportDto, string printTitle)
        {
            var dsCk4c = new dsCk4c();

            var listCk4c = new List<Ck4cReportInformationDto>();
            listCk4c.Add(ck4cReportDto.Detail);

            dsCk4c = AddDataCk4cRow(dsCk4c, ck4cReportDto.Detail, printTitle);
            dsCk4c = AddDataCk4cItemsRow(dsCk4c, ck4cReportDto.Ck4cItemList);
            dsCk4c = AddDataTotalCk4cRow(dsCk4c, ck4cReportDto.Ck4cTotal);
            dsCk4c = AddDataHeaderFooter(dsCk4c, ck4cReportDto.HeaderFooter);

            return dsCk4c;
        }

        private dsCk4c AddDataCk4cRow(dsCk4c dsCk4c, Ck4cReportInformationDto ck4cReportDetails, string printTitle)
        {
            var detailRow = dsCk4c.Ck4c.NewCk4cRow();

            detailRow.Ck4cId = ck4cReportDetails.Ck4cId.ToString();
            detailRow.Number = ck4cReportDetails.Number;
            detailRow.ReportedOn = ck4cReportDetails.ReportedOn;
            detailRow.ReportedPeriodStart = ck4cReportDetails.ReportedPeriodStart;
            detailRow.ReportedPeriodEnd = ck4cReportDetails.ReportedPeriodEnd;
            detailRow.ReportedMonth = ck4cReportDetails.ReportedMonth;
            detailRow.ReportedYear = ck4cReportDetails.ReportedYear;
            detailRow.CompanyName = ck4cReportDetails.CompanyName;
            detailRow.CompanyAddress = ck4cReportDetails.CompanyAddress;
            detailRow.Nppbkc = ck4cReportDetails.Nppbkc;
            detailRow.Poa = ck4cReportDetails.Poa;
            detailRow.ReportedOnDay = ck4cReportDetails.ReportedOnDay;
            detailRow.ReportedOnMonth = ck4cReportDetails.ReportedOnMonth;
            detailRow.ReportedOnYear = ck4cReportDetails.ReportedOnYear;
            detailRow.NBatang = ck4cReportDetails.NBatang;
            detailRow.NGram = ck4cReportDetails.NGram;
            detailRow.ProdTotal = ck4cReportDetails.ProdTotal;
            detailRow.City = ck4cReportDetails.City;
            detailRow.Preview = printTitle;

            dsCk4c.Ck4c.AddCk4cRow(detailRow);

            return dsCk4c;
        }

        private dsCk4c AddDataCk4cItemsRow(dsCk4c dsCk4c, List<Ck4cReportItemDto> listCk4cItemsDto)
        {
            foreach (var itemDto in listCk4cItemsDto)
            {
                var detailRow = dsCk4c.Ck4cItem.NewCk4cItemRow();

                detailRow.Ck4cItemId = itemDto.Ck4cItemId.ToString();
                detailRow.ProdQty = itemDto.ProdQty;
                detailRow.ProdType = itemDto.ProdType;
                detailRow.Merk = itemDto.Merk;
                detailRow.Hje = itemDto.Hje;
                detailRow.No = itemDto.No;
                detailRow.NoProd = itemDto.NoProd;
                detailRow.ProdDate = itemDto.ProdDate;
                detailRow.SumBtg = itemDto.SumBtg;
                detailRow.BtgGr = itemDto.BtgGr;
                detailRow.Isi = itemDto.Isi;
                detailRow.Total = itemDto.Total;
                detailRow.ProdWaste = itemDto.ProdWaste;
                detailRow.Comment = itemDto.Comment;
                detailRow.CollumNo = itemDto.CollumNo;

                dsCk4c.Ck4cItem.AddCk4cItemRow(detailRow);
            }
            return dsCk4c;
        }

        private dsCk4c AddDataTotalCk4cRow(dsCk4c dsCk4c, Ck4cTotalProd ck4cReportTotal)
        {
            var detailRow = dsCk4c.Ck4cTotalProd.NewCk4cTotalProdRow();

            detailRow.ProdType = ck4cReportTotal.ProdType;
            detailRow.ProdTotal = ck4cReportTotal.ProdTotal;
            detailRow.ProdBtg = ck4cReportTotal.ProdBtg;

            dsCk4c.Ck4cTotalProd.AddCk4cTotalProdRow(detailRow);

            return dsCk4c;
        }

        private dsCk4c AddDataHeaderFooter(dsCk4c ds, HEADER_FOOTER_MAPDto headerFooter)
        {
            var dRow = ds.HeaderFooter.NewHeaderFooterRow();
            if (headerFooter != null)
            {
                #region set Image Header

                if (headerFooter.IS_HEADER_SET.HasValue && headerFooter.IS_HEADER_SET.Value)
                {
                    //convert to byte image
                    FileStream fs;
                    BinaryReader br;
                    var imagePath = headerFooter.HEADER_IMAGE_PATH;
                    if (System.IO.File.Exists(Server.MapPath(imagePath)))
                    {
                        fs = new FileStream(Server.MapPath(imagePath), FileMode.Open, FileAccess.Read,
                            FileShare.ReadWrite);
                    }
                    else
                    {
                        // if photo does not exist show the nophoto.jpg file 
                        fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }
                    // initialise the binary reader from file streamobject 
                    br = new BinaryReader(fs);
                    // define the byte array of filelength 
                    byte[] imgbyte = new byte[fs.Length + 1];
                    // read the bytes from the binary reader 
                    imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));

                    dRow.HeaderImage = imgbyte;

                }
                else
                {
                    dRow.HeaderImage = null;
                }

                #endregion

                #region set Footer Content

                dRow.FooterContent = headerFooter.IS_FOOTER_SET.HasValue && headerFooter.IS_FOOTER_SET.Value
                    ? headerFooter.FOOTER_CONTENT.Replace("<br />", Environment.NewLine)
                    : " ";

                #endregion
            }
            else
            {
                dRow.HeaderImage = null;
                dRow.FooterContent = " ";
            }
            ds.HeaderFooter.AddHeaderFooterRow(dRow);
            return ds;
        }

        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var ck4c = _ck4CBll.GetById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.CK4C,
                FORM_ID = ck4c.Ck4CId,
                FORM_NUMBER = ck4c.Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(ck4c.Number));
            return PartialView("_PrintHistoryTable", model);

        }

        #endregion

        #region Summary Reports

        private SelectList GetCk4CNumberList(List<Ck4CSummaryReportDto> listCk4C)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listCk4C
                    select new SelectItemModel()
                    {
                        ValueField = x.Ck4CNo,
                        TextField = x.Ck4CNo
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetPlantList(List<Ck4CSummaryReportDto> listCk4C)
        {

            IEnumerable<SelectItemModel> query;

            query = from x in listCk4C
                    select new SelectItemModel()
                    {
                        ValueField = x.PlantId,
                        TextField = x.PlantId + " - " + x.PlantDescription
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }



        private Ck4CSummaryReportsViewModel InitSummaryReports(Ck4CSummaryReportsViewModel model)
        {
            model.MainMenu = Enums.MenuList.CK4C;
            model.CurrentMenu = PageInfo;

            var listCk4C = _ck4CBll.GetSummaryReportsByParam(new Ck4CGetSummaryReportByParamInput());

            model.SearchView.Ck4CNoList = GetCk4CNumberList(listCk4C);
            model.SearchView.PlantIdList = GetPlantList(listCk4C);


            var filter = new Ck4CSearchSummaryReportsViewModel();

            model.DetailsList = SearchDataSummaryReports(filter);

            return model;
        }

        private List<Ck4CSummaryReportsItem> SearchDataSummaryReports(Ck4CSearchSummaryReportsViewModel filter = null)
        {
            Ck4CGetSummaryReportByParamInput input;
            List<Ck4CSummaryReportDto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Ck4CGetSummaryReportByParamInput();

                dbData = _ck4CBll.GetSummaryReportsByParam(input);
                return Mapper.Map<List<Ck4CSummaryReportsItem>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<Ck4CGetSummaryReportByParamInput>(filter);

            dbData = _ck4CBll.GetSummaryReportsByParam(input);
            return Mapper.Map<List<Ck4CSummaryReportsItem>>(dbData);
        }

        public ActionResult SummaryReports()
        {

            Ck4CSummaryReportsViewModel model;
            try
            {

                model = new Ck4CSummaryReportsViewModel();


                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Ck4CSummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK4C;
                model.CurrentMenu = PageInfo;
            }

            return View("Ck4CSummaryReport", model);
        }

        [HttpPost]
        public PartialViewResult SearchSummaryReports(Ck4CSummaryReportsViewModel model)
        {
            model.DetailsList = SearchDataSummaryReports(model.SearchView);
            return PartialView("_Ck4CListSummaryReport", model);


        }

        public void ExportXlsSummaryReports(Ck4CSummaryReportsViewModel model)
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

        private string CreateXlsSummaryReports(Ck4CExportSummaryReportsViewModel modelExport)
        {
            var filterModel = new Ck4CSearchSummaryReportsViewModel();
            filterModel.Ck4CNo = modelExport.Ck4CNumber;
            filterModel.PlantId = modelExport.Plant;
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


                if (modelExport.Ck4CNo)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck4CNo);
                    iColumn = iColumn + 1;
                }
                if (modelExport.CeOffice)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CeOffice);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BasedOn)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BasedOn);
                    iColumn = iColumn + 1;
                }
                if (modelExport.LicenseNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LicenseNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.PlantId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PlantId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.PlantDescription)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PlantDescription);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Period)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Period);
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
                if (modelExport.ReportPeriod)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReportPeriod);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ProductionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.ProductionDate.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.FaCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.FaCode.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.TobaccoProductType)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.TobaccoProductType.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.BrandDescription)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.BrandDescription.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Hje)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.Hje.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Tariff)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.Tariff.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Content)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.Content.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.PackedQty)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.PackedQty.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.PackedQtyInPack)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.PackedQtyInPack.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnPackQty)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.UnPackQty.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.ProducedQty)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.ProducedQty.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.UomProducedQty)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.UomProducedQty.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Remarks)
                {
                    slDocument.SetCellValue(iRow, iColumn, string.Join(Environment.NewLine, data.Remarks.ToArray()));
                    iColumn = iColumn + 1;
                }

                if (modelExport.PoaApproved)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PoaApproved);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ManagerApproved)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ManagerApproved);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Status)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Status);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFile(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, Ck4CExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;


            if (modelExport.Ck4CNo)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-4C No");
                iColumn = iColumn + 1;
            }

            if (modelExport.CeOffice)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.BasedOn)
            {
                slDocument.SetCellValue(iRow, iColumn, "Based On");
                iColumn = iColumn + 1;
            }

            if (modelExport.LicenseNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nppbkc");
                iColumn = iColumn + 1;
            }

            if (modelExport.PlantId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.PlantDescription)
            {
                slDocument.SetCellValue(iRow, iColumn, "Plant Description");
                iColumn = iColumn + 1;
            }

            if (modelExport.Period)
            {
                slDocument.SetCellValue(iRow, iColumn, "Period");
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

            if (modelExport.ReportPeriod)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported On");
                iColumn = iColumn + 1;
            }

            if (modelExport.ProductionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Production Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.FaCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "FA Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.TobaccoProductType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tobacco Product Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.BrandDescription)
            {
                slDocument.SetCellValue(iRow, iColumn, "Brand Description");
                iColumn = iColumn + 1;
            }

            if (modelExport.Hje)
            {
                slDocument.SetCellValue(iRow, iColumn, "HJE");
                iColumn = iColumn + 1;
            }

            if (modelExport.Tariff)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tariff");
                iColumn = iColumn + 1;
            }

            if (modelExport.Content)
            {
                slDocument.SetCellValue(iRow, iColumn, "Content Per Pack");
                iColumn = iColumn + 1;
            }

            if (modelExport.PackedQty)
            {
                slDocument.SetCellValue(iRow, iColumn, "Packed QTY");
                iColumn = iColumn + 1;
            }

            if (modelExport.PackedQtyInPack)
            {
                slDocument.SetCellValue(iRow, iColumn, "Packed QTY In Pack");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnPackQty)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpacked QTY");
                iColumn = iColumn + 1;
            }

            if (modelExport.ProducedQty)
            {
                slDocument.SetCellValue(iRow, iColumn, "Produced QTY");
                iColumn = iColumn + 1;
            }

            if (modelExport.UomProducedQty)
            {
                slDocument.SetCellValue(iRow, iColumn, "UoM Produced QTY");
                iColumn = iColumn + 1;
            }

            if (modelExport.Remarks)
            {
                slDocument.SetCellValue(iRow, iColumn, "Remarks");
                iColumn = iColumn + 1;
            }

            if (modelExport.PoaApproved)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA Approved");
                iColumn = iColumn + 1;
            }

            if (modelExport.ManagerApproved)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manager Approved");
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

            var fileName = "CK4C_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;
        }

        #endregion

        #region CK4C Item Exports

        public void ExportXlsCk4cItem(int id)
        {
            string pathFile = "";

            pathFile = CreateXlsCk4cItem(id);

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

        private string CreateXlsCk4cItem(int ck4cId)
        {
            var dataExportItem = SearchDataItem(ck4cId);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelForCk4cItem(slDocument);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataExportItem)
            {
                iColumn = 1;

                slDocument.SetCellValue(iRow, iColumn, data.ProductionDate);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Plant);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.TobbacoProdType);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.FaCode);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.BrandDesc);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ProdQty);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ProdQtyUom);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.PackedQty);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.UnpackedQty);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Remarks);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Content);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.TotalPack);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Hje);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Tariff);
                iColumn = iColumn + 1;

                iRow++;
            }

            return CreateXlsFile(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelForCk4cItem(SLDocument slDocument)
        {
            int iColumn = 1;
            int iRow = 1;

            slDocument.SetCellValue(iRow, iColumn, "Production Date (3)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Tobbaco Product Type (4)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "FA Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Brand Description (7)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Produced QTY (5)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Produced QTY UoM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Packed QTY (6)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Unpacked QTY (11)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Remarks (12)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Content (8)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Total Pack (10)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "HJE (9)");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Tariff");
            iColumn = iColumn + 1;

            return slDocument;

        }

        private List<Ck4cExportItem> SearchDataItem(int ck4cId)
        {
            List<Ck4cItemExportDto> dbData;

            dbData = _ck4CBll.GetCk4cItemById(ck4cId);

            return Mapper.Map<List<Ck4cExportItem>>(dbData);
        }

        #endregion

        #region Dashboard
        public ActionResult Dashboard()
        {
            var data = InitDashboardModel(new Ck4cDashboardModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                MonthList = GlobalFunctions.GetMonthList(_monthBll),
                YearList = Ck4cDashboardYear(),
                PoaList = GlobalFunctions.GetPoaAll(_poabll),
                UserList = GlobalFunctions.GetCreatorList()
            });

            return View("Dashboard", data);
        }

        private Ck4cDashboardModel InitDashboardModel(
            Ck4cDashboardModel model)
        {
            var listCk4c = GetAllDocument(model);

            model.Detil.DraftTotal = listCk4c.Where(x => x.Status == Enums.DocumentStatus.Draft).Count();
            model.Detil.WaitingForAppTotal = listCk4c.Where(x => x.Status == Enums.DocumentStatus.WaitingForApproval || x.Status == Enums.DocumentStatus.WaitingForApprovalManager).Count();
            model.Detil.WaitingForPoaTotal = listCk4c.Where(x => x.Status == Enums.DocumentStatus.WaitingForApproval).Count();
            model.Detil.WaitingForManagerTotal = listCk4c.Where(x => x.Status == Enums.DocumentStatus.WaitingForApprovalManager).Count();
            model.Detil.WaitingForGovTotal = listCk4c.Where(x => x.Status == Enums.DocumentStatus.WaitingGovApproval).Count();
            model.Detil.CompletedTotal = listCk4c.Where(x => x.Status == Enums.DocumentStatus.Completed).Count();

            return model;
        }

        private List<Ck4CDto> GetAllDocument(Ck4cDashboardModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var ck4cData = _ck4CBll.GetAllByParam(new Ck4CDashboardParamInput());
                return ck4cData;
            }

            //getbyparams
            var input = Mapper.Map<Ck4CDashboardParamInput>(filter);
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;

            var dbData = _ck4CBll.GetAllByParam(input);
            return dbData;
        }

        [HttpPost]
        public PartialViewResult FilterDashboardPage(Ck4cDashboardModel model)
        {
            var data = InitDashboardModel(model);

            return PartialView("_ChartStatus", data.Detil);
        }

        #endregion
    }
}