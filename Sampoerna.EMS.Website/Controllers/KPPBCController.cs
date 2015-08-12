﻿using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.KPPBC;
using Sampoerna.EMS.Website.Models.POAMap;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class KPPBCController : BaseController
    {
        private IChangesHistoryBLL _changeHistoryBll;
        private Enums.MenuList _mainMenu;

        private IZaidmExKPPBCBLL _kppbcbll;
        public KPPBCController(IPageBLL pageBLL, IZaidmExKPPBCBLL kppbcbll, IChangesHistoryBLL changeHistorybll) 
            : base(pageBLL, Enums.MenuList.POAMap) 
        {
            
            _changeHistoryBll = changeHistorybll;
            _mainMenu = Enums.MenuList.MasterData;
            _kppbcbll = kppbcbll;
        }
        //
        // GET: /POA/
        public ActionResult Index()
        {
            var model = new KppbcIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.Kppbcs = Mapper.Map<List<ZAIDM_EX_KPPBCDto>>(_kppbcbll.GetAll());
            return View("Index", model);
        }

        //
        // GET: /POAMap/Details/5
        public ActionResult Details(string id)
        {
            var existingData = _kppbcbll.GetById(id);
            var model = new PoaMapDetailViewModel
            {
                PoaMap = Mapper.Map<POA_MAPDto>(existingData),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            return View("Detail", model);
        }

       
        public ActionResult Edit(string id)
        {
            var existingData = _kppbcbll.GetKppbcById(id);
            var model = new KppbcDetailViewModel
            {
                Kppbc = Mapper.Map<ZAIDM_EX_KPPBCDto>(existingData),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
           
            return View("Edit", model);
        }

        
       //
        // POST: /POAMap/Edit
        [HttpPost]
        public ActionResult Edit(KppbcDetailViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var existingData = _kppbcbll.GetKppbcById(model.Kppbc.KPPBC_ID);
                existingData.MODIFIED_BY = CurrentUser.USER_ID;
                existingData.MODIFIED_DATE = DateTime.Now;
                _kppbcbll.Save(existingData);

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
