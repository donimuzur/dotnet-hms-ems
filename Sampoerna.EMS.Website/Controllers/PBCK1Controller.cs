﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.ReportingData;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;
using System.Configuration;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK1Controller : BaseController
    {

        private IPBCK1BLL _pbck1Bll;
        private ICK5BLL _ck5Bll;
        private IPbck1DecreeDocBLL _pbck1DecreeDocBll;
        private IPlantBLL _plantBll;
        private Enums.MenuList _mainMenu;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IWorkflowBLL _workflowBll;
        private IMasterDataBLL _masterDataBll;
        private IPrintHistoryBLL _printHistoryBll;
        private IPOABLL _poaBll;
        private ILACK1BLL _lackBll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IMonthBLL _monthBll;
        private ISupplierPortBLL _supplierPortBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private ICompanyBLL _companyBll;
        private IUnitOfMeasurementBLL _uomBll;
        private ILFA1BLL _lfa1Bll;
        private IT001KBLL _t001kBll;
        private IPOABLL _poabll;

        public PBCK1Controller(IPageBLL pageBLL, IUnitOfMeasurementBLL uomBll, ICompanyBLL companyBll, IMasterDataBLL masterDataBll, IMonthBLL monthbll, IZaidmExGoodTypeBLL goodTypeBll, ISupplierPortBLL supplierPortBll, IZaidmExNPPBKCBLL nppbkcbll, IPBCK1BLL pbckBll, IPlantBLL plantBll, IChangesHistoryBLL changesHistoryBll,
            IWorkflowHistoryBLL workflowHistoryBll, IWorkflowBLL workflowBll, IPrintHistoryBLL printHistoryBll, IPOABLL poaBll, ILACK1BLL lackBll, ILFA1BLL lfa1Bll, IT001KBLL t001kBll, IPbck1DecreeDocBLL pbck1DecreeDocBll, ICK5BLL ck5Bll, IPOABLL poabll)
            : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
            _plantBll = plantBll;
            _mainMenu = Enums.MenuList.PBCK1;
            _changesHistoryBll = changesHistoryBll;
            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
            _printHistoryBll = printHistoryBll;
            _poaBll = poaBll;
            _lackBll = lackBll;
            _nppbkcbll = nppbkcbll;
            _monthBll = monthbll;
            _supplierPortBll = supplierPortBll;
            _goodTypeBll = goodTypeBll;
            _companyBll = companyBll;
            _lfa1Bll = lfa1Bll;
            _uomBll = uomBll;
            _t001kBll = t001kBll;
            _pbck1DecreeDocBll = pbck1DecreeDocBll;
            _ck5Bll = ck5Bll;
            _poabll = poabll;
        }

        private List<Pbck1Item> GetOpenDocument(Pbck1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var pbck1Data = _pbck1Bll.GetOpenDocumentByParam(new Pbck1GetOpenDocumentByParamInput()).OrderByDescending(d => d.Pbck1Number);
                return Mapper.Map<List<Pbck1Item>>(pbck1Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetOpenDocumentByParamInput>(filter);
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;

            var dbData = _pbck1Bll.GetOpenDocumentByParam(input).OrderByDescending(c => c.Pbck1Number);
            return Mapper.Map<List<Pbck1Item>>(dbData);
        }

        private List<Pbck1Item> GetCompletedDocument(Pbck1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var pbck1Data = _pbck1Bll.GetCompletedDocumentByParam(new Pbck1GetCompletedDocumentByParamInput());
                return Mapper.Map<List<Pbck1Item>>(pbck1Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetCompletedDocumentByParamInput>(filter);
            var dbData = _pbck1Bll.GetCompletedDocumentByParam(input);
            return Mapper.Map<List<Pbck1Item>>(dbData);
        }

        private SelectList GetYearList(IEnumerable<Pbck1Item> pbck1Data)
        {
            var query = from x in pbck1Data
                        select new SelectItemModel()
                        {
                            ValueField = x.Year,
                            TextField = x.Year
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList LackYearList(int year)
        {
            var years = new List<SelectItemModel>();
            years.Add(new SelectItemModel() { ValueField = year, TextField = year.ToString() });
            years.Add(new SelectItemModel() { ValueField = year - 1, TextField = (year - 1).ToString() });
            years.Add(new SelectItemModel() { ValueField = year - 2, TextField = (year - 2).ToString() });
            years.Add(new SelectItemModel() { ValueField = year - 3, TextField = (year - 3).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId)
        {
            var listPoa = _poaBll.GetPoaByNppbkcIdAndMainPlant(nppbkcId);
            var model = new Pbck1ViewModel { SearchInput = { PoaList = new SelectList(listPoa, "POA_ID", "PRINTED_NAME") } };
            return Json(model);
        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(Pbck1ViewModel model)
        {
            model.Details = GetOpenDocument(model.SearchInput);
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            return PartialView("_Pbck1Table", model);
        }

        [HttpPost]
        public PartialViewResult UploadFileConversion(HttpPostedFileBase prodConvExcelFile, string nppbkc, string isNppbckImportChecked)
        {
            var data = (new ExcelReader()).ReadExcel(prodConvExcelFile);
            var model = new Pbck1ItemViewModel() { Detail = new Pbck1Item() };
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var uploadItem = new Pbck1ProdConvModel();

                    try
                    {
                        var text = datarow[2];
                        decimal value;
                        if (Decimal.TryParse(text, out value))
                        {
                            //text = Math.Round(Convert.ToDecimal(text), 4).ToString();
                            text = Convert.ToDecimal(text).ToString();
                        }

                        uploadItem.ProductCode = datarow[0];
                        uploadItem.BrandCE = datarow[1];
                        uploadItem.ConverterOutput = text;
                        uploadItem.ConverterUom = datarow[3];

                        model.Detail.Pbck1ProdConverter.Add(uploadItem);

                    }
                    catch (Exception)
                    {
                        continue;

                    }
                }
            }

            var input = Mapper.Map<List<Pbck1ProdConverterInput>>(model.Detail.Pbck1ProdConverter);
            var outputResult = _pbck1Bll.ValidatePbck1ProdConverterUpload(input, nppbkc, Boolean.Parse(isNppbckImportChecked));

            model.Detail.Pbck1ProdConverter = Mapper.Map<List<Pbck1ProdConvModel>>(outputResult);

            return PartialView("_ProdConvList", model);
        }

        [HttpPost]
        public PartialViewResult UploadFilePlan(HttpPostedFileBase prodPlanExcelFile, string goodType, DateTime? periodFrom, DateTime? periodTo)
        {
            var data = (new ExcelReader()).ReadExcel(prodPlanExcelFile);
            var model = new Pbck1ItemViewModel() { Detail = new Pbck1Item() };
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var uploadItem = new Pbck1ProdPlanModel();

                    try
                    {
                        uploadItem.Month = datarow[0];
                        uploadItem.ProductCode = datarow[1];
                        uploadItem.Amount = datarow[2];
                        uploadItem.BkcRequired = datarow[3];
                        uploadItem.BkcRequiredUomId = datarow[4];

                        model.Detail.Pbck1ProdPlan.Add(uploadItem);

                    }
                    catch (Exception)
                    {
                        continue;

                    }
                }
            }

            var input = new ValidatePbck1ProdPlanUploadParamInput()
            {
                ProdPlanData = Mapper.Map<List<Pbck1ProdPlanInput>>(model.Detail.Pbck1ProdPlan),
                ProdPlanPeriodFrom = periodFrom,
                ProdPlanPeriodTo = periodTo,
                GoodType = goodType
            };

            var outputResult = _pbck1Bll.ValidatePbck1ProdPlanUpload(input);

            model.Detail.Pbck1ProdPlan = !outputResult.Success ? new List<Pbck1ProdPlanModel>() : Mapper.Map<List<Pbck1ProdPlanModel>>(outputResult.Data);
            model.ProdPlanUploadValidateResult = new ProdPlanUploadResult()
            {
                IsSuccess = outputResult.Success,
                ErrorMessage = outputResult.ErrorMessage
            };
            return PartialView("_ProdPlanList", model);
        }

        private Pbck1ItemViewModel ModelInitial(Pbck1ItemViewModel model)
        {

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.NppbkcList = GlobalFunctions.GetNppbkcByFlagDeletionList(false);
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.SupplierPortList = GlobalFunctions.GetSupplierPortList(_supplierPortBll);
            //model.SupplierPlantList = GlobalFunctions.GetSupplierPlantList();
            model.SupplierPlantList = GlobalFunctions.GetPlantAll();

            var dataGoodType = _goodTypeBll.GetAll().Where(x => x.IS_DELETED != true && (x.EXC_GOOD_TYP == "02" || x.EXC_GOOD_TYP == "04"));
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(dataGoodType);

            model.GoodTypeList = new SelectList(selectItemSource, "ValueField", "TextField");
            model.UomList = GlobalFunctions.GetUomList(_uomBll);

            var pbck1RefList = GetCompletedDocument();

            if (model.Detail != null && model.Detail.Pbck1Reference.HasValue)
            {
                //exclude current pbck1 document on list
                pbck1RefList = pbck1RefList.Where(c => c.Pbck1Id != model.Detail.Pbck1Reference.Value).ToList();
            }

            model.PbckReferenceList = new SelectList(pbck1RefList, "Pbck1Id", "Pbck1Number");

            //model.YearList = CreateYearList();
            var year = DateTime.Now.Year;

            if (model.Detail != null && model.Detail.PeriodFrom.HasValue)
            {
                year = model.Detail.PeriodFrom.Value.Year;
            }

            model.YearList = LackYearList(year);

            model.AllowPrintDocument = false;

            return model;
        }

        private Pbck1ItemViewModel CleanSupplierInfo(Pbck1ItemViewModel model)
        {
            if (model != null && model.Detail != null)
            {
                if (string.IsNullOrEmpty(model.Detail.SupplierKppbcId)
                && !string.IsNullOrEmpty(model.Detail.HiddenSupplierKppbcId))
                {
                    model.Detail.SupplierKppbcId = model.Detail.HiddenSupplierKppbcId;
                }
                if (string.IsNullOrEmpty(model.Detail.SupplierAddress) &&
                    !string.IsNullOrEmpty(model.Detail.HiddendSupplierAddress))
                {
                    model.Detail.SupplierAddress = model.Detail.HiddendSupplierAddress;
                }
                if (string.IsNullOrEmpty(model.Detail.SupplierNppbkcId)
                    && !string.IsNullOrEmpty(model.Detail.HiddenSupplierNppbkcId))
                {
                    model.Detail.SupplierNppbkcId = model.Detail.HiddenSupplierNppbkcId;
                }
            }
            return model;
        }

        [HttpPost]
        public JsonResult GetSupplierPlant(bool isNppbkcImport)
        {
            return Json(GlobalFunctions.GetPlantByNppbkcImport(isNppbkcImport));
        }

        [HttpPost]
        public JsonResult GetNppbkcDetail(string nppbkcid)
        {
            var data = _plantBll.GetMainPlantByNppbkcId(nppbkcid);
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetSupplierPlantDetail(string plantid, bool isNppbkcImport)
        {
            var data = _plantBll.GetId(plantid);

            var lfa1Data = _lfa1Bll.GetById(data.KPPBC_NO);

            data.KPPBC_NAME = lfa1Data.NAME1;

            if (isNppbkcImport)
                data.NPPBKC_ID = data.NPPBKC_IMPORT_ID;

            return Json(Mapper.Map<DetailPlantT1001W>(data));
        }

        public void ExportClientsListToExcel(int id)
        {

            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, id.ToString());

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

            var fileName = "PBCK1" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        private SelectList Pbck1DashboardYear()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }


        #region ------- index ---------

        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            var model = InitPbck1ViewModel(new Pbck1ViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                SearchInput =
                {
                    DocumentType = Enums.Pbck1DocumentType.OpenDocument

                },
                IsShowNewButton = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer
            });
            return View("Index", model);
        }

        public Pbck1ViewModel InitPbck1ViewModel(Pbck1ViewModel model)
        {
            model.SearchInput.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchInput.PoaList = new SelectList(new List<SelectItemModel>(), "ValueField", "TextField");
            switch (model.SearchInput.DocumentType)
            {
                case Enums.Pbck1DocumentType.CompletedDocument:
                    model.Details = GetCompletedDocument(model.SearchInput);
                    break;
                case Enums.Pbck1DocumentType.OpenDocument:
                    model.Details = GetOpenDocument(model.SearchInput);
                    break;
            }

            model.SearchInput.YearList = GetYearList(model.Details);

            return model;
        }

        #endregion

        #region ----- Edit -----

        public ActionResult Edit(int? id)
        {

            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var pbck1Data = _pbck1Bll.GetById(id.Value);

            if (pbck1Data == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Details", new { id });
            }

            var model = new Pbck1ItemViewModel();
            var isCurrManager = false;
            try
            {
                model.Detail = Mapper.Map<Pbck1Item>(pbck1Data);

                model = ModelInitial(model);

                if (CurrentUser.UserRole == Enums.UserRole.Manager)
                {
                    //redirect to details for approval/rejected
                    //return RedirectToAction("Details", new { id });
                    isCurrManager = true;
                }

                var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, id.Value.ToString()));

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = pbck1Data.Pbck1Number;
                workflowInput.DocumentStatus = pbck1Data.Status;
                workflowInput.NppbkcId = pbck1Data.NppbkcId;
                workflowInput.FormType = Enums.FormType.PBCK1;

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));
                model.WorkflowHistory = workflowHistory;
                model.ChangesHistoryList = changeHistory;

                var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(pbck1Data.Pbck1Number));
                model.PrintHistoryList = printHistory;

                model.DocStatus = model.Detail.Status;

                model.SupInfo.SupplierPlantWerks = model.Detail.SupplierPlantWerks;
                model.SupInfo.SupplierAddress = model.Detail.SupplierAddress;
                model.SupInfo.SupplierNppkbc = model.Detail.SupplierNppbkcId;
                model.SupInfo.SupplierKppkbc = model.Detail.SupplierKppbcName;
                model.SupInfo.SupplierPlantName = model.Detail.SupplierPlant;
                model.SupInfo.SupplierPhone = model.Detail.SupplierPhone;

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput
                {
                    DocumentStatus = model.Detail.Status,
                    FormView = Enums.FormViewType.Detail,
                    UserRole = CurrentUser.UserRole,
                    CreatedUser = pbck1Data.CreatedById,
                    CurrentUser = CurrentUser.USER_ID,
                    CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                    DocumentNumber = model.Detail.Pbck1Number,
                    NppbkcId = model.Detail.NppbkcId,
                    FormType = Enums.FormType.PBCK1
                };
                if (isCurrManager) input.ManagerApprove = model.Detail.ApprovedByManagerId;

                ////workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;
                ViewBag.IsCurrManager = isCurrManager;
                if (!allowApproveAndReject)
                {
                    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);

                    if (isCurrManager)
                        model.AllowManagerReject = _workflowBll.AllowManagerReject(input);

                }

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Detail.Status);

                if (model.Detail.Status == Enums.DocumentStatus.WaitingGovApproval)
                {
                    model.ActionType = "GovApproveDocument";
                }

                if (model.Detail.Status == Enums.DocumentStatus.Completed)
                {
                    model.ActionType = "ChangeCompletedDocument";
                }

                if (((model.ActionType == "GovApproveDocument" || model.ActionType == "ChangeCompletedDocument") && model.AllowGovApproveAndReject))
                {

                }
                else if (!ValidateEditDocument(model, false))
                {
                    if (!isCurrManager)
                        return RedirectToAction("Details", new { id });
                }

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        private bool ValidateEditDocument(Pbck1ItemViewModel model, bool message = true)
        {

            //check is Allow Edit Document
            var isAllowEditDocument = _workflowBll.AllowEditDocumentPbck1(new WorkflowAllowEditAndSubmitInput()
            {
                DocumentStatus = model.Detail.Status,
                CreatedUser = model.Detail.CreatedById,
                CurrentUser = CurrentUser.USER_ID
            });

            if (!isAllowEditDocument)
            {
                if (message)
                    AddMessageInfo("Operation not allowed.", Enums.MessageInfoType.Error);

                return false;
            }

            return true;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pbck1ItemViewModel model)
        {
            try
            {
                if (model.Detail.Pbck1ProdConverter.Count == 0)
                {
                    AddMessageInfo("Cannot save PBCK-1. Please fill all the mandatory fields", Enums.MessageInfoType.Error);
                    model = ModelInitial(model);
                    model = SetHistory(model);
                    return View(model);
                }

                var validate = validationForm(model);

                if (validate != "")
                {
                    AddMessageInfo(validate, Enums.MessageInfoType.Error);
                    model = ModelInitial(model);
                    model = SetHistory(model);
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();

                    if (errors.Count > 0)
                    {
                        if (model.Detail.Pbck1Type == Enums.PBCK1Type.Additional && model.Detail.Pbck1Reference == null)
                        {
                            AddMessageInfo("Cannot save PBCK-1. There is no data for references number of PBCK-1", Enums.MessageInfoType.Error);
                        }
                        else
                        {
                            AddMessageInfo("Cannot save PBCK-1. Please fill all the mandatory fields", Enums.MessageInfoType.Error);
                        }
                    }
                    else
                    {
                        AddMessageInfo("Cannot save PBCK-1. Please fill all the mandatory fields", Enums.MessageInfoType.Error);
                    }


                    model = ModelInitial(model);
                    model = SetHistory(model);
                    return View(model);
                }

                if (!ValidateEditDocument(model))
                {
                    model = ModelInitial(model);
                    model = SetHistory(model);
                    return View(model);
                }

                Pbck1ItemViewModel modelOld = model;

                //model.Detail.Status = Enums.DocumentStatus.Revised;
                model = CleanSupplierInfo(model);

                //process save
                var dataToSave = Mapper.Map<Pbck1Dto>(model.Detail);
                var input = new Pbck1SaveInput()
                {
                    Pbck1 = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Modified
                };

                var checkUnique = _pbck1Bll.checkUniquePBCK1(input);

                if (checkUnique != null)
                {
                    AddMessageInfo("PBCK-1 no " + checkUnique + " already exist", Enums.MessageInfoType.Error);
                    return CreateInitial(modelOld);
                }

                //set null, set this field only from Gov Approval
                input.Pbck1.DecreeDate = null;
                input.Pbck1.QtyApproved = null;
                input.Pbck1.StatusGov = null;
                input.Pbck1.Pbck1DecreeDoc = null;

                bool isSubmit = model.Detail.IsSaveSubmit == "submit";

                var saveResult = _pbck1Bll.Save(input);

                if (saveResult.Success)
                {
                    if (isSubmit)
                    {
                        Pbck1Workflow(model.Detail.Pbck1Id, Enums.ActionType.Submit, string.Empty);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
                    }

                    //return RedirectToAction("Index");
                    AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                    return RedirectToAction("Edit", new { id = model.Detail.Pbck1Id });
                }

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = ModelInitial(model);
                model = SetHistory(model);
                return View(model);
            }

            var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, model.Detail.Pbck1Id.ToString()));

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormNumber = model.Detail.Pbck1Number;
            workflowInput.DocumentStatus = model.Detail.Status;
            workflowInput.NppbkcId = model.Detail.NppbkcId;

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            model.WorkflowHistory = workflowHistory;
            model.ChangesHistoryList = changeHistory;

            return View(ModelInitial(model));

        }

        private Pbck1ItemViewModel SetHistory(Pbck1ItemViewModel model)
        {
            var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, model.Detail.Pbck1Id.ToString()));

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormNumber = model.Detail.Pbck1Number;
            workflowInput.DocumentStatus = model.Detail.Status;
            workflowInput.NppbkcId = model.Detail.NppbkcId;

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            model.WorkflowHistory = workflowHistory;
            model.ChangesHistoryList = changeHistory;

            return model;
        }

        #endregion

        #region ------ details ----

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            var pbck1Data = _pbck1Bll.GetById(id.Value);

            if (pbck1Data == null)
            {
                return HttpNotFound();
            }

            bool isCurrManager = CurrentUser.UserRole == Enums.UserRole.Manager;
            ViewBag.IsCurrManager = isCurrManager;

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormNumber = pbck1Data.Pbck1Number;
            workflowInput.DocumentStatus = pbck1Data.Status;
            workflowInput.NppbkcId = pbck1Data.NppbkcId;
            workflowInput.FormType = Enums.FormType.PBCK1;

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1,
                    id.Value.ToString()));

            var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(pbck1Data.Pbck1Number));

            var model = new Pbck1ItemViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<Pbck1Item>(pbck1Data),
                ChangesHistoryList = changesHistory,
                WorkflowHistory = workflowHistory,
                PrintHistoryList = printHistory
            };

            model.DocStatus = model.Detail.Status;

            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Detail.Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = pbck1Data.CreatedById,
                CurrentUser = CurrentUser.USER_ID,
                CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                DocumentNumber = model.Detail.Pbck1Number,
                NppbkcId = model.Detail.NppbkcId,
                ManagerApprove = model.Detail.ApprovedByManagerId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }
            else if (CurrentUser.UserRole == Enums.UserRole.POA)
            {
                model.AllowApproveAndReject = false;
                foreach (POADto poa in _poaBll.GetPoaByNppbkcIdAndMainPlant(model.Detail.NppbkcId))
                {
                    if (poa.POA_ID == CurrentUser.USER_ID)
                    {
                        model.AllowApproveAndReject = true;
                    }
                }

            }



            model.AllowPrintDocument = _workflowBll.AllowPrint(model.Detail.Status);

            return View(model);
        }

        #endregion

        #region ----- create -----

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager || CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                //can't create PBCK1 Document
                AddMessageInfo("Can't create PBCK-1 Document for User with " + EnumHelper.GetDescription(CurrentUser.UserRole) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            return CreateInitial(new Pbck1ItemViewModel()
            {
                Detail = new Pbck1Item()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pbck1ItemViewModel model)
        {
            try
            {
                if (model.Detail.Pbck1ProdConverter.Count == 0)
                {
                    AddMessageInfo("Cannot save PBCK-1. Please fill all the mandatory fields", Enums.MessageInfoType.Error);
                    return CreateInitial(model);
                }

                var validate = validationForm(model);

                if (validate != "")
                {
                    AddMessageInfo(validate, Enums.MessageInfoType.Error);
                    return CreateInitial(model);
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();

                    if (errors.Count > 0)
                    {
                        if (model.Detail.Pbck1Type == Enums.PBCK1Type.Additional && model.Detail.Pbck1Reference == null)
                        {
                            AddMessageInfo("Cannot save PBCK-1. There is no data for references number of PBCK-1", Enums.MessageInfoType.Error);
                        }
                        else
                        {
                            AddMessageInfo("Cannot save PBCK-1. Please fill all the mandatory fields", Enums.MessageInfoType.Error);
                        }
                    }
                    else
                    {
                        AddMessageInfo("Cannot save PBCK-1. Please fill all the mandatory fields", Enums.MessageInfoType.Error);
                    }

                    return CreateInitial(model);
                }

                Pbck1ItemViewModel modelOld = model;

                model = CleanSupplierInfo(model);

                //process save
                var dataToSave = Mapper.Map<Pbck1Dto>(model.Detail);
                dataToSave.CreatedById = CurrentUser.USER_ID;
                dataToSave.GoodTypeDesc = !string.IsNullOrEmpty(dataToSave.GoodTypeDesc) ? dataToSave.GoodTypeDesc : string.Empty;

                var input = new Pbck1SaveInput()
                {
                    Pbck1 = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Created
                };

                var checkUnique = _pbck1Bll.checkUniquePBCK1(input);

                if (checkUnique != null)
                {
                    AddMessageInfo("PBCK-1 no " + checkUnique + " already exist", Enums.MessageInfoType.Error);
                    return CreateInitial(modelOld);
                }


                //only add this information from gov approval,
                //when save create/edit 
                input.Pbck1.DecreeDate = null;
                input.Pbck1.QtyApproved = null;
                input.Pbck1.StatusGov = null;
                input.Pbck1.Pbck1DecreeDoc = null;

                var saveResult = _pbck1Bll.Save(input);

                if (saveResult.Success)
                {
                    AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                    return RedirectToAction("Edit", new { id = saveResult.Id });
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }

            return CreateInitial(model);

        }

        public ActionResult CreateInitial(Pbck1ItemViewModel model)
        {
            return View("Create", ModelInitial(model));
        }

        #endregion

        #region Completed Document

        public ActionResult CompletedDocument()
        {
            var model = InitPbck1ViewModel(new Pbck1ViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                SearchInput = new Pbck1FilterViewModel()
                {
                    DocumentType = Enums.Pbck1DocumentType.CompletedDocument
                },
                IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer
            });
            return View("CompletedDocument", model);
        }

        public string validationForm(Pbck1ItemViewModel model)
        {
            var message = "";

            if (new DateTime(model.Detail.Lack1FormYear, model.Detail.Lack1FromMonthId, 1) > new DateTime(model.Detail.Lack1ToYear, model.Detail.Lack1ToMonthId, 1))
            {
                message = "Lack 1 From cannot be greater than Lack 1 To";
            }

            if (model.Detail.PlanProdFrom > model.Detail.PlanProdTo)
            {
                message = "Plan Production From cannot be greater than Plan Production To";
            }

            if (model.Detail.NppbkcId == model.Detail.SupplierNppbkcId)
            {
                message = "Original NPPBKC cannot be the same as supplier NPPBCK";
            }

            return message;
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(Pbck1ViewModel model)
        {
            model.Details = GetCompletedDocument(model.SearchInput);
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            return PartialView("_Pbck1CompletedDocumentTable", model);
        }

        #endregion

        #region Workflow

        private void Pbck1Workflow(int id, Enums.ActionType actionType, string comment)
        {
            var input = new Pbck1WorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment
            };

            _pbck1Bll.Pbck1Workflow(input);
        }

        private void Pbck1WorkflowGovApprove(Pbck1Item pbck1Data, Enums.ActionType actionType, string comment)
        {
            var input = new Pbck1WorkflowDocumentInput()
            {
                DocumentId = pbck1Data.Pbck1Id,
                ActionType = actionType,
                UserRole = CurrentUser.UserRole,
                UserId = CurrentUser.USER_ID,
                DocumentNumber = pbck1Data.Pbck1Number,
                Comment = comment,
                AdditionalDocumentData = new Pbck1WorkflowDocumentData()
                {
                    DecreeDate = pbck1Data.DecreeDate.Value,
                    QtyApproved = pbck1Data.QtyApproved == null ? 0 : Convert.ToDecimal(pbck1Data.QtyApproved),
                    Pbck1DecreeDoc = Mapper.Map<List<Pbck1DecreeDocDto>>(pbck1Data.Pbck1DecreeDoc)
                }
            };
            _pbck1Bll.Pbck1Workflow(input);
        }

        private void Pbck1WorkflowCompletedEdit(Pbck1Item pbck1Data, Enums.ActionType actionType, string comment)
        {
            var input = new Pbck1WorkflowDocumentInput()
            {
                DocumentId = pbck1Data.Pbck1Id,
                ActionType = actionType,
                UserRole = CurrentUser.UserRole,
                UserId = CurrentUser.USER_ID,
                DocumentNumber = pbck1Data.Pbck1Number,
                Comment = comment,
                DocumentStatus = Enums.DocumentStatus.Completed,
                AdditionalDocumentData = new Pbck1WorkflowDocumentData()
                {
                    DecreeDate = pbck1Data.DecreeDate.Value,
                    QtyApproved = pbck1Data.QtyApproved == null ? 0 : Convert.ToDecimal(pbck1Data.QtyApproved),
                    Pbck1DecreeDoc = Mapper.Map<List<Pbck1DecreeDocDto>>(pbck1Data.Pbck1DecreeDoc)
                }
            };
            _pbck1Bll.Save(input);
        }

        private void Pbck1WorkflowReturnToGovApprove(Pbck1Item pbck1Data, Enums.ActionType actionType, string comment)
        {
            var input = new Pbck1WorkflowDocumentInput()
            {
                DocumentId = pbck1Data.Pbck1Id,
                ActionType = Enums.ActionType.Reject,
                UserRole = CurrentUser.UserRole,
                UserId = CurrentUser.USER_ID,
                DocumentNumber = pbck1Data.Pbck1Number,
                Comment = comment,
                DocumentStatus = Enums.DocumentStatus.Completed,
                AdditionalDocumentData = new Pbck1WorkflowDocumentData()
                {
                    DecreeDate = null,
                    QtyApproved = 0,
                    Pbck1DecreeDoc = Mapper.Map<List<Pbck1DecreeDocDto>>(pbck1Data.Pbck1DecreeDoc)
                }
            };
            _pbck1Bll.Save(input);
        }

        public ActionResult SubmitDocument(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            bool isSuccess = false;

            try
            {
                Pbck1Workflow(id.Value, Enums.ActionType.Submit, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (isSuccess)
            {
                AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
            }

            return RedirectToAction("Details", "Pbck1", new { id });
        }

        public ActionResult ApproveDocument(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            bool isSuccess = false;
            try
            {
                Pbck1Workflow(id.Value, Enums.ActionType.Approve, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Details", "Pbck1", new { id });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult RejectDocument(Pbck1ItemViewModel model)
        {
            bool isSuccess = false;
            try
            {
                Pbck1Workflow(model.Detail.Pbck1Id, Enums.ActionType.Reject, model.Detail.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetLatestSaldoLack(int month, int year, string nppbkcid, string plant, string goodtype)
        {
            var latestSaldo = _lackBll.GetLatestSaldoPerPeriod(new Lack1GetLatestSaldoPerPeriodInput() { MonthTo = month, YearTo = year, NppbkcId = nppbkcid, SupplierPlantWerks = plant, ExcisableGoodsType = goodtype });
            return Json(new { latestSaldo });
        }

        [HttpPost]
        public ActionResult GovApproveDocument(Pbck1ItemViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            //}

            if (model.Detail.Pbck1DecreeFiles == null)
            {
                AddMessageInfo("Decree Doc is required.", Enums.MessageInfoType.Error);
                return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            }

            bool isSuccess = false;
            var currentUserId = CurrentUser;
            try
            {
                model.Detail.Pbck1DecreeDoc = new List<Pbck1DecreeDocModel>();
                if (model.Detail.Pbck1DecreeFiles != null)
                {
                    foreach (var item in model.Detail.Pbck1DecreeFiles)
                    {
                        if (item != null)
                        {
                            var filenamecheck = item.FileName;

                            if (filenamecheck.Contains("\\"))
                            {
                                filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }

                            var decreeDoc = new Pbck1DecreeDocModel()
                            {
                                FILE_NAME = filenamecheck,
                                FILE_PATH = SaveUploadedFile(item, model.Detail.Pbck1Id),
                                CREATED_BY = currentUserId.USER_ID,
                                CREATED_DATE = DateTime.Now
                            };
                            model.Detail.Pbck1DecreeDoc.Add(decreeDoc);
                        }
                        else
                        {
                            AddMessageInfo("Please upload the decree doc", Enums.MessageInfoType.Error);
                            return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
                        }
                    }
                }


                var input = new Pbck1UpdateReportedOn()
                {
                    Id = model.Detail.Pbck1Id,
                    ReportedOn = model.Detail.ReportedOn
                };

                _pbck1Bll.UpdateReportedOn(input);

                Pbck1WorkflowGovApprove(model.Detail, model.Detail.GovApprovalActionType, model.Detail.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            AddMessageInfo("Document " + EnumHelper.GetDescription(model.Detail.StatusGov), Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ChangeCompletedDocument(Pbck1ItemViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            //}

            var oldDoc = _pbck1Bll.GetById(model.Detail.Pbck1Id).Pbck1DecreeDoc.Select(c => c.PBCK1_DECREE_DOC_ID);

            if (model.Detail.Pbck1DecreeFiles == null)
            {
                AddMessageInfo("Decree Doc is required.", Enums.MessageInfoType.Error);
                return RedirectToAction("Edit", "Pbck1", new { id = model.Detail.Pbck1Id });
            }


            bool isSuccess = false;
            bool validDoc = true;
            var currentUserId = CurrentUser;
            try
            {
                model.Detail.Pbck1DecreeDoc = new List<Pbck1DecreeDocModel>();

                if (model.Detail.StatusGov == 0 && model.Detail.DecreeDate == null)
                {
                    Pbck1WorkflowReturnToGovApprove(model.Detail, model.Detail.GovApprovalActionType, model.Detail.Comment);
                    AddMessageInfo("Document Rejected", Enums.MessageInfoType.Success);
                    return RedirectToAction("Index");
                }

                if (model.Detail.Pbck1DecreeFiles != null)
                {
                    foreach (var item in model.Detail.Pbck1DecreeFiles)
                    {
                        if (item != null && validDoc)
                        {
                            var filenamecheck = item.FileName;

                            if (filenamecheck.Contains("\\"))
                            {
                                filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }

                            var decreeDoc = new Pbck1DecreeDocModel()
                            {
                                FILE_NAME = filenamecheck,
                                FILE_PATH = SaveUploadedFile(item, model.Detail.Pbck1Id),
                                CREATED_BY = currentUserId.USER_ID,
                                CREATED_DATE = DateTime.Now
                            };
                            model.Detail.Pbck1DecreeDoc.Add(decreeDoc);
                        }
                        else
                        {
                            validDoc = false;
                        }
                    }
                }

                if (!validDoc && model.Pbck1OldDecreeFilesID == null)
                {
                    AddMessageInfo("Please upload the decree doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Edit", "Pbck1", new { id = model.Detail.Pbck1Id });
                }

                foreach (var item in oldDoc)
                {
                    if ((model.Pbck1OldDecreeFilesID != null && !model.Pbck1OldDecreeFilesID.Contains(item)) || model.Pbck1OldDecreeFilesID == null)
                    {
                        _pbck1DecreeDocBll.RemoveDoc(item);
                    }
                }

                var input = new Pbck1UpdateReportedOn()
                {
                    Id = model.Detail.Pbck1Id,
                    ReportedOn = model.Detail.ReportedOn
                };

                _pbck1Bll.UpdateReportedOn(input);

                Pbck1WorkflowCompletedEdit(model.Detail, model.Detail.GovApprovalActionType, model.Detail.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Edit", "Pbck1", new { id = model.Detail.Pbck1Id });
            AddMessageInfo("Document " + EnumHelper.GetDescription(model.Detail.StatusGov), Enums.MessageInfoType.Success);
            return RedirectToAction("CompletedDocument");
        }

        public ActionResult GovRejectDocument(Pbck1ItemViewModel model)
        {
            bool isSuccess = false;
            try
            {
                Pbck1Workflow(model.Detail.Pbck1Id, Enums.ActionType.GovReject, model.Detail.Comment);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess)
            {
                return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            }
            AddMessageInfo("Success GovReject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        private string SaveUploadedFile(HttpPostedFileBase file, int pbck1Id)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            //initialize folders in case deleted by an test publish profile
            if (!Directory.Exists(Server.MapPath(Constans.Pbck1DecreeDocFolderPath)))
                Directory.CreateDirectory(Server.MapPath(Constans.Pbck1DecreeDocFolderPath));

            sFileName = Constans.Pbck1DecreeDocFolderPath + Path.GetFileName(pbck1Id.ToString("'ID'-##") + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        #endregion

        #region ---------- Summary Report ---------------

        public ActionResult SummaryReports()
        {
            Pbck1SummaryReportViewModel model;
            try
            {

                model = new Pbck1SummaryReportViewModel
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    SearchView =
                    {
                        CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll),
                        YearFromList = GetYearListPbck1(true),
                        YearToList = GetYearListPbck1(false),
                        NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll)
                    },
                    //view all data pbck1 completed document
                    DetailsList = SearchSummaryReports().OrderBy(c => c.NppbkcId).ToList()
                };
                foreach (var item in model.DetailsList)
                {
                    item.PoaList = _poaBll.GetPoaByNppbkcIdAndMainPlant(item.NppbkcId).Select(c => c.PRINTED_NAME).ToList();
                }
                model.SearchView.pbck1NumberList = new SelectList(model.DetailsList.Select(c => c.Pbck1Number).AsEnumerable());
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck1SummaryReportViewModel
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
            }

            return View("SummaryReports", model);
        }

        private List<Pbck1SummaryReportsItem> SearchSummaryReports(Pbck1FilterSummaryReportViewModel filter = null)
        {
            //Get All
            if (filter == null)
            {
                //Get All
                var pbck1Data = _pbck1Bll.GetSummaryReportByParam(new Pbck1GetSummaryReportByParamInput());
                foreach (var item in pbck1Data)
                {
                    var Kppbc = _lfa1Bll.GetById(item.NppbkcKppbcId);
                    var PoaList = _poaBll.GetPoaByNppbkcIdAndMainPlant(item.NppbkcId);
                    item.PoaList = PoaList.Select(c => c.PRINTED_NAME).ToList();
                    item.NppbkcKppbcName = Kppbc == null ? "" : Kppbc.NAME1;
                }
                return Mapper.Map<List<Pbck1SummaryReportsItem>>(pbck1Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetSummaryReportByParamInput>(filter);
            var dbData = _pbck1Bll.GetSummaryReportByParam(input);
            foreach (var item in dbData)
            {
                var Kppbc = _lfa1Bll.GetById(item.NppbkcKppbcId);
                var PoaList = _poaBll.GetPoaByNppbkcIdAndMainPlant(item.NppbkcId);
                item.PoaList = PoaList.Select(c => c.PRINTED_NAME).ToList();
                item.NppbkcKppbcName = Kppbc == null ? "" : Kppbc.NAME1;
            }
            return Mapper.Map<List<Pbck1SummaryReportsItem>>(dbData);
        }

        private SelectList GetYearListPbck1(bool isFrom)
        {
            var pbck1List = _pbck1Bll.GetAllByParam(new Pbck1GetByParamInput());

            IEnumerable<SelectItemModel> query;
            if (isFrom)
                query = from x in pbck1List.OrderBy(c => c.PeriodFrom)
                        select new SelectItemModel()
                        {
                            ValueField = x.PeriodFrom.Year,
                            TextField = x.PeriodFrom.ToString("yyyy")
                        };
            else
                query = from x in pbck1List.Where(c => c.PeriodTo.HasValue).OrderBy(c => c.PeriodFrom)
                        select new SelectItemModel()
                        {
                            // ReSharper disable once PossibleInvalidOperationException
                            ValueField = x.PeriodTo.Value.Year,
                            TextField = x.PeriodTo.Value.ToString("yyyy")
                        };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        [HttpPost]
        public PartialViewResult SearchSummaryReports(Pbck1SummaryReportViewModel model)
        {
            model.DetailsList = SearchSummaryReports(model.SearchView).OrderBy(c => c.NppbkcId).ToList(); ;
            return PartialView("_Pbck1SummaryReportTable", model);
        }

        [HttpPost]
        public ActionResult ExportSummaryReports(Pbck1SummaryReportViewModel model)
        {
            try
            {
                ExportSummaryReportsToExcel(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("SummaryReports");
        }

        public void ExportSummaryReportsToExcel(Pbck1SummaryReportViewModel model)
        {
            model.SearchView.CompanyCode = model.ExportModel.CompanyCode;
            model.SearchView.YearFrom = model.ExportModel.YearFrom;
            model.SearchView.YearTo = model.ExportModel.YearTo;
            model.SearchView.NppbkcId = model.ExportModel.NppbkcId;
            model.SearchView.pbck1Number = model.ExportModel.pbck1NumberCode;
            var dataSummaryReport = SearchSummaryReports(model.SearchView);

            var exportModel = Mapper.Map<List<ExportSummaryDataModel>>(dataSummaryReport);

            foreach (var item in dataSummaryReport)
            {
                
            }

            var grid = new System.Web.UI.WebControls.GridView
            {
                DataSource = exportModel.OrderBy(c => c.Nppbkc).ToList(),
                AutoGenerateColumns = false
            };
            if (model.ExportModel.Company)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Company",
                    HeaderText = "Original Company"
                });
            }
            if (model.ExportModel.Nppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Nppbkc",
                    HeaderText = "Original NPPBKC"
                });
            }
            if (model.ExportModel.Address)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Address",
                    HeaderText = "Original Address",
                    HtmlEncode = false
                });
            }
            if (model.ExportModel.Kppbc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Kppbc",
                    HeaderText = "Original KPPBC"
                });
            }
            if (model.ExportModel.Pbck1Number)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1Number",
                    HeaderText = "PBCK-1 Number"
                });
            }

            if (model.ExportModel.OriginalNppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "OriginalNppbkc",
                    HeaderText = "Supplier NPPBKC"
                });
            }
            if (model.ExportModel.OriginalKppbc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "OriginalKppbc",
                    HeaderText = "Supplier KPPBC"
                });
            }
            if (model.ExportModel.OriginalAddress)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "OriginalAddress",
                    HeaderText = "Supplier Address"
                });
            }
            if (model.ExportModel.ExcGoodsAmount)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ExcGoodsAmount",
                    HeaderText = "ExcGoodsAmount"
                });
            }
            if (model.ExportModel.Status)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Status",
                    HeaderText = "Status"
                });
            }
            if (model.ExportModel.Pbck1Type)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1Type",
                    HeaderText = "Pbck1Type"
                });
            }
            if (model.ExportModel.SupplierPortName)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierPortName",
                    HeaderText = "SupplierPortName"
                });
            }
            if (model.ExportModel.SupplierPlant)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierPlant",
                    HeaderText = "SupplierPlant"
                });
            }
            if (model.ExportModel.GoodTypeDesc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "GoodTypeDesc",
                    HeaderText = "GoodTypeDesc"
                });
            }
            if (model.ExportModel.PlanProdFrom)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PlanProdFrom",
                    HeaderText = "PlanProdFrom"
                });
            }
            if (model.ExportModel.PlanProdTo)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PlanProdTo",
                    HeaderText = "PlanProdTo"
                });
            }
            if (model.ExportModel.SupplierPhone)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierPhone",
                    HeaderText = "SupplierPhone"
                });
            }
            if (model.ExportModel.PoaList)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PoaList",
                    HeaderText = "PoaList",
                    HtmlEncode = false
                });
            }
            if (model.ExportModel.Reference)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Reference",
                    HeaderText = "Reference"
                });
            }
            if (model.ExportModel.LACKFrom)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "LACKFrom",
                    HeaderText = "LACKFrom"
                });
            }
            if (model.ExportModel.LACKTo)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "LACKTo",
                    HeaderText = "LACKTo"
                });
            }
            if (model.ExportModel.LatestSaldo)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "LatestSaldo",
                    HeaderText = "LatestSaldo"
                });
            }
            if (model.ExportModel.PeriodFrom)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PeriodFrom",
                    HeaderText = "PeriodFrom"
                });
            }
            if (model.ExportModel.PeriodTo)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PeriodTo",
                    HeaderText = "PeriodTo"
                });
            }
            if (model.ExportModel.ReportedOn)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ReportedOn",
                    HeaderText = "ReportedOn"
                });
            }
            if (model.ExportModel.PeriodFrom)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PeriodFrom",
                    HeaderText = "PeriodFrom"
                });
            }
            if (model.ExportModel.RequestQty)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "RequestQty",
                    HeaderText = "RequestQty"
                });
            }
            if (model.ExportModel.StatusGov)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "StatusGov",
                    HeaderText = "StatusGov"
                });
            }
            if (model.ExportModel.QtyApproved)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "QtyApproved",
                    HeaderText = "QtyApproved"
                });
            }
            if (model.ExportModel.DecreeDate)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "DecreeDate",
                    HeaderText = "DecreeDate"
                });
            }
            if (model.ExportModel.IsNppbkcImport)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "IsNppbkcImport",
                    HeaderText = "IsNppbkcImport"
                });
            }
            if (model.ExportModel.SupplierCompany)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierCompany",
                    HeaderText = "SupplierCompany"
                });
            }
            if (model.ExportModel.ApprovedByPoaId)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ApprovedByPoaId",
                    HeaderText = "POA Approve"
                });
            }
            if (model.ExportModel.ApprovedByManagerId)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ApprovedByManagerId",
                    HeaderText = "Manager Approve"
                });
            }
            if (model.ExportModel.LatestSaldoUomName)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "LatestSaldoUomName",
                    HeaderText = "LatestSaldoUomName"
                });
            }
            if (model.ExportModel.RequestQtyUomName)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "RequestQtyUomName",
                    HeaderText = "RequestQtyUomName"
                });
            }
            if (model.ExportModel.DocNumberCk5)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "DocNumberCk5",
                    HeaderText = "Doc Number CK-5",
                    HtmlEncode = false
                });
            }
            if (model.ExportModel.StatusDoc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "StatusDocCk5",
                    HeaderText = "Status Doc CK-5",
                    HtmlEncode = false
                });
            }
            if (model.ExportModel.GrandTotalExciseable)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "GrandTotalExcisableCk5",
                    HeaderText = "Grand Total Exciseable",
                    HtmlEncode = false
                });
            }

            if (exportModel.Count == 0)
            {
                grid.ShowHeaderWhenEmpty = true;
            }

            grid.DataBind();

            var fileName = "PBCK1" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        #region Monitoring Usage

        public ActionResult MonitoringUsage()
        {
            Pbck1MonitoringUsageViewModel model;
            try
            {

                model = new Pbck1MonitoringUsageViewModel
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    SearchView =
                    {
                        CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll),
                        YearFromList = GetYearListPbck1(true),
                        YearToList = GetYearListPbck1(false),
                        NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll)
                    },
                    DetailsList = SearchMonitoringUsages().OrderBy(c => c.NppbkcId).ToList()
                };
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck1MonitoringUsageViewModel
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
            }

            return View("MonitoringUsage", model);
        }

        private List<Pbck1MonitoringUsageItem> SearchMonitoringUsages(Pbck1FilterMonitoringUsageViewModel filter = null)
        {
            //Get All
            if (filter == null)
            {
                //Get All
                var pbck1Data = _pbck1Bll.GetMonitoringUsageByParam(new Pbck1GetMonitoringUsageByParamInput());
                foreach (var item in pbck1Data)
                {
                    var Kppbc = _lfa1Bll.GetById(item.NppbkcKppbcId);
                    item.NppbkcKppbcName = Kppbc == null ? "" : Kppbc.NAME1;
                }
                var a = Mapper.Map<List<Pbck1MonitoringUsageItem>>(pbck1Data);
                return a;
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetMonitoringUsageByParamInput>(filter);
            var dbData = _pbck1Bll.GetMonitoringUsageByParam(input);
            foreach (var item in dbData)
            {
                var Kppbc = _lfa1Bll.GetById(item.NppbkcKppbcId);
                item.NppbkcKppbcName = Kppbc == null ? "" : Kppbc.NAME1;
            }
            return Mapper.Map<List<Pbck1MonitoringUsageItem>>(dbData);
        }

        [HttpPost]
        public PartialViewResult SearchMonitoringUsage(Pbck1MonitoringUsageViewModel model)
        {
            var pbck1List = _pbck1Bll.GetAllByParam(new Pbck1GetByParamInput());
            model.DetailsList = SearchMonitoringUsages(model.SearchView).OrderBy(c => c.NppbkcId).ToList();
            return PartialView("_Pbck1MonitoringUsageTable", model);
        }

        [HttpPost]
        public ActionResult ExportMonitoringUsage(Pbck1MonitoringUsageViewModel model)
        {
            try
            {
                ExportMonitoringUsageToExcel(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("MonitoringUsage");
        }

        public void ExportMonitoringUsageToExcel(Pbck1MonitoringUsageViewModel model)
        {
            var pbck1List = _pbck1Bll.GetAllByParam(new Pbck1GetByParamInput());

            model.SearchView.CompanyCode = model.ExportModel.CompanyCode;
            model.SearchView.YearFrom = model.ExportModel.YearFrom;
            model.SearchView.YearTo = model.ExportModel.YearTo;
            model.SearchView.NppbkcId = model.ExportModel.NppbkcId;

            var dataToExport = SearchMonitoringUsages(model.SearchView);

            var grid = new GridView
            {
                DataSource = dataToExport.OrderBy(c => c.NppbkcId).ToList(),
                AutoGenerateColumns = false
            };

            if (model.ExportModel.Pbck1Decree)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1Number",
                    HeaderText = "Pbck-1 Decree"
                });
            }

            if (model.ExportModel.Nppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "NppbkcId",
                    HeaderText = "Nppbkc"
                });
            }
            if (model.ExportModel.Company)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "NppbkcCompanyName",
                    HeaderText = "Company"
                });
            }
            if (model.ExportModel.Kppbc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "NppbkcKppbcName",
                    HeaderText = "Kppbc"
                });
            }
            if (model.ExportModel.Pbck1Period)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1PeriodDisplay",
                    HeaderText = "Pbck-1 Period"
                });
            }
            if (model.ExportModel.ExcGoodsQuota)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ExGoodsQuota",
                    HeaderText = "Excisable Goods Quota"
                });
            }
            if (model.ExportModel.AdditionalExcGoodsQuota)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "AdditionalExGoodsQuota",
                    HeaderText = "Additional Excisable Goods Quota"
                });
            }
            if (model.ExportModel.AdditionalExcGoodsQuota)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PreviousFinalBalance",
                    HeaderText = "Prev Years Final Balance"
                });
            }
            if (model.ExportModel.TotalPbck1Quota)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "TotalPbck1Quota",
                    HeaderText = "Total Pbck-1 Quota"
                });
            }
            if (model.ExportModel.Received)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Received",
                    HeaderText = "Received"
                });
            }
            if (model.ExportModel.QuotaRemaining)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "QuotaRemaining",
                    HeaderText = "Quota Remaining"
                });
            }

            if (dataToExport.Count == 0)
            {
                grid.ShowHeaderWhenEmpty = true;
            }

            grid.DataBind();


            var fileName = "PBCK1MonitoringUsage" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        [HttpPost]
        public JsonResult GetNPPBKCListByCompanyID(string companyId)
        {
            if (String.IsNullOrEmpty(companyId))
            {
                //GET All NPPBKC
                var NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll).ToList();
                return Json(NppbkcIdList.Select(c => c.Text).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var NppbkcIdList = _t001kBll.GetNPPBKCIDByCompany(companyId);
                return Json(NppbkcIdList, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region ------------- Print Out -----------

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            //Get Report Source
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var pbck1Data = _pbck1Bll.GetPrintOutDataById(id.Value);
            if (pbck1Data == null)
                HttpNotFound();

            Stream stream = GetReport(pbck1Data, "PBCK-1");

            return File(stream, "application/pdf");
        }

        [EncryptedParameter]
        public ActionResult PrintPreview(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var pbck1Data = _pbck1Bll.GetPrintOutDataById(id.Value);
            if (pbck1Data == null)
                HttpNotFound();

            Stream stream = GetReport(pbck1Data, "Preview PBCK-1");

            return File(stream, "application/pdf");
        }

        private Stream GetReport(Pbck1ReportDto pbck1Data, string printTitle)
        {
            var dataSet = SetDataSetReport(pbck1Data, printTitle);

            ReportClass rpt = new ReportClass
            {
                FileName = ConfigurationManager.AppSettings["Report_Path"] + "PBCK1\\PBCK1PrintOut.rpt"
            };
            rpt.Load();
            rpt.SetDataSource(dataSet);
            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rpt.Close();
            return stream;
        }

        private DataSet SetDataSetReport(Pbck1ReportDto pbck1ReportData, string printTitle)
        {
            var dsPbck1 = new dsPbck1();
            dsPbck1 = AddDataPbck1Row(dsPbck1, pbck1ReportData.Detail, printTitle);
            dsPbck1 = AddDataPbck1ProdPlan(dsPbck1, pbck1ReportData.Detail.ExcisableGoodsDescription, pbck1ReportData);
            dsPbck1 = AddDataPbck1BrandRegistration(dsPbck1, pbck1ReportData.BrandRegistrationList);
            dsPbck1 = AddDataRealisasiP3Bkc(dsPbck1, pbck1ReportData, pbck1ReportData.SummaryRealisasiP3Bkc);
            //dsPbck1 = FakeDataRealisasiP3Bkc(dsPbck1);
            dsPbck1 = AddDataHeaderFooter(dsPbck1, pbck1ReportData.HeaderFooter);
            return dsPbck1;
        }

        private dsPbck1 AddDataRealisasiP3Bkc(dsPbck1 ds, Pbck1ReportDto reportDto, List<Pbck1SummaryRealisasiProductionDetailDto> summaryData)
        {
            var data = reportDto.RealisasiP3Bkc;
            //var convertedUomId = reportDto.Detail.ConvertedUomId;
            var realisasiUomId = reportDto.Detail.RealisasiUomId;
            var bkcExcisableGoodsTypeDesc = reportDto.Detail.RealisasiBkcExcisableGoodsTypeDesc;
            var sumPemasukan = 0m;
            var sumPenggunaan = 0m;
            if (data != null && data.Count > 0)
            {
                var summaryJenis = string.Join(Environment.NewLine, summaryData.Select(d => d.ProductAlias));
                var summaryTotal = string.Join(Environment.NewLine, summaryData.Select(d => String.Format("{0:n}", d.Total)));
                var dt = data.FirstOrDefault(c => !string.IsNullOrEmpty(c.Lack1UomId));
                var uomId = string.Empty;
                if (dt != null)
                {
                    uomId = dt.Lack1UomId;
                }


                var UomKG = "kg";
                
                var visibilityUomPemasukan = "l"; //code : l (liter), k (kg) regarding to converted uom id
                var visibilityUomPenggunaan = "l"; //code : l (liter), k (kg) regarding to converted uom id
                var visibilityUomBkc = "l"; //code : l (liter), k (kg), b (batang) //from Excisable Goods Type on Brand Registration by Prod_Code in Lack1 Production Data
                decimal conversion;
                decimal conversionBkc = 0;

                if (realisasiUomId.ToLower() == "g" || realisasiUomId.ToLower() == "kg")
                {
                    conversion = (decimal)0.001;
                    visibilityUomPemasukan = "k";
                    visibilityUomPenggunaan = "k";
                }
                else
                {
                    conversion = 1;
                }

                if (bkcExcisableGoodsTypeDesc.ToLower().Contains("hasil tembakau"))
                {
                    visibilityUomBkc = "b";//Batang
                    conversionBkc = 1;
                }
                else if (bkcExcisableGoodsTypeDesc.ToLower().Contains("tembakau iris"))
                {
                    visibilityUomBkc = "k";//Kilogram
                    if (reportDto.Detail.RealisasiBkcUomId.ToLower() == "g")
                    {
                        conversionBkc = (decimal)0.001;
                    }
                    else
                    {
                        conversionBkc = 1;
                    }
                }
                else if (bkcExcisableGoodsTypeDesc.ToLower().Contains("alkohol"))
                {
                    conversionBkc = 1;
                    visibilityUomBkc = "l";//Liter
                }
                var month = "";
                decimal? latestSaldo = null;
                var latestUomId = string.Empty;
                foreach (var item in data)
                {
                    if (month != item.Bulan) { 
                        sumPemasukan += item.Pemasukan == null ? 0 : (conversion*item.Pemasukan.Value);
                        sumPenggunaan += item.Penggunaan == null ? 0 : (conversion*item.Penggunaan.Value);
                    }
                    month = item.Bulan;
                    if (item.ProductionList.Count > 0)
                    {
                        foreach (var prod in item.ProductionList)
                        {

                            var saldoAwalDisplay = "-";
                            decimal saldoAwal = 0;
                            var pemasukanDisplay = "-";
                            decimal pemasukan = 0;
                            var penggunaanDisplay = "-";
                            decimal penggunaan = 0;
                            var saldoAkhirDisplay = "-";
                            decimal saldoAkhir = 0;
                            var jumlahDisplay = "-";
                            decimal jumlah = 0;

                            var detailRow = ds.RealisasiP3BKC.NewRealisasiP3BKCRow();
                            detailRow.Bulan = item.Bulan;
                            detailRow.No = item.BulanId.ToString(CultureInfo.InvariantCulture);

                            detailRow.Jenis = prod.ProductAlias;
                            detailRow.Uom = uomId.ToLower() == "g" ? UomKG : uomId;
                            detailRow.UomBKC = prod.UomId;
                            latestUomId = detailRow.Uom;
                            detailRow.UomTotal = uomId.ToLower() == "g" ? UomKG : uomId;
                            detailRow.UomSaldoAwal = latestUomId;
                            if (item.SaldoAwal.HasValue)
                            {
                                saldoAwal = conversion * item.SaldoAwal.Value;
                                saldoAwalDisplay = saldoAwal.ToString("N2");
                            }
                            if (item.Pemasukan.HasValue)
                            {
                                pemasukan = conversion * item.Pemasukan.Value;
                                pemasukanDisplay = pemasukan.ToString("N2");
                            }
                            if (item.Penggunaan.HasValue)
                            {
                                penggunaan = conversion * item.Penggunaan.Value;
                                penggunaanDisplay = penggunaan.ToString("N2");
                            }
                            if (prod.Amount.HasValue)
                            {
                                jumlah = conversionBkc * prod.Amount.Value;
                                jumlahDisplay = jumlah.ToString("N2");
                            }
                            item.SaldoAkhir = saldoAwal + pemasukan - penggunaan;
                            saldoAkhir = item.SaldoAkhir.Value;
                            saldoAkhirDisplay = saldoAkhir.ToString("N2");
                            latestSaldo = saldoAkhir;

                            detailRow.PemasukanDisplay = pemasukanDisplay;
                            detailRow.Pemasukan = pemasukan;
                            detailRow.SaldoAwalDisplay = saldoAwalDisplay;
                            detailRow.SaldoAwal = saldoAwal;
                            detailRow.PenggunaanDisplay = penggunaanDisplay;
                            detailRow.Penggunaan = penggunaan;
                            detailRow.JumlahDisplay = jumlahDisplay;
                            detailRow.Jumlah = jumlah;
                            detailRow.SaldoAkhir = saldoAkhir;
                            detailRow.SaldoAkhirDisplay = saldoAkhirDisplay;
                            detailRow.VisibilityUomJumlahBkc = visibilityUomBkc;
                            detailRow.VisibilityUomPemasukan = visibilityUomPemasukan;
                            detailRow.VisibilityUomPenggunaan = visibilityUomPenggunaan;
                            detailRow.SummaryJenis = summaryJenis;
                            detailRow.SummaryJumlah = summaryTotal;
                            detailRow.SumAllPemasukan = String.Format("{0:n}",sumPemasukan);
                            detailRow.SumAllPenggunaan = String.Format("{0:n}",sumPenggunaan);
                            ds.RealisasiP3BKC.AddRealisasiP3BKCRow(detailRow);
                        }
                    }
                    else
                    {
                        //Add empty row
                        var detailRow = ds.RealisasiP3BKC.NewRealisasiP3BKCRow();
                        detailRow.Bulan = item.Bulan;
                        detailRow.No = item.BulanId.ToString(CultureInfo.InvariantCulture);

                        detailRow.Jenis = "";
                        detailRow.UomSaldoAwal = item.SaldoAwal.HasValue && latestSaldo.HasValue ? latestUomId : string.Empty;
                        detailRow.UomBKC = "";
                        detailRow.Uom = string.Empty;

                        detailRow.PemasukanDisplay = "";
                        detailRow.Pemasukan = 0;
                        detailRow.SaldoAwalDisplay = item.SaldoAwal.HasValue && latestSaldo.HasValue ? latestSaldo.Value.ToString("N2") : "";
                        detailRow.SaldoAwal = 0;
                        detailRow.PenggunaanDisplay = "";
                        detailRow.Penggunaan = 0;
                        detailRow.JumlahDisplay = "";
                        detailRow.Jumlah = 0;
                        detailRow.SaldoAkhir = 0;
                        detailRow.SaldoAkhirDisplay = "";
                        detailRow.VisibilityUomJumlahBkc = visibilityUomBkc;
                        detailRow.VisibilityUomPemasukan = visibilityUomPemasukan;
                        detailRow.VisibilityUomPenggunaan = visibilityUomPenggunaan;
                        detailRow.SummaryJenis = summaryJenis;
                        detailRow.SummaryJumlah = summaryTotal;
                        detailRow.SumAllPemasukan = String.Format("{0:n}", sumPemasukan);
                        detailRow.SumAllPenggunaan = String.Format("{0:n}", sumPenggunaan);
                        detailRow.UomTotal = uomId.ToLower() == "g" ? UomKG : uomId;
                        ds.RealisasiP3BKC.AddRealisasiP3BKCRow(detailRow);
                    }
                }
            }
            else
            {
                //Add empty row
                var detailRow = ds.RealisasiP3BKC.NewRealisasiP3BKCRow();
                detailRow.Bulan = "";
                detailRow.No = "";
                detailRow.SaldoAwal = 0;
                detailRow.SaldoAwalDisplay = "";
                detailRow.Pemasukan = 0;
                detailRow.PemasukanDisplay = "";
                detailRow.Penggunaan = 0;
                detailRow.PenggunaanDisplay = "";
                detailRow.Jenis = "";
                detailRow.Jumlah = 0;
                detailRow.JumlahDisplay = "";
                detailRow.SaldoAkhir = 0;
                detailRow.SaldoAkhirDisplay = "";
                detailRow.Uom = "";
                detailRow.UomBKC = "";
                detailRow.VisibilityUomJumlahBkc = "l";
                detailRow.VisibilityUomPemasukan = "l";
                detailRow.VisibilityUomPenggunaan = "l";
                detailRow.SummaryJenis = "";
                detailRow.SummaryJumlah = "";
                detailRow.SumAllPemasukan = String.Format("{0:n}", sumPemasukan);
                detailRow.SumAllPenggunaan = String.Format("{0:n}", sumPenggunaan);
                ds.RealisasiP3BKC.AddRealisasiP3BKCRow(detailRow);
            }
            return ds;
        }

        private dsPbck1 AddDataPbck1Row(dsPbck1 ds, Pbck1ReportInformationDto d, string printTitle)
        {
            var detailRow = ds.Pbck1.NewPbck1Row();
            detailRow.Pbck1Id = d.Pbck1Id.ToString();
            detailRow.Pbck1Number = d.Pbck1Number;
            detailRow.Pbck1AdditionalText = d.Pbck1AdditionalText;
            detailRow.Year = d.Year;
            detailRow.VendorAliasName = d.VendorAliasName;
            detailRow.VendorCityName = d.VendorCityName;
            detailRow.PoaName = d.PoaName;
            detailRow.PoaTitle = d.PoaTitle;
            detailRow.CompanyName = d.CompanyName;
            detailRow.NppbkcId = d.NppbkcId;
            detailRow.NppbkcAddress = d.NppbkcAddress;
            detailRow.PlantPhoneNumber = d.PlantPhoneNumber;
            detailRow.ProdConverterProductType = d.ProdConverterProductType;
            detailRow.ExcisableGoodsDescription = d.ExcisableGoodsDescription;
            detailRow.PeriodFrom = d.PeriodFrom;
            detailRow.PeriodTo = d.PeriodTo;
            detailRow.ProductConvertedOutputs = d.ProductConvertedOutputs;
            detailRow.RequestQty = d.RequestQty;
            detailRow.RequestQtyUom = d.RequestQtyUom;
            detailRow.RequestQtyUomName = d.RequestQtyUomName;
            detailRow.LatestSaldo = d.LatestSaldo;
            detailRow.LatestSaldoUom = d.LatestSaldoUom;
            detailRow.SupplierCompanyName = d.SupplierCompanyName;
            detailRow.SupplierNppbkcId = d.SupplierNppbkcId;
            detailRow.SupplierPlantAddress = d.SupplierPlantAddress;
            detailRow.SupplierPlantPhone = d.SupplierPlantPhone;
            detailRow.SupplierKppbcId = d.SupplierKppbcId;
            detailRow.SupplierKppbcMengetahui = d.SupplierKppbcMengetahui;
            detailRow.SupplierPortName = d.SupplierPortName;
            detailRow.NppbkcCity = d.NppbkcCity;
            detailRow.PrintedDate = d.PrintedDate;
            detailRow.ExciseManager = d.ExciseManager;
            detailRow.ProdPlanPeriod = d.ProdPlanPeriode;
            detailRow.LackPeriod = d.Lack1Periode;
            detailRow.DocumentText = printTitle;
            detailRow.PoaAddress = d.PoaAddress;
            detailRow.SupplierPlantId = d.SupplierPlantId;
            detailRow.TipeMadya = d.TipeMadya;
            ds.Pbck1.AddPbck1Row(detailRow);
            return ds;
        }

        private dsPbck1 AddDataPbck1BrandRegistration(dsPbck1 ds, List<Pbck1ReportBrandRegistrationDto> brandData)
        {
            if (brandData != null && brandData.Count > 0)
            {
                int no = 1;
                foreach (var item in brandData)
                {
                    var detailRow = ds.Pbck1BrandRegistration.NewPbck1BrandRegistrationRow();
                    detailRow.Type = item.Type;
                    detailRow.Brand = item.Brand;
                    detailRow.Kadar = item.Kadar;
                    detailRow.Convertion = item.Convertion;
                    detailRow.ConvertionUom = item.ConvertionUom;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    detailRow.No = no.ToString();
                    detailRow.ConvertionUomId = item.ConvertionUomId;
                    ds.Pbck1BrandRegistration.AddPbck1BrandRegistrationRow(detailRow);
                    no++;
                }
            }
            else
            {
                var detailRow = ds.Pbck1BrandRegistration.NewPbck1BrandRegistrationRow();
                detailRow.Type = "";
                detailRow.Brand = " ";
                detailRow.Kadar = " ";
                detailRow.Convertion = " ";
                detailRow.ConvertionUom = " ";
                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                detailRow.No = "";
                ds.Pbck1BrandRegistration.AddPbck1BrandRegistrationRow(detailRow);
            }
            return ds;
        }

        private dsPbck1 AddDataPbck1ProdPlan(dsPbck1 ds, string excisableGoodsType, Pbck1ReportDto reportData)
        {
            var prodPlan = reportData.ProdPlanList;
            var summary = reportData.SummaryProdPlantList;

            if (prodPlan != null && prodPlan.Count > 0)
            {
                var visibilityUomAmount = "l";
                var uomAmount = "Kilogram";
                var visibilityUomBkc = "k";
                var uomBkc = "Kilogram";
                var uomBkcId = "Kg";
                decimal conversiBkc = 1m;
                if (reportData.Detail.ConvertedUomId.ToLower() == "btg")
                {
                    uomAmount = "Batang";
                    visibilityUomAmount = "b";
                }
                else if (reportData.Detail.ConvertedUomId.ToLower() == "l")
                {
                    uomAmount = "Liter";
                    visibilityUomAmount = "l";
                }
                else if (reportData.Detail.ConvertedUomId.ToLower() == "g" || reportData.Detail.ConvertedUomId.ToLower() == "kg")
                {
                    uomAmount = "Kg";
                    visibilityUomAmount = "k";
                }

                var summaryUomBkc = string.Empty;
                var firstDataBkc = prodPlan.FirstOrDefault(c => !string.IsNullOrEmpty(c.BkcRequiredUomId));
                if (firstDataBkc != null)
                {

                    if (firstDataBkc.BkcRequiredUomId.ToLower() == "l")
                    {
                        visibilityUomBkc = "l";
                        uomBkc = firstDataBkc.BkcRequiredUomName;
                        uomBkcId = firstDataBkc.BkcRequiredUomId;
                    }
                    else if (firstDataBkc.BkcRequiredUomId.ToLower() == "g")
                    {
                        conversiBkc = (1m / 1000m);
                        visibilityUomBkc = "k";
                        uomBkc = "kg";
                        uomBkcId = "Kg";
                    }
                    else if (firstDataBkc.BkcRequiredUomId.ToLower() == "kg")
                    {
                        uomBkc = "kg";
                        uomBkcId = "Kg";
                    }


                }

                var summaryJenis = summary.Select(c => c.ProdAlias).Distinct().ToList();
                //var summaryJenisNewLine = string.Join(Environment.NewLine, summary.Select(d => d.ProdAlias).Distinct().ToList());
                var summaryAmount = string.Join(Environment.NewLine,
                    summary.Select(d => d.TotalAmount.ToString("N2")).ToList());

                var summaryBkc = string.Join(Environment.NewLine,
                    summary.Select(d => d.TotalBkc.ToString("N2")).ToList());

                // Set Total Jumlah Produksi dan Kebutuhan Bkc
                var SummaryJenisAmount = new Dictionary<string, decimal>();
                var SummaryBkcRequired = new Dictionary<string, decimal>();

                foreach (var prodAlias in summaryJenis)
                {
                    SummaryJenisAmount.Add(prodAlias, prodPlan.Where(c => c.ProdAlias == prodAlias && c.Amount != null).Select(c => c.Amount.Value).Sum());
                    SummaryBkcRequired.Add(prodAlias, prodPlan.Where(c => c.ProdAlias == prodAlias && c.BkcRequired != null).Select(c => c.BkcRequired.Value).Sum());
                }

                //set total jenis bkc
                var summaryJenisNewLine = String.Join(Environment.NewLine, SummaryJenisAmount.Select(c => c.Key));

                //set total jumlah produksi
                List<string> amountSummary = new List<string>();
                foreach (var item in SummaryJenisAmount.Select(c => c.Value))
                {
                    var sumAmount = item;
                    if (reportData.Detail.ConvertedUomId.ToLower() == "g") sumAmount = item / 1000;
                    amountSummary.Add(String.Format("{0:n}", sumAmount));
                }
                var totalAmountNewLine = String.Join(Environment.NewLine, amountSummary);

                //set kebutuhan bkc
                List<string> bckSummary = new List<string>();
                foreach (var item in SummaryBkcRequired.Select(c => c.Value))
                {
                    bckSummary.Add(String.Format("{0:n}", conversiBkc * item));
                }
                var totalBkcSummaryNewLine = String.Join(Environment.NewLine, bckSummary);

                //set satuan total jumlah produksi
                var summaryUomAmount = string.Join(Environment.NewLine, summary.Select(d => uomAmount).Take(SummaryJenisAmount.Keys.Count()));

                //set satuan kebutuhan bkc
                summaryUomBkc = string.Join(Environment.NewLine, summary.Select(d => uomBkcId).Take(SummaryJenisAmount.Keys.Count()));
                if (summaryUomBkc.Contains("L"))
                    summaryUomBkc = summaryUomBkc.Replace("L", "Liter");

                foreach (var item in prodPlan)
                {
                    var detailRow = ds.Pbck1ProdPlan.NewPbck1ProdPlanRow();

                    detailRow.ProdTypeCode = item.ProdTypeCode;
                    detailRow.ProdTypeName = item.ProdTypeName;
                    detailRow.ProdAlias = item.ProdAlias;
                    detailRow.AmountDecimal = 0;
                    if (item.Amount.HasValue)
                    {
                        var amountPlan = item.Amount.Value;
                        if (reportData.Detail.ConvertedUomId.ToLower() == "g") amountPlan = amountPlan / 1000;
                        detailRow.AmountDecimal = amountPlan;
                    }
                    detailRow.BkcRequired = 0;
                    if (item.BkcRequired.HasValue)
                    {
                        detailRow.BkcRequired = conversiBkc * item.BkcRequired.Value;
                    }

                    detailRow.SummaryAmount = totalAmountNewLine;
                    detailRow.BkcRequiredUomId = uomBkcId;
                    detailRow.BkcRequiredUomName = uomBkc;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    detailRow.MonthId = item.MonthId;
                    detailRow.MonthName = item.MonthName;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    detailRow.No = item.MonthId.ToString();

                    detailRow.VisibilityUomAmount = visibilityUomAmount;
                    detailRow.UomAmount = uomAmount;
                    detailRow.VisibilityUomBkc = visibilityUomBkc;

                    detailRow.SummaryBkcRequired = totalBkcSummaryNewLine;
                    detailRow.SummaryJenis = summaryJenisNewLine;
                    detailRow.SummaryUomAmount = summaryUomAmount;
                    detailRow.SummaryUomBkc = summaryUomBkc;
                    ds.Pbck1ProdPlan.AddPbck1ProdPlanRow(detailRow);

                }
            }
            else
            {
                var detailRow = ds.Pbck1ProdPlan.NewPbck1ProdPlanRow();

                detailRow.ProdTypeCode = "";
                detailRow.ProdTypeName = "";
                detailRow.ProdAlias = "";
                //detailRow.Amount = "";
                detailRow.BkcRequired = 0;
                detailRow.AmountDecimal = 0;
                detailRow.BkcRequiredUomId = "";
                detailRow.BkcRequiredUomName = "";
                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                detailRow.MonthId = 0;
                detailRow.MonthName = "";
                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                detailRow.No = "";
                ds.Pbck1ProdPlan.AddPbck1ProdPlanRow(detailRow);
            }
            return ds;
        }

        private dsPbck1 AddDataHeaderFooter(dsPbck1 ds, HEADER_FOOTER_MAPDto headerFooter)
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
                        // initialise the binary reader from file streamobject 
                        br = new BinaryReader(fs);
                        // define the byte array of filelength 
                        byte[] imgbyte = new byte[fs.Length + 1];
                        // read the bytes from the binary reader 
                        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
                        dRow.HeaderImage = imgbyte;
                    }

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

        #endregion

        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var pbck1Data = _pbck1Bll.GetById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.PBCK1,
                FORM_ID = pbck1Data.Pbck1Id,
                FORM_NUMBER = pbck1Data.Pbck1Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(pbck1Data.Pbck1Number));
            return PartialView("_PrintHistoryTable", model);

        }

        [HttpPost]
        public JsonResult GetPBCK1Reference(DateTime periodFrom, DateTime periodTo, string nppbkcId, string supplierNppbkcId, string supplierPlantWerks, string supplierPlant, string goodType)
        {
            var reference = _pbck1Bll.GetPBCK1Reference(new Pbck1ReferenceSearchInput() { NppbkcId = nppbkcId, PeriodFrom = periodFrom, PeriodTo = periodTo, SupllierNppbkcId = supplierNppbkcId, SupplierPlantWerks = supplierPlantWerks, SupplierPlant = supplierPlant, GoodTypeId = goodType });
            if (reference == null)
            {
                return Json(false);
            }
            else
            {
                return Json(new { referenceId = reference.Pbck1Id, refereceNumber = reference.Pbck1Number });
            }
        }

        [HttpPost]
        public JsonResult GetKPPBCByNPPBKC(string nppbkcid)
        {
            var nppbkc = _nppbkcbll.GetDetailsById(nppbkcid);
            if (nppbkc == null)
            {
                return Json(new { kppbcid = (String)null, kppbcname = (String)null });
            }
            else
            {
                var lfa = _lfa1Bll.GetById(nppbkc.KPPBC_ID);
                return Json(new { kppbcid = nppbkc.KPPBC_ID, kppbcname = lfa.NAME1 });
            }
        }

        #region ------ CK5 details ----

        public ActionResult CK5Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            var pbck1Data = _pbck1Bll.GetById(id.Value);
            var ck5Data = _ck5Bll.GetCk5ByPBCK1(id.Value);
            if (pbck1Data == null)
            {
                return HttpNotFound();
            }

            var model = new PBCK1ListCK5Model()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<Pbck1Item>(pbck1Data),
                CK5List = Mapper.Map<List<CK5Item>>(ck5Data)
            };

            return View(model);
        }

        #endregion

        #region Monitoring Mutasi

        public ActionResult MonitoringMutasi()
        {
            
            var model = InitMonitoringMutasi(new Pbck1MonitoringMutasiViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo
            });
          
            return View(model);
        }

        private Pbck1MonitoringMutasiViewModel InitMonitoringMutasi(Pbck1MonitoringMutasiViewModel model)
        {
            var monitoringDtos = _pbck1Bll.GetMonitoringMutasiByParam(new Pbck1GetMonitoringMutasiByParamInput());
            model.pbck1NumberList = new SelectList(monitoringDtos, "Pbck1Number", "Pbck1Number");
            
            var input = Mapper.Map<Pbck1GetMonitoringMutasiByParamInput>(model);

            var dbData = _pbck1Bll.GetMonitoringMutasiByParam(input);
            model.DetailsList = Mapper.Map<List<Pbck1MonitoringMutasiItem>>(dbData);

            return model;

        }

        [HttpPost]
        public PartialViewResult FilterMutasiIndex(Pbck1MonitoringMutasiViewModel model)
        {
            var input = Mapper.Map<Pbck1GetMonitoringMutasiByParamInput>(model);
         

            var dbData = _pbck1Bll.GetMonitoringMutasiByParam(input);
            var result = Mapper.Map<List<Pbck1MonitoringMutasiItem>>(dbData);
            var viewModel = new Pbck1MonitoringMutasiViewModel ();
            viewModel.DetailsList = result;
            return PartialView("_MutasiList", viewModel);
        }

        [HttpPost]
        public ActionResult ExportMonitoringMutasi(Pbck1ExportMonitoringMutasiViewModel model)
        {
            try
            {
                ExportMonitoringMutasiToExcel(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("MonitoringMutasi");
        }

        public void ExportMonitoringMutasiToExcel(Pbck1ExportMonitoringMutasiViewModel model)
        {
           
            var dbResult = _pbck1Bll.GetMonitoringMutasiByParam(new Pbck1GetMonitoringMutasiByParamInput
            {
                pbck1Number = model.FilterPbck1Number
            });

            //todo refactor mapper
            var mutasiItem = Mapper.Map<List<Pbck1MonitoringMutasiItem>>(dbResult);

            var dataToExport = Mapper.Map<List<ExportMonitoringMutasiDataModel>>(mutasiItem);

            var grid = new GridView
            {
                DataSource = dataToExport,
                AutoGenerateColumns = false
            };

            if (model.Pbck1Number)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1Number",
                    HeaderText = "PBCK-1 Number"
                });
            }
            if (model.TotalPbck1Quota)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "TotalPbck1Quota",
                    HeaderText = "Total PBCK-1 Quota"
                });
            }
            if (model.QuotaRemaining)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "QuotaRemaining",
                    HeaderText = "PBCK-1 Quota remaining"
                });
            
           
            } if (model.Received)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Received",
                    HeaderText = "Total CK-5 Used"
                });
            }
           
            if (model.DocNumberCk5)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "DocNumberCk5",
                    HeaderText = "CK-5 Number",
                    HtmlEncode = false
                });
            }
            if (model.GrandTotalExciseable)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "GrandTotalExciseable",
                    HeaderText = "Total Ex Good Type",
                    HtmlEncode = false
                });
            }
            if (model.UoM)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "UoM",
                    HeaderText = "UoM",
                    HtmlEncode = false
                });
            }
            

            if (dataToExport.Count == 0)
            {
                grid.ShowHeaderWhenEmpty = true;
            }

            grid.DataBind();


            var fileName = "PBCK1MonitoringMutasi" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        #region Dashboard
        public ActionResult Dashboard()
        {
            var data = InitDashboardModel(new Pbck1DashboardModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                YearList = Pbck1DashboardYear(),
                PoaList = GlobalFunctions.GetPoaAll(_poabll),
                UserList = GlobalFunctions.GetCreatorList()
            });

            return View("Dashboard", data);
        }

        private Pbck1DashboardModel InitDashboardModel(
            Pbck1DashboardModel model)
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

        private List<Pbck1Dto> GetAllDocument(Pbck1DashboardModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var ck4cData = _pbck1Bll.GetAllByParam(new Pbck1GetByParamInput());
                return ck4cData;
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetByParamInput>(filter);
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;

            var dbData = _pbck1Bll.GetAllByParam(input);
            return dbData;
        }

        [HttpPost]
        public PartialViewResult FilterDashboardPage(Pbck1DashboardModel model)
        {
            var data = InitDashboardModel(model);

            return PartialView("_ChartStatus", data.Detil);
        }

        #endregion
    }
}