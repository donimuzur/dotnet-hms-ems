using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class StatusBLL : IStatusBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<STATUS> _repository;

        public StatusBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<STATUS>();
        }

        public STATUS GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<STATUS> GetAll()
        {
            return _repository.Get().ToList();
        }
    }
}
