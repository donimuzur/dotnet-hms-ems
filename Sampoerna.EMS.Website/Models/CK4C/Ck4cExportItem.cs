using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4cExportItem : BaseModel
    {
        public DateTime DateProduction { get; set; }
        public string ProductionDate { get; set; }
        public string Plant { get; set; }
        public string ProdCode { get; set; }
        public string TobbacoProdType { get; set; }
        public string FaCode { get; set; }
        public string BrandDesc { get; set; }
        public string ProdQty { get; set; }
        public string ProdQtyUom { get; set; }
        public string PackedQty { get; set; }
        public string Zb { get; set; }
        public string PackedAdjusted { get; set; }
        public string UnpackedQty { get; set; }
        public string Remarks { get; set; }
        public string Content { get; set; }
        public string TotalPack { get; set; }
        public string TotalPackZb { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
    }
}