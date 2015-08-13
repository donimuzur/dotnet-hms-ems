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

        //
        // GET: /POAMap/Details/5
        public ActionResult Detail(int id)
        {
            var existingData = _userPlantMapBll.GetById(id);
            var model = new UserPlantMapDetailViewModel
            {
                UserPlantMap = Mapper.Map<UserPlantMapDto>(existingData),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            return View("Detail", model);
        }

       
        public ActionResult Edit(string id)
        {
           // var existingData = _userPlantMapBll.GetById(id);
            var currenPlant = _userPlantMapBll.GetByUserId(id);
            var model = new UserPlantMapDetailViewModel
            {
                //UserPlantMap = Mapper.Map<UserPlantMapDto>(existingData),
                Plants = Mapper.Map<List<PlantDto>>(_plantBll.GetAllPlant()),
                Users = GlobalFunctions.GetUsers(),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu
            };
            
            return View("Edit", model);
        }


        public ActionResult Create()
        {
            var model = new UserPlantMapDetailViewModel
            {
                Plants = Mapper.Map<List<PlantDto>>(_plantBll.GetAllPlant()),
                Users = GlobalFunctions.GetUsers(),
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
                            var data = Mapper.Map<USER_PLANT_MAP>(model.UserPlantMap);
                            _userPlantMapBll.Save(data);
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
