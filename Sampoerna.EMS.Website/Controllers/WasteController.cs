using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PRODUCTION;
using Sampoerna.EMS.Website.Models.Waste;
using Sampoerna.EMS.Website.Utility;

namespace Sampoerna.EMS.Website.Controllers
{
    public class WasteController : BaseController
    {

        private IWasteBLL _wasteBll;
        private Enums.MenuList _mainMenu;
        private ICompanyBLL _companyBll;
        private IPlantBLL _plantBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IChangesHistoryBLL _changeHistoryBll;

        public WasteController(IPageBLL pageBll, IWasteBLL wasteBll, ICompanyBLL companyBll, IPlantBLL plantBll,
            IUnitOfMeasurementBLL uomBll, IBrandRegistrationBLL brandRegistrationBll, IChangesHistoryBLL changesHistoryBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _wasteBll = wasteBll;
            _mainMenu = Enums.MenuList.CK4C;
            _companyBll = companyBll;
            _plantBll = plantBll;
            _uomBll = uomBll;
            _brandRegistrationBll = brandRegistrationBll;
            _changeHistoryBll = changesHistoryBll;
        }


        #region Index

        private WasteViewModel InitIndexViewModel(WasteViewModel model)
        {
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerksList = GlobalFunctions.GetPlantAll();

            return model;
        }

        //
        // GET: /Waste/
        public ActionResult Index()
        {
            var data = InitIndexViewModel(new WasteViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.DailyProduction,
                WasteProductionDate = DateTime.Today.ToString("dd MMM yyyy"),
                Details = Mapper.Map<List<WasteDetail>>(_wasteBll.GetAllByParam(new WasteGetByParamInput()))


            });

            return View("Index", data);
        }

        [HttpPost]
        public PartialViewResult FilterWasteIndex(WasteViewModel model)
        {
            var input = Mapper.Map<WasteGetByParamInput>(model);
            if (input.WasteProductionDate != null)
            {
                input.WasteProductionDate = Convert.ToDateTime(input.WasteProductionDate).ToString();
            }

            var dbData = _wasteBll.GetAllByParam(input);
            var result = Mapper.Map<List<WasteDetail>>(dbData);
            var viewModel = new WasteViewModel();
            viewModel.Details = result;
            return PartialView("_WasteTableIndex", viewModel);
        }

        [HttpPost]
        public PartialViewResult FilterProductionIndex(WasteViewModel model)
        {
            var input = Mapper.Map<WasteGetByParamInput>(model);
            if (input.WasteProductionDate != null)
            {
                input.WasteProductionDate = Convert.ToDateTime(input.WasteProductionDate).ToString();
            }

            var dbData = _wasteBll.GetAllByParam(input);
            var result = Mapper.Map<List<WasteDetail>>(dbData);
            var viewModel = new WasteViewModel();
            viewModel.Details = result;
            return PartialView("_WasteTableIndex", viewModel);
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            var model = new WasteDetail();
            model = InitCreate(model);
            model.WasteProductionDate = DateTime.Today.ToString("dd MMM yyyy");

            return View(model);
        }

        private WasteDetail InitCreate(WasteDetail model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantByCompanyId("");
            model.FacodeList = GlobalFunctions.GetFaCodeByPlant("");

            return model;

        }

