﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class ProductionDto
    {
        public string CompanyCodeX { get; set; }
        public string PlantWerksX { get; set; }
        public string FaCodeX { get; set; }
        public string ProductionDateX { get; set; }
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public decimal? QtyUnpacked { get; set; }
        //Qty Packed
        public string QtyPackedStr
        {
            get;
            set;
        }
        public decimal? QtyPacked { get; set; }
        public string Uom { get; set; }
        public DateTime ProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }
        public decimal? QtyProduced { get; set; }
        public string TobaccoProductType { get; set; }
        public decimal? Hje { get; set; }
        public decimal? Tarif { get; set; }
        public string ProdCode { get; set; }
        public string Batch { get; set; }
        public string ProdQtyStickStr { get; set; }
        public decimal? ProdQtyStick
        {
            get;
            set;
        }
        //QTY
        public string QtyStr { get; set; }
        public decimal? Qty
        {
            get;
            set;
        }
        public int? Bundle { get; set; }
        public string Market { get; set; }
        public string Docgmvter { get; set; }
        public string MatDoc { get; set; }
        public string Ordr { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public List<ProductionUploadItems> UploadItems { get; set; }
        public int ContentPerPack { get; set; }
        public int PackedInPack { get; set; }

        public bool IsBrandFromSap { get; set; }
    }

    public class ProductionUploadItems
    {
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public string Qty { get; set; }
        public string QtyPacked { get; set; }
        public string Uom { get; set; }
        public string ProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string MesssageUploadFileDocuments { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }

    }

}
