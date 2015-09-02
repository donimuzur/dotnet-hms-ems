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
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class CK4CBLL : ICK4CBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<CK4C> _repository;
        private IMonthBLL _monthBll;
        private IChangesHistoryBLL _changesHistoryBll;

        private string includeTables = "POA, MONTH";

        public CK4CBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK4C>();
        }

        public List<Ck4CDto> GetAllByParam(Ck4CGetByParamInput input)
        {
            Expression<Func<CK4C, bool>> queryFilter = PredicateHelper.True<CK4C>();
            if (!string.IsNullOrEmpty(input.DateProduction))
            {
                var dt = Convert.ToDateTime(input.DateProduction);
                queryFilter = queryFilter.And(c => c.REPORTED_ON == dt);
            }
            
            if (!string.IsNullOrEmpty(input.Company))
            {
                queryFilter = queryFilter.And(c => c.COMPANY_ID == input.Company);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            }

            Func<IQueryable<CK4C>, IOrderedQueryable<CK4C>> orderBy = null;
            if (!string.IsNullOrEmpty(input.ShortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<CK4C>(input.ShortOrderColumn));
            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            var mapResult = Mapper.Map<List<Ck4CDto>>(dbData.ToList());

            return mapResult;
        }
        
        public Ck4CDto Save(Ck4CDto item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Invalid Data Entry");
                }
            }
            catch (Exception exception)
            {
                
                throw;
            }
        }
    }
}
