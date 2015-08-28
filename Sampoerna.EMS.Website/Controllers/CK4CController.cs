using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.CK4C;

namespace Sampoerna.EMS.Website.Controllers
{
    public class CK4CController : BaseController
    {
        private ICK4CBLL _ck4CBll;
        private IMonthBLL _monthBll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poabll;
        private ICompanyBLL _companyBll;
        private IPlantBLL _plantBll;
        private IT001KBLL _t001KBll;
        public CK4CController(IPageBLL pageBll, IPOABLL poabll, ICK4CBLL ck4Cbll, IPlantBLL plantbll, IMonthBLL monthBll,
            ICompanyBLL companyBll, IT001KBLL t001Kbll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _ck4CBll = ck4Cbll;
            _plantBll = plantbll;
            _monthBll = monthBll;
            _plantBll = plantbll;
            _poabll = poabll;
            _companyBll = companyBll;
            _mainMenu = Enums.MenuList.CK4C;
            _t001KBll = t001Kbll;
        }


        #region Index Daily Production
        //
        // GET: /CK4C/
        public ActionResult Index()
        {
            var data = InitCk4ViewModel(new Ck4CIndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.DailyProduction,
                Detail = Mapper.Map<List<DataIndecCk4C>>(_ck4CBll.GetAllByParam(new Ck4CGetByParamInput()))

            });

            return View("Index", data);
        }

        private Ck4CIndexViewModel InitCk4ViewModel(Ck4CIndexViewModel model)
        {
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlanIdList = GlobalFunctions.GetPlantAll();
            return model;
        }

        #endregion

        #region Index Waste Production

        public ActionResult WasteProductionIndex()
        {
            var data =
            InitIndexWasteProductionViewModel(new Ck4CIndexWasteProductionViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.WasteProduction,
            });

            return View("WasteProductionIndex", data);
        }

        private Ck4CIndexWasteProductionViewModel InitIndexWasteProductionViewModel(
            Ck4CIndexWasteProductionViewModel model)
        {
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlanIdList = GlobalFunctions.GetPlantAll();
            return model;
        }
        #endregion

        #region Json
        [HttpPost]
        public JsonResult CompanyListPartialCk4C(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompany(companyId);

            var model = new Ck4CIndexViewModel() { PlanIdList = listPlant };

            return Json(model);

        }

        [HttpPost]
        public JsonResult CompanyListPartialCk4CWaste(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompany(companyId);

            var model = new Ck4CIndexWasteProductionViewModel() { PlanIdList = listPlant };

            return Json(model);

        }


        #endregion


        #region create daily Production

        public ActionResult Ck4CCreateDailyProduction()
        {
            var model = new Ck4cCreateViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(Ck4cCreateViewModel model)
        {
            return View("Ck4CCreateDailyProduction", InitialModel(model));
        }

        private Ck4cCreateViewModel InitialModel(Ck4cCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return (model);

        }

        #endregion 
        #region create Waste Production

        public ActionResult Ck4CCreateWasteProduction()
        {
            var model = new Ck4CCreateWasteProductionViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(Ck4CCreateWasteProductionViewModel model)
        {
            return View("Ck4CCreateWasteProduction", InitialModel(model));
        }

        private Ck4CCreateWasteProductionViewModel InitialModel(Ck4CCreateWasteProductionViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return (model);

        }

        #endregion 
    }
}