using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.BusinessObject;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class POASKBLL : IPOASKBLL
    {
        private IGenericRepository<POA_SK> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public POASKBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<POA_SK>();
          
        }


        public void Save(POA_SK poaSK)
        {
           _repository.InsertOrUpdate(poaSK);
        }

        public int RemovePoaSk(int poaSkId)
        {
            try
            {
                _repository.Delete(poaSkId);
                _uow.SaveChanges();
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;

        }
    }
}
