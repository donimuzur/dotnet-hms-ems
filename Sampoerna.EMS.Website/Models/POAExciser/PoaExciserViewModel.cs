using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.POAExciser
{
    public class PoaExciserViewModel : BaseModel
    {
        public long EXCISER_ID { get; set; }
        public string POA_ID { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
   
}