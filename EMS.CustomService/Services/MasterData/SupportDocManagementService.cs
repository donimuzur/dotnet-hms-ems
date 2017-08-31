using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;

namespace Sampoerna.EMS.CustomService.Services.MasterData
{
    public class SupportDocManagementService : GenericService
    {
        private SystemReferenceService refService;
        public SupportDocManagementService() : base()
        {
            this.refService = new SystemReferenceService();
        }

        #region Get Data

        public IQueryable<MASTER_SUPPORTING_DOCUMENT> GetAll()
        {
            try
            {
                return this.uow.SupportDocRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<T001> GetCompanies()
        {
            try
            {
                var result = this.uow.CompanyRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }
        }

        public T001 GetCompany(string id)
        {
            try
            {
                return this.uow.CompanyRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_SUPPORTING_DOCUMENT Find(long id)
        {
            try
            {
                return this.uow.SupportDocRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }
        }

        //public bool IsExist(string company)
        //{
        //    try
        //    {
        //        return this.uow.SupportDocRepository.GetFirst(x => x.BUKRS == company) != null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
        //    }
        //}

        #endregion

        #region Create

        public MASTER_SUPPORTING_DOCUMENT Create(MASTER_SUPPORTING_DOCUMENT data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.MASTER_SUPPORTING_DOCUMENT.Add(data);
                        context.SaveChanges();
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.LASTAPPROVED_STATUS);
                        data.COMPANY = context.T001.Find(data.BUKRS);                        
                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return data;

        }

        #endregion

        #region Update


        public bool Edit(MASTER_SUPPORTING_DOCUMENT data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.MASTER_SUPPORTING_DOCUMENT.Find(data.DOCUMENT_ID);
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.LASTAPPROVED_STATUS);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return true;
        }

        #endregion

        #region SetProperty

        /// <summary> Change Exciser Flag with True or False </summary>
        /// <param name="data"></param>
        public void SetActive(MASTER_SUPPORTING_DOCUMENT data)
        {
            try
            {
                var supDocument = new MASTER_SUPPORTING_DOCUMENT();

                supDocument = this.uow.SupportDocRepository.GetSingle(a => a.DOCUMENT_ID == data.DOCUMENT_ID);
                supDocument.IS_ACTIVE = data.IS_ACTIVE;

                this.uow.SupportDocRepository.Update(supDocument);
                this.uow.SupportDocRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }
        }

        #endregion

        #region Changes Log
        private Dictionary<string, string[]> GetAllChanges(MASTER_SUPPORTING_DOCUMENT old, MASTER_SUPPORTING_DOCUMENT updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                     "COMPANY",
                     "FORM_ID",
                     "SUPPORTING_DOCUMENT_NAME",
                     "APPROVALSTATUS",
                     "IS_ACTIVE"
                     };
                var oldProps = new Dictionary<string, object>();
                var props = new Dictionary<string, object>();

                foreach (var prop in updated.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    props.Add(prop.Name, prop.GetValue(updated, null));
                    if (old != null)
                        oldProps.Add(prop.Name, prop.GetValue(old, null));
                    else
                        oldProps.Add(prop.Name, null);
                }
                foreach (var item in props)
                {
                    var oldValue = (oldProps[item.Key] != null) ? oldProps[item.Key].ToString() : "N/A";
                    var newValue = (item.Value != null) ? item.ToString() : "N/A";
                    if (!columns.Contains(item.Key))
                        continue;

                    if (item.Key == "COMPANY")
                    {
                        if (item.Value != null)
                            newValue = ((T001)item.Value).BUTXT;

                        if (oldProps[item.Key] != null)
                            oldValue = ((T001)oldProps[item.Key]).BUTXT;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }

                    if (item.Key == "FORM_ID")
                    {
                        //var text = Enum.GetName(typeof(FormList), item.Value);
                        var text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(Enums.FormList), item.Value.ToString()));
                        if (item.Value != null)
                            newValue = text;
                    }

                    if (item.Key == "APPROVALSTATUS")
                    {
                        if (item.Value != null)
                            newValue = ((SYS_REFFERENCES)item.Value).REFF_VALUE;

                        if (oldProps[item.Key] != null)
                            oldValue = ((SYS_REFFERENCES)oldProps[item.Key]).REFF_VALUE;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }
                    if (item.Value != null)
                    {
                        if (item.Value is decimal)
                            newValue = ((decimal)item.Value).ToString("C2");

                        else
                            newValue = item.Value.ToString();
                    }

                    if (oldProps[item.Key] != null)
                    {
                        if (oldProps[item.Key] is decimal)
                            oldValue = ((decimal)oldProps[item.Key]).ToString("C2");

                        else
                            oldValue = oldProps[item.Key].ToString();
                    }
                    if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                        changes.Add(item.Key, new string[] { oldValue, newValue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }
        }

        private void LogsActivity(EMSDataModel context, MASTER_SUPPORTING_DOCUMENT data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        data.DOCUMENT_ID.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    data.DOCUMENT_ID,
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
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Change Status
        public MASTER_SUPPORTING_DOCUMENT ChangeStatus(long id, Core.ReferenceKeys.ApprovalStatus status, int formType, int actionType, int role, string user, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.MASTER_SUPPORTING_DOCUMENT.Find(id);
                        var data = (MASTER_SUPPORTING_DOCUMENT)context.Entry(old).GetDatabaseValues().ToObject();
                        data.LASTAPPROVED_STATUS = refService.GetReferenceByKey(status).REFF_ID;
                        if (status == Core.ReferenceKeys.ApprovalStatus.Completed)
                        {
                            data.LASTAPPROVED_BY = user;
                            data.LASTAPPROVED_DATE = DateTime.Now;
                        }
                        else if (status == Core.ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                        {
                            data.LASTMODIFIED_BY = user;
                            data.LASTMODIFIED_DATE = DateTime.Now;
                        }
                        data.COMPANY = context.T001.Find(data.BUKRS);
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.LASTAPPROVED_STATUS);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user, comment);
                        transaction.Commit();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        #endregion

        #region Additional Service

        public MASTER_SUPPORTING_DOCUMENT IsExist(string DocName, bool IsActive)
        {
            try
            {
                return this.uow.SupportDocRepository.GetFirst(dc => dc.SUPPORTING_DOCUMENT_NAME == DocName && dc.IS_ACTIVE == IsActive);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Supporting Document Service. See Inner Exception property to see details", ex);
            }
        }
        #endregion

    }
}