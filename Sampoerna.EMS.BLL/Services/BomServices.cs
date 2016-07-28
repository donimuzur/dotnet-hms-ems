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
    public class BomServices : IBomService
    {
        private IGenericRepository<BOM> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public BomServices(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<BOM>();
        }

        public List<BOM> GetBomByPlantIdAndMaterial(string plantId, string materialId)
        {
            var data = _repository.Get(x => x.PLANT == plantId && x.MATERIAL_ID == materialId);
            return data.ToList();
        }
    }
}
