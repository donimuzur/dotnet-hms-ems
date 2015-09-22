using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class T001WService : IT001WService
    {
        private IGenericRepository<T001W> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public T001WService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<T001W>();
        }

        public T001W GetById(string werks)
        {
            return _repository.GetByID(werks);
        }

        public List<T001W> GetByNppbkcId(string nppbkcId)
        {
            return _repository.Get(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && c.NPPBKC_ID == nppbkcId).ToList();
        }

        public T001W GetMainPlantByNppbkcId(string nppbkcId)
        {
            return
                _repository.Get(
                    c =>
                        !string.IsNullOrEmpty(c.NPPBKC_ID) && c.NPPBKC_ID == nppbkcId && c.IS_MAIN_PLANT.HasValue &&
                        c.IS_MAIN_PLANT.Value).FirstOrDefault();
        }
    }
}
