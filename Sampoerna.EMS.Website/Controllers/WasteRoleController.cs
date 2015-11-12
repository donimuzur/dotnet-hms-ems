using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WasteRole;

namespace Sampoerna.EMS.Website.Controllers
{
    public class WasteRoleController : BaseController
    {
         private Enums.MenuList _mainMenu;
        private IWasteRoleBLL _wasteRoleBll;

        public WasteRoleController(IWasteRoleBLL wasteRoleBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.WasteRole)
        {
            _wasteRoleBll = wasteRoleBll;
            _mainMenu = Enums.MenuList.WasteRole;
            
        }

        //
        // GET: /WasteRole/
        public ActionResult Index()
        {
            var model = new WasteRoleIndexViewModel();
            model.MainMenu = _mainMenu;
            
            return View();
        }
	}
}