using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class Pbck4ItemDto
    {
        public long PBCK4_ITEM_ID { get; set; }
        public int PBCK4_ID { get; set; }
        public Nullable<long> CK1_ID { get; set; }
        public Nullable<decimal> TOTAL_HJE { get; set; }
        public Nullable<decimal> TOTAL_STAMPS { get; set; }
        public Nullable<decimal> APPROVED_QTY { get; set; }
        public string REMARKS { get; set; }
    }
}
