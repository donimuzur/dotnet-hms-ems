﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class WasteDto
    {
        public string CompanyCodeX { get; set; }
        public string PlantWerksX { get; set; }
        public string FaCodeX { get; set; }
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public decimal? MarkerRejectStickQty { get; set; }
        public string MarkerStr
        {
            get;
            set;
        }
        public decimal? PackerRejectStickQty { get; set; }
        public string PackerStr
        {
            get;
            set;
        }
        public DateTime WasteProductionDate { get; set; }
        public DateTime WasteProductionDateX { get; set; }

        public string WasteProductionDateText {
            get
            {
                return WasteProductionDate.ToString("dd-MMM-yyyy");
            }
        }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }
        public decimal? DustWasteGramQty { get; set; }
        public string DustGramStr
        {
            get;
            set;
        }
        public decimal? FloorWasteGramQty { get; set; }
        public string FloorGramStr
        {
            get;
            set;
        }
        public decimal? DustWasteStickQty { get; set; }
        public string DustStickStr
        {
            get;
            set;
        }
        public decimal? FloorWasteStickQty { get; set; }
        public string FloorStickStr
        {
            get;
            set;
        }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public decimal? StampWasteQty { get; set; }
        public string StampWasteQtyStr
        {
            get;
            set;
        }
        public bool UseForLack10 { get; set; }
        public List<WasteUploadItems> UploadItems { get; set; }
    }

    public class WasteUploadItems
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
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public string StampWasteQty { get; set; }
        public string UseForLack10 { get; set; }
    }
}
