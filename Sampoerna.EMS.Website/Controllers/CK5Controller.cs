using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using NLog.LayoutRenderers;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.ReportingData;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;
using Sampoerna.EMS.XMLReader;
using SpreadsheetLight;
using System.Data;

namespace Sampoerna.EMS.Website.Controllers
{
    public class CK5Controller : BaseController
    {
        private ICK5BLL _ck5Bll;
        private IPBCK1BLL _pbck1Bll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowBLL _workflowBll;
        private IPlantBLL _plantBll;
        private IPrintHistoryBLL _printHistoryBll;
        private IPOABLL _poabll;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IUnitOfMeasurementBLL _uomBll;
        private IMaterialBLL _materialBll;

        public CK5Controller(IPageBLL pageBLL, IUnitOfMeasurementBLL uomBll, IPOABLL poabll, IZaidmExNPPBKCBLL nppbckbll, ICK5BLL ck5Bll,  IPBCK1BLL pbckBll, 
            IWorkflowHistoryBLL workflowHistoryBll,IChangesHistoryBLL changesHistoryBll, IMaterialBLL materialBll,
            IWorkflowBLL workflowBll, IPlantBLL plantBll, IPrintHistoryBLL printHistoryBll)
            : base(pageBLL, Enums.MenuList.CK5)
        {
            _ck5Bll = ck5Bll;
            _pbck1Bll = pbckBll;
            _workflowHistoryBll = workflowHistoryBll;
            _changesHistoryBll = changesHistoryBll;
            _workflowBll = workflowBll;
            _plantBll = plantBll;
            _printHistoryBll = printHistoryBll;
            _poabll = poabll;
            _nppbkcbll = nppbckbll;
            _uomBll = uomBll;
            _materialBll = materialBll;
        }

        #region View Documents

        private List<CK5Item> GetCk5Items(Enums.CK5Type ck5Type, CK5SearchViewModel filter = null)
        {
            CK5GetByParamInput input;
            List<CK5Dto> dbData;
            if (filter == null)
            {
                //Get All
                //input = new CK5Input { Ck5Type = ck5Type };
                input = new CK5GetByParamInput();
                input.Ck5Type = ck5Type;

                dbData = _ck5Bll.GetCK5ByParam(input);
                return Mapper.Map<List<CK5Item>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<CK5GetByParamInput>(filter);
            input.Ck5Type = ck5Type;

            dbData = _ck5Bll.GetCK5ByParam(input);
            return Mapper.Map<List<CK5Item>>(dbData);
        }

        private CK5IndexViewModel CreateInitModelView(Enums.MenuList menulist, Enums.CK5Type ck5Type)
        {
            var model = new CK5IndexViewModel();

            model.MainMenu = menulist;
            model.CurrentMenu = PageInfo;
            model.Ck5Type = ck5Type;
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;

            var listCk5Dto = _ck5Bll.GetAll();
            model.SearchView.DocumentNumberList = new SelectList(listCk5Dto, "SUBMISSION_NUMBER", "SUBMISSION_NUMBER");
          
            model.SearchView.POAList = GlobalFunctions.GetPoaAll(_poabll);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();

            model.SearchView.NPPBKCOriginList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.SearchView.NPPBKCDestinationList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);


            //list table
            //todo refactor
            model.DetailsList = GetCk5Items(ck5Type);
            if (ck5Type == Enums.CK5Type.Domestic)
            {
                model.DetailList2 = GetCk5Items(Enums.CK5Type.Intercompany);
                model.DetailList3 = GetCk5Items(Enums.CK5Type.DomesticAlcohol);
                //model.DetailList2 = model.DetailsList.Where(a=>a.Ck5Type == Enums.CK5Type.Intercompany).ToList();
                //model.DetailList3 = model.DetailsList.Where(a => a.Ck5Type == Enums.CK5Type.DomesticAlcohol).ToList();
            }
            else if (ck5Type == Enums.CK5Type.PortToImporter)
                model.DetailList2 = GetCk5Items(Enums.CK5Type.ImporterToPlant);

            return model;
        }

        //
        // GET: /CK5/
        public ActionResult Index()
        {
            
            CK5IndexViewModel model;
            try
            {
                model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.Domestic);

                
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5IndexViewModel();
            }
           
            return View(model);
        }

        [HttpPost]
        public PartialViewResult Intercompany(CK5IndexViewModel model)
        {
            //only use by domestic and importer!

            Enums.CK5Type ck5Type = Enums.CK5Type.Domestic;

            if (model.Ck5Type == Enums.CK5Type.Domestic)
                ck5Type = Enums.CK5Type.Intercompany;
            else if (model.Ck5Type == Enums.CK5Type.PortToImporter)
                ck5Type = Enums.CK5Type.ImporterToPlant;

            model.DetailList2 = GetCk5Items(ck5Type, model.SearchView);
            return PartialView("_CK5IntercompanyTablePartial", model);
        }

        public PartialViewResult CK5DomesticAlcohol(CK5IndexViewModel model)
        {
            //only use by domestic

            Enums.CK5Type ck5Type = Enums.CK5Type.Domestic;

            if (model.Ck5Type == Enums.CK5Type.Domestic)
                ck5Type = Enums.CK5Type.DomesticAlcohol;
         
            model.DetailList3 = GetCk5Items(ck5Type, model.SearchView);
            return PartialView("_CK5DomesticAlcoholTablePartial", model);
        }

        [HttpPost]
        public PartialViewResult Filter(CK5IndexViewModel model)
        {
            model.DetailsList = GetCk5Items(model.Ck5Type, model.SearchView);
            return PartialView("_CK5TablePartial", model);
        }

