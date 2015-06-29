using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.BrandRegistration;

namespace Sampoerna.EMS.Website.Controllers
{
    public class BrandRegistrationController : BaseController
    {
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IMasterDataBLL _masterBll;
        private IZaidmExProdTypeBLL _productBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;

        public BrandRegistrationController(IBrandRegistrationBLL brandRegistrationBll, IPageBLL pageBLL, IMasterDataBLL masterBll, IZaidmExProdTypeBLL productBll, IZaidmExGoodTypeBLL goodTypeBll)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _brandRegistrationBll = brandRegistrationBll;
            _masterBll = masterBll;
            _productBll = productBll;
            _goodTypeBll = goodTypeBll;
        }

        //
        // GET: /BrandRegistration/
        public ActionResult Index()
        {
            var model = new BrandRegistrationIndexViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            var dbData = _brandRegistrationBll.GetAllBrands();
            model.Details = AutoMapper.Mapper.Map<List<BrandRegistrationDetail>>(dbData);

            return View("Index", model);
        }

        public ActionResult Details(long id)
        {
            var model = new BrandRegistrationDetailsViewModel();


            var dbBrand = _brandRegistrationBll.GetByIdIncludeChild(id);
            model = AutoMapper.Mapper.Map<BrandRegistrationDetailsViewModel>(dbBrand);

            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            return View(model);
        }

        private BrandRegistrationCreateViewModel InitCreate(BrandRegistrationCreateViewModel model)
        {
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.PersonalizationCodeList = GlobalFunctions.GetPersonalizationCodeList();
            model.ProductCodeList = GlobalFunctions.GetProductCodeList();
            model.SeriesList = GlobalFunctions.GetSeriesCodeList();
            model.MarketCodeList = GlobalFunctions.GetMarketCodeList();
            model.CountryCodeList = GlobalFunctions.GetCountryList();
            model.HjeCurrencyList = GlobalFunctions.GetCurrencyList();
            model.TariffCurrencyList = GlobalFunctions.GetCurrencyList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();

            return model;
        }

        public ActionResult Create()
        {
            var model = new BrandRegistrationCreateViewModel();

            model = InitCreate(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult PersonalizationCodeDescription(long personalizationId)
        {
            var personalizationCodeDescription = _masterBll.GetDataPersonalizationById(personalizationId);
            return Json(personalizationCodeDescription.PER_DESC);
        }

        [HttpPost]
        public JsonResult ProductCodeDetail(long productId)
        {
            var product = _productBll.GetById(productId);
            return Json(product);
        }

        [HttpPost]
        public JsonResult SeriesCodeDescription(long seriesId)
        {
            var seriesCodeDescription = _masterBll.GetDataSeriesById(seriesId);
            return Json(seriesCodeDescription.SERIES_VALUE);
        }

        [HttpPost]
        public JsonResult MarketCodeDescription(long marketId)
        {
            var market = _masterBll.GetDataMarketById(marketId);
            return Json(market.MARKET_DESC);
        }

        [HttpPost]
        public JsonResult GoodTypeDescription(int goodTypeId)
        {
            var goodType = _goodTypeBll.GetById(goodTypeId);
            return Json(goodType.EXT_TYP_DESC);
        }

        [HttpPost]
        public ActionResult Create(BrandRegistrationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbBrand = new ZAIDM_EX_BRAND();

                dbBrand = AutoMapper.Mapper.Map<ZAIDM_EX_BRAND>(model);

                dbBrand.IS_FROM_SAP = false;

                _brandRegistrationBll.Save(dbBrand);

                return RedirectToAction("Index");
            }


            InitCreate(model);

            return View(model);
        }

        private BrandRegistrationEditViewModel InitEdit(BrandRegistrationEditViewModel model)
        {
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.PersonalizationCodeList = GlobalFunctions.GetPersonalizationCodeList();
            model.ProductCodeList = GlobalFunctions.GetProductCodeList();
            model.SeriesList = GlobalFunctions.GetSeriesCodeList();
            model.MarketCodeList = GlobalFunctions.GetMarketCodeList();
            model.CountryCodeList = GlobalFunctions.GetCountryList();
            model.HjeCurrencyList = GlobalFunctions.GetCurrencyList();
            model.TariffCurrencyList = GlobalFunctions.GetCurrencyList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList();

            return model;
        }

        public ActionResult Edit(long id)
        {
            var model = new BrandRegistrationEditViewModel();


            var dbBrand = _brandRegistrationBll.GetByIdIncludeChild(id);
            model = AutoMapper.Mapper.Map<BrandRegistrationEditViewModel>(dbBrand);

            model = InitEdit(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BrandRegistrationEditViewModel model)
        {


            if (ModelState.IsValid)
            {

                var dbBrand = _brandRegistrationBll.GetById(model.BrandId);
                if (dbBrand == null)
                {
                    ModelState.AddModelError("BrandName", "Data Not Found");
                    model.MainMenu = Enums.MenuList.MasterData;
                    model.CurrentMenu = PageInfo;

                    return View("Edit", model);
                }

                //convertion

                dbBrand.PRINTING_PRICE = model.PrintingPrice;
                dbBrand.CUT_FILLER_CODE = model.CutFilterCode;

                _brandRegistrationBll.Save(dbBrand);

                return RedirectToAction("Index");
            }

            // InitCreateModel(model);
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            return View("Edit", model);
        }
    }
}