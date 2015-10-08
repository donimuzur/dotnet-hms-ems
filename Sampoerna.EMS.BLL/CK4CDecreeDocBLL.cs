using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class CK4CDecreeDocBLL : ICK4CDecreeDocBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<CK4C_DECREE_DOC> _repository;

        public CK4CDecreeDocBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK4C_DECREE_DOC>();
        }

        public void DeleteByCk4cId(long ck4cId)
        {
            var dataToDelete = _repository.Get(c => c.CK4C_ID == ck4cId);
            if (dataToDelete != null)
            {
                foreach (var ck4cItem in dataToDelete.ToList())
                {
                    _repository.Delete(ck4cItem);
                }
            }
        }
    }
}
