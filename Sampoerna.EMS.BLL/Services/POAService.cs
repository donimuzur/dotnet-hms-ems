using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class POAService : IPoaService
    {
        private IGenericRepository<POA> _repository;

        private ILogger _logger;
        
        private string includeTables = "POA_MAP, USER, USER1, POA_SK";

        public POAService(IUnitOfWork uow, ILogger logger)
        {
            
            _logger = logger;
            _repository = uow.GetGenericRepository<POA>();
            
        }
        public POA GetById(string id)
        {
            return _repository.Get(p => p.POA_ID == id, null, includeTables).FirstOrDefault();
        }

        public void Save(POA poa)
        {
            _repository.InsertOrUpdate(poa);

        }
    }
}
