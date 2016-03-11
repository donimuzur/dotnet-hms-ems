﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Helpers;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    [AuthorizeAD]
    public class BaseController : Controller
    {

        private IPageBLL _pageBLL;
        private Enums.MenuList _menuID;
       
        public BaseController(IPageBLL pageBll, Enums.MenuList menuID)
        {
            _pageBLL = pageBll;
            _menuID = menuID;
        }
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private bool IsAvailableDistrictCookie(string cookieName)
        {
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(cookieName))
            {
                return true;
            }
            return false;
        }

        protected void CreateCookie(string cookieName, string district)
        {
            HttpCookie cookie;
            if (!IsAvailableDistrictCookie(cookieName))
            {
                cookie = new HttpCookie(cookieName);


            }
            else
            {
                cookie = ControllerContext.HttpContext.Request.Cookies[cookieName];

            }
            if (cookie != null)
            {
                cookie.Value = district;
                cookie.Expires = DateTime.Now.AddYears(1);
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }
        
        public Login CurrentUser
        {
            get
            {
                if (Session[Core.Constans.SessionKey.CurrentUser] == null)
                {
                    var userId =  User.Identity.Name.Remove(0,4);
                    IUserBLL userBll = MvcApplication.GetInstance<UserBLL>();
                    IPOABLL poabll = MvcApplication.GetInstance<POABLL>();
                    IUserAuthorizationBLL userAuthorizationBll = MvcApplication.GetInstance<UserAuthorizationBLL>();
                    var loginResult = userBll.GetLogin(userId);

                    if (loginResult != null)
                    {
                        //CurrentUser = loginResult;
                        loginResult.UserRole = poabll.GetUserRole(loginResult.USER_ID);
                        loginResult.AuthorizePages = userAuthorizationBll.GetAuthPages(loginResult.USER_ID);
                        loginResult.NppbckPlants = userAuthorizationBll.GetNppbckPlants(loginResult.USER_ID);
                        loginResult.ListUserPlants = new List<string>();
                        loginResult.ListUserNppbkc = new List<string>();
                        switch (loginResult.UserRole)
                        {
                            case Enums.UserRole.User:
                            case Enums.UserRole.Viewer:
                                loginResult.ListUserPlants =
                                    userAuthorizationBll.GetListPlantByUserId(loginResult.USER_ID);
                                loginResult.ListUserNppbkc =
                                    userAuthorizationBll.GetListNppbkcByUserId(loginResult.USER_ID);
                                break;
                            case Enums.UserRole.POA:
                                loginResult.ListUserPlants = new List<string>();
                                foreach (var nppbkcPlantDto in loginResult.NppbckPlants)
                                {
                                    foreach (var plantDto in nppbkcPlantDto.Plants)
                                    {
                                        loginResult.ListUserPlants.Add(plantDto.WERKS);
                                    }
                                }
                                loginResult.ListUserNppbkc = loginResult.NppbckPlants.Select(c => c.NppbckId).ToList();
                                break;
                        }
                        


                    }
                    Session[Core.Constans.SessionKey.CurrentUser] = loginResult;
                }
                return (Login)Session[Core.Constans.SessionKey.CurrentUser];
            }
            
        }

        protected PAGE PageInfo
        {
            get
            {
                return _pageBLL.GetPageByID((int) _menuID);
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;


            base.OnActionExecuted(filterContext);

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            var descriptor = filterContext.ActionDescriptor;
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerDescriptor.ControllerName;

            if (controllerName == "Login" && actionName == "Index") return;
            if (controllerName == "Home" && actionName == "Index") return;

            if (CurrentUser == null )
            {
                filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary { { "controller", "Error" }, { "action", "Unauthorized" } });
             
                
            }
            var isUsePageAuth = ConfigurationManager.AppSettings["UsePageAuth"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["UsePageAuth"]);
            if (isUsePageAuth)
            {
                CurrentUser.AuthorizePages = _pageBLL.GetAuthPages(CurrentUser.USER_ID);
                if (CurrentUser.AuthorizePages != null)
                {
                    if (!CurrentUser.AuthorizePages.Contains(PageInfo.PAGE_ID))
                    {
                        if (!CurrentUser.AuthorizePages.Contains(PageInfo.PARENT_PAGE_ID))
                        {
                            filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary { { "controller", "Error" }, { "action", "Unauthorized" } });

                        }
                    }
                }
            }


        }

        #region MessageInfo
        private List<MessageInfo> ListMessageInfo { get; set; }

        private void AddMessage(MessageInfo messageInfo)
        {
            ListMessageInfo = (List<MessageInfo>)TempData["MessageInfo"] ?? new List<MessageInfo>();
            ListMessageInfo.Add(messageInfo);

            TempData["MessageInfo"] = ListMessageInfo;
        }

        public void AddMessageInfo(MessageInfo messageinfo)
        {
            AddMessage(messageinfo);
        }

        public void AddMessageInfo(List<string> message, Enums.MessageInfoType messageinfotype)
        {
            AddMessage(new MessageInfo(message, messageinfotype));
        }

        public void AddMessageInfo(string message, Enums.MessageInfoType messageinfotype)
        {
            AddMessage(new MessageInfo(new List<string> { message }, messageinfotype));
        }


        public List<BaseModel> GetListMessageInfo()
        {
            var lsModel = new List<BaseModel>();
            ListMessageInfo = (List<MessageInfo>)TempData["MessageInfo"];

            if (ListMessageInfo != null)
                lsModel.AddRange(ListMessageInfo.Select(messageInfo => new BaseModel()
                {
                    MessageTitle =messageInfo.MessageInfoType.ToString(),// EnumsHelper.GetResourceDisplayEnums(messageInfo.MessageInfoType),
                    MessageBody = messageInfo.MessageText
                }));

            return lsModel;
        }
        #endregion

        #region ---------- Pdf Purpose --------

        protected ActionResult Pdf()
        {
            return Pdf(null, null, null);
        }

        protected ActionResult Pdf(string fileDownloadName)
        {
            return Pdf(fileDownloadName, null, null);
        }

        protected ActionResult Pdf(string fileDownloadName, string viewName)
        {
            return Pdf(fileDownloadName, viewName, null);
        }

        protected ActionResult Pdf(object model)
        {
            return Pdf(null, null, model);
        }

        protected ActionResult Pdf(string fileDownloadName, object model)
        {
            return Pdf(fileDownloadName, null, model);
        }

        protected ActionResult Pdf(string fileDownloadName, string viewName, object model)
        {
            // Based on View() code in Controller base class from MVC
            if (model != null)
            {
                ViewData.Model = model;
            }
            PdfResult pdf = new PdfResult()
            {
                FileDownloadName = fileDownloadName,
                ViewName = viewName,
                ViewData = ViewData,
                TempData = TempData,
                ViewEngineCollection = ViewEngineCollection
            };
            return pdf;
        }

        #endregion

    }
}