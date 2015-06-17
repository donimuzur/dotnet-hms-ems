using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.CK5;

namespace Sampoerna.EMS.Website.Controllers
{
    public class CK5Controller : BaseController
    {
        private ICK5BLL _ck5Bll;

        public CK5Controller(IPageBLL pageBLL, ICK5BLL ck5Bll)
            : base(pageBLL, Enums.MenuList.CK5)
        {
            _ck5Bll = ck5Bll;
        }

     

        private CK5IndexViewModel CreateInitModel(Enums.MenuList menulist)
        {
            var model = new CK5IndexViewModel();

            model.MainMenu = menulist;
            model.CurrentMenu = PageInfo;

            model.SearchView.DocumentNumberList = new SelectList(_ck5Bll.GetAll(), "CK5_NUMBER", "CK5_NUMBER");

            return model;
        }
        //
        // GET: /CK5/
        public ActionResult Index()
        {
            var model = CreateInitModel(Enums.MenuList.CK5);
            return View(model);
        }

        public ActionResult Intercompany()
        {
            //var model = GetPBCKData();
            //model.Ck5Type = Enums.CK5Type.Intercompany;
            //return View("Index", model);
            return View("Index");
        }
    }
}