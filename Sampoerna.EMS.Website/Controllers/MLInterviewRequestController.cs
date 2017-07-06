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
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
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
using Sampoerna.EMS.Website.Helpers;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.NPPBKC;
using Sampoerna.EMS.Website.Models.ManufacturingLicense;
using Sampoerna.EMS.Website.Models.SupportDoc;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.CustomService.Services.ManufactureLicense;
using Sampoerna.EMS.Website.Utility;
using Sampoerna.EMS.XMLReader;
using SpreadsheetLight;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Core;
using System.Net;
using System.Text;

namespace Sampoerna.EMS.Website.Controllers
{
    public class MLInterviewRequestController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        private InterviewRequestService IRservice;
        private InterviewRequestModel IRmodel;
        private InterviewRequestViewModel IRViewmodel;
        private IDocumentSequenceNumberBLL _docbll;
        private IChangesHistoryBLL chBLL;
        private IWorkflowHistoryBLL whBLL;


        public MLInterviewRequestController(IPageBLL pageBLL, IChangesHistoryBLL changeHistoryBLL, IWorkflowHistoryBLL workflowHistoryBLL,IDocumentSequenceNumberBLL docbll) : base(pageBLL, Enums.MenuList.ManufactureLicense)
        {
            this.mainMenu = Enums.MenuList.ManufactureLicense;

            IRmodel = new InterviewRequestModel();
            IRservice = new InterviewRequestService();
            refService = new SystemReferenceService();
            this.chBLL = changeHistoryBLL;
            this.whBLL = workflowHistoryBLL;
            _docbll = docbll;
        }

        public ActionResult Index()
        {
            try
            {                
                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer || CurrentUser.UserRole == Enums.UserRole.Controller )
                {
                    var users = refService.GetAllUser();
                    var documents = GetInterviewRequestList("", "", "", "", 0, false, false);
                    IRViewmodel = new InterviewRequestViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        Filter = new InterviewRequestFilterModel(),
                        CreatorList = GetUserList(users),
                        KppbcList = GetKppbcList(refService.GetAllNppbkc().OrderByDescending(o => o.KPPBC_ID)),
                        PoaList = GetPoaList(refService.GetAllPOA().OrderBy(o => o.PRINTED_NAME)),
                        CompanyType = GetCompanyTypeList(IRservice.GetInterviewReqCompanyTypeList()),
                        YearList = GetYearList(documents),
                        IsNotViewer = (CurrentUser.UserRole == Enums.UserRole.POA),
                        InterviewRequestDocuments = documents,
                        IsCompleted = false,
                        FromMenu = "Index",
                        CurrentUser = CurrentUser.USER_ID
                    };
                }
                else
                {                    
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return View(IRViewmodel);
        }

