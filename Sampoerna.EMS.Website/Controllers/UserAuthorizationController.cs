using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
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
            var model = new UserAuthorizationViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<List<DetailUserAuthorization>>(_userAuthorizationBll.GetAll())
            };
            
            return View("Index", model);
        }
	}
}