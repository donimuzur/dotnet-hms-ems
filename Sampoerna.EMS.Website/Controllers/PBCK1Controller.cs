using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.PBCK1;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK1Controller : BaseController
    {
        private IPBCK1BLL _pbck1Bll;

        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll)
            : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
        }

        private List<PBCK1Item> GetPBCKItems(PBCK1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                return Mapper.Map<List<PBCK1Item>>(_pbck1Bll.GetPBCK1ByParam(new PBCK1Input()));
            }
            //getbyparams
            var input = Mapper.Map<PBCK1Input>(filter);
            return Mapper.Map<List<PBCK1Item>>(_pbck1Bll.GetPBCK1ByParam(input));
        }

        private SelectList GetYearList(List<PBCK1Item> pbck1Data)
        {
            var query = from x in pbck1Data
                        where x.PERIOD_FROM.HasValue
                        select new SelectItemModel()
                        {
                            ValueField = x.PERIOD_FROM.Value.Year,
                            TextField = x.PERIOD_FROM.Value.Year.ToString()
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            return IndexInitial(new PBCK1ViewModel()
            {
                MainMenu = Enums.MenuList.ExcisableGoodsMovement,
                CurrentMenu = PageInfo
            });
        }

        public ActionResult IndexInitial(PBCK1ViewModel model)
        {
            model.SearchInput.NPPBKCIDList = GlobalFunctions.GetNppbkcAll();
            model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchInput.POAList = new SelectList(new List<SelectItemModel>(), "ValueField", "TextField");
            model.Details = GetPBCKItems();
            model.SearchInput.YearList = GetYearList(model.Details);
            return View("Index", model);
        }

        public ActionResult Create()
        {
            var model = new PBCK1ItemViewModel
            {
                MainMenu = Enums.MenuList.ExcisableGoodsMovement,
                CurrentMenu = PageInfo,
                Detail = null
            };
            return View(model);
        }

        public ActionResult Edit(long id)
        {
            return View(new PBCK1ItemViewModel(){ MainMenu = Enums.MenuList.ExcisableGoodsMovement, CurrentMenu = PageInfo });
        }

        public ActionResult Details(long id)
        {
            return View(new PBCK1ItemViewModel() { MainMenu = Enums.MenuList.ExcisableGoodsMovement, CurrentMenu = PageInfo });
        }

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var model = new PBCK1ViewModel { SearchInput = { POAList = listPoa } };
            return Json(model);
        }

        [HttpPost]
        public PartialViewResult Filter(PBCK1ViewModel model)
        {
            model.Details = GetPBCKItems(model.SearchInput);
            return PartialView("_Pbck1Table", model);
        }

        public ActionResult CreateInitial(PBCK1ItemViewModel model)
        {
            return View(model);
        }

    }
}