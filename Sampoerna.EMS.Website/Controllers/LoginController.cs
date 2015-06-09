using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LoginController : BaseController
    {
        private IUserBLL _bll;

        public LoginController( IUserBLL bll)
        {
            _bll = bll;

        }

        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel model)
        {

            var loginResult = _bll.GetLogin(model.Username);

            if (loginResult != null)
            {
                CurrentUser = loginResult;
                
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Username incorrect";
            return PartialView("Index");

        }
	}
}