using System.Web.Mvc;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.Dashboard;

namespace Sampoerna.EMS.Website.Models.PBCK7AndPBCK3
{
    public class Pbck7Pbck3DashboardViewModel : BaseModel
    {
        public Pbck7Pbck3DashboardSearchViewModel SearchViewModel { get; set; }
        public DashboardDetilModel Detail { get; set; }
    }

    public class Pbck7Pbck3DashboardSearchViewModel
    {
        public int? ExecFromMonth { get; set; }
        public int? ExecFromYear { get; set; }
        public int? ExecToMonth { get; set; }
        public int? ExecToYear { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }
        public Enums.Pbck7Type Pbck7Type { get; set; }

        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList UserList { get; set; }
        public Enums.Pbck7Type Pbck7TypeList { get; set; }

    }
}