using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class Pbck4DetailsOutput : BLLBaseOutput
    {
        public Pbck4DetailsOutput()
        {
            ListChangesHistorys = new List<CHANGES_HISTORY>();
            ListWorkflowHistorys = new List<WorkflowHistoryDto>();
        }

        public Pbck4Dto Pbck4Dto { get; set; }
        public List<CHANGES_HISTORY> ListChangesHistorys { get; set; }
        public List<WorkflowHistoryDto> ListWorkflowHistorys { get; set; }
        public List<Pbck4ItemDto> Pbck4ItemsDto { get; set; }

        public List<PrintHistoryDto> ListPrintHistorys { get; set; }
    }

}
