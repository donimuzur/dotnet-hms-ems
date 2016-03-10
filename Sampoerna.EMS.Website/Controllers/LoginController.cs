﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LoginController : BaseController
    {
        private IUserBLL _userBll;
        private IPOABLL _poabll;
        private IUserAuthorizationBLL _userAuthorizationBll;
        public LoginController(IUserBLL userBll, IPageBLL pageBll, IPOABLL poabll, IUserAuthorizationBLL userAuthorizationBll)
            : base(pageBll, Enums.MenuList.USER)
        {
            _userBll = userBll;
            _poabll = poabll;
            _userAuthorizationBll = userAuthorizationBll;
        }

        //
        // GET: /Login/
        public ActionResult Index()
        {
            //var model = new LoginFormModel();
            //model.Users = new SelectList(_userBll.GetUsers(), "USER_ID", "USER_ID");
            //return View(model);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Index(LoginFormModel model)
        {
            
            //var loginResult = _userBll.GetLogin(model.Login.UserId);

            if (loginResult != null)
            {
                CurrentUser = loginResult;
                CurrentUser.UserRole = _poabll.GetUserRole(loginResult.USER_ID);
                CurrentUser.AuthorizePages = _userAuthorizationBll.GetAuthPages(loginResult.USER_ID);
                CurrentUser.NppbckPlants = _userAuthorizationBll.GetNppbckPlants(loginResult.USER_ID);
                CurrentUser.ListUserPlants = new List<string>();
                CurrentUser.ListUserNppbkc = new List<string>();
                switch (CurrentUser.UserRole)
                {
                    case Enums.UserRole.User:
                    case Enums.UserRole.Viewer:
                        CurrentUser.ListUserPlants =
                            _userAuthorizationBll.GetListPlantByUserId(loginResult.USER_ID);
                        CurrentUser.ListUserNppbkc =
                            _userAuthorizationBll.GetListNppbkcByUserId(loginResult.USER_ID);
                        break;
                    case Enums.UserRole.POA:
                        CurrentUser.ListUserPlants = new List<string>();
                        foreach (var nppbkcPlantDto in CurrentUser.NppbckPlants)
                        {
                            foreach (var plantDto in nppbkcPlantDto.Plants)
                            {
                                CurrentUser.ListUserPlants.Add(plantDto.WERKS);
                            }
                        }
                        CurrentUser.ListUserNppbkc = CurrentUser.NppbckPlants.Select(c => c.NppbckId).ToList();
                        break;
                }
              

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Unauthorized", "Error");

        }

        public ActionResult MessageInfo()
        {
            var model = GetListMessageInfo();
            return PartialView("_MessageInfo", model);
        }

        public ActionResult Logout()
        {
            Session[Core.Constans.SessionKey.CurrentUser] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}