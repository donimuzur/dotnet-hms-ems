using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{

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
                return (Login)Session[Core.Constans.SessionKey.CurrentUser];
            }
            set
            {
                Session[Core.Constans.SessionKey.CurrentUser] = value;
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

            //if (ConfigurationManager.AppSettings["BaseUrl"] != null)
            //{
            //    viewResult.ViewBag.BaseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            //}

            //var descriptor = filterContext.ActionDescriptor;
            //var actionName = descriptor.ActionName;
            //var controllerName = descriptor.ControllerDescriptor.ControllerName;

            //_accountBll.InsertLog(controllerName, actionName, CurrentUser
            //    );

            base.OnActionExecuted(filterContext);

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var descriptor = filterContext.ActionDescriptor;
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerDescriptor.ControllerName;

            if (controllerName == "Login" && actionName == "Index") return;

            if (CurrentUser == null)
            {
                //RedirectToAction("Index", "Login");
                //filterContext.Result = new RedirectToRouteResult(
                //    new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });

                CurrentUser = new Login() { USERNAME = "Guest", USER_ID = 0, USER_GROUP_ID = 1, FIRST_NAME = "Guest", LAST_NAME = "User"};

            }
        }
    }
}