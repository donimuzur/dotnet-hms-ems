using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
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
using SpreadsheetLight;


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

        public void ExportClientsListToExcel(int id)
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

                if (!ValidateEditDocument(model))
                {
                    return RedirectToAction("Index");
                }

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

                //set null, set this field only from Gov Approval
                input.Pbck1.DecreeDate = null;
                input.Pbck1.QtyApproved = null;
                input.Pbck1.StatusGov = null;
                input.Pbck1.Pbck1DecreeDoc = null;

                var saveResult = _pbck1Bll.Save(input);

                if (saveResult.Success)
                {
                    //return RedirectToAction("Index");
                    return RedirectToAction("Edit", new { id = model.Detail.Pbck1Id });
                }

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
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
                dataToSave.GoodTypeDesc = !string.IsNullOrEmpty(dataToSave.GoodTypeDesc) ? dataToSave.GoodTypeDesc.Split('-')[1] : string.Empty;

                var input = new Pbck1SaveInput()
                {
                    Pbck1 = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Created
                };

                //only add this information from gov approval,
                //when save create/edit 
                input.Pbck1.DecreeDate = null;
                input.Pbck1.QtyApproved = null;
                input.Pbck1.StatusGov = null;
                input.Pbck1.Pbck1DecreeDoc = null;

                var saveResult = _pbck1Bll.Save(input);

                if (saveResult.Success)
                {
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
                    DecreeDate = pbck1Data.DecreeDate,
                    QtyApproved = pbck1Data.QtyApproved,
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
        public ActionResult GovApproveDocument(Pbck1ItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            }

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
                            var decreeDoc = new Pbck1DecreeDocModel()
                            {
                                FILE_NAME = item.FileName,
                                FILE_PATH = SaveUploadedFile(item, model.Detail.Pbck1Id),
                                CREATED_BY = currentUserId.USER_ID,
                                CREATED_DATE = DateTime.Now
                            };
                            model.Detail.Pbck1DecreeDoc.Add(decreeDoc);
                        }
                    }
                }
                Pbck1WorkflowGovApprove(model.Detail, model.Detail.GovApprovalActionType, "");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "Pbck1", new { id = model.Detail.Pbck1Id });
            AddMessageInfo("Success Gov Approve Document", Enums.MessageInfoType.Success);
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
                        CompanyCodeList = GlobalFunctions.GetCompanyList(),
                        YearFromList = GetYearListPbck1(true),
                        YearToList = GetYearListPbck1(false),
                        NppbkcIdList = GlobalFunctions.GetNppbkcAll()
                    },
                    //view all data pbck1 completed document
                    DetailsList = SearchSummaryReports()
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
                return Mapper.Map<List<Pbck1SummaryReportsItem>>(pbck1Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetSummaryReportByParamInput>(filter);
            var dbData = _pbck1Bll.GetSummaryReportByParam(input);
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
                    Kppbc = "'" + d.NppbkcKppbcId,
                    Pbck1Number = "'" + d.Pbck1Number,
                    Address = string.Join("<br />", d.NppbkcPlants.Select(c => c.ADDRESS).ToArray()),
                    OriginalNppbkc = "'" + d.SupplierNppbkcId,
                    OriginalKppbc = "'" + d.SupplierKppbcId,
                    OriginalAddress = d.SupplierAddress,
                    // ReSharper disable once PossibleInvalidOperationException
                    ExcGoodsAmount = d.QtyApproved.Value.ToString("N0"),
                    Status = d.StatusName
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
                        CompanyCodeList = GlobalFunctions.GetCompanyList(),
                        YearFromList = GetYearListPbck1(true),
                        YearToList = GetYearListPbck1(false),
                        NppbkcIdList = GlobalFunctions.GetNppbkcAll()
                    },
                    DetailsList = SearchMonitoringUsages()
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
                return Mapper.Map<List<Pbck1MonitoringUsageItem>>(pbck1Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetMonitoringUsageByParamInput>(filter);
            var dbData = _pbck1Bll.GetMonitoringUsageByParam(input);
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

            //todo: to automapper
            var src = (from d in dataToExport
                       select new ExportSummaryDataModel()
                       {
                           Company = d.NppbkcCompanyName,
                           Nppbkc = "'" + d.NppbkcId,
                           Kppbc = "'" + d.NppbkcKppbcId,
                           Pbck1Number = "'" + d.Pbck1Number
                       }).ToList();

            var grid = new GridView
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
    }
}