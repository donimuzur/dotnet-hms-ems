using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Configuration
{
    public class ConfigurationCreateViewModel : BaseModel
    {
        
        public string ConfigType { get; set; }       

        [Required , Display(Name = "Configuration Type")]
        public string ConfigText { get; set; }

        [Display(Name = "Configuration Name")]
        public string ConfigName { get; set; }

        [Required, Display(Name = "Configuration Value")]
        public string ConfigValue { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }        

        public SelectList TypeList { get; set; }

        public ConfigurationIndexViewModel ConfigViewModel { get; set; }

        public ConfigurationViewModel ConfigModel { get; set; }

        public List<UserModel> UserList { get; set; }

        public string SelectedTypeValue { get; set; }
        
        public SelectList ApprovalList { get; set; }

        public SelectList HintList { get; set; }



    }
}