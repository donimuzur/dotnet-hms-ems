using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.CustomService.Services
{
    public class ExciseCreditService : GenericService
    {
        private Dictionary<int, string> exciseCreditType;
        private Dictionary<int, string> exciseCreditGuarantee;
        private SystemReferenceService refService = new SystemReferenceService();
        public ExciseCreditService() : base()
        {
            exciseCreditType = new Dictionary<int, string>();
            exciseCreditType.Add(1, "New Excise Credit");
            exciseCreditType.Add(2, "Excise Credit Adjustment");

            exciseCreditGuarantee = new Dictionary<int, string>();
            exciseCreditGuarantee.Add(1, "Company Guarantee");
            exciseCreditGuarantee.Add(2, "Bank Guarantee");
            exciseCreditGuarantee.Add(3, "Excise Bond");
        }

        public Dictionary<int, string> GetExciseCreditTypes()
        {
            return exciseCreditType;
        }

        public Dictionary<int, string> GetExciseCreditGuarantees()
        {
            return exciseCreditGuarantee;
        }

        public string GetExciseCreditTypeName(int id)
        {
            return (exciseCreditType.ContainsKey(id)) ? exciseCreditType[id] : null;
        }

        public string GetExciseCreditGuaranteeName(int id)
        {
            return (exciseCreditGuarantee.ContainsKey(id)) ? exciseCreditGuarantee[id] : null;
        }

        public double GetExciseAdjustment(double liquidity)
        {
            try
            {
                var adjustmentList = this.uow.ExciseAdjustmentRepository.GetAll();
                double adjustment = 50.0;
                if (liquidity < 1)
                    return 0.0;
                foreach (var adj in adjustmentList)
                {
                    if (adj.RATIO2 == null)
                        break;

                    if (liquidity <= (double)adj.RATIO2 && liquidity > (double)adj.RATIO1)
                        return (double)adj.VALUE;
                }
                return adjustment;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }

        }

        public IEnumerable<CK1_EXCISE_CALCULATE> GetCK1List(string nppbkcId, DateTime submitDate, string productType, int period)
        {
            try
            {
                var fromMonth = submitDate.AddMonths(-1 * period);
                fromMonth = DateTime.Parse(string.Format("{0}-{1}-{2}", fromMonth.Year, fromMonth.Month, 1));
                var toMonth = submitDate.AddMonths(-1);
                toMonth = DateTime.Parse(string.Format("{0}-{1}-{2}", toMonth.Year, toMonth.Month, DateTime.DaysInMonth(toMonth.Year, toMonth.Month)));
                var ck1List = this.uow.CK1Repository.GetManyQueryable(ck1 => ck1.NPPBKC_ID == nppbkcId && (ck1.CK1_DATE >= fromMonth && ck1.CK1_DATE <= toMonth)).Select(x => x.CK1_ID).ToList();
                var result = this.uow.Ck1ExciseCalculatRepository.GetMany(x => ck1List.Contains(x.CK1_ID)).Where(x => x.PRODUCT_ALIAS == productType).OrderBy(x => x.CK1_DATE).ThenBy(x => x.TAHUN).ThenBy(x => x.BULAN).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }
        public Decimal GetCK1Average(DateTime submitDate, string nppbkc, string productType, int monthCount)
        {
            try
            {
                Decimal result = Decimal.Zero;
                var resultSet = GetCK1List(nppbkc, submitDate, productType, monthCount).ToList();
                if (resultSet == null)
                    return result;
                DateTime from = DateTime.Parse(string.Format("{0}-{1}-{2}", submitDate.Year, submitDate.Month, DateTime.DaysInMonth(submitDate.Year, submitDate.Month)));
                DateTime to = from.AddMonths(monthCount - 1);
                to = DateTime.Parse(string.Format("{0}-{1}-{2}", to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month)));
                if (resultSet.Count > 0)
                {
                    result = resultSet.Average(x => x.NOMINAL).Value;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public List<string> GetCK1ProductTypes(string nppbkc, DateTime submitDate)
        {
            try
            {
                var fromMonth = submitDate.AddMonths(-6);
                fromMonth = DateTime.Parse(string.Format("{0}-{1}-{2}", fromMonth.Year, fromMonth.Month, 1));
                var toMonth = submitDate.AddMonths(-1);
                toMonth = DateTime.Parse(string.Format("{0}-{1}-{2}", toMonth.Year, toMonth.Month, DateTime.DaysInMonth(toMonth.Year, toMonth.Month)));
                var ck1List = this.uow.Ck1ExciseCalculatRepository.GetManyQueryable(ck1 => ck1.NPPBKC_ID == nppbkc && (ck1.CK1_DATE >= fromMonth && ck1.CK1_DATE <= toMonth)).ToList();

                return ck1List.Select(x => x.PRODUCT_ALIAS).Distinct().ToList();

            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }
        public IEnumerable<MASTER_PRODUCT_TYPE> GetProductTypes(List<string> aliases)
        {
            try
            {
                return this.uow.ProductTypeRepository.GetMany(x => aliases.Contains(x.PRODUCT_ALIAS)).Distinct();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<EXCISE_CREDIT> GetAll()
        {
            try
            {
                return this.uow.ExciseCreditRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<EXCISE_CREDIT> GetOpenDocuments(string POA)
        {
            try
            {
                List<EXCISE_CREDIT> openDocuments = new List<EXCISE_CREDIT>();
                var draftStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Draft);
                var editStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Edited);
                var submittedStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval);
                var waitingGovStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval);
                var waitingSkepStatus = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval);
                var canceled = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Canceled);

                var ownDocuments = this.uow.ExciseCreditRepository.GetMany(item => item.POA_ID == POA && (item.LAST_STATUS == draftStatus.REFF_ID || item.LAST_STATUS == editStatus.REFF_ID || item.LAST_STATUS == canceled.REFF_ID || item.LAST_STATUS == waitingGovStatus.REFF_ID));
                openDocuments.AddRange(ownDocuments);

                var poaDelegatedMe = refService.GetPOADelegatedMe(POA);
                if (poaDelegatedMe != null && poaDelegatedMe.Any())
                {
                    var poaDelegatedMeIds = poaDelegatedMe.Select(x => x.POA_ID).ToList();
                    var delegatedDocuments = this.uow.ExciseCreditRepository.GetMany(item => poaDelegatedMeIds.Contains(item.POA_ID) && (item.LAST_STATUS == draftStatus.REFF_ID || item.LAST_STATUS == editStatus.REFF_ID || item.LAST_STATUS == canceled.REFF_ID || item.LAST_STATUS == waitingGovStatus.REFF_ID)).ToList();
                    openDocuments.AddRange(delegatedDocuments);
                }

                //var poaApprovers = refService.GetPOAWithinNPPBKC(POA);
                //if (poaApprovers.Any())
                //{
                //    var approvers = poaApprovers.Select(x => x.POA_ID);
                //    var documentsRequireApproval = this.uow.ExciseCreditRepository.GetMany(item => approvers.Contains(item.POA_ID) && (item.LAST_STATUS == submittedStatus.REFF_ID));
                //    openDocuments.AddRange(documentsRequireApproval);
                //}

                var documentsRequireApproval = this.uow.ExciseCreditRepository.GetMany(item => item.LAST_STATUS == submittedStatus.REFF_ID || item.LAST_STATUS == waitingSkepStatus.REFF_ID);
                openDocuments.AddRange(documentsRequireApproval);

                return openDocuments;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public EXCISE_CREDIT Find(object id)
        {
            try
            {
                return this.uow.ExciseCreditRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService.Find(). See Inner Exception property to see details", ex);
            }
        }

        public int Count()
        {
            try
            {
                return this.uow.ExciseCreditRepository.GetAll().Count();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<MASTER_FINANCIAL_RATIO> GetFinancialStatements(int submitYear, string company)
        {
            try
            {
                const int DURATION = 2;
                List<int> years = new List<int>();
                for (var i = DURATION; i > 0; i--)
                {
                    years.Add(submitYear - i);
                }

                return this.uow.FinancialRatioRepository.GetMany(x => years.Contains(x.YEAR_PERIOD) && x.BUKRS == company).OrderByDescending(x => x.YEAR_PERIOD);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<MASTER_FINANCIAL_RATIO> GetFinancialStatements(List<long> ids)
        {
            try
            {
                return this.uow.FinancialRatioRepository.GetMany(x => ids.Contains(x.FINANCERATIO_ID)).OrderByDescending(x => x.YEAR_PERIOD);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public long SaveExcise(EXCISE_CREDIT entity, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var nppbkc = refService.GetNppbkc(entity.NPPBKC_ID);
                        var company = nppbkc.COMPANY;
                        var lastCount = (context.EXCISE_CREDIT.Count() + 1).ToString("D10");
                        entity.EXCISE_CREDIT_NO = String.Format("{0}/{1}/{2}/{3}/{4}", lastCount, company.BUTXT_ALIAS, nppbkc.CITY_ALIAS, Utils.MonthHelper.ConvertToRomansNumeral(entity.SUBMISSION_DATE.Month), entity.SUBMISSION_DATE.Year);
                        context.EXCISE_CREDIT.Add(entity);
                        context.SaveChanges();
                        if (entity.APPROVAL_STATUS == null)
                        {
                            entity.APPROVAL_STATUS = context.SYS_REFFERENCES.Find(entity.LAST_STATUS);
                        }
                        var changes = GetAllChanges(null, entity);
                        var productTypes = this.GetCK1ProductTypes(entity.NPPBKC_ID, entity.SUBMISSION_DATE);
                        foreach (var prodType in productTypes)
                        {
                            var result = this.GetCK1List(entity.NPPBKC_ID, entity.SUBMISSION_DATE, prodType, 6);
                            foreach (var item in result)
                            {
                                var prod = context.ZAIDM_EX_PRODTYP.Where(x => x.PRODUCT_ALIAS.ToUpper() == item.PRODUCT_ALIAS.ToUpper()).FirstOrDefault();
                                context.EXCISE_CREDIT_DETAILCK1.Add(new EXCISE_CREDIT_DETAILCK1()
                                {
                                    CK1_ID = item.CK1_ID,
                                    CK1_DATE = item.CK1_DATE,
                                    CK1_NO = item.CK1_NUMBER,
                                    CUKAI_AMOUNT = (item.NOMINAL.HasValue) ? item.NOMINAL.Value : Decimal.Zero,
                                    EXCISE_CREDIT_ID = entity.EXSICE_CREDIT_ID,
                                    ORDER_QTY = (item.ORDERQTY.HasValue) ? item.ORDERQTY.Value : 0,
                                    PERIOD_MONTH = (item.BULAN.HasValue) ? item.BULAN.Value : 0,
                                    PERIOD_YEAR = (item.TAHUN.HasValue) ? item.TAHUN.Value : 0,
                                    PRODUCT_CODE = (prod != null) ? prod.PROD_CODE : null

                                });
                            }
                        }
                        context.SaveChanges();

                        var formType = (int)Enums.MenuList.ExciseCredit;
                        refService.LogsActivity(context, entity.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Created, role, user, null, entity.EXCISE_CREDIT_NO);
                        transaction.Commit();
                        return entity.EXSICE_CREDIT_ID;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }

                }
            }

        }

        public bool EditExcise(long id, DateTime submitDate, SYS_REFFERENCES status, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.SUBMISSION_DATE = submitDate;
                            data.LAST_MODIFIED_DATE = DateTime.Now;
                            data.LAST_MODIFIED_BY = user;
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Modified, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        public bool WithdrawExcise(long id, SYS_REFFERENCES status, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.LAST_MODIFIED_DATE = DateTime.Now;
                            data.LAST_MODIFIED_BY = user;
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Withdraw, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        public bool SubmitExcise(long id, SYS_REFFERENCES status, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.LAST_MODIFIED_DATE = DateTime.Now;
                            data.LAST_MODIFIED_BY = user;
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Submit, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return SendEmail(data, ReferenceKeys.EmailContent.ExciseCreditApprovalRequest);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }
        public bool SubmitSkep(EXCISE_CREDIT entity, string user, int role)
        {
            SYS_REFFERENCES status = refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval);
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(entity.EXSICE_CREDIT_ID);

                        if (old != null)
                        {
                            var data = entity;
                            data.LAST_MODIFIED_DATE = DateTime.Now;
                            data.LAST_MODIFIED_BY = user;
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            SaveApprovedProducts(context, entity.EXCISE_CREDIT_APPROVED_DETAIL.ToList());
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Submit, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return SendEmail(data, ReferenceKeys.EmailContent.ExciseCreditSKEPApprovalRequest);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        private bool SaveApprovedProducts(EMSDataModel context, List<EXCISE_CREDIT_APPROVED_DETAIL> data)
        {
            try
            {
                foreach (var item in data)
                {
                    var existing = context.EXCISE_CREDIT_APPROVED_DETAIL.Where(x => x.EXSICE_CREDIT_ID == item.EXSICE_CREDIT_ID && x.PROD_CODE == item.PROD_CODE).FirstOrDefault();
                    if (existing == null)
                        context.EXCISE_CREDIT_APPROVED_DETAIL.Add(item);
                    else
                    {
                        context.Entry(existing).CurrentValues.SetValues(item);
                    }
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool CancelExcise(long id, SYS_REFFERENCES status, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.LAST_MODIFIED_DATE = DateTime.Now;
                            data.LAST_MODIFIED_BY = user;
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Cancelled, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        public bool ApproveExcise(long id, SYS_REFFERENCES status, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.LAST_MODIFIED_DATE = DateTime.Now;
                            data.LAST_MODIFIED_BY = user;
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.WaitingForPOASKEPApproval, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return SendEmail(data, ReferenceKeys.EmailContent.ExciseCreditApprovalNotification);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        public bool RejectExcise(long id, SYS_REFFERENCES status, string user, int role, string notes)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.LAST_MODIFIED_DATE = DateTime.Now;
                            data.LAST_MODIFIED_BY = user;
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Reject, role, user, notes, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return SendEmail(data, ReferenceKeys.EmailContent.ExciseCreditSKEPApprovalRequest);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        public bool ApproveExciseSkep(long id, SYS_REFFERENCES status, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.Completed, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return SendEmail(data, ReferenceKeys.EmailContent.ExciseCreditSKEPApprovalRequest);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        public bool RejectExciseSkep(long id, SYS_REFFERENCES status, string user, int role)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.EXCISE_CREDIT.Find(id);

                        if (old != null)
                        {
                            var data = (EXCISE_CREDIT)context.Entry(old).GetDatabaseValues().ToObject();
                            data.APPROVAL_STATUS = status;
                            data.LAST_STATUS = status.REFF_ID;
                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            var formType = (int)Enums.MenuList.ExciseCredit;
                            refService.LogsActivity(context, data.EXSICE_CREDIT_ID.ToString(), changes, formType, (int)Enums.ActionType.SubmitSkep, role, user, null, data.EXCISE_CREDIT_NO);
                            transaction.Commit();

                            return SendEmail(data, ReferenceKeys.EmailContent.ExciseCreditSKEPApprovalRejection);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                    }
                }
            }

            return false;
        }

        public FILE_UPLOAD GetGovernmentDoc(long id, string type)
        {
            try
            {
                var excise = this.Find(id);
                if (excise == null)
                    throw new Exception("Excise Credit Document not found!");

                type = type.ToUpper();
                var file = this.uow.FileUploadRepository.GetMany(x => x.IS_GOVERNMENT_DOC && x.IS_ACTIVE && x.FORM_TYPE_ID == (int)Enums.FormList.ExciseRequest && x.FORM_ID == id.ToString() && x.FILE_NAME.Contains(type)).OrderByDescending(x => x.CREATED_DATE).FirstOrDefault();

                return file;
            }
            catch (Exception ex)
            {

                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        private bool SendEmail(EXCISE_CREDIT data, ReferenceKeys.EmailContent content)
        {
            try
            {
                var lastEditBy = refService.GetUser(data.LAST_MODIFIED_BY);
                var contentEmail = GetEmailContent(content, data);
                return ItpiMailer.Instance.SendEmail(refService.GetPOAEmails(lastEditBy.EMAIL).ToArray(), null, null, null, contentEmail.EMAILSUBJECT, contentEmail.EMAILCONTENT, true, lastEditBy.EMAIL, String.Format("POA Creator [{0} {1}]", lastEditBy.FIRST_NAME, lastEditBy.LAST_NAME));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private CONTENTEMAIL GetEmailContent(ReferenceKeys.EmailContent template, EXCISE_CREDIT data, string remarks = null)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                var nppbkc = refService.GetNppbkc(data.NPPBKC_ID);
                data.POA = refService.GetPOA(data.POA_ID);
                parameters.Add("excise_number", data.EXCISE_CREDIT_NO);
                //parameters.Add("approval_status", data.APPROVAL_STATUS.REFF_VALUE);
                parameters.Add("nppbkc", data.NPPBKC_ID);
                parameters.Add("kppbc", nppbkc.KPPBC_ID);
                parameters.Add("amount", string.Format("Rp {0:N}", data.EXCISE_CREDIT_AMOUNT));
                parameters.Add("credit_type", this.GetExciseCreditTypeName(data.REQUEST_TYPE));
                if (template == ReferenceKeys.EmailContent.ExciseCreditApprovalRequest)
                {
                    parameters.Add("creator", data.POA.PRINTED_NAME);
                    parameters.Add("url_approve", String.Format("{0}/ExciseCredit/Approve/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                    parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingPoaApproval).REFF_VALUE);
                    parameters.Add("url_detail", String.Format("{0}/ExciseCredit/Detail/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                }
                else if (template == ReferenceKeys.EmailContent.ExciseCreditApprovalNotification || template == ReferenceKeys.EmailContent.ExciseCreditRejection)
                {
                    if (template == ReferenceKeys.EmailContent.ExciseCreditRejection)
                    {
                        parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.Rejected).REFF_VALUE);
                        parameters.Add("url_edit", String.Format("{0}/ExciseCredit/Edit/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                        parameters.Add("remarks", remarks);
                    }
                    else
                    {
                        parameters.Add("approval_status", refService.GetReferenceByKey(ReferenceKeys.ApprovalStatus.AwaitingGovernmentApproval).REFF_VALUE);
                        parameters.Add("url_skep", String.Format("{0}/ExciseCredit/InputSkep/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                    }
                    parameters.Add("approver", String.Format("{0} {1}", data.APPROVER.FIRST_NAME, data.APPROVER.LAST_NAME));
                    parameters.Add("url_detail", String.Format("{0}/ExciseCredit/Detail/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                }
                else
                {
                    if (template == ReferenceKeys.EmailContent.ExciseCreditSKEPApprovalRequest)
                    {
                        parameters.Add("url_edit", String.Format("{0}/ExciseCredit/ApproveSkep/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                    }
                    else if (template == ReferenceKeys.EmailContent.ExciseCreditSKEPApprovalRejection)
                    {
                        parameters.Add("url_edit", String.Format("{0}/ExciseCredit/InputSkep/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                    }
                    parameters.Add("url_detail", String.Format("{0}/ExciseCredit/DetailSkep/{1}", ConfigurationManager.AppSettings["WebRootUrl"], data.EXSICE_CREDIT_ID));
                }
                return refService.GetMailContent((int)template, parameters);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        private Dictionary<string, string[]> GetAllChanges(EXCISE_CREDIT old, EXCISE_CREDIT updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                     "SUBMISSION_DATE",
                     "REQUEST_TYPE",
                     "GUARANTEE",
                     "LIQUIDITY_RATIO_1",
                     "LIQUIDITY_RATIO_2",
                     "SOLVENCY_RATIO_1",
                     "SOLVENCY_RATIO_2",
                     "RENTABILITY_RATIO_1",
                     "RENTABILITY_RATIO_2",
                     "SKEP_STATUS",
                     "DECREE_NO",
                     "DECREE_DATE",
                     "BPJ_NO",
                     "BPJ_DATE",
                     "NOTES",
                     "EXCISE_CREDIT_NO",
                     "ADJUSTMENT_CALCULATED",
                     "APPROVAL_STATUS",
                     "DECREE_STARTDATE",
                     "POA_ID",
                     "EXCISE_CREDIT_AMOUNT"
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

                    if (item.Key == "REQUEST_TYPE")
                    {
                        if (item.Value != null)
                            newValue = this.GetExciseCreditTypeName(Int32.Parse(item.Value.ToString()));

                        if (oldProps[item.Key] != null)
                            oldValue = this.GetExciseCreditTypeName(Int32.Parse(item.Value.ToString())); ;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }

                    if (item.Key == "APPROVAL_STATUS")
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

                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public bool IsAllowedToModify(string POA, long id)
        {
            try
            {
                var creators = GetCreators(id);
                return creators != null && creators.Contains(POA);
            }
            catch (Exception ex)
            {

                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public bool IsAllowedToApprove(string POA, long id)
        {
            try
            {
                var approvers = GetApprovers(id);
                return approvers != null && approvers.Contains(POA);
            }
            catch (Exception ex)
            {

                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }

        }

        public List<String> GetApprovers(long id)
        {
            try
            {
                var doc = this.uow.ExciseCreditRepository.Find(id);
                if (doc != null)
                {
                    var poaList = new List<string>();
                    var POAs = refService.GetPOAWithinNPPBKC(doc.POA_ID);
                    if (POAs == null || !POAs.Any())
                    {
                        return null;
                    }
                    poaList.AddRange(POAs.Select(x => x.POA_ID));
                    //poaList.Add(doc.CREATED_BY);
                    //poaList.AddRange(refService.GetDelegatedPOA(doc.CREATED_BY).Select(x => x.POA_ID));
                    return poaList;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService.GetApprovers See Inner Exception property to see details", ex);
            }
        }

        public List<String> GetCreators(long id)
        {
            try
            {
                var doc = this.uow.ExciseCreditRepository.Find(id);
                if (doc != null)
                {
                    var poaList = new List<string>();
                    poaList.Add(doc.CREATED_BY);
                    var delegatedPOAs = refService.GetDelegatedPOA(doc.CREATED_BY);
                    if (delegatedPOAs != null && delegatedPOAs.Any())
                    {
                        poaList.AddRange(delegatedPOAs.Select(x => x.POA_ID));
                    }

                    return poaList;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService.GetCreators See Inner Exception property to see details", ex);
            }
        }

        public PRINTOUT_LAYOUT GetPrintoutLayout(string template, string userId)
        {
            try
            {
                var printout = this.uow.PrintoutLayoutRepository.GetMany(x => x.NAME.Trim() == template.Trim().ToUpper()).FirstOrDefault();
                if (printout != null)
                {
                    var userPrintout = this.uow.UserPrintoutLayoutRepository.GetMany(x => x.USER_ID == userId && x.PRINTOUT_LAYOUT_ID == printout.PRINTOUT_LAYOUT_ID).FirstOrDefault();
                    if (userPrintout == null)
                        return printout;

                    return userPrintout.PRINTOUT_LAYOUT;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService.GetPrintoutLayout See Inner Exception property to see details", ex);
            }
        }

        public bool HasEditedTemplate(string template, string userId)
        {
            try
            {
                return GetPrintoutLayout(template, userId) != null;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService.HasEditedTemplate See Inner Exception property to see details", ex);
            }
        }

        public Dictionary<string, string> GetPrintoutTemplateTypes()
        {
            var list = new Dictionary<string, string>();

            return list;
        }

        public IEnumerable<EXCISE_CREDIT_DETAILCK1> GetExciseCk1Detail(long id, string product)
        {
            try
            {
                return this.uow.ExciseCreditDetailCK1.GetMany(x => x.EXCISE_CREDIT_ID == id && x.PRODUCT_TYPE.PRODUCT_ALIAS == product);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService.GetExciseCk1Detail See Inner Exception property to see details", ex);
            }
        }

    }
}
