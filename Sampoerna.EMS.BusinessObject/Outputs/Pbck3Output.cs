using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class Pbck3Output
    {
        public Pbck3Output()
        {
            WorkflowHistoryPbck7 = new List<WorkflowHistoryDto>();
            WorkflowHistoryPbck3 = new List<WorkflowHistoryDto>();
            WorkflowHistoryCk5 = new List<WorkflowHistoryDto>();

            ListChangesHistorys = new List<CHANGES_HISTORY>();
        }
        public Pbck3CompositeDto Pbck3CompositeDto { get; set; }
        public List<WorkflowHistoryDto> WorkflowHistoryPbck7 { get; set; }
        public List<WorkflowHistoryDto> WorkflowHistoryPbck3 { get; set; }
        public List<WorkflowHistoryDto> WorkflowHistoryCk5 { get; set; }
        public List<CHANGES_HISTORY> ListChangesHistorys { get; set; }

    }
}