        public ActionResult CompletedDocument()
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    var users = refService.GetAllUser();
                    var documents = GetInterviewRequestList("", "", "", "", 0, true, false);
                    IRViewmodel = new InterviewRequestViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        Filter = new InterviewRequestFilterModel(),
                        CreatorList = GetUserList(users),
                        KppbcList = GetKppbcList(refService.GetAllNppbkc()),
                        PoaList = GetPoaList(refService.GetAllPOA()),
                        CompanyType = GetCompanyTypeList(IRservice.GetInterviewReqCompanyTypeList()),
                        YearList = GetYearList(documents),
                        IsNotViewer = (CurrentUser.UserRole == Enums.UserRole.POA),
                        InterviewRequestDocuments = documents,
                        IsCompleted = true,
                        FromMenu = "Completed"
                    };
                }
                else
                {
                    //AddMessageInfo("You dont have access to Manufacturing License Request page.", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return View("Index", IRViewmodel);
        }

        private List<InterviewRequestModel> GetInterviewRequestList(string KPPBCId, string Creator, string CompanyType, string POA, Int32 Year, bool IsCompleted, bool IsAllStatus)
        {
            try
            {
                var documents = new List<InterviewRequestModel>();
                var data = IRservice.GetAll();
                if (data.Any())
                {
                    List<string> delegatorname = new List<string>();
                    if (CurrentUser.UserRole == Enums.UserRole.POA)
                    {
                        var delegation = IRservice.GetPOADelegatedUser(CurrentUser.USER_ID);                        
                        if (delegation.Any())
                        {
                            delegatorname = delegation.Select(s => s.POA_FROM).ToList();
                        }
                        var IRWhithSameNPPBKC = IRservice.GetInterviewNeedApproveWithSameNPPBKC(CurrentUser.USER_ID);
                        var IRWhithoutNPPBKC = IRservice.GetInterviewNeedApproveWithoutNPPBKC(CurrentUser.USER_ID);
                        var IRWithNPPBKCButNoExcise = IRservice.GetInterviewNeedApproveWithNPPBKCButNoExcise(CurrentUser.USER_ID);
                        var drafstatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                        var editstatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                        data = data.Where(w => w.CREATED_BY == CurrentUser.USER_ID || delegatorname.Contains(w.CREATED_BY) 
                        || (w.LASTAPPROVED_BY == CurrentUser.USER_ID && w.LASTAPPROVED_STATUS != drafstatus && w.LASTAPPROVED_STATUS != editstatus) || IRWhithSameNPPBKC.Contains(w.VR_FORM_ID) || IRWhithoutNPPBKC.Contains(w.VR_FORM_ID) 
                        || IRWithNPPBKCButNoExcise.Contains(w.VR_FORM_ID));
                    }
                    if (!IsAllStatus)
                    {
                        if (data.Any() && IsCompleted)
                        {
                            data = data.Where(w => w.SYS_REFFERENCES.REFF_VALUE == "COMPLETED" || w.SYS_REFFERENCES.REFF_VALUE == "CANCELED");
                        }
                        else if (data.Any() && !IsCompleted)
                        {
                            data = data.Where(w => w.SYS_REFFERENCES.REFF_VALUE != "COMPLETED" && w.SYS_REFFERENCES.REFF_VALUE != "CANCELED");
                        }
                    }
                    if (data.Any())
                    {
                        documents = data.Select(s => new InterviewRequestModel
                        {
                            SubmissionDate = s.CREATED_DATE,
                            Id = s.VR_FORM_ID,
                            Company_Name = s.T001.BUTXT,
                            KPPBC_ID = s.KPPBC,
                            FormNumber = s.FORM_NUMBER,                            
                            RequestDate = s.REQUEST_DATE,
                            Perihal = s.PERIHAL,
                            Company_Type = s.COMPANY_TYPE,
                            ApprovalStatus = s.SYS_REFFERENCES.REFF_VALUE,
                            StatusKey = s.SYS_REFFERENCES.REFF_KEYS,
                            POAID = s.POA_ID,
                            POAName = s.POA.PRINTED_NAME,
                            POAAddress = s.POA.POA_ADDRESS,
                            POAPosition = s.POA.TITLE,
                            CreatedBy = s.CREATED_BY,
                            CreatedDate = s.CREATED_DATE,
                            KPPBC_Address = s.KPPBC_ADDRESS,
                            Npwp = s.T001.NPWP,
                            Company_Address = s.T001.SPRAS,
                            BAStatus = s.BA_STATUS,
                            BANumber = s.BA_NUMBER,
                            BADate = s.BA_DATE,
                            LastModifiedBy = s.LASTMODIFIED_BY,
                            LastApprovedBy = s.LASTAPPROVED_BY
                        }).OrderByDescending(o => o.CreatedDate).ToList();
                        if (KPPBCId != "" && KPPBCId != null && documents.Count() > 0)
                        {
                            documents = documents.Where(w => w.KPPBC_ID.Equals(KPPBCId)).ToList();
                        }
                        if (Creator != "" && Creator != null && documents.Count() > 0)
                        {
                            documents = documents.Where(w => w.CreatedBy.Equals(Creator)).ToList();
                        }
                        if (CompanyType != "" && CompanyType != null && documents.Count() > 0)
                        {
                            documents = documents.Where(w => w.Company_Type.Equals(CompanyType)).ToList();
                        }
                        if (POA != "" && POA != null && documents.Count() > 0)
                        {
                            documents = documents.Where(w => w.POAID.Equals(POA)).ToList();
                        }
                        if (Year != 0 && Year != null && documents.Count() > 0)
                        {
                            documents = documents.Where(w => w.CreatedDate.Year == Year).ToList();
                        }
                        foreach(var doc in documents)
                        {
                            doc.StrRequestDate = doc.RequestDate.ToString("dd MMM yyyy");
                            doc.IsCanEdit = (doc.CreatedBy == CurrentUser.USER_ID || delegatorname.Contains(doc.CreatedBy) || CurrentUser.UserRole == Enums.UserRole.Administrator);
                            doc.IsApprover = IsPOACanApprove(doc.Id, CurrentUser.USER_ID);
                        }
                    }
                }
                return documents;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return null;
            }
        }

        [HttpPost]
        public PartialViewResult GetInterviewRequestListTable(string KPPBCId, string Creator, string CompanyType, string POA, Int32 Year, bool IsCompleted)
        {
            var viewmodel = new InterviewRequestViewModel();
            var datalist = GetInterviewRequestList(KPPBCId, Creator, CompanyType, POA, Year, IsCompleted, false);
            viewmodel.InterviewRequestDocuments = datalist;
            return PartialView("_InterviewRequestListTable", viewmodel);
        }

        public ActionResult Create()
        {
            if (CurrentUser.UserRole != Enums.UserRole.POA)
            {
                AddMessageInfo("Can't create new Manufacturing License Interview Request Document for User with " + EnumHelper.GetDescription(CurrentUser.UserRole) + " Role", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
            else
            {
                IRmodel = new InterviewRequestModel();
                IRmodel.MainMenu = mainMenu;
                IRmodel.CurrentMenu = PageInfo;
                IRmodel.File_Size = GetMaxFileSize();
                IRmodel.Company_TypeList = GetCompanyTypeList(IRservice.GetInterviewReqCompanyTypeList());
                IRmodel.Perihal_List = GetPerihalList(IRservice.GetInterviewReqPerihalList());
                IRmodel.GovStatus_List = GetGovStatusList(IRservice.GetGovStatusList());
                
                var DataPOA = new POA();
                DataPOA = refService.GetPOA(CurrentUser.USER_ID);
                IRmodel.RequestDate = DateTime.Now;
                IRmodel.BADate = DateTime.Now;
                IRmodel.Status = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_VALUE;
                if (DataPOA != null)
                {
                    IRmodel.POAID = DataPOA.POA_ID;
                    IRmodel.POAName = DataPOA.PRINTED_NAME;
                    IRmodel.POAPosition = DataPOA.TITLE;
                    IRmodel.POAAddress = DataPOA.POA_ADDRESS;
                }
                IRmodel.City = "";
                IRmodel.City_Alias = "";
                IRmodel.StatusKey = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Draft);
                IRmodel.WorkflowHistory = new List<WorkflowHistoryViewModel>();
                IRmodel.Confirmation = GenerateConfirmDialog(true, false, false);
                IRmodel.IsCanEditPrintout = false;
                return View("Create", IRmodel);
            }
        }

        public ActionResult Detail(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(Id, CurrentUser.USER_ID) || IsPOACanApprove(Id, CurrentUser.USER_ID) || CurrentUser.UserRole == Enums.UserRole.Viewer || IsPOAfromSameNPPBKC(Id))
            {
                IRmodel = new InterviewRequestModel();
                IRmodel = GetInterviewRequest(Id, "Detail");
                var manufacturing = IRservice.GetManufacturingUsingThis(Id);
                if (manufacturing != null)
                {
                    IRmodel.ManufactureID = manufacturing.MNF_REQUEST_ID.ToString();
                    IRmodel.ManufactureNO = manufacturing.MNF_FORM_NUMBER;
                    IRmodel.ManufactureSTATUS = manufacturing.SYS_REFFERENCES.REFF_VALUE;
                }
                IRmodel.Confirmation = GenerateConfirmDialog(false, false, false);
                return View("Create", IRmodel);                
            }
            else
            {
                AddMessageInfo("You dont have access to view Interview Request detail.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(Id, CurrentUser.USER_ID))
            {
                IRmodel = new InterviewRequestModel();
                IRmodel = GetInterviewRequest(Id, "Edit");
                IRmodel.Confirmation = GenerateConfirmDialog(true, true, false);
                return View("Create", IRmodel);
            }
            else
            {
                AddMessageInfo("You dont have access to edit this Interview Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Approve(Int64 Id = 0)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanApprove(Id, CurrentUser.USER_ID))
            {
                IRmodel = new InterviewRequestModel();
                IRmodel = GetInterviewRequest(Id, "Approve");
                IRmodel.Confirmation = GenerateConfirmDialog(false, false, true);
                return View("Create", IRmodel);
            }
            else
            {
                AddMessageInfo("You dont have access to approve this Interview Request document.", Enums.MessageInfoType.Warning);
                return RedirectToAction("Index");
            }
        }

        public bool IsPOACanApprove(long IRId, string UserId)
        {
            var isOk = false;
            if (IRId != 0)
            {
                var approverlist = IRservice.GetPOAApproverList(IRId);
                if (approverlist.Count() > 0)
                {
                    var isexist = approverlist.Where(w => w.POA_ID.Equals(UserId)).ToList();
                    if (isexist.Count() > 0)
                    {
                        isOk = true;
                    }
                }
            }
            return isOk;
        }

        public bool IsPOACanAccess(long IRId, string UserId)
        {
            var isOk = true;
            var CreatorId = "";
            var IRequest = IRservice.GetInterviewRequestById(IRId).FirstOrDefault();
            if(IRequest != null)
            {
                CreatorId = IRequest.CREATED_BY;
            }
            if(CreatorId != "")
            {
                if(UserId != CreatorId)
                {
                    /////// Check delegation ///////
                    isOk = IsPOADelegate(UserId, CreatorId);
                }
            }
            return isOk;
        }

        private bool IsPOADelegate(string UserId, string CreatorId)
        {
            var isOk = false;
            var poadelegate = IRservice.GetPOADelegationOfUser(CreatorId);
            if (poadelegate != null)
            {
                if (UserId == poadelegate.POA_TO)
                {
                    isOk = true;
                }
            }
            return isOk;
        }

        private SelectList GetYearList(List<InterviewRequestModel> interviewrequests)
        {
            var query = from x in interviewrequests
                        select new SelectItemModel()
                        {
                            ValueField = x.SubmissionDate.Year,
                            TextField = x.SubmissionDate.Year.ToString()
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetCompanyTypeList(Dictionary<int, string> types)
        {
            var query = from x in types
                        select new SelectItemModel()
                        {
                            ValueField = x.Key,
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetPerihalList(Dictionary<int, string> types)
        {
            var query = from x in types
                        select new SelectItemModel()
                        {
                            ValueField = x.Value,
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetGovStatusList(Dictionary<int, string> status)
        {
            var query = from x in status
                        select new SelectItemModel()
                        {
                            ValueField = x.Key == 1 ? "True" : "False",
                            TextField = x.Value
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetPoaList(IEnumerable<CustomService.Data.POA> poaList)
        {
            var query = from x in poaList
                        select new SelectItemModel()
                        {
                            ValueField = x.POA_ID,
                            TextField = x.PRINTED_NAME
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetUserList(IEnumerable<CustomService.Data.USER> userList)
        {
            var query = from x in userList
                        select new SelectItemModel()
                        {
                            ValueField = x.USER_ID,
                            TextField = String.Format("{0} {1}", x.FIRST_NAME, x.LAST_NAME)
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetKppbcList(IEnumerable<CustomService.Data.MASTER_NPPBKC> nppbkcList)
        {
            var query = from x in nppbkcList
                        select new SelectItemModel()
                        {
                            ValueField = x.KPPBC_ID,
                            TextField = x.KPPBC_ID
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private List<CityModel> GetCityList()
        {
            try
            {
                var citylist = IRservice.GetCityList().Select(s => new CityModel
                {
                    Id = s.CITY_ID,
                    Name = s.CITY_NAME,
                    StateId = s.STATE_ID,
                    StateName = s.MASTER_STATE.STATE_NAME
                }).ToList();
                return citylist;
            }
            catch (Exception e)
            {
                return new List<CityModel>();
            }
        }

        private SelectList GetSelectlistCity(List<CityModel> city)
        {
            var query = from x in city
                        select new SelectItemModel()
                        {
                            ValueField = x.Id.ToString(),
                            TextField = x.Name
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private InterviewRequestModel GetInterviewRequestMasterForm(InterviewRequestModel _IRModel)
        {
            _IRModel = IRservice.GetInterviewRequestById(_IRModel.Id).Select(s => new InterviewRequestModel
            {
                Id = s.VR_FORM_ID,
                FormNumber = s.FORM_NUMBER,
                Status = s.SYS_REFFERENCES.REFF_VALUE,
                StatusKey = s.SYS_REFFERENCES.REFF_KEYS,
                RequestDate = s.REQUEST_DATE,
                NPPBKC_ID = s.NPPBKC_ID ?? "",
                POAID = s.POA_ID,
                POAName = s.POA.PRINTED_NAME,
                POAAddress = s.POA.POA_ADDRESS,
                POAPosition = s.POA.TITLE,
                KPPBC_ID = s.KPPBC,
                KPPBC_Address = s.KPPBC_ADDRESS ?? "",
                Npwp = s.T001.NPWP,
                Company_Address = s.T001.SPRAS,
                Company_ID = s.BUKRS,
                BADate = s.BA_DATE,
                BANumber = s.BA_NUMBER,
                BAStatus = s.BA_STATUS,
                Perihal = s.PERIHAL,
                Company_Type = s.COMPANY_TYPE,
                City = s.CITY,
                City_Alias = s.CITY_ALIAS,
                Text_To = s.TEXT_TO,
                Company_Name = s.T001.BUTXT,
                CreatedBy = s.CREATED_BY
            }).FirstOrDefault();
            return _IRModel;
        }

        private InterviewRequestModel GetInterviewRequest(long ID = 0, string Mode = "")
        {
            try
            {
                var _IRModel = new InterviewRequestModel();
                _IRModel.Id = ID;
                _IRModel = GetInterviewRequestMasterForm(_IRModel);
                _IRModel.interviewRequestDetail = GetInterviewRequestDetail(_IRModel.Id);
                _IRModel.City_List = GetCityList();
                //_IRModel.City_List_option = GetSelectlistCity(GetCityList());
                _IRModel.MainMenu = mainMenu;
                _IRModel.CurrentMenu = PageInfo;
                _IRModel.CompanyList = _GetCompanyList(_IRModel.NPPBKC_ID);
                _IRModel.Company_TypeList = GetCompanyTypeList(IRservice.GetInterviewReqCompanyTypeList());
                _IRModel.Perihal_List = GetPerihalList(IRservice.GetInterviewReqPerihalList());
                _IRModel.GovStatus_List = GetGovStatusList(IRservice.GetGovStatusList());
                
                var filesupload = IRservice.GetFileUploadByIRId(ID);
                if (filesupload != null)
                {
                    var Othersfileupload = filesupload.Where(w => w.DOCUMENT_ID == null && w.IS_GOVERNMENT_DOC == false);
                    _IRModel.InterviewRequestFileOtherList = Othersfileupload.Select(s => new InterviewRequestFileOtherModel
                    {
                        FileId = s.FILE_ID,
                        Path = s.PATH_URL,
                        FileName = s.FILE_NAME
                    }).ToList();
                    foreach(var file in _IRModel.InterviewRequestFileOtherList)
                    {
                        file.Name = GenerateFileName(file.Path);
                        file.Path = GenerateURL(file.Path);                        
                    }
                    var BAsfileupload = filesupload.Where(w => w.IS_GOVERNMENT_DOC == true);
                    _IRModel.File_BA_Path_Plus = BAsfileupload.Select(s => new InterviewRequestFileOtherModel
                    {
                        FileId = s.FILE_ID,
                        Path = s.PATH_URL,
                        FileName = s.FILE_NAME
                    }).ToList();
                    foreach (var file in _IRModel.File_BA_Path_Plus)
                    {
                        file.Name = GenerateFileName(file.Path);
                        file.Path = GenerateURL(file.Path);
                    }
                }
                _IRModel.Count_Lamp = filesupload.Count();
                if(Mode == "Edit")
                {
                    _IRModel.IsFormReadOnly = false;
                    _IRModel.IsDetail = false;
                }
                else if(Mode == "Detail")
                {
                    _IRModel.IsFormReadOnly = true;
                    _IRModel.IsDetail = true;
                }
                else if (Mode == "Approve")
                {
                    _IRModel.IsFormReadOnly = true;
                    _IRModel.IsDetail = false;                    
                }                
                _IRModel.ChangesHistoryList = new List<ChangesHistoryItemModel>();

                //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.InterviewRequest, ID).OrderBy(o => o.ACTION_DATE).ToList();
                var workflowInput = new GetByFormTypeAndFormIdInput();
                workflowInput.FormId = ID;
                workflowInput.FormType = Enums.FormType.InterviewRequestWorkflow;
                var workflow = this.whBLL.GetByFormTypeAndFormId(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID).ToList();

                _IRModel.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
                if (_IRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval)) || _IRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval)))
                {
                    var ApproverList = IRservice.GetPOAApproverList(_IRModel.Id).Select(s => s.POA_ID).ToList();
                    var approvername = "";
                    foreach (var approver in ApproverList)
                    {
                        if (approvername != "")
                        {
                            approvername += ", ";
                        }
                        approvername += approver;
                    }
                    var Action = "";
                    if (_IRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval)))
                    {
                        Action = "Waiting For POA Approval";
                    }
                    else
                    {
                        Action = "Waiting For SKEP Approval";
                    }
                    _IRModel.WorkflowHistory.Add(new WorkflowHistoryViewModel
                    {
                        ACTION = Action,                        
                        USERNAME = approvername
                    });
                }
                _IRModel.File_Size = GetMaxFileSize();
                _IRModel.IsCanEditPrintout = (IsPOACanApprove(ID, CurrentUser.USER_ID) || _IRModel.CreatedBy == CurrentUser.USER_ID);
                return _IRModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new InterviewRequestModel();
            }
        }
        
        public List<InterviewRequestDetailModel> GetInterviewRequestDetail(long IRId = 0)
        {
            try
            {
                var IRDetModel = new List<InterviewRequestDetailModel>();
                var data = IRservice.GetInterviewRequestDetailByIRId(IRId);
                if (data.Any())
                {
                    IRDetModel = data.Select(s => new InterviewRequestDetailModel
                    {
                        DetId = s.VR_FORM_DETAIL_ID,
                        Manufacture_Address = s.MANUFACTURE_ADDRESS,
                        City_Id = s.CITY_ID,
                        City_Name = s.MASTER_CITY.CITY_NAME,
                        Province_Id = s.PROVINCE_ID,
                        Province_Name = s.PROVINCE_ID == 0 ? "" : s.MASTER_STATE.STATE_NAME,
                        Sub_District = s.SUB_DISTRICT,
                        Village = s.VILLAGE,
                        Phone = s.PHONE,                        
                        Fax = s.FAX
                    }).ToList();
                }
                return IRDetModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<InterviewRequestDetailModel>();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(InterviewRequestModel model)
        {
            try
            {
                var maxFileSize = GetMaxFileSize();
                var isOkFileExt = true;
                var isOkFileSize = true;
                var supportingDocFile = new List<HttpPostedFileBase>();
                if (model.interviewRequestSupportingDoc != null)
                {
                    supportingDocFile = model.interviewRequestSupportingDoc.Select(s => s.File).ToList();
                }
                isOkFileExt = CheckFileExtension(supportingDocFile);
                if (isOkFileExt)
                {
                    isOkFileExt = CheckFileExtension(model.File_Other);
                    if (isOkFileExt)
                    {
                        isOkFileExt = CheckFileExtension(model.File_BA);
                    }
                }

                if (isOkFileExt)
                {
                    //isOkFileSize = CheckFileSize(supportingDocFile, maxFileSize);
                    if (isOkFileSize)
                    {
                        isOkFileSize = CheckFileSize(model.File_Other, maxFileSize);
                        if (isOkFileSize)
                        {
                            isOkFileSize = CheckFileSize(model.File_BA, maxFileSize);
                        }
                    }

                    if (isOkFileSize)
                    {
                        if (model.interviewRequestSupportingDoc != null)
                        {
                            foreach (var SuppDoc in model.interviewRequestSupportingDoc)
                            {
                                var PathFile = UploadFile(SuppDoc.File);
                                if (PathFile != "")
                                {
                                    SuppDoc.Path = PathFile;
                                }
                            }
                        }

                        if (model.File_Other != null)
                        {
                            var AddedfileOtherList = new List<string>();
                            var removedIndex = new List<int>();
                            var index = 0;
                            foreach (var FileOther in model.File_Other)
                            {
                                if (AddedfileOtherList.Contains(FileOther.FileName))
                                {
                                    removedIndex.Add(index);
                                }
                                else
                                {
                                    AddedfileOtherList.Add(FileOther.FileName);
                                    var PathFile = UploadFile(FileOther);
                                    if (PathFile != "")
                                    {
                                        model.File_Other_Path.Add(PathFile);
                                    }
                                }
                                index++;
                            }
                            removedIndex = removedIndex.OrderByDescending(o => o).ToList();
                            foreach (var i in removedIndex)
                            {
                                model.File_Other.RemoveAt(i);
                            }
                        }

                        //if (model.File_BA != null)
                        //{
                        //    foreach (var FileBA in model.File_BA)
                        //    {
                        //        var PathFile = UploadFile(FileBA);
                        //        if (PathFile != "")
                        //        {
                        //            model.File_BA_Path.Add(PathFile);
                        //        }
                        //    }
                        //}

                        //if (model.StatusKey == ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Draft) && model.Id != 0 && model.Id != null)
                        //{
                        //    model.Status = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_VALUE;
                        //}

                        //var ActionType = 0;
                        //if(model.Status.ToUpper().Contains("DRAFT"))
                        //{
                        //    ActionType = (int)Enums.ActionType.Modified;
                        //}
                        //else if(model.Status.ToUpper().Equals("WAITING FOR POA APPROVAL"))
                        //{
                        //    ActionType = (int)Enums.ActionType.Submit;
                        //}

                        long ApproveStats = 0;
                        var ActionType = 0;
                        if (model.Id != 0 && model.Id != null)
                        {
                            ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                            ActionType = (int)Enums.ActionType.Modified;
                        }
                        else
                        {
                            ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                            ActionType = (int)Enums.ActionType.Created;
                        }

                        //long ApproveStats = GetSysreffApprovalStatus(model.Status);                        
                        var InsertIRResult = new INTERVIEW_REQUEST();
                        if (model.Id == 0 || model.Id == null)
                        {
                            var docnumberinput = new GenerateDocNumberInput();
                            docnumberinput.FormType = Enums.FormType.InterviewRequest;
                            docnumberinput.Month = model.RequestDate.Month;
                            docnumberinput.Year = model.RequestDate.Year;
                            var formnumber = _docbll.GenerateNumber(docnumberinput);                            
                            formnumber = ChangeFormnumberCompanyCity(formnumber, model.Company_ID, model.City_Alias);
                            InsertIRResult = IRservice.InsertInterviewRequest(model.Perihal, model.Company_Type, CurrentUser.USER_ID, ApproveStats, model.NPPBKC_ID, model.Company_ID, model.POAID, model.KPPBC_ID, model.KPPBC_Address, model.City, model.City_Alias, model.Text_To, model.RequestDate, (int)CurrentUser.UserRole, formnumber);
                        }
                        else
                        {
                            InsertIRResult = IRservice.UpdateInterviewRequest(model.Id, model.Perihal, model.Company_Type, CurrentUser.USER_ID, ApproveStats, model.NPPBKC_ID, model.Company_ID, model.POAID, model.KPPBC_ID, model.KPPBC_Address, model.City, model.City_Alias, model.Text_To, model.RequestDate, ActionType, (int)CurrentUser.UserRole);
                        }

                        if (InsertIRResult != null)
                        {
                            model.Id = InsertIRResult.VR_FORM_ID;
                            model.CreatedBy = InsertIRResult.CREATED_BY;
                            if (model.Id != 0)
                            {
                                //// InActiving/Removing Doc
                                foreach (var removedfile in model.RemovedFilesId)
                                {
                                    if (removedfile != 0)
                                    {
                                        IRservice.DeleteFileUpload(removedfile, CurrentUser.USER_ID);
                                    }
                                }
                                //// Supporting Doc
                                InsertUploadSuppDocFile(model.interviewRequestSupportingDoc, model.Id);
                                //// Other Doc
                                InsertUploadCommonFile(model.File_Other_Path, model.Id, false, model.File_Other_Name);
                                //// BA Doc
                                //InsertUploadCommonFile(model.File_BA_Path, model.Id, true);

                                if (model.interviewRequestDetail != null)
                                {
                                    var TheDetail = GetInterviewRequestDetail(model.Id);
                                    var DeleteDetail = MapToInterviewDetail(TheDetail);
                                    var TheDeleteDetail = DeleteDetail.Where(w => model.RemovedDetailId.Contains(w.VR_FORM_DETAIL_ID)).ToList();                                    
                                    IRservice.DeleteInterviewRequestDetail(model.Id, CurrentUser.USER_ID, TheDeleteDetail);
                                    foreach (var IReqDetail in model.interviewRequestDetail)
                                    {
                                        if (IReqDetail.Manufacture_Address != null && IReqDetail.Manufacture_Address != "")
                                        {
                                            var combinePhone = IReqDetail.Phone_Area_Code + "-" + IReqDetail.Phone;
                                            var combineFax = IReqDetail.Fax_Area_Code + "-" + IReqDetail.Fax;
                                            var ReNewedDetail = DeleteDetail.Where(w => w.VR_FORM_DETAIL_ID == IReqDetail.DetId).FirstOrDefault();
                                            var InsertIRDetResult = IRservice.InsertInterviewRequestDetail(model.Id, IReqDetail.Manufacture_Address, IReqDetail.City_Id, IReqDetail.Province_Id, IReqDetail.Sub_District, IReqDetail.Village, combinePhone, combineFax, ReNewedDetail, CurrentUser.USER_ID);
                                        }
                                    }
                                }
                            }
                        }

                        var msgSuccess = "";
                        if (ActionType == (int)Enums.ActionType.Created)
                        {
                            msgSuccess = "Success create Interview Request";
                        }
                        else if (ActionType == (int)Enums.ActionType.Modified)
                        {
                            msgSuccess = "Success update Interview Request";
                        }
                        //else if(model.Status.ToUpper() == "WAITING FOR POA APPROVAL")
                        //{
                        //    msgSuccess = "Success submit Interview Request";
                        //    var poareceiverlistall = IRservice.GetPOAApproverList(model.Id);
                        //    if (poareceiverlistall.Count() > 0)
                        //    {
                        //        List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();
                        //        var strreqdate = model.RequestDate.ToString("dd MMMM yyyy");
                        //        var CreatorName = refService.GetPOA(model.CreatedBy).PRINTED_NAME;
                        //        var footer = "<tr>";
                        //        footer += "<td>&nbsp;Creator</td>";
                        //        footer += "<td>:</td>";
                        //        footer += "<td>&nbsp;" + CreatorName + "</td>";
                        //        footer += "</tr>";
                        //        var sendmail = SendMail("Visit Location &", "Visit Location &", model.FormNumber, "has already submitted", strreqdate, model.Perihal, model.KPPBC_ID, model.Company_ID, footer, model.Id, poareceiverList, "submit");
                        //        if (!sendmail)
                        //        {
                        //            msgSuccess += " , but failed send mail to POA Approver";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        msgSuccess += " , but failed send mail to POA Approver";
                        //    }
                        //}
                        AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddMessageInfo("Maximum file size is " + maxFileSize.ToString() + " Mb", Enums.MessageInfoType.Warning);
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    AddMessageInfo("Wrong File Extension", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        private string ChangeFormnumberCompanyCity(string formnumber,string company, string cityalias)
        {
            try
            {
                var companyalias = refService.GetCompanyById(company).BUTXT_ALIAS;
                var arr = formnumber.Split('/');
                if (arr.Count() == 5)
                {
                    formnumber = arr[0] + "/" + companyalias + "/" + cityalias + "/" + arr[3] + "/" + arr[4];
                }
                return formnumber;
            }
            catch (Exception)
            {
                return formnumber;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSkep(InterviewRequestModel model)
        {
            try
            {
                var maxFileSize = GetMaxFileSize();
                var isOkFileExt = true;
                var isOkFileSize = true;
                isOkFileExt = CheckFileExtension(model.File_BA);
                var msgSuccess = "Success submit Interview Request";
                if (isOkFileExt)
                {
                    isOkFileSize = CheckFileSize(model.File_BA, maxFileSize);
                    if(isOkFileSize)
                    {
                        if (model.File_BA != null)
                        {
                            var AddedfileBAList = new List<string>();
                            var removedIndex = new List<int>();
                            var index = 0;
                            foreach (var FileBA in model.File_BA)
                            {
                                if (AddedfileBAList.Contains(FileBA.FileName))
                                {
                                    removedIndex.Add(index);
                                }
                                else
                                {
                                    AddedfileBAList.Add(FileBA.FileName);
                                    var PathFile = UploadFile(FileBA);
                                    if (PathFile != "")
                                    {
                                        model.File_BA_Path.Add(PathFile);
                                    }
                                }
                                index++;
                            }
                            removedIndex = removedIndex.OrderByDescending(o => o).ToList();
                            foreach (var i in removedIndex)
                            {
                                model.File_BA.RemoveAt(i);
                            }
                        }
                        //long ApproveStats = GetSysreffApprovalStatus("WAITING FOR POA SKEP APPROVAL");
                        long ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval).REFF_ID;
                        if (model.StatusKey == ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval))
                        {
                            if (model.Id != 0 && model.Id != null)
                            {
                                //// InActiving/Removing Doc
                                foreach (var removedfile in model.RemovedFilesId)
                                {
                                    if (removedfile != 0)
                                    {
                                        IRservice.DeleteFileUpload(removedfile, CurrentUser.USER_ID);
                                    }
                                }
                                var ActionType = 0;
                                ActionType = (int)Enums.ActionType.Submit;
                                var updateSKEP = IRservice.UpdateBASKEP(model.Id, Convert.ToBoolean(model.BAStatus), model.BANumber, Convert.ToDateTime(model.BADate), ApproveStats, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, model.Comment);
                                if (updateSKEP != null)
                                {
                                    InsertUploadCommonFile(model.File_BA_Path, model.Id, true, model.File_Other_Name);
                                    var poareceiverlistall = IRservice.GetPOAApproverList(model.Id);
                                    if (poareceiverlistall.Count() > 0)
                                    {
                                        var govstatus = "Rejected";
                                        if(Convert.ToBoolean(model.BAStatus))
                                        {
                                            govstatus = "Approved";
                                        }
                                        List<string> poareceiverList = poareceiverlistall.Where(w => w.POA_EMAIL != "" && w.POA_EMAIL != null).Select(s => s.POA_EMAIL).ToList();
                                        var strreqdate = updateSKEP.REQUEST_DATE.ToString("dd MMMM yyyy");
                                        var CreatorName = refService.GetPOA(CurrentUser.USER_ID).PRINTED_NAME;
                                        var status_request = govstatus + " by Government and Waiting For POA SKEP Approval.";
                                        var footer = "<tr>";
                                        footer += "<td>&nbsp;BA No</td>";
                                        footer += "<td>:</td>";
                                        footer += "<td>&nbsp;" + model.BANumber + "</td>";
                                        footer += "</tr>";
                                        footer += "<tr>";
                                        footer += "<td>&nbsp;BA Date</td>";
                                        footer += "<td>:</td>";
                                        footer += "<td>&nbsp;" + Convert.ToDateTime(model.BADate).ToString("dd MMMM yyyy") + "</td>";
                                        footer += "</tr>";

                                        var sendmail = SendMail("BA", "BA", updateSKEP.FORM_NUMBER, status_request, strreqdate, updateSKEP.PERIHAL, updateSKEP.KPPBC, updateSKEP.BUKRS, footer, model.Id, poareceiverList, "submit");
                                        if (!sendmail)
                                        {
                                            msgSuccess += " , but failed send mail to POA Approver";
                                        }
                                    }
                                    else
                                    {
                                        msgSuccess += " , but failed send mail to POA Approver";
                                    }
                                }
                            }
                        }
                    }
                }
                AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public PartialViewResult FilterOpenDocument(InterviewRequestViewModel model)
        {
            var FilterCompanyType = "";
            if (model.Filter.CompanyType != null && model.Filter.CompanyType != "0" && model.Filter.CompanyType != "")
            {
                var key = Convert.ToInt32(model.Filter.CompanyType);
                var companyType = IRservice.GetInterviewReqCompanyTypeList();
                FilterCompanyType = companyType.FirstOrDefault(x => x.Key == key).Value;
            }
            model.InterviewRequestDocuments = GetInterviewRequestList(model.Filter.KPPBC, model.Filter.Creator, FilterCompanyType, model.Filter.POA, model.Filter.Year, model.IsCompleted, false);

            return PartialView("_InterviewRequestListTable", model);
        }

        private bool SendMail(string form_subject, string form_body, string form_number, string status_request, string request_date, string perihal, string kppbc, string company, string footer, long irid, List<string> sendto, string MailFor)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("form_subject", form_subject);
                parameters.Add("form_body", form_body);
                parameters.Add("number", form_number);
                parameters.Add("status_request", status_request);
                parameters.Add("date", request_date);
                parameters.Add("perihal", perihal);
                parameters.Add("kppbc", kppbc);
                var context = new EMSDataModel();
                var company_name = context.T001.Where(w => w.BUKRS == company).Select(s => s.BUTXT).FirstOrDefault();
                if(company_name == null)
                {
                    company_name = "";
                }
                parameters.Add("company", company_name);

                var IRDetail = GetInterviewRequestDetail(irid);
                var maildetail = "";
                var index = 0;
                foreach(var detail in IRDetail)
                {
                    index++;
                    maildetail += "<tr>";
                    maildetail += "<td colspan='3'>&nbsp;<b>Visit Location & Interview " + index + "</b></td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Manufacture Address</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.Manufacture_Address + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Province</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.Province_Name + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>City</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.City_Name + "</td>";
                    maildetail += "</tr>";
                }

                parameters.Add("detail", maildetail);
                parameters.Add("footer", footer);
                parameters.Add("url_detail", Url.Action("Detail", "MLInterviewRequest", new { Id = irid }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "MLInterviewRequest", new { Id = irid }, this.Request.Url.Scheme));
                parameters.Add("url_revise", Url.Action("Edit", "MLInterviewRequest", new { Id = irid }, this.Request.Url.Scheme));

                long mailcontentId = 0;
                if (MailFor == "submit")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseInterviewApprovalRequest;
                }
                else if (MailFor == "approve")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseInterviewApprovalNotification;
                }
                else if (MailFor == "revise")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseInterviewRevisionRequest;
                }
                else if (MailFor == "withdraw")
                {
                    mailcontentId = (int)ReferenceKeys.EmailContent.ManufacturingLicenseInterviewApprovalNotification;
                }

                var mailContent = refService.GetMailContent(mailcontentId, parameters);
                //var senderMail = refService.GetUserEmail(CurrentUser.USER_ID);
                var senderMail = ConfigurationManager.AppSettings["DefaultSender"];
                if (senderMail == null || senderMail == "")
                {
                    senderMail = "EMS@pmi.id";
                }
                var senderName = CurrentUser.FIRST_NAME + " " + CurrentUser.LAST_NAME;
                sendto = sendto.Where(w => w != "" && w != null).ToList();
                string[] arrSendto = sendto.ToArray();
                bool mailStatus = true;
                if (arrSendto.Count() > 0)
                {
                    mailStatus = ItpiMailer.Instance.SendEmail(arrSendto, null, null, null, mailContent.EMAILSUBJECT, mailContent.EMAILCONTENT, true, senderMail, senderName);
                }
                return mailStatus;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string UploadFile(HttpPostedFileBase File)
        {
            try
            {
                var filePath = "";                

                if (File != null && File.ContentLength > 0)
                {
                    var baseFolder = "/files_upload/Manufacture/InterviewRequest/Documents/";
                    var uploadToFolder = Server.MapPath("~" + baseFolder);
                    var date_now = DateTime.Now;
                    var date = String.Format("{0:ddMMyyyyHHmmss}", date_now);
                    var extension = Path.GetExtension(File.FileName);
                    var file_name = Path.GetFileNameWithoutExtension(File.FileName) + "=MLIR=" + CurrentUser.USER_ID + "-" + date + extension;
                    var filePathServer = Path.Combine(uploadToFolder, file_name);
                    filePath = Path.Combine(baseFolder, file_name);
                    Directory.CreateDirectory(uploadToFolder);
                    File.SaveAs(filePathServer);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return "Error";
            }
        }

        //public long GetSysreffApprovalStatus(string currStatus)
        //{
        //    long ApproveStats = 0;
        //    var Reff = refService.GetRefByType("APPROVAL_STATUS");
        //    if (Reff.Any())
        //    {
        //        currStatus = currStatus.ToUpper();
        //        if ( currStatus == "DRAFT NEW")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("DRAFT_NEW_STATUS")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //        else if (currStatus == "DRAFT EDIT")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("DRAFT_EDIT_STATUS")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //        else if(currStatus == "WAITING FOR POA APPROVAL")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("WAITING_POA_APPROVAL")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //        else if (currStatus == "WAITING FOR GOVERNMENT APPROVAL")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("WAITING_GOVERNMENT_APPROVAL")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //        else if (currStatus == "WAITING FOR POA SKEP APPROVAL")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("WAITING_POA_SKEP_APPROVAL")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //        else if (currStatus == "REJECTED")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("REJECTED")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //        else if (currStatus == "COMPLETED")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("COMPLETED")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //        else if (currStatus == "CANCELED")
        //        {
        //            ApproveStats = Reff.Where(w => w.REFF_KEYS.Equals("CANCELED")).Select(s => s.REFF_ID).FirstOrDefault();
        //        }
        //    }
        //    return ApproveStats;
        //}

        public List<NppbkcItemModel> GetNppbkcList()
        {
            var data = new List<NppbkcItemModel>();
            var theapp = refService.GetAllNppbkc();
            if (theapp.Any())
            {
                data = theapp.Select(s => new NppbkcItemModel
                {
                    NPPBKC_ID = s.NPPBKC_ID,
                    KPPBC_ID = s.KPPBC_ID,
                    TEXT_TO = s.TEXT_TO,
                    ADDR1 = s.KPPBC_ADDRESS,
                    CITY = s.CITY,
                    CITY_ALIAS = s.CITY_ALIAS,                    
                }).Distinct().ToList();
            }
            return data;
        }

        public ActionResult GetKPPBCList()
        {
            try
            {
                var UserNPPBKC = IRservice.GetUserNPPBKCList(CurrentUser.USER_ID);
                var data = GetNppbkcList().Where(w => UserNPPBKC.Contains(w.NPPBKC_ID));
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }

        private List<InterviewRequestCompanyModel> _GetCompanyList(string NppbkcId)
        {
            var theapp = IRservice.GetCompanyFromNPPBKC(NppbkcId).Distinct();
            var data = new List<InterviewRequestCompanyModel>();
            if (theapp.Any())
            {
                data = theapp.Select(s => new InterviewRequestCompanyModel
                {
                    Company_ID = s.BUKRS,
                    Company_Name = s.BUTXT,
                    Company_Address = s.SPRAS + " " + s.ORT01,
                    Npwp = s.NPWP
                }).ToList();
            }
            return data;
        }

        [HttpPost]
        public ActionResult GetCompanyList(String NppbkcId = "")
        {
            try
            {
                var data = _GetCompanyList(NppbkcId);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }

        [HttpPost]
        public PartialViewResult AddDetailManufactureForm(Int32 Index)
        {
            var irDetail = new InterviewRequestDetailModel();
            irDetail.DetId = 0;
            irDetail.Index = Index;
            //irDetail.City_List_option = GetSelectlistCity(GetCityList());
            irDetail.City_List = GetCityList();
            return PartialView("_DetailManufactureForm", irDetail);
        }

        [HttpPost]
        public ActionResult GetSupportingDocuments(string CompanyId, long IRId, bool IsReadonly, string StatusKey)
        {
            var formId = (long)Enums.FormList.Interview;
            var docs = refService.GetSupportingDocuments(formId, CompanyId);
            var model = docs.Select(x => MapSupportingDocumentModel(x)).ToList();
            if (IRId != 0 && IRId != null)
            {
                var Doclist = IRservice.GetFileUploadByIRId(IRId);
                if(Doclist != null)
                {
                    Doclist = Doclist.Where(w => w.DOCUMENT_ID != null);
                    if (Doclist != null)
                    {
                        var listAvaDocId = Doclist.Select(s => s.DOCUMENT_ID).ToList();
                        if (StatusKey != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Draft) && StatusKey != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Edited))
                        {
                            model = model.Where(w => listAvaDocId.Contains(w.Id)).ToList();
                        }
                        List<InterviewRequestSupportingDocModel> listDoc = Doclist.Select(s => new InterviewRequestSupportingDocModel
                        {
                            DocId = s.DOCUMENT_ID,
                            Path = s.PATH_URL,
                            FileUploadId = s.FILE_ID
                        }).ToList();
                        foreach (var doc in listDoc)
                        {
                            var whereModel = model.Where(w => w.Id.Equals(doc.DocId)).FirstOrDefault();
                            if (whereModel != null)
                            {
                                whereModel.Path = doc.Path;
                                whereModel.IsBrowseFileEnable = false;
                                whereModel.FileUploadId = doc.FileUploadId;
                                whereModel.FileName = GenerateFileName(whereModel.Path);
                                whereModel.Path = GenerateURL(whereModel.Path);
                            }
                        }
                    }
                }
            }
            foreach(var mod in model)
            {
                mod.IsReadonly = IsReadonly;
            }
            return PartialView("_SupportingDocuments", model);
        }

        public InterviewRequestSupportingDocModel MapSupportingDocumentModel(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new InterviewRequestSupportingDocModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    IsBrowseFileEnable = true,
                    Path = "",
                    FileUploadId = 0,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long GetMaxFileSize()
        {
            try
            {
                var size = refService.GetRefByType("UPLOAD_FILE_LIMIT").Select(s => s.REFF_VALUE == null ? 0 : Convert.ToInt64(s.REFF_VALUE)).FirstOrDefault();
                return size;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool CheckFileSize(List<HttpPostedFileBase> FileList, long MaxSize)
        {
            try
            {                
                var isOk = true;
                if (FileList != null)
                {
                    foreach (var File in FileList)
                    {
                        if (File != null)
                        {
                            long b = File.ContentLength;
                            long kb = b / 1024;
                            long mb = kb / 1024;
                            if (mb > MaxSize)
                            {
                                isOk = false;
                                break;
                            }
                        }
                    }
                }
                return isOk;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return false;
            }
        }

        public bool CheckFileExtension(List<HttpPostedFileBase> FileList)
        {
            try
            {
                var isOk = true;
                var extList = IRservice.GetFileExtList();
                if (FileList != null)
                {
                    foreach (var File in FileList)
                    {
                        if (File != null)
                        {
                            var ext = Path.GetExtension(File.FileName);
                            if (!extList.Contains(ext.ToLower()))
                            {
                                isOk = false;
                                break;
                            }
                        }
                    }
                }
                return isOk;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return false;
            }
        }

        public void InsertUploadSuppDocFile(IEnumerable<InterviewRequestSupportingDocModel> SuppDocList, long IRId)
        {
            try
            {
                if (SuppDocList != null)
                {
                    foreach (var Doc in SuppDocList)
                    {                        
                        if (Doc.Path != "" && Doc.Path != null)
                        {
                            var filename = IRservice.GetSupportingDocName(Doc.Id);
                            IRservice.InsertFileUpload(IRId, Doc.Path, CurrentUser.USER_ID, Doc.Id, false, filename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void InsertUploadCommonFile(List<string> FilePath, long IRId, bool IsGov, List<string> FileName)
        {
            try
            {
                if (FilePath != null)
                {
                    var filenamelist = new List<SelectListItem>();
                    if (FileName != null)
                    {
                        foreach (var name in FileName)
                        {
                            var file = name.Split('^');
                            if (file.Count() > 1)
                            {
                                filenamelist.Add(new SelectListItem
                                {
                                    Text = file[0],
                                    Value = file[1]
                                });
                            }
                        }
                    }
                    foreach (var Doc in FilePath)
                    {
                        if (Doc != null && Doc != "")
                        {
                            var DocName = Doc.Replace("/files_upload/Manufacture/InterviewRequest/Documents/", "");
                            var arrfileext = DocName.Split('.');
                            var countext = arrfileext.Count();
                            var fileext = "";
                            if (countext > 0)
                            {
                                fileext = arrfileext[countext - 1];
                            }
                            DocName = DocName.Replace("=MLIR=", "/");
                            var arrfilename = DocName.Split('/');
                            if (arrfilename.Count() > 0)
                            {
                                DocName = arrfilename[0] + "." + fileext;
                            }

                            var thefilename = filenamelist.Where(w => DocName.Contains(w.Text)).Select(s => s.Value).FirstOrDefault();
                            if(thefilename == null)
                            {
                                thefilename = "";
                            }
                            IRservice.InsertFileUpload(IRId, Doc, CurrentUser.USER_ID, 0, IsGov, thefilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ChangeStatus(long IRId, string Status, string Comment, string Action)
        {
            try
            {
                int ActionType = 0;
                long ApproveStats = 0;
                Action = Action.ToLower();
                if (Action == "approve")
                {
                    ActionType = (int)Enums.ActionType.Approve;
                    ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval).REFF_ID;
                }
                else if (Action == "approve_final")
                {
                    ActionType = (int)Enums.ActionType.Approve;
                    ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                }                
                else if (Action == "revise")
                {
                    ActionType = (int)Enums.ActionType.Revise;
                    ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                }
                else if (Action == "reviseskep")
                {
                    ActionType = (int)Enums.ActionType.Revise;
                    ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval).REFF_ID;
                }
                else if (Action == "cancel")
                {
                    ActionType = (int)Enums.ActionType.Cancel;
                    ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Canceled).REFF_ID;
                }
                else if (Action == "withdraw")
                {
                    ActionType = (int)Enums.ActionType.Withdraw;
                    ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                }
                else if (Action == "submit")
                {
                    ActionType = (int)Enums.ActionType.Submit;
                    ApproveStats = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval).REFF_ID;
                }
                string ErrMsg = "";
                //long ApproveStats = GetSysreffApprovalStatus(Status);
                var update = IRservice.UpdateStatus(IRId, ApproveStats, CurrentUser.USER_ID, ActionType, (int)CurrentUser.UserRole, Comment);
                if(update != null)
                {
                    var msgSuccess = "";
                    var strreqdate = update.REQUEST_DATE.ToString("dd MMMM yyyy");                    
                    var Creator = refService.GetPOA(update.CREATED_BY);
                    var CreatorName = Creator.PRINTED_NAME;
                    var CreatorMail = Creator.POA_EMAIL;
                    var ReceiverMail = "";
                    if (Action == "withdraw")
                    {
                        var LastApprover = refService.GetPOA(update.LASTAPPROVED_BY).POA_EMAIL;
                        ReceiverMail = LastApprover;
                    }
                    else
                    {
                        ReceiverMail = CreatorMail;
                    }
                    var Sendto = new List<string>();
                    Sendto.Add(ReceiverMail);
                    var sendmail = true;
                    if (Action == "approve" || Action == "approve_final")
                    {
                        msgSuccess = "Success approve Interview Request";
                        if (Action == "approve")
                        {
                            var footer = "<tr>";
                            footer += "<td>&nbsp;Creator</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + CreatorName + "</td>";
                            footer += "</tr>";
                            sendmail = SendMail("Visit Location", "Interview Request", update.FORM_NUMBER, "Waiting for Government Approval", strreqdate, update.PERIHAL, update.KPPBC, update.BUKRS, footer, IRId, Sendto, "approve");                            
                        }
                        else
                        {
                            var govstatus = "Rejected";
                            if (Convert.ToBoolean(update.BA_STATUS))
                            {
                                govstatus = "Approved";
                            }
                            var status_request = govstatus + " by Government and Completed";
                            var footer = "<tr>";
                            footer += "<td>&nbsp;BA No</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + update.BA_NUMBER + "</td>";
                            footer += "</tr>";
                            footer += "<tr>";
                            footer += "<td>&nbsp;BA Date</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + Convert.ToDateTime(update.BA_DATE).ToString("dd MMMM yyyy") + "</td>";
                            footer += "</tr>";
                            sendmail = SendMail("BA Location Visit", "SKEP Brand Registartion", update.FORM_NUMBER, status_request, strreqdate, update.PERIHAL, update.KPPBC, update.BUKRS, footer, IRId, Sendto, "approve");                            
                        }

                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Creator";
                        }
                    }
                    else if (Action == "revise" || Action == "reviseskep")
                    {
                        msgSuccess = "Success revise Interview Request";
                        if (Action == "revise")
                        {
                            var footer = "<tr>";
                            footer += "<td>&nbsp;Comment</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + Comment + "</td>";
                            footer += "</tr>";
                            sendmail = SendMail("Interview Request", "Interview Request", update.FORM_NUMBER, "Rejected to Revise", strreqdate, update.PERIHAL, update.KPPBC, update.BUKRS, footer, IRId, Sendto, "revise");
                        }
                        else
                        {
                            var footer = "<tr>";
                            footer += "<td>&nbsp;BA No</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + update.BA_NUMBER + "</td>";
                            footer += "</tr>";
                            footer += "<tr>";
                            footer += "<td>&nbsp;BA Date</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + Convert.ToDateTime(update.BA_DATE).ToString("dd MMMM yyyy") + "</td>";
                            footer += "</tr>";
                            footer += "<tr>";
                            footer += "<td>&nbsp;Comment</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + Comment + "</td>";
                            footer += "</tr>";
                            sendmail = SendMail("BA Interview Request", "BA Interview Request", update.FORM_NUMBER, "Rejected to Revise", strreqdate, update.PERIHAL, update.KPPBC, update.BUKRS, footer, IRId, Sendto, "revise");
                        }

                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Creator";
                        }
                    }
                    else if (Action == "submit")
                    {
                        msgSuccess = "Success submit Interview Request";
                        var poareceiverlistall = IRservice.GetPOAApproverList(IRId);
                        if (poareceiverlistall.Count() > 0)
                        {
                            List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();
                            var _strreqdate = update.REQUEST_DATE.ToString("dd MMMM yyyy");
                            var _CreatorName = refService.GetPOA(update.CREATED_BY).PRINTED_NAME;
                            var footer = "<tr>";
                            footer += "<td>&nbsp;Creator</td>";
                            footer += "<td>:</td>";
                            footer += "<td>&nbsp;" + CreatorName + "</td>";
                            footer += "</tr>";
                            sendmail = SendMail("Visit Location &", "Visit Location &", update.FORM_NUMBER, "has already submitted", strreqdate, update.PERIHAL, update.KPPBC, update.BUKRS, footer, IRId, poareceiverList, "submit");
                            if (!sendmail)
                            {
                                msgSuccess += " , but failed send mail to POA Approver";
                            }
                        }
                        else
                        {
                            msgSuccess += " , but failed send mail to POA Approver";
                        }
                    }
                    else if (Action == "withdraw")
                    {
                        msgSuccess = "Success withdraw Interview Request";
                        var footer = "<tr>";
                        footer += "<td>&nbsp;Creator</td>";
                        footer += "<td>:</td>";
                        footer += "<td>&nbsp;" + CreatorName + "</td>";
                        footer += "</tr>";
                        footer = "<tr>";
                        footer += "<td>&nbsp;Reason</td>";
                        footer += "<td>:</td>";
                        footer += "<td>&nbsp;" + Comment + "</td>";
                        footer += "</tr>";
                        sendmail = SendMail("Visit Location", "Visit Location & Interview Request", update.FORM_NUMBER, "Withdrawn", strreqdate, update.PERIHAL, update.KPPBC, update.BUKRS, footer, IRId, Sendto, "withdraw");

                        if (!sendmail)
                        {
                            msgSuccess += " , but failed send mail to Approver";
                        }
                    }
                    else
                    {
                        msgSuccess = "Success cancel Interview Request";
                    }

                    AddMessageInfo(msgSuccess, Enums.MessageInfoType.Success);
                }
                return Json(ErrMsg);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return Json(ex.Message);
            }
        }
        
        public bool CheckIsKppbcExist(string KPPBCId)
        {
            try
            {
                var dataexist = new List<NppbkcItemModel>();
                var nppbkc = GetNppbkcList();
                if(nppbkc != null)
                {
                    dataexist = nppbkc.Where(w => w.KPPBC_ID == KPPBCId).ToList();
                }
                if(dataexist.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        [HttpPost]
        public ActionResult AddNewKppbc(long IRId, string KPPBCId)
        {
            try
            {
                var kppbc = "";
                if (!CheckIsKppbcExist(KPPBCId))
                {
                    kppbc = IRservice.InsertKPPBCId(KPPBCId, CurrentUser.USER_ID);
                }
                if (kppbc == null)
                {
                    kppbc = "";
                }
                return Json(kppbc);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public PartialViewResult GetChangesLogList(long ID)
        {
            var changesmodel = new List<ChangesHistoryItemModel>();            
            var history = refService.GetChangesHistory((int)Enums.MenuList.InterviewRequest, ID.ToString()).ToList();
            changesmodel = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            return PartialView("_ChangesHistoryTable", changesmodel);
        }

        private List<ConfirmDialogModel> GenerateConfirmDialog(bool Submit, bool Cancel, bool Approve)
        {
            try
            {
                var listconfirmation = new List<ConfirmDialogModel>();

                if(Submit)
                {
                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnSubmit",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Submit"
                        },
                        CssClass = " submit-modal interviewrequest",
                        Message = "You are going to Submit Interview Request. Are you sure?",
                        Title = "Submit Confirmation",
                        ModalLabel = "SubmitModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnSubmitSkep",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Submit"
                        },
                        CssClass = " submitskep-modal interviewrequest",
                        Message = "You are going to Submit Interview Request. Are you sure?",
                        Title = "Submit Confirmation",
                        ModalLabel = "SubmitModalLabel"
                    });
                }
                if(Cancel)
                {
                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnCancel",
                            CssClass = "btn btn-danger btn_loader",
                            Label = "Cancel Document"
                        },
                        CssClass = " cancel-modal interviewrequest",
                        Message = "You are going to Cancel Interview Request. Are you sure?",
                        Title = "Cancel Document Confirmation",
                        ModalLabel = "CancelModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnWithdraw",
                            CssClass = "btn btn-danger btn_loader",
                            Label = "Withdraw"
                        },
                        CssClass = " withdraw-modal interviewrequest",
                        Message = "You are going to Withdraw Interview Request. Are you sure?",
                        Title = "Withdraw Document Confirmation",
                        ModalLabel = "WithdrawModalLabel"
                    });
                }
                if (Approve)
                {
                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnApprove",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Approve"
                        },
                        CssClass = " approve-modal interviewrequest",
                        Message = "You are going to approve Interview Request. Are you sure?",
                        Title = "Approve Confirmation",
                        ModalLabel = "ApproveModalLabel"
                    });

                    listconfirmation.Add(new ConfirmDialogModel()
                    {
                        Action = new ConfirmDialogModel.Button()
                        {
                            Id = "btnApproveFinal",
                            CssClass = "btn btn-success btn_loader",
                            Label = "Approve"
                        },
                        CssClass = " approvefinal-modal interviewrequest",
                        Message = "You are going to approve Interview Request. Are you sure?",
                        Title = "Approve Confirmation",
                        ModalLabel = "ApproveModalLabel"
                    });
                }

                //// FOR SET PRINTOUT TO DEFAULT CONFIRMATION ////                
                listconfirmation.Add(new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "btnRestorePrintoutToDefault",
                        CssClass = "btn btn-success btn_loader",
                        Label = "Restore To Default"
                    },
                    CssClass = " restoredefault-modal interviewrequest",
                    Message = "You are going to restore printout layout to default. Are you sure?",
                    Title = "Restore Printout Confirmation",
                    ModalLabel = "RestoreModalLabel"
                });
                //////////////////////////////////////////////////

                return listconfirmation;
            }
            catch (Exception e)
            {
                return new List<ConfirmDialogModel>();
            }
        }

        [HttpPost]
        public ActionResult GetPrintOutLayout(string Creator)
        {
            var result = refService.GetPrintoutLayout("INTERVIEW_REQUEST_PRINTOUT", Creator);
            var layout = "No Layout Found.";
            if(result.Any())
            {
                layout = result.FirstOrDefault().LAYOUT;
            }
            return Json(layout);
        }

        private List<string> GetPrintout(InterviewRequestModel _IRModel)
        {
            _IRModel = GetInterviewRequestMasterForm(_IRModel);
            _IRModel.interviewRequestDetail = GetInterviewRequestDetail(_IRModel.Id);
            var lampiran_count = IRservice.GetFileUploadByIRId(_IRModel.Id).Where(w => w.IS_GOVERNMENT_DOC == false).Count();
            var terbilang_lampiran_count = TerbilangLong(lampiran_count);
            var listLayout = new List<string>();
            var _companyType = _IRModel.Company_Type + " / <span style='text-decoration: line-through;'>Importir Hasil Tembakau</span>";
            if (_IRModel.Company_Type.ToLower() == "importir hasil tembakau")
            {
                _companyType = "<span style='text-decoration: line-through;'>Pabrik Hasil Tembakau</span> / " + _IRModel.Company_Type;
            }
            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("id-ID");
            foreach (var interview in _IRModel.interviewRequestDetail)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("COMPANY_NAME", _IRModel.Company_Name);
                parameters.Add("COMPANY_TYPE", _companyType);
                parameters.Add("COMPANY_ADDRESS", interview.Manufacture_Address);
                parameters.Add("COMPANY_CITY", interview.City_Name);
                parameters.Add("REQUEST_DATE", Convert.ToDateTime(_IRModel.RequestDate).ToString("dd MMMM yyyy", CI));
                parameters.Add("FORM_NUMBER", _IRModel.FormNumber);
                parameters.Add("LAMPIRAN_COUNT", lampiran_count.ToString() + " (" + terbilang_lampiran_count + ")");
                parameters.Add("PERIHAL", _IRModel.Perihal);
                parameters.Add("KPPBC_TEXT_TO", _IRModel.Text_To);
                parameters.Add("POA_NAME", _IRModel.POAName);
                parameters.Add("POA_ROLE", _IRModel.POAPosition);
                parameters.Add("POA_ADDRESS", _IRModel.POAAddress);
                var layout = refService.GeneratePrintout("INTERVIEW_REQUEST_PRINTOUT", parameters, _IRModel.CreatedBy).LAYOUT;
                listLayout.Add(layout);
            }
            return listLayout;
        }

        [HttpPost]
        public ActionResult RestorePrintoutToDefault(string Creator, long IRID)
        {
            var ErrMessage = refService.RestorePrintoutToDefault("INTERVIEW_REQUEST_PRINTOUT", Creator);
            if (ErrMessage == "")
            {
                PrintoutChangelog(IRID);
            }
            return Json(ErrMessage);
        }

        [HttpPost]
        public ActionResult GeneratePrintout(long InterviewID)
        {
            var _IRModel = new InterviewRequestModel();
            _IRModel.Id = InterviewID;
            var layout = GetPrintout(_IRModel);
            return Json(layout);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdatePrintOutLayout(string NewPrintout, string Creator, long IRID)
        {
            var ErrMessage = refService.UpdatePrintoutLayout("INTERVIEW_REQUEST_PRINTOUT", NewPrintout, Creator);
            if (ErrMessage == "")
            {
                PrintoutChangelog(IRID);
            }
            return Json(ErrMessage);
        }

        [HttpPost]        
        public void DownloadPrintOut(InterviewRequestModel _IRModel)
        {
            try
            {
                long InterviewID = _IRModel.Id;
                string FormNumber = _IRModel.FormNumber;
                FormNumber = FormNumber.Replace('/', '-');
                var now = DateTime.Now.ToString("ddMMyyyy");                
                _IRModel.Id = InterviewID;
                var listhtmlText = GetPrintout(_IRModel);
                //MemoryStream ms = new MemoryStream();
                var baseFolder = "/files_upload/Manufacture/InterviewRequest/PrintOut/";
                var uploadToFolder = Server.MapPath("~" + baseFolder);
                Directory.CreateDirectory(uploadToFolder);                
                var margin = Convert.ToSingle(System.Configuration.ConfigurationManager.AppSettings["DefaultMargin"]);
                var leftMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var rightMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var topMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);
                var bottomtMargin = iTextSharp.text.Utilities.MillimetersToPoints(25.4f);

                var _listPath = new List<string>();
                var index = 0;
                foreach (var htmlText in listhtmlText)
                {
                    var _path = uploadToFolder + "PrintOut_InterviewRequest_" + FormNumber + "_" + now + index.ToString() + ".pdf";
                    FileStream stream = new FileStream(_path, FileMode.Create);
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, leftMargin, rightMargin, topMargin, bottomtMargin);
                    var writer = PdfWriter.GetInstance(document, stream);
                    if (_IRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Draft)) || _IRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Edited)) || _IRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Canceled)) || _IRModel.StatusKey.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval)))
                    {
                        PdfWriterEvents writerEvent = new PdfWriterEvents(" D R A F T E D");
                        writer.PageEvent = writerEvent;
                    }
                    writer.CloseStream = false;
                    document.Open();
                    var srHtml = new StringReader(htmlText);
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, srHtml);
                    document.Close();
                    stream.Close();
                    _listPath.Add(_path);
                    index++;
                }
                
                var mergeFile = MergePrintout(_listPath, InterviewID);
                var newFile = new FileInfo(mergeFile);
                var fileName = Path.GetFileName(mergeFile);
                string attachment = string.Format("attachment; filename={0}", fileName);
                Response.Clear();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/pdf";
                Response.WriteFile(newFile.FullName);
                Response.Flush();
                newFile.Delete();
                Response.End();

                var changes = new Dictionary<string, string[]>();
                changes.Add("DOWNLOAD PRINT OUT", new string[] { "", "" });
                IRservice.LogsChages(InterviewID, changes, (int)Enums.MenuList.InterviewRequest, CurrentUser.USER_ID);

                //byte[] bytesInStream = ms.ToArray();
                //ms.Close();
                //Response.Clear();
                //Response.ContentType = "application/force-download";
                //Response.AddHeader("content-disposition", "attachment;filename=PrintOut_InterviewRequest_" + FormNumber + "_" + now + ".pdf");
                //Response.BinaryWrite(bytesInStream);
                //Response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String MergePrintout(List<string> path, long InterviewID)
        {
            try
            {
                var Interview = IRservice.GetInterviewRequestById(InterviewID);
                var bukrs = "";
                var ext = "";
                if (Interview.Any())
                {
                    bukrs = Interview.FirstOrDefault().BUKRS;
                }
                var supportingDocs = refService.GetSupportingDocuments((int)Enums.FormList.Interview, bukrs, InterviewID.ToString()).ToList();
                List<String> sourcePaths = new List<string>();
                var filePath = "";
                var i = 0;
                foreach (var realPath in path)
                {
                    if (i == 0)
                    {
                        filePath = realPath;
                    }
                    sourcePaths.Add(realPath);
                    i++;
                }
                foreach (var doc in supportingDocs)
                {
                    var files = doc.FILE_UPLOAD.ToList();
                    if (files.Count > 0)
                    {
                        ext = files[0].PATH_URL.Substring((files[0].PATH_URL.Length - 3), 3);
                        if (ext.ToLower() == "pdf")
                        {
                            sourcePaths.Add(Server.MapPath("~" + files[0].PATH_URL));
                        }
                    }
                }

                var otherDocs = refService.GetUploadedFiles((int)Enums.FormList.Interview, InterviewID.ToString()).Where(x => x.DOCUMENT_ID == null && x.IS_GOVERNMENT_DOC == false).ToList();
                foreach (var doc in otherDocs)
                {
                    ext = doc.PATH_URL.Substring((doc.PATH_URL.Length - 3), 3);
                    if (ext.ToLower() == "pdf")
                    {
                        sourcePaths.Add(Server.MapPath("~" + doc.PATH_URL));
                    }
                }

                if (PdfMerge.Execute(sourcePaths.ToArray(), filePath))
                    return filePath;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult SummaryReport()
        {
            var IRSummarymodel = new InterviewRequestSummaryReportsViewModel();
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    var users = refService.GetAllUser();
                    var model = new InterviewRequestExportSummaryReportsViewModel();                    
                    var documents = GetSummaryReportList("", "", "", "", 0, false, true, null);
                    IRSummarymodel = new InterviewRequestSummaryReportsViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        Filter = new InterviewRequestFilterModel(),
                        CreatorList = GetUserList(users),                        
                        KppbcList = GetSummaryReportKppbcList(documents),
                        PoaList = GetPoaList(refService.GetAllPOA()),
                        CompanyType = GetCompanyTypeList(IRservice.GetInterviewReqCompanyTypeList()),
                        YearList = GetSummaryReportYearList(documents),
                        IsNotViewer = (CurrentUser.UserRole == Enums.UserRole.POA),
                        InterviewRequestDocuments = documents
                    };
                }
                else
                {
                    //AddMessageInfo("You dont have access to Manufacturing License Request page.", Enums.MessageInfoType.Warning);
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
            }

            return View(IRSummarymodel);
        }

        [HttpPost]
        public PartialViewResult FilterSummaryReports(InterviewRequestSummaryReportsViewModel model)
        {
            var FilterCompanyType = "";
            if (model.Filter.CompanyType != null && model.Filter.CompanyType != "0" && model.Filter.CompanyType != "")
            {
                var key = Convert.ToInt32(model.Filter.CompanyType);
                var companyType = IRservice.GetInterviewReqCompanyTypeList();
                FilterCompanyType = companyType.FirstOrDefault(x => x.Key == key).Value;
            }
            model.InterviewRequestDocuments = GetSummaryReportList(model.Filter.KPPBC, model.Filter.Creator, FilterCompanyType, model.Filter.POA, model.Filter.Year, false, true, null);

            return PartialView("_InterviewRequestListTableSummaryReport", model);
        }

        public void ExportXlsSummaryReports(InterviewRequestSummaryReportsViewModel model)
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

        private string CreateXlsSummaryReports(InterviewRequestExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = GetSummaryReportList(modelExport.Filter.KPPBC, modelExport.Filter.Creator, modelExport.Filter.CompanyType, modelExport.Filter.POA, 0, false, true, modelExport);

            int iRow = 1;
            var slDocument = new SLDocument();

            //create header
            slDocument = CreateHeaderExcel(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;

                if (modelExport.FormNo)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.FormNumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.RequestDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.RequestDate.ToString("dd MMMM yyyy"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.Perihal)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Perihal);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CompanyType)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Company_Type);
                    iColumn = iColumn + 1;
                }

                if (modelExport.POAName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.POAName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.POAPosition)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.POAPosition);
                    iColumn = iColumn + 1;
                }

                if (modelExport.POAAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.POAAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.KPPBCId)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.KPPBC_ID);
                    iColumn = iColumn + 1;
                }

                if (modelExport.KPPBCAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.KPPBC_Address);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Company_Name);
                    iColumn = iColumn + 1;
                }

                if (modelExport.NPWP)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Npwp);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CompanyAddress)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Company_Address);
                    iColumn = iColumn + 1;
                }

                if (modelExport.GovStatus)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BAStatus);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BANumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.BANumber);
                    iColumn = iColumn + 1;
                }

                if (modelExport.BADate)
                {
                    slDocument.SetCellValue(iRow, iColumn, Convert.ToDateTime(data.BADate).ToString("dd MMMM yyyy"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Address)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.ManufactureAddress);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.City)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CityName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Province)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.StateName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.SubDistrict)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.SubDistrict);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Village)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Village);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Phone)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Phone);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Fax)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Fax);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);
        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, InterviewRequestExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.FormNo)
            {
                slDocument.SetCellValue(iRow, iColumn, "Form Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.RequestDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Request Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.Perihal)
            {
                slDocument.SetCellValue(iRow, iColumn, "Perihal");
                iColumn = iColumn + 1;
            }            

            if (modelExport.CompanyType)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Type");
                iColumn = iColumn + 1;
            }

            if (modelExport.POAName)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.POAPosition)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA Position");
                iColumn = iColumn + 1;
            }

            if (modelExport.POAAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "POA Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.KPPBCId)
            {
                slDocument.SetCellValue(iRow, iColumn, "KPPBC");
                iColumn = iColumn + 1;
            }

            if (modelExport.KPPBCAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "KPPBC Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.CompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.NPWP)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPWP");
                iColumn = iColumn + 1;
            }

            if (modelExport.CompanyAddress)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.GovStatus)
            {
                slDocument.SetCellValue(iRow, iColumn, "Goverentment Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.BANumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "BA Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.BADate)
            {
                slDocument.SetCellValue(iRow, iColumn, "BA Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Address)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture Address");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.City)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture City");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Province)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture Province");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.SubDistrict)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture Sub District");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Village)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture Village");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Phone)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture Phone");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Fax)
            {
                slDocument.SetCellValue(iRow, iColumn, "Manufacture Fax");
                iColumn = iColumn + 1;
            }

            return slDocument;

        }

        private string CreateXlsFileSummaryReports(SLDocument slDocument, int iColumn, int iRow)
        {
            SLStyle styleBorder = slDocument.CreateStyle();
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, iColumn - 1);
            slDocument.SetCellStyle(1, 1, iRow - 1, iColumn - 1, styleBorder);

            var fileName = "SummaryReport_InterviewRequest_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.MLFolderPath), fileName);
            slDocument.SaveAs(path);
            return path;
        }

        private string GenerateURL(string path)
        {            
            var url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + path;
            return url;
        }

        private string GenerateFileName(string path)
        {
            var filename = path.Replace("/files_upload/Manufacture/InterviewRequest/Documents/", "");
            var arrfileext = filename.Split('.');
            var countext = arrfileext.Count();
            var fileext = "";
            if (countext > 0)
            {
                fileext = arrfileext[countext - 1];
            }
            filename = filename.Replace("=MLIR=", "/");
            var arrfilename = filename.Split('/');
            if (arrfilename.Count() > 0)
            {
                filename = arrfilename[0] + "." + fileext;
            }
            return filename;
        }

        private List<vwMLInterviewRequestModel> GetSummaryReportList(string KPPBCId, string Creator, string CompanyType, string POA, Int32 Year, bool IsCompleted, bool IsAllStatus, InterviewRequestExportSummaryReportsViewModel modelExport)
        {
            try
            {
                var documents = new List<vwMLInterviewRequestModel>();
                var data = IRservice.GetvwInterviewRequestAll();
                if (data.Any())
                {
                    if ((modelExport != null) && (modelExport.DetailExportModel.Address || modelExport.DetailExportModel.City || modelExport.DetailExportModel.Province || modelExport.DetailExportModel.SubDistrict || modelExport.DetailExportModel.Village || modelExport.DetailExportModel.Phone || modelExport.DetailExportModel.Fax))
                    {
                        documents = data.Select(s => new vwMLInterviewRequestModel
                        {
                            RequestDate = s.CREATED_DATE,
                            FormId = s.VR_FORM_ID,
                            Company_Name = s.COMPANY_NAME,
                            FormNumber = s.FORM_NUMBER,
                            StrRequestDate = s.REQUEST_DATE.ToString(),
                            Perihal = s.PERIHAL,
                            Company_Type = s.COMPANY_TYPE,
                            ApprovalStatus = s.LASTAPPROVED_STATUS_VALUE,
                            POAID = s.POA_ID,
                            POAName = s.PRINTED_NAME,
                            POAAddress = s.POA_ADDRESS,
                            POAPosition = s.TITLE,
                            CreatedBy = s.CREATED_BY,
                            CreatedDate = s.CREATED_DATE,
                            KPPBC_ID = s.KPPBC,
                            KPPBC_Address = s.KPPBC_ADDRESS,
                            Npwp = s.NPWP,
                            Company_Address = s.COMPANY_ADDRESS,
                            BAStatus = s.BA_STATUS,
                            BANumber = s.BA_NUMBER,
                            BADate = s.BA_DATE,
                            NppbkcID = s.NPPBKC_ID,
                            ManufactureAddress = s.MANUFACTURE_ADDRESS,
                            CityName = s.CITY_NAME,
                            StateName = s.STATE_NAME,
                            SubDistrict = s.SUB_DISTRICT,
                            Village = s.VILLAGE,
                            Phone = s.PHONE,
                            Fax = s.FAX
                        }).ToList();
                    }
                    else
                    {
                        documents = data.Select(s => new vwMLInterviewRequestModel
                        {
                            RequestDate = s.CREATED_DATE,
                            FormId = s.VR_FORM_ID,
                            Company_Name = s.COMPANY_NAME,
                            FormNumber = s.FORM_NUMBER,
                            StrRequestDate = s.REQUEST_DATE.ToString(),
                            Perihal = s.PERIHAL,
                            Company_Type = s.COMPANY_TYPE,
                            ApprovalStatus = s.LASTAPPROVED_STATUS_VALUE,
                            POAID = s.POA_ID,
                            POAName = s.PRINTED_NAME,
                            POAAddress = s.POA_ADDRESS,
                            POAPosition = s.TITLE,
                            CreatedBy = s.CREATED_BY,
                            CreatedDate = s.CREATED_DATE,
                            KPPBC_ID = s.KPPBC,
                            KPPBC_Address = s.KPPBC_ADDRESS,
                            Npwp = s.NPWP,
                            Company_Address = s.COMPANY_ADDRESS,
                            BAStatus = s.BA_STATUS,
                            BANumber = s.BA_NUMBER,
                            BADate = s.BA_DATE,
                            NppbkcID = s.NPPBKC_ID
                        }).Distinct().ToList();

                    }
                }
                return documents;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return null;
            }
        }

        private SelectList GetSummaryReportYearList(List<vwMLInterviewRequestModel> interviewrequests)
        {
            var query = from x in interviewrequests
                        select new SelectItemModel()
                        {
                            ValueField = x.RequestDate.Year,
                            TextField = x.RequestDate.Year.ToString()
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetSummaryReportKppbcList(List<vwMLInterviewRequestModel> interviewrequests)
        {
            var query = from x in interviewrequests
                        select new SelectItemModel()
                        {
                            ValueField = x.KPPBC_ID,
                            TextField = x.KPPBC_ID
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private List<INTERVIEW_REQUEST_DETAIL> MapToInterviewDetail(List<InterviewRequestDetailModel> IReqDet)
        {
            var result = new List<INTERVIEW_REQUEST_DETAIL>();
            foreach (var detail in IReqDet)
            {
                result.Add(new INTERVIEW_REQUEST_DETAIL
                {
                    VR_FORM_DETAIL_ID = detail.DetId,
                    MANUFACTURE_ADDRESS = detail.Manufacture_Address,
                    CITY_ID = detail.City_Id,
                    PROVINCE_ID = detail.Province_Id,
                    SUB_DISTRICT = detail.Sub_District,
                    VILLAGE = detail.Village,
                    PHONE = detail.Phone_Area_Code + detail.Phone,
                    FAX = detail.Fax_Area_Code + detail.Fax
                });
            }
            return result;
        }

        public ActionResult ChangeLog(int ID, string Token)
        {
            var model = new InterviewRequestModel();

            //var history = refService.GetChangesHistory((int)Enums.MenuList.InterviewRequest, ID.ToString()).ToList();
            var history = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.InterviewRequest, ID.ToString());
            model.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);

            return PartialView("_ChangesHistoryTable", model);
        }

        private bool IsPOAfromSameNPPBKC(long IRID)
        {
            var yes = true;
            var list = IRservice.GetInterviewNeedApproveWithSameNPPBKC(CurrentUser.USER_ID).Where(w => w == IRID);
            if(list.Count() == 0 || list == null)
            {
                yes = false;
            }
            return yes;
        }

        private void PrintoutChangelog(long IRID)
        {
            Dictionary<string, string[]> changes = new Dictionary<string, string[]>();
            changes.Add("PRINTOUT LAYOUT", new string[] { "Layout Changes", "Layout Changes" });
            IRservice.LogsChages(IRID, changes, (int)Enums.MenuList.InterviewRequest, CurrentUser.USER_ID);
        }

        //[HttpPost]
        //public ActionResult GetCityDetailById(long ID)
        //{
        //    var citydetail = new CityModel();
        //    var citys = IRservice.GetCityList().Where(w => w.CITY_ID == ID);
        //    if (citys.Any())
        //    {
        //        var city = citys.FirstOrDefault();
        //        citydetail.StateName = city.MASTER_STATE.STATE_NAME;
        //        citydetail.StateId = city.STATE_ID;
        //    }
        //    return Json(citydetail);
        //}

        public string TerbilangLong(double amount)
        {
            string word = "";
            double divisor = 1000000000000.00; double large_amount = 0;
            double tiny_amount = 0;
            double dividen = 0; double dummy = 0;
            string weight1 = ""; string unit = ""; string follower = "";
            string[] prefix = { "SE", "DUA ", "TIGA ", "EMPAT ", "LIMA ",
 "ENAM ", "TUJUH ", "DELAPAN ", "SEMBILAN " };
            string[] sufix = { "SATU ", "DUA ", "TIGA ", "EMPAT ", "LIMA ",
 "ENAM ", "TUJUH ", "DELAPAN ", "SEMBILAN " };
            large_amount = Math.Abs(Math.Truncate(amount));
            tiny_amount = Math.Round((Math.Abs(amount) - large_amount) * 100);
            if (large_amount > divisor)
                return "OUT OF RANGE";
            while (divisor >= 1)
            {
                dividen = Math.Truncate(large_amount / divisor);
                large_amount = large_amount % divisor;
                unit = "";
                if (dividen > 0)
                {
                    if (divisor == 1000000000000.00)
                        unit = "TRILYUN ";
                    else
                    if (divisor == 1000000000.00)
                        unit = "MILYAR ";
                    else
                    if (divisor == 1000000.00)
                        unit = "JUTA ";
                    else
                    if (divisor == 1000.00)
                        unit = "RIBU ";
                }
                weight1 = "";
                dummy = dividen;
                if (dummy >= 100)
                    weight1 = prefix[(int)Math.Truncate(dummy / 100) - 1] + "RATUS ";
                dummy = dividen % 100;
                if (dummy < 10)
                {
                    if (dummy == 1 && unit == "RIBU ")
                        weight1 += "SE";
                    else
                    if (dummy > 0)
                        weight1 += sufix[(int)dummy - 1];
                }
                else
                if (dummy >= 11 && dummy <= 19)
                {
                    weight1 += prefix[(int)(dummy % 10) - 1] + "BELAS ";
                }
                else
                {
                    weight1 += prefix[(int)Math.Truncate(dummy / 10) - 1] + "PULUH ";
                    if (dummy % 10 > 0)
                        weight1 += sufix[(int)(dummy % 10) - 1];
                }
                word += weight1 + unit;
                divisor /= 1000.00;
            }
            if (Math.Truncate(amount) == 0)
                word = "NOL ";
            follower = "";
            if (tiny_amount < 10)
            {
                if (tiny_amount > 0)
                    follower = "KOMA NOL " + sufix[(int)tiny_amount - 1];
            }
            else
            {
                follower = "KOMA " + sufix[(int)Math.Truncate(tiny_amount / 10) - 1];
                if (tiny_amount % 10 > 0)
                    follower += sufix[(int)(tiny_amount % 10) - 1];
            }
            word += follower;
            //if (amount < 0)
            //{
            //    word = "MINUS " + word + " RUPIAH";
            //}
            //else
            //{
            //    word = word + " RUPIAH";
            //}
            return word.Trim();
        }
    }
}