using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2DashboardViewModel : BaseModel
    {
        public Lack2DashboardViewModel()
        {
            DataSource = new List<Lack2DashboardSourceItemModel>();
        }
        public Lack2DashboardSearchViewModel SearchViewModel { get; set; }
        public List<Lack2DashboardSourceItemModel> DataSource { get; set; }
    }

    public class Lack2DashboardSourceItemModel
    {
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public int TotalDocument { get; set; }
    }

    public class Lack2DashboardSearchViewModel
    {
        public int? PeriodFromMonth { get; set; }
        public int? PeriodFromYear { get; set; }
        public int? PeriodToMonth { get; set; }
        public int? PeriodToYear { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }
    }

}