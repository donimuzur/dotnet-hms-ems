using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PRODUCTION
{
    public class ProductionViewModel : BaseModel
    {

        public ProductionViewModel()
        {
            Details = new List<ProductionDetail>();
        }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantWerks { get; set; }
        public string PlantName { get; set; }
        public string FaCode { get; set; }
        public string Uom { get; set; }
        public string ProductionDate { get; set; }

        //selecList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerkList { get; set; }
        public SelectList FaCodeList { get; set; }
        public SelectList UomList { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }
        public List<ProductionDetail> Details { get; set; }
        public ProductionDto ProductionDtos { get; set; }

    }

    public class ProductionDetail : BaseModel
    {
        public string CompanyCodeX { get; set; }
        public string PlantWerksX { get; set; }
        public string FaCodeX { get; set; }
        public string ProductionDateX { get; set; }

        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string PlantWerks { get; set; }
        [Required]
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }

        [Required]
        public string QtyPackedStr { get; set; }

        public decimal? QtyPacked
        {
            get;
            set;
        }
        [Required]
        public string Uom { get; set; }
        [Required]
        public string ProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }
        
        public string ProdQtyStickStr { get; set; }
        public decimal? ProdQtyStick
        {
            get;
            set;
        }
        [Required]
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
        public string Batch { get; set; }

        //selecList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerkList { get; set; }
        public SelectList FacodeList { get; set; }
        public SelectList UomList { get; set; }

        //domain model
        public ProductionDto ProductionDtos { get; set; }
        public List<ProductionUploadItems> UploadItems { get; set; }
    }

}