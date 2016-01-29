using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ReversalBLL : IReversalBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<REVERSAL> _repository;

        public ReversalBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<REVERSAL>();
        }

        public List<ReversalDto> GetListDocumentByParam(ReversalGetByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            return Mapper.Map<List<ReversalDto>>(GetReversalData(queryFilter, input.ShortOrderColumn));
        }

        private Expression<Func<REVERSAL, bool>> ProcessQueryFilter(ReversalGetByParamInput input)
        {
            Expression<Func<REVERSAL, bool>> queryFilter = PredicateHelper.True<REVERSAL>();

            if (!string.IsNullOrEmpty(input.DateProduction))
            {
                var dt = Convert.ToDateTime(input.DateProduction);
                queryFilter = queryFilter.And(c => c.PRODUCTION_DATE == dt);
            }

            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.WERKS == input.PlantId);
            }
            
            return queryFilter;
        }

        private List<REVERSAL> GetReversalData(Expression<Func<REVERSAL, bool>> queryFilter, string orderColumn)
        {
            Func<IQueryable<REVERSAL>, IOrderedQueryable<REVERSAL>> orderBy = null;
            if (!string.IsNullOrEmpty(orderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<REVERSAL>(orderColumn));
            }

            var dbData = _repository.Get(queryFilter);

            return dbData.ToList();
        }
    }
}
