using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IInventoryMovementService
    {
        
        List<INVENTORY_MOVEMENT> GetUsageByParam(InvMovementGetUsageByParamInput input);

        List<INVENTORY_MOVEMENT> GetReceivingByParam(InvMovementGetReceivingByParamInput input);

    }
}