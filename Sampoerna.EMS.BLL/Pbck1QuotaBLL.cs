using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class Pbck1QuotaBLL : IPbck1QuotaBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<PBCK1_QUOTA> _quotaRepository;
        private string includeTables = "CK5, PBCK1";

        public Pbck1QuotaBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _quotaRepository = _uow.GetGenericRepository<PBCK1_QUOTA>();
        }

        public List<Pbck1QuotaDto> GetAll()
        {
            var dbData = _quotaRepository.Get(null, null, includeTables);

            return Mapper.Map<List<Pbck1QuotaDto>>(dbData.ToList());
        }

        public List<Pbck1QuotaDto> GetByParam(Pbck1QuotaGetByParamInput input)
        {
            Expression<Func<PBCK1_QUOTA, bool>> queryFilter = PredicateHelper.True<PBCK1_QUOTA>();
            
            if (!string.IsNullOrEmpty(input.CompanyCode))
                queryFilter = queryFilter.And(c => c.PBCK1 != null && c.PBCK1.NPPBKC_BUKRS == input.CompanyCode);
            
            if (input.YearFrom.HasValue)
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.PBCK1 != null && c.PBCK1.PERIOD_FROM.HasValue &&
                            c.PBCK1.PERIOD_FROM.Value.Year >= input.YearFrom.Value);
            
            if (input.YearTo.HasValue)
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.PBCK1 != null && c.PBCK1.PERIOD_TO.HasValue &&
                            c.PBCK1.PERIOD_TO.Value.Year >= input.YearTo.Value);
            
            if (!string.IsNullOrEmpty(input.NppbkcId))
                queryFilter = queryFilter.And(c => c.PBCK1 != null && c.PBCK1.NPPBKC_ID == input.NppbkcId);

            Func<IQueryable<PBCK1_QUOTA>, IOrderedQueryable<PBCK1_QUOTA>> orderBy = null;
            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK1_QUOTA>(input.SortOrderColumn));
            }

            var dbData = _quotaRepository.Get(queryFilter, orderBy, includeTables);

            return Mapper.Map<List<Pbck1QuotaDto>>(dbData.ToList());

        }

    }
}
