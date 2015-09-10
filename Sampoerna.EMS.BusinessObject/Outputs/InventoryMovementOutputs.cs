using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class InvMovementGetForLack1UsageMovementByParamOutput
    {
        public InvMovementGetForLack1UsageMovementByParamOutput()
        {
            IncludeInCk5List = new List<INVENTORY_MOVEMENT>();
            ExcludeFromCk5List = new List<INVENTORY_MOVEMENT>();
            ReceivingList = new List<INVENTORY_MOVEMENT>();
            AllUsageList = new List<INVENTORY_MOVEMENT>();
        }
        public List<INVENTORY_MOVEMENT> IncludeInCk5List { get; set; }
        public List<INVENTORY_MOVEMENT> ExcludeFromCk5List { get; set; }
        public List<INVENTORY_MOVEMENT> ReceivingList { get; set; }
        public List<INVENTORY_MOVEMENT> AllUsageList { get; set; }
    }
}
