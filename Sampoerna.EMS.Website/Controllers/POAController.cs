using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
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
            var poa = new POAViewModel
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
                Details = _poaBll.GetAll()
            };

            return View("Index", poa);
        }

        public ActionResult Create()
        {
            var poa = new POAViewModel
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
            };
            return View(poa);
        }
        
        [HttpPost]
        public ActionResult Create(POAViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                var poa = AutoMapper.Mapper.Map<ZAIDM_EX_POA>(model);
                poa.USER_ID = 100;
                _poaBll.save(poa);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var poamenu = new POAViewModel
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
            };

            var poa = _poaBll.GetById(id);

            if (poa == null)
            {

                return HttpNotFound();
            }
            var model = AutoMapper.Mapper.Map<POAViewModel>(poa);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(POAViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var poaId = model.PoaId;
                var poa = _poaBll.GetById(poaId);
                AutoMapper.Mapper.Map(model, poa);
                _poaBll.save(poa);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        public ActionResult Detail(int id)
        {
           var poa = _poaBll.GetById(id);
            if (poa == null)
            {
                return HttpNotFound();
            }
            var model = AutoMapper.Mapper.Map<POAViewModel>(poa);

            return View(model);
        }
        

	}
}