using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Math;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4CCreateWasteProductionViewModel : BaseModel
    {
        [Required]
        public DateTime? ReportedOn { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        [Required]
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        [Required]
        public string FinishGoods { get; set; }
        public string Description { get; set; }
        [Required]
        public string WasteQty { get; set; }
        [Required]
        public string Uom { get; set; }
        [Required]
        public string QtyPacked { get; set; }
        [Required]
        public string QtyUnpacked { get; set; }

        //selectList
        public SelectList CompanyList { get; set; }
        public SelectList PlantList { get; set; }
        public SelectList FinishGoodsList { get; set; }
        public SelectList UomList { get; set; }
    }
}