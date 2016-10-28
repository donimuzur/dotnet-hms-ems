using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.Reversal;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ReversalController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IReversalBLL _reversalBll;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;

        public ReversalController(IPageBLL pageBll, IReversalBLL reversalBll, IUserPlantMapBLL userPlantBll, IPOAMapBLL poaMapBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _mainMenu = Enums.MenuList.CK4C;
            _reversalBll = reversalBll;
            _userPlantBll = userPlantBll;
            _poaMapBll = poaMapBll;
        }

        #region Index

        public ActionResult Index()
        {
            var data = InitIndexViewModel(new ReversalIndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.Reversal,
                IsShowNewButton = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator ? true : false),
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
            });

            return View("Index", data);
        }

        private ReversalIndexViewModel InitIndexViewModel(
            ReversalIndexViewModel model)
        {
            var listPlant = GlobalFunctions.GetPlantAll();

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var itemPlant = GlobalFunctions.GetPlantAll().Where(x => CurrentUser.ListUserPlants.Contains(x.Value));

                listPlant = new SelectList(itemPlant, "Value", "Text");
            }

            model.PlantWerksList = listPlant;

            model.Detail = GetListDocument(model);

            return model;
        }

        private List<DataReversal> GetListDocument(ReversalIndexViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                var reversalData = _reversalBll.GetListDocumentByParam(new ReversalGetByParamInput()).OrderByDescending(d => d.ProductionDate);
                return Mapper.Map<List<DataReversal>>(reversalData);
            }

            //getbyparams
            var input = Mapper.Map<ReversalGetByParamInput>(filter);
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ListUserPlants = CurrentUser.ListUserPlants;

            var dbData = _reversalBll.GetListDocumentByParam(input).OrderByDescending(c => c.ProductionDate);
            return Mapper.Map<List<DataReversal>>(dbData);
        }

        [HttpPost]
        public PartialViewResult FilterListData(ReversalIndexViewModel model)
        {
            model.Detail = GetListDocument(model);
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false);
            return PartialView("_ReversalList", model);
        }

        #endregion


        #region create new reversal

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new ReversalIndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = new DataReversal()
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(ReversalIndexViewModel model)
        {
            return View("Create", InitialModel(model));
        }

        private ReversalIndexViewModel InitialModel(ReversalIndexViewModel model)
        {
            var plantList = GlobalFunctions.GetPlantAll();

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var distinctPlant = plantList.Where(x => CurrentUser.ListUserPlants.Contains(x.Value));
                var getPlant = new SelectList(distinctPlant, "Value", "Text");
                plantList = getPlant;
            }

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.PlantWerksList = plantList;
            model.FaCodeList = GlobalFunctions.GetFaCodeByPlant(model.Details.Werks);
            model.ZaapShiftList = GlobalFunctions.GetReversalData("", "");
            model.InventoryMovementList = GlobalFunctions.GetReversalInventoryMovementData("", "");

            return (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReversalIndexViewModel model)
        {
            try
            {
                ReversalDto item = new ReversalDto();

                item = Mapper.Map<ReversalDto>(model.Details);

                var createInput = Mapper.Map<ReversalCreateParamInput>(model.Details);
                createInput.ReversalId = 0;

                var checkData = _reversalBll.CheckData(createInput);

                if (checkData.IsForCk4cCompleted)
                {
                    AddMessageInfo("Can't create reversal data for ck4c completed", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                if (checkData.IsPackedQtyNotExists)
                {
                    AddMessageInfo("Can't create reversal data, no packed qty", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                if (checkData.IsMoreThanQuota)
                {
                    AddMessageInfo("Can't create reversal data, quota exceed", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                if (checkData.IsMoreThanPacked)
                {
                    AddMessageInfo("Can't create reversal data, reversal more than packed", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                var reversalData = _reversalBll.Save(item, CurrentUser.USER_ID);
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        #endregion

        #region Edit

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var reversalData = _reversalBll.GetById(id.Value);

            if (reversalData == null)
            {
                return HttpNotFound();
            }

            var model = new ReversalIndexViewModel();

            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                return RedirectToAction("Detail", new { id });
            }

            try
            {
                model.Details = Mapper.Map<DataReversal>(reversalData);
                model = InitialModel(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReversalIndexViewModel model)
        {
            try
            {
                ReversalDto item = new ReversalDto();

                item = Mapper.Map<ReversalDto>(model.Details);

                var createInput = Mapper.Map<ReversalCreateParamInput>(model.Details);

                var checkData = _reversalBll.CheckData(createInput);

                if (checkData.IsForCk4cCompleted)
                {
                    AddMessageInfo("Can't create reversal data for ck4c completed", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                if (checkData.IsPackedQtyNotExists)
                {
                    AddMessageInfo("Can't create reversal data, no packed qty", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                if (checkData.IsMoreThanQuota)
                {
                    AddMessageInfo("Can't create reversal data, quota exceed", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                if (checkData.IsMoreThanPacked)
                {
                    AddMessageInfo("Can't create reversal data, reversal more than packed", Enums.MessageInfoType.Info);
                    model = InitialModel(model);
                    return View(model);
                }

                var reversalData = _reversalBll.Save(item, CurrentUser.USER_ID);
                AddMessageInfo("Save Successfully", Enums.MessageInfoType.Success);
                return RedirectToAction("Edit", new { id = model.Details.ReversalId });
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
                return View(model);
            }
        }

        #endregion


        #region detail

        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var reversalData = _reversalBll.GetById(id.Value);

            if (reversalData == null)
            {
                return HttpNotFound();
            }

            var model = new ReversalIndexViewModel();

            try
            {
                model.Details = Mapper.Map<DataReversal>(reversalData);
                model = InitialModel(model);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        #endregion

        #region json data

        [HttpPost]
        public JsonResult GetFaCodeByPlant(string plantWerk)
        {
            var listBrandCe = GlobalFunctions.GetFaCodeByPlant(plantWerk);

            var model = new ReversalIndexViewModel() { FaCodeList = listBrandCe };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetZaapData(string plantWerk, string faCode)
        {
            var listZaap = GlobalFunctions.GetReversalData(plantWerk, faCode);

            var model = new ReversalIndexViewModel() { ZaapShiftList = listZaap };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetInventoryMovementData(string plantWerk, string faCode)
        {
            var listInventoryMovement = GlobalFunctions.GetReversalInventoryMovementData(plantWerk, faCode);

            var model = new ReversalIndexViewModel() { InventoryMovementList = listInventoryMovement };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetRemainingQuota(string zaapShift, string inventoryMovement)
        {
            if (zaapShift == "") { zaapShift="0";}
            if (inventoryMovement == "") { inventoryMovement = "0"; }
            var paramInput = new ReversalCreateParamInput();
            paramInput.ZaapShiftId = Convert.ToInt32(zaapShift);
            paramInput.InventoryMovementId = Convert.ToInt32(inventoryMovement);
            paramInput.ReversalQty = 0;
            paramInput.ReversalId = 0;
            paramInput.Werks = string.Empty;
            paramInput.FaCode = string.Empty;

            var checkData = _reversalBll.CheckData(paramInput);

            return Json(checkData.RemainingQuota);
        }

        [HttpPost]
        public JsonResult GetPackedData(string plantWerk, string faCode, DateTime prodDate)
        {
            var paramInput = new ReversalCreateParamInput();
            paramInput.ZaapShiftId = 0;
            paramInput.ReversalQty = 0;
            paramInput.ReversalId = 0;
            paramInput.Werks = plantWerk;
            paramInput.FaCode = faCode;
            paramInput.ProductionDate = prodDate;

            var checkData = _reversalBll.CheckData(paramInput);

            return Json(checkData.PackedQty);
        }

        #endregion
    }
}