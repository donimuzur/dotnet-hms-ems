using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sampoerna.EMS.CustomService.Services
{
    public class SystemReferenceService : GenericService
    {
        public SystemReferenceService() : base()
        {

        }

        #region Global Reference Lookup Methods

        public IQueryable<SYS_REFFERENCES> GetRefByType(string type)
        {
            try
            {
                return this.uow.ReferenceRepository.GetManyQueryable(reff => reff.REFF_TYPE == type.ToUpper() && reff.IS_ACTIVE.HasValue && reff.IS_ACTIVE.Value);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public SYS_REFFERENCES GetRefByKey(string key)
        {
            try
            {
                //return this.uow.ReferenceRepository.GetFirst(reff => reff.REFF_KEYS == key.ToUpper());
                return this.uow.ReferenceRepository.GetFirst(reff => reff.REFF_KEYS == key.ToUpper() && reff.IS_ACTIVE.HasValue && reff.IS_ACTIVE.Value);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        #endregion

        #region Approval Status Lookup
        public IQueryable<SYS_REFFERENCES> GetApprovalStatusList()
        {
            try
            {
                string key = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.KeyGroup.ApprovalStatus);
                return this.GetRefByType(key);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<SYS_REFFERENCES> GetAdminApprovers()
        {
            try
            {
                string key = ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.KeyGroup.ApproverUser);
                return this.uow.ReferenceRepository.GetMany(x => x.REFF_KEYS.ToUpper().Trim() == key && x.IS_ACTIVE.HasValue && x.IS_ACTIVE.Value);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<USER> GetAdmins()
        {
            try
            {
                List<string> admins = this.uow.RoleMapRepository.GetMany(x => x.ROLEID.Value == (int)Enums.UserRole.Administrator).Select(y => y.MSACCT.Trim().ToUpper()).ToList();

                return this.uow.UserRepository.GetMany(x => admins.Contains(x.USER_ID.Trim().ToUpper()));

            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }
        public SYS_REFFERENCES GetReferenceByKey(Enum key)
        {
            try
            {
                string keyString = ReferenceLookup.Instance.GetReferenceKey(key);
                return this.GetRefByKey(keyString);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }
        #endregion

        #region Changes History Lookup
        public IQueryable<CHANGES_HISTORY> GetChangesHistory(int formTypeId, string id)
        {
            try
            {
                return this.uow.ChangeLogRepository.GetManyQueryable(c => c.FORM_TYPE_ID == formTypeId && c.FORM_ID == id && c.NEW_VALUE != null).OrderByDescending(c => c.MODIFIED_DATE);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<WORKFLOW_HISTORY> GetWorkflowHistory(int formTypeId, long id)
        {
            try
            {
                return this.uow.WorkflowHistoryRepository.GetManyQueryable(c => c.FORM_TYPE_ID == formTypeId && c.FORM_ID == id).OrderBy(c => c.ACTION_DATE);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public void AddChangeLog(EMSDataModel context, int formType, string formId, string field, string oldVal, string newVal, string actor, DateTime? logTime)
        {
            try
            {
                CHANGES_HISTORY log = new CHANGES_HISTORY()
                {
                    FORM_TYPE_ID = formType,
                    FORM_ID = formId,
                    FIELD_NAME = field,
                    OLD_VALUE = oldVal,
                    NEW_VALUE = newVal,
                    MODIFIED_BY = actor,
                    MODIFIED_DATE = logTime
                };

                context.CHANGES_HISTORY.Add(log);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);

            }
        }

        public void AddWorkflowHistory(EMSDataModel context, int formType, long formId, int action, string actor, DateTime? logTime, int role, string comment = null, string formNumber = null)
        {
            try
            {
                WORKFLOW_HISTORY history = new WORKFLOW_HISTORY()
                {
                    FORM_TYPE_ID = formType,
                    FORM_ID = formId,
                    ACTION = action,
                    ACTION_BY = actor,
                    ACTION_DATE = logTime,
                    ROLE = role,
                    COMMENT = comment,
                    FORM_NUMBER = formNumber
                };
                context.WORKFLOW_HISTORY.Add(history);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public SYS_REFFERENCES GetReferenceById(long Id)
        {
            try
            {
                return this.uow.ReferenceRepository.Find(Id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }
        #endregion

        #region Email Global Lookup
        public string GetUserEmail(string userId)
        {
            try
            {
                var user = this.uow.UserRepository.Find(userId);

                return (user != null) ? user.EMAIL : null;

            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }

        }

        public string GetPOAEmail(string poaId)
        {
            try
            {
                var poa = this.uow.POARepository.Find(poaId);

                return (poa != null) ? poa.POA_EMAIL : null;

            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }

        }

        public CONTENTEMAIL GetMailContent(long id, Dictionary<string, string> parameters)
        {
            try
            {
                var mailContent = this.uow.EmailContentRepository.Find(id);

                if (mailContent == null)
                    throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", new Exception("Invalid mail content ID!"));

                var content = mailContent.EMAILCONTENT;
                var variables = mailContent.EMAILVARIABELS;
                var subject = mailContent.EMAILSUBJECT;
                foreach (var variable in variables)
                {
                    content = content.Replace(String.Format("#{0}", variable.NAME), parameters[variable.NAME]);
                    subject = subject.Replace(String.Format("#{0}", variable.NAME), parameters[variable.NAME]);
                }
                mailContent.EMAILCONTENT = content;
                mailContent.EMAILSUBJECT = subject;



                return mailContent;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }
        #endregion

        #region Global Helpers
        public Dictionary<string, string[]> TraceChanges(object original, object changed, string[] necesarryColumns = null)
        {
            try
            {
                bool useFilter = necesarryColumns != null;
                if (original.GetType() != changed.GetType())
                {
                    throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", new ArgumentException("Incompatible argument types"));
                }
                var propsOld = new Dictionary<string, object>();
                var props = new Dictionary<string, object>();
                Dictionary<string, string[]> changes = new Dictionary<string, string[]>();

                foreach (var prop in original.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    propsOld.Add(prop.Name, prop.GetValue(original, null));
                    props.Add(prop.Name, prop.GetValue(changed, null));
                }
                foreach (var map in props)
                {
                    if (useFilter)
                    {
                        if (!necesarryColumns.Contains(map.Key))
                            continue;
                    }
                    if (map.Value != null && !map.Value.Equals(propsOld[map.Key]))
                    {
                        var stringifiedValue = map.Value.ToString().Trim();
                        var oldValue = (propsOld[map.Key] != null) ? propsOld[map.Key].ToString().Trim() : null;

                        if ((map.Value is IEnumerable && !(map.Value is String)) || map.Value.ToString().Contains("System") || map.Value.ToString().Contains("Sampoerna.EMS"))
                            continue;
                        if (map.Value is decimal)
                        {
                            oldValue = ((decimal)propsOld[map.Key]).ToString("C2");
                            stringifiedValue = ((decimal)map.Value).ToString("C2");
                        }

                        if (oldValue != stringifiedValue)
                        {
                            changes.Add(map.Key, new string[] { oldValue, stringifiedValue });
                        }
                    }
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }

        }

        public Dictionary<String, String[]> TraceAllChanges(Dictionary<string, string> old, Dictionary<string, string> changed)
        {
            try
            {
                Dictionary<string, string[]> changes = new Dictionary<string, string[]>();

                foreach (var map in changed)
                {
                    if (!old.ContainsKey(map.Key)) continue;

                    if (old[map.Key] != map.Value)
                        changes.Add(map.Key, new string[] { old[map.Key], map.Value });
                }

                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public void LogsActivity(EMSDataModel context, string id, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null, string formNumber = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    this.AddChangeLog(context,
                        formType,
                        id,
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                this.AddWorkflowHistory(context,
                    formType,
                    Convert.ToInt64(id),
                    actionType,
                    actor,
                    DateTime.Now,
                    role,
                    comment,
                    formNumber
                    );
                context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
            }

        }

        public bool IsAdminApprover(string userId)
        {
            var refApprover = this.GetAdminApprovers().ToList();

            if (refApprover != null)
            {
                foreach (var approver in refApprover)
                {
                    if (approver.REFF_VALUE == null)
                        continue;

                    if (userId.Trim().ToUpper() == approver.REFF_VALUE.Trim().ToUpper())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public long GetCurrentSequence(Enums.FormType formtype, EMSDataModel context, int? month = null, int? year = null)
        {
            try
            {
                var current = 0L;
                var sequencePerForm = context.DOC_NUMBER_SEQ.Where(x => !x.FORM_TYPE_ID.HasValue).FirstOrDefault();

                if (sequencePerForm == null) // Not present yet
                {
                    var sequence = new DOC_NUMBER_SEQ()
                    {
                        DOC_NUMBER_SEQ_LAST = current,
                        FORM_TYPE_ID = (int)formtype,
                        MONTH = month,
                        YEAR = year
                    };

                    context.DOC_NUMBER_SEQ.Add(sequence);
                }
                else
                {
                    var newSequence = (DOC_NUMBER_SEQ)context.Entry(sequencePerForm).GetDatabaseValues().ToObject();
                    current = sequencePerForm.DOC_NUMBER_SEQ_LAST;
                    newSequence.DOC_NUMBER_SEQ_LAST = current + 1;
                    context.Entry(sequencePerForm).CurrentValues.SetValues(newSequence);
                }
                context.SaveChanges();

                return current + 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Global Data Provider
        public IEnumerable<MASTER_NPPBKC> GetAllNppbkc()
        {
            try
            {
                return this.uow.NppbkcRepository.GetAll().Where(data => !data.IS_DELETED.HasValue || !data.IS_DELETED.Value);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<MASTER_NPPBKC> GetAllNppbkc(string poa)
        {
            try
            {
                var POA = this.GetPOA(poa);
                if (POA == null)
                    return new List<MASTER_NPPBKC>();

                var poaMaps = POA.POA_MAPS.ToList();
                var poaNppbkc = poaMaps.Where(x => x.POA_ID == poa).Select(x => x.NPPBKC_ID);
                return this.uow.NppbkcRepository.GetAll().Where(data => (!data.IS_DELETED.HasValue || !data.IS_DELETED.Value) && poaNppbkc.Contains(data.NPPBKC_ID));
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_NPPBKC GetNppbkc(object id)
        {
            try
            {
                var nppbkc = this.uow.NppbkcRepository.Find(id);
                var plants = this.uow.PlantRepository.GetMany(x => x.NPPBKC_ID == nppbkc.NPPBKC_ID).Select(x => x.WERKS).ToList();
                var plantsMap = this.uow.CompanyPlantMappingRepository.GetManyQueryable(x => plants.Contains(x.BWKEY)).ToList();
                var map = plantsMap.FirstOrDefault();
                nppbkc.COMPANY = (map != null) ? this.uow.CompanyRepository.Find(map.BUKRS) : null;
                return nppbkc;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<POA> GetAllPOA(string nppbkcId)
        {
            try
            {
                return this.uow.POARepository.GetMany(poa => poa.POA_MAPS.Where(x => x.NPPBKC_ID == nppbkcId).Count() > 0);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<POA> GetAllPOA()
        {
            try
            {
                return this.uow.POARepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<string> GetPOAEmails(string sender)
        {
            try
            {
                return this.uow.POARepository.GetMany(x => x.POA_EMAIL != sender).Select(x => x.POA_EMAIL);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public POA GetPOA(object id)
        {
            try
            {
                return this.uow.POARepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<USER> GetAllUser()
        {
            try
            {
                return this.uow.UserRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public USER GetUser(object id)
        {
            try
            {
                return this.uow.UserRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<MASTER_SUPPORTING_DOCUMENT> GetSupportingDocuments(long formId, string company)
        {
            try
            {
                var approved = GetRefByKey(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Completed));
                return this.uow.SupportDocRepository.GetMany(
                    x => x.FORM_ID == formId &&
                    x.LASTAPPROVED_STATUS == approved.REFF_ID &&
                    x.IS_ACTIVE &&
                    x.BUKRS == company);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<MASTER_SUPPORTING_DOCUMENT> GetSupportingDocuments(int formType, string company, string formId)
        {
            try
            {
                var supportingDocs = GetSupportingDocuments(formType, company).ToList();
                var result = new List<MASTER_SUPPORTING_DOCUMENT>();
                foreach (var data in supportingDocs)
                {
                    data.FILE_UPLOAD = GetUploadedFiles(formType, formId).Where(x => x.DOCUMENT_ID == data.DOCUMENT_ID).ToList();
                    result.Add(data);
                }
                return result;

            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<FILE_UPLOAD> GetUploadedFiles(int formType, string formId)
        {
            try
            {
                var approved = GetRefByKey(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Completed));
                return this.uow.FileUploadRepository.GetMany(
                    x => x.FORM_TYPE_ID == formType &&
                    x.IS_ACTIVE &&
                    x.FORM_ID == formId).OrderByDescending(x => x.UPLOAD_DATE);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }


        public IEnumerable<MASTER_SUPPORTING_DOCUMENT> GetSupportingDocuments(long formId)
        {
            try
            {
                var approved = GetRefByKey(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Completed));
                return this.uow.SupportDocRepository.GetMany(
                    x => x.FORM_ID == formId &&
                    x.LASTAPPROVED_STATUS == approved.REFF_ID &&
                    x.IS_ACTIVE);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }
        public MASTER_SUPPORTING_DOCUMENT GetSupportingDocument(long id)
        {
            try
            {
                var approved = GetRefByKey(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Completed));
                return this.uow.SupportDocRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }


        public SYS_REFFERENCES GetUploadFileSizeLimit()
        {
            try
            {
                return this.GetReferenceByKey(ReferenceKeys.UploadFileLimit.General);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService.GetUploadFileSizeLimit() See Inner Exception property to see details", ex);
            }
        }

        public bool SaveDocument(int formType, string formId, string url, string user, long? docId = null, bool isGovDocument = false)
        {
            try
            {
                var now = new DateTime();
                FILE_UPLOAD file = new FILE_UPLOAD()
                {
                    FORM_TYPE_ID = formType,
                    FORM_ID = formId,
                    PATH_URL = url,
                    UPLOAD_DATE = now,
                    CREATED_BY = user,
                    LASTMODIFIED_BY = user,
                    CREATED_DATE = now,
                    LASTMODIFIED_DATE = now,
                    DOCUMENT_ID = docId,
                    IS_GOVERNMENT_DOC = isGovDocument,
                    IS_ACTIVE = true
                };
                return true;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }
        public IEnumerable<ROLE_ADMIN_APPROVER_VIEW> GetAuthorizedPages(string userId)
        {
            try
            {
                return this.uow.RoleAdminApprovalRepository.GetMany(entity => entity.USER_ID.ToUpper() == userId.ToUpper());
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService.GetAuthorizedPages(). See Inner Exception property to see details", ex);
            }
        }

        public T001 GetCompanyById(string BUKRS)
        {
            try
            {
                var context = new EMSDataModel();
                var company = context.T001.Where(w => w.BUKRS.Equals(BUKRS)).FirstOrDefault();
                return company;
            }
            catch (Exception e)
            {
                throw; this.HandleException("Exception occured on SystemReferenceService.GetCompanyById(). See Inner Exception property to see details", e);
            }
        }
        #endregion

        #region File Upload
        public bool SaveUploadedFiles(IEnumerable<FILE_UPLOAD> files)
        {
            try
            {
                using (var context = new EMSDataModel())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach (var item in files)
                        {
                            context.FILE_UPLOAD.Add(item);
                        }
                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                throw this.HandleException("Exception occured on SystemReferenceService.SaveUploadedFiles(). See Inner Exception property to see details", ex);
            }
        }

        public bool RemoveUploadedFiles(IEnumerable<Int64> fileIds)
        {
            try
            {
                if (fileIds == null || fileIds.Count() <= 0) // do nothing
                    return true;

                using (var context = new EMSDataModel())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach (var id in fileIds)
                        {
                            var fileUpload = context.FILE_UPLOAD.Find(id);
                            if (fileUpload != null)
                            {
                                fileUpload.IS_ACTIVE = false;

                                context.SaveChanges();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                throw this.HandleException("Exception occured on SystemReferenceService.SaveUploadedFiles(). See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<POA> GetDelegatedPOA(string poaId)
        {
            try
            {
                var today = DateTime.Today;
                var delegations = this.uow.PoaDelegationRepository.GetMany(x => x.POA_FROM.ToUpper() == poaId.ToUpper() && x.DATE_TO > today).Select(x => x.POA_TO);
                if (delegations == null || !delegations.Any())
                    return null;
                return this.uow.POARepository.GetMany(x => delegations.Contains(x.POA_ID));
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService.SaveUploadedFiles(). See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<POA> GetPOADelegatedMe(string poaId)
        {
            try
            {
                var today = DateTime.Today;
                var delegations = this.uow.PoaDelegationRepository.GetMany(x => x.POA_TO.ToUpper() == poaId.ToUpper() && x.DATE_TO > today).Select(x => x.POA_FROM).ToList();
                if (delegations == null || !delegations.Any())
                    return null;

                return this.uow.POARepository.GetMany(x => delegations.Contains(x.POA_ID));
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService.SaveUploadedFiles(). See Inner Exception property to see details", ex);
            }

        }

        public string GetPOANppbkc(string POA)
        {
            return this.uow.PoaMapRepository.GetMany(x => x.POA_ID == POA).FirstOrDefault().NPPBKC_ID;
        }

        public IEnumerable<POA> GetPOAWithinNPPBKC(string poaId)
        {
            try
            {
                var nppbkc = this.uow.PoaMapRepository.GetMany(x => x.POA_ID == poaId).FirstOrDefault();
                if (nppbkc == null)
                    return null;
                var poaMaps = this.uow.PoaMapRepository.GetMany(x => x.NPPBKC_ID == nppbkc.NPPBKC_ID).Select(x => x.POA_ID).ToList();
                List<string> poaCreators = new List<string>();
                poaCreators.Add(poaId);
                var delegatedPOAs = GetDelegatedPOA(poaId);
                if (delegatedPOAs != null && delegatedPOAs.Any())
                {
                    poaCreators.AddRange(delegatedPOAs.Select(x => x.POA_ID));
                }
                var poaDelegatedMe = GetPOADelegatedMe(poaId);
                if (poaDelegatedMe != null && poaDelegatedMe.Any())
                {
                    poaCreators.AddRange(poaDelegatedMe.Select(x => x.POA_ID));
                }

                var poaList = this.uow.POARepository.GetMany(x => poaMaps.Contains(x.POA_ID) && !poaCreators.Contains(x.POA_ID) && x.IS_ACTIVE.HasValue && x.IS_ACTIVE.Value).ToList();
                var poaExicers = this.uow.PoaExciserRepository.GetMany(x => x.IS_ACTIVE_EXCISER).Select(x => x.POA_ID).ToList();
                poaList = poaList.Where(x => poaExicers.Contains(x.POA_ID)).ToList();
                //return this.uow.POARepository.GetMany(x => poaMaps.Contains(x.POA_ID) && !poaCreators.Contains(x.POA_ID) && x.POA_EXCISER != null && x.POA_EXCISER.Where(y => y.IS_ACTIVE_EXCISER).Any());

                return poaList;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService.SaveUploadedFiles(). See Inner Exception property to see details", ex);
            }
        }

        public USER_PRINTOUT_LAYOUT InsertUserPrintOutLayout(string PrintoutName, string UserID)
        {
            try
            {
                var context = new EMSDataModel();
                var UserPrintout = new USER_PRINTOUT_LAYOUT();
                var Printouts = context.PRINTOUT_LAYOUT.Where(w => w.NAME == PrintoutName);
                if (Printouts.Any())
                {
                    var Printout = Printouts.FirstOrDefault();
                    UserPrintout.PRINTOUT_LAYOUT_ID = Printout.PRINTOUT_LAYOUT_ID;
                    UserPrintout.USER_ID = UserID;
                    UserPrintout.LAYOUT = Printout.LAYOUT;
                    UserPrintout.MODIFIED_DATE = DateTime.Now;
                    context.USER_PRINTOUT_LAYOUT.Add(UserPrintout);
                    context.SaveChanges();
                }
                return UserPrintout;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Insert Printout Layout. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<USER_PRINTOUT_LAYOUT> GetPrintoutLayout(string PrintoutName, string UserID)
        {
            try
            {
                var context = new EMSDataModel();
                var listLayout = context.USER_PRINTOUT_LAYOUT.Where(w => w.PRINTOUT_LAYOUT.NAME == PrintoutName && w.USER_ID == UserID);
                if (!listLayout.Any())
                {
                    var InsertNew = InsertUserPrintOutLayout(PrintoutName, UserID);
                    if (InsertNew != null)
                    {
                        listLayout = GetPrintoutLayout(PrintoutName, UserID);
                    }
                }
                return listLayout;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Get Printout Layout. See Inner Exception property to see details", ex);
            }
        }

        public USER_PRINTOUT_LAYOUT GeneratePrintout(string PrintoutName, Dictionary<string, string> Parameters, string UserID)
        {
            try
            {
                var context = new EMSDataModel();
                var PrintoutLayout = new USER_PRINTOUT_LAYOUT();
                var printoutvariables = context.PRINTOUT_VARIABLE.Where(w => w.PRINTOUT_LAYOUT.NAME == PrintoutName);
                PrintoutLayout = GetPrintoutLayout(PrintoutName, UserID).FirstOrDefault();
                var layout = PrintoutLayout.LAYOUT;
                foreach (var variable in printoutvariables)
                {
                    layout = layout.Replace(String.Format("#{0}", variable.NAME), Parameters[variable.NAME]);
                }
                PrintoutLayout.LAYOUT = layout;
                return PrintoutLayout;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Generate Printout. See Inner Exception property to see details", ex);
            }
        }

        public string UpdatePrintoutLayout(string PrintoutName, string NewLayout, string UserID)
        {
            try
            {
                var message = "";
                var context = new EMSDataModel();
                var printout = new USER_PRINTOUT_LAYOUT();
                var printoutvariable = context.PRINTOUT_VARIABLE.Where(w => w.PRINTOUT_LAYOUT.NAME == PrintoutName);
                if (printoutvariable.Any())
                {
                    printout = context.USER_PRINTOUT_LAYOUT.Where(w => w.PRINTOUT_LAYOUT.NAME == PrintoutName && w.USER_ID == UserID).FirstOrDefault();
                    var isVariableOk = true;
                    var layout = printout.LAYOUT;
                    foreach (var variable in printoutvariable)
                    {
                        if (!NewLayout.Contains(variable.NAME))
                        {
                            isVariableOk = false;
                        }
                    }
                    isVariableOk = true;
                    if (isVariableOk)
                    {
                        printout.LAYOUT = NewLayout;
                        printout.MODIFIED_DATE = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                    {
                        message = "The variables are incomplete. Please make sure the variables are not deleted.";
                    }
                }
                else
                {
                    message = "Cannot find variables in database.";
                }
                return message;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Update Printout Layout. See Inner Exception property to see details", ex);
            }
        }

        public string RestorePrintoutToDefault(string PrintoutName, string UserID)
        {
            try
            {
                var context = new EMSDataModel();
                var message = "";
                var Printout = context.PRINTOUT_LAYOUT.Where(w => w.NAME == PrintoutName);
                if (Printout.Any())
                {
                    var DefaultLayout = Printout.FirstOrDefault().LAYOUT;
                    message = UpdatePrintoutLayout(PrintoutName, DefaultLayout, UserID);
                }
                else
                {
                    message = "Original Printout Template is not Available.";
                }
                return message;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Set Default Printout Layout. See Inner Exception property to see details", ex);
            }
        }

        public WORKFLOW_HISTORY GetAdminApproverList()
        {
            try
            {
                WORKFLOW_HISTORY additional = new WORKFLOW_HISTORY();

                var approvers = this.GetAdminApprovers();
                approvers = approvers.Distinct();
                var accounts = "";
                foreach (var approver in approvers)
                {
                    if (accounts == "")
                    {
                        accounts += approver.REFF_VALUE;
                    }
                    else
                    {
                        accounts += ", " + approver.REFF_VALUE;
                    }
                }

                additional.ACTION_BY = accounts;
                additional.ACTION = (int)Enums.ActionType.WaitingForApproval;
                additional.ROLE = (int)Enums.UserRole.AdminApprover;

                return additional;

            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Set Default Printout Layout. See Inner Exception property to see details", ex);
            }
        }

        #region Old PrintOut

        public IQueryable<PRINTOUT_LAYOUT> GetPrintoutLayout_Old(string PrintoutName)
        {
            try
            {
                var context = new EMSDataModel();
                var listLayout = context.PRINTOUT_LAYOUT.Where(w => w.NAME == PrintoutName);
                return listLayout;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Get Printout Layout. See Inner Exception property to see details", ex);
            }
        }

        public PRINTOUT_LAYOUT GeneratePrintout_Old(string PrintoutName, Dictionary<string, string> Parameters)
        {
            try
            {
                var context = new EMSDataModel();
                var PrintoutLayout = new PRINTOUT_LAYOUT();
                var printoutvariables = context.PRINTOUT_VARIABLE.Where(w => w.PRINTOUT_LAYOUT.NAME == PrintoutName);
                PrintoutLayout = GetPrintoutLayout_Old(PrintoutName).FirstOrDefault();
                var layout = PrintoutLayout.LAYOUT;
                foreach (var variable in printoutvariables)
                {
                    layout = layout.Replace(String.Format("#{0}", variable.NAME), Parameters[variable.NAME]);
                }
                PrintoutLayout.LAYOUT = layout;
                return PrintoutLayout;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Generate Printout. See Inner Exception property to see details", ex);
            }
        }

        public string UpdatePrintoutLayout_Old(string PrintoutName, string NewLayout)
        {
            try
            {
                var message = "";
                var context = new EMSDataModel();
                var printout = new PRINTOUT_LAYOUT();
                var printoutvariable = context.PRINTOUT_VARIABLE.Where(w => w.PRINTOUT_LAYOUT.NAME == PrintoutName);
                if (printoutvariable.Any())
                {
                    printout = printoutvariable.Select(s => s.PRINTOUT_LAYOUT).FirstOrDefault();
                    var isVariableOk = true;
                    var layout = printout.LAYOUT;
                    foreach (var variable in printoutvariable)
                    {
                        if (!layout.Contains(variable.NAME))
                        {
                            isVariableOk = false;
                        }
                    }
                    if (isVariableOk)
                    {
                        printout.LAYOUT = NewLayout;
                        context.SaveChanges();
                    }
                    else
                    {
                        message = "The variables are incomplete. Please make sure the variables are not deleted.";
                    }
                }
                else
                {
                    message = "Cannot find variables in database.";
                }
                return message;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Update Printout Layout. See Inner Exception property to see details", ex);
            }
        }

        #endregion

        #endregion



    }
}
