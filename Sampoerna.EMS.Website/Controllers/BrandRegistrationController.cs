using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
            var model = new BrandRegistrationIndexViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            var dbData = _brandRegistrationBll.GetAllBrands();
            model.Details = AutoMapper.Mapper.Map<List<BrandRegistrationDetail>>(dbData);

            return View("Index", model);
        }

        public ActionResult Details(long id)
        {
            var model = new BrandRegistrationDetailsViewModel();


            var dbBrand = _brandRegistrationBll.GetById(id);
            model = AutoMapper.Mapper.Map<BrandRegistrationDetailsViewModel>(dbBrand);

            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            return View(model);
        }

        public ActionResult Create(long id)
        {
            var model = new BrandRegistrationCreateViewModel();

            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            return View(model);
        }

        public ActionResult Edit(long id)
        {
            var model = new BrandRegistrationEditViewModel();


            var dbBrand = _brandRegistrationBll.GetById(id);
            model = AutoMapper.Mapper.Map<BrandRegistrationEditViewModel>(dbBrand);

            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            return View(model);
        }
    }
}