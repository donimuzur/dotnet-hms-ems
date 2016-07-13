using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.Website.Models.SchedulerSetting
{
    public class SchedulerSettingModel : BaseModel
    {
        public int DailyMinutes { get; set; }

        public int MonthlyMinutes { get; set; }
        public SchedulerConfigJson ConfigJson { get; set; }
    }
}