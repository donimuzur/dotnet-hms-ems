using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    public class SchedulerSettingController : BaseController
    {
        //
        // GET: /SchedulerSetting/
        public SchedulerSettingController(IPageBLL pageBll) : base(pageBll, Enums.MenuList.SchedulerSettings)
        {

        }

        public ActionResult Index()
        {
            return View();
        }
	}
}