        public ActionResult CK5Manual()
        {
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.Manual);
            return View(model);
        }

        public ActionResult CK5Export()
        {
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.Export);
            return View(model);
        }

     
        public ActionResult CK5Completed()
        {
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.Completed);
            model.IsCompletedType = true;
            return View(model);
        }

        public ActionResult CK5Import()
        {
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.PortToImporter);
            return View("CK5Import", model);
        }

        
        private SelectList GetCorrespondingPlantList(string plantId, Enums.CK5Type ck5Type)
        {
            SelectList data;
            T001WDto dataPlant = _plantBll.GetT001ById(plantId);
            
            if (ck5Type == Enums.CK5Type.Domestic) {
                
                data = GlobalFunctions.GetPlantByCompany(dataPlant.CompanyCode);
            }
            else if (ck5Type == Enums.CK5Type.Intercompany)
            {

                data = GlobalFunctions.GetPlantByCompany(dataPlant.CompanyCode,true);
            }
            else {
                data = GlobalFunctions.GetPlantAll();
            
            }
            
            return data;
        }

        #endregion

        #region Save Edit

        public ActionResult Create(string ck5Type)
        {
            var model = new CK5FormViewModel();
            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;
            if (ck5Type == Enums.CK5Type.Domestic.ToString())
                model.Ck5Type = Enums.CK5Type.Domestic;
            else if (ck5Type == Enums.CK5Type.Intercompany.ToString())
                model.Ck5Type = Enums.CK5Type.Intercompany;

            return View(model);
        }

        private CK5FormViewModel InitCreateCK5(Enums.CK5Type ck5Type)
        {
           

            var model = new CK5FormViewModel();
            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;
            model.Ck5Type = ck5Type;
            model.DocumentStatus = Enums.DocumentStatus.Draft;
            model.SubmissionDate = DateTime.Now;
            model = InitCK5List(model);

            return model;
        }

        private CK5FormViewModel InitCK5List(CK5FormViewModel model)
        {

            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

            //model.KppBcCityList = GlobalFunctions.GetKppBcCityList();
            

            model.SourcePlantList = GlobalFunctions.GetPlantAll();
            model.DestPlantList = GlobalFunctions.GetPlantAll();

            //model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();
            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedListByPlant("");

            model.PackageUomList = GlobalFunctions.GetUomList(_uomBll);

            model.CountryCodeList = GlobalFunctions.GetCountryList();

            return model;
        }

        public ActionResult CreateDomestic()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create CK5 Document
                AddMessageInfo("Can't create CK5 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = InitCreateCK5(Enums.CK5Type.Domestic);
            return View("Create", model);
        }

        public ActionResult CreateIntercompany()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create CK5 Document
                AddMessageInfo("Can't create CK5 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var model = InitCreateCK5(Enums.CK5Type.Intercompany);
            return View("Create", model);
        }

        public ActionResult CreatePortToImporter()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create CK5 Document
                AddMessageInfo("Can't create CK5 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var model = InitCreateCK5(Enums.CK5Type.PortToImporter);
            model.IsCk5PortToImporter = true;
            return View("Create", model);
        }

        public ActionResult CreateImporterToPlant()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create CK5 Document
                AddMessageInfo("Can't create CK5 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var model = InitCreateCK5(Enums.CK5Type.ImporterToPlant);
            return View("Create", model);
        }

        public ActionResult CreateExport()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create CK5 Document
                AddMessageInfo("Can't create CK5 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var model = InitCreateCK5(Enums.CK5Type.Export);
            model.IsCk5Export = true;
            return View("Create", model);
        }

        public ActionResult CreateManual()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create CK5 Document
                AddMessageInfo("Can't create CK5 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var model = InitCreateCK5(Enums.CK5Type.Manual);
            model.IsCk5Manual = true;
            return View("Create", model);
        }

        public ActionResult CreateDomesticAlcohol()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                //can't create CK5 Document
                AddMessageInfo("Can't create CK5 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            var model = InitCreateCK5(Enums.CK5Type.DomesticAlcohol);
            return View("Create", model);
        }

        //[HttpPost]
        //public JsonResult GetCompanyCode(string nppBkcCityId)
        //{
        //    var companyCode = "";
        //    var data = GlobalFunctions.GetNppbkcById(nppBkcCityId);
        //    //var data = GlobalFunctions.GetNppbkcByFlagDeletionList(false);
        //    if (data != null)
        //        companyCode = data.BUKRS;
        //    return Json(companyCode);
        //}

        [HttpPost]
        public JsonResult GetSourcePlantDetailsAndPbckItem(string sourcePlantId, string sourceNppbkcId, string destPlantId, DateTime submissionDate, string goodTypeGroupId, Enums.CK5Type ck5Type)
        {
            //var dbPlantSource = _plantBll.GetT001ById(sourcePlantId);
            var dbPlantDest = _plantBll.GetT001ById(destPlantId);
            var model = Mapper.Map<CK5PlantModel>(dbPlantDest);

            GetQuotaAndRemainOutput output;
            if (string.IsNullOrEmpty(goodTypeGroupId))
            {
                output = _ck5Bll.GetQuotaRemainAndDatePbck1Item(sourcePlantId, sourceNppbkcId, submissionDate, dbPlantDest.NPPBKC_ID, null);
            } else {
                Enums.ExGoodsType goodtypeenum = (Enums.ExGoodsType)Enum.Parse(typeof(Enums.ExGoodsType), goodTypeGroupId);
                output = _ck5Bll.GetQuotaRemainAndDatePbck1Item(sourcePlantId, sourceNppbkcId, submissionDate, dbPlantDest.NPPBKC_ID, (int)goodtypeenum);
            }


            if (sourceNppbkcId == dbPlantDest.NPPBKC_ID)
            {
                model.Pbck1Id = null;
                model.Pbck1Number = null;
                model.Pbck1DecreeDate = null;
                model.Pbck1QtyApproved = "0";
                model.Ck5TotalExciseable = "0";
                model.RemainQuota = "0";
                model.PbckUom = "";
            }
            else {
                model.Pbck1Id = output.Pbck1Id;
                model.Pbck1Number = output.Pbck1Number;
                model.Pbck1DecreeDate = output.Pbck1DecreeDate;
                model.Pbck1QtyApproved = output.QtyApprovedPbck1.ToString();
                model.Ck5TotalExciseable = output.QtyCk5.ToString();
                model.RemainQuota = output.RemainQuota.ToString();
                model.PbckUom = output.PbckUom;
            }
            
            

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetSourcePlantDetails(string plantId, Enums.CK5Type ck5Type)
        {
            var dbPlant = _plantBll.GetT001ById(plantId);
            var model = Mapper.Map<CK5PlantModel>(dbPlant);

            if (ck5Type == Enums.CK5Type.ImporterToPlant)
            {
                model.NPPBCK_ID = dbPlant.NPPBKC_IMPORT_ID;
            }

            model.CorrespondingPlantList = GetCorrespondingPlantList(plantId, ck5Type);
      
            return Json(model);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCK5(CK5FormViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.UploadItemModels.Count > 0)
                    {
                        if (model.Ck5Type == Enums.CK5Type.Domestic && (model.SourceNppbkcId == model.DestNppbkcId))
                        {

                        }
                        else
                        {
                            if (model.Ck5Type != Enums.CK5Type.Export &&
                                model.Ck5Type != Enums.CK5Type.PortToImporter)
                            {
                                //double check
                                GetQuotaAndRemainOutput output;

                                if (!model.SubmissionDate.HasValue)
                                    model.SubmissionDate = DateTime.Now;

                                output = _ck5Bll.GetQuotaRemainAndDatePbck1Item(model.SourcePlantId, model.SourceNppbkcId,
                                    model.SubmissionDate.Value, model.DestNppbkcId, (int) model.GoodType);


                                model.RemainQuota = (output.QtyApprovedPbck1 - output.QtyCk5).ToString();
                            }
                        }


                        var saveResult = SaveCk5ToDatabase(model);
                        
                        AddMessageInfo("Success create CK5", Enums.MessageInfoType.Success);

                        
                        return RedirectToAction("Edit", "CK5", new { @id = saveResult.CK5_ID });
                    }

                    AddMessageInfo("Missing CK5 Material", Enums.MessageInfoType.Error);
                }
                else
                    AddMessageInfo("Not Valid Model", Enums.MessageInfoType.Error);

                model = InitCK5List(model);

                return View("Create", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitCK5List(model);

                return View("Create", model);
            }
            
        }


        [HttpPost]
        public PartialViewResult UploadFile(HttpPostedFileBase itemExcelFile, string plantId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new CK5FormViewModel();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var uploadItem = new CK5UploadViewModel();

                    try
                    {
                        uploadItem.Brand = datarow[0];
                        uploadItem.Qty = datarow[1];
                        uploadItem.Uom = datarow[2];
                        uploadItem.Convertion = datarow[3];
                        uploadItem.ConvertedUom = datarow[4];
                        uploadItem.UsdValue = datarow[5];
                        if (datarow.Count > 6)
                            uploadItem.Note = datarow[6];

                        uploadItem.Plant = plantId;

                        model.UploadItemModels.Add(uploadItem);

                    }
                    catch (Exception)
                    {
                        continue;

                    }

                }
            }

            var input = Mapper.Map<List<CK5MaterialInput>>(model.UploadItemModels);

            var outputResult = _ck5Bll.CK5MaterialProcess(input);

            model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(outputResult);

            return PartialView("_CK5UploadList", model.UploadItemModels);
        }
        
        private CK5FormViewModel InitEdit(CK5FormViewModel model)
        {

            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

            //model.KppBcCityList = GlobalFunctions.GetKppBcCityList();
           
            model.SourcePlantList = GlobalFunctions.GetPlantAll();
            model.DestPlantList = GlobalFunctions.GetPlantAll();

            //model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();
            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedListByPlant(model.SourcePlantId);

            model.PackageUomList = GlobalFunctions.GetUomList(_uomBll);

            model.CountryCodeList = GlobalFunctions.GetCountryList();

            return model;
        }

        public ActionResult Edit(long id)
        {
            var model = new CK5FormViewModel();

            try
            {
                var ck5Details = _ck5Bll.GetDetailsCK5(id);

                Mapper.Map(ck5Details.Ck5Dto, model);

                //validate
                //only allow edit/submit when current_user = createdby and document = draft
                var input = new WorkflowAllowEditAndSubmitInput();
                input.DocumentStatus = model.DocumentStatus;
                //todo check
                input.CreatedUser = ck5Details.Ck5Dto.CREATED_BY;
                input.CurrentUser = CurrentUser.USER_ID;
                if (!_workflowBll.AllowEditDocument(input))
                    return RedirectToAction("Details", "CK5", new { @id = model.Ck5Id });

                model = InitEdit(model);
               
                model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(ck5Details.Ck5MaterialDto);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(ck5Details.ListChangesHistorys);
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(ck5Details.ListWorkflowHistorys);
                model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(ck5Details.ListPrintHistorys);

                if (model.Ck5Type == Enums.CK5Type.Domestic && (model.SourceNppbkcId == model.DestNppbkcId))
                {

                }
                else
                {
                    if (model.Ck5Type != Enums.CK5Type.Export &&
                        model.Ck5Type != Enums.CK5Type.PortToImporter)
                    {

                        var output = _ck5Bll.GetQuotaRemainAndDatePbck1ByCk5Id(id);
                        model.Pbck1QtyApproved = output.QtyApprovedPbck1.ToString();
                        //
                        //decimal currentCk5 = output.QtyCk5;//- model.GrandTotalEx;

                        model.Ck5TotalExciseable = (output.QtyCk5 - model.GrandTotalEx).ToString();
                        model.RemainQuota = (output.QtyApprovedPbck1 - output.QtyCk5).ToString();

                        model.PbckUom = output.PbckUom;
                    }

                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                //model.MainMenu = Enums.MenuList.CK5;
                //model.CurrentMenu = PageInfo;
                model = InitEdit(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CK5FormViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.UploadItemModels.Count > 0)
                    {
                        bool isSubmit = model.Command == "Submit";

                        //validate
                        var input = new WorkflowAllowEditAndSubmitInput();
                        input.DocumentStatus = model.DocumentStatus;
                        input.CreatedUser = model.CreatedBy;
                        input.CurrentUser = CurrentUser.USER_ID;
                        if (_workflowBll.AllowEditDocument(input))
                        {
                            //quota
                            if (model.Ck5Type == Enums.CK5Type.Domestic && (model.SourceNppbkcId == model.DestNppbkcId))
                            {

                            }
                            else
                            {
                                if (model.Ck5Type != Enums.CK5Type.Export &&
                                    model.Ck5Type != Enums.CK5Type.PortToImporter)
                                {

                                   // var output = _ck5Bll.GetQuotaRemainAndDatePbck1ByCk5Id(model.Ck5Id);

                                    var submissionDate = DateTime.Now;
                                    if (model.SubmissionDate.HasValue)
                                        submissionDate = model.SubmissionDate.Value;

                                    var output = _ck5Bll.GetQuotaRemainAndDatePbck1Item(model.SourcePlantId,
                                        model.SourceNppbkcId, submissionDate, model.DestNppbkcId, (int) model.GoodType);

                                    model.Pbck1QtyApproved = output.QtyApprovedPbck1.ToString();
                                    decimal currentCk5 = output.QtyCk5 - model.GrandTotalEx;
                                    model.Ck5TotalExciseable = currentCk5.ToString();
                                    model.RemainQuota = (output.QtyApprovedPbck1 - currentCk5).ToString();
                                }
                            }

                            SaveCk5ToDatabase(model);
                            if (isSubmit)
                            {
                                CK5Workflow(model.Ck5Id, Enums.ActionType.Submit, string.Empty);
                                AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                                return RedirectToAction("Details", "CK5", new { @id = model.Ck5Id });
                               
                            }
                            //update info quota
                            //model.RemainQuota = (output.QtyApprovedPbck1 - currentCk5 - model.GrandTotalEx).ToString();
                            AddMessageInfo("Success", Enums.MessageInfoType.Success);
                            return RedirectToAction("Edit", "CK5", new {@id = model.Ck5Id});
                        }
                        else
                        {
                            AddMessageInfo("Not allow to Edit Document", Enums.MessageInfoType.Error);
                            return RedirectToAction("Details", "CK5", new { @id = model.Ck5Id });
                        }

                    }
                    else
                        AddMessageInfo("Missing CK5 Material", Enums.MessageInfoType.Error);
                }
                else
                    AddMessageInfo("Not Valid Model", Enums.MessageInfoType.Error);

                model = InitEdit(model);
                model = GetHistorys(model);

                return View(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                model = InitEdit(model);
                model = GetHistorys(model);

                return View(model);
            }

           
        }

        private CK5FormViewModel GetHistorys(CK5FormViewModel model)
        {
            //todo check
            model.WorkflowHistory =
                Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.SubmissionNumber));

            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5,model.Ck5Id.ToString()));

            return model;
        }
      
        public ActionResult Details(long id)
        {
            var model = new CK5FormViewModel();

            try
            {
                var ck5Details = _ck5Bll.GetDetailsCK5(id);

                
                Mapper.Map(ck5Details.Ck5Dto, model);

                model.SourcePlantId = model.SourcePlantId + " - " + model.SourcePlantName;
                model.DestPlantId = model.DestPlantId + " - " + model.DestPlantName;

                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;


                // model = GetInitDetailsData(model);
                model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(ck5Details.Ck5MaterialDto);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(ck5Details.ListChangesHistorys);
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(ck5Details.ListWorkflowHistorys);

                model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(ck5Details.ListPrintHistorys);

                model.CreatedBy = ck5Details.Ck5Dto.CREATED_BY;

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput();
                input.DocumentStatus = model.DocumentStatus;
                input.FormView = Enums.FormViewType.Detail;
                input.UserRole = CurrentUser.UserRole;
                input.CreatedUser = ck5Details.Ck5Dto.CREATED_BY;
                input.CurrentUser = CurrentUser.USER_ID;
                input.CurrentUserGroup = CurrentUser.USER_GROUP_ID;
                input.DocumentNumber = model.SubmissionNumber;
                input.NppbkcId = model.SourceNppbkcId;

                //workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;


                if (!allowApproveAndReject) 
                {
                    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                    model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
                }
               
                //gov approval purpose
                //if (model.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval)
                //    model.KppBcCity = model.KppBcCityName;

                model.IsAllowPrint = _workflowBll.AllowPrint(model.DocumentStatus);
                
                var outputHistory = _workflowHistoryBll.GetStatusGovHistory(ck5Details.Ck5Dto.SUBMISSION_NUMBER);
                model.GovStatusDesc = outputHistory.StatusGov;
                model.CommentGov = outputHistory.Comment;

                if (model.Ck5Type == Enums.CK5Type.Domestic && (model.SourceNppbkcId == model.DestNppbkcId))
                {

                }
                else
                {
                    if (model.Ck5Type != Enums.CK5Type.Export &&
                        model.Ck5Type != Enums.CK5Type.PortToImporter)
                    {
                        var outputQuota = _ck5Bll.GetQuotaRemainAndDatePbck1ByCk5Id(ck5Details.Ck5Dto.CK5_ID);
                        model.Pbck1QtyApproved = outputQuota.QtyApprovedPbck1.ToString();
                        //decimal currentCk5 = outputQuota.QtyCk5 - model.GrandTotalEx;
                        model.Ck5TotalExciseable = (outputQuota.QtyCk5 - model.GrandTotalEx).ToString();
                        model.RemainQuota = (outputQuota.QtyApprovedPbck1 - outputQuota.QtyCk5).ToString();
                    }
                }

                model.AllowGiCreated = _workflowBll.AllowGiCreated(input);
                model.AllowGrCreated = _workflowBll.AllowGrCreated(input);

                model.AllowCancelSAP = _workflowBll.AllowCancelSAP(input);

                if (model.AllowGovApproveAndReject)
                    model.ActionType = "GovApproveDocument";
                else if (model.AllowGiCreated)
                    model.ActionType = "CK5GICreated";
                else if (model.AllowGrCreated)
                    model.ActionType = "CK5GRCreated";

                

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View(model);
        }

        private CK5Dto SaveCk5ToDatabase(CK5FormViewModel model)
        {
            //process save
            //if (model.Ck5Id >0)

            var dataToSave = Mapper.Map<CK5Dto>(model);
          
            var input = new CK5SaveInput()
            {
                Ck5Dto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                //WorkflowActionType = Enums.ActionType.Save,
                Ck5Material = Mapper.Map<List<CK5MaterialDto>>(model.UploadItemModels)
            };

            return _ck5Bll.SaveCk5(input);
        }


        [HttpPost]
        public PartialViewResult GetOriginalPlant(long ck5Id)
        {
            
            var listCk5MaterialDto = _ck5Bll.GetCK5MaterialByCK5Id(ck5Id);

            var model = new CK5FormViewModel();
            model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(listCk5MaterialDto);

            return PartialView("_CK5UploadListOriginal", model);
        }

        #endregion

        #region export xls

        public void ExportXls(long ck5Id)
        {
           // return File(CreateXlsFile(ck5Id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            var pathFile = CreateXlsFile(ck5Id);
            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);// "CK5" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            
            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsFile(long ck5Id)
        {
            var slDocument = new SLDocument();

            //todo check
            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, ck5Id.ToString());

            var model = Mapper.Map<List<ChangesHistoryItemModel>>(listHistory);
            
            int iRow = 1;

            //create header
            slDocument.SetCellValue(iRow, 1, "DATE");
            slDocument.SetCellValue(iRow, 2, "FIELD");
            slDocument.SetCellValue(iRow, 3, "OLD VALUE");
            slDocument.SetCellValue(iRow, 4, "NEW VALUE");
            slDocument.SetCellValue(iRow, 5, "USER");

            iRow++;
            
            foreach (var changesHistoryItemModel in model)
            {
                slDocument.SetCellValue(iRow, 1,
                    changesHistoryItemModel.MODIFIED_DATE.HasValue
                        ? changesHistoryItemModel.MODIFIED_DATE.Value.ToString("dd MMM yyyy")
                        : string.Empty);
                slDocument.SetCellValue(iRow, 2, changesHistoryItemModel.FIELD_NAME);
                slDocument.SetCellValue(iRow, 3, changesHistoryItemModel.OLD_VALUE);
                slDocument.SetCellValue(iRow, 4, changesHistoryItemModel.NEW_VALUE);
                slDocument.SetCellValue(iRow, 5, changesHistoryItemModel.USERNAME);
                
                iRow++;
            }

            //create style
            SLStyle styleBorder = slDocument.CreateStyle();
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //SLStyle styleHeader = slDocument.CreateStyle();
            //styleHeader.Font.Bold = true;

            slDocument.AutoFitColumn(1, 5);
            slDocument.SetCellStyle(1, 1, iRow - 1, 5, styleBorder);
            //slDocument.SetCellStyle(1, 1, 1, iColumn - 1, styleHeader);

            var fileName = "CK5" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Content/upload/"),fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        public void ExportClientsListToExcel(long ck5Id)
        {
          
            //todo check
            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, ck5Id.ToString());

            var model = Mapper.Map<List<ChangesHistoryItemModel>>(listHistory);

            var grid = new System.Web.UI.WebControls.GridView();


          
            grid.DataSource = from d in model
                             select new
                             {
                                 Date = d.MODIFIED_DATE.HasValue? d.MODIFIED_DATE.Value.ToString("dd MMM yyyy") : string.Empty,
                                 FieldName = d.FIELD_NAME,
                                 OldValue = d.OLD_VALUE,
                                 NewValue = d.NEW_VALUE,
                                 User = d.USERNAME

                             };

            grid.DataBind();

            var fileName = "CK5" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        #region Workflow

        private void CK5Workflow(long id, Enums.ActionType actionType, string comment)
        {
            var input = new CK5WorkflowDocumentInput();
            input.DocumentId = id;
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ActionType = actionType;
            input.Comment = comment;

            _ck5Bll.CK5Workflow(input);
        }


    
        public ActionResult SubmitDocument(long id)
        {
            try
            {
                CK5Workflow(id, Enums.ActionType.Submit,string.Empty);
                AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id });
        }

        public ActionResult ApproveDocument(long id)
        {
            try
            {
                CK5Workflow(id, Enums.ActionType.Approve, string.Empty);
                AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id });
        }

        public ActionResult RejectDocument(CK5FormViewModel model)
        {
            try
            {
                CK5Workflow(model.Ck5Id, Enums.ActionType.Reject, model.Comment);
                AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new {id = model.Ck5Id });
        }

        [HttpPost]
        public ActionResult GovApproveDocument(CK5FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddMessageInfo("Model Not Valid", Enums.MessageInfoType.Success);
               // return View("Details", model);
                return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
            }

            try
            {
                var currentUserId = CurrentUser.USER_ID;

                model.Ck5FileUploadModelList = new List<CK5FileUploadViewModel>();
                if (model.Ck5FileUploadFileList != null)
                {
                    foreach (var item in model.Ck5FileUploadFileList)
                    {
                        if (item != null)
                        {
                            var filenameCk5Check = item.FileName;
                            if (filenameCk5Check.Contains("\\"))
                                filenameCk5Check = filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];
                           
                            var ck5UploadFile = new CK5FileUploadViewModel
                            {
                                FILE_NAME = filenameCk5Check,
                                FILE_PATH = SaveUploadedFile(item, model.Ck5Id),
                                CREATED_DATE = DateTime.Now,
                                CREATED_BY = currentUserId
                            };
                            model.Ck5FileUploadModelList.Add(ck5UploadFile);
                        }
                      
                    }
                }
                else
                {
                    AddMessageInfo("Empty File", Enums.MessageInfoType.Error);
                    RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
                }

                switch (model.GovStatus)
                {
                    case Enums.CK5GovStatus.GovApproved:
                        if (CK5WorkflowGovApproval(model))
                            AddMessageInfo("Success Gov Approve Document", Enums.MessageInfoType.Success);
                        break;
                    case Enums.CK5GovStatus.GovReject:
                        CK5Workflow(model.Ck5Id, Enums.ActionType.GovReject, model.Comment);
                        AddMessageInfo("Success GovReject Document", Enums.MessageInfoType.Success);
                        break;
                    case Enums.CK5GovStatus.GovCancel:
                        CK5Workflow(model.Ck5Id, Enums.ActionType.GovCancel, model.Comment);
                        AddMessageInfo("Success GovCancel Document", Enums.MessageInfoType.Success);
                        break;
                    default:
                        AddMessageInfo("Undefined Gov Status", Enums.MessageInfoType.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
        }

        [HttpPost]
        public ActionResult CK5GICreated(CK5FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddMessageInfo("Model Not Valid", Enums.MessageInfoType.Success);
                // return View("Details", model);
                return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
            }

            try
            {
                //CK5Workflow(model.Ck5Id, Enums.ActionType.Submit, string.Empty);
                var input = new CK5WorkflowDocumentInput();
                input.DocumentId = model.Ck5Id;
                input.UserId = CurrentUser.USER_ID;
                input.UserRole = CurrentUser.UserRole;

                switch (model.DocumentStatus)
                {
                    case Enums.DocumentStatus.GICreated:
                        input.ActionType = Enums.ActionType.GICreated;
                        break;
                    case Enums.DocumentStatus.GICompleted:
                        input.ActionType = Enums.ActionType.GICompleted;
                        break;
                    default:
                        AddMessageInfo("DocumentStatus Not Allowed", Enums.MessageInfoType.Error);
                        return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
                }

                input.SealingNumber = model.SealingNotifNumber;
                input.SealingDate = model.SealingNotifDate;

                _ck5Bll.CK5Workflow(input);

                AddMessageInfo("Success update Sealing Number and Date", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
        }

        [HttpPost]
        public ActionResult CK5GRCreated(CK5FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddMessageInfo("Model Not Valid", Enums.MessageInfoType.Success);
                // return View("Details", model);
                return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
            }

            try
            {
                //CK5Workflow(model.Ck5Id, Enums.ActionType.Submit, string.Empty);
                var input = new CK5WorkflowDocumentInput();
                input.DocumentId = model.Ck5Id;
                input.UserId = CurrentUser.USER_ID;
                input.UserRole = CurrentUser.UserRole;

                switch (model.DocumentStatus)
                {
                    case Enums.DocumentStatus.GRCreated:
                        input.ActionType = Enums.ActionType.GRCreated;
                        break;
                    case Enums.DocumentStatus.GRCompleted:
                        input.ActionType = Enums.ActionType.GRCompleted;
                        break;
                    default:
                        AddMessageInfo("DocumentStatus Not Allowed", Enums.MessageInfoType.Error);
                        return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
                }


                input.SealingNumber = model.SealingNotifNumber;
                input.SealingDate = model.SealingNotifDate;

                input.UnSealingNumber = model.UnSealingNotifNumber;
                input.UnSealingDate = model.UnsealingNotifDate;

                _ck5Bll.CK5Workflow(input);

                AddMessageInfo("Success update Sealing/Unsealing Number and Date", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
        }

        private bool  CK5WorkflowGovApproval(CK5FormViewModel model)
        {
           
            DateTime registrationDate = DateTime.Now;
            if (model.RegistrationDate.HasValue)
                registrationDate = model.RegistrationDate.Value;

            var input = new CK5WorkflowDocumentInput()
            {
                DocumentId = model.Ck5Id,
                ActionType = Enums.ActionType.GovApprove,
                UserRole = CurrentUser.UserRole,
                UserId = CurrentUser.USER_ID,
                AdditionalDocumentData = new CK5WorkflowDocumentData()
                {
                    RegistrationNumber = model.RegistrationNumber,
                    RegistrationDate = registrationDate,
                    Ck5FileUploadList = Mapper.Map<List<CK5_FILE_UPLOADDto>>(model.Ck5FileUploadModelList)
                }
            };
            _ck5Bll.CK5Workflow(input);

            try
            {
                //create xml file
                var ck5XmlDto = _ck5Bll.GetCk5ForXmlById(model.Ck5Id);
              
                var fileName = ConfigurationManager.AppSettings["CK5PathXml"] + "CK5APP_" +
                               model.SubmissionNumber + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml";
                
                ck5XmlDto.Ck5PathXml = fileName;

                XmlCK5DataWriter rt = new XmlCK5DataWriter();
                
                //ck5XmlDto.SUBMISSION_NUMBER = Convert.ToInt32(model.SubmissionNumber.Split('/')[0]).ToString("0000000000");
                rt.CreateCK5Xml(ck5XmlDto);

                return true;
            }
            catch (Exception ex)
            {
                //failed create xml...
                //rollaback the update
                _ck5Bll.GovApproveDocumentRollback(input);
                AddMessageInfo("Failed Create CK5 XMl message : " + ex.Message, Enums.MessageInfoType.Error);
                return false;
            }
          
        }

        private string SaveUploadedFile(HttpPostedFileBase file, long ck5Id)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            //initialize folders in case deleted by an test publish profile
            if (!Directory.Exists(Server.MapPath(Constans.CK5FolderPath)))
                Directory.CreateDirectory(Server.MapPath(Constans.CK5FolderPath));

            sFileName = Constans.CK5FolderPath + Path.GetFileName(ck5Id.ToString("'ID'-##") + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }


        public ActionResult GovRejectDocument(CK5FormViewModel model)
        {
            try
            {
                CK5Workflow(model.Ck5Id, Enums.ActionType.GovReject, model.Comment);
                AddMessageInfo("Success GovReject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
        }

        public ActionResult GovCancelDocument(CK5FormViewModel model)
        {
            try
            {
                CK5Workflow(model.Ck5Id, Enums.ActionType.GovCancel, model.Comment);
                AddMessageInfo("Success GovCancel Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
        }

        public ActionResult CancelDocument(CK5FormViewModel model)
        {
            try
            {
                CK5Workflow(model.Ck5Id, Enums.ActionType.Cancel, model.Comment);
                AddMessageInfo("Success Cancel Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
        }

        public ActionResult CancelSAPDocument(long id)
        {
            try
            {
                var ck5 = _ck5Bll.GetById(id);

                if (ck5.STATUS_ID == Enums.DocumentStatus.STOCreated && string.IsNullOrEmpty(ck5.DN_NUMBER))
                {
                    CK5Workflow(id, Enums.ActionType.CancelSTOCreated, string.Empty);

                    try
                    {
                        //create xml file
                        var ck5XmlDto = _ck5Bll.GetCk5ForXmlById(id);
                        ////todo check validation
                        //var fileName = ConfigurationManager.AppSettings["CK5PathXml"] + "CK5APP_" +
                        //               ck5XmlDto.SUBMISSION_NUMBER + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml";

                        var date = DateTime.Now.ToString("yyyyMMdd");
                        var time = DateTime.Now.ToString("hhmmss");

                        var fileName = string.Format("CK5CAN_{0}-{1}-{2}.xml", ck5.SUBMISSION_NUMBER, date, time);

                        if (fileName.Contains("/"))
                            throw  new Exception("You use Old CK5Number");

                        ck5XmlDto.Ck5PathXml = Path.Combine(ConfigurationManager.AppSettings["CK5PathXml"], fileName);

                        XmlCK5DataWriter rt = new XmlCK5DataWriter();

                        rt.CreateCK5Xml(ck5XmlDto, "03");

                        AddMessageInfo("Success Cancel Document", Enums.MessageInfoType.Success);
                    }
                    catch (Exception ex)
                    {
                        //failed create xml...
                        //rollaback the update
                        var input = new CK5WorkflowDocumentInput();
                        input.DocumentId = id;
                        input.UserId = CurrentUser.USER_ID;
                        input.UserRole = CurrentUser.UserRole;
                        input.ActionType = Enums.ActionType.CancelSTOCreated;

                        _ck5Bll.CancelSTOCreatedRollback(input);
                        AddMessageInfo("Failed Create CK5  XMl 03 message : " + ex.Message, Enums.MessageInfoType.Error);
                    
                    }
                 
                }
                else
                {
                    CK5Workflow(id, Enums.ActionType.CancelSAP, string.Empty);
                    AddMessageInfo("Success Cancel Document", Enums.MessageInfoType.Success);
                }
                
                
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = id });
        }

        #endregion

        #region CK5 Summary Report Forms

        private SelectList GetCompanyList(bool isSource, List<CK5Dto> listCk5)
        {
          //  var listCk5 = _ck5Bll.GetAll();
            
            IEnumerable<SelectItemModel> query;
            if (isSource)
            {
                query = from x in listCk5
                    select new SelectItemModel()
                    {
                        ValueField = x.SOURCE_PLANT_COMPANY_CODE,
                        TextField = x.SOURCE_PLANT_COMPANY_CODE
                    };
            }
            else
            {
                query = from x in listCk5
                        select new SelectItemModel()
                        {
                            ValueField = x.DEST_PLANT_COMPANY_CODE,
                            TextField = x.DEST_PLANT_COMPANY_CODE
                        };
            }

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetNppbkcList(bool isSource, List<CK5Dto> listCk5)
        {
            //  var listCk5 = _ck5Bll.GetAll();

            IEnumerable<SelectItemModel> query;
            if (isSource)
            {
                query = from x in listCk5
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.SOURCE_PLANT_NPPBKC_ID,
                            TextField = x.SOURCE_PLANT_NPPBKC_ID
                        };
            }
            else
            {
                query = from x in listCk5
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.DEST_PLANT_NPPBKC_ID,
                            TextField = x.DEST_PLANT_NPPBKC_ID
                        };
            }

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetPlantList(bool isSource, List<CK5Dto> listCk5)
        {
            //  var listCk5 = _ck5Bll.GetAll();

            IEnumerable<SelectItemModel> query;
            if (isSource)
            {
                query = from x in listCk5
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.SOURCE_PLANT_ID,
                            TextField = x.SOURCE_PLANT_ID + " - " + x.SOURCE_PLANT_NAME
                        };
            }
            else
            {
                query = from x in listCk5
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.DEST_PLANT_ID,
                            TextField = x.DEST_PLANT_ID + " - " + x.DEST_PLANT_NAME
                        };
            }

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetSubmissionDateListCK5(bool isFrom, List<CK5Dto> listCk5)
        {
          
            IEnumerable<SelectItemModel> query;
            if (isFrom)
                query = from x in listCk5.Where(c => c.SUBMISSION_DATE != null)
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.SUBMISSION_DATE,
                            TextField = x.SUBMISSION_DATE.Value.ToString("dd MMM yyyy")
                        };
            else
                query = from x in listCk5.Where(c => c.SUBMISSION_DATE != null).OrderByDescending(c => c.SUBMISSION_DATE)
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.SUBMISSION_DATE,
                            TextField = x.SUBMISSION_DATE.Value.ToString("dd MMM yyyy")
                        };

            return new SelectList(query.DistinctBy(c => c.TextField), "ValueField", "TextField");

        }

        private CK5SummaryReportsViewModel InitSummaryReports(CK5SummaryReportsViewModel model)
        {
            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

            //_ck5Bll.GetSummaryReportsByParam(input);
            //var listCk5 = _ck5Bll.GetCk5CompletedByCk5Type(model.Ck5Type);
            var listCk5 = _ck5Bll.GetSummaryReportsByParam(new CK5GetSummaryReportByParamInput());

            model.SearchView.CompanyCodeSourceList = GetCompanyList(true, listCk5);
            model.SearchView.CompanyCodeDestList = GetCompanyList(false, listCk5);
            model.SearchView.NppbkcIdSourceList = GetNppbkcList(true, listCk5);
            model.SearchView.NppbkcIdDestList = GetNppbkcList(false, listCk5);
            model.SearchView.PlantSourceList = GetPlantList(true, listCk5);
            model.SearchView.PlantDestList = GetPlantList(false, listCk5);
            model.SearchView.DateFromList = GetSubmissionDateListCK5(true, listCk5);
            model.SearchView.DateToList = GetSubmissionDateListCK5(false, listCk5);

            var filter = new CK5SearchSummaryReportsViewModel();
            filter.Ck5Type = model.Ck5Type;
            //view all data ck5 completed document
            model.DetailsList = SearchDataSummaryReports(filter);

            return model;
        }

        public ActionResult SummaryReports()
        {

            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();
            
                model.Ck5Type = Enums.CK5Type.Domestic;

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("CK5SummaryReport", model);
        }

        public ActionResult SummaryReportsExport()
        {

            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();

                model.Ck5Type = Enums.CK5Type.Export;
                model.SearchView.Ck5Type = Enums.CK5Type.Export;

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("CK5SummaryReport", model);
        }

        public ActionResult SummaryReportsIntercompany()
        {

            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();

                model.Ck5Type = Enums.CK5Type.Intercompany;
                model.SearchView.Ck5Type = Enums.CK5Type.Intercompany;

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("CK5SummaryReport", model);
        }

        public ActionResult SummaryReportsDomesticAlcohol()
        {

            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();

                model.Ck5Type = Enums.CK5Type.DomesticAlcohol;
                model.SearchView.Ck5Type = Enums.CK5Type.DomesticAlcohol;

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("CK5SummaryReport", model);
        }

        public ActionResult SummaryReportsPortToImporter()
        {

            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();

                model.Ck5Type = Enums.CK5Type.PortToImporter;
                model.SearchView.Ck5Type = Enums.CK5Type.PortToImporter;
                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("CK5SummaryReport", model);
        }

        public ActionResult SummaryReportsImporterToPlant()
        {

            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();

                model.Ck5Type = Enums.CK5Type.ImporterToPlant;
                model.SearchView.Ck5Type = Enums.CK5Type.ImporterToPlant;

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("CK5SummaryReport", model);
        }

        public ActionResult SummaryReportsManual()
        {

            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();

                model.Ck5Type = Enums.CK5Type.Manual;
                model.SearchView.Ck5Type = Enums.CK5Type.Manual;

                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("CK5SummaryReport", model);
        }

        private List<CK5SummaryReportsItem> SearchDataSummaryReports(CK5SearchSummaryReportsViewModel filter = null)
        {
            CK5GetSummaryReportByParamInput input;
            List<CK5Dto> dbData;
            if (filter == null)
            {
                //Get All
                input = new CK5GetSummaryReportByParamInput();

                dbData = _ck5Bll.GetSummaryReportsByParam(input);
                return Mapper.Map<List<CK5SummaryReportsItem>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<CK5GetSummaryReportByParamInput>(filter);

            dbData = _ck5Bll.GetSummaryReportsByParam(input);
            return Mapper.Map<List<CK5SummaryReportsItem>>(dbData);
        }
      

        [HttpPost]
        public PartialViewResult SearchSummaryReports(CK5SummaryReportsViewModel model)
        {
            model.DetailsList = SearchDataSummaryReports(model.SearchView);
            return PartialView("_CK5ListSummaryReport", model);

            //if (model.Ck5Type == Enums.CK5Type.Domestic)
            //    return PartialView("_CK5ListSummaryReport", model);
            //if (model.Ck5Type == Enums.CK5Type.Intercompany || model.Ck5Type == Enums.CK5Type.DomesticAlcohol)
            //    return PartialView("_CK5ListSummaryReportIntercompany", model);
            //if (model.Ck5Type == Enums.CK5Type.PortToImporter || model.Ck5Type == Enums.CK5Type.ImporterToPlant)
            //    return PartialView("_CK5ListSummaryReportImport", model);
            //if (model.Ck5Type == Enums.CK5Type.Manual)
            //    return PartialView("_CK5ListSummaryManual", model);
             
            //return PartialView("_CK5ListSummaryReportExport", model);
        }

        public void ExportXlsSummaryReports(CK5SummaryReportsViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryReports(model.ExportModel);

        
            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);// "CK5" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

     
        private string CreateXlsSummaryReports(CK5ExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataSummaryReports(modelExport);
            
            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcel(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.Ck5TypeDescription)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5TypeDescription);
                    iColumn = iColumn + 1;
                }
                if (modelExport.KppbcCityName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.KppbcCityName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SubmissionNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SubmissionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExGoodTypeDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExGoodTypeDesc);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExciseSettlement)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExciseSettlement);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ExciseStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExciseStatus);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RequestType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourcePlant)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourcePlant);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DestinationPlant)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestinationPlant);
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnpaidExciseFacilityNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnpaidExciseFacilityNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnpaidExciseFacilityDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnpaidExciseFacilityDate);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.SealingNotificationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotificationDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.SealingNotificationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotificationNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.UnSealingNotificationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotificationNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.UnSealingNotificationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotificationNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Lack1)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack1);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Lack2)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TanggalAju)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TanggalAju);
                    iColumn = iColumn + 1;
                }
                if (modelExport.NomerAju)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NomerAju);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TanggalPendaftaran)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TanggalPendaftaran);
                    iColumn = iColumn + 1;
                }
                if (modelExport.NomerPendaftaran)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NomerPendaftaran);
                    iColumn = iColumn + 1;
                }
                if (modelExport.OriginCeOffice)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.OriginCeOffice);
                    iColumn = iColumn + 1;
                }
                if (modelExport.OriginCompany)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.OriginCompany);
                    iColumn = iColumn + 1;
                }
                if (modelExport.OriginCompanyNppbkc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.OriginCompanyNppbkc);
                    iColumn = iColumn + 1;
                }
                if (modelExport.OriginCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.OriginCompanyAddress);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestinationCountry)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestinationCountry);
                    iColumn = iColumn + 1;
                }
                if (modelExport.NumberBox)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NumberBox);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ContainPerBox)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ContainPerBox);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TotalOfExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TotalOfExcisableGoods);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BanderolPrice)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BanderolPrice);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExciseTariff)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExciseTariff);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExciseValue)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExciseValue);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestinationCeOffice)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestinationCeOffice);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestCompanyAddress);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestCompanyNppbkc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestCompanyNppbkc);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestCompanyName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.LoadingPort)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LoadingPort);
                    iColumn = iColumn + 1;
                }
                if (modelExport.LoadingPortName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LoadingPortName);
                    iColumn = iColumn + 1;
                }
              

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, CK5ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.Ck5TypeDescription)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK5 Type");
                iColumn = iColumn + 1;
            }
            if (modelExport.KppbcCityName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Kppbc City Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.SubmissionNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Submission Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.SubmissionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Submission Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExGoodTypeDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Good Type");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExciseSettlement)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Settlement");
                iColumn = iColumn + 1;
            }

            if (modelExport.ExciseStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.RequestType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Request Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourcePlant)
            {
                slDocument.SetCellValue(iRow, iColumn, "Source Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestinationPlant)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnpaidExciseFacilityNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnpaidExciseFacilityDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Date");
                iColumn = iColumn + 1;
            }

            //start
            if (modelExport.SealingNotificationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.SealingNotificationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.UnSealingNotificationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "UnSealing Notification Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.UnSealingNotificationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "UnSealing Notification Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.Lack1)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-1 Month");
                iColumn = iColumn + 1;
            }
            if (modelExport.Lack2)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-2 Month");
                iColumn = iColumn + 1;
            }
            if (modelExport.TanggalAju)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Aju");
                iColumn = iColumn + 1;
            }
            if (modelExport.NomerAju)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomer Aju");
                iColumn = iColumn + 1;
            }
            if (modelExport.TanggalPendaftaran)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Pendaftaran");
                iColumn = iColumn + 1;
            }
            if (modelExport.NomerPendaftaran)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomer Pendaftaran");
                iColumn = iColumn + 1;
            }
            if (modelExport.OriginCeOffice)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin CE Office");
                iColumn = iColumn + 1;
            }
            if (modelExport.OriginCompany)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Company");
                iColumn = iColumn + 1;
            }
            if (modelExport.OriginCompanyNppbkc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Company NPPBKC");
                iColumn = iColumn + 1;
            }
            if (modelExport.OriginCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Company Address");
                iColumn = iColumn + 1;
            }
           
            if (modelExport.DestinationCountry)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Country");
                iColumn = iColumn + 1;
            }
            if (modelExport.NumberBox)
            {
                slDocument.SetCellValue(iRow, iColumn, "Number Box");
                iColumn = iColumn + 1;
            }
            if (modelExport.ContainPerBox)
            {
                slDocument.SetCellValue(iRow, iColumn, "Contain per Box");
                iColumn = iColumn + 1;
            }

            if (modelExport.TotalOfExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Total of Excisable Goods");
                iColumn = iColumn + 1;
            }
            if (modelExport.BanderolPrice)
            {
                slDocument.SetCellValue(iRow, iColumn, "Banderol Price");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExciseTariff)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Tariff");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExciseValue)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Value");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestinationCeOffice)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Ce Office");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Storage Location");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestCompanyNppbkc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Storage Location's NPPBKC");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Storage Location's CE Office");
                iColumn = iColumn + 1;
            }
            if (modelExport.LoadingPort)
            {
                slDocument.SetCellValue(iRow, iColumn, "Loading Port");
                iColumn = iColumn + 1;
            }
            if (modelExport.LoadingPortName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Loading Port Name");
                iColumn = iColumn + 1;
            }
            return slDocument;

        }

   

        private string CreateXlsFileSummaryReports(SLDocument slDocument, int iColumn, int iRow)
        {
        
            //create style
            SLStyle styleBorder = slDocument.CreateStyle();
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //SLStyle styleHeader = slDocument.CreateStyle();
            //styleHeader.Font.Bold = true;

            slDocument.AutoFitColumn(1, iColumn-1);
            slDocument.SetCellStyle(1, 1, iRow-1, iColumn-1, styleBorder);
            //slDocument.SetCellStyle(1, 1, 1, iColumn - 1, styleHeader);

            var fileName = "CK5" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            //var path = Path.Combine(Server.MapPath("~/Content/upload/"), fileName);
            var path = Path.Combine(Server.MapPath(Constans.CK5FolderPath), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        #endregion

        #region print


        private dsCK5Print AddDataCk5Row(dsCK5Print dsCk5, CK5ReportDetailsDto ck5ReportDetails, int totalMaterial, string printTitle)
        {
            var detailRow = dsCk5.dtCk5.NewdtCk5Row();

            detailRow.OfficeName = ck5ReportDetails.OfficeName;
            detailRow.OfficeCode = ck5ReportDetails.OfficeCode;
            detailRow.SubmissionNumber= ck5ReportDetails.SubmissionNumber;
            detailRow.SubmissionDate = ck5ReportDetails.SubmissionDate;
            detailRow.RegistrationNumber = ck5ReportDetails.RegistrationNumber;
            detailRow.RegistrationDate = ck5ReportDetails.RegistrationDate;
            detailRow.ExGoodType = ck5ReportDetails.ExGoodType;
            detailRow.ExciseSettlement = ck5ReportDetails.ExciseSettlement;
            detailRow.ExciseStatus = ck5ReportDetails.ExciseStatus;
            detailRow.RequestType = ck5ReportDetails.RequestType;
            detailRow.SourcePlantNpwp = ck5ReportDetails.SourcePlantNpwp;
            detailRow.SourcePlantNppbkc = ck5ReportDetails.SourcePlantNppbkc;
            detailRow.SourcePlantName = ck5ReportDetails.SourcePlantName;
            detailRow.SourcePlantAddress = ck5ReportDetails.SourcePlantAddress;
            detailRow.SourceOfficeName = ck5ReportDetails.SourceOfficeName;
            detailRow.SourceOfficeCode = ck5ReportDetails.SourceOfficeCode;
            detailRow.DestPlantNpwp = ck5ReportDetails.DestPlantNpwp;
            detailRow.DestPlantNppbkc = ck5ReportDetails.DestPlantNppbkc;
            detailRow.DestPlantName = ck5ReportDetails.DestPlantName;
            detailRow.DestPlantAddress = ck5ReportDetails.DestPlantAddress;
            detailRow.DestOfficeName = ck5ReportDetails.DestOfficeName;
            detailRow.DestOfficeCode = ck5ReportDetails.DestOfficeCode;
            detailRow.FacilityNumber = ck5ReportDetails.FacilityNumber;
            detailRow.FacilityDate = ck5ReportDetails.FacilityDate;
            detailRow.CarriageMethod = ck5ReportDetails.CarriageMethod;
            detailRow.Total = ck5ReportDetails.Total;
            detailRow.Uom = ck5ReportDetails.Uom;

            detailRow.PrintDate = ck5ReportDetails.PrintDate;
            detailRow.PoaName = ck5ReportDetails.PoaName;
            detailRow.PoaAddress = ck5ReportDetails.PoaAddress;
            detailRow.PoaIdCard = ck5ReportDetails.PoaIdCard;
            detailRow.PoaCity = ck5ReportDetails.PoaCity;
            detailRow.InvoiceNumber = ck5ReportDetails.InvoiceNumber;
            detailRow.InvoiceDate = ck5ReportDetails.InvoiceDate;


            detailRow.TotalMaterial = totalMaterial;

            detailRow.DestinationCountry = ck5ReportDetails.DestinationCountry;
            detailRow.DestinationCode = ck5ReportDetails.DestinationCode;
            detailRow.DestinationNppbkc = ck5ReportDetails.DestinationNppbkc;
            detailRow.DestinationName = ck5ReportDetails.DestinationName;
            detailRow.DestinationAddress = ck5ReportDetails.DestinationAddress;
            detailRow.DestinationOfficeName = ck5ReportDetails.DestinationOfficeName;
            detailRow.DestinationOfficeCode = ck5ReportDetails.DestinationOfficeCode;

            detailRow.LoadingPort = ck5ReportDetails.LoadingPort;
            detailRow.LoadingPortName = ck5ReportDetails.LoadingPortName;
            detailRow.LoadingPortCode = ck5ReportDetails.LoadingPortId;
            detailRow.FinalPort = ck5ReportDetails.FinalPort;
            detailRow.FinalPortName = ck5ReportDetails.FinalPortName;
            detailRow.FinalPortCode = ck5ReportDetails.FinalPortId;

            detailRow.MonthYear = ck5ReportDetails.MonthYear;


            //todo remove
            if (!Utils.ConvertHelper.IsNumeric(detailRow.ExGoodType))
                detailRow.ExGoodType = "3";
            //if (detailRow.CarriageMethod == "0")
            //    detailRow.CarriageMethod = "1";


            detailRow.DocumentText = printTitle;

            dsCk5.dtCk5.AdddtCk5Row(detailRow);

            return dsCk5;
        }

        private dsCK5Print AddDataCk5MaterialRow(dsCK5Print dsCk5, List<CK5ReportMaterialDto> listMaterialDtos)
        {
            int i = 1;
            foreach (var materialDto in listMaterialDtos)
            {
                if (i > 2) break;

                var detailRow = dsCk5.dtCk5Material.NewdtCk5MaterialRow();

                detailRow.Number = i.ToString();
                detailRow.Qty = materialDto.Qty;
                detailRow.Uom = materialDto.Uom;
                detailRow.Convertion = materialDto.Convertion;
                detailRow.ConvertedQty = materialDto.ConvertedQty;
                detailRow.ConvertedUom = materialDto.ConvertedUom;

                detailRow.Hje = materialDto.Hje;
                detailRow.Tariff = materialDto.Tariff;
                detailRow.ExciseValue = materialDto.ExciseValue;
                detailRow.UsdValue = materialDto.UsdValue;
                detailRow.Note = materialDto.Note;
                detailRow.DescMaterial = materialDto.MaterialDescription;

                dsCk5.dtCk5Material.AdddtCk5MaterialRow(detailRow);

                i++;
            }
            return dsCk5;
        }

        private dsCK5Print AddDataCk5MaterialExtendRow(dsCK5Print dsCk5, List<CK5ReportMaterialDto> listMaterialDtos)
        {
            int i = 1;
            foreach (var materialDto in listMaterialDtos)
            {
                if (i > 2)
                {
                    var detailRow = dsCk5.dtCk5MaterialExtend.NewdtCk5MaterialExtendRow();

                    detailRow.Number = i.ToString();
                    detailRow.Qty = materialDto.Qty;
                    detailRow.Uom = materialDto.Uom;
                    detailRow.Convertion = materialDto.Convertion;
                    detailRow.ConvertedQty = materialDto.ConvertedQty;
                    detailRow.ConvertedUom = materialDto.ConvertedUom;

                    detailRow.Hje = materialDto.Hje;
                    detailRow.Tariff = materialDto.Tariff;
                    detailRow.ExciseValue = materialDto.ExciseValue;
                    detailRow.UsdValue = materialDto.UsdValue;
                    detailRow.Note = materialDto.Note;
                    detailRow.DescMaterial = materialDto.MaterialDescription;

                    dsCk5.dtCk5MaterialExtend.AdddtCk5MaterialExtendRow(detailRow);
                }
                i++;
            }
            return dsCk5;
        }


        private DataSet SetDataSetReport(CK5ReportDto ck5ReportDto, string printTitle)
        {
           
            var dsCk5 = new dsCK5Print();
            
           // var ck5ReportDto = _ck5Bll.GetCk5ReportDataById(id);

            var listCk5 = new List<CK5ReportDetailsDto>();
            listCk5.Add(ck5ReportDto.ReportDetails);

            dsCk5 = AddDataCk5Row(dsCk5, ck5ReportDto.ReportDetails, ck5ReportDto.ListMaterials.Count, printTitle);
            dsCk5 = AddDataCk5MaterialRow(dsCk5, ck5ReportDto.ListMaterials);
            if (ck5ReportDto.ListMaterials.Count > 2)
                dsCk5 = AddDataCk5MaterialExtendRow(dsCk5, ck5ReportDto.ListMaterials);

            return dsCk5;
           
        }

        private Stream GetReport(CK5ReportDto ck5Report, string printTitle)
        {
            var dataSet = SetDataSetReport(ck5Report, printTitle);

            ReportClass rpt = new ReportClass
            {
                FileName = ConfigurationManager.AppSettings["Report_Path"] + "CK5\\CK5PrintOut.rpt"
                //FileName = Server.MapPath("/Reports/CK5/CK5PrintOut.rpt")
            };
            rpt.Load();
            rpt.SetDataSource(dataSet);
            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return stream;
        }

        [EncryptedParameter]
        public ActionResult PrintPreview(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            var ck5Data = _ck5Bll.GetCk5ReportDataById(id.Value);
            if (ck5Data == null)
                HttpNotFound();

            Stream stream = GetReport(ck5Data, "CK5 PREVIEW");

            return File(stream, "application/pdf");
        }

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            try
            {
              
                if (!id.HasValue)
                    HttpNotFound();

                var ck5Data = _ck5Bll.GetCk5ReportDataById(id.Value);
                if (ck5Data == null)
                    HttpNotFound();

                Stream stream = GetReport(ck5Data, string.Empty);

                return File(stream, "application/pdf");

                //var dataSet = GetDataSetReport(idCk5, string.Empty);

                //var rpt = new ReportClass
                //{
                //    FileName = Server.MapPath("/Reports/CK5/CK5PrintOut.rpt")
                //};
                //rpt.Load();
                //rpt.SetDataSource(dataSet);

                //Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //return File(stream, "application/pdf"); 
            }
            catch (Exception ex)
            {
                AddMessageInfo("Error : " + ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
           
        }

        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var ck5Data = _ck5Bll.GetById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.CK5,
                FORM_ID = ck5Data.CK5_ID,
                FORM_NUMBER = ck5Data.SUBMISSION_NUMBER,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(ck5Data.SUBMISSION_NUMBER));
            return PartialView("_PrintHistoryTable", model);

        }
        #endregion

        #region Upload File Documents

        public ActionResult CK5UploadFileDocuments()
        {
            var model = new CK5FileDocumentsViewModel();
            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

            return View("CK5UploadFileDocument", model);
        }

        [HttpPost]
        public PartialViewResult UploadFileDocuments(HttpPostedFileBase itemExcelFile)
        {
            var data = (new ExcelReader()).ReadExcelCk5FileDocuments(itemExcelFile);
            var model = new CK5FileDocumentsViewModel();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var uploadItem = new CK5FileDocumentItems();

                    try
                    {
                        uploadItem.DocSeqNumber = datarow[0];
                        uploadItem.MatNumber = datarow[1];
                        uploadItem.Qty = datarow[2];
                        uploadItem.UomMaterial = datarow[3];
                        uploadItem.Convertion = datarow[4];
                        uploadItem.ConvertedUom = datarow[5];
                        uploadItem.UsdValue = datarow[6];
                        uploadItem.Note = datarow[7];
                        
                        uploadItem.Ck5Type = datarow[8];
                        uploadItem.KppBcCityName = datarow[9];
                        uploadItem.ExGoodType = datarow[10];
                        uploadItem.ExciseSettlement = datarow[11];
                        uploadItem.ExciseStatus = datarow[12];
                        uploadItem.RequestType = datarow[13];
                        uploadItem.SourcePlantId = datarow[14];
                        uploadItem.DestPlantId = datarow[15];
                        uploadItem.InvoiceNumber = datarow[16];
                        uploadItem.InvoiceDateDisplay = datarow[17];
                        uploadItem.PbckDecreeNumber = datarow[18];
                        uploadItem.CarriageMethod = datarow[19];
                        uploadItem.GrandTotalEx = datarow[20];
                        uploadItem.Uom = datarow[21];

                        uploadItem.LOADING_PORT = datarow[22];
                        uploadItem.LOADING_PORT_NAME = datarow[23];
                        uploadItem.LOADING_PORT_ID = datarow[24];
                        uploadItem.FINAL_PORT = datarow[25];
                        uploadItem.FINAL_PORT_NAME = datarow[26];
                        uploadItem.FINAL_PORT_ID = datarow[27];
                        uploadItem.DEST_COUNTRY_CODE = datarow[28];
                     

                        model.Ck5FileDocumentItems.Add(uploadItem);

                    }
                    catch (Exception)
                    {
                        continue;

                    }

                }
            }

            var input = Mapper.Map<List<CK5UploadFileDocumentsInput>>(model.Ck5FileDocumentItems);

            List<CK5FileUploadDocumentsOutput> outputResult;
            outputResult = _ck5Bll.CK5UploadFileDocumentsProcess(input);

            model.Ck5FileDocumentItems = Mapper.Map<List<CK5FileDocumentItems>>(outputResult);

            return PartialView("_CK5UploadFileDocumentsList", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCK5FileDocuments(CK5FileDocumentsViewModel model)
        {
            try
            {

                var dataToSave = Mapper.Map<List<CK5FileDocumentDto>>(model.Ck5FileDocumentItems);

                var input = new CK5SaveListInput()
                {
                    ListCk5UploadDocumentDto = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    UserRole = CurrentUser.UserRole,
                   
                };

                _ck5Bll.InsertListCk5(input);
              
                AddMessageInfo("Success create CK5", Enums.MessageInfoType.Success);


                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
                return View("CK5UploadFileDocument", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                return RedirectToAction("Index", "CK5");
            }

        }

        #endregion


        #region "Create XML"

      
        #endregion

        #region "Input Materials"

        public ActionResult InputMaterials()
        {
            var model = new CK5InputManualViewModel();
            //model.MainMenu = Enums.MenuList.CK5;
            //model.CurrentMenu = PageInfo;
          
            return View("CK5InputMaterial", model);
        }

        [HttpPost]
        public JsonResult GetListMaterials(string plantId, Enums.CK5Type ck5Type)
        {

            var dbMaterial = _materialBll.GetMaterialByPlantId(plantId);
            var model = Mapper.Map<List<CK5InputManualViewModel>>(dbMaterial);

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetMaterialHjeAndTariff(string plantId, string materialNumber)
        {

            var dbMaterial = _materialBll.GetMaterialByPlantIdAndMaterialNumber(plantId, materialNumber);
            var model = Mapper.Map<CK5InputManualViewModel>(dbMaterial);

            //model.Hje = dbMaterial.HJE.HasValue ? dbMaterial.HJE.Value : 0;
            //model.Tariff = dbMaterial.TARIFF.HasValue ? dbMaterial.TARIFF.Value : 0;
            return Json(model);
        }

        #endregion
    }
}