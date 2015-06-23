using System;
using System.Collections;
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
        private IUserBLL _userBll;

        public POAController(IPageBLL pageBLL, IZaidmExPOAMapBLL poadMapBll, IZaidmExPOABLL poaBll, IUserBLL userBll
            ) : base(pageBLL, Enums.MenuList.MasterData)
        {
            _poaMapBll = poadMapBll;
            _poaBll = poaBll;
            _userBll = userBll;
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
            
            //var poa = new POAViewModel
            //{
            //    MainMenu = Enums.MenuList.MasterData,
            //    CurrentMenu = PageInfo,
            //};
            var poa = new POAFormModel();
            poa.MainMenu = Enums.MenuList.MasterData;
            poa.CurrentMenu = PageInfo;
            poa.Users = new SelectList(_userBll.GetUserTree(), "USER_ID", "FIRST_NAME");
            return View(poa);
        }
        
        [HttpPost]
        public ActionResult Create(POAFormModel model)
        {
            
            try
            {
                var poa = AutoMapper.Mapper.Map<ZAIDM_EX_POA>(model.Detail);
               _poaBll.save(poa);
                return RedirectToAction("Index");
            }
            catch(Exception exception)
            {
               
                return View();
            }
            return RedirectToAction("Index");
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