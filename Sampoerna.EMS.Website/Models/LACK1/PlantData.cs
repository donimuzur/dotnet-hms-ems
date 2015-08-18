using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class PlantData
    {
        public long Lack1Id { get; set; }
        public string Company { get; set; }
        public string PlantId { get; set; }
        public string TobaccoGoodType { get; set; }
        public string Supplier { get; set; }
        public string Period { get; set; }
        public string Status { get; set; }
    }
}