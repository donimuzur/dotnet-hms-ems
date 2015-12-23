using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5InputManualViewModel 
    {
        public string Plant { get; set; }

        public string MaterialNumber { get; set; }
        public string MaterialDesc { get; set; }

        public decimal Qty { get; set; }
        public string Uom { get; set; }
        public decimal Convertion { get; set; }
        public decimal ConvertedQty { get; set; }
        public string ConvertedUom { get; set; }
        public decimal? Hje { get; set; }
        public decimal? Tariff { get; set; }
        public decimal? ExciseValue { get; set; }
        public decimal? UsdValue { get; set; }
        public string Note { get; set; }
        public string Message { get; set; }

        public string StockRemaining { get; set; }
    }
}