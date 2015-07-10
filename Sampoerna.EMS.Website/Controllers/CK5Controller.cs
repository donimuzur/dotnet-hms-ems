using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
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

            //null ?
            //var sourcePlant = dbCk5.Select(c => c.T1001W.ZAIDM_EX_NPPBKC).ToList().Distinct();
            //var destinationPlant = dbCk5.Select(c => c.T1001W1.ZAIDM_EX_NPPBKC).ToList().Distinct();

            //model.SearchView.NPPBKCOriginList = new SelectList(sourcePlant, "NPPBKC_ID", "NPPBKC_NO");
            //model.SearchView.NPPBKCDestinationList = new SelectList(destinationPlant, "NPPBKC_ID", "NPPBKC_NO");

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
            if (ModelState.IsValid)
            {
                if (model.UploadItemModels.Count > 0)
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

                    var saveResult = _ck5Bll.SaveCk5(input);

                    if (saveResult.Success)
                    {
                        //success.. redirect to edit form
                        return RedirectToAction("Edit", "CK5", new {@id = saveResult.Id});
                    }

                    AddMessageInfo(saveResult.ErrorMessage, Enums.MessageInfoType.Error);

                }

                AddMessageInfo("Missing CK5 Material", Enums.MessageInfoType.Error);
            }
            else
            {
                AddMessageInfo("Error message", Enums.MessageInfoType.Error);
            }
         
            model = InitCK5List(model);
            
            return View("Create", model);
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

        //private CK5FormViewModel GetInitEditData(CK5FormViewModel model)
        //{
            
        //    model.CeOfficeCode = _nppbkcBll.GetCityByNppbkcId(model.KppBcCityId);
            
        //    var dbPlant = _masterDataBll.GetPlantById(model.SourcePlantId);
        //    model.SourceNpwp = dbPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
        //    model.SourceNppbkcId = dbPlant.NPPBCK_ID.ToString();
        //    model.SourceCompanyName = dbPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
        //    model.SourceAddress = dbPlant.ADDRESS;
        //    //var model = Mapper.Map<CK5PlantModel>(dbPlant);

        //    var dbDestPlant = _masterDataBll.GetPlantById(model.DestPlantId);
        //    model.DestNpwp = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
        //    model.DestNppbkcId = dbDestPlant.NPPBCK_ID.ToString();
        //    model.DestCompanyName = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
        //    model.DestAddress = dbDestPlant.ADDRESS;

        //    //pbck
        //    if (model.PbckDecreeId.HasValue)
        //        model.PbckDecreeDate = GetDatePbck1ByPbckId(model.PbckDecreeId);


        //    //model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormTypeAndFormId(Enums.FormType.CK5, model.Ck5Id));
        //    model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, model.Ck5Id));


        //    return model;

        //}

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
            var ck5Details = _ck5Bll.GetDetailsCk5(id);

            var model = new CK5FormViewModel();
           
            Mapper.Map(ck5Details.Ck5Dto, model);

            model.RequestTypeId = ck5Details.Ck5Dto.REQUEST_TYPE_ID;

            model = InitEdit(model);
           // model = GetInitDetailsData(model);

            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(ck5Details.ListChangesHistorys);
            model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(ck5Details.ListWorkflowHistorys);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CK5FormViewModel model)
        {

            if (ModelState.IsValid)
            {
                //var dbData = _ck5Bll.GetByIdIncludeTables(model.Ck5Id);

                //SetChangesLog(dbData, model);

                ////Mapper.Map(model, dbData);
                ////todo : put it into mapper
                //dbData.STATUS_ID = Enums.DocumentStatus.Draft;
                //dbData.CK5_TYPE = model.Ck5Type;
                //dbData.KPPBC_CITY = model.KppBcCity;
                ////dbData.SUBMISSION_NUMBER = model.SubmissionNumber;
                ////dbData.SUBMISSION_DATE = DateTime.Now;
                //dbData.REGISTRATION_NUMBER = model.RegistrationNumber;
                //dbData.EX_GOODS_TYPE_ID = model.GoodTypeId;
                //dbData.EX_SETTLEMENT_ID = model.ExciseSettlement;
                //dbData.EX_STATUS_ID = model.ExciseStatus;
                //dbData.REQUEST_TYPE_ID = model.RequestType;
                
                //dbData.SOURCE_PLANT_ID = model.SourcePlantId;
                //dbData.DEST_PLANT_ID = model.DestPlantId;
                //dbData.INVOICE_NUMBER = model.InvoiceNumber;
                //dbData.PBCK1_DECREE_ID = model.PbckDecreeId;
                //dbData.CARRIAGE_METHOD_ID = model.CarriageMethod;
                //dbData.GRAND_TOTAL_EX = model.GrandTotalEx;
                //dbData.INVOICE_DATE = model.InvoiceDate;
                //dbData.PACKAGE_UOM_ID = model.PackageUomId;
                
               
                //dbData.MODIFIED_DATE = DateTime.Now;
                

                ////workflowhistory
                //SetWorkflowHistory(dbData.CK5_ID, Enums.ActionType.Save);

                //_ck5Bll.SaveCk5(dbData);

                //process save
                var dataToSave = Mapper.Map<CK5Dto>(model);
                dataToSave.CREATED_BY = CurrentUser.USER_ID;
                var input = new CK5SaveInput()
                {
                    Ck5Dto = dataToSave,
                    UserId = CurrentUser.USER_ID,
                    WorkflowActionType = Enums.ActionType.Save
                };
                var saveResult = _ck5Bll.SaveCk5(input);

               if (!saveResult.Success)
                   AddMessageInfo(saveResult.ErrorMessage, Enums.MessageInfoType.Error);

                //return RedirectToAction()
            }
            else
                AddMessageInfo("Not Valid Model", Enums.MessageInfoType.Error);

            model = InitEdit(model);
            model = GetHistorys(model);
            
            return View(model);
        }

        private CK5FormViewModel GetHistorys(CK5FormViewModel model)
        {
            model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.SubmissionNumber));

            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, model.Ck5Id));

            return model;
        }

        ///// <summary>
        ///// todo : remove this function should be got info in mapper
        ///// </summary>
        ///// <param name="model"></param>
        ///// <param name="isNeedGetData"></param>
        ///// <returns></returns>
        //private CK5FormViewModel GetInitDetailsData(CK5FormViewModel model, bool isNeedGetData = false)
        //{

        //    //long city = 0;
        //    //if (model.KppBcCityId.HasValue)
        //    //    city = model.KppBcCityId.Value;

        //    //model.CeOfficeCode = _nppbkcBll.GetCityByNppbkcId(city);
        //    //model.KppBcCityName = _nppbkcBll.GetCityByNppbkcId(city);

        //    //long sourcePlant = 0;
        //    //if (model.SourcePlantId.HasValue)
        //    //    sourcePlant = model.SourcePlantId.Value;

        //    //var dbPlant = _masterDataBll.GetPlantById(sourcePlant);

        //    //model.SourcePlantName = dbPlant.NAME1 + " - " + dbPlant.CITY;
        //    //model.SourceNpwp = dbPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
        //    //model.SourceNppbkcId = dbPlant.NPPBCK_ID.ToString();
        //    //model.SourceCompanyName = dbPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
        //    //model.SourceAddress = dbPlant.ADDRESS;

        //    //long destPlant = 0;
        //    //if (model.DestPlantId.HasValue)
        //    //    destPlant = model.DestPlantId.Value;

        //    //var dbDestPlant = _masterDataBll.GetPlantById(destPlant);
        //    //model.DestPlantName = dbDestPlant.NAME1 + " - " + dbDestPlant.CITY;
        //    //model.DestNpwp = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.NPWP;
        //    //model.DestNppbkcId = dbDestPlant.NPPBCK_ID.ToString();
        //    //model.DestCompanyName = dbDestPlant.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT;
        //    //model.DestAddress = dbDestPlant.ADDRESS;

        //    if (isNeedGetData)
        //    {
        //        model.WorkflowHistory =Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.SubmissionNumber));

        //        model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, model.Ck5Id));

        //    }
          
            
        //    return model;

        //}

        public ActionResult Details(long id)
        {
            var ck5Details = _ck5Bll.GetDetailsCk5(id);

            var model = new CK5FormViewModel();
            Mapper.Map(ck5Details.Ck5Dto, model);

            model.MainMenu = Enums.MenuList.CK5;
            model.CurrentMenu = PageInfo;

           // model = GetInitDetailsData(model);
            model.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(ck5Details.Ck5MaterialDto);
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(ck5Details.ListChangesHistorys);
            model.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(ck5Details.ListWorkflowHistorys);

            

            return View(model);
        }


    }
}