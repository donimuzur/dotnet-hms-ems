using System.Collections.Generic;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1ItemViewModel : BaseModel
    {
        public Lack1ItemViewModel()
        {
            WorkflowHistory = new List<WorkflowHistoryViewModel>();
        }
        public Lack1ItemModel Detail { get; set; }
        public string ControllerAction { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        public bool AllowPrintDocument { get; set; }
        public string Comment { get; set; }
        public Enums.LACK1Type Lack1Type { get; set; }
        public string MenuPlantAddClassCss { get; set; }
        public string MenuNppbkcAddClassCss { get; set; }
        public string MenuCompletedAddClassCss { get; set; }
        public List<WorkflowHistoryViewModel> WorkflowHistory { get; set; }
    }
}