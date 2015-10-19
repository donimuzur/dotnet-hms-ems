using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack2ItemService : ILack2ItemService
    {
        private IGenericRepository<LACK2_ITEM> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public Lack2ItemService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK2_ITEM>();
        }

        public void DeleteByLack2Id(int lack2Id)
        {
            var dataToDelete = _repository.Get(c => c.LACK2_ID == lack2Id);
            if (dataToDelete != null)
            {
                foreach (var item in dataToDelete.ToList())
                {
                    _repository.Delete(item);
                }
            }
        }
        public void DeleteDataList(IEnumerable<LACK2_ITEM> listToDelete)
        {
            if (listToDelete == null) return;
            foreach (var item in listToDelete.ToList())
            {
                _repository.Delete(item);
            }
        }
    }
}
