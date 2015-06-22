using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5PlantModel
    {
        public int PlantId { get; set; }
        public string PlantNpwp { get; set; }
        public long NPPBCK_ID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string KppBcName { get; set; }
    }
}