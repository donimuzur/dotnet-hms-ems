using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IInventoryMovementService
    {
        InvMovementGetForLack1UsageMovementByParamOutput GetForLack1UsageMovementByParam(InvMovementGetForLack1UsageMovementByParamInput input);
    }
}