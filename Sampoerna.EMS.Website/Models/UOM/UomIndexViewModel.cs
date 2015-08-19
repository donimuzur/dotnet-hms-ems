using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.UOM
{
    public class UomIndexViewModel : BaseModel
    {
        public List<UomDetails> Details;
    }

    public class UomDetails
    {
        public string UomId { get; set; }
       
        public string UomName { get; set; }

        public string IsEmsString { get; set; }
    }
}