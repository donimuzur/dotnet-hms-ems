using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class T001Service : IT001Service
    {
        private IGenericRepository<T001> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public T001Service(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<T001>();
        }

        public T001 GetById(string id)
        {
            return _repository.GetByID(id);
        }
    }
}
