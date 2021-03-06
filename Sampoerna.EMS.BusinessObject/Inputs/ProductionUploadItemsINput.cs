﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class ProductionUploadItemsInput
    {
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string StickerCode { get; set; }
        public string BrandDescription { get; set; }
        public string Qty { get; set; }
        public string QtyPacked { get; set; }
        public string Uom { get; set; }
        public string ProductionDate { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public decimal? Zb { get; set; }
        public decimal? PackedAdjusted { get; set; }
        public string Remark { get; set; }
    }
}
