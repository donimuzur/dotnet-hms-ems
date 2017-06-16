using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;


namespace Sampoerna.EMS.CustomService.Services.MasterData
{
    public class TariffManagementService : GenericService
    {
        private SystemReferenceService refService;
        public TariffManagementService() : base()
        {
            this.refService = new SystemReferenceService();
        }

        /// <summary>
        /// Method to retreive all active product
        /// </summary>
        /// <returns>List of product type data</returns>
        public IEnumerable<MASTER_PRODUCT_TYPE> GetProductTypes()
        {
            try
            {
                var approved = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_ID;
                return this.uow.ProductTypeRepository.GetMany(item => (!item.IS_DELETED.HasValue || !item.IS_DELETED.Value) && item.APPROVED_STATUS == approved);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on TariffManagementService. See Inner Exception property to see details", ex);
            }
        }

        /// <summary>
        /// Method to retreive all tariff
        /// </summary>
        /// <returns>List of tariff data ordered by id descending</returns>
        public IEnumerable<TARIFF> GetTariff()
        {
            try
            {
                return this.uow.TariffRepository.GetAll().OrderByDescending(item => item.TARIFF_ID).ToList();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on TariffManagementService. See Inner Exception property to see details", ex);
            }
        }

        public TARIFF Find(long id)
        {
            try
            {
                return this.uow.TariffRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on TariffManagementService. See Inner Exception property to see details", ex);
            }
        }

