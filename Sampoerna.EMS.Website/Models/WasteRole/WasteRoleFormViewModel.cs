using System;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.WasteRole
{
    public class WasteRoleFormViewModel : BaseModel
    {
        public int WasteRoleId { get; set; }

        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public int WasteGroup { get; set; }
        public SelectList WasteGroupList { get; set; }
        public string WasteGroupDescription { get; set; }

        public string EmailAddress { get; set; }
        public string Phone { get; set; }

        public string PlantId { get; set; }
        public string PlantDescription { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
    }
}