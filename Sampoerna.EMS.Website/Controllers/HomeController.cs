using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Microsoft.Reporting.WebForms;
using Sampoerna.EMS.Reporting;

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

        public ActionResult ShowReportDataEmployee()
        {

            var datasource = _employeeBll.GetAll();
            var dataSourceDataTable = DataTableHelper.ConvertToDataTable(datasource.ToArray(), new EMSData.EmployeeDataTable());
            
            Session[Constans.SessionKey.ReportPath] = "Reports/EmployeeData.rdlc";

            //set ReportDataSource
            List<ReportDataSource> reportDataSources = new List<ReportDataSource>();
            reportDataSources.Add(new ReportDataSource("ds_employee", dataSourceDataTable));

            Session[Constans.SessionKey.ReportDataSources] = reportDataSources;

            return RedirectToAction("ShowReport", "AspxReportViewer");

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