using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class NppbkcPlantDto
    {
        public string NppbckId { get; set; }

        public List<PlantDto> Plants { get; set; }
    }
}
