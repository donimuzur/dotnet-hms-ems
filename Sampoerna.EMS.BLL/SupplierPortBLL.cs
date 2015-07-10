using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class SupplierPortBLL : ISupplierPortBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<SUPPLIER_PORT> _repository;

        public SupplierPortBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<SUPPLIER_PORT>();
        }
        public SUPPLIER_PORT GetById(int id)
        {
            return _repository.GetByID(id);
        }

        public List<SUPPLIER_PORT> GetAll()
        {
            return _repository.Get().ToList();
        }
    }
}
