using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.UOM
{
    public class UomDetailViewModel : BaseModel
    {
        [Required]
        public string UomId { get; set; }

        [Required]
        public string UomName { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsEms { get; set; }
    }
}