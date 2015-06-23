using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Utility;


namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK1Controller : BaseController
    {
        private IPBCK1BLL _pbck1Bll;
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IMonthBLL _monthBll;
        private IPlantBLL _plantBll;
        
        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll, IZaidmExProdTypeBLL prodTypeBll, IMonthBLL monthBll, IPlantBLL plantBll)
            : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
            _prodTypeBll = prodTypeBll;
            _monthBll = monthBll;
            _plantBll = plantBll;
        }

        private List<PBCK1Item> GetPBCKItems(PBCK1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                return Mapper.Map<List<PBCK1Item>>(_pbck1Bll.GetPBCK1ByParam(new PBCK1Input()));
            }
            //getbyparams
            var input = Mapper.Map<PBCK1Input>(filter);
            var dbData = _pbck1Bll.GetPBCK1ByParam(input);
            return Mapper.Map<List<PBCK1Item>>(dbData);
        }

        private SelectList GetYearList(List<PBCK1Item> pbck1Data)
        {
            var query = from x in pbck1Data
                        where x.PERIOD_FROM.HasValue
                        select new SelectItemModel()
                        {
                            ValueField = x.PERIOD_FROM.Value.Year,
                            TextField = x.PERIOD_FROM.Value.Year.ToString()
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }
        
        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            return IndexInitial(new PBCK1ViewModel()
            {
                MainMenu = Enums.MenuList.ExcisableGoodsMovement,
                CurrentMenu = PageInfo
            });
        }

        public ActionResult IndexInitial(PBCK1ViewModel model)
        {
            model.SearchInput.NPPBKCIDList = GlobalFunctions.GetNppbkcAll();
            model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchInput.POAList = new SelectList(new List<SelectItemModel>(), "ValueField", "TextField");
            model.Details = GetPBCKItems();
            model.SearchInput.YearList = GetYearList(model.Details);
            return View("Index", model);
        }

        public ActionResult Edit(long id)
        {
            return View(new PBCK1ItemViewModel() { MainMenu = Enums.MenuList.ExcisableGoodsMovement, CurrentMenu = PageInfo });
        }

        public ActionResult Details(long id)
        {
            return View(new PBCK1ItemViewModel() { MainMenu = Enums.MenuList.ExcisableGoodsMovement, CurrentMenu = PageInfo });
        }

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var model = new PBCK1ViewModel { SearchInput = { POAList = listPoa } };
            return Json(model);
        }
        [HttpPost]
        public PartialViewResult Filter(PBCK1ViewModel model)
        {
            model.Details = GetPBCKItems(model.SearchInput);
            return PartialView("_Pbck1Table", model);
        }

        [HttpPost]
        public PartialViewResult UploadFileConversion(HttpPostedFileBase ProdConvExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(ProdConvExcelFile);
            var model = new PBCK1ItemViewModel();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var prodConvModel = new PBCK1ProdConvModel();

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
        public PartialViewResult UploadFilePlan(HttpPostedFileBase ProdPlanExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(ProdPlanExcelFile);
            var model = new PBCK1ItemViewModel();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var prodPlanModel = new PBCK1ProdPlanModel();

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
                            prodPlanModel.BKCRequires = datarow[3];
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

        public ActionResult Create()
        {
            return CreateInitial(new PBCK1ItemViewModel());
        }

        public ActionResult CreateInitial(PBCK1ItemViewModel model)
        {
            model.CurrentMenu = PageInfo;
            model.MainMenu = Enums.MenuList.ExcisableGoodsMovement;
            model.NppbkcList = GlobalFunctions.GetNppbkcAll();
            model.MonthList = GlobalFunctions.GetMonthList();
            model.SupplierPortList = GlobalFunctions.GetSupplierPortList();
            model.SupplierPlantList = GlobalFunctions.GetSupplierPlantList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();
            model.UOMList = GlobalFunctions.GetUomList();
            return View(model);
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

        [HttpPost]
        public ActionResult Save(PBCK1ItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CreateInitial(model);
            }

            //process

            return RedirectToAction("Create");

        }

    }
}