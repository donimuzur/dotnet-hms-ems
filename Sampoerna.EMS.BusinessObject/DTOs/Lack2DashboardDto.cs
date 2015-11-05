using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack2DashboardDto
    {
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public int TotalDocument { get; set; }
    }
}
