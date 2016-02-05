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
    public class MaterialBalanceService : IMaterialBalanceService
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL_BALANCE> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public MaterialBalanceService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL_BALANCE>();
        }

        public List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantAndMaterialList(List<string> plantId, List<string> materialList, int month, int year)
        {
            Expression<Func<ZAIDM_EX_MATERIAL_BALANCE, bool>> queryFilter =
                c => plantId.Contains(c.WERKS) && materialList.Contains(c.MATERIAL_ID)
                    && c.PERIOD_MONTH == month && c.PERIOD_YEAR == year;

            return _repository.Get(queryFilter).ToList();
        }

        public List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantListAndMaterialList(List<string> plantId, List<string> materialList)
        {
            Expression<Func<ZAIDM_EX_MATERIAL_BALANCE, bool>> queryFilter =
                c => plantId.Contains(c.WERKS) && materialList.Contains(c.MATERIAL_ID);

            return _repository.Get(queryFilter).ToList();
        }
    }
}
