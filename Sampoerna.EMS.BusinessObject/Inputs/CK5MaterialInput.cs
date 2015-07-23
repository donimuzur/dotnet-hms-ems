using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class CK5MaterialInput
    {
        public long Plant { get; set; }
        public string Brand { get; set; }
        public string Qty { get; set; }
        public string Uom { get; set; }
        public string Convertion { get; set; }
        public string ConvertedUom { get; set; }
        public string UsdValue { get; set; }
        public string Note { get; set; }
       
    }
}
