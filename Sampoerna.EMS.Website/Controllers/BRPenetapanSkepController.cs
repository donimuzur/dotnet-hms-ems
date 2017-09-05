using Microsoft.Ajax.Utilities;
using AutoMapper;
using System.IO;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Services;
using Sampoerna.EMS.CustomService.Services.MasterData;
using Sampoerna.EMS.Website.Code;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.Market;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Services.BrandRegistrationTransaction;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP;
using Sampoerna.EMS.Website.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.GeneralModel;
using System.Net;
using System.Web;
using Sampoerna.EMS.Website.Utility;
using System.Globalization;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.FileUpload;
using Sampoerna.EMS.BusinessObject.Inputs;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Configuration;

namespace Sampoerna.EMS.Website.Controllers
{
    public class BRPenetapanSkepController : BaseController
    {
        private Enums.MenuList mainMenu;
        private SystemReferenceService refService;
        private ProductDevelopmentService productDevelopmentService;
        private BrandRegistrationService brandRegistrationService;
        private PenetapanSKEPService penetapanSKEPService;
        private NppbkcManagementService nppbkcService;
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBLL;
        private IDocumentSequenceNumberBLL _docbll;
        private IChangesHistoryBLL chBLL;
        private IWorkflowHistoryBLL whBLL;


        public BRPenetapanSkepController(IPageBLL pageBLL, IZaidmExNPPBKCBLL nppbkcbll, IChangesHistoryBLL changesHistoryBll, IWorkflowHistoryBLL workflowHistoryBLL, IDocumentSequenceNumberBLL docbll) : base(pageBLL, Enums.MenuList.BrandRegistrationTransaction)
        {
            this.mainMenu = Enums.MenuList.BrandRegistrationTransaction;
            this.refService = new SystemReferenceService();
            this.productDevelopmentService = new ProductDevelopmentService();
            this.brandRegistrationService = new BrandRegistrationService();
            this.penetapanSKEPService = new PenetapanSKEPService();
            this.nppbkcService = new NppbkcManagementService();
            this._nppbkcbll = nppbkcbll;
            this._changesHistoryBll = changesHistoryBll;
            this._workflowHistoryBLL = workflowHistoryBLL;
            this._docbll = docbll;
            this.chBLL = changesHistoryBll;
            this.whBLL = workflowHistoryBLL;

        }

        private ReceivedDecreeViewModel GeneratePropertiesSKEP(ReceivedDecreeViewModel source, bool update)
        {
            var usernppbkc = penetapanSKEPService.GetUserNPPBKC(CurrentUser.USER_ID).Select(s => s.NPPBKC_ID).ToList();
            var data = source;
            if (!update || data == null)
            {
                data = new ReceivedDecreeViewModel();
            }
            data.MainMenu = mainMenu;
            data.CurrentMenu = PageInfo;
            data.IsNotViewer = CurrentUser.UserRole != Enums.UserRole.Viewer;
            data.ShowActionOptions = data.IsNotViewer;
            data.EditMode = false;
            data.EnableFormInput = true;
            data.ViewModel.IsCreator = false;
            data.NppbkcList = GetNppbkcList(refService.GetAllNppbkc().Where(w => usernppbkc.Contains(w.NPPBKC_ID)));
            //data.BrandList = GetBrandist(penetapanSKEPService.getMasterBrand());
            data.ProductTypeList = GetProdTypeList(penetapanSKEPService.getMasterProductType());
            data.CompanyTierList = GetCompanyTierList(refService.GetRefByType("COMPANY_TIER"));
            data.ViewModel.Decree_Date = DateTime.Now;
            data.ViewModel.Decree_StartDate = DateTime.Now;
            data.File_Size = GetMaxFileSize();
            data.ViewModel.Received_No = "-";            

            return data;
        }

