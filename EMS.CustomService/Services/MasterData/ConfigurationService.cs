using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Sampoerna.EMS.CustomService.Services.MasterData
{
    public class ConfigurationService : GenericService
    {
        private SystemReferenceService refService;
        public ConfigurationService(): base()
        {
            this.refService = new SystemReferenceService();
        }

        #region Get Data
        /// <summary>  Get All References Type </summary>
        /// <returns></returns>
        public IQueryable<SYS_REFFERENCES_TYPE> GetAllType()
        {
            try
            {
                var result = this.uow.ReferenceTypeRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Get References Text By Type </summary>        
        /// <param name="Type"></param>
        /// <returns></returns>
        public IQueryable<SYS_REFFERENCES_TYPE> GetReffTextByType(string Type)
        {
            try
            {
                return this.uow.ReferenceTypeRepository.GetManyQueryable(a => a.SYS_REFFERENCES_TYPE1 == Type.ToUpper());
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Get All List of Sys References </summary>        
        /// <returns></returns>
        public IQueryable<SYS_REFFERENCES> GetAllList()
        {
            try
            {
                return this.uow.ReferenceRepository.GetAll().OrderByDescending(a=>a.REFF_ID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Get Detail of Configuration by Reff ID  </summary>       
        /// <param name="ReffID"></param>
        /// <returns></returns>
        public SYS_REFFERENCES GetConfigDataByID(long ReffID)
        {
            try
            {
                return this.uow.ReferenceRepository.Find(ReffID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }

        }
        public SYS_REFFERENCES FindDataByType(string name)
        {
            try
            {
                return this.uow.ReferenceRepository.GetFirst(reff => reff.REFF_TYPE == name.ToUpper());
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        public SYS_REFFERENCES FindDataByName(string name)
        {
            try
            {
                return this.uow.ReferenceRepository.GetFirst(reff => reff.REFF_NAME == name.ToUpper());
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        public SYS_REFFERENCES FindDataByKey (string key)
        {
            try
            {
                var result = this.uow.ReferenceRepository.GetFirst(reff => reff.REFF_KEYS == key.ToUpper());
                return result;
            }
            catch(Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Get List of References Name by Type for Autocomplete Function </summary>        
        /// <param name="Type"></param>
        /// <returns></returns>
        public IQueryable<SYS_REFFERENCES> SelectNameByReferenceType(string Type)
        {
            try
            {
                return this.uow.ReferenceRepository.GetManyQueryable(a => a.REFF_TYPE == Type.ToUpper());
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Get All List of User for ReffType: Admin Approver </summary>        
        /// <returns></returns>

        public IQueryable<USER> GetAllUser()
        {
            try
            {                        
                var result = this.uow.UserRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }
       
        public IQueryable<ADMIN_APPROVAL_VIEW> GetAdminAvailable()
        {
            try
            {
                var result = this.uow.AdminApprovalViewRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Create

        /// <summary> Create New Entry of Configuration in Sys References also Create Changes Log </summary>        
        /// <param name="data"></param>
        /// <param name="formType"></param>
        /// <returns></returns>
        public SYS_REFFERENCES CreateSysReff(SYS_REFFERENCES data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.SYS_REFFERENCES.Add(data);
                        context.SaveChanges();                      

                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return data;
        }

        #endregion

        #region Update

        /// <summary> Update Certain Field in Sys References also Create Changes Log </summary>        
        /// <param name="data"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public SYS_REFFERENCES UpdateSysReff(SYS_REFFERENCES data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.SYS_REFFERENCES.Find(data.REFF_ID);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return data;
        }
        #endregion

        #region Changes Log

        /// <summary> Part of Changes Log Step which Mark All Available Changes </summary>        
        /// <param name="old"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        private Dictionary<string, string[]> GetAllChanges(SYS_REFFERENCES old, SYS_REFFERENCES updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                        "REFF_NAME",
                        "REFF_TYPE",                      
                        "REFF_VALUE",
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
                    var newValue = (props[item.Key] != null) ? props[item.Key].ToString() : "N/A"; // updated value
                    //  var newValue = (item.Value != null) ? item.ToString() : "N/A"; // updated field and value 

                    if (!columns.Contains(item.Key))
                        continue;

                    if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                        changes.Add(item.Key, new string[] { oldValue, newValue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }

        }

        /// <summary> Part of Changes Log Step which Set All Changed Field into Table Changes Log </summary>        
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="changes"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="actor"></param>
        /// <param name="comment"></param>
        private void LogsActivity(EMSDataModel context, SYS_REFFERENCES data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context, formType, data.REFF_ID.ToString(), map.Key, map.Value[0], map.Value[1], actor, DateTime.Now);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }
        #endregion
       
    }
}
