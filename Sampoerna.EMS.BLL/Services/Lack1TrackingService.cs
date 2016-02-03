using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack1TrackingService : ILack1TrackingService
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK1_TRACKING> _repository;

        public Lack1TrackingService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<LACK1_TRACKING>();
        }

        public void DeleteByLack1Id(int lack1Id)
        {
            var dataToDelete = _repository.Get(c => c.LACK1_ID == lack1Id);
            if (dataToDelete != null)
            {
                foreach (var item in dataToDelete.ToList())
                {
                    _repository.Delete(item);
                }
            }
        }
        public void DeleteDataList(IEnumerable<LACK1_TRACKING> listToDelete)
        {
            if (listToDelete != null)
            {
                foreach (var item in listToDelete)
                {
                    _repository.Delete(item);
                }
            }
        }

        public List<string> GetMovement201FromTracking()
        {
            var data = _repository.Get(x => x.INVENTORY_MOVEMENT.MVT == Core.Enums.MovementTypeCode.Usage201.ToString(), null,
                "INVENTORY_MOVEMENT").Select(x => x.INVENTORY_MOVEMENT.MAT_DOC).ToList();
            return data;
        }   
    }
}
