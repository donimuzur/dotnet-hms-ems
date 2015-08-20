using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.LACK1;
using Sampoerna.EMS.Website.Models;

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
        private ICompanyBLL _companyBll;
        private IPBCK1BLL _pbck1Bll;

        public LACK1Controller(IPageBLL pageBll, IPOABLL poabll, ICompanyBLL companyBll, 
            IZaidmExGoodTypeBLL goodTypeBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK1BLL lack1Bll, IMonthBLL monthBll, 
            IUnitOfMeasurementBLL uomBll, IPBCK1BLL pbck1Bll)
            : base(pageBll, Enums.MenuList.LACK1)
        {
            _lack1Bll = lack1Bll;
            _monthBll = monthBll;
            _uomBll = uomBll;
            _mainMenu = Enums.MenuList.LACK1;
            _poabll = poabll;
            _nppbkcbll = nppbkcbll;
            _goodTypeBll = goodTypeBll;
            _companyBll = companyBll;
            _pbck1Bll = pbck1Bll;
        }


        #region Index

        //
        // GET: /LACK1/
        public ActionResult Index()
        {
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

            return Mapper.Map<List<NppbkcData>>(dbData);

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

        [HttpPost]
        public PartialViewResult FilterListByPlant(Lack1Input model)
        {
            var inputPlant = Mapper.Map<Lack1GetByParamInput>(model);

            var dbDataPlant = _lack1Bll.GetAllByParam(inputPlant);

            var resultPlant = Mapper.Map<List<PlantData>>(dbDataPlant);

            var viewModel = new Lack1IndexPlantViewModel {Details = resultPlant};

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

        [HttpPost]
        public JsonResult GetNppbkcListByCompanyCode(string companyCode)
        {
            var data = _pbck1Bll.GetNppbkByCompanyCode(companyCode);
            return Json(data);
        }

        public JsonResult GetPlantListByNppbkcId(string nppbkcId)
        {
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(nppbkcId);
            var model = new Lack1CreateNppbkcViewModel() { PlantList = listPlant };
            return Json(model);
        }

        #endregion

        #region ----- create -----

        public ActionResult Create()
        {
            var model = new Lack1CreateNppbkcViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo
            };

            return CreateInitial(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lack1CreateNppbkcViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    AddMessageInfo("Model Error", Enums.MessageInfoType.Error);
                    return CreateInitial(model);
                }

                //process save
                var dataToSave = Mapper.Map<Lack1Dto>(model);
                dataToSave.CreateBy = CurrentUser.USER_ID;

                var input = new Lack1SaveInput()
                {
                    Lack1 = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Created
                };

                //only add this information from gov approval,
                //when save create/edit 
                input.Lack1.DecreeDate = null;

                var saveResult = _lack1Bll.Save(input);

                if (saveResult.Success)
                {
                    return RedirectToAction("Edit", new { id = saveResult.Id });
                }

            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception exception)
            {
                AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
            }

            return CreateInitial(model);

        }

        public ActionResult CreateInitial(Lack1CreateNppbkcViewModel model)
        {
            return View("Create", InitialModel(model));
        }

        #endregion

        private SelectList GetNppbkcListOnPbck1ByCompanyCode(string companyCode)
        {
            var data = _pbck1Bll.GetNppbkByCompanyCode(companyCode);
            return new SelectList(data, "NPPBKC_ID", "NPPBKC_ID");
        }

        private Lack1CreateNppbkcViewModel InitialModel(Lack1CreateNppbkcViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.BukrList = GlobalFunctions.GetCompanyList(_companyBll);
            model.MontList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearsList = CreateYearList();
            model.NppbkcList = GetNppbkcListOnPbck1ByCompanyCode(model.Bukrs);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.SupplierList = GlobalFunctions.GetSupplierPlantList();
            model.ExGoodTypeList = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.WasteUomList = GlobalFunctions.GetUomList(_uomBll);
            model.ReturnUomList = GlobalFunctions.GetUomList(_uomBll);

            return (model);

        }

        private SelectList CreateYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            for (int i = 0; i < 2; i++)
            {
                years.Add(new SelectItemModel() { ValueField = currentYear - i, TextField = (currentYear - i).ToString() });
            }
            return new SelectList(years, "ValueField", "TextField");
        }

    }
}