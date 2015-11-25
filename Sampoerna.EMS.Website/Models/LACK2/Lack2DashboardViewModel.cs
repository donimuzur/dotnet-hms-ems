using System.Web.Mvc;
using Sampoerna.EMS.Website.Models.Dashboard;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2DashboardViewModel : BaseModel
    {
        public Lack2DashboardViewModel()
        {
            Detail = new DashboardDetilModel();
        }
        public Lack2DashboardSearchViewModel SearchViewModel { get; set; }
        public DashboardDetilModel Detail { get; set; }
    }
    
    public class Lack2DashboardSearchViewModel
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