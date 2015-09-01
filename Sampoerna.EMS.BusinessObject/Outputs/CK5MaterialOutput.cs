using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class CK5MaterialOutput
    {
        public string Plant { get; set; }
        public string Brand { get; set; }
        public string Qty { get; set; }
        public string Uom { get; set; }
        public string Convertion { get; set; }
        public decimal ConvertedQty { get; set; }
        public string ConvertedUom { get; set; }
        public decimal Hje { get; set; }
        public decimal Tariff { get; set; }
        public decimal ExciseValue { get; set; }
        public string UsdValue { get; set; }
        public string Note { get; set; }
        public string Message { get; set; }
        public string Total { get; set; }
        public bool IsValid { get; set; }
    }
}
