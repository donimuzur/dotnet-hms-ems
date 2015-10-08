using System.Linq;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
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
            _mainMenu = Enums.MenuList.Settings;
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
                Plants = Mapper.Map<List<PlantDto>>(currenPlant.Select(x => x.T001W).ToList()),
                
                Users = GlobalFunctions.GetUsers(),
                Nppbkcs =  GlobalFunctions.GetNppbkcMultiSelectList(),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            model.SelectedNppbkc = model.Plants.GroupBy(x => x.NPPBKC_ID).Select(x => x.Key).ToList();
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

                            model.UserPlantMap.PlantId = plant.WERKS;
                            var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                plant.WERKS);
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
                if (model.Plants == null)
                {
                    AddMessageInfo("Please fill User Plant Map at least one record", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                var currenPlant = _userPlantMapBll.GetByUserId(model.UserPlantMap.UserId);

                if (model.Plants != null)
                {
                    var savePlant = model.Plants.Select(c => c.WERKS).ToList();
                    var currentPlant = currenPlant.Select(c => c.PLANT_ID).ToList();

                    //check if user delete all mapping then return error message
                    var intersectBoth = savePlant.Intersect(currentPlant);

                    if (intersectBoth.Any())
                    {
                        var listCmd = new List<bool>();
                        foreach (var plant1 in intersectBoth)
                        {
                            listCmd.Add(model.Plants.Where(c => c.WERKS == plant1).Select(c => c.IsChecked).FirstOrDefault()); 
                        }
                        if (listCmd.All(c => c != true))
                        {
                            AddMessageInfo("Please fill User Plant Map at least one record", Enums.MessageInfoType.Error);
                            return RedirectToAction("Index");
                        }
                    }

                    //check if model plant have less than current plant then delete other plant
                    //get the other plant to delete
                    var exceptPlant = currentPlant.Except(savePlant).ToList();

                    if (exceptPlant.Any())
                    {
                        foreach (var plant in exceptPlant)
                        {
                            var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId, plant);
                            if (existingPlantMap != null)
                            {

                                _userPlantMapBll.Delete(existingPlantMap.USER_PLANT_MAP_ID);
                            }
                        }

                    }

                    foreach (var plant in model.Plants)
                    {
                        if (currenPlant.Any(x => x.PLANT_ID == plant.WERKS))
                        {

                            if (!plant.IsChecked)
                            {
                                //var currentPlantUpdated = _userPlantMapBll.GetByUserId(model.UserPlantMap.UserId);
                                //if (currentPlantUpdated.Count() == 1)
                                //{
                                //    AddMessageInfo("Please fill User Plant Map at least one record", Enums.MessageInfoType.Error);
                                //    return RedirectToAction("Index");
                                //}
                                var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                    plant.WERKS);
                                if (existingPlantMap != null)
                                {

                                    _userPlantMapBll.Delete(existingPlantMap.USER_PLANT_MAP_ID);
                                }
                            }
                        }
                        else
                        {
                            if (plant.IsChecked)
                            {
                                var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                    plant.WERKS);

                                model.UserPlantMap.PlantId = plant.WERKS;
                                var data = Mapper.Map<USER_PLANT_MAP>(model.UserPlantMap);
                                if (existingPlantMap != null)
                                {
                                    data.USER_PLANT_MAP_ID = existingPlantMap.USER_PLANT_MAP_ID;
                                }
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

        [HttpPost]
        public JsonResult GetPlantByNppbkc(string nppbkcid)
        {
            return Json(_plantBll.GetPlantByNppbkc(nppbkcid));
        }

    }
}
