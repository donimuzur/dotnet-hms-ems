using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class Lack2GetDashboardDataOutput : BLLBaseOutput
    {
        public List<Lack2DashboardDto> DataList { get; set; }
    }
}
