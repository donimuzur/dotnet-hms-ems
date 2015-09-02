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
        public DateTime? ReportedOn { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string FinishGoods { get; set; }
        public string Description { get; set; }
        public string WasteQty { get; set; }
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