using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IInventoryMovementService
    {
        List<INVENTORY_MOVEMENT> GetTotalUsageForLack1Byparam(InvMovementGetForLack1ByParamInput input);
    }
}