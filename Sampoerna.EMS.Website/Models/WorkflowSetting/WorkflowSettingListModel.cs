using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.WorkflowSetting
{
    public class WorkflowSettingListModel : BaseModel
    {
        public WorkflowSettingListModel() {
            Details = new List<WorkflowDetails>();
        }
        public List<WorkflowDetails> Details { get; set; }
    }

    public class WorkflowDetails {
        public string Modul { get; set; }

        public string Form_Id { get; set; }

        public List<WorkflowMappingDetails> Details { get; set; }
    }

    public class WorkflowMappingDetails {
        public string State { get; set; }
    }
}