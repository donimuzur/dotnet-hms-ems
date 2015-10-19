using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DocumentFormat.OpenXml.Spreadsheet;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.PBCK7AndPBCK3;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Utility;
using Sampoerna.EMS.XMLReader;
using SpreadsheetLight;
using Path = System.IO.Path;

namespace Sampoerna.EMS.Website.Controllers
{
    public class PBCK7AndPBCK3Controller : BaseController
    {
        private IPBCK7And3BLL _pbck7Pbck3Bll;
        private IBACK1BLL _back1Bll;
        private Enums.MenuList _mainMenu;
        private IPOABLL _poaBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IPlantBLL _plantBll;
        private IBrandRegistrationBLL _brandRegistration;
        private IDocumentSequenceNumberBLL _documentSequenceNumberBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IWorkflowBLL _workflowBll;
        private IHeaderFooterBLL _headerFooterBll;
        private ILFA1BLL _lfa1Bll;
        private IPrintHistoryBLL _printHistoryBll;
        private IChangesHistoryBLL _changesHistoryBll;
        public PBCK7AndPBCK3Controller(IPageBLL pageBll, IPBCK7And3BLL pbck7AndPbck3Bll, IBACK1BLL back1Bll,
            IPOABLL poaBll, IZaidmExNPPBKCBLL nppbkcBll, IChangesHistoryBLL changesHistoryBll, IPrintHistoryBLL printHistoryBll, ILFA1BLL lfa1Bll, IHeaderFooterBLL headerFooterBll, IWorkflowBLL workflowBll, IWorkflowHistoryBLL workflowHistoryBll, IDocumentSequenceNumberBLL documentSequenceNumberBll, IBrandRegistrationBLL brandRegistrationBll, IPlantBLL plantBll)
            : base(pageBll, Enums.MenuList.PBCK7)
        {
            _pbck7Pbck3Bll = pbck7AndPbck3Bll;
            _back1Bll = back1Bll;
            _mainMenu = Enums.MenuList.PBCK7;
            _poaBll = poaBll;
            _nppbkcBll = nppbkcBll;
            _plantBll = plantBll;
            _brandRegistration = brandRegistrationBll;
            _documentSequenceNumberBll = documentSequenceNumberBll;
            _workflowHistoryBll = workflowHistoryBll;
            _workflowBll = workflowBll;
            _headerFooterBll = headerFooterBll;
            _lfa1Bll = lfa1Bll;
            _printHistoryBll = printHistoryBll;
            _changesHistoryBll = changesHistoryBll;
        }



