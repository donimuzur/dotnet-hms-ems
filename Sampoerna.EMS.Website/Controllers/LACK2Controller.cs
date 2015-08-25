using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.LACK2;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK2Controller : BaseController
    {

        private ILACK2BLL _lack2Bll;
        private IPlantBLL _plantBll;
        private ICompanyBLL _companyBll;
        private IZaidmExGoodTypeBLL _exGroupBll;

        private Enums.MenuList _mainMenu;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IPOABLL _poabll;
        private IMonthBLL _monthBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IDocumentSequenceNumberBLL _documentSequenceNumberBll;
        public LACK2Controller(IPageBLL pageBll, IPOABLL poabll, IZaidmExGoodTypeBLL goodTypeBll, IMonthBLL monthBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK2BLL lack2Bll,
            IPlantBLL plantBll, ICompanyBLL companyBll, IDocumentSequenceNumberBLL documentSequenceNumberBll, IZaidmExGoodTypeBLL exGroupBll)
            : base(pageBll, Enums.MenuList.LACK2)
        {
            _lack2Bll = lack2Bll;
            _plantBll = plantBll;
            _companyBll = companyBll;
            _exGroupBll = exGroupBll;
            _mainMenu = Enums.MenuList.LACK2;
            _nppbkcbll = nppbkcbll;
            _poabll = poabll;
            _monthBll = monthBll;
            _goodTypeBll = goodTypeBll;
            _documentSequenceNumberBll = documentSequenceNumberBll;
        }


        #region List by NPPBKC

        // GET: LACK2
        public ActionResult Index()
        {
            var model = new Lack2IndexViewModel();
            model = InitViewModel(model);

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var dbData = _lack2Bll.GetAll(new Lack2GetByParamInput());
            model.Details = dbData.Select(d => Mapper.Map<LACK2NppbkcData>(d)).ToList();

            return View("Index", model);
        }

        /// <summary>
        /// Fills the select lists for the IndexViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Lack2IndexViewModel</returns>
        private Lack2IndexViewModel InitViewModel(Lack2IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        /// <summary>
        /// Create LACK2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            LACK2CreateViewModel model = new LACK2CreateViewModel();

            model.NPPBKCDDL = GlobalFunctions.GetAuthorizedNppbkc(CurrentUser.NppbckPlants);
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList(_companyBll);
            model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.SendingPlantDDL = GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, null);
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = GlobalFunctions.GetYearList();
            model.UsrRole = CurrentUser.UserRole;

            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(LACK2CreateViewModel model)
        {

            Lack2Dto item = new Lack2Dto();

            item = AutoMapper.Mapper.Map<Lack2Dto>(model.Lack2Model);

            var plant = _plantBll.GetT001ById(model.Lack2Model.LevelPlantId);
            var company = _companyBll.GetById(model.Lack2Model.Burks);
            var goods = _exGroupBll.GetById(model.Lack2Model.ExGoodTyp);

            item.ExTypDesc = goods.EXT_TYP_DESC;
            item.Butxt = company.BUTXT;
            item.LevelPlantName = plant.NAME1;
            item.LevelPlantCity = plant.ORT01;
            item.LevelPlantId = plant.WERKS;
            item.PeriodMonth = model.Lack2Model.PeriodMonth;
            item.PeriodYear = model.Lack2Model.PeriodYear;
            item.CreatedBy = CurrentUser.USER_ID;
            item.CreatedDate = DateTime.Now;
             var inputDoc = new GenerateDocNumberInput();
            inputDoc.Month = item.PeriodMonth;
            inputDoc.Year = item.PeriodYear;
            inputDoc.NppbkcId = item.NppbkcId;
            item.Lack2Number = _documentSequenceNumberBll.GenerateNumber(inputDoc);
           
            if (CurrentUser.UserRole == Enums.UserRole.User || CurrentUser.UserRole == Enums.UserRole.POA)
            {
                item.Status = Enums.DocumentStatus.WaitingForApproval;
            }

            _lack2Bll.Insert(item);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edits the LACK2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            LACK2CreateViewModel model = new LACK2CreateViewModel();

            model.Lack2Model = AutoMapper.Mapper.Map<LACK2Model>(_lack2Bll.GetById(id));

            model.NPPBKCDDL = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList(_companyBll);
            model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeGroupList();
            model.SendingPlantDDL = GlobalFunctions.GetPlantAll();

            model.UsrRole = CurrentUser.UserRole;

            var govStatuses = from Enums.DocumentStatusGov ds in Enum.GetValues(typeof(Enums.DocumentStatusGov))
                              select new { ID = (int)ds, Name = ds.ToString() };

            model.GovStatusDDL = new SelectList(govStatuses, "ID", "Name");

            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;
            model.Lack2Model.LACK2Period = new DateTime(model.Lack2Model.PeriodYear, model.Lack2Model.PeriodMonth, 1);
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(LACK2CreateViewModel model)
        {

            Lack2Dto item = new Lack2Dto();

            item = AutoMapper.Mapper.Map<Lack2Dto>(model.Lack2Model);

            var plant = _plantBll.GetAll().Where(p => p.WERKS == model.Lack2Model.LevelPlantId).FirstOrDefault();
            var company = _companyBll.GetById(model.Lack2Model.Burks);
            var goods = _exGroupBll.GetById(model.Lack2Model.ExGoodTyp);

            item.ExTypDesc = goods.EXT_TYP_DESC;

            item.Butxt = company.BUTXT;
            item.LevelPlantName = plant.NAME1;
            item.LevelPlantCity = plant.ORT01;
            item.PeriodMonth = model.Lack2Model.LACK2Period.Month;
            item.PeriodYear = model.Lack2Model.LACK2Period.Year;

            if (CurrentUser.UserRole == Enums.UserRole.POA) // && if a file is uploaded needs to be added
            {
                item.Status = Enums.DocumentStatus.WaitingForApprovalManager;
            }

            if (CurrentUser.UserRole == Enums.UserRole.Manager)// && if a file is uploaded needs to be added
            {
                item.Status = Enums.DocumentStatus.Completed;
            }

            item.ModifiedBy = CurrentUser.USER_ID;
            item.ModifiedDate = DateTime.Now;

            item.ApprovedBy = CurrentUser.USER_ID;
            item.ApprovedDate = DateTime.Now;

            _lack2Bll.Insert(item);

            return RedirectToAction("Index");
        }

        #endregion

        #region List By Plant

        public ActionResult ListByPlant()
        {
            var data = InitLack2LiistByPlant(new Lack2IndexPlantViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,

                Details = Mapper.Map<List<LACK2PlantData>>(_lack2Bll.GetAll(new Lack2GetByParamInput()))

            });

            return View("ListByPlant", data);
        }

        private Lack2IndexPlantViewModel InitLack2LiistByPlant(Lack2IndexPlantViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        #endregion

        #region List Completed Documents

        public ActionResult ListCompletedDoc()
        {
            var model = new Lack2IndexViewModel();

            model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchInput.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.SearchInput.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.SearchInput.YearList = LackYearList();

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            // gets the completed documents by checking the status
            var dbData = _lack2Bll.GetAllCompleted();
            model.Details = dbData.Select(d => Mapper.Map<LACK2NppbkcData>(d)).ToList();

            return View("ListCompletedDoc", model);
        }

        // this is a cover up for the years we will need a new table or way to get the years for the dropdowns
        private SelectList LackYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }



        #endregion

        #region PreviewActions

        public ActionResult PreviewDocument(LACK2CreateViewModel model)
        {
            return View();
        }

        #endregion


        #region SearchFilters

        private List<LACK2NppbkcData> GetListByNppbkc(Lack2IndexViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var litsByNppbkc = _lack2Bll.GetAll(new Lack2GetByParamInput());
                return Mapper.Map<List<LACK2NppbkcData>>(litsByNppbkc);
            }
            //get by param
            var input = Mapper.Map<Lack2GetByParamInput>(filter);
            var dbData = _lack2Bll.GetAll(input);

            return Mapper.Map<List<LACK2NppbkcData>>(dbData);

        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(Lack2Input model)
        {


            var input = Mapper.Map<Lack2GetByParamInput>(model);

            var dbData = _lack2Bll.GetAllCompletedByParam(input);

            var result = Mapper.Map<List<LACK2NppbkcData>>(dbData);

            var viewModel = new Lack2IndexViewModel();
            viewModel.Details = result;

            return PartialView("_Lack2Table", viewModel);

        }


        private List<LACK2PlantData> GetListByPlant(Lack2IndexPlantViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var litsByNppbkc = _lack2Bll.GetAll(new Lack2GetByParamInput());
                return Mapper.Map<List<LACK2PlantData>>(litsByNppbkc);
            }
            //get by param
            var input = Mapper.Map<Lack2GetByParamInput>(filter);
            var dbData = _lack2Bll.GetAll(input);

            return Mapper.Map<List<LACK2PlantData>>(dbData);

        }

        [HttpPost]
        public PartialViewResult FilterListByPlant(Lack2Input model)
        {

            var inputPlant = Mapper.Map<Lack2GetByParamInput>(model);

            var dbDataPlant = _lack2Bll.GetAll(inputPlant);

            var resultPlant = Mapper.Map<List<LACK2PlantData>>(dbDataPlant);

            var viewModel = new Lack2IndexPlantViewModel();
            viewModel.Details = resultPlant;

            return PartialView("_Lack2ListByPlantTable", viewModel);

        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(LACK2FilterViewModel SearchInput)
        {
            var input = Mapper.Map<Lack2GetByParamInput>(SearchInput);
            // to search trough the completed documents
            input.Status = Enums.DocumentStatus.Completed;

            var dbData = _lack2Bll.GetAllCompletedByParam(input);

            var result = Mapper.Map<List<LACK2NppbkcData>>(dbData);

            var viewModel = new Lack2IndexViewModel();
            viewModel.Details = result;

            return PartialView("_Lack2CompletedDoc", viewModel);
        }

        #endregion


        [HttpPost]
        public JsonResult GetPlantByNppbkcId(string nppbkcid)
        {
            var data = Json(GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, nppbkcid));
            return data;

        }
    }

}