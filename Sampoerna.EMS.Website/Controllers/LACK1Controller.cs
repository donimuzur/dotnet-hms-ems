using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.LACK1;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK1Controller : BaseController
    {
        private ILACK1BLL _lack1Bll;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;
        private Enums.MenuList _mainMenu;


        
        public LACK1Controller(IPageBLL pageBll, ILACK1BLL lack1Bll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll ) 
            : base(pageBll, Enums.MenuList.LACK1)
        {
            _lack1Bll = lack1Bll;
            _monthBll = monthBll;
            _uomBll = uomBll;
            _mainMenu = Enums.MenuList.LACK1;
        }

        //
        // GET: /LACK1/
        public ActionResult Index()
        {
            var data = new LACK1ViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,

            };
           
            return View("Index", data);
        }
	}
}