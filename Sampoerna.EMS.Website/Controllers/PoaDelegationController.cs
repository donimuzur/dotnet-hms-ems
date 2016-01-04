using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using iTextSharp.text;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.PoaDelegation;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PoaDelegationController : BaseController
    {
        private IPoaDelegationBLL _poaDelegationBll;
        private IPOABLL _poabll;

        public PoaDelegationController(IPageBLL pageBLL, IPoaDelegationBLL poaDelegationBll, IPOABLL poabll)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _poaDelegationBll = poaDelegationBll;
            _poabll = poabll;
        }

        // GET: /PoaDelegation/
        public ActionResult Index()
        {
            var model = new PoaDelegationIndexViewModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            model.ListPoaDelegations =
                AutoMapper.Mapper.Map<List<PoaDelegationFormViewModel>>(_poaDelegationBll.GetAllData());
            
            return View(model);
        }

        private PoaDelegationFormViewModel SetListModel(PoaDelegationFormViewModel model)
        {
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            model.ListPoaFrom = GlobalFunctions.GetPoaAll(_poabll);
            model.ListPoaTo = GlobalFunctions.GetPoaAll(_poabll);

            //model.PlantList = GlobalFunctions.GetPlantAll();
            //model.MaterialNumberList = GetMaterialList(model.PlantId);

            return model;
        }
        
        public ActionResult Create()
        {
            var model = new PoaDelegationFormViewModel();
            model = SetListModel(model);
            return View("Create", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(PoaDelegationFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SavePoaDelegationToDatabase(model);
                    AddMessageInfo("Success create Poa Delegation", Enums.MessageInfoType.Success);
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

        private POA_DELEGATIONDto SavePoaDelegationToDatabase(PoaDelegationFormViewModel model)
        {

            var dataToSave = Mapper.Map<POA_DELEGATIONDto>(model);

            var input = new PoaDelegationSaveInput()
            {
                PoaDelegationDto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
            };

            return _poaDelegationBll.SavePoaDelegation(input);
        }
	}
}