using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class T001KService : IT001KService
    {

        private IGenericRepository<T001K> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string includeTables = "T001";

        public T001KService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T001K>();
        }

        public T001K GetByBwkey(string input)
        {
            return _repository.Get(c => c.BWKEY == input, null, includeTables).FirstOrDefault();
        }
    }
}
