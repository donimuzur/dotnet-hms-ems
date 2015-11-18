using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class WasteStockServices : IWasteStockServices
    {
        private IGenericRepository<WASTE_STOCK> _repository;
        
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string _includeTables = "ZAIDM_EX_MATERIAL, USER, T001W";

        public WasteStockServices(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<WASTE_STOCK>();
            
        }

        public List<WASTE_STOCK> GetListByPlant(string plantId)
        {
            return _repository.Get(c => c.WERKS == plantId).ToList();
        }

        public WASTE_STOCK GetByPlantAndMaterialNumber(string plantId, string materialNumber)
        {
            return _repository.Get(c => c.WERKS == plantId && c.MATERIAL_NUMBER == materialNumber).FirstOrDefault();
        }
    }
}
