using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class Login
    {
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string USER_GROUP_ID { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<int?> AuthorizePages { get; set; }

        
        public List<NppbkcPlantDto> NppbckPlants { get; set; } 
    }

}
