using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5ExternalSupplierModel
    {

        public string SupplierName { get; set; }
        public string SuppierNpwp { get; set; }
        public string NPPBCK_ID { get; set; }
        //public string CompanyName { get; set; }
        public string SupplierAddress { get; set; }
        //public string KppBcName { get; set; }
        public string PlantName { get; set; }
        //public string CompanyCode { get; set; }
        public string KppbcCity { get; set; }
        public string KppbcNo { get; set; }

        public int? Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public string Pbck1QtyApproved { get; set; }
        public string Ck5TotalExciseable { get; set; }
        public string RemainQuota { get; set; }
        public string Pbck1DecreeDate { get; set; }

        public string PbckUom { get; set; }

        public List<Ck5ListPbck1Completed> PbckList { get; set; }

        public SelectList CorrespondingPlantList { get; set; }
    }
}