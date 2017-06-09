using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Configuration
{
    public class ConfigurationViewModel : BaseModel
    {
        public long REFF_ID { get; set; }

        [Required, Display(Name = "Configuration Name")]
        public string REFF_NAME { get; set; }
        public string REFF_KEYS { get; set; }
        [Required, Display(Name = "Configuration Type")]
        public string REFF_TYPE { get; set; }
        [Required, Display(Name = "Configuration Value")]
        public string REFF_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string LASTMODIFIED_BY { get; set; }
        public DateTime LASTMODIFIED_DATE { get; set; }
        [Display(Name = "Status")]
        public bool IS_ACTIVE { get; set; }

        public SelectList TypeList { get; set; }
    }
}