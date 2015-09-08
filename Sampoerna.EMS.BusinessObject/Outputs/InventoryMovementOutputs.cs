using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class InvMovementGetForLack1UsageMovementByParamOutput
    {
        public List<INVENTORY_MOVEMENT> IncludeInCk5List { get; set; }
        public List<INVENTORY_MOVEMENT> ExcludeFromCk5List { get; set; }
    }
}
