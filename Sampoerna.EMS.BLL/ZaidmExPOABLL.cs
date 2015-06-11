using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
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

    }
}
