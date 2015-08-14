using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using AutoMapper;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class LACK1BLL : ILACK1BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK1> _repository;
        private IMonthBLL _monthBll;
        private IUnitOfMeasurementBLL _uomBll;

        private string includeTables = "MONTH, UOM";

        public LACK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK1>();
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
        }


        public List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input)
        {
            Expression<Func<LACK1, bool>> queryFilter = PredicateHelper.True<LACK1>();

            if (!string.IsNullOrEmpty(input.NppbKcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbKcId);
            }
            if (!string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId);
                //queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId && c.LEVEL_PLANT_NAME == input.PlantId);
            }
            if (!string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            //if (input.PeriodMonth != null)
            //{
            //    queryFilter = queryFilter.And(c => c.PERIOD_MONTH == input.PeriodMonth);
            //}
            //if (input.PeriodYear != null)
            //{
            //    queryFilter = queryFilter.And(c => c.PERIOD_YEAR == input.PeriodYear);
            //}
            if (!string.IsNullOrEmpty((input.SubmissionDate)))
            {
                var dt = Convert.ToDateTime(input.SubmissionDate);
                DateTime dt2 = DateTime.ParseExact("07/01/2015", "MM/dd/yyyy", CultureInfo.InvariantCulture);
                queryFilter = queryFilter.And(c => dt2.Date.ToString().Contains(c.SUBMISSION_DATE.ToString()));
            }
           
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

            var mapResult = Mapper.Map<List<Lack1Dto>>(dbData.ToList());

            return mapResult;
        }

        public decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input)
        {
            var dtTo = new DateTime(input.YearTo, input.MonthTo, 1);

            var getData = _repository.Get(c => c.NPPBKC_ID == input.NppbkcId
                                               && c.STATUS.HasValue &&
                                               c.STATUS.Value >= (int) Enums.DocumentStatus.Approved, null,
                "LACK1_ITEM").ToList().Select(p => new
                {
                    p.LACK1_ID,
                    p.LACK1_NUMBER,
                    p.PERIOD_MONTH,
                    p.PERIOD_YEAR,
                    PERIODE = new DateTime(p.PERIOD_YEAR.Value, p.PERIOD_MONTH.Value, 1),
                    p.LACK1_ITEM
                }
                ).ToList();

            if (getData.Count == 0) return 0;

            var selected = getData.Where(c => c.PERIODE <= dtTo).OrderByDescending(o => o.PERIODE).FirstOrDefault();

            if (selected == null) return 0;
            
            decimal rc = 0;
            var dataGrouped = selected.LACK1_ITEM.GroupBy(p => new
            {
                p.LACK1_ID
            }).Select(g => new
            {
                g.Key.LACK1_ID,
                TotalBEGINNING_BALANCE = g.Sum(p => p.BEGINNING_BALANCE != null ? p.BEGINNING_BALANCE.Value : 0),
                TotalINCOME = g.Sum(p => p.INCOME != null ? p.INCOME.Value : 0),
                TotalUSAGE = g.Sum(p => p.USAGE != null ? p.USAGE.Value : 0)
            }).FirstOrDefault();

            if (dataGrouped != null)
                rc = dataGrouped.TotalBEGINNING_BALANCE + dataGrouped.TotalINCOME - dataGrouped.TotalUSAGE;
            return rc;
        }
    }
}
