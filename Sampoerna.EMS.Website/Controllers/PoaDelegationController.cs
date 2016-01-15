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
        private IUserBLL _userBll;


        public PoaDelegationController(IPageBLL pageBLL, IPoaDelegationBLL poaDelegationBll, IPOABLL poabll,
            IChangesHistoryBLL changesHistoryBll, IUserBLL userBll)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _poaDelegationBll = poaDelegationBll;
            _poabll = poabll;
            _changesHistoryBll = changesHistoryBll;
            _userBll = userBll;
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

        private PoaDelegationFormViewModel SetListModel(PoaDelegationFormViewModel model, bool isCreateView)
        {
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;

            if (isCreateView)
            {
                var listUsers = _userBll.GetUsers();

                var selectList = from s in listUsers
                                 select new SelectListItem
                                 {
                                     Value = s.USER_ID,
                                     Text = s.USER_ID
                                     //Text = s.POA_ID + "-" + s.PRINTED_NAME
                                 };

                model.ListPoaFrom = new SelectList(selectList, "Value", "Text"); ;// GlobalFunctions.GetPoaAll(_poabll);
                model.ListPoaTo = new SelectList(selectList, "Value", "Text");
            }

            else
            {
                var listUsers = _userBll.GetUsers();

                var selectList = from s in listUsers
                                 select new SelectListItem
                                 {
                                     Value = s.USER_ID,
                                     Text = s.USER_ID
                                 };

                model.ListPoaFrom = new SelectList(selectList, "Value", "Text"); ;// GlobalFunctions.GetPoaAll(_poabll);

                var listUsersTo = _userBll.GetListUserRoleByUserId(model.PoaFrom);

                var selectListTo = from s in listUsersTo
                                 select new SelectListItem
                                 {
                                     Value = s.UserId,
                                     Text = s.UserId
                                 };

                model.ListPoaTo = new SelectList(selectListTo, "Value", "Text");
            }
          
            return model;
        }

        public ActionResult Create()
        {
            var model = new PoaDelegationFormViewModel();
            model = SetListModel(model, true);
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

            model = SetListModel(model,false);
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
                model = SetListModel(model, false);

                model.ChangesHistoryList =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PoaDelegation, id.ToString()));
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = SetListModel(model, false);
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

            model = SetListModel(model, false);
            return View("Edit", model);
        }

        [HttpPost]
        public JsonResult GetListUsersRole(string userId)
        {
            var dbUser = _userBll.GetListUserRoleByUserId(userId);

            //var model = Mapper.Map<WasteRoleFormViewModel>(dbUser);


            return Json(dbUser);
        }


    }
}