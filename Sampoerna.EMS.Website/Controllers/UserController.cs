using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using System.Collections.Generic;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserController : BaseController
    {
        private IUserBLL _bll;
        private Enums.MenuList _mainMenu;
        public UserController(IUserBLL bll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.USER)
        {
            _bll = bll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /User/
        public ActionResult Index()
        {
            var input = new UserInput();
            List<USER> users = _bll.GetUsers(input);
            var model = new UserViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<UserItem>>(users)
            };
            return View(model);
        }

        public ActionResult ViewDetail(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            
            var user = _bll.GetUserTreeByUserID(id.Value);

            var model = new UserItemViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<UserItem>(user)
            };

            return View(model);
        }
    }
}