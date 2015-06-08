using System.Web.Mvc;
using Sampoerna.EMS.Contract;

//using Sampoerna.EMS.Reporting;

namespace Sampoerna.EMS.Website.Controllers
{
    
    public class HomeController : Controller
    {
        private ICompanyBLL _companyBll;
        private IWorkflowBLL _workflowBll;
        public HomeController(ICompanyBLL companyBll, IWorkflowBLL workflowBll)
        {
            _companyBll = companyBll;
            _workflowBll = workflowBll;
        }

        public ActionResult Index()
        {   
           return View();
        }
        
    }
}