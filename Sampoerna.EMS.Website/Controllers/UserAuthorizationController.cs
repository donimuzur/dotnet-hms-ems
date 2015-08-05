using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.UserAuthorization;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserAuthorizationController : BaseController
    {
        private IUserAuthorizationBLL _userAuthorizationBll;
        private Enums.MenuList _mainMenu;
        public UserAuthorizationController(IPageBLL pageBll, IUserAuthorizationBLL userAuthorization) 
            : base (pageBll, Enums.MenuList.UserAuthorization)
        {
            _userAuthorizationBll = userAuthorization;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /UserAuthorization/
        public ActionResult Index()
        {
            var model = new IndexUserAuthorizationViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<List<DetailIndexUserAuthorization>>(_userAuthorizationBll.GetAll())
            };
            
            return View("Index", model);
        }

        public ActionResult Create()
        {
            var model = new CreateUserAuthorizationViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            InitCrateModel(model);
            return View(model);
        }

        private CreateUserAuthorizationViewModel InitCrateModel(CreateUserAuthorizationViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.BroleList = GlobalFunctions.GetBroleList();

            return model;
        }

	}
}