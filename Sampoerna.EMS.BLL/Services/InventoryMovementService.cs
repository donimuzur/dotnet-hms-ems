using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class InventoryMovementService : IInventoryMovementService
    {

        private IGenericRepository<INVENTORY_MOVEMENT> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IT001WService _t001wService;

        public InventoryMovementService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<INVENTORY_MOVEMENT>();
            _t001wService = new T001WService(_uow, _logger);
        }

        public List<INVENTORY_MOVEMENT> GetTotalUsageForLack1Byparam(InvMovementGetForLack1ByParamInput input)
        {
            var mvtTypeForUsage = new List<string>
            {
                EnumHelper.GetDescription(Enums.MovementTypeCode.ProductionAdd),
                EnumHelper.GetDescription(Enums.MovementTypeCode.ProductionMin)
            };

            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => !string.IsNullOrEmpty(c.MVT) && mvtTypeForUsage.Contains(c.MVT)
                && c.POSTING_DATE.HasValue && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;

            if (input.Lack1Level == Enums.Lack1Level.Nppbkc)
            {
                //get plant list by nppbkcid
                var plantList = _t001wService.GetByNppbkcId(input.NppbkcId);
                if (plantList.Count > 0)
                {
                    var plantIdList = plantList.Select(c => c.WERKS).ToList();
                    queryFilter = queryFilter.And(c => plantIdList.Contains(c.PLANT_ID));
                }
            }
            else
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            }

            return _repository.Get(queryFilter).ToList();
        }
    }
}
