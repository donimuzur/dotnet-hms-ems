using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models.PBCK1 
{
    public class PBCK1ViewModel : BaseModel
    {
        public PBCK1ViewModel()
        {
            Details = new List<PBCK1Item>();
            SearchInput = new PBCK1FilterViewModel();
        }
        public List<PBCK1Item> Details { get; set; }

        public PBCK1FilterViewModel SearchInput { get; set; }
        
    }
    
}