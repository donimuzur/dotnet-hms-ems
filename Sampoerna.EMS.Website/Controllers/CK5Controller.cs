using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;
using Sampoerna.EMS.BusinessObject.Outputs;
using SpreadsheetLight;


namespace Sampoerna.EMS.Website.Controllers
{
    public class CK5Controller : BaseController
    {
        private ICK5BLL _ck5Bll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IMasterDataBLL _masterDataBll;
        private IPBCK1BLL _pbck1Bll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IPlantBLL _plantBll;
        private IUnitOfMeasurementBLL _uomBll;

        public CK5Controller(IPageBLL pageBLL, ICK5BLL ck5Bll, IZaidmExNPPBKCBLL nppbkcBll,
            IMasterDataBLL masterDataBll, IPBCK1BLL pbckBll, IWorkflowHistoryBLL workflowHistoryBll,
            IChangesHistoryBLL changesHistoryBll, IZaidmExGoodTypeBLL goodTypeBll,
            IPlantBLL plantBll, IUnitOfMeasurementBLL uomBll)
            : base(pageBLL, Enums.MenuList.CK5)
        {
            _ck5Bll = ck5Bll;
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterDataBll;
            _pbck1Bll = pbckBll;
            _workflowHistoryBll = workflowHistoryBll;
            _changesHistoryBll = changesHistoryBll;
            _goodTypeBll = goodTypeBll;
            _plantBll = plantBll;
            _uomBll = uomBll;
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
            model.DetailsList = GetCk5Items(ck5Type);
            if (ck5Type == Enums.CK5Type.Domestic)
            {
                model.DetailList2 = GetCk5Items(Enums.CK5Type.Intercompany);
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
            //only use by domestic and importer

            Enums.CK5Type ck5Type = Enums.CK5Type.Domestic;

            if (model.Ck5Type == Enums.CK5Type.Domestic)
                ck5Type = Enums.CK5Type.Intercompany;
            else if (model.Ck5Type == Enums.CK5Type.PortToImporter)
                ck5Type = Enums.CK5Type.ImporterToPlant;

            model.DetailList2 = GetCk5Items(ck5Type, model.SearchView);
            return PartialView("_CK5IntercompanyTablePartial", model);
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

        public ActionResult CK5DomesticAlcohol()
        {
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.DomesticAlcohol);
            return View(model);
        }

        public ActionResult CK5Completed()
        {
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.Completed);
            return View(model);
        }

        public ActionResult CK5Import()
        {
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.PortToImporter);
            return View("CK5Import", model);
        }

        #endregion


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

            //submission date
            model.SubmissionDate = DateTime.Now;

            return model;
        }

        private CK5FormViewModel InitCK5List(CK5FormViewModel model)
        {

            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

            model.KppBcCityList = GlobalFunctions.GetKppBcCityList();
            model.GoodTypeList = GlobalFunctions.GetGoodTypeGroupList();
            model.ExciseSettlementList = GlobalFunctions.GetExciseSettlementList();
            model.ExciseStatusList = GlobalFunctions.GetExciseStatusList();
            model.RequestTypeList = GlobalFunctions.GetRequestTypeList();

            model.SourcePlantList = GlobalFunctions.GetSourcePlantList();
            model.DestPlantList = GlobalFunctions.GetSourcePlantList();

            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();
            model.CarriageMethodList = GlobalFunctions.GetCarriageMethodList();

            model.PackageUomList = GlobalFunctions.GetUomList();

            return model;
        }

        public ActionResult CreateDomestic()
        {
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
            return View("Create", model);
        }

        public ActionResult CreateManual()
        {
            var model = InitCreateCK5(Enums.CK5Type.Manual);
            return View("Create", model);
        }

        public ActionResult CreateDomesticAlcohol()
        {
            var model = InitCreateCK5(Enums.CK5Type.DomesticAlcohol);
            return View("Create", model);
        }

        [HttpPost]
        public JsonResult CeOfficeCodePartial(long nppBkcCityId)
        {
            var ceOfficeCode = _nppbkcBll.GetCeOfficeCodeByNppbcId(nppBkcCityId);
            return Json(ceOfficeCode);
        }

        [HttpPost]
        public JsonResult GetSourcePlantDetails(long plantId)
        {
            var dbPlant = _masterDataBll.GetPlantById(plantId);
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
        public ActionResult SaveCK5(CK5FormViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.UploadItemModels.Count > 0)
                    {
                        var saveResult = SaveCk5ToDatabase(model);

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
        public PartialViewResult UploadFile(HttpPostedFileBase itemExcelFile, long plantId)
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
            model.GoodTypeList = GlobalFunctions.GetGoodTypeGroupList();
            model.ExciseSettlementList = GlobalFunctions.GetExciseSettlementList();
            model.ExciseStatusList = GlobalFunctions.GetExciseStatusList();
            model.RequestTypeList = GlobalFunctions.GetRequestTypeList();

            model.SourcePlantList = GlobalFunctions.GetSourcePlantList();
            model.DestPlantList = GlobalFunctions.GetSourcePlantList();

            model.PbckDecreeList = GlobalFunctions.GetPbck1CompletedList();
            model.CarriageMethodList = GlobalFunctions.GetCarriageMethodList();

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

                model.RequestTypeId = ck5Details.Ck5Dto.REQUEST_TYPE_ID;

                model = InitEdit(model);
               
                model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(ck5Details.Ck5MaterialDto);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(ck5Details.ListChangesHistorys);
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(ck5Details.ListWorkflowHistorys);
                
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CK5FormViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.UploadItemModels.Count > 0)
                    {
                        SaveCk5ToDatabase(model);

                        AddMessageInfo("Success", Enums.MessageInfoType.Success);

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
            model.WorkflowHistory =
                Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.SubmissionNumber));

            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5,
                    model.Ck5Id));

            return model;
        }
      
        public ActionResult Details(long id)
        {
            var model = new CK5FormViewModel();

            try
            {
                var ck5Details = _ck5Bll.GetDetailsCK5(id);

                
                Mapper.Map(ck5Details.Ck5Dto, model);

                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;

                // model = GetInitDetailsData(model);
                model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(ck5Details.Ck5MaterialDto);
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(ck5Details.ListChangesHistorys);
                model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(ck5Details.ListWorkflowHistorys);

               
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
            var dataToSave = Mapper.Map<CK5Dto>(model);
            dataToSave.CREATED_BY = CurrentUser.USER_ID;

            var input = new CK5SaveInput()
            {
                Ck5Dto = dataToSave,
                UserId = CurrentUser.USER_ID,
                WorkflowActionType = Enums.ActionType.Save,
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

            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, ck5Id);

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
          
            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, ck5Id);

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
    }
}