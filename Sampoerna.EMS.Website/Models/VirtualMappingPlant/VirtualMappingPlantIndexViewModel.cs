using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.VirtualMappingPlant
{
    public class VirtualMappingPlantIndexViewModel : BaseModel
    {
        public VirtualMappingPlantIndexViewModel()
        {
            Details = new List<VirtualMappingPlantDetail>();
        }
        public List<VirtualMappingPlantDetail> Details { get; set; }
    }

    public class VirtualMappingPlantDetail
    {
        public long VirtualPlantMapId { get; set; }

        public long CompanyId { get; set; }
        public string CompanyName { get; set; }

        public long ImportPlantId { get; set; }
        public string ImportPlanName { get; set; }

        public long ExportPlantId { get; set; }
        public string ExportPlanName { get; set; }

    }
}