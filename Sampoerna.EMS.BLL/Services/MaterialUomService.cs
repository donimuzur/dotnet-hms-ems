using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class MaterialUomService : IMaterialUomService
    {
        private IGenericRepository<MATERIAL_UOM> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "ZAIDM_EX_MATERIAL";

        public MaterialUomService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<MATERIAL_UOM>();
        }

        public List<MATERIAL_UOM> GetByMaterialListAndPlantId(List<string> materialList, string plantId)
        {
            Expression<Func<MATERIAL_UOM, bool>> queryFilter =
                c => c.WERKS == plantId && materialList.Contains(c.STICKER_CODE);
            return _repository.Get(queryFilter, null, "ZAIDM_EX_MATERIAL").ToList();
        }

        public List<MATERIAL_UOM> GetByMaterialListAndPlantIdListSpecificBkcUom(List<string> materialList, List<string> plantId,
            string bkcUomId)
        {
            Expression<Func<MATERIAL_UOM, bool>> queryFilter =
               c => plantId.Contains(c.WERKS) && materialList.Contains(c.STICKER_CODE) && c.MEINH == bkcUomId;
            return _repository.Get(queryFilter, null, "ZAIDM_EX_MATERIAL").ToList();
        }


    }
}
