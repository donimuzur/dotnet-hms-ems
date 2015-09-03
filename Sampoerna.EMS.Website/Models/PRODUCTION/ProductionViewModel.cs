using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string Uom { get; set; }

        //selecList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerkList { get; set; }
        public SelectList FaCodeList { get; set; }
        public SelectList UomList { get; set; }
        public Enums.CK4CType Ck4CType { get; set; }
        public List<ProductionDetail> Details { get; set; }
       
    }

    public class ProductionDetail
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantWerks { get; set; }
        public string PlantName { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public decimal? QtyUnpacked { get; set; }
        public decimal? QtyPacked { get; set; }
        public string Uom { get; set; }
        public DateTime ProductionDate { get; set; }

        //selecList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerkList { get; set; }
        public SelectList FacodeList { get; set; }
        public SelectList UomList { get; set; }
    }
}