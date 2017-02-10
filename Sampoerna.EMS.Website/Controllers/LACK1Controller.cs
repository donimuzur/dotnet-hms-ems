using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.ReportingData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models.LACK1;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using System.IO;
using Sampoerna.EMS.Utils;
using SpreadsheetLight;
using System.Configuration;
using Sampoerna.EMS.Website.Models.Dashboard;

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
        private IWorkflowBLL _workflowBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IPrintHistoryBLL _printHistoryBll;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;

        public LACK1Controller(IPageBLL pageBll, IPOABLL poabll, ICompanyBLL companyBll,
            IZaidmExGoodTypeBLL goodTypeBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK1BLL lack1Bll, IMonthBLL monthBll,
            IUnitOfMeasurementBLL uomBll, IPBCK1BLL pbck1Bll, IPlantBLL plantBll, IWorkflowHistoryBLL workflowHistoryBll, IWorkflowBLL workflowBll,
            IChangesHistoryBLL changesHistoryBll, IPrintHistoryBLL printHistoryBll, IUserPlantMapBLL userPlantBll, IPOAMapBLL poaMapBll)
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
            _userPlantBll = userPlantBll;
            _poaMapBll = poaMapBll;

            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
            _changesHistoryBll = changesHistoryBll;
            _printHistoryBll = printHistoryBll;

        }


        #region Index

        //
        // GET: /LACK1/
        public ActionResult Index()
        {
            var curUser = CurrentUser;
            var data = InitLack1ViewModel(new Lack1IndexViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Type = Enums.LACK1Type.ListByNppbkc,
                Details = Mapper.Map<List<Lack1NppbkcData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput()
                {
                    Lack1Level = Enums.Lack1Level.Nppbkc,
                    IsOpenDocumentOnly = true,
                    UserRole = curUser.UserRole,
                    UserId = curUser.USER_ID,
                    ListNppbkc = curUser.ListUserNppbkc,
                    ListUserPlant = curUser.ListUserPlants
                })),
                IsShowNewButton = (curUser.UserRole != Enums.UserRole.Manager && curUser.UserRole != Enums.UserRole.Viewer && curUser.UserRole != Enums.UserRole.Administrator ? true : false),
                //first code when manager exists
                //IsNotViewer = curUser.UserRole != Enums.UserRole.Viewer
                IsNotViewer = (curUser.UserRole != Enums.UserRole.Manager && curUser.UserRole != Enums.UserRole.Viewer && curUser.UserRole != Enums.UserRole.Administrator ? true : false)
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
            var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var nppbkcList = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                nppbkc = new SelectList(nppbkcList, "Value", "Text");
            }

            model.NppbkcIdList = nppbkc;
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.CreatorList = GlobalFunctions.GetCreatorList();
            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(Lack1Input model)
        {
            var currUser = CurrentUser;
            var input = Mapper.Map<Lack1GetByParamInput>(model);
            input.Lack1Level = Enums.Lack1Level.Nppbkc;
            input.IsOpenDocumentOnly = true;
            input.UserId = currUser.USER_ID;
            input.UserRole = currUser.UserRole;
            input.ListNppbkc = currUser.ListUserNppbkc;
            input.ListUserPlant = currUser.ListUserPlants;

            var dbData = _lack1Bll.GetAllByParam(input);

            var result = Mapper.Map<List<Lack1NppbkcData>>(dbData);

            var viewModel = new Lack1IndexViewModel { Details = result };
            //first code when manager exists
            //viewModel.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            viewModel.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer && CurrentUser.UserRole != Enums.UserRole.Administrator ? true : false);

            return PartialView("_Lack1Table", viewModel);
        }

        #endregion

        #region Index List By Plant

        public ActionResult ListByPlant()
        {
            var curUser = CurrentUser;
            var data = InitLack1LiistByPlant(new Lack1IndexPlantViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<Lack1PlantData>>(_lack1Bll.GetAllByParam(new Lack1GetByParamInput()
                {
                    Lack1Level = Enums.Lack1Level.Plant,
                    IsOpenDocumentOnly = true,
                    UserRole = curUser.UserRole,
                    UserId = curUser.USER_ID,
                    ListNppbkc = curUser.ListUserNppbkc,
                    ListUserPlant = curUser.ListUserPlants
                })),
                IsShowNewButton = (curUser.UserRole != Enums.UserRole.Manager && curUser.UserRole != Enums.UserRole.Viewer && curUser.UserRole != Enums.UserRole.Administrator ? true : false),
                //first code when manager exists
                //IsNotViewer = curUser.UserRole != Enums.UserRole.Viewer
                IsNotViewer = (curUser.UserRole != Enums.UserRole.Manager && curUser.UserRole != Enums.UserRole.Viewer && curUser.UserRole != Enums.UserRole.Administrator ? true : false)
            });

            return View("ListByPlant", data);
        }

        private Lack1IndexPlantViewModel InitLack1LiistByPlant(Lack1IndexPlantViewModel model)
        {
            var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var nppbkcList = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                nppbkc = new SelectList(nppbkcList, "Value", "Text");
            }

            model.NppbkcIdList = nppbkc;
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        [HttpPost]
        public PartialViewResult FilterListByPlant(Lack1Input model)
        {
            var curUser = CurrentUser;
            var inputPlant = Mapper.Map<Lack1GetByParamInput>(model);
            inputPlant.Lack1Level = Enums.Lack1Level.Plant;
            inputPlant.IsOpenDocumentOnly = true;
            inputPlant.UserId = curUser.USER_ID;
            inputPlant.UserRole = curUser.UserRole;
            inputPlant.ListNppbkc = curUser.ListUserNppbkc;
            inputPlant.ListUserPlant = curUser.ListUserPlants;

            var dbDataPlant = _lack1Bll.GetAllByParam(inputPlant);

            var resultPlant = Mapper.Map<List<Lack1PlantData>>(dbDataPlant);

            var viewModel = new Lack1IndexPlantViewModel { Details = resultPlant };
            //first code when manager exists
            //viewModel.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            viewModel.IsNotViewer = (curUser.UserRole != Enums.UserRole.Manager && curUser.UserRole != Enums.UserRole.Viewer && curUser.UserRole != Enums.UserRole.Administrator ? true : false);

            return PartialView("_Lack1ListByPlantTable", viewModel);

        }
        #endregion

        #region --------------- Json ------------------

        [HttpPost]
        public JsonResult PoaAndPlantListPartial(string nppbkcId)
        {
            var plant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var distinctPlant = plant.Where(x => CurrentUser.ListUserPlants.Contains(x.Value));
                plant = new SelectList(distinctPlant, "Value", "Text");
            }
            
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = plant;
            var model = new Lack1IndexViewModel() { PoaList = listPoa, PlantIdList = listPlant };
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetNppbkcListByCompanyCode(string companyCode)
        {
            var data = _pbck1Bll.GetNppbkByCompanyCode(companyCode);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                data = data.Where(x => CurrentUser.ListUserNppbkc.Contains(x.NPPBKC_ID)).ToList();
            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetSupplierPlantDetail(string werks)
        {
            var data = _plantBll.GetT001WById(werks);
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

            var filterPlant = listPlant;

            var newListPlant = new SelectList(filterPlant, "Value", "Text");

            if (CurrentUser.UserRole == Enums.UserRole.User)
            {
                var newFilterPlant = listPlant.Where(x => CurrentUser.ListUserPlants.Contains(x.Value));

                newListPlant = new SelectList(newFilterPlant, "Value", "Text");
            }

            var model = new Lack1CreateViewModel() { ReceivePlantList = newListPlant };

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
            var input = Mapper.Map<Lack1GenerateDataParamInput>(param);
            var outGeneratedData = _lack1Bll.GenerateLack1DataByParam(input);

            var result = Json(outGeneratedData);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        

        #endregion

        #region ----- create -----

        public ActionResult Create(Enums.Lack1Level? lack1Level)
        {

            if (!lack1Level.HasValue)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole == Enums.UserRole.Manager || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                return RedirectToAction(lack1Level.Value == Enums.Lack1Level.Nppbkc ? "Index" : "ListByPlant");
            }

            var model = new Lack1CreateViewModel
            {
                IsCreateNew = true,
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Lack1Level = lack1Level.Value,
                MenuPlantAddClassCss = lack1Level.Value == Enums.Lack1Level.Plant ? "active" : "",
                MenuNppbkcAddClassCss = lack1Level.Value == Enums.Lack1Level.Nppbkc ? "active" : "",
                IsShowNewButton = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                //first code when manager exists
                //IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
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
                    AddMessageInfo("Invalid input, please check the input.", Enums.MessageInfoType.Error);
                    return CreateInitial(model);
                }

                if (CurrentUser.UserRole == Enums.UserRole.Manager || CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allow", Enums.MessageInfoType.Error);
                    return RedirectToAction(model.Lack1Level == Enums.Lack1Level.Nppbkc ? "Index" : "ListByPlant");
                }

                var input = Mapper.Map<Lack1CreateParamInput>(model);
                input.IsCreateNew = true;
                input.UserId = CurrentUser.USER_ID;
                var saveOutput = _lack1Bll.Create(input);
                if (saveOutput.Success)
                {
                    AddMessageInfo("Save successfull", Enums.MessageInfoType.Info);
                    if (model.Lack1Level == Enums.Lack1Level.Nppbkc)
                    {
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("ListByPlant");
                }
                AddMessageInfo("Save failed : " + saveOutput.ErrorMessage, Enums.MessageInfoType.Info);
                return CreateInitial(model);
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

        #region -------------- Private Method --------

        private string SaveUploadedFile(HttpPostedFileBase file, int lack1Id, int counter)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            sFileName = Constans.Lack1UploadFolderPath + Path.GetFileName("LACK1_" + lack1Id + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + counter + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        private List<Lack1SummaryProductionItemModel> ProcessSummaryProductionDetails(
            List<Lack1ProductionDetailItemSummaryByProdTypeModel> input)
        {
            if (input.Count > 0)
            {
                var groupedData = input.GroupBy(p => new
                {
                    p.UomId,
                    p.UomDesc
                }).Select(g => new Lack1SummaryProductionItemModel()
                {
                    UomId = g.Key.UomId,
                    UomDesc = g.Key.UomDesc,
                    Amount = g.Sum(p => p.TotalAmount)
                });

                //foreach (var item in groupedData)
                //{
                //    item.Amount = Math.Round(item.Amount, 0);
                //}
                return groupedData.ToList();

            }
            return new List<Lack1SummaryProductionItemModel>();
        }

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

            var plantListOnNppbkcFlagged = _plantBll.GetCompositeListByNppbkcIdWithFlag(nppbkcId);

            var dataSource = data;
            dataSource.AddRange(plantListOnNppbkcFlagged);
            dataSource = dataSource.DistinctBy(c => c.WERKS).ToList();

            return new SelectList(dataSource, "WERKS", "DROPDOWNTEXTFIELD");
        }

        private SelectList GetWasteAndReturnUomList()
        {
            var data = _uomBll.GetAll().Where(x => x.IS_DELETED != true && x.IS_EMS == true && (x.UOM_ID.ToLower() == "g" || x.UOM_DESC.ToLower() == "gram"));
            return new SelectList(data, "UOM_ID", "UOM_DESC");
        }

        private Lack1CreateViewModel InitialModel(Lack1CreateViewModel model)
        {
            var comp = GlobalFunctions.GetCompanyList(_companyBll);
            var userComp = _userPlantBll.GetCompanyByUserId(CurrentUser.USER_ID);
            var poaComp = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
            var distinctComp = comp.Where(x => userComp.Contains(x.Value));
            if (CurrentUser.UserRole == Enums.UserRole.POA) distinctComp = comp.Where(x => poaComp.Contains(x.Value));
            var getComp = new SelectList(distinctComp, "Value", "Text");

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            model.BukrList = getComp;
            model.MontList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearsList = CreateYearList();
            model.NppbkcList = GetNppbkcListOnPbck1ByCompanyCode(model.Bukrs);
            model.ReceivePlantList = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.NppbkcId);
            model.ExGoodTypeList = GetExciseGoodsTypeList(model.NppbkcId);
            model.SupplierList = GetSupplierPlantListByParam(model.NppbkcId, model.ExGoodsTypeId);
            model.WasteUomList = GetWasteAndReturnUomList();
            model.ReturnUomList = GetWasteAndReturnUomList();

            model.MenuPlantAddClassCss = model.Lack1Level == Enums.Lack1Level.Plant ? "active" : "";
            model.MenuNppbkcAddClassCss = model.Lack1Level == Enums.Lack1Level.Nppbkc ? "active" : "";

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

        #endregion

        #region Completed Document

        public ActionResult ListCompletedDocument()
        {
            var data = InitListCompletedDocument(new Lack1IndexCompletedDocumentViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Details = Mapper.Map<List<Lack1CompletedDocumentData>>(_lack1Bll.GetCompletedDocumentByParam(new Lack1GetByParamInput() { 
                    ListNppbkc = CurrentUser.ListUserNppbkc,
                    ListUserPlant = CurrentUser.ListUserPlants,
                    UserRole = CurrentUser.UserRole
                })),
                //first code when manager exists
                //IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer
                IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false)
            });

            return View("ListCompletedDocument", data);
        }

        private Lack1IndexCompletedDocumentViewModel InitListCompletedDocument(Lack1IndexCompletedDocumentViewModel model)
        {
            var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var nppbkcList = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                nppbkc = new SelectList(nppbkcList, "Value", "Text");
            }

            model.NppbkcIdList = nppbkc;
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
            inputPlant.UserRole = CurrentUser.UserRole;

            var dbDataPlant = _lack1Bll.GetCompletedDocumentByParam(inputPlant);

            var resultPlant = Mapper.Map<List<Lack1CompletedDocumentData>>(dbDataPlant);

            var viewModel = new Lack1IndexCompletedDocumentViewModel { Details = resultPlant };
            //first code when manager exists
            //viewModel.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            viewModel.IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Manager && CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false);

            return PartialView("_Lack1CompletedDocumentTable", viewModel);

        }

        #endregion

        #region -------------- Details -----------

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack1Data = _lack1Bll.GetDetailsById(id.Value);

            if (lack1Data == null)
            {
                return HttpNotFound();
            }

            return RetDetails(lack1Data, true);

        }

        public ActionResult RetDetails(Lack1DetailsDto lack1Data, bool isDisplayOnly)
        {
            var model = InitDetailModel(lack1Data);
            model.JsonData = JsonConvert.SerializeObject(model);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model = SetActiveMenu(model, model.Lack1Type);
            model.IsDisplayOnly = isDisplayOnly;
            return View("Details", model);
        }

        #endregion

        #region ----------------PrintPreview-------------

        [EncryptedParameter]
        public ActionResult PrintPreview(int? id)
        {
            //Get Report Source
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var lack1Data = _lack1Bll.GetPrintOutData(id.Value);
            if (lack1Data == null)
                HttpNotFound();

            Stream stream = GetReport(lack1Data, "Preview LACK-1");

            return File(stream, "application/pdf");
        }

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            //Get Report Source
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var lack1Data = _lack1Bll.GetPrintOutData(id.Value);
            if (lack1Data == null)
                HttpNotFound();

            Stream stream = GetReport(lack1Data, "LACK-1");

            return File(stream, "application/pdf");
        }

        private Stream GetReport(Lack1PrintOutDto data, string printTitle)
        {
            var dataSet = SetDataSetReport(data, printTitle);

            var rpt = new ReportClass
            {
                FileName = ConfigurationManager.AppSettings["Report_Path"] + "LACK1\\Lack1PrintOut.rpt"
            };
            rpt.Load();
            rpt.SetDataSource(dataSet);
            Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            rpt.Close();
            return stream;
        }

        private DataSet SetDataSetReport(Lack1PrintOutDto data, string printTitle)
        {
            var dsReport = new dsLack1();

            //master info
            var dMasterRow = dsReport.Lack1.NewLack1Row();
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            dMasterRow.Lack1Id = data.Lack1Id.ToString();
            dMasterRow.Lack1Number = data.Lack1Number;
            dMasterRow.CompanyCode = data.Bukrs;
            dMasterRow.CompanyName = data.Butxt;
            dMasterRow.PlantAddress = data.Lack1Plant.Count > 0 ? string.Join(Environment.NewLine, data.Lack1Plant.Select(d => d.PLANT_ADDRESS).ToList()) : "-";
            if (data.Lack1Pbck1Mapping.Count > 0)
            {
                dMasterRow.NoTglPbck1 = string.Join(Environment.NewLine,
                    data.Lack1Pbck1Mapping.Select(d => (d.PBCK1_NUMBER + " - " + d.DisplayDecreeDate)).ToList());
            }
            else
            {
                dMasterRow.NoTglPbck1 = "-";
            }

            dMasterRow.ExcisableGoodsTypeId = data.ExGoodsType;
            dMasterRow.ExcisableGoodsTypeDesc = data.ExGoodsTypeDesc;
            dMasterRow.SupplierCompanyName = data.SupplierCompanyName;
            dMasterRow.SupplierCompanyAddress = data.SupplierPlantAddress;
            dMasterRow.Lack1Period = data.PeriodNameInd + " " + data.PeriodYears;
            dMasterRow.NppbkcCity = data.NppbkcCity;
            dMasterRow.NppbkcId = data.NppbkcId;
            dMasterRow.SubmissionDate = data.SubmissionDateDisplayString;
            dMasterRow.CreatorName = data.PoaPrintedName;
            dMasterRow.PrintTitle = printTitle;
            if (data.HeaderFooter != null)
            {
                if (!string.IsNullOrEmpty(data.HeaderFooter.HEADER_IMAGE_PATH))
                    dMasterRow.Header = GetHeader(data.HeaderFooter.HEADER_IMAGE_PATH);
                dMasterRow.Footer = !string.IsNullOrEmpty(data.HeaderFooter.FOOTER_CONTENT) ? data.HeaderFooter.FOOTER_CONTENT.Replace("<br />", Environment.NewLine) : string.Empty;    
            }

            if (data.SupplierCompanyName == null)
            {
                if (data.Lack1Pbck1Mapping.Count > 0)
                {
                    dMasterRow.SupplierCompanyName = data.Lack1Pbck1Mapping.Select(d => d.SUPPLIER_COMPANY).FirstOrDefault();
                }
            }

            dsReport.Lack1.AddLack1Row(dMasterRow);
            
            //for total
            var prodList = Mapper.Map<List<Lack1ProductionDetailItemSummaryByProdTypeModel>>(data.FusionSummaryProductionByProdTypeList);
            var summaryProductionList = ProcessSummaryProductionDetails(prodList);
            var totalSummaryProductionList = string.Join(Environment.NewLine,
                summaryProductionList.Select(d => d.Amount.ToString("N2") + " " + d.UomDesc).ToList());

            //for each Excisable Goods Type per tis to tis and tis to fa
            //process tis to fa first
            string summaryProductionJenis = "";
            string summaryProductionAmount = "0";
            int loopCountForUsage = 0;
            var prodTisToFa = data.InventoryProductionTisToFa.ProductionData;
            if (prodTisToFa != null)
            {
                summaryProductionJenis = string.Join(Environment.NewLine,
                    prodTisToFa.ProductionSummaryByProdTypeList.Select(d => d.ProductAlias).ToList());
                summaryProductionAmount = string.Join(Environment.NewLine,
                    prodTisToFa.ProductionSummaryByProdTypeList.Select(
                        d => d.TotalAmount.ToString("N2") + " " + d.UomDesc).ToList());

                loopCountForUsage = prodTisToFa.ProductionSummaryByProdTypeList.Count;
            }
            


            var usage = data.Usage;

            /*skip this logic for etil alcohol, although IsTisToTis flag is checked*/
            if (data.IsTisToTis && !data.IsEtilAlcohol)
            {
                //with tis to tis
                //process tis to tis
                var prodTisToTis = data.InventoryProductionTisToTis.ProductionData;
                var summaryProductionJenisTisToTis = prodTisToTis == null ? "" : string.Join(Environment.NewLine,
                    prodTisToTis.ProductionSummaryByProdTypeList.Select(d => d.ProductAlias).ToList());
                var summaryProductionAmountTisToTis = prodTisToTis == null ? "0" : string.Join(Environment.NewLine,
                    prodTisToTis.ProductionSummaryByProdTypeList.Select(d => d.TotalAmount.ToString("N2") + " " + d.UomDesc).ToList());

                summaryProductionJenis = summaryProductionJenis + Environment.NewLine + (!string.IsNullOrEmpty(summaryProductionJenisTisToTis) ? summaryProductionJenisTisToTis : "-");
                summaryProductionAmount = summaryProductionAmount + Environment.NewLine + (!string.IsNullOrEmpty(summaryProductionAmountTisToTis) ? summaryProductionAmountTisToTis : "-");

                //for (var i = 0; i < loopCountForUsage; i++)
                //{
                //    usage = usage + Environment.NewLine;
                //}

                usage = usage + (data.UsageTisToTis.HasValue ? data.UsageTisToTis.Value : 0);

            }

            //set detail item
            //if (data.Lack1IncomeDetail.Count <= 0) return dsReport;

            var totalAmount = data.Lack1IncomeDetail.Sum(d => d.AMOUNT);
            //var endingBalance = (data.BeginingBalance - (data.Usage + (data.UsageTisToTis.HasValue ? data.UsageTisToTis.Value  : 0)) + data.TotalIncome - data.ReturnQty);
            var endingBalance = data.CloseBalance;
            var noted = !string.IsNullOrEmpty(data.Noted) ? data.Noted.Replace("<br />", Environment.NewLine) : string.Empty;
            //var docNoted = !string.IsNullOrEmpty(data.DocumentNoted) ? data.DocumentNoted.Replace("<br />", Environment.NewLine) : string.Empty;

            var docNoted = string.Empty;
            
            if (data.Ck5RemarkData != null)
            {
                docNoted = GenerateRemarkContent(data.Ck5RemarkData.Ck5WasteData, "Waste");
                docNoted = docNoted + (docNoted.Trim() == string.Empty ? string.Empty : Environment.NewLine) + GenerateRemarkContent(data.Ck5RemarkData.Ck5ReturnData, "Return");
                docNoted = docNoted + (docNoted.Trim() == string.Empty ? string.Empty : Environment.NewLine) + GenerateRemarkContent(data.Ck5RemarkData.Ck5TrialData, "Trial");
            }

            var docToDisplay = (noted.Trim() != string.Empty ? noted.Trim() + Environment.NewLine : string.Empty) +
                                docNoted;

            if (data.Lack1IncomeDetail.Count > 0)
            {
                foreach (var item in data.Lack1IncomeDetail)
                {
                    var detailRow = dsReport.Lack1Items.NewLack1ItemsRow();
                    detailRow.BeginningBalance = data.BeginingBalance.ToString("N2");
                    detailRow.Ck5RegNumber = item.REGISTRATION_NUMBER;
                    detailRow.Ck5RegDate = item.REGISTRATION_DATE.HasValue
                        ? item.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy")
                        : string.Empty;
                    detailRow.Ck5Amount = item.AMOUNT.ToString("N2");
                    detailRow.Usage = usage.ToString("N2");
                    detailRow.ListJenisBKC = summaryProductionJenis;
                    detailRow.ListJumlahBKC = summaryProductionAmount;
                    detailRow.EndingBalance = endingBalance.ToString("N2");
                    detailRow.Noted = docToDisplay;
                    detailRow.Ck5TotalAmount = totalAmount.ToString("N2");
                    detailRow.ListTotalJumlahBKC = totalSummaryProductionList;

                    dsReport.Lack1Items.AddLack1ItemsRow(detailRow);

                }
            }
            else
            {
                var detailRow = dsReport.Lack1Items.NewLack1ItemsRow();
                detailRow.BeginningBalance = data.BeginingBalance.ToString("N2");
                detailRow.Ck5RegNumber = string.Empty;
                detailRow.Ck5RegDate = string.Empty;
                detailRow.Ck5Amount = "0";
                detailRow.Usage = usage.ToString("N2");
                detailRow.ListJenisBKC = summaryProductionJenis;
                detailRow.ListJumlahBKC = summaryProductionAmount;
                detailRow.EndingBalance = endingBalance.ToString("N2");
                detailRow.Noted = docToDisplay;
                detailRow.Ck5TotalAmount = totalAmount.ToString("N2");
                detailRow.ListTotalJumlahBKC = string.IsNullOrEmpty(totalSummaryProductionList) ? "0" : totalSummaryProductionList;

                dsReport.Lack1Items.AddLack1ItemsRow(detailRow);
            }




            return dsReport;
        }

        private string GenerateRemarkContent(List<Lack1IncomeDetailDto> data, string title)
        {
            var rc = string.Empty;
            if (data == null || data.Count <= 0) return rc;
            rc = title + Environment.NewLine;
            //rc += string.Join(Environment.NewLine, data.Select(
            //    d =>
            //        "CK-5 " + d.REGISTRATION_NUMBER + " - " +
            //        (d.REGISTRATION_DATE.HasValue
            //            ? d.REGISTRATION_DATE.Value.ToString("dd.MM.yyyy")
            //            : string.Empty) + " : " + d.AMOUNT.ToString("N2") + " " + d.PACKAGE_UOM_DESC).ToList());

            //LOGS SKYPE, REMOVE DATE
            rc += string.Join(Environment.NewLine, data.Select(
               d =>
                   "CK-5 " + d.REGISTRATION_NUMBER + " : " + d.AMOUNT.ToString("N2") + " " + d.PACKAGE_UOM_DESC).ToList());
            return rc;
        }

        private byte[] GetHeader(string imagePath)
        {
            byte[] imgbyte = null;
            try
            {

                FileStream fs;
                BinaryReader br;

                if (System.IO.File.Exists(Server.MapPath(imagePath)))
                {
                    fs = new FileStream(Server.MapPath(imagePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    // initialise the binary reader from file streamobject 
                    br = new BinaryReader(fs);
                    // define the byte array of filelength 
                    imgbyte = new byte[fs.Length + 1];
                    // read the bytes from the binary reader 
                    imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));


                    br.Close();
                    // close the binary reader 
                    fs.Close();
                    // close the file stream 
                }
            }
            catch (Exception)
            {
            }
            return imgbyte;
            // Return Datatable After Image Row Insertion
        }

        #endregion

        #region ----------------- Edit -----------

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack1Data = _lack1Bll.GetDetailsById(id.Value);

            if (lack1Data == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                return RedirectToAction("Edits", new { id });
            }

            //first code when manager exists
            //if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            if (CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //redirect to details for approval/rejected
                return RetDetails(lack1Data, true);
            }

            if (lack1Data.Status == Enums.DocumentStatus.WaitingForApproval)
            {
                return RetDetails(lack1Data, false);
            }

            /* Old Code before CR-2 : 2015-12-22 Remove manager approve*/
            //if (lack1Data.Status == Enums.DocumentStatus.WaitingForApproval ||
            //    lack1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager)
            //{
            //    return RetDetails(lack1Data, false);
            //}

            //first code when manager exists
            //if (CurrentUser.UserRole == Enums.UserRole.Manager)
            //{
            //    //redirect to details for approval/rejected
            //    return RetDetails(lack1Data, true);
            //}

            if ((CurrentUser.USER_ID == lack1Data.CreateBy || CurrentUser.USER_ID == lack1Data.ApprovedByPoa ) &&
                lack1Data.Status == Enums.DocumentStatus.WaitingForApproval)
            {
                return RetDetails(lack1Data, false);
            }

            /* Old code before CR-2  : 2015-12-22 Remove manager approve */
            //if (CurrentUser.USER_ID == lack1Data.CreateBy &&
            //    (lack1Data.Status == Enums.DocumentStatus.WaitingForApproval ||
            //     lack1Data.Status == Enums.DocumentStatus.WaitingForApprovalManager))
            //{
            //    return RetDetails(lack1Data, false);
            //}

            var model = InitEditModel(lack1Data);
            model.JsonData = JsonConvert.SerializeObject(model);
            model = InitEditList(model);
            model.IsCreateNew = false;

            if (!IsAllowEditLack1(lack1Data.CreateBy, lack1Data.Status, lack1Data.Lack1Number))
            {
                AddMessageInfo(
                    "Operation not allowed.",
                    Enums.MessageInfoType.Error);
                if (lack1Data.Lack1Level == Enums.Lack1Level.Nppbkc)
                {
                    return RedirectToAction("Index");
                }
                else if (lack1Data.Lack1Level == Enums.Lack1Level.Plant)
                {
                    return RedirectToAction("ListByPlant");
                }
            }

            model.ControllerAction = model.Status == Enums.DocumentStatus.WaitingGovApproval || model.Status == Enums.DocumentStatus.Completed ? "GovApproveDocument" : "Edit";

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View(model);
        }

        private bool IsAllowEditLack1(string userId, Enums.DocumentStatus status, string docNumber)
        {
            //bool isAllow = CurrentUser.USER_ID == userId;
            //if (!(status == Enums.DocumentStatus.Draft || status == Enums.DocumentStatus.Rejected 
            //    || status == Enums.DocumentStatus.WaitingGovApproval || status == Enums.DocumentStatus.Completed))
            //{
            //    isAllow = false;
            //}

            //return isAllow;
            return _workflowBll.IsAllowEditLack1(userId, CurrentUser.USER_ID, status, CurrentUser.UserRole, docNumber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lack1EditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();

                    if (errors.Count > 0)
                    {
                        //get error details
                    }

                    model = InitEditList(model);
                    model = SetEditHistory(model);
                    model.MainMenu = _mainMenu;
                    model.CurrentMenu = PageInfo;
                    AddMessageInfo("Invalid input", Enums.MessageInfoType.Error);
                    return View(model);
                }

                //if (model.CreateBy != CurrentUser.USER_ID)
                //{
                //    return RedirectToAction("Detail", new { id = model.Lack1Id });
                //}
                if (!_workflowBll.IsAllowEditLack1(model.CreateBy, CurrentUser.USER_ID, model.Status, CurrentUser.UserRole, model.Lack1Number))
                    return RedirectToAction("Detail", new { id = model.Lack1Id });

                bool isSubmit = model.IsSaveSubmit == "submit";

                var lack1Data = Mapper.Map<Lack1DetailsDto>(model);

                var input = new Lack1SaveEditInput()
                {
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Modified,
                    Detail = lack1Data,
                    IsTisToTis = model.IsTisToTisReport,
                    IsSupplierNppbkcImport = model.IsSupplierNppbkcImport,
                    IsNeedGenerate = !isSubmit
                };

                var saveResult = _lack1Bll.SaveEdit(input);

                if (saveResult.Success)
                {
                    if (isSubmit)
                    {
                        Lack1Workflow(model.Lack1Id, Enums.ActionType.Submit, string.Empty, saveResult.IsModifiedHistory);
                        AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                        return RedirectToAction("Details", "Lack1", new { id = model.Lack1Id });
                    }
                    AddMessageInfo("Save Successfully", Enums.MessageInfoType.Info);
                    return RedirectToAction("Edit", new { id = model.Lack1Id });
                }
                AddMessageInfo(saveResult.ErrorMessage, Enums.MessageInfoType.Error);
            }
            catch (Exception ex)//just for debugging, uncomment this line
            //catch (Exception)
            {
                model = InitEditList(model);
                model = SetEditHistory(model);
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;
                model = SetEditActiveMenu(model, model.Lack1Type);
                AddMessageInfo("Save edit failed.", Enums.MessageInfoType.Error);
                //AddMessageInfo("Save edit failed : " + ex.Message, Enums.MessageInfoType.Error);//just for debugging
                return View(model);
            }
            model = InitEditList(model);
            model = SetEditHistory(model);
            model = SetEditActiveMenu(model, model.Lack1Type);
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            return View(model);

        }

        private Lack1EditViewModel SetEditHistory(Lack1EditViewModel model)
        {
            //workflow history
            var workflowInput = new GetByFormNumberInput
            {
                FormNumber = model.Lack1Number,
                DocumentStatus = model.Status,
                NppbkcId = model.NppbkcId,
                DocumentCreator = model.CreateBy,
                PlantId = model.LevelPlantId
            };

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK1,
                    model.Lack1Id.ToString()));

            var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(model.Lack1Number));

            model.ChangesHistoryList = changesHistory;
            model.WorkflowHistory = workflowHistory;
            model.PrintHistoryList = printHistory;

            return model;
        }
        

        private Lack1EditViewModel InitEditList(Lack1EditViewModel model)
        {
            var comp = GlobalFunctions.GetCompanyList(_companyBll);
            var userComp = _userPlantBll.GetCompanyByUserId(CurrentUser.USER_ID);
            var poaComp = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
            var distinctComp = comp.Where(x => userComp.Contains(x.Value));
            if (CurrentUser.UserRole == Enums.UserRole.POA) distinctComp = comp.Where(x => poaComp.Contains(x.Value));
            var getComp = new SelectList(distinctComp, "Value", "Text");

            var nppbkc = GetNppbkcListOnPbck1ByCompanyCode(model.Bukrs);
            var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));

            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.NppbkcId);
            var filterPlant = listPlant;
            var newListPlant = new SelectList(filterPlant, "Value", "Text");
            if (CurrentUser.UserRole == Enums.UserRole.User)
            {
                var newFilterPlant = listPlant.Where(x => CurrentUser.ListUserPlants.Contains(x.Value));
                newListPlant = new SelectList(newFilterPlant, "Value", "Text");
            }

            model.BukrList = getComp;
            model.MontList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearsList = CreateYearList();
            model.NppbkcList = new SelectList(filterNppbkc, "Value", "Text");
            model.ReceivePlantList = newListPlant;
            model.ExGoodTypeList = GetExciseGoodsTypeList(model.NppbkcId);
            model.SupplierList = GetSupplierPlantListByParam(model.NppbkcId, model.ExGoodsTypeId);
            model.WasteUomList = GetWasteAndReturnUomList();
            model.ReturnUomList = GetWasteAndReturnUomList();

            return model;

        }

        private Lack1EditViewModel SetEditActiveMenu(Lack1EditViewModel model, Enums.LACK1Type lType)
        {
            const string activeCss = "active";
            switch (lType)
            {
                case Enums.LACK1Type.ListByNppbkc:
                    model.MenuNppbkcAddClassCss = activeCss;
                    break;
                case Enums.LACK1Type.ComplatedDocument:
                    model.MenuCompletedAddClassCss = activeCss;
                    break;
                case Enums.LACK1Type.ListByPlant:
                    model.MenuPlantAddClassCss = activeCss;
                    break;
            }
            return model;
        }

        private Lack1EditViewModel InitEditModel(Lack1DetailsDto lack1Data)
        {

            var model = Mapper.Map<Lack1EditViewModel>(lack1Data);

            model = SetEditHistory(model);

            Enums.LACK1Type lack1Type;
            if (lack1Data.Status == Enums.DocumentStatus.Completed)
            {
                lack1Type = Enums.LACK1Type.ComplatedDocument;
            }
            else
            {
                lack1Type = lack1Data.Lack1Level == Enums.Lack1Level.Nppbkc ? Enums.LACK1Type.ListByNppbkc : Enums.LACK1Type.ListByPlant;
            }

            model.Lack1Type = lack1Type;

            model.FusionSummaryProductionList = ProcessSummaryProductionDetails(model.FusionSummaryProductionByProdTypeList);

            SetEditActiveMenu(model, lack1Type);

            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = lack1Data.CreateBy,
                CurrentUser = CurrentUser.USER_ID,
                CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                DocumentNumber = model.Lack1Number,
                NppbkcId = model.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                //first code when manager exists
                //model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }

            model.AllowPrintDocument = _workflowBll.AllowPrint(model.Status);

            return model;
        }

        #endregion

        #region ----------------- Edit Completed Document -----------

        public ActionResult Edits(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var lack1Data = _lack1Bll.GetDetailsById(id.Value);

            if (lack1Data == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                return RetDetails(lack1Data, true);
            }
            
            var model = InitEditModel(lack1Data);
            model.JsonData = JsonConvert.SerializeObject(model);
            model = InitEditList(model);
            model.IsCreateNew = false;

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            return View(model);
        }

        #endregion

        private Lack1ItemViewModel SetHistory(Lack1ItemViewModel model)
        {
            //workflow history
            var workflowInput = new GetByFormNumberInput
            {
                FormNumber = model.Lack1Number,
                DocumentStatus = model.Status,
                NppbkcId = model.NppbkcId,
                DocumentCreator = model.CreateBy,
                PlantId = model.LevelPlantId
            };

            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            var changesHistory =
                Mapper.Map<List<ChangesHistoryItemModel>>(
                    _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK1,
                    model.Lack1Id.ToString()));

            var printHistory = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(model.Lack1Number));

            model.ChangesHistoryList = changesHistory;
            model.WorkflowHistory = workflowHistory;
            model.PrintHistoryList = printHistory;

            return model;
        }

        private Lack1ItemViewModel InitDetailModel(Lack1DetailsDto lack1Data)
        {

            var model = Mapper.Map<Lack1ItemViewModel>(lack1Data);

            model = SetHistory(model);

            Enums.LACK1Type lack1Type;
            if (lack1Data.Status == Enums.DocumentStatus.Completed)
            {
                lack1Type = Enums.LACK1Type.ComplatedDocument;
            }
            else
            {
                lack1Type = lack1Data.Lack1Level == Enums.Lack1Level.Nppbkc ? Enums.LACK1Type.ListByNppbkc : Enums.LACK1Type.ListByPlant;
            }

            model.Lack1Type = lack1Type;
            //model.SummaryProductionList = ProcessSummaryProductionDetails(model.ProductionSummaryByProdTypeList);
            model.FusionSummaryProductionList = ProcessSummaryProductionDetails(model.FusionSummaryProductionByProdTypeList);

            SetActiveMenu(model, lack1Type);

            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = lack1Data.CreateBy,
                CurrentUser = CurrentUser.USER_ID,
                CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                DocumentNumber = model.Lack1Number,
                NppbkcId = model.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;

            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                //first code when manager exists
                //model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }

            model.AllowPrintDocument = _workflowBll.AllowPrint(model.Status);

            return model;
        }

        private Lack1ItemViewModel SetActiveMenu(Lack1ItemViewModel model, Enums.LACK1Type lType)
        {
            const string activeCss = "active";
            switch (lType)
            {
                case Enums.LACK1Type.ListByNppbkc:
                    model.MenuNppbkcAddClassCss = activeCss;
                    break;
                case Enums.LACK1Type.ComplatedDocument:
                    model.MenuCompletedAddClassCss = activeCss;
                    break;
                case Enums.LACK1Type.ListByPlant:
                    model.MenuPlantAddClassCss = activeCss;
                    break;
            }
            return model;
        }

        #region -------- workflow ------

        public ActionResult ApproveDocument(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            bool isSuccess = false;
            try
            {
                Lack1Workflow(id.Value, Enums.ActionType.Approve, string.Empty);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            if (!isSuccess) return RedirectToAction("Details", "Lack1", new { id });
            AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult RejectDocument(Lack1ItemViewModel model)
        {
            bool isSuccess = false;
            try
            {
                Lack1Workflow(model.Lack1Id, Enums.ActionType.Reject, model.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Details", "Lack1", new { id = model.Lack1Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

        private void Lack1Workflow(int id, Enums.ActionType actionType, string comment, bool isModified = false)
        {
            var input = new Lack1WorkflowDocumentInput()
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                IsModified = isModified
            };

            _lack1Bll.Lack1Workflow(input);
        }

        private void Lack1WorkflowGovApprove(Lack1EditViewModel lack1Data, Enums.ActionType actionType, string comment)
        {
            if (lack1Data.Status == Enums.DocumentStatus.Completed) actionType = Enums.ActionType.Completed;

            var input = new Lack1WorkflowDocumentInput()
            {
                DocumentId = lack1Data.Lack1Id,
                ActionType = actionType,
                UserRole = CurrentUser.UserRole,
                UserId = CurrentUser.USER_ID,
                DocumentNumber = lack1Data.Lack1Number,
                Comment = comment,
                AdditionalDocumentData = new Lack1WorkflowDocumentData()
                {
                    DecreeDate = lack1Data.DecreeDate,
                    Lack1Document = Mapper.Map<List<Lack1DocumentDto>>(lack1Data.Lack1Document)
                }
            };
            _lack1Bll.Lack1Workflow(input);
        }

        [HttpPost]
        public ActionResult GovApproveDocument(Lack1EditViewModel model)
        {
            
            if (model.GovApprovalActionType != Enums.ActionType.BackToGovApprovalAfterCompleted && model.DecreeFiles == null)
            {
                AddMessageInfo("Decree Doc is required.", Enums.MessageInfoType.Error);
                return RedirectToAction("Edit", "Lack1", new { id = model.Lack1Id });
            }

            bool isSuccess = false;
            string err = string.Empty;
            try
            {
                model.Lack1Document = new List<Lack1DocumentItemModel>();
                if (model.DecreeFiles != null)
                {
                    int counter = 0;
                    foreach (var item in model.DecreeFiles)
                    {
                        if (item != null)
                        {
                            var filenamecheck = item.FileName;

                            if (filenamecheck.Contains("\\"))
                            {
                                filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }

                            var decreeDoc = new Lack1DocumentItemModel()
                            {
                                FILE_NAME = filenamecheck,
                                FILE_PATH = SaveUploadedFile(item, model.Lack1Id, counter)
                            };
                            model.Lack1Document.Add(decreeDoc);
                            counter += 1;
                        }
                        else
                        {
                            if (model.GovApprovalActionType == Enums.ActionType.BackToGovApprovalAfterCompleted)
                                continue;
                            AddMessageInfo("Please upload the decree doc", Enums.MessageInfoType.Error);
                            return RedirectToAction("Edit", "Lack1", new { id = model.Lack1Id });
                        }
                    }
                }

                Lack1WorkflowGovApprove(model, model.GovApprovalActionType, model.Comment);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (!isSuccess)
            {
                AddMessageInfo(err, Enums.MessageInfoType.Error);
                return RedirectToAction("Edit", "Lack1", new { id = model.Lack1Id });
            }
            if (model.GovApprovalActionType == Enums.ActionType.BackToGovApprovalAfterCompleted)
            {
                AddMessageInfo("Document Back to Waiting for Government Approval", Enums.MessageInfoType.Success);
            }
            else
            {
                AddMessageInfo("Document " + EnumHelper.GetDescription(model.GovStatus), Enums.MessageInfoType.Success);
            }
            
            return RedirectToAction(model.Lack1Level == Enums.Lack1Level.Plant ? "ListByPlant" : "Index");
        }

        [HttpPost]
        public ActionResult GovCompletedDocument(Lack1EditViewModel model)
        {
            var oldDoc = _lack1Bll.GetDetailsById(model.Lack1Id).Lack1Document;

            if (model.GovApprovalActionType != Enums.ActionType.BackToGovApprovalAfterCompleted && model.DecreeFiles == null && oldDoc.Count == 0)
            {
                AddMessageInfo("Decree Doc is required.", Enums.MessageInfoType.Error);
                return RedirectToAction("Edits", "Lack1", new { id = model.Lack1Id });
            }

            bool isSuccess = false;
            string err = string.Empty;
            try
            {
                model.Lack1Document = new List<Lack1DocumentItemModel>();
                if (model.DecreeFiles != null)
                {
                    int counter = 0;
                    foreach (var item in model.DecreeFiles)
                    {
                        if (item != null)
                        {
                            var filenamecheck = item.FileName;

                            if (filenamecheck.Contains("\\"))
                            {
                                filenamecheck = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }

                            var decreeDoc = new Lack1DocumentItemModel()
                            {
                                FILE_NAME = filenamecheck,
                                FILE_PATH = SaveUploadedFile(item, model.Lack1Id, counter)
                            };
                            model.Lack1Document.Add(decreeDoc);
                            counter += 1;
                        }
                        else
                        {
                            if (oldDoc.Count > 0)
                                continue;
                            AddMessageInfo("Please upload the decree doc", Enums.MessageInfoType.Error);
                            return RedirectToAction("Edit", "Lack1", new { id = model.Lack1Id });
                            
                        }
                    }
                }

                Lack1WorkflowGovApprove(model, model.GovApprovalActionType, model.Comment);

                var input = new Lack1UpdateSomeField()
                {
                    Id = model.Lack1Id,
                    SubmissionDate = model.SubmissionDate,
                    WasteQty = model.WasteQty,
                    WasteUom = model.WasteUom,
                    ReturnQty = model.ReturnQty,
                    ReturnUom = model.ReturnUom,
                    Noted = model.Noted
                };

                _lack1Bll.UpdateSomeField(input);

                //Lack1WorkflowGovApprove(model, model.GovApprovalActionType, string.Empty);

                

                
                
                isSuccess = true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (!isSuccess)
            {
                AddMessageInfo(err, Enums.MessageInfoType.Error);
                return RedirectToAction("Edits", "Lack1", new { id = model.Lack1Id });
            }

            AddMessageInfo("Document is saved", Enums.MessageInfoType.Success);

            return RedirectToAction("ListCompletedDocument");
        }

        #endregion

        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var lack1Data = _lack1Bll.GetDetailsById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.LACK1,
                FORM_ID = lack1Data.Lack1Id,
                FORM_NUMBER = lack1Data.Lack1Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel
            {
                PrintHistoryList =
                    Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack1Data.Lack1Number))
            };
            return PartialView("_PrintHistoryTable", model);

        }

        public void ExportClientsListToExcel(int id)
        {

            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.LACK1, id.ToString());

            var model = Mapper.Map<List<ChangesHistoryItemModel>>(listHistory);

            var grid = new GridView
            {
                DataSource = from d in model
                             select new
                             {
                                 Date = d.MODIFIED_DATE.HasValue ? d.MODIFIED_DATE.Value.ToString("dd MMM yyyy HH:mm:ss") : string.Empty,
                                 FieldName = d.FIELD_NAME,
                                 OldValue = d.OLD_VALUE,
                                 NewValue = d.NEW_VALUE,
                                 User = d.USERNAME

                             }
            };

            grid.DataBind();

            var fileName = "LACK1ChangeLog" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //'Excel 2003 : "application/vnd.ms-excel"
            //'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);
            
            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());

            Response.Flush();

            Response.End();

        }

        #region ----------- Summary Report -------------

        public ActionResult SummaryReport()
        {
            Lack1SummaryReportViewModel model;
            try
            {
                model = new Lack1SummaryReportViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    DetailsList = SearchSummaryReports()
                };
                model = InitSearchSummaryReportViewModel(model);
            }
            catch (Exception ex)
            {
                model = new Lack1SummaryReportViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return View("SummaryReport", model);
        }

        private Lack1SummaryReportViewModel InitSearchSummaryReportViewModel(Lack1SummaryReportViewModel model)
        {
            var comp = GlobalFunctions.GetCompanyList(_companyBll);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var userComp = _userPlantBll.GetCompanyByUserId(CurrentUser.USER_ID);
                var poaComp = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
                var distinctComp = comp.Where(x => userComp.Contains(x.Value));
                if (CurrentUser.UserRole == Enums.UserRole.POA) distinctComp = comp.Where(x => poaComp.Contains(x.Value));
                var getComp = new SelectList(distinctComp, "Value", "Text");
                comp = getComp;
            }

            model.SearchView = new Lack1SearchSummaryReportViewModel
            {
                CompanyCodeList = comp,
                PeriodMonthList = GlobalFunctions.GetMonthList(_monthBll),
                PeriodMonth = DateTime.Now.Month,
                PeriodYearList = GetYearListInLack1Data()
            };

            model.SearchView.NppbkcIdList = GetNppbkcIdListOnLack1Data(model.SearchView.CompanyCode);
            model.SearchView.ReceivingPlantIdList = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.SearchView.NppbkcId);
            model.SearchView.ExcisableGoodsTypeList = GetExciseGoodsTypeList(model.SearchView.NppbkcId);
            model.SearchView.SupplierPlantIdList = GetSupplierPlantListByParam(model.SearchView.NppbkcId, model.SearchView.ExcisableGoodsType);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchView.ApproverList = GlobalFunctions.GetPoaAll(_poabll);
            return model;
        }

        private SelectList GetYearListInLack1Data()
        {
            var lack1YearList = from x in _lack1Bll.GetYearList()
                                orderby x descending
                                select new SelectItemModel()
                                {
                                    ValueField = x,
                                    TextField = x.ToString(CultureInfo.InvariantCulture)
                                };

            return new SelectList(lack1YearList, "ValueField", "TextField");
        }

        private SelectList GetNppbkcIdListOnLack1Data(string companyCode)
        {
            var data = _lack1Bll.GetNppbckListByCompanyCode(companyCode);
            return new SelectList(data, "NPPBKC_ID", "NPPBKC_ID");
        }

        private List<Lack1SummaryReportItemModel> SearchSummaryReports(Lack1SearchSummaryReportViewModel filter = null)
        {
            //Get All
            if (filter == null)
            {
                //Get All
                var lack1Data = _lack1Bll.GetSummaryReportByParam(new Lack1GetSummaryReportByParamInput() { 
                     ListUserPlant = CurrentUser.ListUserPlants,
                     ListNppbkc = CurrentUser.ListUserNppbkc,
                     UserRole = CurrentUser.UserRole,
                     PeriodMonth = DateTime.Now.Month,
                     PeriodYear = DateTime.Now.Year
                });
                return Mapper.Map<List<Lack1SummaryReportItemModel>>(lack1Data);
            }

            //getbyparams
            var input = Mapper.Map<Lack1GetSummaryReportByParamInput>(filter);
            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;
            input.UserRole = CurrentUser.UserRole;

            var dbData = _lack1Bll.GetSummaryReportByParam(input);

            return Mapper.Map<List<Lack1SummaryReportItemModel>>(dbData);
        }

        [HttpPost]
        public PartialViewResult SearchSummaryReports(Lack1SummaryReportViewModel model)
        {
            model.DetailsList = SearchSummaryReports(model.SearchView);
            return PartialView("_Lack1SummaryReport", model);
        }

        [HttpPost]
        public ActionResult ExportSummaryReports(Lack1SummaryReportViewModel model)
        {
            try
            {
                ExportSummaryReportsToExcel(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("SummaryReports");
        }

        public void ExportSummaryReportsToExcel(Lack1SummaryReportViewModel model)
        {
            var dataSummaryReport = SearchSummaryReports(model.ExportModel);

            //todo: to automapper
            var src = Mapper.Map<List<Lack1ExportSummaryDataModel>>(dataSummaryReport);

            var grid = new GridView
            {
                DataSource = src,
                AutoGenerateColumns = false
            };

            if (model.ExportModel.BLack1Number)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Lack1Number",
                    HeaderText = "LACK-1 Number"
                });
            }

            if (model.ExportModel.BExcisableGoodsTypeDesc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ExcisableGoodsTypeDesc",
                    HeaderText = "Excisable Goods Type"
                });
            }

            if (model.ExportModel.BCompanyName)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "CompanyName",
                    HeaderText = "Receiver Company"
                });
            }

            if (model.ExportModel.BNppbkcId)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "NppbkcId",
                    HeaderText = "Receiver NPPBKC"
                });
            }

            if (model.ExportModel.BKppbcId)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "KppbcId",
                    HeaderText = "Receiver KPPBC"
                });
            }

            if (model.ExportModel.BReceivingPlantId)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ReceivingPlantId",
                    HeaderText = "Receiver Plant ID"
                });
            }

            if (model.ExportModel.BReceivingPlantName)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ReceivingPlantName",
                    HeaderText = "Receiver Plant Desc"
                });
            }

            if (model.ExportModel.BSupplierCompany)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierCompany",
                    HeaderText = "Supplier Company"
                });
            }

            if (model.ExportModel.BSupplierNppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierNppbkc",
                    HeaderText = "Supplier NPPBKC"
                });
            }

            if (model.ExportModel.BSupplierKppbc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierKppbc",
                    HeaderText = "Supplier KPPBC"
                });
            }

            if (model.ExportModel.BSupplierPlantId)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierPlantId",
                    HeaderText = "Supplier Plant ID"
                });
            }

            if (model.ExportModel.BSupplierPlantName)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "SupplierPlantName",
                    HeaderText = "Supplier Plant Desc"
                });
            }

            if (model.ExportModel.BPeriod)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Period",
                    HeaderText = "Period"
                });
            }

            if (model.ExportModel.BPbck1Number)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1Number",
                    HeaderText = "PBCK-1 Number"
                });
            }

            if (model.ExportModel.BPbck1Date)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck1Date",
                    HeaderText = "PBCK-1 Date"
                });
            }

            if (model.ExportModel.BApprover)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Approver",
                    HeaderText = "POA Approved by"
                });
            }

            if (model.ExportModel.BCreator)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Creator",
                    HeaderText = "Creator"
                });
            }

            if (model.ExportModel.BDocumentStatus)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "DocumentStatus",
                    HeaderText = "Status"
                });
            }

            grid.DataBind();

            var fileName = "Lack1" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //'Excel 2003 : "application/vnd.ms-excel"
            //'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());

            Response.Flush();

            Response.End();
        }

        #endregion

        #region --------------- Detail Report -------------

        public ActionResult DetailReport()
        {

            Lack1DetailReportViewModel model;
            try
            {
                model = new Lack1DetailReportViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    DetailList = SearchDetailReport()
                };
                model = InitSearchDetilReportViewModel(model);
            }
            catch (Exception ex)
            {
                model = new Lack1DetailReportViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return View("DetailReport", model);
        }

        [HttpPost]
        public ActionResult SearchDetailReport(Lack1DetailReportViewModel model)
        {
            model.DetailList = SearchDetailReport(model.SearchView);
            return PartialView("_Lack1DetailReport", model);
        }

        private List<Lack1DetailReportItemModel> SearchDetailReport(Lack1SearchDetailReportViewModel filter = null)
        {
            //Get All
            if (filter == null)
            {
                //Get All
                var data = _lack1Bll.GetDetailReportByParam(new Lack1GetDetailReportByParamInput() { 
                    ListNppbkc = CurrentUser.ListUserNppbkc,
                    ListUserPlant = CurrentUser.ListUserPlants,
                    UserRole = CurrentUser.UserRole,
                    PeriodMonthFrom = DateTime.Now.Month,
                    PeriodMonthTo = DateTime.Now.Month,
                    PeriodYearFrom = DateTime.Now.Year,
                    PeriodYearTo = DateTime.Now.Year
                });
                return Mapper.Map<List<Lack1DetailReportItemModel>>(data);
            }
            //getbyparams
            var input = Mapper.Map<Lack1GetDetailReportByParamInput>(filter);
            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;
            input.UserRole = CurrentUser.UserRole;

            if (!string.IsNullOrEmpty(filter.PeriodFrom))
            {
                var strList = filter.PeriodFrom.Split('-').ToList();
                input.PeriodMonthFrom = Convert.ToInt32(strList[0]);
                input.PeriodYearFrom = Convert.ToInt32(strList[1]);
            }

            if (!string.IsNullOrEmpty(filter.PeriodTo))
            {
                var strList = filter.PeriodTo.Split('-').ToList();
                input.PeriodMonthTo = Convert.ToInt32(strList[0]);
                input.PeriodYearTo = Convert.ToInt32(strList[1]);
            }

            var dbData = _lack1Bll.GetDetailReportByParam(input);
            
            return Mapper.Map<List<Lack1DetailReportItemModel>>(dbData);
        }

        private Lack1DetailReportViewModel InitSearchDetilReportViewModel(Lack1DetailReportViewModel model)
        {
            var comp = GlobalFunctions.GetCompanyList(_companyBll);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var userComp = _userPlantBll.GetCompanyByUserId(CurrentUser.USER_ID);
                var poaComp = _poaMapBll.GetCompanyByPoaId(CurrentUser.USER_ID);
                var distinctComp = comp.Where(x => userComp.Contains(x.Value));
                if (CurrentUser.UserRole == Enums.UserRole.POA) distinctComp = comp.Where(x => poaComp.Contains(x.Value));
                var getComp = new SelectList(distinctComp, "Value", "Text");
                comp = getComp;
            }

            model.SearchView = new Lack1SearchDetailReportViewModel()
            {
                CompanyCodeList = comp
            };
            model.SearchView.NppbkcIdList = GetNppbkcIdListOnLack1Data(model.SearchView.CompanyCode);
            model.SearchView.ReceivingPlantIdList = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.SearchView.NppbkcId);
            model.SearchView.ExcisableGoodsTypeList = GetExciseGoodsTypeList(model.SearchView.NppbkcId);
            model.SearchView.SupplierPlantIdList = GetSupplierPlantListByParam(model.SearchView.NppbkcId, model.SearchView.ExcisableGoodsType);
            model.SearchView.PeriodFromList = GetPeriodList();
            model.SearchView.PeriodToList = GetPeriodList();
            model.SearchView.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchView.PeriodFrom = string.Format("{0}-{1}", DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());
            model.SearchView.PeriodTo = string.Format("{0}-{1}", DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString());
            return model;
        }

        private SelectList GetPeriodList()
        {
            var yearList = _lack1Bll.GetYearList();
            var monthList = _monthBll.GetAll();
            var selectListSource = new List<SelectItemModel>();
            foreach (int t1 in yearList)
            {
                selectListSource.AddRange(monthList.Select(t => new SelectItemModel()
                {
                    ValueField = new DateTime(t1, t.MONTH_ID, 1).ToString("MM-yyyy"),
                    TextField = new DateTime(t1, t.MONTH_ID, 1).ToString("MM.yyyy")
                }));
            }
            return new SelectList(selectListSource, "ValueField", "TextField");
        }

        public void ExportDetailReport(Lack1DetailReportViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsDetailReport(model.ExportSearchView);
            
            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsDetailReport(Lack1SearchDetailReportViewModel model)
        {
            var dataDetailReport = SearchDetailReport(model);
            
            var slDocument = new SLDocument();
            slDocument = CreateMaterialBalanceDetailReportXls(slDocument, dataDetailReport);
            slDocument = CreateProductionDetailReportXls(slDocument, dataDetailReport);
            int endColumnIndex;
            //create header
            slDocument = CreateHeaderExcel(slDocument, out endColumnIndex);
            
            int iRow = 3; //starting row data

            var needToMerge = new List<DetailReportNeedToMerge>();
            
            foreach (var item in dataDetailReport)
            {
                int iColumn = 1;
                
                if (item.TrackingConsolidations.Count > 0)
                {

                    var iStartRow = iRow;
                    var iEndRow = iStartRow;

                    var lastMaterialCode = item.TrackingConsolidations[0].MaterialCode;
                    var lastBatch = item.TrackingConsolidations[0].Batch;
                    var lastDate = item.TrackingConsolidations[0].GiDate;

                    int dataCount = item.TrackingConsolidations.Count - 1;

                    //first record
                    slDocument.SetCellValue(iRow, iColumn, item.Lack1Number);
                    slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Lack1LevelName);
                    slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.BeginingBalance.ToString("N2"));
                    slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].Ck5Number);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].Ck5TypeText);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].Ck5RegistrationNumber);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].Ck5RegistrationDate);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].Ck5GrDate);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].Qty.ToString("N2"));
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, (item.TrackingConsolidations[0].GiDate.HasValue ?  item.TrackingConsolidations[0].GiDate.Value.ToString("dd-MM-yyyy HHmmss") : string.Empty));
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].MaterialCode);
                    iColumn++;

                    if (string.IsNullOrEmpty(item.TrackingConsolidations[0].MaterialCode))
                    {
                        slDocument.SetCellValue(iRow, iColumn, string.Empty);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, string.Empty);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, string.Empty);
                        iColumn++;
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, !item.TrackingConsolidations[0].UsageQty.HasValue ? "-" : (item.TrackingConsolidations[0].UsageQty.Value).ToString("N3"));
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].OriginalUomDesc);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[0].ConvertedUomDesc);
                        iColumn++;
                    }
                    var prodcount = 0;
                    foreach (var prod in item.ProdQtyExcel)
                    {
                        slDocument.SetCellValue(iRow + prodcount, iColumn, prod);
                        prodcount ++;
                    }
                    //slDocument.SetCellValue(iRow, iColumn, item.ProdQty);
                    slDocument.MergeWorksheetCells(iRow + prodcount - 1, iColumn, (iRow + dataCount - prodcount), iColumn);//RowSpan sesuai dataCount
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EndingBalance.ToString("N2"));
                    slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Poa);
                    slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Creator);
                    slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                    
                    for (int i = 1; i < item.TrackingConsolidations.Count; i++)
                    {
                        iRow++;
                        iColumn = 4;

                        var curMaterialCode = item.TrackingConsolidations[i].MaterialCode;
                        var curBatch = item.TrackingConsolidations[i].Batch;
                        var curDate = item.TrackingConsolidations[i].GiDate;
                        
                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].Ck5Number);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].Ck5TypeText);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].Ck5RegistrationNumber);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].Ck5RegistrationDate);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].Ck5GrDate);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].Qty.ToString("N2"));
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, (item.TrackingConsolidations[i].GiDate.HasValue ? item.TrackingConsolidations[i].GiDate.Value.ToString("dd-MM-yyyy HHmmss") : string.Empty));
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].MaterialCode);
                        iColumn++;

                        if (string.IsNullOrEmpty(item.TrackingConsolidations[i].MaterialCode))
                        {
                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                        }
                        else
                        {
                            slDocument.SetCellValue(iRow, iColumn, !item.TrackingConsolidations[i].UsageQty.HasValue ? "-" : (item.TrackingConsolidations[i].UsageQty.Value).ToString("N3"));
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].OriginalUomDesc);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.TrackingConsolidations[i].ConvertedUomDesc);

                            if (item.TrackingConsolidations[i].Ck5TypeText !=
                                EnumHelper.GetDescription(Enums.CK5Type.Manual))
                            {
                                if (lastMaterialCode == curMaterialCode && lastBatch == curBatch && lastDate == curDate)
                                {
                                    iEndRow = iRow;
                                    if (i == item.TrackingConsolidations.Count - 1)
                                    {
                                        if (iStartRow != iEndRow)
                                        {
                                            //need to merge
                                            needToMerge.Add(new DetailReportNeedToMerge()
                                            {
                                                StartRowIndex = iStartRow,
                                                EndRowIndex = iEndRow
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (iStartRow != iEndRow)
                                    {
                                        //need to merge
                                        needToMerge.Add(new DetailReportNeedToMerge()
                                        {
                                            StartRowIndex = iStartRow,
                                            EndRowIndex = iEndRow
                                        });
                                    }
                                    iStartRow = iRow;
                                    iEndRow = iStartRow;
                                }
                            }
                            
                        }
                        
                        lastMaterialCode = curMaterialCode;
                        lastBatch = curBatch;
                        lastDate = curDate;
                    }
                    
                }
                else
                {

                    slDocument.SetCellValue(iRow, iColumn, item.Lack1Number);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Lack1LevelName);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.BeginingBalance.ToString("N2"));
                    iColumn++;

                    for (int i = 0; i < 11; i++)
                    {
                        slDocument.SetCellValue(iRow, iColumn, "-");
                        iColumn++;
                    }

                    slDocument.SetCellValue(iRow, iColumn, item.ProdQty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EndingBalance.ToString("N2"));
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Poa);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Creator);
                    
                }
                iRow++;
            }

            slDocument = DetailReportDoingMerge(slDocument, needToMerge);
            
            return CreateXlsFileDetailReports(slDocument, endColumnIndex,  iRow - 1);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;

            //first row
            slDocument.SetCellValue(1, iColumn, "LACK-1 Number");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "LACK-1 Level");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Begining Balance");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Receiving");
            slDocument.MergeWorksheetCells(1, iColumn, 1, (iColumn + 5));//ColSpan = 5
            iColumn = iColumn + 6;

            slDocument.SetCellValue(1, iColumn, "Usage");
            slDocument.MergeWorksheetCells(1, iColumn, 1, (iColumn + 4)); //ColSpan = 5
            iColumn = iColumn + 5;

            slDocument.SetCellValue(1, iColumn, "Production Qty");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Ending Balance");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "POA");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Creator");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan 2
            iColumn = iColumn + 1;
            
            endColumnIndex = iColumn;

            //second row
            iColumn = 4;
            slDocument.SetCellValue(2, iColumn, "CK-5 Number");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "CK-5 Type");
            iColumn++;
            
            slDocument.SetCellValue(2, iColumn, "CK-5 Registration Number");
            iColumn++;
            
            slDocument.SetCellValue(2, iColumn, "CK-5 Registration Date");
            iColumn++;
            
            slDocument.SetCellValue(2, iColumn, "CK-5 GR Date");
            iColumn++;
            
            slDocument.SetCellValue(2, iColumn, "Qty");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "GI Date");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Material Code");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Usage Qty");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Original Uom");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Converted Uom");

            return slDocument;

        }

        private string CreateXlsFileDetailReports(SLDocument slDocument, int endColumnIndex, int endRowIndex)
        {

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Alignment.Vertical = VerticalAlignmentValues.Center;

            //set header style
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //set border to value cell
            slDocument.SetCellStyle(3, 1, endRowIndex, endColumnIndex - 1, valueStyle);

            //set header style
            slDocument.SetCellStyle(1, 1, 2, endColumnIndex - 1, headerStyle);

            //set auto fit to all column
            slDocument.AutoFitColumn(1, endColumnIndex - 1);

            


            var fileName = "lack1_detreport" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.Lack1UploadFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        private SLDocument CreateProductionDetailReportXls(SLDocument slDocument, List<Lack1DetailReportItemModel> data)
        {
            var sheetName = "Production Breakdown";
            slDocument.AddWorksheet(sheetName);
            slDocument.SelectWorksheet(sheetName);

            int iColumn = 1;

            //first row
            slDocument.SetCellValue(1, iColumn, "LACK-1 Number");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            //slDocument.SetCellValue(1, iColumn, "Plant Id");
            //slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            //iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "FA Code");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Product Code");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Process Order");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Qty");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Uom");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Is Tis to Tis");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;


            iColumn = 1;
            int iRow = 2; //starting row data
            foreach (var item in data)
            {
                int dataCount = item.TrackingSaldoAwal.Count - 1;

                //first record
                slDocument.SetCellValue(iRow, iColumn, item.Lack1Number);
                slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                iColumn++;


                foreach (var productionData in item.ProductionBreakdown)
                {
                    iColumn = 2;
                    //slDocument.SetCellValue(iRow, iColumn, item.p);
                    slDocument.SetCellValue(iRow, iColumn, productionData.FA_CODE);
                    iColumn ++;
                    
                    slDocument.SetCellValue(iRow, iColumn, productionData.PRODUCT_ALIAS);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, productionData.ORDR);
                    iColumn++;
                    
                    slDocument.SetCellValue(iRow, iColumn, productionData.AMOUNT);
                    iColumn++;
                    
                    slDocument.SetCellValue(iRow, iColumn, productionData.UOM_ID);
                    iColumn++;
                    
                    slDocument.SetCellValue(iRow, iColumn, productionData.IS_TISTOTIS_DATA ? "Yes" : "No");


                    iRow++;
                }

            }

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Alignment.Vertical = VerticalAlignmentValues.Top;

            //set header style
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //set border to value cell
            slDocument.SetCellStyle(2, 1, iRow, 7, valueStyle);

            //set header style
            slDocument.SetCellStyle(1, 1, 1, 7, headerStyle);

            //set auto fit to all column
            slDocument.AutoFitColumn(1, 7);

            slDocument.SelectWorksheet("Sheet1");
            return slDocument;
        }
        private SLDocument CreateMaterialBalanceDetailReportXls(SLDocument slDocument, List<Lack1DetailReportItemModel> data)
        {
            var sheetName = "Begining Balance Breakdown";
            slDocument.AddWorksheet(sheetName);
            slDocument.SelectWorksheet(sheetName);

            int iColumn = 1;

            //first row
            slDocument.SetCellValue(1, iColumn, "LACK-1 Number");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Period Month");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Period Year");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Plant ID");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Material Id");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            

            slDocument.SetCellValue(1, iColumn, "Open Balance");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            iColumn = 1;
            int iRow = 2; //starting row data
            foreach (var item in data)
            {
                int dataCount = item.TrackingSaldoAwal.Count - 1;

                //first record
                slDocument.SetCellValue(iRow, iColumn, item.Lack1Number);
                slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + dataCount), iColumn);//RowSpan sesuai dataCount
                iColumn++;


                foreach (var materialBalance in item.TrackingSaldoAwal)
                {
                    
                    slDocument.SetCellValue(iRow, iColumn, materialBalance.PERIOD_MONTH);
                    slDocument.SetCellValue(iRow, iColumn + 1, materialBalance.PERIOD_YEAR);
                    slDocument.SetCellValue(iRow, iColumn + 2, materialBalance.WERKS);
                    slDocument.SetCellValue(iRow, iColumn + 3, materialBalance.MATERIAL_ID);
                    slDocument.SetCellValue(iRow, iColumn + 4, materialBalance.OPEN_BALANCE);
                    

                    iRow++;
                }
                
            }

            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Alignment.Vertical = VerticalAlignmentValues.Top;

            //set header style
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //set border to value cell
            slDocument.SetCellStyle(2, 1, iRow, 6, valueStyle);

            //set header style
            slDocument.SetCellStyle(1, 1, 1, 6, headerStyle);

            //set auto fit to all column
            slDocument.AutoFitColumn(1, 6);

            slDocument.SelectWorksheet("Sheet1");
            return slDocument;
        }

        private SLDocument DetailReportDoingMerge(SLDocument slDocument, List<DetailReportNeedToMerge> items)
        {
            if (items.Count <= 0) return slDocument;

            foreach (var item in items)
            {
                //need set to empty cell first before doing merge
                for (int i = item.StartRowIndex + 1; i < item.EndRowIndex; i++)
                {
                    slDocument.SetCellValue(i, 12, string.Empty);
                    slDocument.SetCellValue(i, 13, string.Empty);
                    slDocument.SetCellValue(i, 14, string.Empty);
                }

                //Usage Qty
                slDocument.MergeWorksheetCells(item.StartRowIndex, 12, item.EndRowIndex, 12);

                //Original UOM
                slDocument.MergeWorksheetCells(item.StartRowIndex, 13, item.EndRowIndex, 13);

                //Converted UOM
                slDocument.MergeWorksheetCells(item.StartRowIndex, 14, item.EndRowIndex, 14);

            }

            return slDocument;
        }

        private class DetailReportNeedToMerge
        {
            public int StartRowIndex { get; set; }
            public int EndRowIndex { get; set; }
        }
        
        #endregion

        #region ----------------- Dashboard Page -------------

        public ActionResult Dashboard()
        {
            var model = new Lack1DashboardViewModel
            {
                SearchViewModel = new Lack1DashboardSearchViewModel()
            };
            model = InitSelectListDashboardViewModel(model);
            model = InitDashboardViewModel(model);
            return View("Dashboard", model);
        }

        private Lack1DashboardViewModel InitSelectListDashboardViewModel(Lack1DashboardViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.SearchViewModel.UserList = GlobalFunctions.GetCreatorList();
            model.SearchViewModel.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.SearchViewModel.YearList = GetDashboardYear();
            model.SearchViewModel.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            return model;
        }

        private Lack1DashboardViewModel InitDashboardViewModel(Lack1DashboardViewModel model)
        {
            var data = GetDashboardData(model.SearchViewModel);
            if (data.Count == 0) return model;

            model.Detail = new DashboardDetilModel
            {
                //first code when manager exists
                //WaitingForAppTotal = data.Count(x => x.Status == Enums.DocumentStatus.WaitingForApproval || x.Status == Enums.DocumentStatus.WaitingForApprovalManager),
                DraftTotal = data.Count(x => x.Status == Enums.DocumentStatus.Draft),
                WaitingForPoaTotal = data.Count(x => x.Status == Enums.DocumentStatus.WaitingForApproval),
                //first code when manager exists
                //WaitingForManagerTotal =
                //    data.Count(x => x.Status == Enums.DocumentStatus.WaitingForApprovalManager),
                WaitingForGovTotal = data.Count(x => x.Status == Enums.DocumentStatus.WaitingGovApproval),
                CompletedTotal = data.Count(x => x.Status == Enums.DocumentStatus.Completed)
            };

            return model;
        }

        private List<Lack1Dto> GetDashboardData(Lack1DashboardSearchViewModel filter = null)
        {
            if (filter == null)
            {
                //get All Data
                var data = _lack1Bll.GetDashboardDataByParam(new Lack1GetDashboardDataByParamInput());
                return data;
            }

            var input = Mapper.Map<Lack1GetDashboardDataByParamInput>(filter);
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;

            return _lack1Bll.GetDashboardDataByParam(input);
        }

        private SelectList GetDashboardYear()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }

        [HttpPost]
        public PartialViewResult FilterDashboardPage(Lack1DashboardViewModel model)
        {
            var data = InitDashboardViewModel(model);
            return PartialView("_ChartStatus", data.Detail);
        }

        #endregion


        #region ----------------- Reconciliation Page -------------

        public ActionResult Reconciliation()
        {

            Lack1ReconciliationModel model;
            try
            {
                model = new Lack1ReconciliationModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    Detail = SearchReconciliation()
                };
                model = InitSearchReconciliationModel(model);
            }
            catch (Exception ex)
            {
                model = new Lack1ReconciliationModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return View("Reconciliation", model);
        }

        private Lack1ReconciliationModel InitSearchReconciliationModel(Lack1ReconciliationModel model)
        {
            var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                nppbkc = new SelectList(filterNppbkc, "Value", "Text");
            }

            model.SearchView = new Lack1SearchReconciliationModel();
            model.SearchView.NppbkcIdList = nppbkc;
            model.SearchView.PlantIdList = GlobalFunctions.GetPlantByNppbkcId(_plantBll, model.SearchView.NppbkcId);
            model.SearchView.ExGoodTypeList = GetExciseGoodsTypeList(model.SearchView.NppbkcId);
            return model;
        }

        private List<DataReconciliation> SearchReconciliation(Lack1SearchReconciliationModel filter = null)
        {
            //Get All
            if (filter == null)
            {
                //Get All
                var data = _lack1Bll.GetReconciliationByParam(new Lack1GetReconciliationByParamInput() { 
                    ListNppbkc = CurrentUser.ListUserNppbkc,
                    ListUserPlant = CurrentUser.ListUserPlants,
                    UserRole = CurrentUser.UserRole
                });
                return Mapper.Map<List<DataReconciliation>>(data);
            }
            //getbyparams
            var input = Mapper.Map<Lack1GetReconciliationByParamInput>(filter);
            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;
            input.UserRole = CurrentUser.UserRole;

            var dbData = _lack1Bll.GetReconciliationByParam(input);
            return Mapper.Map<List<DataReconciliation>>(dbData);
        }

        [HttpPost]
        public ActionResult SearchReconciliation(Lack1ReconciliationModel model)
        {
            model.Detail = SearchReconciliation(model.SearchView);
            return PartialView("_Lack1Reconciliation", model);
        }

        public void ExportReconciliation(Lack1ReconciliationModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsReconciliation(model.ExportSearchView);

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsReconciliation(Lack1SearchReconciliationModel model)
        {
            var dataReconciliation = SearchReconciliation(model);

            var slDocument = new SLDocument();
            int endColumnIndex;
            //create header
            slDocument = CreateHeaderExcelReconciliation(slDocument, out endColumnIndex);

            int iRow = 3; //starting row data
            int iColumn = 1;
            foreach (var data in dataReconciliation)
            {
                iColumn = 1;

                slDocument.SetCellValue(iRow, iColumn, data.NppbkcId);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.PlantId);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Year);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Month);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Date);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ItemCode);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.FinishGoodCode);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Remaining);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.BeginningStock);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ReceivedCk5No);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Received);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.UsageOther);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.UsageSelf);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ResultTis);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ResultStick);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.EndingStock);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.RemarkDesc);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.RemarkCk5No);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.RemarkQty);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.StickProd);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.PackProd);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Wip);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.RejectMaker);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.RejectPacker);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.FloorSweep);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Stem);
                iColumn = iColumn + 1;

                iRow++;
            }

            return CreateXlsFileReconciliation(slDocument, endColumnIndex, iRow - 1);

        }

        private string CreateXlsFileReconciliation(SLDocument slDocument, int endColumnIndex, int endRowIndex)
        {

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Alignment.Vertical = VerticalAlignmentValues.Center;
            valueStyle.SetWrapText(true);

            //set header style
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //set border to value cell
            slDocument.SetCellStyle(3, 1, endRowIndex, endColumnIndex, valueStyle);

            //set header style
            slDocument.SetCellStyle(1, 1, 2, endColumnIndex, headerStyle);

            //set auto fit to all column
            slDocument.AutoFitColumn(1, endColumnIndex);

            var fileName = "lack1_reconciliation" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.Lack1UploadFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        private SLDocument CreateHeaderExcelReconciliation(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;

            //first row
            slDocument.SetCellValue(1, iColumn, "NPPBKC");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Plant ID");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Year");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Month");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Date");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Item Code");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Finish Goods Code");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Remaining (gr)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Beginning Stock (gr)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "CK-5 No");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Received (gr)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Usage (gr)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, (iColumn + 1));//ColSpan = 2
            iColumn = iColumn + 2;

            slDocument.SetCellValue(1, iColumn, "Production Result");
            slDocument.MergeWorksheetCells(1, iColumn, 1, (iColumn + 1)); //ColSpan = 2
            iColumn = iColumn + 2;

            slDocument.SetCellValue(1, iColumn, "Ending Stock (gr)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Remark");
            slDocument.MergeWorksheetCells(1, iColumn, 1, (iColumn + 2)); //ColSpan = 3
            iColumn = iColumn + 3;

            slDocument.SetCellValue(1, iColumn, "Stick Produced (sticks)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Pack Produced (sticks)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "WIP (sticks)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Reject Maker (sticks)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Reject Packer (sticks)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Floor sweep (gr)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Stem (gr)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan 2

            endColumnIndex = iColumn;

            //second row
            iColumn = 10;
            slDocument.SetCellValue(2, iColumn, "Supplied by other");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Self Supplied");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "TIS (gr)");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Stick (gr)");
            iColumn++;

            iColumn = 15;
            slDocument.SetCellValue(2, iColumn, "Descr");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "CK-5 no.");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Qty (gr)");
            iColumn++;

            return slDocument;

        }

        #endregion


        #region --------------- Detail TIS -------------

        public ActionResult DetailTis()
        {

            Lack1DetailTisViewModel model;
            try
            {
                model = new Lack1DetailTisViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    DetailList = SearchDetailTis()
                };
                model = InitSearchDetilTisViewModel(model);
            }
            catch (Exception ex)
            {
                model = new Lack1DetailTisViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return View("DetailTis", model);
        }

        [HttpPost]
        public ActionResult SearchDetailTis(Lack1DetailTisViewModel model)
        {
            model.DetailList = SearchDetailTis(model.SearchView);
            return PartialView("_Lack1DetailTis", model);
        }

        private List<Lack1DetailTisItemModel> SearchDetailTis(Lack1SearchDetailTisViewModel filter = null)
        {
            //Get All
            if (filter == null)
            {
                //Get All
                var data = _lack1Bll.GetDetailTisByParam(new Lack1GetDetailTisByParamInput() { 
                    PlantReceiverFrom = string.Empty,
                    PlantReceiverTo = string.Empty,
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now
                });
                return Mapper.Map<List<Lack1DetailTisItemModel>>(data);
            }
            //getbyparams
            var input = Mapper.Map<Lack1GetDetailTisByParamInput>(filter);

            var dbData = _lack1Bll.GetDetailTisByParam(input);
            return Mapper.Map<List<Lack1DetailTisItemModel>>(dbData);
        }

        private Lack1DetailTisViewModel InitSearchDetilTisViewModel(Lack1DetailTisViewModel model)
        {
            var plantList = GlobalFunctions.GetPlantAll();

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var distinctPlant = plantList.Where(x => CurrentUser.ListUserPlants.Contains(x.Value));
                var getPlant = new SelectList(distinctPlant, "Value", "Text");
                plantList = getPlant;
            }

            model.SearchView.PlantReceiverFromList = plantList;
            model.SearchView.PlantReceiverToList = plantList;

            return model;
        }

        public void ExportDetailTis(Lack1DetailTisViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsDetailTis(model.ExportSearchView);

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsDetailTis(Lack1SearchDetailTisViewModel model)
        {
            var dataDetailTis = SearchDetailTis(model);

            var slDocument = new SLDocument();

            //create filter
            slDocument.SetCellValue(1, 1, "Date From");
            slDocument.SetCellValue(1, 2, ": " + model.DateFrom.Value.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(2, 1, "Date To");
            slDocument.SetCellValue(2, 2, ": " + model.DateTo.Value.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(3, 1, "Plant Receiver From");
            slDocument.SetCellValue(3, 2, ": " + model.PlantReceiverFrom);

            slDocument.SetCellValue(4, 1, "Plant Receiver To");
            slDocument.SetCellValue(4, 2, ": " + model.PlantReceiverTo);

            //title
            slDocument.SetCellValue(5, 1, "Detail TIS");
            slDocument.MergeWorksheetCells(5, 1, 5, 25);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(5, 1, valueStyle);


            //create header
            slDocument = CreateHeaderExcelDetailTis(slDocument);

            int iRow = 7; //starting row data
            int iColumn = 1;

            foreach (var item in dataDetailTis)
            {
                if (item.UsageList.Count > 0)
                {
                    for (int i = 0; i < item.UsageList.Count - 1; i++)
                    {
                        iColumn = 1;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantIdReceiver);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantDescReceiver);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantIdSupplier);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantDescSupplier);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.CfCode);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.CfDesc);
                        iColumn++;

                        if (string.IsNullOrEmpty(item.BeginingBalance))
                        {
                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;
                        }
                        else
                        {
                            slDocument.SetCellValue(iRow, iColumn, Convert.ToDecimal(item.BeginingBalance));
                            iColumn++;
                        }

                        slDocument.SetCellValue(iRow, iColumn, item.BeginingBalanceUom);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5EmsNo);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5RegNo);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5RegDate);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5GrDate);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5Qty);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.MvtTypeList[i]);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, Convert.ToDecimal(item.UsageList[i]));
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, "Gram");
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.UsagePostingDateList[i]);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.FaCode);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.FaCodeDesc);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.ProdQty);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.ProdUom);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.ProdPostingDate);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.ProdDate);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.EndingBalance);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.EndingBalanceUom);
                        iColumn++;

                        iRow++;
                    }
                }
                else
                {
                    iColumn = 1;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantIdReceiver);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantDescReceiver);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantIdSupplier);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantDescSupplier);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.CfCode);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.CfDesc);
                    iColumn++;

                    if (string.IsNullOrEmpty(item.BeginingBalance))
                    {
                        slDocument.SetCellValue(iRow, iColumn, string.Empty);
                        iColumn++;
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, Convert.ToDecimal(item.BeginingBalance));
                        iColumn++;
                    }
                    

                    slDocument.SetCellValue(iRow, iColumn, item.BeginingBalanceUom);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5EmsNo);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5RegNo);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5RegDate);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5GrDate);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5Qty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FaCode);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FaCodeDesc);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ProdQty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ProdUom);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ProdPostingDate);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ProdDate);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EndingBalance);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EndingBalanceUom);
                    iColumn++;

                    iRow++;
                }
            }

            return CreateXlsFileDetailTis(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelDetailTis(SLDocument slDocument)
        {
            int iColumn = 1;
            int iRow = 6;

            slDocument.SetCellValue(iRow, iColumn, "Plant ID Receiver");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Desc Receiver");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Supplier ID");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Supplier Description");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Description");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Beginning Balance 1st Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Beginning Balance UoM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 EMS No");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 Reg No");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 Reg Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 GR Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Mvt Type");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Usage");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Usage UOM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Usage Posting Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "FA Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "FA Code Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Production Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Production UOM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Production Posting Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Production Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Projected Ending Balance");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Projected Ending Balance UOM");
            iColumn = iColumn + 1;

            return slDocument;

        }

        private string CreateXlsFileDetailTis(SLDocument slDocument, int iColumn, int iRow)
        {

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.SetWrapText(true);

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(7, 1, iRow - 1, iColumn - 1, valueStyle);

            slDocument.SetCellStyle(6, 1, 6, iColumn - 1, headerStyle);

            SLStyle numericStyle = slDocument.CreateStyle();
            numericStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            slDocument.SetCellStyle(7, 6, iRow - 1, 6 - 1, numericStyle);
            slDocument.SetCellStyle(7, 8, iRow - 1, 8 - 1, numericStyle);


            var fileName = "lack1_detailTis_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.Lack1UploadFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        #endregion

        #region Daily Prod

        public ActionResult DailyProd()
        {

            Lack1DailyProdViewModel model;
            try
            {
                model = new Lack1DailyProdViewModel();

                model = InitLack1DailyProd(model);
            }
            catch (Exception ex)
            {
                model = new Lack1DailyProdViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return View("DailyProd", model);
        }

        private Lack1DailyProdViewModel InitLack1DailyProd(Lack1DailyProdViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

        
            model.SearchView.DateFrom = DateTime.Now;
            model.SearchView.DateTo = DateTime.Now;
            model.SearchView.PlantFromList = GlobalFunctions.GetPlantAll();
            model.SearchView.PlantToList = GlobalFunctions.GetPlantAll();

            return model;
        }

        private List<Lack1DailyProdDetail> SearchDailyProd(Lack1SearchDailyProdViewModel filter)
        {

            var input = new Lack1GetDailyProdByParamInput();

            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;
            input.UserRole = CurrentUser.UserRole;

            input.DateFrom = filter.DateFrom;
            input.DateTo = filter.DateTo;
            input.PlantFrom = filter.PlantFrom;
            input.PlantTo = filter.PlantTo;

            var dbData = _lack1Bll.GetDailyProdByParam(input);
            return Mapper.Map<List<Lack1DailyProdDetail>>(dbData);
            
        }

        [HttpPost]
        public ActionResult SearchDailyProd(Lack1DailyProdViewModel model)
        {
            model.Detail = SearchDailyProd(model.SearchView);

            return PartialView("_Lack1DailyProdDetails", model);
        }

        public void ExportDailyProd(Lack1DailyProdViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsDailyProd(model.ExportSearchView);

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsDailyProd(Lack1SearchDailyProdViewModel model)
        {
            var dataDailyProd = SearchDailyProd(model);

            var slDocument = new SLDocument();

            //create filter
            slDocument.SetCellValue(1, 1, "Posting Date From");
            slDocument.SetCellValue(1, 2, ": " + model.DateFrom.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(2, 1, "Posting Date To");
            slDocument.SetCellValue(2, 2, ": " + model.DateTo.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(3, 1, "Plant From");
            slDocument.SetCellValue(3, 2, ": " + model.PlantFrom);

            slDocument.SetCellValue(4, 1, "Plant To");
            slDocument.SetCellValue(4, 2, ": " + model.PlantTo);

            //title
            slDocument.SetCellValue(5, 1, "Daily Production");
            slDocument.MergeWorksheetCells(5, 1, 5, 12);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(5, 1, valueStyle);
           

            //create header
            slDocument = CreateHeaderExcelDailyProd(slDocument);

            int iRow = 7; //starting row data
            int iColumn = 1;
            foreach (var data in dataDailyProd)
            {
                iColumn = 1;

                slDocument.SetCellValue(iRow, iColumn, data.PlantId);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.PlantDescription);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.FaCode);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.FaCodeDescription);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ProductionDate);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, ConvertHelper.ConvertToDecimalOrZero(data.ProdQty));
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ProdUom);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, ConvertHelper.ConvertToDecimalOrZero(data.RejectParkerQty));
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.RejectParkerUom);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Zb);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.ZbUom);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.SapPackedQty);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.SapPackedUom);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.PackedAdjusted);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.PackedAdjustedUom);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.Remark);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.SapReversalQty);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.SapReversalQtyUom);
                iColumn = iColumn + 1;	


                iRow++;
            }

            return CreateXlsFileDailyProd(slDocument, iColumn, iRow);

        }

        private string CreateXlsFileDailyProd(SLDocument slDocument, int iColumn, int iRow)
        {

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            //valueStyle.Alignment.Vertical = VerticalAlignmentValues.Center;
            //valueStyle.SetWrapText(true);
            //set header style

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            //slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(7, 1, iRow - 1, iColumn - 1, valueStyle);

            slDocument.SetCellStyle(6, 1, 6, iColumn - 1, headerStyle);

            SLStyle numericStyle = slDocument.CreateStyle();
            numericStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Alignment.Horizontal = HorizontalAlignmentValues.Right;
            numericStyle.FormatCode = "#,##0.00";
            slDocument.SetCellStyle(7, 6, iRow - 1, 6 - 1, numericStyle);
            slDocument.SetCellStyle(7, 8, iRow - 1, 8 - 1, numericStyle);


            slDocument.AutoFitColumn(1, iColumn - 1);

            var fileName = "lack1_dailyprod" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.Lack1UploadFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        private SLDocument CreateHeaderExcelDailyProd(SLDocument slDocument)
        {
            int iColumn = 1;
            int iRow = 6;

            slDocument.SetCellValue(iRow, iColumn, "PlantId");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "FA Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "FA Code Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Production Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Prod Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Prod UOM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Reject Packer Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Reject Packer UOM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "ZB Prod Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "ZB Prod Qty UOM");
            iColumn = iColumn + 1;
            	
            slDocument.SetCellValue(iRow, iColumn, "SAP Prod Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "SAP Prod Qty UOM (101)");
            iColumn = iColumn + 1;	
            	
            slDocument.SetCellValue(iRow, iColumn, "Adjusted SAP Prod Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Adjusted SAP Prod Qty UOM");
            iColumn = iColumn + 1;		
            	
            slDocument.SetCellValue(iRow, iColumn, "Remark for SAP Adjusted");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "SAP Reversal (102) Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "SAP Reversal Qty (102) UOM");
            iColumn = iColumn + 1;	
            


            return slDocument;

        }

        #endregion

        #region Primary Results

        public ActionResult PrimaryResults()
        {

            Lack1PrimaryResultsViewModel model;
            try
            {
                model = new Lack1PrimaryResultsViewModel();

                model = InitLack1PrimaryResults(model);
            }
            catch (Exception ex)
            {
                model = new Lack1PrimaryResultsViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return View("PrimaryResults", model);
        }

        

        private Lack1PrimaryResultsViewModel InitLack1PrimaryResults(Lack1PrimaryResultsViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;


            model.SearchView.DateFrom = DateTime.Now;
            model.SearchView.DateTo = DateTime.Now;
            model.SearchView.PlantFromList = GlobalFunctions.GetPlantAll();
            model.SearchView.PlantToList = GlobalFunctions.GetPlantAll();

            return model;
        }

        private List<Lack1PrimaryResultsDetail> SearchPrimaryResults(Lack1SearchPrimaryResultsViewModel filter)
        {

            var input = new Lack1GetPrimaryResultsByParamInput();

            input.ListNppbkc = CurrentUser.ListUserNppbkc;
            input.ListUserPlant = CurrentUser.ListUserPlants;
            input.UserRole = CurrentUser.UserRole;

            input.DateFrom = filter.DateFrom;
            input.DateTo = filter.DateTo;
            input.PlantFrom = filter.PlantFrom;
            input.PlantTo = filter.PlantTo;

            var dbData = _lack1Bll.GetPrimaryResultsByParam(input);
            return Mapper.Map<List<Lack1PrimaryResultsDetail>>(dbData);
        }

        [HttpPost]
        public ActionResult SearchPrimaryResults(Lack1PrimaryResultsViewModel model)
        {
            try
            {
                model.Detail = SearchPrimaryResults(model.SearchView);
                foreach (var lack1PrimaryResultsDetail in model.Detail)
                {
                    if (!string.IsNullOrEmpty(lack1PrimaryResultsDetail.Message))
                    {
                        model.ErrorMessage = "Error";
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
            }
            
            //model.Detail = SearchPrimaryResults(model.SearchView);
            return PartialView("_Lack1PrimaryResultsDetails", model);
        }

        public void ExportPrimaryResults(Lack1PrimaryResultsViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsPrimaryResults(model.ExportSearchView);

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsPrimaryResults(Lack1SearchPrimaryResultsViewModel model)
        {
            var dataPrimaryResults = SearchPrimaryResults(model);

            var slDocument = new SLDocument();

            //create filter
            slDocument.SetCellValue(1, 1, "Posting Date From");
            slDocument.SetCellValue(1, 2, ": " + model.DateFrom.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(2, 1, "Posting Date To");
            slDocument.SetCellValue(2, 2, ": " + model.DateTo.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(3, 1, "Plant From");
            slDocument.SetCellValue(3, 2, ": " + model.PlantFrom);

            slDocument.SetCellValue(4, 1, "Plant To");
            slDocument.SetCellValue(4, 2, ": " + model.PlantTo);

            //title
            slDocument.SetCellValue(5, 1, "Primary Results");
            slDocument.MergeWorksheetCells(5, 1, 5, 12);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(5, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelPrimaryResults(slDocument);

            int iRow = 7; //starting row data
            int iColumn = 1;
            foreach (var data in dataPrimaryResults)
            {
                iColumn = 1;

                slDocument.SetCellValue(iRow, iColumn, data.PlantId);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.PlantDescription);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.CfProducedProcessOrder);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.CfCodeProduced);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.CfProducedDescription);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.CfProdDate);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, ConvertHelper.ConvertToDecimalOrZero(data.CfProdQty));
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.CfProdUom);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.BkcUsed);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.BkcDescription);
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, ConvertHelper.ConvertToDecimalOrZero(data.BkcIssueQty));
                iColumn = iColumn + 1;

                slDocument.SetCellValue(iRow, iColumn, data.BkcIssueUom);
                iColumn = iColumn + 1;

                iRow++;
            }

            return CreateXlsFilePrimaryResults(slDocument, iColumn, iRow);

        }

        private string CreateXlsFilePrimaryResults(SLDocument slDocument, int iColumn, int iRow)
        {

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            
            //set header style
            
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

           
            slDocument.SetCellStyle(7, 1, iRow - 1, iColumn - 1, valueStyle);

            slDocument.SetCellStyle(6, 1, 6, iColumn - 1, headerStyle);

            SLStyle numericStyle = slDocument.CreateStyle();
            numericStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Alignment.Horizontal = HorizontalAlignmentValues.Right;
            numericStyle.FormatCode = "#,##0.00";

            slDocument.SetCellStyle(7, 7, iRow - 1, 7 - 1, numericStyle);
            slDocument.SetCellStyle(7, 11, iRow - 1, 11 - 1, numericStyle);

            slDocument.AutoFitColumn(1, iColumn - 1);

            var fileName = "lack1_primaryresults" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.Lack1UploadFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        private SLDocument CreateHeaderExcelPrimaryResults(SLDocument slDocument)
        {
            int iColumn = 1;
            int iRow = 6;

            slDocument.SetCellValue(iRow, iColumn, "PlantId");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Produced Process Order");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Code Produced");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Produced Description");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Prod Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Prod Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Produced UOM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "BKC Used");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "BKC Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "BKC Issue Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "BKC Issue UOM");
            iColumn = iColumn + 1;

            return slDocument;
        }

        #endregion

        #region CFVsFA
        [HttpPost]
        public JsonResult GenerateCFVsFaReport(Lack1CFUsageVsFAByParamInput param)
        {
            var outGeneratedData = _lack1Bll.GetCfUsagevsFaDetailData(param);

            var result = Json(outGeneratedData);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        public ActionResult CfVsFaDetails()
        {
            Lack1CFUsageVsFAViewModel model = new Lack1CFUsageVsFAViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                BeginingPostingDate = DateTime.Today,
                EndPostingDate = DateTime.Today,
                PlantIdList = GlobalFunctions.GetPlantByListUserPlant(CurrentUser.ListUserPlants)
            };
            if (CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                model.PlantIdList = GlobalFunctions.GetPlantAll();
            }
            return View("CfVsFaDetails", model);
        }

        public ActionResult CfVsFaSummary()
        {
            Lack1CFUsageVsFAViewModel model = new Lack1CFUsageVsFAViewModel()
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                BeginingPostingDate = DateTime.Today,
                EndPostingDate = DateTime.Today,
                PlantIdList = GlobalFunctions.GetPlantByListUserPlant(CurrentUser.ListUserPlants)
            };
            if (CurrentUser.UserRole == Enums.UserRole.Administrator)
            {
                model.PlantIdList = GlobalFunctions.GetPlantAll();
            }
            return View("CfVsFaSummary", model);
        }

        
        public ActionResult ExportCfVsFaDetails(Lack1CFUsageVsFAByParamInput param)
        {
            try
            {
                var data = _lack1Bll.GetCfUsagevsFaDetailData(param);
                var html = generateTableDataCsVsFA(data,param.IsSummary);
                ExportCfVsFaDetailsToExcel(html, param.IsSummary);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (param.IsSummary)
            {
                return RedirectToAction("CfVsFaSummary");
            }
            
                return RedirectToAction("CfVsFaDetails");
            
            
        }

        private void ExportCfVsFaDetailsToExcel(string html,bool isSummary)
        {
            var fileName = "LACK-1 Cf Vs Fa Detail" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            if (isSummary)
            {
                fileName = "LACK-1 Cf Vs Fa Summary" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            }
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //'Excel 2003 : "application/vnd.ms-excel"
            //'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"



            Response.Output.Write(html);

            Response.Flush();

            Response.End();
        }

        private string generateHeaderTableDataCsVsFA(bool isSummary) {
            var rc =  /*start header*/
                    "<thead>" +
                        "<tr>" +
                        "<th >PlantId</th>" +
                        "<th >Plant Desc</th>" +
                        "<th >Process Order</th>" +
                        "<th >FA Code</th>" +
                        "<th >Brand Desc</th>";
            if (isSummary)
            {
                rc += "<th >FA Produced Qty</th>" +
                        "<th >FA Produced Uom</th>" +
                        "<th >CF Code</th>" +
                        "<th >CF Description</th>";

            }
            else
            {
                rc += "<th >FA Prod Date</th>" +
                    "<th >FA Posting Date</th>" +
                    "<th >FA Produced Qty</th>" +
                    "<th >FA Produced Uom</th>" +
                    "<th >Mvt</th>" +
                    "<th >Batch</th>" +
                    "<th >CF Code</th>" +
                    "<th >CF Description</th>" +
                    "<th >CF Posting Date</th>";

            }

            rc += "<th >CF Issue Qty</th>" +
                    "<th >CF Issue Uom</th>" +

                    "<th >Reject Maker Qty</th>" +
                    "<th >Reject Maker UoM</th>" +
                    "<th >Reject Packer Qty</th>" +
                    "<th >Reject Packer UoM</th>" +
                    "<th >Dust Qty</th>" +
                    "<th >Dust Uom</th>" +
                    "<th >Floor Qty</th>" +
                    "<th >Floor Uom</th>" +
                    "<th >Stem Qty</th>" +
                    "<th >Stem Uom</th>";
            if (!isSummary) {
                rc += "<th >Waste Date</th>";
            }
    

            rc += "</tr>" +
                "</thead>";
            /*end of header*/
            return rc;
        }

        private string generateTableDataCsVsFA(List<Lack1CFUsagevsFaDetailDto> data, bool isSummary)
        {
            var rc = "<table border='1' >" + generateHeaderTableDataCsVsFA(isSummary);
            for (var i = 0; i < data.Count; i++) {
                var item = data[i];
                var row = generateRowDataCsVsFa(item, isSummary);


                rc = rc + row;
            }
            return rc;
        }


        private string generateRowDataCsVsFa(Lack1CFUsagevsFaDetailDto item,bool isSummary) {

            var facode = item.Fa_Code;
            var plantId = item.PlantId;
            var order = item.Order;
            var plantDesc = item.PlantDesc;
            var brandDesc = item.Brand_Desc;
            var mvt101 = item.Lack1CFUsagevsFaDetailDtoMvt101;
            var mvt261 = item.Lack1CFUsagevsFaDetailDtoMvt261;
            var waste = item.Lack1CFUsagevsFaDetailDtoMvtWaste;
            var iswastedisplayed = false;
            var rc = "";
            for (var i = 0; i < mvt101.Count; i++)
            {
                rc += "<tr>" +
                    "<td>" + plantId + "</td>" +
                    "<td>" + plantDesc + "</td>" +
                    "<td>" + order + "</td>" +
                    "<td>" + facode + "</td>" +
                    "<td>" + brandDesc + "</td>";
                if (isSummary)
                {
                    rc += "<td>" + (mvt101[i].Converted_Qty) + "</td>" +
                        "<td>" + mvt101[i].Converted_Uom + "</td>";

                    if (mvt261.Count > 0)
                    {
                        if (mvt261[i] != null)
                        {
                            rc += "<td>" + mvt261[i].Material_Id + "</td>" +
                                "<td>" + mvt261[i].Material_Id + "</td>" +
                                "<td>" + (-1 * mvt261[i].Converted_Qty) + "</td>" +
                                "<td>" + mvt261[i].Uom + "</td>";
                        }
                        else
                        {
                            rc += "<td></td>" +
                                "<td></td>" +
                                "<td></td>" +
                                "<td></td>";
                        }
                    }
                    else
                    {
                        rc += "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>";
                    }


                }
                else
                {
                    rc += "<td>" + mvt101[i].ProductionDateText + "</td>" +
                        "<td>" + mvt101[i].PostingDateText + "</td>" +
                        "<td>" + (mvt101[i].Converted_Qty) + "</td>" +
                        "<td>" + mvt101[i].Converted_Uom + "</td>" +
                        "<td>" + mvt101[i].Mvt + "</td>" +
                        "<td>" + mvt101[i].Batch + "</td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>";
                    rc += "<td></td>" +
                            "<td></td>";
                }



                if (waste.Count - 1 >= i)
                {
                    rc += "<td>" + (waste[i].MarkerRejectStickQty) + "</td>" +
                        "<td>Btg</td>" +
                        "<td>" + (waste[i].PackerRejectStickQty) + "</td>" +
                        "<td>Btg</td>" +
                        "<td>" + (waste[i].DustWasteGramQty) + "</td>" +
                        "<td>G</td>" +
                        "<td>" + (waste[i].FloorWasteGramQty) + "</td>" +
                        "<td>G</td>" +
                        "<td>" + (waste[i].StampWasteQty) + "</td>" +
                        "<td>G</td>";
                    if (!isSummary)
                    {
                        rc += "<td>" + waste[i].WasteProductionDateText + "</td>";
                    }

                    rc += "</tr>";
                    iswastedisplayed = true;
                }
                else
                {
                    rc += "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>";
                    if (!isSummary)
                    {
                        rc += "<td></td>";
                    }

                    rc += "</tr>";
                }
            }

            if (!isSummary)
            {
                for (var j = 0; j < mvt261.Count; j++)
                {
                    rc += "<tr>" +
                        "<td>" + plantId + "</td>" +
                        "<td>" + plantDesc + "</td>" +
                        "<td>" + order + "</td>" +
                        "<td>" + facode + "</td>" +
                        "<td>" + brandDesc + "</td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td></td>" +
                        "<td>" + mvt261[j].Mvt + "</td>" +
                        "<td>" + mvt261[j].Batch + "</td>" +
                        "<td>" + mvt261[j].Material_Id + "</td>" +
                        "<td>" + mvt261[j].Material_Id + "</td>" +
                        "<td>" + mvt261[j].PostingDateText + "</td>" +
                        "<td>" + (-1 * mvt261[j].Converted_Qty) + "</td>" +
                        "<td>" + mvt261[j].Uom + "</td>";

                    rc += "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "<td></td>" +
                            "</tr>";
                }
            }


            return rc;
        }

        #endregion


        #region --------------- Detail EA -------------

        public ActionResult DetailEa()
        {

            Lack1DetailEaViewModel model;
            try
            {
                model = new Lack1DetailEaViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo,
                    DetailList = SearchDetailEa()
                };
                model = InitSearchDetilEaViewModel(model);
            }
            catch (Exception ex)
            {
                model = new Lack1DetailEaViewModel()
                {
                    MainMenu = _mainMenu,
                    CurrentMenu = PageInfo
                };
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return View("DetailEa", model);
        }

        [HttpPost]
        public ActionResult SearchDetailEa(Lack1DetailEaViewModel model)
        {
            model.DetailList = SearchDetailEa(model.SearchView);
            return PartialView("_Lack1DetailEa", model);
        }

        private List<Lack1DetailEaItemModel> SearchDetailEa(Lack1SearchDetailEaViewModel filter = null)
        {
            //Get All
            if (filter == null)
            {
                //Get All
                var data = _lack1Bll.GetDetailEaByParam(new Lack1GetDetailEaByParamInput()
                {
                    PlantReceiverFrom = string.Empty,
                    PlantReceiverTo = string.Empty,
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now
                });
                return Mapper.Map<List<Lack1DetailEaItemModel>>(data);
            }
            //getbyparams
            var input = Mapper.Map<Lack1GetDetailEaByParamInput>(filter);

            var dbData = _lack1Bll.GetDetailEaByParam(input);
            var mappedData = Mapper.Map<List<Lack1DetailEaItemModel>>(dbData);

            return mappedData;
        }

        private Lack1DetailEaViewModel InitSearchDetilEaViewModel(Lack1DetailEaViewModel model)
        {
            var plantList = GlobalFunctions.GetPlantAll();

            if (CurrentUser.UserRole != Enums.UserRole.Administrator)
            {
                var distinctPlant = plantList.Where(x => CurrentUser.ListUserPlants.Contains(x.Value));
                var getPlant = new SelectList(distinctPlant, "Value", "Text");
                plantList = getPlant;
            }

            model.SearchView.PlantReceiverFromList = plantList;
            model.SearchView.PlantReceiverToList = plantList;

            return model;
        }

        public void ExportDetailEa(Lack1DetailEaViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsDetailEa(model.ExportSearchView);

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsDetailEa(Lack1SearchDetailEaViewModel model)
        {
            var dataDetailEa = SearchDetailEa(model);

            var slDocument = new SLDocument();

            //create filter
            slDocument.SetCellValue(1, 1, "Date From");
            slDocument.SetCellValue(1, 2, ": " + model.DateFrom.Value.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(2, 1, "Date To");
            slDocument.SetCellValue(2, 2, ": " + model.DateTo.Value.ToString("dd-MMM-yyyy"));

            slDocument.SetCellValue(3, 1, "Plant Receiver From");
            slDocument.SetCellValue(3, 2, ": " + model.PlantReceiverFrom);

            slDocument.SetCellValue(4, 1, "Plant Receiver To");
            slDocument.SetCellValue(4, 2, ": " + model.PlantReceiverTo);

            //title
            slDocument.SetCellValue(5, 1, "Detail EA");
            slDocument.MergeWorksheetCells(5, 1, 5, 26);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(5, 1, valueStyle);


            //create header
            slDocument = CreateHeaderExcelDetailEa(slDocument);

            int iRow = 7; //starting row data
            int iColumn = 1;

            foreach (var item in dataDetailEa)
            {
                int levelCount = item.LevelList.Count - 1;
                int dataCount = levelCount;
                int usageCount = item.UsageList.Count - 1;

                if (levelCount < usageCount) dataCount = usageCount;

                if (item.UsageList.Count > 0)
                {
                    for (int i = 0; i < dataCount; i++)
                    {
                        iColumn = 1;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantIdReceiver);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantDescReceiver);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantIdSupplier);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.PlantDescSupplier);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.EaCode);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.EaDesc);
                        iColumn++;

                        if (string.IsNullOrEmpty(item.BeginingBalance))
                        {
                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;
                        }
                        else
                        {
                            slDocument.SetCellValue(iRow, iColumn, Convert.ToDecimal(item.BeginingBalance));
                            iColumn++;
                        }

                        slDocument.SetCellValue(iRow, iColumn, item.BeginingBalanceUom);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5EmsNo);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5RegNo);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5RegDate);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.Ck5Qty);
                        iColumn++;

                        if (i <= usageCount)
                        {
                            slDocument.SetCellValue(iRow, iColumn, Convert.ToDecimal(item.UsageList[i]));
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, "Liter");
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.UsagePostingDateList[i]);
                            iColumn++;
                        }
                        else
                        {
                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;
                        }

                        if (i <= levelCount)
                        {
                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].Level);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].FlavorCode);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].FlavorDesc);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].CfProdCode);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].CfProdDesc);
                            iColumn++;

                            if (string.IsNullOrEmpty(item.LevelList[i].CfProdQty))
                            {
                                slDocument.SetCellValue(iRow, iColumn, string.Empty);
                                iColumn++;
                            }
                            else
                            {
                                slDocument.SetCellValue(iRow, iColumn, Convert.ToDecimal(item.LevelList[i].CfProdQty));
                                iColumn++;
                            }

                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].CfProdUom);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].ProdPostingDate);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, item.LevelList[i].ProdDate);
                            iColumn++;
                        }
                        else
                        {
                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;

                            slDocument.SetCellValue(iRow, iColumn, string.Empty);
                            iColumn++;
                        }

                        slDocument.SetCellValue(iRow, iColumn, item.EndingBalance);
                        iColumn++;

                        slDocument.SetCellValue(iRow, iColumn, item.EndingBalanceUom);
                        iColumn++;

                        iRow++;
                    }
                }
                else
                {
                    iColumn = 1;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantIdReceiver);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantDescReceiver);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantIdSupplier);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PlantDescSupplier);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EaCode);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EaDesc);
                    iColumn++;

                    if (string.IsNullOrEmpty(item.BeginingBalance))
                    {
                        slDocument.SetCellValue(iRow, iColumn, string.Empty);
                        iColumn++;
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, Convert.ToDecimal(item.BeginingBalance));
                        iColumn++;
                    }

                    slDocument.SetCellValue(iRow, iColumn, item.BeginingBalanceUom);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5EmsNo);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5RegNo);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5RegDate);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Ck5Qty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, string.Empty);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EndingBalance);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.EndingBalanceUom);
                    iColumn++;

                    iRow++;
                }
            }

            return CreateXlsFileDetailEa(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelDetailEa(SLDocument slDocument)
        {
            int iColumn = 1;
            int iRow = 6;

            slDocument.SetCellValue(iRow, iColumn, "Plant ID Receiver");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Desc Receiver");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Supplier ID");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Plant Supplier Description");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "EA Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "EA Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Beginning Balance");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Beginning Balance UoM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 EMS No");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 Reg No");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 Reg Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CK-5 Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Usage");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Usage UOM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Usage Posting Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Level");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Flavor Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Flavor Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Produced Code");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Prod Desc");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Prod Qty");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "CF Prod UOM");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Production Posting Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Production Date");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Ending Balance");
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Ending Balance UOM");
            iColumn = iColumn + 1;

            return slDocument;

        }

        private string CreateXlsFileDetailEa(SLDocument slDocument, int iColumn, int iRow)
        {

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.SetWrapText(true);

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(7, 1, iRow - 1, iColumn - 1, valueStyle);

            slDocument.SetCellStyle(6, 1, 6, iColumn - 1, headerStyle);

            SLStyle numericStyle = slDocument.CreateStyle();
            numericStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            numericStyle.Alignment.Horizontal = HorizontalAlignmentValues.Right;

            slDocument.SetCellStyle(7, 6, iRow - 1, 6 - 1, numericStyle);
            slDocument.SetCellStyle(7, 8, iRow - 1, 8 - 1, numericStyle);


            var fileName = "lack1_detailEa_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.Lack1UploadFolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        #endregion
    }
}