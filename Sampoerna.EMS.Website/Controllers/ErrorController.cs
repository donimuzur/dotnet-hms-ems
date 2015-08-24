using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}