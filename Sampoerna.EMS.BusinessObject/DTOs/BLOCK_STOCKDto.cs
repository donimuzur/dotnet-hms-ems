using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class BLOCK_STOCKDto
    {
        public long BLOCK_STOCK_ID { get; set; }
        public string MATERIAL_ID { get; set; }
        public string PLANT { get; set; }
        public Nullable<decimal> BLOCKED { get; set; }
        public string BUN { get; set; }
        public string SLOC { get; set; }
    }
}
