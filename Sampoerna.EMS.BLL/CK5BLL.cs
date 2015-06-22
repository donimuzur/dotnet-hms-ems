using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class CK5BLL : ICK5BLL
    {
         private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<CK5> _repository;
        private string includeTables = "";

        public CK5BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK5>();
        }

        public List<CK5> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }


        public List<CK5> GetCK5ByParam(CK5Input input)
        {
            Expression<Func<CK5, bool>> queryFilter = PredicateHelper.True<CK5>();

            if (!string.IsNullOrEmpty(input.DocumentNumber))
            {
                queryFilter = queryFilter.And(c => c.CK5_NUMBER.Contains(input.DocumentNumber));
            }

            if (input.POA.HasValue)
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY.HasValue && c.APPROVED_BY.Value == input.POA.Value);
            }

            if (input.Creator.HasValue)
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY.HasValue && c.CREATED_BY.Value == input.Creator.Value);
            }

            if (input.NPPBKCOrigin.HasValue)
            {
                queryFilter = queryFilter.And(c => c.SOURCE_PLANT_ID.HasValue && c.SOURCE_PLANT_ID.Value == input.NPPBKCOrigin.Value);
            }

            if (input.NPPBKCDestination.HasValue)
            {
                queryFilter = queryFilter.And(c => c.DEST_PLANT_ID.HasValue && c.DEST_PLANT_ID.Value == input.NPPBKCDestination.Value);
            }

            //if (input.Ck5Type != null)
            //{
            //    queryFilter = queryFilter.And(c => c.CK5_TYPE == Sampoerna.EMS.Core.Enums.CK5Type);
            //}

            Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderBy = null;
            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<CK5>(input.SortOrderColumn));
            }

            var rc = _repository.Get(queryFilter, orderBy, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return rc.ToList();
        }
    }
}
