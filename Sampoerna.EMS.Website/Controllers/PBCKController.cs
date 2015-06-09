using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCKController : Controller
    {
        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            var model = new PBCKViewModel { MainMenu = Enums.MenuList.ExcisableGoodMovement };
            return View(model);
        }
	}
}