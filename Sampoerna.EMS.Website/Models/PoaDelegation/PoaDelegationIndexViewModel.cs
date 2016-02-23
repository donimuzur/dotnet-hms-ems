using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models.PoaDelegation
{
    public class PoaDelegationIndexViewModel : BaseModel
    {
        public PoaDelegationIndexViewModel()
        {
            ListPoaDelegations = new List<PoaDelegationFormViewModel>();
        }

        public List<PoaDelegationFormViewModel> ListPoaDelegations { get; set; }

    }
}