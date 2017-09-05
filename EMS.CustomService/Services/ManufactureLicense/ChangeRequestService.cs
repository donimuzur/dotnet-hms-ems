using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Enums = Sampoerna.EMS.Core.Enums;





namespace Sampoerna.EMS.CustomService.Services
{
    public class ChangeRequestService : GenericService
    {
        //private DbContextTransaction transaction;
        private SystemReferenceService refService;
        //private EMSDataModel context;
        private Dictionary<string, string> DocumentType;
        private Dictionary<int, string> govstatusList;
        private List<String> fileExtList;

        public ChangeRequestService() : base()
        {
            refService = new SystemReferenceService();
            //context = new EMSDataModel();
            //transaction = context.Database.BeginTransaction();

            DocumentType = new Dictionary<string, string>();
            DocumentType.Add("Layout", "Layout");
            DocumentType.Add("Data Perijinan", "Data Perijinan");
            DocumentType.Add("POA Excise", "POA Excise");

            fileExtList = new List<string>();
            fileExtList.Add(".txt");
            fileExtList.Add(".csv");
            fileExtList.Add(".pdf");
            fileExtList.Add(".xls");
            fileExtList.Add(".xlsx");
            fileExtList.Add(".doc");
            fileExtList.Add(".docx");

            govstatusList = new Dictionary<int, string>();
            govstatusList.Add(1, "Approved");
            govstatusList.Add(0, "Rejected");
        }


