using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Market
{
    public class MarketModel
    {
        public string Market_Id { get; set; }
        public string Market_Desc { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime Modified_Date { get; set; }
        public string Created_By { get; set; }
        public string Modified_By { get; set; }
        public bool Is_Deleted { get; set; }
    }
}