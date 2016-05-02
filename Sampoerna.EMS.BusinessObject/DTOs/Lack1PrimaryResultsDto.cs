using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1PrimaryResultsDto
    {
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }

        public string CfProducedProcessOrder { get; set; }
        public string CfCodeProduced { get; set; }
        public string CfProducedDescription { get; set; }
        public DateTime? CfProdDate { get; set; }
        public decimal? CfProdQty { get; set; }
        public string CfProdUom { get; set; }

        public string BkcUsed { get; set; }
        public string BkcDescription { get; set; }
        public decimal? BkcIssueQty { get; set; }
        public string BkcIssueUom { get; set; }
        public string BkcOrdr { get; set; }

        public string Message { get; set; }
    }
}
