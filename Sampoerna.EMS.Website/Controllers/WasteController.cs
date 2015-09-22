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

        public WasteController(IPageBLL pageBll, IWasteBLL wasteBll, ICompanyBLL companyBll, IPlantBLL plantBll,
            IUnitOfMeasurementBLL uomBll,
            IBrandRegistrationBLL brandRegistrationBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _wasteBll = wasteBll;
            _mainMenu = Enums.MenuList.CK4C;
            _companyBll = companyBll;
            _plantBll = plantBll;
            _uomBll = uomBll;
            _brandRegistrationBll = brandRegistrationBll;
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
                var company = _companyBll.GetById(model.CompanyCode);
                var plant = _plantBll.GetT001WById(model.PlantWerks);
                var brandDesc = _brandRegistrationBll.GetById(model.PlantWerks, model.FaCode);

                //get desc
                data.CompanyName = company.BUTXT;
                data.PlantName = plant.NAME1;
                data.BrandDescription = brandDesc.BRAND_CE;

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
                    _wasteBll.Save(data);
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
            model.FloorStickStr = model.FloorWasteStickQty == null ? string.Empty : model.DustWasteStickQty.ToString();

            model = IniEdit(model);

            model.CompanyCodeX = model.CompanyCode;
            model.PlantWerksX = model.PlantWerks;
            model.WasteProductionDateX = model.WasteProductionDate;
            model.FaCodeX = model.FaCode;

            return View(model);
        }

        //
        // POST: /Production/Edit
        [HttpPost]
        public ActionResult Edit(WasteDetail model)
        {

            var dbProduction = _wasteBll.GetById(model.CompanyCodeX, model.PlantWerksX, model.FaCodeX,
               Convert.ToDateTime(model.WasteProductionDateX));

            if (dbProduction == null)
            {
                ModelState.AddModelError("Waste", "Data is not Found");
                model = IniEdit(model);

                return View("Edit, model");

            }

            var dbWasteNew = Mapper.Map<WasteDto>(model);
            var company = _companyBll.GetById(model.CompanyCode);
            var plant = _plantBll.GetT001WById(model.PlantWerks);
            var brandDesc = _brandRegistrationBll.GetById(model.PlantWerks, model.FaCode);

            model.CompanyName = company.BUTXT;
            model.PlantName = plant.NAME1;
            model.BrandDescription = brandDesc.BRAND_CE;

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

                _wasteBll.Save(dbWasteNew);
                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success
                    );


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

            //reject
            model.MarkerStr = model.MarkerRejectStickQty == null ? string.Empty : model.MarkerRejectStickQty.ToString();
            model.PackerStr = model.PackerRejectStickQty == null ? string.Empty : model.PackerRejectStickQty.ToString();

            //Waste Gram
            model.DustGramStr = model.DustWasteGramQty == null ? string.Empty : model.DustWasteGramQty.ToString();
            model.FloorGramStr = model.FloorWasteGramQty == null ? string.Empty : model.FloorWasteGramQty.ToString();

            //Waste Stick
            model.DustStickStr = model.DustWasteStickQty == null ? string.Empty : model.DustWasteStickQty.ToString();
            model.FloorStickStr = model.FloorWasteStickQty == null ? string.Empty : model.DustWasteStickQty.ToString();

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
                foreach (var item in modelDto.UploadItems)
                {
                    var company = _companyBll.GetById(item.CompanyCode);
                    var plant = _plantBll.GetT001WById(item.PlantWerks);

                    item.CompanyName = company.BUTXT;
                    item.PlantName = plant.NAME1;

                    _wasteBll.SaveUpload(item);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                       );
                }
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
                    var item = new WasteUploadItems();

                    item.CompanyCode = dataRow[0];
                    item.PlantWerks = dataRow[1];
                    item.FaCode = dataRow[2];
                    item.BrandDescription = dataRow[3];
                    item.PackerRejectStickQty = Convert.ToDecimal(dataRow[4]);
                    item.MarkerRejectStickQty = Convert.ToDecimal(dataRow[5]);
                    item.DustWasteGramQty = Convert.ToDecimal(dataRow[6]);
                    item.FloorWasteGramQty = Convert.ToDecimal(dataRow[7]);
                    item.DustWasteStickQty = Convert.ToDecimal(dataRow[8]);
                    item.FloorWasteStickQty = Convert.ToDecimal(dataRow[9]);
                    item.WasteProductionDate = DateTime.FromOADate(Convert.ToDouble(data.DataRows[0][10])).ToString("dd MMM yyyy");

                    {
                        model.Add(item);
                    }

                }
            }

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