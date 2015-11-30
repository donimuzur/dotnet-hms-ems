using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Vendor;

namespace Sampoerna.EMS.Website.Controllers
{
    public class VendorController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private ILFA1BLL _vendorBLL;

        public VendorController(IPageBLL pageBLL, ILFA1BLL vendorBll)
            : base(pageBLL, Enums.MenuList.Vendor)
        {
            _vendorBLL = vendorBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /Vendor/
        public ActionResult Index()
        {
            var dataVendors = _vendorBLL.GetAll();
            var model = new VendorIndexViewModel()
            {
                Details = dataVendors,
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo
            };
            return View(model);
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
               return HttpNotFound();
            }

            var dataVendor = _vendorBLL.GetById(id);
            var model = new VendorDetailViewModel()
            {
                Detail = dataVendor,
                MainMenu =  _mainMenu,
                CurrentMenu = PageInfo
            };
            return View(model);
        }
	}
}