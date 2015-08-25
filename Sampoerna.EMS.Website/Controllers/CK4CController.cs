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
        public CK4CController(IPageBLL pageBll, IPOABLL poabll, ICK4CBLL ck4Cbll, IPlantBLL plantbll, IMonthBLL monthBll,
            ICompanyBLL companyBll) : base (pageBll, Enums.MenuList.CK4C)
        {
            _ck4CBll = ck4Cbll;
            _plantBll = plantbll;
            _monthBll = monthBll;
            _plantBll = plantbll;
            _poabll = poabll;
            _mainMenu = Enums.MenuList.CK4C;
        }
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

            return View("Index",data);
        }

        private Ck4CIndexViewModel InitCk4ViewModel(Ck4CIndexViewModel model)
        {
            //model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            //model.PlanIdList = GlobalFunctions.GetPlantAll();
           return model;
        }
	}
}