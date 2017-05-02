using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.MasterDataApprovalSetting;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MasterDataApprovalSettingController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IMasterDataApprovalSettingBLL _masterDataApprovalSettingBLL;
        
        //
        // GET: /MasterDataApprovalSetting/
        public MasterDataApprovalSettingController(IPageBLL pageBll, IMasterDataApprovalSettingBLL masterDataSettingBLL) : base(pageBll, Enums.MenuList.MasterDataApproveSetting)
        {
            _masterDataApprovalSettingBLL = masterDataSettingBLL;
            _mainMenu = Enums.MenuList.Settings;
        }

        public ActionResult Index()
        {
            var data = _masterDataApprovalSettingBLL.GetAllMasterSettingsPage();
            var model = new MasterDataApprovalSettingIndexViewModel();
            model.MainMenu = _mainMenu;
            model.Details = Mapper.Map<List<MasterDataSetting>>(data);
            model.CurrentMenu = PageInfo;
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;

            return View("index",model);
        }

        public ActionResult Edit(int pageId)
        {
            var model = new MasterDataApprovalSettingEditViewModel();
            model.MainMenu = _mainMenu;
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            model.CurrentMenu = PageInfo;

            var data = _masterDataApprovalSettingBLL.GetAllEditableColumn(pageId);
            model.Detail = Mapper.Map<MasterDataSetting>(data);
            

            return View("Edit",model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MasterDataApprovalSettingEditViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            try
            {
                if (ModelState.IsValid)
                {
                    var data = Mapper.Map<MasterDataApprovalSettingDto>(model.Detail);
                    _masterDataApprovalSettingBLL.SaveSetting(data);
                    AddMessageInfo("Success", Enums.MessageInfoType.Success);
                    
                    return View("Edit", model);
                }
                else
                {
                    
                    // Retrieve the error messages as a list of strings.
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x=> x.Value.Errors).ToList();
                    var errorMessages = new List<string>();
                    foreach (var error in errors)
                    {
                        errorMessages.AddRange(error.Select(x=> x.ErrorMessage));
                    }

                    AddMessageInfo(errorMessages, Enums.MessageInfoType.Error);

                    return View("Edit", model);
                }

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);



                return View("Edit", model);
            }
        }

        public ActionResult Detail(int pageId)
        {
            var model = new MasterDataApprovalSettingEditViewModel();
            model.MainMenu = _mainMenu;
            model.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            model.CurrentMenu = PageInfo;

            var data = _masterDataApprovalSettingBLL.GetAllEditableColumn(pageId);
            model.Detail = Mapper.Map<MasterDataSetting>(data);


            return View("Detail", model);
        }
    }
}