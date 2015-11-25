using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5UploadViewModel
    {
        public string Plant { get; set; }
        public string Brand { get; set; }
        public string Qty { get; set; }
        public string Uom { get; set; }
        public string Convertion { get; set; }
        public string ConvertedQty { get; set; }
        public string ConvertedUom { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string ExciseValue { get; set; }
        public string UsdValue { get; set; }
        public string Note { get; set; }
        public string Message { get; set; }
        public string Total { get; set; }

        public string MaterialDesc { get; set; }
        
        public decimal ExciseQty { get; set; }
        public string ExciseUom { get; set; }

        public Enums.ExGoodsType ExGoodsType { get; set; }
        public long? CK5_MATERIAL_ID { get; set; }

        public string WasteStock { get; set; }
        public string EditUrlFunction { get; set; }
    }
}