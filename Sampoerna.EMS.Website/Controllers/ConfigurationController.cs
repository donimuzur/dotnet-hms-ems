using AutoMapper;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.Configuration;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Controllers
{
    public class ConfigurationController : BaseController
    {
        private Enums.MenuList _mainMenu;
        private IChangesHistoryBLL _changesHistoryBll;
        private ConfigurationService configService;

        public ConfigurationController(IPageBLL pageBLL, IChangesHistoryBLL changesHistoryBll)
            : base(pageBLL, Enums.MenuList.Configuration)
        {
            this._mainMenu = Enums.MenuList.MasterData;
            _changesHistoryBll = changesHistoryBll;
            configService = new ConfigurationService();
        }

        #region Index
        public ActionResult Index()
        {
            var model = new ConfigurationIndexViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer);
            model.Detail = Mapper.Map<List<ConfigurationViewModel>>(configService.GetAllList());
           
            var list = new List<ConfigurationViewModel>(model.Detail);
            model.Detail = new List<ConfigurationViewModel>();
           
            foreach(var item in list)
            {
                model.Detail.Add(item);
            }                       

            return View("Index", model);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new ConfigurationCreateViewModel();

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer);

            var typeList = configService.GetAllType().Select(item => new ConfigurationCreateViewModel()
            {
                ConfigType = item.SYS_REFFERENCES_TYPE1,
                ConfigText = item.SYS_REFFERENCES_TEXT
            });

            var hintList = configService.SelectNameByReferenceType("HINT_COMPONENT");
            var selectHintList = from s in hintList
                             select new SelectListItem
                             {
                                 Value = s.REFF_NAME,
                                 Text = s.REFF_NAME
                             };
            var nameHintList = new SelectList(selectHintList.GroupBy(p => p.Value).Select(g => g.First()), "Value", "Text");


            var approvalList = configService.SelectNameByReferenceType("APPROVAL_STATUS");
            var selectApprovalList = from s in approvalList
                                 select new SelectListItem
                                 {
                                     Value = s.REFF_NAME,
                                     Text = s.REFF_NAME
                                 };
            var nameApprovalList = new SelectList(selectApprovalList.GroupBy(p => p.Value).Select(g => g.First()), "Value", "Text");

            model.ApprovalList = nameApprovalList;
            model.HintList = nameHintList;

            var userList = configService.GetAdminAvailable().Select(item => new UserModel()
            {
                UserId = item.USER_ID,
                FullName = item.NAMA_USER,                
            });
        
            model.TypeList = GenericHelpers<ConfigurationCreateViewModel>.GenerateList(typeList, item => item.ConfigType, item => item.ConfigText);
            model.UserList = userList.ToList();

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(ConfigurationCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {                    
                    AddMessageInfo("Data not complete. Please fill required field !", Enums.MessageInfoType.Error);
                    return RedirectToAction("Create");
                }
                else
                {
                    var data = new SYS_REFFERENCES();

                    if (model.ConfigText == "UPLOAD_FILE_LIMIT" || model.ConfigText == "APPROVAL_STATUS" || model.ConfigText== "HINT_COMPONENT")
                    {
                        var config= configService.FindDataByType(model.ConfigText);
                      
                       
                        if (model.ConfigText == "APPROVAL_STATUS" || model.ConfigText == "HINT_COMPONENT")
                        {
                            var tempConfig = configService.FindDataByName(model.ConfigName);
                            data.REFF_ID = tempConfig.REFF_ID;
                            data.REFF_TYPE = tempConfig.REFF_TYPE;
                            data.REFF_NAME = tempConfig.REFF_NAME;
                            data.REFF_KEYS = tempConfig.REFF_KEYS;
                        }

                        if (model.ConfigText == "UPLOAD_FILE_LIMIT")
                        {
                            data.REFF_ID = config.REFF_ID;
                            data.REFF_TYPE = config.REFF_TYPE;
                            data.REFF_NAME = config.REFF_NAME;
                            data.REFF_KEYS = config.REFF_KEYS;
                        }
                        
                        data.REFF_VALUE = model.ConfigValue;
                        data.IS_ACTIVE = model.IsActive;
                        data.CREATED_BY = config.CREATED_BY;
                        data.CREATED_DATE = config.CREATED_DATE;
                        data.LASTMODIFIED_BY = CurrentUser.USER_ID;
                        data.LASTMODIFIED_DATE = DateTime.Now;

                        configService.UpdateSysReff(data, (int)Enums.MenuList.Configuration, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                        AddMessageInfo("Save Configuration Succeed", Enums.MessageInfoType.Success);
                    }                    
                    else
                    {
                        data.REFF_TYPE = model.ConfigText;
                        data.REFF_NAME = model.ConfigText;
                        data.REFF_KEYS = model.ConfigText;                        
                        data.REFF_VALUE = model.ConfigValue;
                        data.CREATED_BY = CurrentUser.USER_ID;
                        data.CREATED_DATE = DateTime.Now;
                        data.LASTMODIFIED_BY = CurrentUser.USER_ID;
                        data.LASTMODIFIED_DATE = DateTime.Now;
                        data.IS_ACTIVE = model.IsActive;

                        configService.CreateSysReff(data, (int)Enums.MenuList.Configuration, (int)Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                        AddMessageInfo("Save Configuration Succeed", Enums.MessageInfoType.Success);
                    }                                     
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public ActionResult Edit(long id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var data = configService.GetConfigDataByID(id);
            var model = new ConfigurationCreateViewModel();
            var detail = Mapper.Map<ConfigurationViewModel>(data);

            var changeHistoryList = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.Configuration, id.ToString());

            var typeList = configService.GetAllType().Select(item => new ConfigurationCreateViewModel()
            {
                ConfigType = item.SYS_REFFERENCES_TYPE1,
                ConfigText = item.SYS_REFFERENCES_TEXT
            });

            var userList = configService.GetAdminAvailable().Select(item => new UserModel()
            {
                UserId = item.USER_ID,
                FullName = item.NAMA_USER
            });

            try
            {
                model.TypeList = GenericHelpers<ConfigurationCreateViewModel>.GenerateList(typeList, item => item.ConfigType, item => item.ConfigText);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.UserList = userList.ToList();
                model.ConfigModel = detail;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer);

            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(ConfigurationCreateViewModel model)
        {
            try
            {
                var reffID = model.ConfigModel.REFF_ID;
                var config = configService.GetConfigDataByID(reffID);
                var data = new SYS_REFFERENCES();

                data.REFF_ID = reffID;
                data.REFF_TYPE = config.REFF_TYPE;
                data.REFF_NAME = config.REFF_NAME;
                data.REFF_KEYS = config.REFF_KEYS;
                data.REFF_VALUE = model.ConfigModel.REFF_VALUE;
                //data.REFF_VALUE = model.ConfigValue;
                data.CREATED_BY = config.CREATED_BY;
                data.CREATED_DATE = config.CREATED_DATE;
                data.LASTMODIFIED_BY = CurrentUser.USER_ID;
                data.LASTMODIFIED_DATE = DateTime.Now;
                data.IS_ACTIVE = model.ConfigModel.IS_ACTIVE;

                configService.UpdateSysReff(data, (int)Enums.MenuList.Configuration, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                AddMessageInfo("Success Update Configuration", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo("Save Failed : " + ex.Message, Enums.MessageInfoType.Error);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public ActionResult Detail(long id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new ConfigurationViewModel();
            var data = configService.GetConfigDataByID(id);
            var changeHistoryList = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.Configuration, id.ToString());
            var typeList = configService.GetAllType().Select(item => new ConfigurationCreateViewModel()
            {
                ConfigType = item.SYS_REFFERENCES_TYPE1,
                ConfigText = item.SYS_REFFERENCES_TEXT
            });

            try
            {
                model = Mapper.Map<ConfigurationViewModel>(data);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
                model.TypeList = GenericHelpers<ConfigurationCreateViewModel>.GenerateList(typeList, item => item.ConfigType, item => item.ConfigText);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer);

            return View("Detail", model);
        }
        #endregion

        #region Helper

        [HttpPost]
        public JsonResult ConfigNameList(string configType)
        {
            var tempList = configService.SelectNameByReferenceType(configType);
            var selectList = from s in tempList
                             select new SelectListItem
                             {
                                 Value = s.REFF_NAME,
                                 Text = s.REFF_NAME
                             };
            var nameList = new SelectList(selectList.GroupBy(p => p.Value).Select(g => g.First()), "Value", "Text");

            return Json(nameList);
        }

        [HttpPost]
        public JsonResult GetNameValue(string field)
        {
            var tempValue = configService.FindDataByName(field);
            string result = "";
            if (tempValue != null)
            {
                if (tempValue.REFF_VALUE == null)
                {
                    result = "";
                }             
            }
            else
            {
                result = "";
            }

            return Json(new
            {
                IsActive = tempValue.IS_ACTIVE,
                ReffValue = tempValue.REFF_VALUE,
                ReffName = tempValue.REFF_NAME,
                ReffId = tempValue.REFF_ID
            }, result);                    
        }

        [HttpPost]
        public JsonResult GetUploadFileLimitValue(string field)
        {
            var tempValue = configService.FindDataByKey(field);
          //  string result = tempValue.REFF_VALUE.ToString();

            return Json(new
            {
                IsActive = tempValue.IS_ACTIVE,
                ReffValue = tempValue.REFF_VALUE                
            });
        }
        #endregion

    }
}