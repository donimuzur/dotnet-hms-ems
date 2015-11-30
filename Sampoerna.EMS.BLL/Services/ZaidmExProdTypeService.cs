using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class ZaidmExProdTypeService : IZaidmExProdTypeService
    {
        private IGenericRepository<ZAIDM_EX_PRODTYP> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public ZaidmExProdTypeService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_PRODTYP>();
        }

        public List<ZAIDM_EX_PRODTYP> GetAll()
        {
            return _repository.Get().ToList();
        }
    }
}
