using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class ReversalOutput : BLLBaseOutput
    {
        public bool IsMoreThanQuota { get; set; }
    }
}
