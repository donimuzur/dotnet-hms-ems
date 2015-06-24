using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.HeaderFooter;

namespace Sampoerna.EMS.Website.Controllers
{
    public class HeaderFooterController : BaseController
    {
        private IHeaderFooterBLL _headerFooterBll;

        public HeaderFooterController(IPageBLL pageBLL, IHeaderFooterBLL headerFooterBll) : base(pageBLL, Enums.MenuList.HHeaderFooter)
        {
            _headerFooterBll = headerFooterBll;
        }

        //
        // GET: /HeaderFooter/
        public ActionResult Index()
        {
            var data = _headerFooterBll.GetAll();
            var model = new HeaderFooterViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = Enums.MenuList.MasterData,
                Details = Mapper.Map<List<HeaderFooterItem>>(data)
            };
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var data = _headerFooterBll.GetDetailsById(id);
            var model = new HeaderFooterItemViewModel()
            {
                CurrentMenu = PageInfo,
                MainMenu = Enums.MenuList.MasterData,
                Detail = Mapper.Map<HeaderFooterDetailItem>(data)
            };
            return View(model);
        }

        public ActionResult CreateInitial(HeaderFooterItemViewModel model)
        {
            return View("Create", model);
        }

        public ActionResult Create()
        {
            return CreateInitial(new HeaderFooterItemViewModel(){ CurrentMenu = PageInfo, MainMenu = Enums.MenuList.MasterData});
        }

        [HttpPost]
        public ActionResult Create(HeaderFooterItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                //do save
                var saveOutput = _headerFooterBll.Save(Mapper.Map<HeaderFooterDetails>(model.Detail));
                if (saveOutput.Success)
                {
                    return View("Index");
                }
                
                //Set ErrorMessage
                model.ErrorMessage = saveOutput.ErrorCode + "\n\r" + saveOutput.ErrorMessage;
            }
            return CreateInitial(model);
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

	}
}