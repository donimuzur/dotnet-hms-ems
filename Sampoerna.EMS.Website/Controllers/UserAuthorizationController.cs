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
using Sampoerna.EMS.Website.Models.UserAuthorization;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserAuthorizationController : BaseController
    {
        private IUserAuthorizationBLL _userAuthorizationBll;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBll;
        public UserAuthorizationController(IPageBLL pageBll, IUserAuthorizationBLL userAuthorization) 
            : base (pageBll, Enums.MenuList.UserAuthorization)
        {
            _userAuthorizationBll = userAuthorization;
            _mainMenu = Enums.MenuList.MasterData;
            _pageBll = pageBll;
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

        public ActionResult Edit(string id)
        {
            var model = new EditUserAuthorizationViewModel();
            model.RoleAuthorizationDto = _userAuthorizationBll.GetById(id);
            model.Pages = GlobalFunctions.GetPageList();
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
                
                if (model.Pages != null)
                {
                    foreach (var page in model.Pages)
                    {
                        if (page.IsChecked)
                        {
                            //var pageMapDto = new PageMapDto();
                            //pageMapDto.Page = page;
                            //pageMapDto.Brole = model.RoleAuthorizationDto.Brole;
                            //var pageMapToSave = Mapper.Map<PAGE_MAP>(pageMapDto);
                            var pageMapToSave = new PAGE_MAP();
                            pageMapToSave.PAGE_ID = page.Id;
                            pageMapToSave.BROLE = model.RoleAuthorizationDto.Brole;
                            
                       
                            _pageBll.Save(pageMapToSave);
                        }
                    }
                }
            }


            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult GetMapByBroleId(string id)
        {
          var result =  _userAuthorizationBll.GetById(id);
          return Json(result);

        }
    }
}