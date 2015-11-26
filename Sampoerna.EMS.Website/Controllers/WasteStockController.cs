using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
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

            foreach (var wasteStockFormViewModel in model.ListWasteStocks)
            {
                wasteStockFormViewModel.StockRemainingDisplay =
                    _wasteStockBll.GetRemainingQuota(wasteStockFormViewModel.Stock, wasteStockFormViewModel.PlantId,
                        wasteStockFormViewModel.MaterialNumber);
            }

            return View("Index", model);
        }

        
        private WasteStockFormViewModel SetListModel(WasteStockFormViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.MaterialNumberList = GetMaterialList(model.PlantId);

            return model;
        }

        public ActionResult Create()
        {
            var model = new WasteStockFormViewModel();
            model = SetListModel(model);
            return View("Create", model);
        }

        private SelectList GetMaterialList(string plantId)
        {
            var listMaterial = _wasteStockBll.GetListMaterialByPlant(plantId);
            IEnumerable<SelectItemModel> query;
            query = from x in listMaterial
                    select new SelectItemModel()
                    {
                        ValueField = x.MaterialNumber,
                        TextField = x.MaterialNumber
                    };


            return new SelectList(query, "ValueField", "TextField");

           
        }

        //public ActionResult Edit(int id)
        //{
        //    var model = new WasteStockFormViewModel();

        //    try
        //    {
        //        model = Mapper.Map<WasteStockFormViewModel>(_wasteStockBll.GetById(id, true));
        //        model = SetListModel(model);
        //        model.MaterialNumberList = GetMaterialList(model.PlantId);
        //    }
        //    catch (Exception ex)
        //    {
        //        AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
        //        model = SetListModel(model);
        //        model.MaterialNumberList = GetMaterialList(model.PlantId);
        //    }

        //    return View("Edit", model);
        //}

        public ActionResult Detail(int id)
        {
            var model = new WasteStockFormViewModel();

            try
            {
                model = Mapper.Map<WasteStockFormViewModel>(_wasteStockBll.GetById(id, true));

                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

                model.ChangesHistoryList =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.WasteStock, id.ToString()));


                model.StockRemainingDisplay = _wasteStockBll.GetRemainingQuota(model.Stock, model.PlantId,
                    model.MaterialNumber);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;
            }

            return View("Detail", model);
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
                    AddMessageInfo("Success create Waste Stock", Enums.MessageInfoType.Success);
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
                    model.MaterialNumberList = GetMaterialList(model.PlantId);
                }
            }

            model = SetListModel(model);
            return View("Create", model);
        }

        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public ActionResult Edit(WasteStockFormViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            SaveWasteStockToDatabase(model);
        //            AddMessageInfo("Success update Waste Stock", Enums.MessageInfoType.Success);
        //            return RedirectToAction("Index");

        //        }
        //        catch (Exception ex)
        //        {
        //            AddMessageInfo("Update Failed : " + ex.Message, Enums.MessageInfoType.Error);
        //        }
        //    }

        //    model = SetListModel(model);
        //    return View("Edit", model);
        //}

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