        /// <summary>
        /// Method to save a tariff data(insert or update or submit or approve or reject)
        /// </summary>
        /// <param name="data">Tariff data</param>
        /// <param name="status">Status approval</param>
        /// <param name="actionType">Action type, insert, update, reject or approve</param>
        /// <param name="role">User role</param>
        /// <param name="user">User ID</param>
        /// <param name="userEmail">User Email</param>
        /// <param name="displayName">User Full Name</param>
        /// <param name="comment">Remarks on Rejection Action</param>
        /// <returns></returns>
        public bool Save(TARIFF data, ReferenceKeys.ApprovalStatus status, Enums.ActionType actionType, int role, string user, string userEmail, string displayName, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var changes = new Dictionary<string, string[]>();
                        bool sendEmail = false;
                        string[] sendTo = null;
                        CONTENTEMAIL email = null;
                        data.STATUS_APPROVAL = refService.GetReferenceByKey(status).REFF_ID;
                        if (actionType == Enums.ActionType.Created)
                        {
                            data.CREATED_BY = user;
                            data.CREATED_DATE = DateTime.Now;
                            context.TARIFF.Add(data);
                            data.APPROVALSTATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);
                            data.PRODUCT_TYPE = context.ZAIDM_EX_PRODTYP.Find(data.PROD_CODE);
                            changes = this.GetAllChanges(null, data);
                            context.SaveChanges();
                            
                        }
                        else if (actionType == Enums.ActionType.Modified)
                        {
                            var old = context.TARIFF.Find(data.TARIFF_ID);
                            data.LASTMODIFIED_BY = user;
                            data.LASTMODIFIED_DATE = DateTime.Now;
                            data.CREATED_BY = old.CREATED_BY;
                            data.CREATED_DATE = old.CREATED_DATE;
                            data.APPROVALSTATUS = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited);
                            data.PRODUCT_TYPE = context.ZAIDM_EX_PRODTYP.Find(data.PROD_CODE);
                            changes = this.GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                        }
                        else // Approved or Rejected
                        {
                            var old = context.TARIFF.Find(data.TARIFF_ID);
                            data = (TARIFF)context.Entry(old).GetDatabaseValues().ToObject();
                            data.STATUS_APPROVAL = refService.GetReferenceByKey(status).REFF_ID;
                            data.PRODUCT_TYPE = context.ZAIDM_EX_PRODTYP.Find(old.PROD_CODE);
                            data.CREATOR = context.USER.Find(data.CREATED_BY);
                            
                            if (status == ReferenceKeys.ApprovalStatus.Completed)
                            {
                                data.APPROVER = context.USER.Find(user);
                                data.STATUS_APPROVAL = refService.GetReferenceByKey(status).REFF_ID;
                                data.APPROVALSTATUS = refService.GetReferenceByKey(status);
                                data.LASTAPPROVED_BY = user;
                                data.LASTAPPROVED_DATE = DateTime.Now;
                                var admins = refService.GetAdmins();
                                List<string> adminEmails = new List<string>();
                                foreach (var adm in admins)
                                {
                                    if(!string.IsNullOrEmpty(adm.EMAIL))
                                    adminEmails.Add(adm.EMAIL);
                                }
                                sendTo = adminEmails.ToArray();
                                email = GetEmailContent(ReferenceKeys.EmailContent.TariffApproved, data);
                            }
                            else if (status == ReferenceKeys.ApprovalStatus.AwaitingAdminApproval || status == ReferenceKeys.ApprovalStatus.Rejected || status == ReferenceKeys.ApprovalStatus.Edited)
                            {
                                data.LASTMODIFIED_BY = user;
                                data.LASTMODIFIED_DATE = DateTime.Now;
                                if (status == ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                                {
                                    var reff = refService.GetReferenceByKey(ReferenceKeys.Approver.AdminApprover);
                                    data.APPROVALSTATUS = refService.GetReferenceByKey(status);
                                    data.LASTEDITOR = refService.GetUser(data.LASTMODIFIED_BY);
                                    var approvers = refService.GetAdminApprovers();
                                    List<string> approverEmails = new List<string>();
                                    foreach (var adm in approvers)
                                    {
                                        var appEmail = refService.GetUserEmail(adm.REFF_VALUE);
                                        if (!string.IsNullOrEmpty(appEmail))
                                            approverEmails.Add(appEmail);
                                    }
                                    sendTo = approverEmails.ToArray();
                                    email = GetEmailContent(ReferenceKeys.EmailContent.TariffApprovalRequest, data);
                                }
                                else
                                {
                                    data.STATUS_APPROVAL = refService.GetReferenceByKey(status).REFF_ID;
                                    data.APPROVALSTATUS = refService.GetReferenceByKey(status);
                                    data.APPROVER = context.USER.Find(user);
                                    var admins = refService.GetAdmins();
                                    List<string> adminEmails = new List<string>();
                                    foreach (var adm in admins)
                                    {
                                        if (!string.IsNullOrEmpty(adm.EMAIL))
                                            adminEmails.Add(adm.EMAIL);
                                    }
                                    sendTo = adminEmails.ToArray();
                                    email = GetEmailContent(ReferenceKeys.EmailContent.TariffRejected, data, comment);
                                }
                            }
                            changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            sendEmail = true;
                            
                        }
                        var formType = (int)Enums.MenuList.Tariff;
                        refService.LogsActivity(context, data.TARIFF_ID.ToString(), changes, formType, (int)actionType, role, user, comment);
                        transaction.Commit();

                        if (sendEmail)
                        {
                            return ItpiMailer.Instance.SendEmail(sendTo, null, null, null, email.EMAILSUBJECT, email.EMAILCONTENT, true, userEmail, displayName);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on TariffManagementService. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        #region Helpers
        private Dictionary<string, string[]> GetAllChanges(TARIFF old, TARIFF updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                     "HJE_FROM",
                     "HJE_TO",
                     "VALID_FROM",
                     "VALID_TO",
                     "TARIFF_VALUE",
                     "APPROVALSTATUS",
                     "PRODUCT_TYPE"
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

                    if (item.Key == "PRODUCT_TYPE")
                    {
                        if (item.Value != null)
                            newValue = ((MASTER_PRODUCT_TYPE)item.Value).PRODUCT_TYPE;

                        if (oldProps[item.Key] != null)
                            oldValue = ((MASTER_PRODUCT_TYPE)oldProps[item.Key]).PRODUCT_TYPE;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
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

                        else if (oldProps[item.Key] is DateTime)
                            oldValue = ((DateTime)oldProps[item.Key]).ToString("dd MMMM yyyy HH:mm:ss");
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

                throw this.HandleException("Exception occured on TariffManagementService. See Inner Exception property to see details", ex);
            }
        }

        private CONTENTEMAIL GetEmailContent(ReferenceKeys.EmailContent template, TARIFF data, string remarks = null)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("product", data.PRODUCT_TYPE.PRODUCT_TYPE);
                parameters.Add("code", data.PROD_CODE);
                parameters.Add("hje_from", data.HJE_FROM.ToString("N"));
                parameters.Add("hje_to", data.HJE_TO.ToString("N"));
                parameters.Add("valid_from", data.VALID_FROM.ToString("dddd, dd MMMM yyyy"));
                parameters.Add("valid_to", data.VALID_TO.ToString("dddd, dd MMMM yyyy"));
                if (template == ReferenceKeys.EmailContent.TariffApprovalRequest)
                {
                    parameters.Add("creator", String.Format("{0} {1}", data.CREATOR.FIRST_NAME, data.CREATOR.LAST_NAME));
                    parameters.Add("submitter", String.Format("{0} {1}", data.LASTEDITOR.FIRST_NAME, data.LASTEDITOR.LAST_NAME));
                    parameters.Add("url_approve", String.Format("{0}/Tariff/Approve/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.TARIFF_ID));
                    parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingAdminApproval).REFF_VALUE);
                }
                else
                {
                    if (template == ReferenceKeys.EmailContent.TariffRejected)
                    {
                        parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Rejected).REFF_VALUE);
                        parameters.Add("url_edit", String.Format("{0}/Tariff/Edit/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.TARIFF_ID));
                        parameters.Add("remark", remarks);
                    }
                    else
                    {
                        parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Completed).REFF_VALUE);
                    }
                    parameters.Add("approver", String.Format("{0} {1}", data.APPROVER.FIRST_NAME, data.APPROVER.LAST_NAME));
                   
                }
                
                parameters.Add("url_detail", String.Format("{0}/Tariff/Detail/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.TARIFF_ID));

                return refService.GetMailContent((int)template, parameters);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on TariffManagementService. See Inner Exception property to see details", ex);
            }
        }
        #endregion
    }
}
