using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LoginController : BaseController
    {
        private IUserBLL _userBll;

        public LoginController( IUserBLL userBll, IPageBLL pageBll) : base(pageBll, Enums.MenuList.USER)
        {
            _userBll = userBll;

        }

        //
        // GET: /Login/
        public ActionResult Index()
        {
            var model = new LoginFormModel();
            model.Users = new SelectList(_userBll.GetUserTree(), "USERNAME", "FIRST_NAME");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginFormModel model)
        {
            
            var loginResult = _userBll.GetLogin(model.Login.Username);

            if (loginResult != null)
            {
                CurrentUser = loginResult;
                
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("UnAuthorize", "Error");

        }
	}
}