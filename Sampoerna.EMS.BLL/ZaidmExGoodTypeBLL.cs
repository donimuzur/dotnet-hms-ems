using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExGoodTypeBLL : IZaidmExGoodTypeBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repository;
        
        public ZaidmExGoodTypeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
        }

        public ZAIDM_EX_GOODTYP GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_GOODTYP> GetAll()
        {
            return _repository.Get().ToList();
        }
    }
}
