using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.LACK1;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK1Controller : BaseController
    {
        private ILACK1BLL _lack1Bll;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poabll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        public LACK1Controller(IPageBLL pageBll, IPOABLL poabll, IZaidmExGoodTypeBLL goodTypeBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK1BLL lack1Bll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll)
            : base(pageBll, Enums.MenuList.LACK1)
        {
            _lack1Bll = lack1Bll;
            _monthBll = monthBll;
            _uomBll = uomBll;
            _mainMenu = Enums.MenuList.LACK1;
            _poabll = poabll;
            _nppbkcbll = nppbkcbll;
            _goodTypeBll = goodTypeBll;
        }


        #region Index

        //
        // GET: /LACK1/
        public ActionResult Index()
        {
            //var x = _lack1Bll.GetAllByParam(new Lack1GetByParamInput());

            var data = InitLack1ViewModel(new Lack1IndexViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Type = Enums.LACK1Type.ListByNppbkc,
                
                Details = Mapper.Map<List<NppbkcData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput()))

            });

            return View("Index", data);
        }

        private Lack1IndexViewModel InitLack1ViewModel(Lack1IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();
            

            return model;
        }

        private List<NppbkcData> GetListByNppbkc(Lack1IndexViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var litsByNppbkc = _lack1Bll.GetAllByParam(new Lack1GetByParamInput());
                return Mapper.Map<List<NppbkcData>>(litsByNppbkc);
            }
            //get by param
            var input = Mapper.Map<Lack1GetByParamInput>(filter);
            var dbData = _lack1Bll.GetAllByParam(input);
            
            return  Mapper.Map<List<NppbkcData>>(dbData);

        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(Lack1Input model)
        {


            //if (!string.IsNullOrEmpty(model.ReportedOn))
            //{
            //    var data = Convert.ToDateTime(model.ReportedOn);
            //    model.PeriodMonth = data.Month;
            //    model.PeriodYear = data.Year;    
            //}
            


            var input = Mapper.Map<Lack1GetByParamInput>(model);

            var dbData = _lack1Bll.GetAllByParam(input);

            var result = Mapper.Map<List<NppbkcData>>(dbData);

            var viewModel = new Lack1IndexViewModel();
            viewModel.Details = result;

            return PartialView("_Lack1Table", viewModel);


            //model.Details = GetListByNppbkc(model);
            //return PartialView("_Lack1Table", model);
            return null;
        }
        
        #endregion

        #region Index List By Plant

        public ActionResult ListByPlant()
        {
            var data = InitLack1LiistByPlant(new Lack1IndexPlantViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Type = Enums.LACK1Type.ListByNppbkc,

                Details = Mapper.Map<List<PlantData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput()))

            });

            return View("ListByPlant", data);
        }

        private Lack1IndexPlantViewModel InitLack1LiistByPlant(Lack1IndexPlantViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();
            
            return model;
        }

        private List<PlantData> GetListByPlant(Lack1IndexPlantViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var litsByNppbkc = _lack1Bll.GetAllByParam(new Lack1GetByParamInput());
                return Mapper.Map<List<PlantData>>(litsByNppbkc);
            }
            //get by param
            var input = Mapper.Map<Lack1GetByParamInput>(filter);
            var dbData = _lack1Bll.GetAllByParam(input);

            return Mapper.Map<List<PlantData>>(dbData);

        }

        [HttpPost]
        public PartialViewResult FilterListByPlant(Lack1Input model)
        {

            //if (!string.IsNullOrEmpty(model.ReportedOn))
            //{
            //    var data = Convert.ToDateTime(model.ReportedOn);
            //    model.PeriodMonth = data.Month;
            //    model.PeriodYear = data.Year;
            //}


            var inputPlant = Mapper.Map<Lack1GetByParamInput>(model);

            var dbDataPlant = _lack1Bll.GetAllByParam(inputPlant);

            var resultPlant = Mapper.Map<List<PlantData>>(dbDataPlant);

            var viewModel = new Lack1IndexPlantViewModel();
            viewModel.Details = resultPlant;

            return PartialView("_Lack1ListByPlantTable", viewModel);

        }
        #endregion

        #region json

        [HttpPost]
        public JsonResult PoaAndPlantListPartial(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(nppbkcId);
            var model = new Lack1IndexViewModel() { PoaList = listPoa, PlantIdList = listPlant };
            return Json(model);
        }

       #endregion

        private Lack1CreateNppbkcViewModel InitialModel(Lack1CreateNppbkcViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.BukrList = GlobalFunctions.GetCompanyList();
            model.MontList = GlobalFunctions.GetMonthList(_monthBll);
            //model.YearsList = GlobalFunctions.GetYears();
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.SupplierList = GlobalFunctions.GetSupplierPlantList();
            model.ExGoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.WasteUomList = GlobalFunctions.GetUomList();
            model.ReturnUomList = GlobalFunctions.GetUomList();
            //liststatus.

            return (model);

        }


        public ActionResult Create()
        {
            var model = new Lack1CreateNppbkcViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View("Create","");
        }

    }
}