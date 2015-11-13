using System;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.WasteRole
{
    public class WasteRoleFormViewModel : BaseModel
    {
        public int WasteRoleId { get; set; }

        public string UserId { get; set; }
        public SelectList UserList { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Enums.WasteGroup WasteGroup { get; set; }
        public Enums.WasteGroup WasteGroupList { get; set; }
        public string WasteGroupDescription { get; set; }

        public string EmailAddress { get; set; }
        public string Phone { get; set; }

        public string PlantId { get; set; }
        public SelectList PlantList { get; set; }
        public string PlantDescription { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}