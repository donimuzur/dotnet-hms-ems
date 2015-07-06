using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExNPPBKCBLL : IZaidmExNPPBKCBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _repository;

        public ZaidmExNPPBKCBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
        }

        public ZAIDM_EX_NPPBKC GetById(long id)
        {
            return _repository.GetByID(id);
        }
        
        public List<ZAIDM_EX_NPPBKC> GetAll()
        {
            return _repository.Get().ToList();
        }

        public string GetCityByNppbkcId(long nppBkcId)
        {
            var dbData = _repository.GetByID(nppBkcId);
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData.CITY;

        }

        public string GetCeOfficeCodeByNppbcId(long nppBkcId)
        {

            var dbData = _repository.Get(n => n.NPPBKC_ID == nppBkcId, null, "ZAIDM_EX_KPPBC").FirstOrDefault();
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbData.ZAIDM_EX_KPPBC.KPPBC_NUMBER;

        }
    }
}
