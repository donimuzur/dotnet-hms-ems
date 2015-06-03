using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class CompanyController : Controller
    {

        private ICompanyBLL _companyBLL;
        
        public CompanyController(ICompanyBLL companyBll)
        {
            _companyBLL = companyBll;
        }

        //
        // GET: /Company/
        public ActionResult Index()
        {       
            var company = _companyBLL.GetMasterData().Select(AutoMapper.Mapper.Map<CompanyViewModel>).ToList();

            return View("Index",company);
                                  
        }
    }
}