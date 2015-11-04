using System.Configuration;
using System.Data;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.LACK2;
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
    public class LACK2Controller : BaseController
    {

        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;

        private ILACK2BLL _lack2Bll;
        private ICompanyBLL _companyBll;
        private IPrintHistoryBLL _printHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IPOABLL _poabll;
        private IMonthBLL _monthBll;
        private IUserPlantMapBLL _userPlantMapBll;
        private IPBCK1BLL _pbck1Bll;
        private ICK5BLL _ck5Bll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IWorkflowBLL _workflowBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IHeaderFooterBLL _headerFooterBll;

        public LACK2Controller(IPageBLL pageBll, ILACK2BLL lack2Bll, ICompanyBLL companyBll, IChangesHistoryBLL changesHistoryBll,
            IPrintHistoryBLL printHistoryBll, IPOABLL poabll, IMonthBLL monthBll, IUserPlantMapBLL userPlantMapBll, IPBCK1BLL pbck1Bll, ICK5BLL ck5Bll,
            IZaidmExNPPBKCBLL nppbkcBll, IWorkflowBLL workflowBll, IWorkflowHistoryBLL workflowHistoryBll, IHeaderFooterBLL headerFooterBll)
            : base(pageBll, Enums.MenuList.LACK2)
        {
            _mainMenu = Enums.MenuList.LACK2;

            _lack2Bll = lack2Bll;
            _companyBll = companyBll;
            _printHistoryBll = printHistoryBll;
            _changesHistoryBll = changesHistoryBll;
            _poabll = poabll;
            _monthBll = monthBll;
            _userPlantMapBll = userPlantMapBll;
            _pbck1Bll = pbck1Bll;
            _ck5Bll = ck5Bll;
            _nppbkcbll = nppbkcBll;
            _workflowBll = workflowBll;
            _workflowHistoryBll = workflowHistoryBll;
            _headerFooterBll = headerFooterBll;
        }

        #endregion

        #region ---------------- Index --------------

        // GET: LACK2
        public ActionResult Index()
        {
            var currUser = CurrentUser;

            var input = new Lack2GetByParamInput()
            {
                UserId = currUser.USER_ID,
                UserRole = currUser.UserRole,
                IsOpenDocList = true
            };

            var dbData = _lack2Bll.GetByParam(input);

            var model = new Lack2IndexViewModel();
            model = InitIndexViewModel(model);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.MenuLack2OpenDocument = "active";
            model.MenuLack2CompletedDocument = "";
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.Details = dbData;
            model.FilterActionController = "FilterOpenDocument";

            return View("Index", model);
        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(LACK2FilterViewModel searchInput)
        {
            var curUser = CurrentUser;
            var input = Mapper.Map<Lack2GetByParamInput>(searchInput);
            input.UserId = curUser.USER_ID;
            input.UserRole = curUser.UserRole;
            input.IsOpenDocList = true;

            var dbData = _lack2Bll.GetByParam(input);
            var model = new Lack2IndexViewModel
            {
                Details = dbData,
                MenuLack2OpenDocument = "active"
            };
            return PartialView("_Lack2OpenDoc", model);
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(LACK2FilterViewModel searchInput)
        {
            var curUser = CurrentUser;
            var input = Mapper.Map<Lack2GetByParamInput>(searchInput);
            input.UserId = curUser.USER_ID;
            input.UserRole = curUser.UserRole;

            var dbData = _lack2Bll.GetCompletedByParam(input);
            var model = new Lack2IndexViewModel { Details = dbData };
            return PartialView("_Lack2OpenDoc", model);
        }

        public ActionResult ListCompletedDoc()
        {
            var currUser = CurrentUser;

            var input = new Lack2GetByParamInput()
            {
                UserId = currUser.USER_ID,
                UserRole = currUser.UserRole
            };
            var model = new Lack2IndexViewModel();
            model = InitIndexViewModel(model);

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.FilterActionController = "FilterCompletedDocument";

            var dbData = _lack2Bll.GetCompletedByParam(input);

            model.Details = dbData;
            model.MenuLack2OpenDocument = "";
            model.MenuLack2CompletedDocument = "active";
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            return View("Index", model);
        }

        #endregion

        #region --------------- Create ----------
        
        /// <summary>
        /// Create LACK2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new LACK2CreateViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager,
                IsCreateNew = true
            };

            return View("Create", CreateInitialViewModel(model));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LACK2CreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Invalid input, please check the input.", Enums.MessageInfoType.Error);
                    return View(CreateInitialViewModel(model));
                }
                if (CurrentUser.UserRole == Enums.UserRole.Manager)
                {
                    AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                var input = Mapper.Map<Lack2CreateParamInput>(model);
                input.UserId = CurrentUser.USER_ID;
                var saveOutput = _lack2Bll.Create(input);
                if (saveOutput.Success)
                {
                    AddMessageInfo("Save successfull", Enums.MessageInfoType.Info);
                    return RedirectToAction("Index");
                }
                model.PoaList = model.PoaListHidden;
                AddMessageInfo("Save failed : " + saveOutput.ErrorMessage, Enums.MessageInfoType.Info);
                return View(CreateInitialViewModel(model));
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }
            return View(CreateInitialViewModel(model));
        }

        private LACK2CreateViewModel CreateInitialViewModel(LACK2CreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList(_companyBll);
            model.NPPBKCDDL = new SelectList(GetNppbkcDataByCompanyId(model.CompanyCode), "NPPBKC_ID", "NPPBKC_ID");
            model.ExcisableGoodsTypeDDL = new SelectList(GetExciseGoodsTypeData(model.NppbkcId), "EXC_GOOD_TYP", "EXT_TYP_DESC");
            model.SendingPlantDDL = new SelectList(GetSendingPlantDataByNppbkcId(model.CompanyCode, model.NppbkcId), "WERKS", "DROPDOWNTEXTFIELD");
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = GetCk5YearList();
            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;
            return model;
        }

        #endregion

        #region --------------- Edit --------------

        /// <summary>
        /// Edits the LACK2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack2Data = _lack2Bll.GetDetailsById(id.Value);

            if (lack2Data == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //redirect to details for approval/rejected
                return RedirectToAction("Detail", new { id });
            }

            if (CurrentUser.USER_ID == lack2Data.CreatedBy &&
                (lack2Data.Status == Enums.DocumentStatus.WaitingForApproval ||
                 lack2Data.Status == Enums.DocumentStatus.WaitingForApprovalManager))
            {
                return RedirectToAction("Detail", new { id });
            }

            if (!IsAllowEditLack1(lack2Data.CreatedBy, lack2Data.Status))
            {
                AddMessageInfo(
                    "Operation not allowed.",
                    Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = InitEditModel(lack2Data);
            model = InitEditList(model);
            model.IsCreateNew = false;

            model.ControllerAction = model.Status == Enums.DocumentStatus.WaitingGovApproval || model.Status == Enums.DocumentStatus.Completed ? "GovApproveDocument" : "Edit";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lack2EditViewModel model)
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
                    model = OnFailedEdit(model);
                    AddMessageInfo("Invalid input", Enums.MessageInfoType.Error);
                    return View(model);
                }

                if (model.CreatedBy != CurrentUser.USER_ID)
                {
                    return RedirectToAction("Detail", new { id = model.Lack2Id });
                }

                bool isSubmit = model.IsSaveSubmit == "submit";

                var input = Mapper.Map<Lack2SaveEditInput>(model);
                input.UserId = CurrentUser.USER_ID;
                input.WorkflowActionType = Enums.ActionType.Modified;

                var saveResult = _lack2Bll.SaveEdit(input);

                if (saveResult.Success)
                {
                    if (isSubmit)
                    {
                        Lack2Workflow(model.Lack2Id, Enums.ActionType.Submit, string.Empty, saveResult.IsModifiedHistory);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("Detail", "Lack2", new {id = model.Lack2Id});
                    }
                    AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                    return RedirectToAction("Edit", new {id = model.Lack2Id});
                }
                model = OnFailedEdit(model);
                AddMessageInfo(saveResult.ErrorMessage, Enums.MessageInfoType.Error);
                return View(model);
            }
            catch (Exception exception)
            {
                model = OnFailedEdit(model);
                AddMessageInfo("Save edit failed. " + exception.Message, Enums.MessageInfoType.Error);
                return View(model);
            }
        }

        private Lack2EditViewModel OnFailedEdit(Lack2EditViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model = InitEditList(model);
            model = SetEditHistory(model);
            model.PoaList = model.PoaListHidden;
            return model;
        }

        private Lack2EditViewModel InitEditModel(Lack2DetailsDto lack2Data)
        {
            var model = Mapper.Map<Lack2EditViewModel>(lack2Data);

            model = SetEditHistory(model);

            var curUser = CurrentUser;

            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = lack2Data.CreatedBy,
                CurrentUser = curUser.USER_ID,
                CurrentUserGroup = curUser.USER_GROUP_ID,
                DocumentNumber = model.Lack2Number,
                NppbkcId = model.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }

            model.AllowPrintDocument = _workflowBll.AllowPrint(model.Status);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.PoaList = GetPoaListByNppbkcId(model.NppbkcId);
            model.PoaListHidden = model.PoaList;

            return model;
        }

        private Lack2EditViewModel InitEditList(Lack2EditViewModel model)
        {
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList(_companyBll);
            model.NPPBKCDDL = new SelectList(GetNppbkcDataByCompanyId(model.CompanyCode), "NPPBKC_ID", "NPPBKC_ID");
            model.ExcisableGoodsTypeDDL = new SelectList(GetExciseGoodsTypeData(model.NppbkcId), "EXC_GOOD_TYP", "EXT_TYP_DESC");
            model.SendingPlantDDL = new SelectList(GetSendingPlantDataByNppbkcId(model.CompanyCode, model.NppbkcId), "WERKS", "DROPDOWNTEXTFIELD");
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = GetCk5YearList();
            return model;
        }

        private Lack2EditViewModel SetEditHistory(Lack2EditViewModel model)
        {
            //workflow history
            var workflowInput = new GetByFormNumberInput
            {
                FormNumber = model.Lack2Number,
                DocumentStatus = model.Status,
                NPPBKC_Id = model.NppbkcId
            };

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK2,
                    model.Lack2Id.ToString()));

            var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(model.Lack2Number));

            model.ChangesHistoryList = changesHistory;
            model.WorkflowHistory = workflowHistory;
            model.PrintHistoryList = printHistory;

            return model;
        }

        private bool IsAllowEditLack1(string userId, Enums.DocumentStatus status)
        {
            bool isAllow = CurrentUser.USER_ID == userId;
            if (!(status == Enums.DocumentStatus.Draft || status == Enums.DocumentStatus.Rejected 
                || status == Enums.DocumentStatus.WaitingGovApproval || status == Enums.DocumentStatus.Completed))
            {
                isAllow = false;
            }

            return isAllow;
        }

        private string GetPoaListByNppbkcId(string nppbkcId)
        {
            var data = _poabll.GetPoaByNppbkcId(nppbkcId);
            return data == null ? string.Empty : string.Join(", ", data.Distinct().Select(d => d.PRINTED_NAME).ToList());
        }

        #endregion

        #region --------------- Detail --------

        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack1Data = _lack2Bll.GetDetailsById(id.Value);

            if (lack1Data == null)
            {
                return HttpNotFound();
            }

            var model = InitDetailModel(lack1Data);
            
            return View(model);
        }

        private Lack2DetailViewModel InitDetailModel(Lack2DetailsDto lack2Data)
        {
            var model = Mapper.Map<Lack2DetailViewModel>(lack2Data);

            model = SetDetailHistory(model);

            var curUser = CurrentUser;

            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = lack2Data.CreatedBy,
                CurrentUser = curUser.USER_ID,
                CurrentUserGroup = curUser.USER_GROUP_ID,
                DocumentNumber = model.Lack2Number,
                NppbkcId = model.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                input.ManagerApprove = lack2Data.ApprovedByManager;
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }

            model.AllowPrintDocument = _workflowBll.AllowPrint(model.Status);

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.PoaList = GetPoaListByNppbkcId(model.NppbkcId);
            model.PoaListHidden = model.PoaList;
            model = SetDetailActiveMenu(model);

            return model;
        }

        private Lack2DetailViewModel SetDetailActiveMenu(Lack2DetailViewModel model)
        {
            if (model.Status == Enums.DocumentStatus.Completed)
            {
                model.MenuLack2CompletedDocument = "active";
                model.MenuLack2OpenDocument = "";
            }
            else
            {
                model.MenuLack2CompletedDocument = "";
                model.MenuLack2OpenDocument = "active";
            }
            return model;
        }

        private Lack2DetailViewModel SetDetailHistory(Lack2DetailViewModel model)
        {
            //workflow history
            var workflowInput = new GetByFormNumberInput
            {
                FormNumber = model.Lack2Number,
                DocumentStatus = model.Status,
                NPPBKC_Id = model.NppbkcId
            };

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK2,
                    model.Lack2Id.ToString()));

            var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(model.Lack2Number));

            model.ChangesHistoryList = changesHistory;
            model.WorkflowHistory = workflowHistory;
            model.PrintHistoryList = printHistory;

            return model;
        }

        #endregion

        #region ------- Other ----------------

        public JsonResult Generate(Lack2GenerateInputModel param)
        {
            var input = Mapper.Map<Lack2GenerateDataParamInput>(param);
            var outGeneratedData = _lack2Bll.GenerateLack2DataByParam(input);
            return Json(outGeneratedData);
        }
        
        [HttpPost]
        public JsonResult GetPlantByNppbkcId(string companyId, string nppbkcId)
        {
            var data = GetSendingPlantDataByNppbkcId(companyId, nppbkcId);
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetPoaByNppbkcId(string nppbkcId)
        {
            var data = _poabll.GetPoaByNppbkcId(nppbkcId);
            return Json(data.Distinct());
        }

        [HttpPost]
        public JsonResult GetGoodsTypeByNppbkc(string nppbkcId)
        {
            var data = GetExciseGoodsTypeData(nppbkcId);
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetNppbkcByCompanyId(string companyId)
        {
            return Json(GetNppbkcDataByCompanyId(companyId));
        }
        
        public void ExportChangesLogToExcel(int id)
        {

            string pathFile = "";

            pathFile = ExportChangeLogs(id);

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

        private string ExportChangeLogs(int id)
        {
            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK2, id.ToString());

            var model = Mapper.Map<List<ChangesHistoryItemModel>>(listHistory);

            var slDocument = new SLDocument();
            int endColumnIndex;
            //create header
            slDocument = HeaderExportChangeLogs(slDocument, out endColumnIndex);

            int iRow = 2; //starting row data

            foreach (var item in model)
            {
                int iColumn = 1;
                slDocument.SetCellValue(iRow, iColumn, item.MODIFIED_DATE.HasValue ? item.MODIFIED_DATE.Value.ToString("dd MMM yyyy HH:mm:ss") : string.Empty);
                iColumn++;

                slDocument.SetCellValue(iRow, iColumn, item.FIELD_NAME);
                iColumn++;

                slDocument.SetCellValue(iRow, iColumn, item.OLD_VALUE);
                iColumn++;

                slDocument.SetCellValue(iRow, iColumn, item.NEW_VALUE);
                iColumn++;

                slDocument.SetCellValue(iRow, iColumn, item.USERNAME);
                
                iRow++;
            }

            return CreateXlsExportChangeLogs(slDocument, endColumnIndex, iRow - 1);
        }

        private SLDocument HeaderExportChangeLogs(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;

            slDocument.SetCellValue(1, iColumn, "Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Field");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Old Value");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "New Value");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "User");
            
            endColumnIndex = iColumn;

            return slDocument;
        }

        private string CreateXlsExportChangeLogs(SLDocument slDocument, int endColumnIndex, int endRowIndex)
        {
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Alignment.Vertical = VerticalAlignmentValues.Center;

            //set header style
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //set border to value cell
            slDocument.SetCellStyle(2, 1, endRowIndex, endColumnIndex, valueStyle);

            //set header style
            slDocument.SetCellStyle(1, 1, 1, endColumnIndex, headerStyle);

            //set auto fit to all column
            slDocument.AutoFitColumn(1, endColumnIndex);

            var fileName = "lack2_changeslog_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.Lack2FolderPaht), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }
        
        #endregion

        #region Summary Reports

        private SelectList GetLack2CompanyCodeList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.CompanyCode,
                        TextField = x.CompanyCode
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2NppbkcIdList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.NppbkcId,
                        TextField = x.NppbkcId
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2SendingPlantList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.Ck5SendingPlant,
                        TextField = x.Ck5SendingPlant
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2GoodTypeList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.TypeExcisableGoods,
                        TextField = x.TypeExcisableGoods + " - " + x.TypeExcisableGoodsDesc
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2PeriodYearList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.PeriodYear,
                        TextField = x.PeriodYear
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private List<Lack2SummaryReportsItem> SearchDataSummaryReports(Lack2SearchSummaryReportsViewModel filter = null)
        {
            Lack2GetSummaryReportByParamInput input;
            List<Lack2SummaryReportDto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Lack2GetSummaryReportByParamInput();

                dbData = _lack2Bll.GetSummaryReportsByParam(input);
                return Mapper.Map<List<Lack2SummaryReportsItem>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<Lack2GetSummaryReportByParamInput>(filter);

            dbData = _lack2Bll.GetSummaryReportsByParam(input);
            return Mapper.Map<List<Lack2SummaryReportsItem>>(dbData);
        }

        private Lack2SummaryReportsViewModel InitSummaryReports(Lack2SummaryReportsViewModel model)
        {
            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            var listLack2 = _lack2Bll.GetSummaryReportsByParam(new Lack2GetSummaryReportByParamInput());

            model.SearchView.CompanyCodeList = GetLack2CompanyCodeList(listLack2);
            model.SearchView.NppbkcIdList = GetLack2NppbkcIdList(listLack2);
            model.SearchView.SendingPlantIdList = GetLack2SendingPlantList(listLack2);
            model.SearchView.GoodTypeList = GetLack2GoodTypeList(listLack2);

            model.SearchView.PeriodMonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.SearchView.PeriodYearList = GetLack2PeriodYearList(listLack2);

            model.SearchView.CreatedByList = GlobalFunctions.GetCreatorList();
            model.SearchView.ApprovedByList = GlobalFunctions.GetPoaAll(_poabll);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchView.ApproverList = GlobalFunctions.GetPoaAll(_poabll);

            var filter = new Lack2SearchSummaryReportsViewModel();

            model.DetailsList = SearchDataSummaryReports(filter);

            return model;
        }

        public ActionResult SummaryReports()
        {

            Lack2SummaryReportsViewModel model;
            try
            {

                model = new Lack2SummaryReportsViewModel();


                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Lack2SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.LACK2;
                model.CurrentMenu = PageInfo;
            }

            return View("Lack2SummaryReport", model);
        }

        [HttpPost]
        public PartialViewResult SearchSummaryReports(Lack2SummaryReportsViewModel model)
        {
            model.DetailsList = SearchDataSummaryReports(model.SearchView);
            return PartialView("_Lack2ListSummaryReport", model);
        }

        public void ExportXlsSummaryReports(Lack2SummaryReportsViewModel model)
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

        private string CreateXlsSummaryReports(Lack2ExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataSummaryReports(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcel(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.BLack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BDocumentType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DocumentType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCompanyCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyCode);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NppbkcId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCk5SendingPlant)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5SendingPlant);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BSendingPlantAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SendingPlantAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BLack2Period)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Period);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BLack2Date)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Date);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BTypeExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TypeExcisableGoodsDesc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BTotalDeliveryExcisable)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TotalDeliveryExcisable);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BUom)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Uom);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.BPoa)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Poa);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BPoaManager)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PoaManager);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCreatedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCreatedTime)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedTime);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCreatedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedBy);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BApprovedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ApprovedDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BApprovedTime)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ApprovedTime);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BApprovedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ApprovedBy);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BLastChangedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastChangedDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BLastChangedTime)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastChangedTime);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Status);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BLegalizeData)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LegalizeData);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, Lack2ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.BLack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.BDocumentType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Document Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCompanyCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }
            if (modelExport.BNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC ID");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCk5SendingPlant)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Sending Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.BSendingPlantAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sending Plant Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.BLack2Period)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Period");
                iColumn = iColumn + 1;
            }

            if (modelExport.BLack2Date)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.BTypeExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }

            if (modelExport.BTotalDeliveryExcisable)
            {
                slDocument.SetCellValue(iRow, iColumn, "Total Delivered Excisable Goods (kg)");
                iColumn = iColumn + 1;
            }

            if (modelExport.BUom)
            {
                slDocument.SetCellValue(iRow, iColumn, "UOM");
                iColumn = iColumn + 1;
            }

            //start
            if (modelExport.BPoa)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA");
                iColumn = iColumn + 1;
            }
            if (modelExport.BPoaManager)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA  Manager");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCreatedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCreatedTime)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created Time");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCreatedBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created By");
                iColumn = iColumn + 1;
            }
            if (modelExport.BApprovedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Approved Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BApprovedTime)
            {
                slDocument.SetCellValue(iRow, iColumn, "Approved Time");
                iColumn = iColumn + 1;
            }
            if (modelExport.BApprovedBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Approved By");
                iColumn = iColumn + 1;
            }
            if (modelExport.BLastChangedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Change Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BLastChangedTime)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Change Time");
                iColumn = iColumn + 1;
            }
            if (modelExport.BStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.BLegalizeData)
            {
                slDocument.SetCellValue(iRow, iColumn, "Legalize Date");
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

            var fileName = "LACK2" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.Lack2FolderPaht), fileName);


            slDocument.SaveAs(path);

            return path;
        }

        #endregion

        #region Detail Reports

        private SelectList GetLack2CompanyCodeList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.CompanyCode,
                        TextField = x.CompanyCode
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2NppbkcIdList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.NppbkcId,
                        TextField = x.NppbkcId
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2SendingPlantList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.Ck5SendingPlant,
                        TextField = x.Ck5SendingPlant
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2GoodTypeList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.TypeExcisableGoods,
                        TextField = x.TypeExcisableGoods + " - " + x.TypeExcisableGoodsDesc
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2PeriodYearList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query = from x in listPbck2
                                                 select new SelectItemModel()
                                                 {
                                                     ValueField = x.PeriodYear,
                                                     TextField = x.PeriodYear
                                                 };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetGiDateList(bool isFrom, List<Lack2DetailReportDto> listLack2)
        {

            IEnumerable<SelectItemModel> query;
            if (isFrom)
                query = from x in listLack2.Where(c => c.GiDate != null).OrderBy(c => c.GiDate)
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.GiDate,
                            TextField = x.GiDate.Value.ToString("dd MMM yyyy")
                        };
            else
                query = from x in listLack2.Where(c => c.GiDate != null).OrderByDescending(c => c.GiDate)
                        select new SelectItemModel()
                        {
                            ValueField = x.GiDate,
                            TextField = x.GiDate.Value.ToString("dd MMM yyyy")
                        };

            return new SelectList(query.DistinctBy(c => c.TextField), "ValueField", "TextField");

        }
        private List<Lack2DetailReportsItem> SearchDataDetailReports(Lack2SearchDetailReportsViewModel filter = null)
        {
            Lack2GetDetailReportByParamInput input;
            List<Lack2DetailReportDto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Lack2GetDetailReportByParamInput();

                dbData = _lack2Bll.GetDetailReportsByParam(input);
                return Mapper.Map<List<Lack2DetailReportsItem>>(dbData);
            }

            //getbyparams
            input = Mapper.Map<Lack2GetDetailReportByParamInput>(filter);

            dbData = _lack2Bll.GetDetailReportsByParam(input);
            return Mapper.Map<List<Lack2DetailReportsItem>>(dbData);
        }

        private Lack2DetailReportsViewModel InitDetailReports(Lack2DetailReportsViewModel model)
        {
            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            var listLack2 = _lack2Bll.GetDetailReportsByParam(new Lack2GetDetailReportByParamInput());

            model.SearchView.CompanyCodeList = GetLack2CompanyCodeList(listLack2);
            model.SearchView.NppbkcIdList = GetLack2NppbkcIdList(listLack2);
            model.SearchView.SendingPlantIdList = GetLack2SendingPlantList(listLack2);
            model.SearchView.GoodTypeList = GetLack2GoodTypeList(listLack2);

            model.SearchView.PeriodMonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.SearchView.PeriodYearList = GetLack2PeriodYearList(listLack2);

            model.SearchView.DateFromList = GetGiDateList(true, listLack2);
            model.SearchView.DateToList = GetGiDateList(false, listLack2);

            var filter = new Lack2SearchDetailReportsViewModel();

            model.DetailsList = SearchDataDetailReports(filter);

            return model;
        }

        public ActionResult DetailReports()
        {

            Lack2DetailReportsViewModel model;
            try
            {

                model = new Lack2DetailReportsViewModel();


                model = InitDetailReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Lack2DetailReportsViewModel { MainMenu = Enums.MenuList.LACK2, CurrentMenu = PageInfo };
            }

            return View("Lack2DetailReport", model);
        }

        [HttpPost]
        public PartialViewResult SearchDetailReports(Lack2DetailReportsViewModel model)
        {
            model.DetailsList = SearchDataDetailReports(model.SearchView);
            return PartialView("_Lack2ListDetailReport", model);


        }

        public void ExportXlsDetailReports(Lack2DetailReportsViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsDetailReports(model.ExportModel);


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

        private string CreateXlsDetailReports(Lack2ExportDetailReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataDetailReports(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelDetail(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.BLack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCk5GiDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5GiDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCk5RegistrationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5RegistrationNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCk5RegistrationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5RegistrationDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCk5Total)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5Total);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BReceivingCompanyCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingCompanyCode);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BReceivingCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingCompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BReceivingNppbkc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingNppbkc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BReceivingAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCk5SendingPlant)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5SendingPlant);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BSendingPlantAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SendingPlantAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCompanyCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyCode);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.BCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NppbkcId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BTypeExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TypeExcisableGoodsDesc);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelDetail(SLDocument slDocument, Lack2ExportDetailReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.BLack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCk5GiDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 GI Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCk5RegistrationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Registration Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCk5RegistrationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Registration Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCk5Total)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Total");
                iColumn = iColumn + 1;
            }
            if (modelExport.BReceivingCompanyCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving Company Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.BReceivingCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving Company Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.BReceivingNppbkc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.BReceivingAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCk5SendingPlant)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Sending Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.BSendingPlantAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sending Plant Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCompanyCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Code");
                iColumn = iColumn + 1;
            }

            //start
            if (modelExport.BCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }
            if (modelExport.BNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC ID");
                iColumn = iColumn + 1;
            }
            if (modelExport.BTypeExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }


            return slDocument;

        }


        #endregion

        #region -------------- workflow --------------

        private void Lack2Workflow(int id, Enums.ActionType actionType, string comment, bool isModified = false)
        {
            var input = new Lack2WorkflowDocumentInput()
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                IsModified = isModified
            };

            _lack2Bll.Lack2Workflow(input);
        }

        public ActionResult ApproveDocument(int? id)
        {

            if (!id.HasValue)
                return HttpNotFound();

            bool isSuccess = false;
            try
            {
                Lack2Workflow(id.Value, Enums.ActionType.Approve, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Detail", "Lack2", new { id });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult RejectDocument(Lack2DetailViewModel model)
        {
            bool isSuccess = false;
            try
            {
                Lack2Workflow(model.Lack2Id, Enums.ActionType.Reject, model.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Detail", "Lack2", new { id = model.Lack2Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult GovApproveDocument(Lack2EditViewModel model)
        {

            if (model.DecreeFiles == null)
            {
                AddMessageInfo("Decree Doc is required.", Enums.MessageInfoType.Error);
                return RedirectToAction("Edit", "Lack2", new { id = model.Lack2Id });
            }

            bool isSuccess = false;
            string err = string.Empty;
            try
            {
                model.Documents = new List<Lack2DocumentDto>();
                if (model.DecreeFiles != null)
                {
                    foreach (var item in model.DecreeFiles)
                    {
                        if (item != null)
                        {
                            var filenamecheck = item.FileName;

                            if (filenamecheck.Contains("\\"))
                            {
                                filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }

                            var decreeDoc = new Lack2DocumentDto()
                            {
                                FILE_NAME = filenamecheck,
                                FILE_PATH = SaveUploadedFile(item, model.Lack2Id)
                            };
                            model.Documents.Add(decreeDoc);
                        }
                        else
                        {
                            AddMessageInfo("Please upload the decree doc", Enums.MessageInfoType.Error);
                            return RedirectToAction("Edit", "Lack2", new { id = model.Lack2Id });
                        }
                    }
                }

                Lack2WorkflowGovApprove(model, model.GovApprovalActionType, model.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (!isSuccess)
            {
                AddMessageInfo(err, Enums.MessageInfoType.Error);
                return RedirectToAction("Edit", "Lack2", new { id = model.Lack2Id });
            }

            AddMessageInfo("Document " + EnumHelper.GetDescription(model.GovStatus), Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        private void Lack2WorkflowGovApprove(Lack2EditViewModel lackData, Enums.ActionType actionType, string comment)
        {
            var input = new Lack2WorkflowDocumentInput()
            {
                DocumentId = lackData.Lack2Id,
                ActionType = actionType,
                UserRole = CurrentUser.UserRole,
                UserId = CurrentUser.USER_ID,
                DocumentNumber = lackData.Lack2Number,
                Comment = comment,
                AdditionalDocumentData = new Lack2WorkflowDocumentData()
                {
                    DecreeDate = lackData.DecreeDate,
                    Lack2DecreeDoc = lackData.Documents
                }
            };
            _lack2Bll.Lack2Workflow(input);
        }

        #endregion

        #region ------------- Private Methods ------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nppbkcId"></param>
        /// <returns></returns>
        private List<ZAIDM_EX_GOODTYPCompositeDto> GetExciseGoodsTypeData(string nppbkcId)
        {
            var data = _pbck1Bll.GetGoodsTypeByNppbkcId(nppbkcId);
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        private List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbkcDataByCompanyId(string companyId)
        {
            var data = _userPlantMapBll.GetAuthorizedNppbkc(new UserPlantMapGetAuthorizedNppbkc()
            {
                UserId = CurrentUser.USER_ID,
                CompanyCode = companyId
            });
            return data;
        }

        private List<T001WCompositeDto> GetSendingPlantDataByNppbkcId(string companyId, string nppbkcId)
        {
            var data = _userPlantMapBll.GetAuthorizdePlant(new UserPlantMapGetAuthorizedPlant()
            {
                UserId = CurrentUser.USER_ID,
                CompanyCode = companyId,
                NppbkcId =  nppbkcId
            });
            return data;
        }

        private SelectList GetCk5YearList()
        {
            var yearList = _ck5Bll.GetAllYearsByGiDate();
            var selectItemSource = yearList.Select(year => new SelectItemModel
            {
                // ReSharper disable SpecifyACultureInStringConversionExplicitly
                TextField = year.ToString(), ValueField = year.ToString()
                // ReSharper restore SpecifyACultureInStringConversionExplicitly
            }).ToList();
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }
        
        /// <summary>
        /// Fills the select lists for the IndexViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Lack2IndexViewModel</returns>
        private Lack2IndexViewModel InitIndexViewModel(Lack2IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        private string SaveUploadedFile(HttpPostedFileBase file, int lack2Id)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            sFileName = Constans.Lack2FolderPaht + Path.GetFileName("LACK2_" + lack2Id + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }


        #endregion

        #region Print and print preview 

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            Stream stream = GetReportStream(id.Value, "LACK-2");
            return File(stream, "application/pdf");
        }

        [EncryptedParameter]
        public ActionResult PrintPreview(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            Stream stream = GetReportStream(id.Value, "PREVIEW LACK-2");
            return File(stream, "application/pdf");
        }

        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var lack2 = _lack2Bll.GetById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.LACK2,
                FORM_ID = lack2.Lack2Id,
                FORM_NUMBER = lack2.Lack2Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel
            {
                PrintHistoryList =
                    Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack2.Lack2Number))
            };
            return PartialView("_PrintHistoryTable", model);

        }

        private Stream GetReportStream(int id, string printTitle)
        {
            var lack2 = _lack2Bll.GetDetailsById(id);

            var dsLack2 = CreateLack2Ds();
            var dt = dsLack2.Tables[0];
            DataRow drow;
            drow = dt.NewRow();
            drow[0] = lack2.Butxt;
            drow[1] = lack2.NppbkcId;
            drow[2] = lack2.LevelPlantName + ", " + lack2.LevelPlantCity;

            var headerFooter = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput
            {
                CompanyCode = lack2.Burks,
                FormTypeId = Enums.FormType.LACK2
            });
            if (headerFooter != null)
            {
                drow[3] = GetHeader(headerFooter.HEADER_IMAGE_PATH);
                drow[4] = headerFooter.FOOTER_CONTENT;
            }
            drow[5] = lack2.ExTypDesc;
            drow[6] = lack2.PeriodNameInd + " " + lack2.PeriodYear;
            drow[7] = lack2.LevelPlantCity;
            
            drow[8] = lack2.SubmissionDate == null ? null : string.Format("{0} {1} {2}", lack2.SubmissionDate.Value.Day, _monthBll.GetMonth(lack2.SubmissionDate.Value.Month).MONTH_NAME_IND, lack2.SubmissionDate.Value.Year);
            if (lack2.ApprovedBy != null)
            {
                var poa = _poabll.GetDetailsById(lack2.ApprovedBy);
                if (poa != null)
                {
                    drow[9] = poa.PRINTED_NAME;
                }
            }

            drow[10] = printTitle;
            if (lack2.DecreeDate != null)
            {
                var lack2DecreeDate = lack2.DecreeDate.Value;
                var lack2Month = _monthBll.GetMonth(lack2DecreeDate.Month).MONTH_NAME_IND;

                drow[11] = string.Format("{0} {1} {2}", lack2DecreeDate.Day, lack2Month, lack2DecreeDate.Year);

            }
            dt.Rows.Add(drow);

            var dtDetail = dsLack2.Tables[1];
            foreach (var item in lack2.Items)
            {
                DataRow drowDetail;
                drowDetail = dtDetail.NewRow();
                drowDetail[0] = item.Ck5Number;
                drowDetail[1] = item.Ck5GIDate;
                drowDetail[2] = item.Ck5ItemQty.ToString("N2");
                drowDetail[3] = item.CompanyName;
                drowDetail[4] = item.CompanyNppbkc;
                drowDetail[5] = item.CompanyAddress;
                dtDetail.Rows.Add(drowDetail);

            }
            // object of data row 

            ReportClass rpt = new ReportClass();
            string report_path = ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = report_path + "LACK2\\Preview.rpt";
            rpt.Load();
            rpt.SetDataSource(dsLack2);

            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return stream;
        }

        private DataSet CreateLack2Ds()
        {
            DataSet ds = new DataSet("dsLack2");

            DataTable dt = new DataTable("Lack2");

            // object of data row 
            DataRow drow;
            dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
            dt.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dt.Columns.Add("Alamat", System.Type.GetType("System.String"));
            dt.Columns.Add("Header", System.Type.GetType("System.Byte[]"));
            dt.Columns.Add("Footer", System.Type.GetType("System.String"));
            dt.Columns.Add("BKC", System.Type.GetType("System.String"));
            dt.Columns.Add("Period", System.Type.GetType("System.String"));
            dt.Columns.Add("City", System.Type.GetType("System.String"));
            dt.Columns.Add("CreatedDate", System.Type.GetType("System.String"));
            dt.Columns.Add("PoaPrintedName", System.Type.GetType("System.String"));
            dt.Columns.Add("Preview", System.Type.GetType("System.String"));
            dt.Columns.Add("DecreeDate", System.Type.GetType("System.String"));

            //detail
            DataTable dtDetail = new DataTable("Lack2Item");
            dtDetail.Columns.Add("Nomor", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Tanggal", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Jumlah", System.Type.GetType("System.String"));

            dtDetail.Columns.Add("NamaPerusahaan", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Alamat", System.Type.GetType("System.String"));

            ds.Tables.Add(dt);
            ds.Tables.Add(dtDetail);
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

        #endregion

        #region ----------------- Dashboard Page -------------

        public ActionResult DashboardPage()
        {
            return View();
        }

        #endregion

    }

}