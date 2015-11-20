using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class WasteRoleDto
    {
        public WasteRoleDto()
        {
            Details = new List<WasteRoleDetailsDto>();
        }
        public int WASTE_ROLE_ID { get; set; }
        public string USER_ID { get; set; }
        public Core.Enums.WasteGroup GROUP_ROLE { get; set; }
        public string WERKS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WasteGroupDescription { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string PlantDescription { get; set; }

        public List<WasteRoleDetailsDto> Details { get; set; }
    }

    public class WasteRoleDetailsDto
    {
        public Enums.WasteGroup WasteGroup { get; set; }
        public string WasteGroupDescription { get; set; }
        public bool IsChecked { get; set; }
    }
}
