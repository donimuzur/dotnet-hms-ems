﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
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
        private IChangesHistoryBLL _changesHistoryBll;

        public NPPBKCController(IZaidmExNPPBKCBLL nppbkcBll,IChangesHistoryBLL changesHistoryBll, ICompanyBLL companyBll, IMasterDataBLL masterData, IZaidmExKPPBCBLL kppbcBll,
            IPageBLL pageBLL, IPlantBLL plantBll)
            : base(pageBLL, Enums.MenuList.MasterData)
        {
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterData;
            _companyBll = companyBll;
            _kppbcBll = kppbcBll;
            _plantBll = plantBll;
             _changesHistoryBll = changesHistoryBll;
        }

        //
        // GET: /NPPBKC/
        public ActionResult Index()
        {
            var plant = new NPPBKCIViewModels
            {
                MainMenu = Enums.MenuList.MasterData,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<VirtualNppbckDetails>>(_nppbkcBll.GetAll()) 
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
            if (nppbkc.IS_DELETED == true)
            {
                return RedirectToAction("Detail", "NPPBKC", new { id = nppbkc.NPPBKC_ID });
            }
            var model = new NppbkcFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            model.Plant = _plantBll.GetAll();

            model.Detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);

            return View(model);

        }
        private void SetChanges(VirtualNppbckDetails origin, ZAIDM_EX_NPPBKC nppbkc)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("REGION_OFFICE_DGCE", (origin.RegionOfficeOfDGCE == null ? true : origin.RegionOfficeOfDGCE.Equals(nppbkc.REGION_OFFICE_DGCE)));
            changesData.Add("CITY_ALIAS", (origin.CityAlias == null ? true : origin.CityAlias.Equals(nppbkc.CITY_ALIAS)));
            changesData.Add("TEXT_TO", (origin.TextTo == null ? true : origin.TextTo.Equals(nppbkc.TEXT_TO)));
           

            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.NPPBKC;
                    changes.FORM_ID = nppbkc.NPPBKC_ID;
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = CurrentUser.USER_ID;
                    changes.MODIFIED_DATE = DateTime.Now;
                    switch (listChange.Key)
                    {
                        case "REGION_OFFICE_DGCE":
                            changes.OLD_VALUE = origin.RegionOfficeOfDGCE;
                            changes.NEW_VALUE = nppbkc.REGION_OFFICE_DGCE;
                            break;

                        case "CITY_ALIAS":
                            changes.OLD_VALUE = origin.CityAlias;
                            changes.NEW_VALUE = nppbkc.CITY_ALIAS;
                            break;
                        case "TEXT_TO":
                            changes.OLD_VALUE = origin.TextTo;
                            changes.NEW_VALUE = nppbkc.TEXT_TO;
                            break;
                      }
                    _changesHistoryBll.AddHistory(changes);


                }
            }



        } 
        [HttpPost]
        public ActionResult Edit(NppbkcFormModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    var Nppbkc = _nppbkcBll.GetById(model.Detail.VirtualNppbckId);
            //    model.MainMenu = Enums.MenuList.MasterData;
            //    model.CurrentMenu = PageInfo;

            //    var detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(Nppbkc);

            //    model.Detail = detail;
            //    return View("Edit", model);
            //}

            try
            {
                var nppbkcId = model.Detail.VirtualNppbckId;
                var nppbkc = _nppbkcBll.GetById(nppbkcId);
                var origin = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);
                 AutoMapper.Mapper.Map(model.Detail, nppbkc);
                 SetChanges(origin,nppbkc);

                _nppbkcBll.Update(nppbkc);
                TempData[Constans.SubmitType.Save] = Constans.SubmitMessage.Saved;
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }

        }
        public ActionResult Detail(long id)
        {
            var nppbkc = _nppbkcBll.GetById(id);
            if (nppbkc == null)
            {
                HttpNotFound();
            }
            var changeHistoryList = _changesHistoryBll.GetByFormTypeId(Enums.MenuList.NPPBKC);

            var model = new NppbkcFormModel();
            model.MainMenu = Enums.MenuList.MasterData;
            model.CurrentMenu = PageInfo;
            var detail = AutoMapper.Mapper.Map<VirtualNppbckDetails>(nppbkc);
            model.Detail = detail;
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changeHistoryList);

            return View(model);

        }
        public ActionResult Delete(int id)
        {
            try
            {
                _nppbkcBll.Delete(id);
                TempData[Constans.SubmitType.Delete] = Constans.SubmitMessage.Deleted;
            }
            catch (Exception ex)
            {
                TempData[Constans.SubmitType.Delete] = ex.Message;
            }
            return RedirectToAction("Index");
        }

    }
}