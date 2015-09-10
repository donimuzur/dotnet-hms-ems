using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.Waste;

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

        #endregion


    }
}