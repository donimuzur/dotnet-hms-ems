using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class PlantBLL : IPlantBLL
    {

        private IGenericRepository<T1001W> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        
        public PlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T1001W>();
        }

        public T1001W GetId(long id)
        {
            return _repository.GetByID(id);
        }

        public List<T1001W> GetAll()
        {
            return _repository.Get().ToList();
        }
    }
}
