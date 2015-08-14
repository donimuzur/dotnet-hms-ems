using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
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

        public LACK1Controller(IPageBLL pageBll, ILACK1BLL lack1Bll, IMonthBLL monthBll, IUnitOfMeasurementBLL uomBll)
            : base(pageBll, Enums.MenuList.LACK1)
        {
            _lack1Bll = lack1Bll;
            _monthBll = monthBll;
            _uomBll = uomBll;
            _mainMenu = Enums.MenuList.LACK1;
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
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
            model.PoaList = GlobalFunctions.GetPoaAll();
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
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll();
            model.PoaList = GlobalFunctions.GetPoaAll();
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
            model.MontList = GlobalFunctions.GetMonthList();
            model.YearsList = CreateYearList();
            model.NppbkcList = GlobalFunctions.GetNppbkcAll();
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.SupplierList = GlobalFunctions.GetSupplierPlantList();
            model.ExGoodTypeList = GlobalFunctions.GetGoodTypeList();
            model.WasteUomList = GlobalFunctions.GetUomList();
            model.ReturnUomList = GlobalFunctions.GetUomList();

            return (model);

        }

        #region ----- create -----

        public ActionResult Create()
        {
            var model = new Lack1CreateNppbkcViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

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