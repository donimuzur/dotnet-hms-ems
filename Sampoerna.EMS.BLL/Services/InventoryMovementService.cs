using System;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class InventoryMovementService : IInventoryMovementService
    {

        private IGenericRepository<INVENTORY_MOVEMENT> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public InventoryMovementService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<INVENTORY_MOVEMENT>();
        }

        public InvMovementGetForLack1UsageMovementByParamOutput GetForLack1UsageMovementByParam(InvMovementGetForLack1UsageMovementByParamInput input)
        {
            Expression<Func<INVENTORY_MOVEMENT, bool>> queryFilter = c => c.POSTING_DATE.HasValue
                && c.POSTING_DATE.Value.Year == input.PeriodYear && c.POSTING_DATE.Value.Month == input.PeriodMonth;

            if (input.PlantIdList.Count > 0)
            {
                queryFilter = queryFilter.And(c => input.PlantIdList.Contains(c.PLANT_ID));
            }

            if (input.MvtCodeList.Count > 0)
            {
                queryFilter = queryFilter.And(c => !string.IsNullOrEmpty(c.MVT) && input.MvtCodeList.Contains(c.MVT));
            }

            //get 100% usage from INVENTORY_MOVEMENT
            var movementUsageAll = _repository.Get(queryFilter);
            var inventoryMovements = movementUsageAll.ToList();

            var receivingMvtType = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving);

            //get usage in receiving data
            var usageReceivingData = (from rec in _repository.Get(c => !string.IsNullOrEmpty(c.MVT) && c.MVT == receivingMvtType)
                join a in inventoryMovements on rec.MATERIAL_ID equals a.MATERIAL_ID
                where input.StoReceiverNumberList.Contains(rec.PURCH_DOC)
                select  a).ToList();

            //get receiving data
            var receivingData = (from rec in _repository.Get(c => !string.IsNullOrEmpty(c.MVT) && c.MVT == receivingMvtType)
                                 join a in inventoryMovements on rec.MATERIAL_ID equals a.MATERIAL_ID
                                 where input.StoReceiverNumberList.Contains(rec.PURCH_DOC)
                                 select rec).ToList();
            
            //get exclude in receiving data
            var movementExclueInCk5List = (inventoryMovements.Where(
                all => !usageReceivingData.Select(d => d.INVENTORY_MOVEMENT_ID)
                    .ToList()
                    .Contains(all.INVENTORY_MOVEMENT_ID))).ToList();

            var rc = new InvMovementGetForLack1UsageMovementByParamOutput()
            {
                IncludeInCk5List = usageReceivingData,
                ExcludeFromCk5List = movementExclueInCk5List,
                ReceivingList = receivingData,
                AllUsageList = inventoryMovements
            };
            
            return rc;
        }

    }
}
