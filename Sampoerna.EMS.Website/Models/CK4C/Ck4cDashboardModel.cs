using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.Dashboard;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4cDashboardModel : BaseModel
    {
        public Ck4cDashboardModel()
        {
            Detil = new DashboardDetilModel();
        }
        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList UserList { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }

        public DashboardDetilModel Detil { get; set; }
    }
}