using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
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
                ProductionDate = DateTime.Today.ToString("dd MMM yyyy"),
                IsShowNewButton = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
            });

            return View("Index", data);
        }

        private ReversalIndexViewModel InitIndexViewModel(
            ReversalIndexViewModel model)
        {
            model.PlantWerksList = GlobalFunctions.GetPlantAll();

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

            var dbData = _reversalBll.GetListDocumentByParam(input).OrderByDescending(c => c.ProductionDate);
            return Mapper.Map<List<DataReversal>>(dbData);
        }

        #endregion


        #region create new reversal

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager || CurrentUser.UserRole == Enums.UserRole.Viewer)
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
            var userPlant = _userPlantBll.GetPlantByUserId(CurrentUser.USER_ID);
            var poaPlant = _poaMapBll.GetPlantByPoaId(CurrentUser.USER_ID);
            var distinctPlant = plantList.Where(x => userPlant.Contains(x.Value) || poaPlant.Contains(x.Value));
            var getPlant = new SelectList(distinctPlant, "Value", "Text");

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.PlantWerksList = getPlant;
            model.FaCodeList = GlobalFunctions.GetFaCodeByPlant("");
            model.ZaapShiftList = GlobalFunctions.GetReversalData("", "");

            return (model);
        }

        #endregion

        #region json data

        [HttpPost]
        public JsonResult GetBrandCeByPlant(string plantWerk)
        {
            var listBrandCe = GlobalFunctions.GetFaCodeByPlant(plantWerk);

            var model = new ReversalIndexViewModel() { FaCodeList = listBrandCe };

            return Json(model);
        }

        #endregion
    }
}