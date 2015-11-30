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
        public string FA_CODE { get; set; }
        public string PLANT_ID { get; set; }
        public string STICKER_CODE { get; set; }
        public string SERIES_CODE { get; set; }
        public string BRAND_NAME { get; set; }
        public string PRODUCT_ALIAS { get; set; }
        public string BRAND_CONTENT { get; set; }
        public Nullable<decimal> HJE { get; set; }
        public Nullable<decimal> TARIFF { get; set; }
        public string COLOUR { get; set; }
        public Nullable<decimal> REQUESTED_QTY { get; set; }
        public string NO_PENGAWAS { get; set; }

        public string CK1_NUMBER { get; set; }
        public DateTime CK1_DATE { get; set; }

        public string BlockedStock { get; set; }
    }
}
