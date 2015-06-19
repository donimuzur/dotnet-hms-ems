using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.BrandRegistration;

namespace Sampoerna.EMS.Website.Controllers
{
    public class BrandRegistrationController : BaseController
    {
        private IBrandRegistrationBLL _brandRegistrationBll;

        public BrandRegistrationController(IBrandRegistrationBLL brandRegistrationBll,  IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _brandRegistrationBll = brandRegistrationBll;
        }

        //
        // GET: /BrandRegistration/
        public ActionResult Index()
        {
            var brandRegistratation = new BrandRegistrationViewModel();
            brandRegistratation.MainMenu = Enums.MenuList.MasterData;
            brandRegistratation.CurrentMenu = PageInfo;

            brandRegistratation.Details = _brandRegistrationBll.GetAll().Select(AutoMapper.Mapper.Map<DetailBrandRegistration>).ToList();

            return View("Index", brandRegistratation);
        }
    }
}