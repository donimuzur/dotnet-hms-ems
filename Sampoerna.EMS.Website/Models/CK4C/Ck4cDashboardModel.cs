using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4cDashboardModel : BaseModel
    {
        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList UserList { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }

        public int DraftTotal { get; set; }
        public int WaitingForPoaTotal { get; set; }
        public int WaitingForManagerTotal { get; set; }
        public int WaitingForGovTotal { get; set; }
        public int CompletedTotal { get; set; }
    }
}