using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
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

        public ProductionController(IPageBLL pageBll, IProductionBLL productionBll, ICompanyBLL companyBll, IPlantBLL plantBll, IUnitOfMeasurementBLL uomBll,
            IBrandRegistrationBLL brandRegistrationBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _productionBll = productionBll;
            _mainMenu = Enums.MenuList.CK4C;
            _companyBll = companyBll;
            _plantBll = plantBll;
            _uomBll = uomBll;
            _brandRegistrationBll = brandRegistrationBll;
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
                
                Details = Mapper.Map<List<ProductionDetail>>(_productionBll.GetAllByParam(new ProductionGetByParamInput()))
            });
           

            return View("Index", data);
        }

        private ProductionViewModel InitProductionViewModel(ProductionViewModel model)
        {
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantAll();
           
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

            var dbData = _productionBll.GetAllByParam(input);
            var result = Mapper.Map<List<ProductionDetail>>(dbData);
            var viewModel = new ProductionViewModel();
            viewModel.Details = result;
            return PartialView("_ProductionTableIndex", viewModel);
        }
        #endregion

        #region Create
        //
        // GET: /Production/Create
        public ActionResult Create()
        {
            var model = new ProductionDetail();
            model = InitCreate(model);
            model.ProductionDate = DateTime.Today.ToString("dd MMM yyyy");

            return View(model);

        }

        private ProductionDetail InitCreate(ProductionDetail model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
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
                var company = _companyBll.GetById(model.CompanyCode);
                var plant = _plantBll.GetT001WById(model.PlantWerks);
                var brandDesc = _brandRegistrationBll.GetById(model.PlantWerks, model.FaCode);

                data.CompanyName = company.BUTXT;
                data.PlantName = plant.NAME1;
                data.BrandDescription = brandDesc.BRAND_CE;
                data.QtyPacked = model.QtyPackedStr == null ? 0 : Convert.ToDecimal(model.QtyPackedStr);
                data.QtyUnpacked = model.QtyUnpackedStr == null ? 0 : Convert.ToDecimal(model.QtyUnpackedStr);
                try
                {

                    _productionBll.Save(data);

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
            model = InitCreate(model);
            return View(model);
        }
        #endregion

        #region Edit
        //
        // GET: /Production/Edit
        public ActionResult Edit(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {

            var model = new ProductionDetail();

            var dbProduction = _productionBll.GetById(companyCode, plantWerk, faCode, productionDate);

            model = Mapper.Map<ProductionDetail>(dbProduction);



            model.QtyPackedStr = model.QtyPacked == null ? string.Empty : model.QtyPacked.ToString();
            model.QtyUnpackedStr = model.QtyUnpacked == null ? string.Empty : model.QtyUnpacked.ToString();
            model.ProdQtyStickStr = model.ProQtyStick == null ? string.Empty : model.ProQtyStick.ToString();
            model.QtyStr = model.Qty == null ? string.Empty : model.Qty.ToString();

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

            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
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

            if (dbProduction == null)
            {
                ModelState.AddModelError("Production", "Data is not Found");
                model = IniEdit(model);

                return View("Edit, model");

            }

            var dbPrductionNew = Mapper.Map<ProductionDto>(model);
            var company = _companyBll.GetById(model.CompanyCode);
            var plant = _plantBll.GetT001WById(model.PlantWerks);
            var brandDesc = _brandRegistrationBll.GetById(model.PlantWerks, model.FaCode);

            dbPrductionNew.CompanyName = company.BUTXT;
            dbPrductionNew.PlantName = plant.NAME1;
            dbPrductionNew.BrandDescription = brandDesc.BRAND_CE;

            dbPrductionNew.QtyPacked = model.QtyPackedStr == null ? 0 : Convert.ToDecimal(model.QtyPackedStr);
            dbPrductionNew.QtyUnpacked = model.QtyUnpackedStr == null ? 0 : Convert.ToDecimal(model.QtyUnpackedStr);

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

                _productionBll.Save(dbPrductionNew);
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
            model.QtyPackedStr = model.QtyPacked == null ? string.Empty : model.QtyPacked.ToString();
            model.QtyUnpackedStr = model.QtyUnpacked == null ? string.Empty : model.QtyUnpacked.ToString();
            model.ProdQtyStickStr = model.ProQtyStick == null ? string.Empty : model.ProQtyStick.ToString();
            model.QtyStr = model.Qty == null ? string.Empty : model.Qty.ToString();

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
            foreach (var item in modelDto.UploadItems)
            {
                var company = _companyBll.GetById(item.CompanyCode);
                var plant = _plantBll.GetT001WById(item.PlantWerks);
               
                item.CompanyName = company.BUTXT;
                item.PlantName = plant.NAME1;
                
                _productionBll.SaveUpload(item);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                       );
                }
                
            }
            
            catch (Exception ex)
            {
                AddMessageInfo("Error, Data is not Valid", Enums.MessageInfoType.Error);
            }

            return RedirectToAction("Index");
                       
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<ProductionUploadItems>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    var item = new ProductionUploadItems();

                    item.CompanyCode = dataRow[0];
                    item.PlantWerks = dataRow[1];
                    item.FaCode = dataRow[2];
                    item.BrandDescription = dataRow[3];
                    item.QtyPacked = Convert.ToDecimal(dataRow[4]);
                    item.QtyUnpacked = Convert.ToDecimal(dataRow[5]);
                    item.Uom = dataRow[6];
                    item.ProductionDate = DateTime.FromOADate(Convert.ToDouble(data.DataRows[0][7])).ToString("dd MMM yyyy");

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

            var model = new ProductionDetail() { PlantWerkList = listPlant };

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


    }
}