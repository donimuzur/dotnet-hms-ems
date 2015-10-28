using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class LACK1Service : ILACK1Service
    {
        private IGenericRepository<LACK1> _repository;
        private IGenericRepository<LACK1_PRODUCTION_DETAIL> _productionDetailRepository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string includeTables = "UOM, UOM1, MONTH";

        public LACK1Service(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<LACK1>();
            _productionDetailRepository = _uow.GetGenericRepository<LACK1_PRODUCTION_DETAIL>();
        }

        public List<LACK1> GetAllByParam(Lack1GetByParamInput input)
        {
            includeTables += ", LACK1_PLANT";
            Expression<Func<LACK1, bool>> queryFilter = c => c.LACK1_LEVEL == input.Lack1Level;

            queryFilter = queryFilter.And(ProcessQueryFilter(input));

            Func<IQueryable<LACK1>, IOrderedQueryable<LACK1>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK1>(input.SortOrderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return dbData.ToList();
        }

        public List<LACK1> GetByPeriod(Lack1GetByPeriodParamInput input)
        {
            return _repository.Get(
                    c => c.NPPBKC_ID == input.NppbkcId && (int)c.STATUS >= (int)Core.Enums.DocumentStatus.Approved, null,
                    "").ToList();
        }

        public List<LACK1_PRODUCTION_DETAIL> GetProductionDetailByPeriode(Lack1GetByPeriodParamInput input)
        {
            return _productionDetailRepository.Get(
                    c =>
                        c.LACK1.NPPBKC_ID == input.NppbkcId &&
                        (int)c.LACK1.STATUS >= (int)Core.Enums.DocumentStatus.Approved, null,
                    "LACK1, LACK1.UOM11, LACK1.MONTH").ToList();
        }

        public List<LACK1> GetCompletedDocumentByParam(Lack1GetByParamInput input)
        {
            includeTables += ", LACK1_PLANT";
            Expression<Func<LACK1, bool>> queryFilter = c => (int)c.STATUS >= (int)Core.Enums.DocumentStatus.Completed;

            queryFilter = queryFilter.And(ProcessQueryFilter(input));

            Func<IQueryable<LACK1>, IOrderedQueryable<LACK1>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK1>(input.SortOrderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return dbData.ToList();

        }

        public decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input)
        {
            var dtTo = new DateTime(input.YearTo, input.MonthTo, 1);

            var getData = _repository.Get(c => c.NPPBKC_ID == input.NppbkcId
                                               &&
                                               (int)c.STATUS >= (int)Core.Enums.DocumentStatus.Approved, null,
                "").ToList().Select(p => new
                {
                    p.LACK1_ID,
                    p.LACK1_NUMBER,
                    p.PERIOD_MONTH,
                    p.PERIOD_YEAR,
                    p.BEGINING_BALANCE,
                    p.TOTAL_INCOME,
                    p.USAGE,
                    p.SUPPLIER_PLANT_WERKS,
                    p.EX_GOODTYP,
                    //p.TOTAL_PRODUCTION,
                    PERIODE = new DateTime(p.PERIOD_YEAR.Value, p.PERIOD_MONTH.Value, 1)
                }).ToList();

            if (getData.Count == 0) return 0;

            var selected = getData.Where(c => c.PERIODE <= dtTo &&  c.SUPPLIER_PLANT_WERKS == input.SupplierPlantWerks && c.EX_GOODTYP == input.ExcisableGoodsType).OrderByDescending(o => o.PERIODE).FirstOrDefault();

            if (selected == null) return 0;

            decimal rc = 0;

            rc = selected.BEGINING_BALANCE + selected.TOTAL_INCOME - selected.USAGE;

            return rc;
        }

        public List<LACK1> GetPbck1RealizationList(Lack1GetPbck1RealizationListParamInput input)
        {
            const string incTables = "LACK1_PRODUCTION_DETAIL";
            Expression<Func<LACK1, bool>> queryFilter = c => c.PERIOD_YEAR.HasValue && c.PERIOD_YEAR.Value == input.Year
                                          && c.PERIOD_MONTH.HasValue && c.PERIOD_MONTH.Value >= input.MonthFrom
                                          && c.PERIOD_MONTH.Value <= input.MonthTo && c.NPPBKC_ID == input.NppbkcId &&
                                          c.SUPPLIER_PLANT_WERKS == input.SupplierPlantId
                                          && c.EX_GOODTYP == input.ExcisableGoodsTypeId;

            var rc = _repository.Get(queryFilter, null, incTables).ToList();
            return rc;
        }

        public LACK1 GetLatestLack1ByParam(Lack1GetLatestLack1ByParamInput input)
        {

            Expression<Func<LACK1, bool>> queryFilter =
                c => c.BUKRS == input.CompanyCode && c.LACK1_LEVEL == input.Lack1Level
                     && c.NPPBKC_ID == input.NppbkcId && c.STATUS == Enums.DocumentStatus.Completed
                     && c.EX_GOODTYP == input.ExcisableGoodsType && c.LACK1_ID != input.ExcludeLack1Id
                     && c.SUPPLIER_PLANT_WERKS == input.SupplierPlantId;

            if (input.Lack1Level == Core.Enums.Lack1Level.Plant)
            {
                queryFilter =
                    queryFilter.And(c => c.LACK1_PLANT.Any(p => p.PLANT_ID == input.ReceivedPlantId));
            }

            var getData = _repository.Get(queryFilter, null,
                "").ToList().Select(p => new
                {
                    p.LACK1_ID,
                    p.LACK1_NUMBER,
                    p.PERIOD_MONTH,
                    p.PERIOD_YEAR,
                    p.BEGINING_BALANCE,
                    p.TOTAL_INCOME,
                    p.USAGE,
                    //p.TOTAL_PRODUCTION,
                    PERIODE = new DateTime(p.PERIOD_YEAR.Value, p.PERIOD_MONTH.Value, 1)
                }).ToList();

            if (getData.Count <= 0)
            {
                return null;
            }

            var selected = getData.Where(c => c.PERIODE <= input.PeriodTo).OrderByDescending(o => o.PERIODE).FirstOrDefault();
            if (selected != null) return _repository.GetByID(selected.LACK1_ID);
            return null;
        }

        public LACK1 GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public void Insert(LACK1 data)
        {
            _repository.Insert(data);
        }

        private Expression<Func<LACK1, bool>> ProcessQueryFilter(Lack1GetByParamInput input)
        {

            Expression<Func<LACK1, bool>> queryFilter = PredicateHelper.True<LACK1>();

            //filter search by nppbkc id, both Level NPPBKC and Level Plant
            if (!string.IsNullOrEmpty(input.NppbKcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbKcId);
            }

            //filter search by plant id, only LACK-1 Level Plant
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter =
                    queryFilter.And(c => c.LACK1_PLANT.Any(p => p.PLANT_ID == input.PlantId));
            }

            //filter search by poa, both Lack-1 Level
            if (!string.IsNullOrEmpty(input.Poa))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Poa || c.APPROVED_BY_POA == input.Poa);
            }

            //filter search by creator
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }

            if (input.SubmissionDate.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.SUBMISSION_DATE.HasValue &&
                            c.SUBMISSION_DATE.Value == input.SubmissionDate.Value);
            }

            if (!input.IsOpenDocumentOnly) return queryFilter;

            queryFilter = queryFilter.And(c => c.STATUS <= Enums.DocumentStatus.WaitingGovApproval || c.STATUS == Enums.DocumentStatus.GovRejected);

            switch (input.UserRole)
            {
                case Enums.UserRole.POA:
                    queryFilter = queryFilter.And(c => (c.CREATED_BY == input.UserId || (c.STATUS != Enums.DocumentStatus.Draft && input.NppbkcList.Contains(c.NPPBKC_ID))));
                    break;
                case Enums.UserRole.Manager:
                    queryFilter =
                        queryFilter.And(
                            c =>
                                c.STATUS != Enums.DocumentStatus.Draft &&
                                c.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                                input.DocumentNumberList.Contains(c.LACK1_NUMBER));
                    break;
                default:
                    queryFilter = queryFilter.And(c => c.CREATED_BY == input.UserId);
                    break;
            }

            return queryFilter;
        }

        public LACK1 GetBySelectionCriteria(Lack1GetBySelectionCriteriaParamInput input)
        {
            Expression<Func<LACK1, bool>> queryFilter =
                c => c.BUKRS == input.CompanyCode && c.NPPBKC_ID == input.NppbkcId
                     && c.EX_GOODTYP == input.ExcisableGoodsType
                     && c.SUPPLIER_PLANT_WERKS == input.SupplierPlantId
                     && c.LACK1_LEVEL == input.Lack1Level
                     && c.PERIOD_MONTH == input.PeriodMonth && c.PERIOD_YEAR == input.PeriodYear;
            if (!string.IsNullOrEmpty(input.ReceivingPlantId))
            {
                queryFilter =
                    queryFilter.And(c => c.LACK1_PLANT.Any(p => p.PLANT_ID == input.ReceivingPlantId));
            }

            return _repository.Get(queryFilter).FirstOrDefault();

        }

        public LACK1 GetDetailsById(int id)
        {
            var incTables = includeTables + ", LACK1_DOCUMENT, LACK1_INCOME_DETAIL, LACK1_PLANT, LACK1_PRODUCTION_DETAIL, LACK1_PRODUCTION_DETAIL.UOM, LACK1_PBCK1_MAPPING, LACK1_PBCK1_MAPPING.PBCK1, LACK1_TRACKING";
            return _repository.Get(c => c.LACK1_ID == id, null, incTables).FirstOrDefault();
        }

        public List<LACK1> GetSummaryReportByParam(Lack1GetSummaryReportByParamInput input)
        {
            const string incTables = "UOM, UOM1, MONTH, LACK1_PLANT";
            //process QueryFilter
            Expression<Func<LACK1, bool>> queryFilter = PredicateHelper.True<LACK1>();
            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilter = queryFilter.And(c => c.BUKRS == input.CompanyCode);
            }
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);
            }
            if (!string.IsNullOrEmpty(input.ReceivingPlantId))
            {
                queryFilter =
                    queryFilter.And(c => c.LACK1_PLANT.Any(p => p.PLANT_ID == input.ReceivingPlantId));
            }
            if (input.PeriodMonth.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c => c.PERIOD_MONTH.HasValue && c.PERIOD_MONTH.Value == input.PeriodMonth.Value);
            }
            if (input.PeriodYear.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c => c.PERIOD_YEAR.HasValue && c.PERIOD_YEAR.Value == input.PeriodYear.Value);
            }
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
                            c.APPROVED_DATE_POA.HasValue &&
                            c.APPROVED_DATE_POA.Value.Year == input.ApprovedDate.Value.Year &&
                            c.APPROVED_DATE_POA.Value.Month == input.ApprovedDate.Value.Month &&
                            c.APPROVED_DATE_POA.Value.Day == input.ApprovedDate.Value.Day);
            }
            if (!string.IsNullOrEmpty(input.ApprovedBy))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY_POA == input.ApprovedBy);
            }
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty(input.Approver))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY_MANAGER == input.Approver);
            }
            return _repository.Get(queryFilter, null, incTables).ToList();
        }

        public List<int> GetYearList()
        {
            var lack1Data = _repository.Get(c => c.PERIOD_YEAR.HasValue).ToList();
            if (lack1Data.Count > 0)
            {
                return lack1Data.Select(d => d.PERIOD_YEAR != null ? d.PERIOD_YEAR.Value : 0).Distinct().ToList();
            }
            return new List<int>();
        }

        public List<LACK1> GetByCompanyCode(string companyCode)
        {
            return _repository.Get(c => c.BUKRS == companyCode).ToList();
        }

        public List<LACK1> GetDetailReportByParamInput(Lack1GetDetailReportByParamInput input)
        {
            const string incTables = "LACK1_TRACKING, LACK1_TRACKING.INVENTORY_MOVEMENT, LACK1_INCOME_DETAIL, LACK1_INCOME_DETAIL.CK5, LACK1_INCOME_DETAIL.CK5.CK5_MATERIAL";
            //process QueryFilter
            Expression<Func<LACK1, bool>> queryFilter = PredicateHelper.True<LACK1>();
            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilter = queryFilter.And(c => c.BUKRS == input.CompanyCode);
            }
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbkcId);
            }
            if (!string.IsNullOrEmpty(input.ReceivingPlantId))
            {
                queryFilter =
                    queryFilter.And(c => c.LACK1_PLANT.Any(p => p.PLANT_ID == input.ReceivingPlantId));
            }
            if (!string.IsNullOrEmpty(input.SupplierPlantId))
            {
                queryFilter = queryFilter.And(c => c.SUPPLIER_PLANT_WERKS == input.SupplierPlantId);
            }
            if (input.PeriodMonthFrom.HasValue && input.PeriodYearFrom.HasValue)
            {
                var dtFrom = new DateTime(input.PeriodYearFrom.Value, input.PeriodMonthFrom.Value, 1);
                queryFilter = queryFilter.And(c => new DateTime(c.PERIOD_YEAR.Value, c.PERIOD_MONTH.Value, 1) >= dtFrom);
            }
            if (input.PeriodMonthTo.HasValue && input.PeriodYearTo.HasValue)
            {
                var dtTo = new DateTime(input.PeriodYearTo.Value, input.PeriodMonthTo.Value, 1);
                queryFilter = queryFilter.And(c => new DateTime(c.PERIOD_YEAR.Value, c.PERIOD_MONTH.Value, 1) <= dtTo);
            }
            return _repository.Get(queryFilter, null, incTables).ToList();
        }

    }
}
