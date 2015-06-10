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

        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll) : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
        }

        private PBCK1ViewModel GetPBCKData(PBCK1Input input = null)
        {
            if (input == null)
            {
                input = new PBCK1Input();
            }
            var model = new PBCK1ViewModel
            {
                MainMenu = Enums.MenuList.ExcisableGoodMovement,
                CurrentMenu = PageInfo
            };
            model.Details = Mapper.Map<List<PBCK1Item>>(_pbck1Bll.GetPBCK1ByParam(input));
            return model;
        }

        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            return View(GetPBCKData());
        }

        [HttpPost]
        public ActionResult Index(PBCK1Input searchInput)
        {
            return View(GetPBCKData(searchInput));
        }


    }
}