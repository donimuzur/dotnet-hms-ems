using System;
using System.Web.Mvc;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Controllers
{
    public class POAController : BaseController
    {

        private IZaidmExPOAMapBLL _poaMapBll;
        private IZaidmExPOABLL _poaBll;

        public POAController(IPageBLL pageBLL, IZaidmExPOAMapBLL poadMapBll, IZaidmExPOABLL poaBll) : base(pageBLL, Enums.MenuList.POA)
        {
            _poaMapBll = poadMapBll;
            _poaBll = poaBll;
        }

        //
        // GET: /POA/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetPOAByNppbkcId(string nppbkcId)
        {
            if (string.IsNullOrEmpty(nppbkcId))
            {
                throw new ArgumentNullException("nppbkcId");
            }
            var result = _poaMapBll.GetPOAByNPPBKCID(nppbkcId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

	}
}