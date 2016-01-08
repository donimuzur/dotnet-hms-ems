using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.Dashboard;
namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1DashboardViewModel : BaseModel
    {
        public Lack1DashboardViewModel()
        {
           Detail = new DashboardDetilModel();
        }

        public Lack1DashboardSearchViewModel SearchViewModel { get; set; }
        public DashboardDetilModel Detail { get; set; }
    }

    public class Lack1DashboardSearchViewModel
    {
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }

        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList UserList { get; set; }
    }
}