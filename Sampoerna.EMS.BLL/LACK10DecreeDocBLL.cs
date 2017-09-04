using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class LACK10DecreeDocBLL : ILACK10DecreeDocBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK10_DECREE_DOC> _repository;

        public LACK10DecreeDocBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK10_DECREE_DOC>();
        }

        public void DeleteByLack10Id(long lack10Id)
        {
            var dataToDelete = _repository.Get(c => c.LACK10_ID == lack10Id);
            if (dataToDelete != null)
            {
                foreach (var lack10Item in dataToDelete.ToList())
                {
                    _repository.Delete(lack10Item);
                }
            }
        }
    }
}
