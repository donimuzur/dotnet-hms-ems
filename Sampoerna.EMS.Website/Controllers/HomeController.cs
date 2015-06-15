using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;

//using Sampoerna.EMS.Reporting;

namespace Sampoerna.EMS.Website.Controllers
{
    
    public class HomeController : Controller
    {
        private ICompanyBLL _companyBll;
        private IWorkflowBLL _workflowBll;
        private ICK4C_BLL _ck4CBll;
        public HomeController(ICompanyBLL companyBll, IWorkflowBLL workflowBll, ICK4C_BLL ck4CBll)
        {
            _companyBll = companyBll;
            _workflowBll = workflowBll;
            _ck4CBll = ck4CBll;
        }

        public ActionResult Index()
        {   
          CK4C ck4c = new CK4C();
            ck4c.CK4C_NUMBER = "cccccc";
            ck4c.COMPANY_ID = 101;
            ck4c.PLANT_ID = 1;
            List<CK4C_ITEM> items = new List<CK4C_ITEM>();
           CK4C_ITEM item = new CK4C_ITEM();
            item.PRODUCED_QTY = 1000;
            items.Add(item);
            ck4c.CK4C_ITEM = items;
            _ck4CBll.Insert(ck4c);
           return View();
        }
        
    }
}