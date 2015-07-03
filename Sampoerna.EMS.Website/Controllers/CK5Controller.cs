using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Utility;


namespace Sampoerna.EMS.Website.Controllers
{
    public class CK5Controller : BaseController
    {
        private ICK5BLL _ck5Bll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IMasterDataBLL _masterDataBll;
        private IPBCK1BLL _pbck1Bll;

        public CK5Controller(IPageBLL pageBLL, ICK5BLL ck5Bll, IZaidmExNPPBKCBLL nppbkcBll, IMasterDataBLL masterDataBll, IPBCK1BLL pbckBll)
            : base(pageBLL, Enums.MenuList.CK5)
        {
            _ck5Bll = ck5Bll;
            _nppbkcBll = nppbkcBll;
            _masterDataBll = masterDataBll;
            _pbck1Bll = pbckBll;
        }

        #region View Documents

        private List<CK5Item> GetCk5Items(Enums.CK5Type ck5Type,CK5SearchViewModel filter = null)
        {
            CK5Input input;

            if (filter == null)
            {
                //Get All
                //input = new CK5Input { Ck5Type = ck5Type };
                input = new CK5Input();
                return Mapper.Map<List<CK5Item>>(_ck5Bll.GetCK5ByParam(input));
            }

            //getbyparams

            input = Mapper.Map<CK5Input>(filter);
            input.Ck5Type = ck5Type;
            return Mapper.Map<List<CK5Item>>(_ck5Bll.GetCK5ByParam(input));
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

            return model;
        }

        public ActionResult CreateDomestic()
        {
            var model = InitCreateCK5(Enums.CK5Type.Domestic);
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
            var ceOfficeCode = _masterDataBll.GetCeOfficeCodeByKppbcId(kppBcCityId);
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
            var pbck1 = _pbck1Bll.GetById(pbck1Id);
            
            return Json(pbck1.DECREE_DATE.HasValue ? pbck1.DECREE_DATE.Value.ToString("dd/MM/yyyy"):string.Empty);
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

                dbCk5.SUBMISSION_DATE = DateTime.Now;
                dbCk5.STATUS_ID = Enums.DocumentStatus.Draft;
                dbCk5.CREATED_DATE = DateTime.Now;
                dbCk5.CREATED_BY = CurrentUser.USER_ID;

                _ck5Bll.SaveCk5(dbCk5);

                //success.. redirect to edit form
                return RedirectToAction("Edit", "CK5", new {@id = dbCk5.CK5_ID});



            }
            if (model.KppBcCity == 0)
            {
                ModelState.AddModelError("KppBcCity", "KppBcCity is required");
            }
            model = InitCK5List(model);
            
            return View("Create", model);
        }

        [HttpPost]
        public PartialViewResult UploadFile(HttpPostedFileBase itemExcelFile)
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

                        model.UploadItemModels.Add(uploadItem);

                    }
                    catch (Exception)
                    {
                        continue;

                    }

                }
            }
            return PartialView("_CK5UploadList", model);
        }

        private CK5EditViewModel GetInitEditData(CK5EditViewModel model)
        {
            
            model.CeOfficeCode = _masterDataBll.GetCeOfficeCodeByKppbcId(model.KppBcCity);
            
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

        [HttpPost]
        public ActionResult Edit(CK5EditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var dbData = _ck5Bll.GetById(model.Ck5Id);

                Mapper.Map(model, dbData);

                _ck5Bll.SaveCk5(dbData);
                
            }

            model = InitEdit(model);
            model = GetInitEditData(model);

            return View(model);
        }
    }
}