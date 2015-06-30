﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
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
                Details = Mapper.Map<List<POAViewDetailModel>>(_poaBll.GetAll())
            };

            ViewBag.Message = TempData["message"];
            return View("Index", poa);
        }

        public ActionResult Create()
        {

            var poa = new POAFormModel();
            poa.MainMenu = Enums.MenuList.MasterData;
            poa.CurrentMenu = PageInfo;
            poa.Users = GlobalFunctions.GetCreatorList();
            return View(poa);
        }

        [HttpPost]
        public ActionResult Create(POAFormModel model)
        {
           
                try
                {
                    var poa = AutoMapper.Mapper.Map<ZAIDM_EX_POA>(model.Detail);
                    poa.IS_FROM_SAP = false;
                    _poaBll.Save(poa);
                    TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData[Constans.SubmitType.Save] = ex.Message;
                    return View();
                }
                
            

            return RedirectToAction("Create");
            
            
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
            
            model.Managers = detail.Manager == null ? GlobalFunctions.GetCreatorList() : GlobalFunctions.GetCreatorList(detail.Manager.USER_ID);
            model.Users = detail.User == null? GlobalFunctions.GetCreatorList(): GlobalFunctions.GetCreatorList(detail.User.USER_ID); 
            model.Detail = detail;
            return View(model);
        }

        [HttpPost]
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
                
                _poaBll.Update(poa);
                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
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
            model.Users = GlobalFunctions.GetCreatorList();
            model.Detail = detail;
            return View(model);

        }

        public ActionResult Delete(int id)
        {
            try
            {
                _poaBll.Delete(id);
                TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
            }
            catch (Exception ex)
            {
                TempData[Constans.SubmitType.Delete] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult GetUser(string userId)
        {
            var id = Convert.ToInt32(userId);
            return Json(_userBll.GetUserById(id));
        }
    }
}