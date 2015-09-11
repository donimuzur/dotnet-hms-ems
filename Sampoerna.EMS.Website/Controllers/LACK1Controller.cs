using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.LACK1;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using System.IO;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK1Controller : BaseController
    {
        private ILACK1BLL _lack1Bll;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poabll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private ICompanyBLL _companyBll;
        private IPBCK1BLL _pbck1Bll;
        private IPlantBLL _plantBll;
        private IWorkflowBLL _workflowBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IPrintHistoryBLL _printHistoryBll;

        public LACK1Controller(IPageBLL pageBll, IPOABLL poabll, ICompanyBLL companyBll,
            IZaidmExGoodTypeBLL goodTypeBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK1BLL lack1Bll, IMonthBLL monthBll,
            IUnitOfMeasurementBLL uomBll, IPBCK1BLL pbck1Bll, IPlantBLL plantBll, IWorkflowHistoryBLL workflowHistoryBll, IWorkflowBLL workflowBll,
            IChangesHistoryBLL changesHistoryBll, IPrintHistoryBLL printHistoryBll)
            : base(pageBll, Enums.MenuList.LACK1)
        {
            _lack1Bll = lack1Bll;
            _monthBll = monthBll;
            _uomBll = uomBll;
            _mainMenu = Enums.MenuList.LACK1;
            _poabll = poabll;
            _nppbkcbll = nppbkcbll;
            _goodTypeBll = goodTypeBll;
            _companyBll = companyBll;
            _pbck1Bll = pbck1Bll;
            _plantBll = plantBll;

            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
            _changesHistoryBll = changesHistoryBll;
            _printHistoryBll = printHistoryBll;

        }


        #region Index

        //
        // GET: /LACK1/
        public ActionResult Index()
        {
            var data = InitLack1ViewModel(new Lack1IndexViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Type = Enums.LACK1Type.ListByNppbkc,
                Details = Mapper.Map<List<Lack1NppbkcData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput() { Lack1Level = Enums.Lack1Level.Nppbkc, IsOpenDocumentOnly = true }))

            });

            return View("Index", data);
        }

        /// <summary>
        /// for LACK-1 data by NPPBKC Level
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Lack1IndexViewModel InitLack1ViewModel(Lack1IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.CreatorList = GlobalFunctions.GetCreatorList();
            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(Lack1Input model)
        {

            var input = Mapper.Map<Lack1GetByParamInput>(model);
            input.Lack1Level = Enums.Lack1Level.Nppbkc;
            input.IsOpenDocumentOnly = true;

            var dbData = _lack1Bll.GetAllByParam(input);

            var result = Mapper.Map<List<Lack1NppbkcData>>(dbData);

            var viewModel = new Lack1IndexViewModel { Details = result };

            return PartialView("_Lack1Table", viewModel);
        }

        #endregion

        #region Index List By Plant

        public ActionResult ListByPlant()
        {
            var data = InitLack1LiistByPlant(new Lack1IndexPlantViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<Lack1PlantData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput()
                {
                    Lack1Level = Enums.Lack1Level.Plant,
                    IsOpenDocumentOnly = true
                }))
            });

            return View("ListByPlant", data);
        }

        private Lack1IndexPlantViewModel InitLack1LiistByPlant(Lack1IndexPlantViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListByPlant(Lack1Input model)
        {
            var inputPlant = Mapper.Map<Lack1GetByParamInput>(model);
            inputPlant.Lack1Level = Enums.Lack1Level.Plant;
            inputPlant.IsOpenDocumentOnly = true;

            var dbDataPlant = _lack1Bll.GetAllByParam(inputPlant);

            var resultPlant = Mapper.Map<List<Lack1PlantData>>(dbDataPlant);

            var viewModel = new Lack1IndexPlantViewModel { Details = resultPlant };

            return PartialView("_Lack1ListByPlantTable", viewModel);

        }
        #endregion

        #region

        [HttpPost]
        public JsonResult PoaAndPlantListPartial(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Lack1IndexViewModel() { PoaList = listPoa, PlantIdList = listPlant };
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetNppbkcListByCompanyCode(string companyCode)
        {
            var data = _pbck1Bll.GetNppbkByCompanyCode(companyCode);
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetSupplierPlantDetail(string werks)
        {
            var data = _plantBll.GetT001WById(werks);
            return Json(data);
        }

        /// <summary>
        /// user for get received plant id
        /// </summary>
        /// <param name="nppbkcId"></param>
        /// <returns></returns>
        public JsonResult GetPlantListByNppbkcId(string nppbkcId)
        {
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Lack1CreateViewModel() { ReceivePlantList = listPlant };
            return Json(model);
        }

        public JsonResult GetExcisableGoodsTypeByNppbkcId(string nppbkcId)
        {
            var data = GetExciseGoodsTypeList(nppbkcId);
            var model = new Lack1CreateViewModel() { ExGoodTypeList = data };
            return Json(model);
        }

        public JsonResult GetSupplierListByParam(string nppbkcId, string excisableGoodsType)
        {
            var data = GetSupplierPlantListByParam(nppbkcId, excisableGoodsType);
            var model = new Lack1CreateViewModel() { SupplierList = data };
            return Json(model);
        }

        public JsonResult Generate(Lack1GenerateInputModel param)
        {
            var input = Mapper.Map<Lack1GenerateDataParamInput>(param);
            var outGeneratedData = _lack1Bll.GenerateLack1DataByParam(input);
            return Json(outGeneratedData);
        }

        #endregion

        #region ----- create -----

        public ActionResult Create(Enums.Lack1Level? lack1Level)
        {

            if (!lack1Level.HasValue)
            {
                return HttpNotFound();
            }

            var model = new Lack1CreateViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Level = lack1Level.Value,
                MenuPlantAddClassCss = lack1Level.Value == Enums.Lack1Level.Plant ? "active" : "",
                MenuNppbkcAddClassCss = lack1Level.Value == Enums.Lack1Level.Nppbkc ? "active" : ""
            };

            return CreateInitial(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lack1CreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Invalid input, please check the input.", Enums.MessageInfoType.Error);
                    return CreateInitial(model);
                }

                var input = Mapper.Map<Lack1CreateParamInput>(model);
                input.UserId = CurrentUser.USER_ID;
                var saveOutput = _lack1Bll.Create(input);
                if (saveOutput.Success)
                {
                    AddMessageInfo("Save successfull", Enums.MessageInfoType.Info);
                    if (model.Lack1Level == Enums.Lack1Level.Nppbkc)
                    {
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("ListByPlant");
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

        public ActionResult CreateInitial(Lack1CreateViewModel model)
        {
            return View("Create", InitialModel(model));
        }

        #endregion

        #region -------------- Private Method --------

        private string SaveUploadedFile(HttpPostedFileBase file, string lack2Num)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            sFileName = Constans.UploadPath + Path.GetFileName("LACK1_" + lack2Num + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        private List<Lack1SummaryProductionItemModel> ProcessSummaryProductionDetails(
            List<Lack1ProductionDetailItemModel> input)
        {
            if (input.Count > 0)
            {
                var groupedData = input.GroupBy(p => new
                {
                    p.UomId,
                    p.UomDesc
                }).Select(g => new Lack1SummaryProductionItemModel()
                {
                    UomId = g.Key.UomId,
                    UomDesc = g.Key.UomDesc,
                    Amount = g.Sum(p => p.Amount)
                });

                return groupedData.ToList();

            }
            return new List<Lack1SummaryProductionItemModel>();
        }

        private SelectList GetNppbkcListOnPbck1ByCompanyCode(string companyCode)
        {
            var data = _pbck1Bll.GetNppbkByCompanyCode(companyCode);
            return new SelectList(data, "NPPBKC_ID", "NPPBKC_ID");
        }

        private SelectList GetExciseGoodsTypeList(string nppbkcId)
        {
            var data = _pbck1Bll.GetGoodsTypeByNppbkcId(nppbkcId);
            return new SelectList(data, "EXC_GOOD_TYP", "EXT_TYP_DESC");
        }

        private SelectList GetSupplierPlantListByParam(string nppbkcId, string excisableGoodsType)
        {
            var data = _pbck1Bll.GetSupplierPlantByParam(new Pbck1GetSupplierPlantByParamInput()
            {
                NppbkcId = nppbkcId,
                ExciseableGoodsTypeId = excisableGoodsType
            });
            return new SelectList(data, "WERKS", "DROPDOWNTEXTFIELD");
        }

        private Lack1CreateViewModel InitialModel(Lack1CreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.BukrList = GlobalFunctions.GetCompanyList(_companyBll);
            model.MontList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearsList = CreateYearList();
            model.NppbkcList = GetNppbkcListOnPbck1ByCompanyCode(model.Bukrs);
            model.ReceivePlantList = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.NppbkcId);
            model.ExGoodTypeList = GetExciseGoodsTypeList(model.NppbkcId);
            model.SupplierList = GetSupplierPlantListByParam(model.NppbkcId, model.ExGoodsTypeId);
            model.WasteUomList = GlobalFunctions.GetUomList(_uomBll);
            model.ReturnUomList = GlobalFunctions.GetUomList(_uomBll);

            return (model);

        }

        private SelectList CreateYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            for (int i = 0; i < 2; i++)
            {
                years.Add(new SelectItemModel() { ValueField = currentYear - i, TextField = (currentYear - i).ToString() });
            }
            return new SelectList(years, "ValueField", "TextField");
        }

        #endregion

        #region Completed Document

        public ActionResult ListCompletedDocument()
        {
            var data = InitListCompletedDocument(new Lack1IndexCompletedDocumentViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<Lack1CompletedDocumentData>>(_lack1Bll.GetCompletedDocumentByParam(new Lack1GetByParamInput()))

            });

            return View("ListCompletedDocument", data);
        }

        private Lack1IndexCompletedDocumentViewModel InitListCompletedDocument(Lack1IndexCompletedDocumentViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListCompletedDocument(Lack1Input model)
        {
            var inputPlant = Mapper.Map<Lack1GetByParamInput>(model);
            inputPlant.IsOpenDocumentOnly = false;

            var dbDataPlant = _lack1Bll.GetCompletedDocumentByParam(inputPlant);

            var resultPlant = Mapper.Map<List<Lack1CompletedDocumentData>>(dbDataPlant);

            var viewModel = new Lack1IndexCompletedDocumentViewModel { Details = resultPlant };

            return PartialView("_Lack1CompletedDocumentTable", viewModel);

        }

        #endregion

        #region -------------- Details -----------

        public ActionResult Details(int? id, Enums.LACK1Type? lType)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            if (!lType.HasValue)
            {
                return HttpNotFound();
            }

            var lack1Data = _lack1Bll.GetDetailsById(id.Value);

            if (lack1Data == null)
            {
                return HttpNotFound();
            }

            var model = InitDetailModel(lack1Data, lType.Value);

            return View(model);

        }

        #endregion

        #region ----------------PrintPreview-------------

        public ActionResult PrintPreview(int? id, Enums.LACK1Type? lType)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            if (!lType.HasValue)
            {
                return HttpNotFound();
            }

            var lack1Data = _lack1Bll.GetPrintOutData(id.Value);

            if (lack1Data == null)
            {
                return HttpNotFound();
            }

            var model = Mapper.Map<Lack1PrintOutModel>(lack1Data);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.SummaryProductionList = ProcessSummaryProductionDetails(model.ProductionList);
            model.PrintOutTitle = "Preview LACK-1";
            return View("PrintDocument", model);
            //return PartialView("PrintDocument", model);
            //return new RazorPDF.PdfResult(model, "PrintDocument");
            //var fileName = "lack1-pdf-" + DateTime.Now.ToString("ddMMyyyyHHmmss");
            //return Pdf(fileName, "PrintDocument", model);
        }

        #endregion

        #region ------------- Workflow ------------

        public ActionResult RejectDocument()
        {
            return View();
        }

        public ActionResult GovCancelDocument()
        {
            return View();
        }

        public ActionResult GovRejectDocument()
        {
            return View();
        }

        #endregion

        #region ----------------- Edit -----------

        public ActionResult Edit(int? id, Enums.LACK1Type? lType)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            if (!lType.HasValue)
            {
                return HttpNotFound();
            }

            var lack1Data = _lack1Bll.GetDetailsById(id.Value);

            if (lack1Data == null)
            {
                return HttpNotFound();
            }

            var model = InitDetailModel(lack1Data, lType.Value);
            model = InitDetailList(model);

            if (model.Status == Enums.DocumentStatus.WaitingGovApproval)
            {
                model.ControllerAction = "GovApproveDocument";
            }

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model = SetActiveMenu(model, lType.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lack1ItemViewModel model)
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
                    model = InitDetailList(model);
                    model = SetHistory(model);
                    model.MainMenu = _mainMenu;
                    model.CurrentMenu = PageInfo;
                    model = SetActiveMenu(model, model.Lack1Type);
                    return View(model);
                }

                if (model.CreateBy != CurrentUser.USER_ID)
                {
                    return RedirectToAction("Detail", new { id = model.Lack1Id });
                }

                bool isSubmit = model.IsSaveSubmit == "submit";

                var lack1Data = Mapper.Map<Lack1DetailsDto>(model);

                var input = new Lack1SaveEditInput()
                {
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Modified,
                    Detail = lack1Data
                };

                var saveResult = _lack1Bll.SaveEdit(input);

                if (saveResult.Success)
                {
                    if (isSubmit)
                    {
                        Lack1Workflow(model.Lack1Id, Enums.ActionType.Submit, string.Empty);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("Details", "Lack1", new { id = model.Lack1Id });
                    }
                    AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                    return RedirectToAction("Edit", new { id = model.Lack1Id });
                }
            }
            catch (Exception)
            {
                AddMessageInfo("Save edit failed.", Enums.MessageInfoType.Error);
                model = InitDetailList(model);
                model = SetHistory(model);
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;
                model = SetActiveMenu(model, model.Lack1Type);
                return View(model);
            }
            model = InitDetailList(model);
            model = SetHistory(model);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model = SetActiveMenu(model, model.Lack1Type);
            return View(model);

        }

        private Lack1ItemViewModel SetHistory(Lack1ItemViewModel model)
        {
            //workflow history
            var workflowInput = new GetByFormNumberInput
            {
                FormNumber = model.Lack1Number,
                DocumentStatus = model.Status,
                NPPBKC_Id = model.NppbkcId
            };

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1,
                    model.Lack1Id.ToString()));

            var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(model.Lack1Number));

            model.ChangesHistoryList = changesHistory;
            model.WorkflowHistory = workflowHistory;
            model.PrintHistoryList = printHistory;

            return model;
        }

        #endregion

        private Lack1ItemViewModel InitDetailModel(Lack1DetailsDto lack1Data, Enums.LACK1Type lType)
        {

            var model = Mapper.Map<Lack1ItemViewModel>(lack1Data);
            
            model = SetHistory(model);

            model.Lack1Type = lType;
            model.SummaryProductionList = ProcessSummaryProductionDetails(model.ProductionList);

            SetActiveMenu(model, lType);

            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = lack1Data.CreateBy,
                CurrentUser = CurrentUser.USER_ID,
                CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                DocumentNumber = model.Lack1Number,
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
            
            return model;
        }

        private Lack1ItemViewModel InitDetailList(Lack1ItemViewModel model)
        {
            model.BukrList = GlobalFunctions.GetCompanyList(_companyBll);
            model.MontList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearsList = CreateYearList();
            model.NppbkcList = GetNppbkcListOnPbck1ByCompanyCode(model.Bukrs);
            model.ReceivePlantList = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.NppbkcId);
            model.ExGoodTypeList = GetExciseGoodsTypeList(model.NppbkcId);
            model.SupplierList = GetSupplierPlantListByParam(model.NppbkcId, model.ExGoodsType);
            model.WasteUomList = GlobalFunctions.GetUomList(_uomBll);
            model.ReturnUomList = GlobalFunctions.GetUomList(_uomBll);
            
            return model;

        }

        private Lack1ItemViewModel SetActiveMenu(Lack1ItemViewModel model, Enums.LACK1Type lType)
        {
            const string activeCss = "active";
            switch (lType)
            {
                case Enums.LACK1Type.ListByNppbkc:
                    model.MenuNppbkcAddClassCss = activeCss;
                    break;
                case Enums.LACK1Type.ComplatedDocument:
                    model.MenuCompletedAddClassCss = activeCss;
                    break;
                case Enums.LACK1Type.ListByPlant:
                    model.MenuPlantAddClassCss = activeCss;
                    break;
            }
            return model;
        }

        #region -------- workflow ------

        public ActionResult ApproveDocument(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            bool isSuccess = false;
            try
            {
                Lack1Workflow(id.Value, Enums.ActionType.Approve, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Details", "Lack1", new { id });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult RejectDocument(Lack1ItemViewModel model)
        {
            bool isSuccess = false;
            try
            {
                Lack1Workflow(model.Lack1Id, Enums.ActionType.Reject, model.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "Lack1", new { id = model.Lack1Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        private void Lack1Workflow(int id, Enums.ActionType actionType, string comment)
        {
            var input = new Lack1WorkflowDocumentInput()
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment
            };

            _lack1Bll.Lack1Workflow(input);
        }

        #endregion

        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var lack1Data = _lack1Bll.GetDetailsById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.PBCK1,
                FORM_ID = lack1Data.Lack1Id,
                FORM_NUMBER = lack1Data.Lack1Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel
            {
                PrintHistoryList =
                    Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack1Data.Lack1Number))
            };
            return PartialView("_PrintHistoryTable", model);

        }

    }
}