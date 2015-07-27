using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1Dto
    {
        public long Lack1Id { get; set; }
        public string Lack1Number { get; set; }
        public string Bukrs { get; set; }
        public string Butxt { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYears { get; set; }
        public string LevelPlantId { get; set; }
        public string LevelPlantName { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string SupplierPlant { get; set; }
        public string ExGoodsType { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public decimal ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatus  GovStatus { get; set; }
        public DateTime? DecreeDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedBy  { get; set; }
        public DateTime ApprovedDate { get; set; }
        
        
    }
}
