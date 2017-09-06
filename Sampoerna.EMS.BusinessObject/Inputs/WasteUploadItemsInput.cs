﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WasteUploadItemsInput
    {
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public string MarkerRejectStickQty { get; set; }
        public string PackerRejectStickQty { get; set; }
        public string WasteProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }
        public string DustWasteGramQty { get; set; }
        public string FloorWasteGramQty { get; set; }
        public string DustWasteStickQty { get; set; }
        public string FloorWasteStickQty { get; set; }
        public string StampWasteQty { get; set; }
        public string UseForLack10 { get; set; }
    }
}
