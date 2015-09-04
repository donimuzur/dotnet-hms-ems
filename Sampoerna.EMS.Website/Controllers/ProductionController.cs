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
               
                data.CompanyName = company.BUTXT;
                data.PlantName = plant.NAME1;

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