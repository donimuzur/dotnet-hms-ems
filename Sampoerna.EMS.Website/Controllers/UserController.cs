using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using System.Collections.Generic;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class UserController : Controller
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
            return View(Mapper.Map<List<UserViewModel>>(users));
        }
	}
}