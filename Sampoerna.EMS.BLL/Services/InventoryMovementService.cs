using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
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
            var inventoryMovements = movementUsageAll.ToArray();

            var receivingCk5MvtType = EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Ck5Receiving);

            //get receiving data
            var ck5ReceivingData = (from rec in _repository.Get(c => !string.IsNullOrEmpty(c.MVT) && c.MVT == receivingCk5MvtType)
                join a in inventoryMovements on new {rec.BATCH, rec.MATERIAL_ID } equals new {a.BATCH, a.MATERIAL_ID }
                select  rec).ToList();

            var batchList = ck5ReceivingData.Select(d => d.BATCH).Distinct().ToList();
            var materialIdList = ck5ReceivingData.Select(d => d.MATERIAL_ID).Distinct().ToList();

            //get Movement only in CK5 List
            var movementIncludeInCk5List = (inventoryMovements.Where(
                all => batchList.Contains(all.BATCH) && materialIdList.Contains(all.MATERIAL_ID))).ToList();

            //get exclude in CK5 List
            var movementExclueInCk5List = (inventoryMovements.Where(
                all => !movementIncludeInCk5List.Select(d => d.INVENTORY_MOVEMENT_ID)
                    .ToList()
                    .Contains(all.INVENTORY_MOVEMENT_ID))).ToList();

            var rc = new InvMovementGetForLack1UsageMovementByParamOutput()
            {
                IncludeInCk5List = movementIncludeInCk5List,
                ExcludeFromCk5List = movementExclueInCk5List,
                Ck5ReceivingList = ck5ReceivingData
            };
            
            return rc;
        }

    }
}
