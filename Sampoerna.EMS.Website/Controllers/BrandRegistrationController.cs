﻿using System;
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
        private IPlantBLL _plantBll;
        private Enums.MenuList _mainMenu;
        

        public BrandRegistrationController(IBrandRegistrationBLL brandRegistrationBll, IPageBLL pageBLL, 
            IMasterDataBLL masterBll, IZaidmExProdTypeBLL productBll, IZaidmExGoodTypeBLL goodTypeBll, 
            IChangesHistoryBLL changesHistoryBll, IPlantBLL plantBll)
            : base(pageBLL, Enums.MenuList.BrandRegistration)
        {
            _brandRegistrationBll = brandRegistrationBll;
            _masterBll = masterBll;
            _productBll = productBll;
            _goodTypeBll = goodTypeBll;
            _changesHistoryBll = changesHistoryBll;
            _plantBll = plantBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /BrandRegistration/
        public ActionResult Index()
        {
            var model = new BrandRegistrationIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var dbData = _brandRegistrationBll.GetAllBrands();
            model.Details = AutoMapper.Mapper.Map<List<BrandRegistrationDetail>>(dbData);

            ViewBag.Message = TempData["message"];
            return View("Index", model);
        }

        public ActionResult Details(long id)
        {
            var model = new BrandRegistrationDetailsViewModel();
            

            var dbBrand = _brandRegistrationBll.GetByIdIncludeChild(id);
            model = Mapper.Map<BrandRegistrationDetailsViewModel>(dbBrand);

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.BrandRegistration, id));

            return View(model);
        }

        private BrandRegistrationCreateViewModel InitCreate(BrandRegistrationCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.StickerCodeList = GlobalFunctions.GetStickerCodeList();
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

                if (dbBrand.STICKER_CODE.Length > 18)
                    dbBrand.STICKER_CODE = dbBrand.STICKER_CODE.Substring(0, 17);
                dbBrand.CREATED_DATE = DateTime.Now;
                dbBrand.IS_FROM_SAP = false;
                
                _brandRegistrationBll.Save(dbBrand);

                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                return RedirectToAction("Index");
            }


            InitCreate(model);

            return View(model);
        }

        private BrandRegistrationEditViewModel InitEdit(BrandRegistrationEditViewModel model)
        {
            model.MainMenu = _mainMenu;
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

            TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Updated;
            return RedirectToAction("Index");
          
        }

        private void SetChangesLog(ZAIDM_EX_BRAND origin, BrandRegistrationEditViewModel updatedModel)
        {
            var changesData = new Dictionary<string, bool>();
            if (origin.IS_FROM_SAP.HasValue == false || origin.IS_FROM_SAP.Value == false)
            {

                if (string.IsNullOrEmpty(origin.BRAND_CONTENT))
                    origin.BRAND_CONTENT = "";
                if (string.IsNullOrEmpty(origin.STICKER_CODE))
                    origin.STICKER_CODE = "";
                if (string.IsNullOrEmpty(origin.FA_CODE))
                    origin.FA_CODE = "";
                if (string.IsNullOrEmpty(origin.BRAND_CE))
                    origin.BRAND_CE = "";
                if (string.IsNullOrEmpty(origin.SKEP_NP))
                    origin.SKEP_NP = "";
                if (string.IsNullOrEmpty(origin.COLOUR))
                    origin.COLOUR = "";
                if (string.IsNullOrEmpty(origin.CUT_FILLER_CODE))
                    origin.CUT_FILLER_CODE = "";


                changesData.Add("STICKER_CODE",origin.STICKER_CODE.Equals(updatedModel.StickerCode));
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
                        changes.OLD_VALUE = origin.STICKER_CODE ?? null;
                        changes.NEW_VALUE = updatedModel.StickerCode ?? null;
                        break;
                    case "PlantId":
                        changes.OLD_VALUE = _plantBll.GetPlantWerksById(origin.PLANT_ID);
                        changes.NEW_VALUE = _plantBll.GetPlantWerksById(updatedModel.PlantId);
                        break;
                    case "FACode":
                        changes.OLD_VALUE = origin.FA_CODE;
                        changes.NEW_VALUE = updatedModel.FaCode;
                        break;
                    case "PersonalizationCode":
                        changes.OLD_VALUE = _masterBll.GetPersonalizationDescById(origin.PER_ID);
                        changes.NEW_VALUE =_masterBll.GetPersonalizationDescById(updatedModel.PersonalizationCode);
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
                        changes.OLD_VALUE = _masterBll.GetProductCodeTypeDescById(origin.PRODUCT_ID);
                        changes.NEW_VALUE = _masterBll.GetProductCodeTypeDescById(updatedModel.ProductCode);
                        break;
                    case "SeriesId":
                        changes.OLD_VALUE = _masterBll.GetDataSeriesDescById(origin.SERIES_ID);
                        changes.NEW_VALUE = _masterBll.GetDataSeriesDescById(updatedModel.SeriesId);
                        break;
                    case "Content":
                        changes.OLD_VALUE = origin.BRAND_CONTENT;
                        changes.NEW_VALUE = updatedModel.Content;
                        break;
                    case "MarketId":
                        changes.OLD_VALUE = _masterBll.GetMarketDescById(origin.MARKET_ID);
                        changes.NEW_VALUE = _masterBll.GetMarketDescById(updatedModel.MarketId);
                        break;
                    case "CountryId":
                        changes.OLD_VALUE = _masterBll.GetCountryCodeById(origin.COUNTRY_ID);
                        changes.NEW_VALUE = _masterBll.GetCountryCodeById(updatedModel.CountryId);
                        break;
                    case "HjeValue":
                        changes.OLD_VALUE = origin.HJE_IDR.ToString();
                        changes.NEW_VALUE = updatedModel.HjeValue.ToString();
                        break;

                    case "HjeCurrency":
                        changes.OLD_VALUE = _masterBll.GetCurrencyCodeById(origin.HJE_CURR);
                        changes.NEW_VALUE = _masterBll.GetCurrencyCodeById(updatedModel.HjeCurrency);
                        break;
                    case "Tariff":
                        changes.OLD_VALUE = origin.TARIFF.ToString();
                        changes.NEW_VALUE = updatedModel.Tariff.ToString();
                        break;
                    case "TariffCurrency":
                        changes.OLD_VALUE = _masterBll.GetCurrencyCodeById(origin.TARIFF_CURR);
                        changes.NEW_VALUE = _masterBll.GetCurrencyCodeById(updatedModel.TariffCurrency);
                        break;
                    case "ColourName":
                        changes.OLD_VALUE = origin.COLOUR;
                        changes.NEW_VALUE = updatedModel.ColourName;
                        break;
                    case "GoodType":
                        changes.OLD_VALUE = _goodTypeBll.GetGoodTypeDescById(origin.GOODTYP_ID);
                        changes.NEW_VALUE = _goodTypeBll.GetGoodTypeDescById(updatedModel.GoodType);
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
            
            _brandRegistrationBll.Delete(id);

            TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
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