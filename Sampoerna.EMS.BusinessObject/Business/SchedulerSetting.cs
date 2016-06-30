using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class SchedulerSetting
    {
        public int DailyMinutes { get; set; }

        public int MonthlyMinutes { get; set; }

        public SchedulerConfigJson ConfigJson { get; set; }
    }
}
