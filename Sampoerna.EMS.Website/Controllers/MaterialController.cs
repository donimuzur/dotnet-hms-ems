using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MaterialController : BaseController
    {

        public MaterialController(IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
           
        }


        //
        // GET: /Material/
        public ActionResult Index()
        {
            return View();
        }
	}
}