using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class ZaidmExMaterialService : IZaidmExMaterialService
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public ZaidmExMaterialService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
        }

        public ZAIDM_EX_MATERIAL GetByMaterialAndPlantId(string materialId, string plantId)
        {
            return _repository.Get(c => c.STICKER_CODE == materialId && c.WERKS == plantId, null, "ZAIDM_EX_GOODTYP, UOM").FirstOrDefault();
        }

        public List<ZAIDM_EX_MATERIAL> GetByPlantId(string plantId)
        {
            return _repository.Get(c => c.WERKS == plantId).ToList();
        }

    }
}
