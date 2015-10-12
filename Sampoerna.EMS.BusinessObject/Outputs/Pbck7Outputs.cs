﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class Pbck7DetailsOutput : BLLBaseOutput
    {
        public Pbck7DetailsOutput()
        {

            Pbck7Dto = new Pbck7AndPbck3Dto();
            Back1Dto = new Back1Dto();
            Pbck3Dto = new Pbck3Dto();
            Back3Dto = new Back3Dto();
            Ck2Dto = new Ck2Dto();

            WorkflowHistoryPbck7 = new List<WorkflowHistoryDto>();
            WorkflowHistoryPbck3 = new List<WorkflowHistoryDto>();

            //ListChangesHistorys = new List<CHANGES_HISTORY>();
            //ListWorkflowHistorys = new List<WorkflowHistoryDto>();
        }

        public Pbck7AndPbck3Dto Pbck7Dto { get; set; }
        public Back1Dto Back1Dto { get; set; }
        public Pbck3Dto Pbck3Dto { get; set; }
        public Back3Dto Back3Dto { get; set; }
        public Ck2Dto Ck2Dto { get; set; }

        public List<WorkflowHistoryDto> WorkflowHistoryPbck7 { get; set; }
        public List<WorkflowHistoryDto> WorkflowHistoryPbck3 { get; set; }

        //public List<CHANGES_HISTORY> ListChangesHistorys { get; set; }
        //public List<WorkflowHistoryDto> ListWorkflowHistorys { get; set; }
        //public List<Pbck4ItemDto> Pbck4ItemsDto { get; set; }
        //public List<PrintHistoryDto> ListPrintHistorys { get; set; }
    }
}
