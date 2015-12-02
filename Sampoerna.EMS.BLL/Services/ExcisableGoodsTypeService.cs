using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class ExcisableGoodsTypeService : IExcisableGoodsTypeService
    {
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public ExcisableGoodsTypeService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
        }

        public ZAIDM_EX_GOODTYP GetById(string id)
        {
            return _repository.GetByID(id);
        }
    }
}
