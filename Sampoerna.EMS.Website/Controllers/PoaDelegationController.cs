using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PoaDelegationController : BaseController
    {
        public PoaDelegationController(IPageBLL pageBLL, IXmlFileLogBLL xmlFileLogBll)
            : base(pageBLL, Enums.MenuList.Settings)
        {
            _xmlFileLogBll = xmlFileLogBll;
        }

        // GET: /PoaDelegation/
        public ActionResult Index()
        {
            return View();
        }
	}
}