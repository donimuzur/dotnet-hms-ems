using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class WasteRoleBLL : IWasteRoleBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<WASTE_ROLE> _repository;

        public WasteRoleBLL(IUnitOfWork uow, ILogger logger)
        {
           _uow = uow;
           _logger = logger;
            //_repository = _uow.GetGenericRepository<BACK1>();

        }
    }
}
