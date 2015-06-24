using System;
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
            )
            : base(pageBLL, Enums.MenuList.MasterData)
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

            ViewBag.Message = TempData["message"];
            return View("Index", poa);
        }

        public ActionResult Create()
        {

            var poa = new POAFormModel();
            poa.MainMenu = Enums.MenuList.MasterData;
            poa.CurrentMenu = PageInfo;
            poa.Users = new SelectList(_userBll.GetUserTree(), "USER_ID", "FIRST_NAME");
            return View(poa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(POAFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var poa = AutoMapper.Mapper.Map<ZAIDM_EX_POA>(model.Detail);
                    _poaBll.save(poa);
                    TempData["message"] = "Save Successful";
                    return RedirectToAction("Index");
                }
                catch (Exception exception)
                {

                    return View();
                }
                
            }

            var viewModel = new POAFormModel();
            viewModel.MainMenu = Enums.MenuList.MasterData;
            viewModel.CurrentMenu = PageInfo;
            viewModel.Users = new SelectList(_userBll.GetUserTree(), "USER_ID", "FIRST_NAME");
            return View(viewModel);
            //return RedirectToAction("Create");
            
            
        }

        public ActionResult Edit(int id)
        {
            var poa = _poaBll.GetById(id);


            if (poa == null)
            {

                return HttpNotFound();
            }
            var model = new POAFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            var detail = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
            model.Users = new SelectList(_userBll.GetUserTree(), "USER_ID", "FIRST_NAME", poa.USER_ID);
            model.Detail = detail;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(POAFormModel model)
        {
            try
            {
                var poaId = model.Detail.PoaId;
                var poa = _poaBll.GetById(poaId);
                if (poa.IS_FROM_SAP == null)
                {
                    poa.TITLE = model.Detail.Title;
                }
                else
                {
                    AutoMapper.Mapper.Map(model.Detail, poa);    
                }
                
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

            var model = new POAFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            var detail = AutoMapper.Mapper.Map<POAViewDetailModel>(poa);
            model.Users = new SelectList(_userBll.GetUserTree(), "USER_ID", "FIRST_NAME", poa.USER_ID);
            model.Detail = detail;
            return View(model);

        }


    }
}