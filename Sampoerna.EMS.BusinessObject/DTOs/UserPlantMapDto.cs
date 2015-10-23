using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class UserPlantMapDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string PlantId { get; set; }

        public string UserName { get; set; }

        public string PlantName { get; set; }

        public bool IsActive { get; set; }
    }
}
