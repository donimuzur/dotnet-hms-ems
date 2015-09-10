using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.CK4C;

namespace Sampoerna.EMS.Website.Controllers
{
    public class CK4CController : BaseController
    {
        private ICK4CBLL _ck4CBll;
        private IMonthBLL _monthBll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poabll;
        private ICompanyBLL _companyBll;
        private IPlantBLL _plantBll;
        private IT001KBLL _t001KBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IProductionBLL _productionBll;
        private IDocumentSequenceNumberBLL _documentSequenceNumberBll;

        public CK4CController(IPageBLL pageBll, IPOABLL poabll, ICK4CBLL ck4Cbll, IPlantBLL plantbll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll,
            IBrandRegistrationBLL brandRegistrationBll, ICompanyBLL companyBll, IT001KBLL t001Kbll, IZaidmExNPPBKCBLL nppbkcbll, IProductionBLL productionBll,
            IDocumentSequenceNumberBLL documentSequenceNumberBll)
            : base(pageBll, Enums.MenuList.CK4C)
        {
            _ck4CBll = ck4Cbll;
            _plantBll = plantbll;
            _monthBll = monthBll;
            _plantBll = plantbll;
            _poabll = poabll;
            _companyBll = companyBll;
            _mainMenu = Enums.MenuList.CK4C;
            _t001KBll = t001Kbll;
            _uomBll = uomBll;
            _brandRegistrationBll = brandRegistrationBll;
            _nppbkcbll = nppbkcbll;
            _productionBll = productionBll;
            _documentSequenceNumberBll = documentSequenceNumberBll;
        }


        #region Index Daily Production
        //
        // GET: /CK4C/
        public ActionResult Index()
        {
            var data = InitCk4ViewModel(new Ck4CIndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.DailyProduction,
                Detail = Mapper.Map<List<DataIndecCk4C>>(_ck4CBll.GetAllByParam(new Ck4CGetByParamInput()))

            });

            return View("Index", data);
        }

        private Ck4CIndexViewModel InitCk4ViewModel(Ck4CIndexViewModel model)
        {
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlanIdList = GlobalFunctions.GetPlantAll();
            return model;
        }

        [HttpPost]
        public PartialViewResult FilterCk4CDailyProductionIndex(Ck4CIndexViewModel model)
        {
            var input = Mapper.Map<Ck4CGetByParamInput>(model);
            input.Ck4CType = Enums.CK4CType.DailyProduction;
            if (input.DateProduction != null)
            {
                input.DateProduction = Convert.ToDateTime(input.DateProduction).ToString();
            }

            var dbData = _ck4CBll.GetAllByParam(input);
            var result = Mapper.Map<List<DataIndecCk4C>>(dbData);
            var viewModel = new Ck4CIndexViewModel();
            viewModel.Detail = result;
            return PartialView("_Ck4CTableIndex", viewModel);
        }

        #endregion

        #region Index Waste Production

        public ActionResult WasteProductionIndex()
        {
            var data =
            InitIndexWasteProductionViewModel(new Ck4CIndexWasteProductionViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.WasteProduction,
                Detail = Mapper.Map<List<DataWasteProduction>>(_ck4CBll.GetAllByParam(new Ck4CGetByParamInput()))
            });

