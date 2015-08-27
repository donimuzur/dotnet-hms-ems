using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Math;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CCreateWasteProductionViewModel : BaseModel
    {
        public DateTime? ReportedOn { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string FinishGoods { get; set; }
        public string Description { get; set; }
        public string WasteQty { get; set; }
        public string Uom { get; set; }
    }
}