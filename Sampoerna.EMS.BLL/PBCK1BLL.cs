using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class PBCK1BLL : IPBCK1BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<PBCK1> _repository;
        private string includeTables = "";

        public PBCK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1>();
        }

        public List<PBCK1> GetPBCK1ByParam(PBCK1Input input)
        {
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            if (!string.IsNullOrEmpty(input.NPBCKID))
            {
                queryFilter = queryFilter.And(c => c.NUMBER.Contains(input.NPBCKID));
            }

            //if (!string.IsNullOrEmpty(input.Pbck1Type))
            //{
            //    queryFilter = queryFilter.And(c => c.PBCK1_TYPE.ToLower().Contains(input.Pbck1Type.ToLower()));
            //}

            if (input.POA.HasValue)
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY.HasValue && c.APPROVED_BY.Value == input.POA.Value);
            }

            if (input.Creator.HasValue)
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY.HasValue && c.CREATED_BY.Value == input.Creator.Value);
            }

            if (input.GoodType_ID.HasValue)
            {
                queryFilter = queryFilter.And(c => c.GOODTYPE_ID.HasValue && c.GOODTYPE_ID.Value == input.GoodType_ID.Value);
            }

            if (input.Year.HasValue)
            {
                queryFilter = queryFilter.And(c => (c.PERIOD_FROM.HasValue && c.PERIOD_FROM.Value.Year == input.Year.Value)
                    || (c.PERIOD_TO.HasValue && c.PERIOD_TO.Value.Year == input.Year.Value));
            }

            Func<IQueryable<PBCK1>, IOrderedQueryable<PBCK1>> orderBy = null;
            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK1>(input.SortOrderColumn));
            }

            var rc = _repository.Get(queryFilter, orderBy, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return rc.ToList();
        }

        public PBCK1 GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public SavePBCK1Output Save(PBCK1 pbck1)
        {
            if (pbck1.PBCK1_ID > 0)
            {
                //update
                _repository.Update(pbck1);
            }
            else
            {
                //Insert
                _repository.Insert(pbck1);
            }

            var output = new SavePBCK1Output();
            
            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.Id = pbck1.PBCK1_ID;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;
        }

        public DeletePBCK1Output Delete(long id)
        {
            var output = new DeletePBCK1Output();
            try
            {
                var dbData = _repository.GetByID(id);

                if (dbData == null)
                {
                    _logger.Error(new BLLException(ExceptionCodes.BLLExceptions.DataNotFound));
                    output.ErrorCode = ExceptionCodes.BLLExceptions.DataNotFound.ToString();
                    output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.DataNotFound);
                }
                else
                {
                    _repository.Delete(dbData);
                    _uow.SaveChanges();
                    output.Success = true;    
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;
        }

    }
}
