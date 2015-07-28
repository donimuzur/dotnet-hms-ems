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
        [Required(ErrorMessage = "please fill this field")]
        public string CompanyId { get; set; }

        public SelectList CompanyNameList { get; set; }

         [Required(ErrorMessage = "please select one of the data")]
        public string ImportPlantId { get; set; }
        public SelectList ImportPlanNameList { get; set; }

         [Required(ErrorMessage = "please select one of the data")]
        public string ExportPlantId { get; set; }
        public SelectList ExportPlanNameList { get; set; }

        public bool IsDeleted { get; set; }
    }
}