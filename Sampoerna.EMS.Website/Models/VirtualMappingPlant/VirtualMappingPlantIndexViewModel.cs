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

        public string CompanyId { get; set; }
        public string CompanyName { get; set; }

        public string ImportPlantId { get; set; }
        public string ImportPlanName { get; set; }

        public string ExportPlantId { get; set; }
        public string ExportPlanName { get; set; }

        public bool IsDeleted { get; set; }

        public string IsDeletedString { 
            get {
                return IsDeleted ? "Yes" : "No";
            } 
        
        }

    }
}