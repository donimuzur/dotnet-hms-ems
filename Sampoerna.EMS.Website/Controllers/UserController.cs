using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using System.Collections.Generic;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserController : BaseController
    {
        private IUserBLL _bll;
        private IChangesHistoryBLL _changesHistoryBll;
        private Enums.MenuList _mainMenu;

        public UserController(IUserBLL bll, IChangesHistoryBLL changesHistoryBll, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.USER)
        {
            _bll = bll;
            _changesHistoryBll = changesHistoryBll;
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

        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            
            var user = _bll.GetUserById(id);
            var changeHistoryList = _changesHistoryBll.GetByFormTypeId(Enums.MenuList.USER);
            var model = new UserItemViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Detail = Mapper.Map<UserItem>(user), 
                ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList)
            };

            return View("Detail",model);
        }
    }
}