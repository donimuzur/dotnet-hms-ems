using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class MaterialBLL : IMaterialBLL
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL> _repository;
        
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "T1001W, ZAIDM_EX_BRAND, ZAIDM_EX_GOODTYP, UOM";

        public MaterialBLL(IUnitOfWork uow, ILogger logger){
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
        }

        public ZAIDM_EX_MATERIAL getByID(long materialId)
        {
            return _repository.GetByID(materialId);
        }

        public List<ZAIDM_EX_MATERIAL> getAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public BusinessObject.Outputs.MaterialOutput Save(BusinessObject.ZAIDM_EX_MATERIAL data)
        {
            
        }
    }
}
