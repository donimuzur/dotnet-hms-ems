using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.SchedulerSetting
{
    public class SchedulerSettingModel : BaseModel
    {
        public int DailyMinutes { get; set; }

        public int MonthlyMinutes { get; set; }

    }
}