using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Math;

namespace Sampoerna.EMS.Website.Models.CK4C
{
    public class Ck4cCreateViewModel : BaseModel
    {
        public string Ck4CNo { get; set; }
        public DateTime? ReportedOn { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string FinishGoods { get; set; }
        public string Description { get; set; }
        public string QtyPacked { get; set; }
        public string QtyUnpacked { get; set; }
        public string Uom { get; set; }
        public string CreatedBy { get; set; }

        //selectList
        public SelectList CompanyList { get; set; }
        public SelectList PlantList { get; set; }
        public SelectList FinishGoodList { get; set; }
        public SelectList UomList { get; set; }


    }   
}