using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.NPPBKC;

namespace Sampoerna.EMS.Website.Controllers
{
    public class NPPBKCController : BaseController
    {
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IMasterDataBLL _masterDataBll;
        private ICompanyBLL _companyBll;

        public NPPBKCController(IZaidmExNPPBKCBLL nppbkcBll, ICompanyBLL companyBll, IMasterDataBLL masterData, IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterData;
            _companyBll = companyBll;
        }
        
        //
        // GET: /NPPBKC/
        public ActionResult Index()
        {
            var nppbkc = new NPPBKCViewModels();
            nppbkc.MainMenu = Enums.MenuList.MasterData;
            nppbkc.CurrentMenu = PageInfo;

            nppbkc.Details = Mapper.Map<List<DetailsNppbck>>(_nppbkcBll.GetAll());

            //ViewBag.Message = TempData["message"];
            return View("Index", nppbkc);
            
        }

        public ActionResult Edit(long id)
        {
            var nppbkc = _nppbkcBll.GetById(id);
            if (nppbkc == null)
            {
                HttpNotFound();
            }
            var menu = new DetailsNppbck();
            menu.MainMenu = Enums.MenuList.MasterData;
            menu.CurrentMenu = PageInfo; 

            var model = AutoMapper.Mapper.Map<NPPBKCViewModels>(nppbkc);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DetailsNppbck model)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                var nppbkcId = model.NppbckId;
                var nppbkc = _nppbkcBll.GetById(nppbkcId);

                 AutoMapper.Mapper.Map(model, nppbkc);
                 _nppbkcBll.Save(nppbkc);

                return RedirectToAction("Index");
            }
            catch 
            {
                return View();
            }
            
        }

	}
}