using System.Configuration;
using System.Data;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.LACK2;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Website.Models;

namespace Sampoerna.EMS.Website.Controllers
{
    public class LACK2Controller : BaseController
    {

        private ILACK2BLL _lack2Bll;
        private IPlantBLL _plantBll;
        private ICompanyBLL _companyBll;
        private IZaidmExGoodTypeBLL _exGroupBll;

        private Enums.MenuList _mainMenu;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IPOABLL _poabll;
        private IMonthBLL _monthBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IDocumentSequenceNumberBLL _documentSequenceNumberBll;
        private ICK5BLL _ck5Bll;
        private IPBCK1BLL _pbck1Bll;
        private IHeaderFooterBLL _headerFooterBll;
        public LACK2Controller(IPageBLL pageBll, IPOABLL poabll, IHeaderFooterBLL headerFooterBll, IPBCK1BLL pbck1Bll, IZaidmExGoodTypeBLL goodTypeBll, IMonthBLL monthBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK2BLL lack2Bll,
            IPlantBLL plantBll, ICompanyBLL companyBll, ICK5BLL ck5Bll, IDocumentSequenceNumberBLL documentSequenceNumberBll, IZaidmExGoodTypeBLL exGroupBll)
            : base(pageBll, Enums.MenuList.LACK2)
        {
            _lack2Bll = lack2Bll;
            _plantBll = plantBll;
            _companyBll = companyBll;
            _exGroupBll = exGroupBll;
            _mainMenu = Enums.MenuList.LACK2;
            _nppbkcbll = nppbkcbll;
            _poabll = poabll;
            _monthBll = monthBll;
            _goodTypeBll = goodTypeBll;
            _documentSequenceNumberBll = documentSequenceNumberBll;
            _ck5Bll = ck5Bll;
            _pbck1Bll = pbck1Bll;
            _headerFooterBll = headerFooterBll;
        }


        #region List by NPPBKC

        // GET: LACK2
        public ActionResult Index()
        {
            var model = new Lack2IndexViewModel();
            model = InitViewModel(model);

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var dbData = _lack2Bll.GetAll(new Lack2GetByParamInput());
            model.Details = dbData.Select(d => Mapper.Map<LACK2NppbkcData>(d)).ToList();

            return View("Index", model);
        }

