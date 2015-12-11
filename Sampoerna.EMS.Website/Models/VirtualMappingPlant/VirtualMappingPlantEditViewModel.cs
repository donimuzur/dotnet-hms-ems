using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.VirtualMappingPlant
{
    public class VirtualMappingPlantEditViewModel : BaseModel
    {
        public long VirtualMapId { get; set; }

        [Required]
        public string CompanyId { get; set; }

        public SelectList CompanyNameList { get; set; }

        [Required]
        public string ImportPlantId { get; set; }
        public SelectList ImportPlanNameList { get; set; }

        [Required]
        public string ExportPlantId { get; set; }
        public SelectList ExportPlanNameList { get; set; }

        public bool? IsDeleted { get; set; }

        public bool IsAllowDelete { get; set; }
    }
}