using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models.PBCK1 
{
    public class Pbck1ViewModel : BaseModel
    {
        public Pbck1ViewModel()
        {
            Details = new List<Pbck1Item>();
            SearchInput = new Pbck1FilterViewModel();
        }
        public List<Pbck1Item> Details { get; set; }

        public Pbck1FilterViewModel SearchInput { get; set; }
        
    }
    
}