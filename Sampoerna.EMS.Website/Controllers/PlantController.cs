using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.PLANT;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PlantController : BaseController
    {
        private IPlantBLL _plantBll;
        private IMasterDataBLL _masterDataBll;

        public PlantController(IPlantBLL plantBll, IMasterDataBLL masterData, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _plantBll = plantBll;
            _masterDataBll = masterData;
        }

        //
        // GET: /Plant/
        public ActionResult Index()
        {
            var plantT1001W = new PlantViewModel();
            plantT1001W.MainMenu = Enums.MenuList.MasterData;
            plantT1001W.CurrentMenu = PageInfo;

            plantT1001W.Details = _plantBll.GetAll().Select(AutoMapper.Mapper.Map<DetailPlantT1001W>).ToList();

            return View("Index", plantT1001W);
        }
	}
}