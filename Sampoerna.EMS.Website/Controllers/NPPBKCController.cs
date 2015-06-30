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

        public NPPBKCController(IZaidmExNPPBKCBLL nppbkcBll, ICompanyBLL companyBll, IMasterDataBLL masterData, IZaidmExKPPBCBLL kppbcBll,  IPageBLL pageBLL)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterData;
            _companyBll = companyBll;
            _kppbcBll = kppbcBll;
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

            model.Detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);
            
            //var model = new NppbkcFormModel();
            //model.Detail.NppbckNo = nppbkc.NPPBKC_NO;
            //model.Detail.City = nppbkc.CITY;
            //model.Detail.Address1 = nppbkc.ADDR1;
            //model.Detail.RegionOfficeOfDGCE = nppbkc.REGION_OFFICE;
            //model.Detail.Address2 = nppbkc.ADDR2;
            //model.Detail.TextTo = nppbkc.TEXT_TO;
            //model.Detail.KppbcId = nppbkc.ZAIDM_EX_KPPBC.KPPBC_NUMBER;
            //model.Detail.CityAlias = nppbkc.CITY_ALIAS;
            ////model.Detail.RegionOfficeOfDGCE = nppbkc.REGION_OFFICE;
            //model.Detail.AcountNumber = nppbkc.C1LFA1.LIFNR;
            //model.Detail.StartDate = nppbkc.START_DATE;
            //model.Detail.EndDate = nppbkc.END_DATE;
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