using System;
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
      
        public PBCK1Controller(IPageBLL pageBLL, IUnitOfMeasurementBLL uomBll, ICompanyBLL companyBll, IMasterDataBLL masterDataBll, IMonthBLL monthbll, IZaidmExGoodTypeBLL goodTypeBll, ISupplierPortBLL supplierPortBll, IZaidmExNPPBKCBLL nppbkcbll, IPBCK1BLL pbckBll, IPlantBLL plantBll, IChangesHistoryBLL changesHistoryBll,
            IWorkflowHistoryBLL workflowHistoryBll, IWorkflowBLL workflowBll, IPrintHistoryBLL printHistoryBll, IPOABLL poaBll, ILACK1BLL lackBll, ILFA1BLL lfa1Bll)
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
        
        private SelectList LackYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear + 1, TextField = (currentYear + 1).ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
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
            return PartialView("_Pbck1Table", model);
        }

        [HttpPost]
        public PartialViewResult UploadFileConversion(HttpPostedFileBase prodConvExcelFile)
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
                        var text = datarow[1];
                        decimal value;
                        if (Decimal.TryParse(text, out value))
                        {
                            //text = Math.Round(Convert.ToDecimal(text), 4).ToString();
                            text = Convert.ToDecimal(text).ToString();
                        }

                        uploadItem.ProductCode = datarow[0];
                        uploadItem.ConverterOutput = text;
                        uploadItem.ConverterUom = datarow[2];

                        model.Detail.Pbck1ProdConverter.Add(uploadItem);

                    }
                    catch (Exception)
                    {
                        continue;

                    }
                }
            }

            var input = Mapper.Map<List<Pbck1ProdConverterInput>>(model.Detail.Pbck1ProdConverter);
            var outputResult = _pbck1Bll.ValidatePbck1ProdConverterUpload(input);

            model.Detail.Pbck1ProdConverter = Mapper.Map<List<Pbck1ProdConvModel>>(outputResult);

            return PartialView("_ProdConvList", model);
        }

        [HttpPost]
        public PartialViewResult UploadFilePlan(HttpPostedFileBase prodPlanExcelFile)
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

            var input = Mapper.Map<List<Pbck1ProdPlanInput>>(model.Detail.Pbck1ProdPlan);
            var outputResult = _pbck1Bll.ValidatePbck1ProdPlanUpload(input);

            model.Detail.Pbck1ProdPlan = Mapper.Map<List<Pbck1ProdPlanModel>>(outputResult);

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
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.UomList = GlobalFunctions.GetUomList(_uomBll);

            var pbck1RefList = GetCompletedDocument();

            if (model.Detail != null && model.Detail.Pbck1Reference.HasValue)
            {
                //exclude current pbck1 document on list
                pbck1RefList = pbck1RefList.Where(c => c.Pbck1Id != model.Detail.Pbck1Reference.Value).ToList();
            }

            model.PbckReferenceList = new SelectList(pbck1RefList, "Pbck1Id", "Pbck1Number");

            //model.YearList = CreateYearList();
            model.YearList = LackYearList();

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
                IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager
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

            var model = new Pbck1ItemViewModel();
            model = ModelInitial(model);

            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //redirect to details for approval/rejected
                return RedirectToAction("Details", new { id });
            }

            try
            {
                model.Detail = Mapper.Map<Pbck1Item>(pbck1Data);

                var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, id.Value.ToString()));

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = pbck1Data.Pbck1Number;
                workflowInput.DocumentStatus = pbck1Data.Status;
                workflowInput.NPPBKC_Id = pbck1Data.NppbkcId;

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                model.WorkflowHistory = workflowHistory;
                model.ChangesHistoryList = changeHistory;

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
                    NppbkcId = model.Detail.NppbkcId
                };

                ////workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;

                if (!allowApproveAndReject)
                {
                    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                }

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Detail.Status);

                if(model.Detail.Status == Enums.DocumentStatus.WaitingGovApproval)
                {
                    model.ActionType = "GovApproveDocument";
                }

                if ((model.ActionType == "GovApproveDocument" && model.AllowGovApproveAndReject) )
                { 
                
                }else if (!ValidateEditDocument(model))
                {
                    return RedirectToAction("Index");
                }

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        private bool ValidateEditDocument(Pbck1ItemViewModel model)
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
                AddMessageInfo(
                    "Operation not allowed.",
                    Enums.MessageInfoType.Error);
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
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();

                    if (errors.Count > 0)
                    {
                        //get error details
                    }

                    AddMessageInfo("Model error", Enums.MessageInfoType.Error);
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
                    AddMessageInfo("PBCK-1 dengan no " + checkUnique + " sudah ada", Enums.MessageInfoType.Error);
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
            workflowInput.NPPBKC_Id = model.Detail.NppbkcId;

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
            workflowInput.NPPBKC_Id = model.Detail.NppbkcId;

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

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormNumber = pbck1Data.Pbck1Number;
            workflowInput.DocumentStatus = pbck1Data.Status;
            workflowInput.NPPBKC_Id = pbck1Data.NppbkcId;

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
                NppbkcId = model.Detail.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }
            else if(CurrentUser.UserRole == Enums.UserRole.POA){
                model.AllowApproveAndReject = false;
                foreach (POADto poa in _poaBll.GetPoaByNppbkcIdAndMainPlant(model.Detail.NppbkcId))
                { 
                    if(poa.POA_ID == CurrentUser.USER_ID){
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
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create PBCK1 Document
                AddMessageInfo("Can't create PBCK-1 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
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
                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Cannot save PBCK-1. Please fill all the mandatory fields", Enums.MessageInfoType.Error);
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
                    AddMessageInfo("PBCK-1 dengan no " + checkUnique +" sudah ada", Enums.MessageInfoType.Error);
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
                }
            });
            return View("CompletedDocument", model);
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(Pbck1ViewModel model)
        {
            model.Details = GetCompletedDocument(model.SearchInput);
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
            model.DetailsList = SearchSummaryReports(model.SearchView);
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
            var dataSummaryReport = SearchSummaryReports(model.SearchView);

            //todo: to automapper
            var src = (from d in dataSummaryReport
                select new ExportSummaryDataModel()
                {
                    Company = d.NppbkcCompanyName,
                    Nppbkc = "'" + d.NppbkcId,
                    Kppbc = d.NppbkcKppbcName,
                    Pbck1Number = "'" + d.Pbck1Number,
                    Address = string.Join("<br />", d.NppbkcPlants.Select(c => c.ADDRESS).ToArray()),
                    OriginalNppbkc = "'" + d.SupplierNppbkcId,
                    OriginalKppbc = "'" + d.SupplierKppbcName,
                    OriginalAddress = d.SupplierAddress,
                    // ReSharper disable once PossibleInvalidOperationException
                    ExcGoodsAmount =  d.QtyApproved == null ? "0" : d.QtyApproved.Value.ToString("N0"),
                    Status = d.StatusName,
                    Pbck1Type = d.Pbck1Type.ToString(),
                    SupplierPortName =  d.SupplierPortName,
                    SupplierPlant = d.SupplierPlant,
                    GoodTypeDesc = d.GoodTypeDesc,
                    PlanProdFrom = d.PlanProdFrom.Value.ToString(),
                    PlanProdTo = d.PlanProdTo.Value.ToString(),
                    SupplierPhone = d.SupplierPhone
                }).ToList();

            var grid = new System.Web.UI.WebControls.GridView
            {
                DataSource = src,
                AutoGenerateColumns = false
            };

            if (model.ExportModel.Nppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Nppbkc",
                    HeaderText = "Nppbkc"
                });
            }
            if (model.ExportModel.Company)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Company",
                    HeaderText = "Company"
                });
            }
            if (model.ExportModel.Kppbc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Kppbc",
                    HeaderText = "Kppbc"
                });
            }
            if (model.ExportModel.Pbck1Number)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1Number",
                    HeaderText = "Pbck1Number"
                });
            }
            if (model.ExportModel.Address)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Address",
                    HeaderText = "Address", 
                    HtmlEncode = false
                });
            }
            if (model.ExportModel.OriginalNppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "OriginalNppbkc",
                    HeaderText = "OriginalNppbkc"
                });
            }
            if (model.ExportModel.OriginalKppbc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "OriginalKppbc",
                    HeaderText = "OriginalKppbc"
                });
            }
            if (model.ExportModel.OriginalAddress)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "OriginalAddress",
                    HeaderText = "OriginalAddress"
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
            model.DetailsList = SearchMonitoringUsages(model.SearchView);
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
            var dataToExport = SearchMonitoringUsages(model.SearchView);
            
            var grid = new GridView
            {
                DataSource = dataToExport,
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
            return stream;
        }
        
        private DataSet SetDataSetReport(Pbck1ReportDto pbck1ReportData, string printTitle)
        {
            var dsPbck1 = new dsPbck1();
            dsPbck1 = AddDataPbck1Row(dsPbck1, pbck1ReportData.Detail, printTitle);
            dsPbck1 = AddDataPbck1ProdPlan(dsPbck1, pbck1ReportData.Detail.ExcisableGoodsDescription, pbck1ReportData.ProdPlanList);
            dsPbck1 = AddDataPbck1BrandRegistration(dsPbck1, pbck1ReportData.BrandRegistrationList);
            //dsPbck1 = AddDataRealisasiP3Bkc(dsPbck1, pbck1ReportData.RealisasiP3Bkc);
            dsPbck1 = FakeDataRealisasiP3Bkc(dsPbck1);
            dsPbck1 = AddDataHeaderFooter(dsPbck1, pbck1ReportData.HeaderFooter);
            return dsPbck1;
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

        private dsPbck1 AddDataPbck1ProdPlan(dsPbck1 ds, string excisableGoodsType, List<Pbck1ReportProdPlanDto> prodPlan)
        {
            if (prodPlan != null && prodPlan.Count > 0)
            {
                int no = 1;

                var visibilityUomAmount = "l";
                var uomAmount = "Kilogram";
                var visibilityUomBkc = "k";
                if (excisableGoodsType.ToLower().Contains("hasil tembakau"))
                {
                    visibilityUomAmount = "b"; //strikeout except "Batang" / "batang"
                    uomAmount = "Batang";
                }
                else if (excisableGoodsType.ToLower().Contains("tembakau iris"))
                {
                    visibilityUomAmount = "k"; //strikeout except "Kilogram" / "kilogram"
                    uomAmount = "Kilogram";
                }
                else if (excisableGoodsType.ToLower().Contains("alkohol"))
                {
                    uomAmount = "Liter";
                    visibilityUomAmount = "l";
                }
                
                foreach (var item in prodPlan)
                {
                    var detailRow = ds.Pbck1ProdPlan.NewPbck1ProdPlanRow();

                    detailRow.ProdTypeCode = item.ProdTypeCode;
                    detailRow.ProdTypeName = item.ProdTypeName;
                    detailRow.ProdAlias = item.ProdAlias;
                    detailRow.AmountDecimal = 0;
                    if (item.Amount != null)
                    {
                        detailRow.AmountDecimal = item.Amount.Value;
                    }
                    detailRow.BkcRequired = 0;
                    if (item.BkcRequired != null)
                    {
                        detailRow.BkcRequired = item.BkcRequired.Value;
                    }
                    detailRow.BkcRequiredUomId = item.BkcRequiredUomId;
                    detailRow.BkcRequiredUomName = item.BkcRequiredUomName;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    detailRow.MonthId = item.MonthId;
                    detailRow.MonthName = item.MonthName;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    detailRow.No = no.ToString();

                    detailRow.VisibilityUomAmount = visibilityUomAmount;
                    detailRow.UomAmount = uomAmount;
                    detailRow.VisibilityUomBkc = visibilityUomBkc;
                    
                    ds.Pbck1ProdPlan.AddPbck1ProdPlanRow(detailRow);
                    no++;
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

        private dsPbck1 AddDataRealisasiP3Bkc(dsPbck1 ds, List<Pbck1RealisasiP3BkcDto> realisasiP3Bkc)
        {
            if (realisasiP3Bkc != null && realisasiP3Bkc.Count > 0)
            {
                decimal totalPemasukan = 0;
                decimal totalPenggunaan = 0;
                decimal totalAmount = 0;

                foreach (var item in realisasiP3Bkc)
                {
                    var detailRow = ds.RealisasiP3BKC.NewRealisasiP3BKCRow();
                    detailRow.Bulan = item.Bulan;
                    detailRow.SaldoAwal = item.SaldoAwal;
                    detailRow.Pemasukan = item.Pemasukan;
                    detailRow.Penggunaan = item.Penggunaan;
                    detailRow.Jenis = item.Jenis;
                    detailRow.Jumlah = item.Jumlah;
                    detailRow.SaldoAkhir = item.SaldoAkhir;
                    detailRow.Uom = item.Lack1UomId;
                    ds.RealisasiP3BKC.AddRealisasiP3BKCRow(detailRow);

                }
            }
            else
            {
                var detailRow = ds.RealisasiP3BKC.NewRealisasiP3BKCRow();
                detailRow.Bulan = "";
                detailRow.SaldoAwal = 0;
                detailRow.Pemasukan = 0;
                detailRow.Penggunaan = 0;
                detailRow.Jenis = "";
                detailRow.Jumlah = 0;
                detailRow.SaldoAkhir = 0;
                detailRow.Uom = "";
                ds.RealisasiP3BKC.AddRealisasiP3BKCRow(detailRow);
            }
            return ds;
        }

        private dsPbck1 FakeDataRealisasiP3Bkc(dsPbck1 ds)
        {
            
            for (int i = 0; i < 12; i++)
            {
                var detailRow = ds.RealisasiP3BKC.NewRealisasiP3BKCRow();
                detailRow.Bulan = "Januari";
                detailRow.SaldoAwal = 20000;
                detailRow.Pemasukan = 10000;
                detailRow.Penggunaan = 10000;
                detailRow.Jenis = "SKT";
                detailRow.Jumlah = 10000;
                detailRow.SaldoAkhir = 20000;
                detailRow.Uom = "Kg";
                detailRow.UomBKC = "Batang";
                detailRow.No = (i + 1).ToString(CultureInfo.InvariantCulture);
                ds.RealisasiP3BKC.AddRealisasiP3BKCRow(detailRow);
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
        public JsonResult GetPBCK1Reference(DateTime periodFrom, DateTime periodTo, string nppbkcId, string supplierNppbkcId,string supplierPlantWerks,string goodType)
        {
            var reference = _pbck1Bll.GetPBCK1Reference(new Pbck1ReferenceSearchInput() { NppbkcId = nppbkcId, PeriodFrom = periodFrom, PeriodTo = periodTo, SupllierNppbkcId = supplierNppbkcId, SupplierPlantWerks = supplierPlantWerks, GoodTypeId = goodType });
            if (reference == null)
            {
                return Json(false);
            }else{
                return Json(new { referenceId = reference.Pbck1Id, refereceNumber = reference.Pbck1Number});
            }
        }

        [HttpPost]
        public JsonResult GetKPPBCByNPPBKC(string nppbkcid)
        {
            var nppbkc = _nppbkcbll.GetDetailsById(nppbkcid);
            if (nppbkc == null)
            {
                return Json(new { kppbcid = (String) null, kppbcname = (String) null });
            }
            else
            {
                var lfa = _lfa1Bll.GetById(nppbkc.KPPBC_ID);
                return Json(new { kppbcid = nppbkc.KPPBC_ID, kppbcname = lfa.NAME1 });
            }
        }


    }
}