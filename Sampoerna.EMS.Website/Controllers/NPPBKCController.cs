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
        private IZaidmExKPPBCBLL _kppbcBll;
        private IPlantBLL _plantBll;

        public NPPBKCController(IZaidmExNPPBKCBLL nppbkcBll, ICompanyBLL companyBll, IMasterDataBLL masterData, IZaidmExKPPBCBLL kppbcBll, 
            IPageBLL pageBLL, IPlantBLL plantBll) : base(pageBLL, Enums.MenuList.MasterData)
        {
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterData;
            _companyBll = companyBll;
            _kppbcBll = kppbcBll;
            _plantBll = plantBll;
        }
        
        //
        // GET: /NPPBKC/
        public ActionResult Index()
        {
            var plant = new NPPBKCIViewModels
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
                Details = _nppbkcBll.GetAll()
            };

            //ViewBag.Message = TempData["message"];
            return View("Index", plant);
            
        }

        public ActionResult Edit(long id)
        {
            var nppbkc = _nppbkcBll.GetById(id);
            if (nppbkc == null)
            {
                HttpNotFound();
            }
            var model = new NppbkcFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.Plant = _plantBll.GetAll();
            
            model.Detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);
            
          return View(model);

           
        }

        [HttpPost]
        public ActionResult Edit(NppbkcFormModel model)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                var nppbkcId = model.Detail.VirtualNppbckId;
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