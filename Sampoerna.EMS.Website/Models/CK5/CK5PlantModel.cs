using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5PlantModel
    {
        public string PlantId { get; set; }
        public string PlantNpwp { get; set; }
        public string NPPBCK_ID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string KppBcName { get; set; }
        public string PlantName { get; set; }
        public string CompanyCode { get; set; }
        public string KppbcCity { get; set; }
        public string KppbcNo { get; set; }

        public int? Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public string Pbck1QtyApproved { get; set; }
        public string Ck5TotalExciseable { get; set; }
        public string RemainQuota { get; set; }
        public string Pbck1DecreeDate { get; set; }

        public List<Ck5ListPbck1Completed> PbckList { get; set; } 

    }

    public class Ck5ListPbck1Completed
    {
        public int PbckId { get; set; }
        public string PbckNumber { get; set; }
        
    }

    public class QuotaPbck1Model
    {
        public string Pbck1QtyApproved { get; set; }
        public string Ck5TotalExciseable { get; set; }
        public string RemainQuota { get; set; }
        public string Pbck1DecreeDate { get; set; }
    }
}