﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.PBCK4;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;
using SpreadsheetLight;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK4Controller : BaseController
    {
        private IPOABLL _poaBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IPBCK4BLL _pbck4Bll;
        private IPlantBLL _plantBll;
        private IWorkflowBLL _workflowBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;

        public PBCK4Controller(IPageBLL pageBLL, IPOABLL poabll, IZaidmExNPPBKCBLL nppbkcBll,
            IPBCK4BLL pbck4Bll, IPlantBLL plantBll, IWorkflowBLL workflowBll, IChangesHistoryBLL changesHistoryBll,
            IWorkflowHistoryBLL workflowHistoryBll)
            : base(pageBLL, Enums.MenuList.PBCK4)
        {
            _poaBll = poabll;
            _nppbkcBll = nppbkcBll;
            _pbck4Bll = pbck4Bll;
            _plantBll = plantBll;
            _workflowBll = workflowBll;
            _changesHistoryBll = changesHistoryBll;
            _workflowHistoryBll = workflowHistoryBll;
        }

        //
        // GET: /PBCK4/
        public ActionResult Index()
        {

            Pbck4IndexViewModel model;
            try
            {
                model = CreateInitModelView();


            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck4IndexViewModel();
            }

            return View(model);
        }

        private Pbck4IndexViewModel CreateInitModelView()
        {
            var model = new Pbck4IndexViewModel();

            model.MainMenu = Enums.MenuList.PBCK4;
            model.CurrentMenu = PageInfo;
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;

            //var listPbck4 = _ck5Bll.GetAll();
            //model.SearchView.DocumentNumberList = new SelectList(listCk5Dto, "SUBMISSION_NUMBER", "SUBMISSION_NUMBER");

            model.SearchView.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.SearchView.PlantIdList = GlobalFunctions.GetPlantAll();

            model.SearchView.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();

            
            //list table
            model.DetailsList = GetPbck4Items();

            return model;
        }

        private List<Pbck4Item> GetPbck4Items(Pbck4SearchViewModel filter = null)
        {
            Pbck4GetByParamInput input;
            List<Pbck4Dto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Pbck4GetByParamInput();
                
                dbData = _pbck4Bll.GetPbck4ByParam(input);
                return Mapper.Map<List<Pbck4Item>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<Pbck4GetByParamInput>(filter);

            dbData = _pbck4Bll.GetPbck4ByParam(input);
            return Mapper.Map<List<Pbck4Item>>(dbData);
        }

        [HttpPost]
        public PartialViewResult Filter(Pbck4IndexViewModel model)
        {
            model.DetailsList = GetPbck4Items(model.SearchView);
            return PartialView("_Pbck4OpenListDocuments", model);
        }

        public ActionResult Create()
        {

            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                AddMessageInfo("Can't create PBCK-4 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new Pbck4FormViewModel();
         

            model.DocumentStatus = Enums.DocumentStatus.Draft;
            model.ReportedOn = DateTime.Now;
            model = InitListPbck4(model);
         

            return View("Create", model);
        }

        private Pbck4FormViewModel InitListPbck4(Pbck4FormViewModel model)
        {
            model.MainMenu = Enums.MenuList.PBCK4;
            model.CurrentMenu = PageInfo;

            model.PlantList = GlobalFunctions.GetPlantAll();

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePbck4(Pbck4FormViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if (model.UploadItemModels.Count > 0)
                    //{

                        var saveResult = SavePbck4ToDatabase(model);

                        AddMessageInfo("Success create PBCK-4", Enums.MessageInfoType.Success);


                        //return RedirectToAction("Edit", "CK5", new { @id = saveResult.CK5_ID });
                 //   }

                   // AddMessageInfo("Missing CK5 Material", Enums.MessageInfoType.Error);
                }
                else
                    AddMessageInfo("Not Valid Model", Enums.MessageInfoType.Error);

                model = InitListPbck4(model);

                return View("Create", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitListPbck4(model);

                return View("Create", model);
            }

        }

        private Pbck4Dto SavePbck4ToDatabase(Pbck4FormViewModel model)
        {
          
            var dataToSave = Mapper.Map<Pbck4Dto>(model);

            var input = new Pbck4SaveInput()
            {
                Pbck4Dto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                //Pbck4Items = Mapper.Map<List<Pbck4ItemDto>>(model.UploadItemModels)
            };

            return _pbck4Bll.SavePbck4(input);
        }

        [HttpPost]
        public JsonResult GetPlantDetails(string plantId)
        {
            var dbPlant = _plantBll.GetT001WById(plantId);
            var model = Mapper.Map<Pbck4PlantModel>(dbPlant);

            var poa = _poaBll.GetById(CurrentUser.USER_ID);
            if (poa != null)
                model.Poa = poa.PRINTED_NAME;

            return Json(model);
        }

        public ActionResult Details(int id)
        {
            var model = new Pbck4FormViewModel();

            try
            {
                var pbck4Details = _pbck4Bll.GetDetailsPbck4(id);


                Mapper.Map(pbck4Details.Pbck4Dto, model);

              
                model.MainMenu = Enums.MenuList.PBCK4;
                model.CurrentMenu = PageInfo;


                // model = GetInitDetailsData(model);
               // model.UploadItemModels = Mapper.Map<List<Pbck4UploadViewModel>>(pbck4Details.Pbck4ItemsDto);
               
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(pbck4Details.ListChangesHistorys);
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(pbck4Details.ListWorkflowHistorys);

                model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(pbck4Details.ListPrintHistorys);
                
                ////validate approve and reject
                //var input = new WorkflowAllowApproveAndRejectInput();
                //input.DocumentStatus = model.DocumentStatus;
                //input.FormView = Enums.FormViewType.Detail;
                //input.UserRole = CurrentUser.UserRole;
                //input.CreatedUser = ck5Details.Ck5Dto.CREATED_BY;
                //input.CurrentUser = CurrentUser.USER_ID;
                //input.CurrentUserGroup = CurrentUser.USER_GROUP_ID;
                //input.DocumentNumber = model.SubmissionNumber;
                //input.NppbkcId = model.SourceNppbkcId;

                ////workflow
                //var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                //model.AllowApproveAndReject = allowApproveAndReject;


                //if (!allowApproveAndReject)
                //{
                //    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                //    model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
                //}

                //model.IsAllowPrint = _workflowBll.AllowPrint(model.DocumentStatus);

                //var outputHistory = _workflowHistoryBll.GetStatusGovHistory(ck5Details.Ck5Dto.SUBMISSION_NUMBER);
                //model.GovStatusDesc = outputHistory.StatusGov;
                //model.CommentGov = outputHistory.Comment;

            
          
                //if (model.AllowGovApproveAndReject)
                //    model.ActionType = "GovApproveDocument";
                //else if (model.AllowGiCreated)
                //    model.ActionType = "CK5GICreated";
                //else if (model.AllowGrCreated)
                //    model.ActionType = "CK5GRCreated";



            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                model.MainMenu = Enums.MenuList.PBCK4;
                model.CurrentMenu = PageInfo;
            }

            return View(model);
        }

        public void ExportXls(int pbckId)
        {
            // return File(CreateXlsFile(ck5Id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            var pathFile = CreateXlsFile(pbckId);
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

        private string CreateXlsFile(int pbckId)
        {
            var slDocument = new SLDocument();

            //todo check
            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK4, pbckId.ToString());

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

            var fileName = "PBCK4" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Content/upload/"), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }

        private Pbck4FormViewModel InitEdit(Pbck4FormViewModel model)
        {

            model.MainMenu = Enums.MenuList.PBCK4;
            model.CurrentMenu = PageInfo;


            model.PlantList = GlobalFunctions.GetPlantAll();

            return model;
        }

        public ActionResult Edit(int id)
        {
            var model = new Pbck4FormViewModel();

            try
            {
                var pbck4Details = _pbck4Bll.GetDetailsPbck4(id);

                Mapper.Map(pbck4Details.Pbck4Dto, model);

                //validate
                //only allow edit/submit when current_user = createdby and document = draft
                var input = new WorkflowAllowEditAndSubmitInput();
                input.DocumentStatus = model.DocumentStatus;
                //todo check
                input.CreatedUser = pbck4Details.Pbck4Dto.CREATED_BY;
                input.CurrentUser = CurrentUser.USER_ID;
                if (!_workflowBll.AllowEditDocument(input))
                    return RedirectToAction("Details", "PBCK4", new { @id = model.Pbck4Id });

                model = InitEdit(model);

              //  model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(ck5Details.Ck5MaterialDto);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(pbck4Details.ListChangesHistorys);
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(pbck4Details.ListWorkflowHistorys);
                model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(pbck4Details.ListPrintHistorys);

             
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitEdit(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pbck4FormViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if (model.UploadItemModels.Count > 0)
                    //{
                        bool isSubmit = model.Command == "Submit";

                        //validate
                        var input = new WorkflowAllowEditAndSubmitInput();
                        input.DocumentStatus = model.DocumentStatus;
                        input.CreatedUser = model.CREATED_BY;
                        input.CurrentUser = CurrentUser.USER_ID;
                        if (_workflowBll.AllowEditDocument(input))
                        {
                            SavePbck4ToDatabase(model);
                            if (isSubmit)
                            {
                                //CK5Workflow(model.Ck5Id, Enums.ActionType.Submit, string.Empty);
                                AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                                return RedirectToAction("Details", "PBCK4", new { @id = model.Pbck4Id });

                            }
                            AddMessageInfo("Success", Enums.MessageInfoType.Success);
                            return RedirectToAction("Edit", "PBCK4", new { @id = model.Pbck4Id });
                        }
                        else
                        {
                            AddMessageInfo("Not allow to Edit Document", Enums.MessageInfoType.Error);
                            return RedirectToAction("Details", "PBCK4", new { @id = model.Pbck4Id });
                        }

                    //}
                    //else
                    //    AddMessageInfo("Missing CK5 Material", Enums.MessageInfoType.Error);
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
        
        private Pbck4FormViewModel GetHistorys(Pbck4FormViewModel model)
        {
            //todo check
            model.WorkflowHistory =
                Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.Pbck4Number));

            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK4, model.Pbck4Id.ToString()));

            return model;
        }

        [HttpPost]
        public PartialViewResult UploadFile(HttpPostedFileBase itemExcelFile, string plantId)
        {
            var data = (new ExcelReader()).ReadExcelCk5FileDocuments(itemExcelFile);
            var model = new Pbck4FormViewModel();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var uploadItem = new Pbck4UploadViewModel();

                    try
                    {
                        uploadItem.FaCode = datarow[0];
                        uploadItem.Ck1No = datarow[1];
                        uploadItem.ReqQty = datarow[2];
                        uploadItem.NoPengawas = datarow[3];
                        uploadItem.ApprovedQty = datarow[4];
                        uploadItem.Remark = datarow[5];
                        
                        uploadItem.Plant = plantId;

                        model.UploadItemModels.Add(uploadItem);

                    }
                    catch (Exception)
                    {
                        continue;

                    }

                }
            }

            var input = Mapper.Map<List<Pbck4ItemsInput>>(model.UploadItemModels);

            var outputResult = _pbck4Bll.Pbck4ItemProcess(input);

            model.UploadItemModels = Mapper.Map<List<Pbck4UploadViewModel>>(outputResult);

            return PartialView("_Pbck4UploadList", model.UploadItemModels);
        }
	}
}