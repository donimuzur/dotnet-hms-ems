using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace Sampoerna.EMS.CustomService.Services.MasterData
{
    public class FinanceRatioManagementService : GenericService
    {
        private SystemReferenceService refService;
        public FinanceRatioManagementService() : base()
        {
            this.refService = new SystemReferenceService();
        }
        /// <summary>
        /// Method to retreive all Finance Ratio data
        /// </summary>
        /// <returns>List of Finance Ratio objects</returns>
        public IQueryable<MASTER_FINANCIAL_RATIO> GetAll()
        {
            try
            {
                return this.uow.FinancialRatioRepository.GetAll().OrderByDescending(item => item.FINANCERATIO_ID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
            }
        }

        /// <summary>
        /// Method to retreive all company data
        /// </summary>
        /// <returns></returns>
        public IQueryable<T001> GetCompanies()
        {
            try
            {
                var result = this.uow.CompanyRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
            }
        }

        /// <summary>
        /// Method to get specific finance ratio data by id
        /// </summary>
        /// <param name="id">The id of finance ratio to retreive</param>
        /// <returns>finance ratio object</returns>
        public MASTER_FINANCIAL_RATIO Find(long id)
        {
            try
            {
                return this.uow.FinancialRatioRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_FINANCIAL_RATIO Create(MASTER_FINANCIAL_RATIO data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.MASTER_FINANCIAL_RATIO.Add(data);
                        context.SaveChanges();
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);
                        data.COMPANY = context.T001.Find(data.BUKRS);
                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
                    }

                }
            }
            return data;

        }

        public bool Edit(MASTER_FINANCIAL_RATIO data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.MASTER_FINANCIAL_RATIO.Find(data.FINANCERATIO_ID);
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
                    }
                }
            }
            return true;
        }

        public MASTER_FINANCIAL_RATIO ChangeStatus(long id, Core.ReferenceKeys.ApprovalStatus status, int formType, int actionType, int role, string user, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.MASTER_FINANCIAL_RATIO.Find(id);
                        var data = (MASTER_FINANCIAL_RATIO)context.Entry(old).GetDatabaseValues().ToObject();
                        data.STATUS_APPROVAL = refService.GetReferenceByKey(status).REFF_ID;
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
                        data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user, comment);
                        transaction.Commit();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public MASTER_FINANCIAL_RATIO IsExist(string company, int period)
        {
            try
            {
                return this.uow.FinancialRatioRepository.GetFirst(fr => fr.YEAR_PERIOD == period && fr.BUKRS == company);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
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
                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
            }
        }

        #region Helpers
        private Dictionary<string, string[]> GetAllChanges(MASTER_FINANCIAL_RATIO old, MASTER_FINANCIAL_RATIO updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                     "COMPANY",
                     "YEAR_PERIOD",
                     "CURRENT_ASSETS",
                     "CURRENT_DEBTS",
                     "LIQUIDITY_RATIO",
                     "TOTAL_ASSETS",
                     "TOTAL_DEBTS",
                     "RENTABLE_RATIO",
                     "TOTAL_CAPITAL",
                     "NET_PROFIT",
                     "SOLVENCY_RATIO",
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
                    var newValue = (item.Value != null) ? item.ToString() :  "N/A";
                    if (!columns.Contains(item.Key))
                        continue;

                    if (item.Key == "COMPANY")
                    {
                        if (item.Value != null)
                            newValue = ((T001)item.Value).BUTXT;

                        if (oldProps[item.Key] != null)
                            oldValue = ((T001)oldProps[item.Key]).BUTXT;
                        if(oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
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
                            newValue = ((decimal)item.Value).ToString("N");
                        else if (item.Value is DateTime)
                            newValue = ((DateTime)item.Value).ToString("dd MMMM yyyy HH:mm:ss");
                        else
                            newValue = item.Value.ToString();
                    }

                    if (oldProps[item.Key] != null)
                    {
                        if (oldProps[item.Key] is decimal)
                            oldValue = ((decimal)oldProps[item.Key]).ToString("N");
                        else if (item.Value is DateTime)
                            oldValue = ((DateTime)item.Value).ToString("dd MMMM yyyy HH:mm:ss");
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

                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
            }



        }

        private void LogsActivity(EMSDataModel context, MASTER_FINANCIAL_RATIO data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        data.FINANCERATIO_ID.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    data.FINANCERATIO_ID,
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

                throw this.HandleException("Exception occured on FinanceRatioManagementService. See Inner Exception property to see details", ex);
            }

        }
        #endregion

    }
}
