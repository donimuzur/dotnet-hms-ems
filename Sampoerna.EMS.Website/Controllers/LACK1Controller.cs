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
using Sampoerna.EMS.Core.Exceptions;
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
        private IPlantBLL _plantBll;

        public LACK1Controller(IPageBLL pageBll, IPOABLL poabll, ICompanyBLL companyBll,
            IZaidmExGoodTypeBLL goodTypeBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK1BLL lack1Bll, IMonthBLL monthBll,
            IUnitOfMeasurementBLL uomBll, IPBCK1BLL pbck1Bll, IPlantBLL plantBll)
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
            _plantBll = plantBll;
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
                Details = Mapper.Map<List<Lack1NppbkcData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput() { Lack1Level = Enums.Lack1Level.Nppbkc, IsOpenDocumentOnly = true }))

            });

            return View("Index", data);
        }

        /// <summary>
        /// for LACK-1 data by NPPBKC Level
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Lack1IndexViewModel InitLack1ViewModel(Lack1IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.CreatorList = GlobalFunctions.GetCreatorList();
            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(Lack1Input model)
        {

            var input = Mapper.Map<Lack1GetByParamInput>(model);
            input.Lack1Level = Enums.Lack1Level.Nppbkc;
            input.IsOpenDocumentOnly = true;

            var dbData = _lack1Bll.GetAllByParam(input);

            var result = Mapper.Map<List<Lack1NppbkcData>>(dbData);

            var viewModel = new Lack1IndexViewModel { Details = result };

            return PartialView("_Lack1Table", viewModel);
        }

        #endregion

        #region Index List By Plant

        public ActionResult ListByPlant()
        {
            var data = InitLack1LiistByPlant(new Lack1IndexPlantViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<Lack1PlantData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput()
                {
                    Lack1Level = Enums.Lack1Level.Plant,
                    IsOpenDocumentOnly = true
                }))
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
            inputPlant.Lack1Level = Enums.Lack1Level.Plant;
            inputPlant.IsOpenDocumentOnly = true;

            var dbDataPlant = _lack1Bll.GetAllByParam(inputPlant);

            var resultPlant = Mapper.Map<List<Lack1PlantData>>(dbDataPlant);

            var viewModel = new Lack1IndexPlantViewModel { Details = resultPlant };

            return PartialView("_Lack1ListByPlantTable", viewModel);

        }
        #endregion

        #region json

        [HttpPost]
        public JsonResult PoaAndPlantListPartial(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Lack1IndexViewModel() { PoaList = listPoa, PlantIdList = listPlant };
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetNppbkcListByCompanyCode(string companyCode)
        {
            var data = _pbck1Bll.GetNppbkByCompanyCode(companyCode);
            return Json(data);
        }

        /// <summary>
        /// user for get received plant id
        /// </summary>
        /// <param name="nppbkcId"></param>
        /// <returns></returns>
        public JsonResult GetPlantListByNppbkcId(string nppbkcId)
        {
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Lack1CreateViewModel() { ReceivePlantList = listPlant };
            return Json(model);
        }

        public JsonResult GetExcisableGoodsTypeByNppbkcId(string nppbkcId)
        {
            var data = GetExciseGoodsTypeList(nppbkcId);
            var model = new Lack1CreateViewModel() { ExGoodTypeList = data };
            return Json(model);
        }

        public JsonResult GetSupplierListByParam(string nppbkcId, string excisableGoodsType)
        {
            var data = GetSupplierPlantListByParam(nppbkcId, excisableGoodsType);
            var model = new Lack1CreateViewModel() { SupplierList = data };
            return Json(model);
        }

        public JsonResult Generate(Lack1GenerateInputModel param)
        {
            //try
            //{
            //    var input = Mapper.Map<Lack1GenerateDataParamInput>(param);
            //    var generatedData = _lack1Bll.GenerateLack1DataByParam(input);
            //    var model = new Lack1CreateViewModel() { Lack1Generated = Mapper.Map<Lack1GeneratedItemModel>(generatedData) };
            //    return PartialView("_Lack1TableBalance", model);
            //}
            //catch (BLLException ex)
            //{
            //    AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            //    return null;
            //}

            var input = Mapper.Map<Lack1GenerateDataParamInput>(param);
            var outGeneratedData = _lack1Bll.GenerateLack1DataByParam(input);
            return Json(outGeneratedData);

        }

        #endregion

        #region ----- create -----

        public ActionResult Create(Enums.Lack1Level? lack1Level)
        {

            if (!lack1Level.HasValue)
            {
                return HttpNotFound();
            }

            var model = new Lack1CreateViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Level = lack1Level.Value,
                MenuPlantAddClassCss = lack1Level.Value == Enums.Lack1Level.Plant ? "active" : "",
                MenuNppbkcAddClassCss = lack1Level.Value == Enums.Lack1Level.Nppbkc ? "active" : ""
            };

            return CreateInitial(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lack1CreateViewModel model)
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

        public ActionResult CreateInitial(Lack1CreateViewModel model)
        {
            return View("Create", InitialModel(model));
        }

        #endregion

        private SelectList GetNppbkcListOnPbck1ByCompanyCode(string companyCode)
        {
            var data = _pbck1Bll.GetNppbkByCompanyCode(companyCode);
            return new SelectList(data, "NPPBKC_ID", "NPPBKC_ID");
        }

        private SelectList GetExciseGoodsTypeList(string nppbkcId)
        {
            var data = _pbck1Bll.GetGoodsTypeByNppbkcId(nppbkcId);
            return new SelectList(data, "EXC_GOOD_TYP", "EXT_TYP_DESC");
        }

        private SelectList GetSupplierPlantListByParam(string nppbkcId, string excisableGoodsType)
        {
            var data = _pbck1Bll.GetSupplierPlantByParam(new Pbck1GetSupplierPlantByParamInput()
            {
                NppbkcId = nppbkcId,
                ExciseableGoodsTypeId = excisableGoodsType
            });
            return new SelectList(data, "WERKS", "DROPDOWNTEXTFIELD");
        }

        private Lack1CreateViewModel InitialModel(Lack1CreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.BukrList = GlobalFunctions.GetCompanyList(_companyBll);
            model.MontList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearsList = CreateYearList();
            model.NppbkcList = GetNppbkcListOnPbck1ByCompanyCode(model.Bukrs);
            model.ReceivePlantList = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.NppbkcId);
            model.ExGoodTypeList = GetExciseGoodsTypeList(model.NppbkcId);
            model.SupplierList = GetSupplierPlantListByParam(model.NppbkcId, model.ExGoodsType);
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

        #region Completed Document

        public ActionResult ListCompletedDocument()
        {
            var data = InitListCompletedDocument(new Lack1IndexCompletedDocumentViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<Lack1CompletedDocumentData>>(_lack1Bll.GetCompletedDocumentByParam(new Lack1GetByParamInput()))

            });

            return View("ListCompletedDocument", data);
        }

        private Lack1IndexCompletedDocumentViewModel InitListCompletedDocument(Lack1IndexCompletedDocumentViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListCompletedDocument(Lack1Input model)
        {
            var inputPlant = Mapper.Map<Lack1GetByParamInput>(model);
            inputPlant.IsOpenDocumentOnly = false;

            var dbDataPlant = _lack1Bll.GetCompletedDocumentByParam(inputPlant);

            var resultPlant = Mapper.Map<List<Lack1CompletedDocumentData>>(dbDataPlant);

            var viewModel = new Lack1IndexCompletedDocumentViewModel { Details = resultPlant };

            return PartialView("_Lack1CompletedDocumentTable", viewModel);

        }

        #endregion

    }
}