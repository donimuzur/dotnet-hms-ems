using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class CompanyController : BaseController
    {

        private ICompanyBLL _companyBLL;
        
        public CompanyController(ICompanyBLL companyBll, IPageBLL pageBLL) : base(pageBLL, Enums.MenuList.COMPANY)
        {
            _companyBLL = companyBll;
        }
            
       
        //
        // GET: /Company/
        public ActionResult Index()
        {
            var company = new CompanyViewModel();
            company.Details = _companyBLL.GetMasterData().Select(AutoMapper.Mapper.Map<CompanyDetail>).ToList();
           
                             
            return View("Index",company);
                                  
        }


    }
}