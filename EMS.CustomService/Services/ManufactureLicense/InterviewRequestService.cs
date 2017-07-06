using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Sampoerna.EMS.CustomService.Services.ManufactureLicense
{
    public class InterviewRequestService : GenericService
    {
        private DbContextTransaction transaction;
        private SystemReferenceService refService;
        private EMSDataModel context;
        private Dictionary<int, string> companyTypeList;
        private Dictionary<int, string> perihalList;
        private Dictionary<int, string> govstatusList;
        private List<String> fileExtList;

        public InterviewRequestService() : base()
        {
            refService = new SystemReferenceService();
            context = new EMSDataModel();
            transaction = context.Database.BeginTransaction();

            companyTypeList = new Dictionary<int, string>();
            companyTypeList.Add(1, "Pabrik Hasil Tembakau");
            companyTypeList.Add(2, "Importir Hasil Tembakau");

            perihalList = new Dictionary<int, string>();
            perihalList.Add(1, "Pendirian Perusahaan Baru");
            perihalList.Add(2, "Perubahan Nama Perusahaan");
            perihalList.Add(3, "Perubahan Jenis Hasil Tembakau");
            perihalList.Add(4, "Pemindahan atau Penambahan Lokasi Pabrik");
            perihalList.Add(5, "Perubahan Kepemilikan Perusahaan");
            perihalList.Add(6, "Perubahan Lokasi dan/atau Bangunan Pabrik ");

            govstatusList = new Dictionary<int, string>();
            govstatusList.Add(1, "Approved");
            govstatusList.Add(0, "Rejected");

            fileExtList = new List<string>();
            fileExtList.Add(".txt");
            fileExtList.Add(".csv");
            fileExtList.Add(".pdf");
            fileExtList.Add(".xls");
            fileExtList.Add(".xlsx");
            fileExtList.Add(".doc");
            fileExtList.Add(".docx");
        }

        public Dictionary<int, string> GetInterviewReqCompanyTypeList()
        {
            return companyTypeList;
        }

        public Dictionary<int, string> GetInterviewReqPerihalList()
        {
            return perihalList;
        }

        public Dictionary<int, string> GetGovStatusList()
        {
            return govstatusList;
        }

        public List<string> GetFileExtList()
        {
            return fileExtList;
        }

        public INTERVIEW_REQUEST InsertInterviewRequest(string Perihal = "", string Company_Type = "", string CreatedBy = "", Int64 LastApprovedStatus = 0, string NPPBCK_Id = null, string BUKRS = null, string POA_Id = null, string KPPBC = "", string KPPBC_ADDRESS = "", string City = "", string City_alias = "", string TextTo = "", DateTime? RequestDate = null, Int32 UserRole = 0, string formnumber = "")
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var now = DateTime.Now;
                var IRequest = new INTERVIEW_REQUEST();
                IRequest.REQUEST_DATE = Convert.ToDateTime(RequestDate);
                IRequest.PERIHAL = Perihal;
                IRequest.COMPANY_TYPE = Company_Type;
                IRequest.CREATED_BY = CreatedBy;
                IRequest.CREATED_DATE = now;
                //IRequest.FORM_NUMBER = GetFormNumber(BUKRS, NPPBCK_Id, City_alias);
                IRequest.FORM_NUMBER = formnumber;
                IRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                //IRequest.BA_NUMBER = BA_Num;
                //IRequest.BA_DATE = BA_Date;
                IRequest.NPPBKC_ID = NPPBCK_Id;
                IRequest.BUKRS = BUKRS;
                //IRequest.BA_STATUS = BA_Status;
                IRequest.POA_ID = POA_Id;
                IRequest.KPPBC = KPPBC;
                IRequest.KPPBC_ADDRESS = KPPBC_ADDRESS;
                IRequest.CITY = City;
                IRequest.CITY_ALIAS = City_alias;
                IRequest.TEXT_TO = TextTo;

                context.INTERVIEW_REQUEST.Add(IRequest);
                context.SaveChanges();
                transaction.Commit();
                
                Dictionary<string, string[]> changes = GetAllChanges(new INTERVIEW_REQUEST(), IRequest);
                LogsActivity(IRequest, changes, (int)Enums.MenuList.InterviewRequest, (int)Enums.ActionType.Created, UserRole, CreatedBy, "");
                return IRequest;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }

        public INTERVIEW_REQUEST UpdateInterviewRequest(long IRId, string Perihal = "", string Company_Type = "", string ModifiedBy = "", Int64 LastApprovedStatus = 0, string NPPBCK_Id = null, string BUKRS = null, string POA_Id = null, string KPPBC = "", string KPPBC_ADDRESS = "", string City = "", string City_alias = "", string TextTo = "", DateTime? RequestDate = null, int ActionType = 0, Int32 UserRole = 0)
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                Dictionary<string, string[]> changes = new Dictionary<string, string[]>();
                var now = DateTime.Now;
                var IRequest = new INTERVIEW_REQUEST();
                var OldIRequest = new INTERVIEW_REQUEST();
                var Where = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(IRId));
                if (Where.Count() > 0)
                {
                    IRequest = Where.FirstOrDefault();
                    OldIRequest = SetOldValueToTempModel(IRequest);
                    IRequest.REQUEST_DATE = Convert.ToDateTime(RequestDate);
                    IRequest.PERIHAL = Perihal;
                    IRequest.COMPANY_TYPE = Company_Type;
                    IRequest.LASTMODIFIED_BY = ModifiedBy;
                    IRequest.LASTMODIFIED_DATE = now;                    
                    IRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                    //IRequest.BA_NUMBER = BA_Num;
                    //IRequest.BA_DATE = BA_Date;
                    IRequest.NPPBKC_ID = NPPBCK_Id;
                    IRequest.BUKRS = BUKRS;
                    //IRequest.BA_STATUS = BA_Status;
                    IRequest.POA_ID = POA_Id;
                    IRequest.KPPBC = KPPBC;
                    IRequest.KPPBC_ADDRESS = KPPBC_ADDRESS;
                    IRequest.CITY = City;
                    IRequest.CITY_ALIAS = City_alias;
                    IRequest.TEXT_TO = TextTo;
                    changes = GetAllChanges(OldIRequest, IRequest);
                    context.SaveChanges();
                }
                transaction.Commit();
                if (Where.Count() > 0)
                {
                    LogsActivity(IRequest, changes, (int)Enums.MenuList.InterviewRequest, ActionType, UserRole, ModifiedBy, "");
                }
                return IRequest;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Update. See Inner Exception property to see details", ex);
            }
        }

        public INTERVIEW_REQUEST UpdateStatus(long IRId, Int64 LastApprovedStatus = 0, string ModifiedBy = "", int ActionType = 0, Int32 UserRole = 0, string Comment = "")
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var now = DateTime.Now;
                var IRequest = new INTERVIEW_REQUEST();
                var OldIRequest = new INTERVIEW_REQUEST();
                var Where = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(IRId));
                if (Where.Count() > 0)
                {                    
                    IRequest = Where.FirstOrDefault();
                    OldIRequest = SetOldValueToTempModel(IRequest);
                    IRequest.LASTMODIFIED_BY = ModifiedBy;
                    IRequest.LASTMODIFIED_DATE = now;
                    IRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                    if (ActionType != (int)Enums.ActionType.Submit && ActionType != (int)Enums.ActionType.Withdraw && ActionType != (int)Enums.ActionType.Cancel)
                    {
                        IRequest.LASTAPPROVED_BY = ModifiedBy;
                        IRequest.LASTAPPROVED_DATE = now;
                    }
                    context.SaveChanges();
                }
                transaction.Commit();
                if (Where.Count() > 0)
                {
                    Dictionary<string, string[]> changes = GetAllChanges(OldIRequest, IRequest);
                    LogsActivity(IRequest, changes, (int)Enums.MenuList.InterviewRequest, ActionType, UserRole, ModifiedBy, Comment);
                }
                return IRequest;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Update. See Inner Exception property to see details", ex);
            }
        }

        public INTERVIEW_REQUEST UpdateBASKEP(long IRId, bool BAStatus, string BANumber, DateTime BADate, Int64 LastApprovedStatus, string ModifiedBy, int ActionType, Int32 UserRole, string Comment)
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var now = DateTime.Now;
                var IRequest = new INTERVIEW_REQUEST();
                var OldIRequest = new INTERVIEW_REQUEST();
                var Where = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(IRId));
                if (Where.Count() > 0)
                {
                    IRequest = Where.FirstOrDefault();
                    OldIRequest = SetOldValueToTempModel(IRequest);
                    IRequest.LASTMODIFIED_BY = ModifiedBy;
                    IRequest.LASTMODIFIED_DATE = now;
                    IRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                    //IRequest.LASTAPPROVED_BY = ModifiedBy;
                    //IRequest.LASTAPPROVED_DATE = now;
                    IRequest.BA_STATUS = BAStatus;
                    IRequest.BA_NUMBER = BANumber;
                    IRequest.BA_DATE = BADate;
                    context.SaveChanges();
                }
                transaction.Commit();
                if (Where.Count() > 0)
                {
                    Dictionary<string, string[]> changes = GetAllChanges(OldIRequest, IRequest);
                    LogsActivity(IRequest, changes, (int)Enums.MenuList.InterviewRequest, ActionType, UserRole, ModifiedBy, Comment);
                }
                return IRequest;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Update. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetInterviewNeedApproveWithSameNPPBKC(string Approver)
        {
            try
            {
                context = new EMSDataModel();
                var listIR = new List<long>();
                var ApproverNPPBKC = context.POA_MAP.Where(w => w.POA_ID == Approver);
                if(ApproverNPPBKC.Any())
                {
                    var statusdraft_key = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Draft);
                    var statusedit_key = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Edited);
                    var NPPBKC = ApproverNPPBKC.Select(s => s.NPPBKC_ID).ToList();
                    listIR = context.INTERVIEW_REQUEST.Where(w => NPPBKC.Contains(w.NPPBKC_ID) && w.SYS_REFFERENCES.REFF_KEYS != statusdraft_key && w.SYS_REFFERENCES.REFF_KEYS != statusedit_key).Select(s => s.VR_FORM_ID).ToList();
                }
                return listIR;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved With Same NPPBKC. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetInterviewNeedApproveWithoutNPPBKC(string Approver)
        {
            try
            {
                context = new EMSDataModel();
                var listIR = new List<long>();
                var isExciser = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true && w.POA_ID == Approver);
                if (isExciser.Any())
                {
                    var statusId = refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID;
                    listIR = context.INTERVIEW_REQUEST.Where(w => w.LASTAPPROVED_STATUS == statusId && w.NPPBKC_ID == null && w.LASTAPPROVED_BY == null).Select(s => s.VR_FORM_ID).ToList();
                }
                return listIR;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved Without NPPBKC. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetInterviewNeedApproveWithNPPBKCButNoExcise(string Approver)
        {
            try
            {
                context = new EMSDataModel();
                var listIR = new List<long>();
                var Exciser = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true);
                var isExciser = Exciser.Where(w => w.POA_ID == Approver);
                if (isExciser.Any())
                {
                    var Interview = context.INTERVIEW_REQUEST.Where(w => w.NPPBKC_ID != null && w.SYS_REFFERENCES.REFF_KEYS == "WAITING_POA_APPROVAL");
                    if (Interview.Any())
                    {
                        var listExciser = Exciser.Select(s => s.POA_ID).ToList();
                        var NPPBKCwithoutExciser = context.POA_MAP.Where(w => !listExciser.Contains(w.POA_ID)).Select(s => s.NPPBKC_ID).ToList();
                        listIR = Interview.Where(w => NPPBKCwithoutExciser.Contains(w.NPPBKC_ID)).Select(s => s.VR_FORM_ID).ToList();
                    }
                }
                return listIR;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved Without NPPBKC. See Inner Exception property to see details", ex);
            }
        }

        public INTERVIEW_REQUEST_DETAIL InsertInterviewRequestDetail(Int64 InterviewReqId = 0, string ManufactAdd = "", Int64 CityId = 0, Int64 ProvinceId = 0, string SubDistrict = "", string Village = "", string Phone = "", string Fax = "", INTERVIEW_REQUEST_DETAIL Old = null, string CreatedBy = "")
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var IReqDet = new INTERVIEW_REQUEST_DETAIL();
                IReqDet.VR_FORM_ID = InterviewReqId;
                IReqDet.MANUFACTURE_ADDRESS = ManufactAdd;
                IReqDet.CITY_ID = CityId;
                IReqDet.PROVINCE_ID = ProvinceId;
                IReqDet.SUB_DISTRICT = SubDistrict;
                IReqDet.VILLAGE = Village;
                IReqDet.PHONE = Phone;
                IReqDet.FAX = Fax;

                context.INTERVIEW_REQUEST_DETAIL.Add(IReqDet);
                context.SaveChanges();
                transaction.Commit();

                if (Old == null)
                {
                    Old = new INTERVIEW_REQUEST_DETAIL();
                }                
                Dictionary<string, string[]> changes = GetAllDetailChanges(Old, IReqDet);
                LogsChages(InterviewReqId, changes, (int)Enums.MenuList.InterviewRequest, CreatedBy);

                return IReqDet;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Detail. See Inner Exception property to see details", ex);
            }
        }

        public void DeleteInterviewRequestDetail(long IRId, string Createdby, List<INTERVIEW_REQUEST_DETAIL> DeletedList)
        {
            try
            {
                context = new EMSDataModel();                
                var deleteList = context.INTERVIEW_REQUEST_DETAIL.Where(w => w.VR_FORM_ID.Equals(IRId));
                if (deleteList.Count() > 0)
                {
                    foreach (var delete in deleteList)
                    {
                        var oldData = DeletedList.Where(w => w.VR_FORM_DETAIL_ID == delete.VR_FORM_DETAIL_ID).FirstOrDefault();
                        if (oldData != null)
                        {
                            Dictionary<string, string[]> changes = GetAllDetailChanges(oldData, new INTERVIEW_REQUEST_DETAIL());
                            LogsChages(IRId, changes, (int)Enums.MenuList.InterviewRequest, Createdby);
                        }
                    }
                    context = new EMSDataModel();
                    transaction = context.Database.BeginTransaction();
                    var deleteList2 = context.INTERVIEW_REQUEST_DETAIL.Where(w => w.VR_FORM_ID.Equals(IRId));
                    foreach (var delete in deleteList2)
                    {                        
                        context.INTERVIEW_REQUEST_DETAIL.Remove(delete);                        
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Detail Delete. See Inner Exception property to see details", ex);
            }
        }

        public string GetFormNumber(string BUKRS, string NPPBKC_Id, string Cityalias)
        {
            try
            {
                var formnumber = "";
                var city_alias = "-";
                if (NPPBKC_Id != null && NPPBKC_Id != "")
                {
                    city_alias = context.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID.Equals(NPPBKC_Id)).Select(s => s.CITY_ALIAS ?? "").FirstOrDefault();
                }
                else
                {
                    city_alias = Cityalias;
                }
                var company_alias = context.T001.Where(w => w.BUKRS.Equals(BUKRS)).Select(s => s.BUTXT_ALIAS ?? "").FirstOrDefault();

                var now = DateTime.Now;
                var month = ToRoman(now.Month);
                var year = now.Year.ToString();
                formnumber = "/" + company_alias + "/" + city_alias + "/" + month + "/" + year;
                var lastFormNumber = context.INTERVIEW_REQUEST.Where(w => w.FORM_NUMBER != null).OrderByDescending(o => o.VR_FORM_ID).Select(s => s.FORM_NUMBER).FirstOrDefault();
                if (lastFormNumber == "" || lastFormNumber == null)
                {
                    lastFormNumber = "0";
                }
                else
                {
                    lastFormNumber = lastFormNumber.Substring(0, 10);
                }
                var numb = Convert.ToInt32(lastFormNumber) + 1;
                var finalNumb = numb.ToString().PadLeft(10, '0');
                formnumber = finalNumb + formnumber;
                return formnumber;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public IQueryable<INTERVIEW_REQUEST> GetInterviewRequestById(Int64 Id)
        {
            try
            {
                var result = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(Id));
                return result;
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<INTERVIEW_REQUEST_DETAIL> GetInterviewRequestDetailByIRId(Int64 IRId)
        {
            try
            {
                var result = context.INTERVIEW_REQUEST_DETAIL.Where(w => w.VR_FORM_ID.Equals(IRId));
                return result;
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request Detail. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<T001> GetCompanyFromNPPBKC(string NPPBKC_ID)
        {
            try
            {
                if (NPPBKC_ID != "" && NPPBKC_ID != "0")
                {
                    var T001WId = context.T001W.Where(w => w.NPPBKC_ID.Equals(NPPBKC_ID)).Select(s => s.WERKS).ToList();
                    var T001KId = context.T001K.Where(w => T001WId.Contains(w.BWKEY)).Select(s => s.BUKRS).ToList();
                    return context.T001.Where(w => T001KId.Contains(w.BUKRS));
                }
                else
                {
                    return uow.CompanyRepository.GetAll();
                }
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MASTER_CITY> GetCityList()
        {
            try
            {
                return context.MASTER_CITY;
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }        

        public IQueryable<INTERVIEW_REQUEST> GetAll()
        {
            try
            {
                return context.INTERVIEW_REQUEST;
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }

        public void InsertFileUpload(long IRId, string Path, string CreatedBy, long DocId, bool IsGovDoc, string FileName)
        {
            try
            {
                var now = DateTime.Now;
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var UploadFile = new FILE_UPLOAD();
                UploadFile.FORM_TYPE_ID = Convert.ToInt32(Enums.FormList.Interview);
                UploadFile.FORM_ID = IRId.ToString();
                UploadFile.PATH_URL = Path;
                UploadFile.UPLOAD_DATE = now;
                UploadFile.CREATED_BY = CreatedBy;
                UploadFile.CREATED_DATE = now;
                UploadFile.LASTMODIFIED_BY = CreatedBy;
                UploadFile.LASTMODIFIED_DATE = now;
                if (DocId != 0)
                {
                    UploadFile.DOCUMENT_ID = DocId;
                }
                UploadFile.IS_GOVERNMENT_DOC = IsGovDoc;
                UploadFile.IS_ACTIVE = true;
                UploadFile.FILE_NAME = FileName;
                context.FILE_UPLOAD.Add(UploadFile);
                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Detail Upload File. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<FILE_UPLOAD> GetFileUploadByIRId(long IRId)
        {
            try
            {
                var strID = IRId.ToString();
                var intFormType = Convert.ToInt32(Enums.FormList.Interview);
                return context.FILE_UPLOAD.Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Detail File Upload List. See Inner Exception property to see details", ex);                
            }
        }

        public void DeleteFileUpload(long fileid, string updatedby)
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var now = DateTime.Now;
                var fileupload = new FILE_UPLOAD();
                var Where = context.FILE_UPLOAD.Where(w => w.FILE_ID.Equals(fileid));
                if (Where.Count() > 0)
                {
                    fileupload = Where.FirstOrDefault();
                    fileupload.LASTMODIFIED_BY = updatedby;
                    fileupload.LASTMODIFIED_DATE = now;
                    fileupload.IS_ACTIVE = false;
                    context.SaveChanges();
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Delete File. See Inner Exception property to see details", ex);
            }
        }

        private Dictionary<string, string[]> GetAllChanges(INTERVIEW_REQUEST old, INTERVIEW_REQUEST updated)
        {
            try
            {                
                var changes = new Dictionary<string, string[]>();
                //if(old.VR_FORM_ID != updated.VR_FORM_ID)
                //{
                //    var oldvalue = old.VR_FORM_ID == 0 ? "N/A" : old.VR_FORM_ID.ToString();
                //    var newvalue = updated.VR_FORM_ID == 0 ? "N/A" : updated.VR_FORM_ID.ToString();
                //    changes.Add("VR_FORM_ID", new string[] { oldvalue, newvalue });
                //}
                if (old.FORM_NUMBER != updated.FORM_NUMBER)
                {
                    var oldvalue = old.FORM_NUMBER == null ? "N/A" : old.FORM_NUMBER.ToString();
                    var newvalue = updated.FORM_NUMBER == null ? "N/A" : updated.FORM_NUMBER.ToString();
                    changes.Add("FORM_NUMBER", new string[] { oldvalue, newvalue });
                }
                if (old.REQUEST_DATE != updated.REQUEST_DATE)
                {                    
                    var oldvalue = old.REQUEST_DATE == DateTime.MinValue ? "N/A" : old.REQUEST_DATE.ToString("dd MMMM yyyy");
                    var newvalue = updated.REQUEST_DATE == DateTime.MinValue ? "N/A" : updated.REQUEST_DATE.ToString("dd MMMM yyyy");
                    changes.Add("REQUEST_DATE", new string[] { oldvalue, newvalue });
                }
                if (old.PERIHAL != updated.PERIHAL)
                {                    
                    var oldvalue = old.PERIHAL == null ? "N/A" : old.PERIHAL.ToString();
                    var newvalue = updated.PERIHAL == null ? "N/A" : updated.PERIHAL.ToString();
                    changes.Add("PERIHAL", new string[] { oldvalue, newvalue });
                }
                if (old.COMPANY_TYPE != updated.COMPANY_TYPE)
                {                    
                    var oldvalue = old.COMPANY_TYPE == null ? "N/A" : old.COMPANY_TYPE.ToString();
                    var newvalue = updated.COMPANY_TYPE == null ? "N/A" : updated.COMPANY_TYPE.ToString();
                    changes.Add("COMPANY_TYPE", new string[] { oldvalue, newvalue });
                }
                //if (old.CREATED_BY != updated.CREATED_BY)
                //{                    
                //    var oldvalue = old.CREATED_BY == null ? "N/A" : old.CREATED_BY.ToString();
                //    var newvalue = updated.CREATED_BY == null ? "N/A" : updated.CREATED_BY.ToString();
                //    changes.Add("CREATED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.CREATED_DATE != updated.CREATED_DATE)
                //{
                //    var oldvalue = old.CREATED_DATE == DateTime.MinValue ? "N/A" : old.CREATED_DATE.ToString("dd MMMM yyyy");
                //    var newvalue = updated.CREATED_DATE == DateTime.MinValue ? "N/A" : updated.CREATED_DATE.ToString("dd MMMM yyyy");
                //    changes.Add("CREATED_DATE", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTMODIFIED_BY != updated.LASTMODIFIED_BY)
                //{                    
                //    var oldvalue = old.LASTMODIFIED_BY == null ? "N/A" : old.LASTMODIFIED_BY.ToString();
                //    var newvalue = updated.LASTMODIFIED_BY == null ? "N/A" : updated.LASTMODIFIED_BY.ToString();
                //    changes.Add("LASTMODIFIED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTMODIFIED_DATE != updated.LASTMODIFIED_DATE)
                //{                    
                //    var oldvalue = old.LASTMODIFIED_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(old.LASTMODIFIED_DATE).ToString("dd MMMM yyyy");
                //    var newvalue = updated.LASTMODIFIED_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(updated.LASTMODIFIED_DATE).ToString("dd MMMM yyyy");
                //    changes.Add("LASTMODIFIED_DATE", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTAPPROVED_BY != updated.LASTAPPROVED_BY)
                //{                    
                //    var oldvalue = old.LASTAPPROVED_BY == null ? "N/A" : old.LASTAPPROVED_BY.ToString();
                //    var newvalue = updated.LASTAPPROVED_BY == null ? "N/A" : updated.LASTAPPROVED_BY.ToString();
                //    changes.Add("LASTAPPROVED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTAPPROVED_DATE != updated.LASTAPPROVED_DATE)
                //{                    
                //    var oldvalue = old.LASTAPPROVED_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(old.LASTAPPROVED_DATE).ToString("dd MMMM yyyy");
                //    var newvalue = updated.LASTAPPROVED_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(updated.LASTAPPROVED_DATE).ToString("dd MMMM yyyy");
                //    changes.Add("LASTAPPROVED_DATE", new string[] { oldvalue, newvalue });
                //}                
                if (old.LASTAPPROVED_STATUS != updated.LASTAPPROVED_STATUS)
                {                    
                    var oldvalue = old.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(old.LASTAPPROVED_STATUS).REFF_VALUE;
                    var newvalue = updated.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(updated.LASTAPPROVED_STATUS).REFF_VALUE;
                    changes.Add("LASTAPPROVED_STATUS", new string[] { oldvalue, newvalue });
                }
                if (old.BA_NUMBER != updated.BA_NUMBER)
                {                    
                    var oldvalue = old.BA_NUMBER == null ? "N/A" : old.BA_NUMBER.ToString();
                    var newvalue = updated.BA_NUMBER == null ? "N/A" : updated.BA_NUMBER.ToString();
                    changes.Add("BA_NUMBER", new string[] { oldvalue, newvalue });
                }
                if (old.BA_DATE != updated.BA_DATE)
                {                    
                    var oldvalue = old.BA_DATE == null ? "N/A" : old.BA_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(old.BA_DATE).ToString("dd MMMM yyyy");
                    var newvalue = updated.BA_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(updated.BA_DATE).ToString("dd MMMM yyyy");
                    changes.Add("BA_DATE", new string[] { oldvalue, newvalue });
                }
                if (old.NPPBKC_ID != updated.NPPBKC_ID)
                {                    
                    var oldvalue = old.NPPBKC_ID == null ? "N/A" : old.NPPBKC_ID.ToString();
                    var newvalue = updated.NPPBKC_ID == null ? "N/A" : updated.NPPBKC_ID.ToString();
                    changes.Add("NPPBKC", new string[] { oldvalue, newvalue });
                }
                if (old.BUKRS != updated.BUKRS)
                {
                    context = new EMSDataModel();
                    var TheCompanies = context.T001;
                    var oldvalue = old.BUKRS == null ? "N/A" : TheCompanies.Where(w => w.BUKRS == old.BUKRS).FirstOrDefault().BUTXT;
                    var newvalue = updated.BUKRS == null ? "N/A" : TheCompanies.Where(w => w.BUKRS == updated.BUKRS).FirstOrDefault().BUTXT;
                    changes.Add("COMPANY", new string[] { oldvalue, newvalue });
                }
                if (old.BA_STATUS != updated.BA_STATUS)
                {
                    var oldvalue = old.BA_STATUS == null ? "N/A" : old.BA_STATUS == true ? "Approved" : "Rejected";
                    var newvalue = updated.BA_STATUS == null ? "N/A" : updated.BA_STATUS == true ? "Approved" : "Rejected";
                    changes.Add("BA_STATUS", new string[] { oldvalue, newvalue });
                }
                if (old.POA_ID != updated.POA_ID)
                {                    
                    var oldvalue = old.POA_ID == null ? "N/A" : old.POA_ID.ToString();
                    var newvalue = updated.POA_ID == null ? "N/A" : updated.POA_ID.ToString();
                    changes.Add("POA", new string[] { oldvalue, newvalue });
                }                
                if (old.KPPBC != updated.KPPBC)
                {                    
                    var oldvalue = old.KPPBC == null ? "N/A" : old.KPPBC.ToString();
                    var newvalue = updated.KPPBC == null ? "N/A" : updated.KPPBC.ToString();
                    changes.Add("KPPBC", new string[] { oldvalue, newvalue });
                }
                if (old.KPPBC_ADDRESS != updated.KPPBC_ADDRESS)
                {
                    var oldvalue = old.KPPBC_ADDRESS == null ? "N/A" : old.KPPBC_ADDRESS.ToString();
                    var newvalue = updated.KPPBC_ADDRESS == null ? "N/A" : updated.KPPBC_ADDRESS.ToString();
                    changes.Add("KPPBC_ADDRESS", new string[] { oldvalue, newvalue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get Change For Log. See Inner Exception property to see details", ex);
            }
        }

        private Dictionary<string, string[]> GetAllDetailChanges(INTERVIEW_REQUEST_DETAIL old, INTERVIEW_REQUEST_DETAIL updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                
                //if (old.VR_FORM_DETAIL_ID != updated.VR_FORM_DETAIL_ID)
                //{
                //    var oldvalue = old.VR_FORM_DETAIL_ID == 0 ? "N/A" : old.VR_FORM_DETAIL_ID.ToString();
                //    var newvalue = updated.VR_FORM_DETAIL_ID == 0 ? "N/A" : updated.VR_FORM_DETAIL_ID.ToString();
                //    changes.Add("VR_FORM_DETAIL_ID", new string[] { oldvalue, newvalue });
                //}
                if (old.MANUFACTURE_ADDRESS != updated.MANUFACTURE_ADDRESS)
                {
                    var oldvalue = old.MANUFACTURE_ADDRESS == null ? "N/A" : old.MANUFACTURE_ADDRESS.ToString();
                    var newvalue = updated.MANUFACTURE_ADDRESS == null ? "N/A" : updated.MANUFACTURE_ADDRESS.ToString();
                    changes.Add("MANUFACTURE_ADDRESS", new string[] { oldvalue, newvalue });
                }
                if (old.CITY_ID != updated.CITY_ID)
                {                    
                    var TheCities = GetCityList();
                    var oldvalue = old.CITY_ID == 0 ? "N/A" : TheCities.Where(w => w.CITY_ID == old.CITY_ID).FirstOrDefault().CITY_NAME;
                    var newvalue = updated.CITY_ID == 0 ? "N/A" : TheCities.Where(w => w.CITY_ID == updated.CITY_ID).FirstOrDefault().CITY_NAME;
                    changes.Add("CITY", new string[] { oldvalue, newvalue });
                }
                if (old.PROVINCE_ID != updated.PROVINCE_ID)
                {
                    context = new EMSDataModel();
                    var TheProvinces = context.MASTER_STATE;
                    var oldvalue = old.PROVINCE_ID == 0 ? "N/A" : TheProvinces.Where(w => w.STATE_ID == old.PROVINCE_ID).FirstOrDefault().STATE_NAME;
                    var newvalue = updated.PROVINCE_ID == 0 ? "N/A" : TheProvinces.Where(w => w.STATE_ID == updated.PROVINCE_ID).FirstOrDefault().STATE_NAME;
                    changes.Add("PROVINCE", new string[] { oldvalue, newvalue });
                }
                if (old.SUB_DISTRICT != updated.SUB_DISTRICT)
                {
                    var oldvalue = old.SUB_DISTRICT == null ? "N/A" : old.SUB_DISTRICT.ToString();
                    var newvalue = updated.SUB_DISTRICT == null ? "N/A" : updated.SUB_DISTRICT.ToString();
                    changes.Add("SUB_DISTRICT", new string[] { oldvalue, newvalue });
                }
                if (old.VILLAGE != updated.VILLAGE)
                {
                    var oldvalue = old.VILLAGE == null ? "N/A" : old.VILLAGE.ToString();
                    var newvalue = updated.VILLAGE == null ? "N/A" : updated.VILLAGE.ToString();
                    changes.Add("VILLAGE", new string[] { oldvalue, newvalue });
                }
                if (old.PHONE != updated.PHONE)
                {
                    var oldvalue = old.PHONE == null ? "N/A" : old.PHONE.ToString();
                    var newvalue = updated.PHONE == null ? "N/A" : updated.PHONE.ToString();
                    changes.Add("PHONE", new string[] { oldvalue, newvalue });
                }                
                if (old.FAX != updated.FAX)
                {
                    var oldvalue = old.FAX == null ? "N/A" : old.FAX.ToString();
                    var newvalue = updated.FAX == null ? "N/A" : updated.FAX.ToString();
                    changes.Add("FAX", new string[] { oldvalue, newvalue });
                }

                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get Change For Log. See Inner Exception property to see details", ex);
            }
        }

        public void LogsActivity(INTERVIEW_REQUEST data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                context = new EMSDataModel();
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        data.VR_FORM_ID.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    Convert.ToInt64(data.VR_FORM_ID),
                    actionType,
                    actor,
                    DateTime.Now,
                    role,
                    comment
                    );
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Save Log. See Inner Exception property to see details", ex);
            }
        }

        public void LogsChages(long Id, Dictionary<string, string[]> changes, int formType, string actor)
        {
            try
            {
                context = new EMSDataModel();
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        Id.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }                
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Save Log. See Inner Exception property to see details", ex);
            }

        }

        private INTERVIEW_REQUEST SetOldValueToTempModel(INTERVIEW_REQUEST IRData)
        {
            var OldIRequest = new INTERVIEW_REQUEST()
            {
                VR_FORM_ID = IRData.VR_FORM_ID,
                REQUEST_DATE = IRData.REQUEST_DATE,
                PERIHAL = IRData.PERIHAL,
                COMPANY_TYPE = IRData.COMPANY_TYPE,
                CREATED_BY = IRData.CREATED_BY,
                CREATED_DATE = IRData.CREATED_DATE,
                LASTMODIFIED_BY = IRData.LASTMODIFIED_BY,
                LASTMODIFIED_DATE = IRData.LASTMODIFIED_DATE,
                LASTAPPROVED_BY = IRData.LASTAPPROVED_BY,
                LASTAPPROVED_DATE = IRData.LASTAPPROVED_DATE,
                FORM_NUMBER = IRData.FORM_NUMBER,
                LASTAPPROVED_STATUS = IRData.LASTAPPROVED_STATUS,
                BA_NUMBER = IRData.BA_NUMBER,
                BA_DATE = IRData.BA_DATE,
                NPPBKC_ID = IRData.NPPBKC_ID,
                BUKRS = IRData.BUKRS,
                BA_STATUS = IRData.BA_STATUS,
                POA_ID = IRData.POA_ID,
                KPPBC = IRData.KPPBC,
                KPPBC_ADDRESS = IRData.KPPBC_ADDRESS
            };
            return OldIRequest;
        }        

        public POA_DELEGATION GetPOADelegationOfUser(string UserId)
        {
            try
            {                
                var now = DateTime.Now.Date;                
                return context.POA_DELEGATION.Where(w => w.DATE_FROM <= now && w.DATE_TO >= now && w.POA_FROM.Equals(UserId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get Delegate. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<POA_DELEGATION> GetPOADelegatedUser(string UserId)
        {
            try
            {
                var now = DateTime.Now.Date;
                return context.POA_DELEGATION.Where(w => w.DATE_FROM <= now && w.DATE_TO >= now && w.POA_TO.Equals(UserId));
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get Delegate. See Inner Exception property to see details", ex);
            }
        }

        public List<POA> GetPOAApproverList(long IRId)
        {
            try
            {
                var ListPOA = new List<POA>();
                var RealListPOA = new List<POA>();
                var IRequest = GetInterviewRequestById(IRId).FirstOrDefault();
                if (IRequest != null)
                {
                    var NPPBKCId = IRequest.NPPBKC_ID;
                    if (IRequest.SYS_REFFERENCES.REFF_KEYS != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval) && IRequest.LASTAPPROVED_BY == null)
                    {                        
                        var ListPOA_Nppbkc = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true && w.POA_ID != IRequest.CREATED_BY).Select(s => s.POA_ID).ToList();
                        var OriexcisePOA = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true && w.POA.IS_ACTIVE == true).Select(s => s.POA_ID).ToList();
                        var excisePOA = new List<string>();
                        if (ListPOA_Nppbkc.Count() == 0)
                        {
                            excisePOA = OriexcisePOA.Where(w => ListPOA_Nppbkc.Contains(w)).ToList();
                        }
                        var ListPOAExcise_Raw = context.POA.Where(w => w.POA_ID != IRequest.CREATED_BY && w.IS_ACTIVE == true);
                        if (excisePOA.Count() == 0 || excisePOA == null)
                        {
                            ListPOAExcise_Raw = ListPOAExcise_Raw.Where(w => OriexcisePOA.Contains(w.POA_ID));
                        }
                        else
                        {
                            ListPOAExcise_Raw = ListPOAExcise_Raw.Where(w => excisePOA.Contains(w.POA_ID));
                        }
                        foreach (var poaresult in ListPOAExcise_Raw)
                        {
                            ListPOA.Add(new POA
                            {
                                POA_ID = poaresult.POA_ID,
                                POA_EMAIL = poaresult.POA_EMAIL,
                                PRINTED_NAME = poaresult.PRINTED_NAME
                            });
                            var poadelegate = GetPOADelegationOfUser(poaresult.POA_ID);
                            if (poadelegate != null)
                            {
                                ListPOA.Add(new POA
                                {
                                    POA_ID = poadelegate.POA_TO,
                                    POA_EMAIL = poadelegate.USER1.EMAIL,
                                    PRINTED_NAME = poadelegate.USER1.FIRST_NAME
                                });
                            }
                        }                        
                    }
                    else
                    {
                        var lastApprover = IRequest;
                        if (lastApprover != null)
                        {
                            ListPOA.Add(new POA
                            {
                                POA_ID = lastApprover.LASTAPPROVED_BY,
                                POA_EMAIL = lastApprover.USER1.EMAIL,
                                PRINTED_NAME = lastApprover.USER1.FIRST_NAME
                            });
                            var poadelegate = GetPOADelegationOfUser(lastApprover.LASTAPPROVED_BY);
                            if (poadelegate != null)
                            {
                                ListPOA.Add(new POA
                                {
                                    POA_ID = poadelegate.POA_TO,
                                    POA_EMAIL = poadelegate.USER1.EMAIL,
                                    PRINTED_NAME = poadelegate.USER1.FIRST_NAME
                                });
                            }
                        }
                    }                    
                    if (ListPOA.Count() > 0)
                    {
                        foreach (var realPoa in ListPOA)
                        {
                            if (!RealListPOA.Where(w => w.POA_ID == realPoa.POA_ID).Any())
                            {
                                RealListPOA.Add(realPoa);
                            }
                        }
                    }                    
                }
                return RealListPOA;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get POA Approver. See Inner Exception property to see details", ex);
            }
        }

        public string InsertKPPBCId(string kppbcid, string createdby)
        {
            try
            {
                context = new EMSDataModel();
                var kppbc = new MASTER_KPPBC();
                kppbc.KPPBC_ID = kppbcid;
                kppbc.CREATED_DATE = DateTime.Now;
                kppbc.CREATED_BY = createdby;
                context.ZAIDM_EX_KPPBC.Add(kppbc);
                context.SaveChanges();
                return kppbcid;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string GetSupportingDocName(long Id)
        {
            var docname = "";
            context = new EMSDataModel();
            var doc = context.MASTER_SUPPORTING_DOCUMENT.Where(w => w.DOCUMENT_ID == Id);
            if(doc.Any())
            {
                docname = doc.Select(s => s.SUPPORTING_DOCUMENT_NAME).FirstOrDefault();
            }
            return docname;
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value between 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        public IQueryable<vwMLInterviewRequest> GetvwInterviewRequestAll()
        {
            try
            {
                return context.vwMLInterviewRequest;
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }

        public List<string> GetUserNPPBKCList(string UserID)
        {
            context = new EMSDataModel();
            var nppbkc = context.POA_MAP.Where(w => w.POA_ID == UserID);
            var thelist = new List<string>();
            if(nppbkc.Any())
            {
                thelist = nppbkc.Select(s => s.NPPBKC_ID).ToList();
            }
            return thelist;
        }

        public MANUFACTURING_LISENCE_REQUEST GetManufacturingUsingThis(long IRID)
        {
            try
            {
                context = new EMSDataModel();
                return context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.VR_FORM_ID.Equals(IRID)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }

    }
}
