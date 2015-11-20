using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.WasteRole;

namespace Sampoerna.EMS.Website.Controllers
{
    public class WasteRoleController : BaseController
    {
         private Enums.MenuList _mainMenu;
        private IWasteRoleBLL _wasteRoleBll;
        private IUserBLL _userBll;
        private IChangesHistoryBLL _changesHistoryBll;

        public WasteRoleController(IWasteRoleBLL wasteRoleBll, IPageBLL pageBLL,
            IUserBLL userBll, IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.WasteRole)
        {
            _wasteRoleBll = wasteRoleBll;
            _mainMenu = Enums.MenuList.MasterData;

            _userBll = userBll;
            _changesHistoryBll = changesHistoryBll;

        }

        //
        // GET: /WasteRole/
        public ActionResult Index()
        {
            var model = new WasteRoleIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.ListWasteRoles = Mapper.Map<List<WasteRoleFormViewModel>>(_wasteRoleBll.GetAllDataOrderByUserAndGroupRole());

            return View("Index",model);
        }

        private SelectList GetUserIdList()
        {
            var users = _userBll.GetUsers();
            IEnumerable<SelectItemModel> query;
            query = from x in users
                    select new SelectItemModel()
                    {
                        ValueField = x.USER_ID,
                        TextField = x.USER_ID + " - " + x.FIRST_NAME
                    };


            return new SelectList(query, "ValueField", "TextField");

            //return new SelectList(users, "USER_ID", "USER_ID");

        }

        private WasteRoleFormViewModel SetListModel(WasteRoleFormViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.UserList = GetUserIdList();
            model.PlantList = GlobalFunctions.GetPlantAll();
            var listDetailsWaste = new List<WasteRoleFormDetails>();

            foreach (Enums.WasteGroup val in Enum.GetValues(typeof(Enums.WasteGroup)))
            {
                var detailsWaste = new WasteRoleFormDetails();
                detailsWaste.IsChecked = false;
                detailsWaste.WasteGroup = val;
                detailsWaste.WasteGroupDescription = EnumHelper.GetDescription(val);

                listDetailsWaste.Add(detailsWaste);
            }

            model.Details = listDetailsWaste;
            return model;
        }

        public ActionResult Create()
        {
            var model = new WasteRoleFormViewModel();
            model = SetListModel(model);
            return View("Create",model);
        }

        public ActionResult Edit(int id)
        {
            var model = new WasteRoleFormViewModel();

            try
            {
                model = Mapper.Map<WasteRoleFormViewModel>(_wasteRoleBll.GetById(id, true));
                model = SetListModel(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = SetListModel(model);
            }

            return View("Edit", model);
        }

        public ActionResult Detail(int id)
        {
            var model = new WasteRoleFormViewModel();

            try
            {
                model = Mapper.Map<WasteRoleFormViewModel>(_wasteRoleBll.GetById(id, true));

                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

                model.ChangesHistoryList =
                    Mapper.Map<List<ChangesHistoryItemModel>>(
                        _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.WasteRole, id.ToString()));


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
        public JsonResult GetUserDetails(string userId)
        {
            var dbUser = _userBll.GetUserById(userId);
            var model = Mapper.Map<WasteRoleFormViewModel>(dbUser);

         
            return Json(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(WasteRoleFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SaveWasteRoleToDatabase(model);
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(WasteRoleFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SaveWasteRoleToDatabase(model);
                    AddMessageInfo("Success update Waste Role", Enums.MessageInfoType.Success);
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

        private WasteRoleDto SaveWasteRoleToDatabase(WasteRoleFormViewModel model)
        {

            var dataToSave = Mapper.Map<WasteRoleDto>(model);


            var input = new WasteRoleSaveInput()
            {
                WasteRoleDto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
            };

            return _wasteRoleBll.SaveWasteRole(input);
        }

       
	}
}