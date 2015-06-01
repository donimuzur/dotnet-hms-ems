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
        private IEmployeeBLL _employeeBll;
        private ICompanyBLL _companyBll;
        public HomeController(IEmployeeBLL employeeBll, ICompanyBLL companyBll)
        {
            _employeeBll = employeeBll;
            _companyBll = companyBll;
        }

        public ActionResult Index()
        {
            //Employee emp = new Employee();
            //emp.name = "adnan";
            //emp.address = "alamat palsu";
            //_employeeBll.Add(emp);
            //var data = _companyBll.GetMasterData();
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