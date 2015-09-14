using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.PBCK7AndPBCK3;
using Sampoerna.EMS.Website.Utility;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK7AndPBCK3Controller : BaseController
    {
        private IPBCK7And3BLL _pbck7AndPbck7And3Bll;
        private IBACK1BLL _back1Bll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poaBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IPlantBLL _plantBll;
        private IBrandRegistrationBLL _brandRegistration;
        public PBCK7AndPBCK3Controller(IPageBLL pageBll, IPBCK7And3BLL pbck7AndPbck3Bll, IBACK1BLL back1Bll,
            IPOABLL poaBll, IZaidmExNPPBKCBLL nppbkcBll, IBrandRegistrationBLL brandRegistrationBll, IPlantBLL plantBll)
            : base(pageBll, Enums.MenuList.PBCK7)
        {
            _pbck7AndPbck7And3Bll = pbck7AndPbck3Bll;
            _back1Bll = back1Bll;
            _mainMenu = Enums.MenuList.PBCK7;
            _poaBll = poaBll;
            _nppbkcBll = nppbkcBll;
            _plantBll = plantBll;
            _brandRegistration = brandRegistrationBll;
        }

        #region Index PBCK7

        //
        // GET: /PBCK7/
        public ActionResult Index()
        {
            var data = InitPbck7ViewModel(new Pbck7IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck7Type = Enums.Pbck7Type.Pbck7List,

                Detail =
                    Mapper.Map<List<DataListIndexPbck7>>(_pbck7AndPbck7And3Bll.GetAllByParam(new Pbck7AndPbck3Input()))
            });

            return View("Index", data);
        }

        #endregion

        private Pbck7IndexViewModel InitPbck7ViewModel(Pbck7IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;

        }

        [HttpPost]
        public PartialViewResult FilterPbck7Index(Pbck7IndexViewModel model)
        {
            var input = Mapper.Map<Pbck7AndPbck3Input>(model);
            input.Pbck7AndPvck3Type = Enums.Pbck7Type.Pbck7List;
            if (input.Pbck7Date != null)
            {
                input.Pbck7Date = Convert.ToDateTime(input.Pbck7Date).ToString();
            }



            var dbData = _pbck7AndPbck7And3Bll.GetAllByParam(input);

            var result = Mapper.Map<List<DataListIndexPbck7>>(dbData);

            var viewModel = new Pbck7IndexViewModel();

            viewModel.Detail = result;

            return PartialView("_Pbck7TableIndex", viewModel);
        }



        #region PBCK3

        public ActionResult ListPbck3Index()
        {
            var data = InitPbck3ViewModel(new Pbck3IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck3Type = Enums.Pbck7Type.Pbck3List,

                Detail =
                    Mapper.Map<List<DataListIndexPbck3>>(_pbck7AndPbck7And3Bll.GetAllByParam(new Pbck7AndPbck3Input()))
            });

            return View("ListPbck3Index", data);
        }

        private Pbck3IndexViewModel InitPbck3ViewModel(Pbck3IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return (model);
        }

        [HttpPost]
        public PartialViewResult FilterPbck3Index(Pbck3IndexViewModel model)
        {
            var input = Mapper.Map<Pbck7AndPbck3Input>(model);
            input.Pbck7AndPvck3Type = Enums.Pbck7Type.Pbck3List;
            if (input.Pbck3Date != null)
            {
                input.Pbck3Date = Convert.ToDateTime(input.Pbck3Date).ToString();
            }


            var dbData = _pbck7AndPbck7And3Bll.GetAllByParam(input);
            var result = Mapper.Map<List<DataListIndexPbck3>>(dbData);

            var viewModel = new Pbck3IndexViewModel();

            viewModel.Detail = result;

            return PartialView("_Pbck3TableIndex", viewModel);

        }

        #endregion

        #region Json

        [HttpPost]
        public JsonResult PoaAndPlantListPartialPbck7(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() {PoaList = listPoa, PlantList = listPlant};

            return Json(model);
        }

        [HttpPost]
        public JsonResult PoaAndPlantListPartialPbck3(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() {PoaList = listPoa, PlantList = listPlant};

            return Json(model);
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            var model = new Pbck7Pbck3CreateViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View("Create", InitialModel(model));
        }

        #endregion
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pbck7Pbck3CreateViewModel model)
        {
            var modelDto = Mapper.Map<Pbck7AndPbck3Dto>(model);
            return RedirectToAction("Index");
        }

        private Pbck7Pbck3CreateViewModel InitialModel(Pbck7Pbck3CreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.NppbkIdList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            // model.DocumentTypeList = 
            return (model);
        }


        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile, string plantId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<Pbck7ItemUpload>();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var item = new Pbck7ItemUpload();
                    item.FaCode = datarow[0];
                    item.Pbck7Qty = Convert.ToDecimal(datarow[1]);
                    item.Back1Qty = Convert.ToDecimal(datarow[2]);
                    item.FiscalYear = Convert.ToInt32(datarow[3]);
                    item.ExciseValue = Convert.ToDecimal(datarow[4]);
                    try
                    {
                        var existingBrand = _brandRegistration.GetByIdIncludeChild(plantId, item.FaCode);
                        if (existingBrand != null)
                        {
                            item.Brand = existingBrand.BRAND_CE;
                            item.SeriesValue =  existingBrand.ZAIDM_EX_SERIES.SERIES_VALUE;
                            item.ProdTypeAlias = existingBrand.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS;
                            item.Content = Convert.ToInt32(existingBrand.BRAND_CONTENT);
                            item.Hje = existingBrand.HJE_IDR;
                            item.Tariff = existingBrand.TARIFF;
                        }
                    }
                    catch (Exception)
                    {


                    }
                    finally
                    {
                        model.Add(item);
                    }

                   
                }
            }
            return Json(model);
        }
    }

}