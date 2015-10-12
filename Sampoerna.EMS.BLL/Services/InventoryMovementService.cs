using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.LinqExtensions;
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

            var receivingMvtType = new List<string>()
            {
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101),
                EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving102)
            };

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
            
            //there is records on receiving Data
            var receivingList = (from rec in _repository.Get(c => receivingMvtType.Contains(c.MVT))
                                  join a in inventoryMovements on new { rec.BATCH, rec.MATERIAL_ID } equals new { a.BATCH, a.MATERIAL_ID }
                                  where input.StoReceiverNumberList.Contains(rec.PURCH_DOC)
                                  select rec).DistinctBy(d => d.INVENTORY_MOVEMENT_ID).ToList();

            var usageReceivingList = (from rec in _repository.Get(c => receivingMvtType.Contains(c.MVT))
                                      join a in inventoryMovements on new { rec.BATCH, rec.MATERIAL_ID } equals new { a.BATCH, a.MATERIAL_ID }
                                      where input.StoReceiverNumberList.Contains(rec.PURCH_DOC)
                                      select a).DistinctBy(d => d.INVENTORY_MOVEMENT_ID).ToList();

            //get exclude in receiving data
            var movementExclueInCk5List = (inventoryMovements.Where(
                all => !usageReceivingList.Select(d => d.INVENTORY_MOVEMENT_ID)
                    .ToList()
                    .Contains(all.INVENTORY_MOVEMENT_ID))).DistinctBy(d => d.INVENTORY_MOVEMENT_ID).ToList();

            var rc = new InvMovementGetForLack1UsageMovementByParamOutput()
            {
                IncludeInCk5List = usageReceivingList,
                ExcludeFromCk5List = movementExclueInCk5List,
                ReceivingList = receivingList,
                AllUsageList = inventoryMovements
            };
            
            return rc;
        }

    }
}
