using System.Linq;
using AutoMapper;
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
using Sampoerna.EMS.Website.Models.UserPlantMap;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserPlantMapController : BaseController
    {
        private IChangesHistoryBLL _changeHistoryBll;
        private Enums.MenuList _mainMenu;

        private IUserPlantMapBLL _userPlantMapBll;
        private IPlantBLL _plantBll;
        
        public UserPlantMapController(IPageBLL pageBLL, IUserPlantMapBLL userPlantMapBll, IPlantBLL plantBll, IChangesHistoryBLL changeHistorybll) 
            : base(pageBLL, Enums.MenuList.POAMap) 
        {
            
            _changeHistoryBll = changeHistorybll;
            _mainMenu = Enums.MenuList.MasterData;
            _userPlantMapBll = userPlantMapBll;
            _plantBll = plantBll;

        }
        //
        // GET: /POA/
        public ActionResult Index()
        {
            var model = new UserPlantMapIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.UserPlantMaps = Mapper.Map<List<UserPlantMapDto>>(_userPlantMapBll.GetAll());
            return View("Index", model);
        }

        

        private UserPlantMapDetailViewModel InitEdit(string id)
        {
            var currenPlant = _userPlantMapBll.GetByUserId(id);
            var model = new UserPlantMapDetailViewModel
            {
                UserPlantMap = Mapper.Map<UserPlantMapDto>(currenPlant.FirstOrDefault()),
                Plants = Mapper.Map<List<PlantDto>>(currenPlant.Select(x=>x.T001W)),
                Users = GlobalFunctions.GetUsers(),
                Nppbkcs =  GlobalFunctions.GetNppbkcMultiSelectList(),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            foreach (var userPlantMap in currenPlant)
            {
                for (int i = 0; i < model.Plants.Count; i++)
                {
                    if (model.Plants[i].Werks == userPlantMap.PLANT_ID)
                    {
                        model.Plants[i].IsChecked = true;
                    }
                }
            }
            return model;
        }

        public ActionResult Edit(string id)
        {
            var model = InitEdit(id);
            return View("Edit", model);
        }
        public ActionResult Detail(string id)
        {
            var model = InitEdit(id);
            return View("Detail", model);
        }

        public ActionResult Create()
        {
            var model = new UserPlantMapDetailViewModel
            {
                
                Users = GlobalFunctions.GetUsers(),
                Nppbkcs = GlobalFunctions.GetNppbkcMultiSelectList(),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(UserPlantMapDetailViewModel model)
        {
            try
            {
                if (model.Plants != null)
                {
                    foreach (var plant in model.Plants )
                    {
                        if (plant.IsChecked)
                        {

                            model.UserPlantMap.PlantId = plant.Werks;
                            var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                plant.Werks);
                            if (existingPlantMap == null)
                            {
                                var data = Mapper.Map<USER_PLANT_MAP>(model.UserPlantMap);
                                _userPlantMapBll.Save(data);
                            }
                        }
                       
                    }
                    
                }
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
       //
        // POST: /POAMap/Edit
        [HttpPost]
        public ActionResult Edit(UserPlantMapDetailViewModel model)
        {
            try
            {

                var currenPlant = _userPlantMapBll.GetByUserId(model.UserPlantMap.UserId);
                if (model.Plants != null)
                {
                    foreach (var plant in model.Plants)
                    {
                        if (plant.IsChecked)
                        {
                            var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                plant.Werks);
                            
                            model.UserPlantMap.PlantId = plant.Werks;
                            var data = Mapper.Map<USER_PLANT_MAP>(model.UserPlantMap);
                            if (existingPlantMap != null)
                            {
                                data.USER_PLANT_MAP_ID = existingPlantMap.USER_PLANT_MAP_ID;
                            }
                            _userPlantMapBll.Save(data);
                        }
                        else
                        {
                            if (currenPlant.Any(x => x.PLANT_ID == plant.Werks))
                            {
                                var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                    plant.Werks);
                                if (existingPlantMap != null)
                                {

                                    _userPlantMapBll.Delete(existingPlantMap.USER_PLANT_MAP_ID);
                                }
                            }
                        }
                    }

                }
           

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

        [HttpPost]
        public JsonResult GetPlantByNppbkc(string nppbkcid)
        {
            return Json(_plantBll.GetPlantByNppbkc(nppbkcid));
        }

    }
}
