using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;


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

        private List<CK5Item> GetCk5Items(Enums.CK5Type ck5Type,CK5SearchViewModel filter = null)
        {
            CK5Input input;
            List<CK5> dbData;
            if (filter == null)
            {
                //Get All
                //input = new CK5Input { Ck5Type = ck5Type };
                input = new CK5Input();
                input.Ck5Type = ck5Type;

                dbData = _ck5Bll.GetCK5ByParam(input);
                return Mapper.Map<List<CK5Item>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<CK5Input>(filter);
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

            var dbCk5 = _ck5Bll.GetAll();
            model.SearchView.DocumentNumberList = new SelectList(dbCk5, "SUBMISSION_NUMBER", "SUBMISSION_NUMBER");
            model.SearchView.POAList = GlobalFunctions.GetPoaAll();
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();

            //null ?
            var sourcePlant = dbCk5.Select(c => c.T1001W.ZAIDM_EX_NPPBKC).ToList().Distinct();
            var destinationPlant = dbCk5.Select(c => c.T1001W1.ZAIDM_EX_NPPBKC).ToList().Distinct();

            model.SearchView.NPPBKCOriginList = new SelectList(sourcePlant, "NPPBKC_ID", "NPPBKC_NO");
            model.SearchView.NPPBKCDestinationList = new SelectList(destinationPlant, "NPPBKC_ID", "NPPBKC_NO");

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
            var model = CreateInitModelView(Enums.MenuList.CK5, Enums.CK5Type.Domestic);
            return View(model);
        }

        [HttpPost]
        public PartialViewResult Intercompany(CK5IndexViewModel model)
        {
            //only use by domestic and importer

            Enums.CK5Type ck5Type= Enums.CK5Type.Domestic;

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
            return View("CK5Import",model);
        }

        #endregion


        public ActionResult Create(string ck5Type)
        {
            var model = new CK5CreateViewModel();
            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;
            if (ck5Type == Enums.CK5Type.Domestic.ToString())
                model.Ck5Type = Enums.CK5Type.Domestic;
            else if (ck5Type == Enums.CK5Type.Intercompany.ToString())
                model.Ck5Type = Enums.CK5Type.Intercompany;

            return View(model);
        }

        private CK5CreateViewModel InitCreateCK5(Enums.CK5Type ck5Type)
        {
            var model = new CK5CreateViewModel();
            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;
            model.Ck5Type = ck5Type;
            model.DocumentStatus = Enums.DocumentStatus.Draft;
            model = InitCK5List(model);

            //submission date
            model.SubmissionDate = DateTime.Now;

            return model;
        }

        private CK5CreateViewModel InitCK5List(CK5CreateViewModel model )
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
        public JsonResult CeOfficeCodePartial(long kppBcCityId)
        {
            var ceOfficeCode = _nppbkcBll.GetCityByNppbkcId(kppBcCityId);
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
        public ActionResult SaveCK5(CK5CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var dbCk5 = Mapper.Map<CK5>(model);
                //todo : put it into mapper
                CK5 dbCk5 = new CK5();
                dbCk5.CK5_TYPE = model.Ck5Type;
                dbCk5.KPPBC_CITY = model.KppBcCity;
                dbCk5.SUBMISSION_NUMBER = model.SubmissionNumber;
                dbCk5.REGISTRATION_NUMBER = model.RegistrationNumber;
                dbCk5.EX_GOODS_TYPE_ID = model.GoodTypeId;
                dbCk5.EX_SETTLEMENT_ID = model.ExciseSettlement;
                dbCk5.EX_STATUS_ID = model.ExciseStatus;
                dbCk5.REQUEST_TYPE_ID = model.RequestType;
                //dbCk5.SUBMISSION_DATE = model.SubmissionDate;
                dbCk5.SOURCE_PLANT_ID = model.SourcePlantId;
                dbCk5.DEST_PLANT_ID = model.DestPlantId;
                dbCk5.INVOICE_NUMBER = model.InvoiceNumber;
                dbCk5.PBCK1_DECREE_ID = model.PbckDecreeId;
                dbCk5.CARRIAGE_METHOD_ID = model.CarriageMethod;
                dbCk5.GRAND_TOTAL_EX = model.GrandTotalEx;
                dbCk5.INVOICE_DATE = model.InvoiceDate;
                dbCk5.PACKAGE_UOM_ID = model.PackageUomId;

                dbCk5.SUBMISSION_DATE = DateTime.Now;
                dbCk5.STATUS_ID = Enums.DocumentStatus.Draft;
                dbCk5.CREATED_DATE = DateTime.Now;
                dbCk5.CREATED_BY = CurrentUser.USER_ID;
               
               // _ck5Bll.SaveCk5(dbCk5);

                //success.. redirect to edit form
                return RedirectToAction("Edit", "CK5", new {@id = dbCk5.CK5_ID});

            }
            else
            {
                AddMessageInfo("Error message", Enums.MessageInfoType.Error);
            }
            if (model.KppBcCity == 0)
            {
                ModelState.AddModelError("KppBcCity", "KppBcCity is required");
            }
            model = InitCK5List(model);
            
            return View("Create", model);
        }

     
        [HttpPost]
        public PartialViewResult UploadFile(HttpPostedFileBase itemExcelFile, long plantId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new CK5CreateViewModel();
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

        private CK5EditViewModel GetInitEditData(CK5EditViewModel model)
        {
            
            model.CeOfficeCode = _nppbkcBll.GetCityByNppbkcId(model.KppBcCity);
            
            var dbPlant = _masterDataBll.GetPlantById(model.SourcePlantId);
            model.SourceNpwp = dbPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
            model.SourceNppbkcId = dbPlant.NPPBCK_ID.ToString();
            model.SourceCompanyName = dbPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
            model.SourceAddress = dbPlant.ADDRESS;
            //var model = Mapper.Map<CK5PlantModel>(dbPlant);

            var dbDestPlant = _masterDataBll.GetPlantById(model.DestPlantId);
            model.DestNpwp = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
            model.DestNppbkcId = dbDestPlant.NPPBCK_ID.ToString();
            model.DestCompanyName = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
            model.DestAddress = dbDestPlant.ADDRESS;

            //pbck
            if (model.PbckDecreeId.HasValue)
                model.PbckDecreeDate = GetDatePbck1ByPbckId(model.PbckDecreeId);


            //model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(Enums.FormType.CK5, model.Ck5Id));
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, model.Ck5Id));


            return model;

        }

        private CK5EditViewModel InitEdit(CK5EditViewModel model)
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
            var dbData = _ck5Bll.GetById(id);

            var model = new CK5EditViewModel();
            Mapper.Map(dbData, model);

            model = InitEdit(model);
            model = GetInitEditData(model);

            //model.SubmissionNumber = dbData.SUBMISSION_NUMBER;
            //model.SubmissionDate = dbData.SUBMISSION_DATE;

            return View(model);
        }

        private void SetWorkflowHistory(long id, Enums.ActionType actionType)
        {
            //WORKFLOW_HISTORY dbWorkflow = _workflowHistoryBll.GetByActionAndFormId(actionType, id);
            //if (dbWorkflow == null)
            //    dbWorkflow = new WORKFLOW_HISTORY();

            //dbWorkflow.FORM_TYPE_ID = Enums.FormType.CK5;
            //dbWorkflow.FORM_ID = id;
            //dbWorkflow.ACTION = actionType;
            //dbWorkflow.ACTION_BY = CurrentUser.USER_ID;
            //dbWorkflow.ACTION_DATE = DateTime.Now;

            //_workflowHistoryBll.Save(dbWorkflow);
        }

        [HttpPost]
        public ActionResult Edit(CK5EditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var dbData = _ck5Bll.GetByIdIncludeTables(model.Ck5Id);

                SetChangesLog(dbData, model);

                //Mapper.Map(model, dbData);
                //todo : put it into mapper
                dbData.STATUS_ID = Enums.DocumentStatus.Draft;
                dbData.CK5_TYPE = model.Ck5Type;
                dbData.KPPBC_CITY = model.KppBcCity;
                //dbData.SUBMISSION_NUMBER = model.SubmissionNumber;
                //dbData.SUBMISSION_DATE = DateTime.Now;
                dbData.REGISTRATION_NUMBER = model.RegistrationNumber;
                dbData.EX_GOODS_TYPE_ID = model.GoodTypeId;
                dbData.EX_SETTLEMENT_ID = model.ExciseSettlement;
                dbData.EX_STATUS_ID = model.ExciseStatus;
                dbData.REQUEST_TYPE_ID = model.RequestType;
                
                dbData.SOURCE_PLANT_ID = model.SourcePlantId;
                dbData.DEST_PLANT_ID = model.DestPlantId;
                dbData.INVOICE_NUMBER = model.InvoiceNumber;
                dbData.PBCK1_DECREE_ID = model.PbckDecreeId;
                dbData.CARRIAGE_METHOD_ID = model.CarriageMethod;
                dbData.GRAND_TOTAL_EX = model.GrandTotalEx;
                dbData.INVOICE_DATE = model.InvoiceDate;
                dbData.PACKAGE_UOM_ID = model.PackageUomId;
                
               
                dbData.MODIFIED_DATE = DateTime.Now;
                

                //workflowhistory
                SetWorkflowHistory(dbData.CK5_ID, Enums.ActionType.Save);

                _ck5Bll.SaveCk5(dbData);
              
            }

            model = InitEdit(model);
            model = GetInitEditData(model);

           
            return View(model);
        }

        private void SetChangesLog(CK5 origin, CK5EditViewModel updatedModel)
        {
            var changesData = new Dictionary<string, bool>();
          
            changesData.Add("KPPBC_CITY", origin.KPPBC_CITY.Equals(updatedModel.KppBcCity));
            changesData.Add("REGISTRATION_NUMBER", origin.REGISTRATION_NUMBER == updatedModel.RegistrationNumber);

            changesData.Add("EX_GOODS_TYPE_ID", origin.EX_GOODS_TYPE_ID.Equals(updatedModel.GoodTypeId));
            changesData.Add("EX_SETTLEMENT_ID", origin.EX_SETTLEMENT_ID.Equals(updatedModel.ExciseSettlement));
            changesData.Add("EX_STATUS_ID", origin.EX_STATUS_ID.Equals(updatedModel.ExciseStatus));
            changesData.Add("REQUEST_TYPE_ID", origin.REQUEST_TYPE_ID.Equals(updatedModel.RequestType));
            changesData.Add("SOURCE_PLANT_ID", origin.SOURCE_PLANT_ID.Equals(updatedModel.SourcePlantId));
            changesData.Add("DEST_PLANT_ID", origin.DEST_PLANT_ID.Equals(updatedModel.DestPlantId));

            changesData.Add("INVOICE_NUMBER", origin.INVOICE_NUMBER == updatedModel.InvoiceNumber);
            changesData.Add("INVOICE_DATE", origin.INVOICE_DATE.Equals(updatedModel.InvoiceDate));

            changesData.Add("PBCK1_DECREE_ID", origin.PBCK1_DECREE_ID.Equals(updatedModel.PbckDecreeId));
            changesData.Add("CARRIAGE_METHOD_ID", origin.CARRIAGE_METHOD_ID.Equals(updatedModel.CarriageMethod));

            changesData.Add("GRAND_TOTAL_EX", origin.GRAND_TOTAL_EX.Equals(updatedModel.GrandTotalEx));
            changesData.Add("PACKAGE_UOM_ID", origin.PACKAGE_UOM_ID.Equals(updatedModel.PackageUomId));

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.CK5;
                changes.FORM_ID = origin.CK5_ID;
                changes.FIELD_NAME = listChange.Key;
                changes.MODIFIED_BY = CurrentUser.USER_ID;
                changes.MODIFIED_DATE = DateTime.Now;
                switch (listChange.Key)
                {
                    case "KPPBC_CITY":
                        long city = 0;
                        if (origin.KPPBC_CITY.HasValue)
                            city = origin.KPPBC_CITY.Value;

                        changes.OLD_VALUE = _nppbkcBll.GetCityByNppbkcId(city);
                        changes.NEW_VALUE = _nppbkcBll.GetCityByNppbkcId(updatedModel.KppBcCity);
                        break;
                    case "REGISTRATION_NUMBER":
                        changes.OLD_VALUE = origin.REGISTRATION_NUMBER;
                        changes.NEW_VALUE = updatedModel.RegistrationNumber;
                        break;
                    case "EX_GOODS_TYPE_ID":
                        changes.OLD_VALUE = _goodTypeBll.GetGoodTypeDescById(origin.EX_GOODS_TYPE_ID);
                        changes.NEW_VALUE = _goodTypeBll.GetGoodTypeDescById(updatedModel.GoodTypeId);
                        break;
                    case "EX_SETTLEMENT_ID":
                        changes.OLD_VALUE = _masterDataBll.GetExSettlementsNameById(origin.EX_SETTLEMENT_ID);
                        changes.NEW_VALUE = _masterDataBll.GetExSettlementsNameById(updatedModel.ExciseSettlement);
                        break;
                    case "EX_STATUS_ID":
                        changes.OLD_VALUE = _masterDataBll.GetExStatusNameById(origin.EX_STATUS_ID);
                        changes.NEW_VALUE = _masterDataBll.GetExStatusNameById(updatedModel.ExciseStatus);
                        break;
                    case "REQUEST_TYPE_ID":
                        changes.OLD_VALUE = _masterDataBll.GetRequestTypeNameById(origin.REQUEST_TYPE_ID);
                        changes.NEW_VALUE = _masterDataBll.GetRequestTypeNameById(updatedModel.ExciseStatus);
                        break;
                    case "SOURCE_PLANT_ID":
                        long sourcePlant = 0;
                        if (origin.SOURCE_PLANT_ID.HasValue)
                            sourcePlant = origin.SOURCE_PLANT_ID.Value;
                        changes.OLD_VALUE = _plantBll.GetPlantWerksById(sourcePlant);
                        changes.NEW_VALUE = _plantBll.GetPlantWerksById(updatedModel.SourcePlantId);
                        break;
                    case "DEST_PLANT_ID":
                        long destPlant = 0;
                        if (origin.DEST_PLANT_ID.HasValue)
                            destPlant = origin.DEST_PLANT_ID.Value;
                        changes.OLD_VALUE = _plantBll.GetPlantWerksById(destPlant);
                        changes.NEW_VALUE = _plantBll.GetPlantWerksById(updatedModel.DestPlantId);
                        break;
                    case "INVOICE_NUMBER":
                        changes.OLD_VALUE = origin.INVOICE_NUMBER;
                        changes.NEW_VALUE = updatedModel.InvoiceNumber;
                        break;
                    case "INVOICE_DATE":
                        changes.OLD_VALUE = origin.INVOICE_DATE != null? origin.INVOICE_DATE.Value.ToString("dd MMM yyyy"): string.Empty;
                        changes.NEW_VALUE = updatedModel.InvoiceDate != null? updatedModel.InvoiceDate.Value.ToString("dd MMM yyyy"): string.Empty;
                        break;
                    case "PBCK1_DECREE_ID":
                         long pbckNumber = 0;
                        if (origin.PBCK1_DECREE_ID.HasValue)
                            pbckNumber = origin.PBCK1_DECREE_ID.Value;
                        changes.OLD_VALUE = _pbck1Bll.GetPbckNumberById(pbckNumber);

                        long updateNumber = 0;
                        if (updatedModel.PbckDecreeId.HasValue)
                            updateNumber = updatedModel.PbckDecreeId.Value;

                        changes.NEW_VALUE = _pbck1Bll.GetPbckNumberById(updateNumber);
                        break;

                    case "CARRIAGE_METHOD_ID":
                        changes.OLD_VALUE = _masterDataBll.GetCarriageMethodeNameById(origin.CARRIAGE_METHOD_ID);
                        changes.NEW_VALUE = _masterDataBll.GetCarriageMethodeNameById(updatedModel.CarriageMethod);
                        break;

                    case "GRAND_TOTAL_EX":
                        changes.OLD_VALUE = origin.GRAND_TOTAL_EX.ToString();
                        changes.NEW_VALUE = updatedModel.GrandTotalEx.ToString();
                        break;

                    case "PACKAGE_UOM_ID":
                        changes.OLD_VALUE = _uomBll.GetUomNameById(origin.PACKAGE_UOM_ID);
                        changes.NEW_VALUE = _uomBll.GetUomNameById(updatedModel.PackageUomId);
                        break;


                }
                _changesHistoryBll.AddHistory(changes);
            }
        }

        private CK5DetailsViewModel GetInitDetailsData(CK5DetailsViewModel model)
        {

            model.CeOfficeCode = _nppbkcBll.GetCityByNppbkcId(model.KppBcCityId);
            model.KppBcCity = _nppbkcBll.GetCityByNppbkcId(model.KppBcCityId);

            var dbPlant = _masterDataBll.GetPlantById(model.SourcePlantId);
            model.SourcePlantName = dbPlant.NAME1 + " - " + dbPlant.CITY;
            model.SourceNpwp = dbPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
            model.SourceNppbkcId = dbPlant.NPPBCK_ID.ToString();
            model.SourceCompanyName = dbPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
            model.SourceAddress = dbPlant.ADDRESS;
            
            var dbDestPlant = _masterDataBll.GetPlantById(model.DestPlantId);
            model.DestPlantName = dbDestPlant.NAME1 + " - " + dbDestPlant.CITY;
            model.DestNpwp = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
            model.DestNppbkcId = dbDestPlant.NPPBCK_ID.ToString();
            model.DestCompanyName = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
            model.DestAddress = dbDestPlant.ADDRESS;

            //model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(Enums.FormType.CK5, model.Ck5Id));
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, model.Ck5Id));

            return model;

        }

        public ActionResult Details(long id)
        {
            var dbData = _ck5Bll.GetByIdIncludeTables(id);

            var model = new CK5DetailsViewModel();
            Mapper.Map(dbData, model);

            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

            model = GetInitDetailsData(model);

            

            return View(model);
        }


    }
}