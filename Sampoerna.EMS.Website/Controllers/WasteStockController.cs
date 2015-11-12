using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.WasteRole;
using Sampoerna.EMS.Website.Models.WasteStock;

namespace Sampoerna.EMS.Website.Controllers
{
    public class WasteStockController : BaseController
    {

         private Enums.MenuList _mainMenu;
        private IWasteStockBLL _wasteStockBll;
        private IUserBLL _userBll;
        private IChangesHistoryBLL _changesHistoryBll;

        public WasteStockController(IWasteStockBLL wasteStockBll, IPageBLL pageBLL,
            IUserBLL userBll, IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.WasteStock)
        {
            _wasteStockBll = wasteStockBll;
            _mainMenu = Enums.MenuList.MasterData;

            _userBll = userBll;
            _changesHistoryBll = changesHistoryBll;

        }

        //
        // GET: /WasteStock/
        public ActionResult Index()
        {
            var model = new WasteStockIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.ListWasteStocks = Mapper.Map<List<WasteStockFormViewModel>>(_wasteStockBll.GetAllDataOrderByUserAndGroupRole());

            return View("Index", model);
        }

        private WasteStockFormViewModel SetListModel(WasteStockFormViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            
            model.PlantList = GlobalFunctions.GetPlantAll();

            return model;
        }

        public ActionResult Create()
        {
            var model = new WasteStockFormViewModel();
            model = SetListModel(model);
            return View("Create", model);
        }

        [HttpPost]
        public JsonResult GetListMaterialByPlant(string plantId)
        {
            var brandOutput = _wasteStockBll.GetListMaterialByPlant(plantId);
            return Json(brandOutput);
        }

        [HttpPost]
        public JsonResult GetUomByMaterialAndPlant(string materialNumber, string plantId)
        {
            var model = _wasteStockBll.GetListMaterialUomByMaterialAndPlant(materialNumber, plantId);
           
            return Json(model);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(WasteStockFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SaveWasteStockToDatabase(model);
                    AddMessageInfo("Success create Waste Role", Enums.MessageInfoType.Success);
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
                }
            }

            model = SetListModel(model);
            return View("Create", model);
        }

        private WasteStockDto SaveWasteStockToDatabase(WasteStockFormViewModel model)
        {

            var dataToSave = Mapper.Map<WasteStockDto>(model);
            
            var input = new WasteStockSaveInput()
            {
                WasteStockDto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
            };

            return _wasteStockBll.SaveWasteStock(input);
        }
	}
}