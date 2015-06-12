using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class StatusGovBLL : IStatusGovBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<STATUS_GOV> _repository;

        public StatusGovBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<STATUS_GOV>();
        }

        public STATUS_GOV GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<STATUS_GOV> GetAll()
        {
            return _repository.Get().ToList();
        }
    }
}
