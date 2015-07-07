using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.Business;
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
                var pbck1Data = _pbck1Bll.GetPBCK1ByParam(new Pbck1GetByParamInput());
                return Mapper.Map<List<Pbck1Item>>(pbck1Data);
            }

            //getbyparams
            var input = Mapper.Map<Pbck1GetByParamInput>(filter);
            var dbData = _pbck1Bll.GetPBCK1ByParam(input);
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
                years.Add(new SelectItemModel(){ ValueField = currentYear - i, TextField = (currentYear - i).ToString()});
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

        public ActionResult Edit(long id)
        {
            var pbck1Data = _pbck1Bll.GetById(id);
            var changeHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id));

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(Enums.FormType.PBKC1, id));

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
            var dataToSave = Mapper.Map<Pbck1>(model.Detail);
            dataToSave.CreatedById = CurrentUser.USER_ID;
            var saveResult = _pbck1Bll.Save(dataToSave);

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

        public ActionResult Details(long id)
        {
            var pbck1Data = _pbck1Bll.GetById(id);
            return View(new Pbck1ItemViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<Pbck1Item>(pbck1Data),
                ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.HeaderFooter, id))
            });
        }

        #endregion

        #region ----- create -----

        public ActionResult Create()
        {
            return CreateInitial(new Pbck1ItemViewModel());
        }

        [HttpPost]
        public ActionResult Create(Pbck1ItemViewModel model)
        {

            model = CleanSupplierInfo(model);

            if (!ModelState.IsValid)
            {
                return CreateInitial(model);
            }

            //process save
            var dataToSave = Mapper.Map<Pbck1>(model.Detail);
            dataToSave.CreatedById = CurrentUser.USER_ID;
            var saveResult = _pbck1Bll.Save(dataToSave);

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
                    var prodConvModel = new Pbck1ProdConvModel();

                    try
                    {
                        var prodCodeFromFile = Convert.ToInt32(datarow[0]);
                        var prodType = _prodTypeBll.GetByCode(prodCodeFromFile);
                        if (prodType != null)
                        {
                            prodConvModel.ProductCode = prodType.PRODUCT_CODE;
                            prodConvModel.ProductType = prodType.PRODUCT_TYPE;
                            prodConvModel.ProductTypeAlias = prodType.PRODUCT_ALIAS;
                            prodConvModel.ConverterOutput = Convert.ToDecimal(datarow[1]);
                            prodConvModel.ConverterUom = datarow[2];
                            model.ProductConversions.Add(prodConvModel);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
            }
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
                    var prodPlanModel = new Pbck1ProdPlanModel();

                    try
                    {
                        var month = _monthBll.GetMonth(Convert.ToInt32(datarow[0]));
                        var prodCodeFromFile = Convert.ToInt32(datarow[1]);
                        var prodType = _prodTypeBll.GetByCode(prodCodeFromFile);
                        if (prodType != null)
                        {
                            prodPlanModel.MonthName = month.MONTH_NAME_IND;
                            prodPlanModel.ProductCode = prodType.PRODUCT_CODE;
                            prodPlanModel.ProductType = prodType.PRODUCT_TYPE;
                            prodPlanModel.ProductTypeAlias = prodType.PRODUCT_ALIAS;
                            prodPlanModel.Amount = Convert.ToDecimal(datarow[2]);
                            prodPlanModel.BkcRequires = datarow[3];
                            model.ProductPlans.Add(prodPlanModel);
                        }
                    }
                    catch (Exception)
                    {
                        continue;

                    }

                }
            }
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
            model.PbckReferenceList = new SelectList(GetPbckItems(), "Pbck1Id", "Pbck1Number");
            model.YearList = CreateYearList();
            return model;
        }

        private Pbck1ItemViewModel CleanSupplierInfo(Pbck1ItemViewModel model)
        {
            if (string.IsNullOrEmpty(model.Detail.SupplierKppbc)
                && !string.IsNullOrEmpty(model.Detail.HiddenSupplierKppbc))
            {
                model.Detail.SupplierKppbc = model.Detail.HiddenSupplierKppbc;
            }

            if (string.IsNullOrEmpty(model.Detail.SupplierAddress) &&
                !string.IsNullOrEmpty(model.Detail.HiddendSupplierAddress))
            {
                model.Detail.SupplierAddress = model.Detail.HiddendSupplierAddress;
            }

            if (string.IsNullOrEmpty(model.Detail.HiddenSupplierNppbkc)
                && !string.IsNullOrEmpty(model.Detail.HiddenSupplierNppbkc))
            {
                model.Detail.SupplierNppbkc = model.Detail.HiddenSupplierNppbkc;
            }
            return model;
        }

        [HttpPost]
        public JsonResult GetSupplierPlant()
        {
            return Json(GlobalFunctions.GetSupplierPlantList());
        }

        [HttpPost]
        public JsonResult GetNppbkcDetail(long nppbkcid)
        {
            var data = GlobalFunctions.GetNppbkcById(nppbkcid);
            return Json(Mapper.Map<CompanyDetail>(data.T1001));
        }

        [HttpPost]
        public JsonResult GetSupplierPlantDetail(long plantid)
        {
            var data = _plantBll.GetId(plantid);
            return Json(Mapper.Map<DetailPlantT1001W>(data));
        }

    }
}