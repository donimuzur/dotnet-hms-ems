using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;


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

        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll, IPlantBLL plantBll, IChangesHistoryBLL changesHistoryBll, IWorkflowHistoryBLL workflowHistoryBll, IWorkflowBLL workflowBll)
            : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
            _plantBll = plantBll;
            _mainMenu = Enums.MenuList.PBCK1;
            _changesHistoryBll = changesHistoryBll;
            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
        }

        private List<Pbck1Item> GetOpenDocument(Pbck1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var pbck1Data = _pbck1Bll.GetOpenDocumentByParam(new Pbck1GetOpenDocumentByParamInput());
                return Mapper.Map<List<Pbck1Item>>(pbck1Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetOpenDocumentByParamInput>(filter);
            var dbData = _pbck1Bll.GetOpenDocumentByParam(input);
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

        private SelectList CreateYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            for (int i = 0; i < 5; i++)
            {
                years.Add(new SelectItemModel() { ValueField = currentYear - i, TextField = (currentYear - i).ToString() });
            }
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
                CurrentUser = CurrentUser
            });
            return View("Index", model);
        }

        public Pbck1ViewModel InitPbck1ViewModel(Pbck1ViewModel model)
        {
            model.SearchInput.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
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

        public ActionResult Edit(long? id)
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

                if (!ValidateEditDocument(model))
                {
                    return RedirectToAction("Index");
                }

                var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, id.Value.ToString()));

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(new GetByFormTypeAndFormIdInput()
                {
                    FormId = id.Value,
                    FormType = Enums.FormType.PBCK1
                }));

                model.WorkflowHistory = workflowHistory;
                model.ChangesHistoryList = changeHistory;

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
            var isAllowEditDocument = _workflowBll.AllowEditDocument(new WorkflowAllowEditAndSubmitInput()
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

            if (model.Detail.Status != Enums.DocumentStatus.Draft)
            {
                //can't edit
                AddMessageInfo(
                    "Can't modify document with status " + EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApproval),
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
                    AddMessageInfo("Model error", Enums.MessageInfoType.Error);
                    return View(ModelInitial(model));
                }

                if (!ValidateEditDocument(model))
                {
                    return View(ModelInitial(model));
                }
                model.Detail.Status = Enums.DocumentStatus.Revised;
                model = CleanSupplierInfo(model);

                //process save
                var dataToSave = Mapper.Map<Pbck1Dto>(model.Detail);
                var input = new Pbck1SaveInput()
                {
                    Pbck1 = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Modified
                };
                var saveResult = _pbck1Bll.Save(input);

                if (saveResult.Success)
                {
                    return RedirectToAction("Index");
                }

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }

            var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, model.Detail.Pbck1Id.ToString()));

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(new GetByFormTypeAndFormIdInput()
            {
                FormId = model.Detail.Pbck1Id,
                FormType = Enums.FormType.PBCK1
            }));

            model.WorkflowHistory = workflowHistory;
            model.ChangesHistoryList = changeHistory;

            return View(ModelInitial(model));

        }

        #endregion

        #region ------ details ----

        public ActionResult Details(long? id)
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

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(new GetByFormTypeAndFormIdInput()
            {
                FormId = id.Value,
                FormType = Enums.FormType.PBCK1
            }));

            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1,
                    id.Value.ToString()));

            var model = new Pbck1ItemViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<Pbck1Item>(pbck1Data),
                ChangesHistoryList = changesHistory,
                WorkflowHistory = workflowHistory
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
                CurrentUserGroup = CurrentUser.USER_GROUP_ID
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
            }

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
                    AddMessageInfo("Model Error", Enums.MessageInfoType.Error);
                    return CreateInitial(model);
                }

                model = CleanSupplierInfo(model);

                //process save
                var dataToSave = Mapper.Map<Pbck1Dto>(model.Detail);
                dataToSave.CreatedById = CurrentUser.USER_ID;

                var input = new Pbck1SaveInput()
                {
                    Pbck1 = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Created
                };

                var saveResult = _pbck1Bll.Save(input);

                if (saveResult.Success)
                {
                    return RedirectToAction("Edit", new { id = saveResult.Id });
                }

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

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var model = new Pbck1ViewModel { SearchInput = { PoaList = listPoa } };
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
                        uploadItem.ProductCode = datarow[0];
                        uploadItem.ConverterOutput = datarow[1];
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
            model.NppbkcList = GlobalFunctions.GetNppbkcAll();
            model.MonthList = GlobalFunctions.GetMonthList();
            model.SupplierPortList = GlobalFunctions.GetSupplierPortList();
            model.SupplierPlantList = GlobalFunctions.GetSupplierPlantList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();
            model.UomList = GlobalFunctions.GetUomList();

            var pbck1RefList = GetCompletedDocument();

            if (model.Detail != null && model.Detail.Pbck1Reference.HasValue)
            {
                //exclude current pbck1 document on list
                pbck1RefList = pbck1RefList.Where(c => c.Pbck1Id != model.Detail.Pbck1Reference.Value).ToList();
            }

            model.PbckReferenceList = new SelectList(pbck1RefList, "Pbck1Id", "Pbck1Number");

            model.YearList = CreateYearList();
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
        public JsonResult GetSupplierPlant()
        {
            return Json(GlobalFunctions.GetSupplierPlantList());
        }

        [HttpPost]
        public JsonResult GetNppbkcDetail(string nppbkcid)
        {
            var data = GlobalFunctions.GetNppbkcById(nppbkcid);
            return Json(Mapper.Map<CompanyDetail>(data.T001));
        }

        [HttpPost]
        public JsonResult GetSupplierPlantDetail(string plantid)
        {
            var data = _plantBll.GetId(plantid);
            return Json(Mapper.Map<DetailPlantT1001W>(data));
        }

        public void ExportClientsListToExcel(long id)
        {

            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, id.ToString());

            var model = Mapper.Map<List<ChangesHistoryItemModel>>(listHistory);

            var grid = new System.Web.UI.WebControls.GridView
            {
                DataSource = from d in model
                             select new
                             {
                                 Date = d.MODIFIED_DATE.HasValue ? d.MODIFIED_DATE.Value.ToString("dd MMM yyyy") : string.Empty,
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

        private void Pbck1Workflow(long id, Enums.ActionType actionType, string comment)
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

        public ActionResult SubmitDocument(long? id)
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

        public ActionResult ApproveDocument(long id)
        {
            try
            {
                Pbck1Workflow(id, Enums.ActionType.Approve, string.Empty);
                AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "Pbck1", new { id });
        }

        public ActionResult RejectDocument(Pbck1ItemViewModel model)
        {
            try
            {
                Pbck1Workflow(model.Detail.Pbck1Id, Enums.ActionType.Reject, model.Detail.Comment);
                AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
        }

        public ActionResult GovApproveDocument(long id)
        {
            try
            {
                Pbck1Workflow(id, Enums.ActionType.GovApprove, "");
                AddMessageInfo("Success Gov Approve Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "Pbck1", new { id });
        }

        public ActionResult GovRejectDocument(Pbck1ItemViewModel model)
        {
            try
            {
                Pbck1Workflow(model.Detail.Pbck1Id, Enums.ActionType.GovReject, model.Detail.Comment);
                AddMessageInfo("Success GovReject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
        }

        public ActionResult GovPartialApproveDocument(Pbck1ItemViewModel model)
        {
            try
            {
                Pbck1Workflow(model.Detail.Pbck1Id, Enums.ActionType.GovPartialApprove, model.Detail.Comment);
                AddMessageInfo("Success GovPartialApprove Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
        }

        #endregion


    }
}