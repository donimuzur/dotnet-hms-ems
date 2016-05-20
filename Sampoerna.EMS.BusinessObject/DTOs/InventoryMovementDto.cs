using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class InventoryMovementDto
    {
    }

    public class InventoryMovementLevelDto
    {
        public string Level { get; set; }
        public string FlavorCode { get; set; }
        public string FlavorDesc { get; set; }
        public string CfProdCode { get; set; }
        public string CfProdDesc { get; set; }
        public string CfProdQty { get; set; }
        public string CfProdUom { get; set; }
        public string ProdPostingDate { get; set; }
        public string ProdDate { get; set; }
    }
}
