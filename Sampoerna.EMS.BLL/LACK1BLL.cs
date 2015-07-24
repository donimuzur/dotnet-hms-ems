using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
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
            var queryFilter = ProcessQueryFilter(input);

            return GetLack1Data(queryFilter, input.SortOrderColumn);
           
        }
        

        public List<Lack1Dto> GetListByNpbkcParam(Lack1GetListByNppbkcParam input)
        {
            var queryFilter = ProcessQueryFilter(input);
            queryFilter = queryFilter.And( c => c.STATUS != Enums)

        }

        public List<Lack1Dto> GetListByPlantParam(Lack1GetListByPlantParam input)
        {
            throw new NotImplementedException();
        }

        private Expression<Func<LACK1, bool>> ProcessQueryFilter(Lack1GetByParamInput input)
        {
            Expression<Func<LACK1, bool>> queryFilter = PredicateHelper.True<LACK1>();
            if (!string.IsNullOrEmpty(input.NppbKcId))
            {
                queryFilter = queryFilter.And(c => c.BUKRS == input.NppbKcId);
            }
            if (string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.BUTXT == input.PlantId);
            }
            if (string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            return queryFilter;
        }

        private List<Lack1Dto> GetLack1Data(Expression<Func<LACK1, bool>> queryFilter, string orderColumn)
        {
            Func<IQueryable<LACK1>, IOrderedQueryable<LACK1>> orderBy = null;
            if(!string.IsNullOrEmpty(orderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK1>(orderColumn));
                    
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);

            var mapResult = Mapper.Map<List<Lack1Dto>>(dbData.ToList());

            return mapResult;
        }

    }
}
