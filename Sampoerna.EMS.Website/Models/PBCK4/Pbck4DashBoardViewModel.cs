using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.Dashboard;

namespace Sampoerna.EMS.Website.Models.PBCK4
{
    public class Pbck4DashBoardViewModel : BaseModel
    {

        public Pbck4DashBoardViewModel()
        {
           Detail = new DashboardDetilModel();
        }
        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList UserList { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }

        public DashboardDetilModel Detail { get; set; }

    }
}