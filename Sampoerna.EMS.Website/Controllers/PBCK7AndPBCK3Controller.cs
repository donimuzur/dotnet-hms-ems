using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using iTextSharp.text.pdf.qrcode;
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

        private void SetChanges(Pbck7AndPbck3Dto origin, Pbck7Pbck3CreateViewModel dataModified)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("DATE", origin.Pbck7Date == dataModified.Pbck7Date);
            changesData.Add("EXEC_FROM", origin.ExecDateFrom == dataModified.ExecDateFrom);
            changesData.Add("EXEC_TO", origin.ExecDateTo == dataModified.ExecDateTo);
            changesData.Add("LAMPIRAN", origin.Lampiran == dataModified.Lampiran);
            changesData.Add("DOC_TYPE", origin.DocumentType == dataModified.DocumentType);
           // changesData.Add("BACK1_NO", origin.Back1Dto.Back1Number == dataModified.Back1Dto.Back1Number);
           // changesData.Add("BACK1_DATE", origin.Back1Dto.Back1Date == dataModified.Back1Dto.Back1Date);
            

            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.PBCK7;
                    changes.FORM_ID = origin.Pbck7Id.ToString();
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = CurrentUser.USER_ID;
                    changes.MODIFIED_DATE = DateTime.Now;
                    switch (listChange.Key)
                    {
                        case "DATE":
                            changes.OLD_VALUE = origin.Pbck7Date.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = dataModified.Pbck7Date.Value.ToString("dd MMM yyyy");
                            break;
                        case "EXEC_FROM":
                            changes.OLD_VALUE = origin.ExecDateFrom.Value.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = dataModified.ExecDateFrom.Value.ToString("dd MMM yyyy");
                            break;
                        case "EXEC_TO":
                            changes.OLD_VALUE = origin.ExecDateTo.Value.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = dataModified.ExecDateTo.Value.ToString("dd MMM yyyy");
                            break;
                        case "LAMPIRAN":
                            changes.OLD_VALUE = origin.Lampiran;
                            changes.NEW_VALUE = dataModified.Lampiran;
                            break;
                        case "DOC_TYPE":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.DocumentType);
                            changes.NEW_VALUE =EnumHelper.GetDescription(dataModified.DocumentType);
                            break;
                        //case "BACK1_NO":
                        //    changes.OLD_VALUE = origin.Back1Dto.Back1Number;
                        //    changes.NEW_VALUE = dataModified.Back1Dto.Back1Number;
                        //    break;
                        //case "BACK1_DATE":
                        //    changes.OLD_VALUE = origin.Back1Dto.Back1Date.Value.ToString("dd MMM yyyy");
                        //    changes.NEW_VALUE = dataModified.Back1Dto.Back1Date.Value.ToString("dd MMM yyyy");
                        //    break;
                     
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
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
            dt.Columns.Add("PoaName", System.Type.GetType("System.String"));
            dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
            dt.Columns.Add("CompanyAddress", System.Type.GetType("System.String"));
            dt.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dt.Columns.Add("Header", System.Type.GetType("System.Byte[]"));
            dt.Columns.Add("Footer", System.Type.GetType("System.String"));
            dt.Columns.Add("TotalKemasan", System.Type.GetType("System.String"));
            dt.Columns.Add("TotalCukai", System.Type.GetType("System.String"));
            dt.Columns.Add("PrintedDate", System.Type.GetType("System.String"));
            dt.Columns.Add("Preview", System.Type.GetType("System.String"));
            dt.Columns.Add("DecreeDate", System.Type.GetType("System.String"));
            dt.Columns.Add("Nomor", System.Type.GetType("System.String"));
            dt.Columns.Add("Lampiran", System.Type.GetType("System.String"));
            dt.Columns.Add("TextTo", System.Type.GetType("System.String"));
            dt.Columns.Add("VendorCity", System.Type.GetType("System.String"));
            dt.Columns.Add("DocumentType", System.Type.GetType("System.String"));
            dt.Columns.Add("NppbkcCity", System.Type.GetType("System.String"));
            dt.Columns.Add("PbckDate", System.Type.GetType("System.String"));
            //detail
            DataTable dtDetail = new DataTable("Detail");
            dtDetail.Columns.Add("Jenis", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Merek", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("IsiKemasan", System.Type.GetType("System.String"));

            dtDetail.Columns.Add("JmlKemasan", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("SeriPitaCukai", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Hje", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Tariff", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("JmlCukai", System.Type.GetType("System.String"));
            ds.Tables.Add(dt);
            ds.Tables.Add(dtDetail);
            return ds;
        }

        [EncryptedParameter]
        public FileResult PrintPreviewPbck7(int id)
        {
            return PrintPreview(id, true);
        }
        [EncryptedParameter]
        public FileResult PrintPreviewPbck3(int id)
        {
            return PrintPreview(id, false);
        }
        public FileResult PrintPreview(int id, bool isPbck7)
        {
            var pbck7 = _pbck7Pbck3Bll.GetPbck7ById(id);
            if (!isPbck7)
            {
                //get pbck3
                if (pbck7 != null)
                {
                    pbck7.Pbck3Dto = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(pbck7.Pbck7Id);
                }
            }
            var dsPbck7 = CreatePbck7Ds();
            var dt = dsPbck7.Tables[0];
            DataRow drow;
            drow = dt.NewRow();
            
           string approvedBy = null;
            if(isPbck7)
                approvedBy = pbck7.ApprovedBy;
            else
                approvedBy = pbck7.Pbck3Dto.ApprovedBy;
            if (approvedBy != null)
            {
                drow["PoaName"] = _poaBll.GetById(approvedBy).PRINTED_NAME;
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
            drow["PrintedDate"] = isPbck7 ? pbck7.Pbck7Date.ToString("dd MMM yyyy") : pbck7.Pbck3Dto.Pbck3Date.Value.ToString("dd MMM yyyy");
            if (isPbck7)
            {
                if (pbck7.Pbck7Status != Enums.DocumentStatus.Completed)
                {
                    drow["Preview"] = "PREVIEW PBCK-7";
                }
                else
                {
                    drow["Preview"] = "PBCK-7";

                }
            }
            else
            {
                if (pbck7.Pbck3Dto.Pbck3Status != Enums.DocumentStatus.Completed)
                {
                    drow["Preview"] = "PREVIEW PBCK-3";
                }
                else
                {
                    drow["Preview"] = "PBCK-3";

                }

            }
            drow["Nomor"] = isPbck7 ? pbck7.Pbck7Number :pbck7.Pbck3Dto.Pbck3Number;
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
            drow["DocumentType"] = EnumHelper.GetDescription(pbck7.DocumentType);
            drow["NppbkcCity"] = nppbkc.CITY;
            drow["PbckDate"] = isPbck7 ? pbck7.Pbck7Date.ToString("dd MMMM yyyy") : pbck7.Pbck3Dto.Pbck3Date.Value.ToString("dd MMMM yyyy");
          
            dt.Rows.Add(drow);



            var dtDetail = dsPbck7.Tables[1];
            foreach (var item in pbck7.UploadItems)
            {
                DataRow drowDetail;
                drowDetail = dtDetail.NewRow();
                drowDetail[0] = item.ProdTypeAlias;
                drowDetail[1] = item.Brand;
                drowDetail[2] = Convert.ToInt32(item.Content);
                drowDetail[3] = item.Pbck7Qty;
                drowDetail[4] = item.SeriesValue;
                drowDetail[5] = item.Hje;
                drowDetail[6] = item.Tariff;
                drowDetail[7] = item.ExciseValue;
                dtDetail.Rows.Add(drowDetail);

            }
            // object of data row 

            ReportClass rpt = new ReportClass();
            string report_path = System.Configuration.ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = System.IO.Path.Combine(report_path, "PBCK7", "Pbck7Report.rpt");
            rpt.Load();
            rpt.SetDataSource(dsPbck7);

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

        private Pbck7IndexViewModel InitPbck7ViewModel(Pbck7IndexViewModel model)
        {
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GlobalFunctions.GetPoaAll(_poaBll);
            model.CreatorList = GlobalFunctions.GetCreatorList();

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
                IsCompletedDoc =  false
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
        public JsonResult PoaAndPlantListPartialPbck7(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() {PoaList = listPoa, PlantList = listPlant};

            return Json(model);
        }

        [HttpPost]
        public JsonResult PoaAndPlantListPartialPbck3(string nppbkcId)
        {
            var listPoa = GlobalFunctions.GetPoaByNppbkcId(nppbkcId);
            var listPlant = GlobalFunctions.GetPlantByNppbkcId(_plantBll, nppbkcId);
            var model = new Pbck7IndexViewModel() {PoaList = listPoa, PlantList = listPlant};

            return Json(model);
        }

        #endregion

    #region Create

        public ActionResult Create()
        {
            var model = new Pbck7Pbck3CreateViewModel();
            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.NppbkIdList = GlobalFunctions.GetNppbkcAll(_nppbkcBll);
            model.PlantList = GlobalFunctions.GetPlantAll();
            model.PoaList = GetPoaList(model.NppbkcId);
            model.Pbck7Date = DateTime.Now;
            return View("Create",model);
        }

        #endregion

        public void GetDetailPbck7(Pbck7AndPbck3Dto existingData)
        {
            existingData.Back1Dto = _pbck7Pbck3Bll.GetBack1ByPbck7(existingData.Pbck7Id);
            existingData.Pbck3Dto = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(existingData.Pbck7Id);

            if (existingData.Pbck3Dto != null)
            {
                existingData.Back3Dto = _pbck7Pbck3Bll.GetBack3ByPbck3Id(existingData.Pbck3Dto.Pbck3Id);
                existingData.Ck2Dto = _pbck7Pbck3Bll.GetCk2ByPbck3Id(existingData.Pbck3Dto.Pbck3Id);
            }
            if (existingData.Back1Dto == null)
                existingData.Back1Dto = new Back1Dto();
            if (existingData.Pbck3Dto == null)
                existingData.Pbck3Dto = new Pbck3Dto();
            if (existingData.Back3Dto == null)
                existingData.Back3Dto = new Back3Dto();
            if (existingData.Ck2Dto == null)
                existingData.Ck2Dto = new Ck2Dto();
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            
            var existingData = _pbck7Pbck3Bll.GetPbck7ById(id);
            if (existingData.CreatedBy != CurrentUser.USER_ID)
            {
                return RedirectToAction("Detail", new {id = id});
            }
            GetDetailPbck7(existingData);
            
          
            var model = Mapper.Map<Pbck7Pbck3CreateViewModel>(existingData);
           
            return View("Edit", InitialModel(model));
        }

        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            var existingData = _pbck7Pbck3Bll.GetPbck7ById(id);
            GetDetailPbck7(existingData);
            var model = Mapper.Map<Pbck7Pbck3CreateViewModel>(existingData);
            model = InitialModel(model);
            if (model.Pbck7Status == Enums.DocumentStatus.Completed)
            {
                var printHistory =
                    Mapper.Map<List<PrintHistoryItemModel>>(
                        _printHistoryBll.GetByFormNumber(model.Pbck7Number));
                model.PrintHistoryList = printHistory;
            }
            if (model.Pbck3Dto != null)
            {
                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Completed)
                {
                    var printHistory =
                        Mapper.Map<List<PrintHistoryItemModel>>(
                            _printHistoryBll.GetByFormNumber(model.Pbck3Dto.Pbck3Number));
                    model.PrintHistoryListPbck3 = printHistory;
                }
            }
            var changesHistoryPbck = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK7, id.Value.ToString());
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(changesHistoryPbck);
           
            return View("Detail", model);
        }

        public void SaveBack3(Pbck7Pbck3CreateViewModel model)
        {
            var existingData = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(model.Id);
            if (existingData != null)
            {

                var back3Dto = new Back3Dto();
                if (model.DocumentsPostBack3 != null)
                {
                    back3Dto.Back3Document = new List<BACK3_DOCUMENT>();
                    foreach (var sk in model.DocumentsPostBack3)
                    {
                        if (sk != null)
                        {
                            var document = new BACK3_DOCUMENT();
                            var filenamecheck = sk.FileName;
                            if (filenamecheck.Contains("\\"))
                            {
                                document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }
                            else
                            {
                                document.FILE_NAME = sk.FileName;
                            }

                            document.FILE_PATH = SaveUploadedFile(sk, model.Back3Dto.Back3Number.Trim().Replace('/', '_'));
                            back3Dto.Back3Document.Add(document);

                        }
                    }
                }

                back3Dto.Back3Number = model.Back3Dto.Back3Number;
                back3Dto.Back3Date = model.Back3Dto.Back3Date;
                back3Dto.Pbck3ID = existingData.Pbck3Id;

                _pbck7Pbck3Bll.InsertBack3(back3Dto);
                var ck2Dto = SaveCk2(model, existingData.Pbck3Id);
                if (existingData.Pbck3Status == Enums.DocumentStatus.GovApproved)
                {
                    existingData.Pbck3Status = Enums.DocumentStatus.Completed;
                   _pbck7Pbck3Bll.InsertPbck3(existingData);
                   CreateXml(ck2Dto, model.NppbkcId, existingData.Pbck3Number);
                    
                    
                }
            }

        }


        public void CreateXml(Ck2Dto ck2, string nppbckId, string pbck3Number)
        {
            var pbck4xmlDto = new Pbck4XmlDto();
            pbck4xmlDto.NppbckId = nppbckId;
            pbck4xmlDto.CompType = "CK-2";
            pbck4xmlDto.PbckNo = pbck3Number;
            pbck4xmlDto.CompnDate = ck2.Ck2Date;
            pbck4xmlDto.CompnValue = ck2.Ck2Value.HasValue? ck2.Ck2Value.ToString() : null;
            pbck4xmlDto.CompNo = ck2.Ck2Number;
            var fileName = System.Configuration.ConfigurationManager.AppSettings["CK5PathXml"] + "COMPENSATION-CK2-" +
                               DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml";

            pbck4xmlDto.GeneratedXmlPath = fileName;
               
            var xmlwriter = new XMLReader.XmlPBCK4DataWriter();
            xmlwriter.CreatePbck4Xml(pbck4xmlDto);
        }

        public Ck2Dto SaveCk2(Pbck7Pbck3CreateViewModel model, int pbck3Id)
        {
            

                var ck2Dto = new Ck2Dto();
                if (model.DocumentsPostCk2 != null)
                {
                    ck2Dto.Ck2Document = new List<CK2_DOCUMENT>();
                    foreach (var sk in model.DocumentsPostCk2)
                    {
                        if (sk != null)
                        {
                            var document = new CK2_DOCUMENT();
                            var filenamecheck = sk.FileName;
                            if(filenamecheck.Contains("\\"))
                            {
                                document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }
                            else
                            {
                                document.FILE_NAME = sk.FileName;
                            }

                            document.FILE_PATH = SaveUploadedFile(sk, model.Back3Dto.Back3Number.Trim().Replace('/', '_'));
                            ck2Dto.Ck2Document.Add(document);

                        }
                    }
                }

                ck2Dto.Ck2Number = model.Ck2Dto.Ck2Number;
                ck2Dto.Ck2Date = model.Ck2Dto.Ck2Date;
                ck2Dto.Pbck3ID = pbck3Id;
                _pbck7Pbck3Bll.InsertCk2(ck2Dto);

            return ck2Dto;

        }

        private void SubmitPbck3(Pbck7Pbck3CreateViewModel model)
        {
            if (model.Pbck3Dto.Pbck3Status != Enums.DocumentStatus.Completed)
            {
                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Draft)
                {
                    if (CurrentUser.UserRole == Enums.UserRole.POA)
                    {
                        model.Pbck3Dto.Pbck3Status = Enums.DocumentStatus.WaitingForApprovalManager;
                    }
                    else if (CurrentUser.UserRole == Enums.UserRole.User)
                    {
                        model.Pbck3Dto.Pbck3Status = Enums.DocumentStatus.WaitingForApproval;
                    }

                }

            }
        }

        public void SavePbck3(Pbck7Pbck3CreateViewModel model)
        {
            var existingData = _pbck7Pbck3Bll.GetPbck7ById(model.Id);
            GetDetailPbck7(existingData);
           
            if (existingData != null)
            {
               var pbck3 = new Pbck3Dto();
              
                if (existingData.Pbck3Dto != null && existingData.Pbck3Dto.Pbck3Id != 0)
                {
                    pbck3 = existingData.Pbck3Dto;
                    pbck3.Pbck3Date = model.Pbck3Dto.Pbck3Date;

                    //if submit
                    if (model.IsSaveSubmitPbck3)
                    {
                        
                        SubmitPbck3(model);
                    }
                    else
                    {
                        //if edit then save
                        if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Rejected)
                        {
                            model.Pbck3Dto.Pbck3Status = Enums.DocumentStatus.Draft;
                        }
                    }

                    if (model.Pbck3Dto.Pbck3GovStatus != null)
                    {
                        if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.WaitingGovApproval)
                        {
                            pbck3.Pbck3Status = Enums.DocumentStatus.GovApproved;
                        }
                    }
                    
                   
                   

                }
                else
                {
                    
                        //if new
                        pbck3.Pbck3Status = Enums.DocumentStatus.Draft;

                        var inputDoc = new GenerateDocNumberInput();
                        inputDoc.Month = model.Pbck3Dto.Pbck3Date.Value.Month;
                        inputDoc.Year = model.Pbck3Dto.Pbck3Date.Value.Year;
                        inputDoc.NppbkcId = existingData.NppbkcId;
                        pbck3.CreateDate = DateTime.Now;
                        pbck3.CreatedBy = CurrentUser.USER_ID;
                        pbck3.Pbck7Id = existingData.Pbck7Id;
                        pbck3.Pbck3Date = model.Pbck3Dto.Pbck3Date;
                        pbck3.Pbck3Number = _documentSequenceNumberBll.GenerateNumberNoReset(inputDoc);
                    
                   

                    
                }
                _pbck7Pbck3Bll.InsertPbck3(pbck3);
                
                
            }
        }

        public void SaveBack1(Pbck7Pbck3CreateViewModel model)
        {
            var existingData = _pbck7Pbck3Bll.GetPbck7ById(model.Id);
            if (existingData != null)
            {
               
                var back1Dto = new Back1Dto();
                if (model.DocumentsPostBack1 != null)
                {
                    back1Dto.Documents = new List<BACK1_DOCUMENT>();
                    foreach (var sk in model.DocumentsPostBack1)
                    {
                        if (sk != null)
                        {
                            var document = new BACK1_DOCUMENT();
                            var filenamecheck = sk.FileName;
                            if (filenamecheck.Contains("\\"))
                            {
                                document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                            }
                            else
                            {
                                document.FILE_NAME = sk.FileName;
                            }
                           
                            document.FILE_PATH = SaveUploadedFile(sk, model.Back1Dto.Back1Number.Trim().Replace('/', '_'));
                            back1Dto.Documents.Add(document);

                        }
                    }
                }

                back1Dto.Back1Number = model.Back1Dto.Back1Number;
                back1Dto.Back1Date = model.Back1Dto.Back1Date;
                back1Dto.Pbck7Id = existingData.Pbck7Id;
                var uploadItem = model.UploadItems;
                foreach (var pbck7ItemUpload in uploadItem)
                {
                    _pbck7Pbck3Bll.InsertPbck7Item(pbck7ItemUpload);

                }
                
                _pbck7Pbck3Bll.InsertBack1(back1Dto);
                if (existingData.Pbck7Status == Enums.DocumentStatus.GovApproved)
                {
                    existingData.Pbck7Status = Enums.DocumentStatus.Completed;
                    //prevent error when update pbck7
                    existingData.UploadItems = null;
                    _pbck7Pbck3Bll.InsertPbck7(existingData);
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pbck7Pbck3CreateViewModel model)
        {

            if (model.Pbck7Status == Enums.DocumentStatus.GovApproved)
            {
                if (!string.IsNullOrEmpty(model.Back1Dto.Back1Number) && model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Draft)
                {
                    SaveBack1(model);
                    return RedirectToAction("Index");
                }
            }
            if (model.Pbck7Status == Enums.DocumentStatus.Completed)
            {
                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.GovApproved)
                {
                    SaveBack3(model);
                  
                }
                else
                {
                    SavePbck3(model);
                }
                return RedirectToAction("Index");
            }

            var item = AutoMapper.Mapper.Map<Pbck7AndPbck3Dto>(model);
            
            if (item.CreatedBy != CurrentUser.USER_ID)
            {
                return RedirectToAction("Detail", new {id = item.Pbck7Id});
            }
            var exItems = new Pbck7ItemUpload[item.UploadItems.Count];
            item.UploadItems.CopyTo(exItems);
            item.UploadItems = new List<Pbck7ItemUpload>();
            foreach (var items in exItems)
            {
                if (items.Id == 0)
                {
                    item.UploadItems.Add(items);
                }
            }

            
            if (item.Pbck7GovStatus == Enums.DocumentStatusGov.PartialApproved)
            {
                item.Pbck7Status = Enums.DocumentStatus.GovApproved;
            }
            if (item.Pbck7GovStatus == Enums.DocumentStatusGov.FullApproved)
            {
                item.Pbck7Status = Enums.DocumentStatus.GovApproved;
                if (exItems != null)
                {
                    foreach (var itemUpload in exItems)
                    {
                        itemUpload.Back1Qty = itemUpload.Pbck7Qty;
                        _pbck7Pbck3Bll.InsertPbck7Item(itemUpload);
                    }
                }
            }
            if (item.Pbck7GovStatus == Enums.DocumentStatusGov.Rejected)
            {
                item.Pbck7Status = Enums.DocumentStatus.GovRejected;
            }
            if (item.Pbck7Status == Enums.DocumentStatus.Rejected)
            {
                item.Pbck7Status = Enums.DocumentStatus.Draft;
            }
            if (model.IsSaveSubmit)
            {
                if (item.Pbck7Status == Enums.DocumentStatus.Draft)
                {
                    if (CurrentUser.UserRole == Enums.UserRole.POA)
                    {
                        item.Pbck7Status = Enums.DocumentStatus.WaitingForApprovalManager;
                        item.ApprovedBy = CurrentUser.USER_ID;
                        item.ApprovedDate = DateTime.Now;
                    }
                    else if (CurrentUser.UserRole == Enums.UserRole.User)
                    {
                        item.Pbck7Status = Enums.DocumentStatus.WaitingForApproval;
                    }

                }
               

            }
           
            item.ModifiedBy = CurrentUser.USER_ID;
            item.ModifiedDate = DateTime.Now;
            var plant = _plantBll.GetId(item.PlantId);
            item.PlantCity = plant.ORT01;
            item.PlantName = plant.NAME1;
            var origin = _pbck7Pbck3Bll.GetPbck7ById(model.Id);
            SetChanges(origin,model);


            _pbck7Pbck3Bll.InsertPbck7(item);
            if(model.IsSaveSubmit)
            {
                AddMessageInfo("Submit Success", Enums.MessageInfoType.Success);
            }
            else
            {
                AddMessageInfo("Update Success", Enums.MessageInfoType.Success);
            }
            if (item.Pbck7Status == Enums.DocumentStatus.Draft)
            {
                return RedirectToAction("Edit", new {id = item.Pbck7Id});
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pbck7Pbck3CreateViewModel model)
        {
            var modelDto = Mapper.Map<Pbck7AndPbck3Dto>(model);
            modelDto.CreatedBy = CurrentUser.USER_ID;
            modelDto.CreateDate = DateTime.Now;
            modelDto.Pbck7Status = Enums.DocumentStatus.Draft;
            var plant = _plantBll.GetId(model.PlantId);
            modelDto.PlantName = plant.NAME1;
            modelDto.PlantCity = plant.ORT01;
            var inputDoc = new GenerateDocNumberInput();
            inputDoc.Month = modelDto.Pbck7Date.Month;
            inputDoc.Year = modelDto.Pbck7Date.Year;
            inputDoc.NppbkcId = modelDto.NppbkcId;
            modelDto.Pbck7Number = _documentSequenceNumberBll.GenerateNumberNoReset(inputDoc);

            int? pbck7IdAfterSave= null;
            try
            {
                pbck7IdAfterSave = _pbck7Pbck3Bll.InsertPbck7(modelDto);
            }
            catch (Exception ex)
            {
               AddMessageInfo(ex.ToString(), Enums.MessageInfoType.Error);
            }
            AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
            return RedirectToAction("Edit", new { id = pbck7IdAfterSave});
        }

        private Pbck7AndPbck3Dto SavePbck7Pbck3ToDatabase(Pbck7Pbck3CreateViewModel model)
        {

            var dataToSave = Mapper.Map<Pbck7AndPbck3Dto>(model);
            

            //dataToSave.APPROVED_BY_POA = null;

            var input = new Pbck7Pbck3SaveInput()
            {
                Pbck7Pbck3Dto = dataToSave,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                Pbck7Pbck3Items = Mapper.Map<List<Pbck7ItemUpload>>(model.UploadItems)
            };

            return _pbck7Pbck3Bll.SavePbck4(input);
        }

        public string GetPoaList(string nppbkcid)
        {
            var poaList = _poaBll.GetPoaByNppbkcId(nppbkcid).Distinct().ToList();
            var poaListStr = string.Empty;
            var poaLength = poaList.Count;
            
            for(int i=0; i< poaLength; i++)
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
            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormId = model.Id;
            workflowInput.FormNumber = model.Pbck7Number;
            workflowInput.DocumentStatus = model.Pbck7Status;
            workflowInput.NPPBKC_Id = model.NppbkcId;
            workflowInput.FormType = Enums.FormType.PBCK7;
            ;
            model.WorkflowHistoryPbck7 = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));

            workflowInput.FormId = model.Pbck3Dto.Pbck3Id;
            workflowInput.FormNumber = model.Pbck3Dto.Pbck3Number;
            workflowInput.DocumentStatus = model.Pbck3Dto.Pbck3Status;
            workflowInput.NPPBKC_Id = model.NppbkcId;
            workflowInput.FormType = Enums.FormType.PBCK3;

            model.WorkflowHistoryPbck3 = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput)); ;
            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Pbck7Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = model.CreatedBy,
                CurrentUser = CurrentUser.USER_ID,
                CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                DocumentNumber = model.Pbck7Number,
                NppbkcId = model.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;
            model.AllowEditAndSubmit = CurrentUser.USER_ID == model.CreatedBy;
            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }
            if (model.Pbck7Status == Enums.DocumentStatus.Completed)
            {
                model.AllowPrintDocument = true;
            }

            if (model.Pbck7Status == Enums.DocumentStatus.Completed)
            {
                //validate approve and reject
                var inputPbck3 = new WorkflowAllowApproveAndRejectInput
                {
                    DocumentStatus = model.Pbck3Dto.Pbck3Status,
                    FormView = Enums.FormViewType.Detail,
                    UserRole = CurrentUser.UserRole,
                    CreatedUser = model.CreatedBy,
                    CurrentUser = CurrentUser.USER_ID,
                    CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                    DocumentNumber = model.Pbck3Dto.Pbck3Number,
                    NppbkcId = model.NppbkcId
                };

                ////workflow

                model.AllowApproveAndRejectPbck3 = _workflowBll.AllowApproveAndReject(inputPbck3);
                model.AllowEditAndSubmitPbck3 = CurrentUser.USER_ID == model.CreatedBy;
                if (!model.AllowApproveAndRejectPbck3)
                {
                    model.AllowGovApproveAndRejectPbck3 = _workflowBll.AllowGovApproveAndReject(inputPbck3);
                    model.AllowManagerRejectPbck3 = _workflowBll.AllowManagerReject(inputPbck3);
                }
                if (model.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Completed)
                {
                    model.AllowPrintDocumentPbck3 = true;
                }
            }



            return (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(Pbck7Pbck3CreateViewModel model)
        {

            if (model.ActionType == "Approve")
            {
                return RedirectToAction("Approve", new { id = model.Id });
            }
            else if (model.ActionType == "ApprovePbck3")
            {
                return RedirectToAction("ApprovePbck3", new { id = model.Id });
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult Approve(int id)
        {
            var urlBuilder =
                   new System.UriBuilder(Request.Url.AbsoluteUri)
                   {
                       Path = Url.Action("Detail", "PBCK7AndPBCK3", new { id = id }),
                       Query = null,
                   };

            Uri uri = urlBuilder.Uri;
            if (uri != Request.UrlReferrer)
                return HttpNotFound();
            var item = _pbck7Pbck3Bll.GetPbck7ById(id);
            
            var statusPbck7 = item.Pbck7Status;
            if (statusPbck7 != Enums.DocumentStatus.Completed)
            {
                if (statusPbck7 == Enums.DocumentStatus.WaitingForApproval)
                {
                    item.Pbck7Status = Enums.DocumentStatus.WaitingForApprovalManager;
                    item.ApprovedBy = CurrentUser.USER_ID;
                    item.ApprovedDate = DateTime.Now;
                }
                else if (statusPbck7 == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    item.Pbck7Status = Enums.DocumentStatus.WaitingGovApproval;
                    item.ApprovedByManager = CurrentUser.USER_ID;
                    item.ApprovedDateManager = DateTime.Now;
                }

                else if (statusPbck7 == Enums.DocumentStatus.WaitingGovApproval)
                {
                    item.Pbck7Status = Enums.DocumentStatus.GovApproved;
                }
            }
            
            item.UploadItems = null;
            _pbck7Pbck3Bll.InsertPbck7(item);
            AddMessageInfo("Approve Success", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }


        public ActionResult ApprovePbck3(int id)
        {
            var urlBuilder =
                   new System.UriBuilder(Request.Url.AbsoluteUri)
                   {
                       Path = Url.Action("Detail", "PBCK7AndPBCK3", new { id = id }),
                       Query = null,
                   };

            Uri uri = urlBuilder.Uri;
            if (uri != Request.UrlReferrer)
                return HttpNotFound();
            var item = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(id);

            var statusPbck3 = item.Pbck3Status;
            if (statusPbck3 != Enums.DocumentStatus.Completed)
            {
                if (statusPbck3 == Enums.DocumentStatus.WaitingForApproval)
                {
                    item.Pbck3Status = Enums.DocumentStatus.WaitingForApprovalManager;
                    item.ApprovedBy = CurrentUser.USER_ID;
                    item.ApprovedDate = DateTime.Now;
                }
                else if (statusPbck3 == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    item.Pbck3Status = Enums.DocumentStatus.WaitingGovApproval;
                    item.ApprovedByManager = CurrentUser.USER_ID;
                    item.ApprovedDateManager = DateTime.Now;
                }

                else if (statusPbck3 == Enums.DocumentStatus.WaitingGovApproval)
                {
                    item.Pbck3Status = Enums.DocumentStatus.GovApproved;
                }
            }
            

            
            _pbck7Pbck3Bll.InsertPbck3(item);
            AddMessageInfo("Approve Success", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase itemExcelFile, string plantId)
        {
            var data = (new ExcelReader()).ReadExcel(itemExcelFile);
            var model = new List<Pbck7ItemUpload>();
            if (data != null)
            {
                foreach (var datarow in data.DataRows)
                {
                    var item = new Pbck7ItemUpload();
                    item.FaCode = datarow[0];
                    item.Pbck7Qty = Convert.ToDecimal(datarow[1]);
                    item.FiscalYear = Convert.ToInt32(datarow[2]);
                    
                    try
                    {
                        var existingBrand = _brandRegistration.GetByIdIncludeChild(plantId, item.FaCode);
                        if (existingBrand != null)
                        {
                            item.Brand = existingBrand.BRAND_CE;
                            item.SeriesValue = existingBrand.ZAIDM_EX_SERIES.SERIES_CODE;
                            item.ProdTypeAlias = existingBrand.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS;
                            item.Content = Convert.ToInt32(existingBrand.BRAND_CONTENT);
                            item.Hje = existingBrand.HJE_IDR;
                            item.Tariff = existingBrand.TARIFF;
                            item.ExciseValue = item.Content*item.Tariff*item.Pbck7Qty;
                            model.Add(item);
                        }
                        else
                        {
                            return Json(-1);
                        }

                    }
                    catch (Exception)
                    {
                        
                    }
                  
                   
                }
            }
            return Json(model);
        }

        private string SaveUploadedFile(HttpPostedFileBase file, string back1Num)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";


            sFileName = Constans.UploadPath + System.IO.Path.GetFileName("BACK1_" + back1Num + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + System.IO.Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
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
                    Pbck7Date =  b.Pbck7Date,
                    Pbck7Status = Sampoerna.EMS.Utils.EnumHelper.GetDescription(b.Pbck7Status),
                    ExecFrom = b.ExecDateFrom,
                    ExecTo = b.ExecDateTo,
                    Back1No = b.Back1Dto != null ? b.Back1Dto.Back1Number : string.Empty,
                    Back1Date  = b.Back1Dto != null ? b.Back1Dto.Back1Date : null

                    
                }).ToList();
            var grid = new System.Web.UI.WebControls.GridView
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
                           Pbck3Status = Sampoerna.EMS.Utils.EnumHelper.GetDescription(b.Pbck3Status),
                           Back3No = b.Back3Dto != null ? b.Back3Dto.Back3Number : string.Empty,
                           Back3Date = b.Back3Dto != null ? b.Back3Dto.Back3Date : null,
                           Ck2No =  b.Ck2Dto != null ? b.Ck2Dto.Ck2Number : string.Empty,
                           Ck2Date = b.Ck2Dto != null ? b.Ck2Dto.Ck2Date : null,
                           Ck2Value = b.Ck2Dto != null ? b.Ck2Dto.Ck2Value : 0
                          

                       }).ToList();
            var grid = new System.Web.UI.WebControls.GridView
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
            bool isSuccess = false;
            try
            {
                var item = _pbck7Pbck3Bll.GetPbck7ById(model.Id);
                item.Pbck7Status = Enums.DocumentStatus.Rejected;
                item.IsRejected = true;
                item.Comment = model.Comment;
                item.RejectedBy = CurrentUser.USER_ID;
                item.RejectedDate = DateTime.Now;
                item.UploadItems = null;
                _pbck7Pbck3Bll.InsertPbck7(item);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Detail", "Pbck7AndPbck3", new { id = model.Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult RejectDocumentPbck3(Pbck7Pbck3CreateViewModel model)
        {
            bool isSuccess = false;
            try
            {
                var item = _pbck7Pbck3Bll.GetPbck3ByPbck7Id(model.Id);
                item.Pbck3Status= Enums.DocumentStatus.Rejected;
                item.IsRejected = true;
                item.Comment = model.Comment;
                item.RejectedBy = CurrentUser.USER_ID;
                item.RejectedDate = DateTime.Now;
                
                _pbck7Pbck3Bll.InsertPbck3(item);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Detail", "Pbck7AndPbck3", new { id = model.Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }

    }

}