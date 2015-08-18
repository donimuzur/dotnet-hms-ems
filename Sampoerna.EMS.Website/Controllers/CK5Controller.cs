using System;
using System.Collections.Generic;
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
        public CK5Controller(IPageBLL pageBLL, IPOABLL poabll, IZaidmExNPPBKCBLL nppbckbll, ICK5BLL ck5Bll,  IPBCK1BLL pbckBll, 
            IWorkflowHistoryBLL workflowHistoryBll,IChangesHistoryBLL changesHistoryBll,
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
            model = InitCK5List(model);

            return model;
        }

        private CK5FormViewModel InitCK5List(CK5FormViewModel model)
        {

            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

            model.KppBcCityList = GlobalFunctions.GetKppBcCityList();

           //model.SourcePlantList = GlobalFunctions.GetSourcePlantList();
            //model.DestPlantList = GlobalFunctions.GetSourcePlantList();
            model.SourcePlantList = GlobalFunctions.GetPlantAll();
            model.DestPlantList = GlobalFunctions.GetPlantAll();

            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();
          
            model.PackageUomList = GlobalFunctions.GetUomList();

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

        [HttpPost]
        public JsonResult GetCompanyCode(string nppBkcCityId)
        {
            var companyCode = "";
            var data = GlobalFunctions.GetNppbkcById(nppBkcCityId);
            if (data != null)
                companyCode = data.BUKRS;
            return Json(companyCode);
        }

        [HttpPost]
        public JsonResult GetSourcePlantDetails(string plantId)
        {
            var dbPlant = _plantBll.GetT001ById(plantId);
            var model = Mapper.Map<CK5PlantModel>(dbPlant);
            return Json(model);
        }

        [HttpPost]
        public JsonResult Pbck1DatePartial(long pbck1Id)
        {
            //var pbck1 = _pbck1Bll.GetById(pbck1Id);

            //return Json(pbck1.DECREE_DATE.HasValue ? pbck1.DECREE_DATE.Value.ToString("dd/MM/yyyy"):string.Empty);
            return Json(GetDatePbck1ByPbckId(pbck1Id));
        }

        private string GetDatePbck1ByPbckId(long? id)
        {
            if (id == null)
                return string.Empty;

            var pbck1 = _pbck1Bll.GetById(id.Value);
            if (pbck1.DecreeDate.HasValue)
                return pbck1.DecreeDate.Value.ToString("dd/MM/yyyy");

            return string.Empty;
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

            model.KppBcCityList = GlobalFunctions.GetKppBcCityList();
            
            
            //model.SourcePlantList = GlobalFunctions.GetSourcePlantList();
            //model.DestPlantList = GlobalFunctions.GetSourcePlantList();
            model.SourcePlantList = GlobalFunctions.GetPlantAll();
            model.DestPlantList = GlobalFunctions.GetPlantAll();

            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();

            model.PackageUomList = GlobalFunctions.GetUomList();

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
                        //validate
                        var input = new WorkflowAllowEditAndSubmitInput();
                        input.DocumentStatus = model.DocumentStatus;
                        input.CreatedUser = model.CreatedBy;
                        input.CurrentUser = CurrentUser.USER_ID;
                        if (_workflowBll.AllowEditDocument(input))
                        {

                            SaveCk5ToDatabase(model);

                            AddMessageInfo("Success", Enums.MessageInfoType.Success);
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
                }
               
                //gov approval purpose
                if (model.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval)
                    model.KppBcCity = model.KppBcCityName;

                model.IsAllowPrint = _workflowBll.AllowPrint(model.DocumentStatus);
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
                            var ck5UploadFile = new CK5FileUploadViewModel
                            {
                                FILE_NAME = item.FileName,
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

                CK5WorkflowGovApproval(model);
                AddMessageInfo("Success Gov Approve Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "CK5", new { id = model.Ck5Id });
        }

        private void  CK5WorkflowGovApproval(CK5FormViewModel model)
        {
            //var input = new CK5WorkflowDocumentInput();
            //input.DocumentId = id;
            //input.UserId = CurrentUser.USER_ID;
            //input.UserRole = CurrentUser.UserRole;
            //input.ActionType = actionType;
            //input.Comment = comment;
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


            var listCk5 = _ck5Bll.GetCk5CompletedByCk5Type(model.Ck5Type);
          
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
            if (model.Ck5Type == Enums.CK5Type.Domestic)
                return PartialView("_CK5ListSummaryReport", model);
            if (model.Ck5Type == Enums.CK5Type.Intercompany || model.Ck5Type == Enums.CK5Type.DomesticAlcohol)
                return PartialView("_CK5ListSummaryReportIntercompany", model);
            if (model.Ck5Type == Enums.CK5Type.PortToImporter || model.Ck5Type == Enums.CK5Type.ImporterToPlant)
                return PartialView("_CK5ListSummaryReportImport", model);
            if (model.Ck5Type == Enums.CK5Type.Manual)
                return PartialView("_CK5ListSummaryManual", model);
             
            return PartialView("_CK5ListSummaryReportExport", model);
        }

        public void ExportXlsSummaryReports(CK5SummaryReportsViewModel model)
        {
            string pathFile = "";

            //var pathFile = CreateXlsFileSummaryReports(modelExport);

            if (model.Ck5Type == Enums.CK5Type.Domestic)
                pathFile = CreateXlsSummaryReportsDomesticType(model.ExportModel);
            else if (model.Ck5Type == Enums.CK5Type.Intercompany || model.Ck5Type == Enums.CK5Type.DomesticAlcohol)
                pathFile = CreateXlsSummaryReportsIntercompanyType(model.ExportModel);
            else if (model.Ck5Type == Enums.CK5Type.PortToImporter || model.Ck5Type == Enums.CK5Type.ImporterToPlant)
                pathFile = CreateXlsSummaryReportsExportType(model.ExportModel);
            else if (model.Ck5Type == Enums.CK5Type.Export)
                pathFile = CreateXlsSummaryReportsExportType(model.ExportModel);
            else if (model.Ck5Type == Enums.CK5Type.Manual)
                pathFile = CreateXlsSummaryReportsManualType(model.ExportModel);


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

        #region domestic

        private string CreateXlsSummaryReportsDomesticType(CK5ExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataSummaryReports(modelExport);
            
            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelDomesticType(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.ExciseStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExciseStatus);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Pbck1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Pbck1Number);
                    iColumn = iColumn + 1;
                }

                if (modelExport.PbckDecreeDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.SealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.UnSealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotifDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnSealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotifNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Lack1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack1Number);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Lack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }


                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelDomesticType(SLDocument slDocument, CK5ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.ExciseStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Status");
                iColumn = iColumn + 1;
            }
            if (modelExport.Pbck1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.PbckDecreeDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.SealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.SealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.UnSealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unsealing Notification Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnSealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unsealing Notification Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.Lack1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-1 Month");
                iColumn = iColumn + 1;
            }

            if (modelExport.Lack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-2 Month");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }

        #endregion

        #region Intercompany
        private string CreateXlsSummaryReportsIntercompanyType(CK5ExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataSummaryReports(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelIntercompanyType(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.SubmissionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.SubmissionNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExGoodTypeDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExGoodTypeDesc);
                    iColumn = iColumn + 1;
                }
                if (modelExport.RequestType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceKppbcName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceNppbkcId);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.SourceCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DestinationCountry)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestinationCountry);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TypeOfTobaccoProduct)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TypeOfTobaccoProduct);
                    iColumn = iColumn + 1;
                }

                if (modelExport.GrandTotal)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.GrandTotal);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ContainBox)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ContainBox);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TotalExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TotalExcisableGoods);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Hje)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Hje);
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

                if (modelExport.ForeignExchange)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ForeignExchange);
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

                if (modelExport.Pbck1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Pbck1Number);
                    iColumn = iColumn + 1;
                }

                if (modelExport.PbckDecreeDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PbckDecreeDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestNameAdress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestNameAdress);
                    iColumn = iColumn + 1;
                }

                //if (modelExport.DestNppbkcId)
                //{
                //    slDocument.SetCellValue(iRow, iColumn, data.DestNppbkcId);
                //    iColumn = iColumn + 1;
                //}

                //if (modelExport.DestKppbcName)
                //{
                //    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                //    iColumn = iColumn + 1;
                //}

                //if (modelExport.LoadingPort)
                //{
                //    slDocument.SetCellValue(iRow, iColumn, data.LoadingPort);
                //    iColumn = iColumn + 1;
                //}
                //if (modelExport.LoadingPortOffice)
                //{
                //    slDocument.SetCellValue(iRow, iColumn, data.LoadingPortOffice);
                //    iColumn = iColumn + 1;
                //}
                if (modelExport.SealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Lack1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack1Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Lack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelIntercompanyType(SLDocument slDocument, CK5ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.SubmissionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Aju");
                iColumn = iColumn + 1;
            }
            if (modelExport.SubmissionNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomer Aju");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Pendaftaran");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomor Pendaftaran");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExGoodTypeDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }
            if (modelExport.RequestType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Request");
                iColumn = iColumn + 1;
            }
            if (modelExport.SourceKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin CE Office");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys Address");
                iColumn = iColumn + 1;
            }
            //start

            if (modelExport.DestinationCountry)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Country");
                iColumn = iColumn + 1;
            }
            if (modelExport.TypeOfTobaccoProduct)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Tobacco Product");
                iColumn = iColumn + 1;
            }

            if (modelExport.GrandTotal)
            {
                slDocument.SetCellValue(iRow, iColumn, "Number of Box");
                iColumn = iColumn + 1;
            }

            if (modelExport.ContainBox)
            {
                slDocument.SetCellValue(iRow, iColumn, "Contain per Box");
                iColumn = iColumn + 1;
            }
            if (modelExport.TotalExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Total of Excisable Goods");
                iColumn = iColumn + 1;
            }
            if (modelExport.Hje)
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

            if (modelExport.ForeignExchange)
            {
                slDocument.SetCellValue(iRow, iColumn, "Foreign Exchange");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExciseSettlement)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excise Settlement");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExciseStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.Pbck1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.PbckDecreeDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination CE Office");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestNameAdress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Storage Location");
                iColumn = iColumn + 1;
            }

            //if (modelExport.DestNppbkcId)
            //{
            //    slDocument.SetCellValue(iRow, iColumn, "Last Storage Locations NPPBKC");
            //    iColumn = iColumn + 1;
            //}

            //if (modelExport.DestKppbcName)
            //{
            //    slDocument.SetCellValue(iRow, iColumn, "Last Storage Location's CE Office");
            //    iColumn = iColumn + 1;
            //}

            //if (modelExport.LoadingPort)
            //{
            //    slDocument.SetCellValue(iRow, iColumn, "Loading Port");
            //    iColumn = iColumn + 1;
            //}
            //if (modelExport.LoadingPortOffice)
            //{
            //    slDocument.SetCellValue(iRow, iColumn, "Loading Port CE Office");
            //    iColumn = iColumn + 1;
            //}
            if (modelExport.SealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.SealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.Lack1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-1 Month");
                iColumn = iColumn + 1;
            }
            if (modelExport.Lack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-2 Month");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }


        #endregion
        
        #region Export

        private string CreateXlsSummaryReportsExportType(CK5ExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataSummaryReports(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelExportType(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.SubmissionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.SubmissionNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExGoodTypeDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExGoodTypeDesc);
                    iColumn = iColumn + 1;
                }
                if (modelExport.RequestType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceKppbcName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceNppbkcId);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.SourceCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DestinationCountry)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestinationCountry);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TypeOfTobaccoProduct)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TypeOfTobaccoProduct);
                    iColumn = iColumn + 1;
                }

                if (modelExport.GrandTotal)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.GrandTotal);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ContainBox)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ContainBox);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TotalExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TotalExcisableGoods);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Hje)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Hje);
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

                if (modelExport.ForeignExchange)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ForeignExchange);
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

                if (modelExport.Pbck1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Pbck1Number);
                    iColumn = iColumn + 1;
                }

                if (modelExport.PbckDecreeDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PbckDecreeDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestNameAdress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestNameAdress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DestNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestNppbkcId);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DestKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.LoadingPort)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LoadingPort);
                    iColumn = iColumn + 1;
                }
                if (modelExport.LoadingPortOffice)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LoadingPortOffice);
                    iColumn = iColumn + 1;
                }
                if (modelExport.SealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Lack1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack1Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Lack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }
               
                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelExportType(SLDocument slDocument, CK5ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.SubmissionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Aju");
                iColumn = iColumn + 1;
            }
            if (modelExport.SubmissionNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomer Aju");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Pendaftaran");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomor Pendaftaran");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExGoodTypeDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }
            if (modelExport.RequestType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Request");
                iColumn = iColumn + 1;
            }
            if (modelExport.SourceKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin CE Office");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys Address");
                iColumn = iColumn + 1;
            }
            //start

            if (modelExport.DestinationCountry)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Country");
                iColumn = iColumn + 1;
            }
            if (modelExport.TypeOfTobaccoProduct)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Tobacco Product");
                iColumn = iColumn + 1;
            }

            if (modelExport.GrandTotal)
            {
                slDocument.SetCellValue(iRow, iColumn, "Number of Box");
                iColumn = iColumn + 1;
            }

            if (modelExport.ContainBox)
            {
                slDocument.SetCellValue(iRow, iColumn, "Contain per Box");
                iColumn = iColumn + 1;
            }
            if (modelExport.TotalExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Total of Excisable Goods");
                iColumn = iColumn + 1;
            }
            if (modelExport.Hje)
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

            if (modelExport.ForeignExchange)
            {
                slDocument.SetCellValue(iRow, iColumn, "Foreign Exchange");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExciseSettlement)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excise Settlement");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExciseStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.Pbck1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.PbckDecreeDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination CE Office");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestNameAdress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Storage Location");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Storage Locations NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Storage Location's CE Office");
                iColumn = iColumn + 1;
            }

            if (modelExport.LoadingPort)
            {
                slDocument.SetCellValue(iRow, iColumn, "Loading Port");
                iColumn = iColumn + 1;
            }
            if (modelExport.LoadingPortOffice)
            {
                slDocument.SetCellValue(iRow, iColumn, "Loading Port CE Office");
                iColumn = iColumn + 1;
            }
            if (modelExport.SealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.SealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.Lack1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-1 Month");
                iColumn = iColumn + 1;
            }
            if (modelExport.Lack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-2 Month");
                iColumn = iColumn + 1;
            }
            
            return slDocument;

        }


        #endregion

        #region Import

        private string CreateXlsSummaryReportsImportType(CK5ExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataSummaryReports(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelImportType(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.SubmissionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.SubmissionNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationNumber);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExGoodTypeDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExGoodTypeDesc);
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

                if (modelExport.SourceKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceKppbcName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceNppbkcId);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.SourceCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DestKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestNppbkcId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestCompanyAddress);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TypeOfTobaccoProduct)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TypeOfTobaccoProduct);
                    iColumn = iColumn + 1;
                }

                if (modelExport.GrandTotal)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.GrandTotal);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ContainBox)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ContainBox);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TotalExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TotalExcisableGoods);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Hje)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Hje);
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

              
                if (modelExport.ExciseSettlement)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExciseSettlement);
                    iColumn = iColumn + 1;
                }
               

                if (modelExport.Pbck1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Pbck1Number);
                    iColumn = iColumn + 1;
                }

                if (modelExport.PbckDecreeDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PbckDecreeDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                    iColumn = iColumn + 1;
                }
                
                if (modelExport.SealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnSealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotifDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnSealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotifNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Lack1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack1Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Lack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelImportType(SLDocument slDocument, CK5ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.SubmissionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Aju");
                iColumn = iColumn + 1;
            }
            if (modelExport.SubmissionNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomer Aju");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Pendaftaran");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomor Pendaftaran");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExGoodTypeDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }

            if (modelExport.ExciseStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.RequestType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Request");
                iColumn = iColumn + 1;
            }
            if (modelExport.SourceKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin CE Office");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys Address");
                iColumn = iColumn + 1;
            }
            //start

            if (modelExport.DestKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination's NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Company Address");
                iColumn = iColumn + 1;
            }
            if (modelExport.TypeOfTobaccoProduct)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Tobacco Product");
                iColumn = iColumn + 1;
            }

            if (modelExport.GrandTotal)
            {
                slDocument.SetCellValue(iRow, iColumn, "Number of Box");
                iColumn = iColumn + 1;
            }

            if (modelExport.ContainBox)
            {
                slDocument.SetCellValue(iRow, iColumn, "Contain per Box");
                iColumn = iColumn + 1;
            }
            if (modelExport.TotalExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Total of Excisable Goods");
                iColumn = iColumn + 1;
            }
            if (modelExport.Hje)
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

          
            if (modelExport.ExciseSettlement)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excise Settlement");
                iColumn = iColumn + 1;
            }

            if (modelExport.Pbck1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.PbckDecreeDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination CE Office");
                iColumn = iColumn + 1;
            }
           
            if (modelExport.SealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.SealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnSealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "UnSealing Notification Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnSealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "UnSealing Notification Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.Lack1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-1 Month");
                iColumn = iColumn + 1;
            }
            if (modelExport.Lack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-2 Month");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }


        #endregion

        #region Manual

        private string CreateXlsSummaryReportsManualType(CK5ExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataSummaryReports(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelManualType(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.SubmissionDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.SubmissionNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubmissionNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RegistrationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RegistrationNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RequestType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ExGoodTypeDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExGoodTypeDesc);
                    iColumn = iColumn + 1;
                }
               
               

                if (modelExport.SourceKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceKppbcName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SourceNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceNppbkcId);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.SourceCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SourceCompanyAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DestKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestCompanyName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestNppbkcId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestCompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestCompanyAddress);
                    iColumn = iColumn + 1;
                }
                if (modelExport.ExGoodTypeDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExGoodTypeDesc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.GrandTotal)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.GrandTotal);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ContainBox)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ContainBox);
                    iColumn = iColumn + 1;
                }
                if (modelExport.TotalExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TotalExcisableGoods);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Hje)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Hje);
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

                if (modelExport.Pbck1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Pbck1Number);
                    iColumn = iColumn + 1;
                }

                if (modelExport.PbckDecreeDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PbckDecreeDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.DestKppbcName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DestKppbcName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SealingNotifNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnSealingNotifDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotifDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.UnSealingNotifNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.UnSealingNotifNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Lack1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack1Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Lack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelManualType(SLDocument slDocument, CK5ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.SubmissionDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Aju");
                iColumn = iColumn + 1;
            }
            if (modelExport.SubmissionNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomer Aju");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tanggal Pendaftaran");
                iColumn = iColumn + 1;
            }

            if (modelExport.RegistrationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Nomor Pendaftaran");
                iColumn = iColumn + 1;
            }

            if (modelExport.RequestType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Request");
                iColumn = iColumn + 1;
            }

            if (modelExport.ExGoodTypeDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }

           
            if (modelExport.SourceKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin CE Office");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.SourceCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Origin Companys Address");
                iColumn = iColumn + 1;
            }
            //start

            if (modelExport.DestKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination CE Office");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination's NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.DestCompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination Company Address");
                iColumn = iColumn + 1;
            }
            if (modelExport.ExGoodTypeDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }

            if (modelExport.GrandTotal)
            {
                slDocument.SetCellValue(iRow, iColumn, "Number of Box");
                iColumn = iColumn + 1;
            }

            if (modelExport.ContainBox)
            {
                slDocument.SetCellValue(iRow, iColumn, "Contain per Box");
                iColumn = iColumn + 1;
            }
            if (modelExport.TotalExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Total of Excisable Goods");
                iColumn = iColumn + 1;
            }
            if (modelExport.Hje)
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


            if (modelExport.ExciseSettlement)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excise Settlement");
                iColumn = iColumn + 1;
            }

            if (modelExport.ExciseStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.Pbck1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.PbckDecreeDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Unpaid Excise Facility Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.DestKppbcName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Destination CE Office");
                iColumn = iColumn + 1;
            }

            if (modelExport.SealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.SealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sealing Notification Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnSealingNotifDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "UnSealing Notification Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.UnSealingNotifNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "UnSealing Notification Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.Lack1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-1 Month");
                iColumn = iColumn + 1;
            }
            if (modelExport.Lack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Reported to LACK-2 Month");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }


        #endregion

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


        private dsCK5Print AddDataCk5Row(dsCK5Print dsCk5, CK5ReportDetailsDto ck5ReportDetails, int totalMaterial)
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


            //todo remove
            if (!Utils.ConvertHelper.IsNumeric(detailRow.ExGoodType))
                detailRow.ExGoodType = "3";
            //if (detailRow.CarriageMethod == "0")
            //    detailRow.CarriageMethod = "1";

          

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

                    dsCk5.dtCk5MaterialExtend.AdddtCk5MaterialExtendRow(detailRow);
                }
                i++;
            }
            return dsCk5;
        }
        

        private DataSet GetDataSetReport(long id)
        {
           
            var dsCk5 = new dsCK5Print();
            
            var ck5ReportDto = _ck5Bll.GetCk5ReportDataById(id);

            var listCk5 = new List<CK5ReportDetailsDto>();
            listCk5.Add(ck5ReportDto.ReportDetails);

            dsCk5 = AddDataCk5Row(dsCk5, ck5ReportDto.ReportDetails, ck5ReportDto.ListMaterials.Count);
            dsCk5 = AddDataCk5MaterialRow(dsCk5, ck5ReportDto.ListMaterials);
            if (ck5ReportDto.ListMaterials.Count > 2)
                dsCk5 = AddDataCk5MaterialExtendRow(dsCk5, ck5ReportDto.ListMaterials);

            return dsCk5;
           
        }

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            try
            {
                long idCk5 = 0;
                if (id.HasValue)
                    idCk5 = id.Value;

                var dataSet = GetDataSetReport(idCk5);

                //add print history
                //_ck5Bll.AddPrintHistory(idCk5, CurrentUser.USER_ID);

                //eks to report
                var rpt = new ReportClass
                {
                    FileName = Server.MapPath("/Reports/CK5/CK5PrintOut.rpt")
                };
                rpt.Load();
                rpt.SetDataSource(dataSet);

                Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf"); 
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
    }
}