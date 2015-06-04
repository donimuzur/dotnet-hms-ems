using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
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

        //
        //GET : /Edit
        public ActionResult Edit(long id)
        {
            var company = _companyBLL.GetCompanyById(id);
            if (company == null)
            {
                return HttpNotFound();
            }

            var model = AutoMapper.Mapper.Map<CompanyViewModel>(company);

            return View(model);
        }

        //
        //POST : /Edit
        [HttpPost]
        public ActionResult Edit(CompanyViewModel companyModel)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                var company = AutoMapper.Mapper.Map<T1001>(companyModel);

                _companyBLL.Save(company);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}