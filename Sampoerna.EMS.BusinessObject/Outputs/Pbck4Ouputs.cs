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

    public class GetListBrandByPlantOutput
    {
        public string PlantId { get; set; }
        public string FaCode { get; set; }
        public string StickerCode { get; set; }
        public decimal RemainingBlockQuota { get; set; }
    }

    public class GetListCk1ByNppbkcOutput
    {
        public string Ck1Id { get; set; }
        public string Ck1No { get; set; }
        public string Ck1Date { get; set; }
    }

    public class GetBrandItemsOutput
    {
        public string PlantId { get; set; }
        public string FaCode { get; set; }
        public string StickerCode { get; set; }
        public string SeriesCode { get; set; }
        public string BrandName { get; set; }
        public string ProductAlias { get; set; }
        public string BrandContent { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Colour { get; set; }

        public string BlockedStock { get; set; }
        public string BlockedStockUsed { get; set; }
        public string BlockedStockRemaining { get; set; }

        public List<GetListCk1ByNppbkcOutput> ListCk1Date { get; set; }
    }

    public class BlockedStockQuotaOutput
    {
        public string BlockedStock { get; set; }
        public string BlockedStockUsed { get; set; }
        public string BlockedStockRemaining { get; set; }
        public decimal BlockedStockRemainingCount { get; set; }

        public string PlantId { get; set; }
        public string FaCode { get; set; }
    }
}
