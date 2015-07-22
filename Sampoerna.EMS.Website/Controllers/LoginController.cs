using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LoginController : BaseController
    {
        private IUserBLL _userBll;
        private IZaidmExPOABLL _zaidmExPoabll;

        public LoginController(IUserBLL userBll, IPageBLL pageBll, IZaidmExPOABLL zaidmExPoabll)
            : base(pageBll, Enums.MenuList.USER)
        {
            _userBll = userBll;
            _zaidmExPoabll = zaidmExPoabll;
        }

        //
        // GET: /Login/
        public ActionResult Index()
        {
            var model = new LoginFormModel();
            model.Users = new SelectList(_userBll.GetUsers(), "USERNAME", "USERNAME");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginFormModel model)
        {
            
            var loginResult = _userBll.GetLogin(model.Login.Username);

            if (loginResult != null)
            {
                CurrentUser = loginResult;
                CurrentUser.UserRole = _zaidmExPoabll.GetUserRole(loginResult.USER_ID);
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