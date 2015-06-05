using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using System.Collections.Generic;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserController : BaseController
    {
        private IUserBLL _bll;
        public UserController(IUserBLL bll)
        {
            _bll = bll;
        }

        //
        // GET: /User/
        public ActionResult Index()
        {
            var input = new UserInput();
            List<USER> users = _bll.GetUsers(input);
            var userViewModels = Mapper.Map<List<UserViewModel>>(users);
            return View(userViewModels);
        }

        public ActionResult ViewDetail(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");

            var user = _bll.GetUserTreeByUserID(id.Value);
            var userModel = Mapper.Map<UserViewModel>(user);
            return View(userModel);
        }
	}
}