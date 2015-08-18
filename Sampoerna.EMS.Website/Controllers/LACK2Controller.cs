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

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK2Controller : BaseController
    {

        private ILACK2BLL _lack2Bll;
        private IPlantBLL _plantBll;
        private ICompanyBLL _companyBll;
        private IZaidmExGoodTypeBLL _exGroupBll;

        private Enums.MenuList _mainMenu;

        public LACK2Controller(IPageBLL pageBll, ILACK2BLL lack2Bll,
            IPlantBLL plantBll, ICompanyBLL companyBll, IZaidmExGoodTypeBLL exGroupBll)
            : base(pageBll, Enums.MenuList.LACK2)
        {
            _lack2Bll = lack2Bll;
            _plantBll = plantBll;
            _companyBll = companyBll;
            _exGroupBll = exGroupBll;
            _mainMenu = Enums.MenuList.LACK2;
        }


        #region CRUD Actions

        // GET: LACK2
        public ActionResult Index()
        {
            var model = new Lack2IndexViewModel();
            model = InitViewModel(model);

            model.MainMenu = Enums.MenuList.LACK2;
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
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
            model.PoaList = GlobalFunctions.GetPoaAll();
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

            model.NPPBKCDDL = GlobalFunctions.GetNppbkcAll();
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList();
            model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeGroupList();
            model.SendingPlantDDL = GlobalFunctions.GetPlantAll();

            var GovStatuses = from Enums.DocumentStatusGov s in Enum.GetValues(typeof(Enums.DocumentStatusGov))
                              select new { ID = (int)s, Name = s.ToString() };

            var Statuses = from Enums.DocumentStatus s in Enum.GetValues(typeof(Enums.DocumentStatus))
                           select new { ID = (int)s, Name = s.ToString() };

            model.GovStatusDDL = new SelectList(GovStatuses, "ID", "Name");
            model.StatusDDL = new SelectList(Statuses, "ID", "Name");

            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(LACK2CreateViewModel model)
        {

            Lack2Dto item = new Lack2Dto();

            item = AutoMapper.Mapper.Map<Lack2Dto>(model.Lack2Model);
    
            var plant = _plantBll.GetAll().Where(p => p.WERKS == model.Lack2Model.LevelPlantId).FirstOrDefault();
            var company = _companyBll.GetById(model.Lack2Model.Burks);
            var goods = _exGroupBll.GetById(model.Lack2Model.ExGoodTyp);

            item.GovStatus = Enums.DocumentStatus.WaitingGovApproval;
            item.Status = Enums.DocumentStatus.WaitingForApproval;

            item.ExTypDesc = goods.EXT_TYP_DESC;

            item.Butxt = company.BUTXT;
            item.LevelPlantName = plant.NAME1;
            item.LevelPlantCity = plant.ORT01;
            item.PeriodMonth = model.Lack2Model.LACK2Period.Month;
            item.PeriodYear = model.Lack2Model.LACK2Period.Year;
            item.CreatedBy = CurrentUser.USER_ID;
            item.CreatedDate = DateTime.Now;

            _lack2Bll.Insert(item);

            return RedirectToAction("Index");
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

            var dbData = _lack2Bll.GetAll(input);

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

        #endregion


    }
            
}