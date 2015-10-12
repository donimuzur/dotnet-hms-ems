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
        private IPlantBLL _plantBll;
        private Enums.MenuList _mainMenu;
        private IMaterialBLL _materialBll;
        
        public BrandRegistrationController(IBrandRegistrationBLL brandRegistrationBll, IPageBLL pageBLL, 
            IMasterDataBLL masterBll, IZaidmExProdTypeBLL productBll, IZaidmExGoodTypeBLL goodTypeBll, 
            IChangesHistoryBLL changesHistoryBll, IPlantBLL plantBll, IMaterialBLL materialBll)
            : base(pageBLL, Enums.MenuList.BrandRegistration)
        {
            _brandRegistrationBll = brandRegistrationBll;
            _masterBll = masterBll;
            _productBll = productBll;
            _goodTypeBll = goodTypeBll;
            _changesHistoryBll = changesHistoryBll;
            _plantBll = plantBll;
            _materialBll = materialBll;
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
            model.Details = Mapper.Map<List<BrandRegistrationDetail>>(dbData);
            ViewBag.Message = TempData["message"];
            return View("Index", model);
        }

        public ActionResult Details(string plant, string facode,string stickercode)
        {
            var model = new BrandRegistrationDetailsViewModel();


            var dbBrand = _brandRegistrationBll.GetById(plant, facode, stickercode);
            model = Mapper.Map<BrandRegistrationDetailsViewModel>(dbBrand);
            model.TariffValueStr = model.Tariff == null ? string.Empty : model.Tariff.ToString();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.BrandRegistration, plant+facode+stickercode));

            
            if (model.IsFromSap.HasValue && model.IsFromSap.Value)
            {
                model.IsAllowDelete = false;
            }
            else
            {
                model.IsAllowDelete = true;
            }
            

            return View(model);
        }

        private BrandRegistrationCreateViewModel InitCreate(BrandRegistrationCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.StickerCodeList = GlobalFunctions.GetStickerCodeList();
            //model.PlantList = GlobalFunctions.GetVirtualPlantList();
            model.PersonalizationCodeList = GlobalFunctions.GetPersonalizationCodeList();
            model.ProductCodeList = GlobalFunctions.GetProductCodeList(_productBll);
            model.SeriesList = GlobalFunctions.GetSeriesCodeList(_masterBll);
            model.MarketCodeList = GlobalFunctions.GetMarketCodeList(_masterBll);
            model.CountryCodeList = GlobalFunctions.GetCountryList();
            model.HjeCurrencyList = GlobalFunctions.GetCurrencyList();
            model.TariffCurrencyList = GlobalFunctions.GetCurrencyList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);

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
        public JsonResult PersonalizationCodeDescription(string personalizationId)
        {
            var personalizationCodeDescription = _masterBll.GetDataPersonalizationById(personalizationId);
            return Json(personalizationCodeDescription.PER_DESC);
        }

        [HttpPost]
        public JsonResult ProductCodeDetail(string productId)
        {
            var product = _productBll.GetById(productId);
            return Json(product);
        }

        [HttpPost]
        public JsonResult SeriesCodeDescription(string seriesId)
        {
            var seriesCodeDescription = _masterBll.GetDataSeriesById(seriesId);
            return Json(seriesCodeDescription.SERIES_VALUE);
        }

        [HttpPost]
        public JsonResult MarketCodeDescription(string marketId)
        {
            var market = _masterBll.GetDataMarketById(marketId);
            return Json(market.MARKET_DESC);
        }

        [HttpPost]
        public JsonResult GoodTypeDescription(string goodTypeId)
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
                dbBrand.HJE_IDR = model.HjeValueStr == null ? 0 : Convert.ToDecimal(model.HjeValueStr);
                dbBrand.TARIFF = model.TariffValueStr == null ? 0 : Convert.ToDecimal(model.TariffValueStr);
                dbBrand.CONVERSION = model.ConversionValueStr == null ? 0 : Convert.ToDecimal(model.ConversionValueStr);
                dbBrand.PRINTING_PRICE = model.PrintingPrice == null ? 0 : Convert.ToDecimal(model.PrintingPriceValueStr);
                dbBrand.STATUS = model.IsActive;
                if (!string.IsNullOrEmpty(dbBrand.PER_CODE_DESC))
                    dbBrand.PER_CODE_DESC = model.PersonalizationCodeDescription.Split('-')[1];

                try
                {
                    _brandRegistrationBll.Save(dbBrand);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    return RedirectToAction("Index");
                }
                catch
                {
                    AddMessageInfo("Save Failed.", Enums.MessageInfoType.Error
                       );
                }
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
            model.CutFillerCodeList = GlobalFunctions.GetCutFillerCodeList(model.PlantId);
            model.ProductCodeList = GlobalFunctions.GetProductCodeList(_productBll);
            model.SeriesList = GlobalFunctions.GetSeriesCodeList(_masterBll);
            model.MarketCodeList = GlobalFunctions.GetMarketCodeList(_masterBll);
            model.CountryCodeList = GlobalFunctions.GetCountryList();
            model.HjeCurrencyList = GlobalFunctions.GetCurrencyList();
            model.TariffCurrencyList = GlobalFunctions.GetCurrencyList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            return model;
        }

        public ActionResult Edit(string plant, string facode,string stickercode)
        {
           
            var model = new BrandRegistrationEditViewModel();


            var dbBrand = _brandRegistrationBll.GetById(plant, facode,stickercode);
          
            if (dbBrand.IS_DELETED.HasValue && dbBrand.IS_DELETED.Value)
                return RedirectToAction("Details", "BrandRegistration", new { plant = dbBrand.WERKS, facode= dbBrand.FA_CODE, stickercode=dbBrand.STICKER_CODE });

            model = Mapper.Map<BrandRegistrationEditViewModel>(dbBrand);
            model.HjeValueStr = model.HjeValue == null ? string.Empty : model.HjeValue.ToString();
            model.TariffValueStr = model.Tariff == null ? string.Empty : model.Tariff.ToString();
            model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();
            model.PrintingPriceValueStr = model.PrintingPrice == null ? string.Empty : model.PrintingPrice.ToString();
            model = InitEdit(model);

            model.IsAllowDelete = !model.IsFromSAP;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BrandRegistrationEditViewModel model)
        {
            var dbBrand = _brandRegistrationBll.GetById(model.PlantId, model.FaCode,model.StickerCode);
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
                dbBrand.CUT_FILLER_CODE = model.CutFillerCode;
                dbBrand.STATUS = model.IsActive;
            }
            else
                Mapper.Map(model, dbBrand);
            dbBrand.HJE_IDR = model.HjeValueStr == null ? 0 : Convert.ToDecimal(model.HjeValueStr);
            dbBrand.TARIFF = model.TariffValueStr == null ? 0 : Convert.ToDecimal(model.TariffValueStr);
            dbBrand.CONVERSION = model.ConversionValueStr == null ? 0 : Convert.ToDecimal(model.ConversionValueStr);
            dbBrand.PRINTING_PRICE = model.PrintingPriceValueStr == null ? 0 : Convert.ToDecimal(model.PrintingPriceValueStr);
            dbBrand.CREATED_BY = CurrentUser.USER_ID;
            if (!string.IsNullOrEmpty(model.PersonalizationCodeDescription))
                dbBrand.PER_CODE_DESC = model.PersonalizationCodeDescription;
            try
            {
                _brandRegistrationBll.Save(dbBrand);
                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success
                         );
                return RedirectToAction("Index");

            }
            catch 
            {
                AddMessageInfo("Edit Failed.", Enums.MessageInfoType.Error
                         );
            }

            model = InitEdit(model);

            return View("Edit", model);
        }

        private void SetChangesLog(ZAIDM_EX_BRAND origin, BrandRegistrationEditViewModel updatedModel)
        {
            var changesData = new Dictionary<string, bool>();
            updatedModel.HjeValue = updatedModel.HjeValueStr == null ? 0: Convert.ToDecimal(updatedModel.HjeValueStr);
            updatedModel.Tariff = updatedModel.TariffValueStr == null ? 0 : Convert.ToDecimal(updatedModel.TariffValueStr);
            updatedModel.Conversion = updatedModel.ConversionValueStr == null ? 0 : Convert.ToDecimal(updatedModel.ConversionValueStr);
            updatedModel.PrintingPrice = updatedModel.PrintingPriceValueStr == null ? 0 : Convert.ToDecimal(updatedModel.PrintingPriceValueStr);
            

            if (origin.IS_FROM_SAP.HasValue == false || origin.IS_FROM_SAP.Value == false)
            {

              

                changesData.Add("FACode", origin.FA_CODE == updatedModel.FaCode);
                changesData.Add("PersonalizationCode", origin.PER_CODE == updatedModel.PersonalizationCode);
                changesData.Add("BrandName", origin.BRAND_CE == updatedModel.BrandName);
                changesData.Add("SkepNo", origin.SKEP_NO == updatedModel.SkepNo);
                changesData.Add("SkepDate", origin.SKEP_DATE == updatedModel.SkepDate);
                changesData.Add("ProductCode", origin.PROD_CODE == updatedModel.ProductCode);
                changesData.Add("SeriesId", origin.SERIES_CODE == updatedModel.SeriesId);
                changesData.Add("Content", origin.BRAND_CONTENT == updatedModel.Content);
                changesData.Add("MarketId", origin.MARKET_ID == updatedModel.MarketId);
                changesData.Add("CountryId", origin.COUNTRY == updatedModel.CountryId);
                changesData.Add("HjeValue", origin.HJE_IDR  ==updatedModel.HjeValue );
                changesData.Add("HjeCurrency", origin.HJE_CURR == updatedModel.HjeCurrency);
                changesData.Add("Tariff", origin.TARIFF == updatedModel.Tariff);
                changesData.Add("TariffCurrency", origin.TARIF_CURR == updatedModel.TariffCurrency);
                changesData.Add("ColourName", origin.COLOUR == updatedModel.ColourName);
                changesData.Add("GoodType", origin.EXC_GOOD_TYP==updatedModel.GoodType);
                changesData.Add("StartDate", origin.START_DATE == updatedModel.StartDate);
                changesData.Add("EndDate", origin.END_DATE == updatedModel.EndDate);
                changesData.Add("Status", origin.STATUS == updatedModel.IsActive );
            }

            changesData.Add("Conversion", origin.CONVERSION == updatedModel.Conversion);
            changesData.Add("CutFilterCode", origin.CUT_FILLER_CODE == updatedModel.CutFillerCode);
            changesData.Add("PRINTING_PRICE", origin.PRINTING_PRICE == updatedModel.PrintingPrice);

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.BrandRegistration;
                changes.FORM_ID = origin.WERKS + origin.FA_CODE + origin.STICKER_CODE;
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
                        changes.OLD_VALUE = _plantBll.GetPlantWerksById(origin.WERKS);
                        changes.NEW_VALUE = _plantBll.GetPlantWerksById(updatedModel.PlantId);
                        break;
                    case "FACode":
                        changes.OLD_VALUE = origin.FA_CODE;
                        changes.NEW_VALUE = updatedModel.FaCode;
                        break;
                    case "PersonalizationCode":
                        changes.OLD_VALUE = _masterBll.GetPersonalizationDescById(origin.PER_CODE);
                        changes.NEW_VALUE =_masterBll.GetPersonalizationDescById(updatedModel.PersonalizationCode);
                        break;
                    case "BrandName":
                        changes.OLD_VALUE = origin.BRAND_CE;
                        changes.NEW_VALUE = updatedModel.BrandName;
                        break;
                    case "SkepNo":
                        changes.OLD_VALUE = origin.SKEP_NO;
                        changes.NEW_VALUE = updatedModel.SkepNo;
                        break;
                    case "SkepDate":
                        changes.OLD_VALUE = origin.SKEP_DATE == null ? string.Empty : Convert.ToDateTime(origin.SKEP_DATE).ToString("dd MMM yyyy");
                        changes.NEW_VALUE = updatedModel.SkepDate.ToString("dd MMM yyyy");
                        break;
                    case "ProductCode":
                        changes.OLD_VALUE = _masterBll.GetProductCodeTypeDescById(origin.PROD_CODE);
                        changes.NEW_VALUE = _masterBll.GetProductCodeTypeDescById(updatedModel.ProductCode);
                        break;
                    case "SeriesId":
                        changes.OLD_VALUE = _masterBll.GetDataSeriesDescById(origin.SERIES_CODE).ToString();
                        changes.NEW_VALUE = _masterBll.GetDataSeriesDescById(updatedModel.SeriesId).ToString();
                        break;
                    case "Content":
                        changes.OLD_VALUE = origin.BRAND_CONTENT == null ? string.Empty : origin.BRAND_CONTENT.ToString();
                        changes.NEW_VALUE = updatedModel.Content == null? string.Empty : updatedModel.Content.ToString();
                        break;
                    case "MarketId":
                        changes.OLD_VALUE = _masterBll.GetMarketDescById(origin.MARKET_ID);
                        changes.NEW_VALUE = _masterBll.GetMarketDescById(updatedModel.MarketId);
                        break;
                    case "CountryId":
                        changes.OLD_VALUE = origin.COUNTRY;
                        changes.NEW_VALUE = updatedModel.CountryId;
                        break;
                    case "HjeValue":
                        changes.OLD_VALUE = origin.HJE_IDR.ToString();
                        changes.NEW_VALUE = updatedModel.HjeValue.ToString();
                        break;

                    case "HjeCurrency":
                        changes.OLD_VALUE = origin.HJE_CURR;
                        changes.NEW_VALUE = updatedModel.HjeCurrency;
                        break;
                    case "Tariff":
                        changes.OLD_VALUE = origin.TARIFF.ToString();
                        changes.NEW_VALUE = updatedModel.Tariff.ToString();
                        break;
                    case "TariffCurrency":
                        changes.OLD_VALUE = origin.TARIF_CURR;
                        changes.NEW_VALUE = updatedModel.TariffCurrency;
                        break;
                    case "ColourName":
                        changes.OLD_VALUE = origin.COLOUR;
                        changes.NEW_VALUE = updatedModel.ColourName;
                        break;
                    case "GoodType":
                        changes.OLD_VALUE = _goodTypeBll.GetGoodTypeDescById(origin.EXC_GOOD_TYP);
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
                        changes.NEW_VALUE = updatedModel.CutFillerCode;
                        break;
                    case "Status":
                        changes.OLD_VALUE = origin.STATUS.ToString();
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

        public ActionResult Delete(string plant, string facode,string stickercode)
        {
            AddHistoryDelete(plant, facode);
            var isDeleted = _brandRegistrationBll.Delete(plant, facode,stickercode);
            
            if(isDeleted)
                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Deleted;
            else
                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Updated;
            
            return RedirectToAction("Index");
        }

        private void AddHistoryDelete(string plant, string facode)
        {
            var history = new CHANGES_HISTORY();
            history.FORM_TYPE_ID = Enums.MenuList.BrandRegistration;
            history.FORM_ID = plant + facode;
            history.FIELD_NAME = "IS_DELETED";
            history.OLD_VALUE = "false";
            history.NEW_VALUE = "true";
            history.MODIFIED_DATE = DateTime.Now;
            history.MODIFIED_BY = CurrentUser.USER_ID;

            _changesHistoryBll.AddHistory(history);
        }
        [HttpPost]
        public JsonResult GetPlantByStickerCode(string mn)
        {
           var data =  _materialBll.getAllPlant(mn);
            return Json(new SelectList(data, "WERKS", "NAME1"));
        }

        [HttpPost]
        public JsonResult GetCutFillerCodeByPlant(string plant)
        {
            var data = GlobalFunctions.GetCutFillerCodeList(plant);
            return Json(data);
        }
    }
}