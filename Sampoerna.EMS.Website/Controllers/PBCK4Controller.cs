using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject.Business;
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
using Sampoerna.EMS.Website.Models.PBCK4;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;
using Sampoerna.EMS.XMLReader;
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
        private IPrintHistoryBLL _printHistoryBll;
        private IMonthBLL _monthBll;
      
       

        public PBCK4Controller(IPageBLL pageBLL, IPOABLL poabll, IZaidmExNPPBKCBLL nppbkcBll,
            IPBCK4BLL pbck4Bll, IPlantBLL plantBll, IWorkflowBLL workflowBll, IChangesHistoryBLL changesHistoryBll,
            IWorkflowHistoryBLL workflowHistoryBll, IPrintHistoryBLL printHistoryBll, IMonthBLL monthBll)
            : base(pageBLL, Enums.MenuList.PBCK4)
        {
            _poaBll = poabll;
            _nppbkcBll = nppbkcBll;
            _pbck4Bll = pbck4Bll;
            _plantBll = plantBll;
            _workflowBll = workflowBll;
            _changesHistoryBll = changesHistoryBll;
            _workflowHistoryBll = workflowHistoryBll;
            _printHistoryBll = printHistoryBll;
            _monthBll = monthBll;
           

        }

        //
        // GET: /PBCK4/
        public ActionResult Index()
        {

            Pbck4IndexViewModel model;
            try
            {
                model = CreateInitModelView(false);


            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck4IndexViewModel();
            }

            return View(model);
        }

        private Pbck4IndexViewModel CreateInitModelView(bool isCompleted)
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
            model.DetailsList = GetPbck4Items(isCompleted);

            return model;
        }

        private List<Pbck4Item> GetPbck4Items(bool isCompletedDocument, Pbck4SearchViewModel filter = null)
        {
            Pbck4GetByParamInput input;
            List<Pbck4Dto> dbData;

            if (filter == null)
            {
                //Get All
                input = new Pbck4GetByParamInput();
                input.IsCompletedDocument = isCompletedDocument;
                input.UserId = CurrentUser.USER_ID;
                input.UserRole = CurrentUser.UserRole;

                dbData = _pbck4Bll.GetPbck4ByParam(input);
                return Mapper.Map<List<Pbck4Item>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<Pbck4GetByParamInput>(filter);
            input.IsCompletedDocument = isCompletedDocument;
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;

            dbData = _pbck4Bll.GetPbck4ByParam(input);
            return Mapper.Map<List<Pbck4Item>>(dbData);
        }

        [HttpPost]
        public PartialViewResult Filter(Pbck4IndexViewModel model)
        {
            model.DetailsList = GetPbck4Items(model.IsCompletedType,model.SearchView);
            return PartialView("_Pbck4OpenListDocuments", model);
        }

        public ActionResult Pbck4Completed()
        {
            Pbck4IndexViewModel model;
            try
            {
                model = CreateInitModelView(true);
                model.IsCompletedType = true;

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck4IndexViewModel();
                model.MainMenu = Enums.MenuList.PBCK4;
                model.CurrentMenu = PageInfo;
            }

            return View("Pbck4CompletedDocuments", model);
           
           
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
                    if (model.UploadItemModels.Count > 0)
                    {

                        var saveResult = SavePbck4ToDatabase(model);

                        AddMessageInfo("Success create PBCK-4", Enums.MessageInfoType.Success);


                        return RedirectToAction("Edit", "PBCK4", new { @id = saveResult.PBCK4_ID });
                    }

                    AddMessageInfo("Missing PBCK-4 Items", Enums.MessageInfoType.Error);
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

            if (dataToSave.POA_PRINTED_NAME.Length > 250)
                dataToSave.POA_PRINTED_NAME = dataToSave.POA_PRINTED_NAME.Substring(0, 249);

            dataToSave.APPROVED_BY_POA = null;

            var input = new Pbck4SaveInput()
            {
                Pbck4Dto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                Pbck4Items = Mapper.Map<List<Pbck4ItemDto>>(model.UploadItemModels)
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
            else
            {
                model.Poa = _pbck4Bll.GetListPoaByNppbkcId(model.NppbkcId);
            }
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
                model.UploadItemModels = Mapper.Map<List<Pbck4UploadViewModel>>(pbck4Details.Pbck4ItemsDto);
               
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(pbck4Details.ListChangesHistorys);
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(pbck4Details.ListWorkflowHistorys);

                model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(pbck4Details.ListPrintHistorys);

                model.Pbck4FileUploadModelList = Mapper.Map<List<Pbck4FileUploadViewModel>>(pbck4Details.Pbck4Dto.Pbck4DocumentDtos.Where(c=>c.DOC_TYPE == 1));
                model.Pbck4FileUploadModelList2 = Mapper.Map<List<Pbck4FileUploadViewModel>>(pbck4Details.Pbck4Dto.Pbck4DocumentDtos.Where(c=>c.DOC_TYPE == 2));


                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput();
                input.DocumentStatus = model.DocumentStatus;
                input.FormView = Enums.FormViewType.Detail;
                input.UserRole = CurrentUser.UserRole;
                input.CreatedUser = pbck4Details.Pbck4Dto.CREATED_BY;
                input.CurrentUser = CurrentUser.USER_ID;
                input.CurrentUserGroup = CurrentUser.USER_GROUP_ID;
                input.DocumentNumber = model.Pbck4Number;
                input.NppbkcId = model.NppbkcId;
                input.ManagerApprove = model.APPROVED_BY_MANAGER;
                //workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;


                if (!allowApproveAndReject)
                {
                    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                    model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
                }

                model.IsAllowPrint = _workflowBll.AllowPrint(model.DocumentStatus);

                //var outputHistory = _workflowHistoryBll.GetStatusGovHistory(ck5Details.Ck5Dto.SUBMISSION_NUMBER);
                //model.GovStatusDesc = outputHistory.StatusGov;
                //model.CommentGov = outputHistory.Comment;



                if (model.AllowGovApproveAndReject)
                {
                    model.ActionType = "GovApproveDocument";
                    if (!pbck4Details.Pbck4Dto.CK3_OFFICE_VALUE.HasValue
                        || pbck4Details.Pbck4Dto.CK3_OFFICE_VALUE.Value <= 0)
                        model.CK3_OFFICE_VALUE = "";
                }
                else if (model.DocumentStatus == Enums.DocumentStatus.Completed)
                    model.ActionType = "UpdateUploadedFilefterCompleted";
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

                model.UploadItemModels = Mapper.Map<List<Pbck4UploadViewModel>>(pbck4Details.Pbck4ItemsDto);
                //get blocked stock
                foreach (var uploadItemModel in model.UploadItemModels)
                {
                    var blockStockOutput = _pbck4Bll.GetBlockedStockQuota(uploadItemModel.Plant, uploadItemModel.FaCode);
                    uploadItemModel.BlockedStock = blockStockOutput.BlockedStock;
                    uploadItemModel.BlockedStockUsed = blockStockOutput.BlockedStockUsed;
                    uploadItemModel.BlockedStockRemaining = blockStockOutput.BlockedStockRemaining;


                    //add remaining with current reqQty
                    uploadItemModel.BlockedStockRemaining =
                        (ConvertHelper.ConvertToDecimalOrZero(uploadItemModel.BlockedStockRemaining) +
                         ConvertHelper.ConvertToDecimalOrZero(uploadItemModel.ReqQty)).ToString();


                    //uploadItemModel.BlockedStock = _pbck4Bll.GetBlockedStockByPlantAndFaCode(uploadItemModel.Plant,uploadItemModel.FaCode).ToString();
                }

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
                    if (model.UploadItemModels.Count > 0)
                    {
                        bool isSubmit = model.Command == "Submit";

                        //validate
                        var input = new WorkflowAllowEditAndSubmitInput();
                        input.DocumentStatus = model.DocumentStatus;
                        input.CreatedUser = model.CREATED_BY;
                        input.CurrentUser = CurrentUser.USER_ID;
                        if (_workflowBll.AllowEditDocument(input))
                        {
                          var resultDto =  SavePbck4ToDatabase(model);
                            if (isSubmit)
                            {
                                PBCK4Workflow(model.Pbck4Id, Enums.ActionType.Submit, string.Empty, resultDto.IsModifiedHistory);
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

                    }
                    else
                        AddMessageInfo("Missing PBCK-4 Items", Enums.MessageInfoType.Error);
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
                        uploadItem.ApprovedQty = datarow[4] == string.Empty ? datarow[2] : datarow[4];
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

            return PartialView("_Pbck4UploadAdd", model.UploadItemModels);
        }

        private void PBCK4Workflow(int id, Enums.ActionType actionType, string comment, bool isModified = false)
        {
            var input = new Pbck4WorkflowDocumentInput();
            input.DocumentId = id;
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ActionType = actionType;
            input.Comment = comment;
            input.IsModified = isModified;

            _pbck4Bll.PBCK4Workflow(input);
        }

        private bool PBCK4GovWorkflow(Pbck4FormViewModel model)
        {
           var actionType = Enums.ActionType.GovApprove;

            if (model.GovStatus == Enums.DocumentStatusGov.PartialApproved)
                actionType = Enums.ActionType.GovPartialApprove;
            else if (model.GovStatus == Enums.DocumentStatusGov.Rejected)
                actionType = Enums.ActionType.GovReject;

            var input = new Pbck4WorkflowDocumentInput();
            input.DocumentId = model.Pbck4Id;
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ActionType = actionType;
            input.Comment = model.Comment;
            input.GovStatusInput = model.GovStatus;

       

            if (model.GovStatus == Enums.DocumentStatusGov.FullApproved
                || model.GovStatus == Enums.DocumentStatusGov.PartialApproved)
            {

                if (model.GovStatus == Enums.DocumentStatusGov.PartialApproved)
                {
                    foreach (var uploadItem in model.UploadItemModels)
                    {
                        if (!ConvertHelper.IsNumeric(uploadItem.ApprovedQty)
                            || ConvertHelper.ConvertToDecimalOrZero(uploadItem.ApprovedQty) <= 0)
                        {
                            AddMessageInfo("PBCK-4 Error BACK-1 QTY Value.", Enums.MessageInfoType.Error);
                            return false;
                        }
                    }
                }

               
                input.UploadItemDto = new List<Pbck4ItemDto>();
                foreach (var pbck4UploadItem in model.UploadItemModels)
                {
                    var uploadToUpdate = new Pbck4ItemDto();
                    uploadToUpdate.PBCK4_ITEM_ID = pbck4UploadItem.PBCK4_ITEM_ID;
                    uploadToUpdate.APPROVED_QTY = ConvertHelper.ConvertToDecimalOrZero(pbck4UploadItem.ApprovedQty);
                    uploadToUpdate.REQUESTED_QTY = ConvertHelper.ConvertToDecimalOrZero(pbck4UploadItem.ReqQty);

                    input.UploadItemDto.Add(uploadToUpdate);
                }
            }


            input.AdditionalDocumentData = new Pbck4WorkflowDocumentData();
            input.AdditionalDocumentData.Back1No = model.BACK1_NO;
            input.AdditionalDocumentData.Back1Date = model.BACK1_DATE;

            input.AdditionalDocumentData.Ck3No = model.CK3_NO;
            input.AdditionalDocumentData.Ck3Date = model.CK3_DATE;
            input.AdditionalDocumentData.Ck3OfficeValue = model.CK3_OFFICE_VALUE;

            input.AdditionalDocumentData.Back1FileUploadList = Mapper.Map<List<PBCK4_DOCUMENTDto>>(model.Pbck4FileUploadModelList);
            input.AdditionalDocumentData.Ck3FileUploadList = Mapper.Map<List<PBCK4_DOCUMENTDto>>(model.Pbck4FileUploadModelList2);


            _pbck4Bll.PBCK4Workflow(input);

            try
            {
                if (model.GovStatus == Enums.DocumentStatusGov.PartialApproved ||
                    model.GovStatus == Enums.DocumentStatusGov.FullApproved)
                {
                    //create xml file
                    var pbck4XmlDto = _pbck4Bll.GetPbck4ForXmlById(model.Pbck4Id);

                    //only completed document can create xml file
                    if (pbck4XmlDto.Status == Enums.DocumentStatus.Completed)
                    {

                        var fileName = ConfigurationManager.AppSettings["Pbck4PathXml"] + "COMPENSATION-" +
                                       DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml";


                        pbck4XmlDto.GeneratedXmlPath = fileName;

                        XmlPBCK4DataWriter rt = new XmlPBCK4DataWriter();
                        rt.CreatePbck4Xml(pbck4XmlDto);

                        //send mail after that
                        _pbck4Bll.SendMailCompletedPbck4Document(input);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                //failed create xml...
                //rollaback the update
                _pbck4Bll.GovApproveDocumentRollback(input);
                AddMessageInfo("Failed Create PBCK4 XMl message : " + ex.Message, Enums.MessageInfoType.Error);
                return false;
            }
        }

        public ActionResult ApproveDocument(int id)
        {
            try
            {
                PBCK4Workflow(id, Enums.ActionType.Approve, string.Empty);
                AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "PBCK4", new { id });
        }

        public ActionResult RejectDocument(Pbck4FormViewModel model)
        {
            try
            {
                PBCK4Workflow(model.Pbck4Id, Enums.ActionType.Reject, model.Comment);
                AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
        }

        public ActionResult CancelDocument(Pbck4FormViewModel model)
        {
            try
            {
                PBCK4Workflow(model.Pbck4Id, Enums.ActionType.Cancel, model.Comment);
                AddMessageInfo("Success Cancel Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
        }

        private string SaveUploadedFile(HttpPostedFileBase file, int pbck4Id, string type)
        {
            if (file == null || file.FileName == "")
                return "";
            
            string sFileName = "";
            
            //initialize folders in case deleted by an test publish profile
            if (!Directory.Exists(Server.MapPath(Constans.CK5FolderPath)))
                Directory.CreateDirectory(Server.MapPath(Constans.CK5FolderPath));

           
            sFileName = Constans.CK5FolderPath + Path.GetFileName(pbck4Id.ToString("'ID'-##") + type + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }


        [HttpPost]
        public ActionResult GovApproveDocument(Pbck4FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddMessageInfo("Model Not Valid", Enums.MessageInfoType.Success);
                return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
            }

            try
            {
                var currentUserId = CurrentUser.USER_ID;
                if (model.Pbck4FileUploadModelList == null)
                    model.Pbck4FileUploadModelList = new List<Pbck4FileUploadViewModel>();

                if (model.Pbck4FileUploadFileList != null)
                {
                    foreach (var item in model.Pbck4FileUploadFileList)
                    {
                        if (item != null)
                        {
                            var filenameCk5Check = item.FileName;
                            if (filenameCk5Check.Contains("\\"))
                                filenameCk5Check = filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];

                            var pbck4UploadFile = new Pbck4FileUploadViewModel
                            {
                                FILE_NAME = filenameCk5Check,
                                FILE_PATH = SaveUploadedFile(item, model.Pbck4Id, "B"),
                                DOC_TYPE = 1, //back1,
                                PBCK4_ID = model.Pbck4Id,
                                IsDeleted = false
                            };
                            model.Pbck4FileUploadModelList.Add(pbck4UploadFile);
                        }

                    }
                }
                else
                {
                    AddMessageInfo("Empty File BACK-1 Doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
                }

                if (model.Pbck4FileUploadModelList.Count == 0)
                {
                    AddMessageInfo("Empty File BACK-1 Doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Details", "PBCK4", new {id = model.Pbck4Id});
                }
                
                bool resultDoc = false;
                foreach (var pbck4FileUploadViewModel in model.Pbck4FileUploadModelList)
                {
                    if (!pbck4FileUploadViewModel.IsDeleted)
                    {
                        resultDoc = true;
                        break;
                    }
                }
                if (!resultDoc)
                {
                    AddMessageInfo("Empty File BACK-1 Doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
                }

                if (model.GovStatus != Enums.DocumentStatusGov.Rejected)
                {
                    if (model.Pbck4FileUploadModelList2 == null)
                        model.Pbck4FileUploadModelList2 = new List<Pbck4FileUploadViewModel>();

                    if (model.Pbck4FileUploadFileList2 != null)
                    {
                        foreach (var item in model.Pbck4FileUploadFileList2)
                        {
                            if (item != null)
                            {
                                var filenameCk5Check = item.FileName;
                                if (filenameCk5Check.Contains("\\"))
                                    filenameCk5Check =
                                        filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];

                                var pbck4UploadFile = new Pbck4FileUploadViewModel
                                {
                                    FILE_NAME = filenameCk5Check,
                                    FILE_PATH = SaveUploadedFile(item, model.Pbck4Id, "C"),
                                    DOC_TYPE = 2, //ck-3
                                    PBCK4_ID = model.Pbck4Id,
                                    IsDeleted = false
                                };
                                model.Pbck4FileUploadModelList2.Add(pbck4UploadFile);
                            }

                        }
                    }
                    //else
                    //{
                    //    AddMessageInfo("Empty File CK-3 Doc", Enums.MessageInfoType.Error);
                    //    return RedirectToAction("Details", "PBCK4", new {id = model.Pbck4Id});
                    //}

                    //if (model.Pbck4FileUploadModelList2.Count == 0)
                    //{
                    //    AddMessageInfo("Empty File CK-3 Doc", Enums.MessageInfoType.Error);
                    //    return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
                    //}

                }
                PBCK4GovWorkflow(model);
                if (model.GovStatus == Enums.DocumentStatusGov.FullApproved)
                    AddMessageInfo("Success Gov FullApproved Document", Enums.MessageInfoType.Success);
                else if (model.GovStatus == Enums.DocumentStatusGov.PartialApproved)
                    AddMessageInfo("Success Gov PartialApproved Document", Enums.MessageInfoType.Success);
                else if (model.GovStatus == Enums.DocumentStatusGov.Rejected)
                    AddMessageInfo("Success Gov Reject Document", Enums.MessageInfoType.Success);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
        }

        [HttpPost]
        public ActionResult UpdateUploadedFilefterCompleted(Pbck4FormViewModel model)
        {
         

            try
            {
                var currentUserId = CurrentUser.USER_ID;
                if (model.Pbck4FileUploadModelList == null)
                    model.Pbck4FileUploadModelList = new List<Pbck4FileUploadViewModel>();

                if (model.Pbck4FileUploadFileList != null)
                {
                    foreach (var item in model.Pbck4FileUploadFileList)
                    {
                        if (item != null)
                        {
                            var filenameCk5Check = item.FileName;
                            if (filenameCk5Check.Contains("\\"))
                                filenameCk5Check = filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];

                            var pbck4UploadFile = new Pbck4FileUploadViewModel
                            {
                                FILE_NAME = filenameCk5Check,
                                FILE_PATH = SaveUploadedFile(item, model.Pbck4Id, "B"),
                                DOC_TYPE = 1, //back1,
                                PBCK4_ID = model.Pbck4Id,
                                IsDeleted = false
                            };
                            model.Pbck4FileUploadModelList.Add(pbck4UploadFile);
                        }

                    }
                }
                else
                {
                    AddMessageInfo("Empty File BACK-1 Doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
                }

                bool ExistDocument = false;

                foreach (var uploadModelList in model.Pbck4FileUploadModelList)
                {
                    if (uploadModelList.IsDeleted == false)
                    {
                        ExistDocument = true;
                        break;
                    }
                }
                if (!ExistDocument)
                {
                    AddMessageInfo("Empty File BACK-1 Doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
                }

                if (model.Pbck4FileUploadModelList2 == null)
                    model.Pbck4FileUploadModelList2 = new List<Pbck4FileUploadViewModel>();

                if (model.Pbck4FileUploadFileList2 != null)
                {
                    foreach (var item in model.Pbck4FileUploadFileList2)
                    {
                        if (item != null)
                        {
                            var filenameCk5Check = item.FileName;
                            if (filenameCk5Check.Contains("\\"))
                                filenameCk5Check = filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];

                            var pbck4UploadFile = new Pbck4FileUploadViewModel
                            {
                                FILE_NAME = filenameCk5Check,
                                FILE_PATH = SaveUploadedFile(item, model.Pbck4Id, "C"),
                                DOC_TYPE = 2, //ck-3
                                PBCK4_ID = model.Pbck4Id,
                                IsDeleted = false
                            };
                            model.Pbck4FileUploadModelList2.Add(pbck4UploadFile);
                        }

                    }
                }
                else
                {
                    AddMessageInfo("Empty File CK-3 Doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
                }

                ExistDocument = false;

                foreach (var uploadModelList in model.Pbck4FileUploadModelList2)
                {
                    if (uploadModelList.IsDeleted == false)
                    {
                        ExistDocument = true;
                        break;
                    }
                }

                if (!ExistDocument)
                {
                    AddMessageInfo("Empty File CK-3 Doc", Enums.MessageInfoType.Error);
                    return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
                }

                var pbckDocument = Mapper.Map<List<PBCK4_DOCUMENTDto>>(model.Pbck4FileUploadModelList);
                pbckDocument.AddRange(Mapper.Map<List<PBCK4_DOCUMENTDto>>(model.Pbck4FileUploadModelList2));

                _pbck4Bll.UpdateUploadedFileCompleted(pbckDocument);

                AddMessageInfo("Success Update Document PBCK-4", Enums.MessageInfoType.Success);


            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Details", "PBCK4", new { id = model.Pbck4Id });
        }

        #region print


        private dsPbck4 AddDataPbck4Row(dsPbck4 dsPbck4, Pbck4ReportDetailsDto pbck4ReportDetails, string printTitle)
        {
            var detailRow = dsPbck4.Pbck4.NewPbck4Row();

            detailRow.Pbck4Number = pbck4ReportDetails.Pbck4Number;
            detailRow.Pbck4Lampiran = pbck4ReportDetails.Pbck4Lampiran;
            detailRow.TextTo = pbck4ReportDetails.TextTo;
            detailRow.CityTo = pbck4ReportDetails.CityTo;
            detailRow.PoaName = pbck4ReportDetails.PoaName;
            detailRow.PoaTitle = pbck4ReportDetails.PoaTitle;
            detailRow.CompanyName = pbck4ReportDetails.CompanyName;
            detailRow.CompanyAddress = pbck4ReportDetails.CompanyAddress;
            detailRow.NppbkcId = pbck4ReportDetails.NppbkcId;
            detailRow.NppbkcDate = pbck4ReportDetails.NppbkcDate;
            detailRow.PlantCity = pbck4ReportDetails.PlantCity;
            detailRow.PrintDate = pbck4ReportDetails.PrintDate;
            detailRow.RegionOffice = pbck4ReportDetails.RegionOffice;
            detailRow.DocumentTitle = printTitle;
            
            //set image
            if (string.IsNullOrEmpty(pbck4ReportDetails.HeaderImage))
                detailRow.HeaderImage = null;
            else
            {
                //convert to byte image
                FileStream fs;
                BinaryReader br;
                var imagePath = pbck4ReportDetails.HeaderImage;
                if (System.IO.File.Exists(Server.MapPath(imagePath)))
                {
                    fs = new FileStream(Server.MapPath(imagePath), FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite);

                    // initialise the binary reader from file streamobject 
                    br = new BinaryReader(fs);
                    // define the byte array of filelength 
                    byte[] imgbyte = new byte[fs.Length + 1];
                    // read the bytes from the binary reader 
                    imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));

                    detailRow.HeaderImage = imgbyte;

                }
               
                //else
                //{
                //    // if photo does not exist show the nophoto.jpg file 
                //    fs = new FileStream(Server.MapPath(imagePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //}
              
            }

           
            dsPbck4.Pbck4.AddPbck4Row(detailRow);

            return dsPbck4;
        }

        private dsPbck4 AddDataPbck4ItemsRow(dsPbck4 dsPbck4, List<Pbck4ItemReportDto> listPbck4ItemsDto)
        {
           foreach (var materialDto in listPbck4ItemsDto)
            {
                
                var detailRow = dsPbck4.Pbck4Items.NewPbck4ItemsRow();

                detailRow.Seri = materialDto.Seri;
                detailRow.RequestedQty = materialDto.ReqQty;
                detailRow.Hje = materialDto.Hje;
                detailRow.Content = materialDto.Content;
                detailRow.Tariff = materialDto.Tariff;
                detailRow.JumlahHje = materialDto.TotalHje;

                detailRow.JumlahCukai = materialDto.TotalCukai;
                detailRow.NoPengawas = materialDto.NoPengawas;

                dsPbck4.Pbck4Items.AddPbck4ItemsRow(detailRow);
                

                
            }
           return dsPbck4;
        }


        private dsPbck4 AddDataPbck4MatrikCk1Row(dsPbck4 dsPbck4, List<Pbck4IMatrikCk1ReportDto> listPbck4ItemsDto)
        {
            foreach (var materialDto in listPbck4ItemsDto)
            {

                var detailRow = dsPbck4.Pbck4MatrikCk1.NewPbck4MatrikCk1Row();

                detailRow.Number = materialDto.Number;
                detailRow.SeriesCode = materialDto.SeriesCode;
                detailRow.Hje = materialDto.Hje;
                detailRow.JenisHt = materialDto.JenisHt;
                detailRow.Content = materialDto.Content;
                detailRow.BrandName = materialDto.BrandName;

                detailRow.Ck1No = materialDto.Ck1No;
                detailRow.Ck1Date = materialDto.Ck1Date;
                detailRow.Ck1OrderQty = materialDto.Ck1OrderQty;
                detailRow.Ck1RequestedQty = materialDto.Ck1RequestedQty;
                detailRow.Tariff = materialDto.Tariff;
                detailRow.TotalHje = materialDto.TotalHje;
                detailRow.TotalCukai = materialDto.TotalCukai;
                detailRow.NoPengawas = materialDto.NoPengawas;

                dsPbck4.Pbck4MatrikCk1.AddPbck4MatrikCk1Row(detailRow);



            }
            return dsPbck4;
        }

        private DataSet SetDataSetReport(Pbck4ReportDto pbck4ReportDto, string printTitle)
        {

            var dsPbck4 = new dsPbck4();

        
            var listPbck4 = new List<Pbck4ReportDetailsDto>();
            listPbck4.Add(pbck4ReportDto.ReportDetails);

            dsPbck4 = AddDataPbck4Row(dsPbck4, pbck4ReportDto.ReportDetails,  printTitle);
            dsPbck4 = AddDataPbck4ItemsRow(dsPbck4, pbck4ReportDto.ListPbck4Items);
            dsPbck4 = AddDataPbck4MatrikCk1Row(dsPbck4, pbck4ReportDto.ListPbck4MatrikCk1);

            return dsPbck4;

        }

        private Stream GetReport(Pbck4ReportDto pbck4Report, string printTitle)
        {
            var dataSet = SetDataSetReport(pbck4Report, printTitle);

            ReportClass rpt = new ReportClass
            {
                FileName = ConfigurationManager.AppSettings["Report_Path"] + "PBCK4\\Pbck4PrintOut.rpt"
               
            };
            rpt.Load();
            rpt.SetDataSource(dataSet);
            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rpt.Close();
            return stream;
        }

        [EncryptedParameter]
        public ActionResult PrintPreview(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            var pbck4Data = _pbck4Bll.GetPbck4ReportDataById(id.Value);
            if (pbck4Data == null)
                HttpNotFound();

            Stream stream = GetReport(pbck4Data, "PBCK-4 PREVIEW");

            return File(stream, "application/pdf");
        }

        [EncryptedParameter]
        public ActionResult PrintOut(int? id)
        {
            try
            {

                if (!id.HasValue)
                    HttpNotFound();

                var pbck4Data = _pbck4Bll.GetPbck4ReportDataById(id.Value);
                if (pbck4Data == null)
                    HttpNotFound();

                Stream stream = GetReport(pbck4Data, "PBCK-4");

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
            var pbck4Data = _pbck4Bll.GetPbck4ById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.PBCK4,
                FORM_ID = pbck4Data.PBCK4_ID,
                FORM_NUMBER = pbck4Data.PBCK4_NUMBER,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(pbck4Data.PBCK4_NUMBER));
            return PartialView("_PrintHistoryTable", model);

        }
        #endregion

        #region Summary Reports

        private SelectList GetPbck4NumberList(List<Pbck4SummaryReportDto> listPbck4)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck4
                    select new SelectItemModel()
                    {
                        ValueField = x.Pbck4No,
                        TextField = x.Pbck4No
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetYearListPbck4(bool isFrom, List<Pbck4SummaryReportDto> listPbck4)
        {
          
            IEnumerable<SelectItemModel> query;
            if (isFrom)
                query = from x in listPbck4.OrderBy(c => c.ReportedOn)
                        select new SelectItemModel()
                        {
                            ValueField = x.ReportedOn,
                            TextField = x.ReportedOn.ToString()
                        };
            else
                query = from x in listPbck4.OrderByDescending(c => c.ReportedOn)
                        select new SelectItemModel()
                        {
                            ValueField = x.ReportedOn,
                            TextField = x.ReportedOn.ToString()
                        };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetPlantList(List<Pbck4SummaryReportDto> listPbck4)
        {
          
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck4
                        select new SelectItemModel()
                        {
                            ValueField = x.PlantId,
                            TextField = x.PlantId + " - " + x.PlantDescription
                        };
            
           
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }



        private Pbck4SummaryReportsViewModel InitSummaryReports(Pbck4SummaryReportsViewModel model)
        {
            model.MainMenu = Enums.MenuList.PBCK4;
            model.CurrentMenu = PageInfo;

            var listPbck4 = _pbck4Bll.GetSummaryReportsByParam(new Pbck4GetSummaryReportByParamInput());

            model.SearchView.Pbck4NoList = GetPbck4NumberList(listPbck4);
            model.SearchView.YearFromList = GetYearListPbck4(true, listPbck4);
            model.SearchView.YearToList = GetYearListPbck4(false, listPbck4);
            model.SearchView.PlantIdList = GetPlantList(listPbck4);
            

            var filter = new Pbck4SearchSummaryReportsViewModel();
        
           model.DetailsList = SearchDataSummaryReports(filter);

            return model;
        }

        private List<Pbck4SummaryReportsItem> SearchDataSummaryReports(Pbck4SearchSummaryReportsViewModel filter = null)
        {
            Pbck4GetSummaryReportByParamInput input;
            List<Pbck4SummaryReportDto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Pbck4GetSummaryReportByParamInput();

                dbData = _pbck4Bll.GetSummaryReportsByParam(input);
                return Mapper.Map<List<Pbck4SummaryReportsItem>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<Pbck4GetSummaryReportByParamInput>(filter);

            dbData = _pbck4Bll.GetSummaryReportsByParam(input);
            return Mapper.Map<List<Pbck4SummaryReportsItem>>(dbData);
        }

        public ActionResult SummaryReports()
        {

            Pbck4SummaryReportsViewModel model;
            try
            {

                model = new Pbck4SummaryReportsViewModel();

                
                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck4SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("Pbck4SummaryReport", model);
        }

        [HttpPost]
        public PartialViewResult SearchSummaryReports(Pbck4SummaryReportsViewModel model)
        {
            model.DetailsList = SearchDataSummaryReports(model.SearchView);
            return PartialView("_Pbck4ListSummaryReport", model);

         
        }

        public void ExportXlsSummaryReports(Pbck4SummaryReportsViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsSummaryReports(model.ExportModel);


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

        private string CreateXlsSummaryReports(Pbck4ExportSummaryReportsViewModel modelExport)
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
                if (modelExport.Pbck4Date)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Pbck4Date);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Pbck4Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Pbck4No);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CeOffice)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CeOffice);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Brand)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Brand);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Content)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Content);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Hje)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Hje);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Tariff)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Tariff);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ProductType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ProductType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.FiscalYear)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.FiscalYear);
                    iColumn = iColumn + 1;
                }

                if (modelExport.SeriesCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SeriesCode);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RequestedQty)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestedQty);
                    iColumn = iColumn + 1;
                }

                if (modelExport.ExciseValue)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ExciseValue);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.Remarks)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Remarks);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Back1Date)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Back1Date);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Back1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Back1Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Ck3Date)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck3Date);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Ck3Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck3Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Ck3Value)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck3Value);
                    iColumn = iColumn + 1;
                }
                if (modelExport.PrintingCost)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PrintingCost);
                    iColumn = iColumn + 1;
                }
                if (modelExport.CompensatedCk1Date)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompensatedCk1Date);
                    iColumn = iColumn + 1;
                }
                if (modelExport.CompensatedCk1Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompensatedCk1Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.PaymentDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PaymentDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.Status)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Status);
                    iColumn = iColumn + 1;
                }
             
                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, Pbck4ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.Pbck4Date)
            {
                slDocument.SetCellValue(iRow, iColumn, "PBCK-4 Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.Pbck4Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "PBCK-4 Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.CeOffice)
            {
                slDocument.SetCellValue(iRow, iColumn, "KPPBC");
                iColumn = iColumn + 1;
            }

            if (modelExport.Brand)
            {
                slDocument.SetCellValue(iRow, iColumn, "Brand");
                iColumn = iColumn + 1;
            }
            if (modelExport.Content)
            {
                slDocument.SetCellValue(iRow, iColumn, "Content");
                iColumn = iColumn + 1;
            }
            if (modelExport.Hje)
            {
                slDocument.SetCellValue(iRow, iColumn, "HJE");
                iColumn = iColumn + 1;
            }

            if (modelExport.Tariff)
            {
                slDocument.SetCellValue(iRow, iColumn, "Tariff");
                iColumn = iColumn + 1;
            }

            if (modelExport.ProductType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Product Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.FiscalYear)
            {
                slDocument.SetCellValue(iRow, iColumn, "Fiscal Year");
                iColumn = iColumn + 1;
            }

            if (modelExport.SeriesCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "Series Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.RequestedQty)
            {
                slDocument.SetCellValue(iRow, iColumn, "Requested Qty");
                iColumn = iColumn + 1;
            }

            if (modelExport.ExciseValue)
            {
                slDocument.SetCellValue(iRow, iColumn, "Excise Value");
                iColumn = iColumn + 1;
            }

            //start
            if (modelExport.Remarks)
            {
                slDocument.SetCellValue(iRow, iColumn, "Remarks");
                iColumn = iColumn + 1;
            }
            if (modelExport.Back1Date)
            {
                slDocument.SetCellValue(iRow, iColumn, "BACK-1 Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.Back1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "BACK-1 Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.Ck3Date)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-3 Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.Ck3Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-3 Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.Ck3Value)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-3 Value");
                iColumn = iColumn + 1;
            }
            if (modelExport.PrintingCost)
            {
                slDocument.SetCellValue(iRow, iColumn, "Printing Cost");
                iColumn = iColumn + 1;
            }
            if (modelExport.CompensatedCk1Date)
            {
                slDocument.SetCellValue(iRow, iColumn, "Compensated CK-1 Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.CompensatedCk1Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "Compensated CK-1 Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.PaymentDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Payment Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.Status)
            {
                slDocument.SetCellValue(iRow, iColumn, "Status");
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


            slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(1, 1, iRow - 1, iColumn - 1, styleBorder);

            var fileName = "PBCK4" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
           
            var path = Path.Combine(Server.MapPath(Constans.CK5FolderPath), fileName);

           
            slDocument.SaveAs(path);

            return path;
        }

        #endregion

        #region "Input Manual"

        [HttpPost]
        public JsonResult GetListFaCode(string plantId)
        {
            var brandOutput = _pbck4Bll.GetListFaCodeHaveBlockStockByPlant(plantId);

            //var brandOutput = _pbck4Bll.GetListBrandByPlant(plantId);
            //var model = Mapper.Map<List<Pbck4InputManualViewModel>>(dbMaterial);

            return Json(brandOutput);
        }

        [HttpPost]
        public JsonResult GetListCk1(string nppbkcId)
        {

            var brandOutput = _pbck4Bll.GetListCk1ByNppbkc(nppbkcId);
           
            return Json(brandOutput);
        }

        [HttpPost]
        public JsonResult GetListCk1Date(string plantId, string faCode, string nppbkcId)
        {

            var input = new GetListCk1ByPlantAndFaCodeInput();
            input.NppbkcId = nppbkcId;
            input.PlantId = plantId;
            input.FaCode = faCode;

            var result = _pbck4Bll.GetListCk1ByPlantAndFaCode(input);

            return Json(result);
        }


        [HttpPost]
        public JsonResult GetBrandItems(string plantId, string faCode, string nppbkcId)
        {

            var brandOutput = _pbck4Bll.GetBrandItemsStickerCodeByPlantAndFaCode(plantId, faCode);

            //getblockedstock
            var blockedStockOutput = _pbck4Bll.GetBlockedStockQuota(plantId, faCode);

            //brandOutput.BlockedStock = _pbck4Bll.GetBlockedStockByPlantAndFaCode(plantId, faCode).ToString();
            brandOutput.BlockedStock = blockedStockOutput.BlockedStock;
            brandOutput.BlockedStockUsed = blockedStockOutput.BlockedStockUsed;
            brandOutput.BlockedStockRemaining = blockedStockOutput.BlockedStockRemaining;

            var input = new GetListCk1ByPlantAndFaCodeInput();
            input.NppbkcId = nppbkcId;
            input.PlantId = plantId;
            input.FaCode = faCode;


            //list ck1
            brandOutput.ListCk1Date = _pbck4Bll.GetListCk1ByPlantAndFaCode(input);

            return Json(brandOutput);
        }

     
      
        [HttpPost]
        public JsonResult GetBrandItemsForEdit(int pbck4Id, string plantId, string faCode, string plantIdOri, string faCodeOri, string nppbkcId)
        {

            var brandOutput = _pbck4Bll.GetBrandItemsStickerCodeByPlantAndFaCode(plantId, faCode);

            //getblockedstock
            var blockedStockOutput = _pbck4Bll.GetBlockedStockQuota(plantId, faCode);

            brandOutput.BlockedStock = blockedStockOutput.BlockedStock;
            brandOutput.BlockedStockUsed = blockedStockOutput.BlockedStockUsed;
            brandOutput.BlockedStockRemaining = blockedStockOutput.BlockedStockRemaining;

            if (plantId == plantIdOri && faCode == faCodeOri)
            {
                var reqQty = _pbck4Bll.GetCurrentReqQtyByPbck4IdAndFaCode(pbck4Id, faCode);
                brandOutput.BlockedStockRemaining =
                    (ConvertHelper.ConvertToDecimalOrZero(brandOutput.BlockedStockRemaining) + reqQty).ToString();
            }

            var input = new GetListCk1ByPlantAndFaCodeInput();
            input.NppbkcId = nppbkcId;
            input.PlantId = plantId;
            input.FaCode = faCode;

            //list ck1
            brandOutput.ListCk1Date = _pbck4Bll.GetListCk1ByPlantAndFaCode(input);

            //brandOutput.BlockedStock = _pbck4Bll.GetBlockedStockByPlantAndFaCode(plantId, faCode).ToString();


            return Json(brandOutput);
        }

        [HttpPost]
        public string GetCk1Date(long ck1Id)
        {

            var ck1Date = _pbck4Bll.GetCk1DateByCk1Id(ck1Id);


            return ck1Date;
        }

        #endregion

        #region DashBoard

        public ActionResult Dashboard()
        {
            var model = InitDashboardModel(new Pbck4DashBoardViewModel
            {
                MainMenu = Enums.MenuList.PBCK4,
                CurrentMenu = PageInfo,
                MonthList = GlobalFunctions.GetMonthList(_monthBll),
                YearList = pbck4cDashboardYear(),
                PoaList = GlobalFunctions.GetPoaAll(_poaBll),
                UserList = GlobalFunctions.GetCreatorList()
            });
            return View("Dashboard", model);
        }

        private Pbck4DashBoardViewModel InitDashboardModel(Pbck4DashBoardViewModel model)
        {
            var listPbck4 = GetAllDocument(model);
            model.Detail.DraftTotal = listPbck4.Where(x => x.Status == Enums.DocumentStatus.Draft).Count();
            model.Detail.WaitingForPoaTotal = listPbck4.Where(x => x.Status == Enums.DocumentStatus.WaitingForApproval).Count();
            model.Detail.WaitingForManagerTotal = listPbck4.Where(x => x.Status == Enums.DocumentStatus.WaitingForApprovalManager).Count();
            model.Detail.WaitingForGovTotal = listPbck4.Where(x => x.Status == Enums.DocumentStatus.WaitingGovApproval).Count();
            model.Detail.CompletedTotal = listPbck4.Where(x => x.Status == Enums.DocumentStatus.Completed).Count();
            model.Detail.WaitingForAppTotal = listPbck4.Where(x => x.Status == Enums.DocumentStatus.WaitingForApproval || x.Status == Enums.DocumentStatus.WaitingForApprovalManager).Count();
            

            return model;

        }

        private List<Pbck4Dto> GetAllDocument(Pbck4DashBoardViewModel filter = null)
        {
            if (filter == null)
            {
                var pbck4 = _pbck4Bll.GetAllByParam(new Pbck4DasboardParamInput());
                return pbck4;
            }

            var input = Mapper.Map<Pbck4DasboardParamInput>(filter);
            var dbData = _pbck4Bll.GetAllByParam(input);
            return dbData;
        }

        private SelectList pbck4cDashboardYear()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }

         [HttpPost]
        public PartialViewResult FilterDashboardPage(Pbck4DashBoardViewModel model)
        {
            var data = InitDashboardModel(model);

            return PartialView("_ChartStatus", data.Detail);
        }
        #endregion

    }

   

}