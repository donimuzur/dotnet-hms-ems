using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK1Controller : BaseController
    {
        private IPBCK1BLL _pbck1Bll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IZaidmExPOAMapBLL _poaMapBll;
        private IUserBLL _userBll;

        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll, IZaidmExNPPBKCBLL nppbkcbll,
            IZaidmExPOAMapBLL poaMapBll, IUserBLL userBll)
            : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
            _nppbkcbll = nppbkcbll;
            _poaMapBll = poaMapBll;
            _userBll = userBll;
        }

        private SelectList GetNPPBKC()
        {
            var nppbkcList = _nppbkcbll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(nppbkcList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
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

        private SelectList GetPoaByNppbkcId(string nppbkcId)
        {
            var poaList = _poaMapBll.GetPOAByNPPBKCID(nppbkcId);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(poaList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        private SelectList GetCreatorList()
        {
            var users = _userBll.GetUsers(new UserInput());
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(users);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        private SelectList GetYearList()
        {
            int currentYear = DateTime.Now.Year;
            var selectItemSource = new List<SelectItemModel>();
            for (var i = 0; i < 5; i++)
            {
                selectItemSource.Add(new SelectItemModel()
                {
                    ValueField = (currentYear - i),
                    TextField = (currentYear - i).ToString()
                });
            }
            return new SelectList(selectItemSource, "ValueField", "TextField");
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
            model.SearchInput.YearList = GetYearList();
            model.SearchInput.NPPBKCIDList = GetNPPBKC();
            model.SearchInput.CreatorList = GetCreatorList();
            model.SearchInput.POAList = new SelectList(new List<SelectItemModel>(), "ValueField", "TextField");
            model.Details = GetPBCKItems();
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

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId)
        {
            var listPoa = GetPoaByNppbkcId(nppbkcId);
            var model = new PBCK1ViewModel { SearchInput = { POAList = listPoa } };
            //return PartialView("PoaListPartial", model);
            return Json(model);
        }

        [HttpPost]
        public PartialViewResult Filter(PBCK1ViewModel model)
        {
            model.Details = GetPBCKItems(model.SearchInput);
            return PartialView("Pbck1TablePartial", model);
        }

    }
}