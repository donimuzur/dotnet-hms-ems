using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class AspxReportViewerController : Controller
    {
        //
        // GET: /AspxReportViewer/
        public void ShowReport()
        {
            Response.Redirect("~/AspnetWebForms/GenericReportViewer.aspx");
        }
	}
}