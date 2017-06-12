using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using System.Web;
using Sampoerna.EMS.Website.Models.NPPBKC;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class BrandRegistrationController : BaseController
    {
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IMasterDataAprovalBLL _masterDataAprovalBLL;
        private IMasterDataBLL _masterBll;
        private IZaidmExProdTypeBLL _productBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IPlantBLL _plantBll;
        private Enums.MenuList _mainMenu;
        private IMaterialBLL _materialBll;
        
        public BrandRegistrationController(IBrandRegistrationBLL brandRegistrationBll, IPageBLL pageBLL, 
            IMasterDataBLL masterBll, IZaidmExProdTypeBLL productBll, IZaidmExGoodTypeBLL goodTypeBll, 
            IChangesHistoryBLL changesHistoryBll, IPlantBLL plantBll, IMaterialBLL materialBll,
            IMasterDataAprovalBLL masterDataAprovalBLL)
            : base(pageBLL, Enums.MenuList.BrandRegistration)
        {
            _brandRegistrationBll = brandRegistrationBll;
            _masterDataAprovalBLL = masterDataAprovalBLL;
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
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Controller ? true : false);
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
            model.PersonalizationCodeDescription = _masterBll.GetPersonalizationDescById(model.PersonalizationCode);
            var materialData = _materialBll.GetByPlantIdAndStickerCode(dbBrand.WERKS, dbBrand.FA_CODE);
            model.SAPBrandDescription = materialData != null ? materialData.MATERIAL_DESC : string.Empty;
            
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
            model.BahanKemasanList = GlobalFunctions.GetBahanKemasanList(_brandRegistrationBll);
            model.FaCodeList = GlobalFunctions.GetStickerCodeList();

            if (!string.IsNullOrEmpty(model.StickerCode))
            {
                var data = _materialBll.getAllPlant(model.StickerCode);
                model.PlantList = new SelectList(data, "WERKS", "WERKS - NAME1");
            }

            return model;
        }

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

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
        public JsonResult BahanKemasanList()
        {
            var bahanKemasanList = GlobalFunctions.GetBahanKemasanList(_brandRegistrationBll);
            return Json(bahanKemasanList);
        }

        [HttpPost]
        public ActionResult Create(BrandRegistrationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isExist;
                var dbBrand = new ZAIDM_EX_BRAND();

                dbBrand = Mapper.Map<ZAIDM_EX_BRAND>(model);
                if (dbBrand.STICKER_CODE.Length > 18)
                    dbBrand.STICKER_CODE = dbBrand.STICKER_CODE.Substring(0, 17);
                dbBrand.FA_CODE = model.FaCode.Trim();
                dbBrand.STICKER_CODE = model.StickerCode.Trim();
                dbBrand.CREATED_DATE = DateTime.Now;
                dbBrand.CREATED_BY = CurrentUser.USER_ID;
                dbBrand.IS_FROM_SAP = model.IsFromSAP;
                dbBrand.HJE_IDR = model.HjeValueStr == null ? 0 : Convert.ToDecimal(model.HjeValueStr);
                dbBrand.TARIFF = model.TariffValueStr == null ? 0 : Convert.ToDecimal(model.TariffValueStr);
                dbBrand.CONVERSION = model.ConversionValueStr == null ? 0 : Convert.ToDecimal(model.ConversionValueStr);
                dbBrand.PRINTING_PRICE = model.PrintingPrice == null ? 0 : Convert.ToDecimal(model.PrintingPriceValueStr);
                dbBrand.STATUS = model.IsActive;
                dbBrand.PACKED_ADJUSTED = model.IsPackedAdjusted;
                dbBrand.BAHAN_KEMASAN = string.IsNullOrEmpty(model.BahanKemasan) ? null : model.BahanKemasan.Trim();
                if (!string.IsNullOrEmpty(dbBrand.PER_CODE_DESC))
                    dbBrand.PER_CODE_DESC = model.PersonalizationCodeDescription.Split('-')[1];

                try
                {
                    MASTER_DATA_APPROVAL approvalData;
                    _masterDataAprovalBLL.MasterDataApprovalValidation((int) Enums.MenuList.BrandRegistration,
                        CurrentUser.USER_ID, new ZAIDM_EX_BRAND(), dbBrand,out isExist,out approvalData, true);
                    // AddHistoryCreate(dbBrand.WERKS, dbBrand.FA_CODE, dbBrand.STICKER_CODE);

                    //_brandRegistrationBll.Save(dbBrand);

                    _masterDataAprovalBLL.SendEmailWorkflow(approvalData.APPROVAL_ID);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    return RedirectToAction("Index");
                }
                catch (BLLException ex)
                {
                    AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                }
                catch(Exception)
                {
                    AddMessageInfo("Save Failed.", Enums.MessageInfoType.Error);
                }
            }

            InitCreate(model);

            return View(model);
        }

        private BrandRegistrationEditViewModel InitEdit(BrandRegistrationEditViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.StickerCodeList = GlobalFunctions.GetStickerCodeList();
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
            model.BahanKemasanList = GlobalFunctions.GetBahanKemasanList(_brandRegistrationBll);
            return model;
        }

        public ActionResult Edit(string plant, string facode,string stickercode)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                return RedirectToAction("Details", new { plant, facode, stickercode });
            }

            var model = new BrandRegistrationEditViewModel();


            var dbBrand = _brandRegistrationBll.GetById(plant, facode,stickercode);
          
            if (dbBrand.IS_DELETED.HasValue && dbBrand.IS_DELETED.Value)
                return RedirectToAction("Details", "BrandRegistration", new { plant = dbBrand.WERKS, facode= dbBrand.FA_CODE, stickercode=dbBrand.STICKER_CODE });

            model = Mapper.Map<BrandRegistrationEditViewModel>(dbBrand);
            model.HjeValueStr = model.HjeValue == null ? string.Empty : model.HjeValue.ToString();
            model.TariffValueStr = model.Tariff == null ? string.Empty : model.Tariff.ToString();
            model.ConversionValueStr = model.Conversion == null ? string.Empty : model.Conversion.ToString();
            model.PrintingPriceValueStr = model.PrintingPrice == null ? string.Empty : model.PrintingPrice.ToString();
            model.PersonalizationCodeDescription = _masterBll.GetPersonalizationDescById(model.PersonalizationCode);
            var materialData = _materialBll.GetByPlantIdAndStickerCode(model.PlantId, model.FaCode);
            model.SAPBrandDescription = materialData != null ? materialData.MATERIAL_DESC : string.Empty ;
            model = InitEdit(model);

            model.IsAllowDelete = !model.IsFromSAP;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BrandRegistrationEditViewModel model)
        {
            ZAIDM_EX_BRAND dbBrand = null;
            bool isApprovalExist;
            try
            {
                dbBrand = _brandRegistrationBll.GetById(model.PlantId, model.FaCode,model.StickerCode);
            }
            catch (Exception ex)
            {
                dbBrand = null;
            }
            
            var oldDbBrand = Mapper.Map<BrandRegistrationEditViewModel>(dbBrand);
            var oldObject = Mapper.Map<ZAIDM_EX_BRAND>(oldDbBrand);
            if (dbBrand == null)
            {
                ModelState.AddModelError("BrandName", "Data Not Found redirected to create form");
                var modelCreate = Mapper.Map<BrandRegistrationCreateViewModel>(model);
                modelCreate = InitCreate(modelCreate);

                return View("Create", modelCreate);
            }

            SetChangesLog(dbBrand, model);

            //if (dbBrand.IS_FROM_SAP.HasValue && dbBrand.IS_FROM_SAP.Value)
            //{
            //    dbBrand.PRINTING_PRICE = model.PrintingPrice;
            //    dbBrand.CONVERSION = model.Conversion;
            //    dbBrand.CUT_FILLER_CODE = model.CutFillerCode;
            //    dbBrand.STATUS = model.IsActive;
            //}
            //else
            Mapper.Map(model, dbBrand);
            dbBrand.HJE_IDR = model.HjeValueStr == null ? (decimal?)null : Convert.ToDecimal(model.HjeValueStr);
            dbBrand.TARIFF = model.TariffValueStr == null ? (decimal?)null : Convert.ToDecimal(model.TariffValueStr);
            dbBrand.CONVERSION = model.ConversionValueStr == null ? (decimal?)null : Convert.ToDecimal(model.ConversionValueStr);
            dbBrand.PRINTING_PRICE = model.PrintingPriceValueStr == null ? (decimal?) null : Convert.ToDecimal(model.PrintingPriceValueStr);
            dbBrand.FA_CODE = model.FaCode.Trim();
            dbBrand.STICKER_CODE = model.StickerCode.Trim();
            dbBrand.BAHAN_KEMASAN = string.IsNullOrEmpty(model.BahanKemasan) ? null : model.BahanKemasan.Trim();
            //dbBrand.PACKED_ADJUSTED = model.IsPackedAdjusted;
            //dbBrand.BRAND_CE = model.BrandName;
            //dbBrand.IS_FROM_SAP = model.IsFromSAP;
            //dbBrand.CREATED_BY = CurrentUser.USER_ID;
            if (!string.IsNullOrEmpty(model.PersonalizationCodeDescription))
                dbBrand.PER_CODE_DESC = model.PersonalizationCodeDescription;

            var materialData = _materialBll.GetByPlantIdAndStickerCode(model.PlantId, model.FaCode);
            if (materialData == null)
            {
                AddMessageInfo("Fa code and plant not registered on Material Master.", Enums.MessageInfoType.Error);
                model = InitEdit(model);

                return View("Edit", model);
            }

            try
            {
                MASTER_DATA_APPROVAL approvalData;
                dbBrand = _masterDataAprovalBLL.MasterDataApprovalValidation((int) Enums.MenuList.BrandRegistration,
                    CurrentUser.USER_ID, oldObject, dbBrand,out isApprovalExist,out approvalData);
                _brandRegistrationBll.Save(dbBrand);

                _masterDataAprovalBLL.SendEmailWorkflow(approvalData.APPROVAL_ID);
                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
                return RedirectToAction("Index");

            }
            catch(Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
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
            changesData.Add("BahanKemasan", origin.BAHAN_KEMASAN == updatedModel.BahanKemasan);

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
                    case "BahanKemasan":
                        changes.OLD_VALUE = origin.BAHAN_KEMASAN;
                        changes.NEW_VALUE = updatedModel.BahanKemasan;
                        break;
                }
                _changesHistoryBll.AddHistory(changes);
            }
        } 

        public ActionResult Delete(string plant, string facode,string stickercode)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var decodeFacode = HttpUtility.UrlDecode(facode);


            //AddHistoryDelete(plant, decodeFacode, stickercode);
            try
            {
                var isDeleted = _brandRegistrationBll.Delete(plant, decodeFacode, stickercode, CurrentUser.USER_ID);

                if (isDeleted)
                    TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Deleted;
                else
                    TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Updated;

                return RedirectToAction("Index");
            }
            catch (BLLException ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Edit", new RouteValueDictionary()
                {
                    {"plant",plant},
                    {"facode",facode},
                    {"stickercode",stickercode}
                });
            }
            
        }

        private void AddHistoryDelete(string plant, string facode, string stickercode)
        {
            var history = new CHANGES_HISTORY();
            history.FORM_TYPE_ID = Enums.MenuList.BrandRegistration;
            history.FORM_ID = plant + facode + stickercode;
            history.FIELD_NAME = "IS_DELETED";
            history.OLD_VALUE = "false";
            history.NEW_VALUE = "true";
            history.MODIFIED_DATE = DateTime.Now;
            history.MODIFIED_BY = CurrentUser.USER_ID;

            _changesHistoryBll.AddHistory(history);
        }
        private void AddHistoryCreate(string plant, string facode, string stickercode)
        {
            var history = new CHANGES_HISTORY();
            history.FORM_TYPE_ID = Enums.MenuList.BrandRegistration;
            history.FORM_ID = plant + facode + stickercode;
            history.FIELD_NAME = "NEW_DATA";
            history.OLD_VALUE = "";
            history.NEW_VALUE = "";
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

        [HttpPost]
        public JsonResult GetBrandMaterialDescription(string plant,string faCode)
        {
            var data = _materialBll.GetByPlantIdAndStickerCode(plant,faCode);
            if (data != null)
            {
                var retData = new ZAIDM_EX_MATERIAL();
                retData.STICKER_CODE = data.STICKER_CODE;
                retData.WERKS = data.WERKS;
                retData.MATERIAL_DESC = data.MATERIAL_DESC;
                return Json(retData);
            }
            else
            {
                return null;
            }
            
        }

        #region export xls

        public void ExportXlsFile()
        {
            string pathFile = "";

            pathFile = CreateXlsFile();

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsFile()
        {
            //get data
            var listData = Mapper.Map<List<BrandRegistrationDetailsViewModel>>(_brandRegistrationBll.GetAllBrands());

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Brand Registration");
            slDocument.MergeWorksheetCells(1, 1, 1, 26);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcel(slDocument);

            //create data
            slDocument = CreateDataExcel(slDocument, listData);

            var fileName = "MasterData_MasterBrandRegistration" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Sticker Code");
            slDocument.SetCellValue(iRow, 2, "Plant");
            slDocument.SetCellValue(iRow, 3, "FA Code");
            slDocument.SetCellValue(iRow, 4, "Personalization Code");
            slDocument.SetCellValue(iRow, 5, "Product Code");
            slDocument.SetCellValue(iRow, 6, "Brand Name Registration by KPPBC");
            slDocument.SetCellValue(iRow, 7, "SKEP No");
            slDocument.SetCellValue(iRow, 8, "SKEP Date");
            slDocument.SetCellValue(iRow, 9, "Series Code");
            slDocument.SetCellValue(iRow, 10, "Content");
            slDocument.SetCellValue(iRow, 11, "Market Code");
            slDocument.SetCellValue(iRow, 12, "Country");
            slDocument.SetCellValue(iRow, 13, "HJE");
            slDocument.SetCellValue(iRow, 14, "HJE Currency");
            slDocument.SetCellValue(iRow, 15, "Tariff");
            slDocument.SetCellValue(iRow, 16, "Tariff Currency");
            slDocument.SetCellValue(iRow, 17, "Exciseable Goods Type");
            slDocument.SetCellValue(iRow, 18, "Colour");
            slDocument.SetCellValue(iRow, 19, "Start Date");
            slDocument.SetCellValue(iRow, 20, "End Date");
            slDocument.SetCellValue(iRow, 21, "Printing Price");
            slDocument.SetCellValue(iRow, 22, "Convertion");
            slDocument.SetCellValue(iRow, 23, "Active");
            slDocument.SetCellValue(iRow, 24, "Cut Filler Code");
            slDocument.SetCellValue(iRow, 25, "Deleted");
            slDocument.SetCellValue(iRow, 26, "Data SAP");
            slDocument.SetCellValue(iRow, 27, "Bahan Kemasan");
            slDocument.SetCellValue(iRow, 28, "Packed Adjusted");
            slDocument.SetCellValue(iRow, 29, "Created By - Date");
            
         
        
        

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 29, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcel(SLDocument slDocument, List<BrandRegistrationDetailsViewModel> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                var personalizationCode = "";
                if (!string.IsNullOrEmpty(data.PersonalizationCode)
                    && !string.IsNullOrEmpty(data.PersonalizationCodeDescription))
                {
                    personalizationCode = data.PersonalizationCode + "-" + data.PersonalizationCodeDescription;
                }

                var productCode = "";
                if (!string.IsNullOrEmpty(data.ProductCode)
                    && !string.IsNullOrEmpty(data.ProductType)
                    && !string.IsNullOrEmpty(data.ProductAlias))
                {
                    productCode = data.ProductCode + "-" + data.ProductType + " [" + data.ProductAlias + "]";
                }

                slDocument.SetCellValue(iRow, 1, data.StickerCode);
                slDocument.SetCellValue(iRow, 2, data.PlantName);
                slDocument.SetCellValue(iRow, 3, data.FaCode);
                slDocument.SetCellValue(iRow, 4, personalizationCode);
                slDocument.SetCellValue(iRow, 5, productCode);
                slDocument.SetCellValue(iRow, 6, data.BrandName);
                slDocument.SetCellValue(iRow, 7, data.SkepNo);
                slDocument.SetCellValue(iRow, 8, ConvertHelper.ConvertDateToStringddMMMyyyy(data.SkepDate));
                slDocument.SetCellValue(iRow, 9, data.SeriesCode + "-" + data.SeriesValue);
                slDocument.SetCellValue(iRow, 10, data.Content);
                slDocument.SetCellValue(iRow, 11, data.MarketDescription);
                slDocument.SetCellValue(iRow, 12, data.CountryCode);
                slDocument.SetCellValue(iRow, 13, data.HjeValueStr);
                slDocument.SetCellValue(iRow, 14, data.HjeCurrency);
                slDocument.SetCellValue(iRow, 15, data.TariffValueStr);
                slDocument.SetCellValue(iRow, 16, data.TariffCurrency);
                slDocument.SetCellValue(iRow, 17, data.GoodType + "-" + data.GoodTypeDescription);
                slDocument.SetCellValue(iRow, 18, data.ColourName);
                slDocument.SetCellValue(iRow, 19, ConvertHelper.ConvertDateToStringddMMMyyyy(data.StartDate));
                slDocument.SetCellValue(iRow, 20, ConvertHelper.ConvertDateToStringddMMMyyyy(data.EndDate));
                slDocument.SetCellValue(iRow, 21, ConvertHelper.ConvertDecimalToStringMoneyFormat(data.PrintingPrice));
                slDocument.SetCellValue(iRow, 22, ConvertHelper.ConvertDecimalToStringMoneyFormat(data.Conversion));
                slDocument.SetCellValue(iRow, 23, data.IsActive ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 24, data.CutFilterCode);
                slDocument.SetCellValue(iRow, 25, data.IsDeleted );
                slDocument.SetCellValue(iRow, 26, data.IsFromSap.HasValue && data.IsFromSap.Value ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 27, data.BahanKemasan);
                slDocument.SetCellValue(iRow, 28, data.IsPackedAdjusted ? "Yes" : "No");
                
                slDocument.SetCellValue(iRow, 29, data.CREATED_BY + "-" + ConvertHelper.ConvertDateToStringddMMMyyyy(data.CREATED_DATE));
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 29);
            slDocument.SetCellStyle(3, 1, iRow - 1, 29, valueStyle);

            return slDocument;
        }

        #endregion
    }
}