        /// <summary>
        /// Fills the select lists for the IndexViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Lack2IndexViewModel</returns>
        private Lack2IndexViewModel InitViewModel(Lack2IndexViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        /// <summary>
        /// Create LACK2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            LACK2CreateViewModel model = new LACK2CreateViewModel();

            model.NPPBKCDDL = GlobalFunctions.GetAuthorizedNppbkc(CurrentUser.NppbckPlants);
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList(_companyBll);
            //model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.SendingPlantDDL = GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, null);
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = GlobalFunctions.GetYearList();
            model.UsrRole = CurrentUser.UserRole;

            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(LACK2CreateViewModel model)
        {

            Lack2Dto item = new Lack2Dto();

            item = AutoMapper.Mapper.Map<Lack2Dto>(model.Lack2Model);

            var plant = _plantBll.GetT001ById(model.Lack2Model.LevelPlantId);
            var company = _companyBll.GetById(model.Lack2Model.Burks);
            var goods = _exGroupBll.GetById(model.Lack2Model.ExGoodTyp);

            item.ExTypDesc = goods.EXT_TYP_DESC;
            item.Butxt = company.BUTXT;
            item.LevelPlantName = plant.NAME1;
            item.LevelPlantCity = plant.ORT01;
            item.LevelPlantId = plant.WERKS;
            item.PeriodMonth = model.Lack2Model.PeriodMonth;
            item.PeriodYear = model.Lack2Model.PeriodYear;
            item.CreatedBy = CurrentUser.USER_ID;
            item.CreatedDate = DateTime.Now;
             var inputDoc = new GenerateDocNumberInput();
            inputDoc.Month = item.PeriodMonth;
            inputDoc.Year = item.PeriodYear;
            inputDoc.NppbkcId = item.NppbkcId;
            item.Lack2Number = _documentSequenceNumberBll.GenerateNumber(inputDoc);
            item.Items = model.Lack2Model.Items.Select(x=>Mapper.Map<Lack2ItemDto>(x)).ToList();
            
             item.Status = Enums.DocumentStatus.Draft;
            

            _lack2Bll.Insert(item);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edits the LACK2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            LACK2CreateViewModel model = new LACK2CreateViewModel();

            model.Lack2Model = AutoMapper.Mapper.Map<LACK2Model>(_lack2Bll.GetById(id));

            model.NPPBKCDDL = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList(_companyBll);
            model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeGroupList();
            model.SendingPlantDDL = GlobalFunctions.GetPlantAll();

            model.UsrRole = CurrentUser.UserRole;

            var govStatuses = from Enums.DocumentStatusGov ds in Enum.GetValues(typeof(Enums.DocumentStatusGov))
                              select new { ID = (int)ds, Name = ds.ToString() };

            model.GovStatusDDL = new SelectList(govStatuses, "ID", "Name");

            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;
            model.Lack2Model.LACK2Period = new DateTime(model.Lack2Model.PeriodYear, model.Lack2Model.PeriodMonth, 1);
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(LACK2CreateViewModel model)
        {

            Lack2Dto item = new Lack2Dto();

            item = AutoMapper.Mapper.Map<Lack2Dto>(model.Lack2Model);

            var plant = _plantBll.GetAll().Where(p => p.WERKS == model.Lack2Model.LevelPlantId).FirstOrDefault();
            var company = _companyBll.GetById(model.Lack2Model.Burks);
            var goods = _exGroupBll.GetById(model.Lack2Model.ExGoodTyp);

            item.ExTypDesc = goods.EXT_TYP_DESC;

            item.Butxt = company.BUTXT;
            item.LevelPlantName = plant.NAME1;
            item.LevelPlantCity = plant.ORT01;
            item.PeriodMonth = model.Lack2Model.LACK2Period.Month;
            item.PeriodYear = model.Lack2Model.LACK2Period.Year;

            if (CurrentUser.UserRole == Enums.UserRole.POA) // && if a file is uploaded needs to be added
            {
                item.Status = Enums.DocumentStatus.WaitingForApprovalManager;
            }

            if (CurrentUser.UserRole == Enums.UserRole.Manager)// && if a file is uploaded needs to be added
            {
                item.Status = Enums.DocumentStatus.Completed;
            }

            item.ModifiedBy = CurrentUser.USER_ID;
            item.ModifiedDate = DateTime.Now;

            item.ApprovedBy = CurrentUser.USER_ID;
            item.ApprovedDate = DateTime.Now;

            _lack2Bll.Insert(item);

            return RedirectToAction("Index");
        }

        #endregion

        #region List By Plant

        public ActionResult ListByPlant()
        {
            var data = InitLack2LiistByPlant(new Lack2IndexPlantViewModel
            {

                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,

                Details = Mapper.Map<List<LACK2PlantData>>(_lack2Bll.GetAll(new Lack2GetByParamInput()))

            });

            return View("ListByPlant", data);
        }

        private Lack2IndexPlantViewModel InitLack2LiistByPlant(Lack2IndexPlantViewModel model)
        {
            model.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.PlantIdList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return model;
        }

        #endregion

        #region List Completed Documents

        public ActionResult ListCompletedDoc()
        {
            var model = new Lack2IndexViewModel();

            model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchInput.NppbkcIdList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.SearchInput.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            model.SearchInput.YearList = LackYearList();

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            // gets the completed documents by checking the status
            var dbData = _lack2Bll.GetAllCompleted();
            model.Details = dbData.Select(d => Mapper.Map<LACK2NppbkcData>(d)).ToList();

            return View("ListCompletedDoc", model);
        }

        // this is a cover up for the years we will need a new table or way to get the years for the dropdowns
        private SelectList LackYearList()
        {
            var years = new List<SelectItemModel>();
            var currentYear = DateTime.Now.Year;
            years.Add(new SelectItemModel() { ValueField = currentYear, TextField = currentYear.ToString() });
            years.Add(new SelectItemModel() { ValueField = currentYear - 1, TextField = (currentYear - 1).ToString() });
            return new SelectList(years, "ValueField", "TextField");
        }



        #endregion

        #region PreviewActions

        public ActionResult PreviewDocument(LACK2CreateViewModel model)
        {
            return View();
        }

        #endregion


        #region SearchFilters

        private List<LACK2NppbkcData> GetListByNppbkc(Lack2IndexViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var litsByNppbkc = _lack2Bll.GetAll(new Lack2GetByParamInput());
                return Mapper.Map<List<LACK2NppbkcData>>(litsByNppbkc);
            }
            //get by param
            var input = Mapper.Map<Lack2GetByParamInput>(filter);
            var dbData = _lack2Bll.GetAll(input);

            return Mapper.Map<List<LACK2NppbkcData>>(dbData);

        }

        [HttpPost]
        public PartialViewResult FilterListByNppbkc(Lack2Input model)
        {


            var input = Mapper.Map<Lack2GetByParamInput>(model);

            var dbData = _lack2Bll.GetAllCompletedByParam(input);

            var result = Mapper.Map<List<LACK2NppbkcData>>(dbData);

            var viewModel = new Lack2IndexViewModel();
            viewModel.Details = result;

            return PartialView("_Lack2Table", viewModel);

        }


        private List<LACK2PlantData> GetListByPlant(Lack2IndexPlantViewModel filter = null)
        {
            if (filter == null)
            {
                //get all 
                var litsByNppbkc = _lack2Bll.GetAll(new Lack2GetByParamInput());
                return Mapper.Map<List<LACK2PlantData>>(litsByNppbkc);
            }
            //get by param
            var input = Mapper.Map<Lack2GetByParamInput>(filter);
            var dbData = _lack2Bll.GetAll(input);

            return Mapper.Map<List<LACK2PlantData>>(dbData);

        }

        [HttpPost]
        public PartialViewResult FilterListByPlant(Lack2Input model)
        {

            var inputPlant = Mapper.Map<Lack2GetByParamInput>(model);

            var dbDataPlant = _lack2Bll.GetAll(inputPlant);

            var resultPlant = Mapper.Map<List<LACK2PlantData>>(dbDataPlant);

            var viewModel = new Lack2IndexPlantViewModel();
            viewModel.Details = resultPlant;

            return PartialView("_Lack2ListByPlantTable", viewModel);

        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(LACK2FilterViewModel SearchInput)
        {
            var input = Mapper.Map<Lack2GetByParamInput>(SearchInput);
            // to search trough the completed documents
            input.Status = Enums.DocumentStatus.Completed;

            var dbData = _lack2Bll.GetAllCompletedByParam(input);

            var result = Mapper.Map<List<LACK2NppbkcData>>(dbData);

            var viewModel = new Lack2IndexViewModel();
            viewModel.Details = result;

            return PartialView("_Lack2CompletedDoc", viewModel);
        }

        #endregion


        [HttpPost]
        public JsonResult GetPlantByNppbkcId(string nppbkcid)
        {
            var data = Json(GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, nppbkcid));
            return data;

        }

        [HttpPost]
        public JsonResult GetCK5ByLack2Period(int month, int year, string desPlantId, string goodstype)
        {
            var data =  _ck5Bll.GetByGIDate(month, year, desPlantId).Select(d=>Mapper.Map<CK5Dto>(d)).ToList();
            return Json(data);

        }

        [HttpPost]
        public JsonResult GetGoodsTypeByNPPBKC(string nppbkcid)
        {
            var pbck1list = _pbck1Bll.GetAllByParam(new Pbck1GetByParamInput() {NppbkcId = nppbkcid});
            var data = pbck1list.GroupBy(x => new {x.GoodType, x.GoodTypeDesc}).Select(x=>new SelectItemModel()
            {
               ValueField = x.Key.GoodType,
               TextField = x.Key.GoodType + "-" + x.Key.GoodTypeDesc,
            }).ToList();
            
            return Json(data);

        }

        public ActionResult PrintPreview(int id)
        {
            var lack2 = _lack2Bll.GetById(id);

            DataSet ds = new DataSet("dsLack2");

            DataTable dt = new DataTable("Lack2");

            // object of data row 
            DataRow drow;
            dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
            dt.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dt.Columns.Add("Alamat", System.Type.GetType("System.String"));
            dt.Columns.Add("Header", System.Type.GetType("System.Byte[]"));
            dt.Columns.Add("Footer", System.Type.GetType("System.String"));
            drow = dt.NewRow();
            drow[0] = "company name";
            drow[1] = "nppb ck nn";
            drow[2] = "ssssss";
            drow[3] = GetHeader("~/files_upload/1616_header04082015165943_.jpg");
            drow[4] = "this is footer";
            dt.Rows.Add(drow);


            //detail
            DataTable dtDetail = new DataTable("Lack2Item");

            // object of data row 
            DataRow drowDetail;
            dtDetail.Columns.Add("Nomor", System.Type.GetType("System.String"));
            //dtDetail.Columns.Add("Tanggal", System.Type.GetType("System.String"));
            //dtDetail.Columns.Add("Jumlah", System.Type.GetType("System.String"));

            //dtDetail.Columns.Add("NamaPerusahaan", System.Type.GetType("System.String"));
            //dtDetail.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            //dtDetail.Columns.Add("Alamat", System.Type.GetType("System.String"));
            drowDetail = dtDetail.NewRow();
            drowDetail[0] = "xxxx";
            dtDetail.Rows.Add(drowDetail);


            ds.Tables.Add(dt);
            ds.Tables.Add(dtDetail);
            ReportClass rpt = new ReportClass();
            string report_path = ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = report_path + "LACK2\\Preview.rpt";
            rpt.Load();
            rpt.SetDataSource(dt);

            Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
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
                }
                else
                {
                    // if photo does not exist show the nophoto.jpg file 
                    fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
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
            catch (Exception ex)
            {
            }
            return imgbyte;
            // Return Datatable After Image Row Insertion

        }

    }

}