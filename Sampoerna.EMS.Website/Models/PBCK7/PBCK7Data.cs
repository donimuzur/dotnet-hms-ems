using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.PBCK7
{
    public class PBCK7Data
    {
        public string Pbck7Number { get; set; }
        public DateTime ReportedOn { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string Poa { get; set; }
        public int Status { get; set; }
        
    }
}