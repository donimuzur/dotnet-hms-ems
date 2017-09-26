using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;

namespace Sampoerna.EMS.CustomService.Services.BrandRegistrationTransaction
{
    public class PenetapanSKEPService : GenericService
    {
        private SystemReferenceService refService;

        public PenetapanSKEPService() : base()
        {
            this.refService = new SystemReferenceService();
        }

        #region Get Data
        public IQueryable<RECEIVED_DECREE> GetAllPenetapanSKEP()
        {
            try
            {
                var result = this.uow.ReceivedDecreeRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<T001> GetCompanyFromNPPBKC(string NPPBKC_ID)
        {
            try
            {
                var context = new EMSDataModel();
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

        public IQueryable<RECEIVED_DECREE_DETAIL> GetAllPenetapanSKEPDetail()
        {
            try
            {
                var result = this.uow.ReceivedDecreeDetailRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public RECEIVED_DECREE FindPenetapanSKEP(long ReceivedID)
        {
            try
            {
                var result = this.uow.ReceivedDecreeRepository.Find(ReceivedID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public RECEIVED_DECREE_DETAIL FindPenetapanSKEPDetail(long ReceivedDetailID)
        {
            try
            {
                var result = this.uow.ReceivedDecreeDetailRepository.Find(ReceivedDetailID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<RECEIVED_DECREE_DETAIL> GetPenetapanDetailByReceivedID(long ReceivedID)
        {
            try
            {
                return this.uow.ReceivedDecreeDetailRepository.GetManyQueryable(a => a.RECEIVED_ID == ReceivedID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public RECEIVED_DECREE GetLastRecordPenetapanSKEP ()
        {
            try
            {
                return this.uow.ReceivedDecreeRepository.GetAll().OrderByDescending(m => m.RECEIVED_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }
        public MASTER_PLANT FindPlantByNppbkcID(string Nppbkc_ID)
        {
            try
            {
                var result = this.uow.PlantRepository.GetFirst(pl => pl.NPPBKC_ID == Nppbkc_ID || pl.NPPBKC_IMPORT_ID == Nppbkc_ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }

        }

        #endregion

        #region Create
        public long CreatePenetapanSKEP(RECEIVED_DECREE data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.RECEIVED_DECREE.Add(data);
                        context.SaveChanges();
                        data.APPROVAL_STATUS = context.SYS_REFFERENCES.Find(data.LASTAPPROVED_STATUS);

                        var changes = GetAllChanges(new RECEIVED_DECREE(), data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);

                        transaction.Commit();
                        return data.RECEIVED_ID;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Create Penetapan SKEP Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public RECEIVED_DECREE UpdatePenetapanSKEP(RECEIVED_DECREE data, int ActionType, int UserRole)
        {
            try
            {
                var context = new EMSDataModel();
                Dictionary<string, string[]> changes = new Dictionary<string, string[]>();                       
                var Old = new RECEIVED_DECREE();
                var decree = new RECEIVED_DECREE();
                Old = context.RECEIVED_DECREE.Where(w => w.RECEIVED_ID.Equals(data.RECEIVED_ID)).FirstOrDefault();                
                var Where = context.RECEIVED_DECREE.Where(w => w.RECEIVED_ID.Equals(data.RECEIVED_ID));
                if (Where.Count() > 0)
                {
                    decree = Where.FirstOrDefault();                    
                    decree.NPPBKC_ID = data.NPPBKC_ID;
                    decree.LASTMODIFIED_BY = data.LASTMODIFIED_BY;
                    decree.LASTMODIFIED_DATE = data.LASTMODIFIED_DATE;
                    decree.DECREE_NO = data.DECREE_NO;
                    decree.DECREE_DATE = data.DECREE_DATE;
                    decree.DECREE_STARTDATE = data.DECREE_STARTDATE;
                    decree.LASTAPPROVED_STATUS = data.LASTAPPROVED_STATUS;
                    context.SaveChanges();
                    changes = GetAllChanges(Old, decree);
                    LogsActivity(context, data, changes, (int)Enums.MenuList.PenetapanSKEP, ActionType, UserRole, data.LASTMODIFIED_BY);
                }
                return decree;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Create Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }


        public void CreatePenetapanDetail(RECEIVED_DECREE_DETAIL data)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.RECEIVED_DECREE_DETAIL.Add(data);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Create Penetapan SKEP Detail Service. See Inner Exception property to see details", ex);
                    }

                }
            }
        }
        
        public FILE_UPLOAD CreateFileUpload(FILE_UPLOAD data)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.FILE_UPLOAD.Add(data);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return data;
        }
        #endregion

        #region Update

        public bool Edit(RECEIVED_DECREE data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.RECEIVED_DECREE.Find(data.RECEIVED_ID); // br detail not yet accomodated
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return true;
        }

        #endregion

        #region Changes Log
        /// <summary> Part of Changes Log Step which Mark All Available Changes </summary>        
        /// <param name="old"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        private Dictionary<string, string[]> GetAllChanges(RECEIVED_DECREE old, RECEIVED_DECREE updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();                

                if (old.RECEIVED_NO != updated.RECEIVED_NO)
                {
                    var oldvalue = old.RECEIVED_NO == null ? "N/A" : old.RECEIVED_NO.ToString();
                    var newvalue = updated.RECEIVED_NO == null ? "N/A" : updated.RECEIVED_NO.ToString();
                    changes.Add("FORM_NUMBER", new string[] { oldvalue, newvalue });
                }
                if (old.LASTAPPROVED_STATUS != updated.LASTAPPROVED_STATUS)
                {
                    var oldvalue = old.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(old.LASTAPPROVED_STATUS).REFF_VALUE;
                    var newvalue = updated.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(updated.LASTAPPROVED_STATUS).REFF_VALUE;
                    changes.Add("LASTAPPROVED_STATUS", new string[] { oldvalue, newvalue });
                }
                if (old.NPPBKC_ID != updated.NPPBKC_ID)
                {
                    var oldvalue = old.NPPBKC_ID == null ? "N/A" : old.NPPBKC_ID;
                    var newvalue = updated.NPPBKC_ID == null ? "N/A" : updated.NPPBKC_ID;
                    changes.Add("NPPBKC", new string[] { oldvalue, newvalue });
                }
                if (old.DECREE_NO != updated.DECREE_NO)
                {
                    var oldvalue = old.DECREE_NO == null ? "N/A" : old.DECREE_NO.ToString();
                    var newvalue = updated.DECREE_NO == null ? "N/A" : updated.DECREE_NO.ToString();
                    changes.Add("DECREE_NUMBER", new string[] { oldvalue, newvalue });
                }
                if (old.DECREE_DATE != updated.DECREE_DATE)
                {
                    var oldvalue = old.DECREE_DATE == null ? "N/A" : old.DECREE_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(old.DECREE_DATE).ToString("dd MMMM yyyy");
                    var newvalue = updated.DECREE_DATE == null ? "N/A" : updated.DECREE_DATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(updated.DECREE_DATE).ToString("dd MMMM yyyy");
                    changes.Add("DECREE_DATE", new string[] { oldvalue, newvalue });
                }
                if (old.DECREE_STARTDATE != updated.DECREE_STARTDATE)
                {
                    var oldvalue = old.DECREE_STARTDATE == null ? "N/A" : old.DECREE_STARTDATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(old.DECREE_STARTDATE).ToString("dd MMMM yyyy");
                    var newvalue = updated.DECREE_STARTDATE == null ? "N/A" : updated.DECREE_STARTDATE == DateTime.MinValue ? "N/A" : Convert.ToDateTime(updated.DECREE_STARTDATE).ToString("dd MMMM yyyy");
                    changes.Add("DECREE_STARTDATE", new string[] { oldvalue, newvalue });
                }                

                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        private Dictionary<string, string[]> GetAllDetailChanges(RECEIVED_DECREE_DETAIL old, RECEIVED_DECREE_DETAIL updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();

                if (old.BRAND_CE != updated.BRAND_CE)
                {
                    var oldvalue = old.BRAND_CE == null ? "N/A" : old.BRAND_CE.ToString();
                    var newvalue = updated.BRAND_CE == null ? "N/A" : updated.BRAND_CE.ToString();
                    changes.Add("BRAND_NAME", new string[] { oldvalue, newvalue });
                }                
                if (old.PROD_CODE != updated.PROD_CODE)
                {
                    var oldvalue = old.PROD_CODE == null ? "N/A" : GetProductTypeByCode(old.PROD_CODE);
                    var newvalue = updated.PROD_CODE == null ? "N/A" : GetProductTypeByCode(updated.PROD_CODE);
                    changes.Add("EXCISE_GOOD_TYPE", new string[] { oldvalue, newvalue });
                }
                if (old.COMPANY_TIER != updated.COMPANY_TIER)
                {
                    var oldvalue = old.COMPANY_TIER == 0 ? "N/A" : refService.GetReferenceById(old.COMPANY_TIER).REFF_VALUE;
                    var newvalue = updated.COMPANY_TIER == 0 ? "N/A" : refService.GetReferenceById(updated.COMPANY_TIER).REFF_VALUE;
                    changes.Add("COMPANY_TIER", new string[] { oldvalue, newvalue });
                }
                if (old.HJE != updated.HJE)
                {
                    var oldvalue = old.HJE == 0 ? "N/A" : string.Format("{0:n0}", old.HJE);
                    var newvalue = updated.HJE == 0 ? "N/A" : string.Format("{0:n0}", updated.HJE);
                    changes.Add("HJE_PER_PACK", new string[] { oldvalue, newvalue });
                }
                if (old.BRAND_CONTENT != updated.BRAND_CONTENT)
                {
                    var oldvalue = old.BRAND_CONTENT == null ? "N/A" : string.Format("{0:n0}", old.BRAND_CONTENT);
                    var newvalue = updated.BRAND_CONTENT == null ? "N/A" : string.Format("{0:n0}", updated.BRAND_CONTENT);
                    changes.Add("CONTENT", new string[] { oldvalue, newvalue });
                }
                if (old.TARIFF != updated.TARIFF)
                {
                    var oldvalue = old.TARIFF == 0 ? "N/A" : string.Format("{0:n0}", old.TARIFF);
                    var newvalue = updated.TARIFF == 0 ? "N/A" : string.Format("{0:n0}", updated.TARIFF);
                    changes.Add("TARIFF", new string[] { oldvalue, newvalue });
                }

                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public string GetProductTypeByCode(string prodcode)
        {
            var context = new EMSDataModel();
            return context.ZAIDM_EX_PRODTYP.Where(w => w.PROD_CODE == prodcode).FirstOrDefault().PRODUCT_TYPE;
        }

        /// <summary> Add record to Changes Log and Workflow History </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="changes"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="actor"></param>
        /// <param name="comment"></param>
        private void LogsActivity(EMSDataModel context, RECEIVED_DECREE data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        data.RECEIVED_ID.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    Convert.ToInt64(data.RECEIVED_ID),
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
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Change Status
        public RECEIVED_DECREE ChangeStatus(string id, Core.ReferenceKeys.ApprovalStatus status, int formType, int actionType, int role, string user, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var ID = Convert.ToInt64(id);
                        var old = context.RECEIVED_DECREE.Find(ID);
                        var data = (RECEIVED_DECREE)context.Entry(old).GetDatabaseValues().ToObject();
                        data.LASTAPPROVED_STATUS = refService.GetReferenceByKey(status).REFF_ID;
                        if (status == Core.ReferenceKeys.ApprovalStatus.Completed || status == Core.ReferenceKeys.ApprovalStatus.Edited)
                        {
                            data.LASTAPPROVED_BY = user;
                            data.LASTAPPROVED_DATE = DateTime.Now;
                        }
                        else if (status == Core.ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                        {
                            data.LASTMODIFIED_BY = user;
                            data.LASTMODIFIED_DATE = DateTime.Now;
                        }

                        data.APPROVAL_STATUS = context.SYS_REFFERENCES.Find(data.LASTAPPROVED_STATUS);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user, comment);
                        transaction.Commit();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        #endregion

        public IQueryable<ZAIDM_EX_BRAND> getMasterBrand()
        {
            try
            {
                var context = new EMSDataModel();
                var result = context.ZAIDM_EX_BRAND.Where(w => w.STATUS == true);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MASTER_PRODUCT_TYPE> getMasterProductType()
        {
            try
            {
                var complete = refService.GetRefByKey("COMPLETED").REFF_ID;
                var context = new EMSDataModel();
                var result = context.ZAIDM_EX_PRODTYP.Where(w => w.APPROVED_STATUS == complete && (w.IS_DELETED == false || w.IS_DELETED == null));
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public TARIFF getTariffByCombine(string prodCode, long HJE, DateTime date)
        {
            try
            {
                var complete = refService.GetRefByKey("COMPLETED").REFF_ID;
                var context = new EMSDataModel();
                var result = context.TARIFF.Where(w => w.STATUS_APPROVAL == complete && w.PROD_CODE == prodCode && w.HJE_FROM <= HJE && w.HJE_TO >= HJE && w.VALID_FROM <= date && w.VALID_TO >= date).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public void DeleteFileUpload(long fileid, string updatedby)
        {
            try
            {
                var context = new EMSDataModel();                
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
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public string GetSupportingDocName(long Id)
        {
            var docname = "";
            var context = new EMSDataModel();
            var doc = context.MASTER_SUPPORTING_DOCUMENT.Where(w => w.DOCUMENT_ID == Id);
            if (doc.Any())
            {
                docname = doc.Select(s => s.SUPPORTING_DOCUMENT_NAME).FirstOrDefault();
            }
            return docname;
        }

        public void InsertFileUpload(long Id, string Path, string CreatedBy, long DocId, bool IsGovDoc, string FileName)
        {
            try
            {
                var now = DateTime.Now;
                var context = new EMSDataModel();
                var UploadFile = new FILE_UPLOAD();
                UploadFile.FORM_TYPE_ID = Convert.ToInt32(Enums.FormList.SKEP);
                UploadFile.FORM_ID = Id.ToString();
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
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public void DeleteReceivedDecreeDetail(long ReceivedID)
        {
            try
            {
                var context = new EMSDataModel();                
                var deleteList = context.RECEIVED_DECREE_DETAIL.Where(w => w.RECEIVED_ID.Equals(ReceivedID));
                if (deleteList.Count() > 0)
                {
                    foreach (var delete in deleteList)
                    {
                        context.RECEIVED_DECREE_DETAIL.Remove(delete);
                    }
                }
                context.SaveChanges();                
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public void DeleteReceivedDecreeDetailById(long ReceivedDetailID)
        {
            try
            {
                var context = new EMSDataModel();
                var deleteList = context.RECEIVED_DECREE_DETAIL.Where(w => w.RECEIVED_DETAIL_ID.Equals(ReceivedDetailID));
                if (deleteList.Count() > 0)
                {
                    var delete = deleteList.FirstOrDefault();
                    context.RECEIVED_DECREE_DETAIL.Remove(delete);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<FILE_UPLOAD> GetFileUploadByReceiveID(long ID)
        {
            try
            {
                var context = new EMSDataModel();
                var strID = ID.ToString();
                var intFormType = Convert.ToInt32(Enums.FormList.SKEP);
                return context.FILE_UPLOAD.Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public long GetEmailContentId(string EmailName)
        {
            var context = new EMSDataModel();
            return context.CONTENTEMAIL.Where(w => w.EMAILNAME == EmailName).Select(s => s.CONTENTEMAILID).FirstOrDefault();
        }

        //public List<POA> GetPOAApproverList(long ReceiveID)
        //{
        //    try
        //    {
        //        var context = new EMSDataModel();
        //        var ListPOA = new List<POA>();
        //        var SKEP = FindPenetapanSKEP(ReceiveID);
        //        if (SKEP != null)
        //        {
        //            var NPPBKCId = SKEP.NPPBKC_ID;

        //            var ListPOA_Raw = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true && w.POA_ID != SKEP.CREATED_BY);
        //            if (ListPOA_Raw.Any())
        //            {
        //                foreach (var poaresult in ListPOA_Raw)
        //                {
        //                    ListPOA.Add(new POA
        //                    {
        //                        POA_ID = poaresult.POA_ID,
        //                        POA_EMAIL = poaresult.POA.POA_EMAIL,
        //                        PRINTED_NAME = poaresult.POA.PRINTED_NAME
        //                    });
        //                    var poadelegate = GetPOADelegationOfUser(poaresult.POA_ID);
        //                    if (poadelegate != null)
        //                    {
        //                        ListPOA.Add(new POA
        //                        {
        //                            POA_ID = poadelegate.POA_TO,
        //                            POA_EMAIL = poadelegate.USER1.EMAIL,
        //                            PRINTED_NAME = poadelegate.USER1.FIRST_NAME
        //                        });
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var excisePOA = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true).Select(s => s.POA_ID).ToList();
        //                var ListPOAExcise_Raw = context.POA.Where(w => w.POA_ID != SKEP.CREATED_BY && w.IS_ACTIVE == true && excisePOA.Contains(w.POA_ID));
        //                foreach (var poaresult in ListPOAExcise_Raw)
        //                {
        //                    ListPOA.Add(new POA
        //                    {
        //                        POA_ID = poaresult.POA_ID,
        //                        POA_EMAIL = poaresult.POA_EMAIL,
        //                        PRINTED_NAME = poaresult.PRINTED_NAME
        //                    });
        //                    var poadelegate = GetPOADelegationOfUser(poaresult.POA_ID);
        //                    if (poadelegate != null)
        //                    {
        //                        ListPOA.Add(new POA
        //                        {
        //                            POA_ID = poadelegate.POA_TO,
        //                            POA_EMAIL = poadelegate.USER1.EMAIL,
        //                            PRINTED_NAME = poadelegate.USER1.FIRST_NAME
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //        var RealListPOA = new List<POA>();
        //        if (ListPOA.Count() > 0)
        //        {
        //            foreach (var realPoa in ListPOA)
        //            {
        //                if (!RealListPOA.Where(w => w.POA_ID == realPoa.POA_ID).Any())
        //                {
        //                    RealListPOA.Add(realPoa);
        //                }
        //            }
        //        }

        //        return RealListPOA;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
        //    }
        //}

        public List<POA> GetPOAApproverList(long ReceiveID)
        {
            try
            {
                var context = new EMSDataModel();
                var ListPOA = new List<POA>();
                var RealListPOA = new List<POA>();
                var SKEP = FindPenetapanSKEP(ReceiveID);
                if (SKEP != null)
                {
                    var NPPBKCId = SKEP.NPPBKC_ID;
                    if (SKEP.LASTAPPROVED_BY == null)
                    {
                        var ListPOA_Nppbkc = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true && w.POA_ID != SKEP.CREATED_BY).Select(s => s.POA_ID).ToList();
                        var OriexcisePOA = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true && w.POA.IS_ACTIVE == true).Select(s => s.POA_ID).ToList();
                        var excisePOA = new List<string>();
                        if (ListPOA_Nppbkc.Count() == 0)
                        {
                            excisePOA = OriexcisePOA.Where(w => ListPOA_Nppbkc.Contains(w)).ToList();
                        }
                        var ListPOAExcise_Raw = context.POA.Where(w => w.POA_ID != SKEP.CREATED_BY && w.IS_ACTIVE == true);
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
                        var lastApprover = SKEP;
                        if (lastApprover != null)
                        {
                            ListPOA.Add(new POA
                            {
                                POA_ID = lastApprover.LASTAPPROVED_BY,
                                POA_EMAIL = lastApprover.APPROVER.EMAIL,
                                PRINTED_NAME = lastApprover.APPROVER.FIRST_NAME
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
                throw this.HandleException("Exception occured on Penetapan SKEP Get POA Approver. See Inner Exception property to see details", ex);
            }
        }

        public POA_DELEGATION GetPOADelegationOfUser(string UserId)
        {
            try
            {
                var context = new EMSDataModel();
                var now = DateTime.Now.Date;
                return context.POA_DELEGATION.Where(w => w.DATE_FROM <= now && w.DATE_TO >= now && w.POA_FROM.Equals(UserId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<POA_DELEGATION> GetPOADelegatedUser(string UserId)
        {
            try
            {
                var context = new EMSDataModel();
                var now = DateTime.Now.Date;
                return context.POA_DELEGATION.Where(w => w.DATE_FROM <= now && w.DATE_TO >= now && w.POA_TO.Equals(UserId));
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetSKEPNeedApproveWithSameNPPBKC(string Approver)
        {
            try
            {
                var context = new EMSDataModel();                
                var listIR = new List<long>();
                var ApproverNPPBKC = context.POA_MAP.Where(w => w.POA_ID == Approver);
                if (ApproverNPPBKC.Any())
                {
                    var drafstatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft).REFF_ID;
                    var editstatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited).REFF_ID;
                    var NPPBKC = ApproverNPPBKC.Select(s => s.NPPBKC_ID).ToList();
                    listIR = context.RECEIVED_DECREE.Where(w => NPPBKC.Contains(w.NPPBKC_ID) && w.LASTAPPROVED_STATUS != drafstatus && w.LASTAPPROVED_STATUS != editstatus).Select(s => s.RECEIVED_ID).ToList();
                }
                return listIR;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetSKEPNeedApproveWithoutNPPBKC(string Approver)
        {
            try
            {
                var context = new EMSDataModel();
                var listIR = new List<long>();
                var isExciser = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true && w.POA_ID == Approver);
                if (isExciser.Any())
                {
                    var statusId = refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID;
                    listIR = context.RECEIVED_DECREE.Where(w => w.LASTAPPROVED_STATUS == statusId && w.NPPBKC_ID == null).Select(s => s.RECEIVED_ID).ToList();
                }
                return listIR;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetSKEPNeedApproveWithNPPBKCButNoExcise(string Approver)
        {
            try
            {
                var context = new EMSDataModel();
                context = new EMSDataModel();
                var listIR = new List<long>();
                var Exciser = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true);
                var isExciser = Exciser.Where(w => w.POA_ID == Approver);
                if (isExciser.Any())
                {                    
                    var skep = context.RECEIVED_DECREE.Where(w => w.NPPBKC_ID != null && w.APPROVAL_STATUS.REFF_KEYS == "WAITING_POA_APPROVAL");
                    if (skep.Any())
                    {
                        var listExciser = Exciser.Select(s => s.POA_ID).ToList();
                        var NPPBKCwithoutExciser = context.POA_MAP.Where(w => !listExciser.Contains(w.POA_ID)).Select(s => s.NPPBKC_ID).ToList();
                        listIR = skep.Where(w => NPPBKCwithoutExciser.Contains(w.NPPBKC_ID)).Select(s => s.RECEIVED_ID).ToList();
                    }
                }
                return listIR;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved Without NPPBKC. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<vwPenetapanSKEP> GetViewPenetapanSKEPByCreator(string CreatedBy)
        {            
            var context = new EMSDataModel();
            var penetapanskeplist = context.vwPenetapanSKEP.Where(w => w.ID != 0);
            if(CreatedBy != "" && CreatedBy != null)
            {
                penetapanskeplist = penetapanskeplist.Where(w => w.CREATED_BY == CreatedBy);
            }
            return penetapanskeplist;
        }

        public IQueryable<POA_MAP> GetUserNPPBKC(string UserID)
        {
            var context = new EMSDataModel();
            var nppbkc = context.POA_MAP.Where(w => w.POA_ID == UserID);
            return nppbkc;
        }

        public bool IsBrandExist(string name)
        {
            var exist = true;
            var context = new EMSDataModel();
            var brand = context.ZAIDM_EX_BRAND.Where(w => w.BRAND_CE == name);
            if(!brand.Any())
            {
                exist = false;
            }
            return exist;
        }

        public void InsertDetailChangesLog(RECEIVED_DECREE_DETAIL newdata, long receivedDetID, int formType, string user)
        {
            var context = new EMSDataModel();
            var olddata = new RECEIVED_DECREE_DETAIL();
            if (receivedDetID > 0)
            {
                olddata = context.RECEIVED_DECREE_DETAIL.Where(w => w.RECEIVED_DETAIL_ID == receivedDetID).FirstOrDefault();
            }
            var changes = GetAllDetailChanges(olddata, newdata);
            context = new EMSDataModel();
            foreach (var map in changes)
            {
                refService.AddChangeLog(context,
                    formType,
                    newdata.RECEIVED_ID.ToString(),
                    map.Key,
                    map.Value[0],
                    map.Value[1],
                   user,
                   DateTime.Now
                    );
            }
            context.SaveChanges();
        }

        public string GetFormNumber(string NPPBKC_Id)
        {
            try
            {
                var context = new EMSDataModel();
                var formnumber = "";
                var city_alias = "-";
                var company_alias = "";
                if (NPPBKC_Id != null && NPPBKC_Id != "")
                {
                    company_alias = GetCompanyFromNPPBKC(NPPBKC_Id).FirstOrDefault().BUTXT_ALIAS;
                    city_alias = context.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID == NPPBKC_Id).FirstOrDefault().CITY_ALIAS;
                }

                var now = DateTime.Now;
                var month = ToRoman(now.Month);
                var year = now.Year.ToString();
                formnumber = "/" + company_alias + "/" + city_alias + "/" + month + "/" + year;
                var lastFormNumber = context.RECEIVED_DECREE.Where(w => w.RECEIVED_NO != null).OrderByDescending(o => o.RECEIVED_ID).Select(s => s.RECEIVED_NO).FirstOrDefault();
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

        //public void InsertToBrand(long ReceivedID)
        //{
        //    try
        //    {
        //        var context = new EMSDataModel();                
        //        var items = context.RECEIVED_DECREE_DETAIL.Where(w => w.RECEIVED_ID == ReceivedID).ToList();
        //        foreach (var item in items)
        //        {
        //            var data = new ZAIDM_EX_BRAND();
        //            data.WERKS = item.PRODUCT_DEVELOPMENT_DETAIL.WERKS;
        //            data.FA_CODE = item.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW;
        //            data.STICKER_CODE = item.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW;
        //            data.SERIES_CODE = "0";
        //            data.BRAND_CE = item.BRAND_CE;
        //            data.SKEP_NO = item.RECEIVED_DECREE.DECREE_NO;
        //            data.SKEP_DATE = item.RECEIVED_DECREE.DECREE_DATE;
        //            data.START_DATE = item.RECEIVED_DECREE.DECREE_STARTDATE;
        //            data.PROD_CODE = item.PROD_CODE;
        //            data.BRAND_CONTENT = item.BRAND_CONTENT;
        //            data.MARKET_ID = item.PRODUCT_DEVELOPMENT_DETAIL.MARKET_ID;
        //            data.COUNTRY = "ID";
        //            data.HJE_IDR = item.HJE;
        //            data.HJE_CURR = "IDR";
        //            data.TARIFF = item.TARIFF;
        //            data.TARIF_CURR = "IDR";
        //            data.EXC_GOOD_TYP = item.PROD_CODE;
        //            data.STATUS = true;
        //            data.CREATED_DATE = DateTime.Now;
        //            data.CREATED_BY = item.RECEIVED_DECREE.CREATED_BY;
        //            data.MODIFIED_DATE = DateTime.Now;
        //            data.MODIFIED_BY = item.RECEIVED_DECREE.CREATED_BY;                    

        //            context.ZAIDM_EX_BRAND.Add(data);
        //            context.SaveChanges();                    
        //        }                
        //    }
        //    catch (Exception ex)
        //    {                
        //        throw this.HandleException("Exception occured on Penetapan SKEP Completed. See Inner Exception property to see details", ex);
        //    }
        //}

        public Boolean InsertToBrand(long ReceivedID, string nppbkc)
        {
            try
            {
                var context = new EMSDataModel();                
                var plants = GetPlantByNPPBKC(nppbkc).ToList();
                var items = context.RECEIVED_DECREE_DETAIL.Where(w => w.RECEIVED_ID == ReceivedID).ToList();
                var brandlist = context.ZAIDM_EX_BRAND.Where(w => w.WERKS != null);
                foreach (var plant in plants)
                {
                    foreach (var item in items)
                    {
                        var isExist = brandlist.Where(w => w.WERKS == plant.WERKS && w.FA_CODE == item.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW && w.STICKER_CODE == item.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW);
                        if (!isExist.Any())
                        {
                            var data = new ZAIDM_EX_BRAND();
                            data.WERKS = plant.WERKS;
                            data.FA_CODE = item.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW;
                            data.STICKER_CODE = item.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW;
                            data.SERIES_CODE = "0";
                            data.BRAND_CE = item.BRAND_CE;
                            data.SKEP_NO = item.RECEIVED_DECREE.DECREE_NO;
                            data.SKEP_DATE = item.RECEIVED_DECREE.DECREE_DATE;
                            data.START_DATE = item.RECEIVED_DECREE.DECREE_STARTDATE;
                            data.PROD_CODE = item.PROD_CODE;
                            data.BRAND_CONTENT = item.BRAND_CONTENT;
                            data.MARKET_ID = item.PRODUCT_DEVELOPMENT_DETAIL.MARKET_ID;
                            data.COUNTRY = "ID";
                            data.HJE_IDR = item.HJE;
                            data.HJE_CURR = "IDR";
                            data.TARIFF = item.TARIFF;
                            data.TARIF_CURR = "IDR";
                            data.EXC_GOOD_TYP = item.PROD_CODE;
                            data.STATUS = true;
                            data.CREATED_DATE = DateTime.Now;
                            data.CREATED_BY = item.RECEIVED_DECREE.CREATED_BY;
                            data.MODIFIED_DATE = DateTime.Now;
                            data.MODIFIED_BY = item.RECEIVED_DECREE.CREATED_BY;

                            context.ZAIDM_EX_BRAND.Add(data);
                            context.SaveChanges();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {                
                throw this.HandleException("Exception occured on Brand Registration Completed. See Inner Exception property to see details", ex);
            }
        }

        public List<MASTER_PLANT> GetPlantByNPPBKC(string NPPBKCId)
        {
            try
            {
                var context = new EMSDataModel();
                var nppbkc = context.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID == NPPBKCId.ToString()).FirstOrDefault();
                var plants = context.T001W.Where(w => w.NPPBKC_ID == nppbkc.NPPBKC_ID).ToList();
                return plants;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }


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

        public List<vwProductDevDetail> GetProductDevDetail(int RegistrationType, long ReceivedID)
        {
            try
            {
                var context = new EMSDataModel();
                //var werks = context.T001W.Where(w => w.NPPBKC_ID.Equals(nppbkc)).Select(s => s.WERKS).ToList();
                var result = context.vwProductDevDetail.Where(w => w.NEXT_ACTION == RegistrationType).ToList();
                var receiveDet = context.RECEIVED_DECREE_DETAIL.Where(w => w.RECEIVED_ID == ReceivedID);
                var receiveDetList = MapToVWProductDevDetail(receiveDet);
                foreach (var rece in receiveDetList)
                {
                    result.Add(rece);
                }
                result = result.Distinct().ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public List<vwProductDevDetail> MapToVWProductDevDetail(IQueryable<RECEIVED_DECREE_DETAIL> receiveDet)
        {
            var list = new List<vwProductDevDetail>();
            foreach(var s in receiveDet)
            {
                var proddevdet = new vwProductDevDetail {
                    PD_NO = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.PD_NO,
                    NEXT_ACTION = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.NEXT_ACTION,
                    PD_DETAIL_ID = s.PD_DETAIL_ID,
                    FA_CODE_OLD = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                    FA_CODE_OLD_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                    FA_CODE_NEW = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                    FA_CODE_NEW_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                    HL_CODE = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                    MARKET_ID = s.PRODUCT_DEVELOPMENT_DETAIL.MARKET_ID,
                    MARKET_DESC = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                    WERKS = s.PRODUCT_DEVELOPMENT_DETAIL.WERKS,
                    PRODUCTION_CENTER = s.PRODUCT_DEVELOPMENT_DETAIL.T001W.NAME1,
                    IS_IMPORT = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT,
                    PD_ID = s.PRODUCT_DEVELOPMENT_DETAIL.PD_ID,
                    REQUEST_NO = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                    BUKRS = s.PRODUCT_DEVELOPMENT_DETAIL.BUKRS,
                    COMPANY_NAME = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                    LASTAPPROVED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_BY,
                    LASTAPPROVED_DATE = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_DATE,
                    LASTAPPROVED_STATUS = s.PRODUCT_DEVELOPMENT_DETAIL.STATUS_APPROVAL,
                    LASTMODIFIED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTMODIFIED_BY
                };
                list.Add(proddevdet);
            }
            //var result = receiveDet.Select(s => new vwProductDevDetail
            //{
            //    PD_NO = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.PD_NO,
            //    NEXT_ACTION = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.NEXT_ACTION,
            //    PD_DETAIL_ID = s.PD_DETAIL_ID,
            //    FA_CODE_OLD = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
            //    FA_CODE_OLD_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
            //    FA_CODE_NEW = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
            //    FA_CODE_NEW_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
            //    HL_CODE = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
            //    MARKET_ID = s.PRODUCT_DEVELOPMENT_DETAIL.MARKET_ID,
            //    MARKET_DESC = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
            //    WERKS = s.PRODUCT_DEVELOPMENT_DETAIL.WERKS,
            //    PRODUCTION_CENTER = s.PRODUCT_DEVELOPMENT_DETAIL.T001W.NAME1,
            //    IS_IMPORT = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT,
            //    PD_ID = s.PRODUCT_DEVELOPMENT_DETAIL.PD_ID,
            //    REQUEST_NO = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
            //    BUKRS = s.PRODUCT_DEVELOPMENT_DETAIL.BUKRS,
            //    COMPANY_NAME = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
            //    LASTAPPROVED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_BY,
            //    LASTAPPROVED_DATE = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_DATE,
            //    LASTAPPROVED_STATUS = s.PRODUCT_DEVELOPMENT_DETAIL.STATUS_APPROVAL,
            //    LASTMODIFIED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTMODIFIED_BY
            //}).ToList();
            return list;
        }
    }
}
