using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models.WasteRole
{
    public class WasteRoleIndexViewModel : BaseModel
    {
        public WasteRoleIndexViewModel()
        {
            ListWasteRoles = new List<WasteRoleFormViewModel>();
        }
        public List<WasteRoleFormViewModel> ListWasteRoles { get; set; }
    }
}