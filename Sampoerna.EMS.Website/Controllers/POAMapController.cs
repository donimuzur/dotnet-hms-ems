﻿using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.POAMap;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class POAMapController : BaseController
    {
        private IPOAMapBLL _poaMapBLL;
        private IChangesHistoryBLL _changeHistoryBll;
        private Enums.MenuList _mainMenu;

        public POAMapController(IPageBLL pageBLL, IPOAMapBLL poaMapBll, IChangesHistoryBLL changeHistorybll) 
            : base(pageBLL, Enums.MenuList.POAMap) 
        {
            _poaMapBLL = poaMapBll;
            _changeHistoryBll = changeHistorybll;
            _mainMenu = Enums.MenuList.MasterData;
        }
        //
        // GET: /POA/
        public ActionResult Index()
        {
            var model = new PoaMapIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.PoaMaps = Mapper.Map<List<POA_MAPDto>>(_poaMapBLL.GetAll());
            return View("Index", model);
        }

        //
        // GET: /POAMap/Details/5
        public ActionResult Details(int id)
        {
            var existingData = _poaMapBLL.GetById(id);
            var model = new PoaMapDetailViewModel
            {
                PoaMap = Mapper.Map<POA_MAPDto>(existingData),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            return View("Detail", model);
        }

        //
        // GET: /POAMap/Create
        public ActionResult Create()
        {
            var model = new PoaMapDetailViewModel
            {
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
                NppbckIds = GlobalFunctions.GetNppbkcAll(),
                Plants = GlobalFunctions.GetPlantAll(),
                POAs = GlobalFunctions.GetPoaAll()
            };
            return View("Create",model);
        }

        //
        // POST: /POAMap/Create
        [HttpPost]
        public ActionResult Create(PoaMapDetailViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var existingData = _poaMapBLL.GetByNppbckId(model.PoaMap.NPPBKC_ID, model.PoaMap.WERKS, model.PoaMap.POA_ID);
                if (existingData != null)
                {
                    AddMessageInfo("data already exist", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Create");
                }
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
        // GET: /POAMap/Edit/5
        public ActionResult Edit(int id)
        {
            var existingData = _poaMapBLL.GetById(id);
            var model = new PoaMapDetailViewModel
            {
                PoaMap = Mapper.Map<POA_MAPDto>(existingData),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            model.NppbckIds = GlobalFunctions.GetNppbkcAll();
            model.Plants = GlobalFunctions.GetPlantAll();
            model.POAs = GlobalFunctions.GetPoaAll();
            return View("Edit", model);
        }

        
        public ActionResult Delete(int id)
        {
            try
            {
                
                _poaMapBLL.Delete(id);

                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success
                     );
                
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error
                       );


                
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /POAMap/Create
        [HttpPost]
        public ActionResult Edit(PoaMapDetailViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var existingData = _poaMapBLL.GetByNppbckId(model.PoaMap.NPPBKC_ID, model.PoaMap.WERKS, model.PoaMap.POA_ID);
                if (existingData != null)
                {
                    AddMessageInfo("data already exist", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Create");
                }
                var data = Mapper.Map<POA_MAP>(model.PoaMap);
                data.CREATED_BY = CurrentUser.USER_ID;
                data.CREATED_DATE = DateTime.Now;
                _poaMapBLL.Save(data);

                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success
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
