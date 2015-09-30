using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Routing;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Filters;
using Sampoerna.EMS.Website.Models.LACK2;
using AutoMapper;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Reports.HeaderFooter;
using SpreadsheetLight;

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
        private IWorkflowBLL _workflowBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IPrintHistoryBLL _printHistoryBll;
        public LACK2Controller(IPageBLL pageBll, IPOABLL poabll, IHeaderFooterBLL headerFooterBll, IPBCK1BLL pbck1Bll, IZaidmExGoodTypeBLL goodTypeBll, IMonthBLL monthBll, IZaidmExNPPBKCBLL nppbkcbll, ILACK2BLL lack2Bll,
            IPlantBLL plantBll, ICompanyBLL companyBll, IPrintHistoryBLL printHistoryBll, IWorkflowBLL workflowBll, IWorkflowHistoryBLL workflowHistoryBll, ICK5BLL ck5Bll, IDocumentSequenceNumberBLL documentSequenceNumberBll, IZaidmExGoodTypeBLL exGroupBll)
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
            _workflowBll = workflowBll;
            _workflowHistoryBll = workflowHistoryBll;
            _printHistoryBll = printHistoryBll;
        }


        #region List by NPPBKC

        // GET: LACK2
        public ActionResult Index()
        {
            var model = new Lack2IndexViewModel();
            model = InitViewModel(model);

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;
            model.IsOpenDocList = true;
            var dbData = _lack2Bll.GetOpenDocument(CurrentUser);
            
            model.Details = dbData;
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
            return View("Index", model);
        }
        public ActionResult ListCompletedDoc()
        {
            var model = new Lack2IndexViewModel();
            model = InitViewModel(model);

            model.MainMenu = _mainMenu;
            model.CurrentMenu = PageInfo;

            var dbData = _lack2Bll.GetCompletedDocument();
            
            model.Details = dbData;
            model.IsShowNewButton = CurrentUser.UserRole != Enums.UserRole.Manager;
            model.PoaList = GlobalFunctions.GetPoaAll(_poabll);
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
            model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.SendingPlantDDL = GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, null);
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = GlobalFunctions.GetYearList(_ck5Bll);
            model.UsrRole = CurrentUser.UserRole;
            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LACK2CreateViewModel model)
        {
            Lack2Dto item = new Lack2Dto();

            item = AutoMapper.Mapper.Map<Lack2Dto>(model.Lack2Model);

            //validate if selection criteria exist
            var isExist = _lack2Bll.IsSelectionCriteriaExist(item);

            if (!isExist)
            {
                var plant = _plantBll.GetT001WById(model.Lack2Model.LevelPlantId);
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
                item.Lack2Number = _documentSequenceNumberBll.GenerateNumberNoReset(inputDoc);
                item.Items = model.Lack2Model.Items.Select(x => Mapper.Map<Lack2ItemDto>(x)).ToList();

                item.Status = Enums.DocumentStatus.Draft;



                _lack2Bll.Insert(item);
                AddMessageInfo("Create Success", Enums.MessageInfoType.Success);
                
            }
            else 
            {
                AddMessageInfo("A record with same parameter is already exist", Enums.MessageInfoType.Error);
               
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edits the LACK2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            var model = InitDetailModel(id);
            model.DocStatus = model.Lack2Model.Status;
            return View("Edit", model);
        }

        public LACK2CreateViewModel InitDetailModel(int? id)
        {
            LACK2CreateViewModel model = new LACK2CreateViewModel();

            model.Lack2Model = AutoMapper.Mapper.Map<LACK2Model>(_lack2Bll.GetByIdAndItem(id.Value));
            model.NPPBKCDDL = GlobalFunctions.GetAuthorizedNppbkc(CurrentUser.NppbckPlants);
            model.CompanyCodesDDL = GlobalFunctions.GetCompanyList(_companyBll);
            model.ExcisableGoodsTypeDDL = GlobalFunctions.GetGoodTypeList(_goodTypeBll);
            model.SendingPlantDDL = GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, null);
            model.MonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.YearList = GlobalFunctions.GetYearList(_ck5Bll);
            model.Lack2Model.StatusName = EnumHelper.GetDescription(model.Lack2Model.Status);
            model.UsrRole = CurrentUser.UserRole;

           
            
            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormNumber = model.Lack2Model.Lack2Number;
            workflowInput.DocumentStatus = model.Lack2Model.Status;
            workflowInput.NPPBKC_Id = model.Lack2Model.NppbkcId;
           
            var workflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(_workflowHistoryBll.GetByFormNumber(workflowInput));
            
            model.WorkflowHistory = workflowHistory;
            //validate approve and reject
            var input = new WorkflowAllowApproveAndRejectInput
            {
                DocumentStatus = model.Lack2Model.Status,
                FormView = Enums.FormViewType.Detail,
                UserRole = CurrentUser.UserRole,
                CreatedUser = model.Lack2Model.CreatedBy,
                CurrentUser = CurrentUser.USER_ID,
                CurrentUserGroup = CurrentUser.USER_GROUP_ID,
                DocumentNumber = model.Lack2Model.Lack2Number,
                NppbkcId = model.Lack2Model.NppbkcId
            };

            ////workflow
            var allowApproveAndReject = _workflowBll.AllowApproveAndReject(input);
            model.AllowApproveAndReject = allowApproveAndReject;
            model.AllowEditAndSubmit = CurrentUser.USER_ID == model.Lack2Model.CreatedBy;
            if (!allowApproveAndReject)
            {
                model.AllowGovApproveAndReject = _workflowBll.AllowGovApproveAndReject(input);
                model.AllowManagerReject = _workflowBll.AllowManagerReject(input);
            }
            if (model.Lack2Model.Status == Enums.DocumentStatus.Completed)
            {
                model.AllowPrintDocument = true;
            }
            return model;
        }
        [HttpPost]
        public ActionResult AddPrintHistory(int? id)
        {
            if (!id.HasValue)
                HttpNotFound();

            // ReSharper disable once PossibleInvalidOperationException
            var lack2  = _lack2Bll.GetById(id.Value);

            //add to print history
            var input = new PrintHistoryDto()
            {
                FORM_TYPE_ID = Enums.FormType.LACK2,
                FORM_ID = lack2.Lack2Id,
                FORM_NUMBER = lack2.Lack2Number,
                PRINT_DATE = DateTime.Now,
                PRINT_BY = CurrentUser.USER_ID
            };

            _printHistoryBll.AddPrintHistory(input);
            var model = new BaseModel();
            model.PrintHistoryList = Mapper.Map<List<PrintHistoryItemModel>>(_printHistoryBll.GetByFormNumber(lack2.Lack2Number));
            return PartialView("_PrintHistoryTable", model);

        }
        [HttpPost]
        public JsonResult RemoveDoc(int docid)
        {

            return Json(_lack2Bll.RemoveDoc(docid));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LACK2CreateViewModel model)
        {

            //if (model.IsSaveSubmit)
            //{
            //    return RedirectToAction("Submit", new {id = model.Lack2Model.Lack2Id});
            //}
             
              var item = AutoMapper.Mapper.Map<Lack2Dto>(model.Lack2Model);
            if (item.CreatedBy != CurrentUser.USER_ID)
            {
                return RedirectToAction("Detail", new {id = item.Lack2Id});
            }
            var exItems = new Lack2ItemDto[item.Items.Count];
              item.Items.CopyTo(exItems);
              item.Items = new List<Lack2ItemDto>();
              foreach (var items in exItems)
              {
                    if (items.Id == 0 )
                    {
                        item.Items.Add(items);
                    }
              }
           
            var plant = _plantBll.GetT001WById(model.Lack2Model.LevelPlantId);
            var company = _companyBll.GetById(model.Lack2Model.Burks);
            var goods = _exGroupBll.GetById(model.Lack2Model.ExGoodTyp);

            item.ExTypDesc = goods.EXT_TYP_DESC;

            item.Butxt = company.BUTXT;
            item.LevelPlantName = plant.NAME1;
            item.LevelPlantCity = plant.ORT01;
            item.PeriodMonth = model.Lack2Model.PeriodMonth;
            item.PeriodYear = model.Lack2Model.PeriodYear;

            item.ModifiedBy = CurrentUser.USER_ID;
            item.ModifiedDate = DateTime.Now;

            item.Status = Enums.DocumentStatus.Draft;

            
            if (item.GovStatus == Enums.DocumentStatusGov.PartialApproved)
            {
                item.Status = Enums.DocumentStatus.GovApproved;
            }
            if (item.GovStatus == Enums.DocumentStatusGov.FullApproved)
            {
                item.Status = Enums.DocumentStatus.Completed;
            }
            if (item.GovStatus == Enums.DocumentStatusGov.Rejected)
            {
                item.Status = Enums.DocumentStatus.GovRejected;
            }
            if (model.IsSaveSubmit)
            {
                if (item.Status == Enums.DocumentStatus.Draft)
                {
                    item.Status = Enums.DocumentStatus.WaitingForApproval;
                }

            }

            if (model.Documents != null)
            {
                item.Documents = new List<LACK2_DOCUMENT>();
                foreach (var sk in model.Documents)
                {
                    if (sk != null)
                    {
                        var document = new LACK2_DOCUMENT();
                        var filenamecheck = sk.FileName;
                        if (filenamecheck.Contains("\\"))
                        {
                            document.FILE_NAME = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                        }
                        else
                        {
                            document.FILE_NAME = sk.FileName;
                        }
                        document.LACK2_ID = item.Lack2Id;
                        document.FILE_PATH = SaveUploadedFile(sk, item.Lack2Number.Substring(0,10));
                        item.Documents.Add(document);
                        _lack2Bll.InsertDocument(document);
                    }
                }
            }


            _lack2Bll.Insert(item);
             AddMessageInfo("Update Success", Enums.MessageInfoType.Success);
            if (item.Status == Enums.DocumentStatus.Completed)
            {
                return RedirectToAction("ListCompletedDoc");
            }
            return RedirectToAction("Index");
        }

        #endregion
        private string SaveUploadedFile(HttpPostedFileBase file, string lack2Num)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

          
            sFileName = Constans.UploadPath + Path.GetFileName("LACK2_"+ lack2Num + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return sFileName;
        }

        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            //var urlBuilder =
            //      new System.UriBuilder(Request.Url.AbsoluteUri)
            //      {
            //          Path = Url.Action("Index", "LACK2"),
            //          Query = null,
            //      };

            //Uri uri = urlBuilder.Uri;
            //if (uri != Request.UrlReferrer)
            //    return HttpNotFound();
            var model = InitDetailModel(id);
            var periodMonth = _monthBll.GetMonth(Convert.ToInt32(model.Lack2Model.PeriodMonth));
            if (periodMonth != null)
                model.Lack2Model.PeriodMonthName = periodMonth.MONTH_NAME_IND;
            model.DocStatus = model.Lack2Model.Status;
            if (model.Lack2Model.Status == Enums.DocumentStatus.Completed)
            {
                var printHistory =
                    Mapper.Map<List<PrintHistoryItemModel>>(
                        _printHistoryBll.GetByFormNumber(model.Lack2Model.Lack2Number));
                model.PrintHistoryList = printHistory;
            }
            return View("Detail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(LACK2CreateViewModel model)
        {
            
            if (model.ActionType == "Approve")
            {
                return RedirectToAction("Approve",new { id = model.Lack2Model.Lack2Id});
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult Submit(int id)
        {
            var urlBuilder =
                    new System.UriBuilder(Request.Url.AbsoluteUri)
                    {
                        Path = Url.Action("Edit", "LACK2", new { id= id }),
                        Query = null ,
                    };

            Uri uri = urlBuilder.Uri;
            if (uri != Request.UrlReferrer)
                return HttpNotFound();
            var item = _lack2Bll.GetByIdAndItem(id);
            if (item.Status == Enums.DocumentStatus.Draft)
            {
                item.Status = Enums.DocumentStatus.WaitingForApproval;
            }
            else if (item.Status == Enums.DocumentStatus.Rejected)
            {
                item.Status = Enums.DocumentStatus.Draft;
            }

            item.Items = null;
            _lack2Bll.Insert(item);
            return RedirectToAction("Index");
        }

        public ActionResult Approve(int id)
        {
            var urlBuilder =
                   new System.UriBuilder(Request.Url.AbsoluteUri)
                   {
                       Path = Url.Action("Detail", "LACK2", new { id = id }),
                       Query = null,
                   };

            Uri uri = urlBuilder.Uri;
            if (uri != Request.UrlReferrer)
                return HttpNotFound();
            var item = _lack2Bll.GetByIdAndItem(id);
            if (item.Status == Enums.DocumentStatus.WaitingForApproval)
            {
                item.Status = Enums.DocumentStatus.WaitingForApprovalManager;
                item.ApprovedBy = CurrentUser.USER_ID;
                item.ApprovedDate = DateTime.Now;
            }
            else if (item.Status == Enums.DocumentStatus.WaitingForApprovalManager)
            {
                item.Status = Enums.DocumentStatus.WaitingGovApproval;
                item.ApprovedByManager = CurrentUser.USER_ID;
                item.ApprovedDateManager = DateTime.Now;
            }
           
            else if (item.Status == Enums.DocumentStatus.WaitingGovApproval)
            {
                item.Status = Enums.DocumentStatus.GovApproved;
            }
          
            item.Items = null;
            _lack2Bll.Insert(item);
            return RedirectToAction("Index");
        }

       

       

        




      
       
        
       
        

      
       

        [HttpPost]
        public PartialViewResult FilterOpenDocument(LACK2FilterViewModel SearchInput)
        {
            var input = Mapper.Map<Lack2GetByParamInput>(SearchInput);
            
            var dbData = _lack2Bll.GetDocumentByParam(input);
            var model = new Lack2IndexViewModel();
            model.Details = dbData;
            return PartialView("_Lack2OpenDoc", model);
        }

      
        public ActionResult RejectDocument(LACK2CreateViewModel model)
        {
            bool isSuccess = false;
            try
            {
                var item = _lack2Bll.GetByIdAndItem(model.Lack2Model.Lack2Id);
                item.Status = Enums.DocumentStatus.Rejected;
                item.IsRejected = true;
                item.Comment = model.Lack2Model.Comment;
                item.RejectedBy = CurrentUser.USER_ID;
                item.RejectedDate = DateTime.Now;
                item.Items = null;
                _lack2Bll.Insert(item);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            if (!isSuccess) return RedirectToAction("Detail", "Lack2", new { id = model.Lack2Model.Lack2Id });
            AddMessageInfo("Success Reject Document", Enums.MessageInfoType.Success);
            return RedirectToAction("Index");
        }
        


        [HttpPost]
        public JsonResult GetPlantByNppbkcId(string nppbkcid)
        {
            var data = Json(GlobalFunctions.GetAuthorizedPlant(CurrentUser.NppbckPlants, nppbkcid));
            return data;

        }
        [HttpPost]
        public JsonResult GetPoaByNppbkcId(string nppbkcid)
        {
            var data = _poabll.GetPoaByNppbkcId(nppbkcid);
            return Json(data.Distinct());

        }
        [HttpPost]
        public JsonResult GetCK5ByLack2Period(int month, int year, string sendPlantId, string goodstype)
        {
            var data =  _ck5Bll.GetByGIDate(month, year, sendPlantId, goodstype).Select(d=>Mapper.Map<CK5Dto>(d)).ToList();
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
        [HttpPost]
        public JsonResult GetNppbkcByCompanyId(string companyId)
        {
            return Json(_nppbkcbll.GetNppbkcsByCompany(companyId));
        }

        

        private DataSet CreateLack2Ds()
        {
            DataSet ds = new DataSet("dsLack2");

            DataTable dt = new DataTable("Lack2");

            // object of data row 
            DataRow drow;
            dt.Columns.Add("CompanyName", System.Type.GetType("System.String"));
            dt.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dt.Columns.Add("Alamat", System.Type.GetType("System.String"));
            dt.Columns.Add("Header", System.Type.GetType("System.Byte[]"));
            dt.Columns.Add("Footer", System.Type.GetType("System.String"));
            dt.Columns.Add("BKC", System.Type.GetType("System.String"));
            dt.Columns.Add("Period", System.Type.GetType("System.String"));
            dt.Columns.Add("City", System.Type.GetType("System.String"));
            dt.Columns.Add("CreatedDate", System.Type.GetType("System.String"));
            dt.Columns.Add("PoaPrintedName", System.Type.GetType("System.String"));
            dt.Columns.Add("Preview", System.Type.GetType("System.String"));
            dt.Columns.Add("DecreeDate", System.Type.GetType("System.String"));

            //detail
            DataTable dtDetail = new DataTable("Lack2Item");
            dtDetail.Columns.Add("Nomor", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Tanggal", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Jumlah", System.Type.GetType("System.String"));

            dtDetail.Columns.Add("NamaPerusahaan", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Nppbkc", System.Type.GetType("System.String"));
            dtDetail.Columns.Add("Alamat", System.Type.GetType("System.String"));

            ds.Tables.Add(dt);
            ds.Tables.Add(dtDetail);
            return ds;
        }

        [EncryptedParameter]
        public FileResult PrintPreview(int id)
        {
            var lack2 = _lack2Bll.GetByIdAndItem(id);

            var dsLack2 = CreateLack2Ds();
            var dt = dsLack2.Tables[0];
            DataRow drow;
            drow = dt.NewRow();
            drow[0] = lack2.Butxt;
            drow[1] = lack2.NppbkcId;
            drow[2] = lack2.LevelPlantName + ", " +lack2.LevelPlantCity;
            


            var headerFooter = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput
            {
                CompanyCode = lack2.Burks,
                FormTypeId = Enums.FormType.LACK2
            });
            if (headerFooter != null)
            {
                drow[3] = GetHeader(headerFooter.HEADER_IMAGE_PATH);
                drow[4] = headerFooter.FOOTER_CONTENT;
            }
            drow[5] = lack2.ExTypDesc;
            drow[6] = lack2.PeriodNameInd + " " + lack2.PeriodYear;
            drow[7] = lack2.LevelPlantCity;


            drow[8] = lack2.SubmissionDate == null ? null : string.Format("{0} {1} {2}", lack2.SubmissionDate.Day, _monthBll.GetMonth(lack2.SubmissionDate.Month).MONTH_NAME_IND, lack2.SubmissionDate.Year); 
            if (lack2.ApprovedBy != null)
            {
                var poa = _poabll.GetDetailsById(lack2.ApprovedBy);
                if (poa != null)
                {
                    drow[9] = poa.PRINTED_NAME;
                }
            }
            if (lack2.Status != Enums.DocumentStatus.Completed)
            {
                drow[10] = "PREVIEW LACK-2";
            }
            else
            {
                drow[10] = "LACK-2";
                if (lack2.DecreeDate != null)
                {
                    var lack2DecreeDate = lack2.DecreeDate.Value;
                    var lack2Month = _monthBll.GetMonth(lack2DecreeDate.Month).MONTH_NAME_IND;

                    drow[11] = string.Format("{0} {1} {2}", lack2DecreeDate.Day, lack2Month, lack2DecreeDate.Year);

                }
            }
            dt.Rows.Add(drow);



            var dtDetail = dsLack2.Tables[1];
            foreach (var item in lack2.Items)
            {
                DataRow drowDetail;
                drowDetail = dtDetail.NewRow();
                drowDetail[0] = item.Ck5Number;
                drowDetail[1] = item.Ck5GIDate;
                drowDetail[2] = item.Ck5ItemQty;
                drowDetail[3] = item.CompanyName;
                drowDetail[4] = item.CompanyNppbkc;
                drowDetail[5] = item.CompanyAddress;
                dtDetail.Rows.Add(drowDetail);

            }
            // object of data row 
           
            ReportClass rpt = new ReportClass();
            string report_path = ConfigurationManager.AppSettings["Report_Path"];
            rpt.FileName = report_path + "LACK2\\Preview.rpt";
            rpt.Load();
            rpt.SetDataSource(dsLack2);

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


        public ActionResult Summary()
        {
            var model = new Lack2SummaryReportModel();
            model.CompanyList = GlobalFunctions.GetCompanyList(_companyBll);
            model.NppbkcList = GlobalFunctions.GetNppbkcAll(_nppbkcbll);
            model.PlantList = GlobalFunctions.GetPlantAll();

            return View("Summary");
        }

        #region Summary Reports

        private SelectList GetLack2CompanyCodeList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.CompanyCode,
                        TextField = x.CompanyCode
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2NppbkcIdList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.NppbkcId,
                        TextField = x.NppbkcId
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2SendingPlantList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.Ck5SendingPlant,
                        TextField = x.Ck5SendingPlant
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2GoodTypeList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.TypeExcisableGoods,
                        TextField = x.TypeExcisableGoods + " - " + x.TypeExcisableGoodsDesc
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2PeriodYearList(List<Lack2SummaryReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.PeriodYear,
                        TextField = x.PeriodYear
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }
        

        private List<Lack2SummaryReportsItem> SearchDataSummaryReports(Lack2SearchSummaryReportsViewModel filter = null)
        {
            Lack2GetSummaryReportByParamInput input;
            List<Lack2SummaryReportDto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Lack2GetSummaryReportByParamInput();

                dbData = _lack2Bll.GetSummaryReportsByParam(input);
                return Mapper.Map<List<Lack2SummaryReportsItem>>(dbData);
            }

            //getbyparams

            input = Mapper.Map<Lack2GetSummaryReportByParamInput>(filter);

            dbData = _lack2Bll.GetSummaryReportsByParam(input);
            return Mapper.Map<List<Lack2SummaryReportsItem>>(dbData);
        }

        private Lack2SummaryReportsViewModel InitSummaryReports(Lack2SummaryReportsViewModel model)
        {
            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            var listLack2 = _lack2Bll.GetSummaryReportsByParam(new Lack2GetSummaryReportByParamInput());

            model.SearchView.CompanyCodeList = GetLack2CompanyCodeList(listLack2);
            model.SearchView.NppbkcIdList = GetLack2NppbkcIdList(listLack2);
            model.SearchView.SendingPlantIdList = GetLack2SendingPlantList(listLack2);
            model.SearchView.GoodTypeList = GetLack2GoodTypeList(listLack2);

            model.SearchView.PeriodMonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.SearchView.PeriodYearList = GetLack2PeriodYearList(listLack2);

            model.SearchView.CreatedByList = GlobalFunctions.GetCreatorList();
            model.SearchView.ApprovedByList = GlobalFunctions.GetPoaAll(_poabll);
            model.SearchView.CreatorList = GlobalFunctions.GetCreatorList();
            model.SearchView.ApproverList = GlobalFunctions.GetPoaAll(_poabll);

            var filter = new Lack2SearchSummaryReportsViewModel();

            model.DetailsList = SearchDataSummaryReports(filter);

            return model;
        }

        public ActionResult SummaryReports()
        {

            Lack2SummaryReportsViewModel model;
            try
            {

                model = new Lack2SummaryReportsViewModel();


                model = InitSummaryReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Lack2SummaryReportsViewModel();
                model.MainMenu = Enums.MenuList.LACK2;
                model.CurrentMenu = PageInfo;
            }

            return View("Lack2SummaryReport", model);
        }

        [HttpPost]
        public PartialViewResult SearchSummaryReports(Lack2SummaryReportsViewModel model)
        {
            model.DetailsList = SearchDataSummaryReports(model.SearchView);
            return PartialView("_Lack2ListSummaryReport", model);


        }

        public void ExportXlsSummaryReports(Lack2SummaryReportsViewModel model)
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

        private string CreateXlsSummaryReports(Lack2ExportSummaryReportsViewModel modelExport)
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
                if (modelExport.BLack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BDocumentType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.DocumentType);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCompanyCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyCode);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NppbkcId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCk5SendingPlant)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5SendingPlant);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BSendingPlantAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SendingPlantAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BLack2Period)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Period);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BLack2Date)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Date);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BTypeExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TypeExcisableGoodsDesc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BTotalDeliveryExcisable)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TotalDeliveryExcisable);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BUom)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Uom);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.BPoa)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Poa);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BPoaManager)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.PoaManager);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCreatedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCreatedTime)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedTime);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCreatedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatedBy);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BApprovedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ApprovedDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BApprovedTime)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ApprovedTime);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BApprovedBy)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ApprovedBy);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BLastChangedDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastChangedDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BLastChangedTime)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LastChangedTime);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Status);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BLegalizeData)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.LegalizeData);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, Lack2ExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.BLack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.BDocumentType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Document Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCompanyCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }
            if (modelExport.BNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC ID");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCk5SendingPlant)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Sending Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.BSendingPlantAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sending Plant Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.BLack2Period)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Period");
                iColumn = iColumn + 1;
            }

            if (modelExport.BLack2Date)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.BTypeExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }

            if (modelExport.BTotalDeliveryExcisable)
            {
                slDocument.SetCellValue(iRow, iColumn, "Total Delivered Excisable Goods (kg)");
                iColumn = iColumn + 1;
            }

            if (modelExport.BUom)
            {
                slDocument.SetCellValue(iRow, iColumn, "UOM");
                iColumn = iColumn + 1;
            }

            //start
            if (modelExport.BPoa)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA");
                iColumn = iColumn + 1;
            }
            if (modelExport.BPoaManager)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA  Manager");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCreatedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCreatedTime)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created Time");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCreatedBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Created By");
                iColumn = iColumn + 1;
            }
            if (modelExport.BApprovedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Approved Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BApprovedTime)
            {
                slDocument.SetCellValue(iRow, iColumn, "Approved Time");
                iColumn = iColumn + 1;
            }
            if (modelExport.BApprovedBy)
            {
                slDocument.SetCellValue(iRow, iColumn, "Approved By");
                iColumn = iColumn + 1;
            }
            if (modelExport.BLastChangedDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Change Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BLastChangedTime)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Change Time");
                iColumn = iColumn + 1;
            }
            if (modelExport.BStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.BLegalizeData)
            {
                slDocument.SetCellValue(iRow, iColumn, "Legalize Date");
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

            var fileName = "LACK2" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.CK5FolderPath), fileName);


            slDocument.SaveAs(path);

            return path;
        }

        #endregion

        #region Detail Reports

        private SelectList GetLack2CompanyCodeList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.CompanyCode,
                        TextField = x.CompanyCode
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2NppbkcIdList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.NppbkcId,
                        TextField = x.NppbkcId
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2SendingPlantList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.Ck5SendingPlant,
                        TextField = x.Ck5SendingPlant
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2GoodTypeList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.TypeExcisableGoods,
                        TextField = x.TypeExcisableGoods + " - " + x.TypeExcisableGoodsDesc
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetLack2PeriodYearList(List<Lack2DetailReportDto> listPbck2)
        {
            IEnumerable<SelectItemModel> query;

            query = from x in listPbck2
                    select new SelectItemModel()
                    {
                        ValueField = x.PeriodYear,
                        TextField = x.PeriodYear
                    };

            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");

        }

        private SelectList GetGiDateList(bool isFrom, List<Lack2DetailReportDto> listLack2)
        {

            IEnumerable<SelectItemModel> query;
            if (isFrom)
                query = from x in listLack2.Where(c => c.GiDate != null).OrderBy(c => c.GiDate)
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.GiDate,
                            TextField = x.GiDate.Value.ToString("dd MMM yyyy")
                        };
            else
                query = from x in listLack2.Where(c => c.GiDate != null).OrderByDescending(c => c.GiDate)
                        select new Models.SelectItemModel()
                        {
                            ValueField = x.GiDate,
                            TextField = x.GiDate.Value.ToString("dd MMM yyyy")
                        };

            return new SelectList(query.DistinctBy(c => c.TextField), "ValueField", "TextField");

        }
        private List<Lack2DetailReportsItem> SearchDataDetailReports(Lack2SearchDetailReportsViewModel filter = null)
        {
            Lack2GetDetailReportByParamInput input;
            List<Lack2DetailReportDto> dbData;
            if (filter == null)
            {
                //Get All
                input = new Lack2GetDetailReportByParamInput();

                dbData = _lack2Bll.GetDetailReportsByParam(input);
                return Mapper.Map<List<Lack2DetailReportsItem>>(dbData);
            }

            //getbyparams
            input = Mapper.Map<Lack2GetDetailReportByParamInput>(filter);

            dbData = _lack2Bll.GetDetailReportsByParam(input);
            return Mapper.Map<List<Lack2DetailReportsItem>>(dbData);
        }

        private Lack2DetailReportsViewModel InitDetailReports(Lack2DetailReportsViewModel model)
        {
            model.MainMenu = Enums.MenuList.LACK2;
            model.CurrentMenu = PageInfo;

            var listLack2 = _lack2Bll.GetDetailReportsByParam(new Lack2GetDetailReportByParamInput());

            model.SearchView.CompanyCodeList = GetLack2CompanyCodeList(listLack2);
            model.SearchView.NppbkcIdList = GetLack2NppbkcIdList(listLack2);
            model.SearchView.SendingPlantIdList = GetLack2SendingPlantList(listLack2);
            model.SearchView.GoodTypeList = GetLack2GoodTypeList(listLack2);

            model.SearchView.PeriodMonthList = GlobalFunctions.GetMonthList(_monthBll);
            model.SearchView.PeriodYearList = GetLack2PeriodYearList(listLack2);

            model.SearchView.DateFromList = GetGiDateList(true, listLack2);
            model.SearchView.DateToList = GetGiDateList(false, listLack2);

            var filter = new Lack2SearchDetailReportsViewModel();

            model.DetailsList = SearchDataDetailReports(filter);

            return model;
        }

        public ActionResult DetailReports()
        {

            Lack2DetailReportsViewModel model;
            try
            {

                model = new Lack2DetailReportsViewModel();


                model = InitDetailReports(model);

            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                model = new Lack2DetailReportsViewModel();
                model.MainMenu = Enums.MenuList.LACK2;
                model.CurrentMenu = PageInfo;
            }

            return View("Lack2DetailReport", model);
        }

        [HttpPost]
        public PartialViewResult SearchDetailReports(Lack2DetailReportsViewModel model)
        {
            model.DetailsList = SearchDataDetailReports(model.SearchView);
            return PartialView("_Lack2ListDetailReport", model);


        }

        public void ExportXlsDetailReports(Lack2DetailReportsViewModel model)
        {
            string pathFile = "";

            pathFile = CreateXlsDetailReports(model.ExportModel);


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

        private string CreateXlsDetailReports(Lack2ExportDetailReportsViewModel modelExport)
        {
            var dataSummaryReport = SearchDataDetailReports(modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcelDetail(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;
                if (modelExport.BLack2Number)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Lack2Number);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCk5GiDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5GiDate);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCk5RegistrationNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5RegistrationNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCk5RegistrationDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5RegistrationDate);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BCk5Total)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5Total);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BReceivingCompanyCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingCompanyCode);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BReceivingCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingCompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BReceivingNppbkc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingNppbkc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BReceivingAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ReceivingAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCk5SendingPlant)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Ck5SendingPlant);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BSendingPlantAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SendingPlantAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BCompanyCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyCode);
                    iColumn = iColumn + 1;
                }

                //start
                if (modelExport.BCompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BNppbkcId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.NppbkcId);
                    iColumn = iColumn + 1;
                }
                if (modelExport.BTypeExcisableGoods)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.TypeExcisableGoodsDesc);
                    iColumn = iColumn + 1;
                }
               
                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);

        }

        private SLDocument CreateHeaderExcelDetail(SLDocument slDocument, Lack2ExportDetailReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.BLack2Number)
            {
                slDocument.SetCellValue(iRow, iColumn, "LACK-2 Number");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCk5GiDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 GI Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCk5RegistrationNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Registration Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCk5RegistrationDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Registration Date");
                iColumn = iColumn + 1;
            }
            if (modelExport.BCk5Total)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Total");
                iColumn = iColumn + 1;
            }
            if (modelExport.BReceivingCompanyCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving Company Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.BReceivingCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving Company Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.BReceivingNppbkc)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.BReceivingAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Receiving Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCk5SendingPlant)
            {
                slDocument.SetCellValue(iRow, iColumn, "CK-5 Sending Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.BSendingPlantAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Sending Plant Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.BCompanyCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Code");
                iColumn = iColumn + 1;
            }

            //start
            if (modelExport.BCompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }
            if (modelExport.BNppbkcId)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC ID");
                iColumn = iColumn + 1;
            }
            if (modelExport.BTypeExcisableGoods)
            {
                slDocument.SetCellValue(iRow, iColumn, "Type of Excisable Goods");
                iColumn = iColumn + 1;
            }
           

            return slDocument;

        }


        #endregion

    }

}