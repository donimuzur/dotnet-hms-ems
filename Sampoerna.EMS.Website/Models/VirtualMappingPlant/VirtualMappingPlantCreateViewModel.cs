using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.VirtualMappingPlant
{
    public class VirtualMappingPlantCreateViewModel : BaseModel
    {
        [Required]
        public long CompanyId { get; set; }

        public SelectList CompanyNameList { get; set; }

         [Required]
        public long ImportPlantId { get; set; }
        public SelectList ImportPlanNameList { get; set; }

         [Required]
        public long ExportPlantId { get; set; }
        public SelectList ExportPlanNameList { get; set; }
    }
}