using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
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
    }
}