        [HttpPost]
        public ActionResult AddPrintHistoryPbck7(int id)
        {

            // ReSharper disable once PossibleInvalidOperationException
            var pbck7 = _pbck7Pbck3Bll.GetPbck7ById(id);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.PBCK7,
                FORM_ID = pbck7.Pbck7Id,
                FORM_NUMBER = pbck7.Pbck7Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(pbck7.Pbck7Number));
            return PartialView("_PrintHistoryTable", model);

        }
        [HttpPost]
        public ActionResult AddPrintHistoryPbck3(int id)
        {

            // ReSharper disable once PossibleInvalidOperationException
            var pbck3 = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(id);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.PBCK3,
                FORM_ID = pbck3.Pbck3Id,
                FORM_NUMBER = pbck3.Pbck3Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(pbck3.Pbck3Number));
            return PartialView("_PrintHistoryTable", model);

        }

        #region Index PBCK7

        //
        // GET: /PBCK7/
        public ActionResult Index()
        {
            var data = InitPbck7ViewModel(new Pbck7IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck7Type = Enums.Pbck7Type.Pbck7List,
                IsCompletedDoc = false,
                Detail =
                    Mapper.Map<List<DataListIndexPbck7>>(_pbck7Pbck3Bll.GetPbck7ByParam(new Pbck7AndPbck3Input(), CurrentUser))
            });

            return View("Index", data);
        }

        public ActionResult Pbck7Completed()
        {
            var data = InitPbck7ViewModel(new Pbck7IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck7Type = Enums.Pbck7Type.Pbck7List,
                IsCompletedDoc = true,
                Detail =
                    Mapper.Map<List<DataListIndexPbck7>>(_pbck7Pbck3Bll.GetPbck7ByParam(new Pbck7AndPbck3Input(), CurrentUser, true))
            });

            return View("Index", data);
        }
        #endregion

        private DataSet CreatePbck7Ds()
        {
            DataSet ds = new DataSet("dsPbck7");

            DataTable dt = new DataTable("Master");

            // object of data row 
            DataRow drow;
            dt.Columns.Add("PoaName", Type.GetType("System.String"));
            dt.Columns.Add("CompanyName", Type.GetType("System.String"));
            dt.Columns.Add("CompanyAddress", Type.GetType("System.String"));
            dt.Columns.Add("Nppbkc", Type.GetType("System.String"));
            dt.Columns.Add("Header", Type.GetType("System.Byte[]"));
            dt.Columns.Add("Footer", Type.GetType("System.String"));
            dt.Columns.Add("TotalKemasan", Type.GetType("System.String"));
            dt.Columns.Add("TotalCukai", Type.GetType("System.String"));
            dt.Columns.Add("PrintedDate", Type.GetType("System.String"));
            dt.Columns.Add("Preview", Type.GetType("System.String"));
            dt.Columns.Add("DecreeDate", Type.GetType("System.String"));
            dt.Columns.Add("Nomor", Type.GetType("System.String"));
            dt.Columns.Add("Lampiran", Type.GetType("System.String"));
            dt.Columns.Add("TextTo", Type.GetType("System.String"));
            dt.Columns.Add("VendorCity", Type.GetType("System.String"));
            dt.Columns.Add("DocumentType", Type.GetType("System.String"));
            dt.Columns.Add("NppbkcCity", Type.GetType("System.String"));
            dt.Columns.Add("PbckDate", Type.GetType("System.String"));
            dt.Columns.Add("PoaTitle", Type.GetType("System.String"));
            //detail
            DataTable dtDetail = new DataTable("Detail");
            dtDetail.Columns.Add("Jenis", Type.GetType("System.String"));
            dtDetail.Columns.Add("Merek", Type.GetType("System.String"));
            dtDetail.Columns.Add("IsiKemasan", Type.GetType("System.String"));

            dtDetail.Columns.Add("JmlKemasan", Type.GetType("System.String"));
            dtDetail.Columns.Add("SeriPitaCukai", Type.GetType("System.String"));
            dtDetail.Columns.Add("Hje", Type.GetType("System.String"));
            dtDetail.Columns.Add("Tariff", Type.GetType("System.String"));
            dtDetail.Columns.Add("JmlCukai", Type.GetType("System.String"));
            dtDetail.Columns.Add("SumJmlCukai", Type.GetType("System.String"));
            dtDetail.Columns.Add("SumJmlKemasan", Type.GetType("System.String"));
            ds.Tables.Add(dt);
            ds.Tables.Add(dtDetail);
            return ds;
        }

        [EncryptedParameter]
        public FileResult PrintPreviewPbck7(int id)
        {
            return PrintPreview(id, true, "Preview PBCK-7");
        }
        [EncryptedParameter]
        public FileResult PrintPreviewPbck3(int id)
        {
            return PrintPreview(id, false, "Preview PBCK-3");
        }
        public FileResult PrintPreview(int id, bool isPbck7, string title)
        {
            string poaId = string.Empty;
            var dsPbck7 = CreatePbck7Ds();
            var dt = dsPbck7.Tables[0];
            DataRow drow;
            drow = dt.NewRow();
            var pbck7 = new Pbck7AndPbck3Dto();
            if (isPbck7)
            {
                pbck7 = _pbck7Pbck3Bll.GetPbck7ById(id);
                poaId = !string.IsNullOrEmpty(pbck7.ApprovedBy) ? pbck7.ApprovedBy : pbck7.CreatedBy;
            }
            else
            {
                var pbck3Data = _pbck7Pbck3Bll.GetPbck3ById(id);
                pbck7 = _pbck7Pbck3Bll.GetPbck7ById(pbck3Data.Pbck7Id);
                pbck7.Pbck3Dto = pbck3Data;
                poaId = !string.IsNullOrEmpty(pbck3Data.ApprovedBy) ? pbck3Data.ApprovedBy : pbck3Data.CreatedBy;
            }
            var poaData = _poaBll.GetById(poaId);
            if (poaData != null)
            {
                drow["PoaName"] = poaData.PRINTED_NAME;
                drow["PoaTitle"] = poaData.TITLE;
            }
            else
            {
                drow["PoaName"] = "-";
                drow["PoaTitle"] = "-";
            }

            var company = _plantBll.GetId(pbck7.PlantId);
            var nppbkc = _nppbkcBll.GetById(pbck7.NppbkcId);

            if (company != null)
            {
                drow["CompanyName"] = company.COMPANY_NAME;
                drow["CompanyAddress"] = company.COMPANY_ADDRESS;
                var headerFooter = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput
                {
                    CompanyCode = company.COMPANY_CODE,
                    FormTypeId = Enums.FormType.LACK2
                });

                drow["Nppbkc"] = pbck7.NppbkcId + " tanggal " + nppbkc.START_DATE.Value.ToString("dd MMMM yyyy");
                if (headerFooter != null)
                {
                    drow["Header"] = GetHeader(headerFooter.HEADER_IMAGE_PATH);
                    drow["Footer"] = headerFooter.FOOTER_CONTENT;
                }
            }
            var detailItem = pbck7.UploadItems;
            var totalKemasan = 0;
            var totalCukai = 0.0;
            if (detailItem != null)
            {
                foreach (var item in detailItem)
                {
                    totalKemasan += Convert.ToInt32(item.Content);
                    totalCukai += Convert.ToDouble(item.ExciseValue);
                }
            }

            drow["TotalKemasan"] = totalKemasan;
            drow["TotalCukai"] = totalCukai;
            var pbck3Date = DateTime.Now;
            if (pbck7.Pbck3Dto.Pbck3Date.HasValue)
                pbck3Date = pbck7.Pbck3Dto.Pbck3Date.Value;
            drow["PrintedDate"] = isPbck7 ? pbck7.Pbck7Date.ToString("dd MMM yyyy") : pbck3Date.ToString("dd MMM yyyy");
            drow["Preview"] = title;
            drow["Nomor"] = isPbck7 ? pbck7.Pbck7Number : pbck7.Pbck3Dto.Pbck3Number;
            drow["Lampiran"] = pbck7.Lampiran;

            if (nppbkc != null)
            {
                drow["TextTo"] = nppbkc.TEXT_TO;
                var vendor = _lfa1Bll.GetById(nppbkc.KPPBC_ID);
                if (vendor != null)
                {
                    drow["VendorCity"] = vendor.ORT01;
                }
            }
            drow["DocumentType"] = EnumHelper.GetDescription(pbck7.DocumentType).ToLower();
            drow["NppbkcCity"] = nppbkc.CITY;
            drow["PbckDate"] = isPbck7 ? pbck7.Pbck7Date.ToString("dd MMMM yyyy") : pbck3Date.ToString("dd MMMM yyyy");

            dt.Rows.Add(drow);

            var dtDetail = dsPbck7.Tables[1];
            var totalPbck7Qty = pbck7.UploadItems.Sum(d => d.Pbck7Qty.HasValue ? d.Pbck7Qty.Value : 0);
            var totalExciseValue = pbck7.UploadItems.Sum(d => d.ExciseValue.HasValue ? d.ExciseValue.Value : 0);
            foreach (var item in pbck7.UploadItems)
            {
                DataRow drowDetail;
                drowDetail = dtDetail.NewRow();
                drowDetail[0] = item.ProdTypeAlias;
                drowDetail[1] = item.Brand;
                drowDetail[2] = item.Content.HasValue ? item.Content.Value.ToString("N2") : "-";
                drowDetail[3] = item.Pbck7Qty.HasValue ? item.Pbck7Qty.Value.ToString("N2") : "-";
                drowDetail[4] = item.SeriesValue;
                drowDetail[5] = item.Hje.HasValue ? item.Hje.Value.ToString("N2") : "-";
                drowDetail[6] = item.Tariff.HasValue ? item.Tariff.Value.ToString("N2") : "-";
                drowDetail[7] = item.ExciseValue.HasValue ? item.ExciseValue.Value.ToString("N2") : "-";
                drowDetail[8] = totalExciseValue.ToString("N2");
                drowDetail[9] = totalPbck7Qty.ToString("N2");
                dtDetail.Rows.Add(drowDetail);

            }
            // object of data row 

            ReportClass rpt = new ReportClass();
            string report_path = ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = Path.Combine(report_path, "PBCK7", "Pbck7Report.rpt");
            rpt.Load();
            rpt.SetDataSource(dsPbck7);

            Stream stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
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
            }
            catch (Exception ex)
            {
            }
            return imgbyte;
            // Return Datatable After Image Row Insertion

        }

        private Pbck7IndexViewModel InitPbck7ViewModel(Pbck7IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.CreatorList = GlobalFunctions.GetCreatorList();
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;
            return model;

        }

        [HttpPost]
        public PartialViewResult FilterPbck7Index(Pbck7IndexViewModel model)
        {
            var input = Mapper.Map<Pbck7AndPbck3Input>(model);
            input.Pbck7AndPvck3Type = Enums.Pbck7Type.Pbck7List;
            if (input.Pbck7Date != null)
            {
                input.Pbck7Date = Convert.ToDateTime(input.Pbck7Date).ToString();
            }



            var dbData = _pbck7Pbck3Bll.GetPbck7ByParam(input, CurrentUser);

            var result = Mapper.Map<List<DataListIndexPbck7>>(dbData);

            var viewModel = new Pbck7IndexViewModel();

            viewModel.Detail = result;

            return PartialView("_Pbck7TableIndex", viewModel);
        }

        #region PBCK3

        public ActionResult ListPbck3Index()
        {
            var detail =
                Mapper.Map<List<DataListIndexPbck3>>(_pbck7Pbck3Bll.GetPbck3ByParam(new Pbck7AndPbck3Input(), CurrentUser));

            var data = InitPbck3ViewModel(new Pbck3IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck3Type = Enums.Pbck7Type.Pbck3List,

                Detail = detail,
                IsCompletedDoc = false
            });

            return View("ListPbck3Index", data);
        }
        public ActionResult Pbck3Completed()
        {
            var detail =
                Mapper.Map<List<DataListIndexPbck3>>(_pbck7Pbck3Bll.GetPbck3ByParam(new Pbck7AndPbck3Input(), CurrentUser, true));

            var data = InitPbck3ViewModel(new Pbck3IndexViewModel
            {
                MainMenu = _mainMenu,
                CurrentMenu = PageInfo,
                Pbck3Type = Enums.Pbck7Type.Pbck3List,

                Detail = detail,
                IsCompletedDoc = true
            });

            return View("ListPbck3Index", data);
        }
        private Pbck3IndexViewModel InitPbck3ViewModel(Pbck3IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.CreatorList = GlobalFunctions.GetCreatorList();

            return (model);
        }

        [HttpPost]
        public PartialViewResult FilterPbck3Index(Pbck3IndexViewModel model)
        {
            var input = Mapper.Map<Pbck7AndPbck3Input>(model);
            input.Pbck7AndPvck3Type = Enums.Pbck7Type.Pbck3List;
            if (input.Pbck3Date != null)
            {
                input.Pbck3Date = Convert.ToDateTime(input.Pbck3Date).ToString();
            }


            var dbData = _pbck7Pbck3Bll.GetPbck3ByParam(input, CurrentUser);
            var result = Mapper.Map<List<DataListIndexPbck3>>(dbData);

            var viewModel = new Pbck3IndexViewModel();

            viewModel.Detail = result;

            return PartialView("_Pbck3TableIndex", viewModel);

        }

        #endregion

        #region Json

        [HttpPost]
        public JsonResult GetPlantByNppbkcId(string nppbkcid)
        {
            var data = Json(GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, nppbkcid));
            return data;

        }
        [HttpPost]
        public JsonResult GetPoaByNppbkcId(string nppbkcid)
        {
            var data = _poaBll.GetPoaByNppbkcId(nppbkcid);
            return Json(data.Distinct());

        }

        [HttpPost]
        public JsonResult PoaAndPlantListPartialPbck7(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() { PoaList = listPoa, PlantList = listPlant };

            return Json(model);
        }

        [HttpPost]
        public JsonResult PoaAndPlantListPartialPbck3(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() { PoaList = listPoa, PlantList = listPlant };

            return Json(model);
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Manager)
            {
                AddMessageInfo("Can't create PBCK-7 Document for User with " + EnumHelper.GetDescription(Enums.UserRole.Manager) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }

            var model = new Pbck7Pbck3CreateViewModel();

            model = InitListPbck7(model);

            return View("Create", model);
        }


        private Pbck7Pbck3CreateViewModel InitListPbck7(Pbck7Pbck3CreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.NppbkIdList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GetPoaList(model.NppbkcId);
            model.Pbck7Date = DateTime.Now;

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pbck7Pbck3CreateViewModel model)
        {
            try
            {
                //todo ask back-1 qty value
                //if (ModelState.IsValid)
                //{
                if (model.UploadItems.Count > 0)
                {
                    var saveResult = SavePbck7ToDatabase(model);

                    AddMessageInfo("Success create PBCK-7", Enums.MessageInfoType.Success);

                    return RedirectToAction("Edit", new { @id = saveResult.Pbck7Id });
                }

                AddMessageInfo("Missing PBCK-7 Items", Enums.MessageInfoType.Error);
                //}
                //else
                //    AddMessageInfo("Not Valid Model", Enums.MessageInfoType.Error);

                model = InitListPbck7(model);

                return View("Create", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitListPbck7(model);

                return View("Create", model);
            }

            //var modelDto = Mapper.Map<Pbck7AndPbck3Dto>(model);
            //modelDto.CreatedBy = CurrentUser.USER_ID;
            //modelDto.CreateDate = DateTime.Now;
            //modelDto.Pbck7Status = Enums.DocumentStatus.Draft;
            //var plant = _plantBll.GetId(model.PlantId);
            //modelDto.PlantName = plant.NAME1;
            //modelDto.PlantCity = plant.ORT01;
            //var inputDoc = new GenerateDocNumberInput();
            //inputDoc.Month = modelDto.Pbck7Date.Month;
            //inputDoc.Year = modelDto.Pbck7Date.Year;
            //inputDoc.NppbkcId = modelDto.NppbkcId;
            //modelDto.Pbck7Number = _documentSequenceNumberBll.GenerateNumberNoReset(inputDoc);

            //int? pbck7IdAfterSave= null;
            //try
            //{
            //    pbck7IdAfterSave = _pbck7Pbck3Bll.InsertPbck7(modelDto);
            //}
            //catch (Exception ex)
            //{
            //   AddMessageInfo(ex.ToString(), Enums.MessageInfoType.Error);
            //}
            //AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
            //return RedirectToAction("Edit", new { id = pbck7IdAfterSave});
        }


        #endregion

        //public void GetDetailPbck7(Pbck7AndPbck3Dto existingData)
        //{
        //    existingData.Back1Dto = _pbck7Pbck3Bll.GetBack1ByPbck7(existingData.Pbck7Id);
        //    existingData.Pbck3Dto = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(existingData.Pbck7Id);

        //    if (existingData.Pbck3Dto != null)
        //    {
        //        existingData.Back3Dto = _pbck7Pbck3Bll.GetBack3ByPbck3Id(existingData.Pbck3Dto.Pbck3Id);
        //        existingData.Ck2Dto = _pbck7Pbck3Bll.GetCk2ByPbck3Id(existingData.Pbck3Dto.Pbck3Id);
        //    }
        //    if (existingData.Back1Dto == null)
        //        existingData.Back1Dto = new Back1Dto();
        //    if (existingData.Pbck3Dto == null)
        //        existingData.Pbck3Dto = new Pbck3Dto();
        //    if (existingData.Back3Dto == null)
        //        existingData.Back3Dto = new Back3Dto();
        //    if (existingData.Ck2Dto == null)
        //        existingData.Ck2Dto = new Ck2Dto();
        //}

        public ActionResult Edit(int id)
        {
            var model = new Pbck7Pbck3CreateViewModel();

            try
            {
                var existingData = _pbck7Pbck3Bll.GetDetailsPbck7ById(id);

                model = Mapper.Map<Pbck7Pbck3CreateViewModel>(existingData.Pbck7Dto);

                var input = new WorkflowAllowEditAndSubmitInput();
                input.DocumentStatus = existingData.Pbck7Dto.Pbck7Status;
                input.CreatedUser = existingData.Pbck7Dto.CreatedBy;
                input.CurrentUser = CurrentUser.USER_ID;
                if (!_workflowBll.AllowEditDocument(input))
                    return RedirectToAction("Detail", new { @id = existingData.Pbck7Dto.Pbck7Id });

                model = InitialModel(model);

                model.Back1Dto = existingData.Back1Dto;
                model.Pbck3Dto = existingData.Pbck3Dto;
                model.Back3Dto = existingData.Back3Dto;
                model.Ck2Dto = existingData.Ck2Dto;

                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(existingData.ListChangesHistorys);
                model.WorkflowHistoryPbck7 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck7);
                model.WorkflowHistoryPbck3 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck3);


                return View("Edit", InitialModel(model));
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = InitialModel(model);
            }

            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = new Pbck7Pbck3CreateViewModel();

            try
            {
                var existingData = _pbck7Pbck3Bll.GetDetailsPbck7ById(id);

                model = Mapper.Map<Pbck7Pbck3CreateViewModel>(existingData.Pbck7Dto);
                model.Back1Dto = existingData.Back1Dto;
                model.Pbck3Dto = existingData.Pbck3Dto;
                model.Back3Dto = existingData.Back3Dto;
                model.Ck2Dto = existingData.Ck2Dto;

                model.WorkflowHistoryPbck7 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck7);
                //model.WorkflowHistoryPbck3 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck3);


                model = InitialModel(model);


                model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(existingData.ListPrintHistorys);

                var changesHistoryPbck = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK7, id.ToString());
                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changesHistoryPbck);

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput();
                input.DocumentStatus = model.Pbck7Status;
                input.FormView = Enums.FormViewType.Detail;
                input.UserRole = CurrentUser.UserRole;
                input.CreatedUser = existingData.Pbck7Dto.CreatedBy;
                input.CurrentUser = CurrentUser.USER_ID;
                input.CurrentUserGroup = CurrentUser.USER_GROUP_ID;
                input.DocumentNumber = model.Pbck7Number;
                input.NppbkcId = model.NppbkcId;

                //workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;


                if (!allowApproveAndReject)
                {
                    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                    model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
                }

                model.AllowPrintDocument = _workflowBll.AllowPrint(model.Pbck7Status);

                if (model.AllowGovApproveAndReject)
                    model.ActionType = "GovApproveDocument";
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                return RedirectToAction("Index");
            }


            return View("Detail", model);
        }

        //public void SaveBack3(Pbck7Pbck3CreateViewModel model)
        //{
        //    var existingData = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(model.Id);
        //    if (existingData != null)
        //    {

        //        var back3Dto = new Back3Dto();
        //        if (model.DocumentsPostBack3 != null)
        //        {
        //            back3Dto.Back3Document = new List<BACK3_DOCUMENT>();
        //            foreach (var sk in model.DocumentsPostBack3)
        //            {
        //                if (sk != null)
        //                {
        //                    var document = new BACK3_DOCUMENT();
        //                    var filenamecheck = sk.FileName;
        //                    if (filenamecheck.Contains("\\"))
        //                    {
        //                        document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
        //                    }
        //                    else
        //                    {
        //                        document.FILE_NAME = sk.FileName;
        //                    }

        //                    document.FILE_PATH = SaveUploadedFile(sk, model.Back3Dto.Back3Number.Trim().Replace('/', '_'));
        //                    back3Dto.Back3Document.Add(document);

        //                }
        //            }
        //        }

        //        back3Dto.Back3Number = model.Back3Dto.Back3Number;
        //        back3Dto.Back3Date = model.Back3Dto.Back3Date;
        //        back3Dto.Pbck3ID = existingData.Pbck3Id;

        //        _pbck7Pbck3Bll.InsertBack3(back3Dto);
        //        var ck2Dto = SaveCk2(model, existingData.Pbck3Id);
        //        if (existingData.Pbck3Status == Enums.DocumentStatus.GovApproved)
        //        {
        //            existingData.Pbck3Status = Enums.DocumentStatus.Completed;
        //           _pbck7Pbck3Bll.InsertPbck3(existingData);
        //           CreateXml(ck2Dto, model.NppbkcId, existingData.Pbck3Number);


        //        }
        //    }

        //}

        public void CreateXml(Ck2Dto ck2, string nppbckId, string pbck3Number)
        {
            var pbck4xmlDto = new Pbck4XmlDto();
            pbck4xmlDto.NppbckId = nppbckId;
            pbck4xmlDto.CompType = "CK-2";
            pbck4xmlDto.PbckNo = pbck3Number;
            pbck4xmlDto.CompnDate = ck2.Ck2Date;
            pbck4xmlDto.CompnValue = ck2.Ck2Value.HasValue ? ck2.Ck2Value.ToString() : null;
            pbck4xmlDto.CompNo = ck2.Ck2Number;
            var fileName = ConfigurationManager.AppSettings["CK5PathXml"] + "COMPENSATION-CK2-" +
                               DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml";

            pbck4xmlDto.GeneratedXmlPath = fileName;

            var xmlwriter = new XmlPBCK4DataWriter();
            xmlwriter.CreatePbck4Xml(pbck4xmlDto);
        }

        //public Ck2Dto SaveCk2(Pbck7Pbck3CreateViewModel model, int pbck3Id)
        //{


        //        var ck2Dto = new Ck2Dto();
        //        if (model.DocumentsPostCk2 != null)
        //        {
        //            ck2Dto.Ck2Document = new List<CK2_DOCUMENT>();
        //            foreach (var sk in model.DocumentsPostCk2)
        //            {
        //                if (sk != null)
        //                {
        //                    var document = new CK2_DOCUMENT();
        //                    var filenamecheck = sk.FileName;
        //                    if(filenamecheck.Contains("\\"))
        //                    {
        //                        document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
        //                    }
        //                    else
        //                    {
        //                        document.FILE_NAME = sk.FileName;
        //                    }

        //                    document.FILE_PATH = SaveUploadedFile(sk, model.Back3Dto.Back3Number.Trim().Replace('/', '_'));
        //                    ck2Dto.Ck2Document.Add(document);

        //                }
        //            }
        //        }

        //        ck2Dto.Ck2Number = model.Ck2Dto.Ck2Number;
        //        ck2Dto.Ck2Date = model.Ck2Dto.Ck2Date;
        //        ck2Dto.Pbck3ID = pbck3Id;
        //        _pbck7Pbck3Bll.InsertCk2(ck2Dto);

        //    return ck2Dto;

        //}

        //private void SubmitPbck3(Pbck7Pbck3CreateViewModel model)
        //{
        //    if (model.Pbck3Dto.Pbck3Status != Enums.DocumentStatus.Completed)
        //    {
        //        if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Draft)
        //        {
        //            if (CurrentUser.UserRole == Enums.UserRole.POA)
        //            {
        //                model.Pbck3Dto.Pbck3Status = Enums.DocumentStatus.WaitingForApprovalManager;
        //            }
        //            else if (CurrentUser.UserRole == Enums.UserRole.User)
        //            {
        //                model.Pbck3Dto.Pbck3Status = Enums.DocumentStatus.WaitingForApproval;
        //            }

        //        }

        //    }
        //}

        //public void SavePbck3(Pbck7Pbck3CreateViewModel model)
        //{
        //    var existingData = _pbck7Pbck3Bll.GetDetailsPbck7ById(model.Id);
        //    //GetDetailPbck7(existingData);
        //    //var model = Mapper.Map<Pbck7Pbck3CreateViewModel>(existingData.Pbck7Dto);
        //    //model.Back1Dto = existingData.Back1Dto;
        //    //model.Pbck3Dto = existingData.Pbck3Dto;
        //    //model.Back3Dto = existingData.Back3Dto;
        //    //model.Ck2Dto = existingData.Ck2Dto;

        //    //model.WorkflowHistoryPbck7 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck7);
        //    //model.WorkflowHistoryPbck3 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck3);


        //    if (existingData != null)
        //    {
        //       var pbck3 = new Pbck3Dto();

        //        if (existingData.Pbck3Dto != null && existingData.Pbck3Dto.Pbck3Id != 0)
        //        {
        //            pbck3 = existingData.Pbck3Dto;
        //            pbck3.Pbck3Date = model.Pbck3Dto.Pbck3Date;

        //            //if submit
        //            if (model.IsSaveSubmitPbck3)
        //            {

        //                SubmitPbck3(model);
        //            }
        //            else
        //            {
        //                //if edit then save
        //                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Rejected)
        //                {
        //                    model.Pbck3Dto.Pbck3Status = Enums.DocumentStatus.Draft;
        //                }
        //            }

        //            if (model.Pbck3Dto.Pbck3GovStatus != null)
        //            {
        //                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.WaitingGovApproval)
        //                {
        //                    pbck3.Pbck3Status = Enums.DocumentStatus.GovApproved;
        //                }
        //            }




        //        }
        //        else
        //        {

        //                //if new
        //                pbck3.Pbck3Status = Enums.DocumentStatus.Draft;

        //                var inputDoc = new GenerateDocNumberInput();
        //                inputDoc.Month = model.Pbck3Dto.Pbck3Date.Month;
        //                inputDoc.Year = model.Pbck3Dto.Pbck3Date.Year;
        //                inputDoc.NppbkcId = existingData.Pbck7Dto.NppbkcId;
        //                pbck3.CreateDate = DateTime.Now;
        //                pbck3.CreatedBy = CurrentUser.USER_ID;
        //                pbck3.Pbck7Id = existingData.Pbck7Dto.Pbck7Id;
        //                pbck3.Pbck3Date = model.Pbck3Dto.Pbck3Date;
        //                pbck3.Pbck3Number = _documentSequenceNumberBll.GenerateNumber(inputDoc);




        //        }
        //        _pbck7Pbck3Bll.InsertPbck3(pbck3);


        //    }
        //}

        //public void SaveBack1(Pbck7Pbck3CreateViewModel model)
        //{
        //    var existingData = _pbck7Pbck3Bll.GetPbck7ById(model.Id);
        //    if (existingData != null)
        //    {

        //        var back1Dto = new Back1Dto();
        //        if (model.DocumentsPostBack != null)
        //        {
        //            back1Dto.Documents = new List<BACK1_DOCUMENT>();
        //            foreach (var sk in model.DocumentsPostBack)
        //            {
        //                if (sk != null)
        //                {
        //                    var document = new BACK1_DOCUMENT();
        //                    var filenamecheck = sk.FileName;
        //                    if (filenamecheck.Contains("\\"))
        //                    {
        //                        document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
        //                    }
        //                    else
        //                    {
        //                        document.FILE_NAME = sk.FileName;
        //                    }

        //                    document.FILE_PATH = SaveUploadedFile(sk, model.Back1Dto.Back1Number.Trim().Replace('/', '_'));
        //                    back1Dto.Documents.Add(document);

        //                }
        //            }
        //        }

        //        back1Dto.Back1Number = model.Back1Dto.Back1Number;
        //        back1Dto.Back1Date = model.Back1Dto.Back1Date;
        //        back1Dto.Pbck7Id = existingData.Pbck7Id;
        //        var uploadItem = model.UploadItems;
        //        foreach (var pbck7ItemUpload in uploadItem)
        //        {
        //            //_pbck7Pbck3Bll.InsertPbck7Item(pbck7ItemUpload);

        //        }

        //        _pbck7Pbck3Bll.InsertBack1(back1Dto);
        //        if (existingData.Pbck7Status == Enums.DocumentStatus.GovApproved)
        //        {
        //            existingData.Pbck7Status = Enums.DocumentStatus.Completed;
        //            //prevent error when update pbck7
        //            existingData.UploadItems = null;
        //            _pbck7Pbck3Bll.InsertPbck7(existingData);
        //        }
        //    }

        //}

        private void PBCK7Workflow(int id, Enums.ActionType actionType, string comment, bool isModified = false)
        {
            var input = new Pbck7Pbck3WorkflowDocumentInput();
            input.DocumentId = id;
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ActionType = actionType;
            input.Comment = comment;
            input.FormType = Enums.FormType.PBCK7;
            input.IsModified = isModified;
            _pbck7Pbck3Bll.PBCK7Workflow(input);
        }

        private Pbck7Pbck3CreateViewModel GetHistorys(Pbck7Pbck3CreateViewModel model)
        {
            model.WorkflowHistoryPbck7 =
                Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.Pbck7Number));
            //model.WorkflowHistoryPbck3 =
            //    Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.Pbck3StatusName));

            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK7, model.Id.ToString()));

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pbck7Pbck3CreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.UploadItems.Count > 0)
                    {
                        //validate
                        var input = new WorkflowAllowEditAndSubmitInput();
                        input.DocumentStatus = model.Pbck7Status;
                        input.CreatedUser = model.CreatedBy;
                        input.CurrentUser = CurrentUser.USER_ID;
                        if (_workflowBll.AllowEditDocument(input))
                        {
                            var resultDto = SavePbck7ToDatabase(model);
                            if (model.IsSaveSubmit)
                            {
                                PBCK7Workflow(model.Id, Enums.ActionType.Submit, string.Empty, resultDto.IsModifiedHistory);
                                AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                                return RedirectToAction("Detail", new { @id = model.Id });

                            }
                            AddMessageInfo("Success", Enums.MessageInfoType.Success);
                            return RedirectToAction("Edit", new { @id = model.Id });
                        }
                        else
                        {
                            AddMessageInfo("Not allow to Edit Document", Enums.MessageInfoType.Error);
                            return RedirectToAction("Detail", new { @id = model.Id });
                        }

                    }
                    else
                        AddMessageInfo("Missing PBCK-7 Items", Enums.MessageInfoType.Error);
                }
                else
                    AddMessageInfo("Not Valid Model", Enums.MessageInfoType.Error);

                model = InitialModel(model);
                model = GetHistorys(model);

                return View(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                model = InitialModel(model);
                model = GetHistorys(model);

                return View(model);
            }



        }


        private Pbck7AndPbck3Dto SavePbck7ToDatabase(Pbck7Pbck3CreateViewModel model)
        {

            var dataToSave = Mapper.Map<Pbck7AndPbck3Dto>(model);

            var input = new Pbck7Pbck3SaveInput()
            {
                Pbck7Pbck3Dto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                Pbck7Pbck3Items = Mapper.Map<List<Pbck7ItemUpload>>(model.UploadItems)
            };

            return _pbck7Pbck3Bll.SavePbck7(input);
        }

        public string GetPoaList(string nppbkcid)
        {
            var poaList = _poaBll.GetPoaByNppbkcId(nppbkcid).Distinct().ToList();
            var poaListStr = string.Empty;
            var poaLength = poaList.Count;

            for (int i = 0; i < poaLength; i++)
            {
                poaListStr += poaList[i].PRINTED_NAME;
                if (i < poaLength)
                {
                    poaListStr += ", ";
                }
            }
            return poaListStr;

        }

        private Pbck7Pbck3CreateViewModel InitialModel(Pbck7Pbck3CreateViewModel model)
        {
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.NppbkIdList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GetPoaList(model.NppbkcId);

            return (model);
        }




        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile, string plantId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);

            var model = new Pbck7Pbck3CreateViewModel();

            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var item = new Pbck7UploadViewModel();

                    try
                    {
                        item.FaCode = datarow[0];
                        item.Pbck7Qty = datarow[1];// Convert.ToDecimal(datarow[1]);
                        item.FiscalYear = datarow[2];// Convert.ToInt32(datarow[2]);

                        item.PlantId = plantId;

                        model.UploadItems.Add(item);

                        //var existingBrand = _brandRegistration.GetByIdIncludeChild(plantId, item.FaCode);
                        //if (existingBrand != null)
                        //{
                        //    item.Brand = existingBrand.BRAND_CE;
                        //    item.SeriesValue = existingBrand.ZAIDM_EX_SERIES.SERIES_CODE;
                        //    item.ProdTypeAlias = existingBrand.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS;
                        //    item.Content = Convert.ToInt32(existingBrand.BRAND_CONTENT);
                        //    item.Hje = existingBrand.HJE_IDR;
                        //    item.Tariff = existingBrand.TARIFF;
                        //    item.ExciseValue = item.Content*item.Tariff*item.Pbck7Qty;
                        //    item.Message = "";
                        //    model.Add(item);
                        //}
                        //else
                        //{
                        //    return Json(-1);
                        //}

                    }
                    catch (Exception)
                    {

                    }


                }
            }

            var input = Mapper.Map<List<Pbck7ItemsInput>>(model.UploadItems);

            var outputResult = _pbck7Pbck3Bll.Pbck7ItemProcess(input);

            //model.UploadItemModels = Mapper.Map<List<Pbck4UploadViewModel>>(outputResult);

            //return PartialView("_Pbck4UploadList", model.UploadItemModels);

            return Json(outputResult);
        }

        private string SaveUploadedFile(HttpPostedFileBase file, string back1Num)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";


            sFileName = Constans.UploadPath + Path.GetFileName("BACK1_" + back1Num + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }


        public ActionResult ApproveDocument(int id)
        {
            try
            {
                PBCK7Workflow(id, Enums.ActionType.Approve, string.Empty);
                AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        public ActionResult GovApproveDocument(Pbck7Pbck3CreateViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    AddMessageInfo("Model Not Valid", Enums.MessageInfoType.Success);
            //    return RedirectToAction("Detail", new { id = model.Id });
            //}

            try
            {
                var currentUserId = CurrentUser.USER_ID;

                model.Back1Dto.Documents = new List<BACK1_DOCUMENT>();
                if (model.DocumentsPostBack != null)
                {
                    foreach (var item in model.DocumentsPostBack)
                    {
                        if (item != null)
                        {
                            var filenameCk5Check = item.FileName;
                            if (filenameCk5Check.Contains("\\"))
                                filenameCk5Check = filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];

                            var pbck4UploadFile = new BACK1_DOCUMENT
                            {
                                FILE_NAME = filenameCk5Check,
                                FILE_PATH = SaveUploadedFile(item, model.Pbck7Number),

                            };
                            model.Back1Dto.Documents.Add(pbck4UploadFile);
                        }

                    }
                }
                else
                {
                    AddMessageInfo("Empty File BACK-1 Doc", Enums.MessageInfoType.Error);
                    RedirectToAction("Details", new { id = model.Id });
                }


                PBCK7GovWorkflow(model);
                if (model.Pbck7GovStatus == Enums.DocumentStatusGov.FullApproved)
                    AddMessageInfo("Success Gov FullApproved Document", Enums.MessageInfoType.Success);
                else if (model.Pbck7GovStatus == Enums.DocumentStatusGov.PartialApproved)
                    AddMessageInfo("Success Gov PartialApproved Document", Enums.MessageInfoType.Success);
                else if (model.Pbck7GovStatus == Enums.DocumentStatusGov.Rejected)
                    AddMessageInfo("Success Gov Reject Document", Enums.MessageInfoType.Success);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Detail", new { id = model.Id });
        }

        private bool PBCK7GovWorkflow(Pbck7Pbck3CreateViewModel model)
        {
            try
            {
                var actionType = Enums.ActionType.GovApprove;

                if (model.Pbck7GovStatus == Enums.DocumentStatusGov.PartialApproved)
                    actionType = Enums.ActionType.GovPartialApprove;
                else if (model.Pbck7GovStatus == Enums.DocumentStatusGov.Rejected)
                    actionType = Enums.ActionType.GovReject;

                var input = new Pbck7Pbck3WorkflowDocumentInput();
                input.DocumentId = model.Id;
                input.DocumentNumber = model.Pbck7Number;
                input.UserId = CurrentUser.USER_ID;
                input.UserRole = CurrentUser.UserRole;
                input.ActionType = actionType;
                input.Comment = model.Comment;
                input.FormType = Enums.FormType.PBCK7;
                input.StatusGovInput = model.Pbck7GovStatus;


                //input.UploadItemDto = new List<Pbck7ItemUpload>();
                //foreach (var pbck7UploadItem in model.UploadItems)
                //{
                //    if (pbck7UploadItem.IsUpdated)
                //        input.UploadItemDto.Add(Mapper.Map<Pbck4ItemDto>(pbck4UploadItem));

                //}


                input.AdditionalDocumentData = new Pbck7WorkflowDocumentData();
                input.AdditionalDocumentData.Back1No = model.Back1Dto.Back1Number;
                input.AdditionalDocumentData.Back1Date = model.Back1Dto.Back1Date;

                input.AdditionalDocumentData.Back1FileUploadList = model.Back1Dto.Documents;

                _pbck7Pbck3Bll.PBCK7Workflow(input);

                return true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return false;
            }



        }


        public ActionResult Pbck7SummaryReport()
        {

            Pbck7SummaryReportModel model;
            try
            {

                model = new Pbck7SummaryReportModel();


                InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck7SummaryReportModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("Pbck7SummaryReport", model);
        }

        public ActionResult Pbck3SummaryReport()
        {

            Pbck3SummaryReportModel model;
            try
            {

                model = new Pbck3SummaryReportModel();


                InitSummaryReportsPbck3(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Pbck3SummaryReportModel();
                model.MainMenu = Enums.MenuList.CK5;
                model.CurrentMenu = PageInfo;
            }

            return View("Pbck3SummaryReport", model);
        }

        private void InitSummaryReports(Pbck7SummaryReportModel model)
        {
            model.MainMenu = Enums.MenuList.PBCK7;
            model.CurrentMenu = PageInfo;

            model.PlantList = GlobalFunctions.GetPlantAll();
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.Pbck7List = GetAllPbck7No();
            model.FromYear = GlobalFunctions.GetYearList();
            model.ToYear = model.FromYear;
            model.ReportItems = _pbck7Pbck3Bll.GetPbck7SummaryReportsByParam(new Pbck7SummaryInput());
        }

        private void InitSummaryReportsPbck3(Pbck3SummaryReportModel model)
        {
            model.MainMenu = Enums.MenuList.PBCK3;
            model.CurrentMenu = PageInfo;

            model.PlantList = GlobalFunctions.GetPlantAll();
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.Pbck3List = GetAllPbck3No();
            model.FromYear = GlobalFunctions.GetYearList();
            model.ToYear = model.FromYear;
            model.ReportItems = _pbck7Pbck3Bll.GetPbck3SummaryReportsByParam(new Pbck3SummaryInput());
        }

        private SelectList GetAllPbck7No()
        {
            var pbck7List = _pbck7Pbck3Bll.GetAllPbck7();
            return new SelectList(pbck7List, "Pbck7Number", "Pbck7Number");

        }

        private SelectList GetAllPbck3No()
        {
            var pbck3List = _pbck7Pbck3Bll.GetAllPbck3();
            return new SelectList(pbck3List, "Pbck3Number", "Pbck3Number");

        }

        [HttpPost]
        public PartialViewResult FilterPbck7SummaryReport(Pbck7SummaryReportModel model)
        {
            var input = Mapper.Map<Pbck7SummaryInput>(model);
            var result = _pbck7Pbck3Bll.GetPbck7SummaryReportsByParam(input);
            return PartialView("_Pbck7SummaryIndex", result);
        }

        [HttpPost]
        public PartialViewResult FilterPbck3SummaryReport(Pbck3SummaryReportModel model)
        {
            var input = Mapper.Map<Pbck3SummaryInput>(model);
            var result = _pbck7Pbck3Bll.GetPbck3SummaryReportsByParam(input);
            return PartialView("_Pbck3SummaryIndex", result);
        }


        [HttpPost]
        public ActionResult Pbck7ExportSummaryReports(Pbck7SummaryReportModel model)
        {
            try
            {
                ExportSummaryReportsToExcel(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Pbck7SummaryReport");
        }

        [HttpPost]
        public ActionResult Pbck3ExportSummaryReports(Pbck3SummaryReportModel model)
        {
            try
            {
                ExportSummaryReportsToExcelPbck3(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Pbck3SummaryReport");
        }

        public void ExportSummaryReportsToExcel(Pbck7SummaryReportModel model)
        {

            var input = Mapper.Map<Pbck7SummaryInput>(model);
            var result = _pbck7Pbck3Bll.GetPbck7SummaryReportsByParam(input);
            var src = (from b in result
                       select new Pbck7SummaryReportItem()
                       {

                           Pbck7Number = b.Pbck7Number,
                           Nppbkc = b.NppbkcId,
                           PlantName = b.PlantId + "-" + b.PlantName,
                           Pbck7Date = b.Pbck7Date,
                           Pbck7Status = EnumHelper.GetDescription(b.Pbck7Status),
                           ExecFrom = b.ExecDateFrom,
                           ExecTo = b.ExecDateTo,
                           Back1No = b.Back1Dto != null ? b.Back1Dto.Back1Number : string.Empty,
                           Back1Date = b.Back1Dto != null ? b.Back1Dto.Back1Date : null


                       }).ToList();
            var grid = new GridView
            {
                DataSource = src.OrderBy(c => c.Pbck7Number).ToList(),
                AutoGenerateColumns = false
            };
            if (model.ExportModel.IsSelectPbck7No)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck7Number",
                    HeaderText = "Number"
                });
            }
            if (model.ExportModel.IsSelectNppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Nppbkc",
                    HeaderText = "Nppbkc"
                });
            }
            if (model.ExportModel.IsSelectPlant)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PlantName",
                    HeaderText = "Plant"
                });
            }
            if (model.ExportModel.IsSelectDate)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck7Date",
                    HeaderText = "Date"
                });
            }
            if (model.ExportModel.IsSelectExecFrom)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ExecFrom",
                    HeaderText = "Exec Date From"
                });
            }
            if (model.ExportModel.IsSelectExecFrom)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "ExecTo",
                    HeaderText = "Exec To From"
                });
            }
            if (model.ExportModel.IsSelectBack1No)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Back1No",
                    HeaderText = "Back-1 No"
                });
            }
            if (model.ExportModel.IsSelectBack1Date)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Back1Date",
                    HeaderText = "Back-1 Date"
                });
            }
            if (model.ExportModel.IsSelectStatus)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck7Status",
                    HeaderText = "Status"
                });
            }
            if (src.Count == 0)
            {
                grid.ShowHeaderWhenEmpty = true;
            }

            grid.DataBind();

            var fileName = "PBCK7" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        public void ExportSummaryReportsToExcelPbck3(Pbck3SummaryReportModel model)
        {

            var input = Mapper.Map<Pbck3SummaryInput>(model);
            var result = _pbck7Pbck3Bll.GetPbck3SummaryReportsByParam(input);
            var src = (from b in result
                       select new Pbck3SummaryReportItem()
                       {
                           Pbck3Number = b.Pbck3Number,
                           Pbck7Number = b.Pbck7Number,
                           Nppbkc = b.NppbckId,
                           PlantName = b.Plant,
                           Pbck3Date = b.Pbck3Date,
                           Pbck3Status = EnumHelper.GetDescription(b.Pbck3Status),
                           Back3No = b.Back3Dto != null ? b.Back3Dto.Back3Number : string.Empty,
                           Back3Date = b.Back3Dto != null ? b.Back3Dto.Back3Date : null,
                           Ck2No = b.Ck2Dto != null ? b.Ck2Dto.Ck2Number : string.Empty,
                           Ck2Date = b.Ck2Dto != null ? b.Ck2Dto.Ck2Date : null,
                           Ck2Value = b.Ck2Dto != null ? b.Ck2Dto.Ck2Value : 0


                       }).ToList();
            var grid = new GridView
            {
                DataSource = src.OrderBy(c => c.Pbck7Number).ToList(),
                AutoGenerateColumns = false
            };
            if (model.ExportModel.IsSelectPbck3No)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck3Number",
                    HeaderText = "Number"
                });
            }
            if (model.ExportModel.IsSelectPbck7)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck7Number",
                    HeaderText = "PBCK-7"
                });
            }
            if (model.ExportModel.IsSelectNppbkc)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Nppbkc",
                    HeaderText = "Nppbkc"
                });
            }
            if (model.ExportModel.IsSelectPlant)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "PlantName",
                    HeaderText = "Plant"
                });
            }
            if (model.ExportModel.IsSelectDate)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck3Date",
                    HeaderText = "Date"
                });
            }

            if (model.ExportModel.IsSelectStatus)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Pbck3Status",
                    HeaderText = "Status"
                });
            }

            if (model.ExportModel.IsSelectBack3No)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Back3No",
                    HeaderText = "BACK-3 No"
                });
            }
            if (model.ExportModel.IsSelectBack3Date)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Back3Date",
                    HeaderText = "BACK-3 Date"
                });
            }
            if (model.ExportModel.IsSelectCk2No)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Ck2No",
                    HeaderText = "CK-2 No"
                });
            }
            if (model.ExportModel.IsSelectCk2Date)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Ck2Date",
                    HeaderText = "CK-2 Date"
                });
            }
            if (model.ExportModel.IsSelectCk2Value)
            {
                grid.Columns.Add(new BoundField()
                {
                    DataField = "Ck2Value",
                    HeaderText = "CK-2 Value"
                });
            }
            if (src.Count == 0)
            {
                grid.ShowHeaderWhenEmpty = true;
            }

            grid.DataBind();

            var fileName = "PBCK3" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        [HttpPost]
        public ActionResult RejectDocumentPbck7(Pbck7Pbck3CreateViewModel model)
        {
            //bool isSuccess = false;
            //try
            //{
            //    var item = _pbck7Pbck3Bll.GetPbck7ById(model.Id);
            //    item.Pbck7Status = Enums.DocumentStatus.Rejected;
            //    item.IsRejected = true;
            //    item.Comment = model.Comment;
            //    item.RejectedBy = CurrentUser.USER_ID;
            //    item.RejectedDate = DateTime.Now;
            //    item.UploadItems = null;
            //    _pbck7Pbck3Bll.InsertPbck7(item);
            //    isSuccess = true;
            //}
            //catch (Exception ex)
            //{
            //    AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            //}

            //if (!isSuccess) return RedirectToAction("Detail", "Pbck7AndPbck3", new { id = model.Id });
            //AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            //return RedirectToAction("Index");
            try
            {
                PBCK7Workflow(model.Id, Enums.ActionType.Reject, model.Comment);
                AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("Detail", new { id = model.Id });

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
            var listHistory = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK7, pbckId.ToString());

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

            var fileName = "PBCK7" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Content/upload/"), fileName);

            //var outpu = new 
            slDocument.SaveAs(path);

            return path;
        }


        public ActionResult EditPbck3(int id)
        {
            var model = new Pbck3ViewModel();

            try
            {
                var existingData = _pbck7Pbck3Bll.GetPbck3DetailsById(id);

                model = Mapper.Map<Pbck3ViewModel>(existingData.Pbck3CompositeDto);

                var input = new WorkflowAllowEditAndSubmitInput();
                input.DocumentStatus = model.Pbck3Status;
                input.CreatedUser = existingData.Pbck3CompositeDto.CREATED_BY;
                input.CurrentUser = CurrentUser.USER_ID;
                if (!_workflowBll.AllowEditDocument(input))
                    return RedirectToAction("DetailPbck3", new { @id = existingData.Pbck3CompositeDto.PBCK3_ID });


                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(existingData.ListChangesHistorys);
                
                model.WorkflowHistoryPbck3 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck3);

                if (existingData.Pbck3CompositeDto.FromPbck7)
                    model.WorkflowHistoryPbck7 =
                        Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck7);
                else
                {
                    model.Ck5FormViewModel =
                        Mapper.Map<CK5FormViewModel>(existingData.Pbck3CompositeDto.Ck5Composite.Ck5Dto);

                    model.Ck5FormViewModel.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(existingData.Pbck3CompositeDto.Ck5Composite.Ck5MaterialDto);

                    model.Ck5FormViewModel.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.Pbck3CompositeDto.Ck5Composite.ListWorkflowHistorys);

                }
                return View("EditPbck3", model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPbck3(Pbck3ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //validate
                    var input = new WorkflowAllowEditAndSubmitInput();
                    input.DocumentStatus = model.Pbck3Status;
                    input.CreatedUser = model.CREATED_BY;
                    input.CurrentUser = CurrentUser.USER_ID;
                    if (_workflowBll.AllowEditDocument(input))
                    {
                        SavePbck3ToDatabase(model);
                        if (model.IsSaveSubmit)
                        {
                            PBCK3Workflow(model.Pbck3Id, Enums.ActionType.Submit, string.Empty);
                            AddMessageInfo("Success Submit Document", Enums.MessageInfoType.Success);
                            return RedirectToAction("DetailPbck3", new { @id = model.Pbck3Id });

                        }
                        AddMessageInfo("Success", Enums.MessageInfoType.Success);
                        return RedirectToAction("EditPbck3", new { @id = model.Pbck3Id });
                    }
                    else
                    {
                        AddMessageInfo("Not allow to Edit Document", Enums.MessageInfoType.Error);
                        return RedirectToAction("EditPbck3", new { @id = model.Pbck3Id });
                    }


                }
                else
                    AddMessageInfo("Not Valid Model", Enums.MessageInfoType.Error);


                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

                // model = GetHistorys(model);

                return View(model);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

                // model = GetHistorys(model);

                return View(model);
            }



        }
        private Pbck3ViewModel GetHistorysPbck3(Pbck3ViewModel model)
        {
            if (model.FromPbck7)
            {
                model.WorkflowHistoryPbck7 =
                    Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(model.Pbck7Number));
            }
            model.ChangesHistoryList =
                Mapper.Map<List<ChangesHistoryItemModel>>(_changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK3, model.Pbck3Id.ToString()));

            return model;
        }

        private Pbck3Dto SavePbck3ToDatabase(Pbck3ViewModel model)
        {

            var dataToSave = Mapper.Map<Pbck3Dto>(model);

            var input = new Pbck3SaveInput()
            {
                Pbck3Dto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                FormType = Enums.FormType.PBCK3
            };

            return _pbck7Pbck3Bll.SavePbck3(input);
        }

        private void PBCK3Workflow(int id, Enums.ActionType actionType, string comment, bool isModified = false)
        {
            var input = new Pbck3WorkflowDocumentInput();
            input.DocumentId = id;
            input.UserId = CurrentUser.USER_ID;
            input.UserRole = CurrentUser.UserRole;
            input.ActionType = actionType;
            input.Comment = comment;
            input.FormType = Enums.FormType.PBCK3;
            input.IsModified = isModified;
            _pbck7Pbck3Bll.PBCK3Workflow(input);
        }

        public ActionResult DetailPbck3(int id)
        {
            var model = new Pbck3ViewModel();

            try
            {
                var existingData = _pbck7Pbck3Bll.GetPbck3DetailsById(id);

                model = Mapper.Map<Pbck3ViewModel>(existingData.Pbck3CompositeDto);


                //model.WorkflowHistoryPbck7 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck7);

                model.MainMenu = _mainMenu;
                model.CurrentMenu = PageInfo;

                model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(existingData.ListChangesHistorys);
                
                model.WorkflowHistoryPbck3 = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck3);

                string nppbkcId = "";

                if (model.FromPbck7)
                {
                    model.WorkflowHistoryPbck7 =
                        Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.WorkflowHistoryPbck7);
                    nppbkcId = model.NppbkcId;
                }
                else
                {
                    model.Ck5FormViewModel =
                      Mapper.Map<CK5FormViewModel>(existingData.Pbck3CompositeDto.Ck5Composite.Ck5Dto);

                    model.Ck5FormViewModel.UploadItemModels = Mapper.Map<List<CK5UploadViewModel>>(existingData.Pbck3CompositeDto.Ck5Composite.Ck5MaterialDto);

                    model.Ck5FormViewModel.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(existingData.Pbck3CompositeDto.Ck5Composite.ListWorkflowHistorys);

                    nppbkcId = model.Ck5FormViewModel.SourceNppbkcId;
                }

                //validate approve and reject
                var input = new WorkflowAllowApproveAndRejectInput();
                input.DocumentStatus = model.Pbck3Status;
                input.FormView = Enums.FormViewType.Detail;
                input.UserRole = CurrentUser.UserRole;
                input.CreatedUser = existingData.Pbck3CompositeDto.CREATED_BY;
                input.CurrentUser = CurrentUser.USER_ID;
                input.CurrentUserGroup = CurrentUser.USER_GROUP_ID;
                input.DocumentNumber = model.Pbck3Number;
                input.NppbkcId = nppbkcId;

                //workflow
                var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
                model.AllowApproveAndReject = allowApproveAndReject;

                if (!allowApproveAndReject)
                {
                    model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                    model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
                }

                //model.AllowPrintDocument = _workflowBll.AllowPrint(model.Pbck7Status);

                if (model.AllowGovApproveAndReject)
                    model.ActionType = "GovApproveDocumentPbck3";
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);

                return RedirectToAction("ListPbck3Index");
            }


            return View("DetailPbck3", model);
        }

        public ActionResult ApproveDocumentPbck3(int id)
        {
            try
            {
                PBCK3Workflow(id, Enums.ActionType.Approve, string.Empty);
                AddMessageInfo("Success Approve Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("DetailPbck3", new { id });
        }

        [HttpPost]
        public ActionResult GovApproveDocumentPbck3(Pbck3ViewModel model)
        {
            try
            {
                var currentUserId = CurrentUser.USER_ID;

                model.Back3Documents = new List<BACK3_DOCUMENT>();
                if (model.Pbck3Back3FileUploadFileList != null)
                {
                    foreach (var item in model.Pbck3Back3FileUploadFileList)
                    {
                        if (item != null)
                        {
                            var filenameCk5Check = item.FileName;
                            if (filenameCk5Check.Contains("\\"))
                                filenameCk5Check = filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];

                            var pbck3UploadFile = new BACK3_DOCUMENT
                            {
                                FILE_NAME = filenameCk5Check,
                                FILE_PATH = SaveUploadedFile(item, model.Pbck3Number),

                            };
                            model.Back3Documents.Add(pbck3UploadFile);
                        }

                    }
                }

                model.Ck2Documents = new List<CK2_DOCUMENT>();
                if (model.Pbck3Ck2FileUploadFileList != null)
                {
                    foreach (var item in model.Pbck3Ck2FileUploadFileList)
                    {
                        if (item != null)
                        {
                            var filenameCk5Check = item.FileName;
                            if (filenameCk5Check.Contains("\\"))
                                filenameCk5Check = filenameCk5Check.Split('\\')[filenameCk5Check.Split('\\').Length - 1];

                            var pbck3UploadFile = new CK2_DOCUMENT
                            {
                                FILE_NAME = filenameCk5Check,
                                FILE_PATH = SaveUploadedFile(item, model.Pbck3Number),

                            };
                            model.Ck2Documents.Add(pbck3UploadFile);
                        }

                    }
                }

                PBCK3GovWorkflow(model);
                if (model.Pbck3GovStatus == Enums.DocumentStatusGov.FullApproved)
                    AddMessageInfo("Success Gov FullApproved Document", Enums.MessageInfoType.Success);
                else if (model.Pbck3GovStatus == Enums.DocumentStatusGov.PartialApproved)
                    AddMessageInfo("Success Gov PartialApproved Document", Enums.MessageInfoType.Success);
                else if (model.Pbck3GovStatus == Enums.DocumentStatusGov.Rejected)
                    AddMessageInfo("Success Gov Reject Document", Enums.MessageInfoType.Success);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("DetailPbck3", new { id = model.Pbck3Id });
        }

        private bool PBCK3GovWorkflow(Pbck3ViewModel model)
        {
            try
            {
                var actionType = Enums.ActionType.GovApprove;

                if (model.Pbck3GovStatus == Enums.DocumentStatusGov.PartialApproved)
                    actionType = Enums.ActionType.GovPartialApprove;
                else if (model.Pbck3GovStatus == Enums.DocumentStatusGov.Rejected)
                    actionType = Enums.ActionType.GovReject;

                var input = new Pbck3WorkflowDocumentInput();
                input.DocumentId = model.Pbck3Id;
                input.DocumentNumber = model.Pbck3Number;
                input.UserId = CurrentUser.USER_ID;
                input.UserRole = CurrentUser.UserRole;
                input.ActionType = actionType;
                input.Comment = model.Comment;
                input.FormType = Enums.FormType.PBCK3;
                input.GovStatusInput = model.Pbck3GovStatus;

                input.AdditionalDocumentData = new Pbck3WorkflowDocumentData();
                input.AdditionalDocumentData.Back3No = model.Back3Number;
                input.AdditionalDocumentData.Back3Date = model.Back3Date;
                input.AdditionalDocumentData.Back3FileUploadList = model.Back3Documents;

                input.AdditionalDocumentData.Ck2No = model.Ck2Number;
                input.AdditionalDocumentData.Ck2Date = model.Ck2Date;
                input.AdditionalDocumentData.Ck2Value = model.Ck2Value;
                input.AdditionalDocumentData.Ck2FileUploadList = model.Ck2Documents;


                _pbck7Pbck3Bll.PBCK3Workflow(input);

                try
                {
                    if (model.Pbck3GovStatus == Enums.DocumentStatusGov.PartialApproved ||
                        model.Pbck3GovStatus == Enums.DocumentStatusGov.FullApproved)
                    {

                        //create xml file
                        var outputPbck3 = _pbck7Pbck3Bll.GetPbck3DetailsById(model.Pbck3Id);

                        //only completed document can create xml file
                        if (outputPbck3.Pbck3CompositeDto.STATUS == Enums.DocumentStatus.Completed)
                        {

                            var ck2 = new Ck2Dto();
                            ck2.Ck2Number = outputPbck3.Pbck3CompositeDto.Ck2Number;
                            ck2.Ck2Date = outputPbck3.Pbck3CompositeDto.Ck2Date;
                            ck2.Ck2Value = outputPbck3.Pbck3CompositeDto.Ck2Value;

                            string nppbkcId = "";
                            if (outputPbck3.Pbck3CompositeDto.FromPbck7)
                                nppbkcId = outputPbck3.Pbck3CompositeDto.Pbck7Composite.NppbkcId;
                            CreateXml(ck2, nppbkcId, outputPbck3.Pbck3CompositeDto.PBCK3_NUMBER);

                            //send mail after that
                            _pbck7Pbck3Bll.SendMailCompletedPbck3Document(input);


                        }

                    }
                    return true;

                }
                catch (Exception ex)
                {
                    //failed create xml...
                    //rollaback the update
                    // _pbck4Bll.GovApproveDocumentRollback(input);
                    AddMessageInfo("Failed Create PBCK3 XMl message : " + ex.Message, Enums.MessageInfoType.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return false;
            }

        }

        [HttpPost]
        public ActionResult RejectDocumentPbck3(Pbck3ViewModel model)
        {
            try
            {
                PBCK3Workflow(model.Pbck3Id, Enums.ActionType.Reject, model.Comment);
                AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }
            return RedirectToAction("DetailPbck3", new { id = model.Pbck3Id });
        }

    }

}