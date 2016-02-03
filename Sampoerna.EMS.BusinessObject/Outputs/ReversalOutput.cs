using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class ReversalOutput : BLLBaseOutput
    {
        public bool IsPackedQtyNotExists { get; set; }
        public bool IsForCk4cCompleted { get; set; }
        public bool IsMoreThanQuota { get; set; }
        public decimal RemainingQuota { get; set; }
    }
}
