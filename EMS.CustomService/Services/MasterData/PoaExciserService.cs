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
    public class PoaExciserService : GenericService
    {
        private SystemReferenceService refService;
        public PoaExciserService():base()
        {
            this.refService = new SystemReferenceService();
        }

        #region Get Data

        /// <summary> Get All Data </summary>
        /// <returns></returns>
        public IQueryable<POA_EXCISER> GetAll()
        {
            try
            {
                return this.uow.PoaExciserRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on POA EXCISER Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Get Data Exciser by POA_ID </summary>
        /// <param name="PoaID"></param>
        /// <returns></returns>
        public POA_EXCISER GetByPoaID(string PoaID)
        {
            try
            {
                return this.uow.PoaExciserRepository.GetSingle(a => a.POA_ID == PoaID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on POA EXCISER Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Create
        /// <summary> Create Exciser Flag - Default: True </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public POA_EXCISER CreateExciser(POA_EXCISER data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.POA_EXCISER.Add(data);
                        context.SaveChanges();
                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on POA EXCISER Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return data;
        }
        #endregion

        #region SetProperty

        /// <summary> Change Exciser Flag with True or False </summary>
        /// <param name="data"></param>
        public POA_EXCISER SetActive(POA_EXCISER data, int formType, int actionType, int role, string user)
        {                  
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.POA_EXCISER.SingleOrDefault(m=>m.EXCISER_ID == data.EXCISER_ID);
                      
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on POA Service Service. See Inner Exception property to see details", ex);
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
        private Dictionary<string, string[]> GetAllChanges(POA_EXCISER old, POA_EXCISER updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                        "IS_ACTIVE_EXCISER"
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
                throw this.HandleException("Exception occured on POA Exciser Service. See Inner Exception property to see details", ex);
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
        private void LogsActivity(EMSDataModel context, POA_EXCISER data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context, formType, data.POA_ID.ToString(), map.Key, map.Value[0], map.Value[1], actor, DateTime.Now);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on POA Exciser Service. See Inner Exception property to see details", ex);
            }
        }
        #endregion
    }
}
