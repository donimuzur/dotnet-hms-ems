using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class SplashScreenController : BaseController
    {
        //
        // GET: /SplashScreen/
        public ActionResult Index()
        {
            var model = CurrentUser;
            return View(model);
        }
	}
}