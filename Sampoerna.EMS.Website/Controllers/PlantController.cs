﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.PlantReceiveMaterial;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PlantController : BaseController
    {
        private IPlantBLL _plantBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private Enums.MenuList _mainMenu;
        private IChangesHistoryBLL _changesHistoryBll;
        
        public PlantController(IPlantBLL plantBll, IZaidmExNPPBKCBLL nppbkcBll, IZaidmExGoodTypeBLL goodTypeBll, IChangesHistoryBLL changesHistoryBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterPlant)
        {
            _plantBll = plantBll;
            _nppbkcBll = nppbkcBll;
            _goodTypeBll = goodTypeBll;
            _mainMenu = Enums.MenuList.MasterData;
            _changesHistoryBll = changesHistoryBll;
        }

        //
        // GET: /Plant/
        public ActionResult Index()
        {
            var plant = new PlantViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<DetailPlantT1001W>>(_plantBll.GetAllPlant())
            };
            ViewBag.Message = TempData["message"];
            return View("Index", plant);

        }

        public ActionResult Edit(string id)
        {

            
            var plant = _plantBll.GetId(id);
            //NPPBKC Import for the dropdown dikosongkan default value
            plant.NPPBKC_IMPORT_ID = "";
            if (plant == null)
            {
                return HttpNotFound();
            }
            
            var detail = Mapper.Map<DetailPlantT1001W>(plant);

            var model = new PlantFormModel
            {
              
                Detail = detail
                
            };
            
            return InitialEdit(model);
        }

        public ActionResult InitialEdit(PlantFormModel model)
        {
            var dataNppbkc = _nppbkcBll.GetAll().Where(x => x.IS_DELETED != true).ToList();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.Nppbkc = new SelectList(dataNppbkc, "NPPBKC_ID", "NPPBKC_ID", model.Detail.NPPBKC_ID);
            model.NppbkcImport = new SelectList(dataNppbkc, "NPPBKC_ID", "NPPBKC_ID", model.Detail.NPPBKC_IMPORT_ID);
            model.IsMainPlantExist = IsMainPlantAlreadyExist(model.Detail.NPPBKC_ID, model.Detail.IsMainPlant,
                model.Detail.Werks);
            model.Detail.ReceiveMaterials = GetPlantReceiveMaterial(model.Detail);
            return View("Edit", model);
        }

        [HttpPost]
        public JsonResult ShowMainPlant(string nppbck1, bool? isMainPlant)
        {
            var checkIfExist = _plantBll.GetT001W(nppbck1, isMainPlant);
            var IsMainPlantExist = checkIfExist != null;
            return Json(IsMainPlantExist);
        }

        [HttpPost]
        public ActionResult Edit(PlantFormModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors);
                var errorMessage = errors.Aggregate("", (current, error) => current + (error.ErrorMessage + "\n"));
                AddMessageInfo(errorMessage, Enums.MessageInfoType.Error);
                      
                return InitialEdit(model);
            }
            var isAlreadyExistMainPlant = IsMainPlantAlreadyExist(model.Detail.NPPBKC_ID, model.Detail.IsMainPlant,
                model.Detail.Werks);
            if (isAlreadyExistMainPlant)
            {
                AddMessageInfo("Main Plant Already Set", Enums.MessageInfoType.Warning);
                return InitialEdit(model);
            }

            if (model.Detail.NPPBKC_ID == model.Detail.NPPBKC_IMPORT_ID) {
                AddMessageInfo("NPPBKC domestic cannot be the same as NPPBKC Import", Enums.MessageInfoType.Warning);
                return InitialEdit(model);
            }
            try
            {
               
                var receiveMaterial = model.Detail.ReceiveMaterials.Where(c => c.IsChecked).ToList();
                model.Detail.ReceiveMaterials = receiveMaterial;
                var t1001w = Mapper.Map<Plant>(model.Detail);
                if (t1001w.PLANT_RECEIVE_MATERIAL != null)
                {
                    var tempRecieveMaterial = t1001w.PLANT_RECEIVE_MATERIAL;
                    foreach (var rm in tempRecieveMaterial)
                    {
                        rm.ZAIDM_EX_GOODTYP = _goodTypeBll.GetById(rm.EXC_GOOD_TYP);
                    }
                    t1001w.PLANT_RECEIVE_MATERIAL = tempRecieveMaterial;
                }
                
                _plantBll.save(t1001w, CurrentUser.USER_ID);
                AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {

                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
              
                return InitialEdit(model);
            }
        }
        public ActionResult Detail(string id)
        {
            var plant = _plantBll.GetId(id);

            if (plant == null)
            {
                return HttpNotFound();
            }

            var detail = Mapper.Map<DetailPlantT1001W>(plant);

            var model = new PlantFormModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Nppbkc = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_ID", plant.NPPBKC_ID),
                NppbkcImport = new SelectList(_nppbkcBll.GetAll(), "NPPBKC_ID", "NPPBKC_ID", plant.NPPBKC_IMPORT_ID),
                Detail = detail
            };

            model.Detail.IsNo = !model.Detail.IsMainPlant;
            model.Detail.IsYes = model.Detail.IsMainPlant;
            model.Detail.ReceiveMaterials = GetPlantReceiveMaterial(model.Detail);
            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.MasterPlant, id));
            
            return View(model);

        }

        private List<PlantReceiveMaterialItemModel> GetPlantReceiveMaterial(DetailPlantT1001W plant)
        {
            var goodTypes = _goodTypeBll.GetAll();

            var planReceives = new List<PlantReceiveMaterialItemModel>();
            
                var recieve = _plantBll.GetReceiveMaterials(plant.Werks);
                foreach (var goodType in goodTypes)
                {
                    var planReceive = new PlantReceiveMaterialItemModel();
                    planReceive.EXC_GOOD_TYP = goodType.EXC_GOOD_TYP;
                    planReceive.PLANT_ID = plant.Werks;
                    planReceive.EXT_TYP_DESC = goodType.EXT_TYP_DESC;
                    planReceive.IsChecked = false;
                    if(recieve.Any(x => x.EXC_GOOD_TYP.Equals(goodType.EXC_GOOD_TYP)))
                    {
                        planReceive.IsChecked = true;
                    }
                    planReceives.Add(planReceive);
                }
            
            return planReceives;
        }

        private bool IsMainPlantAlreadyExist(string nppbkcid, bool IsMainPlant, string plantId)
        {
            if (!IsMainPlant)
                return false;
            var checkIfExist = _plantBll.GetT001W(nppbkcid, IsMainPlant);
            if (checkIfExist == null)
                return false;
            if (checkIfExist.WERKS != plantId)
                return true;
            return false;
           

        }


    }
}