﻿using System;
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
        public DateTime? ProductionDate { get; set; }

        //selecList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerkList { get; set; }
        public SelectList FaCodeList { get; set; }
        public SelectList UomList { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }
        public List<ProductionDetail> Details { get; set; }
        public ProductionDto ProductionDtos { get; set; }

    }

    public class ProductionFormModel : BaseModel
    {

        public ProductionDetail Detail { get; set; }
    }

    public class ProductionDetail : BaseModel
    {
        public string CompanyCodeX { get; set; }
        public string PlantWerksX { get; set; }
        public string FaCodeX { get; set; }
        public DateTime? ProductionDateX { get; set; }

        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string PlantWerks { get; set; }
        [Required]
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }

        [Required]
        public string QtyUnpackedStr { get; set; }
        public decimal? QtyUnpacked
        {
            get;
            set;
        }
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
        public DateTime? ProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }

        //selecList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerkList { get; set; }
        public SelectList FacodeList { get; set; }
        public SelectList UomList { get; set; }
        public ProductionDto ProductionDtos { get; set; }
    }

}