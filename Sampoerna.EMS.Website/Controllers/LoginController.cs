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
            var model = new LoginFormModel();
            model.Users = new SelectList(_userBll.GetUsers(), "USER_ID", "USER_ID");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginFormModel model)
        {
            
            var loginResult = _userBll.GetLogin(model.Login.UserId);

            if (loginResult != null)
            {
                CurrentUser = loginResult;
                CurrentUser.UserRole = _poabll.GetUserRole(loginResult.USER_ID);
                CurrentUser.AuthorizePages = _userAuthorizationBll.GetAuthPages(loginResult.USER_ID);
                CurrentUser.NppbckPlants = _userAuthorizationBll.GetNppbckPlants(loginResult.USER_ID);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("UnAuthorize", "Error");

        }

        public ActionResult MessageInfo()
        {
            var model = GetListMessageInfo();
            return PartialView("_MessageInfo", model);
        }
	}
}