            return View("WasteProductionIndex", data);
        }

        private Ck4CIndexWasteProductionViewModel InitIndexWasteProductionViewModel(
            Ck4CIndexWasteProductionViewModel model)
        {
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlanIdList = GlobalFunctions.GetPlantAll();
            return model;
        }

        [HttpPost]
        public PartialViewResult FilterWasteProductionIndex(Ck4CIndexWasteProductionViewModel model)
        {
            var input = Mapper.Map<Ck4CGetByParamInput>(model);
            input.Ck4CType = Enums.CK4CType.WasteProduction;

            var dbData = _ck4CBll.GetAllByParam(input);
            var result = Mapper.Map<List<DataWasteProduction>>(dbData);
            var viewModel = new Ck4CIndexWasteProductionViewModel();
            viewModel.Detail = result;

            return PartialView("_CK4CTableWasteProduction", viewModel);
        }
        #endregion

        #region Index Document List

        public ActionResult DocumentList()
        {
            var data =
            InitIndexDocumentListViewModel(new Ck4CIndexDocumentListViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Ck4CType = Enums.CK4CType.Ck4CDocument,
                Detail = Mapper.Map<List<DataDocumentList>>(_ck4CBll.GetAllByParam(new Ck4CGetByParamInput()))
            });

            return View("DocumentList", data);
        }

        private Ck4CIndexDocumentListViewModel InitIndexDocumentListViewModel(
            Ck4CIndexDocumentListViewModel model)
        {
            var listCk4cData = _ck4CBll.GetAll();
            model.DocumentNumberList = new SelectList(listCk4cData, "NUMBER", "NUMBER");
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            return model;
        }

        [HttpPost]
        public PartialViewResult FilterDocumentListIndex(Ck4CIndexDocumentListViewModel model)
        {
            var input = Mapper.Map<Ck4CGetByParamInput>(model);
            input.Ck4CType = Enums.CK4CType.Ck4CDocument;

            var dbData = _ck4CBll.GetAllByParam(input);
            var result = Mapper.Map<List<DataDocumentList>>(dbData);
            var viewModel = new Ck4CIndexDocumentListViewModel();
            viewModel.Detail = result;

            return PartialView("_CK4CTableDocumentList", viewModel);
        }

        #endregion

        #region Json
        [HttpPost]
        public JsonResult CompanyListPartialProduction(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var model = new Ck4CIndexViewModel() { PlanIdList = listPlant };

            return Json(model);

        }

        [HttpPost]
        public JsonResult CompanyListPartialCk4CWaste(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var model = new Ck4CIndexWasteProductionViewModel() { PlanIdList = listPlant };

            return Json(model);

        }

        [HttpPost]
        public JsonResult CompanyListPartialCk4CDocument(string companyId)
        {
            var listPlant = GlobalFunctions.GetPlantByCompanyId(companyId);

            var model = new Ck4CIndexDocumentListViewModel() { PlanList = listPlant };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetNppbkcByCompanyId(string companyId)
        {
            return Json(_nppbkcbll.GetNppbkcsByCompany(companyId));
        }

        [HttpPost]
        public JsonResult GetAllNppbkc()
        {
            var listNppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            var model = new Ck4CIndexDocumentListViewModel() { NppbkcIdList = listNppbkc };

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetFaCodeDescription(string faCode)
        {
            var fa = _brandRegistrationBll.GetByFaCode(faCode);
            return Json(fa.BRAND_CE);
        }

        [HttpPost]
        public JsonResult GetProductionData(string comp, string plant, string nppbkc)
        {
            var data = _productionBll.GetByCompPlant(comp, plant, nppbkc).Select(d => Mapper.Map<ProductionDto>(d)).ToList();
            return Json(data);
        }

        #endregion


        #region create daily Production

        public ActionResult Ck4CCreateDailyProduction()
        {
            var model = new Ck4cCreateViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,

            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(Ck4cCreateViewModel model)
        {
            return View("Ck4CCreateDailyProduction", InitialModel(model));
        }

        private Ck4cCreateViewModel InitialModel(Ck4cCreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.FinishGoodList = GlobalFunctions.GetBrandList();
            model.UomList = GlobalFunctions.GetUomList(_uomBll);

            return (model);

        }

        #endregion

        #region create Waste Production

        public ActionResult Ck4CCreateWasteProduction()
        {
            var model = new Ck4CCreateWasteProductionViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(Ck4CCreateWasteProductionViewModel model)
        {
            return View("Ck4CCreateWasteProduction", InitialModel(model));
        }

        private Ck4CCreateWasteProductionViewModel InitialModel(Ck4CCreateWasteProductionViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.FinishGoodsList = GlobalFunctions.GetBrandList();
            model.UomList = GlobalFunctions.GetUomList(_uomBll);

            return (model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ck4CCreateWasteProduction(Ck4cCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Invalid input, please check the input.", Enums.MessageInfoType.Error);
                    return CreateInitial(model);
                }

                var dataToSave = Mapper.Map<Ck4CDto>(model);
                dataToSave.CreatedBy = CurrentUser.USER_ID;

            }
            catch (Exception exception)
            {

                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }

            return CreateInitial(model);
        }

        #endregion

        #region create Document List
        public ActionResult Ck4CCreateDocumentList()
        {
            var model = new Ck4CIndexDocumentListViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
            };

            return CreateInitial(model);
        }

        public ActionResult CreateInitial(Ck4CIndexDocumentListViewModel model)
        {
            return View("Ck4CCreateDocumentList", InitialModel(model));
        }

        private Ck4CIndexDocumentListViewModel InitialModel(Ck4CIndexDocumentListViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.CompanyNameList = GlobalFunctions.GetCompanyList(_companyBll);
            model.PeriodList = Ck4cPeriodList();
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = Ck4cYearList();
            model.PlanList = GlobalFunctions.GetPlantAll();
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            return (model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ck4CCreateDocumentList(Ck4CIndexDocumentListViewModel model)
        {
            Ck4CDto item = new Ck4CDto();

            item = AutoMapper.Mapper.Map<Ck4CDto>(model.Details);

            var plant = _plantBll.GetT001WById(model.Details.PlantId);
            var company = _companyBll.GetById(model.Details.CompanyId);

            item.PlantName = plant.NAME1;
            item.CompanyName = company.BUTXT;
            item.CreatedBy = CurrentUser.USER_ID;
            item.CreatedDate = DateTime.Now;
            var inputDoc = new GenerateDocNumberInput();
            inputDoc.Month = item.ReportedMonth;
            inputDoc.Year = item.ReportedYears;
            inputDoc.NppbkcId = item.NppbkcId;
            item.Number = _documentSequenceNumberBll.GenerateNumber(inputDoc);
            item.Status = Enums.DocumentStatus.Draft;

            _ck4CBll.Save(item);
            AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
            return RedirectToAction("DocumentList");
        }
        #endregion

        private SelectList Ck4cPeriodList()
        {
            var period = new List<SelectItemModel>();
            var currentPeriod = 1;
            period.Add(new SelectItemModel() { ValueField = currentPeriod, TextField = currentPeriod.ToString() });
            period.Add(new SelectItemModel() { ValueField = currentPeriod + 1, TextField = (currentPeriod + 1).ToString() });
            return new SelectList(period, "ValueField", "TextField");
        }

        private SelectList Ck4cYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }
    }
}