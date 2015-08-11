using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK2Controller : BaseController
    {

         private ILACK2BLL _lack2Bll;
        private Enums.MenuList _mainMenu;
        
        public LACK2Controller(IPageBLL pageBll, ILACK2BLL lack2Bll)
            : base(pageBll, Enums.MenuList.LACK2)
        {
            _lack2Bll = lack2Bll;
            _mainMenu = Enums.MenuList.LACK2;
        }

        // GET: LACK2
        public ActionResult Index()
        {
            return View();
        }
    }
}