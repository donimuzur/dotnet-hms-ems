using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.CustomService.Services.MasterData
{
    public class ProductTypeService : GenericService
    {
        private SystemReferenceService refService;

        public ProductTypeService() : base()
        {
            this.refService = new SystemReferenceService();
        }

        #region Get Data

        /// <summary> Get All Product Type </summary>
        /// <returns></returns>
        public IQueryable<MASTER_PRODUCT_TYPE> GetAll()
        {
            try
            {
                return this.uow.ProductTypeRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Find Product Type Record by Id/Code </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MASTER_PRODUCT_TYPE Find(string id)
        {
            try
            {
                return this.uow.ProductTypeRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_PRODUCT_TYPE GetLastRecord()
        {
            try
            {
                return this.uow.ProductTypeRepository.GetAll().OrderByDescending(m => m.PROD_CODE).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
            }
        }
     
        #endregion

        #region Create

        /// <summary> Create New Entry of Product Type also Create Changes Log </summary>
        /// <param name="data"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public MASTER_PRODUCT_TYPE Create(MASTER_PRODUCT_TYPE data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.ZAIDM_EX_PRODTYP.Add(data);
                        context.SaveChanges();
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.APPROVED_STATUS);
                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
                    }

                }
            }
            return data;
        }
        #endregion

        #region Update

        /// <summary> Update certain field of Product Type, also create Changes Log </summary>
        /// <param name="data"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Edit(MASTER_PRODUCT_TYPE data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.ZAIDM_EX_PRODTYP.Find(data.PROD_CODE);
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.APPROVED_STATUS);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
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
        private Dictionary<string, string[]> GetAllChanges(MASTER_PRODUCT_TYPE old, MASTER_PRODUCT_TYPE updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                     "PROD_CODE",
                     "PRODUCT_TYPE",
                     "PRODUCT_ALIAS",
                     "CK4CEDITABLE",
                     "IS_DELETED",
                     "APPROVALSTATUS"
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
                throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
            }
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
        private void LogsActivity(EMSDataModel context, MASTER_PRODUCT_TYPE data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        data.PROD_CODE.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    Convert.ToInt64(data.PROD_CODE),
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
                throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Change Status
        public MASTER_PRODUCT_TYPE ChangeStatus(string id, Core.ReferenceKeys.ApprovalStatus status, int formType, int actionType, int role, string user, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.ZAIDM_EX_PRODTYP.Find(id);
                        var data = (MASTER_PRODUCT_TYPE)context.Entry(old).GetDatabaseValues().ToObject();
                        data.APPROVED_STATUS = refService.GetReferenceByKey(status).REFF_ID;
                        if (status == Core.ReferenceKeys.ApprovalStatus.Completed)
                        {
                            data.LASTAPPROVED_BY = user;
                            data.LASTAPPROVED_DATE = DateTime.Now;
                        }
                        else if (status == Core.ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                        {
                            data.MODIFIED_BY = user;
                            data.MODIFIED_DATE = DateTime.Now;
                        }                      

                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.APPROVED_STATUS);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user, comment);
                        transaction.Commit();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        #endregion

        #region Additional Service

        public MASTER_PRODUCT_TYPE IsExist(string Type, string Alias)
        {
            try
            {
                return this.uow.ProductTypeRepository.GetFirst(pt => pt.PRODUCT_TYPE == Type && pt.PRODUCT_ALIAS == Alias);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
            }
        }
        #endregion




    }
}