        [HttpPost]
        public ActionResult Create(WasteDetail model)
        {
            if (ModelState.IsValid)
            {
                var existingData = _wasteBll.GetExistDto(model.CompanyCode, model.PlantWerks, model.FaCode,
                    Convert.ToDateTime(model.WasteProductionDate));
                if (existingData != null)
                {
                    AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Edit", "Waste", new
                    {
                        companyCode = model.CompanyCode,
                        plantWerk = model.PlantWerks,
                        faCode = model.FaCode,
                        wasteProductionDate = model.WasteProductionDate
                    });
                }

                var data = Mapper.Map<WasteDto>(model);
               

                //waste reject
                data.MarkerRejectStickQty = model.MarkerStr == null ? 0 : Convert.ToDecimal(model.MarkerStr);
                data.PackerRejectStickQty = model.PackerStr == null ? 0 : Convert.ToDecimal(model.PackerStr);
                //waste gram
                data.DustWasteGramQty = model.DustGramStr == null ? 0 : Convert.ToDecimal(model.DustGramStr);
                data.FloorWasteGramQty = model.FloorGramStr == null ? 0 : Convert.ToDecimal(model.FloorGramStr);
                //waste stick
                data.DustWasteStickQty = model.DustStickStr == null ? 0 : Convert.ToDecimal(model.DustStickStr);
                data.FloorWasteStickQty = model.FloorStickStr == null ? 0 : Convert.ToDecimal(model.FloorStickStr);

                try
                {
                    _wasteBll.Save(data, CurrentUser.USER_ID);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);

                    return RedirectToAction("Index");
                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                }
            }
            model = InitCreate(model);
            return View(model);
        }
        #endregion

        #region Edit

        private WasteDetail IniEdit(WasteDetail model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantByCompanyId("");
            model.FacodeList = GlobalFunctions.GetFaCodeByPlant("");

            return model;
        }

        //
        // GET: /Production/Edit
        public ActionResult Edit(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            var model = new WasteDetail();
            var dbWaste = _wasteBll.GetById(companyCode, plantWerk, faCode, wasteProductionDate);

            model = Mapper.Map<WasteDetail>(dbWaste);
            //Reject
            model.MarkerStr = model.MarkerRejectStickQty == null ? string.Empty : model.MarkerRejectStickQty.ToString();
            model.PackerStr = model.PackerRejectStickQty == null ? string.Empty : model.PackerRejectStickQty.ToString();
            // Waste Gram
            model.DustGramStr = model.DustWasteGramQty == null ? string.Empty : model.DustWasteGramQty.ToString();
            model.FloorGramStr = model.FloorWasteGramQty == null ? string.Empty : model.FloorWasteGramQty.ToString();
            //Waste Stick
            model.DustStickStr = model.DustWasteStickQty == null ? string.Empty : model.DustWasteStickQty.ToString();
            model.FloorStickStr = model.FloorWasteStickQty == null ? string.Empty : model.FloorWasteStickQty.ToString();

            model = IniEdit(model);

            model.CompanyCodeX = model.CompanyCode;
            model.PlantWerksX = model.PlantWerks;
            model.WasteProductionDateX = model.WasteProductionDate;
            model.FaCodeX = model.FaCode;

            return View(model);
        }

        //
        // POST: /Waste/Edit
        [HttpPost]
        public ActionResult Edit(WasteDetail model)
        {

            var dbWaste = _wasteBll.GetById(model.CompanyCodeX, model.PlantWerksX, model.FaCodeX,
               Convert.ToDateTime(model.WasteProductionDateX));

            if (dbWaste == null)
            {
                ModelState.AddModelError("Waste", "Data is not Found");
                model = IniEdit(model);

                return View("Edit", model);

            }

            if (model.CompanyCode != model.CompanyCodeX || model.PlantWerks != model.PlantWerksX
                || model.FaCode != model.FaCodeX || Convert.ToDateTime(model.WasteProductionDate) != Convert.ToDateTime(model.WasteProductionDateX))
            {
                var existingData = _wasteBll.GetExistDto(model.CompanyCode, model.PlantWerks, model.FaCode,
                    Convert.ToDateTime(model.WasteProductionDate));
                if (existingData != null)
                {
                    AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Edit", "Waste", new
                    {
                        companyCode = model.CompanyCode,
                        plantWerk = model.PlantWerks,
                        faCode = model.FaCode,
                        wasteProductionDate = model.WasteProductionDate
                    });
                }
            }

            var dbWasteNew = Mapper.Map<WasteDto>(model);
            
            //reject
            dbWasteNew.MarkerRejectStickQty = model.MarkerStr == null ? 0 : Convert.ToDecimal(model.MarkerStr);
            dbWasteNew.PackerRejectStickQty = model.PackerStr == null ? 0 : Convert.ToDecimal(model.PackerStr);
            //waste gram
            dbWasteNew.DustWasteGramQty = model.DustGramStr == null ? 0 : Convert.ToDecimal(model.DustGramStr);
            dbWasteNew.FloorWasteGramQty = model.FloorGramStr == null ? 0 : Convert.ToDecimal(model.FloorGramStr);
            //waste stick
            dbWasteNew.DustWasteStickQty = model.DustStickStr == null ? 0 : Convert.ToDecimal(model.DustStickStr);
            dbWasteNew.FloorWasteStickQty = model.FloorStickStr == null ? 0 : Convert.ToDecimal(model.FloorStickStr);

            try
            {
                if (!ModelState.IsValid)
                {
                    var error = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();
                    if (error.Count > 0)
                    {
                        //
                    }
                }

                var isNewData = _wasteBll.Save(dbWasteNew, CurrentUser.USER_ID);
                var message = Constans.SubmitMessage.Updated;

                if (isNewData)
                {
                    _wasteBll.DeleteOldData(model.CompanyCodeX, model.PlantWerksX, model.FaCodeX,
               Convert.ToDateTime(model.WasteProductionDateX));
                }

                AddMessageInfo(message, Enums.MessageInfoType.Success);


                return RedirectToAction("Index");

            }
            catch (Exception exception)
            {
                AddMessageInfo("Edit Failed.", Enums.MessageInfoType.Error
                    );
            }

            model = IniEdit(model);

            return View("Edit", model);

        }




        #endregion

        #region Detail

        private WasteDetail InitDetail(WasteDetail model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantAll();
            model.FacodeList = GlobalFunctions.GetBrandList();

            return model;
        }

        public ActionResult Detail(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            var model = new WasteDetail();
            var dbWaste = _wasteBll.GetById(companyCode, plantWerk, faCode, wasteProductionDate);

            model = Mapper.Map<WasteDetail>(dbWaste);
            model.ChangesHistoryList =
               Mapper.Map<List<ChangesHistoryItemModel>>(_changeHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK4C,
                   "Waste_" + companyCode + "_" + plantWerk + "_" + faCode + "_" + wasteProductionDate.ToString("ddMMMyyyy")));

            //reject
            model.MarkerStr = model.MarkerRejectStickQty == null ? string.Empty : model.MarkerRejectStickQty.ToString();
            model.PackerStr = model.PackerRejectStickQty == null ? string.Empty : model.PackerRejectStickQty.ToString();

            //Waste Gram
            model.DustGramStr = model.DustWasteGramQty == null ? string.Empty : model.DustWasteGramQty.ToString();
            model.FloorGramStr = model.FloorWasteGramQty == null ? string.Empty : model.FloorWasteGramQty.ToString();

            //Waste Stick
            model.DustStickStr = model.DustWasteStickQty == null ? string.Empty : model.DustWasteStickQty.ToString();
            model.FloorStickStr = model.FloorWasteStickQty == null ? string.Empty : model.FloorWasteStickQty.ToString();

            model = InitDetail(model);
            return View(model);

        }
        #endregion

        #region Upload

        public ActionResult UploadManualWaste()
        {
            var model = new WasteUploadViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View(model);

        }

        [HttpPost]
        public ActionResult UploadManualWaste(WasteUploadViewModel model)
        {
            var modelDto = Mapper.Map<WasteDto>(model);

            try
            {
                var listWaste = new List<WasteUploadItems>();

                //check validation
                foreach (var item in modelDto.UploadItems)
                {
                    var company = _companyBll.GetById(item.CompanyCode);
                    var plant = _plantBll.GetT001WById(item.PlantWerks);
                    var brandCe = _brandRegistrationBll.GetById(item.PlantWerks, item.FaCode);

                    item.CompanyName = company.BUTXT;
                    item.PlantName = plant.NAME1;

                    if (item.BrandDescription != brandCe.BRAND_CE)
                    {
                        AddMessageInfo("Data Brand Description Is Not valid",Enums.MessageInfoType.Error);
                        return RedirectToAction("UploadManualWaste");
                    }

                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = CurrentUser.USER_ID;

                    var existingData = _wasteBll.GetExistDto(item.CompanyCode, item.PlantWerks, item.FaCode,
                    Convert.ToDateTime(item.WasteProductionDate));

                    if (existingData != null)
                    {
                        AddMessageInfo("Data Already Exist, Please Check Data Company Code, Plant Code, Fa Code, and Production Date", Enums.MessageInfoType.Warning);
                        return RedirectToAction("UploadManualWaste");
                    }

                    listWaste.Add(item);
                }

                //do save
                foreach(var data in listWaste)
                {
                    _wasteBll.SaveUpload(data);
                }

                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
            }

            catch (Exception ex)
            {
                AddMessageInfo("Error, Data is not Valid", Enums.MessageInfoType.Error);
                return RedirectToAction("UploadManualWaste");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<WasteUploadItems>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }

                    var item = new WasteUploadItems();
                   

                    item.CompanyCode = dataRow[0];
                    item.PlantWerks = dataRow[1];
                    item.FaCode = dataRow[2];
                    item.BrandDescription = dataRow[3];
                    item.MarkerRejectStickQty = dataRow[4];
                    item.PackerRejectStickQty = dataRow[5];
                    item.DustWasteGramQty = dataRow[6];
                    item.FloorWasteGramQty = dataRow[7];
                    item.DustWasteStickQty = dataRow[8];
                    item.FloorWasteStickQty = dataRow[9];
                    item.WasteProductionDate = dataRow[10]; 
                   
                    {
                        model.Add(item);
                    }
                    
                }
            }

            var input = Mapper.Map<List<WasteUploadItemsInput>>(model);
            var outputResult = _wasteBll.ValidationWasteUploadDocumentProcess(input);

            model = Mapper.Map<List<WasteUploadItems>>(outputResult);
            return Json(model);


        }

        #endregion

        #region Json
        [HttpPost]
        public JsonResult CompanyListPartialProduction(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var model = new WasteDetail() { PlantWerkList = listPlant };

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

            var model = new WasteDetail() { FacodeList = listBrandCe };

            return Json(model);
        }

        #endregion

    }
}