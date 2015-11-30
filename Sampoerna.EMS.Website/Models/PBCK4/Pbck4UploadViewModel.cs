using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.PBCK4
{
    public class Pbck4UploadViewModel
    {
        public string FaCode { get; set; }
        public string StickerCode { get; set; }
        public string Ck1No { get; set; }
        public string Ck1Date { get; set; }
        public string SeriesCode { get; set; }
        public string BrandName { get; set; }
        public string ProductAlias { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Colour { get; set; }
        public string ReqQty { get; set; }
        public string TotalHje { get; set; }
        public string TotalStamps { get; set; }
        public string NoPengawas { get; set; }
        public string ApprovedQty { get; set; }
        public string Remark { get; set; }

        public string Message { get; set; }
        public string Plant { get; set; }
        public Nullable<long> CK1_ID { get; set; }
        public string BlockedStock { get; set; }
        public string BlockedStockUsed { get; set; }
        public string BlockedStockRemaining { get; set; }

        public long PBCK4_ITEM_ID { get; set; }
        public bool IsUpdated { get; set; }
       
    }
}