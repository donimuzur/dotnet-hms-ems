using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class WasteRoleDto
    {
        public int WASTE_ROLE_ID { get; set; }
        public string USER_ID { get; set; }
        public int GROUP_ROLE { get; set; }
        public string WERKS { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WasteGroupDescription { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string PlantDescription { get; set; }
    
    }
}
