using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.POA;
using Sampoerna.EMS.Website.Models.POAMap;
using Sampoerna.EMS.Website.Models.UOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class POAMapController : BaseController
    {
        private IPOAMapBLL _poaMapBLL;
        private IChangesHistoryBLL _changeHistoryBll;
        private Enums.MenuList _mainMenu;

        public POAMapController(IPageBLL pageBLL, IPOAMapBLL poaMapBll, IChangesHistoryBLL changeHistorybll) 
            : base(pageBLL, Enums.MenuList.Uom) 
        {
            _poaMapBLL = poaMapBll;
            _changeHistoryBll = changeHistorybll;
            _mainMenu = Enums.MenuList.MasterData;
        }
        //
        // GET: /Uom/
        public ActionResult Index()
        {
            var model = new PoaMapIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.PoaMaps = Mapper.Map<List<POA_MAPDto>>(_poaMapBLL.GetAll());
            return View("Index", model);
        }

        //
        // GET: /Uom/Details/5
        public ActionResult Details(int id)
        {
            var existingData = _poaMapBLL.GetById(id);
            var model = new PoaMapDetailViewModel();
            model.PoaMap = Mapper.Map<POA_MAPDto>(existingData);
            model.CurrentMenu = PageInfo;
            return View("Detail", model);

        }

        //
        // GET: /Uom/Create
        public ActionResult Create()
        {
            var model = new PoaMapDetailViewModel();
            model.CurrentMenu = PageInfo;
            model.MainMenu = _mainMenu;
            model.NppbckIds = GlobalFunctions.GetNppbkcAll();
            model.Plants = GlobalFunctions.GetPlantAll();
            model.POAs = GlobalFunctions.GetPoaAll();
            return View("Create",model);
        }

        //
        // POST: /Uom/Create
        [HttpPost]
        public ActionResult Create(PoaMapDetailViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var data = Mapper.Map<POA_MAP>(model.PoaMap);
                data.CREATED_BY = CurrentUser.USER_ID;
                data.CREATED_DATE = DateTime.Now;
                _poaMapBLL.Save(data);

                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                     );
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );
              
               
                return View(model);
            }
        }

        //
        // GET: /Uom/Edit/5
        public ActionResult Edit(int id)
        {
            return RedirectToAction("Edit", new {id = id});
        }

        [HttpPost]
        public ActionResult Delete(PoaMapDetailViewModel model)
        {
            try
            {
                
               // _poaMapBLL.Save(data);

                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                     );
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );


                return View(model);
            }
        }

      
        
    }
}
