using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.CK4C;
using Sampoerna.EMS.Website.Models.PRODUCTION;

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
        //
        // GET: /Production/
        public ActionResult Index()
        {
            var data = InitProductionViewModel(new ProductionViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.DailyProduction,
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

        #region Create
        public ActionResult Create()
        {
            var model = new ProductionDetail();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantAll();
            model.FacodeList = GlobalFunctions.GetBrandList();
            model.UomList = GlobalFunctions.GetUomList(_uomBll);

            return View(model);

        }

        [HttpPost]
        public ActionResult Create(ProductionDetail model)
        {
            try
            {
                // TODO: Add insert logic here
                var data = Mapper.Map<ProductionDto>(model);
                var company = _companyBll.GetById(model.CompanyCode);
                var plant = _plantBll.GetT001ById(model.PlantWerks);
                var brandDesc = _brandRegistrationBll.GetById(model.PlantWerks, model.FaCode);

                data.CompanyName = company.BUTXT;
                data.PlantName = plant.NAME1;
                data.BrandDescription = brandDesc.BRAND_CE;
                
                _productionBll.Save(data);

                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                     );
                return RedirectToAction("Index");
               
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error
                        );
                return View(model);
            }
            

        }
        #endregion

        #region Edit
        public ActionResult Edit(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {

            var model = new ProductionDetail();
            
            var dbProduction = _productionBll.GetById(companyCode, plantWerk,faCode, productionDate);

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

            model.CompanyCodeList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantWerkList = GlobalFunctions.GetPlantAll();
            model.FacodeList = GlobalFunctions.GetBrandList();
            model.UomList = GlobalFunctions.GetUomList(_uomBll);

            return model;
        }

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

            dbProduction.QtyPacked = model.QtyPackedStr == null ? 0 : Convert.ToDecimal(model.QtyPackedStr);
            dbProduction.QtyUnpacked = model.QtyUnpackedStr == null ? 0 : Convert.ToDecimal(model.QtyUnpackedStr);

            try
            {
                _productionBll.Save(dbProduction);
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

        public ActionResult Detail(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {
            var model = new ProductionDetail();

            var dbProduction = _productionBll.GetById(companyCode, plantWerk, faCode, productionDate);

            model = Mapper.Map<ProductionDetail>(dbProduction);
            model = IniEdit(model);

            return View(model);
        }
        
        #endregion  



        #region Json
        [HttpPost]
        public JsonResult CompanyListPartialProduction(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompany(companyId);

            var model = new Ck4CIndexViewModel() { PlanIdList = listPlant };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetFaCodeDescription(string faCode)
        {
            var fa = _brandRegistrationBll.GetByFaCode(faCode);
            return Json(fa.BRAND_CE);
        }

        #endregion
    }
}