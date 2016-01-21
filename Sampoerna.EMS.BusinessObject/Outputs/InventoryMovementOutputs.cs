using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class InvMovementGetForLack1UsageMovementByParamOutput
    {
        public InvMovementGetForLack1UsageMovementByParamOutput()
        {
            IncludeInCk5List = new List<InvMovementItemWithConvertion>();
            ExcludeFromCk5List = new List<InvMovementItemWithConvertion>();
            ReceivingList = new List<InvMovementItemWithConvertion>();
            AllUsageList = new List<InvMovementItemWithConvertion>();
            Mvt201List = new List<InvMovementItemWithConvertion>();
        }
        public List<InvMovementItemWithConvertion> IncludeInCk5List { get; set; }
        public List<InvMovementItemWithConvertion> ExcludeFromCk5List { get; set; }
        public List<InvMovementItemWithConvertion> ReceivingList { get; set; }
        public List<InvMovementItemWithConvertion> AllUsageList { get; set; }
        public List<InvMovementItemWithConvertion> Mvt201List { get; set; }
        public List<InvMovementUsageProportional> UsageProportionalList { get; set; }
    }

    public class InvMovementGetForLack1UsageMovementForEtilAlcoholByParamOutput
    {
        public InvMovementGetForLack1UsageMovementForEtilAlcoholByParamOutput()
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
        public List<InvMovementUsageProportional> UsageProportionalList { get; set; }
    }

    public class InvMovementItemWithConvertion : INVENTORY_MOVEMENT
    {
        public string ConvertedUomId { get; set; }
        public string ConvertedUomDesc { get; set; }
        public decimal ConvertedQty { get; set; }
    }

    public class InvMovementUsageProportional
    {
        public string MaterialId { get; set; }
        public string Order { get; set; }
        public decimal Qty { get; set; }
        public decimal TotalQtyPerMaterialId { get; set; }
    }
}
