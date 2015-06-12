using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
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

        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll, ZaidmExNPPBKCBLL nppbkcbll) : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
            _nppbkcbll = nppbkcbll;
        }

        private PBCK1ViewModel GetPBCKData(PBCK1Input input = null)
        {
            if (input == null)
            {
                input = new PBCK1Input();
            }
            var model = new PBCK1ViewModel
            {
                MainMenu = Enums.MenuList.ExcisableGoodsMovement,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<PBCK1Item>>(_pbck1Bll.GetPBCK1ByParam(input)),
                SearchInput = new PBCK1SearchInputModel()
            };
            return model;
        }

        private List<ZAIDM_EX_NPPBKC> GetNPPBKC()
        {
            return _nppbkcbll.GetAll();
        }

        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            var model = GetPBCKData();
            //set filter

            return View(model);
        }

        [HttpPost]
        public ActionResult Filter(PBCK1SearchInputModel searchInput)
        {
            var input = Mapper.Map<PBCK1Input>(searchInput);
            var model = GetPBCKData(input);
            try
            {

            }
            catch
            {
                model = GetPBCKData();
            }
            return View(model);
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

    }
}