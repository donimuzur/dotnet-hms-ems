using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Dashboard
{
    public class DashboardDetilModel
    {
        public int DraftTotal { get; set; }
        public int WaitingForAppTotal { get; set; }
        public int WaitingForPoaTotal { get; set; }
        public int WaitingForManagerTotal { get; set; }
        public int WaitingForGovTotal { get; set; }
        public int CompletedTotal { get; set; }
    }
}