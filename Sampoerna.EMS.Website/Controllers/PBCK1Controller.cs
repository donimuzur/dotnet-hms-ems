using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
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
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IMonthBLL _monthBll;
        private IPlantBLL _plantBll;
        private Enums.MenuList _mainMenu;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;

        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll, IZaidmExProdTypeBLL prodTypeBll, IMonthBLL monthBll, IPlantBLL plantBll, IChangesHistoryBLL changesHistoryBll, IWorkflowHistoryBLL workflowHistoryBll)
            : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
            _prodTypeBll = prodTypeBll;
            _monthBll = monthBll;
            _plantBll = plantBll;
            _mainMenu = Enums.MenuList.PBCK1;
            _changesHistoryBll = changesHistoryBll;
            _workflowHistoryBll = workflowHistoryBll;
        }

        private List<Pbck1Item> GetPbckItems(Pbck1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var pbck1Data = _pbck1Bll.Pbck1GetByParam(new Pbck1GetByParamInput());
                return Mapper.Map<List<Pbck1Item>>(pbck1Data.Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetByParamInput>(filter);
            var dbData = _pbck1Bll.Pbck1GetByParam(input);
            return Mapper.Map<List<Pbck1Item>>(dbData.Data);
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
            return IndexInitial(new Pbck1ViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo
            });
        }

        public ActionResult IndexInitial(Pbck1ViewModel model)
        {
            model.SearchInput.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
            model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchInput.PoaList = new SelectList(new List<SelectItemModel>(), "ValueField", "TextField");
            model.Details = GetPbckItems();
            model.SearchInput.YearList = GetYearList(model.Details);
            return View("Index", model);
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

            var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, id.Value.ToString()));

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(new GetByFormTypeAndFormIdInput()
            {
                FormId = id.Value,
                FormType = Enums.FormType.PBKC1
            }));

            return EditInitial(new Pbck1ItemViewModel()
            {
                ChangesHistoryList = changeHistory,
                Detail = Mapper.Map<Pbck1Item>(pbck1Data),
                WorkflowHistory = workflowHistory
            });
        }

        [HttpPost]
        public ActionResult Edit(Pbck1ItemViewModel model)
        {

            model = CleanSupplierInfo(model);

            if (!ModelState.IsValid)
            {
                return CreateInitial(model);
            }

            //process save
            var dataToSave = Mapper.Map<Pbck1Dto>(model.Detail);
            //dataToSave.CreatedById = CurrentUser.USER_ID;
            var input = new Pbck1SaveInput()
            {
                Pbck1 = dataToSave,
                UserId = CurrentUser.USER_ID,
                WorkflowActionType = Enums.ActionType.Save
            };
            var saveResult = _pbck1Bll.Save(input);

            if (saveResult.Success)
            {
                return RedirectToAction("Index");
            }

            return EditInitial(model);
        }

        public ActionResult EditInitial(Pbck1ItemViewModel model)
        {
            return View("Edit", ModelInitial(model));
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

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(new GetByFormTypeAndFormIdInput() { FormId = id.Value, FormType = Enums.FormType.PBKC1 }));
            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK1, id.Value.ToString()));

            return View(new Pbck1ItemViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<Pbck1Item>(pbck1Data),
                ChangesHistoryList = changesHistory,
                WorkflowHistory = workflowHistory
            });
        }

        #endregion

        #region ----- create -----

        public ActionResult Create()
        {
            return CreateInitial(new Pbck1ItemViewModel()
            {
                Detail = new Pbck1Item()
            });
        }

        [HttpPost]
        public ActionResult Create(Pbck1ItemViewModel model)
        {

            model = CleanSupplierInfo(model);

            //if (model.Detail.Pbck1Type == Enums.PBCK1Type.Additional && !model.Detail.Pbck1Reference.HasValue)
            //{
            //    ModelState.AddModelError("Detail.Pbck1Reference", "PBCK-1 Reference is required.");
            //}

            if (!ModelState.IsValid)
            {
                return CreateInitial(model);
            }

            //process save
            var dataToSave = Mapper.Map<Pbck1Dto>(model.Detail);
            //dataToSave.CreatedById = CurrentUser.USER_ID;

            var input = new Pbck1SaveInput()
            {
                Pbck1 = dataToSave,
                UserId = CurrentUser.USER_ID,
                WorkflowActionType = Enums.ActionType.Save
            };

            var saveResult = _pbck1Bll.Save(input);

            if (saveResult.Success)
            {
                return RedirectToAction("Index");
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
        public PartialViewResult Filter(Pbck1ViewModel model)
        {
            model.Details = GetPbckItems(model.SearchInput);
            return PartialView("_Pbck1Table", model);
        }

        [HttpPost]
        public PartialViewResult UploadFileConversion(HttpPostedFileBase prodConvExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(prodConvExcelFile);
            var model = new Pbck1ItemViewModel();
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

            var input = Mapper.Map<List<Pbck1ProdPlanInput>>(model.Detail.Pbck1ProdPlan);
            var outputResult = _pbck1Bll.ValidatePbck1ProdPlanUpload(input);

            model.Detail.Pbck1ProdPlan = Mapper.Map<List<Pbck1ProdPlanModel>>(outputResult);

            return PartialView("_ProdConvList", model);
        }

        [HttpPost]
        public PartialViewResult UploadFilePlan(HttpPostedFileBase prodPlanExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(prodPlanExcelFile);
            var model = new Pbck1ItemViewModel();
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

            model.PbckReferenceList = model.Detail != null && model.Detail.Pbck1Reference.HasValue ?
                new SelectList(GetPbckItems().Where(c => model.Detail.Pbck1Reference != null
                    && c.Pbck1Id != model.Detail.Pbck1Reference.Value), "Pbck1Id", "Pbck1Number") : new SelectList(GetPbckItems(), "Pbck1Id", "Pbck1Number");

            model.YearList = CreateYearList();
            return model;
        }

        private Pbck1ItemViewModel CleanSupplierInfo(Pbck1ItemViewModel model)
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

            if (string.IsNullOrEmpty(model.Detail.HiddenSupplierNppbkcId)
                && !string.IsNullOrEmpty(model.Detail.HiddenSupplierNppbkcId))
            {
                model.Detail.SupplierNppbkcId = model.Detail.HiddenSupplierNppbkcId;
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

    }
}