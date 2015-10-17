using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class NppbkcPlantDto
    {
        public string NppbckId { get; set; }
        public string CompanyCode { get; set; }

        public List<PlantDto> Plants { get; set; }
    }
}
