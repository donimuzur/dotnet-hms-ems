using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK7Controller : BaseController
    {
        private IPBCK7BLL _pbck7Bll;
        private IBACK1BLL _back1Bll;
        private Enums.MenuList _mainMenu;
        public PBCK7Controller(IPageBLL pageBll, IPBCK7BLL pbck7Bll, IBACK1BLL back1Bll) 
            : base (pageBll, Enums.MenuList.PBCK7)
        {
            _pbck7Bll = pbck7Bll;
            _back1Bll = back1Bll;
            _mainMenu = Enums.MenuList.PBCK7;
        }

        #region Index
        //
        // GET: /PBCK7/
        public ActionResult Index()
        {
            return View();
        }
        #endregion  
       
       
	}
}