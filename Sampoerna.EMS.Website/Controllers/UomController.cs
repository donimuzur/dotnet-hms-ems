using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.UOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UomController : BaseController
    {
        private IUnitOfMeasurementBLL _uomBLL;
        private IChangesHistoryBLL _changeHistoryBll;
        private Enums.MenuList _mainMenu;

        public UomController(IPageBLL pageBLL, IUnitOfMeasurementBLL uomBLL, IChangesHistoryBLL changeHistorybll) 
            : base(pageBLL, Enums.MenuList.Uom) 
        {
            _uomBLL = uomBLL;
            _changeHistoryBll = changeHistorybll;
            _mainMenu = Enums.MenuList.MasterData;
        }
        //
        // GET: /Uom/
        public ActionResult Index()
        {
            var uomModel = new UomIndexViewModel();
            uomModel.MainMenu = _mainMenu;
            uomModel.CurrentMenu = PageInfo;

            uomModel.Details = Mapper.Map<List<UomDetails>>(_uomBLL.GetAll());
            return View("Index",uomModel);
        }

        //
        // GET: /Uom/Details/5
        public ActionResult Details(int id)
        {
            var model = new UomDetailViewModel();

            var data = _uomBLL.GetById(id);

            model = Mapper.Map<UomDetailViewModel>(data);
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;

            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changeHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.Uom, id.ToString()));
            
            
            return View(model);
        }

        //
        // GET: /Uom/Create
        public ActionResult Create()
        {
            UomDetailViewModel model = new UomDetailViewModel();
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;

            return View("Create",model);
        }

        //
        // POST: /Uom/Create
        [HttpPost]
        public ActionResult Create(UomDetailViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var data = Mapper.Map<UOM>(model);
                _uomBLL.Save(data,CurrentUser.USER_ID,false);

                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

        //
        // GET: /Uom/Edit/5
        public ActionResult Edit(int id)
        {
            UomDetailViewModel model = new UomDetailViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            var data = _uomBLL.GetById(id);

            model = Mapper.Map<UomDetailViewModel>(data);
            return View("Edit",model);
        }

        //
        // POST: /Uom/Edit/5
        [HttpPost]
        public ActionResult Edit(UomDetailViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                var data = Mapper.Map<UOM>(model);
             
                _uomBLL.Save(data,CurrentUser.USER_ID, true);
                TempData[Constans.SubmitType.Update] = Constans.SubmitMessage.Updated;
                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

        

        //
        // POST: /Uom/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
