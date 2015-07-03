using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
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
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private string includeTables = "PBCK1_PROD_CONVERTER, PBCK1_PROD_PLAN";

        public PBCK1BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PBCK1>();
            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
        }

        public List<Pbck1> GetPBCK1ByParam(Pbck1GetByParamInput input)
        {
            Expression<Func<PBCK1, bool>> queryFilter = PredicateHelper.True<PBCK1>();

            if (input.NppbkcId.HasValue)
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID.Value == input.NppbkcId.Value);
            }

            if (input.Pbck1Type.HasValue)
            {
                queryFilter = queryFilter.And(c => c.PBCK1_TYPE == input.Pbck1Type.Value);
            }

            if (input.Poa.HasValue)
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY.HasValue && c.APPROVED_BY.Value == input.Poa.Value);
            }

            if (input.Creator.HasValue)
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY.HasValue && c.CREATED_BY.Value == input.Creator.Value);
            }

            if (input.GoodTypeId.HasValue)
            {
                queryFilter = queryFilter.And(c => c.GOODTYPE_ID.HasValue && c.GOODTYPE_ID.Value == input.GoodTypeId.Value);
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

            return Mapper.Map<List<Pbck1>>(rc.ToList());
        }

        public Pbck1 GetById(long id)
        {
            var dbData = _repository.Get(c => c.PBCK1_ID == id, null, includeTables).FirstOrDefault();
            return Mapper.Map<Pbck1>(dbData);
        }

        public SavePbck1Output Save(Pbck1 pbck1)
        {
            PBCK1 dbData = null;
            if (pbck1.Pbck1Id > 0)
            {

                //update
                dbData = _repository.Get(c => c.PBCK1_ID == pbck1.Pbck1Id, null, includeTables).FirstOrDefault();
                Mapper.Map<Pbck1, PBCK1>(pbck1, dbData);

            }
            else
            {
                //Insert
                var input = new GenerateDocNumberInput()
                {
                    Year = pbck1.PeriodFrom.Year,
                    Month = pbck1.PeriodFrom.Month,
                    NppbkcId = pbck1.NppbkcId
                };
                
                pbck1.Pbck1Number = _docSeqNumBll.GenerateNumber(input);
                dbData = new PBCK1();
                Mapper.Map<Pbck1, PBCK1>(pbck1, dbData);
            }

            var output = new SavePbck1Output();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.Id = pbck1.Pbck1Id;
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
