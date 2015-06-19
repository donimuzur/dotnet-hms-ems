using System;
using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.POA;

namespace Sampoerna.EMS.Website.Controllers
{
    public class POAController : BaseController
    {

        private IZaidmExPOAMapBLL _poaMapBll;
        private IZaidmExPOABLL _poaBll;

        public POAController(IPageBLL pageBLL, IZaidmExPOAMapBLL poadMapBll, IZaidmExPOABLL poaBll) : base(pageBLL, Enums.MenuList.MasterData)
        {
            _poaMapBll = poadMapBll;
            _poaBll = poaBll;
        }

        //
        // GET: /POA/
        public ActionResult Index()
        {
            var poa = new POAViewModel();
            poa.MainMenu = Enums.MenuList.MasterData;
            poa.CurrentMenu = PageInfo;

            poa.Details = _poaBll.GetAll().Select(AutoMapper.Mapper.Map<ZaidmExPOAOutput>).ToList();

            return View("Index", poa);
        }

	}
}