using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.POA
{
    public class POAViewModel : BaseModel
    {
        public POAViewModel()
        {
            Details = new List<ZAIDM_EX_POA>();
        }
        public List<ZAIDM_EX_POA> Details { get; set; }

       
    }
    
}