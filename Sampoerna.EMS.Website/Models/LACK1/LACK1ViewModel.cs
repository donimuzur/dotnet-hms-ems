using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class LACK1ViewModel : BaseModel
    {
        public LACK1ViewModel()
        {
            Details = new List<LACK1Item>();
            SearchInput = new LACK1FilterViewModel();
        }

        public List<LACK1Item> Details { get; set; }

        public LACK1FilterViewModel SearchInput { get; set; }
    }
}