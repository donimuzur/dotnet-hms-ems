﻿using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Microsoft.Reporting.WebForms;

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

        public ActionResult ShowReportDataEmployee()
        {
            var datasource = _employeeBll.GetAll();
            var dataSourceDataTable = DataTableHelper.ConvertToDataTable(datasource.ToArray(), new EMSDataSet.EmployeeDataTable());
            
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