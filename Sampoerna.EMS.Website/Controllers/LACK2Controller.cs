using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.LACK2;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Code;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK2Controller : BaseController
    {

        private ILACK2BLL _lack2Bll;
        private Enums.MenuList _mainMenu;

        public LACK2Controller(IPageBLL pageBll, ILACK2BLL lack2Bll)
            : base(pageBll, Enums.MenuList.LACK2)
        {
            _lack2Bll = lack2Bll;
            _mainMenu = Enums.MenuList.LACK2;
        }

        // GET: LACK2
        public ActionResult Index()
        {
            var model = new Lack2IndexViewModel();
            model = InitViewModel(model);

            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            var dbData = _lack2Bll.GetAll(new Lack2GetByParamInput());
            model.Details = dbData.Select(d => Mapper.Map<LACK2NppbkcData>(d)).ToList();

            return View("Index", model);
        }

        /// <summary>
        /// Fills the select lists for the IndexViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Lack2IndexViewModel</returns>
        private Lack2IndexViewModel InitViewModel(Lack2IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
            model.PoaList = GlobalFunctions.GetPoaAll();
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        /// <summary>
        /// Create LACK2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            LACK2CreateViewModel model = new LACK2CreateViewModel();

            model.NPPBKCDDL = GlobalFunctions.GetNppbkcAll();
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList();
            model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeGroupList();
            model.SendingPlantDDL = GlobalFunctions.GetPlantAll();

            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(LACK2CreateViewModel model)
        {
            return View("Create", model);
        }

    }
            
}