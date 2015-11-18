using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    //public class CK5SaveOutput : BLLBaseOutput
    //{
    //    public long Id { get; set; }
    //}

    public class CK5DetailsOutput : BLLBaseOutput
    {
        public CK5DetailsOutput()
        {
            ListChangesHistorys = new List<CHANGES_HISTORY>();
            ListWorkflowHistorys = new List<WorkflowHistoryDto>();
        }

        public CK5Dto Ck5Dto { get; set; }
        public List<CHANGES_HISTORY> ListChangesHistorys { get; set; }
        public List<WorkflowHistoryDto> ListWorkflowHistorys { get; set; }
        public List<CK5MaterialDto> Ck5MaterialDto { get; set; }

        public List<PrintHistoryDto> ListPrintHistorys { get; set; }
    }

    public class GetQuotaAndRemainOutput
    {
        public decimal QtyApprovedPbck1 { get; set; }
        public decimal QtyCk5 { get; set; }
        public decimal RemainQuota { get; set; }
        public string Pbck1DecreeDate { get; set; }
        public string Pbck1Number { get; set; }
        public int? Pbck1Id { get; set; }
        public string PbckUom { get; set; }
    }

    public class GetListMaterialMarketReturnOutput
    {
        public string MaterialNumber { get; set; }
    }

    public class GetBrandByPlantAndMaterialNumberOutput
    {
        public decimal Hje { get; set; }
        public decimal Tariff { get; set; }
        public string Uom { get; set; }
        public string MaterialDesc { get; set; }
    }
}
