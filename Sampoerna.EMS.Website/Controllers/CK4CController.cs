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
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System.Configuration;

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
        private IDocumentSequenceNumberBLL _documentSequenceNumberBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IWorkflowBLL _workflowBll;
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IHeaderFooterBLL _headerFooterBll;

        public CK4CController(IPageBLL pageBll, IPOABLL poabll, ICK4CBLL ck4Cbll, IPlantBLL plantbll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll,
            IBrandRegistrationBLL brandRegistrationBll, ICompanyBLL companyBll, IT001KBLL t001Kbll, IZaidmExNPPBKCBLL nppbkcbll, IProductionBLL productionBll,
            IDocumentSequenceNumberBLL documentSequenceNumberBll, IWorkflowHistoryBLL workflowHistoryBll, IWorkflowBLL workflowBll, IZaidmExProdTypeBLL prodTypeBll,
            IHeaderFooterBLL headerFooterBll)
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
            _documentSequenceNumberBll = documentSequenceNumberBll;
            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
            _prodTypeBll = prodTypeBll;
            _headerFooterBll = headerFooterBll;
        }

        #region Index Document List

        public ActionResult DocumentList()
        {
            var data = InitIndexDocumentListViewModel(new Ck4CIndexDocumentListViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.Ck4CDocument,
                IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager
            });

            return View("DocumentList", data);
        }

        private Ck4CIndexDocumentListViewModel InitIndexDocumentListViewModel(
            Ck4CIndexDocumentListViewModel model)
        {
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
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
            var dbData = _ck4CBll.GetOpenDocumentByParam(input).OrderByDescending(c => c.Number);
            return Mapper.Map<List<DataDocumentList>>(dbData);
        }

        private List<DataDocumentList> GetCompletedDocument(Ck4CIndexDocumentListViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var ck4cData = _ck4CBll.GetCompletedDocumentByParam(new Ck4cGetCompletedDocumentByParamInput());
                return Mapper.Map<List<DataDocumentList>>(ck4cData);
            }

            //getbyparams
            var input = Mapper.Map<Ck4cGetCompletedDocumentByParamInput>(filter);
            var dbData = _ck4CBll.GetCompletedDocumentByParam(input);
            return Mapper.Map<List<DataDocumentList>>(dbData);
        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(Ck4CIndexDocumentListViewModel model)
        {
            model.Detail = GetOpenDocument(model);
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
                Ck4CType = Enums.CK4CType.CompletedDocument
            });
            return View("CompletedDocument", data);
        }

        [HttpPost]
        public PartialViewResult FilterCompletedDocument(Ck4CIndexDocumentListViewModel model)
        {
            model.Detail = GetCompletedDocument(model);
            return PartialView("_CK4CTableCompletedDocument", model);
        }

        #endregion

        #region Json
        [HttpPost]
        public JsonResult CompanyListPartialProduction(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var model = new Ck4CIndexViewModel() { PlanIdList = listPlant };

            return Json(model);

        }

        [HttpPost]
        public JsonResult CompanyListPartialCk4CWaste(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var model = new Ck4CIndexWasteProductionViewModel() { PlanIdList = listPlant };

            return Json(model);

        }

        [HttpPost]
        public JsonResult CompanyListPartialCk4CDocument(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var model = new Ck4CIndexDocumentListViewModel() { PlanList = listPlant };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetNppbkcByCompanyId(string companyId)
        {
            return Json(_nppbkcbll.GetNppbkcsByCompany(companyId));
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
        public JsonResult GetProductionData(string comp, string plant, string nppbkc, int period, int month, int year)
        {
            var data = _productionBll.GetByCompPlant(comp, plant, nppbkc, period, month, year).Select(d => Mapper.Map<ProductionDto>(d)).ToList();
            return Json(data);
        }

        #endregion

        #region create Document List
        public ActionResult Ck4CCreateDocumentList()
        {
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
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PeriodList = Ck4cPeriodList();
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = Ck4cYearList();
            model.PlanList = GlobalFunctions.GetPlantAll();
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            if(model.Details != null) model.Details.ReportedOn = DateTime.Now;

            return (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ck4CCreateDocumentList(Ck4CIndexDocumentListViewModel model)
        {
            Ck4CDto item = new Ck4CDto();

            item = AutoMapper.Mapper.Map<Ck4CDto>(model.Details);

            var plant = _plantBll.GetT001WById(model.Details.PlantId);
            var company = _companyBll.GetById(model.Details.CompanyId);
            var nppbkcId = plant == null ? item.NppbkcId : plant.NPPBKC_ID;

            item.PlantName = plant == null ? "" : plant.NAME1;
            item.CompanyName = company.BUTXT;
            item.CreatedBy = CurrentUser.USER_ID;
            item.CreatedDate = DateTime.Now;
            var inputDoc = new GenerateDocNumberInput();
            inputDoc.Month = item.ReportedMonth;
            inputDoc.Year = item.ReportedYears;
            inputDoc.NppbkcId = nppbkcId;
            item.Number = _documentSequenceNumberBll.GenerateNumber(inputDoc);
            item.Status = Enums.DocumentStatus.Draft;

            if(item.Ck4cItem.Count == 0)
            {
                AddMessageInfo("No item found", Enums.MessageInfoType.Warning);
                model = InitialModel(model);
                return View(model);
            }

            _ck4CBll.Save(item);
            AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
            return RedirectToAction("DocumentList");
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

        private List<Ck4cItemData> SetOtherCk4cItemData(List<Ck4cItemData> ck4cItemData)
        {
            List<Ck4cItemData> listData;

            listData = ck4cItemData;

            foreach(var item in listData)
            {
                var brand = _brandRegistrationBll.GetByFaCode(item.Werks, item.FaCode);
                var plant = _plantBll.GetT001WById(item.Werks);
                var prodType = _prodTypeBll.GetByCode(item.ProdCode);

                item.ProdDateName = item.ProdDate.ToString("dd MMM yyyy");
                item.BrandDescription = brand.BRAND_CE;
                item.PlantName = item.Werks + "-" + plant.NAME1;
                item.ProdType = prodType.PRODUCT_TYPE;
            }

            return listData;
        }

        #endregion

        #region Details

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

            var plant = _plantBll.GetT001WById(ck4cData.PlantId);
            var nppbkcId = plant == null ? ck4cData.NppbkcId : plant.NPPBKC_ID;

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormNumber = ck4cData.Number;
            workflowInput.DocumentStatus = ck4cData.Status;
            workflowInput.NPPBKC_Id = nppbkcId;

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            var model = new Ck4CIndexDocumentListViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<DataDocumentList>(ck4cData),
                WorkflowHistory = workflowHistory
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
                NppbkcId = nppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }

            return View(model);
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

            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //redirect to details for approval/rejected
                return RedirectToAction("Details", new { id });
            }

            try
            {
                model.Details = Mapper.Map<DataDocumentList>(ck4cData);

                if (!ValidateEditDocument(model))
                {
                    return RedirectToAction("DocumentList");
                }

                model.Details.Ck4cItemData = SetOtherCk4cItemData(model.Details.Ck4cItemData);

                var plant = _plantBll.GetT001WById(ck4cData.PlantId);
                var nppbkcId = plant == null ? ck4cData.NppbkcId : plant.NPPBKC_ID;

                //workflow history
                var workflowInput = new GetByFormNumberInput();
                workflowInput.FormNumber = ck4cData.Number;
                workflowInput.DocumentStatus = ck4cData.Status;
                workflowInput.NPPBKC_Id = nppbkcId;

                var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

                model.WorkflowHistory = workflowHistory;

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
                    NppbkcId = nppbkcId
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

                var plant = _plantBll.GetT001WById(model.Details.PlantId);
                var company = _companyBll.GetById(model.Details.CompanyId);

                dataToSave.PlantName = plant == null ? "" : plant.NAME1;
                dataToSave.CompanyName = company.BUTXT;
                dataToSave.ModifiedBy = CurrentUser.USER_ID;
                dataToSave.ModifiedDate = DateTime.Now;

                List<Ck4cItem> list = dataToSave.Ck4cItem;
                foreach(var item in list)
                {
                    item.Ck4CId = dataToSave.Ck4CId;
                }

                dataToSave.Ck4cItem = list;

                bool isSubmit = model.Details.IsSaveSubmit == "submit";

                var saveResult = _ck4CBll.Save(dataToSave);

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

        private bool ValidateEditDocument(Ck4CIndexDocumentListViewModel model)
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
            if (model.Details.Ck4cDecreeFiles == null)
            {
                AddMessageInfo("Decree Doc is required.", Enums.MessageInfoType.Error);
                return RedirectToAction("Details", "CK4C", new { id = model.Details.Ck4CId });
            }

            bool isSuccess = false;
            var currentUserId = CurrentUser;
            try
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
                        else
                        {
                            AddMessageInfo("Please upload the decree doc", Enums.MessageInfoType.Error);
                            return RedirectToAction("Details", "CK4C", new { id = model.Details.Ck4CId });
                        }
                    }
                }


                var input = new Ck4cUpdateReportedOn()
                {
                    Id = model.Details.Ck4CId,
                    ReportedOn = model.Details.ReportedOn
                };

                _ck4CBll.UpdateReportedOn(input);

                Ck4cWorkflowGovApprove(model.Details, model.Details.GovApprovalActionType, model.Details.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "CK4C", new { id = model.Details.Ck4CId });
            AddMessageInfo("Document " + EnumHelper.GetDescription(model.Details.StatusGoverment), Enums.MessageInfoType.Success);
            return RedirectToAction("DocumentList");
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

        #endregion

        #region Print Preview

        [EncryptedParameter]
        public FileResult PrintPreview(int id)
        {
            var ck4c = _ck4CBll.GetById(id);

            var dsCk4c = CreateCk4cDs();
            var dt = dsCk4c.Tables[0];
            DataRow drow;
            drow = dt.NewRow();
            drow[0] = ck4c.Ck4CId;
            drow[1] = ck4c.Number;

            if (ck4c.ReportedOn != null)
            {
                var ck4cReportedOn = ck4c.ReportedOn.Value;
                var ck4cMonth = _monthBll.GetMonth(ck4cReportedOn.Month).MONTH_NAME_IND;

                drow[2] = string.Format("{0} {1} {2}", ck4cReportedOn.Day, ck4cMonth, ck4cReportedOn.Year);
                drow[14] = ck4cReportedOn.Day;
                drow[15] = ck4cMonth;
                drow[16] = ck4cReportedOn.Year;
            }

            if(ck4c.ReportedPeriod == 1)
            {
                drow[3] = "1";
                drow[4] = "14";
            }
            else if (ck4c.ReportedPeriod == 2)
            {
                var endDate = new DateTime(ck4c.ReportedYears, ck4c.ReportedMonth, 1).AddMonths(1).AddDays(-1).Day.ToString();
                drow[3] = "15";
                drow[4] = endDate;
            }

            var ck4cPeriodMonth = _monthBll.GetMonth(ck4c.ReportedMonth).MONTH_NAME_IND;
            drow[5] = ck4cPeriodMonth;
            
            drow[6] = ck4c.ReportedYears;
            drow[7] = ck4c.CompanyName;

            var company = _companyBll.GetById(ck4c.CompanyId);
            drow[8] = company.SPRAS;

            var plant = _plantBll.GetT001WById(ck4c.PlantId);
            var nppbkc = plant == null ? ck4c.NppbkcId : plant.NPPBKC_ID;
            drow[9] = nppbkc;

            if (ck4c.ApprovedByPoa != null)
            {
                var poa = _poabll.GetDetailsById(ck4c.ApprovedByPoa);
                if (poa != null)
                {
                    drow[10] = poa.PRINTED_NAME;
                }
            }

            var headerFooter = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput
            {
                CompanyCode = ck4c.CompanyId,
                FormTypeId = Enums.FormType.CK4C
            });
            if (headerFooter != null)
            {
                drow[11] = GetHeader(headerFooter.HEADER_IMAGE_PATH);
                drow[12] = headerFooter.FOOTER_CONTENT;
            }

            if (ck4c.Status != Enums.DocumentStatus.WaitingGovApproval || ck4c.Status != Enums.DocumentStatus.GovApproved
                || ck4c.Status != Enums.DocumentStatus.Completed)
            {
                drow[13] = "PREVIEW CK-4C";
            }
            else
            {
                drow[13] = "CK-4C";
            }
            
            dt.Rows.Add(drow);

            var dtDetail = dsCk4c.Tables[1];
            foreach (var item in ck4c.Ck4cItem)
            {
                DataRow drowDetail;
                drowDetail = dtDetail.NewRow();
                drowDetail[0] = item.Ck4CItemId;
                drowDetail[1] = item.ProdQty;
                drowDetail[2] = item.ProdQtyUom;
                dtDetail.Rows.Add(drowDetail);
            }
            // object of data row 

            ReportClass rpt = new ReportClass();
            string report_path = ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = report_path + "CK4C\\Preview.rpt";
            rpt.Load();
            rpt.SetDataSource(dsCk4c);

            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        private DataSet CreateCk4cDs()
        {
            DataSet ds = new DataSet("dsCk4c");

            DataTable dt = new DataTable("Ck4c");
            dt.Columns.Add("Ck4cId", System.Type.GetType("System.String"));
            dt.Columns.Add("Number", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedOn", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedPeriodStart", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedPeriodEnd", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedMonth", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedYear", System.Type.GetType("System.String"));
            dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
            dt.Columns.Add("CompanyAddress", System.Type.GetType("System.String"));
            dt.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dt.Columns.Add("Poa", System.Type.GetType("System.String"));
            dt.Columns.Add("Header", System.Type.GetType("System.Byte[]"));
            dt.Columns.Add("Footer", System.Type.GetType("System.String"));
            dt.Columns.Add("Preview", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedOnDay", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedOnMonth", System.Type.GetType("System.String"));
            dt.Columns.Add("ReportedOnYear", System.Type.GetType("System.String"));

            //item
            DataTable dtDetail = new DataTable("Ck4cItem");
            dtDetail.Columns.Add("Ck4cItemId", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("ProdQty", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Uom", System.Type.GetType("System.String"));

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
            catch (Exception ex)
            {
            }
            return imgbyte;
            // Return Datatable After Image Row Insertion
        }

        #endregion
    }
}