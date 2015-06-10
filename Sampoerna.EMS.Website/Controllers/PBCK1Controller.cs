using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK1Controller : BaseController
    {
        private IPBCK1BLL _pbck1Bll;

        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll) : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
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