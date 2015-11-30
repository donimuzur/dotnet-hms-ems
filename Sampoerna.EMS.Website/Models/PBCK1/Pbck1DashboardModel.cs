using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.Dashboard;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1DashboardModel : BaseModel
    {
        public Pbck1DashboardModel()
        {
            Detil = new DashboardDetilModel();
        }
        public SelectList YearList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList UserList { get; set; }

        public int? Year { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }

        public DashboardDetilModel Detil { get; set; }
    }
}