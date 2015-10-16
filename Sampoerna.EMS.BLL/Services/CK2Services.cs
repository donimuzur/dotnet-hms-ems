
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class CK2Services : ICK2Services
    {
        private IGenericRepository<CK2> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string includeTables = "CK2_DOCUMENT";

        public CK2Services(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<CK2>();
            
        }

        public CK2 GetCk2ByPbck3Id(int pbck3Id)
        {
            var dbCk2 = _repository.Get(c => c.PBCK3_ID == pbck3Id, null, includeTables).FirstOrDefault();

            return dbCk2;

        }
    }
}
