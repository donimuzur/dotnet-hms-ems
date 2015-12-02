using System.Linq;
using AutoMapper;
using iTextSharp.text.pdf.qrcode;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
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
        private IUnitOfWork _uow;

        public UserPlantMapController(IPageBLL pageBLL, IUserPlantMapBLL userPlantMapBll, IPlantBLL plantBll, IChangesHistoryBLL changeHistorybll, IUnitOfWork uow)
            : base(pageBLL, Enums.MenuList.UserPlantMap)
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
            var userIdPlant = _userPlantMapBll.GetUser();
            var userPlantDb = _userPlantMapBll.GetAllOrderByUserId();
            model.UserPlantList = Mapper.Map<List<UserPlantMapDetail>>(userIdPlant);
            model.UserPlantMaps = Mapper.Map<List<UserPlantMapDto>>(userPlantDb);


            return View("Index", model);
        }



        private UserPlantMapDetailViewModel InitEdit(string id)
        {
            var currenPlant = _userPlantMapBll.GetByUserId(id);
            var model = new UserPlantMapDetailViewModel
            {
                UserPlantMap = Mapper.Map<UserPlantMapDto>(currenPlant.FirstOrDefault()),
                Users = GlobalFunctions.GetUsers(),
                Nppbkcs = GlobalFunctions.GetNppbkcMultiSelectList(),
                CurrentMenu = PageInfo,
                MainMenu = _mainMenu,
                Plants = new List<PlantDto>()
            };

            //process plant
            foreach (USER_PLANT_MAP t in currenPlant)
            {
                var toInsert = Mapper.Map<PlantDto>(t.T001W);
                toInsert.IS_IMPORT_ID = t.NPPBKC_ID != t.T001W.NPPBKC_ID;
                model.Plants.Add(toInsert);
            }
            
            model.SelectedNppbkc = model.Plants.GroupBy(x => (x.IS_IMPORT_ID ? x.NPPBKC_IMPORT_ID : x.NPPBKC_ID)).Select(x => x.Key).ToList();
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
                    foreach (var plant in model.Plants)
                    {
                        if (plant.IsChecked)
                        {
                            model.UserPlantMap.NppbkcId = plant.IS_IMPORT_ID ? plant.NPPBKC_IMPORT_ID : plant.NPPBKC_ID;
                            model.UserPlantMap.PlantId = plant.WERKS;
                            var existingPlantMap =
                                _userPlantMapBll.GetByUserPlantNppbkcId(
                                    new UserPlantMapGetByUserPlantNppbkcIdParamInput()
                                    {
                                        NppbkcId = model.UserPlantMap.NppbkcId,
                                        PlantId = model.UserPlantMap.PlantId,
                                        UserId = model.UserPlantMap.UserId
                                    });
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
                    var savePlant = model.Plants.Select(c => new
                    {
                        NPPBKC_ID = c.IS_IMPORT_ID ? c.NPPBKC_IMPORT_ID : c.NPPBKC_ID,
                        c.WERKS
                    }).ToList();

                    var currentPlant = currenPlant.Select(c => new
                    {
                        c.NPPBKC_ID,
                        WERKS = c.PLANT_ID
                    }).ToList();

                    //check if user delete all mapping then return error message
                    var intersectBoth = savePlant.Intersect(currentPlant);

                    if (intersectBoth.Any())
                    {
                        var listCmd = new List<bool>();
                        foreach (var plant1 in intersectBoth)
                        {
                            listCmd.Add(model.Plants.Where(c => c.WERKS == plant1.WERKS && c.NPPBKC_ID == plant1.NPPBKC_ID).Select(c => c.IsChecked).FirstOrDefault());
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
                            //var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId, plant);
                            var existingPlantMap =
                                _userPlantMapBll.GetByUserPlantNppbkcId(
                                    new UserPlantMapGetByUserPlantNppbkcIdParamInput()
                                    {
                                        UserId = model.UserPlantMap.UserId,
                                        NppbkcId = plant.NPPBKC_ID,
                                        PlantId = plant.WERKS
                                    });

                            if (existingPlantMap != null)
                            {
                                _userPlantMapBll.Delete(existingPlantMap.USER_PLANT_MAP_ID);
                            }
                        }

                    }

                    foreach (var plant in model.Plants)
                    {
                        USER_PLANT_MAP chkTo;
                        if (plant.IS_IMPORT_ID)
                        {
                            chkTo =
                                currenPlant.FirstOrDefault(
                                    c => c.PLANT_ID == plant.WERKS && c.NPPBKC_ID == plant.NPPBKC_IMPORT_ID);
                        }
                        else
                        {
                            chkTo =
                                currenPlant.FirstOrDefault(
                                    c => c.PLANT_ID == plant.WERKS && c.NPPBKC_ID == plant.NPPBKC_ID);
                        }

                        if (chkTo != null)
                        {

                            if (!plant.IsChecked)
                            {
                                //var currentPlantUpdated = _userPlantMapBll.GetByUserId(model.UserPlantMap.UserId);
                                //if (currentPlantUpdated.Count() == 1)
                                //{
                                //    AddMessageInfo("Please fill User Plant Map at least one record", Enums.MessageInfoType.Error);
                                //    return RedirectToAction("Index");
                                //}
                                //var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                //    plant.WERKS);

                                var existingPlantMap =
                                    _userPlantMapBll.GetByUserPlantNppbkcId(
                                        new UserPlantMapGetByUserPlantNppbkcIdParamInput()
                                        {
                                            UserId = model.UserPlantMap.UserId,
                                            NppbkcId = plant.NPPBKC_ID,
                                            PlantId = plant.WERKS
                                        });

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
                                //var existingPlantMap = _userPlantMapBll.GetByUserIdAndPlant(model.UserPlantMap.UserId,
                                //    plant.WERKS);

                                var existingPlantMap =
                                    _userPlantMapBll.GetByUserPlantNppbkcId(
                                        new UserPlantMapGetByUserPlantNppbkcIdParamInput()
                                        {
                                            UserId = model.UserPlantMap.UserId,
                                            NppbkcId = plant.IS_IMPORT_ID ? plant.NPPBKC_IMPORT_ID : plant.NPPBKC_ID,
                                            PlantId = plant.WERKS
                                        });

                                model.UserPlantMap.PlantId = plant.WERKS;
                                model.UserPlantMap.NppbkcId = plant.IS_IMPORT_ID ? plant.NPPBKC_IMPORT_ID : plant.NPPBKC_ID;
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

        public ActionResult Active(string id)
        {
            try
            {
                _userPlantMapBll.Active(id);

                AddMessageInfo(Constans.SubmitMessage.Updated, Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                TempData[Constans.SubmitType.Update] = ex.Message;
            }
            return RedirectToAction("Index");

        }

    }
}
