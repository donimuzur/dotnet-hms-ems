using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.Contract
{
    public interface ISchedulerSettingBLL
    {
        SchedulerSetting GetMinutesCron();

        SchedulerConfigJson GetConfigJson();

        void SetConfigJsonFile(string fileName);

        void SetXmlFile(string fileName);

        void Save(SchedulerSetting data);
    }
}
