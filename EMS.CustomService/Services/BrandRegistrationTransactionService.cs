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
    public class BrandRegistrationTransactionService : GenericService
    {
        private SystemReferenceService refService;

        public BrandRegistrationTransactionService() : base()
        {
            this.refService = new SystemReferenceService();
        }


        #region Get Data & Find Detail
        public IQueryable<PRODUCT_DEVELOPMENT> GetProductDevelopment()
        {
            try
            {
                var result = this.uow.ProductDevelopmentRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<PRODUCT_DEVELOPMENT_DETAIL> GetProductDevDetail()
        {
            try
            {
                var result = this.uow.ProductDevelopmentDetailRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
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
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MASTER_PLANT> GetPlant()
        {
            try
            {
                var result = this.uow.PlantRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<ZAIDM_EX_MARKET> GetMarket()
        {
            try
            {
                var result = this.uow.MarketRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<ZAIDM_EX_BRAND> GetBrand()
        {
            try
            {
                var result = this.uow.BrandRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }
        public IQueryable<ZAIDM_EX_MATERIAL> GetAllMaterial()
        {
            try
            {
                var result = this.uow.MaterialRepository.GetAll().Where(m => m.EXC_GOOD_TYP == "01");
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public ZAIDM_EX_MATERIAL FindItemDescription(string code)
        {
            try
            {
                var result = this.uow.MaterialRepository.GetFirst(mt => mt.STICKER_CODE == code.ToUpper());
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }
     
        public PRODUCT_DEVELOPMENT FindProductDevelopment(long ID)
        {
            try
            {
                var result = this.uow.ProductDevelopmentRepository.Find(ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public PRODUCT_DEVELOPMENT_DETAIL FindProductDevDetail(long ID)
        {
            try
            {
                var result = this.uow.ProductDevelopmentDetailRepository.Find(ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public USER FindUserDetail(string USERID)
        {
            try
            {
                return this.uow.UserRepository.Find(USERID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }
     

        public IEnumerable<MASTER_SUPPORTING_DOCUMENT> FindSupportDetail(long FormID, string Bukrs)
        {
            try
            {
                //var result = this.uow.SupportDocRepository.GetAll().Where(a => a.FORM_ID == FormID && a.BUKRS == Bukrs);                
                var result = this.uow.SupportDocRepository.GetMany(sd=>sd.FORM_ID == FormID && sd.BUKRS == Bukrs && sd.IS_ACTIVE == true);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }


        public IQueryable<MASTER_PLANT> FindPlantNonImport(string Bukrs)
        {
            try
            {               
                var data = this.uow.CompanyPlantMappingRepository.GetManyQueryable(a => a.BUKRS == Bukrs).Select(x=>x.BWKEY);
                var plant = this.uow.PlantRepository.GetManyQueryable(x => data.Contains(x.WERKS)).Where(x=>x.NPPBKC_ID != null);
                return plant;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MASTER_PLANT> FindPlantImport(string Bukrs)
        {
            try
            {
                var data = this.uow.CompanyPlantMappingRepository.GetManyQueryable(a => a.BUKRS == Bukrs).Select(x => x.BWKEY);
                var plant = this.uow.PlantRepository.GetManyQueryable(x => data.Contains(x.WERKS)).Where(x => x.NPPBKC_IMPORT_ID != null);
                return plant;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public PRODUCT_DEVELOPMENT GetLastRecordPD()
        {
            try
            {
                return this.uow.ProductDevelopmentRepository.GetAll().OrderByDescending(m => m.PD_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }


        public PRODUCT_DEVELOPMENT_DETAIL GetLastRecordPDDetail()
        {
            try
            {
                return this.uow.ProductDevelopmentDetailRepository.GetAll().OrderByDescending(m => m.PD_DETAIL_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }


        public MASTER_NPPBKC FindNppbkcByPlant (string plantName)
        {
            try
            {
                var temp = this.uow.PlantRepository.GetFirst(pl => pl.NAME1 == plantName);
                var result = this.uow.NppbkcRepository.GetFirst(np => np.NPPBKC_ID == temp.NPPBKC_ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_NPPBKC FindNppbkcDetail(string nppbkcId)
        {
            try
            {                
                var result = this.uow.NppbkcRepository.GetFirst(np => np.NPPBKC_ID == nppbkcId);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }
        #endregion

        #region Create
        //public void CreateProduct(PRODUCT_DEVELOPMENT data, PRODUCT_DEVELOPMENT_DETAIL dataDetail, int formType, int actionType, int role, string user)
        public void CreateProduct(PRODUCT_DEVELOPMENT data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Create Product Development
                        context.PRODUCT_DEVELOPMENT.Add(data);
                        context.SaveChanges();

                        // Create Product Development Detail
                        //dataDetail.PD_ID = data.PD_ID;
                        //context.PRODUCT_DEVELOPMENT_DETAIL.Add(dataDetail);
                        //context.SaveChanges();

                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
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
                        throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
                    }
                }
            }
            return data;
        }

        #endregion

        #region Update
        public bool Edit(PRODUCT_DEVELOPMENT data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT.Find(data.PD_ID);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, data, changes, formType, actionType, role, user);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
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
        private Dictionary<string, string[]> GetAllChanges(PRODUCT_DEVELOPMENT old, PRODUCT_DEVELOPMENT updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                     //"PROD_CODE",
                     //"PRODUCT_TYPE",
                     //"PRODUCT_ALIAS",
                     //"CK4CEDITABLE",
                     //"IS_DELETED",
                     //"APPROVALSTATUS"
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
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
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
        private void LogsActivity(EMSDataModel context, PRODUCT_DEVELOPMENT data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        data.PD_ID.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    Convert.ToInt64(data.PD_ID),
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
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Change Status
        public PRODUCT_DEVELOPMENT ChangeStatus(string id, Core.ReferenceKeys.ApprovalStatus status, int formType, int actionType, int role, string user, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT.Find(id);
                        var data = (PRODUCT_DEVELOPMENT)context.Entry(old).GetDatabaseValues().ToObject();
                        //data.STATUS_APPROVAL = refService.GetReferenceByKey(status).REFF_ID;
                        //if (status == Core.ReferenceKeys.ApprovalStatus.Completed)
                        //{
                        //    data.LASTAPPROVED_BY = user;
                        //    data.LASTAPPROVED_DATE = DateTime.Now;
                        //}
                        //else if (status == Core.ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                        //{
                        //    data.LASTMODIFIED_BY = user;
                        //    data.LASTMODIFIED_DATE = DateTime.Now;
                        //}

                        //data.APPROVAL_STATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);
                        //Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        //context.Entry(old).CurrentValues.SetValues(data);
                        //context.SaveChanges();
                        //LogsActivity(context, data, changes, formType, actionType, role, user, comment);
                        //transaction.Commit();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        #endregion
    }
}
