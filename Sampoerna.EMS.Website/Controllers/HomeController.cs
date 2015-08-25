using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
//using Sampoerna.EMS.Reporting;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    
    public class HomeController : BaseController
    {
        private ICompanyBLL _companyBll;
        private IWorkflowBLL _workflowBll;

        public HomeController(ICompanyBLL companyBll, IWorkflowBLL workflowBll, IPageBLL pageBll, Enums.MenuList menuID):
            base(pageBll, Enums.MenuList.User)
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