        #region FileUpload
        public void InsertFileUpload(long CRId, string Path, string CreatedBy, long DocId, bool IsGovDoc, string FileName)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var now = DateTime.Now;
                        var UploadFile = new FILE_UPLOAD();
                        UploadFile.FORM_TYPE_ID = Convert.ToInt32(Enums.FormList.Change);
                        UploadFile.FORM_ID = CRId.ToString();
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
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }

        }

        public IQueryable<FILE_UPLOAD> GetFileUploadByCRId(long CRId)
        {
            var context = new EMSDataModel();

            try
            {
                var strID = CRId.ToString();
                var intFormType = Convert.ToInt32(Enums.FormList.Change);
                return context.FILE_UPLOAD.Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true && w.IS_GOVERNMENT_DOC == false);
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on Manufacture License Change Request Detail File Upload List. See Inner Exception property to see details", ex);
            }
        }

        public void DeleteFileUpload(long fileid, string updatedby)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
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
                        throw this.HandleException("Exception occured on Manufacture License Change Request Delete File. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }


        }

        public string GetSupportingDocName(long Id)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var docname = "";
                    var doc = context.MASTER_SUPPORTING_DOCUMENT.Where(w => w.DOCUMENT_ID == Id);
                    if (doc.Any())
                    {
                        docname = doc.Select(s => s.SUPPORTING_DOCUMENT_NAME).FirstOrDefault();
                    }
                    return docname;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Change Request Delete File. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }

            }
        }

        #endregion

        #region CRUD
        public REPLACEMENT_DOCUMENTS Create(REPLACEMENT_DOCUMENTS data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.REPLACEMENT_DOCUMENTS.Add(data);
                        context.SaveChanges();
                        //data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);
                        //data.COMPANY = context.T001.Find(data.BUKRS);
                        //var changes = GetAllChanges(null, data);
                        //LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }

                }
            }
            return data;

        }

        public REPLACEMENT_DOCUMENTS Save(REPLACEMENT_DOCUMENTS data, Int32 UserRole = 0)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.REPLACEMENT_DOCUMENTS.Add(data);
                        context.SaveChanges();
                        transaction.Commit();


                        Dictionary<string, string[]> changes = GetAllChanges(new REPLACEMENT_DOCUMENTS(), data);
                        LogsActivity(data, changes, (int)Enums.MenuList.ChangeRequest, (int)Enums.ActionType.Created, UserRole, data.CREATED_BY, "");

                        transaction.Dispose();
                        context.Dispose();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }

                }


            }
        }

        public REPLACEMENT_DOCUMENTS Update(REPLACEMENT_DOCUMENTS updateData, int ActionType = 0, Int32 UserRole = 0)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Dictionary<string, string[]> changes = new Dictionary<string, string[]>();
                        var now = DateTime.Now;
                        var OldCRequest = new REPLACEMENT_DOCUMENTS();
                        var CRequest = new REPLACEMENT_DOCUMENTS();
                        var Where = context.REPLACEMENT_DOCUMENTS.Where(w => w.FORM_ID.Equals(updateData.FORM_ID));
                        if (Where.Count() > 0)
                        {
                            CRequest = Where.FirstOrDefault();
                            OldCRequest = SetOldValueToTempModel(CRequest);
                            CRequest.REQUEST_DATE = updateData.REQUEST_DATE;
                            CRequest.DOCUMENT_TYPE = updateData.DOCUMENT_TYPE;
                            CRequest.NPPBKC_ID = updateData.NPPBKC_ID;
                            CRequest.LASTMODIFIED_DATE = now;
                            CRequest.LASTAPPROVED_STATUS = updateData.LASTAPPROVED_STATUS;
                            changes = GetAllChanges(OldCRequest, CRequest);
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        if (Where.Count() > 0)
                        {
                            LogsActivity(CRequest, changes, (int)Enums.MenuList.ChangeRequest, ActionType, UserRole, updateData.LASTMODIFIED_BY, "");
                        }

                        return CRequest;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public REPLACEMENT_DOCUMENTS_DETAIL InsertChangeRequestDetail(Int64 FormId = 0, string UpdateNotes = "", REPLACEMENT_DOCUMENTS_DETAIL Old = null, string CreatedBy = "")
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var CReqDet = new REPLACEMENT_DOCUMENTS_DETAIL();
                        CReqDet.FORM_ID = FormId;
                        CReqDet.UPDATE_NOTES = UpdateNotes;

                        context.REPLACEMENT_DOCUMENTS_DETAIL.Add(CReqDet);
                        context.SaveChanges();
                        transaction.Commit();

                        if (Old == null)
                        {
                            Old = new REPLACEMENT_DOCUMENTS_DETAIL();
                        }
                        Dictionary<string, string[]> changes = GetAllDetailChanges(Old, CReqDet);
                        LogsChanges(FormId, changes, (int)Enums.MenuList.ChangeRequest, CreatedBy);

                        return CReqDet;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Change Request Detail. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }

                }

            }



        }

        public IQueryable<REPLACEMENT_DOCUMENTS_DETAIL> GetDocumentDetails(Int64 FormId = 0)
        {
            var context = new EMSDataModel();
            try
            {
                return context.REPLACEMENT_DOCUMENTS_DETAIL.Where(w => w.FORM_ID == FormId);
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on Manufacture License Change Request Detail Document List. See Inner Exception property to see details", ex);
            }


        }


        public void DeleteChangeRequestDetail(Int64 FormId, string Createdby, List<REPLACEMENT_DOCUMENTS_DETAIL> DeletedList)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        var deleteList = context.REPLACEMENT_DOCUMENTS_DETAIL.Where(w => w.FORM_ID.Equals(FormId));
                        if (deleteList.Count() > 0)
                        {
                            foreach (var delete in deleteList)
                            {
                                var oldData = DeletedList.Where(w => w.FORM_DET_ID == delete.FORM_DET_ID).FirstOrDefault();
                                if (oldData != null)
                                {
                                    Dictionary<string, string[]> changes = GetAllDetailChanges(oldData, new REPLACEMENT_DOCUMENTS_DETAIL());
                                    LogsChanges(FormId, changes, (int)Enums.MenuList.ChangeRequest, Createdby);
                                }
                            }


                            var deleteList2 = context.REPLACEMENT_DOCUMENTS_DETAIL.Where(w => w.FORM_ID.Equals(FormId));
                            foreach (var delete in deleteList2)
                            {
                                context.REPLACEMENT_DOCUMENTS_DETAIL.Remove(delete);
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
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }




        }

        public REPLACEMENT_DOCUMENTS UpdateStatus(long CRId, Int64 LastApprovedStatus = 0, string ModifiedBy = "", int ActionType = 0, Int32 UserRole = 0, string Comment = "")
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var now = DateTime.Now;
                        var CRequest = new REPLACEMENT_DOCUMENTS();
                        var OldCRequest = new REPLACEMENT_DOCUMENTS();
                        var Where = context.REPLACEMENT_DOCUMENTS.Where(w => w.FORM_ID.Equals(CRId));
                        if (Where.Count() > 0)
                        {
                            CRequest = Where.FirstOrDefault();
                            OldCRequest = SetOldValueToTempModel(CRequest);
                            CRequest.LASTMODIFIED_BY = ModifiedBy;
                            CRequest.LASTMODIFIED_DATE = now;
                            CRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                            //if ((LastApprovedStatus != refService.GetRefByKey("DRAFT_EDIT_STATUS").REFF_ID) && (LastApprovedStatus != refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID) && (LastApprovedStatus != refService.GetRefByKey("CANCELED").REFF_ID))
                            if (ActionType == (int)Enums.ActionType.Approve || ActionType == (int)Enums.ActionType.Reject || ActionType == (int)Enums.ActionType.Revise)
                            {
                                CRequest.LASTAPPROVED_BY = ModifiedBy;
                                CRequest.LASTAPPROVED_DATE = now;
                            }
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        if (Where.Count() > 0)
                        {
                            Dictionary<string, string[]> changes = GetAllChanges(OldCRequest, CRequest);
                            LogsActivity(CRequest, changes, (int)Enums.MenuList.ChangeRequest, ActionType, UserRole, ModifiedBy, Comment);
                        }
                        return CRequest;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }

            }



        }

        public REPLACEMENT_DOCUMENTS UpdateBASKEP(long CRId, bool BAStatus, string BANumber, DateTime BADate, Int64 LastApprovedStatus, string ModifiedBy, int ActionType, Int32 UserRole, string Comment)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var now = DateTime.Now;
                        var CRequest = new REPLACEMENT_DOCUMENTS();
                        var OldCRequest = new REPLACEMENT_DOCUMENTS();
                        var Where = context.REPLACEMENT_DOCUMENTS.Where(w => w.FORM_ID.Equals(CRId));
                        if (Where.Count() > 0)
                        {
                            CRequest = Where.FirstOrDefault();
                            OldCRequest = SetOldValueToTempModel(CRequest);
                            CRequest.LASTMODIFIED_BY = ModifiedBy;
                            CRequest.LASTMODIFIED_DATE = now;
                            CRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                            //CRequest.LASTAPPROVED_BY = ModifiedBy;
                            //CRequest.LASTAPPROVED_DATE = now;
                            CRequest.DECREE_STATUS = BAStatus;
                            CRequest.DECREE_NUMBER = BANumber;
                            //IRequest.BA_DATE = BADate;
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        if (Where.Count() > 0)
                        {
                            Dictionary<string, string[]> changes = GetAllChanges(OldCRequest, CRequest);
                            LogsActivity(CRequest, changes, (int)Enums.MenuList.ChangeRequest, ActionType, UserRole, ModifiedBy, Comment);
                        }
                        return CRequest;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }



        }


        #endregion

        #region Helper

        public Dictionary<int, string> GetGovStatusList()
        {
            return govstatusList;
        }

        public string SetNewFormNumber(string partial_number)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var lastFormNumber = context.REPLACEMENT_DOCUMENTS.Where(x => x.FORM_NO.Contains(partial_number)).OrderByDescending(o => o.FORM_NO).Select(s => s.FORM_NO).FirstOrDefault();
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
                    return finalNumb + "/" + partial_number;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }


    public Dictionary<string, string> GetDocumentTypes()
        {
            return DocumentType;
        }


        public IEnumerable<REPLACEMENT_DOCUMENTS> GetAll()
        {
            try
            {
                return this.uow.ReplacementDocumentRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ChangeRequestService. See Inner Exception property to see details", ex);
            }
        }

        //public IQueryable<REPLACEMENT_DOCUMENTS> GetAll()
        //{
        //    try
        //    {
        //        return context.REPLACEMENT_DOCUMENTS;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw HandleException("Exception occured on Manufacture License Change Request. See Inner Exception property to see details", ex);
        //    }
        //}


        public IQueryable<REPLACEMENT_DOCUMENTS> GetChangeRequestById(Int64 Id)
        {
            var context = new EMSDataModel();

            try
            {
                var result = context.REPLACEMENT_DOCUMENTS.Where(w => w.FORM_ID.Equals(Id));
                return result;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
            finally
            {
                //context.Dispose();
            }


            //using (var context = new EMSDataModel())
            //{
            //    try
            //    {
            //        var result = context.REPLACEMENT_DOCUMENTS.Where(w => w.FORM_ID.Equals(Id));
            //        return result;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            //    }
            //    finally
            //    {
            //        //context.Dispose();
            //    }
            //}

        }

        public IQueryable<REPLACEMENT_DOCUMENTS_DETAIL> GetChangeRequestDetailByFormId(Int64 FormId)
        {
            var context = new EMSDataModel();
            try
            {
                var result = context.REPLACEMENT_DOCUMENTS_DETAIL.Where(w => w.FORM_ID.Equals(FormId));
                return result;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture License Channge Request. See Inner Exception property to see details", ex);
            }
            finally
            {
                //context.Dispose();
            }



        }

        public List<string> GetFileExtList()
        {
            return fileExtList;
        }

        private void LogsActivity(REPLACEMENT_DOCUMENTS data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    foreach (var map in changes)
                    {
                        refService.AddChangeLog(context,
                            formType,
                            data.FORM_ID.ToString(),
                            map.Key,
                            map.Value[0],
                            map.Value[1],
                           actor,
                           DateTime.Now
                            );
                    }

                    refService.AddWorkflowHistory(context,
                        formType,
                        Convert.ToInt64(data.FORM_ID),
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
                    throw this.HandleException("Exception occured on Manufacture License Change Request Save Log. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }

            }



        }

        public void LogsPrintActivity(long Id, int formType, string actor)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    refService.AddChangeLog(context,
                        formType,
                        Id.ToString(),
                        "DOWNLOAD PRINT OUT",
                        "",
                        "",
                        actor,
                        DateTime.Now
                        );

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Change Document Request Service. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }

            }



        }

        public void LogsChanges(long Id, Dictionary<string, string[]> changes, int formType, string actor)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
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
                finally
                {
                    context.Dispose();
                }

            }



        }

        private REPLACEMENT_DOCUMENTS SetOldValueToTempModel(REPLACEMENT_DOCUMENTS CRData)
        {
            var OldCRequest = new REPLACEMENT_DOCUMENTS()
            {
                FORM_ID = CRData.FORM_ID,
                FORM_NO = CRData.FORM_NO,
                REQUEST_DATE = CRData.REQUEST_DATE,
                DOCUMENT_TYPE = CRData.DOCUMENT_TYPE,
                NPPBKC_ID = CRData.NPPBKC_ID,
                CREATED_BY = CRData.CREATED_BY,
                CREATED_DATE = CRData.CREATED_DATE,
                LASTMODIFIED_BY = CRData.LASTMODIFIED_BY,
                LASTMODIFIED_DATE = CRData.LASTMODIFIED_DATE,
                LASTAPPROVED_BY = CRData.LASTAPPROVED_BY,
                LASTAPPROVED_DATE = CRData.LASTAPPROVED_DATE,
                LASTAPPROVED_STATUS = CRData.LASTAPPROVED_STATUS,
                DECREE_STATUS = CRData.DECREE_STATUS,
                DECREE_NUMBER = CRData.DECREE_NUMBER
            };
            return OldCRequest;
        }


        private Dictionary<string, string[]> GetAllChanges(REPLACEMENT_DOCUMENTS old, REPLACEMENT_DOCUMENTS updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                if (old.FORM_NO != updated.FORM_NO)
                {
                    var oldvalue = old.FORM_NO == null ? "N/A" : old.FORM_NO.ToString();
                    var newvalue = updated.FORM_NO == null ? "N/A" : updated.FORM_NO.ToString();
                    changes.Add("FORM_NO", new string[] { oldvalue, newvalue });
                }
                if (old.REQUEST_DATE != updated.REQUEST_DATE)
                {
                    var oldvalue = old.REQUEST_DATE == DateTime.MinValue ? "N/A" : old.REQUEST_DATE.ToString("dd MMMM yyyy");
                    var newvalue = updated.REQUEST_DATE == DateTime.MinValue ? "N/A" : updated.REQUEST_DATE.ToString("dd MMMM yyyy");
                    changes.Add("REQUEST_DATE", new string[] { oldvalue, newvalue });
                }
                if (old.DOCUMENT_TYPE != updated.DOCUMENT_TYPE)
                {
                    var oldvalue = old.DOCUMENT_TYPE == null ? "N/A" : old.DOCUMENT_TYPE.ToString();
                    var newvalue = updated.DOCUMENT_TYPE == null ? "N/A" : updated.DOCUMENT_TYPE.ToString();
                    changes.Add("DOCUMENT_TYPE", new string[] { oldvalue, newvalue });
                }
                if (old.NPPBKC_ID != updated.NPPBKC_ID)
                {
                    var oldvalue = old.NPPBKC_ID == null ? "N/A" : old.NPPBKC_ID.ToString();
                    var newvalue = updated.NPPBKC_ID == null ? "N/A" : updated.NPPBKC_ID.ToString();
                    changes.Add("NPPBKC_ID", new string[] { oldvalue, newvalue });
                }
                //if (old.CREATED_BY != updated.CREATED_BY)
                //{
                //    var oldvalue = old.CREATED_BY == null ? "N/A" : old.CREATED_BY.ToString();
                //    var newvalue = updated.CREATED_BY == null ? "N/A" : updated.CREATED_BY.ToString();
                //    changes.Add("CREATED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.CREATED_DATE != updated.CREATED_DATE)
                //{
                //    var oldvalue = old.CREATED_DATE == null ? "N/A" : old.CREATED_DATE.ToString();
                //    var newvalue = updated.CREATED_DATE == null ? "N/A" : updated.CREATED_DATE.ToString();
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
                //    var oldvalue = old.LASTMODIFIED_DATE == null ? "N/A" : old.LASTMODIFIED_DATE.ToString();
                //    var newvalue = updated.LASTMODIFIED_DATE == null ? "N/A" : updated.LASTMODIFIED_DATE.ToString();
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
                //    var oldvalue = old.LASTAPPROVED_DATE == null ? "N/A" : old.LASTAPPROVED_DATE.ToString();
                //    var newvalue = updated.LASTAPPROVED_DATE == null ? "N/A" : updated.LASTAPPROVED_DATE.ToString();
                //    changes.Add("LASTAPPROVED_DATE", new string[] { oldvalue, newvalue });
                //}
                if (old.LASTAPPROVED_STATUS != updated.LASTAPPROVED_STATUS)
                {
                    var oldvalue = old.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(old.LASTAPPROVED_STATUS).REFF_VALUE;
                    var newvalue = updated.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(updated.LASTAPPROVED_STATUS).REFF_VALUE;
                    changes.Add("LASTAPPROVED_STATUS", new string[] { oldvalue, newvalue });
                }
                if (old.LASTAPPROVED_STATUS == refService.GetRefByKey("WAITING_GOVERNMENT_APPROVAL").REFF_ID)
                {
                    var oldvalue1 = "N/A";
                    var newvalue1 = updated.DECREE_STATUS == null ? "N/A" : updated.DECREE_STATUS == true ? "Approved" : "Rejected";
                    changes.Add("DECREE_STATUS", new string[] { oldvalue1, newvalue1 });

                    //if (old.DECREE_STATUS != updated.DECREE_STATUS)
                    //{
                    //    var oldvalue = old.DECREE_STATUS == null ? "N/A" : old.DECREE_STATUS == true ? "Approved" : "Rejected";
                    //    var newvalue = updated.DECREE_STATUS == null ? "N/A" : updated.DECREE_STATUS == true ? "Approved" : "Rejected";
                    //    changes.Add("DECREE_STATUS", new string[] { oldvalue, newvalue });
                    //}
                    if (old.DECREE_NUMBER != updated.DECREE_NUMBER)
                    {
                        var oldvalue = old.DECREE_NUMBER == null ? "N/A" : old.DECREE_NUMBER;
                        var newvalue = updated.DECREE_NUMBER == null ? "N/A" : updated.DECREE_NUMBER;
                        changes.Add("DECREE_NUMBER", new string[] { oldvalue, newvalue });
                    }

                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Change Request Get Change For Log. See Inner Exception property to see details", ex);
            }
        }

        private Dictionary<string, string[]> GetAllDetailChanges(REPLACEMENT_DOCUMENTS_DETAIL old, REPLACEMENT_DOCUMENTS_DETAIL updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();

                if (old.UPDATE_NOTES != updated.UPDATE_NOTES)
                {
                    var oldvalue = old.UPDATE_NOTES == null ? "N/A" : old.UPDATE_NOTES.ToString();
                    var newvalue = updated.UPDATE_NOTES == null ? "N/A" : updated.UPDATE_NOTES.ToString();
                    changes.Add("Item Update", new string[] { oldvalue, newvalue });
                }

                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture Change Request Get Change For Log. See Inner Exception property to see details", ex);
            }
        }

        public POA_DELEGATION GetPOADelegationOfUser(string UserId)
        {
            using (var context = new EMSDataModel())
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
                finally
                {
                    context.Dispose();
                }
            }
        }

        public List<POA> GetPOAApproverList(long IRId)
        {
            using (var context = new EMSDataModel())
            {

                try
                {
                    var ListPOA = new List<POA>();
                    var RealListPOA = new List<POA>();
                    var IRequest = GetChangeRequestById(IRId).FirstOrDefault();
                    if (IRequest != null)
                    {
                        var NPPBKCId = IRequest.NPPBKC_ID;
                        if (IRequest.SYS_REFFERENCES.REFF_KEYS != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval) && IRequest.LASTAPPROVED_BY == null)
                        {
                            var ListPOA_Nppbkc = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true && w.POA_ID != IRequest.CREATED_BY).Select(s => s.POA_ID).ToList();
                            var OriexcisePOA = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true).Select(s => s.POA_ID).ToList();
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
                finally
                {
                    context.Dispose();
                }
            }
        }

        public List<POA> GetPOAInNPPBKCList(string NPPBKCId)
        {
            using (var context = new EMSDataModel())
            {

                try
                {
                    var ListPOA = new List<POA>();

                    var ListPOA_Nppbkc = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true).Select(s => s.POA_ID).ToList();
                    foreach (var poaresult in ListPOA_Nppbkc)
                    {
                        ListPOA.Add(new POA
                        {
                            POA_ID = poaresult,
                        });
                        var poadelegate = GetPOADelegationOfUser(poaresult);
                        if (poadelegate != null)
                        {
                            ListPOA.Add(new POA
                            {
                                POA_ID = poadelegate.POA_TO
                            });
                        }
                    }

                    return ListPOA;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Get POA Approver. See Inner Exception property to see details", ex);
                }
                finally
                {
                    //context.Dispose();
                }
            }
        }

        public List<string> GetNPPBKCByUser(string UserId)
        {
            using (var con = new EMSDataModel())
            {
                try
                {
                    var ListNPPBKC = new List<string>();
                    var ListNPPBKC_Raw = con.POA_MAP.Where(w => w.POA.IS_ACTIVE == true && w.POA_ID.Equals(UserId));
                    if (ListNPPBKC_Raw.Any())
                    {
                        foreach (var dataResult in ListNPPBKC_Raw)
                        {
                            ListNPPBKC.Add(dataResult.NPPBKC_ID);
                        }
                    }
                    //var poadelegate = GetPOADelegationOfUser(UserId);
                    //if (poadelegate != null)
                    //{
                    //    ListNPPBKC.Add(poadelegate.POA_FROM);
                    //}

                    if (ListNPPBKC.Count() > 0)
                    {
                        ListNPPBKC = ListNPPBKC.Distinct().ToList();
                    }
                    return ListNPPBKC;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Change Request Get NPPBKC from POA Map. See Inner Exception property to see details", ex);
                }
                finally
                {
                    con.Dispose();
                }

            }

        }

        public MASTER_NPPBKC GetNppbkc(object id)
        {
            using (var con = new EMSDataModel())
            {
                try
                {
                    var nppbkc = con.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID == id.ToString()).FirstOrDefault();
                    var plants = con.T001W.Where(w => w.NPPBKC_ID == nppbkc.NPPBKC_ID).Select(s => s.WERKS).ToList();
                    var plantsMap = con.T001K.Where(w => plants.Contains(w.BWKEY)).FirstOrDefault();
                    var companyMap = con.T001.Where(w => w.BUKRS == plantsMap.BUKRS).FirstOrDefault();
                    nppbkc.COMPANY = companyMap;

                    //var nppbkc = this.uow.NppbkcRepository.Find(id);
                    //var plants = this.uow.PlantRepository.GetMany(x => id.ToString() == nppbkc.NPPBKC_ID).Select(x => x.WERKS);
                    //var plantsMap = this.uow.CompanyPlantMappingRepository.GetManyQueryable(x => plants.Contains(x.BWKEY)).FirstOrDefault();
                    //nppbkc.COMPANY = (plantsMap != null) ? plantsMap.COMPANY : null;
                    return nppbkc;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
                }
                finally
                {
                    //con.Dispose();
                }
            }
        }

        public List<MASTER_PLANT> GetPlantByNPPBKC(string NPPBKCId)
        {
            using (var context = new EMSDataModel())
            {

                try
                {
                    var nppbkc = context.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID == NPPBKCId.ToString()).FirstOrDefault();
                    var plants = context.T001W.Where(w => w.NPPBKC_ID == nppbkc.NPPBKC_ID).ToList();
                    return plants;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }

        }

        #endregion




    }
}