        public ActionResult GetSupportingDocumentsSKEP(long ID = 0, string nppbkc = "", bool isEnable = true)
        {
            var formId = (long)Enums.FormList.SKEP;
            var company = "";
            if (nppbkc != "" && nppbkc != null)
            {
                company = refService.GetNppbkc(nppbkc).COMPANY.BUKRS;
            }
            else
            {
                if (ID != 0)
                {
                    nppbkc = penetapanSKEPService.FindPenetapanSKEP(ID).NPPBKC_ID;
                    company = refService.GetNppbkc(nppbkc).COMPANY.BUKRS;
                }
            }
            var docs = refService.GetSupportingDocuments(formId, company);
            var model = docs.Select(x => MapSupportingDocumentModelSKEP(x)).ToList();            
            if (ID != 0)
            {
                var Doclist = penetapanSKEPService.GetFileUploadByReceiveID(ID);
                if (Doclist != null)
                {
                    Doclist = Doclist.Where(w => w.DOCUMENT_ID != null);
                    if (Doclist != null)
                    {
                        List<SKEPSupportingDocumentModel> listDoc = Doclist.Select(s => new SKEPSupportingDocumentModel
                        {
                            Id = s.DOCUMENT_ID ?? 0,
                            Path = s.PATH_URL,
                            FileUploadId = s.FILE_ID                            
                        }).ToList();
                        foreach (var doc in listDoc)
                        {
                            var whereModel = model.Where(w => w.Id.Equals(doc.Id)).FirstOrDefault();
                            if (whereModel != null)
                            {
                                whereModel.Path = doc.Path;                                
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
                mod.IsEnable = isEnable;
            }
            return PartialView("_SupportingDocument", model);
        }        

        private T001 GetCompanyFromNPPBKC(string nppbkcid)
        {
            var data = penetapanSKEPService.GetCompanyFromNPPBKC(nppbkcid).FirstOrDefault();
            return data;
        }
        
        private List<ReceivedDecreeModel> GetReceivedList(long SearchStatus = 0, string SearchCreator = "", string SearchNPPBKC = "", bool isComplete = false, bool isAll = true)
        {
            try
            {                
                if (SearchCreator == null)
                {
                    SearchCreator = "";
                }
                if (SearchNPPBKC == null)
                {
                    SearchNPPBKC = "";
                }
                var documents = new List<ReceivedDecreeModel>();
                var data = penetapanSKEPService.GetAllPenetapanSKEP();
                if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                {
                    var delegation = penetapanSKEPService.GetPOADelegatedUser(CurrentUser.USER_ID);
                    List<string> delegatorname = new List<string>();
                    if (delegation.Any())
                    {
                        delegatorname = delegation.Select(s => s.POA_FROM).ToList();
                    }
                    var SKEPWhithSameNPPBKC = penetapanSKEPService.GetSKEPNeedApproveWithSameNPPBKC(CurrentUser.USER_ID);
                    var SKEPWhithoutNPPBKC = penetapanSKEPService.GetSKEPNeedApproveWithoutNPPBKC(CurrentUser.USER_ID);
                    var WithNPPBKCButNoExcise = penetapanSKEPService.GetSKEPNeedApproveWithNPPBKCButNoExcise(CurrentUser.USER_ID);
                    var drafstatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                    var editstatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                    data = data.Where(w => w.CREATED_BY == CurrentUser.USER_ID || delegatorname.Contains(w.CREATED_BY)
                    || (w.LASTAPPROVED_BY == CurrentUser.USER_ID && w.LASTAPPROVED_STATUS != drafstatus && w.LASTAPPROVED_STATUS != editstatus) || SKEPWhithSameNPPBKC.Contains(w.RECEIVED_ID) || SKEPWhithoutNPPBKC.Contains(w.RECEIVED_ID) || WithNPPBKCButNoExcise.Contains(w.RECEIVED_ID));
                }
                if (data.Any())
                {
                    if (!isAll)
                    {
                        var stats_completed = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                        var stats_canceled = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Canceled).REFF_ID;
                        if (isComplete)
                        {
                            data = data.Where(w => w.LASTAPPROVED_STATUS == stats_completed || w.LASTAPPROVED_STATUS == stats_canceled);
                        }
                        else
                        {
                            data = data.Where(w => w.LASTAPPROVED_STATUS != stats_completed && w.LASTAPPROVED_STATUS != stats_canceled);
                        }
                    }

                    if (SearchStatus != 0)
                    {
                        data = data.Where(w => w.LASTAPPROVED_STATUS == SearchStatus);
                    }
                    if (SearchCreator != "")
                    {
                        data = data.Where(w => w.CREATED_BY == SearchCreator);
                    }
                    if (SearchNPPBKC != "")
                    {
                        data = data.Where(w => w.NPPBKC_ID == SearchNPPBKC);
                    }
                    
                    documents = data.Select(s => new ReceivedDecreeModel
                    {
                        Received_No = s.RECEIVED_NO,
                        Received_ID = s.RECEIVED_ID,
                        Nppbkc_ID = s.NPPBKC_ID,
                        Kppbc = s.ZAIDM_EX_NPPBKC.KPPBC_ID,
                        Decree_Date = s.DECREE_DATE,
                        Decree_No = s.DECREE_NO,
                        CompanyName = "",
                        AddressPlant = "",
                        Decree_StartDate = s.DECREE_STARTDATE,
                        StrLastApproved_Status = s.APPROVAL_STATUS.REFF_VALUE,
                        CreatorName = s.CREATOR.FIRST_NAME + " " + s.CREATOR.LAST_NAME,
                        Created_By = s.CREATED_BY,
                        LastModified_By = s.LASTMODIFIED_BY,
                        ApprovalStatusDescription = new ReferenceModel
                        {
                            Key = s.APPROVAL_STATUS.REFF_KEYS
                        }
                    }).ToList();
                    foreach (var doc in documents)
                    {
                        var theCompany = GetCompanyFromNPPBKC(doc.Nppbkc_ID);
                        doc.CompanyName = theCompany.BUTXT;
                        doc.AddressPlant = theCompany.SPRAS;
                        doc.strDecree_Date = Convert.ToDateTime(doc.Decree_Date).ToString("dd MMM yyyy");
                        doc.strDecree_StartDate = Convert.ToDateTime(doc.Decree_StartDate).ToString("dd MMM yyyy");
                    }
                }
                return documents;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult Index()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                var model = new ReceivedDecreeViewModel()
                {
                    MainMenu = mainMenu,
                    CurrentMenu = PageInfo,
                    IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                    ListReceivedDecree = GetReceivedList(0, "", "", false, false),
                    IsCompleted = false,
                    CurrentUser = CurrentUser.USER_ID
                };

                var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

                if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                {
                    var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                    nppbkc = new SelectList(filterNppbkc, "Value", "Text");
                }

                model.SearchInput.NppbkcIdList = nppbkc;
                model.SearchInput.StatusList = penetapanSKEPService.GetAllPenetapanSKEP().Select(s => new SelectListItem
                {
                    Text = s.APPROVAL_STATUS.REFF_VALUE,
                    Value = s.LASTAPPROVED_STATUS.ToString()
                }).Distinct().ToList();
                model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();

                return View("Index", model);
            }
            else
            {                
                return RedirectToAction("Unauthorized", "Error");
            }
        }

        public ActionResult CompletedDocument()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                var model = new ReceivedDecreeViewModel()
                {
                    MainMenu = mainMenu,
                    CurrentMenu = PageInfo,
                    IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                    ListReceivedDecree = GetReceivedList(0, "", "", true, false),
                    IsCompleted = true,
                    CurrentUser = CurrentUser.USER_ID
                };

                var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

                if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                {
                    var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                    nppbkc = new SelectList(filterNppbkc, "Value", "Text");
                }

                model.SearchInput.NppbkcIdList = nppbkc;
                model.SearchInput.StatusList = penetapanSKEPService.GetAllPenetapanSKEP().Select(s => new SelectListItem
                {
                    Text = s.APPROVAL_STATUS.REFF_VALUE,
                    Value = s.LASTAPPROVED_STATUS.ToString()
                }).Distinct().ToList();
                model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();

                return View("Index", model);
            }
            else
            {
                return RedirectToAction("Unauthorized", "Error");
            }
        }

        public ActionResult Create()
        {
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    AddMessageInfo("Operation not allowed", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                var data = GeneratePropertiesSKEP(null, false);
                data.ApproveConfirm = GenerateConfirmDialog("create");
                data.Action = "create";
                data.ViewModel.Received_No = "";
                var status = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft);
                data.ViewModel.ApprovalStatusDescription = new ReferenceModel
                {
                    Value = status.REFF_VALUE,
                    Key = status.REFF_KEYS,
                    Id = status.REFF_ID
                };
                data.EnableFormInput = true;
                data.ChangesHistoryList = new List<ChangesHistoryItemModel>();
                data.WorkflowHistory = new List<WorkflowHistoryViewModel>();

                return View(data);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Save(ReceivedDecreeViewModel model)
        {
            try
            {
                if (model.SKEPSupportingDocumnet != null)
                {
                    foreach (var SuppDoc in model.SKEPSupportingDocumnet)
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

                RECEIVED_DECREE entity = new RECEIVED_DECREE()
                {                    
                    CREATED_DATE = DateTime.Now,
                    CREATED_BY = CurrentUser.USER_ID,
                    RECEIVED_NO = model.ViewModel.Received_No,
                    NPPBKC_ID = model.ViewModel.Nppbkc_ID,
                    DECREE_NO = model.ViewModel.Decree_No,
                    DECREE_DATE = model.ViewModel.Decree_Date ?? DateTime.Now,
                    DECREE_STARTDATE = model.ViewModel.Decree_StartDate ?? DateTime.Now
                };
                long TheId = model.ViewModel.Received_ID;
                if (model.Action == "create")
                {                    
                    entity.LASTAPPROVED_STATUS = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                    //entity.RECEIVED_NO = penetapanSKEPService.GetFormNumber(entity.NPPBKC_ID);
                    var docinput = new GenerateDocNumberInput();
                    docinput.FormType = Enums.FormType.BrandRegistrationSKEP;
                    docinput.Month = entity.DECREE_DATE.Month;
                    docinput.Year = entity.DECREE_DATE.Year;
                    docinput.NppbkcId = entity.NPPBKC_ID;
                    entity.RECEIVED_NO = _docbll.GenerateNumber(docinput);
                    TheId = penetapanSKEPService.CreatePenetapanSKEP(entity, (int)Enums.MenuList.PenetapanSKEP, (int)Enums.ActionType.Created, (int)CurrentUser.UserRole, CurrentUser.USER_ID);
                }
                else if (model.Action == "update")
                {
                    entity.RECEIVED_ID = model.ViewModel.Received_ID;                    
                    entity.LASTAPPROVED_STATUS = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                    entity.LASTMODIFIED_BY = CurrentUser.USER_ID;
                    entity.LASTMODIFIED_DATE = DateTime.Now;
                    var result = penetapanSKEPService.UpdatePenetapanSKEP(entity, (int)Enums.ActionType.Modified, (int)CurrentUser.UserRole);
                }
                else if (model.Action == "submit")
                {
                    entity.RECEIVED_ID = model.ViewModel.Received_ID;
                    entity.LASTAPPROVED_STATUS = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval).REFF_ID;
                    entity.LASTMODIFIED_BY = CurrentUser.USER_ID;
                    entity.LASTMODIFIED_DATE = DateTime.Now;                    
                    var result = penetapanSKEPService.UpdatePenetapanSKEP(entity, (int)Enums.ActionType.Submit, (int)CurrentUser.UserRole);
                }
                if (TheId != 0)
                {                    
                    foreach (var item in model.Item)
                    {
                        RECEIVED_DECREE_DETAIL detail = new RECEIVED_DECREE_DETAIL();
                        if (item.PD_Detail_ID > -1)
                        {
                            detail = new RECEIVED_DECREE_DETAIL()
                            {
                                BRAND_CE = item.Brand_CE,
                                PROD_CODE = item.Prod_Code,
                                COMPANY_TIER = item.Company_Tier,
                                HJE = item.HJEperPack,
                                UNIT = item.Unit,
                                BRAND_CONTENT = item.Brand_Content,
                                TARIFF = item.Tariff,
                                PD_DETAIL_ID = item.PD_Detail_ID,
                                RECEIVED_ID = TheId
                            };                            
                            penetapanSKEPService.CreatePenetapanDetail(detail);
                        }
                        penetapanSKEPService.InsertDetailChangesLog(detail, item.Received_Detail_ID, (int)Enums.MenuList.PenetapanSKEP, CurrentUser.USER_ID);
                        if (item.Received_Detail_ID > 0)
                        {
                            penetapanSKEPService.DeleteReceivedDecreeDetailById(item.Received_Detail_ID);
                        }
                    }
                    //// InActiving/Removing Doc
                    foreach (var removedfile in model.RemovedFilesId)
                    {
                        if (removedfile != 0)
                        {
                            penetapanSKEPService.DeleteFileUpload(removedfile, CurrentUser.USER_ID);
                        }
                    }
                    //// Supporting Doc
                    InsertUploadSuppDocFile(model.SKEPSupportingDocumnet, TheId);
                    //// Other Doc
                    InsertUploadCommonFile(model.File_Other_Path, TheId, false, model.File_Other_Name);
                }
                AddMessageInfo("Successfully Save Penetapan SKEP Document!", Enums.MessageInfoType.Success);
                if (model.Action == "submit")
                {
                    var poareceiverlistall = penetapanSKEPService.GetPOAApproverList(TheId);
                    if (poareceiverlistall.Count() > 0)
                    {
                        List<string> poareceiverList = poareceiverlistall.Select(s => s.POA_EMAIL).ToList();
                        var sendmail = SendMail(entity.RECEIVED_NO, entity.DECREE_NO, Convert.ToDateTime(entity.DECREE_DATE).ToString("dd MMMM yyyy"), TheId, poareceiverList, "submit");
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddMessageInfo("Failed Save Penetapan SKEP Document!", Enums.MessageInfoType.Error);
                Console.WriteLine(ex.StackTrace);
                return RedirectToAction("Index");
            }
        }        
        
        public ActionResult Edit(long id)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanAccess(id, CurrentUser.USER_ID))
            {
                var data = GeneratePropertiesSKEP(null, false);
                var obj = penetapanSKEPService.FindPenetapanSKEP(id);
                var approvalStatusSubmitted = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_ID;
                var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;                
                data.ViewModel = Mapper.Map<ReceivedDecreeModel>(obj);
                data.ViewModel.IsCreator = true;
                data.ViewModel.IsSubmitted = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusSubmitted;
                data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;
                if (data.ViewModel.IsApproved)
                {
                    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);
                    RedirectToAction("Index");
                }
                data.EnableFormInput = true;
                data.EditMode = true;
                data.Item = GenerateDecreeDetail(id);
                var filesupload = penetapanSKEPService.GetFileUploadByReceiveID(id);
                var Othersfileupload = filesupload.Where(w => w.DOCUMENT_ID == null && w.IS_GOVERNMENT_DOC == false);
                data.SKEPFileOther = Othersfileupload.Select(s => new SKEPFileOtherModel
                {
                    FileId = s.FILE_ID,
                    Path = s.PATH_URL,
                    FileName = s.FILE_NAME
                }).ToList();
                foreach (var file in data.SKEPFileOther)
                {
                    file.Name = GenerateFileName(file.Path);
                    file.Path = GenerateURL(file.Path);
                }
                data.ApproveConfirm = GenerateConfirmDialog("update");
                data.Action = "update";
                data = GenerateLogs(data);

                return View("Create", data);
            }
            else
            {
                AddMessageInfo("Operation not allowed!", Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }        
                
        public ActionResult Detail(long id)
        {

            var data = GeneratePropertiesSKEP(null, false);
            var obj = penetapanSKEPService.FindPenetapanSKEP(Convert.ToInt64(id));
            data.ViewModel = Mapper.Map<ReceivedDecreeModel>(obj);            
            data.EnableFormInput = false;
            data.EditMode = false;
            data.Item = GenerateDecreeDetail(id);
            var filesupload = penetapanSKEPService.GetFileUploadByReceiveID(id);
            var Othersfileupload = filesupload.Where(w => w.DOCUMENT_ID == null && w.IS_GOVERNMENT_DOC == false);
            data.SKEPFileOther = Othersfileupload.Select(s => new SKEPFileOtherModel
            {
                FileId = s.FILE_ID,
                Path = s.PATH_URL,
                FileName = s.FILE_NAME
            }).ToList();
            foreach (var file in data.SKEPFileOther)
            {
                file.Name = GenerateFileName(file.Path);
                file.Path = GenerateURL(file.Path);
            }
            data = GenerateLogs(data);

            return View("Create", data);
        }
        
        [HttpPost]
        public ActionResult ChangeStatus(long ReceiveID, string Action, string Comment)
        {
            try
            {
                var ErrMessage = "";
                var SuccMessage = "";
                var statusApproval = ReferenceKeys.ApprovalStatus.Edited;
                var actionType = Enums.ActionType.Revise;
                if (Action == "approve")
                {
                    SuccMessage = "Success Approve Penetapan SKEP.";
                    statusApproval = ReferenceKeys.ApprovalStatus.Completed;
                    actionType = Enums.ActionType.Approve;
                }
                else if (Action == "revise")
                {
                    SuccMessage = "Success Reject to Revise Penetapan SKEP.";
                    statusApproval = ReferenceKeys.ApprovalStatus.Edited;
                    actionType = Enums.ActionType.Revise;                    
                }
                else if (Action == "cancel")
                {
                    SuccMessage = "Success Cancel Penetapan SKEP.";
                    statusApproval = ReferenceKeys.ApprovalStatus.Canceled;
                    actionType = Enums.ActionType.Cancel;
                }
                else if (Action == "submit")
                {
                    SuccMessage = "Success Submit Penetapan SKEP.";
                    statusApproval = ReferenceKeys.ApprovalStatus.AwaitingPoaApproval;
                    actionType = Enums.ActionType.Submit;
                }

                var updated = penetapanSKEPService.ChangeStatus(ReceiveID.ToString(), statusApproval, (int)Enums.MenuList.PenetapanSKEP, (int)actionType, (int)CurrentUser.UserRole, CurrentUser.USER_ID, Comment);

                if (updated != null)
                {
                    if (Action == "approve")
                    {
                        penetapanSKEPService.InsertToBrand(ReceiveID, updated.NPPBKC_ID);
                    }
                    var Creator = refService.GetPOA(updated.CREATED_BY);
                    var CreatorMail = Creator.POA_EMAIL;
                    var Sendto = new List<string>();
                    Sendto.Add(CreatorMail);
                    var sendmail = SendMail(updated.RECEIVED_NO, updated.DECREE_NO, Convert.ToDateTime(updated.DECREE_DATE).ToString("dd MMMM yyyy"), ReceiveID, Sendto, Action);
                    AddMessageInfo(SuccMessage, Enums.MessageInfoType.Success);
                }
                return Json(ErrMessage);
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return Json(ex.Message);
            }
        }

        public ActionResult Approve(long id)
        {
            try
            {
                var data = GeneratePropertiesSKEP(null, false);
                var obj = penetapanSKEPService.FindPenetapanSKEP(id);
                data.ViewModel = Mapper.Map<ReceivedDecreeModel>(obj);                
                data.EnableFormInput = false;
                data.EditMode = true;
                //data.IsAdminApprover = refService.IsAdminApprover(CurrentUser.USER_ID);
                data.IsAdminApprover = true;

                var approvalStatusApproved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;

                data.ViewModel.IsCreator = true;
                data.ViewModel.IsApproved = data.ViewModel.ApprovalStatusDescription.Id == approvalStatusApproved;
                if (data.ViewModel.IsApproved)
                {
                    AddMessageInfo("Operation not allowed!. This entry already approved!", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
                else if (CurrentUser.UserRole == Enums.UserRole.Administrator || IsPOACanApprove(id, CurrentUser.USER_ID))
                {
                    data.Item = GenerateDecreeDetail(id);
                    var filesupload = penetapanSKEPService.GetFileUploadByReceiveID(id);
                    var Othersfileupload = filesupload.Where(w => w.DOCUMENT_ID == null && w.IS_GOVERNMENT_DOC == false);
                    data.SKEPFileOther = Othersfileupload.Select(s => new SKEPFileOtherModel
                    {
                        FileId = s.FILE_ID,
                        Path = s.PATH_URL,
                        FileName = s.FILE_NAME
                    }).ToList();
                    foreach (var file in data.SKEPFileOther)
                    {
                        file.Name = GenerateFileName(file.Path);
                        file.Path = GenerateURL(file.Path);
                    }
                    data.ApproveConfirm = GenerateConfirmDialog("approve");
                    data.Action = "revise";
                    data = GenerateLogs(data);
                    return View("Create", data);
                }
                else
                {
                    AddMessageInfo("Operation not allowed!", Enums.MessageInfoType.Error);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo("Approval Failed : " + ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }                

        List<WorkflowHistoryViewModel> GetWorkflowHistory(long id)
        {

            var workflowInput = new GetByFormTypeAndFormIdInput();
            workflowInput.FormId = id;
            workflowInput.FormType = Enums.FormType.ProductDevelopment;
            var workflow = this._workflowHistoryBLL.GetByFormTypeAndFormId(workflowInput).OrderByDescending(item => item.ACTION_DATE);

            return Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);

        }
        public List<ProductDevDetailModel> GetProductDevelopmentDetail(long PD_ID)
        {
            try
            {
                var PDDetModel = new List<ProductDevDetailModel>();
                var data = productDevelopmentService.GetProductDetailByProductID(PD_ID);
                if (data.Any())
                {
                    PDDetModel = data.Select(s => new ProductDevDetailModel
                    {
                        Fa_Code_Old = s.FA_CODE_OLD,
                        Fa_Code_New = s.FA_CODE_NEW,
                        Hl_Code = s.HL_CODE,
                        Market_Id = s.MARKET_ID,
                        Fa_Code_Old_Desc= s.FA_CODE_OLD_DESCR,
                        Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                        Werks= s.WERKS,
                        Is_Import= s.IS_IMPORT,
                        Request_No= s.REQUEST_NO,
                        Bukrs = s.BUKRS
                        
                     
                    }).ToList();
                }
                return PDDetModel;
            }
            catch (Exception e)
            {
                AddMessageInfo(e.Message, Enums.MessageInfoType.Error);
                return new List<ProductDevDetailModel>();
            }
        }

        private SelectList GetPoaList(IEnumerable<CustomService.Data.POA> poaList)
        {
            var query = from x in poaList
                        select new SelectItemModel()
                        {
                            ValueField = x.POA_ID,
                            TextField = String.Format("{0} {1}", x.USER_LOGIN.FIRST_NAME, x.USER_LOGIN.LAST_NAME)
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        [HttpPost]
        public JsonResult GetNppbkc(string id)
        {
            try
            {
                var nppbkc = refService.GetNppbkc(id);
                var mapped = MapNppbkcModel(nppbkc);
                var serialized = JsonConvert.SerializeObject(mapped);
                var obj = new JObject
                {
                    new JProperty("Success", true),
                    new JProperty("Data", JObject.Parse(serialized))
                };
                var objStr = obj.ToString();
                return Json(objStr);

            }
            catch (Exception ex)
            {
                return Json(new JObject()
                {
                    new JProperty("Success", false),
                    new JProperty("Message", ex.Message)
                });
            }

        }

        [HttpPost]
        public JsonResult GetPlant(string nppbkcId)
        {
            try
            {
                var plant = brandRegistrationService.FindPlantByNppbkcID(nppbkcId);
                var mapped = MapPlantModel(plant);
                var serialized = JsonConvert.SerializeObject(mapped);
                var obj = new JObject
                {
                    new JProperty("Success", true),
                    new JProperty("Data", JObject.Parse(serialized))
                };
                var objStr = obj.ToString();
                return Json(objStr);

            }
            catch (Exception ex)
            {
                return Json(new JObject()
                {
                    new JProperty("Success", false),
                    new JProperty("Message", ex.Message)
                });
            }

        }

        private SelectList GetNppbkcList(IEnumerable<CustomService.Data.MASTER_NPPBKC> nppbkcList)
        {
            var query = from x in nppbkcList
                        select new SelectItemModel()
                        {
                            ValueField = x.NPPBKC_ID,
                            TextField = x.NPPBKC_ID
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetBrandist(IQueryable<CustomService.Data.ZAIDM_EX_BRAND> brandList)
        {
            var query = from x in brandList
                        select new SelectItemModel()
                        {
                            ValueField = x.BRAND_CE,
                            TextField = x.BRAND_CE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetProdTypeList(IQueryable<CustomService.Data.MASTER_PRODUCT_TYPE> prodtypeList)
        {
            var query = from x in prodtypeList
                        select new SelectItemModel()
                        {
                            ValueField = x.PROD_CODE,
                            TextField = x.PRODUCT_TYPE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        private SelectList GetCompanyTierList(IQueryable<CustomService.Data.SYS_REFFERENCES> CompanyTierList)
        {
            var query = from x in CompanyTierList
                        select new SelectItemModel()
                        {
                            ValueField = x.REFF_ID,
                            TextField = x.REFF_VALUE
                        };
            return new SelectList(query.DistinctBy(c => c.ValueField), "ValueField", "TextField");
        }

        [HttpPost]
        public JsonResult GetLastRecordReceivedDecree()
        {
            var temp = penetapanSKEPService.GetLastRecordPenetapanSKEP();

            Int64 result;
            if (temp != null)
            {
                if (temp.RECEIVED_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.RECEIVED_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetLastRecordBR()
        {
            var temp = brandRegistrationService.GetLastRecordBrandReq();

            Int64 result;
            if (temp != null)
            {
                if (temp.REGISTRATION_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.REGISTRATION_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetLastRecordPD()
        {
            var temp = productDevelopmentService.GetLastRecordPD();

            Int64 result;
            if (temp != null)
            {
                if (temp.PD_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.PD_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetLastRecordItem()
        {
            var temp = productDevelopmentService.GetLastRecordPDDetail();       
            
            Int64 result;
            if (temp != null)
            {
                if (temp.PD_DETAIL_ID == 0)
                {
                    result = 0;
                }
                else
                {
                    result = temp.PD_DETAIL_ID;
                }
            }
            else
            {
                result = 0;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetSupportDoc(long formId, string bukrs)
        {
            var tempList = productDevelopmentService.FindSupportDetail(formId, bukrs);
            var selectList = from s in tempList
                             select new SelectListItem
                             {
                                 Value = s.DOCUMENT_ID.ToString(),
                                 Text = s.SUPPORTING_DOCUMENT_NAME
                             };
            var nameList = new SelectList(selectList.GroupBy(p => p.Value).Select(g => g.First()), "Value", "Text");

            return Json(nameList);
        }

        [HttpPost]
        public JsonResult GetPlantByCompanyNonImport(string bukrs)
        {         
            var data = productDevelopmentService.FindPlantNonImport(bukrs);
            return Json(new SelectList(data, "NPPBKC_ID", "NAME1"));
        }

        [HttpPost]
        public JsonResult GetPlantByCompanyImport(string bukrs)
        {
            var data = productDevelopmentService.FindPlantImport(bukrs);
            return Json(new SelectList(data, "NPPBKC_IMPORT_ID", "NAME1"));
        }

        [HttpPost]
        public JsonResult GetCodeDescription(string code)
        {            
            var tempValue = productDevelopmentService.FindItemDescription(code);
            string result = "";

            if (tempValue != null)
            {
                if (tempValue.MATERIAL_DESC == null)
                {
                    result = "";
                }
                else
                {
                    result = tempValue.MATERIAL_DESC.ToString();
                }
            }
            else
            {
                result = "";
            }
            return Json(result);
        }              
                
        public SKEPSupportingDocumentModel MapSupportingDocumentModelSKEP(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new SKEPSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,                    
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

        public BrandRegSupportingDocumentModel MapSupportingDocumentModelBrand(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new BrandRegSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
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

        public ProductDevSupportingDocumentModel MapSupportingDocumentModelProduct(CustomService.Data.MASTER_SUPPORTING_DOCUMENT entity)
        {
            try
            {
                return new ProductDevSupportingDocumentModel()
                {
                    Id = entity.DOCUMENT_ID,
                    Name = entity.SUPPORTING_DOCUMENT_NAME,
                    Company = new CompanyModel()
                    {
                        Id = entity.COMPANY.BUKRS,
                        Name = entity.COMPANY.BUTXT
                    },
                    FileList = AutoMapper.Mapper.Map<List<FileUploadModel>>(entity.FILE_UPLOAD).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PlantGeneralModel MapPlantModel(CustomService.Data.MASTER_PLANT plant)
        {
            try
            {
                return new PlantGeneralModel()
                {
                    IdPlant = plant.WERKS,
                    Name = plant.NAME1,
                    Address = plant.ADDRESS
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private NppbkcGeneralModel MapNppbkcModel(CustomService.Data.MASTER_NPPBKC nppbkc)
        {
            try
            {
                return new NppbkcGeneralModel()
                {
                    Id = nppbkc.NPPBKC_ID,
                    Region = nppbkc.REGION,
                    Address = String.Format("{0}, {1}", nppbkc.ADDR1, nppbkc.ADDR2),
                    City = nppbkc.CITY,
                    CityAlias = nppbkc.CITY_ALIAS,
                    KppbcId = nppbkc.KPPBC_ID,
                    Company = (nppbkc.COMPANY != null) ? new CompanyModel()
                    {
                        Id = nppbkc.COMPANY.BUKRS,
                        Name = nppbkc.COMPANY.BUTXT,
                        Alias = nppbkc.COMPANY.BUTXT_ALIAS,
                        Npwp = nppbkc.COMPANY.NPWP
                    } : null
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private POAGeneralModel MapPoaModel(CustomService.Data.POA poa)
        {
            try
            {
                return new POAGeneralModel()
                {
                    Id = poa.POA_ID,
                    Name = String.Format("{0} {1}", poa.USER_LOGIN.FIRST_NAME, poa.USER_LOGIN.LAST_NAME),
                    Address = poa.POA_ADDRESS,
                    Position = poa.TITLE
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ProductDevDetailModel> MapProductDetailModelList(List<vwProductDevDetail> product)
        {
            var list = product.Select(s => new ProductDevDetailModel
            {
                PD_DETAIL_ID = s.PD_DETAIL_ID,
                Request_No = s.REQUEST_NO,
                Fa_Code_Old = s.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.FA_CODE_NEW,
                Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                Company = new CompanyModel { Id = s.BUKRS, Name = s.COMPANY_NAME },
                Hl_Code = s.HL_CODE,
                Market = new MarketModel { Market_Id = s.MARKET_ID, Market_Desc = s.MARKET_DESC },
                Werks = s.PRODUCTION_CENTER
            }).ToList();
            return list;
        }        

        [HttpPost]
        public ActionResult GetProductDevelopmentItemList(long[] ItemNotIn, long ReceivedID)
        {
            try
            {
                var completeId = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                var theapp = penetapanSKEPService.GetProductDevDetail((int)Enums.ProductDevelopmentAction.PenetapanSKEP, ReceivedID);
                if(ItemNotIn != null)
                {
                    var itemnotinlist = ItemNotIn.ToList();
                    theapp = theapp.Where(w => !ItemNotIn.Contains(w.PD_DETAIL_ID)).ToList();
                }
                var list = MapProductDetailModelList(theapp);
                return Json(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetTariff(long HJE, DateTime StartDate, string GoodType)
        {
            try
            {
                var data = penetapanSKEPService.getTariffByCombine(GoodType, HJE, StartDate);
                var dataAttr = new { attribute = new { TariffId = data.TARIFF_ID, Tariff = data.TARIFF_VALUE } };
                return Json(dataAttr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
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

        public string UploadFile(HttpPostedFileBase File)
        {
            try
            {
                var filePath = "";

                if (File != null && File.ContentLength > 0)
                {
                    var baseFolder = "/files_upload/";
                    var uploadToFolder = Server.MapPath("~" + baseFolder);
                    var date_now = DateTime.Now;
                    var date = String.Format("{0:ddMMyyyyHHmmss}", date_now);
                    var extension = Path.GetExtension(File.FileName);
                    var file_name = Path.GetFileNameWithoutExtension(File.FileName) + "-penetapanskep-" + CurrentUser.USER_ID + "-" + date + extension;
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

        public void InsertUploadSuppDocFile(IEnumerable<SKEPSupportingDocumentModel> SuppDocList, long ID)
        {
            try
            {
                if (SuppDocList != null)
                {
                    foreach (var Doc in SuppDocList)
                    {
                        if (Doc.Path != "" && Doc.Path != null)
                        {
                            var filename = penetapanSKEPService.GetSupportingDocName(Doc.Id);
                            penetapanSKEPService.InsertFileUpload(ID, Doc.Path, CurrentUser.USER_ID, Doc.Id, false, filename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void InsertUploadCommonFile(List<string> FilePath, long ID, bool IsGov, List<string> FileName)
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
                            var DocName = Doc.Replace("/files_upload/", "");
                            var arrfileext = DocName.Split('.');
                            var countext = arrfileext.Count();
                            var fileext = "";
                            if (countext > 0)
                            {
                                fileext = arrfileext[countext - 1];
                            }
                            DocName = DocName.Replace("-penetapanskep-", "/");
                            var arrfilename = DocName.Split('/');
                            if (arrfilename.Count() > 0)
                            {
                                DocName = arrfilename[0] + "." + fileext;
                            }

                            var thefilename = filenamelist.Where(w => DocName.Contains(w.Text)).Select(s => s.Value).FirstOrDefault();
                            if (thefilename == null)
                            {
                                thefilename = "";
                            }
                            penetapanSKEPService.InsertFileUpload(ID, Doc, CurrentUser.USER_ID, 0, IsGov, thefilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private string GenerateURL(string path)
        {
            var url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + path;
            return url;
        }

        private string GenerateFileName(string path)
        {
            var filename = path.Replace("/files_upload/", "");
            var arrfileext = filename.Split('.');
            var countext = arrfileext.Count();
            var fileext = "";
            if (countext > 0)
            {
                fileext = arrfileext[countext - 1];
            }
            filename = filename.Replace("-penetapanskep-", "/");
            var arrfilename = filename.Split('/');
            if (arrfilename.Count() > 0)
            {
                filename = arrfilename[0] + "." + fileext;
            }
            return filename;
        }

        private List<ReceivedDecreeDetailModel> GenerateDecreeDetail(long ReceID)
        {
            var detail = penetapanSKEPService.GetPenetapanDetailByReceivedID(ReceID);
            var result = detail.Select(s => new ReceivedDecreeDetailModel
            {
                Brand_CE = s.BRAND_CE,
                Prod_Code = s.PROD_CODE,
                Company_Tier = s.COMPANY_TIER,
                Unit = s.UNIT,
                Brand_Content = s.BRAND_CONTENT,
                Tariff = s.TARIFF,
                Received_ID = s.RECEIVED_ID,
                PD_Detail_ID = s.PD_DETAIL_ID,
                Received_Detail_ID = s.RECEIVED_DETAIL_ID,
                HJEperPack_dec = s.HJE,

                Request_No = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                Fa_Code_Old = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                Fa_Code_Old_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                Fa_Code_New = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                Fa_Code_New_Desc = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                CompanyName = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                HlCode = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                MarketDesc = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                ProductionCenter = s.PRODUCT_DEVELOPMENT_DETAIL.T001W.NAME1
            }).ToList();
            return result;
        }

        private List<ConfirmDialogModel> GenerateConfirmDialog(string action)
        {
            var confirmdialog = new List<ConfirmDialogModel>();
            if (action == "update")
            {
                confirmdialog.Add(new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "SubmitButtonConfirm",
                        CssClass = "btn btn-success",
                        Label = "Submit"
                    },
                    CssClass = " submit-modal penskep",
                    Id = "PenetapanSubmitModal",
                    Message = "You are going to submit Penetapan SKEP data. Are you sure?",
                    Title = "Submit Confirmation",
                    ModalLabel = "SubmitModalLabel"
                });
                confirmdialog.Add(new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "CancelButtonConfirm",
                        CssClass = "btn btn-success",
                        Label = "Cancel"
                    },
                    CssClass = " cancel-modal penskep",
                    Id = "PenetapanCancelModal",
                    Message = "You are going to cancel Penetapan SKEP data. Are you sure?",
                    Title = "Cancel Confirmation",
                    ModalLabel = "CancelModalLabel"
                });
            }
            else if (action == "approve")
            {
                confirmdialog.Add(new ConfirmDialogModel()
                {
                    Action = new ConfirmDialogModel.Button()
                    {
                        Id = "ApproveButtonConfirm",
                        CssClass = "btn btn-success",
                        Label = "Approve"
                    },
                    CssClass = " approve-modal penskep",
                    Id = "PenetapanApproveModal",
                    Message = "You are going to approve Penetapan SKEP data. Are you sure?",
                    Title = "Approve Confirmation",
                    ModalLabel = "ApproveModalLabel"
                });
            }
            return confirmdialog;
        }

        private bool SendMail(string skep_number, string decree_number, string decree_date, long decreeid, List<string> sendto, string MailFor)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("skep_number", skep_number);
                parameters.Add("decree_number", decree_number);
                parameters.Add("decree_date", decree_date);

                var SkepDetail = GenerateDecreeDetail(decreeid);
                var maildetail = "";
                var index = 0;
                foreach (var detail in SkepDetail)
                {
                    index++;
                    maildetail += "<tr>";
                    maildetail += "<td colspan='3'>&nbsp;<b>SKEP Item " + index + "</b></td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Brand Name Registration</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.Brand_CE + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>FA Code</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.Fa_Code_New + "</td>";
                    maildetail += "</tr>";
                    maildetail += "<tr>";
                    maildetail += "<td style='padding-left:2em;'>Tarif(Rp)</td>";
                    maildetail += "<td>:</td>";
                    maildetail += "<td>&nbsp;" + detail.Tariff + "</td>";
                    maildetail += "</tr>";
                }

                parameters.Add("decree_item", maildetail);
                parameters.Add("url_detail", Url.Action("Detail", "BRPenetapanSkep", new { id = decreeid }, this.Request.Url.Scheme));
                parameters.Add("url_approve", Url.Action("Approve", "BRPenetapanSkep", new { id = decreeid }, this.Request.Url.Scheme));
                parameters.Add("url_revise", Url.Action("Edit", "BRPenetapanSkep", new { id = decreeid }, this.Request.Url.Scheme));

                long mailcontentId = 0;
                string emailname = "";
                if (MailFor == "submit")
                {
                    emailname = "Penetapan SKEP Approval Request";
                }
                else if (MailFor == "approve")
                {
                    emailname = "Penetapan SKEP Approval Notification";
                }
                else if (MailFor == "revise")
                {
                    emailname = "Penetapan SKEP Revision Request";
                }
                mailcontentId = penetapanSKEPService.GetEmailContentId(emailname);

                var mailContent = refService.GetMailContent(mailcontentId, parameters);
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

        [HttpPost]
        public PartialViewResult FilterDocument(ReceivedDecreeViewModel model)
        {
            model.ListReceivedDecree = GetReceivedList(model.SearchInput.Status, model.SearchInput.Creator, model.SearchInput.NppbkcId, model.IsCompleted, false);
            model.CurrentUser = CurrentUser.USER_ID;
            return PartialView("_PenetapanSKEPListTable", model);
        }

        private ReceivedDecreeViewModel GenerateLogs(ReceivedDecreeViewModel data)
        {
            try
            {
                //var workflow = refService.GetWorkflowHistory((int)Enums.MenuList.PenetapanSKEP, data.ViewModel.Received_ID).OrderBy(o => o.ACTION_DATE).ToList();
                var workflowInput = new GetByFormTypeAndFormIdInput();
                workflowInput.FormId = data.ViewModel.Received_ID;
                workflowInput.FormType = Enums.FormType.PenetapanSKEP;
                var workflow = this.whBLL.GetByFormTypeAndFormId(workflowInput).OrderBy(x => x.WORKFLOW_HISTORY_ID).ToList();


                data.WorkflowHistory = Mapper.Map<List<WorkflowHistoryViewModel>>(workflow);
                if (data.ViewModel.ApprovalStatusDescription.Key.Equals(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval)))
                {
                    var ApproverList = penetapanSKEPService.GetPOAApproverList(data.ViewModel.Received_ID).Select(s => s.POA_ID).ToList();
                    var approvername = "";
                    foreach (var approver in ApproverList)
                    {
                        if (approvername != "")
                        {
                            approvername += ", ";
                        }
                        approvername += approver;
                    }                    
                    data.WorkflowHistory.Add(new WorkflowHistoryViewModel
                    {
                        ACTION = "Waiting For POA Approval",
                        USERNAME = approvername
                    });
                }
                return data;
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return null;
            }
        }

        [HttpPost]
        public PartialViewResult FilterSummaryReports(ReceivedDecreeViewModel search)
        {
            var model = new ReceivedDecreeViewModel();
            model.ListReceivedDecree = GetSummaryReportList(search.SearchInput.Creator, search.SearchInput.Status.ToString());
            return PartialView("_PenetapanSKEPListTableSummaryReport", model);
        }

        public ActionResult SummaryReports()
        {            
            try
            {
                if (CurrentUser.UserRole == Enums.UserRole.Administrator || CurrentUser.UserRole == Enums.UserRole.POA || CurrentUser.UserRole == Enums.UserRole.Viewer)
                {
                    var model = new ReceivedDecreeViewModel()
                    {
                        MainMenu = mainMenu,
                        CurrentMenu = PageInfo,
                        IsNotViewer = (CurrentUser.UserRole != Enums.UserRole.Viewer ? true : false),
                        ListReceivedDecree = GetReceivedList(0, "", "", false, true),
                        CurrentUser = CurrentUser.USER_ID
                    };

                    var nppbkc = GlobalFunctions.GetNppbkcAll(_nppbkcbll);

                    if (CurrentUser.UserRole != Enums.UserRole.Administrator)
                    {
                        var filterNppbkc = nppbkc.Where(x => CurrentUser.ListUserNppbkc.Contains(x.Value));
                        nppbkc = new SelectList(filterNppbkc, "Value", "Text");
                    }

                    model.SearchInput.NppbkcIdList = nppbkc;

                    model.SearchInput.CreatorList = GlobalFunctions.GetCreatorList();

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Unauthorized", "Error");
                }
            }
            catch (Exception ex)
            {
                AddMessageInfo(ex.Message, Enums.MessageInfoType.Error);
                return RedirectToAction("Index");
            }
        }

        public void ExportXlsSummaryReports(ReceivedDecreeViewModel model)
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

        private string CreateXlsSummaryReports(PenetapanSKEPExportSummaryReportsViewModel modelExport)
        {
            var dataSummaryReport = GetSummaryReportList("", "");

            int iRow = 1;
            var slDocument = new SLDocument();
            
            slDocument = CreateHeaderExcel(slDocument, modelExport);

            iRow++;
            int iColumn = 1;
            foreach (var data in dataSummaryReport)
            {

                iColumn = 1;

                if (modelExport.Status)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.StrLastApproved_Status);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Creator)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CreatorName);
                    iColumn = iColumn + 1;
                }
                                    
                if (modelExport.FormNo)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Received_No);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Nppbkc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Nppbkc_ID);
                    iColumn = iColumn + 1;
                }

                if (modelExport.Kppbc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Kppbc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.CompanyName)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.CompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.AddressPlant)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.AddressPlant);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Decree_No);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DecreeDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, Convert.ToDateTime(data.Decree_Date).ToString("dd MMM yyyy"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.StartDate)
                {
                    slDocument.SetCellValue(iRow, iColumn, Convert.ToDateTime(data.Decree_StartDate).ToString("dd MMM yyyy"));
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.RequestNumber)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.Request_No);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.FAOld)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.Fa_Code_Old);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.FAOldDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.Fa_Code_Old_Desc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.FANew)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.Fa_Code_New);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.FANewDesc)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.Fa_Code_New_Desc);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Company)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.CompanyName);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.HLCode)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.HlCode);
                    iColumn = iColumn + 1;
                }

                if (modelExport.DetailExportModel.Market)
                {
                    slDocument.SetCellValue(iRow, iColumn, data.Detail.MarketDesc);
                    iColumn = iColumn + 1;
                }

                iRow++;
            }

            return CreateXlsFileSummaryReports(slDocument, iColumn, iRow);
        }

        private SLDocument CreateHeaderExcel(SLDocument slDocument, PenetapanSKEPExportSummaryReportsViewModel modelExport)
        {
            int iColumn = 1;
            int iRow = 1;

            if (modelExport.Status)
            {
                slDocument.SetCellValue(iRow, iColumn, "Last Approved Status");
                iColumn = iColumn + 1;
            }

            if (modelExport.Creator)
            {
                slDocument.SetCellValue(iRow, iColumn, "Creator");
                iColumn = iColumn + 1;
            }

            if (modelExport.FormNo)
            {
                slDocument.SetCellValue(iRow, iColumn, "Form Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.Nppbkc)
            {
                slDocument.SetCellValue(iRow, iColumn, "NPPBKC");
                iColumn = iColumn + 1;
            }

            if (modelExport.Kppbc)
            {
                slDocument.SetCellValue(iRow, iColumn, "KPPBC");
                iColumn = iColumn + 1;
            }

            if (modelExport.CompanyName)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company Name");
                iColumn = iColumn + 1;
            }

            if (modelExport.AddressPlant)
            {
                slDocument.SetCellValue(iRow, iColumn, "Address Plant");
                iColumn = iColumn + 1;
            }

            if (modelExport.DecreeNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Decree Number");
                iColumn = iColumn + 1;
            }

            if (modelExport.DecreeDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Decree Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.StartDate)
            {
                slDocument.SetCellValue(iRow, iColumn, "Start Date");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.RequestNumber)
            {
                slDocument.SetCellValue(iRow, iColumn, "Request Number");
                iColumn = iColumn + 1;
            }            

            if (modelExport.DetailExportModel.FAOld)
            {
                slDocument.SetCellValue(iRow, iColumn, "FA Code Old");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.FAOldDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "FA Code Old Description");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.FANew)
            {
                slDocument.SetCellValue(iRow, iColumn, "FA Code New");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.FANewDesc)
            {
                slDocument.SetCellValue(iRow, iColumn, "FA Code New Description");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Company)
            {
                slDocument.SetCellValue(iRow, iColumn, "Company");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.HLCode)
            {
                slDocument.SetCellValue(iRow, iColumn, "HL Code");
                iColumn = iColumn + 1;
            }

            if (modelExport.DetailExportModel.Market)
            {
                slDocument.SetCellValue(iRow, iColumn, "Market");
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

            var fileName = "PenetapanSKEP" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);
            slDocument.SaveAs(path);
            return path;
        }

        private List<ReceivedDecreeModel> GetSummaryReportList(string src_Creator, string src_Status)
        {
            try
            {
                var documents = new List<ReceivedDecreeModel>();
                var data = penetapanSKEPService.GetViewPenetapanSKEPByCreator(src_Creator);
                if(data.Any())
                {
                    documents = data.Select(s => new ReceivedDecreeModel
                    {
                        Created_By = s.CREATED_BY,
                        StrLastApproved_Status = s.STATUS,
                        Received_No = s.FORM_NUMBER,
                        Nppbkc_ID = s.NPPBKC,
                        Kppbc = s.KPPBC,
                        CompanyName = s.PLANT_NAME,
                        AddressPlant = s.PLANT_ADDRESS,
                        Decree_No = s.DECREE_NUMBER,
                        Decree_Date = s.DECREE_DATE,
                        Decree_StartDate = s.DECREE_STARTDATE,
                        Detail = new ReceivedDecreeDetailModel
                        {
                            Request_No = s.REQUEST_NUMBER,
                            Fa_Code_Old = s.FA_CODE_OLD,
                            Fa_Code_Old_Desc = s.FA_CODE_OLD_DESCR,
                            Fa_Code_New = s.FA_CODE_NEW,
                            Fa_Code_New_Desc = s.FA_CODE_NEW_DESCR,
                            CompanyName = s.COMPANY_NAME,
                            HlCode = s.HL_CODE,
                            MarketDesc = s.MARKET
                        }
                    }).ToList();

                    foreach (var document in documents)
                    {
                        var _user = refService.GetUser(document.Created_By);
                        document.CreatorName = _user.FIRST_NAME + " " + _user.LAST_NAME;
                        document.strDecree_Date = Convert.ToDateTime(document.Decree_Date).ToString("dd MMM yyyy");
                        document.strDecree_StartDate = Convert.ToDateTime(document.Decree_StartDate).ToString("dd MMM yyyy");
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
        public ActionResult ImportItems()
        {
            try
            {
                var FileImport = Request.Files[0];
                var ImportedItem = new List<ReceivedDecreeDetailModel>();
                var err = "";
                if (FileImport != null)
                {                    
                    var allowedExtensions = new[] { ".xls", ".xlsx" };
                    var extension = Path.GetExtension(FileImport.FileName);                    
                    if (allowedExtensions.Contains(extension))
                    {
                        var data = (new ExcelReader()).ReadExcel(FileImport);
                        if (data != null)
                        {
                            string[] ArrItemNotin;
                            List<string> ItemNotinList = new List<string>();
                            var ItemNotIn = Request.Form.Get("item_notin");
                            if(ItemNotIn != "")
                            {
                                ArrItemNotin = ItemNotIn.Split(',');
                                if (ArrItemNotin.Count() > 0)
                                {
                                    ItemNotinList = ArrItemNotin.ToList();
                                }
                            }
                            List<long> addedProductList = new List<long>();
                            var MasterProdtype = penetapanSKEPService.getMasterProductType();
                            var completeId = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                            long ReceivedID = 0;
                            var strReceivedID = Request.Form.Get("ViewModel.Received_ID");
                            if (strReceivedID != "")
                            {
                                ReceivedID = Convert.ToInt64(strReceivedID);
                            }
                            //var ProductdetailAvailable = productDevelopmentService.GetProductDevDetail().Where(w => w.STATUS_APPROVAL == completeId && w.PRODUCT_DEVELOPMENT.NEXT_ACTION == (int)Enums.ProductDevelopmentAction.PenetapanSKEP);
                            var ProductdetailAvailable = penetapanSKEPService.GetProductDevDetail((int)Enums.ProductDevelopmentAction.PenetapanSKEP, ReceivedID);
                            foreach (var datarow in data.DataRows)
                            {
                                if (datarow != null)
                                {
                                    var v_requestNo = datarow[0];
                                    if (v_requestNo == "")
                                    {
                                        err += "* Request Number cannot be empty <br/>";                                        
                                    }
                                    //var v_brandName = datarow[1];
                                    //if (v_brandName != "")
                                    //{
                                    //    if (!penetapanSKEPService.IsBrandExist(v_brandName))
                                    //    {
                                    //        err += "* Brand with name " + v_brandName + " is not exist <br/>";
                                    //    }
                                    //}
                                    var str_v_companyTier = datarow[1];
                                    var v_companyTier = 0;
                                    if (str_v_companyTier != "")
                                    {
                                        var companytier = refService.GetRefByType("COMPANY_TIER").Where(w => w.REFF_VALUE == str_v_companyTier);
                                        if (!companytier.Any())
                                        {
                                            err += "* Company Tier is not exist <br/>";
                                        }
                                        else
                                        {
                                            var tmp_comptier = companytier.Select(s => s.REFF_ID).FirstOrDefault();
                                            v_companyTier = Convert.ToInt32(tmp_comptier);
                                        }
                                    }
                                    var v_prodType = datarow[2];
                                    if (v_prodType != "")
                                    {
                                        var prodtype = MasterProdtype.Where(w => w.PROD_CODE == v_prodType);
                                        if(!prodtype.Any())
                                        {
                                            err += "* Product Type with code " + v_prodType + " is not exist <br/>";
                                        }
                                    }
                                    Int64 v_hjePack = 0;
                                    if (datarow[3] != "")
                                    {
                                        var isHJEnumeric = Int64.TryParse(datarow[3], out v_hjePack);
                                        if (!isHJEnumeric)
                                        {
                                            err += "* HJE must be numeric <br/>";
                                        }
                                    }
                                    Int64 v_content = 0;
                                    if (datarow[4] != "")
                                    {
                                        var isContentnumeric = Int64.TryParse(datarow[4], out v_content);
                                        if (!isContentnumeric)
                                        {
                                            err += "* Content must be numeric <br/>";
                                        }
                                    }
                                    var v_unit = datarow[5];
                                    if(v_unit != "")
                                    {
                                        if(v_unit != "Batang" && v_unit != "Gram")
                                        {
                                            err += "* Unit must be Batang or Gram <br/>";
                                        }
                                    }
                                    var productdev = ProductdetailAvailable.Where(w => w.REQUEST_NO == v_requestNo).ToList();
                                    if (productdev.Count() > 0)
                                    {                                        
                                        var item = MapProductDetailModelList(productdev).FirstOrDefault();
                                        if (!addedProductList.Contains(item.PD_DETAIL_ID))
                                        {
                                            addedProductList.Add(item.PD_DETAIL_ID);
                                            if (ItemNotinList.Contains(item.PD_DETAIL_ID.ToString()))
                                            {
                                                err += "* Product with request number " + item.Request_No + " already added before <br/>";
                                            }
                                            var str_startDate = Request.Form.Get("start_date");
                                            var startDate = Convert.ToDateTime(str_startDate);
                                            decimal v_tariff = 0;
                                            long v_hjeBatang = 0;
                                            if (v_content > 0)
                                            {
                                                v_hjeBatang = v_hjePack / v_content;
                                            }
                                            if (str_startDate != "" && v_hjePack != 0 && v_prodType != "")
                                            {                                                
                                                v_tariff = penetapanSKEPService.getTariffByCombine(v_prodType, v_hjePack, startDate).TARIFF_VALUE;
                                            }
                                            var receivedDet = new ReceivedDecreeDetailModel
                                            {
                                                Item = item,
                                                Brand_CE = GetBrandNameByFACode(item.Fa_Code_Old),
                                                Company_Tier = v_companyTier,
                                                Prod_Code = v_prodType,
                                                HJEperPack = v_hjePack,
                                                Brand_Content = v_content.ToString(),
                                                HJEperBatang = v_hjeBatang,
                                                Unit = v_unit,
                                                Tariff = v_tariff
                                            };
                                            ImportedItem.Add(receivedDet);
                                        }
                                        else
                                        {
                                            err += "* Product with request number " + datarow[0] + " cannot added more than 1 <br/>";
                                        }
                                    }
                                    else
                                    {
                                        err += "* Product with request number " + datarow[0] + " could not be found <br/>";
                                    }
                                }
                            }
                        }
                        else
                        {
                            err = "* Data is empty";
                        }
                    }
                    else
                    {
                        err = "* File extension is not allowed.";
                    }
                }

                var dataAttr = new { data = ImportedItem, attribute = new { ErrorMessage = err } };
                return Json(dataAttr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public ActionResult GetBrandByFACode(string FACode)
        {
            var brandce = GetBrandNameByFACode(FACode);
            return Json(brandce);
        }

        private string GetBrandNameByFACode(string FACode)
        {
            var brandce = "";
            var masterBR = penetapanSKEPService.getMasterBrand().Where(w => w.FA_CODE == FACode).OrderByDescending(o => o.SKEP_DATE);
            if (masterBR.Any())
            {
                brandce = masterBR.FirstOrDefault().BRAND_CE;
            }
            return brandce;
        }

        public ActionResult ChangeLog(int ID, string Token)
        {
            var data = new BaseModel();
            //var history = refService.GetChangesHistory((int)Enums.MenuList.PenetapanSKEP, ID.ToString()).ToList();
            var history = this.chBLL.GetByFormTypeAndFormId(Enums.MenuList.PenetapanSKEP, ID.ToString());
            data.ChangesHistoryList = Mapper.Map<List<ChangesHistoryItemModel>>(history);
            return PartialView("_ChangesHistoryTable", data);
        }

        public bool IsPOACanApprove(long ID, string UserId)
        {
            var isOk = false;
            if (ID != 0)
            {
                var approverlist = penetapanSKEPService.GetPOAApproverList(ID);
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

        public bool IsPOACanAccess(long ID, string UserId)
        {
            var isOk = true;
            var CreatorId = "";
            var ReceivedDec = penetapanSKEPService.FindPenetapanSKEP(ID);
            if (ReceivedDec != null)
            {
                CreatorId = ReceivedDec.CREATED_BY;
            }
            if (CreatorId != "")
            {
                if (UserId != CreatorId)
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
            var poadelegate = penetapanSKEPService.GetPOADelegationOfUser(CreatorId);
            if (poadelegate != null)
            {
                if (UserId == poadelegate.POA_TO)
                {
                    isOk = true;
                }
            }
            return isOk;
        }
    }
}