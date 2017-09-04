using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Configuration
{
    public class ConfigurationIndexViewModel : BaseModel
    {       
        public ConfigurationIndexViewModel()
        {
            Detail = new List<ConfigurationViewModel>();
        }

        public string ConfigText { get; set; }
        public string ConfigType { get; set; }
        public string ConfigName { get; set; }
        public List<ConfigurationViewModel> Detail { get; set; }
        public ConfigurationViewModel DetailConfig { get; set; }
        public SelectList TypeList { get; set; }
    }    
}