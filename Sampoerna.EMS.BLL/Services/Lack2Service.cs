using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack2Service : ILack2Service
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK2> _repository;

        private string includeTables = "MONTH";

        public Lack2Service(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK2>();
        }

        public List<LACK2> GetAll()
        {
            return _repository.Get().ToList();
        }

        public List<LACK2> GetByParam(Lack2GetByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty((input.NppbKcId)))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbKcId);
            }
            if (!string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }

            if (input.SubmissionDate.HasValue)
            {
                var date = input.SubmissionDate.Value.Day;
                var month = input.SubmissionDate.Value.Month;
                var year = input.SubmissionDate.Value.Year;
                var dateToCompare = new DateTime(year, month, date);
                queryFilter = queryFilter.And(c => c.SUBMISSION_DATE.Equals(dateToCompare));
            }

            queryFilter = input.IsOpenDocList ? queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed) : queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);

            switch (input.UserRole)
            {
                case Enums.UserRole.POA:
                    queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId || (c.STATUS != Enums.DocumentStatus.Draft && input.NppbKcId.Contains(c.NPPBKC_ID))));
                    break;
                case Enums.UserRole.Manager:
                    queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Draft 
                                                       && c.STATUS != Enums.DocumentStatus.WaitingForApproval
                                                       && input.DocumentNumberList.Contains(c.LACK2_NUMBER));
                    break;
                default:
                    queryFilter = queryFilter.And(c => c.CREATED_BY == input.UserId);
                    break;
            }

            Func<IQueryable<LACK2>, IOrderedQueryable<LACK2>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK2>(input.SortOrderColumn));

            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            return dbData == null ? null : dbData.ToList();
        }

        public List<LACK2> GetCompletedByParam(Lack2GetByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = c => c.STATUS == Enums.DocumentStatus.Completed;

            if (!string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            
            Func<IQueryable<LACK2>, IOrderedQueryable<LACK2>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK2>(input.SortOrderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);

            return dbData == null ? null : dbData.ToList();
        }

        public LACK2 GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public LACK2 GetDetailsById(int id)
        {
            var incTables = includeTables + ", LACK2_DOCUMENT, LACK2_ITEM, LACK2_ITEM.CK5";
            return _repository.Get(x => x.LACK2_ID == id, null, incTables).FirstOrDefault();
        }

        public void Insert(LACK2 data)
        {
            _repository.Insert(data);
        }

        public LACK2 GetBySelectionCriteria(Lack2GetBySelectionCriteriaParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter =
                c => c.BUKRS == input.CompanyCode && c.NPPBKC_ID == input.NppbkcId
                     && c.EX_GOOD_TYP == input.ExGoodTypeId
                     && c.LEVEL_PLANT_ID == input.SourcePlantId
                     && c.PERIOD_MONTH == input.PeriodMonth && c.PERIOD_YEAR == input.PeriodYear;

            var dataExist = _repository.Get(queryFilter).FirstOrDefault();
            return dataExist;
        }

        public List<LACK2> GetSummaryReportsByParam(Lack2GetSummaryReportByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilter = queryFilter.And(c => c.BUKRS.Contains(input.CompanyCode));
            }

            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID.Contains(input.NppbkcId));
            }

            if (!string.IsNullOrEmpty(input.SendingPlantId))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID.Contains(input.SendingPlantId));
            }

            if (!string.IsNullOrEmpty(input.GoodType))
            {
                queryFilter = queryFilter.And(c => c.EX_GOOD_TYP.Contains(input.GoodType));
            }

            if (input.PeriodMonth.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_MONTH == input.PeriodMonth.Value);

            if (input.PeriodYear.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_YEAR == input.PeriodYear.Value);

            if (input.DocumentStatus.HasValue)
            {
                queryFilter = queryFilter.And(c => c.STATUS == input.DocumentStatus.Value);
            }

            if (input.CreatedDate.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.CREATED_DATE.Year == input.CreatedDate.Value.Year &&
                            c.CREATED_DATE.Month == input.CreatedDate.Value.Month &&
                            c.CREATED_DATE.Day == input.CreatedDate.Value.Day);
            }
            if (!string.IsNullOrEmpty(input.CreatedBy))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.CreatedBy);
            }
            if (input.ApprovedDate.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.APPROVED_DATE.HasValue &&
                            c.APPROVED_DATE.Value.Year == input.ApprovedDate.Value.Year &&
                            c.APPROVED_DATE.Value.Month == input.ApprovedDate.Value.Month &&
                            c.APPROVED_DATE.Value.Day == input.ApprovedDate.Value.Day);
            }
            if (!string.IsNullOrEmpty(input.ApprovedBy))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.ApprovedBy);
            }
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty(input.Approver))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY_MANAGER == input.Approver);
            }

            return _repository.Get(queryFilter, null, "LACK2_ITEM, LACK2_ITEM.CK5").ToList();

        }

        public List<LACK2> GetDetailReportsByParam(Lack2GetDetailReportByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilter = queryFilter.And(c => c.BUKRS.Contains(input.CompanyCode));
            }

            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID.Contains(input.NppbkcId));
            }

            if (!string.IsNullOrEmpty(input.SendingPlantId))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID.Contains(input.SendingPlantId));
            }

            if (!string.IsNullOrEmpty(input.GoodType))
            {
                queryFilter = queryFilter.And(c => c.EX_GOOD_TYP.Contains(input.GoodType));
            }


            if (input.PeriodMonth.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_MONTH == input.PeriodMonth.Value);

            if (input.PeriodYear.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_YEAR == input.PeriodYear.Value);
            
            var rc = _repository.Get(queryFilter, null, "LACK2_ITEM, LACK2_ITEM.CK5").ToList();
            return rc;
        }

    }
}
