using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK1Controller : BaseController
    {
        public PBCK1Controller(IPageBLL pageBLL) : base(pageBLL, Enums.MenuList.PBCK1)
        {
            
        }
        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            var model = new PBCK1ViewModel
            {
                MainMenu = Enums.MenuList.ExcisableGoodMovement,
                CurrentMenu = PageInfo
            };
            return View(model);
        }
    }
}