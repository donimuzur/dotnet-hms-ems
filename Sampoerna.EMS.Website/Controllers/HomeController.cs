using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.BusinessObject;
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
            //Employee emp = new Employee();
            //emp.name = "adnan";
            //emp.address = "alamat palsu";
            //_employeeBll.Add(emp);
            //var data = _companyBll.GetMasterData();
           var userTreeList = _workflowBll.GetUserTree();
           return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}