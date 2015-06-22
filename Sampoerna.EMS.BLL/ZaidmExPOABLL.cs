using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExPOABLL : IZaidmExPOABLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_POA> _repository;
        private string includeTables = "ZAIDM_POA_MAP";

        public ZaidmExPOABLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_POA>();
        }

        public ZAIDM_EX_POA GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_POA> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public ZaidmExPOAOutput save(ZAIDM_EX_POA poa)
        {
            if (poa.POA_ID > 0)
            {
                //update
                _repository.Update(poa);
            }
            else
            {
                //Insert
                _repository.Insert(poa);
            }

            var output = new ZaidmExPOAOutput();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.PoaId = poa.POA_ID;
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
    }
}
