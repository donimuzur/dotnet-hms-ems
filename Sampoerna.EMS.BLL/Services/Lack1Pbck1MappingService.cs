using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack1Pbck1MappingService : ILack1Pbck1MappingService
    {
        private IGenericRepository<LACK1_PBCK1_MAPPING> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public Lack1Pbck1MappingService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<LACK1_PBCK1_MAPPING>();
        }

        public void DeleteByLack1Id(int lack1Id)
        {
            var dataToDelete = _repository.Get(c => c.LACK1_ID == lack1Id);
            if (dataToDelete != null)
            {
                foreach (var item in dataToDelete.ToList())
                {
                    _repository.Delete(item);
                }
            }
        }

    }
}
