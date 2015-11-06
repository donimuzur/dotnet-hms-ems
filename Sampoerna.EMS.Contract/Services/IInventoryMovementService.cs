using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IInventoryMovementService
    {
        InvMovementGetForLack1UsageMovementByParamOutput GetForLack1UsageMovementByParam(InvMovementGetForLack1UsageMovementByParamInput input);

        List<INVENTORY_MOVEMENT> GetUsageByParam(InvMovementGetUsageByParamInput input);

        List<INVENTORY_MOVEMENT> GetReceivingByParam(InvMovementGetReceivingByParamInput input);

    }
}