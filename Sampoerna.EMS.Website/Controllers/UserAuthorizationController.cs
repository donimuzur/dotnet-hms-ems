using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.UserAuthorization;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserAuthorizationController : BaseController
    {
        private IUserAuthorizationBLL _userAuthorizationBll;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBll;
        private IChangesHistoryBLL _changesHistoryBll;
        public UserAuthorizationController(IPageBLL pageBll, IUserAuthorizationBLL userAuthorization, IChangesHistoryBLL changesHistoryBll) 
            : base (pageBll, Enums.MenuList.UserAuthorization)
        {
            _userAuthorizationBll = userAuthorization;
            _mainMenu = Enums.MenuList.MasterData;
            _pageBll = pageBll;
            _changesHistoryBll = changesHistoryBll;
        }

        //
        // GET: /UserAuthorization/
        public ActionResult Index()
        {
            var model = new IndexUserAuthorizationViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<List<DetailIndexUserAuthorization>>(_userAuthorizationBll.GetAll())
            };
            
            return View("Index", model);
        }

        private EditUserAuthorizationViewModel SetDetailModel(string id)
        {
            var model = new EditUserAuthorizationViewModel();
            model.RoleAuthorizationDto = _userAuthorizationBll.GetById(id);
            model.Pages = GlobalFunctions.GetModuleList();
            if (model.RoleAuthorizationDto.PageMaps != null)
            {
                var pageMaps = model.RoleAuthorizationDto.PageMaps;
                foreach (var pageMapDto in model.Pages)
                {

                    var isChecked = pageMaps.Any(x => x.Page.Id == pageMapDto.Id);
                    pageMapDto.IsChecked = isChecked;

                }
            }
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            return model;
        }

        public ActionResult Edit(string id)
        {
            var model = SetDetailModel(id);
            return View("Edit", model);
        }
        [HttpPost]
        public ActionResult Edit(EditUserAuthorizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var originPageMap = _userAuthorizationBll.GetById(model.RoleAuthorizationDto.Brole);
               
                if (originPageMap.PageMaps != null)
                {
                    var pageMaps = originPageMap.PageMaps;
                    foreach (var pageMap in pageMaps)
                    {
                        _pageBll.DeletePageMap(pageMap.Id);

                    }
                }
                var listNewPages = new List<PageMapDto>();
                if (model.Pages != null)
                {
                    foreach (var page in model.Pages)
                    {
                        if (page.IsChecked)
                        {
                            var pageMapDto = new PageMapDto();
                            pageMapDto.Page = page;
                            pageMapDto.Brole = model.RoleAuthorizationDto.Brole;
                            listNewPages.Add(pageMapDto);
                           
                        }
                    }
                }
                    SetChanges(originPageMap,  listNewPages);

                    foreach (var listNewPage in listNewPages)
                    {
                         var pageMapToSave = new PAGE_MAP();
                         pageMapToSave.PAGE_ID = listNewPage.Page.Id;
                         pageMapToSave.BROLE = listNewPage.Brole;
                         _pageBll.Save(pageMapToSave);
                        
                    }
            }


            return RedirectToAction("Index");
        }

        public ActionResult Detail(string id)
        {
            var model = SetDetailModel(id);
            var changeHistoryList = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.UserAuthorization, id);
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);
          
            return View("Detail", model);
        }

        private void SetChanges(UserAuthorizationDto origin, List<PageMapDto> New)
        {
            var changesData = new Dictionary<string, bool>();
            var originPageList = string.Empty;
            if (origin.PageMaps != null)
            {
                for (int i = 0; i < origin.PageMaps.Count; i++)
                {
                    originPageList += origin.PageMaps[i].Page.PageName;
                    if (i < (origin.PageMaps.Count - 1))
                    {
                        originPageList += ", ";
                    }
                }
            }
            var newPageList = string.Empty;
            if (New != null)
            {
                for (int i = 0; i < New.Count; i++)
                {
                    newPageList += New[i].Page.PageName;
                    if (i < (New.Count - 1))
                    {
                        newPageList += ", ";
                    }
                }
            }

            if (originPageList != newPageList)
            {
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.UserAuthorization;
                changes.FORM_ID = origin.Brole;
                changes.FIELD_NAME = "PAGE";
                changes.MODIFIED_BY = CurrentUser.USER_ID;
                changes.MODIFIED_DATE = DateTime.Now;

                changes.OLD_VALUE = originPageList;
                changes.NEW_VALUE = newPageList;


                _changesHistoryBll.AddHistory(changes);

            }

                    


                
            




        } 
    
       
       
    }
}