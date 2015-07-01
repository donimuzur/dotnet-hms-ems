using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Controllers
{
    public class BrandRegistrationController : BaseController
    {
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IMasterDataBLL _masterBll;
        private IZaidmExProdTypeBLL _productBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IChangesHistoryBLL _changesHistoryBll;

        public BrandRegistrationController(IBrandRegistrationBLL brandRegistrationBll, IPageBLL pageBLL, IMasterDataBLL masterBll, IZaidmExProdTypeBLL productBll, IZaidmExGoodTypeBLL goodTypeBll, IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _brandRegistrationBll = brandRegistrationBll;
            _masterBll = masterBll;
            _productBll = productBll;
            _goodTypeBll = goodTypeBll;
            _changesHistoryBll = changesHistoryBll;
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
            model = Mapper.Map<BrandRegistrationDetailsViewModel>(dbBrand);

            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.BrandRegistration, id));

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

            model.IsActive = true;
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

                dbBrand = Mapper.Map<ZAIDM_EX_BRAND>(model);

                dbBrand.CREATED_DATE = DateTime.Now;
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

            if (dbBrand.IS_DELETED.HasValue && dbBrand.IS_DELETED.Value)
                return RedirectToAction("Details", "BrandRegistration", new { id = dbBrand.BRAND_ID });

            model = Mapper.Map<BrandRegistrationEditViewModel>(dbBrand);

            model = InitEdit(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BrandRegistrationEditViewModel model)
        {
            var dbBrand = _brandRegistrationBll.GetById(model.BrandId);
            if (dbBrand == null)
            {
                ModelState.AddModelError("BrandName", "Data Not Found");
                model = InitEdit(model);

                return View("Edit", model);
            }

            SetChangesLog(dbBrand, model);

            if (dbBrand.IS_FROM_SAP.HasValue && dbBrand.IS_FROM_SAP.Value)
            {
                dbBrand.PRINTING_PRICE = model.PrintingPrice;
                dbBrand.CONVERSION = model.Conversion;
                dbBrand.CUT_FILLER_CODE = model.CutFilterCode;
            }
            else
                Mapper.Map(model, dbBrand);

            _brandRegistrationBll.Save(dbBrand);

            return RedirectToAction("Index");
          
        }

        private void SetChangesLog(ZAIDM_EX_BRAND origin, BrandRegistrationEditViewModel updatedModel)
        {
            var changesData = new Dictionary<string, bool>();
            if (origin.IS_FROM_SAP.HasValue == false || origin.IS_FROM_SAP.Value == false)
            {
                changesData.Add("STICKER_CODE", origin.STICKER_CODE.Equals(updatedModel.StickerCode));
                changesData.Add("PlantId", origin.PLANT_ID.Equals(updatedModel.PlantId));
                changesData.Add("FACode", origin.FA_CODE.Equals(updatedModel.FaCode));
                changesData.Add("PersonalizationCode", origin.PER_ID.Equals(updatedModel.PersonalizationCode));
                changesData.Add("BrandName", origin.BRAND_CE.Equals(updatedModel.BrandName));
                changesData.Add("SkepNo", origin.SKEP_NP.Equals(updatedModel.SkepNo));
                changesData.Add("SkepDate", origin.SKEP_DATE.Equals(updatedModel.SkepDate));
                changesData.Add("ProductCode", origin.PRODUCT_ID.Equals(updatedModel.ProductCode));
                changesData.Add("SeriesId", origin.SERIES_ID.Equals(updatedModel.SeriesId));
                changesData.Add("Content", origin.BRAND_CONTENT.Equals(updatedModel.Content));
                changesData.Add("MarketId", origin.MARKET_ID.Equals(updatedModel.MarketId));
                changesData.Add("CountryId", origin.COUNTRY_ID.Equals(updatedModel.CountryId));
                changesData.Add("HjeValue", origin.HJE_IDR.Equals(updatedModel.HjeValue));
                changesData.Add("HjeCurrency", origin.HJE_CURR.Equals(updatedModel.HjeCurrency));
                changesData.Add("Tariff", origin.TARIFF.Equals(updatedModel.Tariff));
                changesData.Add("TariffCurrency", origin.TARIFF_CURR.Equals(updatedModel.TariffCurrency));
                changesData.Add("ColourName", origin.COLOUR.Equals(updatedModel.ColourName));
                changesData.Add("GoodType", origin.GOODTYP_ID.Equals(updatedModel.GoodType));
                changesData.Add("StartDate", origin.START_DATE.Equals(updatedModel.StartDate));
                changesData.Add("EndDate", origin.END_DATE.Equals(updatedModel.EndDate));
                changesData.Add("Status", origin.IS_ACTIVE.Equals(updatedModel.IsActive));
            }

            changesData.Add("Conversion", origin.CONVERSION.Equals(updatedModel.Conversion));
            changesData.Add("CutFilterCode", origin.CUT_FILLER_CODE.Equals(updatedModel.CutFilterCode));
            changesData.Add("PRINTING_PRICE", origin.PRINTING_PRICE.Equals(updatedModel.PrintingPrice));

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.BrandRegistration;
                changes.FORM_ID = origin.BRAND_ID;
                changes.FIELD_NAME = listChange.Key;
                changes.MODIFIED_BY = CurrentUser.USER_ID;
                changes.MODIFIED_DATE = DateTime.Now;
                switch (listChange.Key)
                {
                    case "STICKER_CODE":
                        changes.OLD_VALUE = origin.STICKER_CODE;
                        changes.NEW_VALUE = updatedModel.StickerCode;
                        break;
                    case "PlantId":
                        changes.OLD_VALUE = origin.PLANT_ID.ToString();
                        changes.NEW_VALUE = updatedModel.PlantId.ToString();
                        break;
                    case "FACode":
                        changes.OLD_VALUE = origin.FA_CODE;
                        changes.NEW_VALUE = updatedModel.FaCode;
                        break;
                    case "PersonalizationCode":
                        changes.OLD_VALUE = origin.PER_ID.ToString();
                        changes.NEW_VALUE = updatedModel.PersonalizationCode.ToString();
                        break;
                    case "BrandName":
                        changes.OLD_VALUE = origin.BRAND_CE;
                        changes.NEW_VALUE = updatedModel.BrandName;
                        break;
                    case "SkepNo":
                        changes.OLD_VALUE = origin.SKEP_NP;
                        changes.NEW_VALUE = updatedModel.SkepNo;
                        break;
                    case "SkepDate":
                        changes.OLD_VALUE = origin.SKEP_DATE.ToString("dd MMM yyyy");
                        changes.NEW_VALUE = updatedModel.SkepDate.ToString("dd MMM yyyy");
                        break;
                    case "ProductCode":
                        changes.OLD_VALUE = origin.PRODUCT_ID.ToString();
                        changes.NEW_VALUE = updatedModel.ProductCode.ToString();
                        break;
                    case "SeriesId":
                        changes.OLD_VALUE = origin.SERIES_ID.ToString();
                        changes.NEW_VALUE = updatedModel.SeriesId.ToString();
                        break;
                    case "Content":
                        changes.OLD_VALUE = origin.BRAND_CONTENT;
                        changes.NEW_VALUE = updatedModel.Content;
                        break;
                    case "MarketId":
                        changes.OLD_VALUE = origin.MARKET_ID.ToString();
                        changes.NEW_VALUE = updatedModel.MarketId.ToString();
                        break;
                    case "CountryId":
                        changes.OLD_VALUE = origin.COUNTRY_ID.ToString();
                        changes.NEW_VALUE = updatedModel.CountryId.ToString();
                        break;
                    case "HjeValue":
                        changes.OLD_VALUE = origin.HJE_IDR.ToString();
                        changes.NEW_VALUE = updatedModel.HjeValue.ToString();
                        break;

                    case "HjeCurrency":
                        changes.OLD_VALUE = origin.HJE_CURR.ToString();
                        changes.NEW_VALUE = updatedModel.HjeCurrency.ToString();
                        break;
                    case "Tariff":
                        changes.OLD_VALUE = origin.TARIFF.ToString();
                        changes.NEW_VALUE = updatedModel.Tariff.ToString();
                        break;
                    case "TariffCurrency":
                        changes.OLD_VALUE = origin.TARIFF_CURR.ToString();
                        changes.NEW_VALUE = updatedModel.TariffCurrency.ToString();
                        break;
                    case "ColourName":
                        changes.OLD_VALUE = origin.COLOUR;
                        changes.NEW_VALUE = updatedModel.ColourName;
                        break;
                    case "GoodType":
                        changes.OLD_VALUE = origin.GOODTYP_ID.ToString();
                        changes.NEW_VALUE = updatedModel.GoodType.ToString();
                        break;
                    case "StartDate":
                        changes.OLD_VALUE = origin.START_DATE.HasValue ? origin.START_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                        changes.NEW_VALUE = updatedModel.StartDate.HasValue? updatedModel.StartDate.Value.ToString("dd MMM yyyy") : string.Empty;
                        break;
                    case "EndDate":
                        changes.OLD_VALUE = origin.END_DATE.HasValue ? origin.END_DATE.Value.ToString("dd MMM yyyy"): string.Empty;
                        changes.NEW_VALUE = updatedModel.EndDate.HasValue ? updatedModel.EndDate.Value.ToString("dd MMM yyyy"): string.Empty;
                        break;
                    case "Conversion":
                        changes.OLD_VALUE = origin.CONVERSION.ToString();
                        changes.NEW_VALUE = updatedModel.Conversion.ToString();
                        break;
                    case "CutFilterCode":
                        changes.OLD_VALUE = origin.CUT_FILLER_CODE;
                        changes.NEW_VALUE = updatedModel.CutFilterCode;
                        break;
                    case "Status":
                        changes.OLD_VALUE = origin.IS_ACTIVE.ToString();
                        changes.NEW_VALUE = updatedModel.IsActive.ToString();
                        break;
                    case "PRINTING_PRICE":
                        changes.OLD_VALUE = origin.PRINTING_PRICE.ToString();
                        changes.NEW_VALUE = updatedModel.PrintingPrice.ToString();
                        break;
                }
                _changesHistoryBll.AddHistory(changes);
            }
        } 

        public ActionResult Delete(long id)
        {
            AddHistoryDelete(id);
            _brandRegistrationBll.Delete(id);

            return RedirectToAction("Index");
        }

        private void AddHistoryDelete(long id)
        {
            var history = new CHANGES_HISTORY();
            history.FORM_TYPE_ID = Enums.MenuList.BrandRegistration;
            history.FORM_ID = id;
            history.FIELD_NAME = "IS_DELETED";
            history.OLD_VALUE = "false";
            history.NEW_VALUE = "true";
            history.MODIFIED_DATE = DateTime.Now;
            history.MODIFIED_BY = CurrentUser.USER_ID;

            _changesHistoryBll.AddHistory(history);
        }
    }
}