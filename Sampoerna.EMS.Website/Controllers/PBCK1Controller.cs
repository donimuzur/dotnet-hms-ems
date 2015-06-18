﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Utility;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK1Controller : BaseController
    {
        private IPBCK1BLL _pbck1Bll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IZaidmExPOAMapBLL _poaMapBll;
        private IUserBLL _userBll;
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IMonth _monthBll;
        public PBCK1Controller(IPageBLL pageBLL, IPBCK1BLL pbckBll, IZaidmExNPPBKCBLL nppbkcbll,
            IZaidmExPOAMapBLL poaMapBll, IUserBLL userBll, IZaidmExProdTypeBLL prodTypeBll, IMonth monthBll)
            : base(pageBLL, Enums.MenuList.PBCK1)
        {
            _pbck1Bll = pbckBll;
            _nppbkcbll = nppbkcbll;
            _poaMapBll = poaMapBll;
            _userBll = userBll;
            _prodTypeBll = prodTypeBll;
            _monthBll = monthBll;
        }

        private SelectList GetNPPBKC()
        {
            var nppbkcList = _nppbkcbll.GetAll();
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(nppbkcList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        private List<PBCK1Item> GetPBCKItems(PBCK1FilterViewModel filter = null)
        {
            if (filter == null)
            {
                //Get All
                return Mapper.Map<List<PBCK1Item>>(_pbck1Bll.GetPBCK1ByParam(new PBCK1Input()));
            }
            //getbyparams
            var input = Mapper.Map<PBCK1Input>(filter);
            return Mapper.Map<List<PBCK1Item>>(_pbck1Bll.GetPBCK1ByParam(input));
        }

        private SelectList GetPoaByNppbkcId(string nppbkcId)
        {
            var poaList = _poaMapBll.GetPOAByNPPBKCID(nppbkcId);
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(poaList);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        private SelectList GetCreatorList()
        {
            var users = _userBll.GetUsers(new UserInput());
            var selectItemSource = Mapper.Map<List<SelectItemModel>>(users);
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        private SelectList GetYearList()
        {
            int currentYear = DateTime.Now.Year;
            var selectItemSource = new List<SelectItemModel>();
            for (var i = 0; i < 5; i++)
            {
                selectItemSource.Add(new SelectItemModel()
                {
                    ValueField = (currentYear - i),
                    TextField = (currentYear - i).ToString()
                });
            }
            return new SelectList(selectItemSource, "ValueField", "TextField");
        }

        //
        // GET: /PBCK/
        public ActionResult Index()
        {
            return IndexInitial(new PBCK1ViewModel()
            {
                MainMenu = Enums.MenuList.ExcisableGoodsMovement,
                CurrentMenu = PageInfo
            });
        }

        public ActionResult IndexInitial(PBCK1ViewModel model)
        {
            model.SearchInput.YearList = GetYearList();
            model.SearchInput.NPPBKCIDList = GetNPPBKC();
            model.SearchInput.CreatorList = GetCreatorList();
            model.SearchInput.POAList = new SelectList(new List<SelectItemModel>(), "ValueField", "TextField");
            model.Details = GetPBCKItems();
            return View("Index", model);
        }

        public ActionResult Create()
        {
            var model = new PBCK1ItemViewModel
            {
                MainMenu = Enums.MenuList.ExcisableGoodsMovement,
                CurrentMenu = PageInfo,
                Detail = null
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult PoaListPartial(string nppbkcId)
        {
            var listPoa = GetPoaByNppbkcId(nppbkcId);
            var model = new PBCK1ViewModel { SearchInput = { POAList = listPoa } };
            //return PartialView("PoaListPartial", model);
            return Json(model);
        }
        [HttpPost]
        public PartialViewResult UploadFileConversion(HttpPostedFileBase ProdConvExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(ProdConvExcelFile);
            var model = new PBCK1ItemViewModel();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var prodConvModel = new PBCK1ProdConvModel();
                   
                    try
                    {
                        var prodCodeFromFile = Convert.ToInt32(datarow[0]);
                        var prodType = _prodTypeBll.GetByCode(prodCodeFromFile);
                        if (prodType != null)
                        {
                            prodConvModel.ProductCode = prodType.PRODUCT_CODE;
                            prodConvModel.ProductType = prodType.PRODUCT_TYPE;
                            prodConvModel.ProductTypeAlias = prodType.PRODUCT_ALIAS;
                            prodConvModel.ConverterOutput = Convert.ToDecimal(datarow[1]);
                            prodConvModel.ConverterUom = datarow[2];
                            model.ProductConversions.Add(prodConvModel);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        
                    }
                    
                }
            }
            return PartialView("_ProdConvList", model);
        }
        [HttpPost]
        public PartialViewResult UploadFilePlan(HttpPostedFileBase ProdPlanExcelFile)
        {
            var data = (new ExcelReader()).ReadExcel(ProdPlanExcelFile);
            var model = new PBCK1ItemViewModel();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var prodPlanModel = new PBCK1ProdPlanModel();

                    try
                    {
                        var month = _monthBll.GetMonth(Convert.ToInt32(datarow[0]));
                        var prodCodeFromFile = Convert.ToInt32(datarow[1]);
                        var prodType = _prodTypeBll.GetByCode(prodCodeFromFile);
                        if (prodType != null)
                        {
                            prodPlanModel.MonthName = month.MONTH_NAME_IND;
                            prodPlanModel.ProductCode = prodType.PRODUCT_CODE;
                            prodPlanModel.ProductType = prodType.PRODUCT_TYPE;
                            prodPlanModel.ProductTypeAlias = prodType.PRODUCT_ALIAS;
                            prodPlanModel.Amount = Convert.ToDecimal(datarow[2]);
                            prodPlanModel.BKCRequires = datarow[3];
                            model.ProductPlans.Add(prodPlanModel);
                        }
                    }
                    catch (Exception)
                    {
                        continue;

                    }

                }
            }
            return PartialView("_ProdPlanList", model);
        }

    }
}