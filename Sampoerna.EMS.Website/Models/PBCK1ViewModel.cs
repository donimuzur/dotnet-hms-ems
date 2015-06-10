using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models 
{
    public class PBCK1ViewModel : BaseModel
    {
        public List<PBCK1Item> Details { get; set; }
    }

    public class PBCK1Item
    {
        
    }

    public class PBCK1ItemViewModel : BaseModel
    {
        public PBCK1Item Detail { get; set; }
    }
}