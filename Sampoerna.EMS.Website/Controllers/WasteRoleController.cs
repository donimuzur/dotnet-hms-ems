using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    public class WasteRoleController : BaseController
    {
         private Enums.MenuList _mainMenu;

         public WasteRoleController(IBrandRegistrationBLL brandRegistrationBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.BrandRegistration)
        {
            
            _mainMenu = Enums.MenuList.MasterData;
            
        }

        //
        // GET: /WasteRole/
        public ActionResult Index()
        {
            return View();
        }
	}
}