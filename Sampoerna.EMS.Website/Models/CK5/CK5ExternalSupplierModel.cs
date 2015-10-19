using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5ExternalSupplierModel
    {

        public string SupplierNppbkcId { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierPlant { get; set; }
        public string SupplierCompany { get; set; }
        public string SupplierPortId { get; set; }
        public string SupplierPortName { get; set; }
        public string SupplierKppbcId { get; set; }
        public string SupplierKppbcName { get; set; }
        public string SupplierPhone { get; set; }

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