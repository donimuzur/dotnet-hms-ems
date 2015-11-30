using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK4C;
using Sampoerna.EMS.Website.Models.PRODUCTION;
using Sampoerna.EMS.Website.Utility;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ProductionController : BaseController
    {
        private IProductionBLL _productionBll;
        private Enums.MenuList _mainMenu;
        private ICompanyBLL _companyBll;
        private IPlantBLL _plantBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IChangesHistoryBLL _changeHistoryBll;
        private IUserPlantMapBLL _userPlantMapBll;
        private IPOAMapBLL _poaMapBll;

        public ProductionController(IPageBLL pageBll, IProductionBLL productionBll, ICompanyBLL companyBll, IPlantBLL plantBll, IUnitOfMeasurementBLL uomBll,
            IBrandRegistrationBLL brandRegistrationBll, IChangesHistoryBLL changeHistorybll, IUserPlantMapBLL userPlantMapBll, IPOAMapBLL poaMapBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _productionBll = productionBll;
            _mainMenu = Enums.MenuList.CK4C;
            _companyBll = companyBll;
            _plantBll = plantBll;
            _uomBll = uomBll;
            _brandRegistrationBll = brandRegistrationBll;
            _changeHistoryBll = changeHistorybll;
            _userPlantMapBll = userPlantMapBll;
            _poaMapBll = poaMapBll;
        }

        #region Index
        //
        // GET: /Production/Index
        public ActionResult Index()
        {
            var data = InitProductionViewModel(new ProductionViewModel()
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.DailyProduction,
                ProductionDate = DateTime.Today.ToString("dd MMM yyyy"),
                IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer
            });

            return View("Index", data);
        }

        private ProductionViewModel InitProductionViewModel(ProductionViewModel model)
        {
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantAll();

            var input = Mapper.Map<ProductionGetByParamInput>(model);
            input.ProoductionDate = null;
            input.UserId = CurrentUser.USER_ID;

            var dbData = _productionBll.GetAllByParam(input);

            model.Details = Mapper.Map<List<ProductionDetail>>(dbData);

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterProductionIndex(ProductionViewModel model)
        {
            var input = Mapper.Map<ProductionGetByParamInput>(model);
            if (input.ProoductionDate != null)
            {
                input.ProoductionDate = Convert.ToDateTime(input.ProoductionDate).ToString();
            }
            input.UserId = CurrentUser.USER_ID;

            var dbData = _productionBll.GetAllByParam(input);
            var result = Mapper.Map<List<ProductionDetail>>(dbData);
            var viewModel = new ProductionViewModel();
            viewModel.Details = result;
            viewModel.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            return PartialView("_ProductionTableIndex", viewModel);
        }
        #endregion

        #region Create
        //
        // GET: /Production/Create
        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new ProductionDetail();
            model = InitCreate(model);
            model.ProductionDate = DateTime.Today.ToString("dd MMM yyyy");

            return View(model);

        }

        private ProductionDetail InitCreate(ProductionDetail model)
        {
            var company = GlobalFunctions.GetCompanyList(_companyBll);
            var userPlantCompany = _userPlantMapBll.GetCompanyByUserId(CurrentUser.USER_ID);
            var poaMapCompany = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
            var distinctCompany = company.Where(x => userPlantCompany.Contains(x.Value) || poaMapCompany.Contains(x.Value));
            var getCompany = new SelectList(distinctCompany, "Value", "Text");


            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyCodeList = getCompany;
            model.PlantWerkList = GlobalFunctions.GetPlantByCompanyId("");
            model.FacodeList = GlobalFunctions.GetFaCodeByPlant("");
            model.UomList = GlobalFunctions.GetUomStickGram(_uomBll);

            return model;

        }
        //
        // POST: /Production/Edit
        [HttpPost]
        public ActionResult Create(ProductionDetail model)
        {
            if (ModelState.IsValid)
            {
                var existingData = _productionBll.GetExistDto(model.CompanyCode, model.PlantWerks, model.FaCode,
                    Convert.ToDateTime(model.ProductionDate));
                if (existingData != null)
                {
                    AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Edit", "Production", new
                    {
                        companyCode = model.CompanyCode,
                        plantWerk = model.PlantWerks,
                        faCode = model.FaCode,
                        productionDate = model.ProductionDate
                    });
                }

                var data = Mapper.Map<ProductionDto>(model);

                try
                {

                    _productionBll.Save(data, CurrentUser.USER_ID);

                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                         );
                    return RedirectToAction("Index");

                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error
                            );

                }

            }
            else
            {
                var errorlist = ModelState.Values.Select(x => x.Errors).Single();

                var errMsg = "";

                foreach (var error in errorlist)
                {
                    errMsg = error.ErrorMessage + "\n";
                }
                AddMessageInfo(errMsg, Enums.MessageInfoType.Error
                           );
            }
            model = InitCreate(model);
            return View(model);
        }
        #endregion

        #region Edit
        //
        // GET: /Production/Edit
        public ActionResult Edit(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Edit", "Production", new
                {
                    companyCode = companyCode,
                    plantWerk = plantWerk,
                    faCode = faCode,
                    productionDate = productionDate
                });
            }

            var model = new ProductionDetail();

            var dbProduction = _productionBll.GetById(companyCode, plantWerk, faCode, productionDate);

            model = Mapper.Map<ProductionDetail>(dbProduction);
            model = IniEdit(model);
            model.CompanyCodeX = model.CompanyCode;
            model.PlantWerksX = model.PlantWerks;
            model.ProductionDateX = model.ProductionDate;
            model.FaCodeX = model.FaCode;

            return View(model);
        }

        private ProductionDetail IniEdit(ProductionDetail model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var company = GlobalFunctions.GetCompanyList(_companyBll);
            var userPlantCompany = _userPlantMapBll.GetCompanyByUserId(CurrentUser.USER_ID);
            var poaMapCompany = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
            var distinctCompany = company.Where(x => userPlantCompany.Contains(x.Value) || poaMapCompany.Contains(x.Value));
            var getCompany = new SelectList(distinctCompany, "Value", "Text");

            model.CompanyCodeList = getCompany;
            model.PlantWerkList = GlobalFunctions.GetPlantByCompanyId("");
            model.FacodeList = GlobalFunctions.GetFaCodeByPlant("");
            model.UomList = GlobalFunctions.GetUomStickGram(_uomBll);

            return model;
        }

        //
        // POST: /Production/Edit
        [HttpPost]
        public ActionResult Edit(ProductionDetail model)
        {

            var dbProduction = _productionBll.GetById(model.CompanyCodeX, model.PlantWerksX, model.FaCodeX,
               Convert.ToDateTime(model.ProductionDateX));
            try
            {
                if (dbProduction == null)
                {
                    ModelState.AddModelError("Production", "Data is not Found");
                    model = IniEdit(model);

                    return View("Edit", model);
                }

                if (model.CompanyCode != model.CompanyCodeX || model.PlantWerks != model.PlantWerksX
                    || model.FaCode != model.FaCodeX || Convert.ToDateTime(model.ProductionDate) != Convert.ToDateTime(model.ProductionDateX))
                {
                    var existingData = _productionBll.GetExistDto(model.CompanyCode, model.PlantWerks, model.FaCode,
                        Convert.ToDateTime(model.ProductionDate));
                    if (existingData != null)
                    {
                        AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);
                        return RedirectToAction("Edit", "Production", new
                        {
                            companyCode = model.CompanyCode,
                            plantWerk = model.PlantWerks,
                            faCode = model.FaCode,
                            productionDate = model.ProductionDate
                        });
                    }
                }

                var dbPrductionNew = Mapper.Map<ProductionDto>(model);

                if (!ModelState.IsValid)
                {
                    var error = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();
                    if (error.Count > 0)
                    {
                        //
                    }
                }

                var output = _productionBll.Save(dbPrductionNew, CurrentUser.USER_ID);
                var message = Constans.SubmitMessage.Updated;

                if (output.isNewData)
                    message = Constans.SubmitMessage.Saved;

                if (!output.isFromSap)
                {
                    if (model.CompanyCode != model.CompanyCodeX || model.PlantWerks != model.PlantWerksX || model.FaCode != model.FaCodeX
                        || Convert.ToDateTime(model.ProductionDate) != Convert.ToDateTime(model.ProductionDateX))
                    {
                        MoveOldChangeLogHistory(dbPrductionNew);
                        _productionBll.DeleteOldData(model.CompanyCodeX, model.PlantWerksX, model.FaCodeX, Convert.ToDateTime(model.ProductionDateX));
                    }
                }

                AddMessageInfo(message, Enums.MessageInfoType.Success);

                return RedirectToAction("Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }

            model = IniEdit(model);

            return View("Edit", model);

        }

        #endregion

        #region Detail

        private ProductionDetail InitDetail(ProductionDetail model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantAll();
            model.FacodeList = GlobalFunctions.GetBrandList();
            model.UomList = GlobalFunctions.GetUomStickGram(_uomBll);

            return model;
        }


        //
        // GET: /Production/Detail
        public ActionResult Detail(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {
            var model = new ProductionDetail();

            var dbProduction = _productionBll.GetById(companyCode, plantWerk, faCode, productionDate);

            model = Mapper.Map<ProductionDetail>(dbProduction);

            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(_changeHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK4C,
                  "Daily_" + companyCode + "_" + plantWerk + "_" + faCode + "_" + productionDate.ToString("ddMMMyyyy")));

            model = InitDetail(model);

            return View(model);
        }

        #endregion

        #region Upload file

        public ActionResult UploadManualProduction()
        {
            var model = new ProductionUploadViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View(model);
        }
        [HttpPost]
        public ActionResult UploadManualProduction(ProductionUploadViewModel model)
        {
            var modelDto = Mapper.Map<ProductionDto>(model);

            try
            {
                var listProduction = new List<ProductionUploadItems>();

                //check validation
                foreach (var item in modelDto.UploadItems)
                {
                    var company = _companyBll.GetById(item.CompanyCode);
                    var plant = _plantBll.GetT001WById(item.PlantWerks);
                    var brandCe = _brandRegistrationBll.GetById(item.PlantWerks, item.FaCode);

                    if (brandCe.IS_DELETED == true || (brandCe.STATUS == false || brandCe.STATUS == null))
                    {
                        AddMessageInfo("Data Brand Description Is Inactive", Enums.MessageInfoType.Error);
                        return RedirectToAction("UploadManualProduction");
                    }

                    if (item.Uom == "TH")
                    {
                        item.Uom = "Btg";
                        item.QtyPacked = Convert.ToString(Convert.ToDecimal(item.QtyPacked) * 1000);
                        item.Qty = Convert.ToString(Convert.ToDecimal(item.Qty) * 1000);
                    }

                    if (item.Uom == "KG")
                    {
                        item.Uom = "G";
                        item.QtyPacked = Convert.ToString(Convert.ToDecimal(item.QtyPacked) * 1000);
                        item.Qty = Convert.ToString(Convert.ToDecimal(item.Qty) * 1000);
                    }

                    item.CompanyName = company.BUTXT;
                    item.PlantName = plant.NAME1;

                    if (item.BrandDescription != brandCe.BRAND_CE)
                    {
                        AddMessageInfo("Data Brand Description Is Not valid", Enums.MessageInfoType.Error);
                        return RedirectToAction("UploadManualProduction");
                    }

                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;

                    var existingData = _productionBll.GetExistDto(item.CompanyCode, item.PlantWerks, item.FaCode,
                        Convert.ToDateTime(item.ProductionDate));

                    if (existingData != null)
                    {
                        AddMessageInfo("Data Already Exist, Please Check Data Company Code," +
                                       " Plant Code, Fa Code, and Waste Production Date", Enums.MessageInfoType.Warning);
                        return RedirectToAction("UploadManualProduction");
                    }

                    listProduction.Add(item);
                }

                //do save
                foreach (var data in listProduction)
                {
                    _productionBll.SaveUpload(data, CurrentUser.USER_ID);
                }

                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

            }

            catch (Exception ex)
            {

                AddMessageInfo("Error, Data is not Valid", Enums.MessageInfoType.Error);
                return RedirectToAction("UploadManualProduction");

            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<ProductionUploadItems>();
            if (data != null)
            {
                DateTime temp;
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }


                    var item = new ProductionUploadItems();
                    var brand = _brandRegistrationBll.GetByFaCode(dataRow[1], dataRow[2]);

                    item.CompanyCode = dataRow[0];
                    item.PlantWerks = dataRow[1];
                    item.FaCode = dataRow[2];
                    item.BrandDescription = brand == null ? string.Empty : brand.BRAND_CE;
                    item.QtyPacked = dataRow[3];
                    item.Qty = dataRow[4];
                    item.Uom = dataRow[5];
                    item.ProductionDate = dataRow[6];

                    model.Add(item);





                }
            }
            var input = Mapper.Map<List<ProductionUploadItemsInput>>(model);
            var outputResult = _productionBll.ValidationDailyUploadDocumentProcess(input, qtyPacked, qty);

            model = Mapper.Map<List<ProductionUploadItems>>(outputResult);
            return Json(model);


        }


        #endregion

        #region Json
        [HttpPost]
        public JsonResult CompanyListPartialProduction(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);
            var userPlnatMap = _userPlantMapBll.GetPlantByUserId(CurrentUser.USER_ID);
            var poaMap = _poaMapBll.GetPlantByPoaId(CurrentUser.USER_ID);
            var distinctPlant = listPlant.Where(x => userPlnatMap.Contains(x.Value) || poaMap.Contains(x.Value));
            var listPlanNew = new SelectList(distinctPlant, "Value", "Text");

            var model = new ProductionDetail() { PlantWerkList = listPlanNew };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetFaCodeDescription(string plantWerk, string faCode)
        {
            var fa = _brandRegistrationBll.GetByFaCode(plantWerk, faCode);
            return Json(fa.BRAND_CE);
        }

        [HttpPost]
        public JsonResult GetBrandCeByPlant(string plantWerk)
        {
            var listBrandCe = GlobalFunctions.GetFaCodeByPlant(plantWerk);

            var model = new ProductionDetail() { FacodeList = listBrandCe };

            return Json(model);
        }



        #endregion

        private void MoveOldChangeLogHistory(ProductionDto item)
        {
            DateTime productionDateX = Convert.ToDateTime(item.ProductionDateX);
            DateTime productionDate = Convert.ToDateTime(item.ProductionDate);

            var listHistory = _changeHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK4C,
                  "Daily_" + item.CompanyCodeX + "_" + item.PlantWerksX + "_" + item.FaCodeX + "_" + productionDateX.ToString("ddMMMyyyy"));

            var oldFormId = "Daily_" + item.CompanyCode + "_" + item.PlantWerks + "_" + item.FaCode + "_" + productionDate.ToString("ddMMMyyyy");

            foreach (var data in listHistory)
            {
                _changeHistoryBll.MoveHistoryToNewData(data, oldFormId);
            }
        }
    }
}