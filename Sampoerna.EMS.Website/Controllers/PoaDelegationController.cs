using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using iTextSharp.text;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.PoaDelegation;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PoaDelegationController : BaseController
    {
        private IPoaDelegationBLL _poaDelegationBll;
        private IPOABLL _poabll;
        private IChangesHistoryBLL _changesHistoryBll;

        public PoaDelegationController(IPageBLL pageBLL, IPoaDelegationBLL poaDelegationBll, IPOABLL poabll,
            IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _poaDelegationBll = poaDelegationBll;
            _poabll = poabll;
            _changesHistoryBll = changesHistoryBll;
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

            var listPoa = _poabll.GetAllPoaActive();

            var selectList = from s in listPoa
                             select new SelectListItem
                             {
                                 Value = s.POA_ID,
                                 Text = s.POA_ID 
                                 //Text = s.POA_ID + "-" + s.PRINTED_NAME
                             };

            model.ListPoaFrom = new SelectList(selectList, "Value", "Text"); ;// GlobalFunctions.GetPoaAll(_poabll);
            model.ListPoaTo = new SelectList(selectList, "Value", "Text");

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

        public ActionResult Detail(int id)
        {
            var model = new PoaDelegationFormViewModel();

            try
            {
                model = Mapper.Map<PoaDelegationFormViewModel>(_poaDelegationBll.GetById(id));

                model.MainMenu = Enums.MenuList.MasterData;
                model.CurrentMenu = PageInfo;

                model.ChangesHistoryList =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PoaDelegation, id.ToString()));

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model.MainMenu = Enums.MenuList.MasterData;
                model.CurrentMenu = PageInfo;
            }

            return View("Detail", model);
        }

        public ActionResult Edit(int id)
        {
            var model = new PoaDelegationFormViewModel();

            try
            {
                model = Mapper.Map<PoaDelegationFormViewModel>(_poaDelegationBll.GetById(id));
                model = SetListModel(model);

                model.ChangesHistoryList =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PoaDelegation, id.ToString()));
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = SetListModel(model);
            }

            return View("Edit", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(PoaDelegationFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SavePoaDelegationToDatabase(model);
                    AddMessageInfo("Success update POA Delegation", Enums.MessageInfoType.Success);
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    AddMessageInfo("Update Failed : " + ex.Message, Enums.MessageInfoType.Error);
                }
            }

            model = SetListModel(model);
            return View("Edit", model);
        }
    }
}