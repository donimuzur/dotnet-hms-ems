﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using NLog.LayoutRenderers;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;
using SpreadsheetLight;


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
      
        public CK5Controller(IPageBLL pageBLL, ICK5BLL ck5Bll,  IPBCK1BLL pbckBll, 
            IWorkflowHistoryBLL workflowHistoryBll,IChangesHistoryBLL changesHistoryBll,
            IWorkflowBLL workflowBll, IPlantBLL plantBll)
            : base(pageBLL, Enums.MenuList.CK5)
        {
            _ck5Bll = ck5Bll;
            _pbck1Bll = pbckBll;
            _workflowHistoryBll = workflowHistoryBll;
            _changesHistoryBll = changesHistoryBll;
            _workflowBll = workflowBll;
            _plantBll = plantBll;
         
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

            var listCk5Dto = _ck5Bll.GetAll();
            model.SearchView.DocumentNumberList = new SelectList(listCk5Dto, "SUBMISSION_NUMBER", "SUBMISSION_NUMBER");
          
            model.SearchView.POAList = GlobalFunctions.GetPoaAll();
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();

            model.SearchView.NPPBKCOriginList = GlobalFunctions.GetNppbkcAll();
            model.SearchView.NPPBKCDestinationList = GlobalFunctions.GetNppbkcAll();


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

        //public ActionResult CK5DomesticAlcohol()
        //{
        //    var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.DomesticAlcohol);
        //    return View(model);
        //}

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
            model.GoodTypeList = GlobalFunctions.GetGoodTypeGroupListByDescValue();
          
            model.SourcePlantList = GlobalFunctions.GetSourcePlantList();
            model.DestPlantList = GlobalFunctions.GetSourcePlantList();

            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();
          
            model.PackageUomList = GlobalFunctions.GetUomList();

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
            //AddMessageInfo("Create domestic.", Enums.MessageInfoType.Info);
            return View("Create", model);
        }

        public ActionResult CreateIntercompany()
        {
            var model = InitCreateCK5(Enums.CK5Type.Intercompany);
            return View("Create", model);
        }

    

        public ActionResult CreatePortToImporter()
        {
            var model = InitCreateCK5(Enums.CK5Type.PortToImporter);
            return View("Create", model);
        }

        public ActionResult CreateImporterToPlant()
        {
            var model = InitCreateCK5(Enums.CK5Type.ImporterToPlant);
            return View("Create", model);
        }

        public ActionResult CreateExport()
        {
            var model = InitCreateCK5(Enums.CK5Type.Export);
            model.IsCk5Export = true;
            return View("Create", model);
        }

        public ActionResult CreateManual()
        {
            var model = InitCreateCK5(Enums.CK5Type.Manual);
            model.IsCk5Manual = true;
            return View("Create", model);
        }

        public ActionResult CreateDomesticAlcohol()
        {
            var model = InitCreateCK5(Enums.CK5Type.DomesticAlcohol);
            return View("Create", model);
        }

        //[HttpPost]
        //public JsonResult CeOfficeCodePartial(string nppBkcCityId)
        //{
        //    //todo check
        //    var ceOfficeCode = _nppbkcBll.GetCeOfficeCodeByNppbcId(nppBkcCityId);
        //    return Json(ceOfficeCode);
        //}

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
            model.GoodTypeList = GlobalFunctions.GetGoodTypeGroupListByDescValue();

            model.SourcePlantList = GlobalFunctions.GetSourcePlantList();
            model.DestPlantList = GlobalFunctions.GetSourcePlantList();
            
            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();

            model.PackageUomList = GlobalFunctions.GetUomList();

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

        public ActionResult SummaryReports()
        {
            CK5SummaryReportsViewModel model;
            try
            {

                model = new CK5SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;

                model.SearchView.CompanyCodeList = GlobalFunctions.GetCompanyList();
                model.SearchView.YearFromList = GetYearListCK5(true);
                model.SearchView.YearToList = GetYearListCK5(false);
                model.SearchView.NppbkcIdList = GlobalFunctions.GetNppbkcAll();

                //view all data ck5 completed document
                model.DetailsList = SearchSummaryReports();

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

        private List<CK5SummaryReportsItem> SearchSummaryReports(CK5SearchSummaryReportsViewModel filter = null)
        {
            CK5GetByParamInput input;
            List<CK5Dto> dbData;
            if (filter == null)
            {
                //Get All
                input = new CK5GetByParamInput();
                input.Ck5Type = Enums.CK5Type.Completed;

                dbData = _ck5Bll.GetCK5ByParam(input);
                return Mapper.Map<List<CK5SummaryReportsItem>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<CK5GetByParamInput>(filter);
            input.Ck5Type = Enums.CK5Type.Completed;

            dbData = _ck5Bll.GetCK5ByParam(input);
            return Mapper.Map<List<CK5SummaryReportsItem>>(dbData);
        }
        private SelectList GetYearListCK5(bool isFrom)
        {
            var listCk5 = _ck5Bll.GetAll();

            IEnumerable<SelectItemModel> query;
            if (isFrom)
                query = from x in listCk5.Where(c => c.SUBMISSION_DATE != null).OrderByDescending(c => c.SUBMISSION_DATE) 
                select new Models.SelectItemModel()
                {
                    ValueField = x.SUBMISSION_DATE.Value.Year,
                    TextField = x.SUBMISSION_DATE.Value.ToString("yyyy")
                };
            else
                query = from x in listCk5.Where(c => c.SUBMISSION_DATE != null).OrderByDescending(c => c.SUBMISSION_DATE) 
                    select new Models.SelectItemModel()
                    {
                        ValueField = x.SUBMISSION_DATE.Value.Year,
                        TextField = x.SUBMISSION_DATE.Value.ToString("yyyy")
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        [HttpPost]
        public PartialViewResult SearchSummaryReports(CK5SummaryReportsViewModel model)
        {
            model.DetailsList = SearchSummaryReports(model.SearchView);
            return PartialView("_CK5ListSummaryReport", model);
        }

        public ActionResult ExportSummaryReports(CK5SummaryReportsViewModel model)
        {
            try
            {
                //CK5Workflow(model.Ck5Id, Enums.ActionType.GovReject, model.Comment);
                //AddMessageInfo("Success GovReject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("SummaryReports");
        }

        public void ExportXlsSummaryReports(long ck5Id)
        {
          
            var pathFile = CreateXlsFileSummaryReports(ck5Id);
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

        private string CreateXlsFileSummaryReports(long ck5Id)
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
            var fileName = "CK5" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Content/upload/"), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        #endregion
    }
}